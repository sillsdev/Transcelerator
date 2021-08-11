// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2013' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using SIL.IO;
using SIL.Keyboarding;
using SIL.Reporting;
using SIL.Scripture;
using SIL.Windows.Forms.Keyboarding;
using SIL.Windows.Forms.Reporting;
using JetBrains.Annotations;
using Paratext.PluginInterfaces;
using static System.String;

namespace SIL.Transcelerator
{
	[PublicAPI]
	public class TxlPlugin : IParatextStandalonePlugin
	{
		public const string pluginName = "Transcelerator";
		public const string emailAddress = "transcelerator_feedback@sil.org";
	    private TxlSplashScreen splashScreen;
		private UNSQuestionsDialog unsMainWindow;

		public void RequestShutdown()
		{
		    lock (this)
		    {
		        if (unsMainWindow != null)
		        {
		            InvokeOnUiThread(delegate
		                {
		                    unsMainWindow.Activate();
		                    unsMainWindow.Close();
		                });
		        }
		        else
                    Environment.Exit(0);
		    }
		}

	    public void Activate(string activeProjectName)
	    {
            if (unsMainWindow != null)
	        {
	            lock (this)
	            {
	                InvokeOnUiThread(delegate { unsMainWindow.Activate(); });
	            }
	        }
            else
            {
                // Can't lock because the whole start-up sequence takes several seconds and the
                // whole point of this code is to activate the splash screen so the user can see
                // it's still starting up. But there is no harm in calling Activate on the splash
                // screen if we happen to catch it between the time it is closed and the member
                // variable is set to null, since in that case, the "real" splash screen is closed
                // and Activate is a no-op. But we do need to use a temp variable because it could
                // get set to null between the time we check for null and the call to Activate.
                TxlSplashScreen tempSplashScreen = splashScreen;
				tempSplashScreen?.Activate();
			}
	    }

	    public void Run(IPluginHost host, IParatextChildState state)
		{
			try
			{
				Application.EnableVisualStyles();

				host.Log(this, "Starting " + pluginName);

				string preferredUiLocale = "en";
				try
				{
					preferredUiLocale = host.UserSettings.UiLocale;
					if (IsNullOrWhiteSpace(preferredUiLocale))
						preferredUiLocale = "en";
				}
				catch (Exception)
				{
				}

				SetUpLocalization(preferredUiLocale);

				var project = state.Project;

				Thread mainUIThread = new Thread(() =>
				{
					InitializeErrorHandling(host, project.ShortName);

					UNSQuestionsDialog formToShow;
					lock (this)
					{
						splashScreen = new TxlSplashScreen();
					    splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
						splashScreen.Message = Format(
						    LocalizationManager.GetString("SplashScreen.MsgRetrievingDataFromCaller",
							    "Retrieving data from {0}...", "Param is host application name (Paratext)"),
						    host.ApplicationName);

						var currentRef = state.VerseRef.ChangeVersification(host.GetStandardVersification(StandardScrVersType.English));
						IVerseRef startRef = currentRef.Versification.CreateReference(currentRef.BookNum, 1, 1);
						var lastChapter = currentRef.Versification.GetLastChapter(currentRef.BookNum);
						IVerseRef endRef = currentRef.Versification.CreateReference(currentRef.BookNum,
							lastChapter, currentRef.Versification.GetLastVerse(currentRef.BookNum, lastChapter));

						// See TXL-131 for explanation of this code, if needed.
						if (Properties.Settings.Default.FilterStartRef > 0 &&
							Properties.Settings.Default.FilterStartRef < Properties.Settings.Default.FilterEndRef)
						{
							var savedStartRef = new BCVRef(Properties.Settings.Default.FilterStartRef);
							var savedEndRef = new BCVRef(Properties.Settings.Default.FilterEndRef);
							if (savedStartRef.Valid && savedEndRef.Valid &&
								savedStartRef <= currentRef.BBBCCCVVV && savedEndRef >= currentRef.BBBCCCVVV)
							{
								startRef = currentRef.Versification.CreateReference(savedStartRef);
								endRef = currentRef.Versification.CreateReference(savedEndRef);
							}
						}

						KeyboardController.Initialize();

						Action<bool> activateKeyboard = vern =>
						{
							if (vern)
							{
								//try
								//{
								project.VernacularKeyboard?.Activate();
								//}
								//catch (ApplicationException e)
								//{
								//	// For some reason, the very first time this gets called it throws a COM exception, wrapped as
								//	// an ApplicationException. Mysteriously, it seems to work just fine anyway, and then all subsequent
								//	// calls work with no exception. Paratext seems to make this same call without any exceptions. The
								//	// documentation for ITfInputProcessorProfiles.ChangeCurrentLanguage (which is the method call
								//	// in SIL.Windows.Forms.Keyboarding.Windows that throws the COM exception says that an E_FAIL is an
								//	// unspecified error, so that's fairly helpful.
								//	if (!(e.InnerException is COMException))
								//		throw;
								//}
							}
							else
								Keyboard.Controller.ActivateDefaultKeyboard();
						};

						formToShow = unsMainWindow = new UNSQuestionsDialog(splashScreen, host, project,
							activateKeyboard, startRef, endRef, preferredUiLocale, currentRef);

						splashScreen = null;
					}

#if DEBUG
                    // Always track if this is a debug build, but track to a different segment.io project
                    const bool allowTracking = true;
                    const string key = "0mtsix4obm";
#else
                    // If this is a release build, then allow an environment variable to be set to false
                    // so that testers aren't generating false analytics
                    string feedbackSetting = Environment.GetEnvironmentVariable("FEEDBACK");

                    var allowTracking = string.IsNullOrEmpty(feedbackSetting) || feedbackSetting.ToLower() == "yes" || feedbackSetting.ToLower() == "true";

                    const string key = "3iuv313n8t";
#endif
					using (new Analytics(key, GetUserInfo(host), allowTracking))
					{
						Analytics.Track("Startup", new Dictionary<string, string>
						{{"Specific version", Assembly.GetExecutingAssembly().GetName().Version.ToString()}});

						formToShow.ShowDialog();
					}
					host.Log(this, "Closing " + pluginName);
					Environment.Exit(0);
				});
				mainUIThread.Name = pluginName;
				mainUIThread.IsBackground = false;
				mainUIThread.SetApartmentState(ApartmentState.STA);
				mainUIThread.Start();
				// Avoid putting any code after this line. Any exceptions thrown will not be able to be reported via the
				// "green screen" because we are not running in STA.
			}
			catch (Exception e)
			{
				MessageBox.Show(Format(LocalizationManager.GetString("General.ErrorStarting", "Error occurred attempting to start {0}: ",
					"Param is \"Transcelerator\" (plugin name)"), pluginName) + e.Message);
				throw;
			}
		}

