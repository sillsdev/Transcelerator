// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2019, SIL International.   
// <copyright from='2019' to='2019 company='SIL International'>
//		Copyright (c) 2019, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ScriptureForgeInfoDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	public partial class ScriptureForgeInfoDlg : Form
	{
		public ScriptureForgeInfoDlg()
		{
			InitializeComponent();
		}

		private void HandleLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var url = ((LinkLabel)sender).Tag as string;
			if (!string.IsNullOrEmpty(url))
				Process.Start(url);
		}
	}
}
