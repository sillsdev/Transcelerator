// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: Localizations.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SIL.Scripture;

namespace SIL.Transcelerator.Localization
{
	public enum LocalizableStringType
	{
		SectionHeading,
		Category,
		Question,
		Alternate,
		Answer,
		Note,
	}

	public enum State
	{
		NotLocalized,
		Localized,
		Approved,
	}

	#region Localizations
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("xliff", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class Localizations
	{
		[XmlAttribute("version")]
		public string Version
		{
			get => "1.2";
			set
			{
				if (value != null && value != "1.2")
					throw new XmlException($"Unexpected version number in localization file: {value}");
			}
		}

		[XmlElement("file")]
		public File File { get; set; }

		public bool IsValid(out string error)
		{
			if (File == null)
			{
				error = "file node missing";
				return false;
			}
			return File.IsValid(out error);
		}

		public void Initialize()
		{
			File = new File();
			File.Initialize();
		}
	}
	#endregion

	#region File
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlRoot(ElementName = "file")]
	public class File
	{
		[XmlAttribute("original")]
		public string OriginalFileName
		{
			get => "TransceleratorLocalizations.xml";
			set
			{
				if (value != null && value != "TransceleratorLocalizations.xml")
					throw new XmlException($"Unexpected file name in localization file: {value}");
			}
		}

		[XmlAttribute("source-language")]
		public string SourceLanguage
		{
			get => "en";
			set
			{
				if (value != null && value != "en")
					throw new XmlException($"Unexpected source language in localization file: {value}");
			}
		}

		[XmlAttribute("datatype")]
		public string DataType
		{
			get => "plaintext";
			set
			{
				if (value != null && value != "plaintext")
					throw new XmlException($"Unexpected datatype in localization file: {value}");
			}
		}

		[XmlAttribute("target-language")]
		public string TargetLanguage { get; set; }

		[XmlElement("body")]
		public FileBody Body { get; set; }

		public bool IsValid(out string error)
		{
			if (Body == null)
			{
				error = "body node missing";
				return false;
			}
			return Body.IsValid(out error);
		}

		public void Initialize()
		{
			Body = new FileBody();
			Body.Initialize();
		}
	}
	#endregion

	#region FileBody
	[Serializable]
	[DesignerCategory("code")]
	[XmlRoot(ElementName = "body")]
	public class FileBody
	{
		internal const string kSectionIdPrefix = "S:";
		internal const string kQuestionIdPrefix = "Q:";
		internal const string kAlternatesGroupId = "Alternates";
		internal const string kAnswersGroupId = "Answers";
		internal const string kNotesGroupId = "Notes";

