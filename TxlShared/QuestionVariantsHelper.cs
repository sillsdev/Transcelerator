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
using System.Collections.Generic;
using System.Linq;

namespace SIL.Transcelerator
{
	public static class QuestionVariantsHelper
	{
		public const string kVerseOrBridgeGroup = "groupVerses";
		public const string kFirstVariantLetters = "ACEGIKMOQSUWY"; // "odd" letters
		public const string kSecondVariantLetters = "BDFHJLNPRTVXZ"; // "even" letters

		public static string VerseOrBridge = $"(?<{kVerseOrBridgeGroup}>\\d+(-\\d+)?)";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a series of questions which make up a pair of variants, gets an object
		/// representing information about the comment that tells the user about avoiding
		/// redundant variants.
		/// </summary>
		/// <remarks>A variant can be an individual question or a group of questions that should
		/// be asked together (in sequence).</remarks>
		/// ------------------------------------------------------------------------------------
		public static QuestionVariantComment GetCommentAboutAvoidingRedundantQuestions(
			this IEnumerable<Question> questions)
		{
			foreach (var question in questions)
			{
				if (QuestionVariantComment.TryCreate(question, out var questionGroupComment))
					return questionGroupComment;
			}

			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a question's group identifier, gets the letter portion (typically 'A' or 'B')
		/// that identifies the variant to which that question belongs.
		/// </summary>
		/// <remarks>A group identifier consists of a verse number or bridge, followed by the
		/// letter identifying the group. Within the context of a particular book and chapter,
		/// this uniquely identifies the questions belonging to a variant.</remarks>
		/// ------------------------------------------------------------------------------------
		public static char VariantLetter(this string groupId) => groupId.Last();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the verse number or bridge of the given group.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static string GroupNumericId(this string groupId) =>
			groupId.Substring(0, groupId.Length -1);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the group ID of the variant that should be considered as the other variant
		/// to the given group ID.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string OtherVariantId(this string groupId)
		{
			if (groupId == null)
				throw new ArgumentNullException(nameof(groupId));

			if (groupId.Length < 2)
				throw new ArgumentException($"Not a valid group name: {groupId}", nameof(groupId));

			var groupLetter = groupId.VariantLetter();
			char altGroupLetter;
			try
			{
				altGroupLetter = groupLetter.OtherVariantLetter();
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Not a valid group name.", nameof(groupId), e);
			}
			return $"{groupId.GroupNumericId()}{altGroupLetter}";
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a variant letter, gets the letter corresponding to the other variant in the pair.
		/// For example, the other variant letter for 'A' is 'B', and the other variant
		/// letter for 'D' is 'C'.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static char OtherVariantLetter(this char variantLetter)
		{
			var i = kFirstVariantLetters.IndexOf(variantLetter);
			if (i >= 0)
				return kSecondVariantLetters[i];
			i = kSecondVariantLetters.IndexOf(variantLetter);
			if (i >= 0)
				return kFirstVariantLetters[i];
			throw new ArgumentException("Not a valid variant letter.", nameof(variantLetter));
		}
	}
}
