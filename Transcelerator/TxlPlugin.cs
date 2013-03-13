// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.   
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AddInSideViews;
using Palaso.Reporting;
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
    [AddIn(pluginName, Description = "Assists in rapid translation of Scripture comprehension checking questions.",
        Version = "1.0", Publisher = "SIL International")]
    [QualificationData(PluginMetaDataKeys.menuText, pluginName + "...")]
    [QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|Text Converter")]
    [QualificationData(PluginMetaDataKeys.menuImagePath, @"Transcelerator\TXL no TXL.ico")]
    [QualificationData(PluginMetaDataKeys.enableWhen, WhenToEnable.scriptureProjectActive)]
    public class TxlPlugin : IParatextAddIn
    {
        public const string pluginName = "Transcelerator";
        private UNSQuestionsDialog unsMainWindow;
        private IHost host;
        private string projectName;

        public void RequestShutdown()
        {
            lock (this)
            {
                if (unsMainWindow != null)
                {
                    unsMainWindow.Activate();
                    unsMainWindow.Close();
                }
            }
        }

        public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
        {
            get { return ParatextDataFileProxy.GetDataFileKeySpecifications(); }
        }

        public void Run(IHost ptHost, string activeProjectName)
        {
            try
            {
                host = ptHost;
                projectName = activeProjectName;

                MessageBox.Show("Attach debugger now (if you want to)");

                InitializeErrorHandling();

                UNSQuestionsDialog formToShow;
                lock (this)
                {
                    if (unsMainWindow != null)
                    {
                        // This should never happen
                        unsMainWindow.Activate();
                        return;
                    }
                    TxlSplashScreen splashScreen = new TxlSplashScreen();
                    splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
                    splashScreen.Message = string.Format(
                        Properties.Resources.kstidSplashMsgRetrievingDataFromCaller, host.ApplicationName);

                    int currRef = host.GetCurrentRef(UNSQuestionsDialog.englishVersificationName);
                    BCVRef startRef = new BCVRef(currRef);
                    startRef.Chapter = 1;
                    startRef.Verse = 1;
                    BCVRef endRef = new BCVRef(currRef);
                    endRef.Chapter = host.GetLastChapter(endRef.Book, UNSQuestionsDialog.englishVersificationName);
                    endRef.Verse = host.GetLastVerse(endRef.Book, endRef.Chapter, UNSQuestionsDialog.englishVersificationName);

                    formToShow = unsMainWindow = new UNSQuestionsDialog(splashScreen, projectName,
                        host.GetKeyTerms(projectName, "en"), host.GetProjectFont(projectName),
                        host.GetProjectLanguageId(projectName, "generate templates"), host.GetProjectRtoL(projectName),
                        new ParatextDataFileProxy(fileId => host.GetPlugInData(this, projectName, fileId),
                            (fileId, reader) => host.PutPlugInData(this, projectName, fileId, reader)),
                        host.GetScriptureExtractor(projectName, ExtractorType.USFX), host.ApplicationName,
                        new ScrVers(host, UNSQuestionsDialog.englishVersificationName),
                        new ScrVers(host, host.GetProjectVersificationName(projectName)), startRef,
                        endRef, b => { }, terms => host.LookUpKeyTerm(projectName, terms.Select(t => t.Id).ToList()));
                }
                Application.Run(formToShow);
            }
            catch (Exception e)
            {
                ErrorReport.ReportFatalException(e);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void InitializeErrorHandling()
        {
            ErrorReport.EmailAddress = "tom_bogle@sil.org";
            ErrorReport.AddStandardProperties();
            ExceptionHandler.Init();
        }

        private class ScrVers : IScrVers
        {
            private readonly IHost host;
            private readonly string versificationName;

            public ScrVers(IHost host, string versificationName)
            {
                this.host = host;
                this.versificationName = versificationName;
            }

            public int GetLastChapter(int bookNum)
            {
                return host.GetLastChapter(bookNum, versificationName);
            }

            public int GetLastVerse(int bookNum, int chapterNum)
            {
                return host.GetLastVerse(bookNum, chapterNum, versificationName);
            }

            public int ChangeVersification(int reference, IScrVers scrVersSource)
            {
                return this == scrVersSource ? reference :
                    host.ChangeVersification(reference, ((ScrVers)scrVersSource).versificationName, versificationName);
            }
        }
    }
}
