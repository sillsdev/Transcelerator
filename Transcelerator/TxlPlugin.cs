// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.   
// <copyright from='2013' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using SIL.Scripture;
using JetBrains.Annotations;
using Microsoft.Web.WebView2.Core;
using Paratext.PluginInterfaces;
using SIL.Windows.Forms.LocalizationIncompleteDlg;
using SIL.Reporting;
using SIL.WritingSystems;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.String;
using static SIL.Transcelerator.TxlConstants;

namespace SIL.Transcelerator
{
	[PublicAPI]
	public class TxlPlugin : IParatextStandalonePlugin, IPluginErrorHandler
	{
		public const string kDefaultUILocale = "en";
		public const string kDocsFolder = "docs";
		
		private static readonly string s_company;
		private static readonly string s_version;
		private static UserInfo s_userInfo;

		private static Analytics s_analytics; // Get lock on s_projectStates to set this.
		private static Dictionary<IProject, ProjectState> s_projectStates =
			new Dictionary<IProject, ProjectState>();
		private static IProject s_currentProject;
		private static IPluginHost Host { get; set; }

		internal static string InstallDir { get; }
		internal static string ProgramDataFolder { get; }
		internal static CoreWebView2Environment WebView2Environment { get; set; }

		internal static LocalizationIncompleteViewModel LocIncompleteViewModel { get; private set; }
		internal static ILocalizationManager PrimaryLocalizationManager => LocIncompleteViewModel.PrimaryLocalizationManager;

		/// <summary>
		/// Gets the Analytics singleton. Caller is responsible for ensuring that
		/// <see cref="s_projectStates"/> is locked.
		/// </summary>
		private static Analytics GetAnalytics(IPluginHost host)
		{
			if (s_analytics == null)
			{
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
				s_analytics = new Analytics(key, GetUserInfo(host), allowTracking, useCallingAssemblyVersion:true);
			}

			return s_analytics;
		}

		public string Name => kPluginName;
		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";

		static TxlPlugin()
		{
			var assembly = Assembly.GetExecutingAssembly();
			InstallDir = Path.GetDirectoryName(assembly.Location);
			var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			s_company = attributes.Length == 0 ? "SIL" : ((AssemblyCompanyAttribute)attributes[0]).Company;
			s_version = assembly.GetName().Version.ToString();

			// On Windows, CommonApplicationData is actually the preferred location for this
			// because it is not user-specific, but we do it this way to make it work on Linux.
			try
			{
				var deprecatedProgramDataFolder = Path.Combine(GetFolderPath(CommonApplicationData),
					"SIL", "Transcelerator");
				if (Directory.Exists(deprecatedProgramDataFolder))
				{
					var cachedQuestionsFilename = Path.Combine(deprecatedProgramDataFolder,
						kQuestionsFilename);
					if (File.Exists(cachedQuestionsFilename))
						File.Delete(cachedQuestionsFilename);
					Directory.Delete(deprecatedProgramDataFolder);
				}
			}
			catch (Exception)
			{
				// This was just a clean-up step from a possible previous version of Transcelerator, so if
				// something goes wrong, ignore it.
			}

			ProgramDataFolder = Path.Combine(GetFolderPath(LocalApplicationData), "SIL", "Transcelerator");
			if (!Directory.Exists(ProgramDataFolder))
				Directory.CreateDirectory(ProgramDataFolder);
		}

		public TxlPlugin(IPluginHost host)
		{
			Host = host;
			ErrorHandling.Initialize(host.ApplicationName, host.ApplicationVersion);
			ParatextDataFileAccessor.OnFailedToObtainWriteLock += fileName =>
			{
				ErrorReport.NotifyUserOfProblem(LocalizationManager.GetString("General.RequestLockError",
					"Unable to obtain exclusive write access to data that belongs to {1}: {0}",
					"Param 0: key that indicates the data file that was to be written;" +
					"Param 1: \"Transcelerator\" (plugin name)"), fileName, kPluginName);
			};
		}