		[XmlElement(ElementName = "group", Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
		public List<Group> Groups { get; set; }

		public bool IsValid(out string error)
		{
			if (Groups == null)
			{
				error = "group node missing";
				return false;
			}
			if (Groups.GroupBy(g => g.Id).Any(g => g.Count() > 1))
			{
				error = $"Group {Groups.GroupBy(g => g.Id).First(g => g.Count() > 1).Key} exists more than once in body";
				return false;
			}
			foreach (var group in Groups)
			{
				if (!group.IsValid(out error))
					return false;
			}
			error = null;
			return true;
		}

		public void Initialize()
		{
			Groups = new List<Group> {new Group { Id = "Categories" }};
			Categories.TranslationUnits = new List<TranslationUnit>();
		}

		public Group Categories => Groups.First(g => g.Id == "Categories");

		internal static string GetSectionId(IRefRange refs)
		{
			var bcvStart = new BCVRef(refs.StartRef);
			var bcvEnd = new BCVRef(refs.EndRef);
			Debug.Assert(bcvStart.Book == bcvEnd.Book);
			Debug.Assert(bcvStart.Chapter == bcvEnd.Chapter, "I think sections will always be within a single chapter. If not, we need to rethink.");
			return $"{kSectionIdPrefix}{bcvStart.BookAndChapterContextPrefix()}{bcvStart.Verse}-{bcvEnd.Verse}";
		}

		internal TranslationUnit GetStringLocalization(UIDataString key, bool useAnyAlternate = false)
		{
			if (String.IsNullOrWhiteSpace(key?.SourceUIString))
				return null;

			TranslationUnit transUnit = null;
			if (key.Type == LocalizableStringType.Category)
				transUnit = Categories.GetTranslationUnitIfLocalized(key);
			else if (key.Type == LocalizableStringType.SectionHeading)
			{
				transUnit = Groups.FirstOrDefault(g => g.Id == GetSectionId(key))?.TranslationUnits?.Single();
				return (transUnit != null && transUnit.Target.IsLocalized) ? transUnit : null;
			}
			else
			{
				Group question = FindQuestionGroup(key);
				if (question == null)
					return null;
				switch (key.Type)
				{
					case LocalizableStringType.Question:
						transUnit = question.TranslationUnits.First();
						if (useAnyAlternate)
						{
							if (transUnit.Target.IsLocalized)
								return transUnit;
							return question.GetQuestionSubGroup(LocalizableStringType.Alternate)?.TranslationUnits.FirstOrDefault(tu => tu.Target.IsLocalized);
						}
						break;
					case LocalizableStringType.Alternate:
						var alternates = question.GetQuestionSubGroup(LocalizableStringType.Alternate);
						transUnit = alternates?.GetTranslationUnitIfLocalized(key);
						if (useAnyAlternate)
						{
							if (transUnit != null)
								return transUnit;
							transUnit = question.TranslationUnits.First();
							return transUnit.Target.IsLocalized ? transUnit :
								alternates?.TranslationUnits.FirstOrDefault(tu => tu.Target.IsLocalized);
						}
						break;
					case LocalizableStringType.Answer:
					case LocalizableStringType.Note:
						transUnit = question.GetQuestionSubGroup(key.Type)?.GetTranslationUnitIfLocalized(key);
						break;
				}
			}

			return transUnit;
		}

		internal Group FindQuestionGroup(UIDataString data)
		{
			return FindSectionForQuestion(data)?.FindQuestionGroup(data.Question);
		}

		internal Group FindSectionForQuestion(UIDataString key)
		{
			var bcvQStart = new BCVRef(key.StartRef);
			var sectionIdPrefix = kSectionIdPrefix + bcvQStart.BookAndChapterContextPrefix();
			foreach (var sectionGroup in Groups.Where(g => g.Id.StartsWith(sectionIdPrefix)))
			{
				var sub = sectionGroup.Id.Substring(sectionIdPrefix.Length);
				var verses = sub.Split('-');
				Debug.Assert(verses.Length == 2);
				int sectionVerseStart = Int32.Parse(verses[0]);
				int sectionVerseEnd = Int32.Parse(verses[1]);

				if (sectionVerseStart <= bcvQStart.Verse && sectionVerseEnd >= bcvQStart.Verse)
					return sectionGroup;
			}
			return null;
		}

		public void DeleteGroupsWithoutLocalizations()
		{
			DeleteGroupsWithoutLocalizations(Groups);
			if (!Groups.Any())
				Groups = null;
		}

		private static void DeleteGroupsWithoutLocalizations(List<Group> groups)
		{
			foreach (var group in groups)
			{
				if (group.SubGroups != null)
				{
					DeleteGroupsWithoutLocalizations(group.SubGroups);
					if (!group.SubGroups.Any())
						group.SubGroups = null;
				}
				group.TranslationUnits?.RemoveAll(tu => tu.Target.Status == State.NotLocalized);
				if (group.TranslationUnits?.Count == 0)
					group.TranslationUnits = null;
			}
			groups.RemoveAll(g => g.SubGroups == null && g.TranslationUnits == null);
		}
	}
	#endregion

	#region Group
	[Serializable]
	[DesignerCategory("code")]
	public class Group
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "trans-unit")]
		public List<TranslationUnit> TranslationUnits { get; set; }

