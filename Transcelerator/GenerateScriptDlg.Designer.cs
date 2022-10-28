// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: GenerateScriptDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;

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
				LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
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
			this.components = new System.ComponentModel.Container();
			this.lblSectionRangeTo = new System.Windows.Forms.Label();
			this.groupBoxDocument = new System.Windows.Forms.GroupBox();
			this.m_lblFolder = new System.Windows.Forms.Label();
			this.lblFolder = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.m_txtFilename = new System.Windows.Forms.TextBox();
			this.m_lbFilename = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.m_txtTitle = new System.Windows.Forms.TextBox();
			this.lblUntranslatedQuestions = new System.Windows.Forms.Label();
			this.lblStyleSpecification = new System.Windows.Forms.Label();
			this.lblDetailQuestionsOutOfOrder = new System.Windows.Forms.Label();
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
			this.m_btnChooseCommentColor = new System.Windows.Forms.Button();
			this.m_lblLWCAnswerTextColor = new System.Windows.Forms.Label();
			this.m_btnChooseLWCAnswerColor = new System.Windows.Forms.Button();
			this.m_lblLWCQuestionColor = new System.Windows.Forms.Label();
			this.m_btnChooseLWCQuestionColor = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.colorDlg = new System.Windows.Forms.ColorDialog();
			this.m_numBlankLines = new System.Windows.Forms.NumericUpDown();
			this.panel2 = new System.Windows.Forms.Panel();
			this.m_chkNumberQuestions = new System.Windows.Forms.CheckBox();
			this.lblExtraAnswerLines = new System.Windows.Forms.Label();
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
			this.m_chkIncludeVerseNumbers = new System.Windows.Forms.CheckBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.m_rdoDisplayReferenceForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.m_rdoOutputPassageForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.m_cboUseLWC = new System.Windows.Forms.ComboBox();
			this.lblUseLWC = new System.Windows.Forms.Label();
			this.tabAppearance = new System.Windows.Forms.TabPage();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.groupBoxDocument.SuspendLayout();
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
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblSectionRangeTo
			// 
			this.lblSectionRangeTo.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSectionRangeTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSectionRangeTo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSectionRangeTo, "GenerateScriptDlg.lblSectionRangeTo");
			this.lblSectionRangeTo.Location = new System.Drawing.Point(310, 67);
			this.lblSectionRangeTo.Name = "lblSectionRangeTo";
			this.lblSectionRangeTo.Size = new System.Drawing.Size(16, 13);
			this.lblSectionRangeTo.TabIndex = 6;
			this.lblSectionRangeTo.Text = "to";
			// 
			// groupBoxDocument
			// 
			this.groupBoxDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxDocument.Controls.Add(this.m_lblFolder);
			this.groupBoxDocument.Controls.Add(this.lblFolder);
			this.groupBoxDocument.Controls.Add(this.btnBrowse);
			this.groupBoxDocument.Controls.Add(this.m_txtFilename);
			this.groupBoxDocument.Controls.Add(this.m_lbFilename);
			this.groupBoxDocument.Controls.Add(this.lblTitle);
			this.groupBoxDocument.Controls.Add(this.m_txtTitle);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.groupBoxDocument, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.groupBoxDocument, null);
			this.l10NSharpExtender1.SetLocalizingId(this.groupBoxDocument, "GenerateScriptDlg.groupBoxDocument");
			this.groupBoxDocument.Location = new System.Drawing.Point(6, 109);
			this.groupBoxDocument.Name = "groupBoxDocument";
			this.groupBoxDocument.Size = new System.Drawing.Size(503, 99);
			this.groupBoxDocument.TabIndex = 3;
			this.groupBoxDocument.TabStop = false;
			this.groupBoxDocument.Text = "Document";
			// 
			// m_lblFolder
			// 
			this.m_lblFolder.AutoSize = true;
			this.m_lblFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblFolder, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblFolder, "GenerateScriptDlg.m_lblFolder");
			this.m_lblFolder.Location = new System.Drawing.Point(141, 70);
			this.m_lblFolder.Name = "m_lblFolder";
			this.m_lblFolder.Size = new System.Drawing.Size(14, 13);
			this.m_lblFolder.TabIndex = 6;
			this.m_lblFolder.Text = "#";
			// 
			// lblFolder
			// 
			this.lblFolder.AutoSize = true;
			this.lblFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblFolder, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblFolder, "GenerateScriptDlg.lblFolder");
			this.lblFolder.Location = new System.Drawing.Point(6, 70);
			this.lblFolder.Name = "lblFolder";
			this.lblFolder.Size = new System.Drawing.Size(39, 13);
			this.lblFolder.TabIndex = 5;
			this.lblFolder.Text = "Folder:";
			// 
			// btnBrowse
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnBrowse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnBrowse, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.btnBrowse, "GenerateScriptDlg.btnBrowse");
			this.btnBrowse.Location = new System.Drawing.Point(419, 46);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 4;
			this.btnBrowse.Text = "&Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// m_txtFilename
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtFilename, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtFilename, "Param 0: project name; Param 1: Scripture book or reference range");
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtFilename, "GenerateScriptDlg.m_txtFilename");
			this.m_txtFilename.Location = new System.Drawing.Point(141, 48);
			this.m_txtFilename.Name = "m_txtFilename";
			this.m_txtFilename.Size = new System.Drawing.Size(272, 20);
			this.m_txtFilename.TabIndex = 3;
			this.m_txtFilename.Text = "{0} Comprehension Check for {1}.htm";
			// 
			// m_lbFilename
			// 
			this.m_lbFilename.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lbFilename, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lbFilename, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lbFilename, "GenerateScriptDlg.m_lbFilename");
			this.m_lbFilename.Location = new System.Drawing.Point(6, 48);
			this.m_lbFilename.Name = "m_lbFilename";
			this.m_lbFilename.Size = new System.Drawing.Size(85, 13);
			this.m_lbFilename.TabIndex = 2;
			this.m_lbFilename.Text = "Script &File name:";
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblTitle, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.lblTitle, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.lblTitle, "GenerateScriptDlg.lblTitle");
			this.lblTitle.Location = new System.Drawing.Point(6, 22);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(30, 13);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Title:";
			// 
			// m_txtTitle
			// 
			this.m_txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtTitle, "Param is a Scripture book or reference range");
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtTitle, "GenerateScriptDlg.m_txtTitle");
			this.m_txtTitle.Location = new System.Drawing.Point(141, 19);
			this.m_txtTitle.Name = "m_txtTitle";
			this.m_txtTitle.Size = new System.Drawing.Size(353, 20);
			this.m_txtTitle.TabIndex = 1;
			this.m_txtTitle.Text = "Comprehension Checking Questions for {0}";
			// 
			// lblUntranslatedQuestions
			// 
			this.lblUntranslatedQuestions.AutoSize = true;
			this.lblUntranslatedQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblUntranslatedQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblUntranslatedQuestions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblUntranslatedQuestions, "GenerateScriptDlg.lblUntranslatedQuestions");
			this.lblUntranslatedQuestions.Location = new System.Drawing.Point(0, 8);
			this.lblUntranslatedQuestions.Name = "lblUntranslatedQuestions";
			this.lblUntranslatedQuestions.Size = new System.Drawing.Size(194, 13);
			this.lblUntranslatedQuestions.TabIndex = 17;
			this.lblUntranslatedQuestions.Text = "How to Handle Untranslated Questions:";
			// 
			// lblStyleSpecification
			// 
			this.lblStyleSpecification.AutoSize = true;
			this.lblStyleSpecification.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblStyleSpecification, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblStyleSpecification, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblStyleSpecification, "GenerateScriptDlg.lblStyleSpecification");
			this.lblStyleSpecification.Location = new System.Drawing.Point(12, 15);
			this.lblStyleSpecification.Name = "lblStyleSpecification";
			this.lblStyleSpecification.Size = new System.Drawing.Size(123, 13);
			this.lblStyleSpecification.TabIndex = 0;
			this.lblStyleSpecification.Text = "Where to Specify Styles:";
			// 
			// lblDetailQuestionsOutOfOrder
			// 
			this.lblDetailQuestionsOutOfOrder.AutoSize = true;
			this.lblDetailQuestionsOutOfOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblDetailQuestionsOutOfOrder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblDetailQuestionsOutOfOrder, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblDetailQuestionsOutOfOrder, "GenerateScriptDlg.lblDetailQuestionsOutOfOrder");
			this.lblDetailQuestionsOutOfOrder.Location = new System.Drawing.Point(0, 8);
			this.lblDetailQuestionsOutOfOrder.Name = "lblDetailQuestionsOutOfOrder";
			this.lblDetailQuestionsOutOfOrder.Size = new System.Drawing.Size(196, 13);
			this.lblDetailQuestionsOutOfOrder.TabIndex = 17;
			this.lblDetailQuestionsOutOfOrder.Text = "When Detail Questions are out of Order:";
			// 
			// m_rdoWholeBook
			// 
			this.m_rdoWholeBook.AutoSize = true;
			this.m_rdoWholeBook.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoWholeBook, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoWholeBook, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoWholeBook, "GenerateScriptDlg.m_rdoWholeBook");
			this.m_rdoWholeBook.Location = new System.Drawing.Point(6, 19);
			this.m_rdoWholeBook.Name = "m_rdoWholeBook";
			this.m_rdoWholeBook.Size = new System.Drawing.Size(84, 17);
			this.m_rdoWholeBook.TabIndex = 0;
			this.m_rdoWholeBook.TabStop = true;
			this.m_rdoWholeBook.Text = "&Whole Book";
			this.m_rdoWholeBook.UseVisualStyleBackColor = true;
			this.m_rdoWholeBook.CheckedChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSelectedBook);
			// 
			// m_grpRange
			// 
			this.m_grpRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_grpRange.Controls.Add(this.m_cboBooks);
			this.m_grpRange.Controls.Add(this.m_cboEndSection);
			this.m_grpRange.Controls.Add(this.lblSectionRangeTo);
			this.m_grpRange.Controls.Add(this.m_cboStartSection);
			this.m_grpRange.Controls.Add(this.m_rdoSectionRange);
			this.m_grpRange.Controls.Add(this.m_cboSection);
			this.m_grpRange.Controls.Add(this.m_rdoSingleSection);
			this.m_grpRange.Controls.Add(this.m_rdoWholeBook);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_grpRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_grpRange, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_grpRange, "GenerateScriptDlg.m_grpRange");
			this.m_grpRange.Location = new System.Drawing.Point(6, 6);
			this.m_grpRange.Name = "m_grpRange";
			this.m_grpRange.Size = new System.Drawing.Size(503, 97);
			this.m_grpRange.TabIndex = 2;
			this.m_grpRange.TabStop = false;
			this.m_grpRange.Text = "Range";
			// 
			// m_cboBooks
			// 
			this.m_cboBooks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboBooks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboBooks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboBooks.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboBooks, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboBooks, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboBooks, "GenerateScriptDlg.m_cboBooks");
			this.m_cboBooks.Location = new System.Drawing.Point(141, 14);
			this.m_cboBooks.Name = "m_cboBooks";
			this.m_cboBooks.Size = new System.Drawing.Size(158, 21);
			this.m_cboBooks.TabIndex = 1;
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
			this.m_cboEndSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboEndSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboEndSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboEndSection, "GenerateScriptDlg.m_cboEndSection");
			this.m_cboEndSection.Location = new System.Drawing.Point(336, 64);
			this.m_cboEndSection.MaxDropDownItems = 12;
			this.m_cboEndSection.Name = "m_cboEndSection";
			this.m_cboEndSection.Size = new System.Drawing.Size(158, 21);
			this.m_cboEndSection.TabIndex = 7;
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
			this.m_cboStartSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboStartSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboStartSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboStartSection, "GenerateScriptDlg.m_cboStartSection");
			this.m_cboStartSection.Location = new System.Drawing.Point(141, 64);
			this.m_cboStartSection.MaxDropDownItems = 12;
			this.m_cboStartSection.Name = "m_cboStartSection";
			this.m_cboStartSection.Size = new System.Drawing.Size(158, 21);
			this.m_cboStartSection.TabIndex = 5;
			this.m_cboStartSection.SelectedIndexChanged += new System.EventHandler(this.m_cboStartSection_SelectedIndexChanged);
			this.m_cboStartSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboStartSection.Enter += new System.EventHandler(this.SectionRangeCombo_Enter);
			// 
			// m_rdoSectionRange
			// 
			this.m_rdoSectionRange.AutoSize = true;
			this.m_rdoSectionRange.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSectionRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSectionRange, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSectionRange, "GenerateScriptDlg.m_rdoSectionRange");
			this.m_rdoSectionRange.Location = new System.Drawing.Point(6, 65);
			this.m_rdoSectionRange.Name = "m_rdoSectionRange";
			this.m_rdoSectionRange.Size = new System.Drawing.Size(113, 17);
			this.m_rdoSectionRange.TabIndex = 4;
			this.m_rdoSectionRange.Text = "&Range of Sections";
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
			this.m_cboSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboSection, "GenerateScriptDlg.m_cboSection");
			this.m_cboSection.Location = new System.Drawing.Point(141, 41);
			this.m_cboSection.MaxDropDownItems = 12;
			this.m_cboSection.Name = "m_cboSection";
			this.m_cboSection.Size = new System.Drawing.Size(158, 21);
			this.m_cboSection.TabIndex = 3;
			this.m_cboSection.SelectedIndexChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			this.m_cboSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboSection.Enter += new System.EventHandler(this.m_cboSection_Enter);
			// 
			// m_rdoSingleSection
			// 
			this.m_rdoSingleSection.AutoSize = true;
			this.m_rdoSingleSection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSingleSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSingleSection, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSingleSection, "GenerateScriptDlg.m_rdoSingleSection");
			this.m_rdoSingleSection.Location = new System.Drawing.Point(6, 42);
			this.m_rdoSingleSection.Name = "m_rdoSingleSection";
			this.m_rdoSingleSection.Size = new System.Drawing.Size(93, 17);
			this.m_rdoSingleSection.TabIndex = 2;
			this.m_rdoSingleSection.Text = "&Single Section";
			this.m_rdoSingleSection.UseVisualStyleBackColor = true;
			this.m_rdoSingleSection.CheckedChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			// 
			// m_chkPassageBeforeOverview
			// 
			this.m_chkPassageBeforeOverview.AutoSize = true;
			this.m_chkPassageBeforeOverview.Checked = true;
			this.m_chkPassageBeforeOverview.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkPassageBeforeOverview, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkPassageBeforeOverview, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkPassageBeforeOverview, "GenerateScriptDlg.m_chkPassageBeforeOverview");
			this.m_chkPassageBeforeOverview.Location = new System.Drawing.Point(8, 14);
			this.m_chkPassageBeforeOverview.Name = "m_chkPassageBeforeOverview";
			this.m_chkPassageBeforeOverview.Size = new System.Drawing.Size(241, 17);
			this.m_chkPassageBeforeOverview.TabIndex = 0;
			this.m_chkPassageBeforeOverview.Text = "&Include Entire Passage at the Start of Section";
			this.m_chkPassageBeforeOverview.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.m_rdoSkipUntranslated);
			this.panel1.Controls.Add(this.m_rdoDisplayWarning);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.lblUntranslatedQuestions);
			this.panel1.Controls.Add(this.m_rdoUseOriginal);
			this.panel1.Location = new System.Drawing.Point(256, 6);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(246, 102);
			this.panel1.TabIndex = 4;
			// 
			// m_rdoSkipUntranslated
			// 
			this.m_rdoSkipUntranslated.AutoSize = true;
			this.m_rdoSkipUntranslated.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSkipUntranslated, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSkipUntranslated, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSkipUntranslated, "GenerateScriptDlg.m_rdoSkipUntranslated");
			this.m_rdoSkipUntranslated.Location = new System.Drawing.Point(3, 74);
			this.m_rdoSkipUntranslated.Name = "m_rdoSkipUntranslated";
			this.m_rdoSkipUntranslated.Size = new System.Drawing.Size(138, 17);
			this.m_rdoSkipUntranslated.TabIndex = 21;
			this.m_rdoSkipUntranslated.Text = "Do &Not Include in Script";
			this.m_rdoSkipUntranslated.UseVisualStyleBackColor = true;
			// 
			// m_rdoDisplayWarning
			// 
			this.m_rdoDisplayWarning.AutoSize = true;
			this.m_rdoDisplayWarning.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoDisplayWarning, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoDisplayWarning, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoDisplayWarning, "GenerateScriptDlg.m_rdoDisplayWarning");
			this.m_rdoDisplayWarning.Location = new System.Drawing.Point(3, 29);
			this.m_rdoDisplayWarning.Name = "m_rdoDisplayWarning";
			this.m_rdoDisplayWarning.Size = new System.Drawing.Size(157, 17);
			this.m_rdoDisplayWarning.TabIndex = 18;
			this.m_rdoDisplayWarning.TabStop = true;
			this.m_rdoDisplayWarning.Text = "&Display a Warning Message";
			this.m_rdoDisplayWarning.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label6, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label6, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.label6, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.label6, "GenerateScriptDlg.label6");
			this.label6.Location = new System.Drawing.Point(0, 23);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(246, 2);
			this.label6.TabIndex = 20;
			// 
			// m_rdoUseOriginal
			// 
			this.m_rdoUseOriginal.AutoSize = true;
			this.m_rdoUseOriginal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUseOriginal, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUseOriginal, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUseOriginal, "GenerateScriptDlg.m_rdoUseOriginal");
			this.m_rdoUseOriginal.Location = new System.Drawing.Point(3, 52);
			this.m_rdoUseOriginal.Name = "m_rdoUseOriginal";
			this.m_rdoUseOriginal.Size = new System.Drawing.Size(208, 17);
			this.m_rdoUseOriginal.TabIndex = 19;
			this.m_rdoUseOriginal.Text = "&Use the Original Untranslated Question";
			this.m_rdoUseOriginal.UseVisualStyleBackColor = true;
			// 
			// m_chkIncludeLWCComments
			// 
			this.m_chkIncludeLWCComments.AutoSize = true;
			this.m_chkIncludeLWCComments.Checked = true;
			this.m_chkIncludeLWCComments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCComments.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeLWCComments, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeLWCComments, "Param is the name of a language. To control which character will be the mnemonic " +
        "key (underlined when the user presses the ALT key), put the ampersand before the" +
        " desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeLWCComments, "GenerateScriptDlg.m_chkIncludeLWCComments");
			this.m_chkIncludeLWCComments.Location = new System.Drawing.Point(10, 173);
			this.m_chkIncludeLWCComments.Name = "m_chkIncludeLWCComments";
			this.m_chkIncludeLWCComments.Size = new System.Drawing.Size(147, 17);
			this.m_chkIncludeLWCComments.TabIndex = 3;
			this.m_chkIncludeLWCComments.Text = "Include &Comments (in {0})";
			this.m_chkIncludeLWCComments.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCComments.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_chkIncludeLWCAnswers
			// 
			this.m_chkIncludeLWCAnswers.AutoSize = true;
			this.m_chkIncludeLWCAnswers.Checked = true;
			this.m_chkIncludeLWCAnswers.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCAnswers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeLWCAnswers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeLWCAnswers, "Param is the name of a language. To control which character will be the mnemonic " +
        "key (underlined when the user presses the ALT key), put the ampersand before the" +
        " desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeLWCAnswers, "GenerateScriptDlg.m_chkIncludeLWCAnswers");
			this.m_chkIncludeLWCAnswers.Location = new System.Drawing.Point(10, 150);
			this.m_chkIncludeLWCAnswers.Name = "m_chkIncludeLWCAnswers";
			this.m_chkIncludeLWCAnswers.Size = new System.Drawing.Size(141, 17);
			this.m_chkIncludeLWCAnswers.TabIndex = 2;
			this.m_chkIncludeLWCAnswers.Text = "Include &Answers (in {0}) ";
			this.m_chkIncludeLWCAnswers.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCAnswers.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_chkIncludeLWCQuestions
			// 
			this.m_chkIncludeLWCQuestions.AutoSize = true;
			this.m_chkIncludeLWCQuestions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.m_chkIncludeLWCQuestions.Checked = true;
			this.m_chkIncludeLWCQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_chkIncludeLWCQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeLWCQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeLWCQuestions, "Param is the name of a language. To control which character will be the mnemonic " +
        "key (underlined when the user presses the ALT key), put the ampersand before the" +
        " desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeLWCQuestions, "GenerateScriptDlg.m_chkIncludeLWCQuestions");
			this.m_chkIncludeLWCQuestions.Location = new System.Drawing.Point(10, 127);
			this.m_chkIncludeLWCQuestions.Name = "m_chkIncludeLWCQuestions";
			this.m_chkIncludeLWCQuestions.Size = new System.Drawing.Size(139, 17);
			this.m_chkIncludeLWCQuestions.TabIndex = 1;
			this.m_chkIncludeLWCQuestions.Text = "Include &Questions in {0}";
			this.m_chkIncludeLWCQuestions.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCQuestions.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_lblQuestionGroupHeadingsColor
			// 
			this.m_lblQuestionGroupHeadingsColor.AutoSize = true;
			this.m_lblQuestionGroupHeadingsColor.ForeColor = System.Drawing.Color.Blue;
			this.m_lblQuestionGroupHeadingsColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblQuestionGroupHeadingsColor, "GenerateScriptDlg.m_lblQuestionGroupHeadingsColor");
			this.m_lblQuestionGroupHeadingsColor.Location = new System.Drawing.Point(0, 32);
			this.m_lblQuestionGroupHeadingsColor.Name = "m_lblQuestionGroupHeadingsColor";
			this.m_lblQuestionGroupHeadingsColor.Size = new System.Drawing.Size(156, 13);
			this.m_lblQuestionGroupHeadingsColor.TabIndex = 2;
			this.m_lblQuestionGroupHeadingsColor.Text = "Question Group Headings Color";
			// 
			// btnChooseQuestionGroupHeadingsColor
			// 
			this.btnChooseQuestionGroupHeadingsColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnChooseQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnChooseQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.btnChooseQuestionGroupHeadingsColor, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.btnChooseQuestionGroupHeadingsColor, "GenerateScriptDlg.btnChooseQuestionGroupHeadingsColor");
			this.btnChooseQuestionGroupHeadingsColor.Location = new System.Drawing.Point(201, 27);
			this.btnChooseQuestionGroupHeadingsColor.Name = "btnChooseQuestionGroupHeadingsColor";
			this.btnChooseQuestionGroupHeadingsColor.Size = new System.Drawing.Size(25, 23);
			this.btnChooseQuestionGroupHeadingsColor.TabIndex = 3;
			this.btnChooseQuestionGroupHeadingsColor.Text = "...";
			this.btnChooseQuestionGroupHeadingsColor.UseVisualStyleBackColor = true;
			this.btnChooseQuestionGroupHeadingsColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblCommentTextColor
			// 
			this.m_lblCommentTextColor.AutoSize = true;
			this.m_lblCommentTextColor.ForeColor = System.Drawing.Color.Red;
			this.m_lblCommentTextColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCommentTextColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCommentTextColor, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblCommentTextColor, "GenerateScriptDlg.m_lblCommentTextColor");
			this.m_lblCommentTextColor.Location = new System.Drawing.Point(0, 101);
			this.m_lblCommentTextColor.Name = "m_lblCommentTextColor";
			this.m_lblCommentTextColor.Size = new System.Drawing.Size(102, 13);
			this.m_lblCommentTextColor.TabIndex = 8;
			this.m_lblCommentTextColor.Text = "Comment Text Color";
			// 
			// m_btnChooseCommentColor
			// 
			this.m_btnChooseCommentColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnChooseCommentColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnChooseCommentColor, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_btnChooseCommentColor, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnChooseCommentColor, "GenerateScriptDlg.m_btnChooseCommentColor");
			this.m_btnChooseCommentColor.Location = new System.Drawing.Point(201, 96);
			this.m_btnChooseCommentColor.Name = "m_btnChooseCommentColor";
			this.m_btnChooseCommentColor.Size = new System.Drawing.Size(25, 23);
			this.m_btnChooseCommentColor.TabIndex = 9;
			this.m_btnChooseCommentColor.Text = "...";
			this.m_btnChooseCommentColor.UseVisualStyleBackColor = true;
			this.m_btnChooseCommentColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.m_btnChooseCommentColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblLWCAnswerTextColor
			// 
			this.m_lblLWCAnswerTextColor.AutoSize = true;
			this.m_lblLWCAnswerTextColor.ForeColor = System.Drawing.Color.Green;
			this.m_lblLWCAnswerTextColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblLWCAnswerTextColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblLWCAnswerTextColor, "Param is the name of a language of wider communication");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblLWCAnswerTextColor, "GenerateScriptDlg.m_lblLWCAnswerTextColor");
			this.m_lblLWCAnswerTextColor.Location = new System.Drawing.Point(0, 78);
			this.m_lblLWCAnswerTextColor.Name = "m_lblLWCAnswerTextColor";
			this.m_lblLWCAnswerTextColor.Size = new System.Drawing.Size(110, 13);
			this.m_lblLWCAnswerTextColor.TabIndex = 6;
			this.m_lblLWCAnswerTextColor.Text = "{0} Answer Text Color";
			// 
			// m_btnChooseLWCAnswerColor
			// 
			this.m_btnChooseLWCAnswerColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnChooseLWCAnswerColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnChooseLWCAnswerColor, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_btnChooseLWCAnswerColor, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnChooseLWCAnswerColor, "GenerateScriptDlg.m_btnChooseLWCAnswerColor");
			this.m_btnChooseLWCAnswerColor.Location = new System.Drawing.Point(201, 73);
			this.m_btnChooseLWCAnswerColor.Name = "m_btnChooseLWCAnswerColor";
			this.m_btnChooseLWCAnswerColor.Size = new System.Drawing.Size(25, 23);
			this.m_btnChooseLWCAnswerColor.TabIndex = 7;
			this.m_btnChooseLWCAnswerColor.Text = "...";
			this.m_btnChooseLWCAnswerColor.UseVisualStyleBackColor = true;
			this.m_btnChooseLWCAnswerColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.m_btnChooseLWCAnswerColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblLWCQuestionColor
			// 
			this.m_lblLWCQuestionColor.AutoSize = true;
			this.m_lblLWCQuestionColor.ForeColor = System.Drawing.Color.Gray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblLWCQuestionColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblLWCQuestionColor, "Param is the name of a language of wider communication");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblLWCQuestionColor, "GenerateScriptDlg.m_lblLWCQuestionColor");
			this.m_lblLWCQuestionColor.Location = new System.Drawing.Point(0, 55);
			this.m_lblLWCQuestionColor.Name = "m_lblLWCQuestionColor";
			this.m_lblLWCQuestionColor.Size = new System.Drawing.Size(117, 13);
			this.m_lblLWCQuestionColor.TabIndex = 4;
			this.m_lblLWCQuestionColor.Text = "{0} Question Text Color";
			// 
			// m_btnChooseLWCQuestionColor
			// 
			this.m_btnChooseLWCQuestionColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnChooseLWCQuestionColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnChooseLWCQuestionColor, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_btnChooseLWCQuestionColor, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnChooseLWCQuestionColor, "GenerateScriptDlg.m_btnChooseLWCQuestionColor");
			this.m_btnChooseLWCQuestionColor.Location = new System.Drawing.Point(201, 50);
			this.m_btnChooseLWCQuestionColor.Name = "m_btnChooseLWCQuestionColor";
			this.m_btnChooseLWCQuestionColor.Size = new System.Drawing.Size(25, 23);
			this.m_btnChooseLWCQuestionColor.TabIndex = 5;
			this.m_btnChooseLWCQuestionColor.Text = "...";
			this.m_btnChooseLWCQuestionColor.UseVisualStyleBackColor = true;
			this.m_btnChooseLWCQuestionColor.EnabledChanged += new System.EventHandler(this.ColorSelectionButtonEnabledStateChanged);
			this.m_btnChooseLWCQuestionColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(189, 258);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Location = new System.Drawing.Point(272, 258);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// colorDlg
			// 
			this.colorDlg.AnyColor = true;
			this.colorDlg.SolidColorOnly = true;
			// 
			// m_numBlankLines
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_numBlankLines, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_numBlankLines, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_numBlankLines, "GenerateScriptDlg.m_numBlankLines");
			this.m_numBlankLines.Location = new System.Drawing.Point(201, 3);
			this.m_numBlankLines.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.m_numBlankLines.Name = "m_numBlankLines";
			this.m_numBlankLines.Size = new System.Drawing.Size(44, 20);
			this.m_numBlankLines.TabIndex = 1;
			this.m_numBlankLines.EnabledChanged += new System.EventHandler(this.m_numBlankLines_EnabledChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.m_chkNumberQuestions);
			this.panel2.Controls.Add(this.lblExtraAnswerLines);
			this.panel2.Controls.Add(this.btnChooseQuestionGroupHeadingsColor);
			this.panel2.Controls.Add(this.m_lblCommentTextColor);
			this.panel2.Controls.Add(this.m_lblQuestionGroupHeadingsColor);
			this.panel2.Controls.Add(this.m_btnChooseCommentColor);
			this.panel2.Controls.Add(this.m_lblLWCAnswerTextColor);
			this.panel2.Controls.Add(this.m_btnChooseLWCAnswerColor);
			this.panel2.Controls.Add(this.m_numBlankLines);
			this.panel2.Controls.Add(this.m_lblLWCQuestionColor);
			this.panel2.Controls.Add(this.m_btnChooseLWCQuestionColor);
			this.panel2.Location = new System.Drawing.Point(250, 6);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(248, 152);
			this.panel2.TabIndex = 4;
			// 
			// m_chkNumberQuestions
			// 
			this.m_chkNumberQuestions.AutoSize = true;
			this.m_chkNumberQuestions.Checked = true;
			this.m_chkNumberQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkNumberQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkNumberQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkNumberQuestions, "GenerateScriptDlg.m_chkNumberQuestions");
			this.m_chkNumberQuestions.Location = new System.Drawing.Point(0, 127);
			this.m_chkNumberQuestions.Name = "m_chkNumberQuestions";
			this.m_chkNumberQuestions.Size = new System.Drawing.Size(111, 17);
			this.m_chkNumberQuestions.TabIndex = 10;
			this.m_chkNumberQuestions.Text = "&Number questions";
			this.m_chkNumberQuestions.UseVisualStyleBackColor = true;
			// 
			// lblExtraAnswerLines
			// 
			this.lblExtraAnswerLines.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblExtraAnswerLines, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblExtraAnswerLines, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.lblExtraAnswerLines, "GenerateScriptDlg.label7");
			this.lblExtraAnswerLines.Location = new System.Drawing.Point(0, 9);
			this.lblExtraAnswerLines.Name = "lblExtraAnswerLines";
			this.lblExtraAnswerLines.Size = new System.Drawing.Size(172, 13);
			this.lblExtraAnswerLines.TabIndex = 0;
			this.lblExtraAnswerLines.Text = "Extra &Lines for Recording Answers:";
			// 
			// m_pnlCssOptions
			// 
			this.m_pnlCssOptions.Controls.Add(this.m_chkAbsoluteCssPath);
			this.m_pnlCssOptions.Controls.Add(this.m_chkOverwriteCss);
			this.m_pnlCssOptions.Controls.Add(this.m_txtCssFile);
			this.m_pnlCssOptions.Controls.Add(this.btnBrowseCss);
			this.m_pnlCssOptions.Location = new System.Drawing.Point(30, 83);
			this.m_pnlCssOptions.Name = "m_pnlCssOptions";
			this.m_pnlCssOptions.Size = new System.Drawing.Size(200, 75);
			this.m_pnlCssOptions.TabIndex = 30;
			this.m_pnlCssOptions.Visible = false;
			// 
			// m_chkAbsoluteCssPath
			// 
			this.m_chkAbsoluteCssPath.AutoSize = true;
			this.m_chkAbsoluteCssPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkAbsoluteCssPath, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkAbsoluteCssPath, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkAbsoluteCssPath, "GenerateScriptDlg.m_chkAbsoluteCssPath");
			this.m_chkAbsoluteCssPath.Location = new System.Drawing.Point(0, 27);
			this.m_chkAbsoluteCssPath.Name = "m_chkAbsoluteCssPath";
			this.m_chkAbsoluteCssPath.Size = new System.Drawing.Size(122, 17);
			this.m_chkAbsoluteCssPath.TabIndex = 2;
			this.m_chkAbsoluteCssPath.Text = "Make &Path Absolute";
			this.m_chkAbsoluteCssPath.UseVisualStyleBackColor = true;
			// 
			// m_chkOverwriteCss
			// 
			this.m_chkOverwriteCss.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkOverwriteCss, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkOverwriteCss, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkOverwriteCss, "GenerateScriptDlg.m_chkOverwriteCss");
			this.m_chkOverwriteCss.Location = new System.Drawing.Point(0, 50);
			this.m_chkOverwriteCss.Name = "m_chkOverwriteCss";
			this.m_chkOverwriteCss.Size = new System.Drawing.Size(153, 17);
			this.m_chkOverwriteCss.TabIndex = 3;
			this.m_chkOverwriteCss.Text = "O&verwrite Existing CSS File";
			this.m_chkOverwriteCss.UseVisualStyleBackColor = true;
			// 
			// m_txtCssFile
			// 
			this.m_txtCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtCssFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtCssFile, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtCssFile, "GenerateScriptDlg.m_txtCssFile");
			this.m_txtCssFile.Location = new System.Drawing.Point(0, 2);
			this.m_txtCssFile.Name = "m_txtCssFile";
			this.m_txtCssFile.Size = new System.Drawing.Size(165, 20);
			this.m_txtCssFile.TabIndex = 0;
			this.m_txtCssFile.Text = "ComprehensionChecking.css";
			this.m_txtCssFile.TextChanged += new System.EventHandler(this.m_txtCssFile_TextChanged);
			// 
			// btnBrowseCss
			// 
			this.btnBrowseCss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseCss.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnBrowseCss, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnBrowseCss, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.btnBrowseCss, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.btnBrowseCss, "GenerateScriptDlg.btnBrowseCss");
			this.btnBrowseCss.Location = new System.Drawing.Point(174, 0);
			this.btnBrowseCss.Name = "btnBrowseCss";
			this.btnBrowseCss.Size = new System.Drawing.Size(25, 23);
			this.btnBrowseCss.TabIndex = 1;
			this.btnBrowseCss.Text = "...";
			this.btnBrowseCss.UseVisualStyleBackColor = true;
			this.btnBrowseCss.Click += new System.EventHandler(this.btnBrowseCss_Click);
			// 
			// m_rdoUseExternalCss
			// 
			this.m_rdoUseExternalCss.AutoSize = true;
			this.m_rdoUseExternalCss.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUseExternalCss, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUseExternalCss, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUseExternalCss, "GenerateScriptDlg.m_rdoUseExternalCss");
			this.m_rdoUseExternalCss.Location = new System.Drawing.Point(15, 63);
			this.m_rdoUseExternalCss.Name = "m_rdoUseExternalCss";
			this.m_rdoUseExternalCss.Size = new System.Drawing.Size(143, 17);
			this.m_rdoUseExternalCss.TabIndex = 3;
			this.m_rdoUseExternalCss.Text = "Use an E&xternal CSS File";
			this.m_rdoUseExternalCss.UseVisualStyleBackColor = true;
			this.m_rdoUseExternalCss.CheckedChanged += new System.EventHandler(this.m_rdoUseExternalCss_CheckedChanged);
			// 
			// m_rdoEmbedStyleInfo
			// 
			this.m_rdoEmbedStyleInfo.AutoSize = true;
			this.m_rdoEmbedStyleInfo.Checked = true;
			this.m_rdoEmbedStyleInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoEmbedStyleInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoEmbedStyleInfo, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoEmbedStyleInfo, "GenerateScriptDlg.m_rdoEmbedStyleInfo");
			this.m_rdoEmbedStyleInfo.Location = new System.Drawing.Point(15, 40);
			this.m_rdoEmbedStyleInfo.Name = "m_rdoEmbedStyleInfo";
			this.m_rdoEmbedStyleInfo.Size = new System.Drawing.Size(200, 17);
			this.m_rdoEmbedStyleInfo.TabIndex = 2;
			this.m_rdoEmbedStyleInfo.TabStop = true;
			this.m_rdoEmbedStyleInfo.Text = "&Embed in Script (Maximum Portability)";
			this.m_rdoEmbedStyleInfo.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabGeneral);
			this.tabControl1.Controls.Add(this.tabOptions);
			this.tabControl1.Controls.Add(this.tabAppearance);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(516, 232);
			this.tabControl1.TabIndex = 6;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.m_grpRange);
			this.tabGeneral.Controls.Add(this.groupBoxDocument);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabGeneral, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabGeneral, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabGeneral, "GenerateScriptDlg.tabGeneral");
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabGeneral.Size = new System.Drawing.Size(508, 206);
			this.tabGeneral.TabIndex = 0;
			this.tabGeneral.Text = "General";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// tabOptions
			// 
			this.tabOptions.Controls.Add(this.m_chkIncludeVerseNumbers);
			this.tabOptions.Controls.Add(this.panel3);
			this.tabOptions.Controls.Add(this.m_cboUseLWC);
			this.tabOptions.Controls.Add(this.lblUseLWC);
			this.tabOptions.Controls.Add(this.panel1);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCComments);
			this.tabOptions.Controls.Add(this.m_chkPassageBeforeOverview);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCAnswers);
			this.tabOptions.Controls.Add(this.m_chkIncludeLWCQuestions);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabOptions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabOptions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabOptions, "GenerateScriptDlg.tabOptions");
			this.tabOptions.Location = new System.Drawing.Point(4, 22);
			this.tabOptions.Name = "tabOptions";
			this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptions.Size = new System.Drawing.Size(508, 206);
			this.tabOptions.TabIndex = 1;
			this.tabOptions.Text = "Options";
			this.tabOptions.UseVisualStyleBackColor = true;
			// 
			// m_chkIncludeVerseNumbers
			// 
			this.m_chkIncludeVerseNumbers.AutoSize = true;
			this.m_chkIncludeVerseNumbers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeVerseNumbers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeVerseNumbers, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeVerseNumbers, "GenerateScriptDlg.m_chkIncludeVerseNumbers");
			this.m_chkIncludeVerseNumbers.Location = new System.Drawing.Point(8, 35);
			this.m_chkIncludeVerseNumbers.Name = "m_chkIncludeVerseNumbers";
			this.m_chkIncludeVerseNumbers.Size = new System.Drawing.Size(133, 17);
			this.m_chkIncludeVerseNumbers.TabIndex = 7;
			this.m_chkIncludeVerseNumbers.Text = "Include verse numbers";
			this.m_chkIncludeVerseNumbers.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.Controls.Add(this.m_rdoDisplayReferenceForOutOfOrderQuestions);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(this.lblDetailQuestionsOutOfOrder);
			this.panel3.Controls.Add(this.m_rdoOutputPassageForOutOfOrderQuestions);
			this.panel3.Location = new System.Drawing.Point(256, 114);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(246, 76);
			this.panel3.TabIndex = 22;
			// 
			// m_rdoDisplayReferenceForOutOfOrderQuestions
			// 
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.AutoSize = true;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Checked = true;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoDisplayReferenceForOutOfOrderQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoDisplayReferenceForOutOfOrderQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoDisplayReferenceForOutOfOrderQuestions, "GenerateScriptDlg.m_rdoDisplayReferenceForOutOfOrderQuestions");
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Location = new System.Drawing.Point(3, 29);
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Name = "m_rdoDisplayReferenceForOutOfOrderQuestions";
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Size = new System.Drawing.Size(152, 17);
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.TabIndex = 18;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.TabStop = true;
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Text = "&Just Display the Reference";
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label8, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label8, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.label8, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.label8, "GenerateScriptDlg.label8");
			this.label8.Location = new System.Drawing.Point(0, 23);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(246, 2);
			this.label8.TabIndex = 20;
			// 
			// m_rdoOutputPassageForOutOfOrderQuestions
			// 
			this.m_rdoOutputPassageForOutOfOrderQuestions.AutoSize = true;
			this.m_rdoOutputPassageForOutOfOrderQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoOutputPassageForOutOfOrderQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoOutputPassageForOutOfOrderQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoOutputPassageForOutOfOrderQuestions, "GenerateScriptDlg.m_rdoOutputPassageForOutOfOrderQuestions");
			this.m_rdoOutputPassageForOutOfOrderQuestions.Location = new System.Drawing.Point(3, 52);
			this.m_rdoOutputPassageForOutOfOrderQuestions.Name = "m_rdoOutputPassageForOutOfOrderQuestions";
			this.m_rdoOutputPassageForOutOfOrderQuestions.Size = new System.Drawing.Size(173, 17);
			this.m_rdoOutputPassageForOutOfOrderQuestions.TabIndex = 19;
			this.m_rdoOutputPassageForOutOfOrderQuestions.Text = "&Output the Text of the Passage";
			this.m_rdoOutputPassageForOutOfOrderQuestions.UseVisualStyleBackColor = true;
			// 
			// m_cboUseLWC
			// 
			this.m_cboUseLWC.FormattingEnabled = true;
			this.m_cboUseLWC.Items.AddRange(new object[] {
            "American English",
            "None"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboUseLWC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboUseLWC, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboUseLWC, "GenerateScriptDlg.m_cboUseLWC");
			this.m_cboUseLWC.Location = new System.Drawing.Point(9, 100);
			this.m_cboUseLWC.Name = "m_cboUseLWC";
			this.m_cboUseLWC.Size = new System.Drawing.Size(195, 21);
			this.m_cboUseLWC.TabIndex = 6;
			this.m_cboUseLWC.SelectedIndexChanged += new System.EventHandler(this.HandleLWCSelectedIndexChanged);
			// 
			// lblUseLWC
			// 
			this.lblUseLWC.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblUseLWC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblUseLWC, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.lblUseLWC, "GenerateScriptDlg.lblUseLWC");
			this.lblUseLWC.Location = new System.Drawing.Point(6, 84);
			this.lblUseLWC.Name = "lblUseLWC";
			this.lblUseLWC.Size = new System.Drawing.Size(198, 13);
			this.lblUseLWC.TabIndex = 5;
			this.lblUseLWC.Text = "Use &Language of Wider Communication:";
			// 
			// tabAppearance
			// 
			this.tabAppearance.Controls.Add(this.panel2);
			this.tabAppearance.Controls.Add(this.m_pnlCssOptions);
			this.tabAppearance.Controls.Add(this.lblStyleSpecification);
			this.tabAppearance.Controls.Add(this.m_rdoEmbedStyleInfo);
			this.tabAppearance.Controls.Add(this.m_rdoUseExternalCss);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabAppearance, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabAppearance, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabAppearance, "GenerateScriptDlg.tabAppearance");
			this.tabAppearance.Location = new System.Drawing.Point(4, 22);
			this.tabAppearance.Name = "tabAppearance";
			this.tabAppearance.Size = new System.Drawing.Size(508, 206);
			this.tabAppearance.TabIndex = 2;
			this.tabAppearance.Text = "Appearance";
			this.tabAppearance.UseVisualStyleBackColor = true;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// GenerateScriptDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(536, 292);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "GenerateScriptDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 594);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(552, 331);
			this.Name = "GenerateScriptDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generate Checking Script Template";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HandleHelpButtonClick);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.HandleHelpRequest);
			this.groupBoxDocument.ResumeLayout(false);
			this.groupBoxDocument.PerformLayout();
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
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox m_grpRange;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button m_btnChooseLWCQuestionColor;
		private System.Windows.Forms.ColorDialog colorDlg;
		private System.Windows.Forms.Button m_btnChooseCommentColor;
		private System.Windows.Forms.Button m_btnChooseLWCAnswerColor;
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
		internal System.Windows.Forms.Label m_lblLWCQuestionColor;
		internal System.Windows.Forms.Label m_lblCommentTextColor;
		internal System.Windows.Forms.Label m_lblLWCAnswerTextColor;
		internal System.Windows.Forms.Label m_lblFolder;
		private System.Windows.Forms.ComboBox m_cboBooks;
		internal System.Windows.Forms.Label m_lblQuestionGroupHeadingsColor;
		private System.Windows.Forms.Button btnChooseQuestionGroupHeadingsColor;
		internal System.Windows.Forms.RadioButton m_rdoUseOriginal;
		internal System.Windows.Forms.RadioButton m_rdoDisplayWarning;
		private System.Windows.Forms.Label label6;
		internal System.Windows.Forms.NumericUpDown m_numBlankLines;
		private System.Windows.Forms.Label lblExtraAnswerLines;
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
		private System.Windows.Forms.Label lblUseLWC;
		private System.Windows.Forms.TabPage tabAppearance;
		internal System.Windows.Forms.RadioButton m_rdoSkipUntranslated;
		internal System.Windows.Forms.CheckBox m_chkIncludeVerseNumbers;
		private System.Windows.Forms.Panel panel3;
		internal System.Windows.Forms.RadioButton m_rdoDisplayReferenceForOutOfOrderQuestions;
		private System.Windows.Forms.Label label8;
		internal System.Windows.Forms.RadioButton m_rdoOutputPassageForOutOfOrderQuestions;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label lblSectionRangeTo;
		private System.Windows.Forms.GroupBox groupBoxDocument;
		private System.Windows.Forms.Label lblFolder;
		private System.Windows.Forms.Label lblUntranslatedQuestions;
		private System.Windows.Forms.Label lblStyleSpecification;
		private System.Windows.Forms.Label lblDetailQuestionsOutOfOrder;
	}
}