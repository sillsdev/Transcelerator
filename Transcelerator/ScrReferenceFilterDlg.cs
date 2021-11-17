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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Paratext.PluginInterfaces;
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
		private readonly IProject m_project;

		#region Data members
		private readonly IVerseRef m_firstAvailableRef;
        private readonly IVerseRef m_lastAvailableRef;
		private readonly string m_help;
		#endregion

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ScrReferenceFilterDlg"/> class.
		/// </summary>
		/// <param name="project"></param>
		/// ------------------------------------------------------------------------------------
        internal ScrReferenceFilterDlg(IProject project, IVerseRef initialFromRef,
			IVerseRef initialToRef, int[] canonicalBookIds)
		{
			m_project = project;
			InitializeComponent();
			scrPsgTo.VerseControl.VerseRefChanged += ScrPassageChanged;
			scrPsgFrom.VerseControl.VerseRefChanged += ScrPassageChanged;

			var versification = project.Versification;
			var bookSet = new BookSet(canonicalBookIds);
            m_firstAvailableRef = versification.CreateReference(bookSet.FirstSelectedBookNum, 1, 1);
			var lastBook = bookSet.LastSelectedBookNum;
			var lastChapter = versification.GetLastChapter(lastBook);
			m_lastAvailableRef = versification.CreateReference(lastBook, lastChapter, versification.GetLastVerse(lastBook, lastChapter));
			if (initialFromRef == null)
				initialFromRef = versification.CreateReference(m_firstAvailableRef.BBBCCCVVV);
			if (initialToRef == null)
				initialToRef = versification.CreateReference(m_lastAvailableRef.BBBCCCVVV);

			scrPsgFrom.VerseControl.BooksPresentSet = scrPsgTo.VerseControl.BooksPresentSet = bookSet;
			scrPsgFrom.VerseControl.ShowEmptyBooks = false;
			scrPsgTo.VerseControl.ShowEmptyBooks = false;
			scrPsgFrom.VerseControl.VerseRef = new ScrVersRefAdapter(initialFromRef, project);
            scrPsgTo.VerseControl.VerseRef = new ScrVersRefAdapter(initialToRef, project);
			if (initialFromRef.Equals(m_firstAvailableRef) && initialToRef.Equals(m_lastAvailableRef))
				btnClearFilter.Enabled = false;

			m_help = TxlPlugin.GetFileDistributedWithApplication("docs", "filtering.htm");
			HelpButton = !IsNullOrEmpty(m_help);
		}
		#endregion

		#region Properties
		private bool ReferencesSetToEntireScriptureRange =>
			GetRef(scrPsgFrom.VerseControl.VerseRef).Equals(m_firstAvailableRef) &&
			GetRef(scrPsgTo.VerseControl.VerseRef).Equals(m_lastAvailableRef);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the From reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IVerseRef FromRef => ReferencesSetToEntireScriptureRange ? null : GetRef(scrPsgFrom.VerseControl.VerseRef);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the To reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IVerseRef ToRef=> ReferencesSetToEntireScriptureRange ? null : GetRef(scrPsgTo.VerseControl.VerseRef);
		#endregion

		#region Event handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles change in the from passage
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void ScrPassageChanged(object sender, PropertyChangedEventArgs e)
		{
			var fromReference = GetRef(scrPsgFrom.VerseControl.VerseRef);
			var toReference = GetRef(scrPsgTo.VerseControl.VerseRef);
			if (fromReference.CompareTo(toReference) > 0)
			{
				if (sender == scrPsgFrom.VerseControl)
					scrPsgTo.VerseControl.VerseRef = scrPsgFrom.VerseControl.VerseRef.Clone();
				else
					scrPsgFrom.VerseControl.VerseRef = scrPsgTo.VerseControl.VerseRef.Clone();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnClearFilter control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnClearFilter_Click(object sender, System.EventArgs e)
		{
            scrPsgFrom.VerseControl.VerseRef = new ScrVersRefAdapter(m_firstAvailableRef, m_project);
            scrPsgTo.VerseControl.VerseRef = new ScrVersRefAdapter(m_lastAvailableRef, m_project);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, CancelEventArgs e)
		{
			HandleHelpRequest(sender, new HelpEventArgs(MousePosition));
		}

		private void HandleHelpRequest(object sender, HelpEventArgs args)
		{
			if (!IsNullOrEmpty(m_help))
				Process.Start(m_help);
		}
		#endregion

		#region Private helper methods
		private IVerseRef GetRef(IScrVerseRef verseRef) =>
			m_firstAvailableRef.Versification.CreateReference(verseRef.BookNum, verseRef.ChapterNum, verseRef.VerseNum);
		#endregion
	}
}