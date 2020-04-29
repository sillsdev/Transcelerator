using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SIL.Scripture;
using SIL.Transcelerator;
using static System.Int32;
using static System.String;
using ScrVers = SIL.Scripture.ScrVers;

namespace DataIntegrityTests
{
	public class Tests
	{
		Regex m_regexQuestion = new Regex("<Questions", RegexOptions.Compiled);
		Regex m_regexSummary = new Regex(" summary=\"true\"", RegexOptions.Compiled);

			private class MatchedXmlLine
		{
			public MatchedXmlLine(int level, string line, int lineNumber, int matchEndPosition)
			{
				Level = level;
				Line = line;
				LineNumber = lineNumber;
				MatchEndPosition = matchEndPosition;
			}

			internal int Level { get; }
			internal string Line { get; }
			internal int LineNumber { get; }
			internal int MatchEndPosition { get; }
		}

		[Test]
		public void DataIntegrity_Headings_HaveConsistentReferences()
		{
			var englishVersification = ScrVers.English;
			var regexHeading = new Regex("<Section", RegexOptions.Compiled);
			// Although these references are regarded as human-readable, the current state of the data is that in some cases,
			// instead of book names, there is just an abbreviation, either in mixed case or all caps. Probably not ideal, but
			// not really a problem.
			var regexHeadingHumanReadableRef = new Regex(" heading=\"(?<bookName>([1-3] ?)?(([A-Z][A-Za-z]+))( [A-Za-z]+){0,2}) (?<chapterAndVerse>((?<chapter>[1-9][0-9]*):)?(?<startVerse>[1-9][0-9]*)(?<startVerseSegment>[b])?((?<connector>-|(, ?))(((?<endChapter>[1-9][0-9]*)?):)?(?<endVerse>[1-9][0-9]*)(?<endVerseSegment>a)?)?)", RegexOptions.Compiled);
			var regexWellFormedScrRef = new Regex(" scrref=\"(?<bookId>([1-3][A-Z]{2})|([A-Z]{3})) (?<chapterAndVerseInScrRef>[^\"]*[0-9a-b])\"", RegexOptions.Compiled);
			var regexScrRefAttrib = new Regex(" scrref=", RegexOptions.Compiled);
			var bbbcccvvvRef = "((?<zero>0)|(?<bookNum>0?[0-9]{1,2})(?<chapterNum>[0-9]{3})(?<verseNum>[0-9]{3}))";
			var regexStartRef = new Regex($" startref=\"{bbbcccvvvRef}\"", RegexOptions.Compiled);
			var regexEndRef = new Regex($" endref=\"{bbbcccvvvRef}\"", RegexOptions.Compiled);

			var multilingScrBooks = new MultilingScrBooks();

			string bookId = null;
			var bookNum = -1;
			var chapter = -1;
			var startVerse = -1;
			var endVerse = -1;
			var endChapter = -1;
			var prevBookNum = -1;
			var prevSectionEndedWithSegment = false;
			var prevSectionEndCCCVVV = -1;
			var prevQuestionStartCCCVVV = 0;
			var prevQuestionEndCCCVVV = 0;
			var inSingleChapterBook = false;

			foreach (var matchedLine in GetMatchingLines(regexHeading, m_regexQuestion))
			{
				var line = matchedLine.Line;
				var startPos = matchedLine.MatchEndPosition;
				switch (matchedLine.Level)
				{
					case 0:
						var matchHeadingHumanReadableRef = regexHeadingHumanReadableRef.Match(line, startPos);
						Assert.That(matchHeadingHumanReadableRef.Success,
							"Section head does not contain a valid human-readable reference: " + line);

						var chapterAndVerse = matchHeadingHumanReadableRef.Groups["chapterAndVerse"].Value.Replace(":", ".");
						var bookName = matchHeadingHumanReadableRef.Groups["bookName"].Value;
						var chapterStr = matchHeadingHumanReadableRef.Result("${chapter}");
						chapter = IsNullOrEmpty(chapterStr) ? 1 : Parse(chapterStr);
						startVerse = Parse(matchHeadingHumanReadableRef.Groups["startVerse"].Value);
						var connector = matchHeadingHumanReadableRef.Result("${connector}");
						var endVerseStr = matchHeadingHumanReadableRef.Result("${endVerse}");
						endVerse = IsNullOrEmpty(endVerseStr) ? startVerse : Parse(endVerseStr);
						var endChapterStr = matchHeadingHumanReadableRef.Result("${endChapter}");
						endChapter = IsNullOrEmpty(endChapterStr) ? chapter : Parse(endChapterStr);

						if (connector.StartsWith(","))
						{
							Assert.AreEqual(chapter, endChapter,
								"If human-readable section reference uses a comma, there must not be an end chapter: " + line);
							Assert.AreEqual(startVerse + 1, endVerse,
								"If human-readable section reference uses a comma, end verse must be the verse following the start verse: " + line);
							chapterAndVerse = chapterAndVerse.Replace(connector, "-");
						}

						var matchScrRef = regexWellFormedScrRef.Match(line, startPos);
						Assert.That(matchScrRef.Success, "Section head does not contain a valid scrref attribute: " + line);

						var chapterAndVerseInScrRef = matchScrRef.Groups["chapterAndVerseInScrRef"].Value;
						if (chapterAndVerse != chapterAndVerseInScrRef)
						{
							// Sub-verse letters sometimes occur in the section reference but not in the scrref attribute. These
							// should probably be consistent, but unfortunately are not. I started to change them, but since this
							// could At this point
							Assert.AreEqual(chapterAndVerse.Replace("a", "").Replace("b", ""), chapterAndVerseInScrRef,
								"Chapter and verse in scrref attribute do not match human readable chapter and verse: " + line);
						}

						bookId = matchScrRef.Groups["bookId"].Value;
						bookNum = BCVRef.BookToNumber(bookId);
						if (bookNum != prevBookNum)
						{
							Assert.That(bookNum >= 1 && bookNum <= 66,
								$"Book ID ({bookId}) does not correspond to a valid book: " + line);
							inSingleChapterBook = englishVersification.GetLastChapter(bookNum) == 1;
						}

						if (!bookId.Equals(bookName, StringComparison.OrdinalIgnoreCase))
						{
							var expectedBookName = multilingScrBooks.GetBookName(bookNum);
							Assert.That(expectedBookName.StartsWith(bookName),
								"Book code in scrref attribute does not correspond to book Name in section heading: " + line);
						}

						var matchStartRef = regexStartRef.Match(line, startPos);
						Assert.That(matchStartRef.Success, "Section head does not contain a valid startref attribute: " + line);
						Assert.That(matchStartRef.Groups["zero"].Value == Empty,
							"startref=\"0\" not allowed in section head: " + line);

						Assert.AreEqual(bookNum, Parse(matchStartRef.Groups["bookNum"].Value),
							"Book in startref attribute does not match section book: " + line);
						Assert.AreEqual(chapter, Parse(matchStartRef.Groups["chapterNum"].Value),
							"Chapter in startref attribute does not match section start chapter: " + line);
						Assert.AreEqual(startVerse, Parse(matchStartRef.Groups["verseNum"].Value),
							"Verse in startref attribute does not match section start verse: " + line);

						var matchEndRef = regexEndRef.Match(line, startPos);
						Assert.That(matchEndRef.Success, "Section head does not contain a valid endref attribute: " + line);
						Assert.That(matchEndRef.Groups["zero"].Value == Empty,
							"endref=\"0\" not allowed in section head: " + line);

						Assert.AreEqual(bookNum, Parse(matchEndRef.Groups["bookNum"].Value),
							"Book in endref attribute does not match section book: " + line);
						Assert.AreEqual(endChapter, Parse(matchEndRef.Groups["chapterNum"].Value),
							"Chapter in endref attribute does not match section end chapter: " + line);
						Assert.AreEqual(endVerse, Parse(matchEndRef.Groups["verseNum"].Value),
							"Verse in endref attribute does not match section end verse: " + line);

						if (bookNum == prevBookNum)
						{
							var currSectionStartsWithSegment = matchHeadingHumanReadableRef.Groups["startVerseSegment"].Value != Empty;
							var currSectionEndsWithSegment = matchHeadingHumanReadableRef.Groups["endVerseSegment"].Value != Empty;
							var currSectionEndCCCVVV = endChapter * 1000 + endVerse;
							var currSectionStartCCCVVV = chapter * 1000 + startVerse;

							// Ensure references are in ascending order
							if (prevSectionEndedWithSegment)
							{
								Assert.That(currSectionStartsWithSegment && prevSectionEndCCCVVV == currSectionStartCCCVVV,
									"Section out of order: " + line);
							}
							else
							{
								Assert.That(prevSectionEndCCCVVV < currSectionStartCCCVVV,
									"Section out of order: " + line);
							}

							prevSectionEndCCCVVV = currSectionEndCCCVVV;
							prevSectionEndedWithSegment = currSectionEndsWithSegment;
						}
						else
						{
							prevBookNum = bookNum;
							prevSectionEndCCCVVV = -1;
							Assert.False(prevSectionEndedWithSegment);
						}
						prevQuestionStartCCCVVV = 0;
						break;
					case 1:
						matchScrRef = regexWellFormedScrRef.Match(line, startPos);
						if (matchScrRef.Success)
						{
							Assert.AreEqual(bookId, matchScrRef.Groups["bookId"].Value,
								"Question scrref attribute has incorrect book: " + line);

							Assert.That(inSingleChapterBook || matchScrRef.Groups["chapterAndVerseInScrRef"].Value.Contains("."),
								"Question scrref is missing the chapter or verse: "+ line);
						}
						else
						{
							Assert.IsFalse(regexScrRefAttrib.IsMatch(line, startPos),
								"Question contains ill-formed scrref attribute" + line);
						}

						matchStartRef = regexStartRef.Match(line, startPos);
						Assert.That(matchStartRef.Success, "Question does not contain a valid startref attribute: " + line);

						var questionStartCccVvv = 0;

						var startRefSpecifiedExplicitly = matchStartRef.Groups["zero"].Value == Empty;
						if (startRefSpecifiedExplicitly)
						{
							Assert.AreEqual(bookNum, Parse(matchStartRef.Groups["bookNum"].Value));
							var questionStartChapter = Parse(matchStartRef.Groups["chapterNum"].Value);
							var questionStartVerse = Parse(matchStartRef.Groups["verseNum"].Value);
							questionStartCccVvv = questionStartChapter * 1000 + questionStartVerse;
							Assert.That(chapter * 1000 + startVerse <= questionStartCccVvv,
								"Question starts outside of containing section: " + line);
						}
						Assert.That(m_regexSummary.IsMatch(line, startPos) || questionStartCccVvv >= prevQuestionStartCCCVVV,
							$"Error at line {matchedLine.LineNumber}. Question out of order: " + line);
						prevQuestionStartCCCVVV = questionStartCccVvv;

						matchEndRef = regexEndRef.Match(line, startPos);
						Assert.That(matchEndRef.Success, "Question does not contain a valid endref attribute: " + line);
						if (startRefSpecifiedExplicitly)
						{
							Assert.That(matchEndRef.Groups["zero"].Value == Empty,
								"Start and end ref must both be explicit or both be implicit: " + line);

							Assert.AreEqual(bookNum, Parse(matchEndRef.Groups["bookNum"].Value));
							var questionEndChapter = Parse(matchEndRef.Groups["chapterNum"].Value);
							var questionEndVerse = Parse(matchEndRef.Groups["verseNum"].Value);
							
							var questionEndCccVvv = questionEndChapter * 1000 + questionEndVerse;
							Assert.That(endChapter * 1000 + endVerse >= questionEndCccVvv,
								"Question ends outside of containing section: " + line);
							
							Assert.That(questionStartCccVvv <= questionEndCccVvv,
								"Question start reference is later than the end reference: " + line);
						}
						else
						{
							Assert.That(matchEndRef.Groups["zero"].Value != Empty,
								"Start and end ref must both be explicit or both be implicit: " + line);
						}

						break;
				}
			}
		}

