// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL International.
// <copyright from='2011' to='2025' company='SIL International'>
//		Copyright (c) 2025, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: GenerateScriptDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.Extensions;
using SIL.Scripture;
using SIL.Windows.Forms;
using SIL.Windows.Forms.Miscellaneous;
using static System.String;
using File = System.IO.File;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog to present user with options to generate a script to do comprehension checking.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class GenerateScriptDlg : ParentFormBase
	{
		#region Data members
		private string m_sFilenameTemplate;
		private string m_sTitleTemplate;
		private readonly IList<ISectionInfo> m_sections;
		private readonly HtmlScriptGenerator m_generator;
		private readonly string m_projectName;
		private Func<int, int, bool> DoesDataExistInProject { get; }
		private List<string> m_lwcLocaleIds;
		private string m_fmtChkEnglishQuestions;
		private string m_fmtChkEnglishAnswers;
		private string m_fmtChkIncludeComments;
		private string m_fmLWCAnswerTextColor;
		private string m_fmLWCQuestionTextColor;
		private readonly string m_help;
		#endregion

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal GenerateScriptDlg(string projectName, string defaultFolder,
			Func<int, int, bool> doesDataExistInProject, IList<ISectionInfo> sections,
			IEnumerable<KeyValuePair<string, string>> availableAdditionalLWCs,
			HtmlScriptGenerator generator)
		{
			m_projectName = projectName;
			DoesDataExistInProject = doesDataExistInProject;
			InitializeComponent();
			m_chkIncludeLWCQuestions.Tag = m_btnChooseLWCQuestionColor;
			m_chkIncludeLWCAnswers.Tag = m_btnChooseLWCAnswerColor;
			m_chkIncludeLWCComments.Tag = m_btnChooseCommentColor;
			btnChooseQuestionGroupHeadingsColor.Tag = m_lblQuestionGroupHeadingsColor;
			m_btnChooseLWCQuestionColor.Tag = m_lblLWCQuestionColor;
			m_btnChooseLWCAnswerColor.Tag = m_lblLWCAnswerTextColor;
			m_btnChooseCommentColor.Tag = m_lblCommentTextColor;
			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;

			LoadBooks();
			m_sections = sections;
			m_generator = generator;
			LoadSectionCombos();
			LoadLWCCombo(availableAdditionalLWCs);

			switch (m_generator.GenerateScriptRange)
			{
				case RangeOption.WholeBook:
					m_rdoWholeBook.Checked = true;
					TrySelectItem(m_cboBooks, m_generator.SelectedBook);
					break;
				case RangeOption.SingleSection:
					m_rdoSingleSection.Checked = true;
					TrySelectItem(m_cboSection, Properties.Settings.Default.GenerateTemplateSection);
					break;
				case RangeOption.RangeOfSections:
					m_rdoSectionRange.Checked = true;
					TrySelectItem(m_cboStartSection, Properties.Settings.Default.GenerateTemplateSection);
					TrySelectItem(m_cboEndSection, Properties.Settings.Default.GenerateTemplateEndSection);
					break;
			}

			m_chkPassageBeforeOverview.Checked = m_generator.OutputFullPassageAtStartOfSection;
			m_chkIncludeVerseNumbers.Checked = m_generator.IncludeVerseNumbers;
			m_chkIncludeScriptureForQuestions.Checked = m_generator.OutputScriptureForQuestions;

			SetDefaultCheckedStateForLWCOptions();

			switch (m_generator.HandlingOfUntranslatedQuestions)
			{
				case HandleUntranslatedQuestionsOption.Warn:
					m_rdoDisplayWarning.Checked = true;
					break;
				case HandleUntranslatedQuestionsOption.UseLWC:
					m_rdoUseOriginal.Checked = true;
					break;
				case HandleUntranslatedQuestionsOption.Skip:
					m_rdoSkipUntranslated.Checked = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			m_rdoOutputPassageForOutOfOrderQuestions.Checked = m_generator.OutputPassageForOutOfOrderQuestions;

			m_lblFolder.Text = m_generator.Folder ?? defaultFolder;

			m_numBlankLines.Value = m_generator.NumberOfBlankLinesForAnswer;
			if (!IsNullOrEmpty(m_generator.QuestionGroupHeadingsTextColor))
				m_lblQuestionGroupHeadingsColor.ForeColor = ColorTranslator.FromHtml(m_generator.QuestionGroupHeadingsTextColor);
			if (!IsNullOrEmpty(m_generator.LWCQuestionTextColor))
				m_lblLWCQuestionColor.ForeColor = ColorTranslator.FromHtml(m_generator.LWCQuestionTextColor);
			if (!IsNullOrEmpty(m_generator.LWCAnswerTextColor))
				m_lblLWCAnswerTextColor.ForeColor = ColorTranslator.FromHtml(m_generator.LWCAnswerTextColor);
			if (!IsNullOrEmpty(m_generator.CommentTextColor))
				m_lblCommentTextColor.ForeColor = ColorTranslator.FromHtml(m_generator.CommentTextColor);
			m_chkNumberQuestions.Checked = m_generator.NumberQuestions;

			m_rdoUseExternalCss.Checked = m_generator.UseExternalCss;
			if (m_rdoUseExternalCss.Checked)
			{
				m_txtCssFile.Text = m_generator.CssFile;
				m_chkAbsoluteCssPath.Checked = m_generator.UseAbsolutePathForCssFile;
			}
			UpdateControlsForCssFile(m_txtCssFile.Text);

			m_help = TxlPlugin.GetHelpFile("generatescript");
			HelpButton = !IsNullOrEmpty(m_help);
			
			m_numBlankLines.ValueChanged += delegate
			{ 
				m_chkOverwriteCss.Checked |= m_chkOverwriteCss.Enabled && !m_numBlankLines.Value.Equals(m_generator.NumberOfBlankLinesForAnswer);
			};

			m_lblCommentTextColor.Tag = new Func<Color>(() => ColorTranslator.FromHtml(m_generator.CommentTextColor));
			m_lblCommentTextColor.ForeColorChanged += OnLblForeColorChanged;
			m_lblLWCAnswerTextColor.Tag = new Func<Color>(() => ColorTranslator.FromHtml(m_generator.LWCAnswerTextColor));
			m_lblLWCAnswerTextColor.ForeColorChanged += OnLblForeColorChanged;
			m_lblLWCQuestionColor.Tag = new Func<Color>(() => ColorTranslator.FromHtml(m_generator.LWCQuestionTextColor));
			m_lblLWCQuestionColor.ForeColorChanged += OnLblForeColorChanged;
			m_lblQuestionGroupHeadingsColor.Tag = new Func<Color>(() => ColorTranslator.FromHtml(m_generator.QuestionGroupHeadingsTextColor));
			m_lblQuestionGroupHeadingsColor.ForeColorChanged += OnLblForeColorChanged;
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;
			m_fmtChkEnglishQuestions = m_chkIncludeLWCQuestions.Text;
			m_fmtChkEnglishAnswers = m_chkIncludeLWCAnswers.Text;
			m_fmtChkIncludeComments = m_chkIncludeLWCComments.Text;
			m_sTitleTemplate = m_txtTitle.Text;
			m_sFilenameTemplate = Format(m_txtFilename.Text, m_projectName, "{0}");
			m_fmLWCAnswerTextColor = m_lblLWCAnswerTextColor.Text;
			m_fmLWCQuestionTextColor = m_lblLWCQuestionColor.Text;
		}

		private void SetDefaultCheckedStateForLWCOptions()
		{
			m_chkIncludeLWCQuestions.Checked = m_generator.IncludeLWCQuestions;
			m_chkIncludeLWCAnswers.Checked = m_generator.IncludeLWCAnswers;
			m_chkIncludeLWCComments.Checked = m_generator.IncludeLWCComments;
		}

		private void TrySelectItem(ComboBox cbo, string value)
		{
			int i = cbo.FindStringExact(value);
			if (i >= 0)
				cbo.SelectedIndex = i;
		}

		IEnumerable<int> GetBookIdsToInclude()
		{
			for (var i = 1; i <= BCVRef.LastBook; i++)
			{
				if (m_chkShowUnavailable.Checked || DoesDataExistInProject(i, 0))
					yield return i;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the combo box with the appropriate book ids
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadBooks()
		{
			foreach (int canonicalBookId in GetBookIdsToInclude())
			{
				string bookCode = BCVRef.NumberToBookCode(canonicalBookId);
				m_cboBooks.Items.Add(bookCode);
			}
			if (m_cboBooks.Items.Count > 0)
				m_cboBooks.SelectedIndex = 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the three combo boxes with section information (note that these are not the
		/// section heads in the vernacular Scripture but rather from the master question file).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadSectionCombos()
		{
			m_cboSection.Items.Clear();
			m_cboStartSection.Items.Clear();
			m_cboEndSection.Items.Clear();

			IEnumerable<ISectionInfo> sectionsToInclude;
			if (m_chkShowUnavailable.Checked)
				sectionsToInclude = m_sections;
			else
			{
				var bookIds = new HashSet<int>(GetBookIdsToInclude());
				var list = new List<ISectionInfo>();
				foreach (var sectionInfo in m_sections)
				{
					var bcvStart = new BCVRef(sectionInfo.StartRef);
					if (bookIds.Contains(bcvStart.Book))
					{
						for (int c = bcvStart.Chapter; c <= BCVRef.GetChapterFromBcv(sectionInfo.EndRef); c++)
						{
							if (DoesDataExistInProject(bcvStart.Book, c))
							{
								list.Add(sectionInfo);
								break;
							}
						}
					}
				}

				sectionsToInclude = list;
			}

			using (new WaitCursor(this))
			{
				foreach (var sectionInfo in sectionsToInclude)
				{
					m_cboSection.Items.Add(sectionInfo);
					m_cboStartSection.Items.Add(sectionInfo);
					m_cboEndSection.Items.Add(sectionInfo);
				}
			}
		}

		private void LoadLWCCombo(IEnumerable<KeyValuePair<string, string>> availableAdditionalLWCs)
		{
			m_lwcLocaleIds = new List<string>(new [] { HtmlScriptGenerator.kDefaultLwc });
			int i = 0;
			foreach (var lwc in availableAdditionalLWCs)
			{
				m_cboUseLWC.Items.Insert(++i, lwc.Key);
				m_lwcLocaleIds.Add(lwc.Value);
				if (m_generator.LwcLocale == lwc.Value)
					m_cboUseLWC.SelectedIndex = i;
			}
			if (IsNullOrWhiteSpace(m_generator.LwcLocale))
				m_cboUseLWC.SelectedIndex = m_cboUseLWC.Items.Count - 1; // None
			if (m_cboUseLWC.SelectedIndex == -1)
				m_cboUseLWC.SelectedIndex = 0;
		}
		#endregion

		#region Properties
		public string SelectedBook => m_rdoWholeBook.Checked ? (string)m_cboBooks.SelectedItem : null;

		// ENHANCE: It's less than ideal to provide the selected "sections" in terms of
		// start and end refs because there are a couple sections that start/end mid-verse.
		// If the user selects one of these as a single section or starts/ends the section
		// range in one of these, the generated script is going to include one of more
		// questions from the adjacent section. From a task-oriented perspective this is
		// unlikely. Unfortunately, because the section IDs are not ascending with
		// respect to the canonical order of Scripture books, they cannot be treated as
		// indices for the purpose of establishing a range, and although the sections know
		// which questions they own, the questions themselves do not hold their section IDs.
		// The corresponding TranslatablePhrase objects hold that information. To fix this
		// would require either a fairly expensive lookup:
		// (((Section)sectionInfo).Categories[0].Questions[0] --- helper ---> phrase.SectionId
		// OR a change to QuestionProvider to supply the questions in canonical order.
		// To make this easier, it would obviously make sense to just re-order the books in
		// the source question file. But I'm not sure if this would wreak havoc on the
		// localization files (or perhaps something else).
		private BCVRef VerseRangeStartRef { get; set; } = new BCVRef();

		private BCVRef VerseRangeEndRef { get; set; } = new BCVRef();
		#endregion

		#region Event handlers
		private void OnLblForeColorChanged(object sender, EventArgs e)
		{
			Label lbl = (Label)sender;
			m_chkOverwriteCss.Checked |= m_chkOverwriteCss.Enabled && lbl.ForeColor != ((Func<Color>)lbl.Tag)();
		}

		private void ChooseTextColor(object sender, EventArgs e)
		{
			Label label = (Label)((Button)sender).Tag;
			colorDlg.Color = label.ForeColor;
			if (colorDlg.ShowDialog() == DialogResult.OK)
				label.ForeColor = colorDlg.Color;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog())
			{
				dlg.AddExtension = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = "." + Path.GetExtension(m_txtFilename.Text);
				var description = LocalizationManager.GetString("GenerateScriptDlg.HtmlFileDescription", "Web Page");
				dlg.Filter = Format("{0} (*{1})|*{1}", description, dlg.DefaultExt);
				dlg.FilterIndex = 0;
				dlg.OverwritePrompt = true;
				dlg.InitialDirectory = m_lblFolder.Text;
				dlg.FileName = m_txtFilename.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					m_txtFilename.Text = Path.GetFileName(dlg.FileName);
					m_lblFolder.Text = Path.GetDirectoryName(dlg.FileName);
				}
			}
		}

		private void UpdateTitleAndFilenameForSelectedBook(object sender, EventArgs e)
		{
			if (m_cboBooks.SelectedIndex < 0)
				return;
			string sBook = (string)m_cboBooks.SelectedItem;
			UpdateTextBoxWithSelectedPassage(m_txtFilename, sBook, m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, sBook, m_sTitleTemplate);
		}

		private void UpdateTitleAndFilenameForSingleSection(object sender, EventArgs e)
		{
			UpdateOkButtonEnabledState();
			if (!(m_cboSection.SelectedItem is Section info))
				return;
			VerseRangeStartRef = new BCVRef(info.StartRef);
			VerseRangeEndRef = new BCVRef(info.EndRef);
			UpdateTextBoxWithSelectedPassage(m_txtFilename, info.Heading.SanitizeFilename('_', true), m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, info.Heading, m_sTitleTemplate);
		}

		private static void UpdateTextBoxWithSelectedPassage(TextBox txt, string passage, string fmt)
		{
			if (txt.Tag == null || txt.Text == (string)txt.Tag)
				txt.Tag = txt.Text = Format(fmt, passage);
		}

		private void IncludeOptionCheckedChanged(object sender, EventArgs e)
		{
			CheckBox clickedBox = (CheckBox)sender;
			((Button)clickedBox.Tag).Enabled = clickedBox.Checked;
		}

		private void ColorSelectionButtonEnabledStateChanged(object sender, EventArgs e)
		{
			Control button = (Control)sender;
			((Label)button.Tag).Enabled = button.Enabled;
		}

		private void m_numBlankLines_EnabledChanged(object sender, EventArgs e)
		{
			if (m_numBlankLines.Enabled)
			{
				if (m_numBlankLines.Value == 0)
					m_numBlankLines.Value = 1;
				m_numBlankLines.Minimum = 1;
			}
		}

		private void m_rdoUseExternalCss_CheckedChanged(object sender, EventArgs e)
		{
			m_tableLayoutPanelCssOptions.Visible = m_rdoUseExternalCss.Checked;
		}

		private void btnBrowseCss_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog())
			{
				string folder = Path.GetDirectoryName(m_txtCssFile.Text);
				if (IsNullOrEmpty(folder))
					folder = m_lblFolder.Text;
				dlg.AddExtension = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = ".css";
				dlg.Filter = Format("Cascading Style Sheet ({0})|{0}", "*.css");
				dlg.FilterIndex = 0;
				dlg.OverwritePrompt = false;
				dlg.InitialDirectory = folder;
				dlg.FileName = m_txtCssFile.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					string file = dlg.FileName;
					m_txtCssFile.Text = Path.GetDirectoryName(file) == m_lblFolder.Text ? Path.GetFileName(file) : file;
				}
			}
		}

		private void m_txtCssFile_TextChanged(object sender, EventArgs e)
		{
			UpdateControlsForCssFile(m_txtCssFile.Text);
		}

		private void UpdateControlsForCssFile(string path)
		{
			if (!Path.IsPathRooted(path))
				path = Path.Combine(m_lblFolder.Text, path);
			else
			{
				if (Path.GetDirectoryName(path) != m_lblFolder.Text)
					m_chkAbsoluteCssPath.Checked = true;
			}

			m_chkOverwriteCss.Enabled = File.Exists(path);
		}

		private void m_cboBooks_Enter(object sender, EventArgs e)
		{
			if (!m_rdoWholeBook.Checked)
				m_rdoWholeBook.Checked = true;
		}

		private void m_cboSection_Enter(object sender, EventArgs e)
		{
			if (!m_rdoSingleSection.Checked)
				m_rdoSingleSection.Checked = true;
		}

		private void SectionRangeCombo_Enter(object sender, EventArgs e)
		{
			if (!m_rdoSectionRange.Checked)
				m_rdoSectionRange.Checked = true;
		}

		private void m_cboStartSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_cboStartSection.SelectedIndex > m_cboEndSection.SelectedIndex)
				m_cboEndSection.SelectedIndex = m_cboStartSection.SelectedIndex;
			if (UpdateSectionRangeStartRef())
				UpdateTitleAndFilenameForSectionRange();
			UpdateOkButtonEnabledState();
		}

		private void m_cboEndSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_cboStartSection.SelectedIndex > m_cboEndSection.SelectedIndex)
				m_cboStartSection.SelectedIndex = m_cboEndSection.SelectedIndex;
			if (UpdateSectionRangeEndRef())
				UpdateTitleAndFilenameForSectionRange();
			UpdateOkButtonEnabledState();
		}

		private void m_rdoSectionRange_CheckedChanged(object sender, EventArgs e)
		{
			if (m_rdoSectionRange.Checked && UpdateSectionRangeStartRef() && UpdateSectionRangeEndRef())
				UpdateTitleAndFilenameForSectionRange();
			UpdateOkButtonEnabledState();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (m_rdoWholeBook.Checked)
			{
				m_generator.GenerateScriptRange = RangeOption.WholeBook;
				m_generator.SelectedBook = m_cboBooks.SelectedItem.ToString();
			}
			else
			{
				m_generator.SelectedBook = null;

				if (m_rdoSingleSection.Checked)
				{
					m_generator.GenerateScriptRange = RangeOption.SingleSection;
					Properties.Settings.Default.GenerateTemplateSection = m_cboSection.SelectedItem.ToString();
					Properties.Settings.Default.GenerateTemplateEndSection = "";
				}
				else
				{
					m_generator.GenerateScriptRange = RangeOption.RangeOfSections;
					Properties.Settings.Default.GenerateTemplateSection = m_cboStartSection.SelectedItem.ToString();
					Properties.Settings.Default.GenerateTemplateEndSection = m_cboEndSection.SelectedItem.ToString();
				}

				m_generator.VerseRangeStartRef = VerseRangeStartRef;
				m_generator.VerseRangeEndRef = VerseRangeEndRef;
			}

			var path = Path.Combine(m_lblFolder.Text, m_txtFilename.Text);
			if (!Path.HasExtension(path))
				path = Path.ChangeExtension(path, "htm");
			m_generator.FileName = path;

			m_generator.OutputFullPassageAtStartOfSection = m_chkPassageBeforeOverview.Checked;
			m_generator.IncludeVerseNumbers = m_chkIncludeVerseNumbers.Checked;
			m_generator.OutputScriptureForQuestions= m_chkIncludeScriptureForQuestions.Checked;

			m_generator.QuestionGroupHeadingsTextColor = ColorTranslator.ToHtml(m_lblQuestionGroupHeadingsColor.ForeColor);
			m_generator.LWCQuestionTextColor = ColorTranslator.ToHtml(m_lblLWCQuestionColor.ForeColor);
			m_generator.LWCAnswerTextColor = ColorTranslator.ToHtml(m_lblLWCAnswerTextColor.ForeColor);
			m_generator.CommentTextColor = ColorTranslator.ToHtml(m_lblCommentTextColor.ForeColor);
			m_generator.NumberOfBlankLinesForAnswer = (int)m_numBlankLines.Value;
			m_generator.NumberQuestions = m_chkNumberQuestions.Checked;

			m_generator.Title = m_txtTitle.Text;
			m_generator.LwcLocale = m_chkIncludeLWCQuestions.Enabled ?
				m_lwcLocaleIds[m_cboUseLWC.SelectedIndex] : null;

			m_generator.OutputPassageForOutOfOrderQuestions = m_rdoOutputPassageForOutOfOrderQuestions.Checked;

			m_generator.IncludeLWCQuestions = m_chkIncludeLWCQuestions.Checked;
			m_generator.IncludeLWCAnswers = m_chkIncludeLWCAnswers.Checked;
			m_generator.IncludeLWCComments = m_chkIncludeLWCComments.Checked;

			if (m_rdoDisplayWarning.Checked)
				m_generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.Warn;
			else if (m_rdoUseOriginal.Checked)
				m_generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.UseLWC;
			else
			{
				Debug.Assert(m_rdoSkipUntranslated.Checked);
				m_generator.HandlingOfUntranslatedQuestions = HandleUntranslatedQuestionsOption.Skip;
			}

			m_generator.Folder = m_lblFolder.Text;

			m_generator.UseExternalCss = m_rdoUseExternalCss.Checked;
			if (m_rdoUseExternalCss.Checked)
			{
				m_generator.WriteCssFile = !m_chkOverwriteCss.Enabled || m_chkOverwriteCss.Checked;
				m_generator.CssFile = m_txtCssFile.Text;
				m_generator.UseAbsolutePathForCssFile = m_chkAbsoluteCssPath.Checked;
			}

			if (m_rdoDisplayWarning.Checked)
			{
				var untranslatedQuestions = m_generator.QuestionsToInclude.Count(p => !p.HasUserTranslation);
				if (untranslatedQuestions > 0)
				{
					btnOk.DialogResult = DialogResult.None;
					ShowModalChild(new MessageBoxForm(Format(LocalizationManager.GetString("MainWindow.UntranslatedQuestionsWarning",
							"There are {0} questions in the selected range that do not have confirmed translations. These questions " +
							"will be excluded from the checking script.",
							"Param is a number."), untranslatedQuestions),
						TxlConstants.kPluginName, MessageBoxButtons.OKCancel), form =>
					{
						DialogResult = form.DialogResult;
						form.Disposed += (o, args) => { Close(); };
					});
				}
			}
		}

		private void ComboTextUpdate(object sender, EventArgs e)
		{
			ComboBox cbo = (ComboBox)sender;
			if (cbo.SelectedItem == null || cbo.Text != cbo.SelectedItem.ToString())
				cbo.SelectedIndex = -1;
		}

		private void HandleLWCSelectedIndexChanged(object sender, EventArgs e)
		{
			int i = m_cboUseLWC.SelectedIndex;
			if (i < 0 || i >= m_cboUseLWC.Items.Count - 1)
			{
				m_chkIncludeLWCQuestions.Enabled = m_chkIncludeLWCQuestions.Checked =
				m_chkIncludeLWCAnswers.Enabled = m_chkIncludeLWCAnswers.Checked =
				m_chkIncludeLWCComments.Enabled = m_chkIncludeLWCComments.Checked = false;
				m_chkIncludeLWCQuestions.Text = Format(m_fmtChkEnglishQuestions, Empty);
				m_chkIncludeLWCAnswers.Text = Format(m_fmtChkEnglishAnswers, Empty);
				m_chkIncludeLWCComments.Text = Format(m_fmtChkIncludeComments, Empty);
			}
			else
			{
				if (!m_chkIncludeLWCQuestions.Enabled)
				{
					m_chkIncludeLWCQuestions.Enabled = m_chkIncludeLWCAnswers.Enabled = m_chkIncludeLWCComments.Enabled = true;
					SetDefaultCheckedStateForLWCOptions();
				}
				var languageName = m_cboUseLWC.SelectedItem.ToString();
				m_chkIncludeLWCQuestions.Text = Format(m_fmtChkEnglishQuestions, languageName);
				m_chkIncludeLWCAnswers.Text = Format(m_fmtChkEnglishAnswers, languageName);
				m_chkIncludeLWCComments.Text = Format(m_fmtChkIncludeComments, languageName);
				m_lblLWCQuestionColor.Text = Format(m_fmLWCQuestionTextColor, languageName);
				m_lblLWCAnswerTextColor.Text = Format(m_fmLWCAnswerTextColor, languageName);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, CancelEventArgs e)
		{
			HandleHelpRequest(sender, new HelpEventArgs(MousePosition));
		}

		private void HandleHelpRequest(object sender, HelpEventArgs args)
		{
			if (!IsNullOrEmpty(m_help))
				Process.Start(m_help);
		}

		private void m_chkShowUnavailable_CheckedChanged(object sender, EventArgs e)
		{
			LoadBooks();
			LoadSectionCombos();
		}
		#endregion

		#region Private helper methods
		private void UpdateOkButtonEnabledState()
		{
			btnOk.Enabled = true;
			if (m_rdoSingleSection.Checked && m_cboSection.SelectedIndex < 0)
				btnOk.Enabled = false;
			else if (m_rdoSectionRange.Checked && (m_cboStartSection.SelectedIndex < 0 || m_cboEndSection.SelectedIndex < 0))
				btnOk.Enabled = false;
		}

		private bool UpdateSectionRangeStartRef()
		{
			if (m_cboStartSection.SelectedItem is Section section)
			{
				VerseRangeStartRef = section.StartRef;
				return true;
			}

			return false;
		}

		private bool UpdateSectionRangeEndRef()
		{
			if (m_cboEndSection.SelectedItem is Section section)
			{
				VerseRangeEndRef = section.EndRef;
				return true;
			}

			return false;
		}

		private void UpdateTitleAndFilenameForSectionRange()
		{
			if (m_cboStartSection.SelectedIndex < 0 || m_cboEndSection.SelectedIndex < 0)
				return;
			// Use new version of libpalaso to get title when book names are different
			var sRef = VerseRangeStartRef.Book != VerseRangeEndRef.Book ?
				(VerseRangeStartRef + "-" + VerseRangeEndRef).Replace(":", "."):
				BCVRef.MakeReferenceString(VerseRangeStartRef, VerseRangeEndRef, ".", "-");
			UpdateTextBoxWithSelectedPassage(m_txtFilename, sRef, m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, sRef, m_sTitleTemplate);
		}
		#endregion
	}
}