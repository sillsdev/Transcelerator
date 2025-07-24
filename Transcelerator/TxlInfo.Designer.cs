// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2012' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TxlInfo.Designer.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace SIL.Transcelerator
{
	partial class TxlInfo
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
            this._webBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.m_tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.m_tableLayoutPanelFallback = new System.Windows.Forms.TableLayoutPanel();
            this.m_lblWebView2ErrorResolution = new System.Windows.Forms.Label();
            this.m_lblProduct = new System.Windows.Forms.Label();
            this.m_lblAppVersion = new System.Windows.Forms.Label();
            this.m_lblBuildDate = new System.Windows.Forms.Label();
            this.m_lblCopyrightAndLicense = new System.Windows.Forms.Label();
            this.m_linkWebView2Runtime = new System.Windows.Forms.LinkLabel();
            this.m_lblWebView2Problem = new System.Windows.Forms.Label();
            this.m_btnDetails = new System.Windows.Forms.Button();
            this.m_txtExceptionDetails = new System.Windows.Forms.TextBox();
            this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._webBrowser)).BeginInit();
            this.m_tableLayoutPanelMain.SuspendLayout();
            this.m_tableLayoutPanelFallback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
            this.SuspendLayout();
            // 
            // _webBrowser
            // 
            this._webBrowser.AllowExternalDrop = false;
            this._webBrowser.CreationProperties = null;
            this._webBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
            this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l10NSharpExtender1.SetLocalizableToolTip(this._webBrowser, null);
            this.l10NSharpExtender1.SetLocalizationComment(this._webBrowser, null);
            this.l10NSharpExtender1.SetLocalizingId(this._webBrowser, "TxlInfo._webBrowser");
            this._webBrowser.Location = new System.Drawing.Point(3, 3);
            this._webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(665, 44);
            this._webBrowser.TabIndex = 0;
            this._webBrowser.ZoomFactor = 1D;
            // 
            // m_tableLayoutPanelMain
            // 
            this.m_tableLayoutPanelMain.ColumnCount = 1;
            this.m_tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_tableLayoutPanelMain.Controls.Add(this.m_tableLayoutPanelFallback, 0, 1);
            this.m_tableLayoutPanelMain.Controls.Add(this._webBrowser, 0, 0);
            this.m_tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.m_tableLayoutPanelMain.Name = "m_tableLayoutPanelMain";
            this.m_tableLayoutPanelMain.RowCount = 2;
            this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelMain.Size = new System.Drawing.Size(671, 368);
            this.m_tableLayoutPanelMain.TabIndex = 1;
            // 
            // m_tableLayoutPanelFallback
            // 
            this.m_tableLayoutPanelFallback.ColumnCount = 1;
            this.m_tableLayoutPanelFallback.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblWebView2ErrorResolution, 0, 5);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblProduct, 0, 0);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblAppVersion, 0, 1);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblBuildDate, 0, 2);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblCopyrightAndLicense, 0, 3);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_linkWebView2Runtime, 0, 8);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_lblWebView2Problem, 0, 4);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_btnDetails, 0, 6);
            this.m_tableLayoutPanelFallback.Controls.Add(this.m_txtExceptionDetails, 0, 7);
            this.m_tableLayoutPanelFallback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tableLayoutPanelFallback.Location = new System.Drawing.Point(3, 53);
            this.m_tableLayoutPanelFallback.Name = "m_tableLayoutPanelFallback";
            this.m_tableLayoutPanelFallback.RowCount = 9;
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_tableLayoutPanelFallback.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableLayoutPanelFallback.Size = new System.Drawing.Size(665, 312);
            this.m_tableLayoutPanelFallback.TabIndex = 0;
            this.m_tableLayoutPanelFallback.Visible = false;
            // 
            // m_lblWebView2ErrorResolution
            // 
            this.m_lblWebView2ErrorResolution.AutoSize = true;
            this.m_lblWebView2ErrorResolution.ForeColor = System.Drawing.Color.Red;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblWebView2ErrorResolution, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblWebView2ErrorResolution, "Param 0: \"WebView2 Runtime\"; Param 1: email address");
            this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblWebView2ErrorResolution, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblWebView2ErrorResolution, "TxlInfo.m_lblWebView2ErrorResolution");
            this.m_lblWebView2ErrorResolution.Location = new System.Drawing.Point(3, 95);
            this.m_lblWebView2ErrorResolution.Name = "m_lblWebView2ErrorResolution";
            this.m_lblWebView2ErrorResolution.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.m_lblWebView2ErrorResolution.Size = new System.Drawing.Size(655, 36);
            this.m_lblWebView2ErrorResolution.TabIndex = 6;
            this.m_lblWebView2ErrorResolution.Text = "Please install the {0} (Evergreen) by downloading it from the website below. If t" +
    "his does not resolve the issue, please report this problem to {1}.";
            this.m_lblWebView2ErrorResolution.Visible = false;
            // 
            // m_lblProduct
            // 
            this.m_lblProduct.AutoSize = true;
            this.m_lblProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblProduct, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblProduct, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblProduct, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblProduct, "TxlInfo._lblProduct");
            this.m_lblProduct.Location = new System.Drawing.Point(3, 0);
            this.m_lblProduct.Name = "m_lblProduct";
            this.m_lblProduct.Size = new System.Drawing.Size(659, 33);
            this.m_lblProduct.TabIndex = 0;
            this.m_lblProduct.Text = "Transcelerator";
            this.m_lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lblAppVersion
            // 
            this.m_lblAppVersion.AutoSize = true;
            this.m_lblAppVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAppVersion, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAppVersion, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblAppVersion, "TxlInfo.m_lblAppVersion");
            this.m_lblAppVersion.Location = new System.Drawing.Point(3, 33);
            this.m_lblAppVersion.Name = "m_lblAppVersion";
            this.m_lblAppVersion.Size = new System.Drawing.Size(659, 13);
            this.m_lblAppVersion.TabIndex = 1;
            this.m_lblAppVersion.Text = "Version {0}";
            this.m_lblAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lblBuildDate
            // 
            this.m_lblBuildDate.AutoSize = true;
            this.m_lblBuildDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblBuildDate, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblBuildDate, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblBuildDate, "TxlInfo.lblBuildDate");
            this.m_lblBuildDate.Location = new System.Drawing.Point(3, 46);
            this.m_lblBuildDate.Name = "m_lblBuildDate";
            this.m_lblBuildDate.Size = new System.Drawing.Size(659, 13);
            this.m_lblBuildDate.TabIndex = 2;
            this.m_lblBuildDate.Text = "Built on: {0}";
            this.m_lblBuildDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lblCopyrightAndLicense
            // 
            this.m_lblCopyrightAndLicense.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCopyrightAndLicense, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCopyrightAndLicense, "Param is copyright information. This is displayed in the Help/About box and the s" +
        "plash screen");
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblCopyrightAndLicense, "TransceleratorInfo.CopyrightFmt");
            this.m_lblCopyrightAndLicense.Location = new System.Drawing.Point(3, 59);
            this.m_lblCopyrightAndLicense.Name = "m_lblCopyrightAndLicense";
            this.m_lblCopyrightAndLicense.Size = new System.Drawing.Size(256, 13);
            this.m_lblCopyrightAndLicense.TabIndex = 3;
            this.m_lblCopyrightAndLicense.Text = "{0}. Distributable under the terms of the MIT License.";
            this.m_lblCopyrightAndLicense.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // m_linkWebView2Runtime
            // 
            this.m_linkWebView2Runtime.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_linkWebView2Runtime, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_linkWebView2Runtime, null);
            this.l10NSharpExtender1.SetLocalizationPriority(this.m_linkWebView2Runtime, L10NSharp.LocalizationPriority.NotLocalizable);
            this.l10NSharpExtender1.SetLocalizingId(this.m_linkWebView2Runtime, "TxlInfo._linkWebView2Runtime");
            this.m_linkWebView2Runtime.Location = new System.Drawing.Point(3, 299);
            this.m_linkWebView2Runtime.Name = "m_linkWebView2Runtime";
            this.m_linkWebView2Runtime.Size = new System.Drawing.Size(280, 13);
            this.m_linkWebView2Runtime.TabIndex = 4;
            this.m_linkWebView2Runtime.TabStop = true;
            this.m_linkWebView2Runtime.Text = "developer.microsoft.com/en-us/microsoft-edge/webview2";
            this.m_linkWebView2Runtime.Visible = false;
            this.m_linkWebView2Runtime.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkWebView2Runtime_LinkClicked);
            // 
            // m_lblWebView2Problem
            // 
            this.m_lblWebView2Problem.AutoSize = true;
            this.m_lblWebView2Problem.ForeColor = System.Drawing.Color.Red;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblWebView2Problem, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_lblWebView2Problem, "Param is \"WebView2 Runtime\"");
            this.l10NSharpExtender1.SetLocalizingId(this.m_lblWebView2Problem, "TxlInfo.m_lblWebView2Problem");
            this.m_lblWebView2Problem.Location = new System.Drawing.Point(3, 72);
            this.m_lblWebView2Problem.Name = "m_lblWebView2Problem";
            this.m_lblWebView2Problem.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.m_lblWebView2Problem.Size = new System.Drawing.Size(326, 23);
            this.m_lblWebView2Problem.TabIndex = 5;
            this.m_lblWebView2Problem.Text = "The {0} (used to render the above content properly) is not available.";
            // 
            // m_btnDetails
            // 
            this.m_btnDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnDetails.AutoSize = true;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_btnDetails, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_btnDetails, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_btnDetails, "TxlInfo.m_btnDetails.Show");
            this.m_btnDetails.Location = new System.Drawing.Point(566, 134);
            this.m_btnDetails.Name = "m_btnDetails";
            this.m_btnDetails.Size = new System.Drawing.Size(96, 23);
            this.m_btnDetails.TabIndex = 7;
            this.m_btnDetails.Text = "Show Details";
            this.m_btnDetails.UseVisualStyleBackColor = true;
            this.m_btnDetails.Visible = false;
            this.m_btnDetails.Click += new System.EventHandler(this.m_btnDetails_Click);
            // 
            // m_txtExceptionDetails
            // 
            this.m_txtExceptionDetails.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.m_txtExceptionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_txtExceptionDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtExceptionDetails, null);
            this.l10NSharpExtender1.SetLocalizationComment(this.m_txtExceptionDetails, null);
            this.l10NSharpExtender1.SetLocalizingId(this.m_txtExceptionDetails, "textBox1");
            this.m_txtExceptionDetails.Location = new System.Drawing.Point(10, 170);
            this.m_txtExceptionDetails.Margin = new System.Windows.Forms.Padding(10);
            this.m_txtExceptionDetails.Multiline = true;
            this.m_txtExceptionDetails.Name = "m_txtExceptionDetails";
            this.m_txtExceptionDetails.ReadOnly = true;
            this.m_txtExceptionDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_txtExceptionDetails.Size = new System.Drawing.Size(645, 119);
            this.m_txtExceptionDetails.TabIndex = 8;
            this.m_txtExceptionDetails.Visible = false;
            // 
            // l10NSharpExtender1
            // 
            this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
            this.l10NSharpExtender1.PrefixForNewItems = null;
            // 
            // TxlInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.m_tableLayoutPanelMain);
            this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
            this.l10NSharpExtender1.SetLocalizationComment(this, null);
            this.l10NSharpExtender1.SetLocalizingId(this, "TxlInfo.TxlInfo");
            this.Name = "TxlInfo";
            this.Size = new System.Drawing.Size(671, 368);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this._webBrowser)).EndInit();
            this.m_tableLayoutPanelMain.ResumeLayout(false);
            this.m_tableLayoutPanelFallback.ResumeLayout(false);
            this.m_tableLayoutPanelFallback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 _webBrowser;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelMain;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelFallback;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label m_lblProduct;
		private System.Windows.Forms.Label m_lblAppVersion;
		private System.Windows.Forms.Label m_lblBuildDate;
		private System.Windows.Forms.Label m_lblCopyrightAndLicense;
		private System.Windows.Forms.Label m_lblWebView2Problem;
		public System.Windows.Forms.LinkLabel m_linkWebView2Runtime;
		private System.Windows.Forms.Label m_lblWebView2ErrorResolution;
		private System.Windows.Forms.Button m_btnDetails;
		private System.Windows.Forms.TextBox m_txtExceptionDetails;
	}
}
