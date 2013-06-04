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
using System.Diagnostics;
namespace SIL.Transcelerator.Properties
{
    /// <summary>
    /// This allows upgrading of the user settings (when the version changes).
    /// </summary>
    internal sealed partial class Settings {
        
        public Settings()
        {
            SettingsLoaded += Settings_SettingsLoaded;
        }

        void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            if (UpgradeNeeded)
            {
                try
                {
                    Upgrade();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    // Ignore upgrade errors
                }
                UpgradeNeeded = false;
                Save();
            }
        }
    }
}
