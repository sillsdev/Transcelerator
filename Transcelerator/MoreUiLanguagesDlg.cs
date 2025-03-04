﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.   
// <copyright from='2020' to='2022 company='SIL International'>
//		Copyright (c) 2022, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: MoreUiLanguagesDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using System;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using static System.String;

namespace SIL.Transcelerator
{
	public partial class MoreUiLanguagesDlg : Form
	{
		private readonly ToolStripItem m_displayLanguageMenu;

		public MoreUiLanguagesDlg(ToolStripItem displayLanguageMenu)
		{
			m_displayLanguageMenu = displayLanguageMenu;
			InitializeComponent();
			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;

			m_linkLabelAddDisplayLanguageUsingInstaller.Text =
				Format(m_linkLabelAddDisplayLanguageUsingInstaller.Text,
					m_displayLanguageMenu.Text.Replace("&", "")) + " ";
			var startHyperlink = m_linkLabelAddDisplayLanguageUsingInstaller.Text.Length;
			m_linkLabelAddDisplayLanguageUsingInstaller.Text +=
				LocalizationManager.GetString("MoreUiLanguagesDlg.Download",
					"Download the latest Installer.",
					"Sentence that will be formatted as a hyperlink and appended to " +
					"\"MoreUiLanguagesDlg.m_linkLabelAddDisplayLanguageUsingInstaller\"");
			var endHyperlink = m_linkLabelAddDisplayLanguageUsingInstaller.Text.Length;
			m_linkLabelAddDisplayLanguageUsingInstaller.LinkArea = new LinkArea(startHyperlink, endHyperlink);

			const string kCrowdin = "Crowdin";

			m_linkLabelCrowdinInformation.Text =
				Format(m_linkLabelCrowdinInformation.Text,
					TxlConstants.kPluginName, kCrowdin);
			m_linkLabelCrowdinInformation.LinkArea = new LinkArea(
				m_linkLabelCrowdinInformation.Text.IndexOf(kCrowdin, StringComparison.Ordinal),
				kCrowdin.Length);
		}

		private void HandleLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var url = ((LinkLabel)sender).Tag as string;
			if (!IsNullOrEmpty(url))
				Process.Start(url);
		}
	}
}
