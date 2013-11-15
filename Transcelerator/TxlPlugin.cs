// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.   
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
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
using System.Threading;
using System.Windows.Forms;
using AddInSideViews;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Reporting;
using SILUBS.SharedScrUtils;
using Palaso.UI.WindowsForms.Keyboarding;

namespace SIL.Transcelerator
{
	[AddIn(pluginName, Description = "Assists in rapid translation of Scripture comprehension checking questions.",
		Version = "1.0", Publisher = "SIL International")]
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

						int currRef = host.GetCurrentRef(TxlCore.englishVersificationName);
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
                            endRef.Chapter = host.GetLastChapter(endRef.Book, TxlCore.englishVersificationName);
                            endRef.Verse = host.GetLastVerse(endRef.Book, endRef.Chapter, TxlCore.englishVersificationName);
                        }

						Action<bool> activateKeyboard = vern =>
						{
							string keyboard = host.GetProjectKeyboard(vern ? projectName : null);
							if (!string.IsNullOrEmpty(keyboard))
								KeyboardController.ActivateKeyboard(keyboard);
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
						    host.GetProjectLanguageId(projectName, "generate templates"), host.GetProjectRtoL(projectName),
						    fileAccessor, host.GetScriptureExtractor(projectName, ExtractorType.USFX),
                            () => host.GetCssStylesheet(projectName), host.ApplicationName,
                            new ScrVers(host, TxlCore.englishVersificationName),
						    new ScrVers(host, host.GetProjectVersificationName(projectName)), startRef,
						    endRef, currRef, activateKeyboard, termId => host.GetTermOccurrences(kMajorList, projectName, termId),
						    terms => host.LookUpKeyTerm(projectName, terms), fEnableDragDrop);
					    splashScreen = null;
					}
					formToShow.ShowDialog();
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
