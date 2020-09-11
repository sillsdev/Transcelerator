// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: GenerateTemplateDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AddInSideViews;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using SIL.Utils;
using static System.String;
using File = System.IO.File;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog to present user with options for generating an LCF file to use for generating a
	/// printable script to do comprehension checking.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class GenerateScriptDlg : Form
	{
		public delegate LocalizationsFileAccessor DataLocalizerNeededEventHandler(object sender, string localeId);
		public event DataLocalizerNeededEventHandler DataLocalizerNeeded;
		public enum RangeOption
		{
			WholeBook = 0,
			SingleSection = 1,
			RangeOfSections = 2,
		}

		#region Data members
		private string m_sFilenameTemplate;
		private string m_sTitleTemplate;
		private readonly IList<ISectionInfo> m_sections;
		private readonly string m_projectName;
		private readonly IScrExtractor m_scrExtractor;
		private List<string> m_lwcLocaleIds;
		private LocalizationsFileAccessor m_dataLoc;
		private string m_fmtChkEnglishQuestions;
		private string m_fmtChkEnglishAnswers;
		private string m_fmtChkIncludeComments;

		#endregion

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal GenerateScriptDlg(string projectName, IScrExtractor scrExtractor,
            string defaultFolder, IEnumerable<int> canonicalBookIds,
            IList<ISectionInfo> sections,
			IEnumerable<KeyValuePair<string, string>> availableAdditionalLWCs)
		{
			m_projectName = projectName;
			m_scrExtractor = scrExtractor;
			InitializeComponent();
			m_chkIncludeLWCQuestions.Tag = btnChooseEnglishQuestionColor;
			m_chkIncludeLWCAnswers.Tag = btnChooseEnglishAnswerColor;
			m_chkIncludeLWCComments.Tag = btnChooserCommentColor;
			btnChooseQuestionGroupHeadingsColor.Tag = m_lblQuestionGroupHeadingsColor;
			btnChooseEnglishQuestionColor.Tag = m_lblEnglishQuestionColor;
			btnChooseEnglishAnswerColor.Tag = m_lblEnglishAnswerTextColor;
			btnChooserCommentColor.Tag = m_lblCommentTextColor;
			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;

			LoadBooks(canonicalBookIds);
			m_sections = sections;
			LoadSectionCombos();
			LoadLWCCombo(availableAdditionalLWCs);

			switch ((RangeOption)Properties.Settings.Default.GenerateTemplateRange)
			{
				case RangeOption.WholeBook:
					m_rdoWholeBook.Checked = true;
					TrySelectItem(m_cboBooks, Properties.Settings.Default.GenerateTemplateBook);
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

			m_chkPassageBeforeOverview.Checked = Properties.Settings.Default.GenerateTemplatePassageBeforeOverview;
			if (m_scrExtractor == null)
				m_chkIncludeVerseNumbers.Checked = m_chkIncludeVerseNumbers.Enabled = false;
			else
				m_chkIncludeVerseNumbers.Checked = Properties.Settings.Default.GenerateIncludeVerseNumbers;
			SetDefaultCheckedStateForLWCOptions();
			m_rdoUseOriginal.Checked = Properties.Settings.Default.GenerateTemplateUseOriginalQuestionIfNotTranslated;
			m_rdoSkipUntranslated.Checked = !m_rdoUseOriginal.Checked && Properties.Settings.Default.GenerateTemplateSkipQuestionIfNotTranslated; // These two settings should never be able to both be true, but just to be safe.

			m_rdoOutputPassageForOutOfOrderQuestions.Checked = Properties.Settings.Default.GenerateOutputPassageForOutOfOrderQuestions;

            m_lblFolder.Text = IsNullOrEmpty(Properties.Settings.Default.GenerateTemplateFolder) ? defaultFolder :
                Properties.Settings.Default.GenerateTemplateFolder;

			m_numBlankLines.Value = Properties.Settings.Default.GenerateTemplateBlankLines;
			if (!Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor.IsEmpty)
				m_lblQuestionGroupHeadingsColor.ForeColor = Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor;
			if (!Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor.IsEmpty)
				m_lblEnglishQuestionColor.ForeColor = Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor;
			if (!Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor.IsEmpty)
				m_lblEnglishAnswerTextColor.ForeColor = Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor;
			if (!Properties.Settings.Default.GenerateTemplateCommentTextColor.IsEmpty)
				m_lblCommentTextColor.ForeColor = Properties.Settings.Default.GenerateTemplateCommentTextColor;
			m_chkNumberQuestions.Checked = Properties.Settings.Default.GenerateTemplateNumberQuestions;

			m_rdoUseExternalCss.Checked = Properties.Settings.Default.GenerateTemplateUseExternalCss;
			if (m_rdoUseExternalCss.Checked)
			{
				m_txtCssFile.Text = Properties.Settings.Default.GenerateTemplateCssFile;
				m_chkAbsoluteCssPath.Checked = Properties.Settings.Default.GenerateTemplateAbsoluteCssPath;
			}
		}

		private void HandleStringsLocalized()
		{
			m_fmtChkEnglishQuestions = m_chkIncludeLWCQuestions.Text;
			m_fmtChkEnglishAnswers = m_chkIncludeLWCAnswers.Text;
			m_fmtChkIncludeComments = m_chkIncludeLWCComments.Text;
			m_sTitleTemplate = m_txtTitle.Text;
			m_sFilenameTemplate = Format(m_txtFilename.Text, m_projectName, "{0}");
			if (m_scrExtractor == null)
			{
				m_lbFilename.Text = LocalizationManager.GetString("GenerateScriptDlg.TemplateFileNameLabel",
					"Template &File name:");
				m_sFilenameTemplate = Path.ChangeExtension(m_sFilenameTemplate, "lcf");
			}
		}

		private void SetDefaultCheckedStateForLWCOptions()
		{
			m_chkIncludeLWCQuestions.Checked = Properties.Settings.Default.GenerateTemplateEnglishQuestions;
			m_chkIncludeLWCAnswers.Checked = Properties.Settings.Default.GenerateTemplateEnglishAnswers;
			m_chkIncludeLWCComments.Checked = Properties.Settings.Default.GenerateTemplateIncludeComments;
		}

		private void TrySelectItem(ComboBox cbo, string value)
		{
			int i = cbo.FindStringExact(value);
			if (i >= 0)
				cbo.SelectedIndex = i;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the combo box with the appropriate book ids
		/// </summary>
		/// <param name="canonicalBookIds">1-based Canonical book numbers</param>
		/// ------------------------------------------------------------------------------------
		private void LoadBooks(IEnumerable<int> canonicalBookIds)
		{
			foreach (int canonicalBookId in canonicalBookIds)
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
			foreach (var sectionInfo in m_sections)
			{
				m_cboSection.Items.Add(sectionInfo);
				m_cboStartSection.Items.Add(sectionInfo);
				m_cboEndSection.Items.Add(sectionInfo);
			}
		}

		private void LoadLWCCombo(IEnumerable<KeyValuePair<string, string>> availableAdditionalLWCs)
		{
			m_lwcLocaleIds = new List<string>(new [] {"en-US"});
			int i = 0;
			foreach (var lwc in availableAdditionalLWCs)
			{
				m_cboUseLWC.Items.Insert(++i, lwc.Key);
				m_lwcLocaleIds.Add(lwc.Value);
				if (Properties.Settings.Default.GenerateTemplateUseLWC == lwc.Value)
					m_cboUseLWC.SelectedIndex = i;
			}
			if (IsNullOrWhiteSpace(Properties.Settings.Default.GenerateTemplateUseLWC))
				m_cboUseLWC.SelectedIndex = m_cboUseLWC.Items.Count - 1; // None
			if (m_cboUseLWC.SelectedIndex == -1)
				m_cboUseLWC.SelectedIndex = 0;
		}
		#endregion

		#region Properties
		public string SelectedBook => m_rdoWholeBook.Checked ? (string)m_cboBooks.SelectedItem : null;

		// ENHANCE: It's less than ideal to provide the selected "sections" in terms of
		// start and end refs because there are couple sections that start/end mid-verse.
		// If the user selects one of these as a single section or starts/ends the section
		// range in one of these, the generated script is going to include one of more
		// questions from the adjacent section. From a task-oriented perspective this is
		// unlikely. Unfortunately, because the section IDs are not in ascending with
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
		public BCVRef VerseRangeStartRef { get; private set; } = new BCVRef();

		public BCVRef VerseRangeEndRef { get; private set; } = new BCVRef();

		public string FileName
		{
			get
			{
				string path = Path.Combine(m_lblFolder.Text, m_txtFilename.Text);
				if (IsNullOrEmpty(Path.GetExtension(path)))
					path = Path.ChangeExtension(path, "lcf");
				return path;
			}
		}

		public string CssFile => m_txtCssFile.Text;

		public string FullCssPath
		{
			get 
			{
				string path = m_txtCssFile.Text;
				if (!Path.IsPathRooted(path))
					path = Path.Combine(m_lblFolder.Text, path);
				return path;
			}
		}

		public bool WriteCssFile => m_rdoUseExternalCss.Checked && (!m_chkOverwriteCss.Enabled || m_chkOverwriteCss.Checked);

		public string NormalizedTitle => m_txtTitle.Text.Normalize(NormalizationForm.FormC);
		public string LwcLocale => m_dataLoc?.Locale ?? "en";

		public bool OutputFullPassageAtStartOfSection => m_chkPassageBeforeOverview.Checked;
		public bool OutputPassageForOutOfOrderQuestions => m_rdoOutputPassageForOutOfOrderQuestions.Checked;
		#endregion

		#region Event handlers
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
                var description = (m_scrExtractor == null ? LocalizationManager.GetString(
		                "GenerateScriptDlg.LectionaryControlFile", "Lectionary Control File") :
                    LocalizationManager.GetString("GenerateScriptDlg.HtmlFileDescription", "Web Page"));
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
			if (m_cboSection.SelectedIndex < 0)
				return;
			var info = m_sections[m_cboSection.SelectedIndex];
			VerseRangeStartRef = new BCVRef(info.StartRef);
			VerseRangeEndRef = new BCVRef(info.EndRef);
			UpdateTextBoxWithSelectedPassage(m_txtFilename, StringUtils.FilterForFileName(info.Heading), m_sFilenameTemplate);
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
			m_pnlCssOptions.Visible = m_rdoUseExternalCss.Checked;
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
			string cssFile = FullCssPath;
			if (Path.GetDirectoryName(cssFile) != m_lblFolder.Text)
				m_chkAbsoluteCssPath.Checked = true;
			m_chkOverwriteCss.Enabled = File.Exists(cssFile);
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
                Properties.Settings.Default.GenerateTemplateRange = (int)RangeOption.WholeBook;
                Properties.Settings.Default.GenerateTemplateBook = m_cboBooks.SelectedItem.ToString();
            }
            else if (m_rdoSingleSection.Checked)
            {
                Properties.Settings.Default.GenerateTemplateRange = (int)RangeOption.SingleSection;
                Properties.Settings.Default.GenerateTemplateSection = m_cboSection.SelectedItem.ToString();
            }
            else
            {
                Properties.Settings.Default.GenerateTemplateRange = (int)RangeOption.RangeOfSections;
                Properties.Settings.Default.GenerateTemplateSection = m_cboStartSection.SelectedItem.ToString();
                Properties.Settings.Default.GenerateTemplateEndSection = m_cboEndSection.SelectedItem.ToString();
            }

            Properties.Settings.Default.GenerateTemplatePassageBeforeOverview = m_chkPassageBeforeOverview.Checked;
            if (m_chkIncludeVerseNumbers.Enabled)
            {
	            Properties.Settings.Default.GenerateIncludeVerseNumbers = m_chkIncludeVerseNumbers.Checked;
	            m_scrExtractor.IncludeVerseNumbers = m_chkIncludeVerseNumbers.Checked;
            }
            Properties.Settings.Default.GenerateTemplateUseLWC = m_chkIncludeLWCQuestions.Enabled ?
				m_lwcLocaleIds[m_cboUseLWC.SelectedIndex] : null;
			Properties.Settings.Default.GenerateTemplateEnglishQuestions = m_chkIncludeLWCQuestions.Checked;
            Properties.Settings.Default.GenerateTemplateEnglishAnswers = m_chkIncludeLWCAnswers.Checked;
            Properties.Settings.Default.GenerateTemplateIncludeComments = m_chkIncludeLWCComments.Checked;
			m_dataLoc = DataLocalizerNeeded?.Invoke(this, Properties.Settings.Default.GenerateTemplateUseLWC);
            Properties.Settings.Default.GenerateTemplateUseOriginalQuestionIfNotTranslated = m_rdoUseOriginal.Checked;
			Properties.Settings.Default.GenerateTemplateSkipQuestionIfNotTranslated = m_rdoSkipUntranslated.Checked;

			Properties.Settings.Default.GenerateOutputPassageForOutOfOrderQuestions = m_rdoOutputPassageForOutOfOrderQuestions.Checked;

            Properties.Settings.Default.GenerateTemplateFolder = m_lblFolder.Text;

            Properties.Settings.Default.GenerateTemplateBlankLines = (int)m_numBlankLines.Value;
            Properties.Settings.Default.GenerateTemplateQuestionGroupHeadingsColor = m_lblQuestionGroupHeadingsColor.ForeColor;
            Properties.Settings.Default.GenerateTemplateEnglishQuestionTextColor = m_lblEnglishQuestionColor.ForeColor;
            Properties.Settings.Default.GenerateTemplateEnglishAnswerTextColor = m_lblEnglishAnswerTextColor.ForeColor;
            Properties.Settings.Default.GenerateTemplateCommentTextColor = m_lblCommentTextColor.ForeColor;
            Properties.Settings.Default.GenerateTemplateNumberQuestions = m_chkNumberQuestions.Checked;

            Properties.Settings.Default.GenerateTemplateUseExternalCss = m_rdoUseExternalCss.Checked;
            if (Properties.Settings.Default.GenerateTemplateUseExternalCss)
            {
                Properties.Settings.Default.GenerateTemplateCssFile = m_txtCssFile.Text;
                Properties.Settings.Default.GenerateTemplateAbsoluteCssPath = m_chkAbsoluteCssPath.Checked;
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
			}
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
			if (m_cboStartSection.SelectedIndex < 0)
				return false;
			VerseRangeStartRef = m_sections[m_cboStartSection.SelectedIndex].StartRef;
			return true;
		}

		private bool UpdateSectionRangeEndRef()
		{
			if (m_cboEndSection.SelectedIndex < 0)
				return false;
			VerseRangeEndRef = m_sections[m_cboEndSection.SelectedIndex].EndRef;
			return true;
		}

		private void UpdateTitleAndFilenameForSectionRange()
		{
			if (m_cboStartSection.SelectedIndex < 0 || m_cboEndSection.SelectedIndex < 0)
				return;
			string sRef = BCVRef.MakeReferenceString(VerseRangeStartRef, VerseRangeEndRef, ".", "-");
			UpdateTextBoxWithSelectedPassage(m_txtFilename, sRef, m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, sRef, m_sTitleTemplate);
		}
		#endregion

		public string GetDataString(UIDataString key, out string lang)
		{
			if (m_dataLoc == null)
			{
				lang = "en";
				return key.SourceUIString;
			}
			return m_dataLoc.GetLocalizedDataString(key, out lang);
		}
	}
}