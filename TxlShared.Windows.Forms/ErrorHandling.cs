// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2013' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SIL.Reporting;
using SIL.Windows.Forms.Reporting;

namespace SIL.Transcelerator
{
    public static class ErrorHandling
    {
		private static bool ErrorHandlingInitialized { get; set; }

		public static void Initialize(string hostAppName, Version hostVersion)
		{
			if (ErrorHandlingInitialized)
				return;
			ErrorHandlingInitialized = true;

			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = TxlConstants.kEmailAddress;
			ErrorReport.AddStandardProperties();
			// The version that gets added to the report by default is for the entry assembly, which is
			// AddInProcess32.exe. Even if if reported a version (which it doesn't), it wouldn't be very
			// useful.
			ErrorReport.AddProperty("Plugin Name", TxlConstants.kPluginName);
			Assembly assembly = Assembly.GetExecutingAssembly();
			ErrorReport.AddProperty("Version", $"{assembly.GetName().Version} (apparent build date: {File.GetLastWriteTime(assembly.Location).ToShortDateString()})");
			ErrorReport.AddProperty("Host Application", hostAppName + " " + hostVersion);
			// Note that the following is not thread-safe. In practice, this should be fine since
			// Paratext does not instantiate plugins in different threads.
			var existing = ExceptionHandler.TypeOfExistingHandler;
			if (existing == null)
				ExceptionHandler.Init(new WinFormsExceptionHandler(false));
			else
			{
				var msg = "ExceptionHandler already set (presumably by another plugin) to " +
					$"instance of {existing}";
				Logger.WriteEvent(msg);
#if DEBUG
				// Give developer a chance to explore this situation and determine if there will be
				// any negative implications.
				if (!typeof(WinFormsExceptionHandler).IsAssignableFrom(existing))
					MessageBox.Show(msg, TxlConstants.kPluginName);
#endif
			}
		}
	}
}
