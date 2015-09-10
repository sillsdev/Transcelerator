// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2012' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: NewQuestionDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
		private readonly IScrVers m_versification;
		private readonly string m_vernLanguage;
		private readonly PhraseTranslationHelper m_ptHelper;
		private int m_previouslySelectedRow = -1;
		private bool m_ignoreRowChange;
		private string m_locationFormat;
		private BCVRef m_existingStartRef;
		private int m_existingEndVerse;
		private int m_insertionLocation;

		#region Properties
		public string EnglishQuestion
		{
			get { return m_chkNoEnglish.Checked ? null : m_txtEnglishQuestion.Text; }
		}
		
		public string Answer
		{
			get { return m_txtAnswer.Text; }
		}

		public string Translation
		{
			get { return m_txtVernacularQuestion.Text;  }
		}

		public string Reference
		{
			get
			{
				var startRef = m_scrPsgReference.ScReference;
				var endRef = new BCVRef(startRef);
				endRef.Verse = EndVerse;
				return BCVRef.MakeReferenceString(startRef, endRef, ".", "-");
			}
		}

		public BCVRef StartReference
		{
			get { return m_scrPsgReference.ScReference; }
		}

		private int StartVerse
		{
			get { return StartReference.Verse; }
		}

		public int EndVerse
		{
			get
			{
				if (m_cboEndVerse.SelectedIndex <= 1)
					return StartVerse;
				return m_cboEndVerse.SelectedIndex - 1 + StartVerse;
			}
			private set
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

		public int Category
		{
			get { return m_cboCategory.SelectedIndex; }
		}

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

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This effectively allows the up/down control to work "upside-down" - clicking the down
		/// arrow results in a higher SelectedInsertionLocation value, and vice versa.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private int SelectedInsertionLocation
		{
			get { return m_insertionLocation; }
			set
			{
				m_insertionLocation = value;
				m_btnUp.Enabled = value > 0;
				m_btnDown.Enabled = value < m_dataGridViewExistingQuestions.RowCount;
				if (m_dataGridViewExistingQuestions.RowCount > 0)
					SetCurrentRowAndArrowPosition();
			}
		}

		public TranslatablePhrase BasePhrase
		{
			get
			{
				if (m_dataGridViewExistingQuestions.SelectedRows.Count == 0)
					return null;
				return (TranslatablePhrase)m_dataGridViewExistingQuestions.SelectedRows[0].DataBoundItem;
			}
		}

		public bool InsertBeforeBasePhrase
		{
			get { return SelectedInsertionLocation == 0; }
		}
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NewQuestionDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal NewQuestionDlg(TranslatablePhrase basePhrase, string vernLanguage, IScrVers versification, PhraseTranslationHelper pth,
			int[] canonicalBookIds)
		{
			var baseQuestion = basePhrase.QuestionInfo;
			m_vernLanguage = vernLanguage;
			m_versification = versification;
			m_ptHelper = pth;
			InitializeComponent();

			HandleStringsLocalized();

			m_scrPsgReference.Initialize(new BCVRef(baseQuestion.StartRef), versification, canonicalBookIds);
			m_existingStartRef = m_scrPsgReference.ScReference;
			m_existingEndVerse = BCVRef.GetVerseFromBcv(baseQuestion.EndRef);
			PopulateEndRefComboBox();

			PopulateCategoryComboBox();
			if (basePhrase.Category < m_cboCategory.Items.Count)
				m_cboCategory.SelectedIndex = basePhrase.Category;

			// Find and select basePhrase in datagrid
			DataGridViewRow rowToSelect = m_dataGridViewExistingQuestions.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => ((TranslatablePhrase)row.DataBoundItem) == basePhrase);
			if (rowToSelect != null)
				SelectRowAndScrollIntoView(rowToSelect.Index);

			m_scrPsgReference.PassageChanged += r =>
			{
				// This check overcomes a HACK in the ScrPassageControl, which causes spurious PassageChanged events.
				if (m_existingStartRef != m_scrPsgReference.ScReference)
				{
					m_existingStartRef = m_scrPsgReference.ScReference;
					PopulateMatchingQuestionsGrid();
				}
			};
			m_cboEndVerse.SelectedIndexChanged += (sender, args) =>
			{
				m_existingEndVerse = EndVerse;
				PopulateMatchingQuestionsGrid();
			};
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
			m_lblSelectLocation.Text = String.Format(fmt, m_cboCategory.SelectedItem, Reference);
		}

		private void PopulateEndRefComboBox()
		{
			bool noEndRef = (m_cboEndVerse.Items.Count > 1 && m_cboEndVerse.SelectedIndex == 0) ||
				(m_cboEndVerse.Items.Count == 0 && m_existingEndVerse == StartVerse);
			m_cboEndVerse.Items.Clear();
			m_cboEndVerse.Items.Add(String.Empty);
			for (int i = StartVerse; i <= m_versification.GetLastVerse(m_scrPsgReference.ScReference.Book, m_scrPsgReference.ScReference.Chapter); i++)
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
			btnOk.Enabled = (m_txtEnglishQuestion.Text.Length > 0 || (m_chkNoEnglish.Checked && m_txtVernacularQuestion.Text.Length > 0));
		}

		private void HandleUpArrowClick(object sender, EventArgs e)
		{
			SelectedInsertionLocation--;
		}

		private void HandleDownArrowClick(object sender, EventArgs e)
		{
			SelectedInsertionLocation++;
		}

		private void SetCurrentRowAndArrowPosition()
		{
			m_ignoreRowChange = true;
			var rowToSelect = (SelectedInsertionLocation == m_dataGridViewExistingQuestions.RowCount) ?
				m_dataGridViewExistingQuestions.RowCount - 1 : SelectedInsertionLocation;
			SelectRowAndScrollIntoView(rowToSelect);
			m_ignoreRowChange = false;

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

		private void SelectRowAndScrollIntoView(int rowToSelect)
		{
			m_dataGridViewExistingQuestions.CurrentCell = m_dataGridViewExistingQuestions.Rows[rowToSelect].Cells[0];
			if (!m_dataGridViewExistingQuestions.Rows[rowToSelect].Visible)
				m_dataGridViewExistingQuestions.FirstDisplayedScrollingRowIndex = Math.Max(0, rowToSelect - 1);
			m_previouslySelectedRow = rowToSelect;
		}

		private void m_scrPsgReference_PassageChanged(BCVRef newReference)
		{
			PopulateEndRefComboBox();
		}

		private void HandleCategoryChanged(object sender, EventArgs e)
		{
			PopulateMatchingQuestionsGrid();
		}

		private void PopulateMatchingQuestionsGrid()
		{
			m_ignoreRowChange = true;
			m_dataGridViewExistingQuestions.DataSource =
				new SortableBindingList<TranslatablePhrase>(m_ptHelper.GetMatchingPhrases(Reference, Category).ToList());

			int newHeight = (m_dataGridViewExistingQuestions.RowCount > 0) ?
				m_dataGridViewExistingQuestions.ColumnHeadersHeight + m_dataGridViewExistingQuestions.Rows[0].Height * Math.Min(4, m_dataGridViewExistingQuestions.RowCount) :
				m_dataGridViewExistingQuestions.MinimumSize.Height;
			m_pnlArrow.Height = newHeight + m_dataGridViewExistingQuestions.Margin.Vertical;
			m_dataGridViewExistingQuestions.Height = newHeight;
			m_ignoreRowChange = false;

			SelectedInsertionLocation = m_dataGridViewExistingQuestions.RowCount;

			m_pnlUpDownArrows.Visible = m_insertionPointArrow.Visible =
				m_dataGridViewExistingQuestions.Enabled =
				(m_dataGridViewExistingQuestions.RowCount > 0);

			UpdateDisplay();
		}
		#endregion

		private void m_dataGridViewExistingQuestions_SelectionChanged(object sender, EventArgs e)
		{
			if (m_ignoreRowChange)
				return;
			var newRowIndex = (m_dataGridViewExistingQuestions.SelectedRows.Count == 1)
				? m_dataGridViewExistingQuestions.SelectedRows[0].Index : -1;
			if (m_previouslySelectedRow >= 0 && newRowIndex >= 0 &&
				m_previouslySelectedRow != newRowIndex)
			{
				SelectedInsertionLocation = m_dataGridViewExistingQuestions.SelectedRows[0].Index + 1;
				m_previouslySelectedRow = newRowIndex;
			}
		}

		private void m_dataGridViewExistingQuestions_Scroll(object sender, ScrollEventArgs e)
		{
			SetArrowPosition();
		}
	}
}