namespace SIL.Transcelerator
{
	partial class VariantQuestionInfoDlg
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
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.chkDoNotShowAgain = new System.Windows.Forms.CheckBox();
			this.lblVariantQuestionInfo = new System.Windows.Forms.Label();
			this.pictInfoIcon = new System.Windows.Forms.PictureBox();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.btnOk = new System.Windows.Forms.Button();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictInfoIcon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.chkDoNotShowAgain, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.lblVariantQuestionInfo, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.pictInfoIcon, 0, 0);
			this.tableLayoutPanel.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 3;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(454, 83);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// chkDoNotShowAgain
			// 
			this.chkDoNotShowAgain.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.chkDoNotShowAgain, 2);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.chkDoNotShowAgain, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.chkDoNotShowAgain, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.chkDoNotShowAgain, L10NSharp.LocalizationPriority.Medium);
			this.l10NSharpExtender1.SetLocalizingId(this.chkDoNotShowAgain, "VariantQuestionInfoDlg.chkDoNotShowAgain");
			this.chkDoNotShowAgain.Location = new System.Drawing.Point(3, 63);
			this.chkDoNotShowAgain.Name = "chkDoNotShowAgain";
			this.chkDoNotShowAgain.Size = new System.Drawing.Size(249, 17);
			this.chkDoNotShowAgain.TabIndex = 1;
			this.chkDoNotShowAgain.Text = "Do not show messages about question variants";
			this.chkDoNotShowAgain.UseVisualStyleBackColor = true;
			// 
			// lblVariantQuestionInfo
			// 
			this.lblVariantQuestionInfo.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblVariantQuestionInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblVariantQuestionInfo, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.lblVariantQuestionInfo, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.lblVariantQuestionInfo, "GroupedQuestionInfoDlg.lblVariantQuestionInfo");
			this.lblVariantQuestionInfo.Location = new System.Drawing.Point(13, 5);
			this.lblVariantQuestionInfo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
			this.lblVariantQuestionInfo.Name = "lblVariantQuestionInfo";
			this.lblVariantQuestionInfo.Size = new System.Drawing.Size(14, 13);
			this.lblVariantQuestionInfo.TabIndex = 0;
			this.lblVariantQuestionInfo.Text = "#";
			// 
			// pictInfoIcon
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pictInfoIcon, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pictInfoIcon, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.pictInfoIcon, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.pictInfoIcon, "GroupedQuestionInfoDlg.pictInfoIcon");
			this.pictInfoIcon.Location = new System.Drawing.Point(3, 3);
			this.pictInfoIcon.MinimumSize = new System.Drawing.Size(4, 4);
			this.pictInfoIcon.Name = "pictInfoIcon";
			this.pictInfoIcon.Size = new System.Drawing.Size(4, 4);
			this.pictInfoIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictInfoIcon.TabIndex = 3;
			this.pictInfoIcon.TabStop = false;
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(202, 111);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// VariantQuestionInfoDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(478, 145);
			this.ControlBox = false;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tableLayoutPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "VariantQuestionInfoDlg.WindowTitle");
			this.Name = "VariantQuestionInfoDlg";
			this.Padding = new System.Windows.Forms.Padding(12);
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Variant Question";
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictInfoIcon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label lblVariantQuestionInfo;
		private System.Windows.Forms.CheckBox chkDoNotShowAgain;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.PictureBox pictInfoIcon;
		private System.Windows.Forms.Button btnOk;
	}
}