// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2021 to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: HtmlScriptGenerator.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using static System.String;
using static SIL.Transcelerator.Localization.LocalizableStringType;
using static SIL.Transcelerator.TypeOfPhrase;

namespace SIL.Transcelerator
{
	/// <summary>
	/// Class to encapsulate the business logic 
	/// </summary>
	public class HtmlScriptGenerator : ScriptGenerator
	{
		private readonly IScriptGenerationSettings m_settings;
		public const string kDefaultLwc = "en-US";
		public string DefaultVernFont { get; }
		public Func<TranslatablePhrase, ISectionInfo> FindSectionInfo { get; }
		private const string kLwcQuestionClassName = "questionbt";

		public delegate IDataLocalizer DataLocalizerNeededEventHandler(object sender, string localeId);
		public event DataLocalizerNeededEventHandler DataLocalizerNeeded;

		#region Data members
		private List<TranslatablePhrase> m_questionsToInclude;
		private IDataLocalizer m_dataLoc;
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

		public RangeOption GenerateScriptRange
		{
			get => m_settings.GenerateScriptRange;
			set => m_settings.GenerateScriptRange = value;
		}

		public string SelectedBook
		{
			get => m_settings.SelectedBook;
			set 
			{
				m_settings.SelectedBook = value;
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
			get => m_settings.HandlingOfUntranslatedQuestions;
			set => m_settings.HandlingOfUntranslatedQuestions = value;
		}

		public bool OutputPassageForOutOfOrderQuestions
		{
			get => m_settings.OutputPassageForOutOfOrderQuestions;
			set => m_settings.OutputPassageForOutOfOrderQuestions = value;
		}

		public bool OutputFullPassageAtStartOfSection
		{
			get => m_settings.OutputFullPassageAtStartOfSection;
			set => m_settings.OutputFullPassageAtStartOfSection = value;
		}

		public bool OutputScriptureForQuestions
		{
			get => m_settings.OutputScriptureForQuestions;
			set => m_settings.OutputScriptureForQuestions = value;
		}

		public bool IncludeVerseNumbers
		{
			get => m_settings.IncludeVerseNumbers;
			set => m_settings.IncludeVerseNumbers = value;
		}

		public bool IncludeLWCQuestions
		{
			get => m_settings.IncludeLWCQuestions;
			set => m_settings.IncludeLWCQuestions = value;
		}

		public bool IncludeLWCAnswers
		{
			get => m_settings.IncludeLWCAnswers;
			set => m_settings.IncludeLWCAnswers = value;
		}

		public bool IncludeLWCComments
		{
			get => m_settings.IncludeLWCComments;
			set => m_settings.IncludeLWCComments = value;
		}

		public bool UseExternalCss
		{
			get => m_settings.UseExternalCss;
			set => m_settings.UseExternalCss = value;
		}

		public bool WriteCssFile { get; set; }

		public string CssFile
		{
			get => m_settings.CssFile;
			set => m_settings.CssFile = value;
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
			get => m_settings.UseAbsolutePathForCssFile;
			set => m_settings.UseAbsolutePathForCssFile = value;
		}

		public string Folder
		{
			get => m_settings.Folder;
			set => m_settings.Folder = value;
		}
		/// <summary>
		/// The question group headings color as a CSS keyword or RGB hex value (with leading #)
		/// </summary>
		public string QuestionGroupHeadingsTextColor
		{
			get => m_settings.QuestionGroupHeadingsTextColor;
			set => m_settings.QuestionGroupHeadingsTextColor = value;
		}
		/// <summary>
		/// The LWC question color as a CSS keyword or RGB hex value (with leading #)
		/// </summary>
		public string LWCQuestionTextColor
		{
			get => m_settings.LWCQuestionTextColor;
			set => m_settings.LWCQuestionTextColor = value;
		}
		/// <summary>
		/// The LWC answer color as a CSS keyword or RGB hex value (with leading #)
		/// </summary>
		public string LWCAnswerTextColor
		{
			get => m_settings.LWCAnswerTextColor;
			set => m_settings.LWCAnswerTextColor = value;
		}
		/// <summary>
		/// The comment color as a CSS keyword or RGB hex value (with leading #)
		/// </summary>
		public string CommentTextColor
		{
			get => m_settings.CommentTextColor;
			set => m_settings.CommentTextColor = value;
		}
		public int NumberOfBlankLinesForAnswer
		{
			get => m_settings.NumberOfBlankLinesForAnswer;
			set => m_settings.NumberOfBlankLinesForAnswer = value;
		}
		public bool NumberQuestions
		{
			get => m_settings.NumberQuestions;
			set => m_settings.NumberQuestions = value;
		}

        public string LwcLocale
        {
            get => m_settings.LWCLocale;
            set
            {
	            m_settings.LWCLocale = value;
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

		public HtmlScriptGenerator(IScriptGenerationSettings settings,
			string vernIcuLocale, Func<Func<TranslatablePhrase, bool>, IEnumerable<TranslatablePhrase>> source,
			string defaultVernFont,
			Func<TranslatablePhrase, ISectionInfo> findSectionInfo) :
			base(vernIcuLocale, source)
		{
			m_settings = settings;
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

				LocalizedDataString translatedQuestion;
				if (phrase.HasUserTranslation)
					translatedQuestion = new LocalizedDataString(phrase.Translation, VernIcuLocale, false);
				else
				{
					translatedQuestion = GetDataString(phrase.ToUIDataString());
					if (translatedQuestion.Omit)
					{
						if (phrase.AlternateForms == null)
							continue;

						translatedQuestion = null;
						// The question has not been translated and the primary version of it has
						// been omitted by the localizer. But there is still a (slight) chance there
						// may be an alternative form that has been localized to output instead.
						for (var i = 0; i <phrase.AlternateForms.Count(); i++)
						{
							var alt = GetDataString(new UIAlternateDataString(phrase.QuestionInfo,
								i, false));
							if (!alt.Omit && alt.Lang != kDefaultLwc)
							{
								translatedQuestion = alt;
								break;
							}
						}
						if (translatedQuestion != null)
							continue;
					}
				}

				if (!sectionHeadHasBeenOutput)
				{
					if (section == null || !WriteParagraphElement(tw, null,
						    new UISectionHeadDataString(section), VernIcuLocale, "h2"))
					{
						tw.WriteLine($"<h2>{phrase.Reference}</h2>");
					}

					sectionHeadHasBeenOutput = true;
				}

				if (phrase.Category != prevCategory)
				{
					if (OutputFullPassageAtStartOfSection && prevCategory == -1)
						OutputScripture(section.StartRef, section.EndRef);

					var lwcCategoryName = phrase.GetCategoryName(out var lang);
					if (lang == null)
						lang = VernIcuLocale;
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
						if (OutputScriptureForQuestions)
							OutputScripture(startRef, endRef);
					}

					prevQuestionStartRef = phrase.StartRef;
					prevQuestionEndRef = phrase.EndRef;
				}

				tw.WriteLine($"<div class=\"extras\" lang=\"{LwcLocale}\">");
				WriteParagraphElement(tw, "question", translatedQuestion.Normalized, VernIcuLocale,
					translatedQuestion.Lang);
				tw.WriteLine("</div>");

				if (IncludeLWCQuestions && phrase.HasUserTranslation && phrase.TypeOfPhrase != NoEnglishVersion)
					WriteParagraphElement(tw, kLwcQuestionClassName, phrase.ToUIDataString(), LwcLocale);

				if (IncludeLWCAnswers && question.Answers != null)
				{
					for (var index = 0; index < question.Answers.Length; index++)
					{
						var answer = new UIAnswerOrNoteDataString(question, Answer, index);
						WriteParagraphElement(tw, "answer", answer, LwcLocale);
					}
				}

				if (IncludeLWCComments && question.Notes != null)
				{
					for (var index = 0; index < question.Notes.Length; index++)
					{
						var comment = new UIAnswerOrNoteDataString(question, Note, index);
						WriteParagraphElement(tw, "comment", comment, LwcLocale);
					}
				}
			}

			tw.WriteLine("</body>");
		}

		private bool Omit(TranslatablePhrase phrase) => !phrase.HasUserTranslation &&
			(phrase.TypeOfPhrase == NoEnglishVersion ||
				HandlingOfUntranslatedQuestions == HandleUntranslatedQuestionsOption.Skip);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the CSS style info.
		/// </summary>
		/// <param name="tw">The text writer.</param>
		/// <param name="questionGroupHeadingsClr">The question group headings color as a CSS
		/// keyword or RGB hex value (with leading #).</param>
		/// <param name="lwcQuestionClr">The LWC question color as a CSS keyword or RGB hex
		/// value (with leading #).</param>
		/// <param name="lwcAnswerClr">The LWC answer color as a CSS keyword or RGB hex value
		/// (with leading #).</param>
		/// <param name="commentClr">The comment color as a CSS keyword or RGB hex value (with
		/// leading #).</param>
		/// <param name="cBlankLines">The number of blank lines to include in the script to
		/// allow space for writing in answers or other notes (i.e, on a printed script).</param>
		/// <param name="fNumberQuestions">flag indicating whether or not to number questions
		/// sequentially.</param>
		/// ------------------------------------------------------------------------------------
		private void WriteCssStyleInfo(TextWriter tw, string questionGroupHeadingsClr,
			string lwcQuestionClr, string lwcAnswerClr, string commentClr, int cBlankLines,
			bool fNumberQuestions)
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
			tw.WriteLine("h3 {color:" + questionGroupHeadingsClr + ";}");
			tw.WriteLine("." + kLwcQuestionClassName + " {color:" + lwcQuestionClr + ";}");
			tw.WriteLine(".answer {color:" + lwcAnswerClr + ";}");
			tw.WriteLine(".comment {color:" + commentClr + ";}");
			tw.WriteLine(".extras {margin-bottom:" + cBlankLines + "em;}");

			AddProjectSpecificCssEntries?.Invoke(tw);
		}

