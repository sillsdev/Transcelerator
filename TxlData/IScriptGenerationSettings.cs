namespace SIL.Transcelerator
{
	public enum RangeOption
	{
		WholeBook = 0,
		SingleSection = 1,
		RangeOfSections = 2,
	}

	public enum HandleUntranslatedQuestionsOption
	{
		Warn = 0,
		UseLWC = 1,
		Skip = 2,
	}

	public interface IScriptGenerationSettings
	{
		RangeOption GenerateTemplateRange { get; set; }
		string SelectedBook { get; set; }
		HandleUntranslatedQuestionsOption HandlingOfUntranslatedQuestions { get; set; }
		bool OutputPassageForOutOfOrderQuestions { get; set; }
		bool OutputFullPassageAtStartOfSection { get; set; }
		bool IncludeVerseNumbers { get; set; }
		bool IncludeLWCQuestions { get; set; }
		bool IncludeLWCAnswers { get; set; }
		bool IncludeLWCComments { get; set; }
		bool UseExternalCss { get; set; }
		string CssFile { get; set; }
		bool UseAbsolutePathForCssFile { get; set; }
		string Folder { get; set; }
		string QuestionGroupHeadingsTextColor { get; set; }
		string LWCQuestionTextColor { get; set; }
		string LWCAnswerTextColor { get; set; }
		string CommentTextColor { get; set; }
		int NumberOfBlankLinesForAnswer { get; set; }
		bool NumberQuestions { get; set; }
        string LwcLocale { get; set; }
	}
}
