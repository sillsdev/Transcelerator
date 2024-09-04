// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SIL.Scripture;
using static System.String;
using static SIL.Transcelerator.QuestionGroupHelper;

namespace SIL.Transcelerator
{
	public class QuestionGroupComment
	{
		private const string kUseEitherGroup = "useEitherGroup";
		private const string kUseEitherQuestion = "useEitherQuestion";
		// NB: Don't use "book" - must be different from the group name used in
		// regexRedundantGroupNoteElement in DataIntegrity_Groups_HaveConsistentReferencesAndLetters
		private const string kBookName = "bookName";
		private const string kChapter = "chapter";
		private const string kCount = "count";
		private const string kThisGroup = "thisGroup";
		private const string kOtherGroup = "otherGroup";
		private const string kFirstGroup = "firstGroup";
		private const string kSecondGroup = "secondGroup";
		public const string kFollowing = "following";

		public static readonly Regex RegexRedundantGroupNote = new Regex($"(?<{kUseEitherGroup}>For (?<{kBookName}>(\\d )?\\w+) ((?<{kChapter}>\\d+):)?{VerseOrBridge}, use either the group " +
			$"(?<{kFirstGroup}>[{kFirstGroupLetters}]) questions or the group (?<{kSecondGroup}>[{kSecondGroupLetters}]) questions\\. " +
			$"It would be redundant to ask all (?<{kCount}>\\d+) questions\\.)|" +
			$"(?<{kUseEitherQuestion}>Use either this question \\((?<{kThisGroup}>[A-Z])\\) or the ((?<{kFollowing}>following)|(preceding)) question \\((?<{kOtherGroup}>[A-Z])\\)\\. " +
			"It would be redundant to ask both questions\\.)", RegexOptions.Compiled);

		private readonly Match m_match;
		public int Index { get; }
		public Question QuestionWithCommentAboutRedundancy { get; }
		public string ScriptureReference =>
			QuestionWithCommentAboutRedundancy?.ScriptureReference ??
			(IsForGroup ? $"{Chapter}.{Verses}" : null);
		public string Book => IsForGroup ? m_match.Groups[kBookName].Value : null;
		public string Chapter => QuestionWithCommentAboutRedundancy != null ?
			BCVRef.GetChapterFromBcv(QuestionWithCommentAboutRedundancy.StartRef).ToString() :
			IsForGroup ? m_match.Groups[kChapter].Value : null;
		public string Verses => QuestionWithCommentAboutRedundancy != null ?
			QuestionWithCommentAboutRedundancy.ScriptureReference.Split('.')[1] :
			IsForGroup ? m_match.Groups[kVerseOrBridgeGroup].Value : null;
		public char ThisGroupLetter { get; }
		public char OtherGroupLetter { get; }
		public char FirstGroupLetter { get; }
		public char SecondGroupLetter { get; }

		public QuestionGroupComment(Match match, Question question = null, int commentIndex = -1)
		{
			if (match == null || !match.Success)
				throw new ArgumentException("Must be a successful match", nameof(match));
			Debug.Assert(RegexRedundantGroupNote.IsMatch(match.Value));
			m_match = match;
			QuestionWithCommentAboutRedundancy = question;

			if (question == null)
			{
				if (IsForGroup)
				{
					FirstGroupLetter = match.Groups[kFirstGroup].Value.Single();
					SecondGroupLetter = match.Groups[kSecondGroup].Value.Single();
					// In this case, we can't know which group the associated (unknown) question is for.
				}
				else
				{
					ThisGroupLetter = match.Groups[kThisGroup].Value.Single();
					OtherGroupLetter = match.Groups[kOtherGroup].Value.Single();
					if (OtherGroupLetter != ThisGroupLetter.AlternativeGroupLetter())
						throw new DataException("Second group letter should be one greater than the first");
				}
			}
			else
			{
				ThisGroupLetter = question.Group.GroupLetter();
				OtherGroupLetter = ThisGroupLetter.AlternativeGroupLetter();
			}

			if (ThisGroupLetter != default)
			{
				if (kFirstGroupLetters.IndexOf(ThisGroupLetter) >= 0)
				{
					FirstGroupLetter = ThisGroupLetter;
					SecondGroupLetter = OtherGroupLetter;
				}
				else
				{
					FirstGroupLetter = OtherGroupLetter;
					SecondGroupLetter = ThisGroupLetter;
				}
			}

			Index = commentIndex;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether this is a comment for a group of related questions (as opposed to
		/// a single question).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsForGroup => m_match.Groups[kUseEitherGroup].Value != Empty;

		public int NumberOfQuestionsInBothGroups =>
			IsForGroup ? int.Parse(m_match.Groups[kCount].Value) : 2;
	}
}
