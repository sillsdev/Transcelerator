using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	[TestFixture]
	class HtmlScriptGeneratorTests
	{
		private const string kHTML = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n" + 
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
		".extras {margin-bottom:0em;}\r\n" + 
		"body {\r\n" + 
		"}>\r\n" + 
		".leadingFloat\r\n" + 
		"{vfloat:left;\r\n" + 
		"}\r\n" + 
		".clearFloat {\r\n" + 
		" clear:both;\r\n" + 
		"}\r\n" + 
		".align_start\r\n" + 
		"{\r\n" + 
		"text-align:left;\r\n" + 
		"}\r\n" + 
		".align_center\r\n" + 
		"{\r\n" + 
		"text-align:center;\r\n" + 
		"}\r\n" + 
		".align_end\r\n" + 
		"{\r\n" + 
		"text-align:right;\r\n" + 
		"}\r\n" + 
		".usfm {\r\n" + 
		" font-family:\"Times New Roman\";\r\n" + 
		" font-size:14pt;\r\n" + 
		"}\r\n" + 
		"\r\n" + 
		"\r\n" + 
		"body::before {\r\n" + 
		"    background-image: linear-gradient(to top, rgba(252,252,252,0.4) 0%, rgba(252,252,252,1));\r\n" + 
		"    content: '';\r\n" + 
		"    position: fixed;\r\n" + 
		"    top: 0px;\r\n" + 
		"    width: 100%;\r\n" + 
		"    height: 4px;\r\n" + 
		"}\r\n" + 
		"\r\n" + 
		"</style>\r\n" + 
		"</head>\r\n" + 
		"<body lang=\"%VERNLOCALE%\">\r\n" + 
		"<h1 lang=\"en\">%TITLE%</h1>\r\n" + 
		"<h2 lang=\"en\">%SECTIONREF%</h2>\r\n" + 
		"<h3 lang=\"en\">%SECTIONHEAD%</h3>"; // TODO: Where does this come from? It doesn't appear to be English.

		[Test]
		public void Generate_DefaultOptionsWithSinglePhrase_GeneratesValidHtml()
		{
			ISectionInfo sectionInfo = new Section
			{
				Heading = "Oui Oui",
				Categories = new []
				{
					new Category {IsOverview = true, Type = "Overview"},
					new Category {IsOverview = false, Type = "Details"}
				},
				ScriptureReference = "Exodus 10:1-15",
				StartRef = 2010001,
				EndRef = 2010015
			};
			var repo = new PhraseRepo();
			repo.SourcePhrases.Add(new TranslatablePhrase(new TestQ("What is this?", "EXO 10.4", 2010004, 2010004, null), 0, 1, 0));

			var generator = new HtmlScriptGenerator("fr", repo.GetPhrases, "Comic Sans", tp => sectionInfo);
			generator.Extractor = new TestHtmlExtractor();
			generator.Title = "My Title";
			
			var sb = new StringBuilder();
			using (var tw = new StringWriter(sb))
				generator.Generate(tw);

			var expected = kHTML.Replace("%TITLE%", "My Title").Replace("%VERNLOCALE%", "fr").Replace("%VERNFONT%", "Comic Sans");

			Assert.That(sb.ToString(), Is.EqualTo(expected));
		}

		[Test]
		public void WriteTests()
		{
			Assert.Fail("Write some more tests");
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
				return IncludeVerseNumbers ? "<p>2This is the Scripture</p>" : "<p>This is the Scripture</p>";
			}
		}
	}
}

