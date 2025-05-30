// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2018' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
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
		NonLocalizable,
	}

	#region Localizations
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("xliff", IsNullable = false)]
	public class Localizations
	{
		/// <summary>
		/// This is a special string that localizers can use when any localizable data (except a
		/// category name) is to be omitted in a particular localization. If a question is
		/// omitted but it has one or more alternatives that are not omitted, then the first
		/// alternative that is not omitted will be used. Otherwise, the entire question (along
		/// with any answers and notes) will be omitted. Anything omitted will never be seen by
		/// the user, either in the UI or in the output. Note that this means that in some cases,
		/// the user may see different sets of questions depending on which UI locale they are
		/// working in. Also note that when generating a script, if a question has been translated,
		/// it will not be omitted just because the LWC being used has it omitted (but the output
		/// of the LWC version of the question will be omitted).
		/// </summary>
		public const string kOmitInLocalization = "*Omit*";

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

	#region File (owned by Localizations)
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

	#region FileBody (owned by File)
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
		internal const string kQuestionGroupRefSeparator = "+";
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

		public Group Categories => Groups?.FirstOrDefault(g => g.Id == "Categories");

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
			if (String.IsNullOrWhiteSpace(key?.SourceUIString) || key.Type == LocalizableStringType.NonLocalizable || Groups == null)
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
				var question = FindQuestionGroup(key);
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
							transUnit = question.TranslationUnits?.FirstOrDefault();
							return transUnit != null && transUnit.Target.IsLocalized ? transUnit :
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
			var sections = FindSectionsForQuestion(data).ToList();
			switch (sections.Count)
			{
				case 0:
					return null;
				case 1:
					return sections[0].FindQuestionGroup(data);
				default:
					foreach (var section in sections)
					{
						var group = section.FindQuestionGroup(data);
						if (group != null)
							return group;
					}
					return null;
			}
		}

		internal IEnumerable<Group> FindSectionsForQuestion(UIDataString key)
		{
			var bcvQStart = new BCVRef(key.StartRef);
			var questionBookContextPrefix = bcvQStart.BookContextPrefix();
			var requiredBookPrefix = kSectionIdPrefix + questionBookContextPrefix;
			//Group prevSection = null;
			bool foundFirstOne = false;
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
				{
					yield return sectionGroup;
					foundFirstOne = true;
				}
				else if (foundFirstOne)
					break;
			}
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
				if (group.TranslationUnits != null)
				{
					group.TranslationUnits.RemoveAll(tu => !tu.Target.IsLocalized);
					if (group.TranslationUnits.Count == 0)
						group.TranslationUnits = null;
				}
			}
			groups.RemoveAll(g => g.SubGroups == null && g.TranslationUnits == null);
		}

		

		public void DeleteEmptyGroups()
		{
			DeleteEmptyGroups(Groups);
			if (!Groups.Any())
				Groups = null;
		}

		private static void DeleteEmptyGroups(List<Group> groups)
		{
			foreach (var group in groups)
			{
				if (group.SubGroups != null)
				{
					DeleteEmptyGroups(group.SubGroups);
					if (!group.SubGroups.Any())
						group.SubGroups = null;
				}
			}
			groups.RemoveAll(g => g.SubGroups == null && (g.TranslationUnits == null || g.TranslationUnits.Count == 0));
		}
	}
	#endregion

	#region Group (in a list owned by FileBody)
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

			return SubGroups?.SingleOrDefault(g => g.Id == type.SubQuestionGroupId());
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

		public TranslationUnit AddTranslationUnit(UIDataString data, bool markApproved, string translation = null, bool isLocalized = true)
		{
			if (translation == null)
				isLocalized = false;
			if (!isLocalized)
				markApproved = false;
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
						Debug.Assert(Id == data.Type.SubQuestionGroupId());
						// Using a hash code should pretty much guarantee uniqueness and prevent accidental matches
						// if a subsequent version of the data inserts or removes answers, alternates, or notes.
						var existing = TranslationUnits?.SingleOrDefault(tu => tu.English == data.SourceUIString);
						if (existing != null)
						{
							if (translation != null)
							{
								existing.Target.Text = translation;
								existing.Target.IsLocalized = isLocalized;
								if (markApproved)
									existing.Target.Approved = true;
								existing.Approved = isLocalized && markApproved;
							}
							return existing;
						}
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
			}
			else
			{
				target.Text = translation;
				target.IsLocalized = isLocalized;
				if (markApproved)
					target.Approved = true;
			}
			var newTu = new TranslationUnit {Id = $"{idPrefix}{context}{idSuffix}", English = data.SourceUIString,
				Target = target, Context = context, Approved = markApproved};
			AddTranslationUnit(newTu);
			return newTu;
		}

		public Group AddSubGroup(string id)
		{
			if (SubGroups == null)
				SubGroups = new List<Group>();
			var newSubGroup = new Group { Id = id};
			SubGroups.Add(newSubGroup);
			return newSubGroup;
		}

		internal Group FindQuestionGroup(UIDataString data)
		{
			if (!Id.StartsWith(FileBody.kSectionIdPrefix))
				throw new InvalidOperationException("FindQuestionGroup should only be called on a section group.");

			if (SubGroups == null)
				return null; // Localization file might only have a trans-unit (e.g., for the section heading)

			// There will almost always be only one matching "group", but it is possible for the same exact question to
			// be asked in two different verses in a section (and they could have different translations).
			Group bestGroup = null;
			foreach (var group in SubGroups.SelectMany(g => g.SubGroups).Where(qGrp => qGrp.Id.EndsWith(FileBody.kQuestionGroupRefSeparator + data.Question)))
			{
				if (bestGroup == null)
					bestGroup = group;
				else if (!bestGroup.Id.Contains(data.ScriptureReference) && group.Id.Contains(data.ScriptureReference))
					bestGroup = group;
			}

			return bestGroup;
		}
	}
	#endregion

	#region TranslationUnit (in a list owned by Group)
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class TranslationUnit
	{
		private bool m_approved;

		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		// Although the XLIFF 1.2 standard says a Target can have a "signed-off" State, Crowdin indicates
		// this by means of an "approved" attribute on the TranslationUnit itself as well as marking the
		// Target state as "final".
		[XmlAttribute(AttributeName = "approved")]
		[DefaultValue(null)]
		public string ApprovedStr
		{
			get => Approved ? "yes" : null;
			set => Approved = (value == "yes");
		}

		[XmlIgnore]
		public bool Approved
		{
			get => m_approved && Target != null && Target.IsLocalized;
			set => m_approved = value;
		}

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

	#region Localization (the Target  of a Translation Unit)
	[XmlRoot(ElementName = "target")]
	public class Localization
	{
		private const string kFinal = "final";
		private const string kTranslated = "translated";

		private bool m_isLocalized;
		private string m_state;

		[XmlAttribute(AttributeName = "state")]
		public string State
		{
			get => m_state ?? (IsLocalized ? kTranslated : "needs-translation");
			set
			{
				switch (value)
				{
					default: IsLocalized = false; break;
					case kTranslated:
					case kFinal: // This is what we get from Crowdin
						IsLocalized = true; 
						m_state = value;
						break;
					case "signed-off": // Defined in XLIFF 1.2, but no longer used in TXL's XLIFF files
						Approved = true;
						break;
				}
			}
		}

		[XmlIgnore]
		internal bool IsLocalized
		{
			get => m_isLocalized && !String.IsNullOrEmpty(Text);
			set => m_isLocalized = value;
		}

		[XmlText]
		public string Text { get; set; }

		[XmlIgnore]
		public bool Approved
		{
			get => IsLocalized && State == kFinal;
			set
			{
				IsLocalized = true;
				State = kFinal;
			}
		}
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

		internal static string SubQuestionGroupId(this LocalizableStringType type)
		{
			switch (type)
			{
				case LocalizableStringType.Alternate: return FileBody.kAlternatesGroupId;
				case LocalizableStringType.Answer: return FileBody.kAnswersGroupId;
				case LocalizableStringType.Note: return FileBody.kNotesGroupId;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), "Unexpected string type. SubQuestionGroupId only intended for sub-Question types.");
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
