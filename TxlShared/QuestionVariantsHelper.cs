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
		public const string kVerseOrBridgeOfVariant = "variantVerses";
		public const string kFirstVariantLetters = "ACEGIKMOQSUWY"; // "odd" letters
		public const string kSecondVariantLetters = "BDFHJLNPRTVXZ"; // "even" letters

		public static string VerseOrBridge = $"(?<{kVerseOrBridgeOfVariant}>\\d+(-\\d+)?)";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a series of questions which make up a pair of variants, gets an object
		/// representing information about the comment that tells the user about avoiding
		/// redundant variants.
		/// </summary>
		/// <remarks>A variant can be an individual question or a series of questions that should
		/// be asked together.</remarks>
		/// ------------------------------------------------------------------------------------
		public static QuestionVariantComment GetCommentAboutAvoidingRedundantQuestions(
			this IEnumerable<Question> questions)
		{
			foreach (var question in questions)
			{
				if (QuestionVariantComment.TryCreate(question, out var questionVariantComment))
					return questionVariantComment;
			}

			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Given a question's variant identifier, gets the letter portion (typically 'A'
		/// or 'B') that identifies the variant to which that question belongs.
		/// </summary>
		/// <remarks>A variant identifier consists of a verse number or bridge followed by the
		/// letter identifying the variant. Within the context of a particular book and chapter,
		/// this uniquely identifies the questions belonging to a variant.</remarks>
		/// ------------------------------------------------------------------------------------
		public static char VariantLetter(this string variantId) => variantId.Last();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the verse number or bridge of the given variant.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static string GetVerseNumber(this string variantId) =>
			variantId.Substring(0, variantId.Length -1);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the variant ID of the variant that should be considered as the other variant
		/// for the variant identified by the given <paramref name="variantId"/>.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string OtherVariantId(this string variantId)
		{
			if (variantId == null)
				throw new ArgumentNullException(nameof(variantId));

			if (variantId.Length < 2)
				throw new ArgumentException($"Not a valid variant ID: {variantId}", nameof(variantId));

			var variantLetter = variantId.VariantLetter();
			char otherVariantLetter;
			try
			{
				otherVariantLetter = variantLetter.OtherVariantLetter();
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Not a valid variant ID.", nameof(variantId), e);
			}
			return $"{variantId.GetVerseNumber()}{otherVariantLetter}";
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
