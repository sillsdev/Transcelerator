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
using SIL.Scripture;

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
				existing = Localizations.Categories.TranslationUnits.FirstOrDefault(c => c.Id == data.Text);
				if (existing == null)
				{
					Localizations.Categories.AddTranslationUnit(data, localizedString);
					return;
				}
			}
			else if (type == LocalizableStringType.SectionHeading)
			{
				existing = Localizations.Groups.FirstOrDefault(g => g.Id == $"{FileBody.kSectionIdPrefix}{data.ScriptureReference}")?.TranslationUnits.Single();
				if (existing == null)
				{
					var sectionGroup = new Group {Id = $"{FileBody.kSectionIdPrefix}{data.ScriptureReference}"};
					Localizations.Groups.Add(sectionGroup);
					sectionGroup.AddTranslationUnit(data, localizedString);
					return;
				}
			}
			else
			{
				group = Localizations.FindQuestion(data) ?? AddQuestion(data, type != LocalizableStringType.Question);
				switch (type)
				{
					case LocalizableStringType.Question:
						existing = group.TranslationUnits?.FirstOrDefault();
						break;
					case LocalizableStringType.Alternate:
						group = group.SubGroups?.SingleOrDefault(g => g.Id == FileBody.kAlternatesGroupId) ?? group.AddSubGroup(FileBody.kAnswersGroupId);
						existing = group.TranslationUnits?.FirstOrDefault(a => a.English == data.SourceUIString);
						break;
					//	var alternates = question.SubGroups.SingleOrDefault(g => g.Id == FileBody.kAlternatesGroupId);
					//	transUnit = alternates?.GetTranslationUnitIfLocalized(key);
					//	if (useAnyAlternate)
					//	{
					//		if (transUnit != null)
					//			return transUnit;
					//		transUnit = question.TranslationUnits.First();
					//		return transUnit.Target.IsLocalized ? transUnit :
					//			alternates?.TranslationUnits.FirstOrDefault(tu => tu.Target.IsLocalized);
					//	}
					//	break;
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

		private Group AddQuestion(UIDataString key, bool addDummyQuestion)
		{
			var bcvStart = new BCVRef(key.StartRef);
			var bcvEnd = new BCVRef(key.EndRef);
			var bookId = BCVRef.NumberToBookCode(bcvStart.Book);
			var bookAndChapterPrefix = $"!{bookId}!{bcvStart.Chapter}~";
			Group sectionToUse = null;
			foreach (var sectionGroup in Localizations.Groups.Where(g => g.Id.StartsWith(FileBody.kSectionIdPrefix)))
			{
				var context = sectionGroup.TranslationUnits.Single().Context;
				if (context.StartsWith(bookAndChapterPrefix))
				{
					var sub = context.Substring(bookAndChapterPrefix.Length);
					sub = sub.Substring(0, sub.IndexOf("#"));
					int verseNum = Int32.Parse(sub);

					if (verseNum > bcvEnd.Verse)
						break; // Use previous section or create a new one
					sectionToUse = sectionGroup;
					if (verseNum == bcvStart.Verse)
						break;
				}
			}
			if (sectionToUse == null)
			{
				sectionToUse = new Group {Id = $"{FileBody.kSectionIdPrefix}{key.ScriptureReference}"};
				Localizations.Groups.Add(sectionToUse);
				sectionToUse.AddTranslationUnit(LocalizableStringType.SectionHeading,
					new UIDataString(key, key.ScriptureReference + ": Added section head", LocalizableStringType.SectionHeading));
			}

			var categoryGroup = sectionToUse.SubGroups?.SingleOrDefault(g => g.Id == "Details") ?? sectionToUse.AddSubGroup("Details");
			var questionGroup = categoryGroup.AddSubGroup($"{FileBody.kQuestionIdPrefix}{key.ScriptureReference}+{key.PhraseInUse}");
			if (addDummyQuestion)
				questionGroup.AddTranslationUnit(LocalizableStringType.Question, key);
			return questionGroup;
		}

		internal TranslationUnit GetLocalizableStringInfo(UIDataString key, bool useAnyAlternate = false)
		{
			var info = Localizations.GetStringLocalization(key, useAnyAlternate);
			if (info == null || !info.Target.IsLocalized)
				return null;
			return info;
		}
	}
}