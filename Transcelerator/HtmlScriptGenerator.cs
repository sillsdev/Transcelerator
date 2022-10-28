// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021 to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: HtmlScriptGenerator.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using static System.String;

namespace SIL.Transcelerator
{
	/// <summary>
	/// Class to encapsulate the business logic 
	/// </summary>
	internal class HtmlScriptGenerator : ScriptGenerator
	{
		public const string kDefaultLwc = "en-US";
		public string DefaultVernFont { get; }
		public Func<TranslatablePhrase, ISectionInfo> FindSectionInfo { get; }
		private const string kLwcQuestionClassName = "questionbt";

		public delegate LocalizationsFileAccessor DataLocalizerNeededEventHandler(object sender, string localeId);
		public event DataLocalizerNeededEventHandler DataLocalizerNeeded;

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

		#region Data members
		private List<TranslatablePhrase> m_questionsToInclude;
		private LocalizationsFileAccessor m_dataLoc;
		private BCVRef m_verseRangeStartRef;
		private BCVRef m_verseRangeEndRef;
		private string m_normalizedTitle;
		#endregion

		#region Properties
		public Func<int, int> ChangeVersification { get; set; } = input => input;
		public Action<TextWriter> AddProjectSpecificCssEntries { get; set; }
		// This MUST be set before calling Generate
		public IHtmlScriptureExtractor Extractor { get; set; }

		public string FileName { get; set; }

		/// <summary>
		/// Internal and visible Title of the document. Will be Normalized (composed).
		/// </summary>
		public string Title
		{
			get => m_normalizedTitle;
			set => m_normalizedTitle = value.Normalize(NormalizationForm.FormC);
		}

		public RangeOption GenerateTemplateRange
		{
			get => (RangeOption)Properties.Settings.Default.GenerateTemplateRange;
			set => Properties.Settings.Default.GenerateTemplateRange = (int)value;
		}

		public string SelectedBook
		{
			get => Properties.Settings.Default.GenerateTemplateBook;
			set 
			{
				Properties.Settings.Default.GenerateTemplateBook = value;
				m_questionsToInclude = null;
			}
		}

