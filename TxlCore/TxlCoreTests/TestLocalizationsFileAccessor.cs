// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2013' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TestLocalizationsFileAccessor.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace SIL.Transcelerator.Localization
{
	public class TestLocalizationsFileAccessor : LocalizationsFileGenerator
	{
		public TranslationUnit AddLocalizationEntry(UIDataString data, string localizedString = null, State status = State.Approved)
		{
			if (String.IsNullOrWhiteSpace(data?.SourceUIString))
				throw new ArgumentException("Invalid key!", nameof(data));

			InitializeLookupTable();

			TranslationUnit existing;
			Group group = null;
			var type = data.Type;
			if (type == LocalizableStringType.Category)
			{
				existing = Localizations.Categories.TranslationUnits.SingleOrDefault(c => c.Id == data.SourceUIString);
				if (existing == null)
					return Localizations.Categories.AddTranslationUnit(data, localizedString, status);
			}
			else if (type == LocalizableStringType.SectionHeading)
			{
				var id = FileBody.GetSectionId(data);
				existing = Localizations.Groups.FirstOrDefault(g => g.Id == id)?.TranslationUnits.Single();
				if (existing == null)
				{
					var sectionGroup = new Group {Id = id};
					Localizations.Groups.Add(sectionGroup);
					sectionGroup.SubGroups = new List<Group>();
					return sectionGroup.AddTranslationUnit(data, localizedString, status);
				}
			}
			else
			{
				var sectionGroup = Localizations.FindSectionForQuestion(data);
				Debug.Assert(sectionGroup != null);
				group = sectionGroup.FindQuestionGroup(data.Question) ?? AddQuestionGroup(sectionGroup, data);
				switch (type)
				{
					case LocalizableStringType.Question:
						existing = group.TranslationUnits?.FirstOrDefault();
						break;
					case LocalizableStringType.Alternate:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAlternatesGroupId) ?? group.AddSubGroup(FileBody.kAlternatesGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					case LocalizableStringType.Answer:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAnswersGroupId) ?? group.AddSubGroup(FileBody.kAnswersGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					case LocalizableStringType.Note:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kNotesGroupId) ?? group.AddSubGroup(FileBody.kNotesGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (existing == null)
			{
				Debug.Assert(group != null);
				return group.AddTranslationUnit(data, localizedString, status);
			}
			existing.Target.Text = localizedString;
			existing.Target.Status = status;
			return existing;
		}

		private Group AddQuestionGroup(Group sectionToUse, UIDataString key)
		{
			var categoryGroup = sectionToUse.SubGroups?.SingleOrDefault(g => g.Id == "Details") ?? sectionToUse.AddSubGroup("Details");
			var questionGroup = categoryGroup.AddSubGroup($"{FileBody.kQuestionIdPrefix}{key.ScriptureReference}+{key.Question}");
			questionGroup.SubGroups = new List<Group>();
			return questionGroup;
		}

		internal FileBody LocalizationsAccessor => Localizations;

		internal TranslationUnit GetLocalizableStringInfo(UIDataString key)
		{
			if (String.IsNullOrWhiteSpace(key?.SourceUIString))
				return null;
			
			if (key.Type == LocalizableStringType.Category)
				return Localizations.Categories.TranslationUnits.SingleOrDefault(tu => tu.English == key.SourceUIString);
			if (key.Type == LocalizableStringType.SectionHeading)
				return Localizations.Groups.SingleOrDefault(g => g.Id == FileBody.GetSectionId(key))?.TranslationUnits?.Single();
			
			var question = Localizations.FindQuestionGroup(key);
			if (question == null)
				return null;
			switch (key.Type)
			{
				case LocalizableStringType.Question:
					return question.TranslationUnits?.SingleOrDefault();
				case LocalizableStringType.Answer:
				case LocalizableStringType.Note:
				case LocalizableStringType.Alternate:
					return question.GetQuestionSubGroup(key.Type)?.
						TranslationUnits.SingleOrDefault(tu => tu.English == key.SourceUIString);
				default:
					throw new Exception("Unhandled type!");
			}
		}

		internal TranslationUnit GetTranslationUnit(UIDataString key)
		{
			switch (key.Type)
			{
				case LocalizableStringType.Category:
					return Localizations.Categories.TranslationUnits.FirstOrDefault(tu => tu.English == key.SourceUIString);
				case LocalizableStringType.SectionHeading:
					return Localizations.Groups.FirstOrDefault(g => g.Id == FileBody.GetSectionId(key))?.TranslationUnits.Single();
				case LocalizableStringType.Question:
					return Localizations.FindQuestionGroup(key)?.TranslationUnits.Single();
				case LocalizableStringType.Alternate:
				case LocalizableStringType.Answer:
				case LocalizableStringType.Note:
					return Localizations.FindQuestionGroup(key)?.GetQuestionSubGroup(key.Type)?.TranslationUnits.SingleOrDefault(tu => tu.English == key.SourceUIString);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal IReadOnlyList<TranslationUnit> GetQuestionSubgroupTranslationUnits(Question question, LocalizableStringType type)
		{
			switch (type)
			{
				case LocalizableStringType.Alternate:
				case LocalizableStringType.Answer:
				case LocalizableStringType.Note:
					UIDataString key = new UIQuestionDataString(question, true, false);
					return Localizations.FindQuestionGroup(key)?.GetQuestionSubGroup(type)?.TranslationUnits;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}