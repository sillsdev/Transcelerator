// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2013' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JetBrains.Annotations;
using Paratext.PluginInterfaces;
using SIL.Reporting;
using SIL.Windows.Forms.Reporting;

namespace SIL.TxlMasterQuestionPreProcessor
{
	[PublicAPI]
	public class TxlMasterQuestionPreProcessorPlugin : IParatextStandalonePlugin
	{
        public const string pluginName = "Transcelerator Question Pre-Processor";

		public string Name => pluginName;
		public Version Version => new Version(2, 0);
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";
		public IEnumerable<KeyValuePair<string, XMLDataMergeInfo>> MergeDataInfo => null;
		public IEnumerable<PluginMenuEntry> PluginMenuEntries
		{
			get
			{
				yield return new PluginMenuEntry(pluginName + "...", Run, PluginMenuLocation.MainDefault);
			}
		}

		public void Run(IPluginHost host, IParatextChildState state)
		{
			try
			{
//#if DEBUG
//                MessageBox.Show("Attach debugger now (if you want to)", pluginName);
//#endif
				host.Log(this, "Starting " + pluginName);

				InitializeErrorHandling(host);

				TxlMasterQuestionPreProcessorForm formToShow;
				lock (this)
				{
					formToShow = new TxlMasterQuestionPreProcessorForm(host.GetStandardVersification(StandardScrVersType.English));
				}
				formToShow.ShowDialog();
				
				host.Log(this, "Closing " + pluginName);
			}
			catch (Exception e)
			{
                MessageBox.Show("Error occurred attempting to start Transcelerator Question Pre-Processor: " + e.Message);
				throw;
			}
		}

		private void InitializeErrorHandling(IPluginHost host)
		{
			ErrorReport.SetErrorReporter(new WinFormsErrorReporter());
			ErrorReport.EmailAddress = "transcelerator_feedback@sil.org";
			ErrorReport.AddStandardProperties();
			ErrorReport.AddProperty("Host Application", host.ApplicationName + " " + host.ApplicationVersion);
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}

		public string GetDescription(string locale) =>
			"Prepares the master question file for Transcelerator - not for distribution to end-users.";
	}
}
