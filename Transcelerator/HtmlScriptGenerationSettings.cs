// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2023 to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System.Drawing;
using System.Linq;
using static System.String;
using static SIL.Transcelerator.Localization.LocalizationsFileAccessor;

namespace SIL.Transcelerator
{
	internal class HtmlScriptGenerationSettings : IScriptGenerationSettings
	{
		public RangeOption GenerateTemplateRange
		{
			get => (RangeOption)Properties.Settings.Default.GenerateTemplateRange;
			set => Properties.Settings.Default.GenerateTemplateRange = (int)value;
		}

		public string SelectedBook
		{
			get => Properties.Settings.Default.GenerateTemplateBook;
			set => Properties.Settings.Default.GenerateTemplateBook = value;
		}

		public HandleUntranslatedQuestionsOption HandlingOfUntranslatedQuestions
		{
			get => (HandleUntranslatedQuestionsOption)Properties.Settings.Default.GenerateHandleUntranslatedQuestionsOption;
			set => Properties.Settings.Default.GenerateHandleUntranslatedQuestionsOption = (int)value;
		}

		public bool OutputPassageForOutOfOrderQuestions
		{
			get => Properties.Settings.Default.GenerateOutputPassageForOutOfOrderQuestions;
			set => Properties.Settings.Default.GenerateOutputPassageForOutOfOrderQuestions = value;
		}

		public bool OutputFullPassageAtStartOfSection
		{
			get => Properties.Settings.Default.GenerateTemplatePassageBeforeOverview;
			set => Properties.Settings.Default.GenerateTemplatePassageBeforeOverview = value;
		}

		public bool IncludeVerseNumbers
		{
			get => Properties.Settings.Default.GenerateIncludeVerseNumbers;
			set => Properties.Settings.Default.GenerateIncludeVerseNumbers = value;
		}

		public bool IncludeLWCQuestions
		{
			get => Properties.Settings.Default.GenerateTemplateEnglishQuestions;
			set => Properties.Settings.Default.GenerateTemplateEnglishQuestions = value;
		}

		public bool IncludeLWCAnswers
		{
			get => Properties.Settings.Default.GenerateTemplateEnglishAnswers;
			set => Properties.Settings.Default.GenerateTemplateEnglishAnswers = value;
		}

		public bool IncludeLWCComments
		{
			get => Properties.Settings.Default.GenerateTemplateIncludeComments;
			set => Properties.Settings.Default.GenerateTemplateIncludeComments = value;
		}

		public bool UseExternalCss
		{
			get => Properties.Settings.Default.GenerateTemplateUseExternalCss;
			set => Properties.Settings.Default.GenerateTemplateUseExternalCss = value;
		}

		public string CssFile
		{
			get => Properties.Settings.Default.GenerateTemplateCssFile;
			set => Properties.Settings.Default.GenerateTemplateCssFile = value;
		}

		public bool UseAbsolutePathForCssFile
		{
			get => Properties.Settings.Default.GenerateTemplateAbsoluteCssPath;
			set => Properties.Settings.Default.GenerateTemplateAbsoluteCssPath = value;
		}

		public string Folder
		{
			get => IsNullOrEmpty(Properties.Settings.Default.GenerateTemplateFolder) ? null :
				Properties.Settings.Default.GenerateTemplateFolder;
			set => Properties.Settings.Default.GenerateTemplateFolder = value;
		}

		public string QuestionGroupHeadingsTextColor
		{
			get => ColorTranslator.ToHtml(Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor);
			set => Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor = ColorTranslator.FromHtml(value);
		}
		public string LWCQuestionTextColor
		{
			get =>  ColorTranslator.ToHtml(Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor);
			set => Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor = ColorTranslator.FromHtml(value);
		}
		public string LWCAnswerTextColor
		{
			get =>  ColorTranslator.ToHtml(Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor);
			set => Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor = ColorTranslator.FromHtml(value);
		}
		public string CommentTextColor
		{
			get => ColorTranslator.ToHtml( Properties.Settings.Default.GenerateTemplateCommentTextColor);
			set => Properties.Settings.Default.GenerateTemplateCommentTextColor = ColorTranslator.FromHtml(value);
		}
		public int NumberOfBlankLinesForAnswer
		{
			get => Properties.Settings.Default.GenerateTemplateBlankLines;
			set => Properties.Settings.Default.GenerateTemplateBlankLines = value;
		}
		public bool NumberQuestions
		{
			get => Properties.Settings.Default.GenerateTemplateNumberQuestions;
			set => Properties.Settings.Default.GenerateTemplateNumberQuestions = value;
		}

        public string LWCLocale
        {
            get 
            {
                if (!IsNullOrEmpty(Properties.Settings.Default.GenerateTemplateUseLWC))
                    return Properties.Settings.Default.GenerateTemplateUseLWC;
                var currentUiLocale = Properties.Settings.Default.OverrideDisplayLanguage;
                return IsNullOrEmpty(currentUiLocale) ||
                    !TxlPlugin.PrimaryLocalizationManager.GetAvailableUILanguageTags().Contains(currentUiLocale) ?
                    "en-US" : currentUiLocale;
            }
            set => Properties.Settings.Default.GenerateTemplateUseLWC =
	            AreEquivalentLocales(Properties.Settings.Default.OverrideDisplayLanguage, value) ? null : value;
        }
	}
}
