// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.   
// <copyright from='2011' to='2013 company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: UNSQuestionsDialog.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using AddInSideViews;
using SIL.Utils;
using SILUBS.SharedScrControls;
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
	#region UNSQuestionsDialog class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// UNSQuestionsDialog.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class UNSQuestionsDialog : Form
	{
		#region Member Data
		private readonly string m_projectName;
	    private readonly Func<string> m_getKeyTermListName;
	    private readonly Func<IEnumerable<IKeyTerm>> m_getKeyTerms;
		private readonly Font m_vernFont;
		private readonly string m_vernIcuLocale;
		private readonly Action<bool> m_selectKeyboard;
		private readonly Action m_helpDelegate;
		private readonly Action<IEnumerable<IKeyTerm>> m_lookupTermDelegate;
		private PhraseTranslationHelper m_helper;
        private readonly DataFileProxy m_fileProxy;
		private readonly string m_defaultLcfFolder = null;
        private readonly IScrExtractor m_scrExtractor;
		private readonly string m_appName;
        private readonly IScrVers m_masterVersification;
        private readonly IScrVers m_projectVersification;
	    private BCVRef m_startRef;
        private BCVRef m_endRef;
		private IDictionary<string, string> m_sectionHeadText;
		private int[] m_availableBookIds;
		private readonly string m_questionsFilename;
		private string m_keyTermRulesFilename;
		private DateTime m_lastSaveTime;
		private readonly List<Substitution> m_phraseSubstitutions;
		private bool m_fIgnoreNextRecvdSantaFeSyncMessage;
		private bool m_fProcessingSyncMessage;
		private BCVRef m_queuedReference;
		private int m_lastRowEntered = -1;
		private TranslatablePhrase m_currentPhrase = null;
		private int m_iCurrentColumn = -1;
		private int m_normalRowHeight = -1;
		private int m_lastTranslationSet = -1;
		private bool m_saving = false;
		private bool m_postponeRefresh;
		private int m_maximumHeightOfKeyTermsPane;
		private bool m_loadingBiblicalTermsPane = false;
		#endregion

		#region Delegates
		public Func<IEnumerable<int>> GetAvailableBooks { private get; set; }
		#endregion

		#region Properties
		private DataGridViewTextBoxEditingControl TextControl { get; set;}
		private bool RefreshNeeded { get; set; }
		internal int MaximumHeightOfKeyTermsPane
		{
			get { return m_maximumHeightOfKeyTermsPane; }
			private set { m_maximumHeightOfKeyTermsPane = Math.Max(38, value); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to postpone refreshing the data grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool PostponeRefresh
		{
			get { return m_postponeRefresh; }
			set
			{
				if (value && !m_postponeRefresh)
					RefreshNeeded = false;
				m_postponeRefresh = value;
				if (!value && RefreshNeeded)
					dataGridUns.Refresh();
			}
		}

		internal PhraseTranslationHelper.KeyTermFilterType CheckedKeyTermFilterType
		{
			get
			{
				return (PhraseTranslationHelper.KeyTermFilterType)mnuKtFilter.DropDownItems.Cast<ToolStripMenuItem>().First(menu => menu.Checked).Tag;
			}
			private set
			{
				mnuKtFilter.DropDownItems.Cast<ToolStripMenuItem>().First(
                    menu => (PhraseTranslationHelper.KeyTermFilterType)menu.Tag == value).Checked = true;
				ApplyFilter();
			}
		}

		protected bool SaveNeeded
		{
			get { return btnSave.Enabled; }
			set
			{
				if (value && mnuAutoSave.Checked && DateTime.Now > m_lastSaveTime.AddSeconds(10))
					Save(true);
				else
					saveToolStripMenuItem.Enabled = btnSave.Enabled = value;
			}
		}

		protected IEnumerable<int> AvailableBookIds
		{
			get
			{
				if (GetAvailableBooks != null)
				{
					foreach (int i in GetAvailableBooks())
						yield return i;
				}
				else
				{
					for (int i = 1; i <= BCVRef.LastBook; i++)
						yield return i;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether the textual question filter requires whole-
		/// word matches.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool MatchWholeWords
		{
			get { return mnuMatchWholeWords.Checked; }
			set { mnuMatchWholeWords.Checked = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether toolbar is displayed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool ShowToolbar
		{
			get { return mnuViewToolbar.Checked; }
			set { mnuViewToolbar.Checked = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to show a pane with the answers and comments
		/// on the questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool ShowAnswersAndComments
		{
			get { return mnuViewAnswers.Checked; }
			set { mnuViewAnswers.Checked = value; }
		}

		protected TranslatablePhrase CurrentPhrase
		{
			get
			{
				Debug.Assert(dataGridUns.CurrentRow != null, "Caller is responsible for checking dataGridUns.CurrentRow before accessing this property.");
				return m_helper[dataGridUns.CurrentRow.Index];
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether a cell in the Translation column is the current cell.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool InTranslationCell
		{
			get
			{
				return dataGridUns.CurrentCell != null &&
				  dataGridUns.CurrentCell.ColumnIndex == m_colTranslation.Index;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether the grid is in edit mode in a cell in the
		/// Translation column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool EditingTranslation
		{
			get
			{
				return InTranslationCell && dataGridUns.IsCurrentCellInEditMode;
			}
		}
		#endregion

		#region Constructors
	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Initializes a new instance of the <see cref="UNSQuestionsDialog"/> class.
	    /// </summary>
	    /// <param name="splashScreen">The splash screen (can be null)</param>
	    /// <param name="projectName">Name of the project.</param>
	    /// <param name="keyTerms">The collection of key terms.</param>
	    /// <param name="vernFont">The vernacular font.</param>
	    /// <param name="vernIcuLocale">The vernacular icu locale.</param>
	    /// <param name="fVernIsRtoL">if set to <c>true</c> the vernacular language is r-to-L].</param>
        /// <param name="datafileProxy">helper object to store and retrieve data.</param>
	    /// <param name="sDefaultLcfFolder">The LCF folder.</param>
	    /// <param name="appName">Name of the calling application</param>
	    /// <param name="versification">The versification.</param>
	    /// <param name="startRef">The starting Scripture reference</param>
	    /// <param name="endRef">The ending Scripture reference</param>
	    /// <param name="selectKeyboard">The delegate to select vern/anal keyboard.</param>
	    /// <param name="lookupTermDelegate">The lookup term delegate.</param>
	    /// ------------------------------------------------------------------------------------
        public UNSQuestionsDialog(TxlSplashScreen splashScreen, string projectName,
            IEnumerable<IKeyTerm> keyTerms, Font vernFont, string vernIcuLocale, bool fVernIsRtoL,
            DataFileProxy datafileProxy, string sDefaultLcfFolder, string appName, IScrVers versification,
            BCVRef startRef, BCVRef endRef, Action<bool> selectKeyboard,
            Action<IEnumerable<IKeyTerm>> lookupTermDelegate) :this(splashScreen, projectName, null,
            () => keyTerms, vernFont, vernIcuLocale, fVernIsRtoL, datafileProxy, default(IScrExtractor),
            appName, versification, versification, startRef, endRef, selectKeyboard, lookupTermDelegate)
        {
            m_defaultLcfFolder = sDefaultLcfFolder;
            mnuGenerate.Text = Properties.Resources.kstidGenerateTemplateMenuText;
        }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Initializes a new instance of the <see cref="UNSQuestionsDialog"/> class.
	    /// </summary>
	    /// <param name="splashScreen">The splash screen (can be null)</param>
	    /// <param name="projectName">Name of the project.</param>
        /// <param name="getKeyTermListName">Function to get the Name of the key terms list
	    /// currently associated with the project.</param>
	    /// <param name="getKeyTerms">Function to get collection of key terms.</param>
	    /// <param name="vernFont">The vernacular font.</param>
	    /// <param name="vernIcuLocale">The vernacular icu locale.</param>
	    /// <param name="fVernIsRtoL">if set to <c>true</c> the vernacular language is r-to-L].</param>
	    /// <param name="datafileProxy">helper object to store and retrieve data.</param>
	    /// <param name="scrExtractor">The Scripture extractor (can be null).</param>
	    /// <param name="appName">Name of the calling application</param>
	    /// <param name="projectVersification">The versification of the external project (to
	    /// be used for passing references to the scrExtractor).</param>
	    /// <param name="startRef">The starting Scripture reference</param>
	    /// <param name="endRef">The ending Scripture reference</param>
	    /// <param name="selectKeyboard">The delegate to select vern/anal keyboard.</param>
	    /// <param name="lookupTermDelegate">The lookup term delegate.</param>
	    /// ------------------------------------------------------------------------------------
	    public UNSQuestionsDialog(TxlSplashScreen splashScreen, string projectName,
            Func<string> getKeyTermListName, Func<IEnumerable<IKeyTerm>> getKeyTerms, Font vernFont,
            string vernIcuLocale, bool fVernIsRtoL, DataFileProxy datafileProxy,
            IScrExtractor scrExtractor, string appName, IScrVers englishVersification,
            IScrVers projectVersification, BCVRef startRef, BCVRef endRef,
            Action<bool> selectKeyboard, Action<IEnumerable<IKeyTerm>> lookupTermDelegate)
		{
            if (splashScreen == null)
            {
                splashScreen = new TxlSplashScreen();
                splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
            }
            splashScreen.Message = Properties.Resources.kstidSplashMsgInitializing;

            InitializeComponent();

            m_fileProxy = datafileProxy;

            if (startRef != BCVRef.Empty && endRef != BCVRef.Empty && startRef > endRef)
				throw new ArgumentException("startRef must be before endRef");
			m_projectName = projectName;
	        m_getKeyTermListName = getKeyTermListName;
	        m_getKeyTerms = getKeyTerms;
			m_vernFont = vernFont;
			m_vernIcuLocale = vernIcuLocale;
            if (string.IsNullOrEmpty(m_vernIcuLocale))
                mnuGenerate.Enabled = false;
			m_selectKeyboard = selectKeyboard;
			m_lookupTermDelegate = lookupTermDelegate;
	        m_scrExtractor = scrExtractor;
	        m_appName = appName;
            m_masterVersification = englishVersification;
            m_projectVersification = projectVersification;
		    TermRenderingCtrl.s_AppName = appName;
			m_startRef = startRef;
			m_endRef = endRef;

            m_questionsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                TxlCore.questionsFilename);

			ClearBiblicalTermsPane();

			Text = String.Format(Text, projectName);
			HelpButton = (m_helpDelegate != null);

			mnuShowAllPhrases.Tag = PhraseTranslationHelper.KeyTermFilterType.All;
			mnuShowPhrasesWithKtRenderings.Tag = PhraseTranslationHelper.KeyTermFilterType.WithRenderings;
			mnuShowPhrasesWithMissingKtRenderings.Tag = PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings;
			m_lblAnswerLabel.Tag = m_lblAnswerLabel.Text.Trim();
			m_lblCommentLabel.Tag = m_lblCommentLabel.Text.Trim();
			lblFilterIndicator.Tag = lblFilterIndicator.Text;
			lblRemainingWork.Tag = lblRemainingWork.Text;

            Location = Properties.Settings.Default.WindowLocation;
			WindowState = Properties.Settings.Default.DefaultWindowState;
			if (MinimumSize.Height <= Properties.Settings.Default.WindowSize.Height &&
				MinimumSize.Width <= Properties.Settings.Default.WindowSize.Width)
			{
				Size = Properties.Settings.Default.WindowSize;
			}
			MatchWholeWords = !Properties.Settings.Default.MatchPartialWords;
			ShowToolbar = Properties.Settings.Default.ShowToolbar;
			btnReceiveScrReferences.Checked = Properties.Settings.Default.ReceiveScrRefs;
			ShowAnswersAndComments = Properties.Settings.Default.ShowAnswersAndComments;
			MaximumHeightOfKeyTermsPane = Properties.Settings.Default.MaximumHeightOfKeyTermsPane;

			DataGridViewCellStyle translationCellStyle = new DataGridViewCellStyle();
			translationCellStyle.Font = vernFont;
			m_colTranslation.DefaultCellStyle = translationCellStyle;
			if (fVernIsRtoL)
				m_colTranslation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			dataGridUns.RowTemplate.MinimumHeight = dataGridUns.RowTemplate.Height = m_normalRowHeight =
				(int)Math.Ceiling(vernFont.Height * CreateGraphics().DpiY / 72) + 2;
			Margin = new Padding(Margin.Left, toolStrip1.Height, Margin.Right, Margin.Bottom);

            m_phraseSubstitutions = XmlSerializationHelper.LoadOrCreateListFromString<Substitution>(
                m_fileProxy.Read(DataFileProxy.DataFileId.PhraseSubstitutions), true);

	        KeyTermMatch.FileProxy = m_fileProxy;

			LoadTranslations(splashScreen);

			// Now apply settings that have filtering or other side-effects
			CheckedKeyTermFilterType = (PhraseTranslationHelper.KeyTermFilterType)Properties.Settings.Default.KeyTermFilterType;
			btnSendScrReferences.Checked = Properties.Settings.Default.SendScrRefs;

			splashScreen.Close();
		}
		#endregion

		#region Events
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (SaveNeeded)
			{
				if (mnuAutoSave.Checked)
				{
					Save(true);
					return;
				}
				switch (MessageBox.Show(this, "You have made changes. Do you wish to save before closing?",
					"Save changes?", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						Save(true);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
            if (!e.Cancel)
                Properties.Settings.Default.Save();

			base.OnClosing(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="msg">The Windows Message to process.</param>
		/// ------------------------------------------------------------------------------------
		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == SantaFeFocusMessageHandler.FocusMsg)
			{
				// Always assume the English versification scheme for passing references.
				var scrRef = new BCVRef(SantaFeFocusMessageHandler.ReceiveFocusMessage(msg));

				if (!btnReceiveScrReferences.Checked || m_fIgnoreNextRecvdSantaFeSyncMessage ||
					m_fProcessingSyncMessage)
				{
					if (m_fProcessingSyncMessage)
						m_queuedReference = scrRef;

					m_fIgnoreNextRecvdSantaFeSyncMessage = false;
					return;
				}

				ProcessReceivedMessage(scrRef);
			}

			base.WndProc(ref msg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Refreshes the data grid when the translations change.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_helper_TranslationsChanged()
		{
			if (PostponeRefresh)
				RefreshNeeded = true;
			else
				dataGridUns.Refresh();
		}

		private void UNSQuestionsDialog_Activated(object sender, EventArgs e)
		{
			if (m_selectKeyboard != null)
				m_selectKeyboard(InTranslationCell);
		}

		private void dataGridUns_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colTranslation.Index && m_selectKeyboard != null)
				m_selectKeyboard(true);
			if (e.ColumnIndex != m_colUserTranslated.Index || e.RowIndex != m_lastTranslationSet)
				m_lastTranslationSet = -1;
		}

		private void dataGridUns_CellLeave(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colTranslation.Index && m_selectKeyboard != null)
				m_selectKeyboard(false);
		}

		private void mnuViewDebugInfo_CheckedChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			if (!item.Checked)
				dataGridUns.Columns.Remove(m_colDebugInfo);
			else
				dataGridUns.Columns.Add(m_colDebugInfo);
		}

		private void mnuViewAnswersColumn_CheckedChanged(object sender, EventArgs e)
		{
			m_pnlAnswersAndComments.Visible = ShowAnswersAndComments;
		}

		private void dataGridUns_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			switch (e.ColumnIndex)
			{
				case 0: e.Value = m_helper[e.RowIndex].Reference; break;
				case 1: e.Value = m_helper[e.RowIndex].PhraseToDisplayInUI; break;
				case 2: e.Value = m_helper[e.RowIndex].Translation; break;
				case 3: e.Value = m_helper[e.RowIndex].HasUserTranslation; break;
				case 4: e.Value = m_helper[e.RowIndex].Parts; break;
			}
		}

		private void dataGridUns_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (m_saving)
				return;

			PostponeRefresh = true;

			if (e.ColumnIndex == m_colTranslation.Index)
			{
				m_helper[e.RowIndex].Translation = (string)e.Value;
				m_lastTranslationSet = e.RowIndex;
				SaveNeeded = true;
			}
			else if (e.ColumnIndex == m_colUserTranslated.Index)
			{
				m_helper[e.RowIndex].HasUserTranslation = (bool)e.Value;
				SaveNeeded = true;
				dataGridUns.InvalidateRow(e.RowIndex);
			}

			PostponeRefresh = false;
			UpdateCountsAndFilterStatus();
		}

		private void dataGridUns_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colTranslation.Index)
				dataGridUns.BeginEdit(true);
		}

		private void dataGridUns_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colUserTranslated.Index && e.RowIndex != m_lastTranslationSet)
			{
				if (m_helper[e.RowIndex].Translation.Any(Char.IsLetter))
				{
					m_helper[e.RowIndex].HasUserTranslation = !m_helper[e.RowIndex].HasUserTranslation;
					SaveNeeded = true;
					dataGridUns.InvalidateRow(e.RowIndex);
				}
			}
		}

		private void dataGridUns_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			int iClickedCol = e.ColumnIndex;
			// We want to sort it ascending unless it already was ascending.
			bool sortAscending = (dataGridUns.Columns[iClickedCol].HeaderCell.SortGlyphDirection != SortOrder.Ascending);
			if (!sortAscending)
			{
				dataGridUns.Columns[iClickedCol].HeaderCell.SortGlyphDirection = SortOrder.Descending;
			}
			else
			{
				for (int i = 0; i < dataGridUns.Columns.Count; i++)
				{
					dataGridUns.Columns[i].HeaderCell.SortGlyphDirection = (i == iClickedCol) ?
						SortOrder.Ascending : SortOrder.None;
				}
			}
			SortByColumn(iClickedCol, sortAscending);
			dataGridUns.Refresh();
		}

		private void SortByColumn(int iClickedCol, bool sortAscending)
		{
			switch (iClickedCol)
			{
				case 0: m_helper.Sort(PhraseTranslationHelper.SortBy.Reference, sortAscending); break;
				case 1: m_helper.Sort(PhraseTranslationHelper.SortBy.EnglishPhrase, sortAscending); break;
				case 2: m_helper.Sort(PhraseTranslationHelper.SortBy.Translation, sortAscending); break;
				case 3: m_helper.Sort(PhraseTranslationHelper.SortBy.Status, sortAscending); break;
				case 4: m_helper.Sort(PhraseTranslationHelper.SortBy.Default, sortAscending); break;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Enter event of the txtFilterByPart control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void txtFilterByPart_Enter(object sender, EventArgs e)
		{
			RememberCurrentSelection();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Remembers the current selection so the same phrase and column can be re-selected
		/// after filtering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void RememberCurrentSelection()
		{
			if (dataGridUns.CurrentRow != null && dataGridUns.CurrentCell != null)
			{
				m_currentPhrase = CurrentPhrase;
				m_iCurrentColumn = dataGridUns.CurrentCell.ColumnIndex;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Filtering is done, so
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void txtFilterByPart_Leave(object sender, EventArgs e)
		{
			m_currentPhrase = null;
			m_iCurrentColumn = -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckChanged event of the mnuMatchWholeWords control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuMatchWholeWords_CheckChanged(object sender, EventArgs e)
		{
			if (txtFilterByPart.Text.Trim().Length > 0)
				ApplyFilter();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void ApplyFilter(object sender, EventArgs e)
		{
			ApplyFilter();
		}
		private void ApplyFilter()
		{
			bool clearCurrentSelection = false;
			if (m_currentPhrase == null)
			{
				RememberCurrentSelection();
				clearCurrentSelection = true;
			}
            Func<int, int, string, bool> refFilter = (m_startRef == BCVRef.Empty) ? null :
				new Func<int, int, string, bool>((start, end, sref) => m_endRef >= start && m_startRef <= end);
			dataGridUns.RowCount = 0;
			m_biblicalTermsPane.Hide();
            dataGridUns.RowEnter -= dataGridUns_RowEnter;

			m_helper.Filter(txtFilterByPart.Text, MatchWholeWords, CheckedKeyTermFilterType, refFilter, mnuViewExcludedQuestions.Checked);
			dataGridUns.RowCount = m_helper.Phrases.Count();

            dataGridUns.RowEnter += dataGridUns_RowEnter;

            int currentRow = dataGridUns.CurrentCell == null ? -1 : dataGridUns.CurrentCell.RowIndex;
			if (m_currentPhrase != null)
			{
				for (int i = 0; i < dataGridUns.Rows.Count; i++)
				{
					if (m_helper[i] == m_currentPhrase)
					{
						dataGridUns.CurrentCell = dataGridUns.Rows[i].Cells[m_iCurrentColumn];
						break;
					}
				}
				if (clearCurrentSelection)
				{
					m_currentPhrase = null;
					m_iCurrentColumn = -1;
				}
			}

            if (dataGridUns.CurrentCell != null && currentRow == dataGridUns.CurrentCell.RowIndex)
                dataGridUns_RowEnter(dataGridUns, new DataGridViewCellEventArgs(m_iCurrentColumn, currentRow));

			UpdateCountsAndFilterStatus();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellDoubleClick event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 4)
			{
				StringBuilder sbldr = new StringBuilder();
				sbldr.AppendLine("Key Terms:");
				foreach (KeyTermMatch keyTermMatch in m_helper[e.RowIndex].GetParts().OfType<KeyTermMatch>())
				{
					foreach (string sEnglishTerm in keyTermMatch.AllTerms.Select(term => term.Term))
					{
						sbldr.AppendLine(sEnglishTerm);
					}
				}
				MessageBox.Show(sbldr.ToString(), "More Key Term Debug Info");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when one of the Key Term filtering sub-menus is clicked.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void OnKeyTermsFilterChange(object sender, EventArgs e)
		{
			if (sender == mnuShowAllPhrases && mnuShowAllPhrases.Checked)
				return;

			if (!((ToolStripMenuItem)sender).Checked)
				mnuShowAllPhrases.Checked = true;
			ApplyFilter();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called when one of the Key Term filtering sub-menus is checked.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void OnKeyTermsFilterChecked(object sender, EventArgs e)
		{
			ToolStripMenuItem clickedMenu = (ToolStripMenuItem)sender;
			if (clickedMenu.Checked)
			{
				foreach (ToolStripMenuItem menu in mnuKtFilter.DropDownItems)
				{
					if (menu != clickedMenu)
						menu.Checked = false;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the HelpButtonClicked event of the UNSQuestionsDialog control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UNSQuestionsDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
		{
			m_helpDelegate();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the UNS Translation data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Save(object sender, EventArgs e)
		{
			Save(false);
		}

		private void Save(bool fForce)
		{
			if (m_saving || (!fForce && !SaveNeeded))
				return;
			m_saving = true;
			SaveNeeded = false;
			m_lastSaveTime = DateTime.Now;
			if (dataGridUns.IsCurrentCellInEditMode)
				dataGridUns.EndEdit();
			m_fileProxy.Write(DataFileProxy.DataFileId.Translations, XmlSerializationHelper.SerializeToString(
				(from translatablePhrase in m_helper.UnfilteredPhrases
				where translatablePhrase.HasUserTranslation
				select new XmlTranslation(translatablePhrase)).ToList()));

			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			foreach (TranslatablePhrase translatablePhrase in m_helper.UnfilteredPhrases)
			{
				if (translatablePhrase.IsExcludedOrModified)
					customizations.Add(new PhraseCustomization(translatablePhrase));
				if (translatablePhrase.InsertedPhraseBefore != null)
				{
					customizations.Add(new PhraseCustomization(translatablePhrase.QuestionInfo.Text,
						translatablePhrase.InsertedPhraseBefore,
						PhraseCustomization.CustomizationType.InsertionBefore));
				}
				if (translatablePhrase.AddedPhraseAfter != null)
				{
					customizations.Add(new PhraseCustomization(translatablePhrase.QuestionInfo.Text,
						translatablePhrase.AddedPhraseAfter,
						PhraseCustomization.CustomizationType.AdditionAfter));
				}
			}

            if (customizations.Count > 0 || m_fileProxy.Exists(DataFileProxy.DataFileId.QuestionCustomizations))
            {
                m_fileProxy.Write(DataFileProxy.DataFileId.QuestionCustomizations,
                    XmlSerializationHelper.SerializeToString(customizations));
            }

			m_saving = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the closeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuGenerate control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuGenerate_Click(object sender, EventArgs e)
		{
            GenerateScript(m_scrExtractor == null ? m_defaultLcfFolder :
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the Click event of the mnuGenerate control.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void GenerateScript(string defaultFolder)
        {
            using (GenerateScriptDlg dlg = new GenerateScriptDlg(m_projectName, m_scrExtractor,
                defaultFolder, AvailableBookIds, m_sectionHeadText.AsEnumerable()))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Func<int, int, bool> InRange;
                    if (dlg.m_rdoWholeBook.Checked)
                    {
                        int bookNum = BCVRef.BookToNumber((string)dlg.m_cboBooks.SelectedItem);
                        InRange = (bcvStart, bcvEnd) =>
                        {
                            return BCVRef.GetBookFromBcv(bcvStart) == bookNum;
                        };
                    }
                    else
                    {
                        BCVRef startRef = dlg.VerseRangeStartRef;
                        BCVRef endRef = dlg.VerseRangeEndRef;
                        InRange = (bcvStart, bcvEnd) =>
                        {
                            return bcvStart >= startRef && bcvEnd <= endRef;
                        };
                    }

                    List<TranslatablePhrase> allPhrasesInRange = m_helper.UnfilteredPhrases.Where(tp => tp.Category > -1 && InRange(tp.StartRef, tp.EndRef) && !tp.IsExcluded).ToList();
                    if (dlg.m_rdoDisplayWarning.Checked)
                    {
                        int untranslatedQuestions = allPhrasesInRange.Count(p => !p.HasUserTranslation);
                        if (untranslatedQuestions > 0 &&
                            MessageBox.Show(string.Format(Properties.Resources.kstidUntranslatedQuestionsWarning, untranslatedQuestions),
                            m_appName, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                        sw.WriteLine("<html>");
                        sw.WriteLine("<head>");
                        sw.WriteLine("<meta content=\"text/html; charset=UTF-8\" http-equiv=\"content-type\"/>");
                        sw.WriteLine("<title>" + dlg.m_txtTitle.Text.Normalize(NormalizationForm.FormC) + "</title>");
                        if (!dlg.m_rdoEmbedStyleInfo.Checked)
                        {
                            sw.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href= \"" + dlg.CssFile + "\"/>");
                            if (dlg.WriteCssFile)
                            {
                                if (dlg.m_chkOverwriteCss.Checked)
                                {
                                    using (StreamWriter css = new StreamWriter(dlg.FullCssPath))
                                    {
                                        WriteCssStyleInfo(css, dlg.m_lblQuestionGroupHeadingsColor.ForeColor,
                                            dlg.m_lblEnglishQuestionColor.ForeColor, dlg.m_lblEnglishAnswerTextColor.ForeColor,
                                            dlg.m_lblCommentTextColor.ForeColor, (int)dlg.m_numBlankLines.Value,
                                            dlg.m_chkNumberQuestions.Checked);
                                    }
                                }
                            }
                        }

                        sw.WriteLine("<style type=\"text/css\">");
                        // This CSS directive always gets written directly to the template file because it's
                        // important to get right and it's unlikely that someone will want to do a global override.
                        sw.WriteLine(":lang(" + m_vernIcuLocale + ") {font-family:serif," +
                            m_colTranslation.DefaultCellStyle.Font.FontFamily.Name + ",Arial Unicode MS;}");
                        if (dlg.m_rdoEmbedStyleInfo.Checked)
                        {
                            WriteCssStyleInfo(sw, dlg.m_lblQuestionGroupHeadingsColor.ForeColor,
                                dlg.m_lblEnglishQuestionColor.ForeColor, dlg.m_lblEnglishAnswerTextColor.ForeColor,
                                dlg.m_lblCommentTextColor.ForeColor, (int)dlg.m_numBlankLines.Value,
                                dlg.m_chkNumberQuestions.Checked);
                        }
                        sw.WriteLine("</style>");
                        sw.WriteLine("</head>");
                        sw.WriteLine("<body lang=\"" + m_vernIcuLocale + "\">");
                        sw.WriteLine("<h1 lang=\"en\">" + dlg.m_txtTitle.Text.Normalize(NormalizationForm.FormC) + "</h1>");
                        int prevCategory = -1;
                        int prevSectionStartRef = -1, prevSectionEndRef = -1;
                        string prevQuestionRef = null;
                        string pendingSectionHead = null;

                        foreach (TranslatablePhrase phrase in allPhrasesInRange)
                        {
                            if (phrase.Category == 0 && (phrase.StartRef < prevSectionStartRef || phrase.EndRef > prevSectionEndRef))
                            {
                                if (!m_sectionHeadText.TryGetValue(phrase.Reference, out pendingSectionHead))
                                    pendingSectionHead = phrase.Reference;
                                prevCategory = -1;
                            }
                            prevSectionStartRef = phrase.StartRef;
                            prevSectionEndRef = phrase.EndRef;

                            if (!phrase.HasUserTranslation && (phrase.TypeOfPhrase == TypeOfPhrase.NoEnglishVersion || !dlg.m_rdoUseOriginal.Checked))
                                continue; // skip this question

                            if (pendingSectionHead != null)
                            {
                                sw.WriteLine("<h2 lang=\"en\">" + pendingSectionHead.Normalize(NormalizationForm.FormC) + "</h2>");
                                pendingSectionHead = null;
                            }

                            if (phrase.Category != prevCategory)
                            {
                                sw.WriteLine("<h3>" + phrase.CategoryName.Normalize(NormalizationForm.FormC) + "</h3>");
                                prevCategory = phrase.Category;
                            }

                            if (prevQuestionRef != phrase.Reference)
                            {
                                if (phrase.Category > 0 || dlg.m_chkPassageBeforeOverview.Checked)
                                {
                                    sw.WriteLine("<p class=\"scripture\">");
                                    int startRef = m_projectVersification.ChangeVersification(phrase.StartRef, m_masterVersification);
                                    int endRef = m_projectVersification.ChangeVersification(phrase.EndRef, m_masterVersification);
                                    if (m_scrExtractor == null)
                                        sw.WriteLine(@"\ref " + BCVRef.MakeReferenceString(startRef, endRef, ".", "-"));
                                    else
                                    {
                                        try
                                        {
                                            sw.Write(m_scrExtractor.Extract(startRef, endRef));
                                        }
                                        catch (Exception ex)
                                        {
                                            sw.Write(ex.Message);
#if DEBUG
                                            throw;
#endif
                                        }
                                    }
                                    sw.WriteLine("</p>");
                                }
                                prevQuestionRef = phrase.Reference;
                            }

                            sw.WriteLine("<p class=\"question\">" +
                                (phrase.HasUserTranslation ? phrase.Translation : phrase.PhraseToDisplayInUI).Normalize(NormalizationForm.FormC) + "</p>");

                            sw.WriteLine("<div class=\"extras\" lang=\"en\">");
                            if (dlg.m_chkEnglishQuestions.Checked && phrase.HasUserTranslation && phrase.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion)
                                sw.WriteLine("<p class=\"questionbt\">" + phrase.PhraseToDisplayInUI.Normalize(NormalizationForm.FormC) + "</p>");
                            Question answersAndComments = phrase.QuestionInfo;
                            if (dlg.m_chkEnglishAnswers.Checked && answersAndComments.Answers != null)
                            {
                                foreach (string answer in answersAndComments.Answers)
                                    sw.WriteLine("<p class=\"answer\">" + answer.Normalize(NormalizationForm.FormC) + "</p>");
                            }
                            if (dlg.m_chkIncludeComments.Checked && answersAndComments.Notes != null)
                            {
                                foreach (string comment in answersAndComments.Notes)
                                    sw.WriteLine("<p class=\"comment\">" + comment.Normalize(NormalizationForm.FormC) + "</p>");
                            }
                            sw.WriteLine("</div>");
                        }

                        sw.WriteLine("</body>");
                    }
                    MessageBox.Show(Properties.Resources.kstidTemplateGenerationComplete);
                }
            }
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes the CSS style info.
		/// </summary>
		/// <param name="sw">The sw.</param>
		/// <param name="questionGroupHeadingsClr">The question group headings CLR.</param>
		/// <param name="englishQuestionClr">The english question CLR.</param>
		/// <param name="englishAnswerClr">The english answer CLR.</param>
		/// <param name="commentClr">The comment CLR.</param>
		/// <param name="cBlankLines">The c blank lines.</param>
		/// <param name="fNumberQuestions">if set to <c>true</c> [f number questions].</param>
		/// ------------------------------------------------------------------------------------
		private void WriteCssStyleInfo(StreamWriter sw, Color questionGroupHeadingsClr,
			Color englishQuestionClr, Color englishAnswerClr, Color commentClr, int cBlankLines, bool fNumberQuestions)
		{
			if (fNumberQuestions)
			{
				sw.WriteLine("body {font-size:100%; counter-reset:qnum;}");
				sw.WriteLine(".question {counter-increment:qnum;}");
				sw.WriteLine("p.question:before {content:counter(qnum) \". \";}");
			}
			else
				sw.WriteLine("body {font-size:100%;}");
			sw.WriteLine("h1 {font-size:2.0em;");
			sw.WriteLine("  text-align:center}");
			sw.WriteLine("h2 {font-size:1.7em;");
			sw.WriteLine("  color:white;");
			sw.WriteLine("  background-color:black;}");
			sw.WriteLine("h3 {font-size:1.3em;");
			sw.WriteLine("  color:blue;}");
			sw.WriteLine("p {font-size:1.0em;}");
			sw.WriteLine("h1:lang(en) {font-family:sans-serif;}");
			sw.WriteLine("h2:lang(en) {font-family:serif;}");
			sw.WriteLine("p:lang(en) {font-family:serif;");
  			sw.WriteLine("font-size:0.85em;}");
			sw.WriteLine("h3 {color:" + questionGroupHeadingsClr.Name + ";}");
			sw.WriteLine(".questionbt {color:" + englishQuestionClr.Name + ";}");
			sw.WriteLine(".answer {color:" + englishAnswerClr.Name + ";}");
			sw.WriteLine(".comment {color:" + commentClr.Name + ";}");
			sw.WriteLine(".extras {margin-bottom:" + cBlankLines + "em;}");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckedChanged event of the mnuViewToolbar control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuViewToolbar_CheckedChanged(object sender, EventArgs e)
		{
			toolStrip1.Visible = mnuViewToolbar.Checked;
			if (toolStrip1.Visible)
				m_mainMenu.SendToBack(); // this makes the toolbar appear below the menu
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckedChanged event of the mnuAutoSave control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuAutoSave_CheckedChanged(object sender, EventArgs e)
		{
			if (mnuAutoSave.Checked)
				Save(false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the phraseSubstitutionsToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void phraseSubstitutionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (PhraseSubstitutionsDlg dlg = new PhraseSubstitutionsDlg(m_phraseSubstitutions,
				m_helper.Phrases.Where(tp => tp.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion).Select(p => p.PhraseInUse),
				dataGridUns.CurrentRow.Index))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					m_phraseSubstitutions.Clear();
					m_phraseSubstitutions.AddRange(dlg.Substitutions);
                    m_fileProxy.Write(DataFileProxy.DataFileId.PhraseSubstitutions,
                        XmlSerializationHelper.SerializeToString(m_phraseSubstitutions));

					Reload(false);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the reloadToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Reload(false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Reloads the data grid view and attempts to re-select the same cell of the same
		/// question as was previously selected.
		/// </summary>
		/// <param name="fForceSave">if set to <c>true</c> [f force save].</param>
		/// <param name="key">The key of the question to try to select after reloading.</param>
		/// ------------------------------------------------------------------------------------
		private void Reload(bool fForceSave)
		{
			TranslatablePhrase phrase = dataGridUns.CurrentRow != null ? CurrentPhrase : null;
			Reload(fForceSave, (phrase == null) ? null : phrase.PhraseKey, 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Reloads the specified f force save.
		/// </summary>
		/// <param name="fForceSave">if set to <c>true</c> [f force save].</param>
		/// <param name="key">The key of the question to try to select after reloading.</param>
		/// <param name="fallBackRow">the index of the row to select if a question witht the
		/// given key cannot be found.</param>
		/// ------------------------------------------------------------------------------------
		private void Reload(bool fForceSave, QuestionKey key, int fallBackRow)
		{
			using (new WaitCursor(this))
			{
				int iCol = dataGridUns.CurrentCell.ColumnIndex;
				Save(fForceSave);

				int iSortedCol = -1;
				bool sortAscending = true;
				for (int i = 0; i < dataGridUns.Columns.Count; i++)
				{
					switch (dataGridUns.Columns[i].HeaderCell.SortGlyphDirection)
					{
						case SortOrder.Ascending: iSortedCol = i; break;
						case SortOrder.Descending: iSortedCol = i; sortAscending = false; break;
						default:
							continue;
					}
					break;
				}

				m_helper.TranslationsChanged -= m_helper_TranslationsChanged;
				dataGridUns.RowCount = 0;
				LoadTranslations(null);
				ApplyFilter();
				if (iSortedCol >= 0)
					SortByColumn(iSortedCol, sortAscending);
				if (key != null)
				{
					int iRow = m_helper.FindPhrase(key);
					if (iRow < 0)
						iRow = fallBackRow;
					if (iRow < dataGridUns.Rows.Count)
						dataGridUns.CurrentCell = dataGridUns.Rows[iRow].Cells[iCol];
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CheckedChanged event of the mnuViewBiblicalTermsPane control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuViewBiblicalTermsPane_CheckedChanged(object sender, EventArgs e)
		{
			if (mnuViewBiblicalTermsPane.Checked && dataGridUns.CurrentRow != null)
				LoadKeyTermsPane(dataGridUns.CurrentRow.Index);
			else
			{
				m_biblicalTermsPane.Hide();
				ClearBiblicalTermsPane();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the RowEnter event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			m_lastRowEntered = e.RowIndex;
			if (mnuViewBiblicalTermsPane.Checked)
				LoadKeyTermsPane(e.RowIndex);
			if (m_pnlAnswersAndComments.Visible)
				LoadAnswerAndComment(e.RowIndex);
			if (btnSendScrReferences.Checked)
				SendScrReference(e.RowIndex);

			DataGridViewRow row = dataGridUns.Rows[e.RowIndex];
			row.ReadOnly = m_helper[e.RowIndex].IsExcluded;

			m_normalRowHeight = row.Height;
			dataGridUns.AutoResizeRow(e.RowIndex);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the RowLeave event of the dataGridUns control. Resets the row height to the
		/// standard height.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_RowLeave(object sender, DataGridViewCellEventArgs e)
		{
			dataGridUns.Rows[e.RowIndex].Height = m_normalRowHeight;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Resize event of the main window.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UNSQuestionsDialog_Resize(object sender, EventArgs e)
		{
			dataGridUns.MinimumSize = new Size(dataGridUns.MinimumSize.Width, Height / 2);
			m_biblicalTermsPane.SuspendLayout();
			m_pnlAnswersAndComments.MaximumSize = new Size(dataGridUns.Width, dataGridUns.Height);
			ResizeKeyTermPaneColumns();
			m_biblicalTermsPane.ResumeLayout();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Resize event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		private void dataGridUns_Resize(object sender, EventArgs e)
		{
			if (m_lastRowEntered < 0 || m_lastRowEntered >= dataGridUns.RowCount)
				return;
			// TODO-Linux: GetRowDisplayRectangle doesn't use cutVoerflow parameter on Mono
			int heightOfDisplayedPortionOfRow = dataGridUns.GetRowDisplayRectangle(m_lastRowEntered, true).Height;
			if (heightOfDisplayedPortionOfRow != dataGridUns.Rows[m_lastRowEntered].Height)
			{
				// Changing panel sizes have now hidden the current row. Need to scroll it into view.
				int iNewFirstRow = dataGridUns.FirstDisplayedScrollingRowIndex + 1; // bump it up at least 1 whole row
				if (heightOfDisplayedPortionOfRow == 0)
					iNewFirstRow++; // Completely hidden, so bump it up at least one more row.
				int iRow = m_lastRowEntered;
				while (iRow > 0 && !dataGridUns.Rows[--iRow].Displayed && iNewFirstRow < dataGridUns.RowCount)
					iNewFirstRow++;
				if (iNewFirstRow < dataGridUns.RowCount)
					dataGridUns.FirstDisplayedScrollingRowIndex = iNewFirstRow;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuIncludeQuestion or mnuExcludeQuestion control.
		/// </summary>
		/// <param name="sender">The menu that was the source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void mnuIncludeOrExcludeQuestion_Click(object sender, EventArgs e)
		{
			if (dataGridUns.CurrentRow == null)
				return;

			CurrentPhrase.IsExcluded = (sender == mnuExcludeQuestion);
			Reload(true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuEditQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void mnuEditQuestion_Click(object sender, EventArgs e)
		{
			TranslatablePhrase phrase = CurrentPhrase;
			m_selectKeyboard(false);
			using (EditQuestionDlg dlg = new EditQuestionDlg(phrase))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					phrase.ModifiedPhrase = dlg.ModifiedPhrase;
					Reload(true);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuInsertQuestion or mnuAddQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void InsertOrAddQuestion(object sender, EventArgs e)
		{
			TranslatablePhrase phrase = CurrentPhrase;
			m_selectKeyboard(false);
			using (NewQuestionDlg dlg = new NewQuestionDlg(phrase.QuestionInfo))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					if (sender == mnuInsertQuestion)
						phrase.InsertedPhraseBefore = dlg.NewQuestion;
					else
						phrase.AddedPhraseAfter = dlg.NewQuestion;
					string sRef = phrase.Reference;

					Reload(true, dlg.NewQuestion, dataGridUns.CurrentRow.Index);
				}
			}
		}

		private void dataGridUns_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
		{
			e.ContextMenuStrip = (m_helper[e.RowIndex].Category == -1) ? null : dataGridContextMenu;
		}

		private void dataGridContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool fExcluded = CurrentPhrase.IsExcluded;
			mnuExcludeQuestion.Visible = !fExcluded;
			mnuIncludeQuestion.Visible = fExcluded;
			mnuEditQuestion.Enabled = !fExcluded;
		}

		private void dataGridUns_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			if (m_helper[e.RowIndex].IsExcluded)
				dataGridUns.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
		}

		private void dataGridUns_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
			{
				dataGridUns.CurrentCell = dataGridUns.Rows[e.RowIndex].Cells[e.ColumnIndex];
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuReferenceRange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void mnuReferenceRange_Click(object sender, EventArgs e)
		{
			m_selectKeyboard(false);
			using (ScrReferenceFilterDlg dlg = new ScrReferenceFilterDlg(m_masterVersification,
                new BCVRef(m_startRef), new BCVRef(m_endRef), m_availableBookIds))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					m_startRef = dlg.FromRef;
					m_endRef = dlg.ToRef;
					ApplyFilter();
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event for one of the biblical terms list boxes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void KeyTermRenderingSelected(TermRenderingCtrl sender)
		{
			if (dataGridUns.CurrentRow == null)
				return;
			if (sender.SelectedRendering == null)
				return;
			int rowIndex = dataGridUns.CurrentRow.Index;
			m_helper[rowIndex].ReplaceKeyTermRendering(FindTermRenderingInUse(sender, rowIndex),
				sender.SelectedRendering);
			dataGridUns.InvalidateRow(rowIndex);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Need to invalidate any columns that might be showing key term renderings.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void KeyTermBestRenderingsChanged()
		{
			dataGridUns.InvalidateColumn(m_colTranslation.Index);
			if (dataGridUns.ColumnCount == (m_colDebugInfo.Index + 1))
				dataGridUns.InvalidateColumn(m_colDebugInfo.Index);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the CheckStateChanged event of the btnSendScrReferences control.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void btnSendScrReferences_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendScrRefs = btnSendScrReferences.Checked;
            if (btnSendScrReferences.Checked && dataGridUns.CurrentRow != null)
                SendScrReference(dataGridUns.CurrentRow.Index);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the CheckStateChanged event of the btnSendScrReferences control.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void btnReceiveScrReferences_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReceiveScrRefs = btnReceiveScrReferences.Checked;
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the VisibleChanged event of the m_pnlAnswersAndComments control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_pnlAnswersAndComments_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlAnswersAndComments.Visible && dataGridUns.CurrentRow != null)
				LoadAnswerAndComment(dataGridUns.CurrentRow.Index);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the biblicalTermsRenderingSelectionRulesToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void biblicalTermsRenderingSelectionRulesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (RenderingSelectionRulesDlg dlg = new RenderingSelectionRulesDlg(
				m_helper.TermRenderingSelectionRules, m_selectKeyboard))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					m_helper.TermRenderingSelectionRules = new List<RenderingSelectionRule>(dlg.Rules);
					KeyTermBestRenderingsChanged();
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the EditingControlShowing event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DataGridViewEditingControlShowingEventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			Debug.WriteLine("dataGridUns_EditingControlShowing: m_lastTranslationSet = " + m_lastTranslationSet);
			TextControl = e.Control as DataGridViewTextBoxEditingControl;
			if (TextControl == null)
				return;
			TextControl.KeyDown += txtControl_KeyDown;
			TextControl.PreviewKeyDown += txtControl_PreviewKeyDown;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellEndEdit event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			Debug.WriteLine("dataGridUns_CellEndEdit: m_lastTranslationSet = " + m_lastTranslationSet);
			if (TextControl != null)
			{
				TextControl.KeyDown -= txtControl_KeyDown;
				TextControl.PreviewKeyDown -= txtControl_PreviewKeyDown;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SplitterMoving event of the m_hSplitter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.SplitterEventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_hSplitter_SplitterMoving(object sender, SplitterEventArgs e)
		{
			if (e.SplitY < dataGridUns.Top + dataGridUns.MinimumSize.Height)
				e.SplitY = dataGridUns.Top + dataGridUns.MinimumSize.Height;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Resize event of the m_biblicalTermsPane control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_biblicalTermsPane_Resize(object sender, EventArgs e)
		{
			if (!m_loadingBiblicalTermsPane)
				MaximumHeightOfKeyTermsPane = m_biblicalTermsPane.Height;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the KeyDown event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void txtControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (EditingTranslation && e.Alt && e.Shift && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))
			{
				e.SuppressKeyPress = TextControl.MoveSelectedWord(e.KeyCode == Keys.Right);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the PreviewKeyDown event of the txtControl control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void txtControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (!InTranslationCell || e.Shift || e.Control)
				return;
			if (e.KeyCode == Keys.Home)
				TextControl.Select(0, 0);
			else if (e.KeyCode == Keys.End)
				TextControl.Select(TextControl.TextLength, 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles copying and pasting cell contents (TXL-100)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuCopy_Click(object sender, EventArgs e)
		{
			if (txtFilterByPart.Focused)
				txtFilterByPart.Copy();
			else if (EditingTranslation)
			{
				string text = dataGridUns.CurrentCell.Value as string;
				if (text != null)
					Clipboard.SetText(text);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles copying and pasting cell contents (TXL-100)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuPaste_Click(object sender, EventArgs e)
		{
			if (txtFilterByPart.Focused)
				txtFilterByPart.Paste();
			else if (EditingTranslation)
			{
				string text = Clipboard.GetText();
				if (!string.IsNullOrEmpty(text))
					m_helper[dataGridUns.CurrentCell.RowIndex].Translation = text; SaveNeeded = true;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the aboutTransceleratorToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuHelpAbout_Click(object sender, EventArgs e)
		{
			using (HelpAboutDlg dlg = new HelpAboutDlg())
			{
				dlg.ShowDialog();
			}
		}
		#endregion

		#region Private helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the Scripture reference of the row corresponding to the given index.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private BCVRef GetScrRefOfRow(int iRow)
		{
			string sRef = dataGridUns.Rows[iRow].Cells[m_colReference.Index].Value as string;
			if (string.IsNullOrEmpty(sRef))
				return null;
			int ichDash = sRef.IndexOf('-');
			if (ichDash > 0)
				sRef = sRef.Substring(0, ichDash);
			return new BCVRef(sRef);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the translations.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadTranslations(TxlSplashScreen splashScreen)
		{
			if (splashScreen != null)
				splashScreen.Message = Properties.Resources.kstidSplashMsgLoadingQuestions;

			Exception e;
            m_keyTermRulesFilename = Path.Combine(Path.GetDirectoryName(
                 Assembly.GetExecutingAssembly().Location) ?? string.Empty, "keyTermRules.xml");


			KeyTermRules rules = XmlSerializationHelper.DeserializeFromFile<KeyTermRules>(m_keyTermRulesFilename, out e);
			if (e != null)
				MessageBox.Show(e.ToString(), Text);
            else
                rules.Initialize(m_getKeyTermListName == null ? null : m_getKeyTermListName());

			List<PhraseCustomization> customizations = null;
		    string phraseCustData = m_fileProxy.Read(DataFileProxy.DataFileId.QuestionCustomizations);
			if (!string.IsNullOrEmpty(phraseCustData))
			{
				customizations = XmlSerializationHelper.DeserializeFromString<List<PhraseCustomization>>(phraseCustData, out e);
				if (e != null)
					MessageBox.Show(e.ToString());
			}

			QuestionProvider qp = new QuestionProvider(m_questionsFilename, customizations);
            m_helper = new PhraseTranslationHelper(qp, m_getKeyTerms(), rules, m_phraseSubstitutions);
		    m_helper.FileProxy = m_fileProxy;
			m_sectionHeadText = qp.SectionHeads;
			m_availableBookIds = qp.AvailableBookIds;
		    string translationData = m_fileProxy.Read(DataFileProxy.DataFileId.Translations);
			if (!string.IsNullOrEmpty(translationData))
			{
				if (splashScreen != null)
					splashScreen.Message = Properties.Resources.kstidSplashMsgLoadingTranslations;			

				List<XmlTranslation> translations = XmlSerializationHelper.DeserializeFromString<List<XmlTranslation>>(translationData, out e);
				if (e != null)
					MessageBox.Show(e.ToString());
				else
				{
					foreach (XmlTranslation unsTranslation in translations)
					{
						TranslatablePhrase phrase = m_helper.GetPhrase(unsTranslation.Reference, unsTranslation.PhraseKey);
						if (phrase != null && !phrase.IsExcluded)
							phrase.Translation = unsTranslation.Translation;
					}
				}
			}
			m_helper.ProcessAllTranslations();
			m_helper.TranslationsChanged += m_helper_TranslationsChanged;

			dataGridUns.RowCount = m_helper.Phrases.Count();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the key terms pane.
		/// </summary>
		/// <param name="rowIndex">Index of the row to load for.</param>
		/// ------------------------------------------------------------------------------------
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="ktRenderCtrl gets added to m_biblicalTermsPane.Controls collection and disposed there")]
		private void LoadKeyTermsPane(int rowIndex)
		{
			m_loadingBiblicalTermsPane = true;
			m_biblicalTermsPane.SuspendLayout();
			ClearBiblicalTermsPane();
			m_biblicalTermsPane.Height = MaximumHeightOfKeyTermsPane;
			int col = 0;
			int longestListHeight = 0;
			Dictionary<KeyTermMatch, int> previousKeyTermEndOfRenderingOffsets = new Dictionary<KeyTermMatch, int>();
			foreach (KeyTermMatch keyTerm in m_helper[rowIndex].GetParts().OfType<KeyTermMatch>().Where(ktm => ktm.Renderings.Any()))
			{
				int ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm;
				previousKeyTermEndOfRenderingOffsets.TryGetValue(keyTerm, out ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm);
				TermRenderingCtrl ktRenderCtrl = new TermRenderingCtrl(keyTerm,
					ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm, m_selectKeyboard, LookupTerm);
				ktRenderCtrl.VernacularFont = m_vernFont;

				SubstringDescriptor sd = FindTermRenderingInUse(ktRenderCtrl, rowIndex);
				if (sd == null)
				{
					// Didn't find any renderings for this term in the translation, so don't select anything
					previousKeyTermEndOfRenderingOffsets[keyTerm] = m_helper[rowIndex].Translation.Length;
				}
				else
				{
					previousKeyTermEndOfRenderingOffsets[keyTerm] = sd.EndOffset;
					ktRenderCtrl.SelectedRendering = m_helper[rowIndex].Translation.Substring(sd.Offset, sd.Length);
				}
				ktRenderCtrl.Dock = DockStyle.Fill;
				m_biblicalTermsPane.Controls.Add(ktRenderCtrl, col, 0);
				if (ktRenderCtrl.NaturalHeight > longestListHeight)
					longestListHeight = ktRenderCtrl.NaturalHeight;
				ktRenderCtrl.SelectedRenderingChanged += KeyTermRenderingSelected;
				ktRenderCtrl.BestRenderingsChanged += KeyTermBestRenderingsChanged;
				col++;
			}
			m_biblicalTermsPane.ColumnCount = col;
			ResizeKeyTermPaneColumns();
			m_biblicalTermsPane.Height = Math.Min(longestListHeight, MaximumHeightOfKeyTermsPane);
			m_biblicalTermsPane.Visible = m_biblicalTermsPane.ColumnCount > 0;
			m_biblicalTermsPane.ResumeLayout();
			m_loadingBiblicalTermsPane = false;
		}

		private void LookupTerm(IEnumerable<IKeyTerm> terms)
		{
		    List<IKeyTerm> prioritizedTerms = new List<IKeyTerm>();
		    List<IKeyTerm> termsNotOccurringInCurrentRefRange = new List<IKeyTerm>();
		    int startRef = CurrentPhrase.StartRef;
		    int endRef = CurrentPhrase.EndRef;
            foreach (IKeyTerm keyTerm in terms)
            {
                if (keyTerm.BcvOccurences.Any(o => o >= startRef && o <= endRef))
                    prioritizedTerms.Add(keyTerm);
                else
                    termsNotOccurringInCurrentRefRange.Add(keyTerm);
            }
		    prioritizedTerms.AddRange(termsNotOccurringInCurrentRefRange);
			m_lookupTermDelegate(prioritizedTerms);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Resizes the key term pane columns.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void ResizeKeyTermPaneColumns()
		{
			int columnsToFit = m_biblicalTermsPane.ColumnCount;
			if (columnsToFit == 0)
				return;
			int colWidth = (m_biblicalTermsPane.ClientSize.Width) / columnsToFit;
			SizeType type = SizeType.Percent;
			if (colWidth < m_biblicalTermsPane.Controls[0].MinimumSize.Width)
			{
				type = SizeType.AutoSize;
				colWidth = m_biblicalTermsPane.Controls[0].MinimumSize.Width;
			}
			m_biblicalTermsPane.ColumnStyles.Clear();
			for (int iCol = 0; iCol < columnsToFit; iCol++)
			{
				m_biblicalTermsPane.ColumnStyles.Add(new ColumnStyle(
					type, colWidth));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the counts and filter status.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateCountsAndFilterStatus()
		{
			if (m_helper.FilteredPhraseCount == m_helper.UnfilteredPhraseCount)
			{
				lblFilterIndicator.Text = (string)lblFilterIndicator.Tag;
				lblFilterIndicator.Image = null;
			}
			else
			{
				lblFilterIndicator.Text = Properties.Resources.kstidFilteredStatus;
				lblFilterIndicator.Image = Properties.Resources.Filtered;
			}
			lblRemainingWork.Text = string.Format((string)lblRemainingWork.Tag,
				m_helper.Phrases.Count(p => !p.HasUserTranslation), m_helper.FilteredPhraseCount);
			lblRemainingWork.Visible = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clears the biblical terms pane.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void ClearBiblicalTermsPane()
		{
			foreach (TermRenderingCtrl ctrl in m_biblicalTermsPane.Controls.OfType<TermRenderingCtrl>())
				ctrl.SelectedRenderingChanged -= KeyTermRenderingSelected;
			m_biblicalTermsPane.Controls.Clear();
			m_biblicalTermsPane.ColumnCount = 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the term rendering (from the known ones in the renderingInfo) in use in
		/// the current translation.
		/// </summary>
		/// <param name="renderingInfo">The information about a single occurrence of a key
		/// biblical term and its rendering in a string in the target language.</param>
		/// <param name="rowIndex">Index of the row.</param>
		/// <returns>An object that indicates where in the translation string the match was
		/// found (offset and length)</returns>
		/// ------------------------------------------------------------------------------------
		private SubstringDescriptor FindTermRenderingInUse(ITermRenderingInfo renderingInfo, int rowIndex)
		{
			return m_helper[rowIndex].FindTermRenderingInUse(renderingInfo);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sends the start reference for the given row as a Santa-Fe "focus" message.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SendScrReference(int iRow)
		{
			m_fIgnoreNextRecvdSantaFeSyncMessage = true;
			BCVRef currRef = GetScrRefOfRow(iRow);
			if (currRef != null && currRef.Valid)
                SantaFeFocusMessageHandler.SendFocusMessage(currRef.ToString());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes the received sync message.
		/// </summary>
		/// <param name="reference">The reference (in English versification scheme).</param>
		/// ------------------------------------------------------------------------------------
		private void ProcessReceivedMessage(BCVRef reference)
		{
			// While we process the given reference we might get additional synch events, the
			// most recent of which we store in m_queuedReference. If we're done
			// and we have a new reference in m_queuedReference we process that one, etc.
			for (; reference != null; reference = m_queuedReference)
			{
				m_queuedReference = null;
				m_fProcessingSyncMessage = true;

				try
				{
					if (reference.Valid && (dataGridUns.CurrentRow == null ||
						!QuestionCoversRef(dataGridUns.CurrentRow.Index, reference)))
					{
						GoToReference(reference);
					}
				}
				finally
				{
					m_fProcessingSyncMessage = false;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the question in the given row covers the given Scripture
		/// reference.
		/// </summary>
		/// <param name="iRow">The index of the row</param>
		/// <param name="reference">The reference.</param>
		/// ------------------------------------------------------------------------------------
		internal bool QuestionCoversRef(int iRow, BCVRef reference)
		{
			string sRef = dataGridUns.Rows[iRow].Cells[m_colReference.Index].Value as string;
			BCVRef bcvStartRef, bcvEndRef;
            sRef.ParseRefRange(out bcvStartRef, out bcvEndRef);
			return reference >= bcvStartRef && reference <= bcvEndRef;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Goes to the first row in the data grid corresponding to the given reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void GoToReference(BCVRef reference)
		{
			for (int iRow = 0; iRow < dataGridUns.Rows.Count; iRow++)
			{
				if (QuestionCoversRef(iRow, reference))
				{
					dataGridUns.CurrentCell = dataGridUns.Rows[iRow].Cells[dataGridUns.CurrentCell.ColumnIndex];
					return;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the answer and comment labels for the given row.
		/// </summary>
		/// <param name="rowIndex">Index of the row.</param>
		/// ------------------------------------------------------------------------------------
		private void LoadAnswerAndComment(int rowIndex)
		{
			Question answersAndComments = m_helper[rowIndex].QuestionInfo;
			if (answersAndComments == null || ((answersAndComments.Answers == null || answersAndComments.Answers.Length == 0) &&
				(answersAndComments.Notes == null || answersAndComments.Notes.Length == 0)))
			{
				m_lblAnswerLabel.Visible = m_lblAnswers.Visible = false;
				m_lblCommentLabel.Visible = m_lblComments.Visible = false;
				return;
			}
			PopulateAnswerOrCommentLabel(answersAndComments.Answers, m_lblAnswerLabel,
				m_lblAnswers, Properties.Resources.kstidAnswersLabel);
			PopulateAnswerOrCommentLabel(answersAndComments.Notes, m_lblCommentLabel,
				m_lblComments, Properties.Resources.kstidCommentsLabel);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Populates the answer or comment label.
		/// </summary>
		/// <param name="details">The list of answers or comments.</param>
		/// <param name="label">The label that has the "Answer:" or "Comment:" label.</param>
		/// <param name="contents">The label that is to be populated with the actual answer(s)
		/// or comment(s).</param>
		/// <param name="sLabelMultiple">The text to use for <see cref="label"/> if there are
		/// multiple answers/comments.</param>
		/// ------------------------------------------------------------------------------------
		private static void PopulateAnswerOrCommentLabel(IEnumerable<string> details,
			Label label, Label contents, string sLabelMultiple)
		{
			label.Visible = contents.Visible = details != null;
			if (label.Visible)
			{
				label.Show();
				label.Text = (details.Count() == 1) ? (string)label.Tag : sLabelMultiple;
				contents.Text = details.ToString(Environment.NewLine + "\t");
			}
		}
		#endregion
	}
	#endregion

	#region class SubstringDescriptor
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Simple class to allow methods to pass an offset and a length in order to descibe a
	/// substring.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SubstringDescriptor
	{
		public int Offset { get; set; }
		public int Length { get; set; }

		public int EndOffset
		{
			get { return Offset + Length; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(int offset, int length)
		{
			Offset = offset;
			Length = length;
		}
	}
	#endregion
}