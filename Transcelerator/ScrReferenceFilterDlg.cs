// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: ScrReferenceFilterDlg.cs
// ---------------------------------------------------------------------------------------------
using System.Linq;
using System.Windows.Forms;
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog to present user with options for generating an LCF file to use for generating a
	/// printable script to do comprehension checking.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class ScrReferenceFilterDlg : Form
	{
		#region Data members
		private readonly BCVRef m_firstAvailableRef;
        private readonly BCVRef m_lastAvailableRef;
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
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the From reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public BCVRef FromRef
		{
			get { return scrPsgFrom.ScReference; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the To reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public BCVRef ToRef
		{
			get { return scrPsgTo.ScReference; }
		}
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
		#endregion
	}
}