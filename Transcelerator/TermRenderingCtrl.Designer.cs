// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TermRenderingCtrl.Designer.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;

namespace SIL.Transcelerator
{
	partial class TermRenderingCtrl
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
				LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermRenderingCtrl));
			this.mnuAddRenderingC = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSetAsDefault = new System.Windows.Forms.ToolStripMenuItem();
			this.m_lblKeyTermColHead = new System.Windows.Forms.Label();
			this.headerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuAddRenderingH = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuLookUpTermH = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRefreshRenderingsH = new System.Windows.Forms.ToolStripMenuItem();
			this.m_lbRenderings = new System.Windows.Forms.ListBox();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuDeleteRendering = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuLookUpTermC = new System.Windows.Forms.ToolStripMenuItem();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.headerContextMenuStrip.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
			// 
			// mnuAddRenderingC
			// 
			this.mnuAddRenderingC.Image = global::SIL.Transcelerator.Properties.Resources._1321382935_plus;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAddRenderingC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAddRenderingC, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAddRenderingC, "TermRenderingCtrl.mnuAddRenderingC");
			this.mnuAddRenderingC.Name = "mnuAddRenderingC";
			this.mnuAddRenderingC.Size = new System.Drawing.Size(198, 22);
			this.mnuAddRenderingC.Text = "&Add rendering...";
			this.mnuAddRenderingC.Click += new System.EventHandler(this.mnuAddRendering_Click);
			// 
			// mnuSetAsDefault
			// 
			this.mnuSetAsDefault.Image = global::SIL.Transcelerator.Properties.Resources.check_circle;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuSetAsDefault, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuSetAsDefault, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuSetAsDefault, "TermRenderingCtrl.mnuSetAsDefault");
			this.mnuSetAsDefault.Name = "mnuSetAsDefault";
			this.mnuSetAsDefault.Size = new System.Drawing.Size(198, 22);
			this.mnuSetAsDefault.Text = "&Set as default rendering";
			this.mnuSetAsDefault.Click += new System.EventHandler(this.mnuSetAsDefault_Click);
			// 
			// m_lblKeyTermColHead
			// 
			this.m_lblKeyTermColHead.AutoEllipsis = true;
			this.m_lblKeyTermColHead.ContextMenuStrip = this.headerContextMenuStrip;
			this.m_lblKeyTermColHead.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_lblKeyTermColHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
			this.m_lblKeyTermColHead.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblKeyTermColHead, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblKeyTermColHead, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblKeyTermColHead, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblKeyTermColHead, "TermRenderingCtrl.m_lblKeyTermColHead");
			this.m_lblKeyTermColHead.Location = new System.Drawing.Point(0, 0);
			this.m_lblKeyTermColHead.Name = "m_lblKeyTermColHead";
			this.m_lblKeyTermColHead.Size = new System.Drawing.Size(100, 20);
			this.m_lblKeyTermColHead.TabIndex = 1;
			this.m_lblKeyTermColHead.Text = "#";
			this.m_lblKeyTermColHead.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// headerContextMenuStrip
			// 
			this.headerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddRenderingH,
            this.mnuLookUpTermH,
            this.mnuRefreshRenderingsH});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.headerContextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.headerContextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.headerContextMenuStrip, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.headerContextMenuStrip, "TermRenderingCtrl.contextMenuStrip1");
			this.headerContextMenuStrip.Name = "contextMenuStrip1";
			this.headerContextMenuStrip.Size = new System.Drawing.Size(222, 70);
			// 
			// mnuAddRenderingH
			// 
			this.mnuAddRenderingH.Image = global::SIL.Transcelerator.Properties.Resources._1321382935_plus;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAddRenderingH, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAddRenderingH, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAddRenderingH, "TermRenderingCtrl.mnuAddRenderingH");
			this.mnuAddRenderingH.Name = "mnuAddRenderingH";
			this.mnuAddRenderingH.Size = new System.Drawing.Size(221, 22);
			this.mnuAddRenderingH.Text = "&Add rendering...";
			this.mnuAddRenderingH.Click += new System.EventHandler(this.mnuAddRendering_Click);
			// 
			// mnuLookUpTermH
			// 
			this.mnuLookUpTermH.Image = global::SIL.Transcelerator.Properties.Resources._1330980033_search_button;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuLookUpTermH, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuLookUpTermH, resources.GetString("mnuLookUpTermH.LocalizationComment"));
			this.l10NSharpExtender1.SetLocalizingId(this.mnuLookUpTermH, "TermRenderingCtrl.mnuLookUpTermH");
			this.mnuLookUpTermH.Name = "mnuLookUpTermH";
			this.mnuLookUpTermH.Size = new System.Drawing.Size(221, 22);
			this.mnuLookUpTermH.Text = "Find &Term in {0}...";
			this.mnuLookUpTermH.Click += new System.EventHandler(this.LookUpTermInHostApplicaton);
			// 
			// mnuRefreshRenderingsH
			// 
			this.mnuRefreshRenderingsH.Image = global::SIL.Transcelerator.Properties.Resources.Refresh;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuRefreshRenderingsH, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuRefreshRenderingsH, resources.GetString("mnuRefreshRenderingsH.LocalizationComment"));
			this.l10NSharpExtender1.SetLocalizingId(this.mnuRefreshRenderingsH, "TermRenderingCtrl.mnuRefreshRenderingsH");
			this.mnuRefreshRenderingsH.Name = "mnuRefreshRenderingsH";
			this.mnuRefreshRenderingsH.Size = new System.Drawing.Size(221, 22);
			this.mnuRefreshRenderingsH.Text = "Refresh Renderings from {0}";
			this.mnuRefreshRenderingsH.Click += new System.EventHandler(this.mnuRefreshRenderingsH_Click);
			// 
			// m_lbRenderings
			// 
			this.m_lbRenderings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_lbRenderings.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.m_lbRenderings.FormattingEnabled = true;
			this.m_lbRenderings.IntegralHeight = false;
			this.m_lbRenderings.Location = new System.Drawing.Point(0, 20);
			this.m_lbRenderings.Name = "m_lbRenderings";
			this.m_lbRenderings.Size = new System.Drawing.Size(100, 20);
			this.m_lbRenderings.Sorted = true;
			this.m_lbRenderings.TabIndex = 2;
			this.m_lbRenderings.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.m_lbRenderings_DrawItem);
			this.m_lbRenderings.SelectedIndexChanged += new System.EventHandler(this.m_lbRenderings_SelectedIndexChanged);
			this.m_lbRenderings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_lbRenderings_MouseDown);
			this.m_lbRenderings.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_lbRenderings_MouseUp);
			this.m_lbRenderings.Resize += new System.EventHandler(this.m_lbRenderings_Resize);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSetAsDefault,
            this.mnuAddRenderingC,
            this.mnuDeleteRendering,
            toolStripSeparator1,
            this.mnuLookUpTermC});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.contextMenuStrip, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.contextMenuStrip, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.contextMenuStrip, "TermRenderingCtrl.contextMenuStrip.contextMenuStrip1");
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(199, 98);
			this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// mnuDeleteRendering
			// 
			this.mnuDeleteRendering.Image = global::SIL.Transcelerator.Properties.Resources._20130910100219692_easyicon_net_16;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuDeleteRendering, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuDeleteRendering, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuDeleteRendering, "TermRenderingCtrl.mnuDeleteRendering");
			this.mnuDeleteRendering.Name = "mnuDeleteRendering";
			this.mnuDeleteRendering.Size = new System.Drawing.Size(198, 22);
			this.mnuDeleteRendering.Text = "&Delete this rendering";
			this.mnuDeleteRendering.Click += new System.EventHandler(this.mnuDeleteRendering_Click);
			// 
			// mnuLookUpTermC
			// 
			this.mnuLookUpTermC.Image = global::SIL.Transcelerator.Properties.Resources._1330980033_search_button;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuLookUpTermC, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuLookUpTermC, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuLookUpTermC, "TermRenderingCtrl.mnuLookUpTermC");
			this.mnuLookUpTermC.Name = "mnuLookUpTermC";
			this.mnuLookUpTermC.Size = new System.Drawing.Size(198, 22);
			this.mnuLookUpTermC.Text = "Find &Term in {0}...";
			this.mnuLookUpTermC.Click += new System.EventHandler(this.LookUpTermInHostApplicaton);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = "TermRenderingCtrl";
			// 
			// TermRenderingCtrl
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.m_lbRenderings);
			this.Controls.Add(this.m_lblKeyTermColHead);
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "TermRenderingCtrl");
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MinimumSize = new System.Drawing.Size(100, 40);
			this.Name = "TermRenderingCtrl";
			this.Size = new System.Drawing.Size(100, 40);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TermRenderingCtrl_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TermRenderingCtrl_DragEnter);
			this.headerContextMenuStrip.ResumeLayout(false);
			this.contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label m_lblKeyTermColHead;
		private System.Windows.Forms.ListBox m_lbRenderings;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem mnuDeleteRendering;
		private System.Windows.Forms.ToolStripMenuItem mnuLookUpTermC;
		private System.Windows.Forms.ContextMenuStrip headerContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem mnuLookUpTermH;
        private System.Windows.Forms.ToolStripMenuItem mnuAddRenderingH;
        private System.Windows.Forms.ToolStripMenuItem mnuRefreshRenderingsH;
        private System.Windows.Forms.ToolStripMenuItem mnuSetAsDefault;
		private L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.ToolStripMenuItem mnuAddRenderingC;
	}
}