		[XmlElement(ElementName = "group")]
		public List<Group> SubGroups { get; set; }

		private int NextSequentialId => TranslationUnits.Count + 1;

		public bool IsValid(out string error)
		{
			if (SubGroups == null || !SubGroups.Any())
			{
				if (TranslationUnits == null || !TranslationUnits.Any())
				{
					error = $"group {Id} contains no subgroups or translation units.";
					return false;
				}
			}
			else
			{
				if (SubGroups.GroupBy(g => g.Id).Any(g => g.Count() > 1))
				{
					error = $"Group {SubGroups.GroupBy(g => g.Id).First(g => g.Count() > 1).Key} exists more than once in group {Id}";
					return false;
				}
				foreach (var group in SubGroups)
				{
					if (!group.IsValid(out error))
						return false;
				}
			}
			if (TranslationUnits.Any(tu => String.IsNullOrEmpty(tu.English)))
			{
				error = $"group {Id} contains a translation unit with no source specified.";
				return false;
			}
			error = null;
			return true;
		}

		internal Group GetQuestionSubGroup(LocalizableStringType type)
		{
			if (!Id.StartsWith(FileBody.kQuestionIdPrefix))
				throw new InvalidOperationException("GetQuestionSubGroup should only be called on a question group.");

			switch (type)
			{
				case LocalizableStringType.Alternate:
					return SubGroups.SingleOrDefault(g => g.Id == FileBody.kAlternatesGroupId);
				case LocalizableStringType.Answer:
					return SubGroups.SingleOrDefault(g => g.Id == FileBody.kAnswersGroupId);
				case LocalizableStringType.Note:
					return SubGroups.SingleOrDefault(g => g.Id == FileBody.kNotesGroupId);
				default:
					throw new ArgumentOutOfRangeException(nameof(type), "GetQuestionSubGroup is for getting Alternates, Answers, or Notes.");
			}
		}

		internal TranslationUnit GetTranslationUnitIfLocalized(UIDataString key)
		{
			return TranslationUnits.FirstOrDefault(tu => tu.English == key.SourceUIString && tu.Target.IsLocalized);
		}

		public void AddTranslationUnit(UIDataString data, string translation = null)
		{
			if (TranslationUnits == null)
				TranslationUnits = new List<TranslationUnit>();

			bool appendSequenceNumber = true;
			string id = data.Type.IdLetter();
			string context;

			if (data.Type == LocalizableStringType.Category)
			{
				context = "Category name";
			}
			else
			{
				var bcv = new BCVRef(data.StartRef);
				var contextPrefix = $"{bcv.BookAndChapterContextPrefix()}{bcv.Verse}#";
				if (data.Type == LocalizableStringType.SectionHeading)
				{
					appendSequenceNumber = false;
					context = $"{contextPrefix}Section Heading";
				}
				else
				{
					context = $"{contextPrefix}{data.Type}:{data.Question}";
					appendSequenceNumber = data.Type != LocalizableStringType.Question;
				}
			}
			Localization target = new Localization();
			if (translation == null)
			{
				target.Text = data.SourceUIString;
				target.Status = State.NotLocalized;
			}
			else
			{
				target.Text = translation;
				target.Status = State.Approved;
			}
			if (appendSequenceNumber)
				id += $":{NextSequentialId}";
			TranslationUnits.Add(new TranslationUnit { Id = id, English = data.SourceUIString, Target = target, Context = context });
		}

		public Group AddSubGroup(string id)
		{
			if (SubGroups == null)
				SubGroups = new List<Group>();
			var newSubGroup = new Group { Id = id};
			SubGroups.Add(newSubGroup);
			return newSubGroup;
		}