		[Test]
		public void DataIntegrity_Categories_OverviewTrueOthersFalse()
		{
			Regex regexCategory = new Regex("<Category", RegexOptions.Compiled);
			Regex regexAttribs = new Regex(" overview=\"(?<isOverview>(true)|(false))\" ?(type=\"(?<category>[A-Z][^\"]*)\")?>", RegexOptions.Compiled);

			var regexZeroRef = new Regex("( startref=\"0\")|( endref=\"0\")", RegexOptions.Compiled);

			var inOverviewSection = false;

			foreach (var matchedLine in GetMatchingLines(regexCategory, m_regexQuestion))
			{
				var line = matchedLine.Line;
				var startPos = matchedLine.MatchEndPosition;

				switch (matchedLine.Level)
				{
					case 0:
						var matchAttribs = regexAttribs.Match(line, startPos);
						Assert.That(matchAttribs.Success, "Category does not contain expected attributes: " + line);
						Assert.AreEqual(startPos, matchAttribs.Index, "Category has unexpected attributes: " + line);

						var isOverview = matchAttribs.Groups["isOverview"].Value;
						var category = matchAttribs.Result("${category}");

						switch (isOverview)
						{
							case "true":
								Assert.AreEqual("Overview", category,
									"The \"Overview\" category should have overview=\"true\"" + line);
								inOverviewSection = true;
								break;
							case "false":
								Assert.AreNotEqual("Overview", category,
									"Only the \"Overview\" category should have overview=\"true\"" + line);
								inOverviewSection = false;
								break;
							default:
								Assert.Fail("Unexpected boolean value in overview attribute " + line);
								break;
						}
						break;
					case 1:
						if (!m_regexSummary.IsMatch(line, startPos))
						{
							if (inOverviewSection)
							{
								var firstMatch = regexZeroRef.Match(line, startPos);
								Assert.True(firstMatch.Success && regexZeroRef.IsMatch(line, firstMatch.Index + firstMatch.Length),
									$"Error at line {matchedLine.LineNumber}. Overview questions should be marked as \"summary\" questions unless they have 0 startref and endref attributes: " + line);
							}
							else
							{
								Assert.False(regexZeroRef.IsMatch(line, startPos),
									$"Error at line {matchedLine.LineNumber}. Only summary and overview questions should have 0 startref or endref attributes: " + line);
							}
						}

						break;
				}
				
			}
		}

		private IEnumerable<MatchedXmlLine> GetMatchingLines(Regex regexLevel0, Regex regexLevel1 = null)
		{
			using (var reader = new StreamReader(new FileStream(TxlCore.kQuestionsFilename, FileMode.Open)))
			{
				Assert.IsNotNull(reader);
				var lineNumber = 1;
				var line = reader.ReadLine();
				while (line != null)
				{
					var match = regexLevel0.Match(line);
					if (match.Success)
						yield return new MatchedXmlLine(0, line, lineNumber, match.Index + match.Length);
					else if (regexLevel1 != null)
					{
						match = regexLevel1.Match(line);
						if (match.Success)
							yield return new MatchedXmlLine(1, line, lineNumber, match.Index + match.Length);
					}
					line = reader.ReadLine();
					lineNumber++;
				}
			}
		}
	}
}