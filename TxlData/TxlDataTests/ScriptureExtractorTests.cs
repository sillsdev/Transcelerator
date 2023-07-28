// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2022' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
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
	public class ScriptureExtractorTests
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
		public void GetAsHtmlFragment_CrossRefAndFootnoteInPrecedingVerses_ConvertedToDivAndSpanElements()
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 57);
			var endRef = new BCVRef(bookNumActs, 7, 57);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens());

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_m\">" +
				"<span class=\"verse\" number=\"57\">57</span>Entonces ellos se taparon los oídos.</div>" + Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_ExtraneousFootnoteCloser_ExtraMarkerIgnored()
		{
			int bookNumGen = BCVRef.BookToNumber("GEN");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumGen, 37, 29);
			var endRef = new BCVRef(bookNumGen, 37, 36);
			var tokens = new List<IUSFMToken>();
			tokens.Add(StubbedToken.GetChapter(bookNumGen, 37));
			tokens.Add(StubbedToken.GetParagraph("s", StubbedToken.TextType.Other));
			tokens.Add(StubbedToken.GetScriptureText("José es vendido por sus hermanos", StubbedToken.TextType.Other));
			tokens.Add(StubbedToken.GetParagraph("p"));
			tokens.Add(StubbedToken.GetVerse(1));
			tokens.Add(StubbedToken.GetScriptureText("Habitó Jacob en Canaán."));
			tokens.Add(StubbedToken.GetParagraph("p"));
			tokens.Add(StubbedToken.GetVerse(29));
			tokens.Add(StubbedToken.GetScriptureText("Después Rubén volvió y no halló a José, y se rasgó los vestidos. "));
			tokens.Add(StubbedToken.GetVerse(34));
			tokens.Add(StubbedToken.GetScriptureText("Verse thirty-four. "));
			tokens.Add(StubbedToken.GetVerse(35));
			tokens.Add(StubbedToken.GetScriptureText("Y todos sus hijos lo consolaron; mas él no quiso recibirlo, y dijo: Descenderé enlutado hasta el Seol."));
			tokens.AddRange(StubbedToken.GetFootnoteTokens("Nombre hebreo del lugar de los muertos."));
			tokens.Add(StubbedToken.GetMarkerToken(((IUSFMMarkerToken)tokens.Last()).Marker, MarkerType.End));
			tokens.Add(StubbedToken.GetScriptureText(" Y lo lloró su padre. "));
			tokens.Add(StubbedToken.GetVerse(36));
			tokens.Add(StubbedToken.GetScriptureText("Verse thirty-six."));

			mockedProject.Stub(p => p.GetUSFMTokens(bookNumGen, 37)).Return(tokens);

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"29\">29</span>Después Rubén volvió y no halló a José, y se rasgó los vestidos. " +
				"<span class=\"verse\" number=\"34\">34</span>Verse thirty-four. " +
				"<span class=\"verse\" number=\"35\">35</span>Y todos sus hijos lo consolaron; mas él no quiso recibirlo, y dijo: Descenderé enlutado hasta el Seol. Y lo lloró su padre. " +
				"<span class=\"verse\" number=\"36\">36</span>Verse thirty-six.</div>" +
				Environment.NewLine,
				result);
		}

		[Test]
		public void GetAsHtmlFragment_ItalicTextInFootnoteNotClosedExplicitly_EntireFootnoteIgnored()
		{
			int bookNumGen = BCVRef.BookToNumber("GEN");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumGen, 37, 29);
			var endRef = new BCVRef(bookNumGen, 37, 36);
			var tokens = new List<IUSFMToken>();
			tokens.Add(StubbedToken.GetChapter(bookNumGen, 37));
			tokens.Add(StubbedToken.GetParagraph("s", StubbedToken.TextType.Other));
			tokens.Add(StubbedToken.GetScriptureText("José es vendido por sus hermanos", StubbedToken.TextType.Other));
			tokens.Add(StubbedToken.GetParagraph("p"));
			tokens.Add(StubbedToken.GetVerse(1));
			tokens.Add(StubbedToken.GetScriptureText("Habitó Jacob en Canaán."));
			tokens.Add(StubbedToken.GetParagraph("p"));
			tokens.Add(StubbedToken.GetVerse(29));
			tokens.Add(StubbedToken.GetScriptureText("Después Rubén volvió y no halló a José, y se rasgó los vestidos. "));
			tokens.Add(StubbedToken.GetVerse(34));
			tokens.Add(StubbedToken.GetScriptureText("Verse thirty-four. "));
			tokens.Add(StubbedToken.GetVerse(35));
			tokens.Add(StubbedToken.GetScriptureText("Y todos sus hijos lo consolaron; mas él no quiso recibirlo, y dijo: Descenderé enlutado hasta el Seol."));
			tokens.AddRange(StubbedToken.GetFootnoteTokens("Nombre hebreo del lugar de los "));
			var i = tokens.Count - 2;
			tokens.Insert(i, StubbedToken.GetMarkerToken("i", MarkerType.Character, type: StubbedToken.TextType.FootnoteOrCrossReference));
			tokens.Insert(i + 1, StubbedToken.GetScriptureText("muertos", StubbedToken.TextType.FootnoteOrCrossReference));
			tokens.Add(StubbedToken.GetScriptureText(" Y lo lloró su padre. "));
			tokens.Add(StubbedToken.GetVerse(36));
			tokens.Add(StubbedToken.GetScriptureText("Verse thirty-six."));

			mockedProject.Stub(p => p.GetUSFMTokens(bookNumGen, 37)).Return(tokens);

			var extractor = new ScriptureExtractor(mockedProject, MakeVerseRef);
			extractor.IncludeVerseNumbers = true;
			var result = extractor.GetAsHtmlFragment(startRef, endRef);
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"29\">29</span>Después Rubén volvió y no halló a José, y se rasgó los vestidos. " +
				"<span class=\"verse\" number=\"34\">34</span>Verse thirty-four. " +
				"<span class=\"verse\" number=\"35\">35</span>Y todos sus hijos lo consolaron; mas él no quiso recibirlo, y dijo: Descenderé enlutado hasta el Seol. Y lo lloró su padre. " +
				"<span class=\"verse\" number=\"36\">36</span>Verse thirty-six.</div>" +
				Environment.NewLine,
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

		[TestCase(true)]
		[TestCase(false)]
		public void GetAsHtmlFragment_StartOfChapter_ResultExcludesChapterNumber(bool includeCpMarker)
		{
			int bookNumActs = BCVRef.BookToNumber("ACT");
			IProject mockedProject = MockRepository.GenerateStub<IProject>();
			var startRef = new BCVRef(bookNumActs, 7, 1);
			var endRef = new BCVRef(bookNumActs, 7, 2);
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 6)).Throw(new NotImplementedException("This test should not have requested ACTS 6 tokens."));
			mockedProject.Stub(p => p.GetUSFMTokens(bookNumActs, 7)).Return(GetActs7Tokens(true, includeCpMarker));

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

			List<IUSFMToken> acts6Tokens = new List<IUSFMToken>
			{
				StubbedToken.GetChapter(BCVRef.BookToNumber("ACT"), 6),
				StubbedToken.GetParagraph("p"),
				StubbedToken.GetVerse(1),
				StubbedToken.GetScriptureText("Verse one. "),
				StubbedToken.GetVerse(15),
				StubbedToken.GetScriptureText("Todo los del consejo miraron a Esteban que su rostro parecía el de un ángel.")
			};

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

		private IEnumerable<IUSFMToken> GetActs7Tokens(bool dientesAsGlossaryWord = false, bool includeCpMarker = false)
		{
			yield return StubbedToken.GetChapter(BCVRef.BookToNumber("ACT"), 7);
			if (includeCpMarker)
			{
				yield return StubbedToken.GetChapterLabel();
				yield return StubbedToken.GetScriptureText("Chpt. Seven", StubbedToken.TextType.Special);
			}
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
			foreach (var tok in StubbedToken.GetFootnoteTokens("Ps 35.16", "x"))
				yield return tok;
			yield return StubbedToken.GetVerse(55);
			yield return StubbedToken.GetScriptureText("Pero Esteban vio la gloria de Dios, y a Jesús, ");
			yield return StubbedToken.GetVerse(56);
			yield return StubbedToken.GetScriptureText("y dijo:");
			yield return StubbedToken.GetParagraph("q");
			yield return StubbedToken.GetScriptureText("He aquí, veo los cielos abiertos,");
			foreach (var tok in StubbedToken.GetFootnoteTokens("Ignore this"))
				yield return tok;
			yield return StubbedToken.GetScriptureText(" y al Hijo del Hombre que está a la diestra de Dios.");
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

			internal static IUSFMMarkerToken GetMarkerToken(string styleTag, MarkerType markerType, string data = null, TextType type = TextType.Scripture)
			{
				IUSFMMarkerToken mockedToken = MockRepository.GenerateStub<IUSFMMarkerToken>();
				SetTokenFlagsAndRef(mockedToken, type);
				mockedToken.Stub(t => t.Marker).Return(styleTag);
				mockedToken.Stub(t => t.Type).Return(markerType);
				mockedToken.Stub(t => t.Data).Return(data);
				return mockedToken;
			}

			internal static IUSFMToken GetChapter(int book, int chapter)
			{
				s_currentRef = new BCVRef(book, chapter, 0);
				return GetMarkerToken("c", MarkerType.Chapter, chapter.ToString());
			}

			internal static IUSFMToken GetChapterLabel()
			{
				IUSFMMarkerToken mockedToken = GetMarkerToken("cp", MarkerType.Paragraph);
				mockedToken.Stub(t => t.IsSpecial).Return(true);
				mockedToken.Stub(t => t.IsScripture).Return(true);
				mockedToken.Stub(t => t.IsPublishableVernacular).Return(false);

				return mockedToken;
			}

			internal static IUSFMToken GetParagraph(string styleTag, TextType type = TextType.Scripture)
			{
				return GetMarkerToken(styleTag, MarkerType.Paragraph, type: type);
			}

			internal static IUSFMToken GetVerse(int verse)
			{
				s_currentRef.Verse = verse;
				return GetMarkerToken("v", MarkerType.Verse, verse.ToString());
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
				IUSFMMarkerToken mockedMarkerToken = GetMarkerToken(styleTag, MarkerType.Character);
				var endMarkerToken = GetMockedEndMarkerToken(mockedMarkerToken);
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

				yield return endMarkerToken;
			}

			/// <summary>
			/// Call this before yielding the passed-in mockedMarkerToken because this also has the
			/// side-effect of stubbing out the EndMarker property for that token.
			/// </summary>
			private static IUSFMToken GetMockedEndMarkerToken(IUSFMMarkerToken mockedMarkerToken)
			{
				var endMarker = mockedMarkerToken.Marker + "*";
				mockedMarkerToken.Stub(t => t.EndMarker).Return(endMarker);
				IUSFMMarkerToken mockedEndMarkerToken = GetMarkerToken(endMarker, MarkerType.End);
				return mockedEndMarkerToken;
			}

			internal static IEnumerable<IUSFMToken> GetFootnoteTokens(string noteText, string marker = "f")
			{
				IUSFMMarkerToken mockedMarkerToken = GetMarkerToken(marker, MarkerType.Note, "*", TextType.FootnoteOrCrossReference);
				var endMarkerToken = GetMockedEndMarkerToken(mockedMarkerToken);
				yield return mockedMarkerToken;

				IUSFMMarkerToken mockedFtToken = GetMarkerToken(marker + "t", MarkerType.Character, type: TextType.FootnoteOrCrossReference);
				mockedMarkerToken.Stub(t => t.EndMarker).Return(mockedFtToken.Marker + "*");
				yield return mockedMarkerToken;

				yield return endMarkerToken;
			}
		}
	}
}
