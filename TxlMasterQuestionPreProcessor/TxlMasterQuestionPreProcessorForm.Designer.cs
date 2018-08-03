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
			this.SuspendLayout();
			// 
			// btnTextToSfm
			// 
			this.btnTextToSfm.Location = new System.Drawing.Point(15, 59);
			this.btnTextToSfm.Name = "btnTextToSfm";
			this.btnTextToSfm.Size = new System.Drawing.Size(96, 23);
			this.btnTextToSfm.TabIndex = 0;
			this.btnTextToSfm.Text = "Text to SFM...";
			this.btnTextToSfm.UseVisualStyleBackColor = true;
			this.btnTextToSfm.Click += new System.EventHandler(this.btnTextToSfm_Click);
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerate.Location = new System.Drawing.Point(348, 239);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(96, 23);
			this.btnGenerate.TabIndex = 1;
			this.btnGenerate.Text = "Generate XML";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// txtSourceFile
			// 
			this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSourceFile.Location = new System.Drawing.Point(15, 129);
			this.txtSourceFile.Name = "txtSourceFile";
			this.txtSourceFile.Size = new System.Drawing.Size(392, 20);
			this.txtSourceFile.TabIndex = 2;
			this.txtSourceFile.TextChanged += new System.EventHandler(this.UpdateGenerateButtonEnabledState);
			// 
			// lblSource
			// 
			this.lblSource.AutoSize = true;
			this.lblSource.Location = new System.Drawing.Point(12, 113);
			this.lblSource.Name = "lblSource";
			this.lblSource.Size = new System.Drawing.Size(189, 13);
			this.lblSource.TabIndex = 3;
			this.lblSource.Text = "Source Standard Format Question File:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 164);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(131, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Master XML Question File:";
			// 
			// txtXmlQuestionFile
			// 
			this.txtXmlQuestionFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtXmlQuestionFile.Location = new System.Drawing.Point(15, 180);
			this.txtXmlQuestionFile.Name = "txtXmlQuestionFile";
			this.txtXmlQuestionFile.Size = new System.Drawing.Size(429, 20);
			this.txtXmlQuestionFile.TabIndex = 4;
			this.txtXmlQuestionFile.TextChanged += new System.EventHandler(this.UpdateGenerateButtonEnabledState);
			// 
			// chkWriteTempFile
			// 
			this.chkWriteTempFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkWriteTempFile.AutoSize = true;
			this.chkWriteTempFile.Location = new System.Drawing.Point(15, 211);
			this.chkWriteTempFile.Name = "chkWriteTempFile";
			this.chkWriteTempFile.Size = new System.Drawing.Size(302, 17);
			this.chkWriteTempFile.TabIndex = 6;
			this.chkWriteTempFile.Text = "Write results to temp file instead of appending to master file";
			this.chkWriteTempFile.UseVisualStyleBackColor = true;
			this.chkWriteTempFile.CheckedChanged += new System.EventHandler(this.UpdateGenerateButtonEnabledState);
			// 
			// rdoSfmToXml
			// 
			this.rdoSfmToXml.AutoSize = true;
			this.rdoSfmToXml.Checked = true;
			this.rdoSfmToXml.Location = new System.Drawing.Point(15, 12);
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
			this.rdoLocalization.Location = new System.Drawing.Point(15, 36);
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
			this.txtLocale.Enabled = false;
			this.txtLocale.Location = new System.Drawing.Point(324, 35);
			this.txtLocale.Name = "txtLocale";
			this.txtLocale.Size = new System.Drawing.Size(120, 20);
			this.txtLocale.TabIndex = 9;
			this.txtLocale.TextChanged += new System.EventHandler(this.UpdateGenerateButtonEnabledState);
			// 
			// lblLocale
			// 
			this.lblLocale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocale.AutoSize = true;
			this.lblLocale.Enabled = false;
			this.lblLocale.Location = new System.Drawing.Point(230, 38);
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
			this.btnNavigateToSourceFile.Location = new System.Drawing.Point(413, 129);
			this.btnNavigateToSourceFile.Name = "btnNavigateToSourceFile";
			this.btnNavigateToSourceFile.Size = new System.Drawing.Size(31, 22);
			this.btnNavigateToSourceFile.TabIndex = 11;
			this.btnNavigateToSourceFile.Tag = "...";
			this.btnNavigateToSourceFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnNavigateToSourceFile.UseVisualStyleBackColor = true;
			this.btnNavigateToSourceFile.Click += new System.EventHandler(this.btnNavigateToSourceFile_Click);
			// 
			// TxlMasterQuestionPreProcessorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(461, 274);
			this.Controls.Add(this.btnNavigateToSourceFile);
			this.Controls.Add(this.lblLocale);
			this.Controls.Add(this.txtLocale);
			this.Controls.Add(this.rdoLocalization);
			this.Controls.Add(this.rdoSfmToXml);
			this.Controls.Add(this.chkWriteTempFile);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtXmlQuestionFile);
			this.Controls.Add(this.lblSource);
			this.Controls.Add(this.txtSourceFile);
			this.Controls.Add(this.btnGenerate);
			this.Controls.Add(this.btnTextToSfm);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TxlMasterQuestionPreProcessorForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Transcelerator Question Pre-Processor";
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}