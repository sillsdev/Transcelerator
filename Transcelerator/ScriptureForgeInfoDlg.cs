// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.   
// <copyright from='2019' to='2023 company='SIL International'>
//		Copyright (c) 2023, SIL International.   
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

			void FormatControlTextWithProductNames(Control ctrl)
            {
                ctrl.Text = Format(ctrl.Text, UNSQuestionsDialog.kScriptureForgeProductName,
                    TxlCore.kPluginName, m_hostAppName, UNSQuestionsDialog.kPTXPrintProductName);
            }

            void DealWithOutdatedTranslation(Control ctrl)
            {
                if (ctrl.Text.Contains("{0}") && !ctrl.Text.Contains("{3}"))
                    ctrl.Text = ctrl.Text.Replace("{0}", "{0}/{3}");
            }

			DealWithOutdatedTranslation(this);
            FormatControlTextWithProductNames(this);
			DealWithOutdatedTranslation(m_lblExplanation);
			FormatControlTextWithProductNames(m_lblExplanation);
			FormatControlTextWithProductNames(m_linkLabelWorkingWithScriptureForge);
			// If there is a translation for the SF website label but not for the PTXprint
			// website label, then most likely we can/should just re-use the one for the SF
			// label, assuming the SF website link's translation does have the placeholder.
			// Arguably, we could also implement this logic in the other direction as well,
			// but since support for SF pre-dates support for PTXprint, it is much more likely
			// that existing translations will be present for SF.
            if (m_linkLabelScriptureForge.Text != "{0} website" && // Keep in sync with English text in Designer
                m_linkLabelPtxPrint.Text == "{3} website" && // Keep in sync with English text in Designer
                m_linkLabelScriptureForge.Text.Contains("{0}"))
            {
                m_linkLabelPtxPrint.Text = m_linkLabelScriptureForge.Text.Replace("{0}", "{3}");
            }
			FormatControlTextWithProductNames(m_linkLabelScriptureForge);
            FormatControlTextWithProductNames(m_linkLabelPtxPrint);
        }

		private void HandleLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            var url = ((LinkLabel)sender).Tag as string;
			if (!IsNullOrEmpty(url))
				Process.Start(url);
		}
	}
}
