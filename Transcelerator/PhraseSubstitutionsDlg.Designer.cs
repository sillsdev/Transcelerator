// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
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
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhraseSubstitutionsDlg));
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label lblSuffix;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			this.m_numTimesToMatch = new System.Windows.Forms.NumericUpDown();
			this.m_btnMatchSingleWord = new System.Windows.Forms.Button();
			this.m_txtMatchPrefix = new System.Windows.Forms.TextBox();
			this.m_txtMatchSuffix = new System.Windows.Forms.TextBox();
			this.m_cboMatchGroup = new System.Windows.Forms.ComboBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.m_dataGridView = new System.Windows.Forms.DataGridView();
			this.m_regexMatchHelper = new System.Windows.Forms.Panel();
			this.m_grpPreview = new System.Windows.Forms.GroupBox();
			this.m_cboPreviewQuestion = new System.Windows.Forms.ComboBox();
			this.m_regexReplacementHelper = new System.Windows.Forms.Panel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.toolStripUpDownButtons = new System.Windows.Forms.ToolStrip();
			this.btnUp = new System.Windows.Forms.ToolStripButton();
			this.btnDown = new System.Windows.Forms.ToolStripButton();
			this.colMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colReplacement = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colIsRegEx = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colMatchCase = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colPreviewResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
			label1 = new System.Windows.Forms.Label();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			label3 = new System.Windows.Forms.Label();
			lblSuffix = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			label4 = new System.Windows.Forms.Label();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numTimesToMatch)).BeginInit();
			tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
			this.m_regexMatchHelper.SuspendLayout();
			this.m_grpPreview.SuspendLayout();
			this.m_regexReplacementHelper.SuspendLayout();
			this.toolStripUpDownButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
			tableLayoutPanel1.Controls.Add(this.m_numTimesToMatch, 1, 3);
			tableLayoutPanel1.Controls.Add(this.m_btnMatchSingleWord, 0, 0);
			tableLayoutPanel1.Controls.Add(label3, 0, 3);
			tableLayoutPanel1.Controls.Add(this.m_txtMatchPrefix, 1, 1);
			tableLayoutPanel1.Controls.Add(this.m_txtMatchSuffix, 1, 2);
			tableLayoutPanel1.Controls.Add(lblSuffix, 0, 2);
			tableLayoutPanel1.Controls.Add(label2, 0, 1);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// m_numTimesToMatch
			// 
			resources.ApplyResources(this.m_numTimesToMatch, "m_numTimesToMatch");
			this.m_numTimesToMatch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_numTimesToMatch.Name = "m_numTimesToMatch";
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
			resources.ApplyResources(this.m_btnMatchSingleWord, "m_btnMatchSingleWord");
			this.m_btnMatchSingleWord.Name = "m_btnMatchSingleWord";
			this.m_btnMatchSingleWord.UseVisualStyleBackColor = true;
			this.m_btnMatchSingleWord.Click += new System.EventHandler(this.m_btnMatchSingleWord_Click);
			// 
			// label3
			// 
			resources.ApplyResources(label3, "label3");
			label3.Name = "label3";
			// 
			// m_txtMatchPrefix
			// 
			resources.ApplyResources(this.m_txtMatchPrefix, "m_txtMatchPrefix");
			this.m_txtMatchPrefix.Name = "m_txtMatchPrefix";
			this.m_txtMatchPrefix.TextChanged += new System.EventHandler(this.SuffixOrPrefixChanged);
			// 
			// m_txtMatchSuffix
			// 
			this.m_txtMatchSuffix.AcceptsTab = true;
			resources.ApplyResources(this.m_txtMatchSuffix, "m_txtMatchSuffix");
			this.m_txtMatchSuffix.Name = "m_txtMatchSuffix";
			this.m_txtMatchSuffix.TextChanged += new System.EventHandler(this.SuffixOrPrefixChanged);
			// 
			// lblSuffix
			// 
			resources.ApplyResources(lblSuffix, "lblSuffix");
			lblSuffix.Name = "lblSuffix";
			// 
			// label2
			// 
			resources.ApplyResources(label2, "label2");
			label2.Name = "label2";
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
			tableLayoutPanel2.Controls.Add(label4, 0, 0);
			tableLayoutPanel2.Controls.Add(this.m_cboMatchGroup, 1, 0);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// label4
			// 
			resources.ApplyResources(label4, "label4");
			label4.Name = "label4";
			// 
			// m_cboMatchGroup
			// 
			this.m_cboMatchGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboMatchGroup.FormattingEnabled = true;
			this.m_cboMatchGroup.Items.AddRange(new object[] {
            resources.GetString("m_cboMatchGroup.Items")});
			resources.ApplyResources(this.m_cboMatchGroup, "m_cboMatchGroup");
			this.m_cboMatchGroup.Name = "m_cboMatchGroup";
			this.m_cboMatchGroup.SelectedIndexChanged += new System.EventHandler(this.m_cboMatchGroup_SelectedIndexChanged);
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_dataGridView
			// 
			resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
			this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMatch,
            this.colReplacement,
            this.colIsRegEx,
            this.colMatchCase,
            this.colPreviewResult});
			this.m_dataGridView.Name = "m_dataGridView";
			this.m_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellValueChanged);
			this.m_dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_RowEnter);
			this.m_dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellEndEdit);
			this.m_dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.m_dataGridView_EditingControlShowing);
			// 
			// m_regexMatchHelper
			// 
			this.m_regexMatchHelper.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this.m_regexMatchHelper, "m_regexMatchHelper");
			this.m_regexMatchHelper.Controls.Add(tableLayoutPanel1);
			this.m_regexMatchHelper.Name = "m_regexMatchHelper";
			// 
			// m_grpPreview
			// 
			resources.ApplyResources(this.m_grpPreview, "m_grpPreview");
			this.m_grpPreview.Controls.Add(this.m_cboPreviewQuestion);
			this.m_grpPreview.Name = "m_grpPreview";
			this.m_grpPreview.TabStop = false;
			// 
			// m_cboPreviewQuestion
			// 
			resources.ApplyResources(this.m_cboPreviewQuestion, "m_cboPreviewQuestion");
			this.m_cboPreviewQuestion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboPreviewQuestion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboPreviewQuestion.FormattingEnabled = true;
			this.m_cboPreviewQuestion.Name = "m_cboPreviewQuestion";
			this.m_cboPreviewQuestion.TextChanged += new System.EventHandler(this.UpdatePreview);
			// 
			// m_regexReplacementHelper
			// 
			this.m_regexReplacementHelper.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this.m_regexReplacementHelper, "m_regexReplacementHelper");
			this.m_regexReplacementHelper.Controls.Add(tableLayoutPanel2);
			this.m_regexReplacementHelper.Name = "m_regexReplacementHelper";
			// 
			// toolStripUpDownButtons
			// 
			this.toolStripUpDownButtons.AllowMerge = false;
			resources.ApplyResources(this.toolStripUpDownButtons, "toolStripUpDownButtons");
			this.toolStripUpDownButtons.BackColor = System.Drawing.Color.Transparent;
			this.toolStripUpDownButtons.CanOverflow = false;
			this.toolStripUpDownButtons.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripUpDownButtons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUp,
            this.btnDown});
			this.toolStripUpDownButtons.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this.toolStripUpDownButtons.Name = "toolStripUpDownButtons";
			// 
			// btnUp
			// 
			this.btnUp.BackColor = System.Drawing.Color.Transparent;
			this.btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnUp.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnDown.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// colMatch
			// 
			this.colMatch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			resources.ApplyResources(this.colMatch, "colMatch");
			this.colMatch.Name = "colMatch";
			// 
			// colReplacement
			// 
			this.colReplacement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.colReplacement, "colReplacement");
			this.colReplacement.Name = "colReplacement";
			// 
			// colIsRegEx
			// 
			this.colIsRegEx.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle7.NullValue = false;
			this.colIsRegEx.DefaultCellStyle = dataGridViewCellStyle7;
			resources.ApplyResources(this.colIsRegEx, "colIsRegEx");
			this.colIsRegEx.Name = "colIsRegEx";
			// 
			// colMatchCase
			// 
			this.colMatchCase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle8.NullValue = false;
			this.colMatchCase.DefaultCellStyle = dataGridViewCellStyle8;
			resources.ApplyResources(this.colMatchCase, "colMatchCase");
			this.colMatchCase.Name = "colMatchCase";
			// 
			// colPreviewResult
			// 
			this.colPreviewResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.colPreviewResult, "colPreviewResult");
			this.colPreviewResult.Name = "colPreviewResult";
			this.colPreviewResult.ReadOnly = true;
			// 
			// PhraseSubstitutionsDlg
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.toolStripUpDownButtons);
			this.Controls.Add(this.m_regexReplacementHelper);
			this.Controls.Add(this.m_grpPreview);
			this.Controls.Add(this.m_regexMatchHelper);
			this.Controls.Add(this.m_dataGridView);
			this.Controls.Add(label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PhraseSubstitutionsDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
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
		private System.Windows.Forms.DataGridViewTextBoxColumn colMatch;
		private System.Windows.Forms.DataGridViewTextBoxColumn colReplacement;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colIsRegEx;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colMatchCase;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPreviewResult;
	}
}