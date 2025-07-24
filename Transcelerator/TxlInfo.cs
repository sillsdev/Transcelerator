// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2012' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using Microsoft.Web.WebView2.Core;
using SIL.Reporting;
using SIL.Transcelerator.Properties;
using static System.String;
using static SIL.Transcelerator.TxlConstants;
using static SIL.Transcelerator.TxlPlugin;

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
		private const string kTempResources = "Temp";
		
		private readonly string m_versionStr;
		private readonly string m_buildDate;
		private readonly string m_showDetailsLabel;
		private string m_copyright;
		private string m_htmlTemplate;
		private string m_tempTxlLogoPath;
		private string m_tempSilLogoPath;
		private string m_creditsAndLicense;
		private bool m_webBrowserReady;
		private bool m_allowInternetAccess;

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
			m_versionStr = assembly.GetName().Version.ToString();
			m_buildDate = File.GetLastWriteTime(assembly.Location).ToShortDateString();

			object[] attributes = assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
			if (attributes.Length > 0)
				m_copyright = ((AssemblyCopyrightAttribute) attributes[0]).Copyright;

			m_showDetailsLabel = m_btnDetails.Text;
		}

		private string VersionInfo => Format(m_lblAppVersion.Text, m_versionStr);

		private string BuildDateInfo => Format(m_lblBuildDate.Text, m_buildDate);

		private string FullCopyrightNotice
		{
			get
			{
				if (m_copyright == null)
					m_copyright = "© SIL Global";
				return Format(m_lblCopyrightAndLicense.Text, m_copyright);
			}
		}

		private async void OnLoad(object sender, EventArgs e)
		{
			try
			{
				var htmlPath = Path.Combine(InstallDir, "TxlInfo.htm");

				m_tempTxlLogoPath = Path.ChangeExtension(Path.GetTempFileName(), "png");
				Resources.Transcelerator.Save(m_tempTxlLogoPath);
				m_tempSilLogoPath = Path.ChangeExtension(Path.GetTempFileName(), "png");
				Windows.Forms.Widgets.SilResources.SilLogoRandom.Save(m_tempSilLogoPath);

				m_htmlTemplate = File.ReadAllText(htmlPath)
					.Replace("src=\"Properties/Transcelerator.png", $"src=\"http://{kTempResources}/{Path.GetFileName(m_tempTxlLogoPath)}")
					.Replace("src=\"DevResources/SILLogoBlue101x113.png", $"src=\"http://{kTempResources}/{Path.GetFileName(m_tempSilLogoPath)}");

				LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;

				await InitializeWebBrowserAsync();
				if (WebView2EnvironmentInitializationException == null)
				{
					m_webBrowserReady = true;
					_webBrowser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
				}
				SetHtmlDocument();
			}
			catch (Exception ex)
			{
				ErrorReport.ReportFatalException(ex);
			}
		}

		private async Task InitializeWebBrowserAsync()
		{
			if (WebView2EnvironmentInitializationException != null)
				return;
			
			try
			{
				await _webBrowser.EnsureCoreWebView2Async(WebView2Environment);
			}
			catch (Exception e)
			{
				WebView2EnvironmentInitializationException = e;
			}
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm == null || lm == PrimaryLocalizationManager)
				SetHtmlDocument();
		}

		private void SetHtmlDocument()
		{
			m_lblAppVersion.Text = VersionInfo;
			m_lblBuildDate.Text = BuildDateInfo;

			if (WebView2EnvironmentInitializationException != null)
			{
				DisplayBasicInfoFallback();
				return;
			}

			if (!m_webBrowserReady)
				return;

			InitializeWebBrowserUserInteractionSettings();

			var htmlContents = Format(m_htmlTemplate, VersionInfo, BuildDateInfo,
				m_creditsAndLicense ?? Empty);

			var matchCopyright = Regex.Match(htmlContents, "&#169;[^<]*");

			if (m_copyright == null && matchCopyright.Success)
				m_copyright = matchCopyright.ToString();

			if (matchCopyright.Success)
				htmlContents = matchCopyright.Result("$`" + FullCopyrightNotice + "$'");

			try
			{
				_webBrowser.CoreWebView2.SetVirtualHostNameToFolderMapping(kTempResources,
					Path.GetDirectoryName(m_tempTxlLogoPath),
					CoreWebView2HostResourceAccessKind.Allow);
				_webBrowser.NavigateToString(htmlContents);
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalException(e);
			}
		}

		private void DisplayBasicInfoFallback()
		{
			_webBrowser.Hide();
			m_lblCopyrightAndLicense.Text = FullCopyrightNotice;
			m_lblWebView2Problem.Text = Format(m_lblWebView2Problem.Text,
					"WebView2 Runtime");
			if (m_allowInternetAccess)
			{
				m_lblWebView2ErrorResolution.Text = Format(m_lblWebView2ErrorResolution.Text,
					"WebView2 Runtime", kEmailAddress);
				m_lblWebView2ErrorResolution.Visible = true;
				m_btnDetails.Visible = true;
				m_linkWebView2Runtime.Visible = true;
				m_txtExceptionDetails.Text = WebView2EnvironmentInitializationException.ToString();
			}

			m_tableLayoutPanelFallback.Show();
		}

		private void InitializeWebBrowserUserInteractionSettings()
		{
			var interactionAllowed = m_creditsAndLicense != null;
			_webBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = interactionAllowed;
			_webBrowser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = interactionAllowed;
			#if DEBUG
				_webBrowser.CoreWebView2.Settings.AreDevToolsEnabled = true;
			#else
				_webBrowser.CoreWebView2.Settings.AreDevToolsEnabled = false;
			#endif
		}

		/// ------------------------------------------------------------------------------------
		public void ShowCreditsAndLicense(bool allowInternetAccess)
		{
			m_allowInternetAccess = allowInternetAccess;
			if (m_creditsAndLicense == null)
				LoadCreditsAndLicense();

			SetHtmlDocument();
		}

		/// ------------------------------------------------------------------------------------
		private void LoadCreditsAndLicense()
		{
			var htmlPath = Path.Combine(InstallDir, "CreditsAndLicense.htm");
			try
			{
				var html = File.ReadAllText(htmlPath);
				m_creditsAndLicense = Regex.Match(html,
					@"<div\s+class=([""'])credits\1[^>]*>[\s\S]*?</div>").Value;
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalException(e);
			}
		}

		/// <summary>
		/// By default, links that are clicked in the Credits and License pane would open in
		/// Edge. This causes them to open in the default browser instead. If Internet access
		/// is not allowed, the user is informed and the link is not followed.
		/// </summary>
		private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
		{
			var url = e.Uri;
			if (!IsNullOrEmpty(url))
			{
				if (m_allowInternetAccess)
				{
					try
					{
						Process.Start(url);
					}
					catch (Exception exception)
					{
						ErrorReport.ReportNonFatalException(exception);
					}
				}
				else
				{
					MessageBox.Show(ParentForm,
						LocalizationManager.GetString("TxlInfo.InternetDisabled",
						"Internet access is disabled via 'Paratext > Paratext settings'",
						"The text of this message should be identical to the one Paratext displays (in HelpManagerBase.cs)"),
						kPluginName);
				}
			}

			e.Handled = true;
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
				m_webBrowserReady = false;

				components?.Dispose();
				try
				{
					_webBrowser?.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// Already disposed; safe to ignore
				}

				if (m_tempTxlLogoPath != null)
					File.Delete(m_tempTxlLogoPath);
				if (m_tempSilLogoPath != null)
					File.Delete(m_tempSilLogoPath);
			}
			base.Dispose(disposing);
		}

		private void m_btnDetails_Click(object sender, EventArgs e)
		{
			if (m_txtExceptionDetails.Visible)
			{
				m_txtExceptionDetails.Hide();
				m_btnDetails.Text = m_showDetailsLabel;
			}
			else
			{
				m_txtExceptionDetails.Show();
				m_btnDetails.Text = LocalizationManager.GetString("TxlInfo.m_btnDetails.Show",
					"Hide details");
			}
		}

		private void m_linkWebView2Runtime_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start($"https://{m_linkWebView2Runtime.Text}/");
		}
	}
}
