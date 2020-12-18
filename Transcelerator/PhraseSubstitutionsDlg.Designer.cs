// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PhrasesToIgnoreDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace SIL.Transcelerator
{
	partial class PhraseSubstitutionsDlg
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
			if (disposing)
			{
				if (components != null)
					components.Dispose();

				// REVIEW: it might be better to add the two drop downs to the Controls collection
				if (m_regexMatchDropDown != null)
					m_regexMatchDropDown.Dispose();
				if (m_regexReplaceDropDown != null)
					m_regexReplaceDropDown.Dispose();
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
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhraseSubstitutionsDlg));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.m_numTimesToMatch = new System.Windows.Forms.NumericUpDown();
			this.m_btnMatchSingleWord = new System.Windows.Forms.Button();
			this.lblMaxMatch = new System.Windows.Forms.Label();
			this.m_txtMatchPrefix = new System.Windows.Forms.TextBox();
			this.m_txtMatchSuffix = new System.Windows.Forms.TextBox();
			this.lblSuffix = new System.Windows.Forms.Label();
			this.lblMatchPrefix = new System.Windows.Forms.Label();
			this.lblMatchGroup = new System.Windows.Forms.Label();
			this.m_cboMatchGroup = new System.Windows.Forms.ComboBox();
			this.lblInstructions = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.m_dataGridView = new System.Windows.Forms.DataGridView();
			this.colMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colReplacement = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colIsRegEx = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colMatchCase = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colPreviewResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_regexMatchHelper = new System.Windows.Forms.Panel();
			this.m_grpPreview = new System.Windows.Forms.GroupBox();
			this.m_cboPreviewQuestion = new System.Windows.Forms.ComboBox();
			this.m_regexReplacementHelper = new System.Windows.Forms.Panel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.toolStripUpDownButtons = new System.Windows.Forms.ToolStrip();
			this.btnUp = new System.Windows.Forms.ToolStripButton();
			this.btnDown = new System.Windows.Forms.ToolStripButton();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numTimesToMatch)).BeginInit();
			tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
			this.m_regexMatchHelper.SuspendLayout();
			this.m_grpPreview.SuspendLayout();
			this.m_regexReplacementHelper.SuspendLayout();
			this.toolStripUpDownButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(this.m_numTimesToMatch, 1, 3);
			tableLayoutPanel1.Controls.Add(this.m_btnMatchSingleWord, 0, 0);
			tableLayoutPanel1.Controls.Add(this.lblMaxMatch, 0, 3);
			tableLayoutPanel1.Controls.Add(this.m_txtMatchPrefix, 1, 1);
			tableLayoutPanel1.Controls.Add(this.m_txtMatchSuffix, 1, 2);
			tableLayoutPanel1.Controls.Add(this.lblSuffix, 0, 2);
			tableLayoutPanel1.Controls.Add(this.lblMatchPrefix, 0, 1);
			tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 4;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			tableLayoutPanel1.Size = new System.Drawing.Size(272, 113);
			tableLayoutPanel1.TabIndex = 10;
			// 
			// m_numTimesToMatch
			// 
			this.m_numTimesToMatch.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_numTimesToMatch, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_numTimesToMatch, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_numTimesToMatch, "PhraseSubstitutionsDlg.m_numTimesToMatch");
			this.m_numTimesToMatch.Location = new System.Drawing.Point(162, 87);
			this.m_numTimesToMatch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_numTimesToMatch.Name = "m_numTimesToMatch";
			this.m_numTimesToMatch.Size = new System.Drawing.Size(107, 20);
			this.m_numTimesToMatch.TabIndex = 7;
			this.m_numTimesToMatch.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_numTimesToMatch.ValueChanged += new System.EventHandler(this.m_numTimesToMatch_ValueChanged);
			// 
			// m_btnMatchSingleWord
			// 
			tableLayoutPanel1.SetColumnSpan(this.m_btnMatchSingleWord, 2);
			this.m_btnMatchSingleWord.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnMatchSingleWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnMatchSingleWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnMatchSingleWord, "PhraseSubstitutionsDlg.m_btnMatchSingleWord");
			this.m_btnMatchSingleWord.Location = new System.Drawing.Point(3, 3);
			this.m_btnMatchSingleWord.Name = "m_btnMatchSingleWord";
			this.m_btnMatchSingleWord.Size = new System.Drawing.Size(244, 23);
			this.m_btnMatchSingleWord.TabIndex = 0;
			this.m_btnMatchSingleWord.Text = "Match any single word";
			this.m_btnMatchSingleWord.UseVisualStyleBackColor = true;
			this.m_btnMatchSingleWord.Click += new System.EventHandler(this.m_btnMatchSingleWord_Click);
			// 
			// lblMaxMatch
			// 
			this.lblMaxMatch.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblMaxMatch.AutoSize = true;
			this.lblMaxMatch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblMaxMatch, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblMaxMatch, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblMaxMatch, "PhraseSubstitutionsDlg.lblMaxMatch");
			this.lblMaxMatch.Location = new System.Drawing.Point(3, 90);
			this.lblMaxMatch.Name = "lblMaxMatch";
			this.lblMaxMatch.Size = new System.Drawing.Size(122, 13);
			this.lblMaxMatch.TabIndex = 5;
			this.lblMaxMatch.Text = "Maximum times to match";
			// 
			// m_txtMatchPrefix
			// 
			this.m_txtMatchPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtMatchPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtMatchPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtMatchPrefix, "PhraseSubstitutionsDlg.m_txtMatchPrefix");
			this.m_txtMatchPrefix.Location = new System.Drawing.Point(162, 32);
			this.m_txtMatchPrefix.Name = "m_txtMatchPrefix";
			this.m_txtMatchPrefix.Size = new System.Drawing.Size(107, 20);
			this.m_txtMatchPrefix.TabIndex = 2;
			this.m_txtMatchPrefix.TextChanged += new System.EventHandler(this.SuffixOrPrefixChanged);
			// 
			// m_txtMatchSuffix
			// 
			this.m_txtMatchSuffix.AcceptsTab = true;
			this.m_txtMatchSuffix.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtMatchSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtMatchSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtMatchSuffix, "PhraseSubstitutionsDlg.m_txtMatchSuffix");
			this.m_txtMatchSuffix.Location = new System.Drawing.Point(162, 58);
			this.m_txtMatchSuffix.Name = "m_txtMatchSuffix";
			this.m_txtMatchSuffix.Size = new System.Drawing.Size(107, 20);
			this.m_txtMatchSuffix.TabIndex = 4;
			this.m_txtMatchSuffix.TextChanged += new System.EventHandler(this.SuffixOrPrefixChanged);
			// 
			// lblSuffix
			// 
			this.lblSuffix.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblSuffix.AutoSize = true;
			this.lblSuffix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblSuffix, "PhraseSubstitutionsDlg.lblSuffix");
			this.lblSuffix.Location = new System.Drawing.Point(3, 61);
			this.lblSuffix.Name = "lblSuffix";
			this.lblSuffix.Size = new System.Drawing.Size(73, 13);
			this.lblSuffix.TabIndex = 3;
			this.lblSuffix.Text = "Match a suffix";
			// 
			// lblMatchPrefix
			// 
			this.lblMatchPrefix.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblMatchPrefix.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblMatchPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblMatchPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblMatchPrefix, "PhraseSubstitutionsDlg.lblMatchPrefix");
			this.lblMatchPrefix.Location = new System.Drawing.Point(3, 35);
			this.lblMatchPrefix.Name = "lblMatchPrefix";
			this.lblMatchPrefix.Size = new System.Drawing.Size(74, 13);
			this.lblMatchPrefix.TabIndex = 1;
			this.lblMatchPrefix.Text = "Match a prefix";
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.AutoSize = true;
			tableLayoutPanel2.ColumnCount = 2;
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(this.lblMatchGroup, 0, 0);
			tableLayoutPanel2.Controls.Add(this.m_cboMatchGroup, 1, 0);
			tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 2;
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			tableLayoutPanel2.Size = new System.Drawing.Size(215, 30);
			tableLayoutPanel2.TabIndex = 10;
			// 
			// lblMatchGroup
			// 
			this.lblMatchGroup.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblMatchGroup.AutoSize = true;
			this.lblMatchGroup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblMatchGroup, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblMatchGroup, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblMatchGroup, "PhraseSubstitutionsDlg.lblMatchGroup");
			this.lblMatchGroup.Location = new System.Drawing.Point(3, 7);
			this.lblMatchGroup.Name = "lblMatchGroup";
			this.lblMatchGroup.Size = new System.Drawing.Size(67, 13);
			this.lblMatchGroup.TabIndex = 5;
			this.lblMatchGroup.Text = "Match group";
			// 
			// m_cboMatchGroup
			// 
			this.m_cboMatchGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboMatchGroup.FormattingEnabled = true;
			this.m_cboMatchGroup.Items.AddRange(new object[] {
            "Remove"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboMatchGroup, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboMatchGroup, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboMatchGroup, "PhraseSubstitutionsDlg.m_cboMatchGroup");
			this.m_cboMatchGroup.Location = new System.Drawing.Point(76, 3);
			this.m_cboMatchGroup.Name = "m_cboMatchGroup";
			this.m_cboMatchGroup.Size = new System.Drawing.Size(121, 21);
			this.m_cboMatchGroup.TabIndex = 6;
			this.m_cboMatchGroup.SelectedIndexChanged += new System.EventHandler(this.m_cboMatchGroup_SelectedIndexChanged);
			// 
			// lblInstructions
			// 
			this.lblInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblInstructions, "PhraseSubstitutionsDlg.lblInstructions");
			this.lblInstructions.Location = new System.Drawing.Point(12, 9);
			this.lblInstructions.Name = "lblInstructions";
			this.lblInstructions.Size = new System.Drawing.Size(744, 103);
			this.lblInstructions.TabIndex = 8;
			this.lblInstructions.Text = resources.GetString("lblInstructions.Text");
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(314, 403);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Location = new System.Drawing.Point(397, 403);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_dataGridView
			// 
			this.m_dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMatch,
            this.colReplacement,
            this.colIsRegEx,
            this.colMatchCase,
            this.colPreviewResult});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_dataGridView, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_dataGridView, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_dataGridView, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_dataGridView, "PhraseSubstitutionsDlg.Replacements");
			this.m_dataGridView.Location = new System.Drawing.Point(12, 180);
			this.m_dataGridView.Name = "m_dataGridView";
			this.m_dataGridView.Size = new System.Drawing.Size(740, 207);
			this.m_dataGridView.TabIndex = 9;
			this.m_dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellEndEdit);
			this.m_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellValueChanged);
			this.m_dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.m_dataGridView_EditingControlShowing);
			this.m_dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_RowEnter);
			// 
			// colMatch
			// 
			this.colMatch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.colMatch.HeaderText = "_L10N_:PhraseSubstitutionsDlg.Replacements.WordOrPhraseToReplace!Word or Phrase t" +
    "o Replace";
			this.colMatch.MinimumWidth = 200;
			this.colMatch.Name = "colMatch";
			this.colMatch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.colMatch.ToolTipText = "Enter text to replace or omit from the original text of the question";
			this.colMatch.Width = 396;
			// 
			// colReplacement
			// 
			this.colReplacement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colReplacement.HeaderText = "_L10N_:PhraseSubstitutionsDlg.Replacements.Replacement!Replacement";
			this.colReplacement.MinimumWidth = 100;
			this.colReplacement.Name = "colReplacement";
			this.colReplacement.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.colReplacement.ToolTipText = "Enter replacement text or expression (leave blank to omit the word or phrase enti" +
    "rely)";
			// 
			// colIsRegEx
			// 
			this.colIsRegEx.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.NullValue = false;
			this.colIsRegEx.DefaultCellStyle = dataGridViewCellStyle5;
			this.colIsRegEx.HeaderText = "_L10N_:PhraseSubstitutionsDlg.Replacements.RegularExpression!Regular Expression";
			this.colIsRegEx.Name = "colIsRegEx";
			this.colIsRegEx.ToolTipText = "Select to interpret as a regular expression";
			this.colIsRegEx.Width = 377;
			// 
			// colMatchCase
			// 
			this.colMatchCase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.NullValue = false;
			this.colMatchCase.DefaultCellStyle = dataGridViewCellStyle6;
			this.colMatchCase.HeaderText = "_L10N_:PhraseSubstitutionsDlg.Replacements.MatchCase!Match Case";
			this.colMatchCase.Name = "colMatchCase";
			this.colMatchCase.ToolTipText = "Select to indicat that matches should be case-sensitive";
			this.colMatchCase.Width = 351;
			// 
			// colPreviewResult
			// 
			this.colPreviewResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colPreviewResult.HeaderText = "_L10N_:PhraseSubstitutionsDlg.Replacements.PreviewResult!Preview Result";
			this.colPreviewResult.MinimumWidth = 100;
			this.colPreviewResult.Name = "colPreviewResult";
			this.colPreviewResult.ReadOnly = true;
			this.colPreviewResult.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// m_regexMatchHelper
			// 
			this.m_regexMatchHelper.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.m_regexMatchHelper.AutoSize = true;
			this.m_regexMatchHelper.Controls.Add(tableLayoutPanel1);
			this.m_regexMatchHelper.Location = new System.Drawing.Point(21, 259);
			this.m_regexMatchHelper.Name = "m_regexMatchHelper";
			this.m_regexMatchHelper.Size = new System.Drawing.Size(278, 119);
			this.m_regexMatchHelper.TabIndex = 10;
			this.m_regexMatchHelper.Visible = false;
			// 
			// m_grpPreview
			// 
			this.m_grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_grpPreview.Controls.Add(this.m_cboPreviewQuestion);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_grpPreview, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_grpPreview, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_grpPreview, "PhraseSubstitutionsDlg.m_grpPreview");
			this.m_grpPreview.Location = new System.Drawing.Point(12, 125);
			this.m_grpPreview.Name = "m_grpPreview";
			this.m_grpPreview.Size = new System.Drawing.Size(744, 49);
			this.m_grpPreview.TabIndex = 11;
			this.m_grpPreview.TabStop = false;
			this.m_grpPreview.Text = "Preview Sample Question";
			// 
			// m_cboPreviewQuestion
			// 
			this.m_cboPreviewQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_cboPreviewQuestion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboPreviewQuestion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboPreviewQuestion.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboPreviewQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboPreviewQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboPreviewQuestion, "PhraseSubstitutionsDlg.m_cboPreviewQuestion");
			this.m_cboPreviewQuestion.Location = new System.Drawing.Point(6, 19);
			this.m_cboPreviewQuestion.Name = "m_cboPreviewQuestion";
			this.m_cboPreviewQuestion.Size = new System.Drawing.Size(732, 21);
			this.m_cboPreviewQuestion.TabIndex = 0;
			this.m_cboPreviewQuestion.TextChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// m_regexReplacementHelper
			// 
			this.m_regexReplacementHelper.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.m_regexReplacementHelper.AutoSize = true;
			this.m_regexReplacementHelper.Controls.Add(tableLayoutPanel2);
			this.m_regexReplacementHelper.Location = new System.Drawing.Point(287, 259);
			this.m_regexReplacementHelper.Name = "m_regexReplacementHelper";
			this.m_regexReplacementHelper.Size = new System.Drawing.Size(221, 37);
			this.m_regexReplacementHelper.TabIndex = 12;
			this.m_regexReplacementHelper.Visible = false;
			// 
			// toolStripUpDownButtons
			// 
			this.toolStripUpDownButtons.AllowMerge = false;
			this.toolStripUpDownButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.toolStripUpDownButtons.BackColor = System.Drawing.Color.Transparent;
			this.toolStripUpDownButtons.CanOverflow = false;
			this.toolStripUpDownButtons.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripUpDownButtons.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripUpDownButtons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUp,
            this.btnDown});
			this.toolStripUpDownButtons.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripUpDownButtons, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripUpDownButtons, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripUpDownButtons, "PhraseSubstitutionsDlg.toolStripUpDownButtons");
			this.toolStripUpDownButtons.Location = new System.Drawing.Point(762, 220);
			this.toolStripUpDownButtons.Name = "toolStripUpDownButtons";
			this.toolStripUpDownButtons.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
			this.toolStripUpDownButtons.Size = new System.Drawing.Size(29, 48);
			this.toolStripUpDownButtons.TabIndex = 14;
			// 
			// btnUp
			// 
			this.btnUp.BackColor = System.Drawing.Color.Transparent;
			this.btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnUp.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon;
			this.btnUp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnUp, "Move Up");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnUp, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnUp, "PhraseSubstitutionsDlg.btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(22, 20);
			this.btnUp.Text = "Up";
			this.btnUp.ToolTipText = "Move Up";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnDown.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon;
			this.btnDown.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnDown, "Move Down");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnDown, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnDown, "PhraseSubstitutionsDlg.btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(22, 20);
			this.btnDown.Text = "Down";
			this.btnDown.ToolTipText = "Move Down";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// PhraseSubstitutionsDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(786, 438);
			this.Controls.Add(this.toolStripUpDownButtons);
			this.Controls.Add(this.m_regexReplacementHelper);
			this.Controls.Add(this.m_grpPreview);
			this.Controls.Add(this.m_regexMatchHelper);
			this.Controls.Add(this.m_dataGridView);
			this.Controls.Add(this.lblInstructions);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "PhraseSubstitutionsDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PhraseSubstitutionsDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Question Adjustments";
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numTimesToMatch)).EndInit();
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
			this.m_regexMatchHelper.ResumeLayout(false);
			this.m_regexMatchHelper.PerformLayout();
			this.m_grpPreview.ResumeLayout(false);
			this.m_regexReplacementHelper.ResumeLayout(false);
			this.m_regexReplacementHelper.PerformLayout();
			this.toolStripUpDownButtons.ResumeLayout(false);
			this.toolStripUpDownButtons.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.DataGridView m_dataGridView;
		private System.Windows.Forms.Panel m_regexMatchHelper;
		private System.Windows.Forms.Button m_btnMatchSingleWord;
		private System.Windows.Forms.NumericUpDown m_numTimesToMatch;
		private System.Windows.Forms.GroupBox m_grpPreview;
		private System.Windows.Forms.ComboBox m_cboPreviewQuestion;
		protected System.Windows.Forms.TextBox m_txtMatchPrefix;
		protected System.Windows.Forms.TextBox m_txtMatchSuffix;
		private System.Windows.Forms.Panel m_regexReplacementHelper;
		private System.Windows.Forms.ComboBox m_cboMatchGroup;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStrip toolStripUpDownButtons;
		private System.Windows.Forms.ToolStripButton btnUp;
		private System.Windows.Forms.ToolStripButton btnDown;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMatch;
		private System.Windows.Forms.DataGridViewTextBoxColumn colReplacement;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colIsRegEx;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colMatchCase;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPreviewResult;
		private System.Windows.Forms.Label lblInstructions;
		private System.Windows.Forms.Label lblMaxMatch;
		private System.Windows.Forms.Label lblSuffix;
		private System.Windows.Forms.Label lblMatchPrefix;
		private System.Windows.Forms.Label lblMatchGroup;
	}
}