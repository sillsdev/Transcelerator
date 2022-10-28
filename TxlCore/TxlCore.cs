// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2013' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
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
using SIL.Scripture;
using SIL.Windows.Forms.Reporting;

namespace SIL.Transcelerator
{
    public static class TxlCore
    {
		public const string kPluginName = "Transcelerator";
		public const string kQuestionsFilename = "TxlQuestions.xml";
		public const string kKeyTermRulesFilename = "keyTermRules.xml";
		public const string kQuestionWordsFilename = "TxlQuestionWords.xml";
		public const string kEmailAddress = "transcelerator_feedback@sil.org";

		private static bool ErrorHandlingInitialized { get; set; }

      	/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Parses the given string to get a starting and ending Scripture reference.
		/// </summary>
		/// <param name="sReference">The string-representation of the Scripture reference.</param>
		/// <param name="startRef">The start reference.</param>
		/// <param name="endRef">The end reference.</param>
		/// ------------------------------------------------------------------------------------
		public static void ParseRefRange(this string sReference, out BCVRef startRef, out BCVRef endRef)
        {
            startRef = new BCVRef();
            endRef = new BCVRef();
            BCVRef.ParseRefRange(sReference, ref startRef, ref endRef);
        }

		public static void InitializeErrorHandling(string hostAppName, Version hostVersion)
		{
			if (ErrorHandlingInitialized)
				return;
			ErrorHandlingInitialized = true;

			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = kEmailAddress;
			ErrorReport.AddStandardProperties();
			// The version that gets added to the report by default is for the entry assembly, which is
			// AddInProcess32.exe. Even if if reported a version (which it doesn't), it wouldn't be very
			// useful.
			ErrorReport.AddProperty("Plugin Name", kPluginName);
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
					MessageBox.Show(msg, kPluginName);
#endif
			}
		}
	}
}
