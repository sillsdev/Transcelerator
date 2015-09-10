// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.   
// <copyright from='2013' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AddInSideViews;
using DesktopAnalytics;
using SIL.Keyboarding;
using SIL.Reporting;
using SIL.Scripture;
using SIL.Windows.Forms.Keyboarding;
using SIL.Windows.Forms.Reporting;

namespace SIL.Transcelerator
{
	[AddIn(pluginName, Description = "Assists in rapid translation of Scripture comprehension checking questions.",
		Version = "1.1", Publisher = "SIL International")]
	[QualificationData(PluginMetaDataKeys.menuText, pluginName + "...")]
	[QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|Text Converter")]
	[QualificationData(PluginMetaDataKeys.menuImagePath, @"Transcelerator\TXL no TXL.ico")]
	[QualificationData(PluginMetaDataKeys.enableWhen, WhenToEnable.scriptureProjectActive)]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.forEachActiveProject)]
	public class TxlPlugin : IParatextAddIn2
	{
		public const string pluginName = "Transcelerator";
	    private TxlSplashScreen splashScreen;
		private UNSQuestionsDialog unsMainWindow;
		private IHost host;
		private string projectName;

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

		public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
		{
			get { return ParatextDataFileAccessor.GetDataFileKeySpecifications(); }
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
                // variable is set to null, since in that case, the "real" splash scree is closed
                // and Activate is a no-op. But we do need to use a temp variable because it could
                // get set to null between the time we check for null and the call to Activate.
                TxlSplashScreen tempSplashScreen = splashScreen;
                if (tempSplashScreen != null)
                    tempSplashScreen.Activate();
            }
	    }

	    public void Run(IHost ptHost, string activeProjectName)
		{
            lock (this)
            {
                if (host != null)
                {
                    // This should never happen, but just in case Host does something wrong...
                    ptHost.WriteLineToLog(this, "Run called more than once!");
                    return;
                }
            }

			try
			{
				Application.EnableVisualStyles();

				host = ptHost;
				projectName = activeProjectName;
#if DEBUG
				MessageBox.Show("Attach debugger now (if you want to)", pluginName);
#endif
				ptHost.WriteLineToLog(this, "Starting " + pluginName);

				Thread mainUIThread = new Thread(() =>
				{
					InitializeErrorHandling();

                    const string kMajorList = "Major";

					UNSQuestionsDialog formToShow;
					lock (this)
					{
						splashScreen = new TxlSplashScreen();
					    splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
						splashScreen.Message = string.Format(
						    Properties.Resources.kstidSplashMsgRetrievingDataFromCaller, host.ApplicationName);

						int currRef = host.GetCurrentRef(TxlCore.kEnglishVersificationName);
						BCVRef startRef = new BCVRef(currRef);
						BCVRef endRef = new BCVRef(currRef);
					    bool useSavedRefRange = false;
                        // See TXL-131 for explanation of this code, if needed.
                        if (Properties.Settings.Default.FilterStartRef > 0 &&
                            Properties.Settings.Default.FilterStartRef < Properties.Settings.Default.FilterEndRef)
                        {
                            var savedStartRef = new BCVRef(Properties.Settings.Default.FilterStartRef);
                            var savedEndRef = new BCVRef(Properties.Settings.Default.FilterEndRef);
                            if (savedStartRef.Valid && savedEndRef.Valid &&
                                savedStartRef <= startRef && savedEndRef >= endRef)
                            {
                                useSavedRefRange = true;
                                startRef = savedStartRef;
                                endRef = savedEndRef;
                            }
                        }

                        if (!useSavedRefRange)
                        {
                            startRef.Chapter = 1;
                            startRef.Verse = 1;
                            endRef.Chapter = host.GetLastChapter(endRef.Book, TxlCore.kEnglishVersificationName);
                            endRef.Verse = host.GetLastVerse(endRef.Book, endRef.Chapter, TxlCore.kEnglishVersificationName);
                        }

						KeyboardController.Initialize();

						Action<bool> activateKeyboard = vern =>
						{
							if (vern)
							{
								try
								{
									string keyboard = host.GetProjectKeyboard(projectName);
									if (!string.IsNullOrEmpty(keyboard))
										Keyboard.Controller.GetKeyboard(keyboard).Activate();

								}
								catch (ApplicationException e)
								{
									// For some reason, the very first time this gets called it throws a COM exception, wrapped as
									// an ApplicationException. Mysteriously, it seems to work just fine anyway, and then all subsequent
									// calls work with no exception. Paratext seems to make this same call without any exceptions. The
									// documentation for ITfInputProcessorProfiles.ChangeCurrentLanguage (which is the method call
									// in PalasoUIWindowsForms that throws the COM exception says that an E_FAIL is an unspecified error,
									// so that's fairly helpful.
									if (!(e.InnerException is COMException))
										throw;
								}
							}
							else
								Keyboard.Controller.ActivateDefaultKeyboard();
						};

                        var fileAccessor = new ParatextDataFileAccessor(fileId => host.GetPlugInData(this, projectName, fileId),
                            (fileId, reader) => host.PutPlugInData(this, projectName, fileId, reader),
                            fileId => host.GetPlugInDataLastModifiedTime(this, projectName, fileId));

						bool fEnableDragDrop = true;
						try
						{
						    string dragDropSetting = host.GetApplicationSetting("EnableDragAndDrop");
						    if (dragDropSetting != null)
						        fEnableDragDrop = bool.Parse(dragDropSetting);
						}
						catch (Exception)
						{
						}
						formToShow = unsMainWindow = new UNSQuestionsDialog(splashScreen, projectName,
                            () => host.GetFactoryKeyTerms(kMajorList, "en", 01001001, 66022021),
                            termId => host.GetProjectTermRenderings(projectName, termId, true),
                            host.GetProjectFont(projectName),
						    host.GetProjectLanguageId(projectName, "generate templates"),
							host.GetProjectSetting(projectName, "Language"), host.GetProjectRtoL(projectName),
						    fileAccessor, host.GetScriptureExtractor(projectName, ExtractorType.USFX),
                            () => host.GetCssStylesheet(projectName), host.ApplicationName,
                            new ScrVers(host, TxlCore.kEnglishVersificationName),
						    new ScrVers(host, host.GetProjectVersificationName(projectName)), startRef,
						    endRef, currRef, activateKeyboard, termId => host.GetTermOccurrences(kMajorList, projectName, termId),
						    terms => host.LookUpKeyTerm(projectName, terms), fEnableDragDrop);
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
					using (new Analytics(key, GetuserInfo(), allowTracking))
					{
						Analytics.Track("Startup", new Dictionary<string, string>
						{{"Specific version", Assembly.GetExecutingAssembly().GetName().Version.ToString()}});
						formToShow.ShowDialog();
					}
					ptHost.WriteLineToLog(this, "Closing " + pluginName);
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
				MessageBox.Show("Error occurred attempting to start Transcelerator: " + e.Message);
				throw;
			}
		}

		private UserInfo GetuserInfo()
		{
			string lastName = host.UserName;
			string firstName = null;
			if (lastName != null)
			{
				var split = lastName.LastIndexOf(" ", StringComparison.Ordinal);
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

		private void InitializeErrorHandling()
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = "transcelerator_feedback@sil.org";
			ErrorReport.AddStandardProperties();
			// The version that gets added to the report by default is for the entry assembly, which is
			// AddInProcess32.exe. Even if if reported a version (which it doesn't), it wouldn't be very
			// useful.
			ErrorReport.AddProperty("Plugin Name", pluginName);
			Assembly assembly = Assembly.GetExecutingAssembly();
			ErrorReport.AddProperty("Version", string.Format("{0} (apparent build date: {1})",
				assembly.GetName().Version,
				File.GetLastWriteTime(assembly.Location).ToShortDateString()));
			ErrorReport.AddProperty("Host Application", host.ApplicationName + " " + host.ApplicationVersion);
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}
	}
}
