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
using Paratext.PluginFramework;
using SILUBS.SharedScrUtils;

namespace SILUBS.PhraseTranslationHelper
{
    [Export(typeof(ParatextMenuPlugin))]
    [ExportMetadata("InsertAfterMenuName", "Tools|Text Converter")]
    [ExportMetadata("MenuText", "Transcelerator")]
    [ExportMetadata("RequiresActiveProject", true)]
    public class TxlPlugin : ParatextMenuPlugin
    {
        [Import("Project Name")]
        public Func<string> ProjectName { get; set; }

        [Import("Calling Application Name")]
        public Func<string> ApplicationName { get; set; }

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

        [Import("Current Reference")]
        public Func<IScrVers, int> CurrentRef { get; set; }

        [Import("Get Versification")]
        public Func<string, IScrVers> GetVersification { get; set; }

        public void HandleMenuClick()
        {
            IScrVers englishVersification = GetVersification("English");
            int currRef = CurrentRef(englishVersification);
            BCVRef startRef = new BCVRef(currRef);
            startRef.Chapter = 1;
            startRef.Verse = 1;
            BCVRef endRef = new BCVRef(currRef);
            endRef.Chapter = englishVersification.LastChapter(endRef.Book);
            endRef.Verse = englishVersification.LastVerse(endRef.Book, endRef.Chapter);

            var unsDlg = new UNSQuestionsDialog(ProjectName(), KeyTerms(), VernFont(),
                VernIcuLocale("generate templates"), VernRtoL(), DefaultLcfFolder(), ApplicationName(),
                englishVersification, startRef, endRef, b => { }, null, null);

            unsDlg.Show();
        }
    }
}
