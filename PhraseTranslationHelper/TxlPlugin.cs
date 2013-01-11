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
using System.ComponentModel.Composition;
using System.Drawing;
using Paratext.PluginFramework;
using SILUBS.SharedScrUtils;

namespace SILUBS.PhraseTranslationHelper
{
    [Export(typeof(ParatextMenuPlugin))]
    [ExportMetadata("InsertAfterMenuName", "Tools|Text Converter")]
    [ExportMetadata("MenuText", "Transcelerator")]
    public class TxlPlugin : ParatextMenuPlugin
    {
        [Import("Project Name")]
        public string ProjectName { get; set; }

        [Import("Calling Application Name")]
        public string ApplicationName { get; set; }

        [Import("Key Terms")]
        public IKeyTerm[] KeyTerms { get; set; }

        [Import("Vernacular Font")]
        public Font VernFont { get; set; }

        [Import("Vernacular ICU Locale")]
        public string VernIcuLocale { get; set; }

        [Import("Vernacular Right-to-Left")]
        public bool VernRtoL { get; set; }

        [Import("LCF Folder")]
        public string DefaultLcfFolder { get; set; }

        [Import("Current Reference")]
        public int CurrentRef { get; set; }

        public void HandleMenuClick()
        {
            ScrReference startRef = new ScrReference(CurrentRef, ScrVers.Original);
            startRef.Chapter = 1;
            startRef.Verse = 1;
            ScrReference endRef = new ScrReference(CurrentRef, ScrVers.Original);
            endRef.Chapter = endRef.LastChapter;
            endRef.Verse = endRef.LastVerse;

            var unsDlg = new UNSQuestionsDialog(ProjectName, KeyTerms, VernFont,
                VernIcuLocale, VernRtoL, DefaultLcfFolder, ApplicationName,
                startRef, endRef, null, null, null);

            unsDlg.Show();
        }
    }
}
