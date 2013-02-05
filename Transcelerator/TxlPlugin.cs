// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.   
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: UNSQuestionsDialog.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using Paratext.PluginFramework;
using SILUBS.SharedScrUtils;

namespace SILUBS.Transcelerator
{
    [Export(typeof(ParatextMenuPlugin))]
    [ExportMetadata("InsertAfterMenuName", "Tools|Text Converter")]
    [ExportMetadata("MenuText", "Transcelerator")]
    [ExportMetadata("RequiresActiveProject", true)]
    public class TxlPlugin : ParatextMenuPlugin
    {
        [Import("Project Name")]
        public Func<string> ProjectName { get; set; }

        [Import("Application Name")]
        public string CallingApplicationName { get; set; }

        [Import("Key Terms")]
        public Func<IEnumerable<IKeyTerm>> KeyTerms { get; set; }

        [Import("Vernacular Font")]
        public Func<Font> VernFont { get; set; }

        [Import("Get Vernacular ICU Locale")]
        public Func<string, string> VernIcuLocale { get; set; }

        [Import("Vernacular Right-to-Left")]
        public Func<bool> VernRtoL { get; set; }

        [Import("LCF Folder")]
        public Func<string> DefaultLcfFolder { get; set; }

        [Import("Project Data Folder")]
        public Func<string> ProjectDataFolder { get; set; }

        [Import("Get Current Reference")]
        public Func<IScrVers, int> GetCurrentRef { get; set; }

        [Import("Get Versification")]
        public Func<string, IScrVers> GetVersification { get; set; }

        [Import("Display Key Term")]
        public Action<string, IEnumerable<IKeyTerm>> DisplayKeyTerm { get; set; }

        public void HandleMenuClick()
        {
            TxlSplashScreen splashScreen = new TxlSplashScreen();
            splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
            splashScreen.Message = string.Format(
                Properties.Resources.kstidSplashMsgRetrievingDataFromCaller, CallingApplicationName);

            IScrVers englishVersification = GetVersification("English");
            int currRef = GetCurrentRef(englishVersification);
            BCVRef startRef = new BCVRef(currRef);
            startRef.Chapter = 1;
            startRef.Verse = 1;
            BCVRef endRef = new BCVRef(currRef);
            endRef.Chapter = englishVersification.LastChapter(endRef.Book);
            endRef.Verse = englishVersification.LastVerse(endRef.Book, endRef.Chapter);
            string projectName = ProjectName();

            var unsDlg = new UNSQuestionsDialog(splashScreen, projectName, KeyTerms(), VernFont(),
                VernIcuLocale("generate templates"), VernRtoL(), ProjectDataFolder(),
                DefaultLcfFolder(), CallingApplicationName, englishVersification, startRef,
                endRef, b => { }, terms => DisplayKeyTerm(projectName, terms));

            unsDlg.Show();
        }
    }
}
