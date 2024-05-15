// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2012' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: HelpAboutDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.Windows.Forms;
using SIL.Windows.Forms.ReleaseNotes;
using SIL.WritingSystems;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class HelpAboutDlg : ParentFormBase
	{
		private readonly bool m_allowInternetAccess;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Form1"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public HelpAboutDlg(Icon icon, bool allowInternetAccess)
		{
			m_allowInternetAccess = allowInternetAccess;
			InitializeComponent();
			Icon = icon;

			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
			HandleStringsLocalized();
		}

		/// ------------------------------------------------------------------------------------
		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;
			Text = string.Format(Text, TxlConstants.kPluginName);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleCreated(System.EventArgs e)
		{
			base.OnHandleCreated(e);
			m_txlInfo.ShowCreditsAndLicense(m_allowInternetAccess);
		}

		/// ------------------------------------------------------------------------------------
		private void m_linkLabelReleaseNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var locale = IetfLanguageTag.GetGeneralCode(LocalizationManager.UILanguageId);
			string path = null;
			if (locale != TxlPlugin.kDefaultUILocale)
			{
				path = TxlPlugin.GetFileDistributedWithApplication($"ReleaseNotes.{locale}.md");
				if (!File.Exists(path))
					path = null;
			}

			if (path == null)
				path = TxlPlugin.GetFileDistributedWithApplication("ReleaseNotes.md");

			ShowModalChild(new ShowReleaseNotesDialog(Icon, path));
		}
	}
}