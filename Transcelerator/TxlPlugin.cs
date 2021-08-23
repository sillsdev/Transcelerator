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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using SIL.Keyboarding;
using SIL.Scripture;
using SIL.Windows.Forms.Keyboarding;
using JetBrains.Annotations;
using Paratext.PluginInterfaces;
using static System.String;

namespace SIL.Transcelerator
{
	[PublicAPI]
	public class TxlPlugin : IParatextStandalonePlugin
	{
		public const string pluginName = "Transcelerator";

		private static readonly string s_baseInstallFolder;
		private static readonly string s_company;
		private static readonly string s_version;
		private static Dictionary<IProject, ProjectState> s_projectStates = new Dictionary<IProject, ProjectState>();
		
		public string Name => pluginName;
		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";

		static TxlPlugin()
		{
			var assembly = Assembly.GetExecutingAssembly();
			s_baseInstallFolder = Path.GetDirectoryName(assembly.Location);
			var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			s_company = attributes.Length == 0 ? "SIL" : ((AssemblyCompanyAttribute)attributes[0]).Company;
			s_version = assembly.GetName().Version.ToString();
		}

	    public void Run(IPluginHost host, IParatextChildState state)
		{
			try
			{
				Application.EnableVisualStyles();

				TxlSplashScreen splashScreen;
				var project = state.Project;
			
				host.Log(this, $"Starting {pluginName} for project {project}");

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

				Action<bool> activateKeyboard;
				IVerseRef currentRef, startRef, endRef;

				lock (s_projectStates)
				{
					if (s_projectStates.TryGetValue(project, out var existing))
					{
						existing.Activate();
						return;
					}

					host.ShuttingDown += Host_ShuttingDown;

					if (!s_projectStates.Any()) // If there's already an active Transcelerator, just use the existing LM
						SetUpLocalization(preferredUiLocale);

					TxlCore.InitializeErrorHandling(host.ApplicationName, host.ApplicationVersion);

					splashScreen = new TxlSplashScreen();
					s_projectStates[project] = new ProjectState(splashScreen);
					splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
					splashScreen.Message = Format(
						LocalizationManager.GetString("SplashScreen.MsgRetrievingDataFromCaller",
							"Retrieving data from {0}...", "Param is host application name (Paratext)"),
						host.ApplicationName);

					currentRef = state.VerseRef.ChangeVersification(host.GetStandardVersification(StandardScrVersType.English));
					startRef = currentRef.Versification.CreateReference(currentRef.BookNum, 1, 1);
					var lastChapter = currentRef.Versification.GetLastChapter(currentRef.BookNum);
					endRef = currentRef.Versification.CreateReference(currentRef.BookNum,
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

					activateKeyboard = vern =>
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
				}

				UNSQuestionsDialog mainWindow = new UNSQuestionsDialog(splashScreen, host, project,
						activateKeyboard, startRef, endRef, preferredUiLocale, currentRef);

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

				lock (s_projectStates)
				{
					void SpecifyName(object s, EventArgs e) => MainWindow_Closed(project);

					mainWindow.Closed += SpecifyName;
					try
					{
						s_projectStates[project].ShowMainWindow(new Analytics(key, GetUserInfo(host), allowTracking), mainWindow);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						s_projectStates.Remove(project);
						throw;
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(Format(LocalizationManager.GetString("General.ErrorStarting", "Error occurred attempting to start {0}: ",
					"Param is \"Transcelerator\" (plugin name)"), pluginName) + e.Message);
				throw;
			}
		}

		private void MainWindow_Closed(IProject project)
		{
			lock (s_projectStates)
			{
				if (s_projectStates.TryGetValue(project, out var state))
				{
					state.Dispose();
					s_projectStates.Remove(project);
				}
			}
		}

		private void Host_ShuttingDown(object sender, System.ComponentModel.CancelEventArgs e)
		{
			lock (s_projectStates)
			{
				List<IProject> keysToDelete = new List<IProject>();
				foreach (var kvp in s_projectStates)
				{
					if (!kvp.Value.Close())
						keysToDelete.Add(kvp.Key);
				}

				foreach (var key in keysToDelete)
					s_projectStates.Remove(key);
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
			return new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = "en"};
		}

		private static void SetUpLocalization(string desiredUiLangId)
		{
			var installedStringFileFolder = Path.Combine(s_baseInstallFolder, "localization");
			var relativeSettingPathForLocalizationFolder = Path.Combine(s_company, pluginName);
			LocalizationManager.Create(TranslationMemory.XLiff, desiredUiLangId, pluginName, pluginName, s_version,
				installedStringFileFolder, relativeSettingPathForLocalizationFolder, new Icon(GetFileDistributedWithApplication("TXL no TXL.ico")), TxlCore.emailAddress,
				"SIL.Transcelerator", "SIL.Utils");
		}

		public string GetDescription(string locale)
		{
			return LocalizationManager.GetString("Transcelerator.Description",
				"Assists in rapid translation of Scripture comprehension checking questions.",
				"This will be requested using the current Paratext UI locale",
				new [] {locale, LocalizationManager.UILanguageId}, out _);
		}

		public IDataFileMerger GetMerger(IPluginHost host, string dataIdentifier)
		{
			return host.GetXmlMerger(ParatextDataFileAccessor.GetXMLDataMergeInfo(dataIdentifier));
		}

		public IEnumerable<PluginMenuEntry> PluginMenuEntries
		{
			get
			{
				yield return new PluginMenuEntry(pluginName + "...", Run, PluginMenuLocation.ScrTextDefault,
					@"TXL no TXL.ico");
			}
		}

		public static string GetFileDistributedWithApplication(params string[] partsOfTheSubPath)
		{
			var path = partsOfTheSubPath.Aggregate(s_baseInstallFolder, Path.Combine);

			if (File.Exists(path))
				return path;

			if (partsOfTheSubPath[0].Equals("docs", StringComparison.OrdinalIgnoreCase))
				return null; // Help files are optional in installer

			throw new ApplicationException("Could not locate the required file, " + path);
		}

		private class ProjectState : IDisposable
		{
			private TxlSplashScreen SplashScreen { get; set; }

			private UNSQuestionsDialog MainWindow { get; set; }

			private Analytics Analytics { get; set; }

			public ProjectState(TxlSplashScreen splashScreen)
			{
				SplashScreen = splashScreen;
			}

			public void Dispose()
			{
				SplashScreen?.Close();
				SplashScreen?.Dispose();
				MainWindow?.Dispose();
				Analytics?.Dispose();
			}

			public void Activate()
			{
				if (MainWindow != null)
				{
					InvokeOnUiThread(() => MainWindow.Activate());
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
					TxlSplashScreen tempSplashScreen = SplashScreen;
					tempSplashScreen?.Activate();
				}
			}

			/// <summary>
			/// Closes the main window if it has been set (in which case it is open)
			/// </summary>
			/// <returns><c>true</c> if the window was closed; <c>false</c> if the main window was not set,
			/// in which case the objects held by this state object are merely disposed.</returns>
			public bool Close()
			{
				if (MainWindow != null)
				{
					InvokeOnUiThread(() => MainWindow.Close());
					return true;
				}

				Dispose();
				return false;
			}

			private void InvokeOnUiThread(Action action)
			{
				if (MainWindow.InvokeRequired)
					MainWindow.Invoke(action);
				else
					action();
			}

			public void ShowMainWindow(Analytics analytics, UNSQuestionsDialog mainWindow)
			{
				if (MainWindow != null || Analytics != null || SplashScreen == null)
					throw new InvalidOperationException("Object in invalid state.");

				MainWindow = mainWindow;
				Analytics = analytics;

				Analytics.Track("Startup", new Dictionary<string, string>
					{{"Specific version", Assembly.GetExecutingAssembly().GetName().Version.ToString()}});

				mainWindow.Show();
			}
		}
	}
}
