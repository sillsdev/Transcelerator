// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2018' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: LocalizationsFileGenerator.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SIL.Xml;
using static System.String;

namespace SIL.Transcelerator.Localization
{
	public class LocalizationsFileGenerator : LocalizationsFileAccessor
	{
		public enum OverwriteOption
		{
			None,
			Unapproved,
			All,
		}

		private XmlSerializer Serializer => new XmlSerializer(typeof(Localizations),
			"urn:oasis:names:tc:xliff:document:1.2");

		public Regex RegexLocIdsToSet { get; set; }
		public bool MarkApproved { get; set; }
		public OverwriteOption Overwrite { get; set; }

		private static Regex s_regexAmericanToBritish;

		static readonly Dictionary<string, string> AmericanToBritish = new Dictionary<string, string>
		{
			{"plow", "plough"},
			{"scepter", "sceptre"},
			{"center", "centre"},
			{"fiber", "fibre"},
			{"liters", "litres"},
			{"meters", "metres"},
			{"centimeters", "centimetres"},
			{"kilometers", "kilometres"},
			{"sepulcher", "sepulchre"},
			{"theater", "theatre"},
			{"neighbor", "neighbour"},
			{"color", "colour"},
			{"armor", "armour"},
			{"favor", "favour"},
			{"honor", "honour"},
			{"flavor", "flavour"},
			{"behavior", "behaviour"},
			{"offense", "offence"},
			{"defense", "defence"},
			{"license", "licence"},
			{"pluralize", "pluralise"},
			{"practice", "practise"},
			{"practicing", "practising"},
			{"judgment", "judgement"},
			{"fulfill", "fulfil"}, // This gets special handling in regex to prevent partial-word match
			{"fulfills", "fulfils"},
			{"worshipe", "worshippe"},
			{"worshiping", "worshipping"},
			{"apologize", "apologise"},
			{"recognize", "recognise"},
			{"recognizing", "recognising"},
			{"traveler", "traveller"},
			{"traveling", "travelling"},
			{"jewelery", "jewellery"},
			{"gray", "grey"},
			{"mold", "mould"},
			{"sulfur", "sulphur"},
			{"program", "programme"},
			{"canceling", "cancelling"},
			{"canceled", "cancelled"}
		};

		public LocalizationsFileGenerator(string directory, string locale) : base(directory, locale)
		{
			if (locale == "en-GB")
			{
				var pattern = @"\b(" + Join("|", AmericanToBritish.Keys) + ")";
				s_regexAmericanToBritish = new Regex(pattern.Replace("fulfill|", @"fulfill\b|"), RegexOptions.Compiled | RegexOptions.IgnoreCase);
			}
		}

		// For testing only
		internal LocalizationsFileGenerator()
        {
		}

		// For testing only
		internal bool AreLocalizationsValid(out string error) => Localizations.IsValid(out error);

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

		private Action<Group, UIDataString> AddTranslationUnitIfIncluded { get; set; }