		private UserInfo GetUserInfo(IPluginHost host)
		{
			string lastName = host.UserInfo.Name;
			string firstName = "";
			if (lastName != null)
			{
				var split = lastName.LastIndexOf(" ", StringComparison.Ordinal);
				if (split <= 0)
					split = lastName.LastIndexOf("_", StringComparison.Ordinal);
				if (split > 0)
				{
					firstName = lastName.Substring(0, split);
					lastName = lastName.Substring(split + 1);
				}
			}
			//return new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = LocalizationManager.UILanguageId, Email = emailAddress };
			// TODO: Enhance plugin API to get access to e-mail address
			return new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = "en"};
		}

		private void InvokeOnUiThread(Action action)
		{
		    lock (this)
		    {
		        if (unsMainWindow.InvokeRequired)
		            unsMainWindow.Invoke(action);
		        else
		            action();
		    }
		}

		private void InitializeErrorHandling(IPluginHost host, string projectName)
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = emailAddress;
			ErrorReport.AddStandardProperties();
			// The version that gets added to the report by default is for the entry assembly, which is
			// AddInProcess32.exe. Even if if reported a version (which it doesn't), it wouldn't be very
			// useful.
			ErrorReport.AddProperty("Plugin Name", pluginName);
			Assembly assembly = Assembly.GetExecutingAssembly();
			ErrorReport.AddProperty("Version", Format("{0} (apparent build date: {1})",
				assembly.GetName().Version,
				File.GetLastWriteTime(assembly.Location).ToShortDateString()));
			ErrorReport.AddProperty("Host Application", host.ApplicationName + " " + host.ApplicationVersion);
			ErrorReport.AddProperty("Project Name", projectName);
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}
		
		private static void SetUpLocalization(string desiredUiLangId)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			var company = attributes.Length == 0 ? "SIL" : ((AssemblyCompanyAttribute)attributes[0]).Company;
			var installedStringFileFolder = FileLocationUtilities.GetDirectoryDistributedWithApplication("localization");
			var relativeSettingPathForLocalizationFolder = Path.Combine(company, pluginName);
			var version = assembly.GetName().Version.ToString();
			LocalizationManager.Create(TranslationMemory.XLiff, desiredUiLangId, pluginName, pluginName, version,
				installedStringFileFolder, relativeSettingPathForLocalizationFolder, new Icon(FileLocationUtilities.GetFileDistributedWithApplication("TXL no TXL.ico")), emailAddress,
				"SIL.Transcelerator", "SIL.Utils");
		}

		public string GetDescription(string locale)
		{
			return LocalizationManager.GetString("Transcelerator.Description",
				"Assists in rapid translation of Scripture comprehension checking questions.",
				"This will be requested using the current Paratext UI locale",
				new [] {locale, LocalizationManager.UILanguageId}, out _);
		}

		public string Name => pluginName;
		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";

		public IEnumerable<KeyValuePair<string, XMLDataMergeInfo>> MergeDataInfo =>
			ParatextDataFileAccessor.GetDataFileKeySpecifications();

		public IEnumerable<PluginMenuEntry> PluginMenuEntries
		{
			get
			{
				yield return new PluginMenuEntry(pluginName + "...", Run, PluginMenuLocation.ScrTextDefault,
					@"TXL no TXL.ico");
			}
		}
	}
}
