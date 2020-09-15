// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: EditQuestionDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace SIL.Transcelerator
{
	partial class EditQuestionDlg
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
			Justification="Labels get added to Controls collection and disposed there")]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label m_lblOriginalText;
			System.Windows.Forms.Label m_lblAlternatives;
			System.Windows.Forms.Label m_lblQuestionToUse;
			this.m_txtOriginal = new System.Windows.Forms.TextBox();
			this.m_rdoAlternative = new System.Windows.Forms.RadioButton();
			this.m_lblAlternative = new System.Windows.Forms.Label();
			this.m_txtModified = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.m_pnlAlternatives = new System.Windows.Forms.FlowLayoutPanel();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			this.m_lblQuestionAlreadyExists = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			m_lblOriginalText = new System.Windows.Forms.Label();
			m_lblAlternatives = new System.Windows.Forms.Label();
			m_lblQuestionToUse = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
			this.m_pnlAlternatives.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_lblOriginalText
			// 
			m_lblOriginalText.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalText, "EditQuestionDlg.m_lblOriginalText");
			m_lblOriginalText.Location = new System.Drawing.Point(12, 9);
			m_lblOriginalText.Name = "m_lblOriginalText";
			m_lblOriginalText.Size = new System.Drawing.Size(126, 13);
			m_lblOriginalText.TabIndex = 0;
			m_lblOriginalText.Text = "Text of Original Question:";
			// 
			// m_lblAlternatives
			// 
			m_lblAlternatives.AutoSize = true;
			this.m_pnlAlternatives.SetFlowBreak(m_lblAlternatives, true);
			m_lblAlternatives.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblAlternatives, "EditQuestionDlg.m_lblAlternatives");
			m_lblAlternatives.Location = new System.Drawing.Point(3, 0);
			m_lblAlternatives.Name = "m_lblAlternatives";
			m_lblAlternatives.Size = new System.Drawing.Size(119, 13);
			m_lblAlternatives.TabIndex = 2;
			m_lblAlternatives.Text = "Suggested Alternatives:";
			// 
			// m_lblQuestionToUse
			// 
			m_lblQuestionToUse.AutoSize = true;
			m_lblQuestionToUse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblQuestionToUse, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblQuestionToUse, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblQuestionToUse, "EditQuestionDlg.m_lblQuestionToUse");
			m_lblQuestionToUse.Location = new System.Drawing.Point(3, 44);
			m_lblQuestionToUse.Name = "m_lblQuestionToUse";
			m_lblQuestionToUse.Size = new System.Drawing.Size(86, 13);
			m_lblQuestionToUse.TabIndex = 5;
			m_lblQuestionToUse.Text = "Question to Use:";
			// 
			// m_txtOriginal
			// 
			this.m_txtOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_txtOriginal.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtOriginal, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtOriginal, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtOriginal, "EditQuestionDlg.m_txtOriginal");
			this.m_txtOriginal.Location = new System.Drawing.Point(144, 6);
			this.m_txtOriginal.Multiline = true;
			this.m_txtOriginal.Name = "m_txtOriginal";
			this.m_txtOriginal.Size = new System.Drawing.Size(550, 57);
			this.m_txtOriginal.TabIndex = 1;
			// 
			// m_rdoAlternative
			// 
			this.m_rdoAlternative.AutoSize = true;
			this.m_pnlAlternatives.SetFlowBreak(this.m_rdoAlternative, true);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoAlternative, "EditQuestionDlg.m_rdoAlternative");
			this.m_rdoAlternative.Location = new System.Drawing.Point(3, 22);
			this.m_rdoAlternative.Name = "m_rdoAlternative";
			this.m_rdoAlternative.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
			this.m_rdoAlternative.Size = new System.Drawing.Size(22, 13);
			this.m_rdoAlternative.TabIndex = 3;
			this.m_rdoAlternative.TabStop = true;
			this.m_rdoAlternative.Tag = "0";
			this.m_rdoAlternative.UseVisualStyleBackColor = true;
			this.m_rdoAlternative.CheckedChanged += new System.EventHandler(this.m_rdoAlternative_CheckedChanged);
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
			// m_txtModified
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtModified, "EditQuestionDlg.m_txtModified");
			this.m_txtModified.Location = new System.Drawing.Point(92, 44);
			this.m_txtModified.Margin = new System.Windows.Forms.Padding(0);
			this.m_txtModified.Multiline = true;
			this.m_txtModified.Name = "m_txtModified";
			this.m_txtModified.Size = new System.Drawing.Size(587, 64);
			this.m_txtModified.TabIndex = 4;
			this.m_txtModified.TextChanged += new System.EventHandler(this.m_txtModified_TextChanged);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.m_pnlAlternatives);
			this.flowLayoutPanel1.Controls.Add(m_lblQuestionToUse);
			this.flowLayoutPanel1.Controls.Add(this.m_txtModified);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(15, 79);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(679, 170);
			this.flowLayoutPanel1.TabIndex = 5;
			// 
			// m_pnlAlternatives
			// 
			this.m_pnlAlternatives.AutoSize = true;
			this.m_pnlAlternatives.Controls.Add(m_lblAlternatives);
			this.m_pnlAlternatives.Controls.Add(this.m_rdoAlternative);
			this.flowLayoutPanel1.SetFlowBreak(this.m_pnlAlternatives, true);
			this.m_pnlAlternatives.Location = new System.Drawing.Point(3, 3);
			this.m_pnlAlternatives.Name = "m_pnlAlternatives";
			this.m_pnlAlternatives.Size = new System.Drawing.Size(153, 38);
			this.m_pnlAlternatives.TabIndex = 6;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(275, 274);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
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
			this.btnCancel.Location = new System.Drawing.Point(356, 274);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnReset, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnReset, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnReset, "EditQuestionDlg.btnReset");
			this.btnReset.Location = new System.Drawing.Point(38, 28);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 8;
			this.btnReset.Text = "&Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// m_lblQuestionAlreadyExists
			// 
			this.m_lblQuestionAlreadyExists.AutoSize = true;
			this.m_lblQuestionAlreadyExists.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblQuestionAlreadyExists, "EditQuestionDlg.m_lblQuestionAlreadyExists");
			this.m_lblQuestionAlreadyExists.Location = new System.Drawing.Point(18, 279);
			this.m_lblQuestionAlreadyExists.Name = "m_lblQuestionAlreadyExists";
			this.m_lblQuestionAlreadyExists.Size = new System.Drawing.Size(118, 13);
			this.m_lblQuestionAlreadyExists.TabIndex = 9;
			this.m_lblQuestionAlreadyExists.Text = "Question already exists!";
			this.m_lblQuestionAlreadyExists.Visible = false;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// EditQuestionDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(706, 309);
			this.Controls.Add(this.m_lblQuestionAlreadyExists);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.m_txtOriginal);
			this.Controls.Add(m_lblOriginalText);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "EditQuestionDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditQuestionDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Question";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.m_pnlAlternatives.ResumeLayout(false);
			this.m_pnlAlternatives.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_txtOriginal;
		private System.Windows.Forms.RadioButton m_rdoAlternative;
		private System.Windows.Forms.Label m_lblAlternative;
		private System.Windows.Forms.TextBox m_txtModified;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.FlowLayoutPanel m_pnlAlternatives;
		private System.Windows.Forms.Label m_lblQuestionAlreadyExists;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}