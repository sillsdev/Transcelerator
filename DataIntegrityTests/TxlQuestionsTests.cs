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
		Regex m_regexSummary = new Regex(" ordered=\"true\"", RegexOptions.Compiled);

			private class MatchedXmlLine
		{
			public MatchedXmlLine(int level, string line, int lineNumber, Match match)
			{
				Level = level;
				Line = line;
				LineNumber = lineNumber;
				MatchEndPosition = match.Index + match.Length;
				Match = match;
			}

			internal int Level { get; }
			internal string Line { get; }
			internal int LineNumber { get; }
			internal int MatchEndPosition { get; }
			internal Match Match { get; }
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
									$"Error at line {matchedLine.LineNumber}. Overview questions with specified references should be marked as \"ordered\" questions: " + line);
							}
							else
							{
								Assert.False(regexZeroRef.IsMatch(line, startPos),
									$"Error at line {matchedLine.LineNumber}. Only ordered and overview questions should have 0 startref or endref attributes: " + line);
							}
						}

						break;
				}
				
			}
		}

		[Test]
		public void DataIntegrity_Groups_HaveConsistentReferencesAndLetters()
		{
			const string kVerseOrBridge = "(?<groupVerses>\\d+(-\\d+)?)";
			var regexGroupedQuestion = new Regex("<Questions [^>]*\\bscrref=\"(?<book>[1-3]?[A-Z]{2,3}) " +
				"((?<chapter>\\d{1,3})\\.)?(?<verses>[^\"]+)\"[^>]*\\bgroup=\"", RegexOptions.Compiled);
			var regexGroupRelatedNote = new Regex("<Note>((?<idType>(group)|(question)) (?<group>[A-Z]) \\((?<book>\\w+) ((?<chapterInGrpId>\\d{1,3}):)?(?<verses>[^\\)]+)\\))|" +
				$"(?<useEitherGroup>For \\w+ ((?<chapter>\\d+):)?{kVerseOrBridge}, use either the group (?<firstGroup>[ACEGIKMOQSUWY]) questions or the group (?<secondGroup>[BDFHJLNPRTVXZ]) questions. It would be redundant to ask all (?<count>\\d+) questions.)|" +
				"(?<useEitherQuestion>Use either this question \\((?<thisGroup>[A-Z])\\) or the ((?<following>following)|(preceding)) question \\((?<otherGroup>[A-Z])\\). It would be redundant to ask both questions.)<\\/Note>", RegexOptions.Compiled);

			var regexGroupAttrib = new Regex($"{kVerseOrBridge}(?<group>[A-Z])\"", RegexOptions.Compiled);

			string bookId = null;
			string chapter = null;
			string verses = null;
			char group = (char)0;
			int questionLineNumber = -1;
			var groupInstructionsMissing = false;
			var groupOrQuestionIdNoteMissing = false;
			var expectedTotalQuestionsInCurrentPairOfGroups = -1;
			var countOfQuestionsInCurrentPairOfGroups = 0;

			foreach (var matchedLine in GetMatchingLines(regexGroupedQuestion, regexGroupRelatedNote))
			{
				var line = matchedLine.Line;
				var startPos = matchedLine.MatchEndPosition;
				switch (matchedLine.Level)
				{
					case 0:
						Assert.IsFalse(groupInstructionsMissing,
							$"Did not find note with instructions for group {verses}{group} in {bookId} {chapter} at line {questionLineNumber}.");
						Assert.IsFalse(groupOrQuestionIdNoteMissing,
							$"Did not find note with the group information for {verses}{group} in {bookId} {chapter} at line {questionLineNumber}.");

						questionLineNumber = matchedLine.LineNumber;
						var newBookId = matchedLine.Match.Groups["book"].Value;
						if (bookId != newBookId)
						{
							groupInstructionsMissing = true;
							bookId = newBookId;
							group = (char)0;
						}

						var newChapter = matchedLine.Match.Groups["chapter"].Value;
						if (chapter != newChapter)
						{
							groupInstructionsMissing = true;
							chapter = newChapter;
							group = (char)0;
						}

						var questionVerses = matchedLine.Match.Groups["verses"].Value;

						var matchGroupAttrib = regexGroupAttrib.Match(matchedLine.Line, startPos);

						Assert.IsTrue(matchGroupAttrib.Success && matchGroupAttrib.Index == startPos,
							"The group attribute failed to match the expected format: " + line);

						var newVerses = matchGroupAttrib.Groups["groupVerses"].Value;
						if (verses != newVerses)
						{
							groupInstructionsMissing = true;
							verses = newVerses;
							group = (char)0;
						}

						if (verses != questionVerses)
						{
							var questionVerseParts = verses.Split('-');
							var questionStartVerse = Parse(questionVerseParts[0]);
							var groupVerseParts = verses.Split('-');
							var groupStartVerse = Parse(groupVerseParts[0]);
							Assert.IsTrue(questionStartVerse >= groupStartVerse,
								"Question has a group with a reference range that does not contain the verse(s) in the scrref attribute: " + line);
							var questionEndVerse = questionVerseParts.Length > 1 ? Parse(questionVerseParts[1]) : questionStartVerse;
							var groupEndVerse = groupVerseParts.Length > 1 ? Parse(groupVerseParts[1]) : groupStartVerse;
							Assert.IsTrue(questionEndVerse <= groupEndVerse,
								"Question has a group with a reference range that does not contain the verse(s) in the scrref attribute: " + line);
						}

						var newGroup = matchGroupAttrib.Groups["group"].Value[0];
						if (group != newGroup)
						{
							if (group != (char)0)
							{
								Assert.AreEqual(1, newGroup - group,
									"Group letter should increment for each new group in a particular verse or verse range: " + line);
							}

							if (!groupInstructionsMissing)
								groupInstructionsMissing = true;
							else
								expectedTotalQuestionsInCurrentPairOfGroups = -1;

							group = newGroup;

							if ((group - 'A') % 2 == 0)
							{
								countOfQuestionsInCurrentPairOfGroups = 0;
								expectedTotalQuestionsInCurrentPairOfGroups = -1;
							}
						}
						countOfQuestionsInCurrentPairOfGroups++;

						if (!groupInstructionsMissing)
						{
							Assert.IsTrue(countOfQuestionsInCurrentPairOfGroups <= expectedTotalQuestionsInCurrentPairOfGroups,
								"This question puts the total number of questions over the expected number for the current pair of groups: " + line);
						}

						groupOrQuestionIdNoteMissing = true;
						break;
					case 1:
						var bookIdInGroupId = matchedLine.Match.Groups["book"].Value;
						if (bookIdInGroupId != Empty)
						{
							groupOrQuestionIdNoteMissing = false;

							var type = matchedLine.Match.Groups["idType"].Value;
							Assert.IsFalse(groupInstructionsMissing,
								$"Instructions - with correct group letters - should come before Note with {type} information: " + line);

							if (type == "group")
							{
								Assert.IsTrue(expectedTotalQuestionsInCurrentPairOfGroups > 2,
									"Note should use \"group\" when there are more than 2 questions involved :" + line);
							}
							else
							{
								Assert.AreEqual(2, expectedTotalQuestionsInCurrentPairOfGroups,
									"Note should use \"question\" when there are only 2 questions involved :" + line);
							}

							Assert.That(bookIdInGroupId.Equals(bookId, StringComparison.OrdinalIgnoreCase),
								"Book mismatch in group ID line: " + line);
							Assert.AreEqual(chapter, matchedLine.Match.Groups["chapterInGrpId"].Value,
								"Chapter mismatch in group ID line: " + line);
							Assert.AreEqual(verses, matchedLine.Match.Groups["verses"].Value,
								"Chapter mismatch in group ID line: " + line);
						}
						else
						{
							Assert.IsTrue(groupInstructionsMissing,
								$"Found additional unexpected group instructions at line {matchedLine.LineNumber}: " + line);

							int expectedCount = -1;
							if (matchedLine.Match.Groups["useEitherGroup"].Value != Empty)
							{
								expectedCount = Parse(matchedLine.Match.Groups["count"].Value);
								Assert.That(expectedCount > 2,
									"Total questions in pair of groups should be more than 2: " + line);

								Assert.AreEqual(chapter, matchedLine.Match.Groups["chapter"].Value,
									"Chapter mismatch in group ID line: " + line);
								Assert.AreEqual(verses, matchedLine.Match.Groups["groupVerses"].Value,
									"Chapter mismatch in group ID line: " + line);

								var firstGroup = matchedLine.Match.Groups["firstGroup"].Value[0];
								var secondGroup = matchedLine.Match.Groups["secondGroup"].Value[0];
								Assert.AreEqual(1, secondGroup - firstGroup,
									"Second group letter should be one greater than the first: " + line);
								if (countOfQuestionsInCurrentPairOfGroups == 1)
								{
									Assert.AreEqual(group, firstGroup,
										"For first occurence of instructions, first group ID should match current group: " + line);
								}
								else
								{
									Assert.AreEqual(group, secondGroup,
										"For second occurence of instructions, second group ID should match current group: " + line);
								}
							}
							else if (matchedLine.Match.Groups["useEitherQuestion"].Value != Empty)
							{
								expectedCount = 2;

								Assert.AreEqual(group, matchedLine.Match.Groups["thisGroup"].Value[0],
									$"{matchedLine.LineNumber} Incorrect group ID for \"this group\":" + line);
								var otherGroup = matchedLine.Match.Groups["otherGroup"].Value[0];

								if (matchedLine.Match.Groups["following"].Value != Empty)
								{
									Assert.AreEqual(1, otherGroup - group,
										$"{matchedLine.LineNumber} Incorrect group ID for \"following group\":" + line);
								}
								else
								{
									Assert.AreEqual(1, group, otherGroup,
										$"{matchedLine.LineNumber} Incorrect group ID for \"preceding group\":" + line);
								}
							}
							else
							{
								Assert.Fail("Regex claimed to match but didn't set expected value for any group.");
							}

							groupInstructionsMissing = false;

							if (expectedTotalQuestionsInCurrentPairOfGroups > -1)
							{
								Assert.AreEqual(expectedTotalQuestionsInCurrentPairOfGroups, expectedCount,
									$"Instructions at line {matchedLine.LineNumber} contain total number of questions that does not match instructions for first question in previous group: " + line);
							}
							else
							{
								expectedTotalQuestionsInCurrentPairOfGroups = expectedCount;
							}

							Assert.IsTrue(countOfQuestionsInCurrentPairOfGroups <= expectedTotalQuestionsInCurrentPairOfGroups,
								$"Instructions contain incorrect total number of questions (line {matchedLine.LineNumber}: {line}");
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
						yield return new MatchedXmlLine(0, line, lineNumber, match);
					else if (regexLevel1 != null)
					{
						match = regexLevel1.Match(line);
						if (match.Success)
							yield return new MatchedXmlLine(1, line, lineNumber, match);
					}
					line = reader.ReadLine();
					lineNumber++;
				}
			}
		}
	}
}