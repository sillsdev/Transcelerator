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
using SIL.Transcelerator;
using SIL.Windows.Forms.Reporting;

namespace SIL.TxlMasterQuestionPreProcessor
{
	[PublicAPI]
	public class TxlMasterQuestionPreProcessorPlugin : IParatextStandalonePlugin
	{
        public const string pluginName = "Transcelerator Question Pre-Processor";

		public IDataFileMerger GetMerger(IPluginHost host, string dataIdentifier)
		{
			throw new NotImplementedException();
		}

		public string Name => pluginName;
		public Version Version => new Version(2, 0);
		public string VersionString => Version.ToString();
		public string Publisher => "SIL International";
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
				host.Log(this, "Starting " + pluginName);

				TxlCore.InitializeErrorHandling(host.ApplicationName, host.ApplicationVersion);

				var formToShow = new TxlMasterQuestionPreProcessorForm(host.GetStandardVersification(StandardScrVersType.English));
				formToShow.Show();
				
				host.Log(this, "Closing " + pluginName);
			}
			catch (Exception e)
			{
                MessageBox.Show("Error occurred attempting to start Transcelerator Question Pre-Processor: " + e.Message);
				throw;
			}
		}

		public string GetDescription(string locale) =>
			"Prepares the master question file for Transcelerator - not for distribution to end-users.";
	}
}
