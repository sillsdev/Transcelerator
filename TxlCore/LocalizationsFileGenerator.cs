// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2013' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: LocalizationsFileGenerator.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SIL.Xml;

namespace SIL.Transcelerator.Localization
{
	public class LocalizationsFileGenerator : LocalizationsFileAccessor
	{
		private XmlSerializer Serializer => new XmlSerializer(typeof(Localizations),
			"urn:oasis:names:tc:xliff:document:1.2");

		public LocalizationsFileGenerator(string directory, string locale) : base(directory, locale)
		{
		}

		// For testing only
		internal LocalizationsFileGenerator() : base()
		{
		}

		public void GenerateOrUpdateFromMasterQuestions(string masterQuestionsFilename, string existingTxlTranslationsFilename = null, bool retainOnlyTranslatedStrings = false)
		{
			var questions = XmlSerializationHelper.DeserializeFromFile<QuestionSections>(masterQuestionsFilename);
			var existingTxlTranslations = (existingTxlTranslationsFilename == null) ? null :
				XmlSerializationHelper.DeserializeFromFile<List<XmlTranslation>>(existingTxlTranslationsFilename);
			GenerateOrUpdateFromMasterQuestions(questions, existingTxlTranslations, retainOnlyTranslatedStrings);
			Save();
		}

		internal void Save()
		{
			XliffRoot.File.TargetLanguage = Locale;
			using (var writer = new StreamWriter(FileName))
				Serializer.Serialize(writer, XliffRoot);
		}

		private Action<Group, UIDataString> AddTranslationUnit { get; set; }

		internal void GenerateOrUpdateFromMasterQuestions(QuestionSections questions, List<XmlTranslation> existingTxlTranslations = null, bool retainOnlyTranslatedStrings = false)
		{
			InitializeLookupTable();

			// Note: there are two possible sources for existing localized translations of strings: either a Transcelerator project
			// (the list passed into this method), or the content read from a previous version of the file represented by this accessor.
			var existingLocalizations = XliffRoot.File.Body;
			existingLocalizations.DeleteGroupsWithoutLocalizations();
			if (existingLocalizations.Groups == null)
				existingLocalizations = null;

			if (existingTxlTranslations == null && retainOnlyTranslatedStrings)
				return;

			InitializeLocalizations();

			if (existingTxlTranslations == null)
			{
				if (existingLocalizations == null)
					AddTranslationUnit = (group, data) => group.AddTranslationUnit(data);
				else
				{
					AddTranslationUnit = (group, data) =>
					{
						var tu = existingLocalizations.GetStringLocalization(data);
						if (tu == null)
							group.AddTranslationUnit(data);
						else
							group.AddTranslationUnit(tu);
					};
				}
			}
			else
			{
				if (existingLocalizations == null)
				{
					AddTranslationUnit = (group, data) =>
					{
						group.AddTranslationUnit(data, LookupTranslation(existingTxlTranslations, data));
					};
				}
				else
				{
					AddTranslationUnit = (group, data) =>
					{
						var tu = existingLocalizations.GetStringLocalization(data);
						if (tu == null)
							group.AddTranslationUnit(data, LookupTranslation(existingTxlTranslations, data));
						else
							group.AddTranslationUnit(tu);
					};
				}
			}

			UIDataString key;
			foreach (var section in questions.Items)
			{
				var sectionGroup = new Group {Id = FileBody.GetSectionId(section)};
				Localizations.Groups.Add(sectionGroup);
				key = new UISectionHeadDataString(section);
				AddTranslationUnit(sectionGroup, key);
				
				foreach (Category category in section.Categories)
				{
					var categoryGroup = sectionGroup.AddSubGroup(category.Type);
					if (category.Type != null)
					{
						if (!Localizations.Categories.TranslationUnits.Any(tu => tu.English == category.Type))
						{
							key = new UISimpleDataString(category.Type, LocalizableStringType.Category);
							AddTranslationUnit(Localizations.Categories, key);
						}
					}

					foreach (Question q in category.Questions.Where(q => !String.IsNullOrWhiteSpace(q.Text)))
					{
						if (q.ScriptureReference == null)
						{
							q.ScriptureReference = section.ScriptureReference;
							q.StartRef = section.StartRef;
							q.EndRef = section.EndRef;
						}
						// The following line handles the unusual case of the same question twice in the same verse.
						var questionGroup = categoryGroup.SubGroups?.SingleOrDefault(qg => qg.Id == $"{FileBody.kQuestionIdPrefix}{q.ScriptureReference}+{q.PhraseInUse}");
						if (questionGroup == null)
						{
							questionGroup = categoryGroup.AddSubGroup($"{FileBody.kQuestionIdPrefix}{q.ScriptureReference}{FileBody.kQuestionGroupRefSeparator}{q.PhraseInUse}");
							key = new UIQuestionDataString(q, true, false);
							AddTranslationUnit(questionGroup, key);
						}

						AddAlternatesSubgroupAndLocalizableStringsIfNeeded(q, questionGroup);
						AddAnswerOrNoteSubgroupAndLocalizableStringsIfNeeded(q, questionGroup, LocalizableStringType.Answer, qu => qu.Answers);
						AddAnswerOrNoteSubgroupAndLocalizableStringsIfNeeded(q, questionGroup, LocalizableStringType.Note, qu => qu.Notes);
					}
				}
			}

			if (retainOnlyTranslatedStrings)
				Localizations.DeleteGroupsWithoutLocalizations();

			AddTranslationUnit = null;
		}

		private void AddAlternatesSubgroupAndLocalizableStringsIfNeeded(Question q, Group questionGroup)
		{
			var alternatives = q.Alternatives;
			if (alternatives != null && alternatives.Any())
			{
				var subGroup = questionGroup.GetQuestionSubGroup(LocalizableStringType.Alternate) ?? questionGroup.AddSubGroup(LocalizableStringType.Alternate.SubQuestionGroupId());
				for (var index = 0; index < alternatives.Length; index++)
				{
					// No need for localizing hidden alternatives
					if (alternatives[index].Hide || String.IsNullOrWhiteSpace(alternatives[index].Text))
						continue;

					var key = new UIAlternateDataString(q, index, false);
					AddTranslationUnit(subGroup, key);
				}
			}
		}

		private void AddAnswerOrNoteSubgroupAndLocalizableStringsIfNeeded(Question q, Group questionGroup, LocalizableStringType type, Func<Question, string[]> data)
		{
			var stringsToAdd = data(q);
			if (stringsToAdd != null)
			{
				var subGroup = questionGroup.GetQuestionSubGroup(type) ?? questionGroup.AddSubGroup(type.SubQuestionGroupId());
				for (var index = 0; index < stringsToAdd.Length; index++)
				{
					if (String.IsNullOrWhiteSpace(stringsToAdd[index]))
						continue;

					var key = new UIAnswerOrNoteDataString(q, type, index);
					AddTranslationUnit(subGroup, key);
				}
			}
		}

		string LookupTranslation(List<XmlTranslation> translations, UIDataString key)
		{
			XmlTranslation firstMatchOnPhrase = null;
			foreach (var translation in translations)
			{
				if (translation.PhraseKey == key.SourceUIString)
				{
					if (translation.Reference == key.ScriptureReference)
						return translation.Translation;
					if (firstMatchOnPhrase == null)
						firstMatchOnPhrase = translation;
				}
			}
			return firstMatchOnPhrase?.Translation;
		}
	}
}