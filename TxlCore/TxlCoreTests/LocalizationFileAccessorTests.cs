// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: LocalizationFileAccessorTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SIL.TestUtilities;

namespace SIL.Transcelerator.Localization
{
	[TestFixture]
	public class LocalizationFileAccessorTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void GetLocalizedString_NoMatchingCategoryWithFailover_ReturnsEnglish(bool includeSomeOtherCategory)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeSomeOtherCategory)
				sut.AddLocalizationEntry(new UIDataString("Overview", LocalizableStringType.Category));
			Assert.AreEqual("Details", sut.GetLocalizedString(new UIDataString("Details", LocalizableStringType.Category)));
		}

		[Test]
		public void GetLocalizedString_NoMatchingCategoryNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("Details", LocalizableStringType.Category), false));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetLocalizedString_NoMatchingSectionWithFailover_ReturnsEnglish(bool includeSomeOtherSection)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeSomeOtherSection)
				sut.AddLocalizationEntry(new UIDataString("Jude 3-13 Some section", LocalizableStringType.SectionHeading, "JUD 1.3-13", 65001003, 65001013));
			Assert.AreEqual("Acts 3:1-15 This is Bryan's birthday.",
				sut.GetLocalizedString(new UIDataString("Acts 3:1-15 This is Bryan's birthday.", LocalizableStringType.SectionHeading, "ACT 3.1-15", 44003001, 44003015)));
		}

		[Test]
		public void GetLocalizedString_NoMatchingSectionNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("Rev 9:2-14 More stuff that's going to happen", LocalizableStringType.SectionHeading, "REV 9.2-14", 66009002, 66009014), false));
		}

		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, false)]
		public void GetLocalizedString_NoMatchingQuestionWithFailover_ReturnsEnglish(bool includeSection, bool includeSomeOtherQuestion)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeSection)
			{
				sut.AddLocalizationEntry(new UIDataString("Matthew 3:1-20 Some section", LocalizableStringType.SectionHeading, "MAT 3.1-20", 40003001, 40003020));
				if (includeSomeOtherQuestion)
					sut.AddLocalizationEntry(new UIDataString("Is this really a detail question?", LocalizableStringType.Question, "MAT 3.19", 40003019, 40003019));
			}
			Assert.AreEqual("What are we testing here?",
				sut.GetLocalizedString(new UIDataString("What are we testing here?", LocalizableStringType.Question, "MAT 3.2", 40003002, 40003002)));
		}

		[Test]
		public void GetLocalizedString_NoMatchingQuestionNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("What are we testing here?", LocalizableStringType.Question, "MAT 3.2", 40003002, 40003002), false));
		}

		[TestCase(LocalizableStringType.Answer, false, false)]
		[TestCase(LocalizableStringType.Answer, true, false)]
		[TestCase(LocalizableStringType.Answer, true, true)]
		[TestCase(LocalizableStringType.Alternate, false, false)]
		[TestCase(LocalizableStringType.Alternate, true, false)]
		[TestCase(LocalizableStringType.Alternate, true, true)]
		[TestCase(LocalizableStringType.Note, false, false)]
		[TestCase(LocalizableStringType.Note, true, false)]
		[TestCase(LocalizableStringType.Note, true, true)]
		public void GetLocalizedString_NoMatchingEntryWithFailover_ReturnsEnglish(LocalizableStringType type, bool includeBaseQuestion, bool includeOtherEntry)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeBaseQuestion)
			{
				sut.AddLocalizationEntry(new UIDataString("Matthew 3:1-20 Some section", LocalizableStringType.SectionHeading, "MAT 3.1-20", 40003001, 40003020));
				sut.AddLocalizationEntry(new UIDataString("Do you like this base question?", LocalizableStringType.Question, "MAT 3.2-3", 40003002, 40003003));
				if (includeOtherEntry)
					sut.AddLocalizationEntry(new UIDataString("This is not the right thingy.", type, "MAT 3.2-3", 40003002, 40003003, "Do you like this base question?"));
			}
			Assert.AreEqual("This is a whatchamacallit.",
				sut.GetLocalizedString(new UIDataString("This is a whatchamacallit.", type, "MAT 3.2-3", 40003002, 40003003, "Do you like this base question?")));
		}

		[TestCase(LocalizableStringType.Answer)]
		[TestCase(LocalizableStringType.Alternate)]
		[TestCase(LocalizableStringType.Note)]
		public void GetLocalizedString_NoMatchingEntryNoFailover_ReturnsNull(LocalizableStringType type)
		{
			var sut = new TestLocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizedString(
				new UIDataString("This is a whatchamacallit.", type, "MAT 3.2-3", 40003002, 40003003, "Do you like this base question?"), false));
		}

		[Test]
		public void GetLocalizedString_MatchingCategoryNotLocalized_ReturnsEnglish()
		{
			var key = new UIDataString("Details", LocalizableStringType.Category);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(key);
			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingSectionNotLocalized_ReturnsEnglish()
		{
			var key = new UIDataString("Rev 9:2-14 More stuff that's going to happen", LocalizableStringType.SectionHeading, "REV 9.2-14", 66009002, 66009014);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(key);
			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingQuestionNotLocalized_ReturnsEnglish()
		{
			var key = new UIDataString("Do you like this base question?", LocalizableStringType.Question, "MAT 3.2-3", 40003002, 40003003);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UIDataString("Matthew 3:1-20 Some section", LocalizableStringType.SectionHeading, "MAT 3.1-20", 40003001, 40003020));

			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[TestCase(LocalizableStringType.Answer)]
		[TestCase(LocalizableStringType.Alternate)]
		[TestCase(LocalizableStringType.Note)]
		public void GetLocalizedString_MatchingEntryNotLocalized_ReturnsEnglish(LocalizableStringType type)
		{
			var key = new UIDataString("This is a whatchamacallit.", type, "MAT 3.2-3", 40003002, 40003003, "Do you like this base question?");
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UIDataString("Matthew 3:1-20 Some section", LocalizableStringType.SectionHeading, "MAT 3.1-20", 40003001, 40003020));
			sut.AddLocalizationEntry(new UIDataString("Do you like this base question?", LocalizableStringType.Question, "MAT 3.2-3", 40003002, 40003003));
			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizableStringInfo_AccessorHasNoEntries_ReturnsNullEntry()
		{
			var sut = new TestLocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString("This is some text for which no localization entry has been added.", LocalizableStringType.Question, "EXO 2.7", 002002007, 002002007)));
		}

		[Test]
		public void GetLocalizableStringInfo_AddedQuestionMatchesExistingWithDifferentEndRef_GetsExisting()
		{
			// I'm not 100% sure this is even possible to set up via TXL's UI.
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UIDataString("Dummy section head", LocalizableStringType.SectionHeading, "GEN 1.1-10", 001001001, 001001010));
			var unmodifiedKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", null);
			sut.AddLocalizationEntry(new UIDataString(unmodifiedKey, LocalizableStringType.Question), "¿Qué hizo él?");
			var altFormKey = new Question("Gen 1.6", 001001006, 001001007, "What did he do?", null) { ModifiedPhrase = "What did he do?" };
			Assert.AreEqual(sut.GetLocalizableStringInfo(new UIDataString(unmodifiedKey, LocalizableStringType.Question)), 
				sut.GetLocalizableStringInfo(new UIDataString(altFormKey, LocalizableStringType.Question)));
		}

		[TestCase(true, LocalizableStringType.Question)]
		[TestCase(false, LocalizableStringType.Question)]
		[TestCase(true, LocalizableStringType.Alternate)]
		[TestCase(false, LocalizableStringType.Alternate)]
		public void GetLocalizedString_ModifiedQuestionWithNoExistingTranslationForBaseQuestionOrAlternates_ReturnsModifiedForm(bool callGenerateOrUpdateFromMasterQuestions,
			LocalizableStringType questionOrAlternate)
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			if (callGenerateOrUpdateFromMasterQuestions)
				sut.GenerateOrUpdateFromMasterQuestions(qs);

			// If the user has customized the question in a way that does not match any of the known alternate forms (for any question)
			// and none of the alternate forms for the question have been localized, retrieving the localized form will just return the unlocalized
			// custom phrase in use.
			var lastQuestion = qs.Items.Last().Categories.Last().Questions.Last();
			lastQuestion.ModifiedPhrase = "This is also gobbledy-gook, wouldn't you say?";
			var key = new UIDataString(lastQuestion, questionOrAlternate, lastQuestion.ModifiedPhrase);
			Assert.AreEqual("This is also gobbledy-gook, wouldn't you say?", sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_OnlyAlternateFormsHaveLocalizations_BestLocalizationIsReturnedForEachFormOfQuestion()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se puede definir la sabiduría?"},
				new XmlTranslation {PhraseKey = "What is meant by \"wisdom?\"", Reference = "PRO 3.1-35", Translation = "¿Qué significa la palabra \"sabiduría?\""},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			// First check that the correct localized form of each alternate is returned.
			var firstQuestion = qs.Items[0].Categories[0].Questions.First();
			var key = new UIDataString(firstQuestion, LocalizableStringType.Alternate, "How would you define wisdom?");
			Assert.AreEqual("¿Cómo se puede definir la sabiduría?", sut.GetLocalizedString(key));
			key = new UIDataString(firstQuestion, LocalizableStringType.Alternate, "What is meant by \"wisdom?\"");
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));

			// Next check that the first* localized alternate is returned for the base question (which is not itself localized).
			// * "first" in the order the alternate occurs in the question, not in the order in the list above.
			key = new UIDataString(firstQuestion, LocalizableStringType.Question);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));

			// Finally check that if the user has customized the question in a way that does not match any of the known forms,
			// retrieving the localized form will still get the localized form of the first localized alternate.
			firstQuestion.ModifiedPhrase = "This is pure gobbledy-gook, isn't it?";
			key = new UIDataString(firstQuestion, LocalizableStringType.Question);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));
			// And for good measure, is UseAnyAlternate is false, then it will just return the unlocalized modified version.
			key.UseAnyAlternate = false;
			Assert.AreEqual("This is pure gobbledy-gook, isn't it?", sut.GetLocalizedString(key));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWhenNoEntriesHaveLocalizations_NoChangesAndNoErrors()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

			VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWhenSomeEntriesHaveLocalizations_NoChangesAndNoErrors()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.AddLocalizationEntry(new UIDataString("Details", LocalizableStringType.Category), "Detalles");
			sut.AddLocalizationEntry(new UIDataString("Overview", LocalizableStringType.Category), "Resumen");
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			sut.AddLocalizationEntry(new UIDataString(firstOverviewQuestion, LocalizableStringType.Question), "¿Qué es la sabidurría?");
			sut.AddLocalizationEntry(new UIDataString(firstOverviewQuestion, LocalizableStringType.Answer, "A gift from God"), "Un don de Dios");
			sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabidurría?", "Un don de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWithAddedAndModifiedTxlTranslations_CorrectEntriesHaveCorrectLocalizations()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));

			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.AddRange(new[]
			{
				// Any modified TXL translations will be ignored. The XLIFF version "wins".
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se defina la sabidurría?"},
				new XmlTranslation {PhraseKey = "Riches", Reference = "MAT 6.19", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.6", Translation = "Joyas"},
			});
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "Jewels", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Riquezas", "Un don de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_MasterFileHasBlanks_BlanksAreEliminated()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 3);
			int iC = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What is wisdom?";
			q.Answers = new[] { "Smartness on steroids", "" };
			q.AlternateForms = new[] { "", null, "How would you define wisdom?" };

			iC = 1;
			int iQ = 0;
			q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.StartRef = 20003012;
			q.EndRef = 20003014;
			q.ScriptureReference = "PRO 3.12-14";
			q.Text = "How many words are there?";
			q.Answers = new[] { null, "A bunch" };
			q.Notes = new[] { "Exact answer is: Five", null };

			var sut = new TestLocalizationsFileAccessor();
			sut.GenerateOrUpdateFromMasterQuestions(qs);
			var question = qs.Items[0].Categories[0].Questions[0];
			var key = new UIDataString(question, LocalizableStringType.Answer, "Smartness on steroids");
			Assert.AreEqual("Smartness on steroids", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Answer).Count);

			key = new UIDataString(question, LocalizableStringType.Alternate, "How would you define wisdom?");
			Assert.AreEqual("How would you define wisdom?", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Alternate).Count);

			question = qs.Items[0].Categories[1].Questions[0];
			key = new UIDataString(question, LocalizableStringType.Answer, "A bunch");
			Assert.AreEqual("A bunch", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Answer).Count);

			key = new UIDataString(question, LocalizableStringType.Note, "Exact answer is: Five");
			Assert.AreEqual("Exact answer is: Five", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Note).Count);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithNoExistingTranslations_AllEntriesCreatedWithoutLocalizations(bool includeAlternates)
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(includeAlternates);
			sut.GenerateOrUpdateFromMasterQuestions(qs);

			VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithTxlTranslations_CorrectEntriesHaveLocalizations()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
				new XmlTranslation {PhraseKey = "Riches", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Riquezas", "Un regalo de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_OnlyAlternateFormsHaveTxlTranslations_StoredLocalizationsAreForAlternates()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se puede definir la sabiduría?"},
				new XmlTranslation {PhraseKey = "What is meant by \"wisdom?\"", Reference = "PRO 3.1-35", Translation = "¿Qué significa la palabra \"sabiduría?\""},
				new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.AreEqual(4, results.Count); // One of the alternate forms is used for two different questions.
			Assert.IsTrue(results.Select(k => k.SourceUIString).All(s => existingTxlTranslations.Select(e => e.PhraseKey).Contains(s)));
			Assert.IsTrue(results.All(k => k.Type == LocalizableStringType.Alternate));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_TxlTranslationsOfQuestionsAndAlternates_CorrectEntriesHaveLocalizations()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
				new XmlTranslation {PhraseKey = "How much verbiage exists?", Reference = "PRO 3.12-14", Translation = "¿Cuánta palabraría existe?"}, // Should be ignored
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es sabiduría?"},
				new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo definiría usted la palabra \"sabiduría\"?"},
				new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
				new XmlTranslation {PhraseKey = "What person is happy?", Reference = "PRO 3.13", Translation = "¿Qué persona es felíz?"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs, true).ToList();
			Assert.AreEqual(10, results.Count);
			// The "Question" property is the unmodified original question.
			Assert.IsTrue(results.Skip(1).Select(k => k.Question).SequenceEqual(
				new[] { "What is wisdom?", "What is wisdom?", "What is wisdom?",
					"What man is happy?", "What man is happy?", "What man is happy?", "What man is happy?",
					"What pictures describe wisdom?", "What pictures describe wisdom?" }));
			// The "SourceUIString" property is the modified (alternate) form of the question.
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Details",
					"What is wisdom?", "What is meant by \"wisdom?\"", "How would you define wisdom?",
					"What man is happy?", "What person is happy?", "Who is happy?", "What kind of person is blessed?",
					"What pictures describe wisdom?", "How would you define wisdom?"}));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Detalles",
					"¿Qué es sabiduría?", "¿Qué es sabiduría?", "¿Cómo definiría usted la palabra \"sabiduría\"?",
					"¿Qué persona es felíz?", "¿Qué persona es felíz?", "¿Qué persona es felíz?", "¿Qué tipo de persona es bendecida?",
					"¿Cómo definiría usted la palabra \"sabiduría\"?", "¿Cómo definiría usted la palabra \"sabiduría\"?"
				}));

			// Finally, check that the translation of an unknown form passed in as an existing TXL translation was truly ignored.
			var questionAboutWordCount = qs.Items.Last().Categories.Last().Questions[0];
			questionAboutWordCount.ModifiedPhrase = "How much verbiage exists?";
			var key = new UIDataString(questionAboutWordCount, LocalizableStringType.Question);
			Assert.AreEqual("How much verbiage exists?", sut.GetLocalizedString(key));
			key = new UIDataString(questionAboutWordCount, LocalizableStringType.Alternate, questionAboutWordCount.ModifiedPhrase);
			Assert.AreEqual("How much verbiage exists?", sut.GetLocalizedString(key));
		}

		[Test]
		public void SaveAndLoad_NoAlternates_NoLoss()
		{
			using (var folder = new TemporaryFolder("SaveAndLoad_NoAlternates_NoLoss"))
			{
				var accessorToSave = new LocalizationsFileAccessor(folder.Path, "es");
				var qs = GenerateProverbsQuestionSections();
				var existingTxlTranslations = new List<XmlTranslation>
				{
					new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
					new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
					new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
					new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
				};
				accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
				accessorToSave.Save();

				var accessorToLoad = new LocalizationsFileAccessor(folder.Path, "es");
				var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
				Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
					new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
				Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
					new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));
			}
		}

		[Test]
		public void SaveAndLoad_Alternates_NoLoss()
		{
			using (var folder = new TemporaryFolder("SaveAndLoad_Alternates_NoLoss"))
			{
				var accessorToSave = new LocalizationsFileAccessor(folder.Path, "es");
				var qs = GenerateProverbsQuestionSections(true);
				var existingTxlTranslations = new List<XmlTranslation>
				{
					new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
					new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
					new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
					new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
					new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
				};
				accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
				accessorToSave.Save();

				var accessorToLoad = new LocalizationsFileAccessor(folder.Path, "es");
				var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
				Assert.AreEqual(6, results.Count);
				Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
					new[] { "Overview",
						"What is wisdom?",
						"A gift from God",
						"What kind of person is blessed?",
						"Jewels",
						"A gift from God" }));
				Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
					new[] { "Resumen",
						"¿Qué es la sabidurría?",
						"Un don de Dios",
						"¿Qué tipo de persona es bendecida?",
						"Perlas preciosas",
						"Un don de Dios" }));
			}
		}

		private static QuestionSections GenerateProverbsQuestionSections(bool includeAlternatives = false)
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 3);
			int iC = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What is wisdom?";
			q.Answers = new[] { "Smartness on steroids", "A gift from God" };
			if (includeAlternatives)
			{
				q.AlternateForms = new[] { "What is meant by \"wisdom?\"", "How would you define wisdom?" };
			}

			iC = 1;
			int iQ = 0;
			q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.StartRef = 20003012;
			q.EndRef = 20003014;
			q.ScriptureReference = "PRO 3.12-14";
			q.Text = "How many words are there?";
			q.Answers = new[] { "A bunch" };
			q.Notes = new[] { "Exact answer is: Five", "In some cultures, it may be taboo to count words." };

			q = qs.Items[iS].Categories[iC].Questions[++iQ];
			q.StartRef = 20003013;
			q.EndRef = 20003013;
			q.ScriptureReference = "PRO 3.13";
			q.Text = "What man is happy?";
			q.Notes = new[] { "Make sure respondent understands the differnece between temporal happiness and deep blessing." };
			if (includeAlternatives)
			{
				q.AlternateForms = new[] { "What person is happy?", "Who is happy?", "What kind of person is blessed?" };
			}

			q = qs.Items[iS].Categories[iC].Questions[++iQ];
			q.StartRef = 20003015;
			q.EndRef = 20003016;
			q.ScriptureReference = "PRO 3.15-16";
			q.Text = "What pictures describe wisdom?";
			q.Answers = new[] { "Jewels", "Life", "Riches", "A gift from God" };
			if (includeAlternatives)
			{
				q.AlternateForms = new[] { "How would you define wisdom?" };
			}

			return qs;
		}

		/// ------------------------------------------------------------------------------------
		internal static Section CreateSection(string sRef, string heading, int startRef, int endRef, int cOverviewQuestions, int cDetailQuestions)
		{
			Section s = new Section();
			s.ScriptureReference = sRef;
			s.Heading = heading;
			s.StartRef = startRef;
			s.EndRef = endRef;
			s.Categories = new Category[(cOverviewQuestions > 0 ? 1 : 0) + (cDetailQuestions > 0 ? 1 : 0)];
			int iC = 0;
			if (cOverviewQuestions > 0)
			{
				s.Categories[iC] = new Category();
				s.Categories[iC].Type = "Overview";
				s.Categories[iC].IsOverview = true;
				for (int i = 0; i < cOverviewQuestions; i++)
					s.Categories[iC].Questions.Add(new Question());
				iC++;
			}

			if (cDetailQuestions > 0)
			{
				s.Categories[iC] = new Category();
				s.Categories[iC].Type = "Details";
				for (int i = 0; i < cDetailQuestions; i++)
					s.Categories[iC].Questions.Add(new Question());
			}
			return s;
		}

		private void VerifyAllEntriesExistWithNoLocalizedStrings(LocalizationsFileAccessor sut, QuestionSections qs)
		{
			Assert.AreEqual("Overview", sut.GetLocalizedString(new UIDataString("Overview", LocalizableStringType.Category)));
			Assert.AreEqual("Details", sut.GetLocalizedString(new UIDataString("Details", LocalizableStringType.Category)));

			foreach (var section in qs.Items)
			{
				Assert.AreEqual(section.Heading, sut.GetLocalizedString(
					new UIDataString(section.Heading, LocalizableStringType.SectionHeading, section.ScriptureReference, section.StartRef, section.EndRef)));

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					var key = new UIDataString(question, LocalizableStringType.Question);
					Assert.AreEqual(question.PhraseInUse, sut.GetLocalizedString(key));

					VerifyNotLocalized(sut, question, question.AlternateForms, LocalizableStringType.Alternate);
					VerifyNotLocalized(sut, question, question.Answers, LocalizableStringType.Answer);
					VerifyNotLocalized(sut, question, question.Notes, LocalizableStringType.Note);
				}
			}
		}

		private void VerifyNotLocalized(LocalizationsFileAccessor sut, Question question, string[] subStrings, LocalizableStringType type)
		{
			if (subStrings != null)
			{
				foreach (var s in subStrings)
					Assert.AreEqual(s, sut.GetLocalizedString(new UIDataString(question, type, s)));
			}
		}

		private IEnumerable<UIDataString> VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(LocalizationsFileAccessor sut, QuestionSections qs, bool useAnyAlternate = false)
		{
			var key = new UIDataString("Overview", LocalizableStringType.Category);
			var localizedString = sut.GetLocalizedString(key);
			if (localizedString != key.SourceUIString)
				yield return key;
			key = new UIDataString("Details", LocalizableStringType.Category);
			localizedString = sut.GetLocalizedString(key);
			if (localizedString != key.SourceUIString)
				yield return key;

			foreach (var section in qs.Items)
			{
				key = new UIDataString(section.Heading, LocalizableStringType.SectionHeading, section.ScriptureReference, section.StartRef, section.EndRef);
				localizedString = sut.GetLocalizedString(key);
				if (localizedString != key.SourceUIString)
					yield return key;

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					key = new UIDataString(question, LocalizableStringType.Question) {UseAnyAlternate = useAnyAlternate};
					if (sut.GetLocalizedString(key) != question.PhraseInUse)
						yield return key;

					if (question.AlternateForms != null)
					{
						foreach (var alternateForm in question.AlternateForms)
						{
							key = new UIDataString(question, LocalizableStringType.Alternate, alternateForm)
								{ UseAnyAlternate = useAnyAlternate };
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}

					if (question.Answers != null)
					{
						foreach (var answer in question.Answers)
						{
							key = new UIDataString(question, LocalizableStringType.Answer, answer);
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}
					if (question.Notes != null)
					{
						foreach (var comment in question.Notes)
						{
							key = new UIDataString(question, LocalizableStringType.Note, comment);
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}
				}
			}
		}
	}
}