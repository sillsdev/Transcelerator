namespace SIL.Utils
{
	partial class ConfirmFileOverwriteDlg
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label3;
			this.lblFilename = new System.Windows.Forms.Label();
			this.chkApplyToAll = new System.Windows.Forms.CheckBox();
			this.btnYes = new System.Windows.Forms.Button();
			this.btnNo = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 13);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(92, 13);
			label1.TabIndex = 0;
			label1.Text = "File already exists:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(13, 43);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(139, 13);
			label3.TabIndex = 2;
			label3.Text = "Do you want to overwrite it?";
			// 
			// lblFilename
			// 
			this.lblFilename.AutoSize = true;
			this.lblFilename.Location = new System.Drawing.Point(13, 28);
			this.lblFilename.Name = "lblFilename";
			this.lblFilename.Size = new System.Drawing.Size(14, 13);
			this.lblFilename.TabIndex = 1;
			this.lblFilename.Text = "#";
			// 
			// chkApplyToAll
			// 
			this.chkApplyToAll.AutoSize = true;
			this.chkApplyToAll.Location = new System.Drawing.Point(16, 69);
			this.chkApplyToAll.Name = "chkApplyToAll";
			this.chkApplyToAll.Size = new System.Drawing.Size(98, 17);
			this.chkApplyToAll.TabIndex = 3;
			this.chkApplyToAll.Text = "&Apply to all files";
			this.chkApplyToAll.UseVisualStyleBackColor = true;
			// 
			// btnYes
			// 
			this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btnYes.Location = new System.Drawing.Point(130, 97);
			this.btnYes.Name = "btnYes";
			this.btnYes.Size = new System.Drawing.Size(75, 23);
			this.btnYes.TabIndex = 4;
			this.btnYes.Text = "&Yes";
			this.btnYes.UseVisualStyleBackColor = true;
			// 
			// btnNo
			// 
			this.btnNo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnNo.Location = new System.Drawing.Point(211, 97);
			this.btnNo.Name = "btnNo";
			this.btnNo.Size = new System.Drawing.Size(75, 23);
			this.btnNo.TabIndex = 5;
			this.btnNo.Text = "&No";
			this.btnNo.UseVisualStyleBackColor = true;
			// 
			// ConfirmFileOverwriteDlg
			// 
			this.AcceptButton = this.btnNo;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnNo;
			this.ClientSize = new System.Drawing.Size(416, 132);
			this.Controls.Add(this.btnNo);
			this.Controls.Add(this.btnYes);
			this.Controls.Add(this.chkApplyToAll);
			this.Controls.Add(label3);
			this.Controls.Add(this.lblFilename);
			this.Controls.Add(label1);
			this.MaximumSize = new System.Drawing.Size(6000, 170);
			this.MinimumSize = new System.Drawing.Size(0, 170);
			this.Name = "ConfirmFileOverwriteDlg";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Overwrite Existing File?";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFilename;
		private System.Windows.Forms.CheckBox chkApplyToAll;
		private System.Windows.Forms.Button btnYes;
		private System.Windows.Forms.Button btnNo;
	}
}