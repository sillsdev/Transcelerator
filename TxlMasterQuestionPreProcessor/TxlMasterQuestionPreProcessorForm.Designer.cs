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
			this.btnGenerateMasterQuestionFile = new System.Windows.Forms.Button();
			this.txtSfmQuestionFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtXmlQuestionFile = new System.Windows.Forms.TextBox();
			this.chkWriteTempFile = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnTextToSfm
			// 
			this.btnTextToSfm.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnTextToSfm.Location = new System.Drawing.Point(108, 144);
			this.btnTextToSfm.Name = "btnTextToSfm";
			this.btnTextToSfm.Size = new System.Drawing.Size(96, 23);
			this.btnTextToSfm.TabIndex = 0;
			this.btnTextToSfm.Text = "Text to SFM...";
			this.btnTextToSfm.UseVisualStyleBackColor = true;
			this.btnTextToSfm.Click += new System.EventHandler(this.btnTextToSfm_Click);
			// 
			// btnGenerateMasterQuestionFile
			// 
			this.btnGenerateMasterQuestionFile.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnGenerateMasterQuestionFile.Location = new System.Drawing.Point(256, 144);
			this.btnGenerateMasterQuestionFile.Name = "btnGenerateMasterQuestionFile";
			this.btnGenerateMasterQuestionFile.Size = new System.Drawing.Size(96, 23);
			this.btnGenerateMasterQuestionFile.TabIndex = 1;
			this.btnGenerateMasterQuestionFile.Text = "SFM to XML";
			this.btnGenerateMasterQuestionFile.UseVisualStyleBackColor = true;
			this.btnGenerateMasterQuestionFile.Click += new System.EventHandler(this.btnGenerateMasterQuestionFile_Click);
			// 
			// txtSfmQuestionFile
			// 
			this.txtSfmQuestionFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSfmQuestionFile.Location = new System.Drawing.Point(15, 25);
			this.txtSfmQuestionFile.Name = "txtSfmQuestionFile";
			this.txtSfmQuestionFile.Size = new System.Drawing.Size(429, 20);
			this.txtSfmQuestionFile.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(187, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Master Standard Format Question File:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(131, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Master XML Question File:";
			// 
			// txtXmlQuestionFile
			// 
			this.txtXmlQuestionFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtXmlQuestionFile.Location = new System.Drawing.Point(15, 76);
			this.txtXmlQuestionFile.Name = "txtXmlQuestionFile";
			this.txtXmlQuestionFile.Size = new System.Drawing.Size(429, 20);
			this.txtXmlQuestionFile.TabIndex = 4;
			// 
			// chkWriteTempFile
			// 
			this.chkWriteTempFile.AutoSize = true;
			this.chkWriteTempFile.Location = new System.Drawing.Point(15, 109);
			this.chkWriteTempFile.Name = "chkWriteTempFile";
			this.chkWriteTempFile.Size = new System.Drawing.Size(302, 17);
			this.chkWriteTempFile.TabIndex = 6;
			this.chkWriteTempFile.Text = "Write results to temp file instead of appending to master file";
			this.chkWriteTempFile.UseVisualStyleBackColor = true;
			// 
			// TxlMasterQuestionPreProcessorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(461, 190);
			this.Controls.Add(this.chkWriteTempFile);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtXmlQuestionFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSfmQuestionFile);
			this.Controls.Add(this.btnGenerateMasterQuestionFile);
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
        private System.Windows.Forms.Button btnGenerateMasterQuestionFile;
        private System.Windows.Forms.TextBox txtSfmQuestionFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtXmlQuestionFile;
        private System.Windows.Forms.CheckBox chkWriteTempFile;
    }
}