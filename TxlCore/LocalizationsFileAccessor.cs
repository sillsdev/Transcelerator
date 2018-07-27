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
using System.IO;
using System.Linq;
using SIL.Xml;

namespace SIL.Transcelerator
{
	public class LocalizationsFileAccessor
	{
		private readonly Dictionary<string, LocalizableString> m_dataDictionary;
		private string DirectoryName { get; }
		private string Locale { get; }

		public LocalizationsFileAccessor(string directory, string locale)
		{
			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException("Attempt to initialize LocalizationsFileAccessor with non-existent directory failed.");
			DirectoryName = directory;
			Locale = locale;
			if (Exists)
			{
				var localizations = XmlSerializationHelper.DeserializeFromFile<Localizations>(FileName);
				m_dataDictionary = localizations.Items.ToDictionary(l => l.English, l => l);
			}
			else
			{
				m_dataDictionary = new Dictionary<string, LocalizableString>();
			}
		}

		internal LocalizationsFileAccessor()
		{
			m_dataDictionary = new Dictionary<string, LocalizableString>();
		}

		public string FileName => Path.Combine(DirectoryName, $"LocalizedPhrases-{Locale}.xml");
		public FileInfo FileInfo => new FileInfo(FileName);
		public bool Exists => FileInfo.Exists;

		public void GenerateOrUpdateFromMasterQuestions(string masterQuestionsFilename, string existingTxlTranslationsFilename = null)
		{
			var questions = XmlSerializationHelper.DeserializeFromFile<QuestionSections>(masterQuestionsFilename);
			var existingTxlTranslations = (existingTxlTranslationsFilename == null) ? null :
				XmlSerializationHelper.DeserializeFromFile<List<XmlTranslation>>(existingTxlTranslationsFilename);
			GenerateOrUpdateFromMasterQuestions(questions, existingTxlTranslations);
			Save();
		}

		internal void Save()
		{
			var localizations = new Localizations { Items = m_dataDictionary.Values.ToArray() };
			XmlSerializationHelper.SerializeToFile(FileName, localizations);
		}

		internal void GenerateOrUpdateFromMasterQuestions(QuestionSections questions, List<XmlTranslation> existingTxlTranslations = null)
		{
			QuestionKey key;
			string Translation(QuestionKey k) => existingTxlTranslations == null ? null : LookupTranslation(existingTxlTranslations, k);
			foreach (var section in questions.Items)
			{
				key = new Question(section.ScriptureReference, section.StartRef, section.EndRef, section.Heading, null);
				AddLocalizationEntry(key, LocalizableStringType.SectionHeading, Translation(key));
				foreach (Category category in section.Categories)
				{
					if (category.Type != null)
					{
						key = new SimpleQuestionKey(category.Type);
						AddLocalizationEntry(key, LocalizableStringType.Category, Translation(key));
					}

					foreach (Question q in category.Questions)
					{
						if (q.ScriptureReference == null)
						{
							q.ScriptureReference = section.ScriptureReference;
							q.StartRef = section.StartRef;
							q.EndRef = section.EndRef;
						}
						AddLocalizationEntry(q, LocalizableStringType.Question, Translation(q));
						if (q.Answers != null)
						{
							foreach (var answer in q.Answers)
							{
								key = new Question(q, answer, null);
								AddLocalizationEntry(key, LocalizableStringType.Answer, Translation(key));
							}
						}
						if (q.Notes != null)
						{
							foreach (var comment in q.Notes)
							{
								key = new Question(q, comment, null);
								AddLocalizationEntry(key, LocalizableStringType.Note, Translation(key));
							}
						}
					}
				}
			}
		}

		string LookupTranslation(List<XmlTranslation> translations, QuestionKey key)
		{
			XmlTranslation firstMatchOnPhrase = null;
			foreach (var translation in translations)
			{
				if (translation.PhraseKey == key.PhraseInUse)
				{
					if (translation.Reference == key.ScriptureReference)
						return translation.Translation;
					if (firstMatchOnPhrase == null)
						firstMatchOnPhrase = translation;
				}
			}
			return firstMatchOnPhrase?.Translation;
		}

		internal void AddLocalizationEntry(QuestionKey key, LocalizableStringType type, string localizedString = null)
		{
			var localizableStringInfo = GetLocalizableStringInfo(key);
			var localization = localizableStringInfo?.Localization;
			if (localization == null)
			{
				localization = new Localization();
				m_dataDictionary.Add(key.PhraseInUse, new LocalizableString {English = key.PhraseInUse, Type = type, Localization = localization });
			}
			else
			{
				if (localizableStringInfo.Type != type)
					localizableStringInfo.Type = LocalizableStringType.Undefined;
			}

			if ((localization.Text != localizedString && localizedString != null) || localization.Text == null)
			{
				// We need to force every entry in the output to have a "real" string, because this is
				// what the localizers will actually localize.
				if (localizedString == null)
					localizedString = key.PhraseInUse;

				Occurrence existingOverride = null;
				if (localization.Occurrences == null)
				{
					localization.Occurrences = new List<Occurrence>();
				}
				else
				{
					existingOverride = localization.GetMatchingOverrideIfAny(key);
				}
				if (existingOverride == null)
				{
					localization.Occurrences.Add(new Occurrence
						{StartRef = key.StartRef, EndRef = key.EndRef, LocalizedString = localizedString});
				}
				else
				{
					existingOverride.LocalizedString = localizedString;
				}
			}
		}

		internal Localization GetLocalizationEntry(QuestionKey key)
		{
			return GetLocalizableStringInfo(key)?.Localization;
		}

		internal LocalizableString GetLocalizableStringInfo(QuestionKey key)
		{
			return m_dataDictionary.TryGetValue(key.PhraseInUse, out LocalizableString value) ?
				value : null;
		}

		public string GetLocalizedString(QuestionKey key)
		{
			var existingLocalization = GetLocalizationEntry(key);
			if (existingLocalization?.Text == null)
				return key.PhraseInUse;

			return existingLocalization?.GetMatchingOverrideIfAny(key)?.LocalizedString ?? existingLocalization.Text;
		}
	}
}