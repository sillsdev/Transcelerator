// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2026, SIL International.
// <copyright from='2026' to='2026' company='SIL International'>
//		Copyright (c) 2026, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ConcurrentEditSyncTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TXL-257: reported bug where, when one user edits the English text of a question while
	/// another user concurrently translates it, the translation can be lost after Send/Receive.
	/// These tests reproduce the two candidate mechanisms in isolation, without a real Paratext
	/// Send/Receive or a second machine: they take "what a correct Send/Receive merge already
	/// wrote to the data files" as a given (that part is a simple, well-defined keyed list
	/// merge, not in question) and drive Transcelerator's own save/load code - the real
	/// production methods, not reimplementations of them - directly against that input.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class ConcurrentEditSyncTests : PhraseTranslationTestBase
	{
		private const string kReference = "ACT 1.6";
		private const string kOriginalText = "What question did the apostles ask Jesus about his kingdom?";
		private const string kModifiedText = "What query did the apostles pose to Jesus about his realm?";
		private const string kVernacularTranslation = "Vernacular translation of the original question";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Builds a PhraseTranslationHelper the same way UNSQuestionsDialog.LoadTranslations does
		/// (MasterQuestionParser -> PhrasePartManager -> QuestionProvider ->
		/// PhraseTranslationHelper), optionally applying saved customizations, so this exercises
		/// the real "fresh parse from files" code path rather than a stand-in for it.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private PhraseTranslationHelper BuildHelper(IEnumerable<PhraseCustomization> customizations)
		{
			var qs = MasterQuestionParserTests.GenerateStandardQuestionSections();
			var parser = new MasterQuestionParser(qs, new List<string>(), null, null, customizations, null);
			var pq = parser.Result;
			var phrasePartManager = new PhrasePartManager(pq.TranslatableParts, pq.KeyTerms, RenderingsRepo);
			var qp = new QuestionProvider(pq, phrasePartManager);
			return new PhraseTranslationHelper(qp);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates UNSQuestionsDialog.LoadTranslations's matching loop
		/// (GetPhrase(reference, translation.PhraseKey)) after MasterQuestionParser has applied a
		/// Modification customization. Confirms this is NOT the mechanism behind the data loss:
		/// MasterQuestionParser deliberately leaves Question.Text (the matching key) at its
		/// original value and records the edit separately in ModifiedPhrase, specifically so a
		/// wording edit doesn't orphan an existing translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_AfterModificationCustomization_MatchesTranslationKeyedToPreEditText()
		{
			var pc = new PhraseCustomization
			{
				Reference = kReference,
				OriginalPhrase = kOriginalText,
				ModifiedPhrase = kModifiedText,
				Type = PhraseCustomization.CustomizationType.Modification
			};

			var helper = BuildHelper(new[] { pc });

			var phrase = helper.UnfilteredPhrases.Single(p => p.PhraseKey.ScriptureReference == kReference);
			Assert.AreEqual(kModifiedText, phrase.PhraseInUse,
				"Sanity check: the customization should be applied and visible in the UI.");

			var found = helper.GetPhrase(kReference, kOriginalText);

			Assert.IsNotNull(found,
				"A translation keyed to the pre-edit English text should still find its question " +
				"after a customization changes the displayed text.");
			found.Translation = kVernacularTranslation;
			Assert.AreEqual(kVernacularTranslation, found.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates the bug: before the TXL-257 fix, UNSQuestionsDialog.Reload called Save
		/// before LoadTranslations re-read the merged files. Save unconditionally rewrites the
		/// whole Translations file from PhraseTranslationHelper.TranslationsToSave - the exact
		/// property exercised here, not a copy of its logic, so this test can't silently drift
		/// out of sync with what Save actually does. If that in-memory snapshot predates a
		/// Send/Receive merge that already wrote a collaborator's translation to disk, this
		/// overwrite silently destroys it before it's ever read back in.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TranslationsToSave_FromStalePreMergeHelper_OverwritesTranslationAlreadyMergedToDisk()
		{
			var accessor = new InMemoryDataFileAccessor();

			// What Paratext's S/R merge already wrote to disk: the collaborator's translation,
			// keyed on the English text as it existed when they saved it.
			var mergedTranslations = new List<XmlTranslation>
			{
				new XmlTranslation { Reference = kReference, PhraseKey = kOriginalText, Translation = kVernacularTranslation }
			};
			accessor.Write(DataFileAccessor.DataFileId.Translations, mergedTranslations);

			// "My" local, pre-merge in-memory helper at the moment the post-S/R WholeProject
			// notification arrives - it doesn't know about the collaborator's translation yet,
			// because it hasn't been loaded from disk.
			var localHelper = BuildHelper(null);

			accessor.Write(DataFileAccessor.DataFileId.Translations, localHelper.TranslationsToSave);

			var survivingData = accessor.Read(DataFileAccessor.DataFileId.Translations);
			Assert.IsNotNull(survivingData);
			Assert.IsFalse(survivingData.Contains(kVernacularTranslation),
				"Writing TranslationsToSave from a stale pre-merge helper should not silently " +
				"discard a collaborator's translation that was already merged to disk.");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Confirms the TXL-257 fix: UNSQuestionsDialog.OnProjectDataChanged now passes
		/// fSaveFirst: false to Reload for a WholeProject/Send-Receive-triggered reload, so no
		/// stale local overwrite happens before the merged file is read. This simulates
		/// LoadTranslations's matching loop run directly against the merged file, with no
		/// intervening Save, and confirms the collaborator's translation survives.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_AfterLoadingMergedFileWithoutPriorStaleSave_ReturnsCollaboratorsTranslation()
		{
			var accessor = new InMemoryDataFileAccessor();

			var mergedTranslations = new List<XmlTranslation>
			{
				new XmlTranslation { Reference = kReference, PhraseKey = kOriginalText, Translation = kVernacularTranslation }
			};
			accessor.Write(DataFileAccessor.DataFileId.Translations, mergedTranslations);

			// No stale Save happens here - unlike the test above, we go straight to loading.
			var helper = BuildHelper(null);
			var reloadedTranslations = accessor.Read(DataFileAccessor.DataFileId.Translations);
			var deserialized = SIL.Xml.XmlSerializationHelper.DeserializeFromString<List<XmlTranslation>>(reloadedTranslations);
			foreach (var translation in deserialized)
			{
				var phrase = helper.GetPhrase(translation.Reference, translation.PhraseKey);
				if (phrase != null)
					phrase.Translation = translation.Translation;
			}

			var reattached = helper.GetPhrase(kReference, kOriginalText);
			Assert.AreEqual(kVernacularTranslation, reattached.Translation,
				"With no premature Save, the merged translation should load and attach normally.");
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Minimal in-memory DataFileAccessor for tests, standing in for the on-disk plugin data
	/// files that Paratext's Send/Receive merges. Following the pattern of
	/// TestKeyTermRenderingDataFileAccessor in KeyTermTests.cs.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal class InMemoryDataFileAccessor : DataFileAccessor
	{
		private readonly Dictionary<DataFileId, string> m_data = new Dictionary<DataFileId, string>();

		protected override void Write(DataFileId fileId, string data) => m_data[fileId] = data;

		public override string Read(DataFileId fileId) => m_data.TryGetValue(fileId, out var data) ? data : null;

		public override bool Exists(DataFileId fileId) => m_data.ContainsKey(fileId);

		public override DateTime ModifiedTime(DataFileId fileId) => DateTime.UtcNow;

		public override bool BookSpecificDataExists(BookSpecificDataFileId fileId, string bookId) =>
			throw new NotImplementedException();

		protected override void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data) =>
			throw new NotImplementedException();
	}
}