		internal void GenerateOrUpdateFromMasterQuestions(QuestionSections questions, List<XmlTranslation> existingTxlTranslations = null, bool retainOnlyTranslatedStrings = false)
		{
			InitializeLookupTable();

			// Note: there are two possible sources for existing localized translations of strings: either a Transcelerator project
			// (the list passed into this method), or the content read from a previous version of the file represented by this accessor.
			var existingLocalizations = XliffRoot.File.Body;
			existingLocalizations.DeleteGroupsWithoutLocalizations();
			if (existingLocalizations.Groups == null)
				existingLocalizations = null;

			InitializeLocalizations();

			Action<Group, UIDataString> AddTranslationUnit;

			if (existingTxlTranslations == null)
			{
				if (existingLocalizations == null)
					AddTranslationUnit = (group, data) => group.AddTranslationUnit(data, MarkApproved);
				else
				{
					AddTranslationUnit = (group, data) =>
					{
						var tu = GetStringLocalization(existingLocalizations, data);
						if (tu == null)
						{
							var localization = Locale == "en-GB" ? GetBritishLocalization(data.SourceUIString) : null;
							if (localization == data.SourceUIString)
								localization = null;
							group.AddTranslationUnit(data, MarkApproved, localization);
						}
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
						group.AddTranslationUnit(data, MarkApproved, LookupTranslation(existingTxlTranslations, data));
					};
				}
				else
				{
					AddTranslationUnit = (group, data) =>
					{
						var tu = GetStringLocalization(existingLocalizations, data);

						if (tu == null)
							group.AddTranslationUnit(data, MarkApproved, LookupTranslation(existingTxlTranslations, data));
						else if (ShouldOverwrite(tu))
						{
							var localization = LookupTranslation(existingTxlTranslations, data);
							if (localization != null && (tu.Target.Text != localization || !tu.Approved && MarkApproved))
							{
								tu = null;
								group.AddTranslationUnit(data, MarkApproved, localization);
							}
						}

						if (tu != null)
							group.AddTranslationUnit(tu);
					};
				}
			}

			AddTranslationUnitIfIncluded = RegexLocIdsToSet == null ?
				AddTranslationUnit : (group, data) =>
				{
					if (!IsNullOrEmpty(data.ScriptureReference) && RegexLocIdsToSet.IsMatch(data.ScriptureReference))
						AddTranslationUnit(group, data);
				};


            foreach (var section in questions.Items)
			{
				var sectionGroup = new Group {Id = FileBody.GetSectionId(section)};
				Localizations.Groups.Add(sectionGroup);
				UIDataString key = new UISectionHeadDataString(section);
				AddTranslationUnitIfIncluded(sectionGroup, key);
				
				foreach (Category category in section.Categories)
				{
					var categoryGroup = sectionGroup.AddSubGroup(category.Type);
					if (category.Type != null)
					{
						if (!Localizations.Categories.TranslationUnits.Any(tu => tu.English == category.Type))
						{
							key = new UISimpleDataString(category.Type, LocalizableStringType.Category);
							AddTranslationUnitIfIncluded(Localizations.Categories, key);
						}
					}

					foreach (Question q in category.Questions.Where(q => !IsNullOrWhiteSpace(q.Text)))
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
							AddTranslationUnitIfIncluded(questionGroup, key);
						}

						AddAlternatesSubgroupAndLocalizableStringsIfNeeded(q, questionGroup);
						AddAnswerOrNoteSubgroupAndLocalizableStringsIfNeeded(q, questionGroup, LocalizableStringType.Answer, qu => qu.Answers);
						AddAnswerOrNoteSubgroupAndLocalizableStringsIfNeeded(q, questionGroup, LocalizableStringType.Note, qu => qu.Notes);
					}
				}
			}

			if (retainOnlyTranslatedStrings)
				Localizations.DeleteGroupsWithoutLocalizations();
			else if (RegexLocIdsToSet != null)
				Localizations.DeleteEmptyGroups();

			AddTranslationUnitIfIncluded = null;
		}

		private TranslationUnit GetStringLocalization(FileBody existingLocalizations, UIDataString data)
		{
			var tu = existingLocalizations.GetStringLocalization(data);
			if (tu != null && Locale == "en-GB")
				tu.Target.Text = GetBritishLocalization(data.SourceUIString);

			return tu;
		}

		private static string GetBritishLocalization(string source)
		{
			return s_regexAmericanToBritish.Replace(source, m =>
			{
				string american = m.Value;
				string british = AmericanToBritish[american.ToLower(CultureInfo.GetCultureInfo("en-US"))];
				// Preserve casing for the first character if it was uppercase in the original string
				if (char.IsUpper(american[0]))
					british = char.ToUpper(british[0]) + british.Substring(1);
				return british;
			});
		}

		private bool ShouldOverwrite(TranslationUnit tu)
		{
			switch (Overwrite)
			{
				case OverwriteOption.None:
					return false;
				case OverwriteOption.Unapproved:
					return !tu.Approved;
				case OverwriteOption.All:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void AddAlternatesSubgroupAndLocalizableStringsIfNeeded(Question q, Group questionGroup)
		{
			bool IncludeAsLocalizable(AlternativeForm af) => !af.Hide && !IsNullOrWhiteSpace(af.Text);

            var alternatives = q.Alternatives;
			if (alternatives != null && alternatives.Any(IncludeAsLocalizable))
			{
				var subGroup = questionGroup.GetQuestionSubGroup(LocalizableStringType.Alternate) ?? questionGroup.AddSubGroup(LocalizableStringType.Alternate.SubQuestionGroupId());
				for (var index = 0; index < alternatives.Length; index++)
				{
					// No need for localizing hidden alternatives
					if (!IncludeAsLocalizable(alternatives[index]))
						continue;

					var key = new UIAlternateDataString(q, index, false);
					AddTranslationUnitIfIncluded(subGroup, key);
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
					if (IsNullOrWhiteSpace(stringsToAdd[index]))
						continue;

					var key = new UIAnswerOrNoteDataString(q, type, index);
					AddTranslationUnitIfIncluded(subGroup, key);
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