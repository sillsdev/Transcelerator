// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2012' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: HelpAboutDlg.Designer.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	partial class HelpAboutDlg
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
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.m_linkLabelReleaseNotes = new System.Windows.Forms.LinkLabel();
            this.btnOk = new System.Windows.Forms.Button();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.m_txlInfo = new SIL.Transcelerator.TxlInfo();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_linkLabelReleaseNotes
            // 
            this.m_linkLabelReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_linkLabelReleaseNotes.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelReleaseNotes, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelReleaseNotes, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelReleaseNotes, "HelpAboutDlg.m_linkLabelReleaseNotes");
            this.m_linkLabelReleaseNotes.Location = new System.Drawing.Point(12, 397);
            this.m_linkLabelReleaseNotes.Name = "m_linkLabelReleaseNotes";
            this.m_linkLabelReleaseNotes.Size = new System.Drawing.Size(84, 13);
            this.m_linkLabelReleaseNotes.TabIndex = 2;
            this.m_linkLabelReleaseNotes.TabStop = true;
            this.m_linkLabelReleaseNotes.Text = "Release notes...";
            this.m_linkLabelReleaseNotes.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkLabelReleaseNotes_LinkClicked);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
            this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
            this.btnOk.Location = new System.Drawing.Point(300, 392);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
            this.l10NSharpExtender1.PrefixForNewItems = null;
            // 
            // m_txlInfo
            // 
            this.m_txlInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txlInfo.AutoSize = true;
            this.m_txlInfo.BackColor = System.Drawing.Color.Transparent;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txlInfo, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_txlInfo, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_txlInfo, "HelpAboutDlg.TxlInfo");
            this.m_txlInfo.Location = new System.Drawing.Point(0, 0);
            this.m_txlInfo.Name = "m_txlInfo";
            this.m_txlInfo.Size = new System.Drawing.Size(666, 368);
            this.m_txlInfo.TabIndex = 0;
            // 
            // HelpAboutDlg
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(674, 427);
            this.Controls.Add(this.m_linkLabelReleaseNotes);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.m_txlInfo);
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, "Parameter is \"Transcelerator\" (plugin name).");
            this.l10NSharpExtender1.SetLocalizingId(this, "HelpAboutDlg.WindowTitle");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(690, 465);
            this.Name = "HelpAboutDlg";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "About {0}";
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private TxlInfo m_txlInfo;
		private System.Windows.Forms.Button btnOk;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.LinkLabel m_linkLabelReleaseNotes;
	}
}