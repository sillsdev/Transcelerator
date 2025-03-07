// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2021' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.   
//	
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: HtmlScriptGeneratorTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using SIL.Scripture;
using Is = NUnit.Framework.Is;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class HtmlScriptGeneratorTests
	{
		private const string kHtmlSection = "<h2 lang=\"en-US\">%SECTIONHEADORREF%</h2>\r\n" + 
			"<p>%SECTVERSENUM%This is the section head Scripture.</p>\r\n" + 
			"<h3%CATEGORY_LANG%>%CATEGORY%</h3>\r\n";

		private const string kHtmlStart = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n" + 
		"<html>\r\n" + 
		"<head>\r\n" + 
		"<meta content=\"text/html; charset=UTF-8\" http-equiv=\"content-type\"/>\r\n" + 
		"<title>%TITLE%</title>\r\n" + 
		"<style type=\"text/css\">\r\n" + 
		":lang(%VERNLOCALE%) {font-family:%VERNFONT%,serif,Arial Unicode MS;}\r\n" + 
		"body {font-size:100%; counter-reset:qnum;}\r\n" + 
		".question {counter-increment:qnum;}\r\n" + 
		"p.question:before {content:counter(qnum) \". \";}\r\n" + 
		"h1 {font-size:2.0em;\r\n" + 
		"  text-align:center}\r\n" + 
		"h2 {font-size:1.7em;\r\n" + 
		"  color:white;\r\n" + 
		"  background-color:black;}\r\n" + 
		"h3 {font-size:1.3em;\r\n" + 
		"  color:blue;}\r\n" + 
		"h4 {font-size:1.1em;}\r\n" + 
		"p {font-size:1.0em;}\r\n" + 
		"h1:lang(en) {font-family:sans-serif;}\r\n" + 
		"h2:lang(en) {font-family:serif;}\r\n" + 
		"p:lang(en) {font-family:serif;\r\n" + 
		"font-size:0.85em;}\r\n" + 
		".verse {vertical-align: super; font-size: .80em; color:DimGray;}\r\n" + 
		"h3 {color:Blue;}\r\n" + 
		".questionbt {color:Gray;}\r\n" + 
		".answer {color:Green;}\r\n" + 
		".comment {color:Red;}\r\n" + 
		".extras {margin-bottom:%BLANKLINES%em;}\r\n" +
		"</style>\r\n" +
		"</head>\r\n" +
		"<body lang=\"%VERNLOCALE%\">\r\n" + 
		"<h1 lang=\"en-US\">%TITLE%</h1>\r\n" +
		kHtmlSection;

		private const string kHtmlExtrasDiv = "<div class=\"extras\" lang=\"en-US\">\r\n";
		private const string kHtmlEnd = "</body>\r\n";

		private readonly IPhraseTranslationHelper m_helper = MockRepository.GenerateMock<IPhraseTranslationHelper>();

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			m_helper.Stub(h => h.GetCategoryName(Arg<int>.Matches(i => i == 0), out Arg<string>.Out("en-US").Dummy)).Return("Overview");
			m_helper.Stub(h => h.GetCategoryName(Arg<int>.Matches(i => i == 1), out Arg<string>.Out(null).Dummy)).Return("Details-translated");
		}

		[TestCase("EXO")]
		[TestCase(null)]
		public void Generate_DefaultOptionsWithSingleUntranslatedPhrase_GeneratesValidHtml(string book)
		{
			ISectionInfo sectionInfo = GetSectionInfoForExo1V1ThruV15();
			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("What is this?", "EXO 10.4",
				2010004, 2010004, null), 0, 1, 0, m_helper));

			var generator = new HtmlScriptGenerator(new TestScriptGenerationSettings(), "fr",
				repo.GetPhrases, "Comic Sans", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.UseLWC;
			SetRefRange(generator, book, sectionInfo);

			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHtmlStart.Replace("%TITLE%", "My Title")
				.Replace("%VERNLOCALE%", "fr")
				.Replace("%VERNFONT%", "Comic Sans")
				.Replace("%BLANKLINES%", 0.ToString())
				.Replace("%SECTIONHEADORREF%", sectionInfo.Heading)
				.Replace("%SECTVERSENUM%", "")
				.Replace("%CATEGORY_LANG%", "")
				.Replace("%CATEGORY%", "Details-translated") +
				"<p>This is the Scripture.</p>\r\n" +
				kHtmlExtrasDiv +
				"<p class=\"question\" lang=\"en-US\">What is this?</p>\r\n</div>\r\n" +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[TestCase("EXO")]
		[TestCase(null)]
		public void Generate_IncludeVerseNumbersWithSingleTranslatedPhrase_GeneratesValidHtml(string book)
		{
			ISectionInfo sectionInfo = GetSectionInfoForExo1V1ThruV15();

			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 10.4",
				2010004, 2010004, "What is this?", "Nothing"), 0, 1, 0, m_helper)
			{
				Translation = "Fmugh zorb wis Blen#"
			});

			var generator = new HtmlScriptGenerator(new TestScriptGenerationSettings(), "xyz",
				repo.GetPhrases, "Arial", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.UseLWC;
			generator.IncludeVerseNumbers = true;
			int addProjectSpecificCssEntriesCalled = 0;
			generator.AddProjectSpecificCssEntries = delegate { addProjectSpecificCssEntriesCalled++; };
			SetRefRange(generator, book, sectionInfo);
			
			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHtmlStart.Replace("%TITLE%", "My Title")
					.Replace("%VERNLOCALE%", "xyz")
					.Replace("%VERNFONT%", "Arial")
					.Replace("%BLANKLINES%", 0.ToString())
					.Replace("%SECTIONHEADORREF%", sectionInfo.Heading)
					.Replace("%SECTVERSENUM%", "1")
					.Replace("%CATEGORY_LANG%", "")
					.Replace("%CATEGORY%", "Details-translated") +
				"<p>4This is the Scripture.</p>\r\n" +
				kHtmlExtrasDiv +
				"<p class=\"question\">Fmugh zorb wis Blen#</p>\r\n</div>\r\n" +
				"<p class=\"questionbt\">What is this?</p>\r\n" +
				"<p class=\"answer\">Nothing</p>\r\n" +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
			Assert.That(addProjectSpecificCssEntriesCalled, Is.EqualTo(1));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Generate_ExcludeLWCQuestionsAndAnswers_SkipUntranslated_MultipleSections_GeneratesValidHtml(bool includeComments)
		{
			const string book = "EXO";

			ISectionInfo sectionInfo1 = GetSectionInfoForExo1V1ThruV15();
			ISectionInfo sectionInfo2 = GetSectionInfo("Somebody Does Something", "Exodus 11:1-7", 2, 11, 1, 7);

			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 10.4",
				2010004, 2010004, "What is this?", "Nothing"), 0, 1, 0, m_helper)
			{
				Translation = "Fmugh zorb wis Blen#"
			});
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 11.5-6",
				2011005, 2011006, "Why is this not translated?", null), 1, 0, 0, m_helper));
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 11.6-7",
				2011006, 2011007, "Who did what?", null)
			{
				Notes = new []{"This is a comment."} }, 1, 0, 1, m_helper)
			{
				Translation = "Klumpf zad 'op#"
			});

			var generator = new HtmlScriptGenerator(new TestScriptGenerationSettings(), "xyz",
				repo.GetPhrases, "Arial", tp => tp.SectionId == 0 ? sectionInfo1 : sectionInfo2);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "Questions for Exodus";
			generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.Skip;
			generator.IncludeLWCQuestions = false;
			generator.IncludeLWCAnswers = false;
			generator.IncludeLWCComments = includeComments;
			SetRefRange(generator, book, null);
			
			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHtmlStart.Replace("%TITLE%", "Questions for Exodus")
					.Replace("%VERNLOCALE%", "xyz")
					.Replace("%VERNFONT%", "Arial")
					.Replace("%BLANKLINES%", 0.ToString())
					.Replace("%SECTIONHEADORREF%", sectionInfo1.Heading)
					.Replace("%SECTVERSENUM%", "")
					.Replace("%CATEGORY_LANG%", "")
					.Replace("%CATEGORY%", "Details-translated") +
				"<p>This is the Scripture.</p>\r\n" +
				kHtmlExtrasDiv +
				"<p class=\"question\">Fmugh zorb wis Blen#</p>\r\n</div>\r\n" +
				kHtmlSection.Replace("%SECTIONHEADORREF%", sectionInfo2.Heading)
					.Replace("%SECTVERSENUM%", "")
					.Replace("%CATEGORY_LANG%", " lang=\"en-US\"")
					.Replace("%CATEGORY%", "Overview") +
				kHtmlExtrasDiv +
				"<p class=\"question\">Klumpf zad 'op#</p>\r\n</div>\r\n" +
				(includeComments ? "<p class=\"comment\">This is a comment.</p>\r\n" : "") +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[Test]
		public void Generate_ExcludeLWCQuestionsAndAnswers_ExcludeScriptureForQuestions_GeneratesValidHtml()
		{
			const string book = "EXO";

			ISectionInfo sectionInfo1 = GetSectionInfoForExo1V1ThruV15();
			ISectionInfo sectionInfo2 = GetSectionInfo("Somebody Does Something", "Exodus 11:1-7", 2, 11, 1, 7);

			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 10.4",
				2010004, 2010004, "What is this?", "Nothing"), 0, 1, 0, m_helper)
			{
				Translation = "Fmugh zorb wis Blen#"
			});
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 11.5-6",
				2011005, 2011006, "Why is this not translated?", null), 1, 0, 0, m_helper));
			repo.SourcePhrases.Add(new TranslatablePhrase(new Question("EXO 11.6-7",
				2011006, 2011007, "Who did what?", null)
			{
				Notes = new []{"This is a comment."} }, 1, 0, 1, m_helper)
			{
				Translation = "Klumpf zad 'op#"
			});

			var generator = new HtmlScriptGenerator(new TestScriptGenerationSettings(), "xyz",
				repo.GetPhrases, "Arial", tp => tp.SectionId == 0 ? sectionInfo1 : sectionInfo2);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "Questions for Exodus";
			generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.Skip;
			generator.IncludeLWCQuestions = false;
			generator.IncludeLWCAnswers = false;
			generator.IncludeLWCComments = false;
			generator.OutputScriptureForQuestions = false;
			SetRefRange(generator, book, null);
			
			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHtmlStart.Replace("%TITLE%", "Questions for Exodus")
					.Replace("%VERNLOCALE%", "xyz")
					.Replace("%VERNFONT%", "Arial")
					.Replace("%BLANKLINES%", 0.ToString())
					.Replace("%SECTIONHEADORREF%", sectionInfo1.Heading)
					.Replace("%SECTVERSENUM%", "")
					.Replace("%CATEGORY_LANG%", "")
					.Replace("%CATEGORY%", "Details-translated") +
				kHtmlExtrasDiv +
				"<p class=\"question\">Fmugh zorb wis Blen#</p>\r\n</div>\r\n" +
				kHtmlSection.Replace("%SECTIONHEADORREF%", sectionInfo2.Heading)
					.Replace("%SECTVERSENUM%", "")
					.Replace("%CATEGORY_LANG%", " lang=\"en-US\"")
					.Replace("%CATEGORY%", "Overview") +
				kHtmlExtrasDiv +
				"<p class=\"question\">Klumpf zad 'op#</p>\r\n</div>\r\n" +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Generate_OutOfOrderQuestion_GeneratesValidHtml(bool outputPassageForOutOfOrderQuestions)
		{
			ISectionInfo sectionInfo = GetSectionInfoForExo1V1ThruV15();
			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("Tell me some stuff.", "",
				2010001, 2010015, null), 0, 0, 0, m_helper));
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("What is this?", "EXO 10.4",
				2010004, 2010004, null), 0, 1, 0, m_helper));
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("Can I go back and ask something?",
				"EXO 10.2-3", 2010002, 2010003, null), 0, 1, 1, m_helper));

			var generator = new HtmlScriptGenerator(new TestScriptGenerationSettings(), "fr",
				repo.GetPhrases, "Comic Sans", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.UseLWC;
			generator.IncludeVerseNumbers = true;
			generator.OutputPassageForOutOfOrderQuestions = outputPassageForOutOfOrderQuestions;
			SetRefRange(generator, "EXO", null);

			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHtmlStart.Replace("%TITLE%", "My Title")
					.Replace("%VERNLOCALE%", "fr")
					.Replace("%VERNFONT%", "Comic Sans")
					.Replace("%BLANKLINES%", 0.ToString())
					.Replace("%SECTIONHEADORREF%", sectionInfo.Heading)
					.Replace("%SECTVERSENUM%", "1")
					.Replace("%CATEGORY_LANG%", " lang=\"en-US\"")
					.Replace("%CATEGORY%", "Overview") +
				kHtmlExtrasDiv +
				"<p class=\"question\" lang=\"en-US\">Tell me some stuff.</p>\r\n</div>\r\n" +
				"<h3>Details-translated</h3>\r\n" +
				"<p>4This is the Scripture.</p>\r\n" +
				kHtmlExtrasDiv +
				"<p class=\"question\" lang=\"en-US\">What is this?</p>\r\n</div>\r\n" +
				(outputPassageForOutOfOrderQuestions ? "<p>2This is the Scripture.</p>\r\n" :
					"<h4 class=\"summaryRef\">EXO 10.2-3</h4>\r\n") +
				kHtmlExtrasDiv +
				"<p class=\"question\" lang=\"en-US\">Can I go back and ask something?</p>\r\n</div>\r\n" +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		private static Section GetSectionInfoForExo1V1ThruV15() => GetSectionInfo("Oui Oui", "Exodus 10:1-15", 2, 10, 1, 15);

		private static Section GetSectionInfo(string heading, string scrRef, int book, int chapter, int startVerse, int endVerse)
		{
			return new Section
			{
				Heading = heading,
				Categories = new []
				{
					new Category {IsOverview = true, Type = "Overview"},
					new Category {IsOverview = false, Type = "Details"}
				},
				ScriptureReference = scrRef,
				StartRef = new BCVRef(book, chapter, startVerse),
				EndRef = new BCVRef(book, chapter, endVerse),
			};
		}

		private void SetRefRange(HtmlScriptGenerator generator, string book, ISectionInfo sectionInfo)
		{
			if (book == null)
			{
				generator.VerseRangeStartRef = new BCVRef(sectionInfo.StartRef);
				generator.VerseRangeEndRef = new BCVRef(sectionInfo.EndRef);
			}
			else
				generator.SelectedBook = "EXO";
		}

		private class PhraseRepo
		{
			public List<TranslatablePhrase> SourcePhrases { get; set; } = new List<TranslatablePhrase>();

			public IEnumerable<TranslatablePhrase> GetPhrases(Func<TranslatablePhrase, bool> include)
			{
				return SourcePhrases.Where(include);
			}
		}

		private class TestHtmlExtractor : IHtmlScriptureExtractor
		{
			public bool IncludeVerseNumbers { get; set; }

			public string GetAsHtmlFragment(int startRef, int endRef)
			{
				var html = new StringBuilder("<p>");
				if (IncludeVerseNumbers)
					html.Append(new BCVRef(startRef).Verse);
				if (endRef - startRef > 3)
					html.Append("This is the section head Scripture.");
				else
					html.Append("This is the Scripture.");
				html.Append("</p>\r\n");
				return html.ToString();
			}
		}
	}

	class TestScriptGenerationSettings : IScriptGenerationSettings
	{
		public RangeOption GenerateScriptRange { get; set; }
		public string SelectedBook { get; set; }
		public HandleUntranslatedQuestionsOption HandlingOfUntranslatedQuestions { get; set; }
		public bool OutputPassageForOutOfOrderQuestions { get; set; }
		public bool OutputFullPassageAtStartOfSection { get; set; } = true;
		public bool OutputScriptureForQuestions { get; set; } = true;
		public bool IncludeVerseNumbers { get; set; }
		public bool IncludeLWCQuestions { get; set; } = true;
		public bool IncludeLWCAnswers { get; set; } = true;
		public bool IncludeLWCComments { get; set; } = true;
		public bool UseExternalCss { get; set; }
		public string CssFile { get; set; } = "ComprehensionChecking.css";
		public bool UseAbsolutePathForCssFile { get; set; }
		public string Folder { get; set; }
		public string QuestionGroupHeadingsTextColor { get; set; } = "Blue";
		public string LWCQuestionTextColor { get; set; } = "Gray";
		public string LWCAnswerTextColor { get; set; } = "Green";
		public string CommentTextColor { get; set; } = "Red";
		public int NumberOfBlankLinesForAnswer { get; set; }
		public bool NumberQuestions { get; set; } = true;
		public string LWCLocale { get; set; } = "en-US";
	}
}

