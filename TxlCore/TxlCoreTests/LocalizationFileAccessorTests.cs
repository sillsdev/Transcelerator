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

namespace SIL.Transcelerator
{
	[TestFixture]
	public class LocalizationFileAccessorTests
	{
		[Test]
		public void GetLocalizedString_NoMatchingEntry_ReturnsEnglish()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.AreEqual("Blah", sut.GetLocalizedString(new Question("Gen 1.1", 001001001, 001001001, "Blah", "Not used in this test")));
		}

		[Test]
		public void GetLocalizedString_MatchingEntryWithNoLocalization_ReturnsEnglish()
		{
			var sut = new LocalizationsFileAccessor();
			var key = new SimpleQuestionKey("Mary had a little lamb.");
			sut.AddLocalizationEntry(key, LocalizableStringType.Answer);
			Assert.AreEqual("Mary had a little lamb.", sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingEntryWithNoOverrides_ReturnsBaseLocalization()
		{
			var sut = new LocalizationsFileAccessor();
			var key = new SimpleQuestionKey("Mary had a little lamb.");
			sut.AddLocalizationEntry(key, LocalizableStringType.Answer, "María tuvo un pequeño cordero.");
			Assert.AreEqual("María tuvo un pequeño cordero.", sut.GetLocalizedString(key));
		}

		[Test]
		public void GetLocalizedString_MatchingEntryWithNoOverridesForSpecificReference_ReturnsBaseLocalization()
		{
			var sut = new LocalizationsFileAccessor();
			var key = new SimpleQuestionKey("What did he say?");
			sut.AddLocalizationEntry(key, LocalizableStringType.Question, "¿Qué dijo?");
			Assert.AreEqual("¿Qué dijo?", sut.GetLocalizedString(new Question("Gen 1.1", 001001001, 001001001, "What did he say?", "Not used in this test")));
		}

		[Test]
		public void GetLocalizedString_MatchingEntryWithOverridesForUnmatchedReference_ReturnsBaseLocalization()
		{
			var sut = new LocalizationsFileAccessor();
			// Note: First one becomes the default (at least for now)
			sut.AddLocalizationEntry(new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test"), LocalizableStringType.Question, "¿Qué hizo Dios?");
			sut.AddLocalizationEntry(new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test"), LocalizableStringType.Question, "¿Qué hizo Jesús?");
			Assert.AreEqual("¿Qué hizo Dios?", sut.GetLocalizedString(new Question("Mat 14.6", 040014006, 040014006, "What did he do?", "Not used in this test")));
		}

		[Test]
		public void GetLocalizedString_MatchingEntryWithOverridesForMatchedReference_ReturnsOverridenLocalization()
		{
			var sut = new LocalizationsFileAccessor();
			// Note: First one becomes the default (at least for now)
			sut.AddLocalizationEntry(new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test"), LocalizableStringType.Question, "¿Qué hizo Dios?");
			var matchingQuestionKey = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(matchingQuestionKey, LocalizableStringType.Question, "¿Qué hizo Jesús?");
			Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(matchingQuestionKey));
		}

		[TestCase(LocalizableStringType.Answer, "Mary had a little lamb.")]
		[TestCase(LocalizableStringType.Question, "Why did he say that?")]
		[TestCase(LocalizableStringType.Note, "If respondent is mute, he probably won't answer this question.")]
		public void GetLocalizedStringInfo_MatchingEntryWithNoLocalization_ReturnsEnglish(LocalizableStringType type, string english)
		{
			var sut = new LocalizationsFileAccessor();
			var key = new SimpleQuestionKey(english);
			sut.AddLocalizationEntry(key, type);
			var info = sut.GetLocalizableStringInfo(key);
			Assert.AreEqual(english, info.English);
			Assert.AreEqual(type, info.Type);
			Assert.AreEqual(english, info.Localization.Text);
		}

		[Test]
		public void AddLocalizationEntry_MultipleOverridesWithDifferentLocations_AllOverridesSavedCorrectly()
		{
			var sut = new LocalizationsFileAccessor();
			// Note: First one becomes the default (at least for now)
			var defaultKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo?");
			var overrideKey1 = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKey1, LocalizableStringType.Question, "¿Qué hizo Jesús?");
			var overrideKey2 = new Question("Mat 14.2", 040014002, 040014002, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKey2, LocalizableStringType.Question, "¿Qué hizo Dios?");
			var overrideKey3 = new Question("Mat 14.1-2", 040014001, 040014002, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKey3, LocalizableStringType.Question, "¿Qué hizo él?");
			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(defaultKey));
			Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKey1));
			Assert.AreEqual("¿Qué hizo Dios?", sut.GetLocalizedString(overrideKey2));
			Assert.AreEqual("¿Qué hizo él?", sut.GetLocalizedString(overrideKey3));
		}

