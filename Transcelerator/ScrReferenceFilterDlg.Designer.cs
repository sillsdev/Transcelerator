// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ScrReferenceFilterDlg.cs
// Responsibility: Bogle
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrReferenceFilterDlg));
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.scrPsgFrom = new SIL.ScriptureControls.ScrPassageControl();
			this.scrPsgTo = new SIL.ScriptureControls.ScrPassageControl();
			this.btnClearFilter = new System.Windows.Forms.Button();
			this.SuspendLayout();
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
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// scrPsgFrom
			// 
			resources.ApplyResources(this.scrPsgFrom, "scrPsgFrom");
			this.scrPsgFrom.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgFrom.ErrorCaption = "From Reference";
			this.scrPsgFrom.Name = "scrPsgFrom";
			this.scrPsgFrom.Reference = "GEN 1:1";
			this.scrPsgFrom.PassageChanged += new SIL.ScriptureControls.ScrPassageControl.PassageChangedHandler(this.scrPsgFrom_PassageChanged);
			// 
			// scrPsgTo
			// 
			this.scrPsgTo.BackColor = System.Drawing.SystemColors.Window;
			this.scrPsgTo.ErrorCaption = "To Reference";
			resources.ApplyResources(this.scrPsgTo, "scrPsgTo");
			this.scrPsgTo.Name = "scrPsgTo";
			this.scrPsgTo.Reference = "REV 22:21";
			this.scrPsgTo.PassageChanged += new SIL.ScriptureControls.ScrPassageControl.PassageChangedHandler(this.scrPsgTo_PassageChanged);
			// 
			// btnClearFilter
			// 
			resources.ApplyResources(this.btnClearFilter, "btnClearFilter");
			this.btnClearFilter.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClearFilter.Name = "btnClearFilter";
			this.btnClearFilter.UseVisualStyleBackColor = true;
			this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
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
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScrReferenceFilterDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private SIL.ScriptureControls.ScrPassageControl scrPsgFrom;
		private SIL.ScriptureControls.ScrPassageControl scrPsgTo;
		private System.Windows.Forms.Button btnClearFilter;
	}
}