// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.   
// <copyright from='2019' to='2020 company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ScriptureForgeInfoDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using static System.String;

namespace SIL.Transcelerator
{
	public partial class ScriptureForgeInfoDlg : Form
	{
		private readonly string m_hostAppName;

		public ScriptureForgeInfoDlg(string hostAppName)
		{
			m_hostAppName = hostAppName;
			InitializeComponent();
			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;

			void FormatControlTextWithProductNames(Control ctrl) =>
				ctrl.Text = Format(ctrl.Text, UNSQuestionsDialog.kScriptureForgeProductName,
					TxlPlugin.pluginName, m_hostAppName);

			FormatControlTextWithProductNames(this);
			FormatControlTextWithProductNames(m_lblExplanation);
			FormatControlTextWithProductNames(m_linkLabelScriptureForge);
			FormatControlTextWithProductNames(m_linkLabelWorkingWithScriptureForge);
		}

		private void HandleLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var url = ((LinkLabel)sender).Tag as string;
			if (!IsNullOrEmpty(url))
				Process.Start(url);
		}
	}
}