		public BCVRef VerseRangeStartRef
		{
			get => m_verseRangeStartRef;
			set 
			{
				m_verseRangeStartRef = value;
				m_questionsToInclude = null;
			}
		}
		public BCVRef VerseRangeEndRef
		{
			get => m_verseRangeEndRef;
			
			set 
			{
				m_verseRangeEndRef = value;
				m_questionsToInclude = null;
			}
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

		public bool WriteCssFile { get; set; }

		public string CssFile
		{
			get => Properties.Settings.Default.GenerateTemplateCssFile;
			set => Properties.Settings.Default.GenerateTemplateCssFile = value;
		}

		private string FullCssPath
		{
			get 
			{
				var path = CssFile;
				if (!Path.IsPathRooted(path))
					path = Path.Combine(Folder, path);
				return path;
			}
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

		public Color QuestionGroupHeadingsTextColor
		{
			get => Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor;
			set => Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor = value;
		}
		public Color LWCQuestionTextColor
		{
			get => Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor;
			set => Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor = value;
		}
		public Color LWCAnswerTextColor
		{
			get => Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor;
			set => Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor = value;
		}
		public Color CommentTextColor
		{
			get => Properties.Settings.Default.GenerateTemplateCommentTextColor;
			set => Properties.Settings.Default.GenerateTemplateCommentTextColor = value;
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

		public string LwcLocale
		{
			get => Properties.Settings.Default.GenerateTemplateUseLWC;
			set
			{
				Properties.Settings.Default.GenerateTemplateUseLWC = value;
				m_dataLoc = DataLocalizerNeeded?.Invoke(this, value);
			}
		}

		public IEnumerable<TranslatablePhrase> QuestionsToInclude
		{
			get
			{
				if (m_questionsToInclude == null)
				{
					Func<TranslatablePhrase, bool> inRange;
					var book = SelectedBook;
					if (!IsNullOrEmpty(book))
					{
						int bookNum = BCVRef.BookToNumber(book);
						inRange = tp => BCVRef.GetBookFromBcv(tp.StartRef) == bookNum;
					}
					else
					{
						var startRef = VerseRangeStartRef;
						var endRef = VerseRangeEndRef;
						inRange = tp => tp.StartRef >= startRef && tp.EndRef <= endRef;
					}

					m_questionsToInclude = Source(inRange).ToList();
				}

				return m_questionsToInclude;
			}
		}
		#endregion

		public HtmlScriptGenerator(string vernIcuLocale, Func<Func<TranslatablePhrase, bool>, IEnumerable<TranslatablePhrase>> source,
			string defaultVernFont,
			Func<TranslatablePhrase, ISectionInfo> findSectionInfo) :
			base(vernIcuLocale, source)
		{
			DefaultVernFont = defaultVernFont;
			FindSectionInfo = findSectionInfo;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Generates a script (called if the user clicks OK in the GenerateScriptDlg).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override void Generate(TextWriter tw, string bookId = null)
		{
			tw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
			tw.WriteLine("<html>");
			tw.WriteLine("<head>");
			tw.WriteLine("<meta content=\"text/html; charset=UTF-8\" http-equiv=\"content-type\"/>");
			tw.WriteLine("<title>" + Title + "</title>");
			if (UseExternalCss)
			{
				tw.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href= \"" + CssFile + "\"/>");
				if (WriteCssFile)
				{
					using (StreamWriter css = new StreamWriter(FullCssPath))
					{
						WriteCssStyleInfo(css, QuestionGroupHeadingsTextColor,
							LWCQuestionTextColor, LWCAnswerTextColor,
							CommentTextColor, NumberOfBlankLinesForAnswer,
							NumberQuestions);
					}
				}
			}

			tw.WriteLine("<style type=\"text/css\">");
			// This CSS directive always gets written directly to the template file because it's
			// important to get right and it's unlikely that someone will want to do a global override.
			tw.WriteLine(":lang(" + VernIcuLocale + ") {font-family:" + DefaultVernFont +
				",serif,Arial Unicode MS;}");
			if (!UseExternalCss)
			{
				WriteCssStyleInfo(tw, QuestionGroupHeadingsTextColor,
					LWCQuestionTextColor, LWCAnswerTextColor,
					CommentTextColor, NumberOfBlankLinesForAnswer,
					NumberQuestions);
			}

			tw.WriteLine("</style>");
			tw.WriteLine("</head>");
			tw.WriteLine("<body lang=\"" + VernIcuLocale + "\">");
			// ENHANCE: Support titles in language other than English (the chosen LWC or the vernacular)
			tw.WriteLine($"<h1 lang=\"{kDefaultLwc}\">" + Title + "</h1>");
			int prevCategory = -1;
			int prevSection = -1;
			ISectionInfo section = null;
			var prevQuestionStartRef = -1;
			var prevQuestionEndRef = -1;
			bool sectionHeadHasBeenOutput = false;
			Extractor.IncludeVerseNumbers = IncludeVerseNumbers;

			void OutputScripture(int startRef, int endRef)
			{
				try
				{
					tw.Write(Extractor.GetAsHtmlFragment(startRef, endRef));
				}
				catch (Exception ex)
				{
					tw.Write(ex.Message);
#if DEBUG
					throw;
#endif
				}
			}

			foreach (var phrase in QuestionsToInclude)
			{
				var question = phrase.QuestionInfo;
				string lang;
				if (section == null || phrase.SectionId != prevSection)
				{
					section = FindSectionInfo?.Invoke(phrase);
					sectionHeadHasBeenOutput = false;

					prevSection = phrase.SectionId;
					prevCategory = -1;
					prevQuestionStartRef = -1;
				}

				if (Omit(phrase))
					continue;

				if (!sectionHeadHasBeenOutput)
				{
					if (section == null)
						tw.WriteLine($"<h2>{phrase.Reference}</h2>");
					else
					{
						var h2 = GetDataString(new UISectionHeadDataString(section), out lang);
						WriteParagraphElement(tw, null, h2, VernIcuLocale, lang, "h2");
					}

					sectionHeadHasBeenOutput = true;
				}

				if (phrase.Category != prevCategory)
				{
					if (OutputFullPassageAtStartOfSection && prevCategory == -1)
					{
						OutputScripture(section.StartRef, section.EndRef);
					}

					var lwcCategoryName = GetDataString(new UISimpleDataString(phrase.CategoryName, LocalizableStringType.Category), out lang);
					WriteParagraphElement(tw, null, lwcCategoryName, VernIcuLocale, lang, "h3");

					prevCategory = phrase.Category;
					prevQuestionStartRef = -1;
					prevQuestionEndRef = -1;
				}

				// Questions are allowed to occur out of reference order, but we want them to accurately
				// reflect the range of Scripture to which they pertain. Within the overview section,
				// most questions do not specify a specific verse or range of verses. For the ones that
				// do, we just output a line with the reference. In the detail questions, when we go back
				// and ask a question whose answer comes from a preceding verse, in order to avoid
				// confusion, there is an option to either output the question's reference range (same as
				// for an overview question) or actually output the relevant Scripture passage.
				// Note: we do not do this for "summary" questions -- typically at the end of the detail
				// questions -- since the question itself should make it clear that the question is
				// asking about something that pertains to the whole section (or sometimes even preceding
				// sections).
				var outOfOrderQuestion = phrase.StartRef < prevQuestionStartRef;
				var outputPassage = false;
				if (outOfOrderQuestion)
				{
					// Regardless of category, if the question covers then entire section (i.e., is
					// "verse-specific") we want to basically treat it as an overview question and
					// not output the reference or the text of the passage.
					var verseSpecificQuestion = section == null || phrase.StartRef != section.StartRef
						|| phrase.EndRef != section.EndRef;
					if (verseSpecificQuestion)
					{
						outputPassage = OutputPassageForOutOfOrderQuestions && phrase.Category > 0;
						if (!outputPassage)
						{
							int startRef = ChangeVersification(phrase.StartRef);
							int endRef = ChangeVersification(phrase.EndRef);
							tw.Write("<h4 class=\"summaryRef\">");
							tw.Write(BCVRef.MakeReferenceString(startRef, endRef, ".", "-"));
							tw.WriteLine("</h4>");
						}
					}
				}

				if ((!outOfOrderQuestion && prevQuestionEndRef < phrase.EndRef) || outputPassage)
				{
					if (phrase.Category > 0)
					{
						int startRef = ChangeVersification(phrase.StartRef);
						int endRef = ChangeVersification(phrase.EndRef);
						OutputScripture(startRef, endRef);
					}

					prevQuestionStartRef = phrase.StartRef;
					prevQuestionEndRef = phrase.EndRef;
				}

				lang = VernIcuLocale;
				var questionText = phrase.HasUserTranslation ? phrase.Translation :
					GetDataString(phrase.ToUIDataString(), out lang);
				tw.WriteLine($"<div class=\"extras\" lang=\"{LwcLocale}\">");
				WriteParagraphElement(tw, "question", questionText, VernIcuLocale, lang);
				tw.WriteLine("</div>");

				if (IncludeLWCQuestions && phrase.HasUserTranslation && phrase.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion)
				{
					var lwcQuestion = GetDataString(phrase.ToUIDataString(), out lang);
					WriteParagraphElement(tw, kLwcQuestionClassName, lwcQuestion, LwcLocale, lang);
				}

				if (IncludeLWCAnswers && question.Answers != null)
				{
					for (var index = 0; index < question.Answers.Length; index++)
					{
						var lwcAnswer = GetDataString(new UIAnswerOrNoteDataString(question, LocalizableStringType.Answer, index), out lang);
						WriteParagraphElement(tw, "answer", lwcAnswer, LwcLocale, lang);
					}
				}

				if (IncludeLWCComments && question.Notes != null)
				{
					for (var index = 0; index < question.Notes.Length; index++)
					{
						var lwcComment = GetDataString(new UIAnswerOrNoteDataString(question, LocalizableStringType.Note, index), out lang);
						WriteParagraphElement(tw, "comment", lwcComment, LwcLocale, lang);
					}
				}
			}

			tw.WriteLine("</body>");
		}

		private bool Omit(TranslatablePhrase phrase) => !phrase.HasUserTranslation &&
			(phrase.TypeOfPhrase == TypeOfPhrase.NoEnglishVersion ||
				HandlingOfUntranslatedQuestions == HandleUntranslatedQuestionsOption.Skip);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the CSS style info.
		/// </summary>
		/// <param name="tw">The text writer.</param>
		/// <param name="questionGroupHeadingsClr">The question group headings CLR.</param>
		/// <param name="englishQuestionClr">The english question CLR.</param>
		/// <param name="englishAnswerClr">The english answer CLR.</param>
		/// <param name="commentClr">The comment CLR.</param>
		/// <param name="cBlankLines">The c blank lines.</param>
		/// <param name="fNumberQuestions">if set to <c>true</c> [f number questions].</param>
		/// ------------------------------------------------------------------------------------
		private void WriteCssStyleInfo(TextWriter tw, Color questionGroupHeadingsClr,
			Color englishQuestionClr, Color englishAnswerClr, Color commentClr, int cBlankLines, bool fNumberQuestions)
		{
			if (fNumberQuestions)
			{
				tw.WriteLine("body {font-size:100%; counter-reset:qnum;}");
				tw.WriteLine(".question {counter-increment:qnum;}");
				tw.WriteLine("p.question:before {content:counter(qnum) \". \";}");
			}
			else
				tw.WriteLine("body {font-size:100%;}");
			tw.WriteLine("h1 {font-size:2.0em;");
			tw.WriteLine("  text-align:center}");
			tw.WriteLine("h2 {font-size:1.7em;");
			tw.WriteLine("  color:white;");
			tw.WriteLine("  background-color:black;}");
			tw.WriteLine("h3 {font-size:1.3em;");
			tw.WriteLine("  color:blue;}");
			tw.WriteLine("h4 {font-size:1.1em;}");
			tw.WriteLine("p {font-size:1.0em;}");
			tw.WriteLine("h1:lang(en) {font-family:sans-serif;}");
			tw.WriteLine("h2:lang(en) {font-family:serif;}");
			tw.WriteLine("p:lang(en) {font-family:serif;");
  			tw.WriteLine("font-size:0.85em;}");
			tw.WriteLine(".verse {vertical-align: super; font-size: .80em; color:DimGray;}");
			tw.WriteLine("h3 {color:" + questionGroupHeadingsClr.Name + ";}");
			tw.WriteLine("." + kLwcQuestionClassName + " {color:" + englishQuestionClr.Name + ";}");
			tw.WriteLine(".answer {color:" + englishAnswerClr.Name + ";}");
			tw.WriteLine(".comment {color:" + commentClr.Name + ";}");
			tw.WriteLine(".extras {margin-bottom:" + cBlankLines + "em;}");

			AddProjectSpecificCssEntries?.Invoke(tw);
		}

		private static void WriteParagraphElement(TextWriter sw, string className, string data, string defaultLangInContext, string langOfData, string paragraphType = "p")
		{
			var langAttribute = langOfData == defaultLangInContext ? null : $" lang=\"{langOfData}\"";
			var classAttribute = className == null ? null : $" class=\"{className}\"";
			sw.WriteLine($"<{paragraphType}{classAttribute}{langAttribute}>{data.Normalize(NormalizationForm.FormC)}</{paragraphType}>");
		}

		private string GetDataString(UIDataString key, out string lang)
		{
			if (m_dataLoc == null)
			{
				lang = kDefaultLwc;
				return key.SourceUIString;
			}
			return m_dataLoc.GetLocalizedDataString(key, out lang);
		}
	}
}
