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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
	[XmlRoot("xliff", IsNullable = false)]
	public class Localizations
	{
		[XmlAttribute("version")]
		public string Version
		{
			get => "1.2";
			set // Setter only used for deserialization
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
		private static Regex s_regexMultiChapterSection;
		private static Regex s_regexSingleChapterSection;
		private static Regex s_regexSingleVerseSection;
		static FileBody()
		{
			var firstBCVPattern = "^" + kSectionIdPrefix + @"!...!(?<chapterNum>\d+)" + LocalizationsExtensions.kChapterVerseSeparator + @"(?<startVerse>\d+)";
			s_regexSingleVerseSection = new Regex(firstBCVPattern, RegexOptions.Compiled);
			s_regexMultiChapterSection = new Regex(firstBCVPattern + LocalizationsExtensions.kRangeCharacter + @"(?<endingChapterNum>\d+)" + LocalizationsExtensions.kChapterVerseSeparator + @"(?<endVerse>\d+)", RegexOptions.Compiled);
			s_regexSingleChapterSection = new Regex(firstBCVPattern + LocalizationsExtensions.kRangeCharacter + @"(?<endVerse>\d+)", RegexOptions.Compiled);
		}
		internal const string kSectionIdPrefix = "S:";
		internal const string kQuestionIdPrefix = "Q:";
		internal const string kAlternatesGroupId = "Alternates";
		internal const string kAnswersGroupId = "Answers";
		internal const string kNotesGroupId = "Notes";

		[XmlElement(ElementName = "group")]
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

		public Group Categories => Groups.FirstOrDefault(g => g.Id == "Categories");

		internal static string GetSectionId(IRefRange refs)
		{
			var bcvStart = new BCVRef(refs.StartRef);
			var bcvEnd = new BCVRef(refs.EndRef);
			Debug.Assert(bcvStart.Book == bcvEnd.Book);
			if (bcvStart.Chapter == bcvEnd.Chapter)
			{
				if (bcvStart.Verse == bcvEnd.Verse)
					return $"{kSectionIdPrefix}{bcvStart.BookAndChapterContextPrefix()}{bcvStart.Verse}";
				return $"{kSectionIdPrefix}{bcvStart.BookAndChapterContextPrefix()}{bcvStart.Verse}{LocalizationsExtensions.kRangeCharacter}{bcvEnd.Verse}";
			}
			return $"{kSectionIdPrefix}{bcvStart.BookAndChapterContextPrefix()}{bcvStart.Verse}{LocalizationsExtensions.kRangeCharacter}{bcvEnd.Chapter}{LocalizationsExtensions.kChapterVerseSeparator}{bcvEnd.Verse}";
		}

		internal TranslationUnit GetStringLocalization(UIDataString key)
		{
			if (String.IsNullOrWhiteSpace(key?.SourceUIString))
				return null;

			TranslationUnit transUnit = null;
			if (key.Type == LocalizableStringType.Category)
				transUnit = Categories?.GetTranslationUnitIfLocalized(key);
			else if (key.Type == LocalizableStringType.SectionHeading)
			{
				transUnit = Groups.FirstOrDefault(g => g.Id == GetSectionId(key))?.TranslationUnits?.SingleOrDefault();
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
						transUnit = question.TranslationUnits?.FirstOrDefault();
						if (key.UseAnyAlternate)
						{
							if (transUnit != null && transUnit.Target.IsLocalized)
								return transUnit;
							return question.GetQuestionSubGroup(LocalizableStringType.Alternate)?.TranslationUnits.FirstOrDefault(tu => tu.Target.IsLocalized);
						}
						break;
					case LocalizableStringType.Alternate:
						var alternates = question.GetQuestionSubGroup(LocalizableStringType.Alternate);
						transUnit = alternates?.GetTranslationUnitIfLocalized(key);
						if (key.UseAnyAlternate)
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
			var questionBookContextPrefix = bcvQStart.BookContextPrefix();
			var requiredBookPrefix = kSectionIdPrefix + questionBookContextPrefix;
			//Group prevSection = null;
			foreach (var sectionGroup in Groups.Where(s => s.Id.StartsWith(requiredBookPrefix)))
			{
				int endChapter = 0, endVerse = 0;
				var match = s_regexMultiChapterSection.Match(sectionGroup.Id);
				if (match.Success)
				{
					endChapter = Int32.Parse(match.Result("${endingChapterNum}"));
					endVerse = Int32.Parse(match.Result("${endVerse}"));
				}
				else
				{
					match = s_regexSingleChapterSection.Match(sectionGroup.Id);
					if (match.Success)
						endVerse = Int32.Parse(match.Result("${endVerse}"));
					else
						match = s_regexSingleVerseSection.Match(sectionGroup.Id);
				}
				var startChapter = Int32.Parse(match.Result("${chapterNum}"));
				var startVerse = Int32.Parse(match.Result("${startVerse}"));
				if (endChapter == 0)
					endChapter = startChapter;
				if (endVerse == 0)
					endVerse = startVerse;
				// ENHANCE: Cache these
				var bcvRefSectionStart = new BCVRef(bcvQStart.Book, startChapter, startVerse);
				var bcvRefSectionEnd = new BCVRef(bcvQStart.Book, endChapter, endVerse);
				if (bcvRefSectionStart <= bcvQStart && bcvRefSectionEnd >= bcvQStart)
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

		private int NextSequentialId => TranslationUnits?.Count + 1 ?? 1;

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
			if (TranslationUnits != null && TranslationUnits.Any(tu => String.IsNullOrEmpty(tu.English)))
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

			if (SubGroups == null)
				return null;

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

		internal void AddTranslationUnit(TranslationUnit tu)
		{
			if (TranslationUnits == null)
				TranslationUnits = new List<TranslationUnit>();
			TranslationUnits.Add(tu);
		}

		public void AddTranslationUnit(UIDataString data, string translation = null)
		{
			var idPrefix = $"{data.Type.IdLetter()}:";
			var idSuffix = "";
			string context;

			if (data.Type == LocalizableStringType.Category)
			{
				context = "Category name";
				idSuffix = data.SourceUIString;
			}
			else
			{
				var bcv = new BCVRef(data.StartRef);
				var contextPrefix = $"{bcv.BookAndChapterContextPrefix()}{bcv.Verse}#";
				switch (data.Type)
				{
					case LocalizableStringType.SectionHeading:
						context = $"{contextPrefix}Section Heading"; break;
					case LocalizableStringType.Question:
						context = $"{contextPrefix}{data.Type}:{data.Question}"; break;
					case LocalizableStringType.Alternate:
					case LocalizableStringType.Answer:
					case LocalizableStringType.Note:
						// Using a hash code should pretty much guarantee uniqueness and prevent accidental matches
						// if a subsequent version of the data inserts or removes answers, alternates, or notes.
						idSuffix = data.SourceUIString.GetHashCode().ToString();
						goto case LocalizableStringType.Question;
					default:
						throw new ArgumentOutOfRangeException();
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
			AddTranslationUnit(new TranslationUnit { Id = $"{idPrefix}{context}{idSuffix}", English = data.SourceUIString, Target = target, Context = context });
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

			return SubGroups.SelectMany(g => g.SubGroups).FirstOrDefault(qGrp => qGrp.Id.EndsWith(question));
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
#if DEBUG
		public LocalizableStringType Type => Id.Substring(0, 1).GetLocalizableStringType();
#endif
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
	}
	#endregion

	internal static class LocalizationsExtensions
	{
		internal const string kChapterVerseSeparator = "~";
		internal const string kRangeCharacter = "-";

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

#if DEBUG
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
#endif

		internal static string BookContextPrefix(this BCVRef bcvStart)
		{
			return $"!{BCVRef.NumberToBookCode(bcvStart.Book)}!";
		}

		internal static string BookAndChapterContextPrefix(this BCVRef bcvStart)
		{
			return $"{bcvStart.BookContextPrefix()}{bcvStart.Chapter}{kChapterVerseSeparator}";
		}
	}
}
