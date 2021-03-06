// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ScrReferenceFilterDlg.Designer.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	partial class ScrReferenceFilterDlg
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
			this.m_lblFrom = new System.Windows.Forms.Label();
			this.m_lblTo = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.scrPsgFrom = new SIL.Windows.Forms.Scripture.ScrPassageControl();
			this.scrPsgTo = new SIL.Windows.Forms.Scripture.ScrPassageControl();
			this.btnClearFilter = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_lblFrom
			// 
			this.m_lblFrom.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblFrom, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblFrom, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblFrom, "ScrReferenceFilterDlg.m_lblFrom");
			this.m_lblFrom.Location = new System.Drawing.Point(13, 17);
			this.m_lblFrom.Name = "m_lblFrom";
			this.m_lblFrom.Size = new System.Drawing.Size(33, 13);
			this.m_lblFrom.TabIndex = 0;
			this.m_lblFrom.Text = "From:";
			// 
			// m_lblTo
			// 
			this.m_lblTo.AutoSize = true;
			this.m_lblTo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblTo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblTo, "ScrReferenceFilterDlg.m_lblTo");
			this.m_lblTo.Location = new System.Drawing.Point(184, 17);
			this.m_lblTo.Name = "m_lblTo";
			this.m_lblTo.Size = new System.Drawing.Size(23, 13);
			this.m_lblTo.TabIndex = 2;
			this.m_lblTo.Text = "To:";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(133, 52);
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
			this.btnCancel.Location = new System.Drawing.Point(214, 52);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// scrPsgFrom
			// 
			this.scrPsgFrom.AutoScroll = true;
			this.scrPsgFrom.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgFrom.ErrorCaption = "From Reference";
			this.l10NSharpExtender1.SetLocalizableToolTip(this.scrPsgFrom, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.scrPsgFrom, null);
			this.l10NSharpExtender1.SetLocalizingId(this.scrPsgFrom, "ScrReferenceFilterDlg.ScrPassageControl");
			this.scrPsgFrom.Location = new System.Drawing.Point(52, 14);
			this.scrPsgFrom.Name = "scrPsgFrom";
			this.scrPsgFrom.Padding = new System.Windows.Forms.Padding(1);
			this.scrPsgFrom.Reference = "GEN 1:1";
			this.scrPsgFrom.Size = new System.Drawing.Size(110, 20);
			this.scrPsgFrom.TabIndex = 1;
			this.scrPsgFrom.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.scrPsgFrom_PassageChanged);
			// 
			// scrPsgTo
			// 
			this.scrPsgTo.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgTo.ErrorCaption = "To Reference";
			this.l10NSharpExtender1.SetLocalizableToolTip(this.scrPsgTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.scrPsgTo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.scrPsgTo, "ScrReferenceFilterDlg.ScrPassageControl");
			this.scrPsgTo.Location = new System.Drawing.Point(213, 14);
			this.scrPsgTo.Name = "scrPsgTo";
			this.scrPsgTo.Padding = new System.Windows.Forms.Padding(1);
			this.scrPsgTo.Reference = "REV 22:21";
			this.scrPsgTo.Size = new System.Drawing.Size(110, 20);
			this.scrPsgTo.TabIndex = 3;
			this.scrPsgTo.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.scrPsgTo_PassageChanged);
			// 
			// btnClearFilter
			// 
			this.btnClearFilter.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnClearFilter.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClearFilter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnClearFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnClearFilter, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.btnClearFilter, "ScrReferenceFilterDlg.btnClearFilter");
			this.btnClearFilter.Location = new System.Drawing.Point(52, 52);
			this.btnClearFilter.Name = "btnClearFilter";
			this.btnClearFilter.Size = new System.Drawing.Size(75, 23);
			this.btnClearFilter.TabIndex = 4;
			this.btnClearFilter.Text = "&Clear Filter";
			this.btnClearFilter.UseVisualStyleBackColor = true;
			this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// ScrReferenceFilterDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(338, 87);
			this.Controls.Add(this.btnClearFilter);
			this.Controls.Add(this.scrPsgTo);
			this.Controls.Add(this.scrPsgFrom);
			this.Controls.Add(this.m_lblTo);
			this.Controls.Add(this.m_lblFrom);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ScrReferenceFilterDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 594);
			this.MinimizeBox = false;
			this.Name = "ScrReferenceFilterDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Filter by Reference";
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private SIL.Windows.Forms.Scripture.ScrPassageControl scrPsgFrom;
		private SIL.Windows.Forms.Scripture.ScrPassageControl scrPsgTo;
		private System.Windows.Forms.Button btnClearFilter;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label m_lblFrom;
		private System.Windows.Forms.Label m_lblTo;
	}
}