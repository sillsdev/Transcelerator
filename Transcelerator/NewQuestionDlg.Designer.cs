// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: NewQuestion.cs
// Responsibility: Bogle
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

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
			System.Windows.Forms.Label m_lblEnglishQuestion;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewQuestionDlg));
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
			this.colQuestion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_lblSelectLocation = new System.Windows.Forms.Label();
			this.m_txtVernacularQuestion = new System.Windows.Forms.TextBox();
			this.m_insertionPointArrow = new System.Windows.Forms.PictureBox();
			this.m_tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.m_pnlArrow = new System.Windows.Forms.Panel();
			this.m_pnlUpDownArrows = new System.Windows.Forms.Panel();
			this.m_btnDown = new System.Windows.Forms.Button();
			this.m_btnUp = new System.Windows.Forms.Button();
			this.m_cboCategory = new System.Windows.Forms.ComboBox();
			m_lblEnglishQuestion = new System.Windows.Forms.Label();
			m_lblAnswer = new System.Windows.Forms.Label();
			m_lblCategory = new System.Windows.Forms.Label();
			m_lblAnswerIsOptional = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExistingQuestions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_insertionPointArrow)).BeginInit();
			this.m_tableLayoutPanel.SuspendLayout();
			this.m_pnlArrow.SuspendLayout();
			this.m_pnlUpDownArrows.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lblEnglishQuestion
			// 
			resources.ApplyResources(m_lblEnglishQuestion, "m_lblEnglishQuestion");
			this.m_tableLayoutPanel.SetColumnSpan(m_lblEnglishQuestion, 3);
			m_lblEnglishQuestion.Name = "m_lblEnglishQuestion";
			// 
			// m_lblAnswer
			// 
			resources.ApplyResources(m_lblAnswer, "m_lblAnswer");
			this.m_tableLayoutPanel.SetColumnSpan(m_lblAnswer, 3);
			m_lblAnswer.Name = "m_lblAnswer";
			// 
			// m_lblCategory
			// 
			resources.ApplyResources(m_lblCategory, "m_lblCategory");
			m_lblCategory.Name = "m_lblCategory";
			// 
			// m_lblAnswerIsOptional
			// 
			resources.ApplyResources(m_lblAnswerIsOptional, "m_lblAnswerIsOptional");
			m_lblAnswerIsOptional.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			m_lblAnswerIsOptional.Name = "m_lblAnswerIsOptional";
			// 
			// m_lblVernacularQuestionIsOptional
			// 
			resources.ApplyResources(this.m_lblVernacularQuestionIsOptional, "m_lblVernacularQuestionIsOptional");
			this.m_lblVernacularQuestionIsOptional.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.m_lblVernacularQuestionIsOptional.Name = "m_lblVernacularQuestionIsOptional";
			// 
			// m_lblVernacularQuestion
			// 
			resources.ApplyResources(this.m_lblVernacularQuestion, "m_lblVernacularQuestion");
			this.m_tableLayoutPanel.SetColumnSpan(this.m_lblVernacularQuestion, 3);
			this.m_lblVernacularQuestion.Name = "m_lblVernacularQuestion";
			// 
			// lblReference
			// 
			resources.ApplyResources(this.lblReference, "lblReference");
			this.lblReference.Name = "lblReference";
			// 
			// m_txtEnglishQuestion
			// 
			resources.ApplyResources(this.m_txtEnglishQuestion, "m_txtEnglishQuestion");
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtEnglishQuestion, 4);
			this.m_txtEnglishQuestion.Name = "m_txtEnglishQuestion";
			this.m_txtEnglishQuestion.TextChanged += new System.EventHandler(this.HandleQuestionTextChanged);
			// 
			// m_lblAlternative
			// 
			resources.ApplyResources(this.m_lblAlternative, "m_lblAlternative");
			this.m_lblAlternative.Name = "m_lblAlternative";
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
			// m_chkNoEnglish
			// 
			resources.ApplyResources(this.m_chkNoEnglish, "m_chkNoEnglish");
			this.m_chkNoEnglish.Name = "m_chkNoEnglish";
			this.m_chkNoEnglish.UseVisualStyleBackColor = true;
			this.m_chkNoEnglish.CheckedChanged += new System.EventHandler(this.chkNoEnglish_CheckedChanged);
			// 
			// m_txtAnswer
			// 
			resources.ApplyResources(this.m_txtAnswer, "m_txtAnswer");
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtAnswer, 4);
			this.m_txtAnswer.Name = "m_txtAnswer";
			// 
			// m_scrPsgReference
			// 
			resources.ApplyResources(this.m_scrPsgReference, "m_scrPsgReference");
			this.m_scrPsgReference.BackColor = System.Drawing.SystemColors.Window;
			this.m_scrPsgReference.ErrorCaption = "From Reference";
			this.m_scrPsgReference.Name = "m_scrPsgReference";
			this.m_scrPsgReference.Reference = "GEN 1:1";
			this.m_scrPsgReference.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.m_scrPsgReference_PassageChanged);
			// 
			// m_cboEndVerse
			// 
			this.m_cboEndVerse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboEndVerse.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboEndVerse, "m_cboEndVerse");
			this.m_cboEndVerse.Name = "m_cboEndVerse";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// m_dataGridViewExistingQuestions
			// 
			this.m_dataGridViewExistingQuestions.AllowUserToAddRows = false;
			this.m_dataGridViewExistingQuestions.AllowUserToDeleteRows = false;
			this.m_dataGridViewExistingQuestions.AllowUserToResizeRows = false;
			resources.ApplyResources(this.m_dataGridViewExistingQuestions, "m_dataGridViewExistingQuestions");
			this.m_dataGridViewExistingQuestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dataGridViewExistingQuestions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colQuestion,
            this.colTranslation});
			this.m_tableLayoutPanel.SetColumnSpan(this.m_dataGridViewExistingQuestions, 2);
			this.m_dataGridViewExistingQuestions.MultiSelect = false;
			this.m_dataGridViewExistingQuestions.Name = "m_dataGridViewExistingQuestions";
			this.m_dataGridViewExistingQuestions.ReadOnly = true;
			this.m_dataGridViewExistingQuestions.RowHeadersVisible = false;
			this.m_dataGridViewExistingQuestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.m_dataGridViewExistingQuestions.ShowCellErrors = false;
			this.m_dataGridViewExistingQuestions.ShowCellToolTips = false;
			this.m_dataGridViewExistingQuestions.ShowEditingIcon = false;
			this.m_dataGridViewExistingQuestions.ShowRowErrors = false;
			this.m_dataGridViewExistingQuestions.Scroll += new System.Windows.Forms.ScrollEventHandler(this.m_dataGridViewExistingQuestions_Scroll);
			this.m_dataGridViewExistingQuestions.SelectionChanged += new System.EventHandler(this.m_dataGridViewExistingQuestions_SelectionChanged);
			// 
			// colQuestion
			// 
			this.colQuestion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colQuestion.DataPropertyName = "PhraseToDisplayInUI";
			resources.ApplyResources(this.colQuestion, "colQuestion");
			this.colQuestion.Name = "colQuestion";
			this.colQuestion.ReadOnly = true;
			this.colQuestion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colTranslation
			// 
			this.colTranslation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colTranslation.DataPropertyName = "Translation";
			resources.ApplyResources(this.colTranslation, "colTranslation");
			this.colTranslation.Name = "colTranslation";
			this.colTranslation.ReadOnly = true;
			this.colTranslation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// m_lblSelectLocation
			// 
			resources.ApplyResources(this.m_lblSelectLocation, "m_lblSelectLocation");
			this.m_tableLayoutPanel.SetColumnSpan(this.m_lblSelectLocation, 4);
			this.m_lblSelectLocation.Name = "m_lblSelectLocation";
			// 
			// m_txtVernacularQuestion
			// 
			resources.ApplyResources(this.m_txtVernacularQuestion, "m_txtVernacularQuestion");
			this.m_tableLayoutPanel.SetColumnSpan(this.m_txtVernacularQuestion, 4);
			this.m_txtVernacularQuestion.Name = "m_txtVernacularQuestion";
			this.m_txtVernacularQuestion.TextChanged += new System.EventHandler(this.HandleQuestionTextChanged);
			// 
			// m_insertionPointArrow
			// 
			resources.ApplyResources(this.m_insertionPointArrow, "m_insertionPointArrow");
			this.m_insertionPointArrow.Image = global::SIL.Transcelerator.Properties.Resources.control_right;
			this.m_insertionPointArrow.Name = "m_insertionPointArrow";
			this.m_insertionPointArrow.TabStop = false;
			// 
			// m_tableLayoutPanel
			// 
			resources.ApplyResources(this.m_tableLayoutPanel, "m_tableLayoutPanel");
			this.m_tableLayoutPanel.Controls.Add(this.m_lblVernacularQuestionIsOptional, 3, 4);
			this.m_tableLayoutPanel.Controls.Add(m_lblEnglishQuestion, 0, 2);
			this.m_tableLayoutPanel.Controls.Add(this.m_pnlArrow, 1, 1);
			this.m_tableLayoutPanel.Controls.Add(this.m_lblVernacularQuestion, 0, 4);
			this.m_tableLayoutPanel.Controls.Add(this.m_lblSelectLocation, 0, 0);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtVernacularQuestion, 0, 5);
			this.m_tableLayoutPanel.Controls.Add(this.m_dataGridViewExistingQuestions, 2, 1);
			this.m_tableLayoutPanel.Controls.Add(this.m_chkNoEnglish, 3, 2);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtEnglishQuestion, 0, 3);
			this.m_tableLayoutPanel.Controls.Add(this.m_txtAnswer, 0, 7);
			this.m_tableLayoutPanel.Controls.Add(m_lblAnswer, 0, 6);
			this.m_tableLayoutPanel.Controls.Add(this.m_pnlUpDownArrows, 0, 1);
			this.m_tableLayoutPanel.Controls.Add(m_lblAnswerIsOptional, 3, 6);
			this.m_tableLayoutPanel.Name = "m_tableLayoutPanel";
			// 
			// m_pnlArrow
			// 
			resources.ApplyResources(this.m_pnlArrow, "m_pnlArrow");
			this.m_pnlArrow.Controls.Add(this.m_insertionPointArrow);
			this.m_pnlArrow.Name = "m_pnlArrow";
			// 
			// m_pnlUpDownArrows
			// 
			resources.ApplyResources(this.m_pnlUpDownArrows, "m_pnlUpDownArrows");
			this.m_pnlUpDownArrows.Controls.Add(this.m_btnDown);
			this.m_pnlUpDownArrows.Controls.Add(this.m_btnUp);
			this.m_pnlUpDownArrows.Name = "m_pnlUpDownArrows";
			// 
			// m_btnDown
			// 
			resources.ApplyResources(this.m_btnDown, "m_btnDown");
			this.m_btnDown.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon24x24;
			this.m_btnDown.Name = "m_btnDown";
			this.m_btnDown.UseVisualStyleBackColor = true;
			this.m_btnDown.Click += new System.EventHandler(this.HandleDownArrowClick);
			// 
			// m_btnUp
			// 
			resources.ApplyResources(this.m_btnUp, "m_btnUp");
			this.m_btnUp.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon24x24;
			this.m_btnUp.Name = "m_btnUp";
			this.m_btnUp.UseVisualStyleBackColor = true;
			this.m_btnUp.Click += new System.EventHandler(this.HandleUpArrowClick);
			// 
			// m_cboCategory
			// 
			this.m_cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboCategory.FormattingEnabled = true;
			resources.ApplyResources(this.m_cboCategory, "m_cboCategory");
			this.m_cboCategory.Name = "m_cboCategory";
			this.m_cboCategory.SelectedIndexChanged += new System.EventHandler(this.HandleCategoryChanged);
			// 
			// NewQuestionDlg
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.m_cboCategory);
			this.Controls.Add(m_lblCategory);
			this.Controls.Add(this.m_tableLayoutPanel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.m_cboEndVerse);
			this.Controls.Add(this.m_scrPsgReference);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblReference);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewQuestionDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExistingQuestions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_insertionPointArrow)).EndInit();
			this.m_tableLayoutPanel.ResumeLayout(false);
			this.m_tableLayoutPanel.PerformLayout();
			this.m_pnlArrow.ResumeLayout(false);
			this.m_pnlUpDownArrows.ResumeLayout(false);
			this.m_pnlUpDownArrows.PerformLayout();
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
		private System.Windows.Forms.DataGridViewTextBoxColumn colQuestion;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTranslation;
		private System.Windows.Forms.Panel m_pnlUpDownArrows;
		private System.Windows.Forms.Button m_btnUp;
		private System.Windows.Forms.Button m_btnDown;
		private System.Windows.Forms.Label m_lblVernacularQuestionIsOptional;
	}
}