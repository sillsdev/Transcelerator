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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrReferenceFilterDlg));
			System.Windows.Forms.Label m_lblFrom;
			System.Windows.Forms.Label m_lblTo;
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.scrPsgFrom = new SIL.Windows.Forms.Scripture.ScrPassageControl();
			this.scrPsgTo = new SIL.Windows.Forms.Scripture.ScrPassageControl();
			this.btnClearFilter = new System.Windows.Forms.Button();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			m_lblFrom = new System.Windows.Forms.Label();
			m_lblTo = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
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
			// m_lblFrom
			// 
			resources.ApplyResources(m_lblFrom, "m_lblFrom");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblFrom, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblFrom, null);
			this.l10NSharpExtender1.SetLocalizationPriority(m_lblFrom, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(m_lblFrom, "ScrReferenceFilterDlg.m_lblFrom");
			m_lblFrom.Name = "m_lblFrom";
			// 
			// m_lblTo
			// 
			resources.ApplyResources(m_lblTo, "m_lblTo");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblTo, null);
			this.l10NSharpExtender1.SetLocalizationPriority(m_lblTo, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(m_lblTo, "ScrReferenceFilterDlg.m_lblTo");
			m_lblTo.Name = "m_lblTo";
			// 
			// scrPsgFrom
			// 
			resources.ApplyResources(this.scrPsgFrom, "scrPsgFrom");
			this.scrPsgFrom.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgFrom.ErrorCaption = "From Reference";
			this.l10NSharpExtender1.SetLocalizableToolTip(this.scrPsgFrom, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.scrPsgFrom, null);
			this.l10NSharpExtender1.SetLocalizingId(this.scrPsgFrom, "ScrReferenceFilterDlg.ScrPassageControl");
			this.scrPsgFrom.Name = "scrPsgFrom";
			this.scrPsgFrom.Reference = "GEN 1:1";
			this.scrPsgFrom.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.scrPsgFrom_PassageChanged);
			// 
			// scrPsgTo
			// 
			this.scrPsgTo.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgTo.ErrorCaption = "To Reference";
			this.l10NSharpExtender1.SetLocalizableToolTip(this.scrPsgTo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.scrPsgTo, null);
			this.l10NSharpExtender1.SetLocalizingId(this.scrPsgTo, "ScrReferenceFilterDlg.ScrPassageControl");
			resources.ApplyResources(this.scrPsgTo, "scrPsgTo");
			this.scrPsgTo.Name = "scrPsgTo";
			this.scrPsgTo.Reference = "REV 22:21";
			this.scrPsgTo.PassageChanged += new SIL.Windows.Forms.Scripture.ScrPassageControl.PassageChangedHandler(this.scrPsgTo_PassageChanged);
			// 
			// btnClearFilter
			// 
			resources.ApplyResources(this.btnClearFilter, "btnClearFilter");
			this.btnClearFilter.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnClearFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnClearFilter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnClearFilter, "ScrReferenceFilterDlg.btnClearFilter");
			this.btnClearFilter.Name = "btnClearFilter";
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
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnClearFilter);
			this.Controls.Add(this.scrPsgTo);
			this.Controls.Add(this.scrPsgFrom);
			this.Controls.Add(m_lblTo);
			this.Controls.Add(m_lblFrom);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "ScrReferenceFilterDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScrReferenceFilterDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
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
	}
}