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
				sut.AddLocalizationEntry(new UISimpleDataString("Overview", LocalizableStringType.Category));
			Assert.AreEqual("Details", sut.GetLocalizedString(new UISimpleDataString("Details", LocalizableStringType.Category)));
		}

		[Test]
		public void GetLocalizedString_NoMatchingCategoryNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizedString(new UISimpleDataString("Details", LocalizableStringType.Category), false));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetLocalizedString_NoMatchingSectionWithFailover_ReturnsEnglish(bool includeSomeOtherSection)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeSomeOtherSection)
			{
				var otherSection = new Section {Heading = "Jude 3-13 Some section", StartRef = 65001003, EndRef = 65001013};
				sut.AddLocalizationEntry(new UISectionHeadDataString(new SectionInfo(otherSection)));
			}
			var sectionToGet = new Section { Heading = "Acts 3:1-15 This is Bryan's birthday.", StartRef = 44003001, EndRef = 44003015 };
			Assert.AreEqual(sectionToGet.Heading, sut.GetLocalizedString(new UISectionHeadDataString(new SectionInfo(sectionToGet))));
		}

		[Test]
		public void GetLocalizedString_NoMatchingSectionNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			var sectionToGet = new Section { Heading = "Rev 9:2-14 More stuff that's going to happen", StartRef = 66009002, EndRef = 66009014 };
			Assert.IsNull(sut.GetLocalizedString(new UISectionHeadDataString(new SectionInfo(sectionToGet)), false));
		}

		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, false)]
		public void GetLocalizedString_NoMatchingQuestionWithFailover_ReturnsEnglish(bool includeSection, bool includeSomeOtherQuestion)
		{
			var sut = new TestLocalizationsFileAccessor();
			if (includeSection)
			{
				var section = new Section { Heading = "Matthew 3:1-20 Some section", StartRef = 40003001, EndRef = 40003020 };
				sut.AddLocalizationEntry(new UISectionHeadDataString(new SectionInfo(section)));
				if (includeSomeOtherQuestion)
				{
					var otherQuestion = new Question("MAT 3:19", 40003019, 40003019, "Is this really a detail question?", "Answer not relevant in this test");
					sut.AddLocalizationEntry(new UIQuestionDataString(otherQuestion, false, false));
				}
			}
			var questionToGet = new Question("MAT 3:2", 40003002, 40003002, "What are we testing here?", "Answer not relevant in this test");
			Assert.AreEqual("What are we testing here?",
				sut.GetLocalizedString(new UIQuestionDataString(questionToGet, false, false)));
		}

		[Test]
		public void GetLocalizedString_NoMatchingQuestionNoFailover_ReturnsNull()
		{
			var sut = new LocalizationsFileAccessor();
			var questionToGet = new Question("MAT 3:2", 40003002, 40003002, "What are we testing here?", "Answer not relevant in this test");
			Assert.IsNull(sut.GetLocalizedString(new UIQuestionDataString(questionToGet, false, false), false));
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
			var baseQuestion = new Question("MAT 3:2-3", 40003002, 40003003, "Do you like this base question?", null);
			baseQuestion.Answers = new[] { "Answer 1", "This is a whatchamacallit.", "Answer 3" };
			baseQuestion.Notes = new[] { "Note 1", "This is a whatchamacallit.", "Note 3" };
			baseQuestion.AlternateForms = new[] { "Do you adore this underlying question?", "This is a whatchamacallit.", "Is this the sort of platform question you appreciate?" };

			var sut = new TestLocalizationsFileAccessor();
			if (includeBaseQuestion)
			{
				var section = new Section { Heading = "Matthew 3:1-20 Some section", StartRef = 40003001, EndRef = 40003020 };
				sut.AddLocalizationEntry(new UISectionHeadDataString(new SectionInfo(section)));
				sut.AddLocalizationEntry(new UIQuestionDataString(baseQuestion, true, true));
				if (includeOtherEntry)
				{
					sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(baseQuestion, LocalizableStringType.Answer, 0));
					sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(baseQuestion, LocalizableStringType.Note, 0));
					sut.AddLocalizationEntry(new UIAlternateDataString(baseQuestion, 0));
				}
			}
			UIDataString key = type == LocalizableStringType.Alternate ? new UIAlternateDataString(baseQuestion, 1, false) :
				(UIDataString)new UIAnswerOrNoteDataString(baseQuestion, type, 1);
			Assert.AreEqual("This is a whatchamacallit.",
				sut.GetLocalizedString(key));
		}

		[TestCase(LocalizableStringType.Answer)]
		[TestCase(LocalizableStringType.Alternate)]
		[TestCase(LocalizableStringType.Note)]
		public void GetLocalizedString_NoMatchingEntryNoFailover_ReturnsNull(LocalizableStringType type)
		{
			var baseQuestion = new Question("MAT 3:2-3", 40003002, 40003003, "Do you like this base question?", "This is a whatchamacallit.");
			baseQuestion.Notes = new[] { "This is a whatchamacallit." };
			baseQuestion.AlternateForms = new[] { "This is a whatchamacallit." };

			var sut = new LocalizationsFileAccessor();
			UIDataString key = type == LocalizableStringType.Alternate ? new UIAlternateDataString(baseQuestion, 0, false) :
				(UIDataString)new UIAnswerOrNoteDataString(baseQuestion, type, 0);
			Assert.IsNull(sut.GetLocalizedString(key, false));
		}

		[Test]
		public void GetLocalizedString_MatchingCategoryNotLocalized_ReturnsEnglish()
		{
			var key = new UISimpleDataString("Details", LocalizableStringType.Category);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(key);
			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[TestCase("Rev 9:2-14 More stuff that's going to happen", 66009002, 66009014)] // Normal (multi-verse in single chapter) section
		[TestCase("Rev 9:15-11:5 Crazy stuff that will happen", 66009015, 66011005)] // Multi-chapter section
		[TestCase("Hebrews 2:10", 58002010, 58002010)] // Single-verse section
		public void GetLocalizedString_MatchingSectionNotLocalized_ReturnsEnglish(string heading, int startRef, int endRef)
		{
			var section = new Section { Heading = heading, StartRef = startRef, EndRef = endRef };
			var sectionInfo = new SectionInfo(section);
			var sut = new TestLocalizationsFileAccessor();
			var key = new UISectionHeadDataString(sectionInfo);
			sut.AddLocalizationEntry(key);
			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingQuestionNotLocalized_ReturnsEnglish()
		{
			var sut = new TestLocalizationsFileAccessor();
			var section = new Section { Heading = "Matthew 3:1-20 Some section", StartRef = 40003001, EndRef = 40003020 };
			sut.AddLocalizationEntry(new UISectionHeadDataString(new SectionInfo(section)));

			var baseQuestion = new Question("MAT 3:2-3", 40003002, 40003003, "Do you like this base question?", null);
			var key = new UIQuestionDataString(baseQuestion, true, false);
			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_LocalizedStrings_ReturnsLocalized()
		{
			var sut = new TestLocalizationsFileAccessor();

			var section = new Section { Heading = "Matthew 3:1-20 Some section", StartRef = 40003001, EndRef = 40003020 };
			var sectionKey = new UISectionHeadDataString(new SectionInfo(section));
			sut.AddLocalizationEntry(sectionKey, "Mateo 3:1-20 Sección");
			var q1 = new Question("MAT 3:2-3", 40003002, 40003003, "Why is this here?", null);
			q1.Answers = new[] {"Because it needs to be.", "It's a place-holder."};
			q1.Notes = new[] { "This is a bad question.", "See guidelines for writing good questions." };
			q1.AlternateForms = new[] { "What's up with this?", "For what reason is this here?" };

			var questionKey1 = new UIQuestionDataString(q1, true, false);
			sut.AddLocalizationEntry(questionKey1, "¿Por qué está aquí esto?");
			var q2 = new Question("MAT 3:4", 40003004, 40003004, "Can we skip this question?", null);
			var questionKey2 = new UIQuestionDataString(q2, true, false);
			sut.AddLocalizationEntry(questionKey2, "¿Podríamos saltart esta pregunta?");

			var answerKey1 = new UIAnswerOrNoteDataString(q1, LocalizableStringType.Answer, 0);
			sut.AddLocalizationEntry(answerKey1, "¡Porque sí!");
			var answerKey2 = new UIAnswerOrNoteDataString(q1, LocalizableStringType.Answer, 1);
			sut.AddLocalizationEntry(answerKey2, "Solo está ocupando el lugar.");

			var altKey1 = new UIAlternateDataString(q1, 0, false);
			sut.AddLocalizationEntry(altKey1, "¿Qué pasa con esto?");
			var altKey2 = new UIAlternateDataString(q1, 1, false);
			sut.AddLocalizationEntry(altKey2, "¿Por qué razón está aquí esto?");

			var noteKey1 = new UIAnswerOrNoteDataString(q1, LocalizableStringType.Note, 0);
			sut.AddLocalizationEntry(noteKey1, "Esta pregunta no sirve.");
			var noteKey2 = new UIAnswerOrNoteDataString(q1, LocalizableStringType.Note, 1);
			sut.AddLocalizationEntry(noteKey2, "Vea el guía de como hacer buenas preguntas.");

			Assert.AreEqual("Mateo 3:1-20 Sección", sut.GetLocalizedString(sectionKey));
			Assert.AreEqual("¿Por qué está aquí esto?", sut.GetLocalizedString(questionKey1));
			Assert.AreEqual("¿Podríamos saltart esta pregunta?", sut.GetLocalizedString(questionKey2));
			Assert.AreEqual("¡Porque sí!", sut.GetLocalizedString(answerKey1));
			Assert.AreEqual("Solo está ocupando el lugar.", sut.GetLocalizedString(answerKey2));
			Assert.AreEqual("¿Qué pasa con esto?", sut.GetLocalizedString(altKey1));
			Assert.AreEqual("¿Por qué razón está aquí esto?", sut.GetLocalizedString(altKey2));
			Assert.AreEqual("Esta pregunta no sirve.", sut.GetLocalizedString(noteKey1));
			Assert.AreEqual("Vea el guía de como hacer buenas preguntas.", sut.GetLocalizedString(noteKey2));
		}

		[Test]
		public void GetLocalizedString_MultipleSections_FindsLocalizedQuestionInCorrectSection()
		{
			var sut = new TestLocalizationsFileAccessor();

			var sectionKey1 = new UITestDataString("Matthew 3:1-20", LocalizableStringType.SectionHeading, 40003001, 40003020);
			sut.AddLocalizationEntry(sectionKey1);
			var sectionKey2 = new UITestDataString("Matthew 3:21-29", LocalizableStringType.SectionHeading, 40003021, 40003029);
			sut.AddLocalizationEntry(sectionKey2);

			var questionKey1 = new UITestDataString("Why is this here?", LocalizableStringType.Question, 40003002, 40003003);
			sut.AddLocalizationEntry(questionKey1, "¿Por qué está aquí esto?");

			var questionKey2 = new UITestDataString("Why is this here?", LocalizableStringType.Question, 40003021, 40003021);
			sut.AddLocalizationEntry(questionKey2, "¿Qué hace esto aquí?");

			var questionKey3 = new UITestDataString("Is this unique?", LocalizableStringType.Question, 40003022, 40003025);
			sut.AddLocalizationEntry(questionKey3, "¿Es único esto?");

			Assert.AreEqual("¿Por qué está aquí esto?", sut.GetLocalizedString(questionKey1));
			Assert.AreEqual("¿Qué hace esto aquí?", sut.GetLocalizedString(questionKey2));
			Assert.AreEqual("¿Es único esto?", sut.GetLocalizedString(questionKey3));
		}

		[Test]
		public void GetLocalizedString_MatchingQuestionInMultiChapterSectionNotLocalized_ReturnsEnglish()
		{
			var key = new UITestDataString("Do you like this base question?", LocalizableStringType.Question, 40004002, 40004003);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UITestDataString("Matthew 3:21-4:7 Multi-chapter section", LocalizableStringType.SectionHeading, 40003021, 40004007));

			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingQuestionInSingleVerseSectionNotLocalized_ReturnsEnglish()
		{
			var key = new UITestDataString("Do you like this base question?", LocalizableStringType.Question, 58002010, 58002010);
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UITestDataString("Hebrews 2:10 Single-verse section", LocalizableStringType.SectionHeading, 58002010, 58002010));

			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[TestCase(LocalizableStringType.Answer)]
		[TestCase(LocalizableStringType.Alternate)]
		[TestCase(LocalizableStringType.Note)]
		public void GetLocalizedString_MatchingEntryNotLocalized_ReturnsEnglish(LocalizableStringType type)
		{
			var key = new UITestDataString("This is a whatchamacallit.", type, 40003002, 40003003, "Do you like this base question?");
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UITestDataString("Matthew 3:1-20 Some section", LocalizableStringType.SectionHeading, 40003001, 40003020));
			sut.AddLocalizationEntry(new UITestDataString("Do you like this base question?", LocalizableStringType.Question, 40003002, 40003003));
			sut.AddLocalizationEntry(key);

			Assert.AreEqual(key.SourceUIString, sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizableStringInfo_AccessorHasNoEntries_ReturnsNullEntry()
		{
			var sut = new TestLocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizableStringInfo(new UITestDataString("This is some text for which no localization entry has been added.", LocalizableStringType.Question, 002002007, 002002007)));
		}

		[Test]
		public void GetLocalizableStringInfo_AddedQuestionMatchesExistingWithDifferentEndRef_GetsExisting()
		{
			// I'm not 100% sure this is even possible to set up via TXL's UI.
			var sut = new TestLocalizationsFileAccessor();
			sut.AddLocalizationEntry(new UITestDataString("Dummy section head", LocalizableStringType.SectionHeading, 001001001, 001001010));
			var origQuestion = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", null);
			sut.AddLocalizationEntry(new UIQuestionDataString(origQuestion, true, false), "¿Qué hizo él?");
			var questionWithDifferentEndRef = new Question("Gen 1.6", 001001006, 001001007, "What did he do?", null) { ModifiedPhrase = "What did he do?" };
			Assert.AreEqual(sut.GetLocalizableStringInfo(new UIQuestionDataString(origQuestion, false, true)),
				sut.GetLocalizableStringInfo(new UIQuestionDataString(questionWithDifferentEndRef, false, true)));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetLocalizedString_ModifiedQuestionWithNoExistingTranslationForBaseQuestionOrAlternates_ReturnsModifiedForm(bool callGenerateOrUpdateFromMasterQuestions)
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections(true);
			if (callGenerateOrUpdateFromMasterQuestions)
				sut.GenerateOrUpdateFromMasterQuestions(qs);

			// If the user has customized the question in a way that does not match any of the known alternate forms (for any question)
			// and none of the alternate forms for the question have been localized, retrieving the localized form will just return the unlocalized
			// custom phrase in use.
			var lastQuestion = qs.Items.Last().Categories.Last().Questions.Last();
			lastQuestion.ModifiedPhrase = "This is also gobbledy-gook, wouldn't you say?";

			var key = new UIQuestionDataString(lastQuestion, false, false);
			Assert.AreEqual("This is also gobbledy-gook, wouldn't you say?", sut.GetLocalizedString(key));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetLocalizedString_OnlyAlternateFormsHaveLocalizations_BestLocalizationIsReturnedForEachFormOfQuestion(bool retainOnlyTranslatedStrings)
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se puede definir la sabiduría?"},
				new XmlTranslation {PhraseKey = "What is meant by \"wisdom?\"", Reference = "PRO 3.1-35", Translation = "¿Qué significa la palabra \"sabiduría?\""},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations, retainOnlyTranslatedStrings);

			// First check that the correct localized form of each alternate is returned.
			var firstQuestion = qs.Items[0].Categories[0].Questions.First();
			UIDataString key = new UIAlternateDataString(firstQuestion, 1, false);
			Assert.AreEqual("¿Cómo se puede definir la sabiduría?", sut.GetLocalizedString(key));
			key = new UIAlternateDataString(firstQuestion, 0, false);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));

			// Next check that the first* localized alternate is returned for the base question (which is not itself localized).
			// * "first" in the order the alternate occurs in the question, not in the order in the list above.
			key = new UIQuestionDataString(firstQuestion, false, true);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));
			key = new UIQuestionDataString(firstQuestion, true, true);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));

			// Now check that if the user has customized the question in a way that does not match any of the known forms,
			// retrieving the localized form of the ORIGINAL question will still get the localized form of the first localized alternate.
			firstQuestion.ModifiedPhrase = "This is pure gobbledy-gook, isn't it?";
			key = new UIQuestionDataString(firstQuestion, true, true);
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));
			// Unless UseAnyAlternate is false, in which case it will return the original (unlocalized) text of the question.
			key = new UIQuestionDataString(firstQuestion, true, false);
			Assert.AreEqual(firstQuestion.Text, sut.GetLocalizedString(key));
			// Repeat the above two checks if the key is based on the modifed (Phrase-in-use) form of the question.
			key = new UIQuestionDataString(firstQuestion, false, true); // This key should never be created in normal production code.
			Assert.AreEqual("¿Qué significa la palabra \"sabiduría?\"", sut.GetLocalizedString(key));
			key = new UIQuestionDataString(firstQuestion, false, false);
			Assert.AreEqual("This is pure gobbledy-gook, isn't it?", sut.GetLocalizedString(key));
		}

		[TestCase("How would you define wisdom?", "¿Cómo se puede definir la sabiduría?")]
		[TestCase("What is meant by \"wisdom?\"", "¿Qué significa la palabra \"sabiduría?\"")]
		public void GetLocalizedString_OneAlternateFormLocalizedAnyAlternate_ReturnsLocalizedForm(string englishAlt, string localized)
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = englishAlt, Reference = "PRO 3.1-35", Translation = localized},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations, true);

			var firstQuestion = qs.Items[0].Categories[0].Questions.First();
			UIDataString key = new UIAlternateDataString(firstQuestion, 0, true);
			Assert.AreEqual(localized, sut.GetLocalizedString(key));
			key = new UIAlternateDataString(firstQuestion, 1, true);
			Assert.AreEqual(localized, sut.GetLocalizedString(key));
			key = new UIQuestionDataString(firstQuestion, true, true);
			Assert.AreEqual(localized, sut.GetLocalizedString(key));
			firstQuestion.ModifiedPhrase = englishAlt;
			key = new UIQuestionDataString(firstQuestion, false, true);
			Assert.AreEqual(localized, sut.GetLocalizedString(key));
			firstQuestion.ModifiedPhrase = localized;
			key = new UIQuestionDataString(firstQuestion, false, true);
			Assert.AreEqual(localized, sut.GetLocalizedString(key));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWhenNoEntriesHaveLocalizations_NoChangesAndNoErrors()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

			VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_NoLocalizationsRetainOnlyTranslatedStrings_NothingLeft()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs, null, true);
			Assert.IsNull(sut.LocalizationsAccessor.Groups);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateToRemoveUntranslatedNodesWhenNoCategoriesHaveLocalizations_NoErrorsAndOnlyTranslatedRetained()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			var tuForQ1 = sut.AddLocalizationEntry(new UIQuestionDataString(firstOverviewQuestion, true, false), "¿Qué es la sabiduría?");
			var tuForAnswerToQ1 = sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(firstOverviewQuestion, LocalizableStringType.Answer, 1), "Un don de Dios");
			// Check setup:
			Assert.IsTrue(tuForQ1.Target.IsLocalized);
			Assert.IsTrue(tuForAnswerToQ1.Target.IsLocalized);
			sut.GenerateOrUpdateFromMasterQuestions(qs, null, true); // Update
			Assert.IsTrue(tuForQ1.Target.IsLocalized);
			Assert.IsTrue(tuForAnswerToQ1.Target.IsLocalized);

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "What is wisdom?", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "¿Qué es la sabiduría?", "Un don de Dios" }));
			var onlyQuestionLeftInLocalizations = sut.LocalizationsAccessor.Groups.Single().SubGroups.Single().SubGroups.Single();
			Assert.AreEqual(1, onlyQuestionLeftInLocalizations.TranslationUnits.Count);
			Assert.AreEqual("Answers", onlyQuestionLeftInLocalizations.SubGroups.Single().Id);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateToRemoveUntranslatedNodesWhenNothingIsLocalized_NothingRetained()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			var tuForQ1 = sut.AddLocalizationEntry(new UIQuestionDataString(firstOverviewQuestion, true, false), "¿Qué es la sabiduría?", false);
			var tuForAnswerToQ1 = sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(firstOverviewQuestion, LocalizableStringType.Answer, 1), "Un don de Dios", false);
			// Check setup:
			Assert.IsFalse(tuForQ1.Target.IsLocalized);
			Assert.IsFalse(tuForAnswerToQ1.Target.IsLocalized);
			sut.GenerateOrUpdateFromMasterQuestions(qs, null, true); // Update
			Assert.IsFalse(tuForQ1.Target.IsLocalized);
			Assert.IsFalse(tuForAnswerToQ1.Target.IsLocalized);

			Assert.AreEqual(0, GetKeysWithLocalizedStrings(sut, qs).Count());
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateAddTxlTranslationsAndRemoveUntranslatedNodesWhenNoAnswersAlternatesOrNotesHaveLocalizations_NoErrorsAndOnlyTranslatedRetained()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			sut.AddLocalizationEntry(new UISimpleDataString("Details", LocalizableStringType.Category), "Detalles");
			sut.AddLocalizationEntry(new UIQuestionDataString(firstOverviewQuestion, true, false), "¿Qué es la sabiduría?");
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations, true); // Update

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabiduría?" }));

			Assert.AreEqual(2, sut.LocalizationsAccessor.Groups[0].TranslationUnits.Count);
			var onlyQuestionLeftInLocalizations = sut.LocalizationsAccessor.Groups[1].SubGroups.Single().SubGroups.Single();
			Assert.AreEqual("What is wisdom?", onlyQuestionLeftInLocalizations.TranslationUnits.Single().English);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWhenSomeEntriesHaveLocalizations_NoChangesAndNoErrors()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.AddLocalizationEntry(new UISimpleDataString("Details", LocalizableStringType.Category), "Detalles");
			sut.AddLocalizationEntry(new UISimpleDataString("Overview", LocalizableStringType.Category), "Resumen");
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			sut.AddLocalizationEntry(new UIQuestionDataString(firstOverviewQuestion, true, false), "¿Qué es la sabiduría?");
			sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(firstOverviewQuestion, LocalizableStringType.Answer, 1), "Un don de Dios");
			sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabiduría?", "Un don de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWhenSomeEntriesHaveLocalizationsRetainOnlyTranslated_OnlyTranslatedRetained()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.AddLocalizationEntry(new UISimpleDataString("Details", LocalizableStringType.Category), "Detalles");
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			sut.AddLocalizationEntry(new UIQuestionDataString(firstOverviewQuestion, true, false), "¿Qué es la sabiduría?");
			sut.AddLocalizationEntry(new UIAnswerOrNoteDataString(firstOverviewQuestion, LocalizableStringType.Answer, 1), "Un don de Dios");
			sut.GenerateOrUpdateFromMasterQuestions(qs, null, true); // Update

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Details", "What is wisdom?", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Detalles", "¿Qué es la sabiduría?", "Un don de Dios" }));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UISimpleDataString("Overview", LocalizableStringType.Category)));
			var section = qs.Items.Single();
			Assert.IsNull(sut.GetLocalizableStringInfo(new UISectionHeadDataString(new SectionInfo(section))));
			var detailQuestion = qs.Items.First().Categories.Last().Questions[0];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Answer, 0)));
			Assert.IsTrue(detailQuestion.Notes.Select((n, i) => i).All(i => sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Note, i)) == null));
			detailQuestion = qs.Items.First().Categories.Last().Questions[1];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Note, 0)));
			Assert.IsTrue(detailQuestion.AlternateForms.Select((n, i) => i).All(i => sut.GetLocalizableStringInfo(new UIAlternateDataString(detailQuestion, i)) == null));
			detailQuestion = qs.Items.First().Categories.Last().Questions[2];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsTrue(detailQuestion.Answers.Select((n, i) => i).All(i => sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Answer, i)) == null));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAlternateDataString(detailQuestion, 0)));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWithAddedAndModifiedTxlTranslations_CorrectEntriesHaveCorrectLocalizations()
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections();
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabiduría?"},
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabiduría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));

			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.AddRange(new[]
			{
				// Any modified TXL translations will be ignored. The XLIFF version "wins".
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se defina la sabiduría?"},
				new XmlTranslation {PhraseKey = "Riches", Reference = "MAT 6.19", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.6", Translation = "Joyas"},
			});
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations, true);

			results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "Jewels", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabiduría?", "Un don de Dios", "Perlas preciosas", "Riquezas", "Un don de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_UpdateWithAddedAndModifiedTxlTranslationsRetainOnlyTranslated_OnlyTranslatedRetained()
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabiduría?"},
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.AddRange(new[]
			{
				// Any modified TXL translations will be ignored. The XLIFF version "wins".
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se defina la sabiduría?"},
				new XmlTranslation {PhraseKey = "Riches", Reference = "MAT 6.19", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.6", Translation = "Joyas"},
			});
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations, true);

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabiduría?", "Un don de Dios", "Perlas preciosas", "Riquezas", "Un don de Dios" }));

			Assert.IsNull(sut.GetLocalizableStringInfo(new UISimpleDataString("Details", LocalizableStringType.Category)));
			var section = qs.Items.Single();
			Assert.IsNull(sut.GetLocalizableStringInfo(new UISectionHeadDataString(new SectionInfo(section))));
			var overviewQuestion = qs.Items.First().Categories.First().Questions.Single();
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(overviewQuestion, LocalizableStringType.Answer, 0)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAlternateDataString(overviewQuestion, 0)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAlternateDataString(overviewQuestion, overviewQuestion.AlternateForms.Length - 1)));
			var detailQuestion = qs.Items.First().Categories.Last().Questions[0];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Answer, 0)));
			Assert.IsTrue(detailQuestion.Notes.Select((n, i) => i).All(i => sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Note, i)) == null));
			detailQuestion = qs.Items.First().Categories.Last().Questions[1];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Note, 0)));
			Assert.IsTrue(detailQuestion.AlternateForms.Select((n, i) => i).All(i => sut.GetLocalizableStringInfo(new UIAlternateDataString(detailQuestion, i)) == null));
			detailQuestion = qs.Items.First().Categories.Last().Questions[2];
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIQuestionDataString(detailQuestion, true, true)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAnswerOrNoteDataString(detailQuestion, LocalizableStringType.Answer, 1)));
			Assert.IsNull(sut.GetLocalizableStringInfo(new UIAlternateDataString(detailQuestion, 0)));
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
			UIDataString key = new UIAnswerOrNoteDataString(question, LocalizableStringType.Answer, 0);
			Assert.AreEqual("Smartness on steroids", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Answer).Count);

			key = new UIAlternateDataString(question, 2);
			Assert.AreEqual("How would you define wisdom?", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Alternate).Count);

			question = qs.Items[0].Categories[1].Questions[0];
			key = new UIAnswerOrNoteDataString(question, LocalizableStringType.Answer, 1);
			Assert.AreEqual("A bunch", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Answer).Count);

			key = new UIAnswerOrNoteDataString(question, LocalizableStringType.Note, 0);
			Assert.AreEqual("Exact answer is: Five", sut.GetTranslationUnit(key).English);
			Assert.AreEqual(1, sut.GetQuestionSubgroupTranslationUnits(question, LocalizableStringType.Note).Count);
		}

		// Note: If we ever need to translate the two questions differently, we'll need to introduce some additional
		// component into the ID to distinguish them (or maybe put some special notation, such as [#1] into the source
		// question itself). That will definitely complicate lookups and display.
		[Test]
		public void GenerateOrUpdateFromMasterQuestions_DuplicateQuestionInVerseWithDifferentAnswers_QuestionsCoalescedIntoOneWithAllAnswers()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("HEB 9.16-22", "Hebrews 9:16-22", 58009016, 58009022, 0, 4);
			int iC = 0;
			int iQ = 0;
			var q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.StartRef = 58009021;
			q.EndRef = 58009022;
			q.ScriptureReference = "HEB 9.21-22";
			q.Text = "What else did Moses do?";
			q.Answers = new[] { "He sprinkled that blood over the tent and the things used for worship there." };

			var q2 = qs.Items[iS].Categories[iC].Questions[++iQ];
			q2.StartRef = 58009021;
			q2.EndRef = 58009022;
			q2.ScriptureReference = "HEB 9.21-22";
			q2.Text = "Why?";
			q2.Answers = new[] { "To make them acceptable to God, to remove (people's) sin from it, set them apart for him." };

			q = qs.Items[iS].Categories[iC].Questions[++iQ];
			q.StartRef = 58009021;
			q.EndRef = 58009022;
			q.ScriptureReference = "HEB 9.21-22";
			q.Text = "Why did he do it that way? // What else does it say about sin and blood? // What does it say about how things were made \"clean?\"";
			q.Answers = new[] { "Almost everything under the Old Agreement was made acceptable to God by sprinkling it with blood." };
			q.AlternateForms = new[] { "Why did he do it that way?", "What else does it say about sin and blood?", "What does it say about how things were made \"clean?\"" };

			var q4 = qs.Items[iS].Categories[iC].Questions[++iQ];
			q4.StartRef = 58009021;
			q4.EndRef = 58009022;
			q4.ScriptureReference = "HEB 9.21-22";
			q4.Text = "Why?";
			q4.Answers = new[] { "Because God cannot forgive sin unless there has been a sacrifice in which blood is shed." };

			var sut = new TestLocalizationsFileAccessor();
			sut.GenerateOrUpdateFromMasterQuestions(qs);
			Assert.IsTrue(sut.LocalizationsAccessor.IsValid(out string error), error);

			var keyQ4 = new UIQuestionDataString(q4, true, true);
			sut.AddLocalizationEntry(keyQ4, "This one gets clobbered by the next one, right?");
			var keyQ4A1 = new UIAnswerOrNoteDataString(q4, LocalizableStringType.Answer, 0);
			sut.AddLocalizationEntry(keyQ4A1, "Porque Dios no pueded perdonar...");
			Assert.AreEqual("This one gets clobbered by the next one, right?", sut.GetLocalizedString(keyQ4));
			var keyQ2 = new UIQuestionDataString(q2, true, true);
			var keyQ2A1 = new UIAnswerOrNoteDataString(q2, LocalizableStringType.Answer, 0);
			sut.AddLocalizationEntry(keyQ2, "Por que?");
			sut.AddLocalizationEntry(keyQ2A1, "Para hacerles aceptables...");
			Assert.AreEqual("Por que?", sut.GetLocalizedString(keyQ2));
			Assert.AreEqual("Por que?", sut.GetLocalizedString(keyQ4));
			Assert.AreEqual("Porque Dios no pueded perdonar...", sut.GetLocalizedString(keyQ4A1));
			Assert.AreEqual("Para hacerles aceptables...", sut.GetLocalizedString(keyQ2A1));
		}

		// Note: If we ever need to translate the two questions differently, we'll need to introduce some additional
		// component into the ID to distinguish them (or maybe put some special notation, such as [#1] into the source
		// question itself). That will definitely complicate lookups and display.
		[Test]
		public void GenerateOrUpdateFromMasterQuestions_DuplicateQuestionInVerseWithRepeatedAlternates_QuestionsCoalescedIntoOneWithNoDuplicateAlternates()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("HEB 12.12-13", "Hebrews 12:12, 13", 58012012, 58012013, 3, 0);
			int iC = 0;
			int iQ = 0;
			var q1 = qs.Items[iS].Categories[iC].Questions[iQ];
			q1.StartRef = 58012012;
			q1.EndRef = 58012013;
			q1.ScriptureReference = "HEB 12.12-13";
			q1.Text = "What does the author tell/want them to do here?";
			q1.Answers = new[] { "To make every effort to reach the goal God has set for them." };
			q1.AlternateForms = new[] { "What does the author tell them to do here?", "What does the author want them to do here?" };

			var q2 = qs.Items[iS].Categories[iC].Questions[++iQ];
			q2.StartRef = 58012012;
			q2.EndRef = 58012013;
			q2.ScriptureReference = "HEB 12.12-13";
			q2.Text = "What does he say about tired hands and shaky legs?";
			q2.Answers = new[] { "He tells them to take a new grip with their hands and to stand firm on their shaky legs." };

			var q3 = qs.Items[iS].Categories[iC].Questions[++iQ];
			q3.StartRef = 58012012;
			q3.EndRef = 58012013;
			q3.ScriptureReference = "HEB 12.12-13";
			q3.Text = "What does the author tell/want them to do here?";
			q3.Answers = new[] { "He tells them to take a new grip with their hands and to stand firm on their shaky legs." };
			q3.AlternateForms = new[] { "What does the author tell them to do here?", "What does the author want them to do here?" };
			var sut = new TestLocalizationsFileAccessor();
			sut.GenerateOrUpdateFromMasterQuestions(qs);
			Assert.IsTrue(sut.LocalizationsAccessor.IsValid(out string error), error);

			var keyQ3Alt1 = new UIAlternateDataString(q3, 0);
			sut.AddLocalizationEntry(keyQ3Alt1, "¿Qué les dice el autor que hagan?");
			var keyQ3Alt2 = new UIAlternateDataString(q3, 1);
			sut.AddLocalizationEntry(keyQ3Alt2, "¿Qué quiere el autor que hagan? Please review this translation");
			var keyQ1Alt1 = new UIAlternateDataString(q1, 0);
			var keyQ1Alt2 = new UIAlternateDataString(q1, 1);
			sut.AddLocalizationEntry(keyQ1Alt2, "¿Qué quiere el autor que hagan?");
			Assert.AreEqual("¿Qué les dice el autor que hagan?", sut.GetLocalizedString(keyQ1Alt1));
			Assert.AreEqual("¿Qué quiere el autor que hagan?", sut.GetLocalizedString(keyQ1Alt2));
			Assert.AreEqual("¿Qué les dice el autor que hagan?", sut.GetLocalizedString(keyQ3Alt1));
			Assert.AreEqual("¿Qué quiere el autor que hagan?", sut.GetLocalizedString(keyQ3Alt2));
			Assert.AreEqual(2, sut.LocalizationsAccessor.Groups[1].SubGroups.Single().SubGroups.Count);
			Assert.AreEqual(2, sut.LocalizationsAccessor.Groups[1].SubGroups.Single().SubGroups[0].SubGroups.Single(g => g.Id == FileBody.kAlternatesGroupId).TranslationUnits.Count);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_MultipleSections_NoDuplicateCategories()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[2];
			int iS = 0;
			qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 1);
			int iC = 0;
			int iQ = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.Text = "What is wisdom?";

			iC = 1;
			q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.StartRef = 20003012;
			q.EndRef = 20003014;
			q.ScriptureReference = "PRO 3.12-14";
			q.Text = "How many words are there?";

			qs.Items[++iS] = CreateSection("PRO 4.1-15", "Proverbs 4:1-15 More Stuff About Wisdom.", 20004001, 20004015, 1, 1);
			iC = 0;
			q = qs.Items[iS].Categories[iC].Questions[iQ];
			q.Text = "Now what?";

			q = qs.Items[iS].Categories[++iC].Questions[iQ];
			q.StartRef = 20004002;
			q.EndRef = 20004003;
			q.ScriptureReference = "PRO 4.2-3";
			q.Text = "What is the best way to do stuff?";

			var sut = new TestLocalizationsFileAccessor();
			sut.GenerateOrUpdateFromMasterQuestions(qs);
			Assert.IsTrue(sut.LocalizationsAccessor.Categories.TranslationUnits.Select(c => c.English).SequenceEqual(new [] {"Overview", "Details"}));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithNoExistingTranslations_AllEntriesCreatedWithoutLocalizations(bool includeAlternates)
		{
			var sut = new TestLocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections(includeAlternates);
			sut.GenerateOrUpdateFromMasterQuestions(qs);

			VerifyAllEntriesExistWithNoLocalizedStrings(sut, qs);
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithTxlTranslations_CorrectEntriesHaveLocalizations()
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections();
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabiduría?"},
				new XmlTranslation {PhraseKey = "Riches", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabiduría?", "Un don de Dios", "Riquezas", "Un regalo de Dios" }));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_OnlyAlternateFormsHaveTxlTranslations_StoredLocalizationsAreForAlternates()
		{
			var sut = new LocalizationsFileGenerator();
			var qs = GenerateProverbsQuestionSections(true);
			var existingTxlTranslations = new List<XmlTranslation>
			{
				new XmlTranslation {PhraseKey = "How would you define wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se puede definir la sabiduría?"},
				new XmlTranslation {PhraseKey = "What is meant by \"wisdom?\"", Reference = "PRO 3.1-35", Translation = "¿Qué significa la palabra \"sabiduría?\""},
				new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
			};
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			var results = GetKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.AreEqual(4, results.Count); // One of the alternate forms is used for two different questions.
			Assert.IsTrue(results.Select(k => k.SourceUIString).All(s => existingTxlTranslations.Select(e => e.PhraseKey).Contains(s)));
			Assert.IsTrue(results.All(k => k.Type == LocalizableStringType.Alternate));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_TxlTranslationsOfQuestionsAndAlternates_CorrectEntriesHaveLocalizations()
		{
			var sut = new LocalizationsFileGenerator();
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

			var results = GetKeysWithLocalizedStrings(sut, qs, true).ToList();
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
			UIDataString key = new UIQuestionDataString(questionAboutWordCount, false, true);
			Assert.AreEqual("How much verbiage exists?", sut.GetLocalizedString(key));
			key = new UITestDataString(questionAboutWordCount.ModifiedPhrase, LocalizableStringType.Alternate,
				questionAboutWordCount.StartRef, questionAboutWordCount.EndRef, questionAboutWordCount.Text, true);
			Assert.AreEqual("How much verbiage exists?", sut.GetLocalizedString(key));
		}

		[Test]
		public void SaveAndLoad_NoAlternates_NoLoss()
		{
			using (var folder = new TemporaryFolder("SaveAndLoad_NoAlternates_NoLoss"))
			{
				var accessorToSave = new LocalizationsFileGenerator(folder.Path, "es");
				var qs = GenerateProverbsQuestionSections();
				var existingTxlTranslations = new List<XmlTranslation>
				{
					new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabiduría?"},
					new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
					new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
					new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
				};
				accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
				accessorToSave.Save();

				var accessorToLoad = new LocalizationsFileGenerator(folder.Path, "es");
				var results = GetKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
				Assert.IsTrue(results.Select(k => k.SourceUIString).SequenceEqual(
					new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
				Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
					new[] { "Resumen", "¿Qué es la sabiduría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));
			}
		}

		[Test]
		public void SaveAndLoad_Alternates_NoLoss()
		{
			using (var folder = new TemporaryFolder("SaveAndLoad_Alternates_NoLoss"))
			{
				var accessorToSave = new LocalizationsFileGenerator(folder.Path, "es");
				var qs = GenerateProverbsQuestionSections(true);
				var existingTxlTranslations = new List<XmlTranslation>
				{
					new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Qué es la sabiduría?"},
					new XmlTranslation {PhraseKey = "Overview", Translation = "Resumen"},
					new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.3", Translation = "Perlas preciosas"},
					new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.1-35", Translation = "Un don de Dios"},
					new XmlTranslation {PhraseKey = "What kind of person is blessed?", Reference = "PRO 3.13", Translation = "¿Qué tipo de persona es bendecida?"},
				};
				accessorToSave.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);
				accessorToSave.Save();

				var accessorToLoad = new LocalizationsFileGenerator(folder.Path, "es");
				var results = GetKeysWithLocalizedStrings(accessorToLoad, qs).ToList();
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
						"¿Qué es la sabiduría?",
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

		private void VerifyAllEntriesExistWithNoLocalizedStrings(TestLocalizationsFileAccessor sut, QuestionSections qs)
		{
			Assert.IsFalse(sut.GetLocalizableStringInfo(new UISimpleDataString("Overview", LocalizableStringType.Category)).Target.IsLocalized);
			Assert.IsFalse(sut.GetLocalizableStringInfo(new UISimpleDataString("Details", LocalizableStringType.Category)).Target.IsLocalized);

			foreach (var section in qs.Items)
			{
				Assert.IsFalse(sut.GetLocalizableStringInfo(
					new UISectionHeadDataString(new SectionInfo(section))).Target.IsLocalized);

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					var key = new UIQuestionDataString(question, false, true);
					Assert.IsFalse(sut.GetLocalizableStringInfo(key).Target.IsLocalized);

					VerifyNotLocalized(sut, question, question.AlternateForms, LocalizableStringType.Alternate);
					VerifyNotLocalized(sut, question, question.Answers, LocalizableStringType.Answer);
					VerifyNotLocalized(sut, question, question.Notes, LocalizableStringType.Note);
				}
			}
		}

		private void VerifyNotLocalized(TestLocalizationsFileAccessor sut, Question question, string[] subStrings, LocalizableStringType type)
		{
			if (subStrings != null)
			{
				foreach (var s in subStrings)
					Assert.IsFalse(sut.GetLocalizableStringInfo(new UITestDataString(s, type, question.StartRef, question.EndRef, question.Text)).Target.IsLocalized);
			}
		}

		private IEnumerable<UIDataString> GetKeysWithLocalizedStrings(LocalizationsFileGenerator sut, QuestionSections qs, bool useAnyAlternate = false)
		{
			UIDataString key = new UISimpleDataString("Overview", LocalizableStringType.Category);
			var localizedString = sut.GetLocalizedString(key);
			if (localizedString != key.SourceUIString)
				yield return key;
			key = new UISimpleDataString("Details", LocalizableStringType.Category);
			localizedString = sut.GetLocalizedString(key);
			if (localizedString != key.SourceUIString)
				yield return key;

			foreach (var section in qs.Items)
			{
				key = new UISectionHeadDataString(new SectionInfo(section));
				localizedString = sut.GetLocalizedString(key);
				if (localizedString != key.SourceUIString)
					yield return key;

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					key = new UIQuestionDataString(question, false, useAnyAlternate);
					if (sut.GetLocalizedString(key) != question.PhraseInUse)
						yield return key;

					if (question.AlternateForms != null)
					{
						for (var index = 0; index < question.AlternateForms.Length; index++)
						{
							key = new UIAlternateDataString(question, index, useAnyAlternate);
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}

					if (question.Answers != null)
					{
						for (var index = 0; index < question.Answers.Length; index++)
						{
							key = new UIAnswerOrNoteDataString(question, LocalizableStringType.Answer, index);
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}
					if (question.Notes != null)
					{
						for (var index = 0; index < question.Notes.Length; index++)
						{
							key = new UIAnswerOrNoteDataString(question, LocalizableStringType.Note, index);
							if (sut.GetLocalizedString(key) != key.SourceUIString)
								yield return key;
						}
					}
				}
			}
		}
	}
}