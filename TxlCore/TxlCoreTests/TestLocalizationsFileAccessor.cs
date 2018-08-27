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
	public class TestLocalizationsFileAccessor : LocalizationsFileAccessor
	{
		public void AddLocalizationEntry(UIDataString data, string localizedString = null)
		{
			if (String.IsNullOrWhiteSpace(data?.SourceUIString))
				throw new ArgumentException("Invalid key!", nameof(data));

			TranslationUnit existing;
			Group group = null;
			var type = data.Type;
			if (type == LocalizableStringType.Category)
			{
				existing = Localizations.Categories.TranslationUnits.FirstOrDefault(c => c.Id == data.SourceUIString);
				if (existing == null)
				{
					Localizations.Categories.AddTranslationUnit(data, localizedString);
					return;
				}
			}
			else if (type == LocalizableStringType.SectionHeading)
			{
				var id = FileBody.GetSectionId(data);
				existing = Localizations.Groups.FirstOrDefault(g => g.Id == id)?.TranslationUnits.Single();
				if (existing == null)
				{
					var sectionGroup = new Group {Id = id};
					Localizations.Groups.Add(sectionGroup);
					sectionGroup.AddTranslationUnit(data, localizedString);
					sectionGroup.SubGroups = new List<Group>();
					return;
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
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAlternatesGroupId) ?? group.AddSubGroup(FileBody.kAnswersGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					case LocalizableStringType.Answer:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAnswersGroupId) ?? group.AddSubGroup(FileBody.kAnswersGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					case LocalizableStringType.Note:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kNotesGroupId) ?? group.AddSubGroup(FileBody.kAnswersGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (existing == null)
			{
				Debug.Assert(group != null);
				group.AddTranslationUnit(data, localizedString);
			}
			else
			{
				existing.Target.Text = localizedString;
				existing.Target.Status = State.Approved;
			}
		}

		private Group AddQuestionGroup(Group sectionToUse, UIDataString key)
		{
			var categoryGroup = sectionToUse.SubGroups?.SingleOrDefault(g => g.Id == "Details") ?? sectionToUse.AddSubGroup("Details");
			var questionGroup = categoryGroup.AddSubGroup($"{FileBody.kQuestionIdPrefix}{key.ScriptureReference}+{key.Question}");
			questionGroup.SubGroups = new List<Group>();
			return questionGroup;
		}

		internal TranslationUnit GetLocalizableStringInfo(UIDataString key)
		{
			if (key.Type == LocalizableStringType.Question || key.Type == LocalizableStringType.Alternate)
				key.UseAnyAlternate = false;
			var info = Localizations.GetStringLocalization(key);
			if (info == null || !info.Target.IsLocalized)
				return null;
			return info;
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
					UIDataString key = new UIDataString(question, LocalizableStringType.Question);
					return Localizations.FindQuestionGroup(key)?.GetQuestionSubGroup(type)?.TranslationUnits;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}