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
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using Paratext.PluginInterfaces;
using SIL.Scripture;
using SIL.Windows.Forms.Scripture;
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
		private readonly IProject m_project;
		private readonly IVerseRef m_firstAvailableRef;
        private readonly IVerseRef m_lastAvailableRef;
		private readonly string m_help;
		private readonly Color m_origWarningLabelColor;
		private readonly int m_initialDelay;
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
			scrPsgFrom.VerseControl.VerseRef = new ScrVerseRefAdapter(initialFromRef, project);
            scrPsgTo.VerseControl.VerseRef = new ScrVerseRefAdapter(initialToRef, project);
			if (initialFromRef.Equals(m_firstAvailableRef) && initialToRef.Equals(m_lastAvailableRef))
				btnClearFilter.Enabled = false;

			m_origWarningLabelColor = m_lblInvalidReference.ForeColor;
			m_initialDelay = m_timerWarning.Interval;

			m_help = TxlPlugin.GetHelpFile("filtering");
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
		/// Handles leaving the to or from passage
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void OnScrPassageLeave(object sender, EventArgs e)
		{
			var psgCtrl = (ToolStripVerseControl)sender;
			try
			{
				psgCtrl.VerseControl.AcceptData();
				ScrPassageChanged(sender, new PropertyChangedEventArgs(psgCtrl.Name));
			}
			catch (Exception)
			{
				btnOk.DialogResult = DialogResult.None;
				btnOk.Enabled = false;

				m_timerWarning.Stop();
				SystemSounds.Beep.Play();
				psgCtrl.VerseControl.VerseRef = psgCtrl.VerseControl.VerseRef;

				// reset variables and kick off fade operation
				m_lblInvalidReference.ForeColor = m_origWarningLabelColor;
				m_timerWarning.Interval = m_initialDelay;
				m_lblInvalidReference.Show();
				m_timerWarning.Start();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles change in the to or from passage
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
            scrPsgFrom.VerseControl.VerseRef = new ScrVerseRefAdapter(m_firstAvailableRef, m_project);
            scrPsgTo.VerseControl.VerseRef = new ScrVerseRefAdapter(m_lastAvailableRef, m_project);
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

		private void ResetOkButtonAndStartToFadeWarning(object sender, EventArgs e)
		{
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Enabled = true;

			// timer interval set to 10 to ensure smooth fading
			m_timerWarning.Interval = 10;
			m_timerWarning.Tick -= ResetOkButtonAndStartToFadeWarning;
			m_timerWarning.Tick += FadeWarning;

			FadeWarning(sender, e);
		}

		private void FadeWarning(object sender, EventArgs e)
		{
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Enabled = true;

			// timer interval set to 10 to ensure smooth fading
			m_timerWarning.Interval = 10;

			int r = m_lblInvalidReference.ForeColor.R;
			int g = m_lblInvalidReference.ForeColor.G;
			int b = m_lblInvalidReference.ForeColor.B;
			var back = BackColor;

			if (r < back.R)
				r++;
			else if (r > back.R)
				r--;
			if (g < back.G)
				g++;
			else if (g > back.G)
				g--;
			if (b < back.B)
				b++;
			else if (b > back.B)
				b--;

			m_lblInvalidReference.ForeColor = Color.FromArgb(255, r, g, b);

			if (r == back.R && g == back.G && b == back.B) // arrived at target
			{
				// fade is complete
				m_timerWarning.Stop();
				m_timerWarning.Tick -= FadeWarning;
				// For next time...
				m_timerWarning.Tick += ResetOkButtonAndStartToFadeWarning;
				m_lblInvalidReference.Visible = false;
			}
		}
		#endregion

		#region Private helper methods
		private IVerseRef GetRef(IScrVerseRef verseRef) =>
			m_firstAvailableRef.Versification.CreateReference(verseRef.BookNum, verseRef.ChapterNum, verseRef.VerseNum);
		#endregion
	}
}