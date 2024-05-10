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
using System.Windows.Forms;
using SIL.Windows.Forms;
using SIL.Windows.Forms.ReleaseNotes;

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
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleCreated(System.EventArgs e)
		{
			base.OnHandleCreated(e);
			m_txlInfo.ShowCreditsAndLicense(m_allowInternetAccess);
		}

		private void m_linkLabelReleaseNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var path = TxlPlugin.GetFileDistributedWithApplication("ReleaseNotes.md");
			ShowModalChild(new ShowReleaseNotesDialog(Icon, path));
		}
	}
}