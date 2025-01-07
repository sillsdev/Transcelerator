// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
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
			this.m_lblOriginalText = new System.Windows.Forms.Label();
			this.m_lblAlternatives = new System.Windows.Forms.Label();
			this.m_rdoAlternative = new System.Windows.Forms.RadioButton();
			this.m_btnOk = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lblQuestionAlreadyExists = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.m_txtModified = new System.Windows.Forms.TextBox();
			this.m_rdoOriginal = new System.Windows.Forms.RadioButton();
			this.m_rdoCustom = new System.Windows.Forms.RadioButton();
			this.m_tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this.m_pnlVScroll = new System.Windows.Forms.Panel();
			this.m_lblGroupedSetQuestionWarning = new System.Windows.Forms.Label();
			this.m_lblAltQuestionWarning = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.m_tableLayoutPanelMain.SuspendLayout();
			this.m_pnlVScroll.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lblOriginalText
			// 
			this.m_lblOriginalText.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblOriginalText, "EditQuestionDlg.m_lblOriginalText");
			this.m_lblOriginalText.Location = new System.Drawing.Point(4, 37);
			this.m_lblOriginalText.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this.m_lblOriginalText.Name = "m_lblOriginalText";
			this.m_lblOriginalText.Size = new System.Drawing.Size(90, 13);
			this.m_lblOriginalText.TabIndex = 0;
			this.m_lblOriginalText.Text = "Original Question:";
			// 
			// m_lblAlternatives
			// 
			this.m_lblAlternatives.AutoSize = true;
			this.m_lblAlternatives.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAlternatives, "EditQuestionDlg.m_lblAlternatives");
			this.m_lblAlternatives.Location = new System.Drawing.Point(4, 81);
			this.m_lblAlternatives.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
			this.m_lblAlternatives.Name = "m_lblAlternatives";
			this.m_lblAlternatives.Size = new System.Drawing.Size(65, 13);
			this.m_lblAlternatives.TabIndex = 2;
			this.m_lblAlternatives.Text = "Alternatives:";
			// 
			// m_rdoAlternative
			// 
			this.m_rdoAlternative.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoAlternative, "EditQuestionDlg.m_rdoAlternative");
			this.m_rdoAlternative.Location = new System.Drawing.Point(9, 97);
			this.m_rdoAlternative.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
			this.m_rdoAlternative.Name = "m_rdoAlternative";
			this.m_rdoAlternative.Size = new System.Drawing.Size(14, 13);
			this.m_rdoAlternative.TabIndex = 3;
			this.m_rdoAlternative.TabStop = true;
			this.m_rdoAlternative.Tag = "0";
			this.m_rdoAlternative.UseVisualStyleBackColor = true;
			this.m_rdoAlternative.CheckedChanged += new System.EventHandler(this.HandleOriginalOrAlternativeCheckedChanged);
			// 
			// m_btnOk
			// 
			this.m_btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOk.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnOk, "Common.OK");
			this.m_btnOk.Location = new System.Drawing.Point(287, 296);
			this.m_btnOk.Name = "m_btnOk";
			this.m_btnOk.Size = new System.Drawing.Size(75, 23);
			this.m_btnOk.TabIndex = 6;
			this.m_btnOk.Text = "OK";
			this.m_btnOk.UseVisualStyleBackColor = true;
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_btnCancel, "Common.Cancel");
			this.m_btnCancel.Location = new System.Drawing.Point(367, 296);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 7;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_lblQuestionAlreadyExists
			// 
			this.m_lblQuestionAlreadyExists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_lblQuestionAlreadyExists.AutoSize = true;
			this.m_lblQuestionAlreadyExists.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblQuestionAlreadyExists, "EditQuestionDlg.m_lblQuestionAlreadyExists");
			this.m_lblQuestionAlreadyExists.Location = new System.Drawing.Point(4, 254);
			this.m_lblQuestionAlreadyExists.Name = "m_lblQuestionAlreadyExists";
			this.m_lblQuestionAlreadyExists.Size = new System.Drawing.Size(118, 13);
			this.m_lblQuestionAlreadyExists.TabIndex = 9;
			this.m_lblQuestionAlreadyExists.Text = "Question already exists!";
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// m_txtModified
			// 
			this.m_txtModified.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_txtModified.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtModified, "EditQuestionDlg.m_txtModified");
			this.m_txtModified.Location = new System.Drawing.Point(5, 145);
			this.m_txtModified.Margin = new System.Windows.Forms.Padding(4);
			this.m_txtModified.MinimumSize = new System.Drawing.Size(50, 75);
			this.m_txtModified.Multiline = true;
			this.m_txtModified.Name = "m_txtModified";
			this.m_txtModified.Size = new System.Drawing.Size(672, 105);
			this.m_txtModified.TabIndex = 9;
			// 
			// m_rdoOriginal
			// 
			this.m_rdoOriginal.AutoSize = true;
			this.m_rdoOriginal.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoOriginal, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoOriginal, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_rdoOriginal, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoOriginal, "EditQuestionDlg.m_rdoOriginal");
			this.m_rdoOriginal.Location = new System.Drawing.Point(9, 53);
			this.m_rdoOriginal.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
			this.m_rdoOriginal.Name = "m_rdoOriginal";
			this.m_rdoOriginal.Size = new System.Drawing.Size(32, 17);
			this.m_rdoOriginal.TabIndex = 10;
			this.m_rdoOriginal.TabStop = true;
			this.m_rdoOriginal.Text = "#";
			this.m_rdoOriginal.UseVisualStyleBackColor = true;
			this.m_rdoOriginal.CheckedChanged += new System.EventHandler(this.HandleOriginalOrAlternativeCheckedChanged);
			// 
			// m_rdoCustom
			// 
			this.m_rdoCustom.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoCustom, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoCustom, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_rdoCustom, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoCustom, "EditQuestionDlg.m_rdoCustom");
			this.m_rdoCustom.Location = new System.Drawing.Point(9, 121);
			this.m_rdoCustom.Margin = new System.Windows.Forms.Padding(8, 8, 3, 3);
			this.m_rdoCustom.Name = "m_rdoCustom";
			this.m_rdoCustom.Size = new System.Drawing.Size(60, 17);
			this.m_rdoCustom.TabIndex = 12;
			this.m_rdoCustom.TabStop = true;
			this.m_rdoCustom.Text = "Custom";
			this.m_rdoCustom.UseVisualStyleBackColor = true;
			this.m_rdoCustom.CheckedChanged += new System.EventHandler(this.HandleCustomCheckedChanged);
			// 
			// m_tableLayoutPanelMain
			// 
			this.m_tableLayoutPanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_tableLayoutPanelMain.ColumnCount = 1;
			this.m_tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_tableLayoutPanelMain.Controls.Add(this.m_rdoCustom, 0, 6);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_lblQuestionAlreadyExists, 0, 8);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_txtModified, 0, 7);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_rdoAlternative, 0, 5);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_lblAlternatives, 0, 4);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_lblOriginalText, 0, 2);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_rdoOriginal, 0, 3);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_lblGroupedSetQuestionWarning, 0, 0);
			this.m_tableLayoutPanelMain.Controls.Add(this.m_lblAltQuestionWarning, 0, 1);
			this.m_tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
			this.m_tableLayoutPanelMain.Name = "m_tableLayoutPanelMain";
			this.m_tableLayoutPanelMain.Padding = new System.Windows.Forms.Padding(1);
			this.m_tableLayoutPanelMain.RowCount = 9;
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_tableLayoutPanelMain.Size = new System.Drawing.Size(682, 268);
			this.m_tableLayoutPanelMain.TabIndex = 11;
			// 
			// m_pnlVScroll
			// 
			this.m_pnlVScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_pnlVScroll.AutoScroll = true;
			this.m_pnlVScroll.Controls.Add(this.m_tableLayoutPanelMain);
			this.m_pnlVScroll.Location = new System.Drawing.Point(12, 12);
			this.m_pnlVScroll.Name = "m_pnlVScroll";
			this.m_pnlVScroll.Size = new System.Drawing.Size(682, 268);
			this.m_pnlVScroll.TabIndex = 12;
			this.m_pnlVScroll.Layout += new System.Windows.Forms.LayoutEventHandler(this.HandlePanelLayout);
			// 
			// m_lblGroupedSetQuestionWarning
			// 
			this.m_lblGroupedSetQuestionWarning.AutoSize = true;
			this.m_lblGroupedSetQuestionWarning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
			this.m_lblGroupedSetQuestionWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_lblGroupedSetQuestionWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblGroupedSetQuestionWarning.ForeColor = System.Drawing.SystemColors.ControlText;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblGroupedSetQuestionWarning, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblGroupedSetQuestionWarning, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblGroupedSetQuestionWarning, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblGroupedSetQuestionWarning, "EditQuestionDlg.m_lblGroupedSetQuestionWarning");
			this.m_lblGroupedSetQuestionWarning.Location = new System.Drawing.Point(4, 1);
			this.m_lblGroupedSetQuestionWarning.Name = "m_lblGroupedSetQuestionWarning";
			this.m_lblGroupedSetQuestionWarning.Size = new System.Drawing.Size(545, 15);
			this.m_lblGroupedSetQuestionWarning.TabIndex = 13;
			this.m_lblGroupedSetQuestionWarning.Text = "This question is part of a group of related questions. Usually, only one question" +
    " from this group should be included.";
			// 
			// m_lblAltQuestionWarning
			// 
			this.m_lblAltQuestionWarning.AutoSize = true;
			this.m_lblAltQuestionWarning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
			this.m_lblAltQuestionWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAltQuestionWarning, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAltQuestionWarning, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAltQuestionWarning, "label1");
			this.m_lblAltQuestionWarning.Location = new System.Drawing.Point(4, 16);
			this.m_lblAltQuestionWarning.Name = "m_lblAltQuestionWarning";
			this.m_lblAltQuestionWarning.Size = new System.Drawing.Size(508, 15);
			this.m_lblAltQuestionWarning.TabIndex = 14;
			this.m_lblAltQuestionWarning.Text = "This question is one of two related questions. Usually, only one of these two que" +
    "stions should be included.";
			// 
			// EditQuestionDlg
			// 
			this.AcceptButton = this.m_btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(706, 331);
			this.Controls.Add(this.m_pnlVScroll);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOk);
			this.HelpButton = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "EditQuestionDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(720, 243);
			this.Name = "EditQuestionDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Question";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HandleHelpButtonClick);
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.HandleHelpRequest);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.m_tableLayoutPanelMain.ResumeLayout(false);
			this.m_tableLayoutPanelMain.PerformLayout();
			this.m_pnlVScroll.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.RadioButton m_rdoAlternative;
		private System.Windows.Forms.Button m_btnOk;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Label m_lblQuestionAlreadyExists;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label m_lblOriginalText;
		private System.Windows.Forms.Label m_lblAlternatives;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelMain;
		private System.Windows.Forms.TextBox m_txtModified;
		private System.Windows.Forms.RadioButton m_rdoOriginal;
		private System.Windows.Forms.RadioButton m_rdoCustom;
		private System.Windows.Forms.Panel m_pnlVScroll;
		private System.Windows.Forms.Label m_lblGroupedSetQuestionWarning;
		private System.Windows.Forms.Label m_lblAltQuestionWarning;
	}
}