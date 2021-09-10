using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using NUnit.Framework;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	[TestFixture]
	class ScriptureExtractorTests
	{
		IVerseRef MakeVerseRef(int bbcccvvv) => new BcvRefIVerseAdapter(new BCVRef(bbcccvvv));

		[Test]
		public void GetAsHtmlFragment_ExcludeVerseNumbers_ParagraphsAndVerses_ConvertedToDivAndSpanElements()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 54);
			var endRef = new BCVRef(bookNumActs, 7, 57);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens());

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = false;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"Oyendo esto, se enfurecían y crujían los dientes. Pero Esteban vio la gloria de Dios, y a Jesús, y dijo:</div>" +
				"<div class=\"usfm_q\">" +
				"He aquí, veo los cielos abiertos, y al Hijo del Hombre que está a la diestra de Dios.</div>" +
				"<div class=\"usfm_m\">" +
				"Entonces ellos se taparon los oídos.</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_IncludeVerseNumbers_ParagraphsAndVerses_ConvertedToDivAndSpanElements()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 54);
			var endRef = new BCVRef(bookNumActs, 7, 57);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens());

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"54\">54</span>Oyendo esto, se enfurecían y crujían los dientes. " +
				"<span class=\"verse\" number=\"55\">55</span>Pero Esteban vio la gloria de Dios, y a Jesús, " +
				"<span class=\"verse\" number=\"56\">56</span>y dijo:</div>" +
				"<div class=\"usfm_q\">" +
				"He aquí, veo los cielos abiertos, y al Hijo del Hombre que está a la diestra de Dios.</div>" +
				"<div class=\"usfm_m\">" +
				"<span class=\"verse\" number=\"57\">57</span>Entonces ellos se taparon los oídos.</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_GlossaryWord_GlossaryJunkRemoved()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 54);
			var endRef = new BCVRef(bookNumActs, 7, 54);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens(true));

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"54\">54</span>Oyendo esto, se enfurecían y crujían los <span class=\"usfm_w\">dientes</span>. " +
				"</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_StartOfChapter_ResultExcludesChapterNumber()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 1);
			var endRef = new BCVRef(bookNumActs, 7, 2);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 6)).Throw(new NotImplementedException("This test should not have requested ACTS 6 tokens."));
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens(true));

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"1\">1</span>El sumo sacerdote preguntó a Esteban:</div>" +
				"<div class=\"usfm_p\">" + 
				"— ¿Es eso cierto?</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_CrossesChapterBreak_ResultIncludesChapterNumber()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 6, 15);
			var endRef = new BCVRef(bookNumActs, 7, 2);

			List<IUSFMToken> acts6Tokens = new List<IUSFMToken>();
			acts6Tokens.Add(StubbedToken.GetChapter(BCVRef.BookToNumber("ACT"), 6));
			acts6Tokens.Add(StubbedToken.GetParagraph("p"));
			acts6Tokens.Add(StubbedToken.GetVerse(1));
			acts6Tokens.Add(StubbedToken.GetScriptureText("Verse one. "));
			acts6Tokens.Add(StubbedToken.GetVerse(15));
			acts6Tokens.Add(StubbedToken.GetScriptureText("Todo los del consejo miraron a Esteban que su rostro parecía el de un ángel."));

			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs)).Return(acts6Tokens.Union(GetActs7Tokens(true)));

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"15\">15</span>Todo los del consejo miraron a Esteban que su rostro parecía el de un ángel.</div>" +
				"<div class=\"chapter\" number=\"7\">7</div>" +
				"<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"1\">1</span>El sumo sacerdote preguntó a Esteban:</div>" +
				"<div class=\"usfm_p\">" + 
				"— ¿Es eso cierto?</div>" + Environment.NewLine,
				result);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetAsHtmlFragment_EmptyVerses_Removed(bool includeEmptyVerseAtEnd)
		{
			int bookNumMat = BCVRef.BookToNumber("MAT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumMat, 1, 1);
			var endRef = new BCVRef(bookNumMat, 1, 17);

			List<IUSFMToken> tokens = new List<IUSFMToken>();
			StubbedToken.s_currentRef = new BCVRef(bookNumMat, 1, 0);
			tokens.Add(StubbedToken.GetParagraph("p"));
			tokens.Add(StubbedToken.GetVerse(1));
			tokens.Add(StubbedToken.GetScriptureText("This is the "));
			tokens.AddRange(StubbedToken.GetCharacterStringTokens("w", "genealogy", "list of begats"));
			tokens.Add(StubbedToken.GetScriptureText(" of Jesus Christ. "));
			tokens.Add(StubbedToken.GetVerse(2));
			tokens.Add(StubbedToken.GetVerse(3));
			tokens.Add(StubbedToken.GetVerse(4));
			tokens.Add(StubbedToken.GetVerse(6));
			tokens.Add(StubbedToken.GetVerse(7));
			tokens.Add(StubbedToken.GetScriptureText("Then spake He, saying, "));
			tokens.AddRange(StubbedToken.GetCharacterStringTokens("wj", "This is my genealogy."));
			tokens.Add(StubbedToken.GetVerse(8));
			tokens.Add(StubbedToken.GetVerse(12));
			tokens.Add(StubbedToken.GetVerse(13));
			tokens.Add(StubbedToken.GetParagraph("q"));
			tokens.Add(StubbedToken.GetScriptureText("This is the start of verse 13, but in a different paragraph. "));
			tokens.Add(StubbedToken.GetVerse(14));
			tokens.Add(StubbedToken.GetVerse(15));
			tokens.Add(StubbedToken.GetVerse(16));
			tokens.Add(StubbedToken.GetScriptureText("Blah."));
			if (includeEmptyVerseAtEnd)
				tokens.Add(StubbedToken.GetVerse(17));

			mockedProject.Stub(p => p.GetUSFMTokens(bookNumMat, 1)).Return(tokens);

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);

			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"1\">1</span>This is the <span class=\"usfm_w\">genealogy</span> of Jesus Christ. " +
				"<span class=\"verse\" number=\"7\">7</span>Then spake He, saying, " +
				"<span class=\"usfm_wj\">This is my genealogy.</span>" +
				"<span class=\"verse\" number=\"13\">13</span></div>" +
				"<div class=\"usfm_q\">" +
				"This is the start of verse 13, but in a different paragraph. " +
				"<span class=\"verse\" number=\"16\">16</span>Blah." +
				"</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_NoTokensForChapter_Empty()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 2, 1);
			var endRef = new BCVRef(bookNumActs, 2, 5);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 2)).Return(new IUSFMToken[0]);

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = false;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual(Environment.NewLine, result);
		}

		private IEnumerable<IUSFMToken> GetActs7Tokens(bool dientesAsGlossaryWord = false)
		{
			yield return StubbedToken.GetChapter(BCVRef.BookToNumber("ACT"), 7);
			yield return StubbedToken.GetParagraph("s", StubbedToken.TextType.Other);
			yield return StubbedToken.GetScriptureText("Discurso de Esteban", StubbedToken.TextType.Other);
			yield return StubbedToken.GetParagraph("p");
			yield return StubbedToken.GetVerse(1);
			yield return StubbedToken.GetScriptureText("El sumo sacerdote preguntó a Esteban:");
			yield return StubbedToken.GetParagraph("p");
			yield return StubbedToken.GetScriptureText("— ¿Es eso cierto?");
			yield return StubbedToken.GetParagraph("p");
			yield return StubbedToken.GetVerse(53);
			yield return StubbedToken.GetScriptureText("This is verse fifty-three. ");
			yield return StubbedToken.GetParagraph("p");
			yield return StubbedToken.GetVerse(54);
			if (dientesAsGlossaryWord)
			{
				yield return StubbedToken.GetScriptureText("Oyendo esto, se enfurecían y crujían los ");
				// \w dientes|basically, we're talking teeth\w*
				foreach (var tok in StubbedToken.GetCharacterStringTokens("w", "dientes", "basically, we're talking teeth"))
					yield return tok;
				yield return StubbedToken.GetScriptureText(". ");
			}
			else
				yield return StubbedToken.GetScriptureText("Oyendo esto, se enfurecían y crujían los dientes. ");
			yield return StubbedToken.GetVerse(55);
			yield return StubbedToken.GetScriptureText("Pero Esteban vio la gloria de Dios, y a Jesús, ");
			yield return StubbedToken.GetVerse(56);
			yield return StubbedToken.GetScriptureText("y dijo:");
			yield return StubbedToken.GetParagraph("q");
			yield return StubbedToken.GetScriptureText("He aquí, veo los cielos abiertos, y al Hijo del Hombre que está a la diestra de Dios.");
			yield return StubbedToken.GetParagraph("m");
			yield return StubbedToken.GetVerse(57);
			yield return StubbedToken.GetScriptureText("Entonces ellos se taparon los oídos.");
		}

		private static class StubbedToken
		{
			public enum TextType
			{
				Scripture,
				Figure,
				FootnoteOrCrossReference,
				Special,
				Other,
			}

			internal static BCVRef s_currentRef;

			private static void SetTokenFlagsAndRef(IUSFMToken mockedToken, TextType type = TextType.Scripture)
			{
				mockedToken.Stub(t => t.IsFigure).Return(type == TextType.Figure);
				mockedToken.Stub(t => t.IsFootnoteOrCrossReference).Return(type == TextType.FootnoteOrCrossReference);
				mockedToken.Stub(t => t.IsMetadata).Return(false);
				mockedToken.Stub(t => t.IsPublishableVernacular).Return(true);
				mockedToken.Stub(t => t.IsScripture).Return(type == TextType.Scripture);
				mockedToken.Stub(t => t.IsSpecial).Return(type == TextType.Special);
				mockedToken.Stub(t => t.VerseRef).Return(new BcvRefIVerseAdapter(new BCVRef(s_currentRef)));
			}

			private static IUSFMMarkerToken GetMarkerToken(string styleTag, TextType type = TextType.Scripture)
			{
				IUSFMMarkerToken mockedToken = MockRepository.GenerateStub<IUSFMMarkerToken>();
				SetTokenFlagsAndRef(mockedToken, type);
				mockedToken.Stub(t => t.Marker).Return(styleTag);
				return mockedToken;
			}

			internal static IUSFMToken GetChapter(int book, int chapter)
			{
				s_currentRef = new BCVRef(book, chapter, 0);
				IUSFMMarkerToken mockedToken = GetMarkerToken("c");
				mockedToken.Stub(t => t.Data).Return(chapter.ToString());
				mockedToken.Stub(t => t.Type).Return(MarkerType.Chapter);

				return mockedToken;
			}

			internal static IUSFMToken GetParagraph(string styleTag, TextType type = TextType.Scripture)
			{
				IUSFMMarkerToken mockedToken = GetMarkerToken(styleTag, type);
				mockedToken.Stub(t => t.Data).Return(null);
				mockedToken.Stub(t => t.Type).Return(MarkerType.Paragraph);

				return mockedToken;
			}

			internal static IUSFMToken GetVerse(int verse)
			{
				s_currentRef.Verse = verse;
				IUSFMMarkerToken mockedToken = GetMarkerToken("v");
				mockedToken.Stub(t => t.Data).Return(verse.ToString());
				mockedToken.Stub(t => t.Type).Return(MarkerType.Verse);

				return mockedToken;
			}

			internal static IUSFMToken GetScriptureText(string text, TextType type = TextType.Scripture)
			{
				IUSFMTextToken mockedToken = MockRepository.GenerateStub<IUSFMTextToken>();
				SetTokenFlagsAndRef(mockedToken, type);
				mockedToken.Stub(t => t.Text).Return(text);

				return mockedToken;
			}

			internal static IEnumerable<IUSFMToken> GetCharacterStringTokens(string styleTag, string text, string defaultAttribute = null)
			{
				IUSFMMarkerToken mockedMarkerToken = GetMarkerToken(styleTag);
				mockedMarkerToken.Stub(t => t.Data).Return(null);
				mockedMarkerToken.Stub(t => t.Type).Return(MarkerType.Character);
				IEnumerable<IUSFMAttribute> attribs = null;
				if (defaultAttribute != null)
				{
					IUSFMAttribute mockedAttribute = MockRepository.GenerateStub<IUSFMAttribute>();
					mockedAttribute.Stub(a => a.Name).Return(null);
					mockedAttribute.Stub(a => a.Value).Return(defaultAttribute);
					attribs = new[] {mockedAttribute};
					mockedMarkerToken.Stub(t => t.Attributes).Return(attribs);
				}
				yield return mockedMarkerToken;
				yield return GetScriptureText(text);

				IUSFMAttributeToken mockedAttribToken = MockRepository.GenerateStub<IUSFMAttributeToken>();
				mockedAttribToken.Stub(t => t.Attributes).Return(attribs);
				yield return mockedAttribToken;

				IUSFMMarkerToken mockedEndMarkerToken = GetMarkerToken(styleTag + "*");
				mockedEndMarkerToken.Stub(t => t.Data).Return(null);
				mockedEndMarkerToken.Stub(t => t.Type).Return(MarkerType.End);

				yield return mockedEndMarkerToken;
			}
		}
	}
}
