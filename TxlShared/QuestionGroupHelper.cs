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
using System.Collections.Generic;
using System.Linq;

namespace SIL.Transcelerator
{
	public static class QuestionGroupHelper
	{
		public const string kVerseOrBridgeGroup = "groupVerses";
		public const string kFirstGroupLetters = "ACEGIKMOQSUWY";
		public const string kSecondGroupLetters = "BDFHJLNPRTVXZ";

		public static string VerseOrBridge = $"(?<{kVerseOrBridgeGroup}>\\d+(-\\d+)?)";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a question which is part of a group, gets a tuple with the match information
		/// and index for the comment that tells the user about avoiding redundant questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static QuestionGroupComment GetCommentAboutAvoidingRedundantQuestions(
			this IEnumerable<Question> questions)
		{
			foreach (var question in questions)
			{
				for (var i = 0; i < question.Notes?.Length; i++)
				{
					var match = QuestionGroupComment.RegexRedundantGroupNote.Match(question.Notes[i]);
					if (match.Success)
						return new QuestionGroupComment(match, question, i);
				}
			}

			return null;
		}

		public static char GroupLetter(this string groupName) => groupName.Last();

		private static string GroupNumericId(this string groupName) => groupName.Substring(0, groupName.Length -1);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the name of the question/group that should be considered as an alternative to
		/// the given group name.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string AlternativeGroup(this string groupName)
		{
			if (groupName == null)
				throw new ArgumentNullException(nameof(groupName));

			if (groupName.Length < 2)
				throw new ArgumentException($"Not a valid group name: {groupName}", nameof(groupName));

			var groupLetter = groupName.GroupLetter();
			char altGroupLetter;
			try
			{
				altGroupLetter = groupLetter.AlternativeGroupLetter();
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Not a valid group name.", nameof(groupName), e);
			}
			return $"{groupName.GroupNumericId()}{altGroupLetter}";
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the name of the question/group that should be considered as an alternative to
		/// the given group name.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static char AlternativeGroupLetter(this char groupLetter)
		{
			var i = kFirstGroupLetters.IndexOf(groupLetter);
			if (i >= 0)
				return kSecondGroupLetters[i];
			i = kSecondGroupLetters.IndexOf(groupLetter);
			if (i >= 0)
				return kFirstGroupLetters[i];
			throw new ArgumentException("Not a valid group letter.", nameof(groupLetter));
		}
	}
}
