// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: GenerateTemplateDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SIL.Utils;
using SILUBS.SharedScrUtils;

namespace SILUBS.PhraseTranslationHelper
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog to present user with options for generating an LCF file to use for generating a
	/// printable script to do comprehension checking.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class GenerateTemplateDlg : Form
	{
		#region RangeOption enum
		public enum RangeOption
		{
			WholeBook,
			SingleSection,
			RangeOfSections,
		}
		#endregion

		#region Data members
		private string m_sFilenameTemplate;
		private string m_sTitleTemplate;
		private readonly List<string> m_sectionRefs = new List<string>();
		private BCVRef m_startRef = new BCVRef();
		private BCVRef m_endRef = new BCVRef();
		#endregion

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:GenerateTemplateDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal GenerateTemplateDlg(string projectName, string defaultLcfFolder,
			GenerateTemplateSettings settings, IEnumerable<int> canonicalBookIds,
			IEnumerable<KeyValuePair<string, string>> sections)
		{
			InitializeComponent();
			m_chkEnglishQuestions.Tag = btnChooseEnglishQuestionColor;
			m_chkEnglishAnswers.Tag = btnChooseEnglishAnswerColor;
			m_chkIncludeComments.Tag = btnChooserCommentColor;
			btnChooseQuestionGroupHeadingsColor.Tag = m_lblQuestionGroupHeadingsColor;
			btnChooseEnglishQuestionColor.Tag = m_lblEnglishQuestionColor;
			btnChooseEnglishAnswerColor.Tag = m_lblEnglishAnswerTextColor;
			btnChooserCommentColor.Tag = m_lblCommentTextColor;
			m_sTitleTemplate = m_txtTitle.Text;
			m_sFilenameTemplate = string.Format(m_txtFilename.Text, projectName, "{0}");

			LoadBooks(canonicalBookIds);
			LoadSectionCombos(sections);

			if (settings == null)
				m_lblFolder.Text = defaultLcfFolder;
			else
			{
				switch (settings.Range)
				{
					case RangeOption.WholeBook:
						m_rdoWholeBook.Checked = true;
						TrySelectItem(m_cboBooks, settings.Book);
						break;
					case RangeOption.SingleSection:
						m_rdoSingleSection.Checked = true;
						TrySelectItem(m_cboSection, settings.Section);
						break;
					case RangeOption.RangeOfSections:
						m_rdoSectionRange.Checked = true;
						TrySelectItem(m_cboStartSection, settings.Section);
						TrySelectItem(m_cboEndSection, settings.EndSection);
						break;
				}

				m_chkPassageBeforeOverview.Checked = settings.PassageBeforeOverview;
				m_chkEnglishQuestions.Checked = settings.EnglishQuestions;
				m_chkEnglishAnswers.Checked = settings.EnglishAnswers;
				m_chkIncludeComments.Checked = settings.IncludeComments;
				m_rdoUseOriginal.Checked = settings.UseOriginalQuestionIfNotTranslated;

				m_lblFolder.Text = settings.Folder;

				m_numBlankLines.Value = settings.BlankLines;
				if (!settings.QuestionGroupHeadingsColor.IsEmpty)
					m_lblQuestionGroupHeadingsColor.ForeColor = settings.QuestionGroupHeadingsColor;
				if (!settings.EnglishQuestionTextColor.IsEmpty)
					m_lblEnglishQuestionColor.ForeColor = settings.EnglishQuestionTextColor;
				if (!settings.EnglishAnswerTextColor.IsEmpty)
					m_lblEnglishAnswerTextColor.ForeColor = settings.EnglishAnswerTextColor;
				if (!settings.CommentTextColor.IsEmpty)
					m_lblCommentTextColor.ForeColor = settings.CommentTextColor;
				m_chkNumberQuestions.Checked = settings.NumberQuestions;

				m_rdoUseExternalCss.Checked = settings.UseExternalCss;
				if (m_rdoUseExternalCss.Checked)
				{
					m_txtCssFile.Text = settings.CssFile;
					m_chkAbsoluteCssPath.Checked = settings.AbsoluteCssPath;
				}
			}
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
		private void LoadSectionCombos(IEnumerable<KeyValuePair<string, string>> sections)
		{
			foreach (KeyValuePair<string, string> sectionInfo in sections)
			{
				m_sectionRefs.Add(sectionInfo.Key);
				m_cboSection.Items.Add(sectionInfo.Value);
				m_cboStartSection.Items.Add(sectionInfo.Value);
				m_cboEndSection.Items.Add(sectionInfo.Value);
			}
		}
		#endregion

		#region Properties
		public BCVRef VerseRangeStartRef
		{
			get { return m_startRef; }
		}

		public BCVRef VerseRangeEndRef
		{
			get { return m_endRef; }
		}

		public string FileName
		{
			get
			{
				string path = Path.Combine(m_lblFolder.Text, m_txtFilename.Text);
				if (string.IsNullOrEmpty(Path.GetExtension(path)))
					path = Path.ChangeExtension(path, "lcf");
				return path;
			}
		}

		public string CssFile
		{
			get { return m_txtCssFile.Text; }
		}

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

		public bool WriteCssFile
		{
			get
			{
				return m_rdoUseExternalCss.Checked && (!m_chkOverwriteCss.Enabled || m_chkOverwriteCss.Checked);
			}
		}

		internal GenerateTemplateSettings Settings
		{
			get
			{
				GenerateTemplateSettings settings = new GenerateTemplateSettings();
				if (m_rdoWholeBook.Checked)
				{
					settings.Range = RangeOption.WholeBook;
					settings.Book = m_cboBooks.SelectedItem.ToString();
				}
				else if (m_rdoSingleSection.Checked)
				{
					settings.Range = RangeOption.SingleSection;
					settings.Section = m_cboSection.SelectedItem.ToString();					
				}
				else
				{
					settings.Range = RangeOption.RangeOfSections;
					settings.Section = m_cboStartSection.SelectedItem.ToString();					
					settings.EndSection = m_cboEndSection.SelectedItem.ToString();					
				}

				settings.PassageBeforeOverview = m_chkPassageBeforeOverview.Checked;
				settings.EnglishQuestions = m_chkEnglishQuestions.Checked;
				settings.EnglishAnswers = m_chkEnglishAnswers.Checked;
				settings.IncludeComments = m_chkIncludeComments.Checked;
				settings.UseOriginalQuestionIfNotTranslated = m_rdoUseOriginal.Checked;

				settings.Folder = m_lblFolder.Text;

				settings.BlankLines = (int)m_numBlankLines.Value;
				settings.QuestionGroupHeadingsColor = m_lblQuestionGroupHeadingsColor.ForeColor;
				settings.EnglishQuestionTextColor = m_lblEnglishQuestionColor.ForeColor;
				settings.EnglishAnswerTextColor = m_lblEnglishAnswerTextColor.ForeColor;
				settings.CommentTextColor = m_lblCommentTextColor.ForeColor;
				settings.NumberQuestions = m_chkNumberQuestions.Checked;

				settings.UseExternalCss = m_rdoUseExternalCss.Checked;
				if (settings.UseExternalCss)
				{
					settings.CssFile = m_txtCssFile.Text;
					settings.AbsoluteCssPath = m_chkAbsoluteCssPath.Checked;
				}
				return settings;
			}
		}
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
				dlg.DefaultExt = ".lcf";
				dlg.Filter = String.Format("Lectionary Control File ({0})|{0}", "*.lcf");
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
			if (m_cboSection.SelectedIndex < 0)
				return;
			string sRef = m_sectionRefs[m_cboSection.SelectedIndex];
			BCVRef.ParseRefRange(sRef, ref m_startRef, ref m_endRef);
			UpdateTextBoxWithSelectedPassage(m_txtFilename, StringUtils.FilterForFileName(sRef), m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, sRef, m_sTitleTemplate);
		}

		private static void UpdateTextBoxWithSelectedPassage(TextBox txt, string passage, string fmt)
		{
			if (txt.Tag == null || txt.Text == (string)txt.Tag)
				txt.Tag = txt.Text = string.Format(fmt, passage);
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
				if (string.IsNullOrEmpty(folder))
					folder = m_lblFolder.Text;
				dlg.AddExtension = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = ".css";
				dlg.Filter = String.Format("Cascading Style Sheet ({0})|{0}", "*.css");
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
		}

		private void m_cboEndSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_cboStartSection.SelectedIndex > m_cboEndSection.SelectedIndex)
				m_cboStartSection.SelectedIndex = m_cboEndSection.SelectedIndex;
			if (UpdateSectionRangeEndRef())
				UpdateTitleAndFilenameForSectionRange();
		}

		private void m_rdoSectionRange_CheckedChanged(object sender, EventArgs e)
		{
			if (m_rdoSectionRange.Checked && UpdateSectionRangeStartRef() && UpdateSectionRangeEndRef())
				UpdateTitleAndFilenameForSectionRange();
		}
		#endregion

		#region Private helper methods
		private bool UpdateSectionRangeStartRef()
		{
			if (m_cboStartSection.SelectedIndex < 0)
				return false;
			string sRef = m_sectionRefs[m_cboStartSection.SelectedIndex];
			BCVRef dummy = new BCVRef();
			BCVRef.ParseRefRange(sRef, ref m_startRef, ref dummy);
			return true;
		}

		private bool UpdateSectionRangeEndRef()
		{
			if (m_cboEndSection.SelectedIndex < 0)
				return false;
			string sRef = m_sectionRefs[m_cboEndSection.SelectedIndex];
			BCVRef dummy = new BCVRef();
			BCVRef.ParseRefRange(sRef, ref dummy, ref m_endRef);
			return true;
		}

		private void UpdateTitleAndFilenameForSectionRange()
		{
			if (m_cboStartSection.SelectedIndex < 0 || m_cboEndSection.SelectedIndex < 0)
				return;
			string sRef = BCVRef.MakeReferenceString(m_startRef, m_endRef, ".", "-");
			UpdateTextBoxWithSelectedPassage(m_txtFilename, sRef, m_sFilenameTemplate);
			UpdateTextBoxWithSelectedPassage(m_txtTitle, sRef, m_sTitleTemplate);
		}
		#endregion
	}
}