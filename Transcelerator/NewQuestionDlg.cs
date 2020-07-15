// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2017, SIL International.
// <copyright from='2012' to='2017' company='SIL International'>
//		Copyright (c) 2017, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: NewQuestionDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIL.Extensions;
using SIL.ObjectModel;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog box for adding a new question
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	internal partial class NewQuestionDlg : Form
	{
		private readonly IScrVers m_projectVersification;
		private readonly IScrVers m_masterVersification;
		private readonly string m_vernLanguage;
		private readonly TransceleratorSections m_sectionInfo;
		private readonly PhraseTranslationHelper m_ptHelper;
		private int m_previouslySelectedRow = -1;
		private string m_locationFormat;
		private BCVRef m_existingStartRef;
		private List<KeyValuePair<int, ISectionInfo>> m_currentSections;
		private List<TranslatablePhrase> m_existingPhrasesInCurrentSections;
		private List<TranslatablePhrase> m_currentExistingPhrases;
		private int m_existingEndVerse;
		private int m_insertionLocation;
		private Action<bool> m_changeKeyboard;

		#region Properties
		public string EnglishQuestion => m_chkNoEnglish.Checked ? null : m_txtEnglishQuestion.Text;

		public string Answer => m_txtAnswer.Text;

		public string Translation => m_txtVernacularQuestion.Text;

		private string ReferenceInProjectVersification
		{
			get
			{
				var startRef = m_scrPsgReference.ScReference;
				var endRef = new BCVRef(startRef);
				endRef.Verse = EndVerse;
				return BCVRef.MakeReferenceString(startRef, endRef, ".", "-");
			}
		}

		/// <summary>
		/// Starting reference (in master versification)
		/// </summary>
		public BCVRef StartReference =>
			new BCVRef(m_masterVersification.ChangeVersification(m_scrPsgReference.ScReference, m_projectVersification));

		/// <summary>
		/// Ending reference (in master versification)
		/// </summary>
		public BCVRef EndReference
		{
			get
			{
				var endRef = m_scrPsgReference.ScReference;
				endRef.Verse = EndVerse;
				return new BCVRef(m_masterVersification.ChangeVersification(endRef, m_projectVersification));
			}
		}

		/// <summary>
		/// Starting verse number (in project versification - i.e., matches what the user sees)
		/// </summary>
		private int StartVerse => m_scrPsgReference.ScReference.Verse;

		/// <summary>
		/// Ending verse number (in project versification - i.e., matches what the user sees)
		/// </summary>
		private int EndVerse
		{
			get
			{
				if (m_cboEndVerse.SelectedIndex <= 1)
					return StartVerse;
				return m_cboEndVerse.SelectedIndex - 1 + StartVerse;
			}
			set
			{
				var index = value + 1 - StartVerse;
				if (index >= m_cboEndVerse.Items.Count || index < 0)
					index = 0;

				if (index != m_cboEndVerse.SelectedIndex)
				{
					if (value != StartVerse || m_cboEndVerse.SelectedIndex != 0)
						m_cboEndVerse.SelectedIndex = index;
				}
			}
		}

		public int Category => m_cboCategory.SelectedIndex;

		public int SequenceNumber
		{
			get
			{
				if (m_dataGridViewExistingQuestions.RowCount == 0)
					return 1;
				
				return ((TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[0].DataBoundItem).SequenceNumber +
					SelectedInsertionLocation;
			}
		}

		private int SelectedInsertionLocation
		{
			get => m_insertionLocation;
			set
			{
				m_insertionLocation = value;
				m_btnUp.Enabled = value > 0;
				m_btnDown.Enabled = value < m_dataGridViewExistingQuestions.RowCount;
			}
		}

		public int OwningSection
		{
			get
			{
				if (m_currentSections.Count == 1)
					return m_sectionInfo.AllSections.IndexOf(m_currentSections[0]);
				if (BasePhrase != null)
					return BasePhrase.SectionIndex;
				// This is the very rare (probably nonexistent) case where a verse
				// is split across two sections and a question is being added to a
				// category that does not have any existing questions. If the end
				// ref is greater than the start ref, the new question must belong
				// to the second section. If it's same, then we can't know which of
				// the two sections is intended. But we can safely - albeit
				// arbitrarily choose the 2nd one.
				return m_sectionInfo.AllSections.IndexOf(m_currentSections.Last());
			}
		}

		/// <summary>
		/// Gets the phrase/question that the new question is to hang off of. Although it would seem
		/// that this should be the selected one in the UI, in fact, we always return the one BEFORE
		/// the insertion position except when inserting at the beginning of a category. Inserting
		/// before an existing question causes the question to get added twice, and that's not good.
		/// </summary>
		public TranslatablePhrase BasePhrase
		{
			get
			{
				if (m_dataGridViewExistingQuestions.SelectedRows.Count == 0)
					return null;
				var row = m_dataGridViewExistingQuestions.CurrentCellAddress.Y;
				Debug.Assert(row >= 0);
				Debug.Assert(SelectedInsertionLocation == row || SelectedInsertionLocation == row + 1,
					"Insertion must be either immediately before or immediately after current (selected) row");
				if (row > 0 && !InsertBeforeBasePhrase)
					row--;

				//if (SelectedInsertionLocation > 0 && SelectedInsertionLocation == row)
				//{
				//	// See if we should insert after the row before the selected one instead.
				//	if (m_currentSections.Count == 1)
				//		row--;
				//	else
				//	{
				//		// This handles the special case when we are inserting a question for a
				//		// verse that spans two sections. In this case, the section corresponding to the
				//		// selected question is the one we want to add the new question to.
				//		var selectedQuestion = (TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[row].DataBoundItem;
				//		var prevQuestion = (TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[row -1].DataBoundItem;
				//		if (selectedQuestion.SectionIndex == prevQuestion.SectionIndexy)
				//		{
				//			row--;
				//		}
				//	}
				//}

				return (TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[row].DataBoundItem;
			}
		}

		public bool InsertBeforeBasePhrase
		{
			get
			{
				if (SelectedInsertionLocation == 0)
					return true;
				if (m_currentSections.Count == 1)
					return false;

				// This handles the special case when we are inserting a question for a
				// verse that spans two sections. In this case, the section corresponding to the
				// selected question is the one we want to add the new question to.
				var row = m_dataGridViewExistingQuestions.CurrentCellAddress.Y;
				if (row == 0)
					return false;
				var selectedQuestion = (TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[row].DataBoundItem;
				var prevQuestion = (TranslatablePhrase)m_dataGridViewExistingQuestions.Rows[row -1].DataBoundItem;
				return selectedQuestion.SectionIndex != prevQuestion.SectionIndex;
			}
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NewQuestionDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal NewQuestionDlg(TranslatablePhrase basePhrase, string vernLanguage,
			TransceleratorSections sectionInfo, IScrVers projectVersification, IScrVers masterVersification,
			PhraseTranslationHelper pth, int[] canonicalBookIds, Action<bool> changeKeyboard)
		{
			var baseQuestion = basePhrase.QuestionInfo;
			m_vernLanguage = vernLanguage;
			m_sectionInfo = sectionInfo;
			m_projectVersification = projectVersification;
			m_masterVersification = masterVersification;
			m_ptHelper = pth;
			m_changeKeyboard = changeKeyboard;
			InitializeComponent();

			HandleStringsLocalized();

			var startref = m_projectVersification.ChangeVersification(baseQuestion.StartRef, m_masterVersification);
			m_scrPsgReference.Initialize(new BCVRef(startref), m_projectVersification, canonicalBookIds);
			m_existingStartRef = m_scrPsgReference.ScReference;
			SetCurrentSections();
			var endref = m_projectVersification.ChangeVersification(baseQuestion.EndRef, m_masterVersification);
			m_existingEndVerse = BCVRef.GetVerseFromBcv(endref);
			PopulateEndRefComboBox();

			PopulateCategoryComboBox();
			if (basePhrase.Category < m_cboCategory.Items.Count)
				m_cboCategory.SelectedIndex = basePhrase.Category;

			// Find and select basePhrase in datagrid
			DataGridViewRow rowToSelect = m_dataGridViewExistingQuestions.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => ((TranslatablePhrase)row.DataBoundItem) == basePhrase);
			if (rowToSelect != null)
			{
				SelectRowAndScrollIntoView(rowToSelect.Index);
				SelectedInsertionLocation = rowToSelect.Index + 1;
			}

			m_scrPsgReference.PassageChanged += r =>
			{
				// This check overcomes a HACK in the ScrPassageControl, which causes spurious PassageChanged events.
				var newRef = m_scrPsgReference.ScReference;
				if (m_existingStartRef != newRef)
				{
					var newRefInMasterVersification = StartReference;
					var sectionChange = newRefInMasterVersification < m_currentSections[0].StartRef ||
						newRefInMasterVersification > m_currentSections.Last().EndRef;
					m_existingStartRef = newRef;
					if (sectionChange)
						PopulateExistingQuestionsGrid();
					else
						SetDefaultInsertionLocation();
				}
			};
			m_cboEndVerse.SelectedIndexChanged += (sender, args) =>
			{
				var repopulateQuestionGrid = m_currentSections.Count > 1 && m_existingEndVerse < EndVerse;
				m_existingEndVerse = EndVerse;
				if (repopulateQuestionGrid)
				{
					SetCurrentSections();
					PopulateExistingQuestionsGrid();
				}
				else
				{
					SetDefaultInsertionLocation();
				}
			};
			// We don't want to hook up this handler until we're all done because it messes up initialization
			m_dataGridViewExistingQuestions.CellClick += HandleGridRowClicked;
		}

		private void SetCurrentSections()
		{
			m_currentSections = m_sectionInfo.GetSectionsThatContainRange(StartReference, EndReference).ToList();
			BCVRef startRef = m_currentSections.First().Value.StartRef;
			BCVRef endRef = m_currentSections.Last().Value.EndRef;
			m_existingPhrasesInCurrentSections = m_ptHelper.UnfilteredPhrases
				.Where(tp => tp.StartRef >= startRef && tp.EndRef <= endRef).ToList();
		}

		#region Event handlers and helper methods
		private void HandleStringsLocalized()
		{
			m_locationFormat = m_lblSelectLocation.Text;
			m_lblVernacularQuestion.Text = string.Format(m_lblVernacularQuestion.Text, m_vernLanguage);
			if (IsHandleCreated)
			{
				if (InvokeRequired)
					Invoke(new Action(UpdateDisplay));
				else
					UpdateDisplay();
			}
		}

		private void UpdateDisplay()
		{
			var fmt = (m_dataGridViewExistingQuestions.RowCount > 0) ? m_locationFormat : Properties.Resources.kstidNoExistingQuestions;
			m_lblSelectLocation.Text = String.Format(fmt, m_cboCategory.SelectedItem, ReferenceInProjectVersification);
		}

		private void PopulateEndRefComboBox()
		{
			bool noEndRef = (m_cboEndVerse.Items.Count > 1 && m_cboEndVerse.SelectedIndex == 0) ||
				(m_cboEndVerse.Items.Count == 0 && m_existingEndVerse == StartVerse);
			m_cboEndVerse.Items.Clear();
			m_cboEndVerse.Items.Add(String.Empty);

			var lastCoveredVerse = new BCVRef(m_currentSections.Last().Value.EndRef).Verse;
			for (int i = StartVerse; i <= lastCoveredVerse; i++)
				m_cboEndVerse.Items.Add(i.ToString());

			if (noEndRef)
				m_cboEndVerse.SelectedIndex = 0;
			else
				EndVerse = m_existingEndVerse;
		}

		private void PopulateCategoryComboBox()
		{
			m_cboCategory.Items.Clear();
			m_cboCategory.Items.AddRange(m_ptHelper.AllCategories.Cast<object>().ToArray());
		}

		private void HandleQuestionTextChanged(object sender, EventArgs e)
		{
			SetButtonState();
		}

		private void chkNoEnglish_CheckedChanged(object sender, EventArgs e)
		{
			SetButtonState();
			m_txtEnglishQuestion.Enabled = !m_chkNoEnglish.Checked;
			m_lblVernacularQuestionIsOptional.Visible = !m_chkNoEnglish.Checked;
		}

		private void SetButtonState()
		{
			if (m_txtEnglishQuestion.Text.Length > 0)
			{
				btnOk.Enabled = true;
				if (m_ptHelper.GetMatchingPhrases(StartReference, EndReference)
					.Any(mp => mp.PhraseToDisplayInUI == m_txtEnglishQuestion.Text))
				{
					btnOk.Enabled = false;
					m_chkNoEnglish.Visible = false;
					m_lblIdenticalQuestion.Visible = true;
					return;
				}
			}
			else
				btnOk.Enabled = m_chkNoEnglish.Checked && m_txtVernacularQuestion.Text.Length > 0;
			m_lblIdenticalQuestion.Visible = false;
			m_chkNoEnglish.Visible = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This effectively allows the up/down control to work "upside-down" - clicking the up
		/// arrow results in a lower SelectedInsertionLocation value, and vice versa.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleArrowClick(object sender, EventArgs e)
		{
			if (sender == m_btnUp)
				SelectedInsertionLocation--;
			else
				SelectedInsertionLocation++;

			if (m_dataGridViewExistingQuestions.RowCount > 0)
			{
				var rowToSelect = (SelectedInsertionLocation == m_dataGridViewExistingQuestions.RowCount) ?
					m_dataGridViewExistingQuestions.RowCount - 1 : SelectedInsertionLocation;
				SetRowAndArrowPosition(rowToSelect);
			}
		}

		private void SetRowAndArrowPosition(int rowToSelect)
		{
			SelectRowAndScrollIntoView(rowToSelect);
			if (Visible) // Wait to do this until window shows; otherwise calculations are off and it doesn't do anything useful.
				SetArrowPosition();
		}

		private void SetArrowPosition()
		{
			m_insertionPointArrow.Location = new Point(m_insertionPointArrow.Location.X,
				m_dataGridViewExistingQuestions.Padding.Top + m_dataGridViewExistingQuestions.ColumnHeadersHeight -
				m_dataGridViewExistingQuestions.VerticalScrollingOffset +
				SelectedInsertionLocation * m_dataGridViewExistingQuestions.Rows[0].Height -
				(m_insertionPointArrow.Height / 2));
			m_insertionPointArrow.Visible = m_insertionPointArrow.Top >= 0;
		}

		protected override void OnShown(EventArgs e)
		{
			SetArrowPosition();
		}

		private void SelectRowAndScrollIntoView(int rowToSelect)
		{
			m_dataGridViewExistingQuestions.CurrentCell = m_dataGridViewExistingQuestions.Rows[rowToSelect].Cells[0];
			if (!m_dataGridViewExistingQuestions.Rows[rowToSelect].Visible)
				m_dataGridViewExistingQuestions.FirstDisplayedScrollingRowIndex = Math.Max(0, rowToSelect - 1);
			m_previouslySelectedRow = rowToSelect;
		}

		private void m_scrPsgReference_PassageChanged(BCVRef newReference)
		{
			SetCurrentSections();
			PopulateEndRefComboBox();
		}

		private void HandleCategoryChanged(object sender, EventArgs e)
		{
			PopulateExistingQuestionsGrid();
		}

		private void PopulateExistingQuestionsGrid()
		{
			m_currentExistingPhrases = m_existingPhrasesInCurrentSections.Where(tp => tp.Category == Category).ToList();
			m_dataGridViewExistingQuestions.DataSource = new SortableBindingList<TranslatablePhrase>(m_currentExistingPhrases);

			// Seems like colExcluded.Visible = ... should be sufficient, but apparently when using data binding,
			// the original columns only serve as a template for the actual columns.
			m_dataGridViewExistingQuestions.Columns.OfType<DataGridViewColumn>().Single(c => c.Name == colExcluded.Name).Visible = (m_currentExistingPhrases.Any(p => p.IsExcluded));

			int newHeight = (m_dataGridViewExistingQuestions.RowCount > 0) ?
				m_dataGridViewExistingQuestions.ColumnHeadersHeight + m_dataGridViewExistingQuestions.Rows[0].Height * Math.Min(4, m_dataGridViewExistingQuestions.RowCount) :
				m_dataGridViewExistingQuestions.MinimumSize.Height;
			m_pnlArrow.Height = newHeight + m_dataGridViewExistingQuestions.Margin.Vertical;
			m_dataGridViewExistingQuestions.Height = newHeight;

			SetDefaultInsertionLocation();

			m_pnlUpDownArrows.Visible = m_insertionPointArrow.Visible =
				m_dataGridViewExistingQuestions.Enabled =
				(m_dataGridViewExistingQuestions.RowCount > 0);

			UpdateDisplay();
			SetButtonState();
		}

		private void SetDefaultInsertionLocation()
		{
			if (m_dataGridViewExistingQuestions.RowCount > 0)
			{
				SetRowWithInsertionAfter(m_currentExistingPhrases.FindLastIndex(
					p => p.StartRef < StartReference ||
						(p.StartRef == StartReference && p.EndRef <= EndReference)));
			}
			else
				SelectedInsertionLocation = 0;
		}

		private void SetRowWithInsertionAfter(int rowToSet)
		{
			SelectedInsertionLocation = rowToSet + 1;
			SetRowAndArrowPosition(rowToSet);
		}

		#endregion

		private void HandleGridRowClicked(object sender, EventArgs e)
		{
			var newRowIndex = (m_dataGridViewExistingQuestions.CurrentRow == null)
				? -1 : m_dataGridViewExistingQuestions.CurrentCellAddress.Y;
			if (m_previouslySelectedRow >= 0 && newRowIndex >= 0 &&
				m_previouslySelectedRow != newRowIndex)
			{
				SetRowWithInsertionAfter(newRowIndex);
			}
		}

		private void m_dataGridViewExistingQuestions_Scroll(object sender, ScrollEventArgs e)
		{
			SetArrowPosition();
		}

		private void m_txtVernacularQuestion_Enter(object sender, EventArgs e)
		{
			m_changeKeyboard(true);
		}

		private void m_txtVernacularQuestion_Leave(object sender, EventArgs e)
		{
			m_changeKeyboard(false);
		}
	}
}