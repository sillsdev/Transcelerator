// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2013' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: LocalizationsFileAccessor.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SIL.Xml;

namespace SIL.Transcelerator.Localization
{
	public class LocalizationsFileAccessor
	{
		private Localizations m_xliffRoot;
		protected FileBody Localizations { get; private set; } // Exposed for sake of subclass used in unit tests
		private string DirectoryName { get; }
		public string Locale { get; }
		private const string klocaleFilenamePrefix = "LocalizedPhrases-";
		private const string kLocaleFilenameExtension = ".xlf";

		private XmlSerializer Serializer => new XmlSerializer(typeof(Localizations),
			"urn:oasis:names:tc:xliff:document:1.2");

		public LocalizationsFileAccessor(string directory, string locale)
		{
			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException("Attempt to initialize LocalizationsFileAccessor with non-existent directory failed.");
			DirectoryName = directory;
			Locale = locale;
			if (Exists)
			{
				try
				{
					using (var reader = new XmlTextReader(new StreamReader(FileName)))
						m_xliffRoot = (Localizations)Serializer.Deserialize(reader);
				}
				catch (Exception ex)
				{
					throw new DataException($"File {FileName} could not be deserialized.", ex);
				}

				if (!m_xliffRoot.IsValid(out string error))
					throw new DataException(error);
				if (String.IsNullOrWhiteSpace(m_xliffRoot.File.TargetLanguage))
					m_xliffRoot.File.TargetLanguage = locale;
				else if (m_xliffRoot.File.TargetLanguage != locale)
					throw new DataException($"The target language ({m_xliffRoot.File.TargetLanguage}) specified in the data does not match the locale indicated by the file name: {FileName}");
				Localizations = m_xliffRoot.File.Body;
			}
			else
			{
				InitializeLocalizations();
			}
		}

		internal LocalizationsFileAccessor()
		{
			InitializeLocalizations();
		}

		private void InitializeLocalizations()
		{
			m_xliffRoot = new Localizations();
			m_xliffRoot.Initialize();
			Localizations = m_xliffRoot.File.Body;
		}

		public static IEnumerable<string> GetAvailableLocales(string directory)
		{
			try
			{
				DirectoryInfo dirInfo = new DirectoryInfo(directory);
				return dirInfo.GetFiles($"{klocaleFilenamePrefix}*{kLocaleFilenameExtension}")
					.Select(fi => fi.Name)
					.Select(n => n.Substring(klocaleFilenamePrefix.Length))
					.Select(l => l.Substring(0, l.Length - kLocaleFilenameExtension.Length));
			}
			catch (Exception)
			{
				// Bad path or something
				return new string[0];
			}
		}

		public string FileName => Path.Combine(DirectoryName, $"{klocaleFilenamePrefix}{Locale}{kLocaleFilenameExtension}");
		public FileInfo FileInfo => new FileInfo(FileName);
		public bool Exists => FileInfo.Exists;

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
			m_xliffRoot.File.TargetLanguage = Locale;
			using (var writer = new StreamWriter(FileName))
				Serializer.Serialize(writer, m_xliffRoot);
		}

		internal void GenerateOrUpdateFromMasterQuestions(QuestionSections questions, List<XmlTranslation> existingTxlTranslations = null, bool retainOnlyTranslatedStrings = false)
		{
			// Note: there are two possible sources for existing localized translations of strings: either a Transcelerator project
			// (the list passed into this method), or the content read from a previous version of the file represented by this accessor.
			var existingLocalizations = m_xliffRoot.File.Body;
			existingLocalizations.DeleteGroupsWithoutLocalizations();
			if (existingLocalizations.Groups == null)
				existingLocalizations = null;

			if (existingTxlTranslations == null && retainOnlyTranslatedStrings)
				return;

			InitializeLocalizations();

			Action<Group, UIDataString> AddTranslationUnit;
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
				key = new UIDataString(section.Heading, LocalizableStringType.SectionHeading,
					section.StartRef, section.EndRef);
				AddTranslationUnit(sectionGroup, key);
				
				foreach (Category category in section.Categories)
				{
					var categoryGroup = sectionGroup.AddSubGroup(category.Type);
					if (category.Type != null)
					{
						if (!Localizations.Categories.TranslationUnits.Any(tu => tu.English == category.Type))
						{
							key = new UIDataString(category.Type, LocalizableStringType.Category);
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
							questionGroup = categoryGroup.AddSubGroup($"{FileBody.kQuestionIdPrefix}{q.ScriptureReference}+{q.PhraseInUse}");
							key = new UIDataString(q, LocalizableStringType.Question) {UseAnyAlternate = false};
							AddTranslationUnit(questionGroup, key);
						}

						if (q.AlternateForms != null)
						{
							var alternatesGroup = questionGroup.AddSubGroup(FileBody.kAlternatesGroupId);
							foreach (var altForm in q.AlternateForms.Where(a => !String.IsNullOrWhiteSpace(a)))
							{
								key = new UIDataString(q, LocalizableStringType.Alternate, altForm) {UseAnyAlternate = false};
								AddTranslationUnit(alternatesGroup, key);
							}
						}
						if (q.Answers != null)
						{
							var answersGroup = questionGroup.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAnswersGroupId) ??
								questionGroup.AddSubGroup(FileBody.kAnswersGroupId);
							foreach (var answer in q.Answers.Where(a => !string.IsNullOrWhiteSpace(a)))
							{
								key = new UIDataString(q, LocalizableStringType.Answer, answer);
								AddTranslationUnit(answersGroup, key);
							}
						}
						if (q.Notes != null)
						{
							var notesGroup = questionGroup.AddSubGroup(FileBody.kNotesGroupId);

							foreach (var comment in q.Notes.Where(n => !string.IsNullOrWhiteSpace(n)))
							{
								key = new UIDataString(q, LocalizableStringType.Note, comment);
								AddTranslationUnit(notesGroup, key);
							}
						}
					}
				}
			}

			if (retainOnlyTranslatedStrings)
				Localizations.DeleteGroupsWithoutLocalizations();
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

		public string GetLocalizedString(UIDataString key, bool failoverToEnglish = true)
		{
			return TryGetLocalizedString(key, out string localized) ? localized : (failoverToEnglish ? key.SourceUIString : null);
		}

		public bool TryGetLocalizedString(UIDataString key, out string localized)
		{
			var info = Localizations.GetStringLocalization(key);
			if (info != null && info.Target.IsLocalized)
			{
				localized = info.Target.Text;
				return true;
			}
			localized = key.SourceUIString;
			return false;
		}
	}
}