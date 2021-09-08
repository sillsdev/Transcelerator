// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ScrReferenceFilterDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SIL.IO;
using SIL.Scripture;
using static System.String;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog to allow user to select a reference range on which to filter questions.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class ScrReferenceFilterDlg : Form
	{
		#region Data members
		private readonly BCVRef m_firstAvailableRef;
        private readonly BCVRef m_lastAvailableRef;
		private readonly string m_help;
		#endregion

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ScrReferenceFilterDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
        internal ScrReferenceFilterDlg(IScrVers versification, BCVRef initialFromRef, BCVRef initialToRef,
			int[] canonicalBookIds)
		{
			InitializeComponent();
			scrPsgFrom.Initialize(new BCVRef(initialFromRef), versification, canonicalBookIds);
            scrPsgTo.Initialize(new BCVRef(initialToRef), versification, canonicalBookIds);
            m_firstAvailableRef = new BCVRef(canonicalBookIds[0], 1, 1);
			m_lastAvailableRef = new BCVRef(canonicalBookIds.Last(), 1, 1);
            m_lastAvailableRef.Chapter = versification.GetLastChapter(m_lastAvailableRef.Book);
            m_lastAvailableRef.Verse = versification.GetLastVerse(m_lastAvailableRef.Book, m_lastAvailableRef.Chapter);
			if (initialFromRef == m_firstAvailableRef && initialToRef == m_lastAvailableRef)
				btnClearFilter.Enabled = false;

			m_help = FileLocationUtilities.GetFileDistributedWithApplication(true, "docs", "filtering.htm");
			HelpButton = !IsNullOrEmpty(m_help);
		}
		#endregion

		#region Properties
		private bool ReferencesSetToEntireScriptureRange =>
			scrPsgFrom.ScReference == m_firstAvailableRef && scrPsgTo.ScReference == m_lastAvailableRef;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the From reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public BCVRef FromRef => ReferencesSetToEntireScriptureRange?  BCVRef.Empty :
			scrPsgFrom.ScReference;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the To reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public BCVRef ToRef=> ReferencesSetToEntireScriptureRange ? BCVRef.Empty :
			scrPsgTo.ScReference;
		#endregion

		#region Event handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles change in the from passage
		/// </summary>
		/// <param name="newReference">The new reference.</param>
		/// ------------------------------------------------------------------------------------
		private void scrPsgFrom_PassageChanged(BCVRef newReference)
		{
			if (newReference != BCVRef.Empty && newReference > scrPsgTo.ScReference)
				scrPsgTo.ScReference = scrPsgFrom.ScReference;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles change in the from passage
		/// </summary>
		/// <param name="newReference">The new reference.</param>
		/// ------------------------------------------------------------------------------------
		private void scrPsgTo_PassageChanged(BCVRef newReference)
		{
			if (newReference != BCVRef.Empty && newReference < scrPsgFrom.ScReference)
				scrPsgFrom.ScReference = scrPsgTo.ScReference;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnClearFilter control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnClearFilter_Click(object sender, System.EventArgs e)
		{
            scrPsgFrom.ScReference = new BCVRef(m_firstAvailableRef);
            scrPsgTo.ScReference = new BCVRef(m_lastAvailableRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, System.ComponentModel.CancelEventArgs e)
		{
			HandleHelpRequest(sender, new HelpEventArgs(MousePosition));
		}

		private void HandleHelpRequest(object sender, HelpEventArgs args)
		{
			Process.Start(m_help);
		}
		#endregion
	}
}