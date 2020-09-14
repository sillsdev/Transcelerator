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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditQuestionDlg));
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
			resources.ApplyResources(m_lblOriginalText, "m_lblOriginalText");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalText, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalText, "EditQuestionDlg.m_lblOriginalText");
			m_lblOriginalText.Name = "m_lblOriginalText";
			// 
			// m_lblAlternatives
			// 
			resources.ApplyResources(m_lblAlternatives, "m_lblAlternatives");
			this.m_pnlAlternatives.SetFlowBreak(m_lblAlternatives, true);
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblAlternatives, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblAlternatives, "EditQuestionDlg.m_lblAlternatives");
			m_lblAlternatives.Name = "m_lblAlternatives";
			// 
			// m_lblQuestionToUse
			// 
			resources.ApplyResources(m_lblQuestionToUse, "m_lblQuestionToUse");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblQuestionToUse, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblQuestionToUse, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblQuestionToUse, "EditQuestionDlg.m_lblQuestionToUse");
			m_lblQuestionToUse.Name = "m_lblQuestionToUse";
			// 
			// m_txtOriginal
			// 
			resources.ApplyResources(this.m_txtOriginal, "m_txtOriginal");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtOriginal, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtOriginal, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtOriginal, "EditQuestionDlg.m_txtOriginal");
			this.m_txtOriginal.Name = "m_txtOriginal";
			// 
			// m_rdoAlternative
			// 
			resources.ApplyResources(this.m_rdoAlternative, "m_rdoAlternative");
			this.m_pnlAlternatives.SetFlowBreak(this.m_rdoAlternative, true);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoAlternative, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoAlternative, "EditQuestionDlg.m_rdoAlternative");
			this.m_rdoAlternative.Name = "m_rdoAlternative";
			this.m_rdoAlternative.TabStop = true;
			this.m_rdoAlternative.Tag = "0";
			this.m_rdoAlternative.UseVisualStyleBackColor = true;
			this.m_rdoAlternative.CheckedChanged += new System.EventHandler(this.m_rdoAlternative_CheckedChanged);
			// 
			// m_lblAlternative
			// 
			resources.ApplyResources(this.m_lblAlternative, "m_lblAlternative");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAlternative, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAlternative, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAlternative, "m_lblAlternative");
			this.m_lblAlternative.Name = "m_lblAlternative";
			// 
			// m_txtModified
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtModified, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtModified, "EditQuestionDlg.m_txtModified");
			resources.ApplyResources(this.m_txtModified, "m_txtModified");
			this.m_txtModified.Name = "m_txtModified";
			this.m_txtModified.TextChanged += new System.EventHandler(this.m_txtModified_TextChanged);
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this.m_pnlAlternatives);
			this.flowLayoutPanel1.Controls.Add(m_lblQuestionToUse);
			this.flowLayoutPanel1.Controls.Add(this.m_txtModified);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// m_pnlAlternatives
			// 
			resources.ApplyResources(this.m_pnlAlternatives, "m_pnlAlternatives");
			this.m_pnlAlternatives.Controls.Add(m_lblAlternatives);
			this.m_pnlAlternatives.Controls.Add(this.m_rdoAlternative);
			this.flowLayoutPanel1.SetFlowBreak(this.m_pnlAlternatives, true);
			this.m_pnlAlternatives.Name = "m_pnlAlternatives";
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnReset, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnReset, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnReset, "EditQuestionDlg.btnReset");
			resources.ApplyResources(this.btnReset, "btnReset");
			this.btnReset.Name = "btnReset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// m_lblQuestionAlreadyExists
			// 
			resources.ApplyResources(this.m_lblQuestionAlreadyExists, "m_lblQuestionAlreadyExists");
			this.m_lblQuestionAlreadyExists.ForeColor = System.Drawing.Color.Red;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblQuestionAlreadyExists, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblQuestionAlreadyExists, "EditQuestionDlg.m_lblQuestionAlreadyExists");
			this.m_lblQuestionAlreadyExists.Name = "m_lblQuestionAlreadyExists";
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// EditQuestionDlg
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
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