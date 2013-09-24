// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: TranslatablePhraseTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddInSideViews;
using NUnit.Framework;
using Rhino.Mocks;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
    /// Tests the TranslatablePhrase implementation
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
    public class TranslatablePhraseTests : PhraseTranslationTestBase
	{
	    [SetUp]
	    public override void Setup()
	    {
            base.Setup();
	        DummyKeyTermRenderingInfo.s_ktRenderings = m_dummyKtRenderings;
	        TranslatablePhrase.s_helper = MockRepository.GenerateMock<IPhraseTranslationHelper>();
        }

        #region AppliesToReference tests
        [Test]
        public void AppliesToReference_CategoryName_ReturnsFalse()
        {
            m_sections.Items[0].Categories[0].Type = "Overview";
            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();
            Assert.AreEqual(1, phrases.Count);
            Assert.IsTrue(phrases[0].IsCategoryName);
            Assert.IsFalse(phrases[0].AppliesToReference(01001001));
            Assert.IsFalse(phrases[0].AppliesToReference(66022021));
        }

        [Test]
        public void AppliesToReference_QuestionDoesNotContainRef_ReturnsFalse()
        {
            var cat = m_sections.Items[0].Categories[0];

            cat.Questions.Add(new TestQ("Abc", "EXO 3:1-12", 02003001, 02003012, null));
            cat.Questions.Add(new TestQ("Xyz", "JUD 10-11", 65001010, 65001011, null));

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();
            Assert.AreEqual(2, phrases.Count);
            Assert.IsFalse(phrases[0].IsCategoryName);
            Assert.IsFalse(phrases[0].AppliesToReference(02002021));
            Assert.IsFalse(phrases[0].AppliesToReference(02003013));
            Assert.IsFalse(phrases[1].IsCategoryName);
            Assert.IsFalse(phrases[1].AppliesToReference(65001009));
            Assert.IsFalse(phrases[1].AppliesToReference(65001012));
        }

        [Test]
        public void AppliesToReference_QuestionDoesContainRef_ReturnsTrue()
        {
            var cat = m_sections.Items[0].Categories[0];

            cat.Questions.Add(new TestQ("Abc", "EXO 3:1-12", 02003001, 02003012, null));
            cat.Questions.Add(new TestQ("Xyz", "JUD 10-11", 65001010, 65001011, null));

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();
            Assert.AreEqual(2, phrases.Count);
            Assert.IsFalse(phrases[0].IsCategoryName);
            Assert.IsTrue(phrases[0].AppliesToReference(02003001));
            Assert.IsTrue(phrases[0].AppliesToReference(02003009));
            Assert.IsTrue(phrases[0].AppliesToReference(02003012));
            Assert.IsFalse(phrases[1].IsCategoryName);
            Assert.IsTrue(phrases[1].AppliesToReference(65001010));
            Assert.IsTrue(phrases[1].AppliesToReference(65001011));
        }
        #endregion

        #region PartPatternMatches tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the PartPatternMatches method with phrases that consist of a single part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void PartPatternMatches_SinglePart()
        {
            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "Wuzzee?", "wuzzee");
            AddTestQuestion(cat, "Wuzzee!", "wuzzee");
            AddTestQuestion(cat, "As a man thinks in his heart, how is he?", "as a man thinks in his heart how is he");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            Assert.IsTrue(phrases[0].PartPatternMatches(phrases[1]));
            Assert.IsTrue(phrases[1].PartPatternMatches(phrases[0]));
            Assert.IsTrue(phrases[1].PartPatternMatches(phrases[1]));
            Assert.IsFalse(phrases[2].PartPatternMatches(phrases[1]));
            Assert.IsFalse(phrases[2].PartPatternMatches(phrases[0]));
            Assert.IsFalse(phrases[0].PartPatternMatches(phrases[2]));
            Assert.IsFalse(phrases[1].PartPatternMatches(phrases[2]));
            Assert.IsTrue(phrases[2].PartPatternMatches(phrases[2]));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the PartPatternMatches method with phrases that consist of multiple parts.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void PartPatternMatches_OneTranslatablePartOneKeyTerm()
        {
            AddMockedKeyTerm("wunkers");
            AddMockedKeyTerm("punkers");

            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
            AddTestQuestion(cat, "Wuzzee punkers.", "wuzzee", "kt:punkers");
            AddTestQuestion(cat, "Wuzzee wunkers!", "wuzzee", "kt:wunkers");
            AddTestQuestion(cat, "Wunkers wuzzee!", "kt:wunkers", "wuzzee");
            AddTestQuestion(cat, "A dude named punkers?", "a dude named", "kt:punkers");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            Assert.IsTrue(phrases[0].PartPatternMatches(phrases[1]));
            Assert.IsTrue(phrases[0].PartPatternMatches(phrases[2]));
            Assert.IsFalse(phrases[0].PartPatternMatches(phrases[3]));
            Assert.IsFalse(phrases[0].PartPatternMatches(phrases[4]));

            Assert.IsTrue(phrases[1].PartPatternMatches(phrases[2]));
            Assert.IsFalse(phrases[1].PartPatternMatches(phrases[3]));
            Assert.IsFalse(phrases[1].PartPatternMatches(phrases[4]));

            Assert.IsFalse(phrases[2].PartPatternMatches(phrases[3]));
            Assert.IsFalse(phrases[2].PartPatternMatches(phrases[4]));

            Assert.IsFalse(phrases[3].PartPatternMatches(phrases[4]));
        }
        #endregion
        
        #region FindTermRenderingInUse tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void TestFindTermRenderingInUse_Present()
        {
            IKeyTerm ktGod = AddMockedKeyTerm("God", "Dios");
            IKeyTerm ktPaul = AddMockedKeyTerm("Paul", "paulo", "Pablo", "luaP");
            IKeyTerm ktHave = AddMockedKeyTerm("have", "tenemos");
            IKeyTerm ktSay = AddMockedKeyTerm("say", "dice");

            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "What did God tell Paul?",
                "what did", "kt:god", "tell", "kt:paul");
            AddTestQuestion(cat, "What does Paul say we have to give to God?",
                "what does", "kt:paul", "kt:say", "we", "kt:have", "to give to", "kt:god");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            TranslatablePhrase phrase1 = phrases[0];
            TranslatablePhrase phrase2 = phrases[1];

            phrase1.Translation = "\u00BFQue\u0301 le dijo Dios a Pablo?";
            phrase2.Translation = "\u00BFQue\u0301 dice luaP que tenemos que dar a Dios?";

            SubstringDescriptor sd;

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, 0));
            Assert.AreEqual(13, sd.Start);
            Assert.AreEqual(4, sd.Length);

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, 0));
            Assert.AreEqual(20, sd.Start);
            Assert.AreEqual(5, sd.Length);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, 0));
            Assert.AreEqual(10, sd.Start);
            Assert.AreEqual(4, sd.Length);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktSay, 0));
            Assert.AreEqual(5, sd.Start);
            Assert.AreEqual(4, sd.Length);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktHave, 0));
            Assert.AreEqual(19, sd.Start);
            Assert.AreEqual(7, sd.Length);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, 0));
            Assert.AreEqual(37, sd.Start);
            Assert.AreEqual(4, sd.Length);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void TestFindTermRenderingInUse_SomeMissing()
        {
            IKeyTerm ktGod = AddMockedKeyTerm("God", "Dios");
            IKeyTerm ktPaul = AddMockedKeyTerm("Paul", "Pablo");
            IKeyTerm ktHave = AddMockedKeyTerm("have", "tenemos");
            IKeyTerm ktSay = AddMockedKeyTerm("say", "dice");

            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "What did God tell Paul?",
                "what did", "kt:god", "tell", "kt:paul");
            AddTestQuestion(cat, "What does Paul say we have to give to God?",
                "what does", "kt:paul", "kt:say", "we", "kt:have", "to give to", "kt:god");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            TranslatablePhrase phrase1 = phrases[0];
            TranslatablePhrase phrase2 = phrases[1];

            phrase1.Translation = "\u00BFQue\u0301 le dijo Jehovah a Pablo?";
            phrase2.Translation = "Pi\u0301dale ayuda a Bill.";

            SubstringDescriptor sd;

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, 0));
            Assert.IsNull(sd);

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, 0));
            Assert.AreEqual(23, sd.Start);
            Assert.AreEqual(5, sd.Length);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, 0));
            Assert.IsNull(sd);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktSay, 0));
            Assert.IsNull(sd);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktHave, 0));
            Assert.IsNull(sd);

            sd = phrase2.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, 0));
            Assert.IsNull(sd);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void TestFindTermRenderingInUse_RepeatedTerms()
        {
            IKeyTerm ktGod = AddMockedKeyTerm("God", "Dios");
            IKeyTerm ktPaul = AddMockedKeyTerm("Paul", "Pablo");

            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "What did God tell Paul?/What was Paul told by God?",
                "what did", "kt:god", "tell", "kt:paul", "what was", "kt:paul", "told by", "kt:god");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            TranslatablePhrase phrase1 = phrases[0];

            phrase1.Translation = "\u00BFQue\u0301 le dijo Dios a Pablo?/\u00BFQue\u0301 le fue dicho a Pablo por Dios?";

            SubstringDescriptor sd;

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, 0));
            Assert.AreEqual(13, sd.Start);
            Assert.AreEqual(4, sd.Length);
            int endOfLastOccurrenceOfGod = sd.EndOffset;

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, 0));
            Assert.AreEqual(20, sd.Start);
            Assert.AreEqual(5, sd.Length);
            int endOfLastOccurrenceOfPaul = sd.EndOffset;

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktGod, endOfLastOccurrenceOfGod));
            Assert.AreEqual(57, sd.Start);
            Assert.AreEqual(4, sd.Length);

            sd = phrase1.FindTermRenderingInUse(new DummyKeyTermRenderingInfo(ktPaul, endOfLastOccurrenceOfPaul));
            Assert.AreEqual(47, sd.Start);
            Assert.AreEqual(5, sd.Length);
        }
        #endregion

        #region UserTransSansOuterPunctuation tests
        [Test]
        public void UserTransSansOuterPunctuation_LeadingPunctuation_GetsRemoved()
        {
            TranslatablePhrase p = new TranslatablePhrase("That is so cool!");
            p.Translation = "!Cool is so that";
            Assert.AreEqual("Cool is so that", p.UserTransSansOuterPunctuation);
        }

        [Test]
        public void UserTransSansOuterPunctuation_TrailingPunctuation_GetsRemoved()
        {
            TranslatablePhrase p = new TranslatablePhrase("That is so cool!");
            p.Translation = "Cool is so that!";
            Assert.AreEqual("Cool is so that", p.UserTransSansOuterPunctuation);
        }

        [Test]
        public void UserTransSansOuterPunctuation_MultiplePunctuationCharacters_GetsRemoved()
        {
            TranslatablePhrase p = new TranslatablePhrase("That is so cool!");
            p.Translation = "\".Cool is so that!?";
            Assert.AreEqual("Cool is so that", p.UserTransSansOuterPunctuation);
        }
        #endregion

        #region Translation tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_UserTranslation_IsComposed()
        {
            var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "What did God tell Paul?", "what did god tell paul");

            var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

            TranslatablePhrase phrase1 = phrases[0];

            phrase1.Translation = "\u00BFQue\u0301 le dijo Dios a Pablo?";

            Assert.AreEqual("\u00BFQue\u0301 le dijo Dios a Pablo?".Normalize(NormalizationForm.FormC),
                phrase1.Translation);
            Assert.IsTrue(phrase1.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetTranslation_QuestionWithNoUserTranslation_GetsStringWithInitialAndFinalPunctuation()
        {
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.Question)).Return(";");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.Question)).Return("?");
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return("");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return(".");

            TranslatablePhrase phrase1 = new TranslatablePhrase("Why don't I have a translation?");

            Assert.AreEqual(";?", phrase1.Translation);
            Assert.IsFalse(phrase1.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetTranslation_StatementWithNoUserTranslation_GetsStringWithInitialAndFinalPunctuation()
        {
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.Question)).Return(";");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.Question)).Return("?");
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return("");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return(".");

            TranslatablePhrase phrase1 = new TranslatablePhrase("I don't have a translation.");

            Assert.AreEqual(".", phrase1.Translation);
            Assert.IsFalse(phrase1.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests finding renderings of key terms when there is exactly one occurrence of each
        /// term in a particular phrase and one of the renderings is present in the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetTranslation_UnknownWithNoUserTranslation_GetsStringWithInitialAndFinalPunctuation()
        {
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.Question)).Return(";");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.Question)).Return("?");
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return("");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.StatementOrImperative)).Return(".");
            TranslatablePhrase.s_helper.Stub(h => h.InitialPunctuationForType(TypeOfPhrase.Unknown)).Return("-");
            TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.Unknown)).Return("-");

            TranslatablePhrase phrase1 = new TranslatablePhrase("-OR-");

            Assert.AreEqual("--", phrase1.Translation);
            Assert.IsFalse(phrase1.HasUserTranslation);
            
            TranslatablePhrase phrase2 = new TranslatablePhrase("Oops, I forgot the puctuation");

            Assert.AreEqual("--", phrase2.Translation);
            Assert.IsFalse(phrase1.HasUserTranslation);
        }
        #endregion

		#region InsertKeyTermRendering tests
		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRendering_InsertsNewRenderingBeforeFinalPunctuation()
		{
			TranslatablePhrase.s_helper.Stub(h => h.FinalPunctuationForType(TypeOfPhrase.Question)).Return("?");

			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			Assert.AreEqual("best?", phrases[0].Translation);
			phrases[0].Translation = "Why is the term missing?";
			Assert.AreEqual("Why is the term missing?", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "fair"));
			Assert.AreEqual("Why is the term missing fair?", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRenderingNoFinalPunctuation_InsertsNewRenderingAtEndWithSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			Assert.AreEqual("best", phrases[0].Translation);
			phrases[0].Translation = "Why is the term missing";
			Assert.AreEqual("Why is the term missing", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "pretty good"));
			Assert.AreEqual("Why is the term missing pretty good", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_TranslationEndsWithSpaceAndHasNoExistingRendering_InsertsNewRenderingAtEnd()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			Assert.AreEqual("best", phrases[0].Translation);
			phrases[0].Translation = "Why is the term missing ";
			Assert.AreEqual("Why is the term missing ", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "best"));
			Assert.AreEqual("Why is the term missing best", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_TranslationHasExistingRendering_NewRenderingReplacesExistingRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			Assert.AreEqual("best", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "pretty good"));
			Assert.AreEqual("pretty good", phrases[0].Translation);
			phrases[0].Translation = "Is this a pretty good translation?";
			Assert.AreEqual("Is this a pretty good translation?", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "fair"));
			Assert.AreEqual("Is this a fair translation?", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_TranslationHasExistingRenderingsForTwoInstancesOfTerm_NewRenderingReplacesCorrectExistingRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(20);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers fuzzy wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Was this pretty good translation done by the best guy?";
			Assert.AreEqual("Was this pretty good translation done by the best guy?", phrases[0].Translation);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, null, "fair"));
			Assert.AreEqual("Was this pretty good translation done by the fair guy?", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_OnlyExistingRenderingSelected_NewRenderingReplacesExistingRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a pretty good translation?";
			Assert.AreEqual("Is this a pretty good translation?", phrases[0].Translation);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, new SubstringDescriptor(10, 11), "fair"));
			Assert.AreEqual("Is this a fair translation?", phrases[0].Translation);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_ExistingPartialWordRenderingSelected_NewRenderingReplacesExistingRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a fairly good translation?";
			Assert.AreEqual("Is this a fairly good translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(10, 4);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "best"));
			Assert.AreEqual("Is this a bestly good translation?", phrases[0].Translation);
			Assert.AreEqual(10, sd.Start);
			Assert.AreEqual(4, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_ExistingWordRenderingPlusTrailingSpaceSelected_NewRenderingReplacesExistingRenderingRetainingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this the best translation?";
			Assert.AreEqual("Is this the best translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(12, 5);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this the pretty good translation?", phrases[0].Translation);
			Assert.AreEqual(12, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_ExistingWordRenderingPlusLeadingSpaceSelected_NewRenderingReplacesExistingRenderingRetainingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this the best translation?";
			Assert.AreEqual("Is this the best translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(11, 5);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this the pretty good translation?", phrases[0].Translation);
			Assert.AreEqual(12, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_ExistingWordRenderingPlusSurroundingSpaceSelected_NewRenderingReplacesExistingRenderingRetainingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this the best translation?";
			Assert.AreEqual("Is this the best translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(11, 6);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this the pretty good translation?", phrases[0].Translation);
			Assert.AreEqual(12, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRenderingOneSpaceSelected_NewRenderingReplacesSelectionWithSingleLeadingAndTrailingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a translation?";
			Assert.AreEqual("Is this a translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(9, 1);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this a pretty good translation?", phrases[0].Translation);
			Assert.AreEqual(10, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRenderingTwoSpacesSelected_NewRenderingReplacesSelectionRetainingSingleLeadingAndTrailingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a  translation?";
			Assert.AreEqual("Is this a  translation?", phrases[0].Translation);
			var sd = new SubstringDescriptor(9, 2);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this a pretty good translation?", phrases[0].Translation);
			Assert.AreEqual(10, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_SecondExistingRenderingSelected_NewRenderingReplacesSelectedRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a pretty good translation, or is it just fair?";
			Assert.AreEqual("Is this a pretty good translation, or is it just fair?", phrases[0].Translation);
			int ichStartOfFair = phrases[0].Translation.Length - 5;
			var sd = new SubstringDescriptor(ichStartOfFair, 4);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "best"));
			Assert.AreEqual("Is this a pretty good translation, or is it just best?", phrases[0].Translation);
			Assert.AreEqual(ichStartOfFair, sd.Start);
			Assert.AreEqual("best".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRenderingEditedInsertionPointAfterWord_NewRenderingInsertedAtIPWithLeadingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a frog?";
			Assert.AreEqual("Is this a frog?", phrases[0].Translation);
			var sd = new SubstringDescriptor(9, 0);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this a pretty good frog?", phrases[0].Translation);
			Assert.AreEqual(10, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_NoExistingRenderingEditedInsertionPointBeforeWord_NewRenderingInsertedAtIPWithTrailingSpace()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this a frog?";
			Assert.AreEqual("Is this a frog?", phrases[0].Translation);
			var sd = new SubstringDescriptor(10, 0);
			Assert.IsTrue(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "pretty good"));
			Assert.AreEqual("Is this a pretty good frog?", phrases[0].Translation);
			Assert.AreEqual(10, sd.Start);
			Assert.AreEqual("pretty good".Length, sd.Length);
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void InsertKeyTermRendering_OnlyExistingRenderingNotSelected_NewRenderingReplacesExistingRendering()
		{
			var wunkers = AddMockedKeyTerm("wunkers", "sreknuw", "best", "pretty good", "fair");

			var renderingCtrl = MockRepository.GenerateMock<ITermRenderingInfo>();
			renderingCtrl.Stub(r => r.Renderings).Return(m_dummyKtRenderings[wunkers.Id]);
			renderingCtrl.Stub(r => r.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm).Return(0);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Wuzzee wunkers?", "wuzzee", "kt:wunkers");
			var phrases = (new QuestionProvider(GetParsedQuestions())).ToList();

			phrases[0].Translation = "Is this really best?";
			Assert.AreEqual("Is this really best?", phrases[0].Translation);
			var sd = new SubstringDescriptor(0, 2);
			Assert.IsFalse(phrases[0].InsertKeyTermRendering(renderingCtrl, sd, "fair"));
			Assert.AreEqual("Is this really fair?", phrases[0].Translation);
			// No change to sd since we didn't replace edited selection (InsertKeyTermRendering returns false)
			Assert.AreEqual(0, sd.Start);
			Assert.AreEqual(2, sd.Length);
		}
		#endregion

		#region Private helper methods
		/// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Adds a test question to the given category and adds info about key terms and parts
	    /// to dictionaries used by GetParsedQuestions. Note that items in the parts array will
	    /// be treated as translatable parts unless prefixed with "kt:", in which case they
	    /// will be treated as key terms (corresponding key terms must be added by calling
	    /// AddMockedKeyTerm.
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    private void AddTestQuestion(Category cat, string text, params string[] parts)
	    {
	        var q = new TestQ(text, "A", 1, 1, GetParsedParts(parts));
	        cat.Questions.Add(q);
	    }
	    #endregion
	}

    /// ----------------------------------------------------------------------------------------
    /// <summary>
    /// Dummy class so we don't have to use a real list box
    /// </summary>
    /// ----------------------------------------------------------------------------------------
    internal class DummyKeyTermRenderingInfo : ITermRenderingInfo
    {
        internal static Dictionary<string, List<string>> s_ktRenderings;

        #region Implementation of ITermRenderingInfo
        public IEnumerable<string> Renderings { get; private set; }

        public int EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm { get; set; }

        public DummyKeyTermRenderingInfo(IKeyTerm kt, int endOffsetOfPrev)
        {
            Renderings = s_ktRenderings[kt.Id];
            EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm = endOffsetOfPrev;
        }
        #endregion

    }
}
