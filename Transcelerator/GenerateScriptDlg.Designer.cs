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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.m_lblFolder = new System.Windows.Forms.Label();
			this.m_txtTitle = new System.Windows.Forms.TextBox();
			this.m_txtFilename = new System.Windows.Forms.TextBox();
			this.lblFolder = new System.Windows.Forms.Label();
			this.m_lbFilename = new System.Windows.Forms.Label();
			this.lblUntranslatedQuestions = new System.Windows.Forms.Label();
			this.lblStyleSpecification = new System.Windows.Forms.Label();
			this.lblDetailQuestionsOutOfOrder = new System.Windows.Forms.Label();
			this.m_rdoWholeBook = new System.Windows.Forms.RadioButton();
			this.m_grpRange = new System.Windows.Forms.GroupBox();
			this.m_tableLayoutPanelRange = new System.Windows.Forms.TableLayoutPanel();
			this.m_cboBooks = new System.Windows.Forms.ComboBox();
			this.m_cboEndSection = new System.Windows.Forms.ComboBox();
			this.m_rdoSingleSection = new System.Windows.Forms.RadioButton();
			this.m_cboStartSection = new System.Windows.Forms.ComboBox();
			this.m_cboSection = new System.Windows.Forms.ComboBox();
			this.m_rdoSectionRange = new System.Windows.Forms.RadioButton();
			this.m_chkShowUnavailable = new System.Windows.Forms.CheckBox();
			this.m_chkPassageBeforeOverview = new System.Windows.Forms.CheckBox();
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
			this.m_chkNumberQuestions = new System.Windows.Forms.CheckBox();
			this.lblExtraAnswerLines = new System.Windows.Forms.Label();
			this.m_chkAbsoluteCssPath = new System.Windows.Forms.CheckBox();
			this.m_chkOverwriteCss = new System.Windows.Forms.CheckBox();
			this.m_txtCssFile = new System.Windows.Forms.TextBox();
			this.btnBrowseCss = new System.Windows.Forms.Button();
			this.m_rdoUseExternalCss = new System.Windows.Forms.RadioButton();
			this.m_rdoEmbedStyleInfo = new System.Windows.Forms.RadioButton();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.tabOptions = new System.Windows.Forms.TabPage();
			this.m_tableLayoutPanelOptions = new System.Windows.Forms.TableLayoutPanel();
			this.m_tableLayoutPanelIncludeOptions = new System.Windows.Forms.TableLayoutPanel();
			this.m_chkIncludeScriptureForQuestions = new System.Windows.Forms.CheckBox();
			this.lblUseLWC = new System.Windows.Forms.Label();
			this.m_chkIncludeVerseNumbers = new System.Windows.Forms.CheckBox();
			this.m_cboUseLWC = new System.Windows.Forms.ComboBox();
			this.m_tableLayoutPanelUntranslatedQuestions = new System.Windows.Forms.TableLayoutPanel();
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder = new System.Windows.Forms.TableLayoutPanel();
			this.m_rdoOutputPassageForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.m_rdoDisplayReferenceForOutOfOrderQuestions = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.tabAppearance = new System.Windows.Forms.TabPage();
			this.m_tableLayoutPanelAppearance = new System.Windows.Forms.TableLayoutPanel();
			this.m_tableLayoutPanelStyles = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.m_tableLayoutPanelCssOptions = new System.Windows.Forms.TableLayoutPanel();
			this.m_tableLayoutPanelDisplayOptions = new System.Windows.Forms.TableLayoutPanel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.groupBoxDocument.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.m_grpRange.SuspendLayout();
			this.m_tableLayoutPanelRange.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numBlankLines)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabOptions.SuspendLayout();
			this.m_tableLayoutPanelOptions.SuspendLayout();
			this.m_tableLayoutPanelIncludeOptions.SuspendLayout();
			this.m_tableLayoutPanelUntranslatedQuestions.SuspendLayout();
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.SuspendLayout();
			this.tabAppearance.SuspendLayout();
			this.m_tableLayoutPanelAppearance.SuspendLayout();
			this.m_tableLayoutPanelStyles.SuspendLayout();
			this.m_tableLayoutPanelCssOptions.SuspendLayout();
			this.m_tableLayoutPanelDisplayOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblSectionRangeTo
			// 
			this.lblSectionRangeTo.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblSectionRangeTo.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSectionRangeTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSectionRangeTo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSectionRangeTo, "GenerateScriptDlg.lblSectionRangeTo");
			this.lblSectionRangeTo.Location = new System.Drawing.Point(295, 61);
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
			this.groupBoxDocument.Controls.Add(this.tableLayoutPanel1);
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
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnBrowse, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.m_lblFolder, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.m_txtTitle, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.m_txtFilename, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.lblFolder, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.m_lbFilename, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(497, 80);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblTitle.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblTitle, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.lblTitle, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.lblTitle, "GenerateScriptDlg.lblTitle");
			this.lblTitle.Location = new System.Drawing.Point(3, 6);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(30, 13);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Title:";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnBrowse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnBrowse, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.btnBrowse, "GenerateScriptDlg.btnBrowse");
			this.btnBrowse.Location = new System.Drawing.Point(419, 29);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 4;
			this.btnBrowse.Text = "&Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// m_lblFolder
			// 
			this.m_lblFolder.AutoSize = true;
			this.m_lblFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblFolder, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblFolder, "GenerateScriptDlg.m_lblFolder");
			this.m_lblFolder.Location = new System.Drawing.Point(94, 58);
			this.m_lblFolder.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.m_lblFolder.Name = "m_lblFolder";
			this.m_lblFolder.Size = new System.Drawing.Size(14, 13);
			this.m_lblFolder.TabIndex = 6;
			this.m_lblFolder.Text = "#";
			// 
			// m_txtTitle
			// 
			this.m_txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtTitle, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtTitle, "Param is a Scripture book or reference range");
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtTitle, "GenerateScriptDlg.m_txtTitle");
			this.m_txtTitle.Location = new System.Drawing.Point(94, 3);
			this.m_txtTitle.Name = "m_txtTitle";
			this.m_txtTitle.Size = new System.Drawing.Size(319, 20);
			this.m_txtTitle.TabIndex = 1;
			this.m_txtTitle.Text = "Comprehension Checking Questions for {0}";
			// 
			// m_txtFilename
			// 
			this.m_txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtFilename, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtFilename, "Param 0: project name; Param 1: Scripture book or reference range");
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtFilename, "GenerateScriptDlg.m_txtFilename");
			this.m_txtFilename.Location = new System.Drawing.Point(94, 31);
			this.m_txtFilename.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
			this.m_txtFilename.Name = "m_txtFilename";
			this.m_txtFilename.Size = new System.Drawing.Size(319, 20);
			this.m_txtFilename.TabIndex = 3;
			this.m_txtFilename.Text = "{0} Comprehension Check for {1}.htm";
			// 
			// lblFolder
			// 
			this.lblFolder.AutoSize = true;
			this.lblFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblFolder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblFolder, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblFolder, "GenerateScriptDlg.lblFolder");
			this.lblFolder.Location = new System.Drawing.Point(3, 58);
			this.lblFolder.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.lblFolder.Name = "lblFolder";
			this.lblFolder.Size = new System.Drawing.Size(39, 13);
			this.lblFolder.TabIndex = 5;
			this.lblFolder.Text = "Folder:";
			// 
			// m_lbFilename
			// 
			this.m_lbFilename.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.m_lbFilename.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lbFilename, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lbFilename, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lbFilename, "GenerateScriptDlg.m_lbFilename");
			this.m_lbFilename.Location = new System.Drawing.Point(3, 34);
			this.m_lbFilename.Name = "m_lbFilename";
			this.m_lbFilename.Size = new System.Drawing.Size(85, 13);
			this.m_lbFilename.TabIndex = 2;
			this.m_lbFilename.Text = "Script &File name:";
			// 
			// lblUntranslatedQuestions
			// 
			this.lblUntranslatedQuestions.AutoEllipsis = true;
			this.lblUntranslatedQuestions.AutoSize = true;
			this.lblUntranslatedQuestions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUntranslatedQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblUntranslatedQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblUntranslatedQuestions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblUntranslatedQuestions, "GenerateScriptDlg.lblUntranslatedQuestions");
			this.lblUntranslatedQuestions.Location = new System.Drawing.Point(3, 0);
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
			this.lblStyleSpecification.Location = new System.Drawing.Point(3, 3);
			this.lblStyleSpecification.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.lblStyleSpecification.Name = "lblStyleSpecification";
			this.lblStyleSpecification.Size = new System.Drawing.Size(123, 13);
			this.lblStyleSpecification.TabIndex = 0;
			this.lblStyleSpecification.Text = "Where to Specify Styles:";
			// 
			// lblDetailQuestionsOutOfOrder
			// 
			this.lblDetailQuestionsOutOfOrder.AutoEllipsis = true;
			this.lblDetailQuestionsOutOfOrder.AutoSize = true;
			this.lblDetailQuestionsOutOfOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDetailQuestionsOutOfOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblDetailQuestionsOutOfOrder, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblDetailQuestionsOutOfOrder, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblDetailQuestionsOutOfOrder, "GenerateScriptDlg.lblDetailQuestionsOutOfOrder");
			this.lblDetailQuestionsOutOfOrder.Location = new System.Drawing.Point(3, 0);
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
			this.m_rdoWholeBook.Location = new System.Drawing.Point(3, 3);
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
			this.m_grpRange.Controls.Add(this.m_tableLayoutPanelRange);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_grpRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_grpRange, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_grpRange, "GenerateScriptDlg.m_grpRange");
			this.m_grpRange.Location = new System.Drawing.Point(6, 6);
			this.m_grpRange.Name = "m_grpRange";
			this.m_grpRange.Padding = new System.Windows.Forms.Padding(6, 3, 10, 1);
			this.m_grpRange.Size = new System.Drawing.Size(503, 97);
			this.m_grpRange.TabIndex = 2;
			this.m_grpRange.TabStop = false;
			this.m_grpRange.Text = "Range";
			// 
			// m_tableLayoutPanelRange
			// 
			this.m_tableLayoutPanelRange.ColumnCount = 4;
			this.m_tableLayoutPanelRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.m_tableLayoutPanelRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.m_tableLayoutPanelRange.Controls.Add(this.m_cboBooks, 1, 0);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_cboEndSection, 3, 2);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_rdoWholeBook, 0, 0);
			this.m_tableLayoutPanelRange.Controls.Add(this.lblSectionRangeTo, 2, 2);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_rdoSingleSection, 0, 1);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_cboStartSection, 1, 2);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_cboSection, 1, 1);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_rdoSectionRange, 0, 2);
			this.m_tableLayoutPanelRange.Controls.Add(this.m_chkShowUnavailable, 3, 0);
			this.m_tableLayoutPanelRange.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelRange.Location = new System.Drawing.Point(6, 16);
			this.m_tableLayoutPanelRange.Margin = new System.Windows.Forms.Padding(0);
			this.m_tableLayoutPanelRange.Name = "m_tableLayoutPanelRange";
			this.m_tableLayoutPanelRange.RowCount = 3;
			this.m_tableLayoutPanelRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelRange.Size = new System.Drawing.Size(487, 80);
			this.m_tableLayoutPanelRange.TabIndex = 7;
			// 
			// m_cboBooks
			// 
			this.m_cboBooks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboBooks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboBooks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_cboBooks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboBooks.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboBooks, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboBooks, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboBooks, "GenerateScriptDlg.m_cboBooks");
			this.m_cboBooks.Location = new System.Drawing.Point(122, 3);
			this.m_cboBooks.Name = "m_cboBooks";
			this.m_cboBooks.Size = new System.Drawing.Size(167, 21);
			this.m_cboBooks.TabIndex = 1;
			this.m_cboBooks.SelectedIndexChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSelectedBook);
			this.m_cboBooks.Enter += new System.EventHandler(this.m_cboBooks_Enter);
			// 
			// m_cboEndSection
			// 
			this.m_cboEndSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboEndSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboEndSection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_cboEndSection.DropDownHeight = 156;
			this.m_cboEndSection.DropDownWidth = 250;
			this.m_cboEndSection.FormattingEnabled = true;
			this.m_cboEndSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboEndSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboEndSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboEndSection, "GenerateScriptDlg.m_cboEndSection");
			this.m_cboEndSection.Location = new System.Drawing.Point(317, 57);
			this.m_cboEndSection.MaxDropDownItems = 12;
			this.m_cboEndSection.Name = "m_cboEndSection";
			this.m_cboEndSection.Size = new System.Drawing.Size(167, 21);
			this.m_cboEndSection.TabIndex = 7;
			this.m_cboEndSection.SelectedIndexChanged += new System.EventHandler(this.m_cboEndSection_SelectedIndexChanged);
			this.m_cboEndSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboEndSection.Enter += new System.EventHandler(this.SectionRangeCombo_Enter);
			// 
			// m_rdoSingleSection
			// 
			this.m_rdoSingleSection.AutoSize = true;
			this.m_rdoSingleSection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSingleSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSingleSection, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSingleSection, "GenerateScriptDlg.m_rdoSingleSection");
			this.m_rdoSingleSection.Location = new System.Drawing.Point(3, 30);
			this.m_rdoSingleSection.Name = "m_rdoSingleSection";
			this.m_rdoSingleSection.Size = new System.Drawing.Size(93, 17);
			this.m_rdoSingleSection.TabIndex = 2;
			this.m_rdoSingleSection.Text = "&Single Section";
			this.m_rdoSingleSection.UseVisualStyleBackColor = true;
			this.m_rdoSingleSection.CheckedChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			// 
			// m_cboStartSection
			// 
			this.m_cboStartSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboStartSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboStartSection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_cboStartSection.DropDownHeight = 156;
			this.m_cboStartSection.DropDownWidth = 250;
			this.m_cboStartSection.FormattingEnabled = true;
			this.m_cboStartSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboStartSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboStartSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboStartSection, "GenerateScriptDlg.m_cboStartSection");
			this.m_cboStartSection.Location = new System.Drawing.Point(122, 57);
			this.m_cboStartSection.MaxDropDownItems = 12;
			this.m_cboStartSection.Name = "m_cboStartSection";
			this.m_cboStartSection.Size = new System.Drawing.Size(167, 21);
			this.m_cboStartSection.TabIndex = 5;
			this.m_cboStartSection.SelectedIndexChanged += new System.EventHandler(this.m_cboStartSection_SelectedIndexChanged);
			this.m_cboStartSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboStartSection.Enter += new System.EventHandler(this.SectionRangeCombo_Enter);
			// 
			// m_cboSection
			// 
			this.m_cboSection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboSection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboSection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_cboSection.DropDownHeight = 156;
			this.m_cboSection.DropDownWidth = 250;
			this.m_cboSection.FormattingEnabled = true;
			this.m_cboSection.IntegralHeight = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboSection, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboSection, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboSection, "GenerateScriptDlg.m_cboSection");
			this.m_cboSection.Location = new System.Drawing.Point(122, 30);
			this.m_cboSection.MaxDropDownItems = 12;
			this.m_cboSection.Name = "m_cboSection";
			this.m_cboSection.Size = new System.Drawing.Size(167, 21);
			this.m_cboSection.TabIndex = 3;
			this.m_cboSection.SelectedIndexChanged += new System.EventHandler(this.UpdateTitleAndFilenameForSingleSection);
			this.m_cboSection.TextUpdate += new System.EventHandler(this.ComboTextUpdate);
			this.m_cboSection.Enter += new System.EventHandler(this.m_cboSection_Enter);
			// 
			// m_rdoSectionRange
			// 
			this.m_rdoSectionRange.AutoSize = true;
			this.m_rdoSectionRange.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSectionRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSectionRange, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSectionRange, "GenerateScriptDlg.m_rdoSectionRange");
			this.m_rdoSectionRange.Location = new System.Drawing.Point(3, 57);
			this.m_rdoSectionRange.Name = "m_rdoSectionRange";
			this.m_rdoSectionRange.Size = new System.Drawing.Size(113, 17);
			this.m_rdoSectionRange.TabIndex = 4;
			this.m_rdoSectionRange.Text = "&Range of Sections";
			this.m_rdoSectionRange.UseVisualStyleBackColor = true;
			this.m_rdoSectionRange.CheckedChanged += new System.EventHandler(this.m_rdoSectionRange_CheckedChanged);
			// 
			// m_chkShowUnavailable
			// 
			this.m_chkShowUnavailable.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkShowUnavailable, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkShowUnavailable, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_chkShowUnavailable, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkShowUnavailable, "GenerateScriptDlg.m_chkShowUnavailable");
			this.m_chkShowUnavailable.Location = new System.Drawing.Point(317, 3);
			this.m_chkShowUnavailable.Name = "m_chkShowUnavailable";
			this.m_chkShowUnavailable.Size = new System.Drawing.Size(142, 17);
			this.m_chkShowUnavailable.TabIndex = 8;
			this.m_chkShowUnavailable.Text = "Show unavailable books";
			this.m_chkShowUnavailable.UseVisualStyleBackColor = true;
			this.m_chkShowUnavailable.CheckedChanged += new System.EventHandler(this.m_chkShowUnavailable_CheckedChanged);
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
			this.m_chkPassageBeforeOverview.Location = new System.Drawing.Point(3, 3);
			this.m_chkPassageBeforeOverview.Name = "m_chkPassageBeforeOverview";
			this.m_chkPassageBeforeOverview.Size = new System.Drawing.Size(241, 17);
			this.m_chkPassageBeforeOverview.TabIndex = 0;
			this.m_chkPassageBeforeOverview.Text = "&Include Entire Passage at the Start of Section";
			this.m_chkPassageBeforeOverview.UseVisualStyleBackColor = true;
			// 
			// m_rdoSkipUntranslated
			// 
			this.m_rdoSkipUntranslated.AutoSize = true;
			this.m_rdoSkipUntranslated.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSkipUntranslated, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSkipUntranslated, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSkipUntranslated, "GenerateScriptDlg.m_rdoSkipUntranslated");
			this.m_rdoSkipUntranslated.Location = new System.Drawing.Point(3, 69);
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
			this.m_rdoDisplayWarning.Location = new System.Drawing.Point(3, 23);
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
			this.label6.Location = new System.Drawing.Point(3, 15);
			this.label6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(236, 2);
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
			this.m_rdoUseOriginal.Location = new System.Drawing.Point(3, 46);
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
			this.m_chkIncludeLWCComments.Location = new System.Drawing.Point(3, 170);
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
			this.m_chkIncludeLWCAnswers.Location = new System.Drawing.Point(3, 147);
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
			this.m_chkIncludeLWCQuestions.Location = new System.Drawing.Point(3, 124);
			this.m_chkIncludeLWCQuestions.Name = "m_chkIncludeLWCQuestions";
			this.m_chkIncludeLWCQuestions.Size = new System.Drawing.Size(139, 17);
			this.m_chkIncludeLWCQuestions.TabIndex = 1;
			this.m_chkIncludeLWCQuestions.Text = "Include &Questions in {0}";
			this.m_chkIncludeLWCQuestions.UseVisualStyleBackColor = true;
			this.m_chkIncludeLWCQuestions.CheckedChanged += new System.EventHandler(this.IncludeOptionCheckedChanged);
			// 
			// m_lblQuestionGroupHeadingsColor
			// 
			this.m_lblQuestionGroupHeadingsColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblQuestionGroupHeadingsColor.AutoEllipsis = true;
			this.m_lblQuestionGroupHeadingsColor.BackColor = System.Drawing.Color.Transparent;
			this.m_lblQuestionGroupHeadingsColor.ForeColor = System.Drawing.Color.Blue;
			this.m_lblQuestionGroupHeadingsColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblQuestionGroupHeadingsColor, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblQuestionGroupHeadingsColor, "GenerateScriptDlg.m_lblQuestionGroupHeadingsColor");
			this.m_lblQuestionGroupHeadingsColor.Location = new System.Drawing.Point(3, 33);
			this.m_lblQuestionGroupHeadingsColor.Name = "m_lblQuestionGroupHeadingsColor";
			this.m_lblQuestionGroupHeadingsColor.Size = new System.Drawing.Size(187, 13);
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
			this.btnChooseQuestionGroupHeadingsColor.Location = new System.Drawing.Point(196, 28);
			this.btnChooseQuestionGroupHeadingsColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnChooseQuestionGroupHeadingsColor.Name = "btnChooseQuestionGroupHeadingsColor";
			this.btnChooseQuestionGroupHeadingsColor.Size = new System.Drawing.Size(25, 23);
			this.btnChooseQuestionGroupHeadingsColor.TabIndex = 3;
			this.btnChooseQuestionGroupHeadingsColor.Text = "...";
			this.btnChooseQuestionGroupHeadingsColor.UseVisualStyleBackColor = true;
			this.btnChooseQuestionGroupHeadingsColor.Click += new System.EventHandler(this.ChooseTextColor);
			// 
			// m_lblCommentTextColor
			// 
			this.m_lblCommentTextColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblCommentTextColor.AutoEllipsis = true;
			this.m_lblCommentTextColor.ForeColor = System.Drawing.Color.Red;
			this.m_lblCommentTextColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCommentTextColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCommentTextColor, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblCommentTextColor, "GenerateScriptDlg.m_lblCommentTextColor");
			this.m_lblCommentTextColor.Location = new System.Drawing.Point(3, 114);
			this.m_lblCommentTextColor.Name = "m_lblCommentTextColor";
			this.m_lblCommentTextColor.Size = new System.Drawing.Size(187, 13);
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
			this.m_btnChooseCommentColor.Location = new System.Drawing.Point(196, 109);
			this.m_btnChooseCommentColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
			this.m_lblLWCAnswerTextColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblLWCAnswerTextColor.AutoEllipsis = true;
			this.m_lblLWCAnswerTextColor.ForeColor = System.Drawing.Color.Green;
			this.m_lblLWCAnswerTextColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblLWCAnswerTextColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblLWCAnswerTextColor, "Param is the name of a language of wider communication");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblLWCAnswerTextColor, "GenerateScriptDlg.m_lblLWCAnswerTextColor");
			this.m_lblLWCAnswerTextColor.Location = new System.Drawing.Point(3, 87);
			this.m_lblLWCAnswerTextColor.Name = "m_lblLWCAnswerTextColor";
			this.m_lblLWCAnswerTextColor.Size = new System.Drawing.Size(187, 13);
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
			this.m_btnChooseLWCAnswerColor.Location = new System.Drawing.Point(196, 82);
			this.m_btnChooseLWCAnswerColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
			this.m_lblLWCQuestionColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblLWCQuestionColor.AutoEllipsis = true;
			this.m_lblLWCQuestionColor.ForeColor = System.Drawing.Color.Gray;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblLWCQuestionColor, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblLWCQuestionColor, "Param is the name of a language of wider communication");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblLWCQuestionColor, "GenerateScriptDlg.m_lblLWCQuestionColor");
			this.m_lblLWCQuestionColor.Location = new System.Drawing.Point(3, 60);
			this.m_lblLWCQuestionColor.Name = "m_lblLWCQuestionColor";
			this.m_lblLWCQuestionColor.Size = new System.Drawing.Size(187, 13);
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
			this.m_btnChooseLWCQuestionColor.Location = new System.Drawing.Point(196, 55);
			this.m_btnChooseLWCQuestionColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
			this.m_numBlankLines.Location = new System.Drawing.Point(196, 3);
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
			// m_chkNumberQuestions
			// 
			this.m_chkNumberQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_chkNumberQuestions.AutoEllipsis = true;
			this.m_chkNumberQuestions.BackColor = System.Drawing.Color.Transparent;
			this.m_chkNumberQuestions.Checked = true;
			this.m_chkNumberQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_tableLayoutPanelDisplayOptions.SetColumnSpan(this.m_chkNumberQuestions, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkNumberQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkNumberQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkNumberQuestions, "GenerateScriptDlg.m_chkNumberQuestions");
			this.m_chkNumberQuestions.Location = new System.Drawing.Point(3, 142);
			this.m_chkNumberQuestions.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this.m_chkNumberQuestions.Name = "m_chkNumberQuestions";
			this.m_chkNumberQuestions.Size = new System.Drawing.Size(237, 17);
			this.m_chkNumberQuestions.TabIndex = 10;
			this.m_chkNumberQuestions.Text = "&Number questions";
			this.m_chkNumberQuestions.UseVisualStyleBackColor = false;
			// 
			// lblExtraAnswerLines
			// 
			this.lblExtraAnswerLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblExtraAnswerLines.AutoEllipsis = true;
			this.lblExtraAnswerLines.BackColor = System.Drawing.Color.Transparent;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblExtraAnswerLines, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblExtraAnswerLines, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.lblExtraAnswerLines, "GenerateScriptDlg.label7");
			this.lblExtraAnswerLines.Location = new System.Drawing.Point(3, 6);
			this.lblExtraAnswerLines.Name = "lblExtraAnswerLines";
			this.lblExtraAnswerLines.Size = new System.Drawing.Size(187, 13);
			this.lblExtraAnswerLines.TabIndex = 0;
			this.lblExtraAnswerLines.Text = "Extra &Lines for Recording Answers:";
			// 
			// m_chkAbsoluteCssPath
			// 
			this.m_chkAbsoluteCssPath.AutoSize = true;
			this.m_chkAbsoluteCssPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkAbsoluteCssPath, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkAbsoluteCssPath, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkAbsoluteCssPath, "GenerateScriptDlg.m_chkAbsoluteCssPath");
			this.m_chkAbsoluteCssPath.Location = new System.Drawing.Point(3, 32);
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
			this.m_chkOverwriteCss.Location = new System.Drawing.Point(3, 55);
			this.m_chkOverwriteCss.Name = "m_chkOverwriteCss";
			this.m_chkOverwriteCss.Size = new System.Drawing.Size(153, 17);
			this.m_chkOverwriteCss.TabIndex = 3;
			this.m_chkOverwriteCss.Text = "O&verwrite Existing CSS File";
			this.m_chkOverwriteCss.UseVisualStyleBackColor = true;
			// 
			// m_txtCssFile
			// 
			this.m_txtCssFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtCssFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtCssFile, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtCssFile, "GenerateScriptDlg.m_txtCssFile");
			this.m_txtCssFile.Location = new System.Drawing.Point(3, 4);
			this.m_txtCssFile.Name = "m_txtCssFile";
			this.m_txtCssFile.Size = new System.Drawing.Size(184, 20);
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
			this.btnBrowseCss.Location = new System.Drawing.Point(193, 3);
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
			this.m_rdoUseExternalCss.Location = new System.Drawing.Point(3, 53);
			this.m_rdoUseExternalCss.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
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
			this.m_rdoEmbedStyleInfo.Location = new System.Drawing.Point(3, 28);
			this.m_rdoEmbedStyleInfo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
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
			this.tabOptions.Controls.Add(this.m_tableLayoutPanelOptions);
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
			// m_tableLayoutPanelOptions
			// 
			this.m_tableLayoutPanelOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanelOptions.ColumnCount = 2;
			this.m_tableLayoutPanelOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelOptions.Controls.Add(this.m_tableLayoutPanelIncludeOptions, 0, 0);
			this.m_tableLayoutPanelOptions.Controls.Add(this.m_tableLayoutPanelUntranslatedQuestions, 1, 0);
			this.m_tableLayoutPanelOptions.Controls.Add(this.m_tableLayoutPanelDetailQuestionsOutOfOrder, 1, 1);
			this.m_tableLayoutPanelOptions.Location = new System.Drawing.Point(10, 8);
			this.m_tableLayoutPanelOptions.Name = "m_tableLayoutPanelOptions";
			this.m_tableLayoutPanelOptions.RowCount = 2;
			this.m_tableLayoutPanelOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelOptions.Size = new System.Drawing.Size(495, 192);
			this.m_tableLayoutPanelOptions.TabIndex = 7;
			// 
			// m_tableLayoutPanelIncludeOptions
			// 
			this.m_tableLayoutPanelIncludeOptions.AutoSize = true;
			this.m_tableLayoutPanelIncludeOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_tableLayoutPanelIncludeOptions.ColumnCount = 1;
			this.m_tableLayoutPanelIncludeOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkIncludeScriptureForQuestions, 0, 2);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.lblUseLWC, 0, 3);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkPassageBeforeOverview, 0, 0);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkIncludeLWCQuestions, 0, 5);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkIncludeVerseNumbers, 0, 1);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkIncludeLWCComments, 0, 7);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_chkIncludeLWCAnswers, 0, 6);
			this.m_tableLayoutPanelIncludeOptions.Controls.Add(this.m_cboUseLWC, 0, 4);
			this.m_tableLayoutPanelIncludeOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelIncludeOptions.Location = new System.Drawing.Point(0, 0);
			this.m_tableLayoutPanelIncludeOptions.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
			this.m_tableLayoutPanelIncludeOptions.Name = "m_tableLayoutPanelIncludeOptions";
			this.m_tableLayoutPanelIncludeOptions.RowCount = 8;
			this.m_tableLayoutPanelOptions.SetRowSpan(this.m_tableLayoutPanelIncludeOptions, 2);
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelIncludeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.m_tableLayoutPanelIncludeOptions.Size = new System.Drawing.Size(247, 192);
			this.m_tableLayoutPanelIncludeOptions.TabIndex = 0;
			// 
			// m_chkIncludeScriptureForQuestions
			// 
			this.m_chkIncludeScriptureForQuestions.AutoSize = true;
			this.m_chkIncludeScriptureForQuestions.Checked = true;
			this.m_chkIncludeScriptureForQuestions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeScriptureForQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeScriptureForQuestions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeScriptureForQuestions, "GenerateScriptDlg.m_chkIncludeScriptureForQuestions");
			this.m_chkIncludeScriptureForQuestions.Location = new System.Drawing.Point(3, 49);
			this.m_chkIncludeScriptureForQuestions.Name = "m_chkIncludeScriptureForQuestions";
			this.m_chkIncludeScriptureForQuestions.Size = new System.Drawing.Size(187, 17);
			this.m_chkIncludeScriptureForQuestions.TabIndex = 7;
			this.m_chkIncludeScriptureForQuestions.Text = "Include Scripture before questions";
			this.m_chkIncludeScriptureForQuestions.UseVisualStyleBackColor = true;
			// 
			// lblUseLWC
			// 
			this.lblUseLWC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblUseLWC.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblUseLWC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblUseLWC, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.lblUseLWC, "GenerateScriptDlg.lblUseLWC");
			this.lblUseLWC.Location = new System.Drawing.Point(3, 81);
			this.lblUseLWC.Name = "lblUseLWC";
			this.lblUseLWC.Size = new System.Drawing.Size(198, 13);
			this.lblUseLWC.TabIndex = 5;
			this.lblUseLWC.Text = "Use &Language of Wider Communication:";
			// 
			// m_chkIncludeVerseNumbers
			// 
			this.m_chkIncludeVerseNumbers.AutoSize = true;
			this.m_chkIncludeVerseNumbers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkIncludeVerseNumbers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkIncludeVerseNumbers, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkIncludeVerseNumbers, "GenerateScriptDlg.m_chkIncludeVerseNumbers");
			this.m_chkIncludeVerseNumbers.Location = new System.Drawing.Point(3, 26);
			this.m_chkIncludeVerseNumbers.Name = "m_chkIncludeVerseNumbers";
			this.m_chkIncludeVerseNumbers.Size = new System.Drawing.Size(133, 17);
			this.m_chkIncludeVerseNumbers.TabIndex = 7;
			this.m_chkIncludeVerseNumbers.Text = "Include verse numbers";
			this.m_chkIncludeVerseNumbers.UseVisualStyleBackColor = true;
			// 
			// m_cboUseLWC
			// 
			this.m_cboUseLWC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_cboUseLWC.FormattingEnabled = true;
			this.m_cboUseLWC.Items.AddRange(new object[] {
            "American English",
            "None"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboUseLWC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboUseLWC, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboUseLWC, "GenerateScriptDlg.m_cboUseLWC");
			this.m_cboUseLWC.Location = new System.Drawing.Point(3, 97);
			this.m_cboUseLWC.Name = "m_cboUseLWC";
			this.m_cboUseLWC.Size = new System.Drawing.Size(241, 21);
			this.m_cboUseLWC.TabIndex = 6;
			this.m_cboUseLWC.SelectedIndexChanged += new System.EventHandler(this.HandleLWCSelectedIndexChanged);
			// 
			// m_tableLayoutPanelUntranslatedQuestions
			// 
			this.m_tableLayoutPanelUntranslatedQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanelUntranslatedQuestions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_tableLayoutPanelUntranslatedQuestions.ColumnCount = 1;
			this.m_tableLayoutPanelUntranslatedQuestions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelUntranslatedQuestions.Controls.Add(this.label6, 0, 1);
			this.m_tableLayoutPanelUntranslatedQuestions.Controls.Add(this.m_rdoSkipUntranslated, 0, 4);
			this.m_tableLayoutPanelUntranslatedQuestions.Controls.Add(this.lblUntranslatedQuestions, 0, 0);
			this.m_tableLayoutPanelUntranslatedQuestions.Controls.Add(this.m_rdoDisplayWarning, 0, 2);
			this.m_tableLayoutPanelUntranslatedQuestions.Controls.Add(this.m_rdoUseOriginal, 0, 3);
			this.m_tableLayoutPanelUntranslatedQuestions.Location = new System.Drawing.Point(253, 0);
			this.m_tableLayoutPanelUntranslatedQuestions.Margin = new System.Windows.Forms.Padding(0);
			this.m_tableLayoutPanelUntranslatedQuestions.Name = "m_tableLayoutPanelUntranslatedQuestions";
			this.m_tableLayoutPanelUntranslatedQuestions.RowCount = 5;
			this.m_tableLayoutPanelUntranslatedQuestions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelUntranslatedQuestions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelUntranslatedQuestions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelUntranslatedQuestions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelUntranslatedQuestions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelUntranslatedQuestions.Size = new System.Drawing.Size(242, 100);
			this.m_tableLayoutPanelUntranslatedQuestions.TabIndex = 1;
			// 
			// m_tableLayoutPanelDetailQuestionsOutOfOrder
			// 
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.AutoSize = true;
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.ColumnCount = 1;
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Controls.Add(this.m_rdoOutputPassageForOutOfOrderQuestions, 0, 3);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Controls.Add(this.m_rdoDisplayReferenceForOutOfOrderQuestions, 0, 2);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Controls.Add(this.lblDetailQuestionsOutOfOrder, 0, 0);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Controls.Add(this.label8, 0, 1);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Location = new System.Drawing.Point(253, 110);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Name = "m_tableLayoutPanelDetailQuestionsOutOfOrder";
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.RowCount = 4;
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.Size = new System.Drawing.Size(242, 82);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.TabIndex = 2;
			// 
			// m_rdoOutputPassageForOutOfOrderQuestions
			// 
			this.m_rdoOutputPassageForOutOfOrderQuestions.AutoSize = true;
			this.m_rdoOutputPassageForOutOfOrderQuestions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoOutputPassageForOutOfOrderQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoOutputPassageForOutOfOrderQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoOutputPassageForOutOfOrderQuestions, "GenerateScriptDlg.m_rdoOutputPassageForOutOfOrderQuestions");
			this.m_rdoOutputPassageForOutOfOrderQuestions.Location = new System.Drawing.Point(3, 46);
			this.m_rdoOutputPassageForOutOfOrderQuestions.Name = "m_rdoOutputPassageForOutOfOrderQuestions";
			this.m_rdoOutputPassageForOutOfOrderQuestions.Size = new System.Drawing.Size(173, 17);
			this.m_rdoOutputPassageForOutOfOrderQuestions.TabIndex = 19;
			this.m_rdoOutputPassageForOutOfOrderQuestions.Text = "&Output the Text of the Passage";
			this.m_rdoOutputPassageForOutOfOrderQuestions.UseVisualStyleBackColor = true;
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
			this.m_rdoDisplayReferenceForOutOfOrderQuestions.Location = new System.Drawing.Point(3, 23);
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
			this.label8.Location = new System.Drawing.Point(3, 15);
			this.label8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(236, 2);
			this.label8.TabIndex = 20;
			// 
			// tabAppearance
			// 
			this.tabAppearance.Controls.Add(this.m_tableLayoutPanelAppearance);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.tabAppearance, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.tabAppearance, null);
			this.l10NSharpExtender1.SetLocalizingId(this.tabAppearance, "GenerateScriptDlg.tabAppearance");
			this.tabAppearance.Location = new System.Drawing.Point(4, 22);
			this.tabAppearance.Name = "tabAppearance";
			this.tabAppearance.Padding = new System.Windows.Forms.Padding(3);
			this.tabAppearance.Size = new System.Drawing.Size(508, 206);
			this.tabAppearance.TabIndex = 2;
			this.tabAppearance.Text = "Appearance";
			this.tabAppearance.UseVisualStyleBackColor = true;
			// 
			// m_tableLayoutPanelAppearance
			// 
			this.m_tableLayoutPanelAppearance.BackColor = System.Drawing.Color.Transparent;
			this.m_tableLayoutPanelAppearance.ColumnCount = 2;
			this.m_tableLayoutPanelAppearance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelAppearance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelAppearance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.m_tableLayoutPanelAppearance.Controls.Add(this.m_tableLayoutPanelStyles, 0, 0);
			this.m_tableLayoutPanelAppearance.Controls.Add(this.m_tableLayoutPanelDisplayOptions, 1, 0);
			this.m_tableLayoutPanelAppearance.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelAppearance.Location = new System.Drawing.Point(3, 3);
			this.m_tableLayoutPanelAppearance.Name = "m_tableLayoutPanelAppearance";
			this.m_tableLayoutPanelAppearance.RowCount = 1;
			this.m_tableLayoutPanelAppearance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelAppearance.Size = new System.Drawing.Size(502, 200);
			this.m_tableLayoutPanelAppearance.TabIndex = 7;
			// 
			// m_tableLayoutPanelStyles
			// 
			this.m_tableLayoutPanelStyles.AutoSize = true;
			this.m_tableLayoutPanelStyles.BackColor = System.Drawing.Color.Transparent;
			this.m_tableLayoutPanelStyles.ColumnCount = 1;
			this.m_tableLayoutPanelStyles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelStyles.Controls.Add(this.label1, 0, 1);
			this.m_tableLayoutPanelStyles.Controls.Add(this.lblStyleSpecification, 0, 0);
			this.m_tableLayoutPanelStyles.Controls.Add(this.m_rdoEmbedStyleInfo, 0, 2);
			this.m_tableLayoutPanelStyles.Controls.Add(this.m_rdoUseExternalCss, 0, 3);
			this.m_tableLayoutPanelStyles.Controls.Add(this.m_tableLayoutPanelCssOptions, 0, 4);
			this.m_tableLayoutPanelStyles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelStyles.Location = new System.Drawing.Point(3, 6);
			this.m_tableLayoutPanelStyles.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.m_tableLayoutPanelStyles.Name = "m_tableLayoutPanelStyles";
			this.m_tableLayoutPanelStyles.RowCount = 5;
			this.m_tableLayoutPanelStyles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelStyles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelStyles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelStyles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelStyles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelStyles.Size = new System.Drawing.Size(242, 191);
			this.m_tableLayoutPanelStyles.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.label1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.label1, "GenerateScriptDlg.label6");
			this.label1.Location = new System.Drawing.Point(3, 18);
			this.label1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(236, 2);
			this.label1.TabIndex = 21;
			// 
			// m_tableLayoutPanelCssOptions
			// 
			this.m_tableLayoutPanelCssOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanelCssOptions.AutoSize = true;
			this.m_tableLayoutPanelCssOptions.ColumnCount = 2;
			this.m_tableLayoutPanelCssOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelCssOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelCssOptions.Controls.Add(this.m_chkOverwriteCss, 0, 2);
			this.m_tableLayoutPanelCssOptions.Controls.Add(this.m_chkAbsoluteCssPath, 0, 1);
			this.m_tableLayoutPanelCssOptions.Controls.Add(this.m_txtCssFile, 0, 0);
			this.m_tableLayoutPanelCssOptions.Controls.Add(this.btnBrowseCss, 1, 0);
			this.m_tableLayoutPanelCssOptions.Location = new System.Drawing.Point(18, 76);
			this.m_tableLayoutPanelCssOptions.Name = "m_tableLayoutPanelCssOptions";
			this.m_tableLayoutPanelCssOptions.RowCount = 3;
			this.m_tableLayoutPanelCssOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelCssOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelCssOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelCssOptions.Size = new System.Drawing.Size(221, 75);
			this.m_tableLayoutPanelCssOptions.TabIndex = 4;
			this.m_tableLayoutPanelCssOptions.Visible = false;
			// 
			// m_tableLayoutPanelDisplayOptions
			// 
			this.m_tableLayoutPanelDisplayOptions.AutoSize = true;
			this.m_tableLayoutPanelDisplayOptions.BackColor = System.Drawing.Color.Transparent;
			this.m_tableLayoutPanelDisplayOptions.ColumnCount = 2;
			this.m_tableLayoutPanelDisplayOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelDisplayOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelDisplayOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_btnChooseCommentColor, 1, 4);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.btnChooseQuestionGroupHeadingsColor, 1, 1);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_btnChooseLWCAnswerColor, 1, 3);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_chkNumberQuestions, 0, 5);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_btnChooseLWCQuestionColor, 1, 2);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.lblExtraAnswerLines, 0, 0);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_lblQuestionGroupHeadingsColor, 0, 1);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_numBlankLines, 1, 0);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_lblCommentTextColor, 0, 4);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_lblLWCQuestionColor, 0, 2);
			this.m_tableLayoutPanelDisplayOptions.Controls.Add(this.m_lblLWCAnswerTextColor, 0, 3);
			this.m_tableLayoutPanelDisplayOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_tableLayoutPanelDisplayOptions.Location = new System.Drawing.Point(256, 3);
			this.m_tableLayoutPanelDisplayOptions.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
			this.m_tableLayoutPanelDisplayOptions.Name = "m_tableLayoutPanelDisplayOptions";
			this.m_tableLayoutPanelDisplayOptions.RowCount = 6;
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelDisplayOptions.Size = new System.Drawing.Size(243, 194);
			this.m_tableLayoutPanelDisplayOptions.TabIndex = 1;
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
			this.Text = "Generate Checking Script";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HandleHelpButtonClick);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.HandleHelpRequest);
			this.groupBoxDocument.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.m_grpRange.ResumeLayout(false);
			this.m_tableLayoutPanelRange.ResumeLayout(false);
			this.m_tableLayoutPanelRange.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numBlankLines)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabOptions.ResumeLayout(false);
			this.m_tableLayoutPanelOptions.ResumeLayout(false);
			this.m_tableLayoutPanelOptions.PerformLayout();
			this.m_tableLayoutPanelIncludeOptions.ResumeLayout(false);
			this.m_tableLayoutPanelIncludeOptions.PerformLayout();
			this.m_tableLayoutPanelUntranslatedQuestions.ResumeLayout(false);
			this.m_tableLayoutPanelUntranslatedQuestions.PerformLayout();
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.ResumeLayout(false);
			this.m_tableLayoutPanelDetailQuestionsOutOfOrder.PerformLayout();
			this.tabAppearance.ResumeLayout(false);
			this.m_tableLayoutPanelAppearance.ResumeLayout(false);
			this.m_tableLayoutPanelAppearance.PerformLayout();
			this.m_tableLayoutPanelStyles.ResumeLayout(false);
			this.m_tableLayoutPanelStyles.PerformLayout();
			this.m_tableLayoutPanelCssOptions.ResumeLayout(false);
			this.m_tableLayoutPanelCssOptions.PerformLayout();
			this.m_tableLayoutPanelDisplayOptions.ResumeLayout(false);
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
		private System.Windows.Forms.CheckBox m_chkPassageBeforeOverview;
		private System.Windows.Forms.TextBox m_txtFilename;
		private System.Windows.Forms.CheckBox m_chkIncludeLWCQuestions;
		private System.Windows.Forms.CheckBox m_chkIncludeLWCAnswers;
		private System.Windows.Forms.CheckBox m_chkIncludeLWCComments;
		private System.Windows.Forms.Label m_lblLWCQuestionColor;
		private System.Windows.Forms.Label m_lblCommentTextColor;
		private System.Windows.Forms.Label m_lblLWCAnswerTextColor;
		private System.Windows.Forms.Label m_lblFolder;
		private System.Windows.Forms.ComboBox m_cboBooks;
		private System.Windows.Forms.Label m_lblQuestionGroupHeadingsColor;
		private System.Windows.Forms.Button btnChooseQuestionGroupHeadingsColor;
		private System.Windows.Forms.RadioButton m_rdoUseOriginal;
		private System.Windows.Forms.RadioButton m_rdoDisplayWarning;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown m_numBlankLines;
		private System.Windows.Forms.Label lblExtraAnswerLines;
		private System.Windows.Forms.RadioButton m_rdoUseExternalCss;
		private System.Windows.Forms.RadioButton m_rdoEmbedStyleInfo;
		private System.Windows.Forms.Button btnBrowseCss;
		private System.Windows.Forms.TextBox m_txtCssFile;
		private System.Windows.Forms.CheckBox m_chkAbsoluteCssPath;
		private System.Windows.Forms.CheckBox m_chkOverwriteCss;
		private System.Windows.Forms.CheckBox m_chkNumberQuestions;
        private System.Windows.Forms.Label m_lbFilename;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.TabPage tabOptions;
		private System.Windows.Forms.ComboBox m_cboUseLWC;
		private System.Windows.Forms.Label lblUseLWC;
		private System.Windows.Forms.TabPage tabAppearance;
		private System.Windows.Forms.RadioButton m_rdoSkipUntranslated;
		private System.Windows.Forms.CheckBox m_chkIncludeVerseNumbers;
		private System.Windows.Forms.RadioButton m_rdoDisplayReferenceForOutOfOrderQuestions;
		private System.Windows.Forms.Label label8;
		internal System.Windows.Forms.RadioButton m_rdoOutputPassageForOutOfOrderQuestions;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label lblSectionRangeTo;
		private System.Windows.Forms.GroupBox groupBoxDocument;
		private System.Windows.Forms.Label lblFolder;
		private System.Windows.Forms.Label lblUntranslatedQuestions;
		private System.Windows.Forms.Label lblStyleSpecification;
		private System.Windows.Forms.Label lblDetailQuestionsOutOfOrder;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelRange;
		private System.Windows.Forms.CheckBox m_chkShowUnavailable;
		private System.Windows.Forms.CheckBox m_chkIncludeScriptureForQuestions;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelOptions;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelIncludeOptions;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelUntranslatedQuestions;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelDetailQuestionsOutOfOrder;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelAppearance;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelStyles;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelCssOptions;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelDisplayOptions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}