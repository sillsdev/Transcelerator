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
using System.Threading;
using System.Windows.Forms;
using AddInSideViews;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Reporting;
using SIL.Transcelerator;

namespace SIL.TxlMasterQuestionPreProcessor
{
	[AddIn(pluginName, Description = "Prepares the master question file for Transcelerator - not for distribution to end-users.",
		Version = "1.0", Publisher = "SIL International")]
	[QualificationData(PluginMetaDataKeys.menuText, pluginName + "...")]
	[QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|Transcelerator")]
	[QualificationData(PluginMetaDataKeys.enableWhen, WhenToEnable.always)]
	public class TxlMasterQuestionPreProcessorPlugin : IParatextAddIn
	{
        public const string pluginName = "Transcelerator Question Pre-Processor";
		private IHost host;
        private TxlMasterQuestionPreProcessorForm unsMainWindow;
		public void RequestShutdown()
		{
            InvokeOnMainWindowIfNotNull(delegate 
            {
                unsMainWindow.Activate();
                unsMainWindow.Close();
            });
		}

	    public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
	    {
	        get { return null; }
	    }

	    public void Run(IHost ptHost, string activeProjectName)
		{
			try
			{
				host = ptHost;
//#if DEBUG
//                MessageBox.Show("Attach debugger now (if you want to)", pluginName);
//#endif
				ptHost.WriteLineToLog(this, "Starting " + pluginName);

				Thread mainUIThread = new Thread(() =>
				{
					InitializeErrorHandling();

					TxlMasterQuestionPreProcessorForm formToShow;
					lock (this)
					{
						formToShow = unsMainWindow = new TxlMasterQuestionPreProcessorForm(
						    new ScrVers(host, TxlCore.englishVersificationName));
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
                MessageBox.Show("Error occurred attempting to start Transcelerator Question Pre-Processor: " + e.Message);
				throw;
			}
		}

		private bool InvokeOnMainWindowIfNotNull(Action action)
		{
			lock (this)
			{
				if (unsMainWindow != null)
				{
					if (unsMainWindow.InvokeRequired)
						unsMainWindow.Invoke(action);
					else
						action();
					return true;
				}
			}
			return false;
		}

		private void InitializeErrorHandling()
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = "transcelerator_feedback@sil.org";
			ErrorReport.AddStandardProperties();
			ErrorReport.AddProperty("Host Application", host.ApplicationName + " " + host.ApplicationVersion);
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}
	}
}
