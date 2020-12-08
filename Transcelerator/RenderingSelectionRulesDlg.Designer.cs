// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: RenderingSelectionRulesDlg.Designer.cs
// ---------------------------------------------------------------------------------------------
using L10NSharp.UI;

namespace SIL.Transcelerator
{
	partial class RenderingSelectionRulesDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderingSelectionRulesDlg));
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblInstructions = new System.Windows.Forms.Label();
			this.m_listRules = new System.Windows.Forms.CheckedListBox();
			this.lblRuleDescription = new System.Windows.Forms.Label();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnNew = new System.Windows.Forms.ToolStripButton();
			this.btnEdit = new System.Windows.Forms.ToolStripButton();
			this.btnCopy = new System.Windows.Forms.ToolStripButton();
			this.btnDelete = new System.Windows.Forms.ToolStripButton();
			this.m_lblDescription = new System.Windows.Forms.Label();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(188, 368);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
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
			this.btnCancel.Location = new System.Drawing.Point(269, 368);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblInstructions
			// 
			this.lblInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblInstructions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblInstructions, "RenderingSelectionRulesDlg.lblInstructions");
			this.lblInstructions.Location = new System.Drawing.Point(12, 9);
			this.lblInstructions.Name = "lblInstructions";
			this.lblInstructions.Size = new System.Drawing.Size(509, 37);
			this.lblInstructions.TabIndex = 2;
			this.lblInstructions.Text = "Define rules to enable Transcelerator to more successfully choose the best render" +
    "ing for biblical terms. To disable a rule, clear its check box.";
			// 
			// m_listRules
			// 
			this.m_listRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_listRules.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_listRules, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_listRules, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_listRules, "RenderingSelectionRulesDlg.m_listRules");
			this.m_listRules.Location = new System.Drawing.Point(12, 74);
			this.m_listRules.Name = "m_listRules";
			this.m_listRules.Size = new System.Drawing.Size(509, 154);
			this.m_listRules.TabIndex = 3;
			this.m_listRules.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.m_listRules_ItemCheck);
			this.m_listRules.SelectedIndexChanged += new System.EventHandler(this.m_listRules_SelectedIndexChanged);
			// 
			// lblRuleDescription
			// 
			this.lblRuleDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblRuleDescription.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblRuleDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblRuleDescription, "");
			this.l10NSharpExtender1.SetLocalizingId(this.lblRuleDescription, "RenderingSelectionRulesDlg.lblRuleDescription");
			this.lblRuleDescription.Location = new System.Drawing.Point(12, 266);
			this.lblRuleDescription.Name = "lblRuleDescription";
			this.lblRuleDescription.Size = new System.Drawing.Size(86, 13);
			this.lblRuleDescription.TabIndex = 4;
			this.lblRuleDescription.Text = "Rule description:";
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnEdit,
            this.btnCopy,
            this.btnDelete});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.toolStrip1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStrip1, "RenderingSelectionRulesDlg.toolStrip1");
			this.toolStrip1.Location = new System.Drawing.Point(12, 46);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(284, 25);
			this.toolStrip1.TabIndex = 9;
			// 
			// btnNew
			// 
			this.btnNew.Image = global::SIL.Transcelerator.Properties.Resources._1321382935_plus;
			this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnNew, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnNew, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnNew, "RenderingSelectionRulesDlg.btnNew");
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(86, 22);
			this.btnNew.Text = "New Rule...";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
			this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnEdit, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnEdit, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnEdit, "RenderingSelectionRulesDlg.btnEdit");
			this.btnEdit.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(56, 22);
			this.btnEdit.Text = "Edit...";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnCopy
			// 
			this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
			this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCopy, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCopy, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCopy, "RenderingSelectionRulesDlg.btnCopy");
			this.btnCopy.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(64, 22);
			this.btnCopy.Text = "Copy...";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Image = global::SIL.Transcelerator.Properties.Resources._20130910100219692_easyicon_net_16;
			this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnDelete, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnDelete, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnDelete, "RenderingSelectionRulesDlg.btnDelete");
			this.btnDelete.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(60, 22);
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// m_lblDescription
			// 
			this.m_lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblDescription.BackColor = System.Drawing.SystemColors.Window;
			this.m_lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_lblDescription.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblDescription, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblDescription, "RenderingSelectionRulesDlg.m_lblDescription");
			this.m_lblDescription.Location = new System.Drawing.Point(12, 285);
			this.m_lblDescription.Name = "m_lblDescription";
			this.m_lblDescription.Size = new System.Drawing.Size(509, 69);
			this.m_lblDescription.TabIndex = 10;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// RenderingSelectionRulesDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(533, 403);
			this.Controls.Add(this.m_lblDescription);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.lblRuleDescription);
			this.Controls.Add(this.m_listRules);
			this.Controls.Add(this.lblInstructions);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RenderingSelectionRulesDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RenderingSelectionRulesDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Biblical Terms Rendering Selection Rules";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblInstructions;
		private System.Windows.Forms.CheckedListBox m_listRules;
		private System.Windows.Forms.Label lblRuleDescription;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btnNew;
		private System.Windows.Forms.ToolStripButton btnCopy;
		private System.Windows.Forms.ToolStripButton btnDelete;
		private System.Windows.Forms.ToolStripButton btnEdit;
		private System.Windows.Forms.Label m_lblDescription;
		private L10NSharpExtender l10NSharpExtender1;
	}
}