		[Test]
		public void AddLocalizationEntry_ChangeLocalizationForDefaultLocation_DefaultSavedCorrectly()
		{
			var sut = new LocalizationsFileAccessor();
			// Note: First one becomes the default (at least for now)
			var defaultKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo Dios?");
			var overrideKeyMat14_1 = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKeyMat14_1, LocalizableStringType.Question, "¿Qué hizo Jesús?");

			Assert.AreEqual("¿Qué hizo Dios?", sut.GetLocalizedString(defaultKey));
			// Now attempt to change the default.
			sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo?");
			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(defaultKey));
			Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_1));
			// Ensure default changed
			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(new Question("Mat 14.1-2", 040014001, 040014002, "What did he do?", "Not used in this test")));
		}

		[Test]
		public void AddLocalizationEntry_SaveSameLocalizationForMultipleLocations_AllVariantsSavedCorrectly()
		{
			var sut = new LocalizationsFileAccessor();
			// Note: First one becomes the default (at least for now)
			var defaultKey = new Question("Gen 1.6", 001001006, 001001006, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(defaultKey, LocalizableStringType.Question, "¿Qué hizo?");
			var overrideKeyMat14_1 = new Question("Mat 14.1", 040014001, 040014001, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKeyMat14_1, LocalizableStringType.Question, "¿Qué hizo Jesús?");
			var overrideKeyGen2_3 = new Question("Gen 2.3", 001002003, 001002003, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKeyGen2_3, LocalizableStringType.Question, "¿Qué hizo?");
			var overrideKeyMat14_3 = new Question("Mat 14.3", 040014003, 040014003, "What did he do?", "Not used in this test");
			sut.AddLocalizationEntry(overrideKeyMat14_3, LocalizableStringType.Question, "¿Qué hizo Jesús?");

			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(defaultKey));
			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(overrideKeyGen2_3));
			Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_1));
			Assert.AreEqual("¿Qué hizo Jesús?", sut.GetLocalizedString(overrideKeyMat14_3));
			// Ensure First one is the default
			Assert.AreEqual("¿Qué hizo?", sut.GetLocalizedString(new Question("Mat 14.1-2", 040014001, 040014002, "What did he do?", "Not used in this test")));
		}

		[Test]
		public void GetLocalizationEntry_AccessorHasNoEntries_RturnsNullEntry()
		{
			var sut = new LocalizationsFileAccessor();
			Assert.IsNull(sut.GetLocalizationEntry(new SimpleQuestionKey("This is some text for which no localization entry has been added.")));
		}

		[Test]
		public void GenerateOrUpdateFromMasterQuestions_GenerateFirstTimeWithNoExistingTranslations_AllEntriesCreatedWithoutLocalizations()
		{
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
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
			Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Riquezas", "Un regalo de Dios" }));
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
			var sut = new LocalizationsFileAccessor();
			var qs = GenerateProverbsQuestionSections();
			sut.GenerateOrUpdateFromMasterQuestions(qs); // First time
			sut.AddLocalizationEntry(new SimpleQuestionKey("Details"), LocalizableStringType.Category, "Detalles");
			sut.AddLocalizationEntry(new SimpleQuestionKey("Overview"), LocalizableStringType.Category, "Resumen");
			var firstOverviewQuestion = qs.Items.First().Categories.First().Questions.First();
			sut.AddLocalizationEntry(new Question(firstOverviewQuestion, "A gift from God", null), LocalizableStringType.Answer, "Un don de Dios");
			sut.AddLocalizationEntry(firstOverviewQuestion, LocalizableStringType.Question, "¿Qué es la sabidurría?");
			sut.GenerateOrUpdateFromMasterQuestions(qs); // Update

			var results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Qué es la sabidurría?", "Un don de Dios", "Un don de Dios" }));
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
			Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
				new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));

			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.RemoveAt(0);
			existingTxlTranslations.AddRange(new []
			{
				new XmlTranslation {PhraseKey = "What is wisdom?", Reference = "PRO 3.1-35", Translation = "¿Cómo se defina la sabidurría?"},
				new XmlTranslation {PhraseKey = "Riches", Reference = "MAT 6.19", Translation = "Riquezas"},
				new XmlTranslation {PhraseKey = "Details", Translation = "Detalles"},
				new XmlTranslation {PhraseKey = "A gift from God", Reference = "PRO 3.15-16", Translation = "Un regalo de Dios"},
				new XmlTranslation {PhraseKey = "Jewels", Reference = "REV 14.6", Translation = "Joyas"},
			});
			sut.GenerateOrUpdateFromMasterQuestions(qs, existingTxlTranslations);

			results = VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(sut, qs).ToList();
			Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
				new[] { "Overview", "Details", "What is wisdom?", "A gift from God", "Jewels", "Riches", "A gift from God" }));
			Assert.IsTrue(results.Select(k => sut.GetLocalizedString(k)).SequenceEqual(
				new[] { "Resumen", "Detalles", "¿Cómo se defina la sabidurría?", "Un don de Dios", "Joyas", "Riquezas", "Un regalo de Dios" }));
		}

		[Test]
		public void SaveAndLoad_NoLoss()
		{
			using (var folder = new TemporaryFolder("SaveAndLoad_NoLoss"))
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
				Assert.IsTrue(results.Select(k => k.PhraseInUse).SequenceEqual(
					new[] { "Overview", "What is wisdom?", "A gift from God", "Jewels", "A gift from God" }));
				Assert.IsTrue(results.Select(k => accessorToLoad.GetLocalizedString(k)).SequenceEqual(
					new[] { "Resumen", "¿Qué es la sabidurría?", "Un don de Dios", "Perlas preciosas", "Un don de Dios" }));
			}
		}

		private static QuestionSections GenerateProverbsQuestionSections()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001, 20003035, 1, 3);
			int iC = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What is wisdom?";
			q.Answers = new[] { "Smartness on steroids", "A gift from God" };

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
			q.Notes = new[] {"Make sure respondent understands the differnece between temporal happiness and deep blessing."};

			q = qs.Items[iS].Categories[iC].Questions[++iQ];
			q.StartRef = 20003015;
			q.EndRef = 20003016;
			q.ScriptureReference = "PRO 3.15-16";
			q.Text = "What pictures describe wisdom?";
			q.Answers = new[] { "Jewels", "Life", "Riches", "A gift from God" };

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
			// These assertions will throw exceptions if the localization entry is not found and fail if the localized text is not set
			// to be the same as the English text.
			Assert.AreEqual("Overview", sut.GetLocalizationEntry(new SimpleQuestionKey("Overview")).Text);
			Assert.AreEqual("Details", sut.GetLocalizationEntry(new SimpleQuestionKey("Details")).Text);

			foreach (var section in qs.Items)
			{
				Assert.AreEqual(section.Heading, sut.GetLocalizationEntry(new SimpleQuestionKey(section.Heading)).Text);

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					Assert.AreEqual(question.PhraseInUse, sut.GetLocalizationEntry(question).Text);
					Assert.AreEqual(question.PhraseInUse, sut.GetLocalizationEntry(new SimpleQuestionKey(question.Text)).Text);
					if (question.Answers != null)
					{
						foreach (var answer in question.Answers)
						{
							Assert.AreEqual(answer, sut.GetLocalizationEntry(new Question(question, answer, "Not relevant")).Text);
							Assert.AreEqual(answer, sut.GetLocalizationEntry(new SimpleQuestionKey(answer)).Text);
						}
					}
					if (question.Notes != null)
					{
						foreach (var comment in question.Notes)
						{
							Assert.AreEqual(comment, sut.GetLocalizationEntry(new Question(question, comment, "Not relevant")).Text);
							Assert.AreEqual(comment, sut.GetLocalizationEntry(new SimpleQuestionKey(comment)).Text);
						}
					}
				}
			}
		}

		private IEnumerable<QuestionKey> VerifyAllEntriesExistAndReturnAnyKeysWithLocalizedStrings(LocalizationsFileAccessor sut, QuestionSections qs)
		{
			QuestionKey key = new SimpleQuestionKey("Overview");
			var info = sut.GetLocalizableStringInfo(key);
			Assert.AreEqual(LocalizableStringType.Category, info.Type);
			if (info.Localization.Text != key.PhraseInUse)
				yield return key;
			key = new SimpleQuestionKey("Details");
			info = sut.GetLocalizableStringInfo(key);
			Assert.AreEqual(LocalizableStringType.Category, info.Type);
			if (info.Localization.Text != key.PhraseInUse)
				yield return key;

			foreach (var section in qs.Items)
			{
				key = new SimpleQuestionKey(section.Heading);
				info = sut.GetLocalizableStringInfo(key);
				Assert.AreEqual(LocalizableStringType.SectionHeading, info.Type);
				if (info.Localization.Text != key.PhraseInUse)
					yield return key;

				foreach (var question in section.Categories.SelectMany(c => c.Questions))
				{
					if (sut.GetLocalizationEntry(question).Text != question.PhraseInUse)
						yield return question;

					if (question.Answers != null)
					{
						foreach (var answer in question.Answers)
						{
							key = new Question(question, answer, "Not relevant");
							if (sut.GetLocalizationEntry(key).Text != key.PhraseInUse)
								yield return key;
						}
					}
					if (question.Notes != null)
					{
						foreach (var comment in question.Notes)
						{
							key = new Question(question, comment, "Not relevant");
							info = sut.GetLocalizableStringInfo(key);
							Assert.AreEqual(LocalizableStringType.Note, info.Type);
							if (info.Localization.Text != key.PhraseInUse)
								yield return key;
						}
					}
				}
			}
		}
	}
}