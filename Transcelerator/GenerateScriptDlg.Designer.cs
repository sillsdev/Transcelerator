// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: GenerateTemplateDlg.cs
// Responsibility: Bogle
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace SIL.Transcelerator
{
	partial class GenerateScriptDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		// TODO-Linux: AutoCompletion is not implemented in Mono
		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateScriptDlg));
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			this.m_lblFolder = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.m_txtFilename = new System.Windows.Forms.TextBox();
			this.m_lbFilename = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.m_txtTitle = new System.Windows.Forms.TextBox();
			this.m_rdoWholeBook = new System.Windows.Forms.RadioButton();
			this.m_grpRange = new System.Windows.Forms.GroupBox();
			this.m_cboBooks = new System.Windows.Forms.ComboBox();
			this.m_cboEndSection = new System.Windows.Forms.ComboBox();
			this.m_cboStartSection = new System.Windows.Forms.ComboBox();
			this.m_rdoSectionRange = new System.Windows.Forms.RadioButton();
			this.m_cboSection = new System.Windows.Forms.ComboBox();
			this.m_rdoSingleSection = new System.Windows.Forms.RadioButton();
			this.m_chkPassageBeforeOverview = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.m_rdoSkipUntranslated = new System.Windows.Forms.RadioButton();
			this.m_rdoDisplayWarning = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.m_rdoUseOriginal = new System.Windows.Forms.RadioButton();
			this.m_chkIncludeLWCComments = new System.Windows.Forms.CheckBox();
			this.m_chkIncludeLWCAnswers = new System.Windows.Forms.CheckBox();
			this.m_chkIncludeLWCQuestions = new System.Windows.Forms.CheckBox();
			this.m_lblQuestionGroupHeadingsColor = new System.Windows.Forms.Label();
			this.btnChooseQuestionGroupHeadingsColor = new System.Windows.Forms.Button();
			this.m_lblCommentTextColor = new System.Windows.Forms.Label();
			this.btnChooserCommentColor = new System.Windows.Forms.Button();
			this.m_lblEnglishAnswerTextColor = new System.Windows.Forms.Label();
			this.btnChooseEnglishAnswerColor = new System.Windows.Forms.Button();
			this.m_lblEnglishQuestionColor = new System.Windows.Forms.Label();
			this.btnChooseEnglishQuestionColor = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.colorDlg = new System.Windows.Forms.ColorDialog();
			this.m_numBlankLines = new System.Windows.Forms.NumericUpDown();
			this.panel2 = new System.Windows.Forms.Panel();
			this.m_chkNumberQuestions = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.m_pnlCssOptions = new System.Windows.Forms.Panel();
			this.m_chkAbsoluteCssPath = new System.Windows.Forms.CheckBox();
			this.m_chkOverwriteCss = new System.Windows.Forms.CheckBox();
			this.m_txtCssFile = new System.Windows.Forms.TextBox();
			this.btnBrowseCss = new System.Windows.Forms.Button();
			this.m_rdoUseExternalCss = new System.Windows.Forms.RadioButton();
			this.m_rdoEmbedStyleInfo = new System.Windows.Forms.RadioButton();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.tabOptions = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.m_rdoDisplayReferenceForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.m_rdoOutputPassageForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.m_cboUseLWC = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tabAppearance = new System.Windows.Forms.TabPage();
			label1 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			groupBox2.SuspendLayout();
			this.m_grpRange.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numBlankLines)).BeginInit();
			this.panel2.SuspendLayout();
			this.m_pnlCssOptions.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabOptions.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tabAppearance.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// groupBox2
			// 
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Controls.Add(this.m_lblFolder);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(this.btnBrowse);
			groupBox2.Controls.Add(this.m_txtFilename);
			groupBox2.Controls.Add(this.m_lbFilename);
			groupBox2.Controls.Add(this.label2);
			groupBox2.Controls.Add(this.m_txtTitle);
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			// 
			// m_lblFolder
			// 
			resources.ApplyResources(this.m_lblFolder, "m_lblFolder");
			this.m_lblFolder.Name = "m_lblFolder";
			// 
			// label4
			// 
			resources.ApplyResources(label4, "label4");
			label4.Name = "label4";
			// 
			// btnBrowse
			// 
			resources.ApplyResources(this.btnBrowse, "btnBrowse");
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// m_txtFilename
			// 
			resources.ApplyResources(this.m_txtFilename, "m_txtFilename");
			this.m_txtFilename.Name = "m_txtFilename";
			// 
			// m_lbFilename
			// 
			resources.ApplyResources(this.m_lbFilename, "m_lbFilename");
			this.m_lbFilename.Name = "m_lbFilename";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// m_txtTitle
			// 
			resources.ApplyResources(this.m_txtTitle, "m_txtTitle");
			this.m_txtTitle.Name = "m_txtTitle";
			// 
			// label5
			// 
			resources.ApplyResources(label5, "label5");
			label5.Name = "label5";
			// 
			// label9
			// 
			resources.ApplyResources(label9, "label9");
			label9.Name = "label9";
			// 
			// label10
			// 
			resources.ApplyResources(label10, "label10");
			label10.Name = "label10";
			// 
			// m_rdoWholeBook
			// 
			resources.ApplyResources(this.m_rdoWholeBook, "m_rdoWholeBook");
			this.m_rdoWholeBook.Checked = true;
			this.m_rdoWholeBook.Name = "m_rdoWholeBook";
			this.m_rdoWholeBook.TabStop = true;
			this.m_rdoWholeBook.UseVisualStyleBackColor = true;
			this.m_rdoWholeBook.CheckedChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSelectedBook);
			// 
			// m_grpRange
			// 
			resources.ApplyResources(this.m_grpRange, "m_grpRange");
			this.m_grpRange.Controls.Add(this.m_cboBooks);
			this.m_grpRange.Controls.Add(this.m_cboEndSection);
			this.m_grpRange.Controls.Add(label1);
			this.m_grpRange.Controls.Add(this.m_cboStartSection);
			this.m_grpRange.Controls.Add(this.m_rdoSectionRange);
			this.m_grpRange.Controls.Add(this.m_cboSection);
			this.m_grpRange.Controls.Add(this.m_rdoSingleSection);
			this.m_grpRange.Controls.Add(this.m_rdoWholeBook);
			this.m_grpRange.Name = "m_grpRange";
			this.m_grpRange.TabStop = false;
			// 
			// m_cboBooks
			// 
			this.m_cboBooks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboBooks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboBooks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboBooks.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboBooks, "m_cboBooks");
			this.m_cboBooks.Name = "m_cboBooks";
			this.m_cboBooks.SelectedIndexChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSelectedBook);
			this.m_cboBooks.Enter += new System.EventHandler(this.m_cboBooks_Enter);
			// 
			// m_cboEndSection
			// 
			this.m_cboEndSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboEndSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboEndSection.DropDownHeight = 156;
			this.m_cboEndSection.DropDownWidth = 250;
			this.m_cboEndSection.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboEndSection, "m_cboEndSection");
			this.m_cboEndSection.Name = "m_cboEndSection";
			this.m_cboEndSection.SelectedIndexChanged += new System.EventHandler(this.m_cboEndSection_SelectedIndexChanged);
			this.m_cboEndSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboEndSection.Enter += new System.EventHandler(this.SectionRangeCombo_Enter);
			// 
			// m_cboStartSection
			// 
			this.m_cboStartSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboStartSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboStartSection.DropDownHeight = 156;
			this.m_cboStartSection.DropDownWidth = 250;
			this.m_cboStartSection.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboStartSection, "m_cboStartSection");
			this.m_cboStartSection.Name = "m_cboStartSection";
			this.m_cboStartSection.SelectedIndexChanged += new System.EventHandler(this.m_cboStartSection_SelectedIndexChanged);
			this.m_cboStartSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboStartSection.Enter += new System.EventHandler(this.SectionRangeCombo_Enter);
			// 
			// m_rdoSectionRange
			// 
			resources.ApplyResources(this.m_rdoSectionRange, "m_rdoSectionRange");
			this.m_rdoSectionRange.Name = "m_rdoSectionRange";
			this.m_rdoSectionRange.UseVisualStyleBackColor = true;
			this.m_rdoSectionRange.CheckedChanged += new System.EventHandler(this.m_rdoSectionRange_CheckedChanged);
			// 
			// m_cboSection
			// 
			this.m_cboSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboSection.DropDownHeight = 156;
			this.m_cboSection.DropDownWidth = 250;
			this.m_cboSection.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboSection, "m_cboSection");
			this.m_cboSection.Name = "m_cboSection";
			this.m_cboSection.SelectedIndexChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			this.m_cboSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboSection.Enter += new System.EventHandler(this.m_cboSection_Enter);
			// 
			// m_rdoSingleSection
			// 
			resources.ApplyResources(this.m_rdoSingleSection, "m_rdoSingleSection");
			this.m_rdoSingleSection.Name = "m_rdoSingleSection";
			this.m_rdoSingleSection.UseVisualStyleBackColor = true;
			this.m_rdoSingleSection.CheckedChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			// 
			// m_chkPassageBeforeOverview
			// 
			resources.ApplyResources(this.m_chkPassageBeforeOverview, "m_chkPassageBeforeOverview");
			this.m_chkPassageBeforeOverview.Checked = true;
			this.m_chkPassageBeforeOverview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkPassageBeforeOverview.Name = "m_chkPassageBeforeOverview";
			this.m_chkPassageBeforeOverview.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.m_rdoSkipUntranslated);
			this.panel1.Controls.Add(this.m_rdoDisplayWarning);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(label5);
			this.panel1.Controls.Add(this.m_rdoUseOriginal);
			this.panel1.Name = "panel1";
			// 
			// m_rdoSkipUntranslated
			// 
			resources.ApplyResources(this.m_rdoSkipUntranslated, "m_rdoSkipUntranslated");
			this.m_rdoSkipUntranslated.Name = "m_rdoSkipUntranslated";
			this.m_rdoSkipUntranslated.UseVisualStyleBackColor = true;
			// 
			// m_rdoDisplayWarning
			// 
			resources.ApplyResources(this.m_rdoDisplayWarning, "m_rdoDisplayWarning");
			this.m_rdoDisplayWarning.Checked = true;
			this.m_rdoDisplayWarning.Name = "m_rdoDisplayWarning";
			this.m_rdoDisplayWarning.TabStop = true;
			this.m_rdoDisplayWarning.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Name = "label6";
			// 
			// m_rdoUseOriginal
			// 
			resources.ApplyResources(this.m_rdoUseOriginal, "m_rdoUseOriginal");
			this.m_rdoUseOriginal.Name = "m_rdoUseOriginal";
			this.m_rdoUseOriginal.UseVisualStyleBackColor = true;
			// 
			// m_chkIncludeLWCComments
			// 
			resources.ApplyResources(this.m_chkIncludeLWCComments, "m_chkIncludeLWCComments");
			this.m_chkIncludeLWCComments.Checked = true;
			this.m_chkIncludeLWCComments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCComments.Name = "m_chkIncludeLWCComments";
			this.m_chkIncludeLWCComments.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCComments.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_chkIncludeLWCAnswers
			// 
			resources.ApplyResources(this.m_chkIncludeLWCAnswers, "m_chkIncludeLWCAnswers");
			this.m_chkIncludeLWCAnswers.Checked = true;
			this.m_chkIncludeLWCAnswers.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCAnswers.Name = "m_chkIncludeLWCAnswers";
			this.m_chkIncludeLWCAnswers.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCAnswers.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_chkIncludeLWCQuestions
			// 
			resources.ApplyResources(this.m_chkIncludeLWCQuestions, "m_chkIncludeLWCQuestions");
			this.m_chkIncludeLWCQuestions.Checked = true;
			this.m_chkIncludeLWCQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCQuestions.Name = "m_chkIncludeLWCQuestions";
			this.m_chkIncludeLWCQuestions.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCQuestions.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_lblQuestionGroupHeadingsColor
			// 
			resources.ApplyResources(this.m_lblQuestionGroupHeadingsColor, "m_lblQuestionGroupHeadingsColor");
			this.m_lblQuestionGroupHeadingsColor.ForeColor = System.Drawing.Color.Blue;
			this.m_lblQuestionGroupHeadingsColor.Name = "m_lblQuestionGroupHeadingsColor";
			// 
			// btnChooseQuestionGroupHeadingsColor
			// 
			resources.ApplyResources(this.btnChooseQuestionGroupHeadingsColor, "btnChooseQuestionGroupHeadingsColor");
			this.btnChooseQuestionGroupHeadingsColor.Name = "btnChooseQuestionGroupHeadingsColor";
			this.btnChooseQuestionGroupHeadingsColor.UseVisualStyleBackColor = true;
			this.btnChooseQuestionGroupHeadingsColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblCommentTextColor
			// 
			resources.ApplyResources(this.m_lblCommentTextColor, "m_lblCommentTextColor");
			this.m_lblCommentTextColor.ForeColor = System.Drawing.Color.Red;
			this.m_lblCommentTextColor.Name = "m_lblCommentTextColor";
			// 
			// btnChooserCommentColor
			// 
			resources.ApplyResources(this.btnChooserCommentColor, "btnChooserCommentColor");
			this.btnChooserCommentColor.Name = "btnChooserCommentColor";
			this.btnChooserCommentColor.UseVisualStyleBackColor = true;
			this.btnChooserCommentColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.btnChooserCommentColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblEnglishAnswerTextColor
			// 
			resources.ApplyResources(this.m_lblEnglishAnswerTextColor, "m_lblEnglishAnswerTextColor");
			this.m_lblEnglishAnswerTextColor.ForeColor = System.Drawing.Color.Green;
			this.m_lblEnglishAnswerTextColor.Name = "m_lblEnglishAnswerTextColor";
			// 
			// btnChooseEnglishAnswerColor
			// 
			resources.ApplyResources(this.btnChooseEnglishAnswerColor, "btnChooseEnglishAnswerColor");
			this.btnChooseEnglishAnswerColor.Name = "btnChooseEnglishAnswerColor";
			this.btnChooseEnglishAnswerColor.UseVisualStyleBackColor = true;
			this.btnChooseEnglishAnswerColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.btnChooseEnglishAnswerColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblEnglishQuestionColor
			// 
			resources.ApplyResources(this.m_lblEnglishQuestionColor, "m_lblEnglishQuestionColor");
			this.m_lblEnglishQuestionColor.ForeColor = System.Drawing.Color.Gray;
			this.m_lblEnglishQuestionColor.Name = "m_lblEnglishQuestionColor";
			// 
			// btnChooseEnglishQuestionColor
			// 
			resources.ApplyResources(this.btnChooseEnglishQuestionColor, "btnChooseEnglishQuestionColor");
			this.btnChooseEnglishQuestionColor.Name = "btnChooseEnglishQuestionColor";
			this.btnChooseEnglishQuestionColor.UseVisualStyleBackColor = true;
			this.btnChooseEnglishQuestionColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.btnChooseEnglishQuestionColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// colorDlg
			// 
			this.colorDlg.AnyColor = true;
			this.colorDlg.SolidColorOnly = true;
			// 
			// m_numBlankLines
			// 
			resources.ApplyResources(this.m_numBlankLines, "m_numBlankLines");
			this.m_numBlankLines.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.m_numBlankLines.Name = "m_numBlankLines";
			this.m_numBlankLines.EnabledChanged += new System.EventHandler(this.m_numBlankLines_EnabledChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.m_chkNumberQuestions);
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.btnChooseQuestionGroupHeadingsColor);
			this.panel2.Controls.Add(this.m_lblCommentTextColor);
			this.panel2.Controls.Add(this.m_lblQuestionGroupHeadingsColor);
			this.panel2.Controls.Add(this.btnChooserCommentColor);
			this.panel2.Controls.Add(this.m_lblEnglishAnswerTextColor);
			this.panel2.Controls.Add(this.btnChooseEnglishAnswerColor);
			this.panel2.Controls.Add(this.m_numBlankLines);
			this.panel2.Controls.Add(this.m_lblEnglishQuestionColor);
			this.panel2.Controls.Add(this.btnChooseEnglishQuestionColor);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// m_chkNumberQuestions
			// 
			resources.ApplyResources(this.m_chkNumberQuestions, "m_chkNumberQuestions");
			this.m_chkNumberQuestions.Checked = true;
			this.m_chkNumberQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkNumberQuestions.Name = "m_chkNumberQuestions";
			this.m_chkNumberQuestions.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// m_pnlCssOptions
			// 
			this.m_pnlCssOptions.Controls.Add(this.m_chkAbsoluteCssPath);
			this.m_pnlCssOptions.Controls.Add(this.m_chkOverwriteCss);
			this.m_pnlCssOptions.Controls.Add(this.m_txtCssFile);
			this.m_pnlCssOptions.Controls.Add(this.btnBrowseCss);
			resources.ApplyResources(this.m_pnlCssOptions, "m_pnlCssOptions");
			this.m_pnlCssOptions.Name = "m_pnlCssOptions";
			// 
			// m_chkAbsoluteCssPath
			// 
			resources.ApplyResources(this.m_chkAbsoluteCssPath, "m_chkAbsoluteCssPath");
			this.m_chkAbsoluteCssPath.Name = "m_chkAbsoluteCssPath";
			this.m_chkAbsoluteCssPath.UseVisualStyleBackColor = true;
			// 
			// m_chkOverwriteCss
			// 
			resources.ApplyResources(this.m_chkOverwriteCss, "m_chkOverwriteCss");
			this.m_chkOverwriteCss.Name = "m_chkOverwriteCss";
			this.m_chkOverwriteCss.UseVisualStyleBackColor = true;
			// 
			// m_txtCssFile
			// 
			resources.ApplyResources(this.m_txtCssFile, "m_txtCssFile");
			this.m_txtCssFile.Name = "m_txtCssFile";
			this.m_txtCssFile.TextChanged += new System.EventHandler(this.m_txtCssFile_TextChanged);
			// 
			// btnBrowseCss
			// 
			resources.ApplyResources(this.btnBrowseCss, "btnBrowseCss");
			this.btnBrowseCss.Name = "btnBrowseCss";
			this.btnBrowseCss.UseVisualStyleBackColor = true;
			this.btnBrowseCss.Click += new System.EventHandler(this.btnBrowseCss_Click);
			// 
			// m_rdoUseExternalCss
			// 
			resources.ApplyResources(this.m_rdoUseExternalCss, "m_rdoUseExternalCss");
			this.m_rdoUseExternalCss.Name = "m_rdoUseExternalCss";
			this.m_rdoUseExternalCss.UseVisualStyleBackColor = true;
			this.m_rdoUseExternalCss.CheckedChanged += new System.EventHandler(this.m_rdoUseExternalCss_CheckedChanged);
			// 
			// m_rdoEmbedStyleInfo
			// 
			resources.ApplyResources(this.m_rdoEmbedStyleInfo, "m_rdoEmbedStyleInfo");
			this.m_rdoEmbedStyleInfo.Checked = true;
			this.m_rdoEmbedStyleInfo.Name = "m_rdoEmbedStyleInfo";
			this.m_rdoEmbedStyleInfo.TabStop = true;
			this.m_rdoEmbedStyleInfo.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Controls.Add(this.tabGeneral);
			this.tabControl1.Controls.Add(this.tabOptions);
			this.tabControl1.Controls.Add(this.tabAppearance);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.m_grpRange);
			this.tabGeneral.Controls.Add(groupBox2);
			resources.ApplyResources(this.tabGeneral, "tabGeneral");
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// tabOptions
			// 
			this.tabOptions.Controls.Add(this.panel3);
			this.tabOptions.Controls.Add(this.m_cboUseLWC);
			this.tabOptions.Controls.Add(this.label3);
			this.tabOptions.Controls.Add(this.panel1);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCComments);
			this.tabOptions.Controls.Add(this.m_chkPassageBeforeOverview);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCAnswers);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCQuestions);
			resources.ApplyResources(this.tabOptions, "tabOptions");
			this.tabOptions.Name = "tabOptions";
			this.tabOptions.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Controls.Add(this.m_rdoDisplayReferenceForOutOfOrderQuestions);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(label10);
			this.panel3.Controls.Add(this.m_rdoOutputPassageForOutOfOrderQuestions);
			this.panel3.Name = "panel3";
			// 
			// m_rdoDisplayReferenceForOutOfOrderQuestions
			// 
			resources.ApplyResources(this.m_rdoDisplayReferenceForOutOfOrderQuestions, "m_rdoDisplayReferenceForOutOfOrderQuestions");
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Checked = true;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Name = "m_rdoDisplayReferenceForOutOfOrderQuestions";
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.TabStop = true;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.Name = "label8";
			// 
			// m_rdoOutputPassageForOutOfOrderQuestions
			// 
			resources.ApplyResources(this.m_rdoOutputPassageForOutOfOrderQuestions, "m_rdoOutputPassageForOutOfOrderQuestions");
			this.m_rdoOutputPassageForOutOfOrderQuestions.Name = "m_rdoOutputPassageForOutOfOrderQuestions";
			this.m_rdoOutputPassageForOutOfOrderQuestions.UseVisualStyleBackColor = true;
			// 
			// m_cboUseLWC
			// 
			this.m_cboUseLWC.FormattingEnabled = true;
			this.m_cboUseLWC.Items.AddRange(new object[] {
            resources.GetString("m_cboUseLWC.Items"),
            resources.GetString("m_cboUseLWC.Items1")});
			resources.ApplyResources(this.m_cboUseLWC, "m_cboUseLWC");
			this.m_cboUseLWC.Name = "m_cboUseLWC";
			this.m_cboUseLWC.SelectedIndexChanged += new System.EventHandler(this.HandleLWCSelectedIndexChanged);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// tabAppearance
			// 
			this.tabAppearance.Controls.Add(this.panel2);
			this.tabAppearance.Controls.Add(this.m_pnlCssOptions);
			this.tabAppearance.Controls.Add(label9);
			this.tabAppearance.Controls.Add(this.m_rdoEmbedStyleInfo);
			this.tabAppearance.Controls.Add(this.m_rdoUseExternalCss);
			resources.ApplyResources(this.tabAppearance, "tabAppearance");
			this.tabAppearance.Name = "tabAppearance";
			this.tabAppearance.UseVisualStyleBackColor = true;
			// 
			// GenerateScriptDlg
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenerateScriptDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			this.m_grpRange.ResumeLayout(false);
			this.m_grpRange.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numBlankLines)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.m_pnlCssOptions.ResumeLayout(false);
			this.m_pnlCssOptions.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabOptions.ResumeLayout(false);
			this.tabOptions.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.tabAppearance.ResumeLayout(false);
			this.tabAppearance.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox m_grpRange;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnChooseEnglishQuestionColor;
		private System.Windows.Forms.ColorDialog colorDlg;
		private System.Windows.Forms.Button btnChooserCommentColor;
		private System.Windows.Forms.Button btnChooseEnglishAnswerColor;
		private System.Windows.Forms.RadioButton m_rdoWholeBook;
		private System.Windows.Forms.ComboBox m_cboSection;
		private System.Windows.Forms.RadioButton m_rdoSingleSection;
		private System.Windows.Forms.ComboBox m_cboStartSection;
		private System.Windows.Forms.RadioButton m_rdoSectionRange;
		private System.Windows.Forms.ComboBox m_cboEndSection;
		private System.Windows.Forms.TextBox m_txtTitle;
		internal System.Windows.Forms.CheckBox m_chkPassageBeforeOverview;
		private System.Windows.Forms.TextBox m_txtFilename;
		internal System.Windows.Forms.CheckBox m_chkIncludeLWCQuestions;
		internal System.Windows.Forms.CheckBox m_chkIncludeLWCAnswers;
		internal System.Windows.Forms.CheckBox m_chkIncludeLWCComments;
		internal System.Windows.Forms.Label m_lblEnglishQuestionColor;
		internal System.Windows.Forms.Label m_lblCommentTextColor;
		internal System.Windows.Forms.Label m_lblEnglishAnswerTextColor;
		internal System.Windows.Forms.Label m_lblFolder;
		private System.Windows.Forms.ComboBox m_cboBooks;
		internal System.Windows.Forms.Label m_lblQuestionGroupHeadingsColor;
		private System.Windows.Forms.Button btnChooseQuestionGroupHeadingsColor;
		internal System.Windows.Forms.RadioButton m_rdoUseOriginal;
		internal System.Windows.Forms.RadioButton m_rdoDisplayWarning;
		private System.Windows.Forms.Label label6;
		internal System.Windows.Forms.NumericUpDown m_numBlankLines;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton m_rdoUseExternalCss;
		internal System.Windows.Forms.RadioButton m_rdoEmbedStyleInfo;
		private System.Windows.Forms.Button btnBrowseCss;
		private System.Windows.Forms.TextBox m_txtCssFile;
		private System.Windows.Forms.Panel m_pnlCssOptions;
		private System.Windows.Forms.CheckBox m_chkAbsoluteCssPath;
		internal System.Windows.Forms.CheckBox m_chkOverwriteCss;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		internal System.Windows.Forms.CheckBox m_chkNumberQuestions;
        private System.Windows.Forms.Label m_lbFilename;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.TabPage tabOptions;
		private System.Windows.Forms.ComboBox m_cboUseLWC;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TabPage tabAppearance;
		internal System.Windows.Forms.RadioButton m_rdoSkipUntranslated;
		private System.Windows.Forms.Panel panel3;
		internal System.Windows.Forms.RadioButton m_rdoDisplayReferenceForOutOfOrderQuestions;
		private System.Windows.Forms.Label label8;
		internal System.Windows.Forms.RadioButton m_rdoOutputPassageForOutOfOrderQuestions;
	}
}