// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TxlInfo.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[Serializable]
	public partial class TxlInfo : UserControl
	{
		private WebBrowser browserCreditsAndLicense;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TxlInfo"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TxlInfo()
		{
			InitializeComponent();

			// Get copyright information from assembly info. By doing this we don't have
			// to update the splash screen each year.
			var assembly = Assembly.GetExecutingAssembly();
			object[] attributes = assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
			if (attributes.Length > 0)
				m_lblCopyright.Text = ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
			m_lblCopyright.Text = string.Format(Properties.Resources.kstidCopyrightFmt, m_lblCopyright.Text.Replace("(C)", "©"));

			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			m_lblAppVersion.Text = string.Format(m_lblAppVersion.Text, version);
			lblBuildDate.Text = string.Format(lblBuildDate.Text,
				File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToShortDateString());
		}

		/// ------------------------------------------------------------------------------------
		public bool ShowCreditsAndLicense
		{
			set
			{
				if (value)
				{
					if (browserCreditsAndLicense == null)
					{
						browserCreditsAndLicense = new WebBrowser();
						browserCreditsAndLicense.AllowWebBrowserDrop = false;
						browserCreditsAndLicense.AllowNavigation = true;
						tableLayoutPanel.Controls.Add(browserCreditsAndLicense,
							0, tableLayoutPanel.RowCount - 1);
						tableLayoutPanel.SetColumnSpan(browserCreditsAndLicense, 2);
						browserCreditsAndLicense.Padding = new Padding(0, 6, 0, 0);
					}
					if (browserCreditsAndLicense.IsHandleCreated)
						LoadCreditsAndLicense();
					else
						browserCreditsAndLicense.HandleCreated += delegate { LoadCreditsAndLicense(); };
				}
				else if (browserCreditsAndLicense != null)
				{
					browserCreditsAndLicense.Dispose();
					browserCreditsAndLicense = null;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		private void LoadCreditsAndLicense()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(LoadCreditsAndLicense));
				return;
			}
			browserCreditsAndLicense.Navigate(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"CreditsAndLicense.htm"));

			browserCreditsAndLicense.Navigating += browserCreditsAndLicense_Navigating;
		}

		/// ------------------------------------------------------------------------------------
		void browserCreditsAndLicense_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			var url = e.Url.ToString();
			e.Cancel = true;
			if (!string.IsNullOrEmpty(url))
				Process.Start(url);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing)
			{
				if (components != null)
					components.Dispose();
				if (browserCreditsAndLicense != null)
					browserCreditsAndLicense.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
