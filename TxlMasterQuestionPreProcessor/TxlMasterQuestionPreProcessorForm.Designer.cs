namespace SIL.TxlMasterQuestionPreProcessor
{
    partial class TxlMasterQuestionPreProcessorForm
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
            this.btnTextToSfm = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtXmlQuestionFile = new System.Windows.Forms.TextBox();
            this.chkWriteTempFile = new System.Windows.Forms.CheckBox();
            this.rdoSfmToXml = new System.Windows.Forms.RadioButton();
            this.rdoLocalization = new System.Windows.Forms.RadioButton();
            this.txtLocale = new System.Windows.Forms.TextBox();
            this.lblLocale = new System.Windows.Forms.Label();
            this.btnNavigateToSourceFile = new System.Windows.Forms.Button();
            this.m_chkRetainOnlyTranslated = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblRegexFilterForLocIds = new System.Windows.Forms.Label();
            this.txtRegexToMatchRefs = new System.Windows.Forms.TextBox();
            this.lblOverwrite = new System.Windows.Forms.Label();
            this.cboOverwrite = new System.Windows.Forms.ComboBox();
            this.chkMarkApproved = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTextToSfm
            // 
            this.btnTextToSfm.Location = new System.Drawing.Point(11, 60);
            this.btnTextToSfm.Name = "btnTextToSfm";
            this.btnTextToSfm.Size = new System.Drawing.Size(96, 23);
            this.btnTextToSfm.TabIndex = 0;
            this.btnTextToSfm.Text = "Text to SFM...";
            this.btnTextToSfm.UseVisualStyleBackColor = true;
            this.btnTextToSfm.Click += new System.EventHandler(this.btnTextToSfm_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnGenerate.AutoSize = true;
            this.btnGenerate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelMain.SetColumnSpan(this.btnGenerate, 2);
            this.btnGenerate.Location = new System.Drawing.Point(438, 283);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.btnGenerate.Size = new System.Drawing.Size(86, 31);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate XML";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.SetColumnSpan(this.txtSourceFile, 3);
            this.txtSourceFile.Location = new System.Drawing.Point(11, 110);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(476, 20);
            this.txtSourceFile.TabIndex = 2;
            this.txtSourceFile.TextChanged += new System.EventHandler(this.HandleSourceFileChanged);
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.lblSource, 4);
            this.lblSource.Location = new System.Drawing.Point(11, 86);
            this.lblSource.Name = "lblSource";
            this.lblSource.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblSource.Size = new System.Drawing.Size(189, 21);
            this.lblSource.TabIndex = 3;
            this.lblSource.Text = "Source Standard Format Question File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.label2, 4);
            this.label2.Location = new System.Drawing.Point(11, 199);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.label2.Size = new System.Drawing.Size(131, 21);
            this.label2.TabIndex = 5;
            this.label2.Text = "Master XML Question File:";
            // 
            // txtXmlQuestionFile
            // 
            this.txtXmlQuestionFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.SetColumnSpan(this.txtXmlQuestionFile, 3);
            this.txtXmlQuestionFile.Location = new System.Drawing.Point(11, 223);
            this.txtXmlQuestionFile.Name = "txtXmlQuestionFile";
            this.txtXmlQuestionFile.Size = new System.Drawing.Size(476, 20);
            this.txtXmlQuestionFile.TabIndex = 4;
            this.txtXmlQuestionFile.TextChanged += new System.EventHandler(this.HandleSourceFileChanged);
            // 
            // chkWriteTempFile
            // 
            this.chkWriteTempFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkWriteTempFile.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.chkWriteTempFile, 4);
            this.chkWriteTempFile.Location = new System.Drawing.Point(11, 249);
            this.chkWriteTempFile.Name = "chkWriteTempFile";
            this.chkWriteTempFile.Size = new System.Drawing.Size(302, 17);
            this.chkWriteTempFile.TabIndex = 6;
            this.chkWriteTempFile.Text = "Write results to temp file instead of appending to master file";
            this.chkWriteTempFile.UseVisualStyleBackColor = true;
            this.chkWriteTempFile.CheckedChanged += new System.EventHandler(this.HandleSourceFileChanged);
            // 
            // rdoSfmToXml
            // 
            this.rdoSfmToXml.AutoSize = true;
            this.rdoSfmToXml.Checked = true;
            this.rdoSfmToXml.Location = new System.Drawing.Point(11, 11);
            this.rdoSfmToXml.Name = "rdoSfmToXml";
            this.rdoSfmToXml.Size = new System.Drawing.Size(84, 17);
            this.rdoSfmToXml.TabIndex = 7;
            this.rdoSfmToXml.TabStop = true;
            this.rdoSfmToXml.Text = "SFM to XML";
            this.rdoSfmToXml.UseVisualStyleBackColor = true;
            this.rdoSfmToXml.CheckedChanged += new System.EventHandler(this.HandleOptionChanged);
            // 
            // rdoLocalization
            // 
            this.rdoLocalization.AutoSize = true;
            this.rdoLocalization.Location = new System.Drawing.Point(11, 34);
            this.rdoLocalization.Name = "rdoLocalization";
            this.rdoLocalization.Size = new System.Drawing.Size(155, 17);
            this.rdoLocalization.TabIndex = 8;
            this.rdoLocalization.Text = "Create/Update Localization";
            this.rdoLocalization.UseVisualStyleBackColor = true;
            this.rdoLocalization.CheckedChanged += new System.EventHandler(this.HandleOptionChanged);
            // 
            // txtLocale
            // 
            this.txtLocale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.SetColumnSpan(this.txtLocale, 2);
            this.txtLocale.Enabled = false;
            this.txtLocale.Location = new System.Drawing.Point(404, 34);
            this.txtLocale.Name = "txtLocale";
            this.txtLocale.Size = new System.Drawing.Size(120, 20);
            this.txtLocale.TabIndex = 9;
            this.txtLocale.TextChanged += new System.EventHandler(this.HandleSourceFileChanged);
            // 
            // lblLocale
            // 
            this.lblLocale.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLocale.AutoSize = true;
            this.lblLocale.Enabled = false;
            this.lblLocale.Location = new System.Drawing.Point(311, 37);
            this.lblLocale.Name = "lblLocale";
            this.lblLocale.Size = new System.Drawing.Size(87, 13);
            this.lblLocale.TabIndex = 10;
            this.lblLocale.Text = "Locale (BCP-47):";
            this.lblLocale.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnNavigateToSourceFile
            // 
            this.btnNavigateToSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNavigateToSourceFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnNavigateToSourceFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavigateToSourceFile.Image = global::SIL.TxlMasterQuestionPreProcessor.Properties.Resources.ellipsis;
            this.btnNavigateToSourceFile.Location = new System.Drawing.Point(493, 110);
            this.btnNavigateToSourceFile.Name = "btnNavigateToSourceFile";
            this.btnNavigateToSourceFile.Size = new System.Drawing.Size(31, 22);
            this.btnNavigateToSourceFile.TabIndex = 11;
            this.btnNavigateToSourceFile.Tag = "...";
            this.btnNavigateToSourceFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNavigateToSourceFile.UseVisualStyleBackColor = true;
            this.btnNavigateToSourceFile.Click += new System.EventHandler(this.btnNavigateToSourceFile_Click);
            // 
            // m_chkRetainOnlyTranslated
            // 
            this.m_chkRetainOnlyTranslated.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.m_chkRetainOnlyTranslated.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.m_chkRetainOnlyTranslated, 3);
            this.m_chkRetainOnlyTranslated.Location = new System.Drawing.Point(277, 63);
            this.m_chkRetainOnlyTranslated.Name = "m_chkRetainOnlyTranslated";
            this.m_chkRetainOnlyTranslated.Size = new System.Drawing.Size(161, 17);
            this.m_chkRetainOnlyTranslated.TabIndex = 12;
            this.m_chkRetainOnlyTranslated.Text = "Retain only translated strings";
            this.m_chkRetainOnlyTranslated.UseVisualStyleBackColor = true;
            this.m_chkRetainOnlyTranslated.Visible = false;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 4;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.Controls.Add(this.rdoLocalization, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.btnGenerate, 2, 12);
            this.tableLayoutPanelMain.Controls.Add(this.chkWriteTempFile, 0, 10);
            this.tableLayoutPanelMain.Controls.Add(this.btnNavigateToSourceFile, 3, 4);
            this.tableLayoutPanelMain.Controls.Add(this.txtXmlQuestionFile, 0, 9);
            this.tableLayoutPanelMain.Controls.Add(this.label2, 0, 8);
            this.tableLayoutPanelMain.Controls.Add(this.m_chkRetainOnlyTranslated, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.btnTextToSfm, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.lblSource, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.txtLocale, 2, 1);
            this.tableLayoutPanelMain.Controls.Add(this.lblLocale, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.txtSourceFile, 0, 4);
            this.tableLayoutPanelMain.Controls.Add(this.rdoSfmToXml, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.lblRegexFilterForLocIds, 0, 5);
            this.tableLayoutPanelMain.Controls.Add(this.txtRegexToMatchRefs, 1, 5);
            this.tableLayoutPanelMain.Controls.Add(this.lblOverwrite, 0, 6);
            this.tableLayoutPanelMain.Controls.Add(this.cboOverwrite, 1, 6);
            this.tableLayoutPanelMain.Controls.Add(this.chkMarkApproved, 2, 6);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(8);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.Padding = new System.Windows.Forms.Padding(8);
            this.tableLayoutPanelMain.RowCount = 13;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(535, 325);
            this.tableLayoutPanelMain.TabIndex = 13;
            // 
            // lblRegexFilterForLocIds
            // 
            this.lblRegexFilterForLocIds.AutoSize = true;
            this.lblRegexFilterForLocIds.Location = new System.Drawing.Point(11, 135);
            this.lblRegexFilterForLocIds.Name = "lblRegexFilterForLocIds";
            this.lblRegexFilterForLocIds.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblRegexFilterForLocIds.Size = new System.Drawing.Size(187, 17);
            this.lblRegexFilterForLocIds.TabIndex = 13;
            this.lblRegexFilterForLocIds.Text = "Regex to match references to include:";
            this.lblRegexFilterForLocIds.Visible = false;
            // 
            // txtRegexToMatchRefs
            // 
            this.txtRegexToMatchRefs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.SetColumnSpan(this.txtRegexToMatchRefs, 3);
            this.txtRegexToMatchRefs.Location = new System.Drawing.Point(277, 138);
            this.txtRegexToMatchRefs.Name = "txtRegexToMatchRefs";
            this.txtRegexToMatchRefs.Size = new System.Drawing.Size(247, 20);
            this.txtRegexToMatchRefs.TabIndex = 14;
            this.txtRegexToMatchRefs.Visible = false;
            this.txtRegexToMatchRefs.TextChanged += new System.EventHandler(this.txtRegexToMatchLocIDs_TextChanged);
            // 
            // lblOverwrite
            // 
            this.lblOverwrite.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOverwrite.AutoSize = true;
            this.lblOverwrite.Location = new System.Drawing.Point(11, 168);
            this.lblOverwrite.Name = "lblOverwrite";
            this.lblOverwrite.Size = new System.Drawing.Size(146, 13);
            this.lblOverwrite.TabIndex = 15;
            this.lblOverwrite.Text = "Overwrite existing translations";
            this.lblOverwrite.Visible = false;
            // 
            // cboOverwrite
            // 
            this.cboOverwrite.FormattingEnabled = true;
            this.cboOverwrite.Items.AddRange(new object[] {
            "None",
            "Unapproved",
            "All (including approved)"});
            this.cboOverwrite.Location = new System.Drawing.Point(277, 164);
            this.cboOverwrite.Name = "cboOverwrite";
            this.cboOverwrite.Size = new System.Drawing.Size(121, 21);
            this.cboOverwrite.TabIndex = 16;
            this.cboOverwrite.Visible = false;
            // 
            // chkMarkApproved
            // 
            this.chkMarkApproved.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkMarkApproved.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.chkMarkApproved, 2);
            this.chkMarkApproved.Location = new System.Drawing.Point(426, 166);
            this.chkMarkApproved.Name = "chkMarkApproved";
            this.chkMarkApproved.Size = new System.Drawing.Size(98, 17);
            this.chkMarkApproved.TabIndex = 17;
            this.chkMarkApproved.Text = "Mark approved";
            this.chkMarkApproved.UseVisualStyleBackColor = true;
            this.chkMarkApproved.Visible = false;
            // 
            // TxlMasterQuestionPreProcessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 341);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TxlMasterQuestionPreProcessorForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transcelerator Question Pre-Processor";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTextToSfm;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtXmlQuestionFile;
        private System.Windows.Forms.CheckBox chkWriteTempFile;
		private System.Windows.Forms.RadioButton rdoSfmToXml;
		private System.Windows.Forms.RadioButton rdoLocalization;
		private System.Windows.Forms.TextBox txtLocale;
		private System.Windows.Forms.Label lblLocale;
		private System.Windows.Forms.Button btnNavigateToSourceFile;
		private System.Windows.Forms.CheckBox m_chkRetainOnlyTranslated;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
		private System.Windows.Forms.Label lblRegexFilterForLocIds;
		private System.Windows.Forms.TextBox txtRegexToMatchRefs;
		private System.Windows.Forms.Label lblOverwrite;
		private System.Windows.Forms.ComboBox cboOverwrite;
		private System.Windows.Forms.CheckBox chkMarkApproved;
	}
}