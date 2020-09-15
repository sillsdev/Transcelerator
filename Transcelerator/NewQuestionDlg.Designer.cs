// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2012' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: NewQuestion.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;

namespace SIL.Transcelerator
{
	partial class NewQuestionDlg
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
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label m_lblEnglishQuestion;
			System.Windows.Forms.Label m_lblAnswer;
			System.Windows.Forms.Label m_lblCategory;
			System.Windows.Forms.Label m_lblAnswerIsOptional;
			this.m_lblVernacularQuestionIsOptional = new System.Windows.Forms.Label();
			this.m_lblVernacularQuestion = new System.Windows.Forms.Label();
			this.lblReference = new System.Windows.Forms.Label();
			this.m_txtEnglishQuestion = new System.Windows.Forms.TextBox();
			this.m_lblAlternative = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.m_chkNoEnglish = new System.Windows.Forms.CheckBox();
			this.m_txtAnswer = new System.Windows.Forms.TextBox();
			this.m_scrPsgReference = new SIL.Windows.Forms.Scripture.ScrPassageControl();
			this.m_cboEndVerse = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.m_dataGridViewExistingQuestions = new System.Windows.Forms.DataGridView();
			this.m_lblSelectLocation = new System.Windows.Forms.Label();
			this.m_txtVernacularQuestion = new System.Windows.Forms.TextBox();
			this.m_insertionPointArrow = new System.Windows.Forms.PictureBox();
			this.m_tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.m_pnlEnglishQuestionControls = new System.Windows.Forms.TableLayoutPanel();
			this.m_lblIdenticalQuestion = new System.Windows.Forms.Label();
			this.m_pnlArrow = new System.Windows.Forms.Panel();
			this.m_pnlUpDownArrows = new System.Windows.Forms.Panel();
			this.m_btnDown = new System.Windows.Forms.Button();
			this.m_btnUp = new System.Windows.Forms.Button();
			this.m_cboCategory = new System.Windows.Forms.ComboBox();
			this.m_linklblWishForTxl218 = new System.Windows.Forms.LinkLabel();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.colQuestion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colExcluded = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			m_lblEnglishQuestion = new System.Windows.Forms.Label();
			m_lblAnswer = new System.Windows.Forms.Label();
			m_lblCategory = new System.Windows.Forms.Label();
			m_lblAnswerIsOptional = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExistingQuestions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_insertionPointArrow)).BeginInit();
			this.m_tableLayoutPanel.SuspendLayout();
			this.m_pnlEnglishQuestionControls.SuspendLayout();
			this.m_pnlArrow.SuspendLayout();
			this.m_pnlUpDownArrows.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_lblEnglishQuestion
			// 
			m_lblEnglishQuestion.Anchor = System.Windows.Forms.AnchorStyles.Left;
			m_lblEnglishQuestion.AutoSize = true;
			this.m_tableLayoutPanel.SetColumnSpan(m_lblEnglishQuestion, 3);
			m_lblEnglishQuestion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblEnglishQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblEnglishQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblEnglishQuestion, "NewQuestionDlg.m_lblEnglishQuestion");
			m_lblEnglishQuestion.Location = new System.Drawing.Point(3, 136);
			m_lblEnglishQuestion.Name = "m_lblEnglishQuestion";
			m_lblEnglishQuestion.Padding = new System.Windows.Forms.Padding(0, 10, 0, 4);
			m_lblEnglishQuestion.Size = new System.Drawing.Size(100, 27);
			m_lblEnglishQuestion.TabIndex = 0;
			m_lblEnglishQuestion.Text = "Question in English:";
			// 
			// m_lblAnswer
			// 
			m_lblAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			m_lblAnswer.AutoSize = true;
			this.m_tableLayoutPanel.SetColumnSpan(m_lblAnswer, 4);
			m_lblAnswer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblAnswer, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblAnswer, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblAnswer, "NewQuestionDlg.m_lblAnswer");
			m_lblAnswer.Location = new System.Drawing.Point(3, 308);
			m_lblAnswer.Name = "m_lblAnswer";
			m_lblAnswer.Padding = new System.Windows.Forms.Padding(0, 10, 0, 4);
			m_lblAnswer.Size = new System.Drawing.Size(45, 27);
			m_lblAnswer.TabIndex = 4;
			m_lblAnswer.Text = "Answer:";
			// 
			// m_lblCategory
			// 
			m_lblCategory.AutoSize = true;
			m_lblCategory.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblCategory, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblCategory, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblCategory, "NewQuestionDlg.m_lblCategory");
			m_lblCategory.Location = new System.Drawing.Point(12, 44);
			m_lblCategory.Name = "m_lblCategory";
			m_lblCategory.Size = new System.Drawing.Size(52, 13);
			m_lblCategory.TabIndex = 7;
			m_lblCategory.Text = "Category:";
			// 
			// m_lblAnswerIsOptional
			// 
			m_lblAnswerIsOptional.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			m_lblAnswerIsOptional.AutoSize = true;
			m_lblAnswerIsOptional.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
			m_lblAnswerIsOptional.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblAnswerIsOptional, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblAnswerIsOptional, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblAnswerIsOptional, "NewQuestionDlg.m_lblAnswerIsOptional");
			m_lblAnswerIsOptional.Location = new System.Drawing.Point(444, 318);
			m_lblAnswerIsOptional.Name = "m_lblAnswerIsOptional";
			m_lblAnswerIsOptional.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			m_lblAnswerIsOptional.Size = new System.Drawing.Size(44, 17);
			m_lblAnswerIsOptional.TabIndex = 8;
			m_lblAnswerIsOptional.Text = "optional";
			// 
			// m_lblVernacularQuestionIsOptional
			// 
			this.m_lblVernacularQuestionIsOptional.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblVernacularQuestionIsOptional.AutoSize = true;
			this.m_lblVernacularQuestionIsOptional.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
			this.m_lblVernacularQuestionIsOptional.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.m_lblVernacularQuestionIsOptional.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblVernacularQuestionIsOptional, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblVernacularQuestionIsOptional, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblVernacularQuestionIsOptional, "NewQuestionDlg.m_lblVernacularQuestionIsOptional");
			this.m_lblVernacularQuestionIsOptional.Location = new System.Drawing.Point(444, 239);
			this.m_lblVernacularQuestionIsOptional.Name = "m_lblVernacularQuestionIsOptional";
			this.m_lblVernacularQuestionIsOptional.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.m_lblVernacularQuestionIsOptional.Size = new System.Drawing.Size(44, 17);
			this.m_lblVernacularQuestionIsOptional.TabIndex = 7;
			this.m_lblVernacularQuestionIsOptional.Text = "optional";
			// 
			// m_lblVernacularQuestion
			// 
			this.m_lblVernacularQuestion.AutoSize = true;
			this.m_tableLayoutPanel.SetColumnSpan(this.m_lblVernacularQuestion, 4);
			this.m_lblVernacularQuestion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblVernacularQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblVernacularQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblVernacularQuestion, "NewQuestionDlg.m_lblVernacularQuestion");
			this.m_lblVernacularQuestion.Location = new System.Drawing.Point(3, 229);
			this.m_lblVernacularQuestion.Name = "m_lblVernacularQuestion";
			this.m_lblVernacularQuestion.Padding = new System.Windows.Forms.Padding(0, 10, 0, 4);
			this.m_lblVernacularQuestion.Size = new System.Drawing.Size(80, 27);
			this.m_lblVernacularQuestion.TabIndex = 2;
			this.m_lblVernacularQuestion.Text = "Question in {0}:";
			// 
			// lblReference
			// 
			this.lblReference.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblReference, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblReference, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblReference, "NewQuestionDlg.lblReference");
			this.lblReference.Location = new System.Drawing.Point(12, 9);
			this.lblReference.Name = "lblReference";
			this.lblReference.Size = new System.Drawing.Size(105, 13);
			this.lblReference.TabIndex = 3;
			this.lblReference.Text = "Scripture Reference:";
			// 
			// m_txtEnglishQuestion
			// 
			this.m_txtEnglishQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtEnglishQuestion, 5);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtEnglishQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtEnglishQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtEnglishQuestion, "NewQuestionDlg.m_txtEnglishQuestion");
			this.m_txtEnglishQuestion.Location = new System.Drawing.Point(3, 180);
			this.m_txtEnglishQuestion.MinimumSize = new System.Drawing.Size(40, 20);
			this.m_txtEnglishQuestion.Multiline = true;
			this.m_txtEnglishQuestion.Name = "m_txtEnglishQuestion";
			this.m_txtEnglishQuestion.Size = new System.Drawing.Size(485, 46);
			this.m_txtEnglishQuestion.TabIndex = 1;
			this.m_txtEnglishQuestion.TextChanged += new System.EventHandler(this.HandleQuestionTextChanged);
			// 
			// m_lblAlternative
			// 
			this.m_lblAlternative.AutoSize = true;
			this.m_lblAlternative.Dock = System.Windows.Forms.DockStyle.Fill;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAlternative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAlternative, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAlternative, "m_lblAlternative");
			this.m_lblAlternative.Location = new System.Drawing.Point(148, 0);
			this.m_lblAlternative.Name = "m_lblAlternative";
			this.m_lblAlternative.Size = new System.Drawing.Size(14, 19);
			this.m_lblAlternative.TabIndex = 4;
			this.m_lblAlternative.Text = "#";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(181, 464);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Location = new System.Drawing.Point(262, 464);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_chkNoEnglish
			// 
			this.m_chkNoEnglish.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.m_chkNoEnglish.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_chkNoEnglish, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_chkNoEnglish, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_chkNoEnglish, "NewQuestionDlg.m_chkNoEnglish");
			this.m_chkNoEnglish.Location = new System.Drawing.Point(3, 3);
			this.m_chkNoEnglish.Name = "m_chkNoEnglish";
			this.m_chkNoEnglish.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.m_chkNoEnglish.Size = new System.Drawing.Size(334, 23);
			this.m_chkNoEnglish.TabIndex = 6;
			this.m_chkNoEnglish.Text = "I just want to add a question without providing an English version.";
			this.m_chkNoEnglish.UseVisualStyleBackColor = true;
			this.m_chkNoEnglish.CheckedChanged += new System.EventHandler(this.chkNoEnglish_CheckedChanged);
			// 
			// m_txtAnswer
			// 
			this.m_txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtAnswer, 5);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtAnswer, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtAnswer, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtAnswer, "NewQuestionDlg.m_txtAnswer");
			this.m_txtAnswer.Location = new System.Drawing.Point(3, 338);
			this.m_txtAnswer.MinimumSize = new System.Drawing.Size(40, 20);
			this.m_txtAnswer.Multiline = true;
			this.m_txtAnswer.Name = "m_txtAnswer";
			this.m_txtAnswer.Size = new System.Drawing.Size(485, 26);
			this.m_txtAnswer.TabIndex = 5;
			// 
			// m_scrPsgReference
			// 
			this.m_scrPsgReference.AutoScroll = true;
			this.m_scrPsgReference.BackColor = System.Drawing.SystemColors.Window;
			this.m_scrPsgReference.ErrorCaption = "From Reference";
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_scrPsgReference, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_scrPsgReference, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_scrPsgReference, "NewQuestionDlg.ScrPassageControl");
			this.m_scrPsgReference.Location = new System.Drawing.Point(139, 6);
			this.m_scrPsgReference.Name = "m_scrPsgReference";
			this.m_scrPsgReference.Padding = new System.Windows.Forms.Padding(1);
			this.m_scrPsgReference.Reference = "GEN 1:1";
			this.m_scrPsgReference.Size = new System.Drawing.Size(110, 20);
			this.m_scrPsgReference.TabIndex = 4;
			this.m_scrPsgReference.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.m_scrPsgReference_PassageChanged);
			// 
			// m_cboEndVerse
			// 
			this.m_cboEndVerse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboEndVerse.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboEndVerse, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboEndVerse, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboEndVerse, "NewQuestionDlg.m_cboEndVerse");
			this.m_cboEndVerse.Location = new System.Drawing.Point(355, 6);
			this.m_cboEndVerse.MaxDropDownItems = 20;
			this.m_cboEndVerse.Name = "m_cboEndVerse";
			this.m_cboEndVerse.Size = new System.Drawing.Size(62, 21);
			this.m_cboEndVerse.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.label3, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.label3, null);
			this.l10NSharpExtender1.SetLocalizingId(this.label3, "NewQuestionDlg.label3");
			this.label3.Location = new System.Drawing.Point(290, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "End Verse:";
			// 
			// m_dataGridViewExistingQuestions
			// 
			this.m_dataGridViewExistingQuestions.AllowUserToAddRows = false;
			this.m_dataGridViewExistingQuestions.AllowUserToDeleteRows = false;
			this.m_dataGridViewExistingQuestions.AllowUserToResizeRows = false;
			this.m_dataGridViewExistingQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_dataGridViewExistingQuestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dataGridViewExistingQuestions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colQuestion,
            this.colTranslation,
            this.colExcluded});
			this.m_tableLayoutPanel.SetColumnSpan(this.m_dataGridViewExistingQuestions, 3);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_dataGridViewExistingQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_dataGridViewExistingQuestions, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_dataGridViewExistingQuestions, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_dataGridViewExistingQuestions, "NewQuestionDlg.ExistingQuestions");
			this.m_dataGridViewExistingQuestions.Location = new System.Drawing.Point(56, 17);
			this.m_dataGridViewExistingQuestions.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.m_dataGridViewExistingQuestions.MinimumSize = new System.Drawing.Size(0, 42);
			this.m_dataGridViewExistingQuestions.MultiSelect = false;
			this.m_dataGridViewExistingQuestions.Name = "m_dataGridViewExistingQuestions";
			this.m_dataGridViewExistingQuestions.ReadOnly = true;
			this.m_dataGridViewExistingQuestions.RowHeadersVisible = false;
			this.m_dataGridViewExistingQuestions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_dataGridViewExistingQuestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.m_dataGridViewExistingQuestions.ShowCellErrors = false;
			this.m_dataGridViewExistingQuestions.ShowCellToolTips = false;
			this.m_dataGridViewExistingQuestions.ShowEditingIcon = false;
			this.m_dataGridViewExistingQuestions.ShowRowErrors = false;
			this.m_dataGridViewExistingQuestions.Size = new System.Drawing.Size(435, 98);
			this.m_dataGridViewExistingQuestions.TabIndex = 11;
			this.m_dataGridViewExistingQuestions.Scroll += new System.Windows.Forms.ScrollEventHandler(this.m_dataGridViewExistingQuestions_Scroll);
			// 
			// m_lblSelectLocation
			// 
			this.m_lblSelectLocation.AutoSize = true;
			this.m_tableLayoutPanel.SetColumnSpan(this.m_lblSelectLocation, 5);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblSelectLocation, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblSelectLocation, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblSelectLocation, "NewQuestionDlg.m_lblSelectLocation");
			this.m_lblSelectLocation.Location = new System.Drawing.Point(3, 0);
			this.m_lblSelectLocation.Name = "m_lblSelectLocation";
			this.m_lblSelectLocation.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.m_lblSelectLocation.Size = new System.Drawing.Size(253, 17);
			this.m_lblSelectLocation.TabIndex = 9;
			this.m_lblSelectLocation.Text = "Select location in list of existing {0} questions for {1}.";
			// 
			// m_txtVernacularQuestion
			// 
			this.m_txtVernacularQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtVernacularQuestion, 5);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtVernacularQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtVernacularQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtVernacularQuestion, "NewQuestionDlg.m_txtVernacularQuestion");
			this.m_txtVernacularQuestion.Location = new System.Drawing.Point(3, 259);
			this.m_txtVernacularQuestion.MinimumSize = new System.Drawing.Size(40, 20);
			this.m_txtVernacularQuestion.Multiline = true;
			this.m_txtVernacularQuestion.Name = "m_txtVernacularQuestion";
			this.m_txtVernacularQuestion.Size = new System.Drawing.Size(485, 46);
			this.m_txtVernacularQuestion.TabIndex = 3;
			this.m_txtVernacularQuestion.TextChanged += new System.EventHandler(this.HandleQuestionTextChanged);
			this.m_txtVernacularQuestion.Enter += new System.EventHandler(this.m_txtVernacularQuestion_Enter);
			this.m_txtVernacularQuestion.Leave += new System.EventHandler(this.m_txtVernacularQuestion_Leave);
			// 
			// m_insertionPointArrow
			// 
			this.m_insertionPointArrow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_insertionPointArrow.Image = global::SIL.Transcelerator.Properties.Resources.control_right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_insertionPointArrow, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_insertionPointArrow, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_insertionPointArrow, "NewQuestionDlg.m_insertionPointArrow");
			this.m_insertionPointArrow.Location = new System.Drawing.Point(0, 8);
			this.m_insertionPointArrow.Name = "m_insertionPointArrow";
			this.m_insertionPointArrow.Size = new System.Drawing.Size(17, 26);
			this.m_insertionPointArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.m_insertionPointArrow.TabIndex = 21;
			this.m_insertionPointArrow.TabStop = false;
			// 
			// m_tableLayoutPanel
			// 
			this.m_tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_tableLayoutPanel.ColumnCount = 5;
			this.m_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanel.Controls.Add(this.m_pnlEnglishQuestionControls, 3, 2);
			this.m_tableLayoutPanel.Controls.Add(m_lblEnglishQuestion, 0, 2);
			this.m_tableLayoutPanel.Controls.Add(this.m_pnlArrow, 1, 1);
			this.m_tableLayoutPanel.Controls.Add(this.m_lblVernacularQuestion, 0, 4);
			this.m_tableLayoutPanel.Controls.Add(this.m_lblSelectLocation, 0, 0);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtVernacularQuestion, 0, 5);
			this.m_tableLayoutPanel.Controls.Add(this.m_dataGridViewExistingQuestions, 2, 1);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtEnglishQuestion, 0, 3);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtAnswer, 0, 7);
			this.m_tableLayoutPanel.Controls.Add(m_lblAnswer, 0, 6);
			this.m_tableLayoutPanel.Controls.Add(this.m_pnlUpDownArrows, 0, 1);
			this.m_tableLayoutPanel.Controls.Add(this.m_lblVernacularQuestionIsOptional, 4, 4);
			this.m_tableLayoutPanel.Controls.Add(m_lblAnswerIsOptional, 4, 6);
			this.m_tableLayoutPanel.Location = new System.Drawing.Point(15, 79);
			this.m_tableLayoutPanel.Name = "m_tableLayoutPanel";
			this.m_tableLayoutPanel.RowCount = 8;
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.46154F));
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.46154F));
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
			this.m_tableLayoutPanel.Size = new System.Drawing.Size(491, 367);
			this.m_tableLayoutPanel.TabIndex = 0;
			// 
			// m_pnlEnglishQuestionControls
			// 
			this.m_pnlEnglishQuestionControls.AutoSize = true;
			this.m_pnlEnglishQuestionControls.ColumnCount = 1;
			this.m_tableLayoutPanel.SetColumnSpan(this.m_pnlEnglishQuestionControls, 2);
			this.m_pnlEnglishQuestionControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_pnlEnglishQuestionControls.Controls.Add(this.m_chkNoEnglish, 0, 0);
			this.m_pnlEnglishQuestionControls.Controls.Add(this.m_lblIdenticalQuestion, 0, 1);
			this.m_pnlEnglishQuestionControls.Location = new System.Drawing.Point(148, 126);
			this.m_pnlEnglishQuestionControls.Name = "m_pnlEnglishQuestionControls";
			this.m_pnlEnglishQuestionControls.RowCount = 2;
			this.m_pnlEnglishQuestionControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_pnlEnglishQuestionControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_pnlEnglishQuestionControls.Size = new System.Drawing.Size(340, 48);
			this.m_pnlEnglishQuestionControls.TabIndex = 9;
			// 
			// m_lblIdenticalQuestion
			// 
			this.m_lblIdenticalQuestion.AutoSize = true;
			this.m_lblIdenticalQuestion.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblIdenticalQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblIdenticalQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblIdenticalQuestion, "NewQuestionDlg.m_lblIdenticalQuestion");
			this.m_lblIdenticalQuestion.Location = new System.Drawing.Point(3, 29);
			this.m_lblIdenticalQuestion.Name = "m_lblIdenticalQuestion";
			this.m_lblIdenticalQuestion.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.m_lblIdenticalQuestion.Size = new System.Drawing.Size(198, 19);
			this.m_lblIdenticalQuestion.TabIndex = 7;
			this.m_lblIdenticalQuestion.Text = "This question already exists (see above).";
			this.m_lblIdenticalQuestion.Visible = false;
			// 
			// m_pnlArrow
			// 
			this.m_pnlArrow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.m_pnlArrow.Controls.Add(this.m_insertionPointArrow);
			this.m_pnlArrow.Location = new System.Drawing.Point(39, 17);
			this.m_pnlArrow.Margin = new System.Windows.Forms.Padding(0);
			this.m_pnlArrow.MinimumSize = new System.Drawing.Size(17, 50);
			this.m_pnlArrow.Name = "m_pnlArrow";
			this.m_pnlArrow.Size = new System.Drawing.Size(17, 106);
			this.m_pnlArrow.TabIndex = 10;
			// 
			// m_pnlUpDownArrows
			// 
			this.m_pnlUpDownArrows.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.m_pnlUpDownArrows.AutoSize = true;
			this.m_pnlUpDownArrows.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_pnlUpDownArrows.Controls.Add(this.m_btnDown);
			this.m_pnlUpDownArrows.Controls.Add(this.m_btnUp);
			this.m_pnlUpDownArrows.Location = new System.Drawing.Point(3, 35);
			this.m_pnlUpDownArrows.Name = "m_pnlUpDownArrows";
			this.m_pnlUpDownArrows.Size = new System.Drawing.Size(33, 69);
			this.m_pnlUpDownArrows.TabIndex = 24;
			// 
			// m_btnDown
			// 
			this.m_btnDown.AutoSize = true;
			this.m_btnDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_btnDown.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon24x24;
			this.m_btnDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnDown, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnDown, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnDown, "NewQuestionDlg.m_btnDown");
			this.m_btnDown.Location = new System.Drawing.Point(0, 36);
			this.m_btnDown.Name = "m_btnDown";
			this.m_btnDown.Size = new System.Drawing.Size(30, 30);
			this.m_btnDown.TabIndex = 1;
			this.m_btnDown.UseVisualStyleBackColor = true;
			this.m_btnDown.Click += new System.EventHandler(this.HandleArrowClick);
			// 
			// m_btnUp
			// 
			this.m_btnUp.AutoSize = true;
			this.m_btnUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_btnUp.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon24x24;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnUp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnUp, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnUp, "NewQuestionDlg.m_btnUp");
			this.m_btnUp.Location = new System.Drawing.Point(0, 0);
			this.m_btnUp.Name = "m_btnUp";
			this.m_btnUp.Size = new System.Drawing.Size(30, 30);
			this.m_btnUp.TabIndex = 0;
			this.m_btnUp.UseVisualStyleBackColor = true;
			this.m_btnUp.Click += new System.EventHandler(this.HandleArrowClick);
			// 
			// m_cboCategory
			// 
			this.m_cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboCategory.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboCategory, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboCategory, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboCategory, "NewQuestionDlg.m_cboCategory");
			this.m_cboCategory.Location = new System.Drawing.Point(139, 41);
			this.m_cboCategory.Name = "m_cboCategory";
			this.m_cboCategory.Size = new System.Drawing.Size(110, 21);
			this.m_cboCategory.TabIndex = 8;
			this.m_cboCategory.SelectedIndexChanged += new System.EventHandler(this.HandleCategoryChanged);
			// 
			// m_linklblWishForTxl218
			// 
			this.m_linklblWishForTxl218.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linklblWishForTxl218, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_linklblWishForTxl218, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_linklblWishForTxl218, "NewQuestionDlg.m_linklblWishForTxl218");
			this.m_linklblWishForTxl218.Location = new System.Drawing.Point(293, 43);
			this.m_linklblWishForTxl218.Name = "m_linklblWishForTxl218";
			this.m_linklblWishForTxl218.Size = new System.Drawing.Size(156, 13);
			this.m_linklblWishForTxl218.TabIndex = 9;
			this.m_linklblWishForTxl218.TabStop = true;
			this.m_linklblWishForTxl218.Text = "I wish I could add this category.";
			this.m_linklblWishForTxl218.Visible = false;
			this.m_linklblWishForTxl218.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linklblWishForTxl218_LinkClicked);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// colQuestion
			// 
			this.colQuestion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colQuestion.DataPropertyName = "PhraseToDisplayInUI";
			this.colQuestion.FillWeight = 200F;
			this.colQuestion.HeaderText = "_L10N_:NewQuestionsDlg.ExistingQuestions.Question!Question";
			this.colQuestion.Name = "colQuestion";
			this.colQuestion.ReadOnly = true;
			this.colQuestion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colTranslation
			// 
			this.colTranslation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colTranslation.DataPropertyName = "Translation";
			this.colTranslation.HeaderText = "_L10N_:NewQuestionsDlg.ExistingQuestions.Translation!Translation";
			this.colTranslation.Name = "colTranslation";
			this.colTranslation.ReadOnly = true;
			this.colTranslation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colExcluded
			// 
			this.colExcluded.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.colExcluded.DataPropertyName = "IsExcluded";
			this.colExcluded.HeaderText = "_L10N_:NewQuestionsDlg.ExistingQuestions.Excluded!Excluded";
			this.colExcluded.MinimumWidth = 15;
			this.colExcluded.Name = "colExcluded";
			this.colExcluded.ReadOnly = true;
			this.colExcluded.Visible = false;
			this.colExcluded.Width = 319;
			// 
			// NewQuestionDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(518, 499);
			this.Controls.Add(this.m_linklblWishForTxl218);
			this.Controls.Add(this.m_cboCategory);
			this.Controls.Add(m_lblCategory);
			this.Controls.Add(this.m_tableLayoutPanel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.m_cboEndVerse);
			this.Controls.Add(this.m_scrPsgReference);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblReference);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "NewQuestionDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(490, 482);
			this.Name = "NewQuestionDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Question";
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExistingQuestions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_insertionPointArrow)).EndInit();
			this.m_tableLayoutPanel.ResumeLayout(false);
			this.m_tableLayoutPanel.PerformLayout();
			this.m_pnlEnglishQuestionControls.ResumeLayout(false);
			this.m_pnlEnglishQuestionControls.PerformLayout();
			this.m_pnlArrow.ResumeLayout(false);
			this.m_pnlUpDownArrows.ResumeLayout(false);
			this.m_pnlUpDownArrows.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_txtEnglishQuestion;
		private System.Windows.Forms.Label m_lblAlternative;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox m_chkNoEnglish;
		private System.Windows.Forms.Label lblReference;
		private System.Windows.Forms.TextBox m_txtAnswer;
		private Windows.Forms.Scripture.ScrPassageControl m_scrPsgReference;
		private System.Windows.Forms.ComboBox m_cboEndVerse;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGridView m_dataGridViewExistingQuestions;
		private System.Windows.Forms.Label m_lblSelectLocation;
		private System.Windows.Forms.TextBox m_txtVernacularQuestion;
		private System.Windows.Forms.PictureBox m_insertionPointArrow;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanel;
		private System.Windows.Forms.Panel m_pnlArrow;
		private System.Windows.Forms.Label m_lblVernacularQuestion;
		private System.Windows.Forms.ComboBox m_cboCategory;
		private System.Windows.Forms.Panel m_pnlUpDownArrows;
		private System.Windows.Forms.Button m_btnUp;
		private System.Windows.Forms.Button m_btnDown;
		private System.Windows.Forms.Label m_lblVernacularQuestionIsOptional;
		private System.Windows.Forms.TableLayoutPanel m_pnlEnglishQuestionControls;
		private System.Windows.Forms.Label m_lblIdenticalQuestion;
		private System.Windows.Forms.LinkLabel m_linklblWishForTxl218;
		private L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.DataGridViewTextBoxColumn colQuestion;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTranslation;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colExcluded;
	}
}