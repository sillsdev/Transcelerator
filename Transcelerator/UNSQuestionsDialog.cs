// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.   
// <copyright from='2011' to='2018 company='SIL International'>
//		Copyright (c) 2018, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
using System.Media;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AddInSideViews;
using Keyman7Interop;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using SIL.Windows.Forms.Scripture;
using SIL.Utils;
using SIL.Windows.Forms.FileDialogExtender;
using SIL.Xml;
using File = System.IO.File;
using SIL.ComprehensionCheckingData;
using SIL.Transcelerator.Properties;

namespace SIL.Transcelerator
{
	#region UNSQuestionsDialog class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// UNSQuestionsDialog.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class UNSQuestionsDialog : Form, IMessageFilter
	{
		#region Constants
		private const string kKeyTermRulesFilename = "keyTermRules.xml";
		#endregion

		#region Member Data
		private readonly string m_projectName;
	    private readonly Func<IEnumerable<IKeyTerm>> m_getKeyTerms;
	    private readonly Font m_vernFont;
		private readonly string m_vernIcuLocale;
		private readonly string m_vernLanguageName;
		private readonly bool m_fVernIsRtoL;
		private readonly Action<bool> m_selectKeyboard;
	    private readonly Func<string, IList<int>> m_getTermOccurrences;
	    private readonly Action m_helpDelegate;
		private readonly Action<IList<string>> m_lookupTermDelegate;
		private readonly bool m_fEnableDragDrop;
		private LocalizationsFileAccessor m_dataLocalizer;
		private PhraseTranslationHelper m_helper;
        private readonly DataFileAccessor m_fileAccessor;
		private readonly string m_defaultLcfFolder = null;
        private readonly IScrExtractor m_scrExtractor;
	    private readonly Func<string> m_getCss;
	    private readonly string m_appName;
		private readonly string m_installDir;
        private readonly IScrVers m_masterVersification;
        private readonly IScrVers m_projectVersification;
	    private BCVRef m_startRef;
        private BCVRef m_endRef;
        private SortedDictionary<int, SectionInfo> m_sectionInfo;
		private int[] m_availableBookIds;
		private readonly string m_masterQuestionsFilename;
        private static readonly string s_programDataFolder;
		private static Regex s_regexGlossaryEntry;
        private readonly string m_parsedQuestionsFilename;
		private DateTime m_lastSaveTime;
		private MasterQuestionParser m_parser;
		/// <summary>Use PhraseSubstitutions property to ensure non-null cache</summary>
		private List<Substitution> m_cachedPhraseSubstitutions;
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
		private SubstringDescriptor m_lastTranslationSelectionState;
		private bool m_preventReEntrantCommitEditDuringSave = false;
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

		private List<Tuple<string, string>> AvailableLocales { get; set; }

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
					Save(true, false);
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
	    static UNSQuestionsDialog()
	    {

		    // On Windows, CommonApplicationData is actually the preferred location for this because it is not user-specific, but we do it this way to make it work on Linux.
			try
			{
				var deprecatedProgramDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SIL", "Transcelerator");
			    if (Directory.Exists(deprecatedProgramDataFolder))
			    {
				    var cachedQuestionsFilename = Path.Combine(deprecatedProgramDataFolder, TxlCore.kQuestionsFilename);
					if (File.Exists(cachedQuestionsFilename))
						File.Delete(cachedQuestionsFilename);
					Directory.Delete(deprecatedProgramDataFolder);
				}
		    }
		    catch (Exception)
		    {
				// This was just a clean-up step from a possible previous version of Transcelerator, so if something goes
				// wrong, ignore it.
		    }
			
			s_programDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIL", "Transcelerator");
             if (!Directory.Exists(s_programDataFolder))
                 Directory.CreateDirectory(s_programDataFolder);
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="UNSQuestionsDialog"/> class.
		/// </summary>
		/// <param name="splashScreen">The splash screen (can be null)</param>
		/// <param name="projectName">Name of the project.</param>
		/// <param name="getKeyTerms">Function to get collection of key terms.</param>
		/// <param name="getTermRenderings">Function to get renderings for given key term.</param>
		/// <param name="vernFont">The vernacular font.</param>
		/// <param name="vernIcuLocale">The vernacular icu locale.</param>
		/// <param name="vernLanguageName">The vernacular language name</param>
		/// <param name="fVernIsRtoL">if set to <c>true</c> the vernacular language is r-to-L].</param>
		/// <param name="datafileProxy">helper object to store and retrieve data.</param>
		/// <param name="scrExtractor">The Scripture extractor (can be null).</param>
		/// <param name="getCss">Function to retrieve the project's USFX cascading style sheet</param>
		/// <param name="appName">Name of the calling application</param>
		/// <param name="englishVersification">The versification typically used in English Bibles</param>
		/// <param name="projectVersification">The versification of the external project (to
		/// be used for passing references to the scrExtractor).</param>
		/// <param name="startRef">The starting Scripture reference to filter on</param>
		/// <param name="endRef">The ending Scripture reference to filter on</param>
		/// <param name="currRef">The current reference in the calling application</param>
		/// <param name="selectKeyboard">The delegate to select vern/anal keyboard.</param>
		/// <param name="getTermOccurrences">Function to get occurrences for given key term.</param>
		/// <param name="lookupTermDelegate">The lookup term delegate.</param>
		/// <param name="fEnableDragDrop">Allow drag-drop editing (moving text within a translation)</param>
		/// <param name="preferredUiLocale">THE BCP-47 locale identifier to use for the user
		/// interface (including localized questions, answers, etc.)</param>
		/// ------------------------------------------------------------------------------------
		public UNSQuestionsDialog(TxlSplashScreen splashScreen, string projectName,
            Func<IEnumerable<IKeyTerm>> getKeyTerms, Func<string, IList<string>> getTermRenderings,
            Font vernFont, string vernIcuLocale, string vernLanguageName, bool fVernIsRtoL, DataFileAccessor datafileProxy,
            IScrExtractor scrExtractor, Func<string> getCss, string appName, IScrVers englishVersification,
            IScrVers projectVersification, BCVRef startRef, BCVRef endRef, BCVRef currRef,
            Action<bool> selectKeyboard, Func<string, IList<int>> getTermOccurrences,
            Action<IList<string>> lookupTermDelegate, bool fEnableDragDrop, string preferredUiLocale)
		{
            if (splashScreen == null)
            {
                splashScreen = new TxlSplashScreen();
                splashScreen.Show(Screen.FromPoint(Properties.Settings.Default.WindowLocation));
            }
            splashScreen.Message = Properties.Resources.kstidSplashMsgInitializing;

            InitializeComponent();

#if DEBUG
		    generateOutputForArloToolStripMenuItem.Visible = true;
		    mnuLoadTranslationsFromTextFile.Visible = true;
#endif

            m_fileAccessor = datafileProxy;

            if (startRef != BCVRef.Empty && endRef != BCVRef.Empty && startRef > endRef)
				throw new ArgumentException("startRef must be before endRef");
			m_projectName = projectName;
	        m_getKeyTerms = getKeyTerms;
	        KeyTerm.GetTermRenderings = getTermRenderings;
	        m_vernFont = vernFont;
			m_vernIcuLocale = vernIcuLocale;
			m_vernLanguageName = vernLanguageName;
			m_fVernIsRtoL = fVernIsRtoL;
		    if (string.IsNullOrEmpty(m_vernIcuLocale))
                mnuGenerate.Enabled = false;
			m_selectKeyboard = selectKeyboard;
	        m_getTermOccurrences = getTermOccurrences;
	        m_lookupTermDelegate = lookupTermDelegate;
		    m_fEnableDragDrop = fEnableDragDrop;

			m_scrExtractor = scrExtractor;
	        m_getCss = getCss;
	        m_appName = appName;
            m_masterVersification = englishVersification;
            m_projectVersification = projectVersification;
		    TermRenderingCtrl.s_AppName = appName;
           
            m_startRef = startRef;
            m_endRef = endRef;
		    m_installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            
            m_masterQuestionsFilename = Path.Combine(m_installDir, TxlCore.kQuestionsFilename);
	        m_parsedQuestionsFilename = Path.Combine(s_programDataFolder, projectName, TxlCore.kQuestionsFilename);

			if (!String.IsNullOrEmpty(Properties.Settings.Default.OverrideDisplayLanguage))
				preferredUiLocale = Properties.Settings.Default.OverrideDisplayLanguage;

			PopulateAvailableLocales();
			AddAvailableLocalizationsToMenu(preferredUiLocale);
			SetLocalizer(preferredUiLocale);

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
			mnuProduceScriptureForgeFiles.Checked = Properties.Settings.Default.ProduceScriptureForgeFiles;
			mnuAutoSave.Checked = Properties.Settings.Default.AutoSave;

			DataGridViewCellStyle translationCellStyle = new DataGridViewCellStyle();
			translationCellStyle.Font = vernFont;
			m_colTranslation.DefaultCellStyle = translationCellStyle;
			if (fVernIsRtoL)
				m_colTranslation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			dataGridUns.RowTemplate.MinimumHeight = dataGridUns.RowTemplate.Height = m_normalRowHeight =
				(int)Math.Ceiling(vernFont.Height * CreateGraphics().DpiY / 72) + 2;
			Margin = new Padding(Margin.Left, toolStrip1.Height, Margin.Right, Margin.Bottom);

	        KeyTerm.FileAccessor = m_fileAccessor;

			LoadTranslations(splashScreen);

            dataGridUns.HandleCreated += (sender, args) => ProcessReceivedMessage(currRef);

			// Now apply settings that have filtering or other side-effects
			CheckedKeyTermFilterType = (PhraseTranslationHelper.KeyTermFilterType)Properties.Settings.Default.KeyTermFilterType;
			btnSendScrReferences.Checked = Properties.Settings.Default.SendScrRefs;

			splashScreen.Close();
		}

		private void PopulateAvailableLocales()
		{
			var locales = new List<Tuple<string, string>>();

			// All the commented-out code here is the "right" way to do this, but it requires adding ICU
			// to Transcelerator, which seems more bloat than is needed unless/until we really start seeing
			// a demand for ad-hoc localizations.
#if UseGlobalWritingSystemRepo
			Sldr.Initialize();
			try
			{
				var repo = GlobalWritingSystemRepository.Initialize();
#endif
			foreach (var locale in LocalizationsFileAccessor.GetAvailableLocales(m_installDir))
			{
				string languageName;
#if UseGlobalWritingSystemRepo
					if (repo.TryGet(locale, out WritingSystemDefinition wsDef))
					{
						languageName = wsDef.Language.Name;
					}
					else
#endif
				switch (locale)
				{
					case "es": languageName = "español"; break;
					case "fr": languageName = "français"; break;
					case "en-GB": languageName = "British English"; break;
					case "en-US":
					case "en":
						throw new ApplicationException("English (US) is the default. There should not be a localization for this language!");
					default: languageName = locale; break;
				}
				locales.Add(new Tuple<string, string>(languageName, locale));
			}
#if UseGlobalWritingSystemRepo
			}
			finally
			{
				Sldr.Cleanup();
			}
#endif
			AvailableLocales = locales.OrderBy(l => l.Item1).ToList();
		}

		private void AddAvailableLocalizationsToMenu(string preferredLocale)
		{
			var menuItemNameSuffix = en_ToolStripMenuItem.Name.Substring(2);
			foreach (var availableLocalization in AvailableLocales)
			{
				var subItem = new ToolStripMenuItem(availableLocalization.Item1)
				{
					Tag = availableLocalization.Item2,
					Name = availableLocalization.Item2 + menuItemNameSuffix
				};
				displayLanguageToolStripMenuItem.DropDownItems.Add(subItem);
				if (availableLocalization.Item2 == preferredLocale)
				{
					en_ToolStripMenuItem.Checked = false;
					subItem.Checked = true;
				}
				subItem.Click += HandleDisplayLanguageSelected;
			}
		}

		private void SetLocalizer(string preferredUiLocale)
		{
			m_dataLocalizer = GetDataLocalizer(preferredUiLocale);
		}

		private LocalizationsFileAccessor GetDataLocalizer(string localeId)
		{
			if (localeId == "en" || localeId == "en-US" || String.IsNullOrWhiteSpace(localeId))
				return null;

			if (m_dataLocalizer?.Locale == localeId)
				return m_dataLocalizer;

			var dataLocalizer = new LocalizationsFileAccessor(m_installDir, localeId);
			return dataLocalizer.Exists ? dataLocalizer : null;
		}

		#endregion

		#region Events
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.
		/// </param>
		/// ------------------------------------------------------------------------------------
		protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // On Windows XP, TXL comes up underneath Paratext. See if this fixes it:
            TopMost = true;
            TopMost = false;
			Application.AddMessageFilter(this);
        }

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
					Save(true, false);
					return;
				}
				switch (MessageBox.Show(this, "You have made changes. Do you wish to save before closing?",
					"Save changes?", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						Save(true, false);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
			if (!e.Cancel)
			{
				Properties.Settings.Default.Save();

				if (Properties.Settings.Default.ProduceScriptureForgeFiles)
					ProduceScriptureForgeFiles();
			}

			Application.RemoveMessageFilter(this);

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
        /// Cut cell contents (Translation column only) - from context menu for cell, not in
        /// editing mode
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutToClipboard();
        }

	    /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Copy cell contents - from context menu for cell, not in editing mode
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Paste cell contents (Translation column only)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Cuts cell contents (single Translation cell only)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (txtFilterByPart.Focused)
                txtFilterByPart.Copy();
            else if (EditingTranslation)
            {
                Clipboard.SetDataObject(new DataObject(TextControl.SelectedText));
                TextControl.SelectedText = string.Empty;
            }
            else
            {
                CutToClipboard();
            }
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
                string text = TextControl.SelectedText;
                if (text != null)
                    Clipboard.SetText(text);
            }
            else
            {
                CopyToClipboard();
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
                    TextControl.SelectedText = text;
            }
            else
            {
                PasteClipboardValue();
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Cut is only valid for a single Translation cell. Puts the formatted values that
        /// represent the contents of the selected cells onto the
        /// <see cref="T:System.Windows.Forms.Clipboard"/> and clears the existing cell value.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void CutToClipboard()
        {
            if (dataGridUns.CurrentCell.ColumnIndex != m_colTranslation.Index ||
                dataGridUns.SelectedCells.Count > 1)
                return;

            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cell
            dataGridUns.SelectedCells[0].Value = string.Empty;
            m_helper[dataGridUns.CurrentCell.RowIndex].HasUserTranslation = false;
            SaveNeeded = true;
            dataGridUns.InvalidateRow(dataGridUns.CurrentCell.RowIndex);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Puts the formatted values that represent the contents of the selected cells onto the
        /// <see cref="T:System.Windows.Forms.Clipboard"/>.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridUns.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Paste is only valid for a single Translation cell. Sets the value of the current
        /// cell from the text value on the <see cref="T:System.Windows.Forms.Clipboard"/>. To
        /// prevent pasting garbage, if clipboard contains any line breaks, this does nothing.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void PasteClipboardValue()
        {
            if (dataGridUns.CurrentCell.ColumnIndex != m_colTranslation.Index ||
                dataGridUns.SelectedCells.Count > 1)
                return;

            string clipboardText = Clipboard.GetText();
            if (!clipboardText.Contains('\n'))
                dataGridUns.CurrentCell.Value = clipboardText;

            SaveNeeded = true;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Navigates to the next untranslated question in the grid. Beeps if all currently
        /// filtered questions have been translated
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void nextUntranslatedQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int iRow = dataGridUns.CurrentRow.Index + 1; iRow < m_helper.FilteredPhraseCount; iRow++)
            {
                if (!m_helper[iRow].HasUserTranslation)
                {
                    dataGridUns.CurrentCell = dataGridUns.Rows[iRow].Cells[m_colTranslation.Index];
                    return;
                }
            }
            SystemSounds.Beep.Play();
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Navigates to the previous untranslated question in the grid. Beeps if all currently
        /// filtered questions have been translated
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void prevUntranslatedQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int iRow = dataGridUns.CurrentRow.Index - 1; iRow >= 0; iRow--)
            {
                if (!m_helper[iRow].HasUserTranslation)
                {
                    dataGridUns.CurrentCell = dataGridUns.Rows[iRow].Cells[m_colTranslation.Index];
                    return;
                }
            }
            SystemSounds.Beep.Play();
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
			m_selectKeyboard?.Invoke(InTranslationCell);
		}

