// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2024' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.Transcelerator.Localization;
using static System.String;
using static SIL.Transcelerator.Localization.LocalizableStringType;

namespace SIL.Transcelerator
{
	/// -------------------------------------------------------------------------------------------
	/// <summary>
	/// Little dialog box to alert the user about a question/phrase that is part of a variant pair
	/// for which they have not apparently made a decision about which variant to use.
	/// </summary>
	/// -------------------------------------------------------------------------------------------
	public partial class VariantQuestionInfoDlg : Form
	{
		private readonly IEnumerable<Question> m_groupedQuestions;
		private readonly LocalizationsFileAccessor m_dataLocalizer;

		/// <summary>
		/// Constructs a dialog box to alert the user about a question/phrase that is part of a
		/// variant
		/// </summary>
		/// <param name="variantGroupQuestions">All the questions in a particular variant</param>
		/// <param name="dataLocalizer">Optional localizer object that knows how to get localized
		/// "data" strings (i.e., comments in the questions data that may have been translated by a
		/// localizer).</param>
		public VariantQuestionInfoDlg(IEnumerable<Question> variantGroupQuestions,
			LocalizationsFileAccessor dataLocalizer)
		{
			m_groupedQuestions = variantGroupQuestions;
			m_dataLocalizer = dataLocalizer;
			InitializeComponent();

			pictInfoIcon.Image = SystemIcons.Information.ToBitmap();

			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
		}
		
		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;
			lblVariantQuestionInfo.Text = GetMessage();
		}

		/// <summary>
		/// Gets a message about the variant to which the given phrase belongs which is suitable for
		/// UI display (i.e., localized if needed).
		/// </summary>
		/// <exception cref="InvalidOperationException">if <see cref="m_groupedQuestions"/>
		/// does not represent a question group</exception>
		private string GetMessage()
		{
			var comment = m_groupedQuestions.GetCommentAboutAvoidingRedundantQuestions();
			if (comment == null)
			{
				throw new InvalidOperationException($"The {nameof(VariantQuestionInfoDlg)} " +
					"should only be displayed for questions that are part of a group (and which " +
					"therefore should have a comment about avoiding redundant questions.");
			}
			// There are two possible sources for a localized version of this comment: A generic
			// version with parameters or a specific version for the specific comment. For
			// historical reasons, the specific version will perhaps be more likely, though down
			// the road the other may become more prevalent. It might be best to go for consistency
			// and use the generic version if present, but it could also be argued that if a
			// specific localization is available, it is more likely to be better, especially since
			// that would allow for localizing the group ID letter when the target language uses a
			// non-Roman script. My guess is that it will seldom, if ever, matter.
			// Note that I decided to add "For {0}," to the start of the AvoidRedundantQuestionsFmt
			// version and simplified the rest of the text to make it more consistent with
			// AvoidRedundantQuestionSetsFmt and also eliminate the need for two different versions
			// of it (one for the first variant question and one for the second variant question).

			var localizedComment = m_dataLocalizer?.GetLocalizedDataString(new UIAnswerOrNoteDataString(
				comment.QuestionWithCommentAboutRedundancy, Note, comment.Index));
			if (localizedComment != null && localizedComment.Lang != LocalizationManager.kDefaultLang)
				return localizedComment.Data;

			string fmt = comment.IsForVariantSeries ?
				LocalizationManager.GetString("VariantQuestionInfoDlg.AvoidRedundantQuestionSetsFmt",
					"For {0}, use either the group {1} questions or the group {2} questions. It would be redundant to ask all {3} questions.",
					"Param 0: Scripture reference of the variant questions; " +
					"Param 1: Letter identifying the first variant (e.g., 'A'); " +
					"Param 2: Letter identifying the second variant (e.g., 'B'); " +
					"Param 3: The total number of questions in both variants") :
				LocalizationManager.GetString("VariantQuestionInfoDlg.AvoidRedundantQuestionsFmt",
					"For {0}, use either question {1} or question {2}. It would be redundant to ask both questions.",
					"Param 0: Scripture reference of the variant questions; " +
					"Param 1: Letter identifying the first variant question (e.g., 'A'); " +
					"Param 2: Letter identifying the second variant question (e.g., 'B')");

			return Format(fmt, comment.ScriptureReference, comment.FirstVariantLetter,
				comment.SecondVariantLetter, comment.NumberOfQuestionsInBothVariants);
		}

		public bool DoNotShowFutureVariantWarnings => chkDoNotShowAgain.Checked;
	}
}
