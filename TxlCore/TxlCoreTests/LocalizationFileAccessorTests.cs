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
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("Details", LocalizableStringType.Category)));
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
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("Rev 9:2-14 More stuff that's going to happen", LocalizableStringType.SectionHeading, "REV 9.2-14", 66009002, 66009014)));
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
			Assert.IsNull(sut.GetLocalizedString(new UIDataString("What are we testing here?", LocalizableStringType.Question, "MAT 3.2", 40003002, 40003002)));
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
				new UIDataString("This is a whatchamacallit.", type, "MAT 3.2-3", 40003002, 40003003, "Do you like this base question?")));
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

		//[Test]
		//public void GetLocalizedString_MatchingEntryWithNoLocalization_ReturnsEnglish()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var q = new Question("Gen 1.1", 001001001, 001001001, "Some question", "Mary had a little lamb.");
		//	sut.AddLocalizationEntry(q, LocalizableStringType.Answer);
		//	Assert.AreEqual("Mary had a little lamb.", sut.GetLocalizedString(new UIDataString(q, q.Answers.Single(), LocalizableStringType.Answer)));
		//}

		//[Test]
		//public void GetLocalizedString_MatchingEntryWithNoOverridesForSpecificReference_ReturnsBaseLocalization()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var q = new Question("Gen 1.1", 001001001, 001001001, "What did he say?", "no answer");
		//	sut.AddLocalizationEntry(q, LocalizableStringType.Question, "¿Qué dijo?");
		//	Assert.AreEqual("¿Qué dijo?", sut.GetLocalizedString(new UIDataString(q, q.PhraseInUse, LocalizableStringType.Question)));
		//}

		//[TestCase(LocalizableStringType.Answer, "Mary had a little lamb.")]
		//[TestCase(LocalizableStringType.Question, "Why did he say that?")]
		//[TestCase(LocalizableStringType.Note, "If respondent is mute, he probably won't answer this question.")]
		//public void GetLocalizedStringInfo_MatchingEntryWithNoLocalization_ReturnsEnglish(LocalizableStringType type, string english)
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var key = new SimpleQuestionKey(english);
		//	sut.AddLocalizationEntry(key, type);
		//	var info = sut.GetLocalizableStringInfo(new UIDataString(key, english, type));
		//	Assert.AreEqual(english, info.English);
		//	Assert.AreEqual(type, info.Type);
		//	Assert.AreEqual(english, info.Target.Text);
		//}

		//[Test]
		//public void AddLocalizationEntry_ChangeLocalizationForDefaultLocation_DefaultSavedCorrectly()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	// Note: First one becomes the default (at least for now)
		//	var defaultKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo Dios?");
		//	var overrideKeyMat14_1 = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(overrideKeyMat14_1, LocalizableStringType.Question, "¿Qué hizo Jesús?");

		//	Assert.AreEqual("¿Qué hizo Dios?", sut.GetLocalizedString(defaultKey));
		//	// Now attempt to change the default.
		//	sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo?");
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(defaultKey));
		//	Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_1));
		//	// Ensure default changed
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(new Question("Mat 14.1-2", 040014001, 040014002, "What did he do?", "Not used in this test")));
		//}

		//[Test]
		//public void AddLocalizationEntry_ChangeLocalizationForAlternateForm_NewLocalizationSavedCorrectly()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	// Note: First one becomes the default (at least for now)
		//	var unmodifiedKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(unmodifiedKey, LocalizableStringType.Question, "¿Qué hizo él?");
		//	var altFormKey = new Question(unmodifiedKey, unmodifiedKey.Text, null) { ModifiedPhrase = "What was the action he preformed?" };
		//	sut.AddLocalizationEntry(altFormKey, LocalizableStringType.Question, "¿Qué acción realizó?");

		//	Assert.AreEqual("¿Qué hizo él?", sut.GetLocalizedString(unmodifiedKey));
		//	Assert.AreEqual("¿Qué acción realizó?", sut.GetLocalizedString(altFormKey));
		//	// Now attempt to change the alternate form localization.
		//	sut.AddLocalizationEntry(altFormKey, LocalizableStringType.Question, "¿Qué acción realizó él?");
		//	Assert.AreEqual("¿Qué hizo él?", sut.GetLocalizedString(unmodifiedKey));
		//	Assert.AreEqual("¿Qué acción realizó él?", sut.GetLocalizedString(altFormKey));
		//	// Now attempt to change the base form localization.
		//	sut.AddLocalizationEntry(unmodifiedKey, LocalizableStringType.Question, "¿Qué hizo?");
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(unmodifiedKey));
		//	Assert.AreEqual("¿Qué acción realizó él?", sut.GetLocalizedString(altFormKey));
		//}

		//[Test]
		//public void AddLocalizationEntry_ModifiedFormMatchesExisting_NewOccurrenceDoesNotReplaceExistingDefault()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var unmodifiedKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", null);
		//	sut.AddLocalizationEntry(unmodifiedKey, LocalizableStringType.Question, "¿Qué hizo él?");
		//	var altFormKey = new Question("Gen 1.20", 001001020, 001001020, "How would you describe what he did?", null) { ModifiedPhrase = "What did he do?" };
		//	sut.AddLocalizationEntry(altFormKey, LocalizableStringType.Question, "¿Qué hizo?");

		//	Assert.AreEqual("¿Qué hizo él?", sut.GetLocalizedString(unmodifiedKey));
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(altFormKey));
		//}

		//[Test]
		//public void AddLocalizationEntry_ModifiedFormDoesNotMatchesExisting_LocalizationAddedToCoverOriginalAndModifiedStrings()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var unmodifiedKey = new Question("Exo 1.21", 002001021, 001001021, "Describe his action.", null);
		//	var altFormKey = new Question(unmodifiedKey, unmodifiedKey.Text, null) { ModifiedPhrase = "What did he do?" };
		//	sut.AddLocalizationEntry(altFormKey, LocalizableStringType.Question, "¿Qué hizo?");

		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(altFormKey));
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(unmodifiedKey));
		//}

		//[Test]
		//public void AddLocalizationEntry_SaveSameLocalizationForMultipleLocations_AllVariantsSavedCorrectly()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	// Note: First one becomes the default (at least for now)
		//	var defaultKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo?");
		//	var overrideKeyMat14_1 = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(overrideKeyMat14_1, LocalizableStringType.Question, "¿Qué hizo Jesús?");
		//	var overrideKeyGen2_3 = new Question("Gen 2.3", 001002003, 001002003, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(overrideKeyGen2_3, LocalizableStringType.Question, "¿Qué hizo?");
		//	var overrideKeyMat14_3 = new Question("Mat 14.3", 040014003, 040014003, "What did he do?", "Not used in this test");
		//	sut.AddLocalizationEntry(overrideKeyMat14_3, LocalizableStringType.Question, "¿Qué hizo Jesús?");

		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(defaultKey));
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(overrideKeyGen2_3));
		//	Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_1));
		//	Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_3));
		//	// Ensure First one is the default
		//	Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(new Question("Mat 14.1-2", 040014001, 040014002, "What did he do?", "Not used in this test")));
		//}

		//[Test]
		//public void GetLocalizableStringInfo_AccessorHasNoEntries_ReturnsNullEntry()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString(new SimpleQuestionKey("This is some text for which no localization entry has been added."), "This is some text for which no localization entry has been added.", LocalizableStringType.Question)));
		//}

		//[Test]
		//public void GetLocalizableStringInfo_ModifiedFormMatchesExisting_GetsExisting()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var unmodifiedKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", null);
		//	sut.AddLocalizationEntry(unmodifiedKey, LocalizableStringType.Question, "¿Qué hizo él?");
		//	var altFormKey = new Question("Gen 1.20", 001001020, 001001020, "How would you describe what he did?", null) { ModifiedPhrase = "What did he do?" };
		//	Assert.AreEqual(sut.GetLocalizableStringInfo(new UIDataString(altFormKey, altFormKey.PhraseInUse, LocalizableStringType.Alternate), true),
		//		sut.GetLocalizableStringInfo(new UIDataString(unmodifiedKey, unmodifiedKey.Text, LocalizableStringType.Question)));
		//}

		//[TestCase(true)]
		//[TestCase(false)]
		//public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithNoExistingTranslations_AllEntriesCreatedWithoutLocalizations(bool includeAlternates)
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections(includeAlternates);
		//	sut.GenerateOrUpdateFromMasterQuestions(qs);

		//	VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithTxlTranslations_CorrectEntriesHaveLocalizations()
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections();
		//	var existingTxlTranslations = new List<XmlTranslation>
		//	{
		//		new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
		//		new XmlTranslation {PhraseKey = "Riches", Translation = "Riquezas"},
		//		new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
		//		new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
		//		new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
		//	};
		//	sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

		//	var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "Overview", "What is wisdom?", "A gift from God", "Riches", "A gift from God" }));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Riquezas", "Un regalo de Dios" }));
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_OnlyAlternateFormsHaveTxlTranslations_BestLocalizationIsReturnedForEachFormOfQuestion()
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections(true);
		//	var existingTxlTranslations = new List<XmlTranslation>
		//	{
		//		new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se puede definir la sabiduría?"},
		//		new XmlTranslation {PhraseKey = "What is meant by \"wisdom?\"", Reference = "PRO 3.1-35", Translation = "¿Qué significa la palabra \"sabiduría?\""},
		//		new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
		//	};
		//	sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

		//	var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.AreEqual(9, results.Count);
		//	// The "Text" property is the unmodified original question.
		//	Assert.IsTrue(results.Select(k => k.Text).SequenceEqual(
		//		new[] { "What is wisdom?", "What is wisdom?", "What is wisdom?",
		//			"What man is happy?", "What man is happy?", "What man is happy?", "What man is happy?",
		//			"What pictures describe wisdom?", "What pictures describe wisdom?" }));
		//	// The "PhraseInUse" property is the modified (alternate) form of the question.
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "What is wisdom?", "What is meant by \"wisdom?\"", "How would you define wisdom?",
		//			"What man is happy?", "What person is happy?", "Who is happy?", "What kind of person is blessed?",
		//		"What pictures describe wisdom?", "How would you define wisdom?"}));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "¿Qué significa la palabra \"sabiduría?\"", "¿Qué significa la palabra \"sabiduría?\"", "¿Cómo se puede definir la sabiduría?",
		//			"¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?",
		//			"¿Cómo se puede definir la sabiduría?", "¿Cómo se puede definir la sabiduría?"
		//		}));
		//	// Finally, check that if the user has customized the question in a way that does not match any of the known alternate forms,
		//	// retrieving the localized form will still get the localized form of the first localized alternate.
		//	var firstQuestion = qs.Items[0].Categories[0].Questions.First();
		//	var key = new Question(firstQuestion, firstQuestion.Text, null) {ModifiedPhrase = "This is pure gobbledy-gook, isn't it?"};
		//	Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_TxlTranslationsOfQuestionsAndAlternates_BestLocalizationIsReturnedForEachFormOfQuestion()
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections(true);
		//	var existingTxlTranslations = new List<XmlTranslation>
		//	{
		//		new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
		//		new XmlTranslation {PhraseKey = "How much verbiage exists?", Reference = "PRO 3.12-14", Translation = "¿Cuánta palabraría existe?"}, // Should be ignored
		//		new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es sabiduría?"},
		//		new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo definiría usted la palabra \"sabiduría\"?"},
		//		new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
		//		new XmlTranslation {PhraseKey = "What person is happy?", Reference = "PRO 3.13", Translation = "¿Qué persona es felíz?"},
		//	};
		//	sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

		//	var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.AreEqual(10, results.Count);
		//	// The "Text" property is the unmodified original question.
		//	Assert.IsTrue(results.Select(k => k.Text).SequenceEqual(
		//		new[] { "Details",
		//			"What is wisdom?", "What is wisdom?", "What is wisdom?",
		//			"What man is happy?", "What man is happy?", "What man is happy?", "What man is happy?",
		//			"What pictures describe wisdom?", "What pictures describe wisdom?" }));
		//	// The "PhraseInUse" property is the modified (alternate) form of the question.
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "Details",
		//			"What is wisdom?", "What is meant by \"wisdom?\"", "How would you define wisdom?",
		//			"What man is happy?", "What person is happy?", "Who is happy?", "What kind of person is blessed?",
		//			"What pictures describe wisdom?", "How would you define wisdom?"}));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "Detalles",
		//			"¿Qué es sabiduría?", "¿Qué es sabiduría?", "¿Cómo definiría usted la palabra \"sabiduría\"?",
		//			"¿Qué persona es felíz?", "¿Qué persona es felíz?", "¿Qué persona es felíz?", "¿Qué tipo de persona es bendecida?",
		//			"¿Cómo definiría usted la palabra \"sabiduría\"?", "¿Cómo definiría usted la palabra \"sabiduría\"?"
		//		}));
		//	// Finally, check that the translation of an unknown form passed in as an existing TXL translation was truly ignored.
		//	var questionAboutWordCount = qs.Items.Last().Categories.Last().Questions[0];
		//	var key = new Question(questionAboutWordCount, questionAboutWordCount.Text, null) { ModifiedPhrase = "How much verbiage exists?" };
		//	Assert.AreEqual("How much verbiage exists?", sut.GetLocalizedString(key));
		//}

		//[TestCase(true)]
		//[TestCase(false)]
		//public void GetLocalizedString_ModifiedQuestionWithNoExistingTranslationForBaseQuestionOrAlternates_ReturnsModifiedForm(bool callGenerateOrUpdateFromMasterQuestions)
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections(true);
		//	if (callGenerateOrUpdateFromMasterQuestions)
		//		sut.GenerateOrUpdateFromMasterQuestions(qs);

		//	// If the user has customized the question in a way that does not match any of the known alternate forms (for any question)
		//	// and none of the alternate forms for the question have been localized, retrieving the localized form will just return the unlocalized
		//	// custom phrase in use.
		//	var lastQuestion = qs.Items.Last().Categories.Last().Questions.Last();
		//	var key = new Question(lastQuestion, lastQuestion.Text, null) { ModifiedPhrase = "This is also gobbledy-gook, wouldn't you say?" };
		//	Assert.AreEqual("This is also gobbledy-gook, wouldn't you say?", sut.GetLocalizedString(key));
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_UpdateWhenNoEntriesHaveLocalizations_NoChangesAndNoErrors()
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections();
		//	sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
		//	sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

		//	VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_UpdateWhenSomeEntriesHaveLocalizations_NoChangesAndNoErrors()
		//{
		//	var sut = new TestLocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections();
		//	sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
		//	sut.AddLocalizationEntry(new SimpleQuestionKey("Details"), LocalizableStringType.Category, "Detalles");
		//	sut.AddLocalizationEntry(new SimpleQuestionKey("Overview"), LocalizableStringType.Category, "Resumen");
		//	var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
		//	sut.AddLocalizationEntry(new Question(firstOverviewQuestion, "A gift from God", null), LocalizableStringType.Answer, "Un don de Dios");
		//	sut.AddLocalizationEntry(firstOverviewQuestion, LocalizableStringType.Question, "¿Qué es la sabidurría?");
		//	sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

		//	var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "A gift from God" }));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "Resumen", "Detalles", "¿Qué es la sabidurría?", "Un don de Dios", "Un don de Dios" }));
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_UpdateWithAddedAndModifiedTxlTranslations_CorrectEntriesHaveCorrectLocalizations()
		//{
		//	var sut = new LocalizationsFileAccessor();
		//	var qs = GenerateProverbsQuestionSections();
		//	var existingTxlTranslations = new List<XmlTranslation>
		//	{
		//		new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
		//		new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
		//		new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
		//		new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
		//	};
		//	sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
		//	var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));

		//	existingTxlTranslations.RemoveAt(0);
		//	existingTxlTranslations.RemoveAt(0);
		//	existingTxlTranslations.RemoveAt(0);
		//	existingTxlTranslations.AddRange(new []
		//	{
		//		new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se defina la sabidurría?"},
		//		new XmlTranslation {PhraseKey = "Riches", Reference = "MAT 6.19", Translation = "Riquezas"},
		//		new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
		//		new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
		//		new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.6", Translation = "Joyas"},
		//	});
		//	sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

		//	results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
		//	Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//		new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "Jewels", "Riches", "A gift from God" }));
		//	Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
		//		new[] { "Resumen", "Detalles", "¿Cómo se defina la sabidurría?", "Un don de Dios", "Joyas", "Riquezas", "Un regalo de Dios" }));
		//}

		//[Test]
		//public void GenerateOrUpdateFromMasterQuestions_MasterFileHasBlanks_BlanksAreEliminated()
		//{
		//	QuestionSections qs = new QuestionSections();
		//	qs.Items = new Section[1];
		//	int iS = 0;
		//	qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 3);
		//	int iC = 0;
		//	Question q = qs.Items[iS].Categories[iC].Questions[0];
		//	q.Text = "What is wisdom?";
		//	q.Answers = new[] { "Smartness on steroids", "" };
		//	q.AlternateForms = new[] { "", null, "How would you define wisdom?" };

		//	iC = 1;
		//	int iQ = 0;
		//	q = qs.Items[iS].Categories[iC].Questions[iQ];
		//	q.StartRef = 20003012;
		//	q.EndRef = 20003014;
		//	q.ScriptureReference = "PRO 3.12-14";
		//	q.Text = "How many words are there?";
		//	q.Answers = new[] { null, "A bunch" };
		//	q.Notes = new[] { "Exact answer is: Five", null };

		//	var sut = new TestLocalizationsFileAccessor();
		//	sut.GenerateOrUpdateFromMasterQuestions(qs);
		//	var key = new UIDataString(qs.Items[0].Categories[0].Questions[0], "Smartness on steroids", LocalizableStringType.Answer);
		//	Assert.AreEqual("Smartness on steroids", sut.GetLocalizableStringInfo(key).Target.LocalizedString);
		//	Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString(key, "", LocalizableStringType.Answer)));
		//	Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString(key, null, LocalizableStringType.Answer)));
		//	key = new UIDataString(qs.Items[0].Categories[0].Questions[0], "How would you define wisdom?", LocalizableStringType.Question);
		//	Assert.AreEqual("How would you define wisdom?", sut.GetLocalizableStringInfo(key).Target.LocalizedString);
		//	key = new UIDataString(qs.Items[0].Categories[1].Questions[0], "A bunch", LocalizableStringType.Answer);
		//	Assert.AreEqual("A bunch", sut.GetLocalizableStringInfo(key).Target.LocalizedString);
		//	key = new UIDataString(key, "Exact answer is: Five", LocalizableStringType.Note);
		//	Assert.AreEqual("Exact answer is: Five", sut.GetLocalizableStringInfo(key).Target.LocalizedString);
		//	Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString(key, "", LocalizableStringType.Note)));
		//	Assert.IsNull(sut.GetLocalizableStringInfo(new UIDataString(key, null, LocalizableStringType.Note)));
		//}

		//[Test]
		//public void SaveAndLoad_NoAlternates_NoLoss()
		//{
		//	using (var folder = new TemporaryFolder("SaveAndLoad_NoAlternates_NoLoss"))
		//	{
		//		var accessorToSave = new LocalizationsFileAccessor(folder.Path, "es");
		//		var qs = GenerateProverbsQuestionSections();
		//		var existingTxlTranslations = new List<XmlTranslation>
		//		{
		//			new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
		//			new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
		//			new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
		//			new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
		//		};
		//		accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
		//		accessorToSave.Save();

		//		var accessorToLoad = new LocalizationsFileAccessor(folder.Path, "es");
		//		var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
		//		Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//			new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
		//		Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
		//			new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));
		//	}
		//}

		//[Test]
		//public void SaveAndLoad_Alternates_NoLoss()
		//{
		//	using (var folder = new TemporaryFolder("SaveAndLoad_Alternates_NoLoss"))
		//	{
		//		var accessorToSave = new LocalizationsFileAccessor(folder.Path, "es");
		//		var qs = GenerateProverbsQuestionSections(true);
		//		var existingTxlTranslations = new List<XmlTranslation>
		//		{
		//			new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabidurría?"},
		//			new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
		//			new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
		//			new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
		//			new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
		//		};
		//		accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
		//		accessorToSave.Save();

		//		var accessorToLoad = new LocalizationsFileAccessor(folder.Path, "es");
		//		var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
		//		Assert.AreEqual(11, results.Count);
		//		Assert.IsTrue(results.Select(k => k.Text).SequenceEqual(
		//			new[] { "Overview",
		//				"What is wisdom?", "What is wisdom?", "What is wisdom?",
		//				"A gift from God",
		//				"What man is happy?", "What man is happy?", "What man is happy?", "What man is happy?",
		//				"Jewels",
		//				"A gift from God" }));
		//		Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
		//			new[] { "Overview",
		//				"What is wisdom?", "What is meant by \"wisdom?\"", "How would you define wisdom?",
		//				"A gift from God",
		//				"What man is happy?", "What person is happy?", "Who is happy?", "What kind of person is blessed?",
		//				"Jewels",
		//				"A gift from God" }));
		//		Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
		//			new[] { "Resumen",
		//				"¿Qué es la sabidurría?", "¿Qué es la sabidurría?", "¿Qué es la sabidurría?",
		//				"Un don de Dios",
		//				"¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?", "¿Qué tipo de persona es bendecida?",
		//				"Perlas preciosas",
		//				"Un don de Dios" }));
		//	}
		//}

		//private static QuestionSections GenerateProverbsQuestionSections(bool includeAlternatives = false)
		//{
		//	QuestionSections qs = new QuestionSections();
		//	qs.Items = new Section[1];
		//	int iS = 0;
		//	qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 3);
		//	int iC = 0;
		//	Question q = qs.Items[iS].Categories[iC].Questions[0];
		//	q.Text = "What is wisdom?";
		//	q.Answers = new[] { "Smartness on steroids", "A gift from God" };
		//	if (includeAlternatives)
		//	{
		//		q.AlternateForms = new[] {"What is meant by \"wisdom?\"", "How would you define wisdom?"};
		//	}

		//	iC = 1;
		//	int iQ = 0;
		//	q = qs.Items[iS].Categories[iC].Questions[iQ];
		//	q.StartRef = 20003012;
		//	q.EndRef = 20003014;
		//	q.ScriptureReference = "PRO 3.12-14";
		//	q.Text = "How many words are there?";
		//	q.Answers = new[] { "A bunch" };
		//	q.Notes = new[] { "Exact answer is: Five", "In some cultures, it may be taboo to count words." };

		//	q = qs.Items[iS].Categories[iC].Questions[++iQ];
		//	q.StartRef = 20003013;
		//	q.EndRef = 20003013;
		//	q.ScriptureReference = "PRO 3.13";
		//	q.Text = "What man is happy?";
		//	q.Notes = new[] {"Make sure respondent understands the differnece between temporal happiness and deep blessing."};
		//	if (includeAlternatives)
		//	{
		//		q.AlternateForms = new[] { "What person is happy?", "Who is happy?", "What kind of person is blessed?" };
		//	}

		//	q = qs.Items[iS].Categories[iC].Questions[++iQ];
		//	q.StartRef = 20003015;
		//	q.EndRef = 20003016;
		//	q.ScriptureReference = "PRO 3.15-16";
		//	q.Text = "What pictures describe wisdom?";
		//	q.Answers = new[] { "Jewels", "Life", "Riches", "A gift from God" };
		//	if (includeAlternatives)
		//	{
		//		q.AlternateForms = new[] { "How would you define wisdom?" };
		//	}

		//	return qs;
		//}

		///// ------------------------------------------------------------------------------------
		//internal static Section CreateSection(string sRef, string heading, int startRef, int endRef, int cOverviewQuestions, int cDetailQuestions)
		//{
		//	Section s = new Section();
		//	s.ScriptureReference = sRef;
		//	s.Heading = heading;
		//	s.StartRef = startRef;
		//	s.EndRef = endRef;
		//	s.Categories = new Category[(cOverviewQuestions > 0 ? 1 : 0) + (cDetailQuestions > 0 ? 1 : 0)];
		//	int iC = 0;
		//	if (cOverviewQuestions > 0)
		//	{
		//		s.Categories[iC] = new Category();
		//		s.Categories[iC].Type = "Overview";
		//		s.Categories[iC].IsOverview = true;
		//		for (int i = 0; i < cOverviewQuestions; i++)
		//			s.Categories[iC].Questions.Add(new Question());
		//		iC++;
		//	}

		//	if (cDetailQuestions > 0)
		//	{
		//		s.Categories[iC] = new Category();
		//		s.Categories[iC].Type = "Details";
		//		for (int i = 0; i < cDetailQuestions; i++)
		//			s.Categories[iC].Questions.Add(new Question());
		//	}
		//	return s;
		//}

		//private void VerifyAllEntriesExistWithNoLocalizedStrings(LocalizationsFileAccessor sut, QuestionSections qs)
		//{
		//	Assert.AreEqual("Overview", sut.GetLocalizedString(new SimpleQuestionKey("Overview"), false));
		//	Assert.AreEqual("Details", sut.GetLocalizedString(new SimpleQuestionKey("Details"), false));

		//	foreach (var section in qs.Items)
		//	{
		//		Assert.AreEqual(section.Heading, sut.GetLocalizedString(new SimpleQuestionKey(section.Heading), false));

		//		foreach (var question in section.Categories.SelectMany(c => c.Questions))
		//		{
		//			Assert.AreEqual(question.PhraseInUse, sut.GetLocalizedString(question, false));
		//			Assert.AreEqual(question.PhraseInUse, sut.GetLocalizedString(new SimpleQuestionKey(question.Text), false));
		//			if (question.AlternateForms != null)
		//			{
		//				foreach (var alternateForm in question.AlternateForms)
		//				{
		//					var key = new Question(question, question.Text, "Not relevant") {ModifiedPhrase = alternateForm};
		//					Assert.AreEqual(alternateForm, sut.GetLocalizedString(key, false));
		//					Assert.AreEqual(alternateForm, sut.GetLocalizedString(new SimpleQuestionKey(alternateForm), false));
		//				}
		//			}
		//			if (question.Answers != null)
		//			{
		//				foreach (var answer in question.Answers)
		//				{
		//					Assert.AreEqual(answer, sut.GetLocalizedString(new Question(question, answer, "Not relevant"), false));
		//					Assert.AreEqual(answer, sut.GetLocalizedString(new SimpleQuestionKey(answer), false));
		//				}
		//			}
		//			if (question.Notes != null)
		//			{
		//				foreach (var comment in question.Notes)
		//				{
		//					Assert.AreEqual(comment, sut.GetLocalizedString(new Question(question, comment, "Not relevant"), false));
		//					Assert.AreEqual(comment, sut.GetLocalizedString(new SimpleQuestionKey(comment), false));
		//				}
		//			}
		//		}
		//	}
		//}

		private IEnumerable<UIDataString> VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(LocalizationsFileAccessor sut, QuestionSections qs)
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
				localizedString = sut.GetLocalizedString(new UIDataString(section.Heading, LocalizableStringType.SectionHeading, section.ScriptureReference, section.StartRef, section.EndRef));
				if (localizedString != key.SourceUIString)
					yield return key;

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					key = new UIDataString(question, LocalizableStringType.Question);
					if (sut.GetLocalizedString(key) != question.PhraseInUse)
						yield return key;

					if (question.AlternateForms != null)
					{
						foreach (var alternateForm in question.AlternateForms)
						{
							key = new UIDataString(question, LocalizableStringType.Alternate, alternateForm);
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