		internal Group FindQuestionGroup(string question)
		{
			if (!Id.StartsWith(FileBody.kSectionIdPrefix))
				throw new InvalidOperationException("FindQuestionGroup should only be called on a section group.");

			return SubGroups.SelectMany(g => g.SubGroups).FirstOrDefault(qGrp => qGrp.TranslationUnits.Single().English == question);
		}
	}
	#endregion

	#region TranslationUnit
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class TranslationUnit
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement("source")]
		public string English { get; set; }

		[XmlElement("target")]
		public Localization Target { get; set; }

		[XmlElement("note")]
		public string Context { get; set; }

		public LocalizableStringType Type => Id.Substring(0, 1).GetLocalizableStringType();
	}
	#endregion

	#region Target
	[XmlRoot(ElementName = "target")]
	public class Localization
	{
		[XmlAttribute(AttributeName = "state")]
		public string State
		{
			get
			{
				switch (Status)
				{
					default:
					case Transcelerator.Localization.State.NotLocalized:
						return "needs-translation";
					case Transcelerator.Localization.State.Localized: return "translated";
					case Transcelerator.Localization.State.Approved: return "signed-off";
				}
			}
			set
			{
				switch (value)
				{
					case "needs-translation": Status = Transcelerator.Localization.State.NotLocalized; break;
					case "translated": Status = Transcelerator.Localization.State.Localized; break;
					case "signed-off": Status = Transcelerator.Localization.State.Approved; break;
				}
			}
		}

		[XmlIgnore]
		public State Status { get; set; }

		internal bool IsLocalized => Status != Transcelerator.Localization.State.NotLocalized;

		[XmlText]
		public string Text { get; set; }

		public string LocalizedString => IsLocalized ? Text : null;
	}
	#endregion

	#region AlternateTranslation
	[XmlRoot(ElementName = "alt-trans")]
	public class AlternateTranslation
	{
		[XmlElement(ElementName = "target")]
		public string Target { get; set; }
	}
	#endregion

	//#region class AlternateForm
	//[Serializable]
	//[DesignerCategory("code")]
	//[XmlType(AnonymousType = true)]
	//public class LocalizableStringForm
	//{
	//	[XmlElement("English")]
	//	public string English { get; set; }

	//	[XmlElement("Localization")]
	//	public Localization Localization { get; set; }

	//	// Note: There is always the slight possibility that the actual localization happens to be an
	//	// exact match on the English (e.g., if locale is a dialect/creole of English or if it is a
	//	// 1- or 2-word string and just happens to be an exact cognate). In that case, we can't really
	//	// distinguish between a true localization and an English string copied over into the localized
	//	// text field. But this should be extraordinarily unlikely for any string that actually has
	//	// alternate forms.
	//	public bool HasBeenLocalized(IQuestionKey key) => English != Localization.GetLocalizedString(key);
	//}
	//#endregion

	//#region class LocalizableString
	//[Serializable]
	//[DesignerCategory("code")]
	//[XmlType(AnonymousType = true)]
	//public class LocalizableString : LocalizableStringForm
	//{

	//	// The type is currently just informative. It's not used for anything functional, but it
	//	// could be helpful in prioritizing strings for localization
	//	[XmlAttribute]
	//	[DefaultValue(LocalizableStringType.Undefined)]
	//	public LocalizableStringType Type { get; set; }

	//	[XmlArray(Form = XmlSchemaForm.Unqualified, IsNullable = true), XmlArrayItem("Alt", typeof(LocalizableStringForm))]
	//	public List<LocalizableStringForm> Alternates { get; set; }

	//	// See http://www.xiirus.net/articles/article-avoid-serialization-of-empty-arraylist-or-collection-6wm4e.aspx
	//	[XmlIgnore]
	//	public bool AlternatesSpecified => Alternates?.Count > 0;

	//	internal String GetLocalizedString(IQuestionKey key)
	//	{
	//		if (Localization?.Text == null)
	//			return null;

	//		bool foundLocalizedVersion = HasBeenLocalized(key);
	//		Localization loc = key.PhraseInUse == key.Text || foundLocalizedVersion ? Localization : null;
	//		if (Alternates!= null && Alternates.Any())
	//		{
	//			LocalizableStringForm matchingAlt = null;
	//			foreach (var alternate in Alternates)
	//			{
	//				if (!foundLocalizedVersion && alternate.HasBeenLocalized(key))
	//				{
	//					loc = alternate.Localization;
	//					if (alternate.English == key.PhraseInUse)
	//						break;
	//					foundLocalizedVersion = true;
	//				}
	//				if (matchingAlt == null && alternate.English == key.PhraseInUse)
	//				{
	//					if (alternate.HasBeenLocalized(key))
	//					{
	//						loc = alternate.Localization;
	//						break;
	//					}
	//					matchingAlt = alternate;
	//				}
	//			}
	//			// If we didn't find an exact match that was localized, we prefer a localized one over an
	//			// exact match. But if none of the alternates was localized, then we go with the exact match, if any.
	//			if (!foundLocalizedVersion && matchingAlt != null)
	//				loc = matchingAlt.Localization;
	//		}

	//		return loc?.GetLocalizedString(key);
	//	}
	//}
	//#endregion

	//#region class Category
	//[Serializable]
	//[DesignerCategory("code")]
	//[XmlType(AnonymousType = true)]
	//public class Localization
	//{
	//	public string Text => Occurrences?.FirstOrDefault()?.LocalizedString;

	//	[XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("Occurrence", typeof(Occurrence), IsNullable = false)]
	//	public List<Occurrence> Occurrences { get; set; }

	//	internal Occurrence GetMatchingOverrideIfAny(IQuestionKey key)
	//	{
	//		return Occurrences?.SingleOrDefault(o => key.CompareRefs(o.StartRef, o.EndRef) == 0);
	//	}

	//	internal string GetLocalizedString(IQuestionKey key)
	//	{
	//		return GetMatchingOverrideIfAny(key)?.LocalizedString ?? Text;
	//	}
	//}
	//#endregion

	//#region class Category
	//[Serializable]
	//[DebuggerStepThrough]
	//[DesignerCategory("code")]
	//[XmlType(AnonymousType = true)]
	//public class Occurrence
	//{
	//	[XmlAttribute("startref")]
	//	public int StartRef { get; set; }

	//	[XmlAttribute("endref")]
	//	public int EndRef { get; set; }

	//	[XmlText]
	//	public string LocalizedString { get; set; }
	//}
	//#endregion

	internal static class LocalizationsExtensions
	{
		internal static string IdLetter(this LocalizableStringType type)
		{
			switch (type)
			{
				case LocalizableStringType.SectionHeading: return "h";
				case LocalizableStringType.Category: return "c";
				case LocalizableStringType.Question: return "q";
				case LocalizableStringType.Alternate: return "l";
				case LocalizableStringType.Answer: return "a";
				case LocalizableStringType.Note: return "n";
				default:
					throw new ArgumentOutOfRangeException(nameof(type), "Unexpected string type.");
			}
		}

		internal static LocalizableStringType GetLocalizableStringType(this string letter)
		{
			switch (letter)
			{
				case "h": return LocalizableStringType.SectionHeading;
				case "c": return LocalizableStringType.Category;
				case "q": return LocalizableStringType.Question;
				case "l": return LocalizableStringType.Alternate;
				case "a": return LocalizableStringType.Answer;
				case "n": return LocalizableStringType.Note;
				default:
					throw new ArgumentOutOfRangeException(nameof(letter), "Unexpected Id letter does not correspond to a known localizable string type.");
			}
		}

		internal static string BookAndChapterContextPrefix(this BCVRef bcvStart)
		{
			return $"!{BCVRef.NumberToBookCode(bcvStart.Book)}!{bcvStart.Chapter}~";
		}
	}
}
