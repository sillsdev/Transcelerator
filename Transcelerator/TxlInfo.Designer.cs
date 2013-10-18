// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TxlInfo.cs
// Responsibility: bogle
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
			System.Windows.Forms.Label lblProductName;
			this.m_lblAppVersion = new System.Windows.Forms.Label();
			this.m_lblCopyright = new System.Windows.Forms.Label();
			this.m_picLogo = new System.Windows.Forms.PictureBox();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.pictLogo = new System.Windows.Forms.PictureBox();
			this.lblBuildDate = new System.Windows.Forms.Label();
			this.panelBrowser = new System.Windows.Forms.Panel();
			lblProductName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_picLogo)).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// lblProductName
			// 
			lblProductName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			lblProductName.AutoSize = true;
			lblProductName.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel.SetColumnSpan(lblProductName, 2);
			lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lblProductName.ForeColor = System.Drawing.Color.Black;
			lblProductName.Location = new System.Drawing.Point(386, 0);
			lblProductName.Name = "lblProductName";
			lblProductName.Padding = new System.Windows.Forms.Padding(0, 14, 0, 10);
			lblProductName.Size = new System.Drawing.Size(282, 66);
			lblProductName.TabIndex = 21;
			lblProductName.Text = "Transcelerator";
			lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			lblProductName.UseMnemonic = false;
			// 
			// m_lblAppVersion
			// 
			this.m_lblAppVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblAppVersion.AutoSize = true;
			this.m_lblAppVersion.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel.SetColumnSpan(this.m_lblAppVersion, 2);
			this.m_lblAppVersion.Location = new System.Drawing.Point(386, 66);
			this.m_lblAppVersion.Name = "m_lblAppVersion";
			this.m_lblAppVersion.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.m_lblAppVersion.Size = new System.Drawing.Size(282, 19);
			this.m_lblAppVersion.TabIndex = 19;
			this.m_lblAppVersion.Text = "Version {0} (Release Candidate)";
			this.m_lblAppVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_lblAppVersion.UseMnemonic = false;
			// 
			// m_lblCopyright
			// 
			this.m_lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblCopyright.AutoSize = true;
			this.m_lblCopyright.BackColor = System.Drawing.Color.Transparent;
			this.m_lblCopyright.ForeColor = System.Drawing.Color.Black;
			this.m_lblCopyright.Location = new System.Drawing.Point(386, 105);
			this.m_lblCopyright.Name = "m_lblCopyright";
			this.m_lblCopyright.Size = new System.Drawing.Size(183, 109);
			this.m_lblCopyright.TabIndex = 20;
			this.m_lblCopyright.Text = "(C) 2011-2013, SIL International.";
			this.m_lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_picLogo
			// 
			this.m_picLogo.Image = global::SIL.Transcelerator.Properties.Resources.Transcelerator;
			this.m_picLogo.Location = new System.Drawing.Point(3, 3);
			this.m_picLogo.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
			this.m_picLogo.Name = "m_picLogo";
			this.tableLayoutPanel.SetRowSpan(this.m_picLogo, 5);
			this.m_picLogo.Size = new System.Drawing.Size(365, 362);
			this.m_picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.m_picLogo.TabIndex = 18;
			this.m_picLogo.TabStop = false;
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.Controls.Add(lblProductName, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.m_picLogo, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.m_lblAppVersion, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.m_lblCopyright, 1, 3);
			this.tableLayoutPanel.Controls.Add(this.pictLogo, 2, 3);
			this.tableLayoutPanel.Controls.Add(this.lblBuildDate, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.panelBrowser, 1, 4);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(671, 368);
			this.tableLayoutPanel.TabIndex = 27;
			// 
			// pictLogo
			// 
			this.pictLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.pictLogo.Image = global::SIL.Transcelerator.Properties.Resources.FWSILLogoColor2005;
			this.pictLogo.InitialImage = global::SIL.Transcelerator.Properties.Resources.FWSILLogoColor2005;
			this.pictLogo.Location = new System.Drawing.Point(575, 108);
			this.pictLogo.Name = "pictLogo";
			this.pictLogo.Size = new System.Drawing.Size(93, 103);
			this.pictLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictLogo.TabIndex = 27;
			this.pictLogo.TabStop = false;
			// 
			// lblBuildDate
			// 
			this.lblBuildDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblBuildDate.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.lblBuildDate, 2);
			this.lblBuildDate.Location = new System.Drawing.Point(386, 85);
			this.lblBuildDate.Name = "lblBuildDate";
			this.lblBuildDate.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.lblBuildDate.Size = new System.Drawing.Size(282, 19);
			this.lblBuildDate.TabIndex = 28;
			this.lblBuildDate.Text = "Built on: {0}";
			this.lblBuildDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// panelBrowser
			// 
			this.panelBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.SetColumnSpan(this.panelBrowser, 2);
			this.panelBrowser.Location = new System.Drawing.Point(386, 223);
			this.panelBrowser.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
			this.panelBrowser.Name = "panelBrowser";
			this.panelBrowser.Size = new System.Drawing.Size(282, 142);
			this.panelBrowser.TabIndex = 29;
			// 
			// TxlInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "TxlInfo";
			this.Size = new System.Drawing.Size(671, 368);
			((System.ComponentModel.ISupportInitialize)(this.m_picLogo)).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictLogo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox m_picLogo;
		private System.Windows.Forms.Label m_lblCopyright;
		private System.Windows.Forms.Label m_lblAppVersion;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.PictureBox pictLogo;
		private System.Windows.Forms.Label lblBuildDate;
		private System.Windows.Forms.Panel panelBrowser;
	}
}