		private void dataGridUns_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			m_lastTranslationSelectionState = null;
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
			var tp = m_helper[e.RowIndex];
			switch (e.ColumnIndex)
			{
				case 0: e.Value = tp.Reference; break;
				case 1:
					e.Value = m_dataLocalizer == null ? tp.PhraseInUse : m_dataLocalizer.GetLocalizedString(tp.ToUIDataString());
					break;
				case 2: e.Value = tp.Translation; break;
				case 3: e.Value = tp.HasUserTranslation; break;
				case 4: e.Value = tp.DebugInfo; break;
			}
		}

		private void dataGridUns_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (m_saving)
				return;

			PostponeRefresh = m_preventReEntrantCommitEditDuringSave = true;

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

			PostponeRefresh = m_preventReEntrantCommitEditDuringSave = false;
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
				case 0: m_helper.Sort(PhrasesSortedBy.Reference, sortAscending); break;
				case 1: m_helper.Sort(PhrasesSortedBy.EnglishPhrase, sortAscending); break;
				case 2: m_helper.Sort(PhrasesSortedBy.Translation, sortAscending); break;
				case 3: m_helper.Sort(PhrasesSortedBy.Status, sortAscending); break;
				case 4: m_helper.Sort(PhrasesSortedBy.Default, sortAscending); break;
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

			m_helper.Filter(txtFilterByPart.Text, MatchWholeWords, CheckedKeyTermFilterType, refFilter,
				mnuViewExcludedQuestions.Checked, m_dataLocalizer == null ? null :
				(Func<TranslatablePhrase, string>)(tp => m_dataLocalizer.GetLocalizedString(tp.ToUIDataString())));
			dataGridUns.RowCount = m_helper.Phrases.Count();

            dataGridUns.RowEnter += dataGridUns_RowEnter;

            int currentRow = dataGridUns.CurrentCell?.RowIndex ?? -1;
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

			if (dataGridUns.CurrentCell == null)
			{
				if (m_pnlAnswersAndComments.Visible)
					m_lblAnswerLabel.Visible = m_lblAnswers.Visible = m_lblCommentLabel.Visible = m_lblComments.Visible = false;
			}
			else if (currentRow == dataGridUns.CurrentCell.RowIndex)
			{
				dataGridUns_RowEnter(dataGridUns, new DataGridViewCellEventArgs(m_iCurrentColumn, currentRow));
			}

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
			Save(false, false);
		}

		private void Save(bool fForce, bool fSaveCustomizations)
		{
			if (m_saving || (!fForce && !SaveNeeded))
				return;
			m_saving = true;
			SaveNeeded = false;
			m_lastSaveTime = DateTime.Now;
			if (dataGridUns.IsCurrentCellInEditMode && !m_preventReEntrantCommitEditDuringSave)
				dataGridUns.EndEdit();
			m_fileAccessor.Write(DataFileAccessor.DataFileId.Translations, XmlSerializationHelper.SerializeToString(
				(from translatablePhrase in m_helper.UnfilteredPhrases
				where translatablePhrase.HasUserTranslation
				select new XmlTranslation(translatablePhrase)).ToList()));

			if (fSaveCustomizations)
			{
				List<PhraseCustomization> customizations = m_helper.CustomizedPhrases;

				if (customizations.Count > 0 || m_fileAccessor.Exists(DataFileAccessor.DataFileId.QuestionCustomizations))
				{
					m_fileAccessor.Write(DataFileAccessor.DataFileId.QuestionCustomizations,
						XmlSerializationHelper.SerializeToString(customizations));
				}
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
                defaultFolder, AvailableBookIds, m_sectionInfo.Values, AvailableLocales))
            {
	            dlg.DataLocalizerNeeded += (sender, id) => GetDataLocalizer(id);

				if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Func<TranslatablePhrase, bool> InRange;
                    if (dlg.m_rdoWholeBook.Checked)
                    {
                        int bookNum = BCVRef.BookToNumber((string)dlg.m_cboBooks.SelectedItem);
                        InRange = (tp) => BCVRef.GetBookFromBcv(tp.StartRef) == bookNum;
                    }
                    else
                    {
                        BCVRef startRef = dlg.VerseRangeStartRef;
                        BCVRef endRef = dlg.VerseRangeEndRef;
                        InRange = (tp) => tp.StartRef >= startRef && tp.EndRef <= endRef;
                    }

                    List<TranslatablePhrase> allPhrasesInRange = m_helper.AllActivePhrasesWhere(InRange).ToList();
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
                        sw.WriteLine("<title>" + dlg.NormalizedTitle + "</title>");
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
                        sw.WriteLine("<h1 lang=\"en\">" + dlg.NormalizedTitle + "</h1>");
                        int prevCategory = -1;
                        SectionInfo section = null;
                        string prevQuestionRef = null;
                        bool sectionHeadHasBeenOutput = false;

                        foreach (TranslatablePhrase phrase in allPhrasesInRange)
                        {
	                        var question = phrase.QuestionInfo;
	                        string lang;
                            if (section == null || phrase.EndRef > section.EndRef)
                            {
	                            if (!m_sectionInfo.TryGetValue(phrase.StartRef, out section))
									section = m_sectionInfo.Values.FirstOrDefault(s => s.StartRef <= phrase.StartRef && s.EndRef >= phrase.EndRef);
								if (section != null)
									sectionHeadHasBeenOutput = false;
								else
								{
									// This is a last-ditch fallback - should never happen.
									section = null;
								}

								prevCategory = -1;
                            }

                            if (!phrase.HasUserTranslation && (phrase.TypeOfPhrase == TypeOfPhrase.NoEnglishVersion || !dlg.m_rdoUseOriginal.Checked))
                                continue; // skip this question

                            if (!sectionHeadHasBeenOutput)
                            {
	                            if (section == null)
		                            sw.WriteLine($"<h2>{phrase.Reference}</h2>");
	                            else
	                            {
		                            var h2 = dlg.GetDataString(new UISectionHeadDataString(section), out lang);
									WriteParagraphElement(sw, null, h2, m_vernIcuLocale, lang, "h2");
	                            }
	                            sectionHeadHasBeenOutput = true;
                            }

                            if (phrase.Category != prevCategory)
                            {
	                            var lwcCategoryName = dlg.GetDataString(new UISimpleDataString(phrase.CategoryName, LocalizableStringType.Category), out lang);
								WriteParagraphElement(sw, null, lwcCategoryName, m_vernIcuLocale, lang, "h3");
                                prevCategory = phrase.Category;
                            }

                            if (prevQuestionRef != phrase.Reference)
                            {
                                if (phrase.Category > 0 || dlg.m_chkPassageBeforeOverview.Checked)
                                {
                                    int startRef = m_projectVersification.ChangeVersification(phrase.StartRef, m_masterVersification);
                                    int endRef = m_projectVersification.ChangeVersification(phrase.EndRef, m_masterVersification);
                                    if (m_scrExtractor == null)
                                    {
                                        sw.WriteLine("<p class=\"scripture\">");
                                        sw.WriteLine(@"\ref " + BCVRef.MakeReferenceString(startRef, endRef, ".", "-"));
                                        sw.WriteLine("</p>");
                                    }
                                    else
                                    {
                                        try
                                        {
											sw.Write(GetExtractedScripture(startRef, endRef));
                                        }
                                        catch (Exception ex)
                                        {
                                            sw.Write(ex.Message);
#if DEBUG
                                            throw;
#endif
                                        }
                                    }
                                }
                                prevQuestionRef = phrase.Reference;
                            }

	                        lang = m_vernIcuLocale;
	                        var questionText = phrase.HasUserTranslation ? phrase.Translation :
								dlg.GetDataString(phrase.ToUIDataString(), out lang);
	                        WriteParagraphElement(sw, "question", questionText, m_vernIcuLocale, lang);

							sw.WriteLine($"<div class=\"extras\" lang=\"{dlg.LwcLocale}\">");
	                        if (dlg.m_chkIncludeLWCQuestions.Checked && phrase.HasUserTranslation && phrase.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion)
	                        {
		                        var lwcQuestion = dlg.GetDataString(phrase.ToUIDataString(), out lang);
		                        WriteParagraphElement(sw, "questionbt", lwcQuestion, dlg.LwcLocale, lang);
	                        }
	                        if (dlg.m_chkIncludeLWCAnswers.Checked && question.Answers != null)
                            {
	                            for (var index = 0; index < question.Answers.Length; index++)
	                            {
		                            var lwcAnswer = dlg.GetDataString(new UIAnswerOrNoteDataString(question, LocalizableStringType.Answer, index), out lang);
		                            WriteParagraphElement(sw, "answer", lwcAnswer, dlg.LwcLocale, lang);
	                            }
                            }
                            if (dlg.m_chkIncludeLWCComments.Checked && question.Notes != null)
                            {
								for (var index = 0; index < question.Notes.Length; index++)
								{
									var lwcComment = dlg.GetDataString(new UIAnswerOrNoteDataString(question, LocalizableStringType.Note, index), out lang);
									WriteParagraphElement(sw, "comment", lwcComment, dlg.LwcLocale, lang);
	                            }
                            }
                            sw.WriteLine("</div>");
                        }

                        sw.WriteLine("</body>");
                    }
                    Process.Start(dlg.FileName);
                }
            }
		}

		private static void WriteParagraphElement(StreamWriter sw, string className, string data, string defaultLangInContext, string langOfData, string paragraphType = "p")
		{
			var langAttrib = langOfData == defaultLangInContext ? null : $" lang=\"{langOfData}\"";
			var classAttrib = className == null ? null : $" class=\"{className}\"";
			sw.WriteLine($"<{paragraphType}{classAttrib}{langAttrib}>{data.Normalize(NormalizationForm.FormC)}</{paragraphType}>");
		}

		private void ProduceScriptureForgeFiles()
		{
			var allAvailableLocalizers = LocalizationsFileAccessor.GetAvailableLocales(m_installDir).Select(GetDataLocalizer).ToList();

			int prevBook = -1;
			ComprehensionCheckingQuestionsForBook currentBookQuestions = null; 

			foreach (TranslatablePhrase phrase in m_helper.AllActivePhrasesWhere(p => p.HasUserTranslation))
			{
				var question = phrase.QuestionInfo;
				var startRef = new BCVRef(phrase.StartRef);
				int currBook = startRef.Book;
				if (currBook != prevBook)
				{
					if (currentBookQuestions != null)
					{
						m_fileAccessor.WriteBookSpecificData(DataFileAccessor.BookSpecificDataFileId.ScriptureForge,
							currentBookQuestions.BookId, XmlSerializationHelper.SerializeToString(currentBookQuestions));
					}
					currentBookQuestions = new ComprehensionCheckingQuestionsForBook
					{
						Lang = m_vernIcuLocale,
						BookId = BCVRef.NumberToBookCode(currBook),
						Questions = new List<ComprehensionCheckingQuestion>()
					};
					prevBook = currBook;
				}

				var q = new ComprehensionCheckingQuestion
				{
					Question = GetQuestionAlternates(phrase, allAvailableLocalizers),
					IsOverview = !phrase.IsDetail,
					Chapter = startRef.Chapter,
					StartVerse = startRef.Verse,
					Answers = GetMultilingualStrings(question, LocalizableStringType.Answer, allAvailableLocalizers),
					Notes = GetMultilingualStrings(question, LocalizableStringType.Note, allAvailableLocalizers)
				};

				if (phrase.StartRef != phrase.EndRef)
				{
					var endRef = new BCVRef(phrase.EndRef);
					if (startRef.Chapter != endRef.Chapter)
						q.EndChapter = endRef.Chapter;
					q.EndVerse = endRef.Verse;
				}
				
				currentBookQuestions.Questions.Add(q);
			}
			// Now output the final book's questions.
			if (currentBookQuestions != null)
			{
				m_fileAccessor.WriteBookSpecificData(DataFileAccessor.BookSpecificDataFileId.ScriptureForge,
					currentBookQuestions.BookId, XmlSerializationHelper.SerializeToString(currentBookQuestions));
			}
		}

		private StringAlt[] GetQuestionAlternates(TranslatablePhrase question, IReadOnlyList<LocalizationsFileAccessor> localizers)
		{
			var list = new List<StringAlt> {new StringAlt {Lang = m_vernIcuLocale, Text = question.Translation}};
			string variant = null;
			var locKey = question.ToUIDataString();
			list.Add(new StringAlt { Lang = "en-US", Text = locKey.SourceUIString });
			list.AddRange(from loc in localizers.Where(l => l.Locale != m_vernIcuLocale)
				where loc.TryGetLocalizedString(locKey, out variant)
				select new StringAlt { Lang = loc.Locale, Text = variant });
				
			return list.ToArray();
		}

		private StringAlt[][] GetMultilingualStrings(Question question, LocalizableStringType type, IReadOnlyList<LocalizationsFileAccessor> localizers)
		{
			string[] sourceStrings = type == LocalizableStringType.Answer ? question.Answers : question.Notes;
			if (sourceStrings == null)
				return null;
			string variant;
			var ms = new StringAlt[sourceStrings.Length][];
			for (var i = 0; i < sourceStrings.Length; i++)
			{
				var locKey = new UIAnswerOrNoteDataString(question, type, i);
				variant = null;
				List<StringAlt> list = new List<StringAlt> {new StringAlt {Lang = "en-US", Text = sourceStrings[i]}};
				list.AddRange(from loc in localizers
							  where loc.TryGetLocalizedString(locKey, out variant)
							  select new StringAlt {Lang = loc.Locale, Text = variant});
				ms[i] = list.ToArray();
			}
			return ms;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the requested range of Scripture text.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string GetExtractedScripture(int startRef, int endRef)
		{
			StringBuilder extractedScr = new StringBuilder();

			BCVRef startRefTemp = new BCVRef(startRef);
			BCVRef endRefTemp = new BCVRef(endRef);
			int endChapter = endRefTemp.Chapter;
			if (endRefTemp.Chapter > startRefTemp.Chapter)
			{
				endRefTemp = new BCVRef(startRefTemp);
				endRefTemp.Verse = m_projectVersification.GetLastVerse(endRefTemp.Book, endRefTemp.Chapter);
			}

			for (;;)
			{
				extractedScr.Append(GetExtractedScriptureFromSingleChapter(startRefTemp.BBCCCVVV, endRefTemp.BBCCCVVV));
				extractedScr.Append(Environment.NewLine);
				if (endRefTemp.Chapter == endChapter)
					break;
				startRefTemp.Chapter++;
				startRefTemp.Verse = 1;
				if (startRefTemp.Chapter == endChapter)
					endRefTemp = new BCVRef(endRef);
				else
				{
					endRefTemp.Chapter++;
					endRefTemp.Verse = m_projectVersification.GetLastVerse(endRefTemp.Book, endRefTemp.Chapter);
				}
			}
			return extractedScr.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the requested range of Scripture text. (Until Paratext 7.5 is available, this
		/// is limited to text within a single chapter.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string GetExtractedScriptureFromSingleChapter(int startRef, int endRef)
		{
			StringBuilder extractedScr = new StringBuilder(m_scrExtractor.Extract(startRef, endRef));
			// ENHANCE: Rather than getting the data as USFX and doing these somewhat-kludgey cleanup
			// steps, we could implement an HTML extractor in Paratext and let it do the transformation
			// using its XSLT scripts.
			const string usfxHead = "<usx version=\"2.0\">";
			const string usfxTail = "</usx>";
			if (extractedScr.ToString(0, usfxHead.Length) == usfxHead)
				extractedScr.Remove(0, usfxHead.Length);
			if (extractedScr.ToString(extractedScr.Length - usfxTail.Length, usfxTail.Length) == usfxTail)
				extractedScr.Remove(extractedScr.Length - usfxTail.Length, usfxTail.Length);
			extractedScr.Replace("para style=\"", "DIV class=\"usfm_");
			extractedScr.Replace("/para", "/DIV");
			extractedScr.Replace(" />", "></DIV>");

			if (s_regexGlossaryEntry == null)
				s_regexGlossaryEntry = new Regex("\\<char style=\"w\"\\>(?<surfaceFormOfGlossaryWord>[^|]*)\\|[^<]*\\</char\\>", RegexOptions.Compiled);

			return s_regexGlossaryEntry.Replace(extractedScr.ToString(), "${surfaceFormOfGlossaryWord}");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes a file with the questions having verse numbers in parentheses and an
		/// line containing the answers.
		/// </summary>
		/// ------------------------------------------------------------------------------------		
		private void generateOutputForArloToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var dlg = new SaveFileDialog())
			{
				dlg.DefaultExt = "txt";
				dlg.InitialDirectory = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					"SoftDev"), "Transcelerator");

				string sRef;
				Func<BCVRef, BCVRef, bool> InRange;
				if (m_startRef == BCVRef.Empty)
				{
					sRef = "- All";
					InRange = (bcvStart, bcvEnd) => true;
				}
				else
				{
					sRef = BCVRef.NumberToBookCode(m_startRef.Book);
					if (m_startRef.Book != m_endRef.Book)
						sRef += "-" + BCVRef.NumberToBookCode(m_endRef.Book);

					InRange = (bcvStart, bcvEnd) => bcvStart >= m_startRef && bcvEnd <= m_endRef;
				}

				string language = m_vernIcuLocale;
				string sChapter = "Chapter";
				string sPsalm = "Psalm";
				if (language == "es")
				{
					language = "Spanish";
					sChapter = "Capi\u0301tulo";
					sPsalm = "Salmo";
				}

				dlg.FileName = string.Format("Translations of {0} Questions {1}", language, sRef);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					try
					{
						using (var sw = new StreamWriter(dlg.FileName, false, Encoding.UTF8))
						{
							int questionNbr = 0;
							int lastBookNbr = -1;
							int lastChapterNbr = -1;
							int psalms = BCVRef.BookToNumber("PSA");
							var multilingScrBooks = new MultilingScrBooks();
							foreach (TranslatablePhrase phrase in m_helper.UnfilteredPhrases.Where(tp => tp.Category >= 0))
							{
								BCVRef startRef = phrase.StartRef;
								BCVRef endRef = phrase.EndRef;

								if (!InRange(startRef, endRef))
									continue;

								if (startRef.Book != lastBookNbr)
								{
									if (lastBookNbr != -1)
										sw.WriteLine();
									sw.WriteLine("{0} Study Questions in {1}", multilingScrBooks.GetBookName(startRef.Book), language);
									lastBookNbr = startRef.Book;
								}

								if (startRef.Chapter != lastChapterNbr)
								{
									questionNbr = 1;
									lastChapterNbr = startRef.Chapter;
									sw.WriteLine();
									sw.WriteLine("{0} {1}", lastBookNbr == psalms ? sPsalm : sChapter, lastChapterNbr);
									sw.WriteLine();
								}
								string sVerse = " (" + ((startRef.Verse != endRef.Verse)
									? startRef.Verse.ToString() + "-" + endRef.Verse.ToString()
									: startRef.Verse.ToString()) + ")";

								if (phrase.IsUserAdded)
									sw.WriteLine("***Added Question:");
								else if (phrase.ModifiedPhrase != null)
								{
									sw.WriteLine("***Modified Question");
									sw.WriteLine("Original: " + phrase.OriginalPhrase);
									sw.WriteLine("Modified: " + phrase.ModifiedPhrase);
								}

								if (!phrase.HasUserTranslation || phrase.IsExcluded)
								{
									sw.WriteLine("***Not translated:" + phrase.OriginalPhrase + sVerse);
								}
								else
								{
									sw.WriteLine("{0}. {1}", questionNbr, phrase.Translation + sVerse);
									questionNbr++;

									//var qi = phrase.QuestionInfo;
									//if (qi != null)
									//{
									//    if (qi.Answers != null)
									//    {
									//        foreach (string a in qi.Answers)
									//            sw.WriteLine("A: " + a);
									//    }
									//    if (qi.Notes != null)
									//    {
									//        foreach (string n in qi.Notes)
									//            sw.WriteLine("Note: " + n);
									//    }
									//}
								}
							}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, Text);
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes translations in a file having format: \rf BOOK C#, followed by questions
		/// with verse reference(s) in parentheses.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuLoadTranslationsFromTextFile_Click(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.CheckFileExists = true;
				dlg.Multiselect = true;
				dlg.RestoreDirectory = true;
				dlg.Filter = "Text Files (*.txt)|*.txt|All Supported Files (*.sfm;*.txt)|*.sfm;*.txt";
				dlg.InitialDirectory = @"C:\Projects\Transcelerator\Transcelerator\QTTGenerate\Original Word documents from Arlo\Spanish Translations";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					string reportFilename = Path.GetTempFileName();
					using (var reportWriter = new StreamWriter(reportFilename))
					{
						foreach (var filename in dlg.FileNames)
						{
							using (var reader = new StreamReader(filename, Encoding.UTF8))
							{
								m_helper.SetTranslationsFromText(reader, filename, m_masterVersification, reportWriter);
							}
						}
					}
					dataGridUns.Invalidate();
					MessageBox.Show("Finished! See report in " + reportFilename, TxlPlugin.pluginName);
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

            if (m_getCss != null)
                sw.WriteLine(m_getCss());
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
				Save(false, false);
			Properties.Settings.Default.AutoSave = mnuAutoSave.Checked;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the phraseSubstitutionsToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void phraseSubstitutionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
		    using (PhraseSubstitutionsDlg dlg = new PhraseSubstitutionsDlg(PhraseSubstitutions,
				m_helper.Phrases.Where(tp => tp.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion).Select(p => p.PhraseInUse),
				dataGridUns.CurrentRow.Index))
			{
				m_selectKeyboard(false);
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					PhraseSubstitutions.Clear();
					PhraseSubstitutions.AddRange(dlg.Substitutions);
                    m_fileAccessor.Write(DataFileAccessor.DataFileId.PhraseSubstitutions,
						XmlSerializationHelper.SerializeToString(PhraseSubstitutions));

					Reload(false);
				}
				m_selectKeyboard(true);
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
		/// ------------------------------------------------------------------------------------
		private void Reload(bool fForceSave)
		{
			TranslatablePhrase phrase = dataGridUns.CurrentRow != null ? CurrentPhrase : null;
			Reload(fForceSave, (phrase == null) ? null : phrase.PhraseKey, 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
        /// Reloads the data grid view and attempts to re-select the given question.
		/// </summary>
		/// <param name="fForceSave">if set to <c>true</c> data will be saved even if
		/// Transcelerator doesn't know the data is dirty. Note: Currently, when this method is
		/// called with fForceSave set, it's always in response to a customization change, so
		/// this parameter will also control whether customization changes are saved.</param>
		/// <param name="key">The key of the question to try to select after reloading.</param>
		/// <param name="fallBackRow">the index of the row to select if a question witht the
		/// given key cannot be found.</param>
		/// ------------------------------------------------------------------------------------
		private void Reload(bool fForceSave, IQuestionKey key, int fallBackRow)
		{
			using (new WaitCursor(this))
			{
				int iCol = dataGridUns.CurrentCell.ColumnIndex;
				Save(fForceSave, fForceSave); // See comment above for fForceSave

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
				dataGridUns.CurrentCell = null;
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
					{
						try
						{
							dataGridUns.CurrentCell = dataGridUns.Rows[iRow].Cells[iCol];
						}
						catch (InvalidOperationException e)
						{
							Debug.Fail("Got the ever-elusive InvalidOperationException: " + e.Message);
							
							// What to do? Ignore?
						}
					}
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
			Save(true, true);
			var addressToSelect = dataGridUns.CurrentCellAddress;
			ApplyFilter();
			dataGridUns.RowCount = m_helper.Phrases.Count();
			if (dataGridUns.RowCount == addressToSelect.Y)
				addressToSelect.Y--;
			dataGridUns.CurrentCell = dataGridUns.Rows[addressToSelect.Y].Cells[addressToSelect.X];
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
			using (EditQuestionDlg dlg = new EditQuestionDlg(phrase, m_helper.GetMatchingPhrases(phrase.StartRef, phrase.EndRef, phrase.Category).Where(p => p != phrase).Select(p => p.PhraseInUse).ToList(), m_dataLocalizer))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					int currentCol = dataGridUns.CurrentCellAddress.X;
					if (m_parser == null)
						m_parser = new MasterQuestionParser(GetQuestionWords(), m_getKeyTerms(), GetKeyTermRules(), PhraseSubstitutions);
					m_helper.ModifyQuestion(phrase, dlg.ModifiedPhrase, m_parser);
					Save(true, true);
					int row = m_helper.FindPhrase(phrase.QuestionInfo);
					dataGridUns.CurrentCell = dataGridUns.Rows[row].Cells[currentCol];
					dataGridUns.InvalidateRow(row);
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
		private void AddNewQuestion(object sender, EventArgs e)
		{
			m_selectKeyboard(false);
			string language = string.Format("{0} ({1})", m_vernLanguageName, m_vernIcuLocale);
			using (NewQuestionDlg dlg = new NewQuestionDlg(CurrentPhrase, language, m_projectVersification, m_masterVersification, m_helper, m_availableBookIds, m_selectKeyboard))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (m_parser == null)
						m_parser = new MasterQuestionParser(GetQuestionWords(), m_getKeyTerms(), GetKeyTermRules(), PhraseSubstitutions);

					Question newQuestion;
					var basePhrase = dlg.BasePhrase;
					if (basePhrase == null)
					{
						var startRef = dlg.StartReference;
						var endRef = dlg.EndReference;
						newQuestion = new Question(BCVRef.MakeReferenceString(startRef, endRef, ".", "-"),
							startRef.BBCCCVVV, endRef.BBCCCVVV, dlg.EnglishQuestion, dlg.Answer);
					}
					else
					{
						newQuestion = new Question(basePhrase.QuestionInfo, dlg.EnglishQuestion, dlg.Answer);

						if (dlg.InsertBeforeBasePhrase)
							basePhrase.InsertedPhraseBefore = newQuestion;
						else
							basePhrase.AddedPhraseAfter = newQuestion;
					}

					var newPhrase = m_helper.AddQuestion(newQuestion, dlg.Category, dlg.SequenceNumber, m_parser);
					if (basePhrase == null)
						m_helper.AttachNewQuestionToAdjacentPhrase(newPhrase);

					if (dlg.Translation != string.Empty)
						newPhrase.Translation = dlg.Translation;

					Save(true, true);
					dataGridUns.RowCount = m_helper.Phrases.Count();
					
					dataGridUns.CurrentCell = dataGridUns.Rows[m_helper.FindPhrase(newPhrase.QuestionInfo)].Cells[m_colTranslation.Index];
					UpdateCountsAndFilterStatus();
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
			mnuExcludeQuestion.Visible = !fExcluded && CurrentPhrase.Category != -1; // Can't exclude categories
			mnuIncludeQuestion.Visible = fExcluded;
			mnuEditQuestion.Enabled = !fExcluded;
		}

		private void dataGridUns_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			dataGridUns.Rows[e.RowIndex].DefaultCellStyle.BackColor = (m_helper[e.RowIndex].IsExcluded) ?
				Color.LightCoral : dataGridUns.DefaultCellStyle.BackColor;
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
                    Properties.Settings.Default.FilterStartRef = m_startRef;
                    Properties.Settings.Default.FilterEndRef = m_endRef;
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
			if (m_helper[rowIndex].InsertKeyTermRendering(sender, m_lastTranslationSelectionState,
				sender.SelectedRendering))
			{
				// Replacement was based on previous editing selection, so put the translation
				// back into edit mode, and select the inserted rendering.
				dataGridUns.BeginEdit(false);
				// Start and Length values may have been modified
				TextControl.SelectionStart = m_lastTranslationSelectionState.Start;
				TextControl.SelectionLength = m_lastTranslationSelectionState.Length;
				m_lastTranslationSelectionState = null;
				TextControl.Focus();
			}
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
		/// Handles the VisibleChanged event of the m_pnlAnswersAndComments control. Also used
		/// to re-populate controls when Display language changes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadAnswersAndCommentsIfShowing(object sender, EventArgs e)
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
			TextControl.AllowDrop = true;
			TextControl.DragDrop += TextControl_DragDrop;
			TextControl.DragEnter += TextControl_Drag;
			TextControl.DragOver += TextControl_Drag;
			TextControl.GiveFeedback += TextControl_GiveFeedback;
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
				m_lastTranslationSelectionState = new SubstringDescriptor(TextControl);
				TextControl.PreviewKeyDown -= txtControl_PreviewKeyDown;
				TextControl.DragEnter -= TextControl_Drag;
				TextControl.DragOver -= TextControl_Drag;
				TextControl.DragDrop -= TextControl_DragDrop;
				TextControl.GiveFeedback -= TextControl_GiveFeedback;
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
		/// Handles hiding/showing the mnuShiftWordsRight and mnuShiftWordsLeft menus.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			mnuShiftWordsLeft.Visible = mnuShiftWordsRight.Visible = toolStripSeparatorShiftWords.Visible =
				(TextControl != null && EditingTranslation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the click event for mnuShiftWordsRight and mnuShiftWordsLeft.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleShiftWordsMenuClick(object sender, EventArgs e)
		{
			if (TextControl == null || !EditingTranslation)
				return;

			TextControl.MoveSelectedWord(sender == (m_fVernIsRtoL ? mnuShiftWordsLeft : mnuShiftWordsRight));
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

		private void HandleDisplayLanguageSelected(object sender, EventArgs e)
		{
			var clickedMenu = (ToolStripMenuItem)sender;
			if (clickedMenu.Checked)
				return;
			clickedMenu.Checked = true;
			foreach (ToolStripMenuItem subMenu in displayLanguageToolStripMenuItem.DropDownItems)
			{
				if (subMenu != clickedMenu)
					subMenu.Checked = false;
			}
			var localeId = (string)clickedMenu.Tag;
			SetLocalizer(localeId);
			Properties.Settings.Default.OverrideDisplayLanguage = localeId;
			if (txtFilterByPart.Text.Trim().Length > 0)
				ApplyFilter();
			else
				dataGridUns.Invalidate();
			LoadAnswersAndCommentsIfShowing(null, null);
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
		private void LoadTranslations(IProgressMessage splashScreen)
	    {
	        if (splashScreen != null)
	            splashScreen.Message = Properties.Resources.kstidSplashMsgLoadingQuestions;

			FileInfo finfoMasterQuestions = new FileInfo(m_masterQuestionsFilename);
			if (!finfoMasterQuestions.Exists)
			{
				MessageBox.Show("Required file missing: " + m_masterQuestionsFilename + ". Please re-run the Transcelerator installer to repair this problem.", Text);
				return;
			}

			string keyTermRulesFilename = Path.Combine(m_installDir, kKeyTermRulesFilename);

            FileInfo finfoKtRules = new FileInfo(keyTermRulesFilename);
            if (!finfoKtRules.Exists)
                MessageBox.Show("Expected file missing: " + keyTermRulesFilename + ". Please re-run the Transcelerator installer to repair this problem.", Text);

			string questionWordsFilename = Path.Combine(m_installDir, TxlCore.kQuestionWordsFilename);

			FileInfo finfoQuestionWords = new FileInfo(questionWordsFilename);
			if (!finfoQuestionWords.Exists)
				MessageBox.Show("Expected file missing: " + questionWordsFilename + ". Please re-run the Transcelerator installer to repair this problem.", Text);

			FileInfo finfoParsedQuestions = new FileInfo(m_parsedQuestionsFilename);

	        FileInfo finfoTxlDll = new FileInfo(Assembly.GetExecutingAssembly().Location);

            Exception e;
	        ParsedQuestions parsedQuestions;
	        if (finfoParsedQuestions.Exists &&
                finfoMasterQuestions.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc &&
				finfoTxlDll.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc &&
				(!finfoKtRules.Exists || finfoKtRules.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc) &&
				(!finfoQuestionWords.Exists || finfoQuestionWords.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc) &&
                m_fileAccessor.ModifiedTime(DataFileAccessor.DataFileId.QuestionCustomizations) < finfoParsedQuestions.LastWriteTimeUtc &&
                m_fileAccessor.ModifiedTime(DataFileAccessor.DataFileId.PhraseSubstitutions) < finfoParsedQuestions.LastWriteTimeUtc)
	        {
	            parsedQuestions = XmlSerializationHelper.DeserializeFromFile<ParsedQuestions>(m_parsedQuestionsFilename);
	        }
	        else
	        {
	            if (splashScreen != null)
	                splashScreen.Message = Properties.Resources.kstidSplashMsgParsingQuestions;

	            List<PhraseCustomization> customizations = null;
	            string phraseCustData = m_fileAccessor.Read(DataFileAccessor.DataFileId.QuestionCustomizations);
	            if (!string.IsNullOrEmpty(phraseCustData))
	            {
	                customizations = XmlSerializationHelper.DeserializeFromString<List<PhraseCustomization>>(phraseCustData, out e);
	                if (e != null)
	                    MessageBox.Show(e.ToString());
	            }

		        m_parser = new MasterQuestionParser(m_masterQuestionsFilename, GetQuestionWords(),
                    m_getKeyTerms(), GetKeyTermRules(keyTermRulesFilename), customizations, PhraseSubstitutions);
	            parsedQuestions = m_parser.Result;
		        Directory.CreateDirectory(Path.GetDirectoryName(m_parsedQuestionsFilename));
	            XmlSerializationHelper.SerializeToFile(m_parsedQuestionsFilename, parsedQuestions);
	        }

			var phrasePartManager = new PhrasePartManager(parsedQuestions.TranslatableParts, parsedQuestions.KeyTerms);
	        var qp = new QuestionProvider(parsedQuestions, phrasePartManager);
            m_helper = new PhraseTranslationHelper(qp);
		    m_helper.FileProxy = m_fileAccessor;
	        m_sectionInfo = qp.SectionInfo;
			m_availableBookIds = qp.AvailableBookIds;
		    string translationData = m_fileAccessor.Read(DataFileAccessor.DataFileId.Translations);
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
        /// Loads the phrase substitutions if not already loaded
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private List<Substitution> PhraseSubstitutions
	    {
	        get
	        {
		        if (m_cachedPhraseSubstitutions == null)
		        {
					m_cachedPhraseSubstitutions = ScrTextSerializationHelper.LoadOrCreateListFromString<Substitution>(
				        m_fileAccessor.Read(DataFileAccessor.DataFileId.PhraseSubstitutions), true);
		        }
				return m_cachedPhraseSubstitutions;
	        }
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads and initializes the key term rules
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private KeyTermRules GetKeyTermRules(string keyTermRulesFilename = null)
		{
			if (keyTermRulesFilename == null)
				keyTermRulesFilename = Path.Combine(m_installDir, kKeyTermRulesFilename);
			Exception e;

			KeyTermRules rules = XmlSerializationHelper.DeserializeFromFile<KeyTermRules>(keyTermRulesFilename, out e);
			if (e != null)
				MessageBox.Show(e.ToString(), Text);
			else
				rules.Initialize();
			return rules;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the built-in question words
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string[] GetQuestionWords(string questionWordsFilename = null)
		{
			if (questionWordsFilename == null)
			{
				questionWordsFilename = Path.Combine(m_installDir, TxlCore.kQuestionWordsFilename);
			}
			Exception e;

			QuestionWords questionWords = XmlSerializationHelper.DeserializeFromFile<QuestionWords>(questionWordsFilename, out e);
			if (e != null)
				MessageBox.Show(e.ToString(), Text);
			return questionWords.Items;
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
			Dictionary<KeyTerm, int> previousKeyTermEndOfRenderingOffsets = new Dictionary<KeyTerm, int>();
			foreach (KeyTerm keyTerm in m_helper[rowIndex].GetParts().OfType<KeyTerm>())//.Where(ktm => ktm.Renderings.Any()))
			{
				int ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm;
				previousKeyTermEndOfRenderingOffsets.TryGetValue(keyTerm, out ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm);
				TermRenderingCtrl ktRenderCtrl = new TermRenderingCtrl(keyTerm,
					ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm, m_selectKeyboard, LookupTerm);
				ktRenderCtrl.VernacularFont = m_vernFont;

				SubstringDescriptor sd = m_helper[rowIndex].FindTermRenderingInUse(ktRenderCtrl);
				if (sd == null)
				{
					// Didn't find any renderings for this term in the translation, so don't select anything
					previousKeyTermEndOfRenderingOffsets[keyTerm] = m_helper[rowIndex].Translation.Length;
				}
				else
				{
					previousKeyTermEndOfRenderingOffsets[keyTerm] = sd.EndOffset;
					ktRenderCtrl.SelectedRendering = m_helper[rowIndex].Translation.Substring(sd.Start, sd.Length);
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

		private void LookupTerm(IEnumerable<string> termIds)
		{
		    List<string> prioritizedTerms = new List<string>();
		    List<string> termsNotOccurringInCurrentRefRange = new List<string>();
		    int startRef = CurrentPhrase.StartRef;
		    int endRef = CurrentPhrase.EndRef;
            foreach (string termId in termIds)
            {
                var occurrences = m_getTermOccurrences(termId);
                if (occurrences.Any(o => o >= startRef && o <= endRef))
                    prioritizedTerms.Add(termId);
                else
                    termsNotOccurringInCurrentRefRange.Add(termId);
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
            if (dataGridUns.IsCurrentCellInEditMode)
                return;

			// While we process the given reference we might get additional synch events, the
			// most recent of which we store in m_queuedReference. If we're done
			// and we have a new reference in m_queuedReference we process that one, etc.
			for (; reference != null; reference = m_queuedReference)
			{
				m_queuedReference = null;
				m_fProcessingSyncMessage = true;

			    try
			    {
			        if (reference.Valid)
			        {
                        TranslatablePhrase phrase = (dataGridUns.CurrentRow == null) ? null :
                            m_helper.Phrases.ElementAt(dataGridUns.CurrentRow.Index);

			            bool phraseAppliesToRef = phrase != null && phrase.AppliesToReference(reference);
                        
			            if (!phraseAppliesToRef || !phrase.IsDetail)
			                GoToReference(reference, phraseAppliesToRef && !phrase.IsDetail);
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
		/// Goes to the first row in the data grid corresponding to the given reference,
		/// preferably for a detail question.
		/// </summary>
        /// <param name="reference">The reference to check against</param>
        /// <param name="detailOnly">If <c>true</c> only move to a new row if a matching detail
        /// question is found (i.e., disregard overview questions)</param>
        /// ------------------------------------------------------------------------------------
        private void GoToReference(BCVRef reference, bool detailOnly)
		{
		    int iFound = -1;
		    int iRow = 0;
		    foreach (TranslatablePhrase phrase in m_helper.Phrases)
		    {
                if (phrase.AppliesToReference(reference))
		        {
		            if (phrase.IsDetail)
		            {
		                iFound = iRow;
		                break;
		            }
                    if (!detailOnly && iFound < 0)
		                iFound = iRow; // But don't break yet because we might find a detail question, which is preferred.
		        }
		        iRow++;
		    }
            if (iFound >= 0)
            {
                dataGridUns.CurrentCell = dataGridUns.Rows[iFound].Cells[dataGridUns.CurrentCell == null ? 2 :
                    dataGridUns.CurrentCell.ColumnIndex];
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
			var question = m_helper[rowIndex].QuestionInfo;
			PopulateAnswerOrCommentLabel(question, question?.Answers, LocalizableStringType.Answer,
				m_lblAnswerLabel, m_lblAnswers, Properties.Resources.kstidAnswersLabel);
			PopulateAnswerOrCommentLabel(question, question?.Notes, LocalizableStringType.Note,
				m_lblCommentLabel, m_lblComments, Properties.Resources.kstidCommentsLabel);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Populates the answer or comment label.
		/// </summary>
		/// <param name="question">The question whose answers or comments are to be used.</param>
		/// <param name="details">The answers/comments data.</param>
		/// <param name="type">The type (Answer/Notes) of stuff (used for getting localization)</param>
		/// <param name="label">The label that has the "Answer:" or "Comment:" label.</param>
		/// <param name="contents">The label that is to be populated with the actual answer(s)
		/// or comment(s).</param>
		/// <param name="sLabelMultiple">The text to use for <see cref="label"/> if there are
		/// multiple answers/comments.</param>
		/// ------------------------------------------------------------------------------------
		private void PopulateAnswerOrCommentLabel(Question question, string[] details, LocalizableStringType type,
			Label label, Label contents, string sLabelMultiple)
		{
			label.Visible = contents.Visible = details?.Length > 0;
			if (label.Visible)
			{
				string[] loc;
				if (m_dataLocalizer == null)
					loc = details;
				else
				{
					loc = new string[details.Length];
					for (int i = 0; i < details.Length; i++)
						loc[i] = m_dataLocalizer.GetLocalizedDataString(new UIAnswerOrNoteDataString(question, type, i), out string notUsed);
				}
				label.Show(); // Is this needed?
				label.Text = details.Length == 1 ? (string)label.Tag : sLabelMultiple;
				contents.Text = loc.ToString(Environment.NewLine + "\t");
			}
		}
#endregion

#region Drag-drop stuff
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Catch a left-mouse down in selected text when translation is being edited to
		/// prevent clearing selection so user can start drag-drop operation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == (int)Msg.WM_LBUTTONDOWN &&
				IsPointInSelectedTextInTranslationEditingControl(MousePosition))
			{
				// We support both move and copy, but we don't need to handle the result because
				// we really only support move if the text is moved to a new location within the
				// TextControl, in which case the "destination" code handles the entire operation,
				// including removing the existing selected text.
				TextControl.DoDragDrop(TextControl.SelectedText, DragDropEffects.Copy | DragDropEffects.Move);
				return true;
			}
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the given point is contained within the selected region of text
		/// in the text control that is currently being used to edit the Translation.
		/// </summary>
		/// <param name="position">Point (typically a mouse positon) relative to screen</param>
		/// ------------------------------------------------------------------------------------
		private bool IsPointInSelectedTextInTranslationEditingControl(Point position)
		{
			if (!EditingTranslation || TextControl.SelectedText.Length == 0)
				return false;

			var topLeft = TextControl.PointToScreen(new Point(TextControl.ClientRectangle.Top, TextControl.ClientRectangle.Left));
			var bottom = TextControl.PointToScreen(new Point(0, TextControl.ClientRectangle.Bottom)).Y;
			if (position.Y >= topLeft.Y && position.Y <= bottom)
			{
				var dxStartOfSelection = TextControl.PointToScreen(new Point(
					(TextControl.SelectionStart == 0 ? TextControl.ClientRectangle.Left :
					TextControl.GetPositionFromCharIndex(TextControl.SelectionStart - 1).X), 0)).X;
				var limSelection = TextControl.SelectionStart + TextControl.SelectionLength;
				var dxEndOfSelection = TextControl.PointToScreen(new Point(TextControl.Location.X +
					(limSelection == TextControl.Text.Length ? TextControl.ClientSize.Width :
					TextControl.GetPositionFromCharIndex(limSelection).X), 0)).X;

				if (position.X >= dxStartOfSelection && position.X <= dxEndOfSelection)
					return true;
			}
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles DragOver and DragEnter events
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void TextControl_Drag(object sender, DragEventArgs e)
		{
			if ((e.AllowedEffect & (DragDropEffects.Move)) > 0 &&
				(e.Data.GetDataPresent(DataFormats.StringFormat, false)))
			{
				// For now, we can safely assume that any "string" that
				// allows move is originating in the same TextControl because any
				// text from an outside app will not come in as a StringFormat
				// object and no other control in Transcelerator supports
				// moving string data.
				e.Effect = DragDropEffects.Move;
			}
			else if ((e.AllowedEffect & (DragDropEffects.Copy)) > 0 &&
				(e.Data.GetDataPresent(DataFormats.StringFormat, false) ||
				e.Data.GetDataPresent(DataFormats.UnicodeText, false) ||
				e.Data.GetDataPresent(DataFormats.Text, false)))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles GiveFeedback event to show the user where the text will be dropped.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void TextControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (!m_fEnableDragDrop && e.Effect == DragDropEffects.Move)
			{
				e.UseDefaultCursors = false;
			}
			//var ichInsert = TextControl.GetCharIndexFromPosition(TextControl.PointToClient(MousePosition));

			//if (e.Effect == DragDropEffects.Move && TextControl.SelectionLength > 0 &&
			//    ichInsert >= TextControl.SelectionStart &&
			//    ichInsert <= TextControl.SelectionStart + TextControl.SelectionLength)
			//{
			//    // Dropping selected text onto itself just removes the selection.
			//    e.UseDefaultCursors = false; // TODO: This doesn't do anything!
			//    return;
			//}
			//e.UseDefaultCursors = true;
			//// TODO: Need to return early (just do default behavior) if not over the TextControl
			//// TODO: Draw special insertion point to show where dropped text would go.
			//Debug.WriteLine("Insert position: " + ichInsert);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles DragDrop event to complete the copy or move.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void TextControl_DragDrop(object sender, DragEventArgs e)
		{
			var ichInsert = TextControl.GetCharIndexFromPosition(TextControl.PointToClient(new Point(e.X, e.Y)));

			if (!m_fEnableDragDrop)
			{
				TextControl.SelectionStart = ichInsert;
				TextControl.SelectionLength = 0;
				return;
			}

			bool fMove = e.Effect == DragDropEffects.Move;

			if ((fMove || e.Effect == DragDropEffects.Copy) &&
				(e.Data.GetDataPresent(DataFormats.StringFormat, false) ||
				e.Data.GetDataPresent(DataFormats.UnicodeText, false) ||
				e.Data.GetDataPresent(DataFormats.Text, false)))
			{
				string text = ((string)e.Data.GetData(DataFormats.StringFormat));

				if (fMove && TextControl.SelectionLength > 0 &&
					ichInsert >= TextControl.SelectionStart &&
					ichInsert <= TextControl.SelectionStart + TextControl.SelectionLength)
				{
					// Don't try to move selected text onto itself. Instead just remove selection.
					// This allows a simple click to behave properly.
					TextControl.SelectionStart = ichInsert;
					TextControl.SelectionLength = 0;
					return;
				}

				if (text.Length > 0)
				{
					var textBefore = TextControl.Text.Substring(0, ichInsert);
					var textAfter = TextControl.Text.Substring(ichInsert);
					var removeStart = TextControl.SelectionStart;
					var removeLen = TextControl.SelectionLength;
					if (ichInsert <= removeStart)
						removeStart += text.Length;
					else // post-adjust the insert location 
						ichInsert -= text.Length;
					TextControl.Text = textBefore + text + textAfter;
					if (removeLen > 0)
					{
						// We need to handle removal of originally selected text because
						// the code where the drag-drop originates assumes we will (it
						// treats any copy/move as a copy because we don't want dragging
						// from TXL to Word, for example, to result in a move.  
						TextControl.Text = TextControl.Text.Remove(removeStart, removeLen);
						// Now select the moved text
						TextControl.SelectionStart = ichInsert;
						TextControl.SelectionLength = text.Length;
					}
				}
			}
		}
		#endregion

		private void mnuProduceScriptureForgeFiles_CheckedChanged(object sender, EventArgs e)
		{
			mnuProduceScriptureForgeFiles.Image = mnuProduceScriptureForgeFiles.Checked ?
				Resources.sf_logo_medium___selected : Resources.sf_logo_medium;
			Properties.Settings.Default.ProduceScriptureForgeFiles = mnuProduceScriptureForgeFiles.Checked;
		}
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
		public int Start { get; set; }
		public int Length { get; set; }

		public int EndOffset
		{
			get { return Start + Length; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(int start, int length)
		{
			Start = start;
			Length = length;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class based on
		/// the existing text selection in a text box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(TextBox textBoxCtrl)
		{
			Start = textBoxCtrl.SelectionStart;
			Length = textBoxCtrl.SelectionLength;
		}
	}
#endregion
}