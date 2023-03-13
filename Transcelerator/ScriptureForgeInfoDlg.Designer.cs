using L10NSharp.UI;
using L10NSharp.XLiffUtils;

namespace SIL.Transcelerator
{
	partial class ScriptureForgeInfoDlg
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
			if (disposing)
			{
				if (components != null)
					components.Dispose();
				LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptureForgeInfoDlg));
            this.m_lblExplanation = new System.Windows.Forms.Label();
            this.m_lblMoreInfoOnline = new System.Windows.Forms.Label();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_linkLabelWorkingWithScriptureForge = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_linkLabelScriptureForge = new System.Windows.Forms.LinkLabel();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            this.m_linkLabelPtxPrint = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblExplanation
            // 
            this.m_lblExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblExplanation.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblExplanation, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblExplanation, resources.GetString("m_lblExplanation.LocalizationComment"));
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblExplanation, "ScriptureForgeInfoDlg.m_lblExplanation");
            this.m_lblExplanation.Location = new System.Drawing.Point(3, 0);
            this.m_lblExplanation.Name = "m_lblExplanation";
            this.m_lblExplanation.Size = new System.Drawing.Size(442, 39);
            this.m_lblExplanation.TabIndex = 3;
            this.m_lblExplanation.Text = resources.GetString("m_lblExplanation.Text");
            // 
            // m_lblMoreInfoOnline
            // 
            this.m_lblMoreInfoOnline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblMoreInfoOnline.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblMoreInfoOnline, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblMoreInfoOnline, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblMoreInfoOnline, "ScriptureForgeInfoDlg.m_lblMoreInfoOnline");
            this.m_lblMoreInfoOnline.Location = new System.Drawing.Point(3, 42);
            this.m_lblMoreInfoOnline.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.m_lblMoreInfoOnline.Name = "m_lblMoreInfoOnline";
            this.m_lblMoreInfoOnline.Size = new System.Drawing.Size(442, 13);
            this.m_lblMoreInfoOnline.TabIndex = 4;
            this.m_lblMoreInfoOnline.Text = "More information is available on-line:";
            // 
            // m_btnOk
            // 
            this.m_btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnOk, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_btnOk, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_btnOk, "Common.OK");
            this.m_btnOk.Location = new System.Drawing.Point(200, 143);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 1;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // m_linkLabelWorkingWithScriptureForge
            // 
            this.m_linkLabelWorkingWithScriptureForge.AutoSize = true;
            this.m_linkLabelWorkingWithScriptureForge.LinkArea = new System.Windows.Forms.LinkArea(16, 44);
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelWorkingWithScriptureForge, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelWorkingWithScriptureForge, "Params are product names 0) \"Scripture Forge\"; 1) \"Transcelerator\"");
            this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelWorkingWithScriptureForge, "ScriptureForgeInfoDlg.m_linkLabelWorkingWithScriptureForge");
            this.m_linkLabelWorkingWithScriptureForge.Location = new System.Drawing.Point(3, 61);
            this.m_linkLabelWorkingWithScriptureForge.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.m_linkLabelWorkingWithScriptureForge.Name = "m_linkLabelWorkingWithScriptureForge";
            this.m_linkLabelWorkingWithScriptureForge.Size = new System.Drawing.Size(108, 17);
            this.m_linkLabelWorkingWithScriptureForge.TabIndex = 2;
            this.m_linkLabelWorkingWithScriptureForge.TabStop = true;
            this.m_linkLabelWorkingWithScriptureForge.Tag = "https://software.sil.org/transcelerator/working-with-scripture-forge/";
            this.m_linkLabelWorkingWithScriptureForge.Text = "{1}: Working With {0}";
            this.m_linkLabelWorkingWithScriptureForge.UseCompatibleTextRendering = true;
            this.m_linkLabelWorkingWithScriptureForge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.m_linkLabelPtxPrint, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_linkLabelScriptureForge, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_lblExplanation, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_linkLabelWorkingWithScriptureForge, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_lblMoreInfoOnline, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(448, 117);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // m_linkLabelScriptureForge
            // 
            this.m_linkLabelScriptureForge.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelScriptureForge, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelScriptureForge, "Param is \"Scripture Forge\" (product name)");
            this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelScriptureForge, "ScriptureForgeInfoDlg.m_linkLabelScriptureForge");
            this.m_linkLabelScriptureForge.Location = new System.Drawing.Point(3, 84);
            this.m_linkLabelScriptureForge.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.m_linkLabelScriptureForge.Name = "m_linkLabelScriptureForge";
            this.m_linkLabelScriptureForge.Size = new System.Drawing.Size(60, 13);
            this.m_linkLabelScriptureForge.TabIndex = 5;
            this.m_linkLabelScriptureForge.TabStop = true;
            this.m_linkLabelScriptureForge.Tag = "http://scriptureforge.org/";
            this.m_linkLabelScriptureForge.Text = "{0} website";
            this.m_linkLabelScriptureForge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
            this.l10NSharpExtender1.PrefixForNewItems = null;
            // 
            // m_linkLabelPtxPrint
            // 
            this.m_linkLabelPtxPrint.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkLabelPtxPrint, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_linkLabelPtxPrint, "Param (3) is \"PTXprint\" (product name)");
            this.l10NSharpExtender1.SetLocalizingId(this.m_linkLabelPtxPrint, "ScriptureForgeInfoDlg.m_linkLabelPtxPrint");
            this.m_linkLabelPtxPrint.Location = new System.Drawing.Point(3, 104);
            this.m_linkLabelPtxPrint.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.m_linkLabelPtxPrint.Name = "m_linkLabelPtxPrint";
            this.m_linkLabelPtxPrint.Size = new System.Drawing.Size(60, 13);
            this.m_linkLabelPtxPrint.TabIndex = 6;
            this.m_linkLabelPtxPrint.TabStop = true;
            this.m_linkLabelPtxPrint.Tag = "https://software.sil.org/ptxprint/";
            this.m_linkLabelPtxPrint.Text = "{3} website";
            this.m_linkLabelPtxPrint.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
            // 
            // ScriptureForgeInfoDlg
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 178);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.m_btnOk);
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, "Param is \"Scripture Forge\" (product name)");
            this.l10NSharpExtender1.SetLocalizingId(this, "ScriptureForgeInfoDlg.WindowTitle");
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(491, 217);
            this.MinimizeBox = false;
            this.Name = "ScriptureForgeInfoDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "External Application Integration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button m_btnOk;
		private System.Windows.Forms.LinkLabel m_linkLabelWorkingWithScriptureForge;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.LinkLabel m_linkLabelScriptureForge;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label m_lblExplanation;
		private System.Windows.Forms.Label m_lblMoreInfoOnline;
        private System.Windows.Forms.LinkLabel m_linkLabelPtxPrint;
    }
}