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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptureForgeInfoDlg));
			System.Windows.Forms.Label label2;
			this.m_btnOk = new System.Windows.Forms.Button();
			this.m_linkLabelWorkingWithScriptureForge = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.m_linkLabelScriptureForge = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(442, 39);
			label1.TabIndex = 3;
			label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 42);
			label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(442, 13);
			label2.TabIndex = 4;
			label2.Text = "More information is available on-line:";
			// 
			// m_btnOk
			// 
			this.m_btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
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
			this.m_linkLabelWorkingWithScriptureForge.Location = new System.Drawing.Point(3, 61);
			this.m_linkLabelWorkingWithScriptureForge.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this.m_linkLabelWorkingWithScriptureForge.Name = "m_linkLabelWorkingWithScriptureForge";
			this.m_linkLabelWorkingWithScriptureForge.Size = new System.Drawing.Size(232, 17);
			this.m_linkLabelWorkingWithScriptureForge.TabIndex = 2;
			this.m_linkLabelWorkingWithScriptureForge.TabStop = true;
			this.m_linkLabelWorkingWithScriptureForge.Tag = "https://software.sil.org/transcelerator/working-with-scripture-forge/";
			this.m_linkLabelWorkingWithScriptureForge.Text = "Transcelerator: Working With Scripture Forge";
			this.m_linkLabelWorkingWithScriptureForge.UseCompatibleTextRendering = true;
			this.m_linkLabelWorkingWithScriptureForge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.m_linkLabelScriptureForge, 0, 3);
			this.tableLayoutPanel1.Controls.Add(label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.m_linkLabelWorkingWithScriptureForge, 0, 2);
			this.tableLayoutPanel1.Controls.Add(label2, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(448, 117);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// m_linkLabelScriptureForge
			// 
			this.m_linkLabelScriptureForge.AutoSize = true;
			this.m_linkLabelScriptureForge.Location = new System.Drawing.Point(3, 84);
			this.m_linkLabelScriptureForge.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this.m_linkLabelScriptureForge.Name = "m_linkLabelScriptureForge";
			this.m_linkLabelScriptureForge.Size = new System.Drawing.Size(118, 13);
			this.m_linkLabelScriptureForge.TabIndex = 5;
			this.m_linkLabelScriptureForge.TabStop = true;
			this.m_linkLabelScriptureForge.Tag = "http://scriptureforge.org/";
			this.m_linkLabelScriptureForge.Text = "Scripture Forge website";
			this.m_linkLabelScriptureForge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HandleLinkClicked);
			// 
			// ScriptureForgeInfoDlg
			// 
			this.AcceptButton = this.m_btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(475, 178);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.m_btnOk);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(491, 217);
			this.MinimizeBox = false;
			this.Name = "ScriptureForgeInfoDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Scripture Forge Integration";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button m_btnOk;
		private System.Windows.Forms.LinkLabel m_linkLabelWorkingWithScriptureForge;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.LinkLabel m_linkLabelScriptureForge;
	}
}