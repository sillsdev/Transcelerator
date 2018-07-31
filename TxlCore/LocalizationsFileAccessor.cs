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
						if (q.AlternateForms != null)
						{
							var alt = new Question(q, q.Text, null); // Make a copy so we don't alter the underyling question.
							foreach (var altForm in q.AlternateForms)
							{
								alt.ModifiedPhrase = altForm;
								AddLocalizationEntry(alt, LocalizableStringType.Question, Translation(alt));
							}
						}
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
			Localization localization = null;
			if (localizableStringInfo == null)
			{
				localization = new Localization();
				localizableStringInfo = new LocalizableString {English = key.Text, Type = type, Localization = localization };
				if (key.PhraseInUse != key.Text)
				{
					localizableStringInfo.Alternates = new List<LocalizableStringForm> {
						new LocalizableStringForm {English = key.PhraseInUse, Localization = localization}};
				}
				m_dataDictionary.Add(key.Text, localizableStringInfo);
			}
			else
			{
				if (localizableStringInfo.Type != type)
					localizableStringInfo.Type = LocalizableStringType.Undefined;

				if (key.PhraseInUse == key.Text)
				{
					localization = localizableStringInfo.Localization;
				}
				else if (key.Text == localizableStringInfo.English)
				{
					localization = new Localization();
					if (localizableStringInfo.Alternates == null)
					{
						localizableStringInfo.Alternates = new List<LocalizableStringForm> {new LocalizableStringForm {English = key.PhraseInUse, Localization = localization}};
					}
					else
					{
						var matchingAlt = localizableStringInfo.Alternates.SingleOrDefault(a => a.English == key.PhraseInUse);
						if (matchingAlt == null)
						{
							localizableStringInfo.Alternates.Add(new LocalizableStringForm { English = key.PhraseInUse, Localization = localization });
						}
						else
						{
							localization = matchingAlt.Localization;
						}
					}
				}
				else
				{
					// This is not really an alternate form of the existing string we found. Rather it is one where the alternate form itself
					// happened to be an exact match on the unmodified string from (presumably) some other location. So just treat it as another
					// occurrence.
					localization = localizableStringInfo.Localization;
				}
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

		internal LocalizableString GetLocalizableStringInfo(QuestionKey key)
		{
			LocalizableString value;
			if (m_dataDictionary.TryGetValue(key.Text, out value))
				return value;
			if (key.PhraseInUse != key.Text && m_dataDictionary.TryGetValue(key.PhraseInUse, out value))
				return value;
			return m_dataDictionary.Values.FirstOrDefault(v => v.Alternates?.Any(a => a.English == key.PhraseInUse) ?? false);
		}

		public string GetLocalizedString(QuestionKey key, bool failoverToEnglish = true)
		{
			return GetLocalizableStringInfo(key)?.GetLocalizedString(key) ?? (failoverToEnglish ? key.PhraseInUse : null);
		}
	}
}