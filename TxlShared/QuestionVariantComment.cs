// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL International.
// <copyright from='2024' to='2025' company='SIL International'>
//		Copyright (c) 2025, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SIL.Scripture;
using static System.String;
using static SIL.Transcelerator.QuestionVariantsHelper;

namespace SIL.Transcelerator
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Represents information about the comment that tells the user about avoiding redundant
	/// variants when a question or series of questions belongs to a pair of variants.
	/// </summary>
	/// <remarks>This allows for such comments to be localized in a standard way instead of
	/// having to localize each comment individually.</remarks>
	/// ------------------------------------------------------------------------------------
	public class QuestionVariantComment
	{
		private const string kUseEitherSeries = "useEitherSeries";
		private const string kUseEitherQuestion = "useEitherQuestion";
		// NB: Don't use "book" - must be different from the Regex group name used in
		// regexRedundantGroupNoteElement in DataIntegrity_Groups_HaveConsistentReferencesAndLetters
		private const string kBookName = "bookName";
		private const string kChapter = "chapter";
		private const string kCount = "count";
		private const string kThisVariant = "thisVariant";
		private const string kOtherVariant = "otherVariant";
		private const string kFirstVariant = "firstVariant";
		private const string kSecondVariant = "secondVariant";
		public const string kFollowing = "following";

		public static readonly Regex RegexUseOneVariantNote = new Regex(
			$"(?<{kUseEitherSeries}>For (?<{kBookName}>(\\d )?\\w+) ((?<{kChapter}>\\d+):)?{VerseOrBridge}, use either the group " +
			$"(?<{kFirstVariant}>[{kFirstVariantLetters}]) questions or the group (?<{kSecondVariant}>[{kSecondVariantLetters}]) questions\\. " +
			$"It would be redundant to ask all (?<{kCount}>\\d+) questions\\.)|" +
			$"(?<{kUseEitherQuestion}>Use either this question \\((?<{kThisVariant}>[A-Z])\\) or the ((?<{kFollowing}>following)|(preceding)) question " +
			$"\\((?<{kOtherVariant}>[A-Z])\\)\\. It would be redundant to ask both questions\\.)",
			RegexOptions.Compiled);

		private readonly Match m_match;
		public int Index { get; }
		public Question QuestionWithCommentAboutRedundancy { get; }
		public string ScriptureReference =>
			QuestionWithCommentAboutRedundancy?.ScriptureReference ??
			(IsForVariantSeries ? $"{Book} {Chapter}.{Verses}" : null);
		public string Book => IsForVariantSeries ? m_match.Groups[kBookName].Value : null;
		public string Chapter => QuestionWithCommentAboutRedundancy != null ?
			BCVRef.GetChapterFromBcv(QuestionWithCommentAboutRedundancy.StartRef).ToString() :
			IsForVariantSeries ? m_match.Groups[kChapter].Value : null;
		public string Verses => QuestionWithCommentAboutRedundancy != null ?
			QuestionWithCommentAboutRedundancy.ScriptureReference.Split('.')[1] :
			IsForVariantSeries ? m_match.Groups[kVerseOrBridgeOfVariant].Value : null;
		public char ThisVariantLetter { get; }
		public char OtherVariantLetter { get; }
		public char FirstVariantLetter { get; }
		public char SecondVariantLetter { get; }

		public static bool TryCreate(Question question, out QuestionVariantComment questionVariantComment)
		{
			for (var i = 0; i < question.Notes?.Length; i++)
			{
				var match = RegexUseOneVariantNote.Match(question.Notes[i]);
				if (match.Success)
				{
					questionVariantComment = new QuestionVariantComment(match, question, i);
					return true;
				}
			}
			questionVariantComment = null;
			return false;
		}

		/// <summary>
		/// Constructor for testing only.
		/// </summary>
		/// <param name="match">A successful regex match for
		/// <see cref="RegexUseOneVariantNote"/></param>
		internal QuestionVariantComment(Match match) : this(match, null, -1)
		{
			if (!match.Success)
				throw new ArgumentException("Tests should not pass an unsuccessful match to this constructor.");
		}

		private QuestionVariantComment(Match match, Question question, int commentIndex)
		{
			if (match == null || !match.Success)
				throw new ArgumentException("Must be a successful match", nameof(match));
			Debug.Assert(RegexUseOneVariantNote.IsMatch(match.Value));
			m_match = match;
			QuestionWithCommentAboutRedundancy = question;

			if (question == null)
			{
				if (IsForVariantSeries)
				{
					FirstVariantLetter = match.Groups[kFirstVariant].Value.Single();
					SecondVariantLetter = match.Groups[kSecondVariant].Value.Single();
					// In this case, we can't know which variant the associated (unknown) question is for.
				}
				else
				{
					ThisVariantLetter = match.Groups[kThisVariant].Value.Single();
					OtherVariantLetter = match.Groups[kOtherVariant].Value.Single();
					if (OtherVariantLetter != ThisVariantLetter.OtherVariantLetter())
						throw new DataException("Second variant letter should be one greater than the first");
				}
			}
			else
			{
				ThisVariantLetter = question.Group.VariantLetter();
				OtherVariantLetter = ThisVariantLetter.OtherVariantLetter();
			}

			if (ThisVariantLetter != default)
			{
				if (kFirstVariantLetters.IndexOf(ThisVariantLetter) >= 0)
				{
					FirstVariantLetter = ThisVariantLetter;
					SecondVariantLetter = OtherVariantLetter;
				}
				else
				{
					FirstVariantLetter = OtherVariantLetter;
					SecondVariantLetter = ThisVariantLetter;
				}
			}

			Index = commentIndex;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether this represents a comment for a pair of related variants where one or
		/// both of the variants consist of a series of questions (i.e., there are 3 or more
		/// total questions), as opposed to each variant consisting of a single question.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsForVariantSeries => m_match.Groups[kUseEitherSeries].Value != Empty;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the combined total number of questions in both variants.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int NumberOfQuestionsInBothVariants =>
			IsForVariantSeries ? int.Parse(m_match.Groups[kCount].Value) : 2;
	}
}