	    public async void Run(IPluginHost host, IParatextChildState state)
		{
			try
			{
				Application.EnableVisualStyles();

				TxlSplashScreen splashScreen;
				var project = state.Project;
			
				host.Log(this, $"Starting {Name} for project {project}");

				string preferredUiLocale = kDefaultUILocale;
				try
				{
					preferredUiLocale = host.UserSettings.UiLocale;
					if (IsNullOrWhiteSpace(preferredUiLocale))
						preferredUiLocale = kDefaultUILocale;
				}
				catch (Exception)
				{
					// If something goes wrong, just use the default
				}

				CreateCoreWebView2Environment(preferredUiLocale);

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

					splashScreen = new TxlSplashScreen();
					s_projectStates[project] = new ProjectState(splashScreen);
					splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
					splashScreen.Message = Format(
						LocalizationManager.GetString("SplashScreen.MsgRetrievingDataFromCaller",
							"Retrieving data from {0} project {1}...",
							"Param 0: host application name (Paratext); " +
							"Param 1: Paratext project name"),
						host.ApplicationName, project.ShortName);
				}

				Action<bool> activateKeyboard = vern =>
				{
					if (vern)
						project.VernacularKeyboard?.Activate();
					else
						host.DefaultKeyboard.Activate();
				};

				var englishVersification = host.GetStandardVersification(StandardScrVersType.English);

				Action<BCVRef> sendReference = bcvRef =>
				{
					// ENHANCE: Some day we can maybe make it so the user can select the scroll group.
					// 999+ times out of 1000, the scroll group of the window at the time the user runs
					// Transcelerator will be the one they want.
					host.SetReferenceForSyncGroup(englishVersification.CreateReference(bcvRef.BBCCCVVV), state.SyncReferenceGroup);
				};

				IVerseRef currentRef = state.VerseRef.ChangeVersification(englishVersification);
				var startRef = currentRef.Versification.CreateReference(currentRef.BookNum, 1, 1);
				var lastChapter = currentRef.Versification.GetLastChapter(currentRef.BookNum);
				var endRef = currentRef.Versification.CreateReference(currentRef.BookNum,
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

				UNSQuestionsDialog mainWindow = new UNSQuestionsDialog(host, project, startRef, endRef,
					activateKeyboard, sendReference);

				await Task.Run(() => { InitMainWindow(mainWindow, splashScreen, project); });

				lock (s_projectStates)
				{
					if (!s_projectStates.ContainsKey(project))
					{
						// There was either a startup error that was already caught and handled,
						// or the user decided to shut down Paratext while the data was loading,
						// or a fatal exception occurred in some other Transcelerator window and
						// all instances were cleaned up.
						return;
					}
					mainWindow.Closed += (s, e) => MainWindow_Closed(project);
					mainWindow.Activated += (sender, e) =>
					{
						s_currentProject = s_projectStates.FirstOrDefault(s => s.Value.IsFor(sender as UNSQuestionsDialog)).Key;
					};

					try
					{
						s_projectStates[project].ShowMainWindow(GetAnalytics(host), mainWindow);
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
				ShowStartupError(e);
			}
		}

	    private void CleanUpForFatalException(IProject projectToKill = null)
		{
			bool lockTaken = false;
			try
			{
				Monitor.TryEnter(s_projectStates, 100, ref lockTaken);
				if (lockTaken)
				{
					List<ProjectState> copyOfProjectStates;
					if (projectToKill == null)
						copyOfProjectStates = s_projectStates.Values.ToList();
					else if (s_projectStates.TryGetValue(projectToKill, out var state))
						copyOfProjectStates = new List<ProjectState>(new [] {state});
					else
						return;

					foreach (var projectState in copyOfProjectStates)
						projectState.CleanUp();

					// Most likely this dictionary will already be empty now, but if any were showing
					// the splash screen, there is no handler to remove them.
					if (projectToKill == null)
						s_projectStates.Clear();
				}
			}
			finally {
				if (lockTaken) {
					Monitor.Exit(s_projectStates);
				}
			}
		}

		private void ShowStartupError(Exception e)
		{
			ErrorReport.ReportFatalException(e);
		}

		private void InitMainWindow(UNSQuestionsDialog mainWindow, TxlSplashScreen splashScreen, IProject project)
		{
			try
			{
				mainWindow.LoadTranslations(splashScreen);
			}
			catch (Exception e)
			{
				ShowStartupError(e);
				lock (s_projectStates)
				{
					if (s_projectStates.TryGetValue(project, out var state))
					{
						state.CleanUp();
						s_projectStates.Remove(project);
					}
				}
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

		private void Host_ShuttingDown(object sender, CancelEventArgs e)
		{
			lock (s_projectStates)
			{
				List<IProject> keysToDelete = new List<IProject>();
				foreach (var kvp in s_projectStates)
				{
					kvp.Value.RequestClose(e);
					if (e.Cancel)
						break;
					keysToDelete.Add(kvp.Key);
				}

				foreach (var key in keysToDelete)
					s_projectStates.Remove(key);

				// TXL-244: Analytics is a singleton and can't be re-created, so we only dispose it
				// when Paratext says it's shutting down.
				{
					s_analytics?.Dispose();
					s_analytics = null;
				}
			}
		}

		private static UserInfo GetUserInfo(IPluginHost host)
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
			s_userInfo = new UserInfo { FirstName = firstName, LastName = lastName,
				UILanguageCode = LocalizationManager.UILanguageId};
			return s_userInfo;
		}

		public static void UpdateUiLanguageForUser(string languageId)
		{
			s_userInfo.UILanguageCode = languageId;
			Analytics.IdentifyUpdate(s_userInfo);
			CreateCoreWebView2Environment(languageId);
		}

		/// <summary>
		/// Set up a writable folder for WebView2 (used currently for splash screen and Help About)
		/// to store local user data (cookies, etc.)
		/// </summary>
		/// <param name="language">The locale of the language used for controls in the browser
		/// (e.g., for the context menu).</param>
		private static async void CreateCoreWebView2Environment(string language)
		{
			try
			{
				var userDataFolder = Path.Combine(ProgramDataFolder, "WebView2");
				Directory.CreateDirectory(userDataFolder);
				var task = CoreWebView2Environment.CreateAsync(null, userDataFolder,
					new CoreWebView2EnvironmentOptions(null, language));

				await task.ContinueWith(t =>
				{
					WebView2Environment = t.Result;
				});
			}
			catch (Exception e)
			{
				Logger.WriteError(e);
			}
		}

		private static void SetUpLocalization(string desiredUiLangId)
		{
			var installedLocFolder = Path.Combine(InstallDir, "localization");
			var relativeSettingPathForLocFolder = Path.Combine(s_company, kPluginName);
			var icon = new Icon(GetFileDistributedWithApplication("TXL no TXL.ico"));

			// ENHANCE (L10nSharp): Not sure what the best way is to deal with this: the desired UI
			// language might be available in the XLIFF files for one of the localization managers
			// but not the other. Normally, part of the creation process for a LM is to check to
			// see whether the requested language is available. But if the first LM we create does
			// not have the requested language, the user sees a dialog box alerting them to that
			// and requiring them to choose a different language. For now, in Transcelerator, we
			// can work around that by creating the Palaso LM first, since its set of available
			// languages is a superset of the languages available for Transcelerator. (There is the
			// case of British English, but since that is still a flavor of "en" it's not a
			// problem.) But it feels weird not to create the primary LM first, and the day could
			// come where neither set of languages is a superset, and then this strategy wouldn't
			// work.

			LocalizationManager.Create(desiredUiLangId, "Palaso", "SIL Shared Strings", s_version,
				installedLocFolder, relativeSettingPathForLocFolder, icon,
				kEmailAddress, new [] {"SIL.Windows.Forms.Reporting", "SIL.Windows.Forms.ReleaseNotes"});

			var primaryMgr = LocalizationManager.Create(desiredUiLangId, kPluginName,
				kPluginName, s_version, installedLocFolder,
				relativeSettingPathForLocFolder, icon, kEmailAddress,
				new [] {"SIL.Transcelerator"});
			LocIncompleteViewModel = new TxlLocalizationIncompleteViewModel(primaryMgr,
				"transcelerator", IssueRequestForLocalization);
		}

		private static void IssueRequestForLocalization()
		{
			Analytics.Track("UI language request", LocIncompleteViewModel.StandardAnalyticsInfo);
		}

		public string GetDescription(string locale)
		{
			return LocalizationManager.GetString("Transcelerator.Description",
				"Assists in rapid translation of Scripture comprehension checking questions.",
				"This will be requested using the current Paratext UI locale",
				new [] {locale, LocalizationManager.UILanguageId}, out _);
		}

		public IDataFileMerger GetMerger(IPluginHost host, string dataIdentifier) =>
			host.GetXmlMerger(ParatextDataFileAccessor.GetXMLDataMergeInfo(dataIdentifier));

		public IEnumerable<PluginMenuEntry> PluginMenuEntries
		{
			get
			{
				yield return new PluginMenuEntry(Name + "...", Run, PluginMenuLocation.ScrTextTools,
					@"TXL no TXL.ico");
			}
		}

		public static string GetFileDistributedWithApplication(params string[] partsOfTheSubPath)
		{
			var path = partsOfTheSubPath.Aggregate(InstallDir, Path.Combine);

			if (File.Exists(path))
				return path;

			if (partsOfTheSubPath[0].Equals(kDocsFolder, StringComparison.OrdinalIgnoreCase))
				return null; // Help files are optional in installer

			throw new ApplicationException("Could not locate the required file, " + path);
		}

		public static string GetHelpFile(string filenameWithoutExtension)
		{
			var filename = filenameWithoutExtension + ".htm";
			return GetFileDistributedWithApplication(kDocsFolder,
					IetfLanguageTag.GetGeneralCode(LocalizationManager.UILanguageId), filename) ??
				GetFileDistributedWithApplication(kDocsFolder,
					kDefaultUILocale, filename);
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
				Analytics = null; // Singleton; this class is not responsible for disposing this.
			}

			public bool IsFor(UNSQuestionsDialog window) => MainWindow == window;

			public void Activate()
			{
				if (MainWindow == null)
					SplashScreen?.Activate();
				else
					InvokeOnUiThread(() => MainWindow.Activate());
			}

			/// <summary>
			/// Closes the main window if it has been set (in which case it is open)
			/// </summary>
			/// <returns><c>true</c> if the window was closed; <c>false</c> if the main window was not set,
			/// in which case the objects held by this state object are merely disposed.</returns>
			private bool Close()
			{
				if (MainWindow != null)
				{
					InvokeOnUiThread(() => MainWindow.Close());
					return true;
				}

				Dispose();
				return false;
			}
			
			/// <summary>
			/// Unconditionally close the main window or splash screen and dispose everything.
			/// </summary>
			/// <returns></returns>
			public void CleanUp()
			{
				if (SplashScreen != null)
				{
					SplashScreen.Close();
					Dispose();
				}
				else
					Close();
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

				mainWindow.Show(SplashScreen);
				SplashScreen.Dispose();
				SplashScreen = null;
			}

			public void RequestClose(CancelEventArgs cancelEventArgs)
			{
				if (MainWindow != null)
					MainWindow.RequestClose(cancelEventArgs);
				else
					SplashScreen?.Close();
			}
		}

		public bool ReportUnhandledException(Exception exception)
		{
			bool isFatal = false;

			try
			{
				if (!(exception is ParatextPluginException))
				{
					StackTrace stackTrace = new StackTrace(exception);
					var methodAtTopOfCallStack = stackTrace.GetFrames()?.LastOrDefault()?.GetMethod();
					isFatal = methodAtTopOfCallStack.Name == "Callback" && methodAtTopOfCallStack.DeclaringType == typeof(NativeWindow);
				}
			}
			catch (Exception e)
			{
				Host.Log(this, "Error occurred trying to determine if exception was fatal: " + e.Message);
				throw;
			}

			if (isFatal)
			{
				ErrorReport.ReportFatalException(exception);
				CleanUpForFatalException(s_currentProject);
			}
			else
				ErrorReport.ReportNonFatalException(exception);
			return true;
		}
	}
}