		private static void WriteParagraphElement(TextWriter sw, string className, string data, string defaultLangInContext, string langOfData, string paragraphType = "p")
		{
			var langAttribute = langOfData == defaultLangInContext ? null : $" lang=\"{langOfData}\"";
			var classAttribute = className == null ? null : $" class=\"{className}\"";
			sw.WriteLine($"<{paragraphType}{classAttribute}{langAttribute}>{data.Normalize(NormalizationForm.FormC)}</{paragraphType}>");
		}

		private bool WriteParagraphElement(TextWriter sw, string className, UIDataString key, string defaultLangInContext, string paragraphType = "p")
		{
			var dataString = GetDataString(key);
			if (dataString.Omit)
				return false;

			var langAttribute = dataString.Lang == defaultLangInContext ? null : $" lang=\"{dataString.Lang}\"";
			var classAttribute = className == null ? null : $" class=\"{className}\"";
			sw.WriteLine($"<{paragraphType}{classAttribute}{langAttribute}>{dataString.Normalized}</{paragraphType}>");

			return true;
		}

		private LocalizedDataString GetDataString(UIDataString key)
		{
			return m_dataLoc == null ? new LocalizedDataString(key.SourceUIString, kDefaultLwc) :
				m_dataLoc.GetLocalizedDataString(key);
		}
	}
}
