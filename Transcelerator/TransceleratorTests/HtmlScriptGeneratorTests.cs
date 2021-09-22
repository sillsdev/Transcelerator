using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	[TestFixture]
	class HtmlScriptGeneratorTests
	{
		private const string kHTMLSTart = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n" + 
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
		"<h2 lang=\"en-US\">%SECTIONHEADORREF%</h2>\r\n" + 
		"%SECTIONSCRIPTURE%\r\n" + 
		"<h3 lang=\"en-US\">%CATEGORY%</h3>\r\n";

		private const string kHtmlExtrasDiv = "<div class=\"extras\" lang=\"en-US\">\r\n</div>\r\n";
		private const string kHtmlEnd = "</body>\r\n";

		[SetUp]
		public void Setup()
		{
			TranslatablePhrase.s_helper = MockRepository.GenerateMock<IPhraseTranslationHelper>();
			TranslatablePhrase.s_helper.Stub(h => h.GetCategoryName(0)).Return("Overview");
			TranslatablePhrase.s_helper.Stub(h => h.GetCategoryName(1)).Return("Details");
		}

		[TestCase("EXO")]
		[TestCase(null)]
		public void Generate_DefaultOptionsWithSingleUntranslatedPhrase_GeneratesValidHtml(string book)
		{
			ISectionInfo sectionInfo = GetSectionInfoForExo1V1ThruV15();
			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("What is this?", "EXO 10.4", 2010004, 2010004, null), 0, 1, 0));

			var generator = new HtmlScriptGenerator("fr", repo.GetPhrases, "Comic Sans", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			generator.HandlingOfUntranslatedQuestions = HtmlScriptGenerator.HandleUntranslatedQuestionsOption.UseLWC;
			SetRefRange(generator, book, sectionInfo);

			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHTMLSTart.Replace("%TITLE%", "My Title")
				.Replace("%VERNLOCALE%", "fr")
				.Replace("%VERNFONT%", "Comic Sans")
				.Replace("%BLANKLINES%", 0.ToString())
				.Replace("%SECTIONHEADORREF%", sectionInfo.Heading)
				.Replace("%SECTIONSCRIPTURE%", "<p>This is the section head Scripture.</p>")
				.Replace("%CATEGORY%", "Details") +
				"<p>This is the Scripture.</p>\r\n" +
				"<p class=\"question\" lang=\"en-US\">What is this?</p>\r\n" +
				kHtmlExtrasDiv +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[TestCase("EXO")]
		[TestCase(null)]
		public void Generate_IncludeVerseNumbersWithSingleTranslatedPhrase_GeneratesValidHtml(string book)
		{
			ISectionInfo sectionInfo = GetSectionInfoForExo1V1ThruV15();

			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("What is this?", "EXO 10.4", 2010004, 2010004, null), 0, 1, 0)
			{
				Translation = "Fmugh zorb wis Blen#"
			});

			var generator = new HtmlScriptGenerator("xyz", repo.GetPhrases, "Arial", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			generator.HandlingOfUntranslatedQuestions = HtmlScriptGenerator.HandleUntranslatedQuestionsOption.UseLWC;
			generator.IncludeVerseNumbers = true;
			SetRefRange(generator, book, sectionInfo);
			
			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHTMLSTart.Replace("%TITLE%", "My Title")
					.Replace("%VERNLOCALE%", "xyz")
					.Replace("%VERNFONT%", "Arial")
					.Replace("%BLANKLINES%", 0.ToString())
					.Replace("%SECTIONHEADORREF%", sectionInfo.Heading)
					.Replace("%SECTIONSCRIPTURE%", "<p>1This is the section head Scripture.</p>")
					.Replace("%CATEGORY%", "Details") +
				"<p>4This is the Scripture.</p>\r\n" +
				"<p class=\"question\">Fmugh zorb wis Blen#</p>\r\n" +
				"<p class=\"questionbt\">What is this?</p>\r\n" +
				kHtmlExtrasDiv +
				kHtmlEnd;

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[Test]
		public void WriteTests()
		{
			Assert.Fail("Write some more tests");
		}

		private static Section GetSectionInfoForExo1V1ThruV15()
		{
			return new Section
			{
				Heading = "Oui Oui",
				Categories = new []
				{
					new Category {IsOverview = true, Type = "Overview"},
					new Category {IsOverview = false, Type = "Details"}
				},
				ScriptureReference = "Exodus 10:1-15",
				StartRef = 2010001,
				EndRef = 2010015,
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
}

