// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.   
// <copyright from='2011' to='2022 company='SIL International'>
//		Copyright (c) 2022, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: UNSQuestionsDialog.cs
// ---------------------------------------------------------------------------------------------
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using SIL.Utils;
using SIL.Windows.Forms.FileDialogExtender;
using SIL.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using Paratext.PluginInterfaces;
using SIL.Reporting;
using SIL.Windows.Forms;
using SIL.Windows.Forms.Extensions;
using static System.Char;
using static System.String;
using static SIL.WritingSystems.IetfLanguageTag;
using Application = System.Windows.Forms.Application;
using DateTime = System.DateTime;
using File = System.IO.File;
using FileInfo = System.IO.FileInfo;
using Process = System.Diagnostics.Process;
using Resources = SIL.Transcelerator.Properties.Resources;
using Task = System.Threading.Tasks.Task;

namespace SIL.Transcelerator
{
	#region UNSQuestionsDialog class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// UNSQuestionsDialog.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class UNSQuestionsDialog : ParentFormBase, IMessageFilter
	{
		#region Constants
		public const string kScriptureForgeProductName = "Scripture Forge";
		#endregion

		#region Member Data
		private readonly Func<IEnumerable<IBiblicalTerm>> m_getKeyTerms;
		// Function to get the vernacular font
		private Font m_vernFont;
		private readonly IPluginHost m_host;
		private readonly IProject m_project;
		private readonly Action<bool> m_selectKeyboard;
		private LocalizationsFileAccessor m_dataLocalizer;
		private BiblicalTermLocalizer m_termLocalizer;
		private PhraseTranslationHelper m_helper;
		private readonly ParatextDataFileAccessor m_fileAccessor;
		private ParatextTermRenderingsRepo m_renderingsRepo;
		private readonly string m_installDir;
		private readonly IVersification m_masterVersification;
		private IVerseRef m_startRef;
		private IVerseRef m_endRef;
		private readonly Action<BCVRef> m_sendReference;
		private TransceleratorSections m_sectionInfo;
		private int[] m_availableBookIds;
		private readonly string m_masterQuestionsFilename;
		private static readonly string s_programDataFolder;
		private readonly string m_parsedQuestionsFilename;
		private DateTime m_lastSaveTime;
		private MasterQuestionParser m_parser;
		/// <summary>Use PhraseSubstitutions property to ensure non-null cache</summary>
		private List<Substitution> m_cachedPhraseSubstitutions;
		private bool m_fProcessingSyncMessage;
		private bool m_fSendingSyncMessage;
		private BCVRef m_queuedReference;
		private int m_longTaskStackCount = 0;
		private int m_lastRowEntered = -1;
		private TranslatablePhrase m_currentPhrase = null;
		private int m_iCurrentColumn = -1;
		private int m_normalRowHeight = -1;
		private int m_lastTranslationSet = -1;
		private bool m_translationEditWasCommitted;
		private bool m_saving = false;
		private bool m_postponeRefresh;
		private int m_maximumHeightOfKeyTermsPane;
		private bool m_loadingBiblicalTermsPane = false;
		private bool m_preventReEntrantCommitEditDuringSave = false;
		private bool m_forceSaveOnCloseModal = false;
		private DataFileAccessor.DataFileId m_lockToHold = DataFileAccessor.DataFileId.Translations;
		#endregion

		#region Delegates
		public Func<IEnumerable<int>> GetAvailableBooks { private get; set; }
		#endregion

		#region Properties
		private DataGridViewTextBoxEditingControl TextControl => dataGridUns.EditingControl as DataGridViewTextBoxEditingControl;
		private bool RefreshNeeded { get; set; }
		private int MaximumHeightOfKeyTermsPane
		{
			get => m_maximumHeightOfKeyTermsPane;
			set => m_maximumHeightOfKeyTermsPane = Math.Max(38, value);
		}

		private string ReadonlyAlert => m_fileAccessor.IsReadonly ? LocalizationManager.GetString("General.ReadonlyWindowAlert", " - Readonly") : null;

		private SortedDictionary<string, string> AvailableLocales { get; } = new SortedDictionary<string, string>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to postpone refreshing the data grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool PostponeRefresh
		{
			get => m_postponeRefresh;
			set
			{
				if (value && !m_postponeRefresh)
					RefreshNeeded = false;
				m_postponeRefresh = value;
				if (!value && RefreshNeeded)
					dataGridUns.Refresh();
			}
		}

		private bool DragAndDropDisabled => !m_host.UserSettings.IsDragAndDropEnabled;

		private PhraseTranslationHelper.KeyTermFilterType CheckedKeyTermFilterType
		{
			get => (PhraseTranslationHelper.KeyTermFilterType)mnuFilterBiblicalTerms
				.DropDownItems.Cast<ToolStripMenuItem>().First(menu => menu.Checked).Tag;
			set
			{
				mnuFilterBiblicalTerms.DropDownItems.Cast<ToolStripMenuItem>().First(
					menu => (PhraseTranslationHelper.KeyTermFilterType)menu.Tag == value).Checked = true;
				ApplyFilter();
			}
		}

		private bool SaveNeeded
		{
			get => btnSave.Enabled;
			set
			{
				if (m_fileAccessor.IsReadonly)
					return;
				if (value && mnuAutoSave.Checked && DateTime.Now > m_lastSaveTime.AddSeconds(10))
					Save(true, false);
				else
				{
					if (value)
						m_fileAccessor.EnsureLockForTranslationData();
					mnuSave.Enabled = btnSave.Enabled = value;
				}
			}
		}

		private IEnumerable<int> AvailableBookIds
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
		private bool MatchWholeWords
		{
			get => mnuMatchWholeWords.Checked;
			set => mnuMatchWholeWords.Checked = value;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether toolbar is displayed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool ShowToolbar
		{
			get => mnuViewToolbar.Checked;
			set => mnuViewToolbar.Checked = value;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether to show a pane with the answers and comments
		/// on the questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool ShowAnswersAndComments
		{
			get => mnuViewAnswers.Checked;
			set => mnuViewAnswers.Checked = value;
		}

		private TranslatablePhrase CurrentPhrase
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
		private bool InTranslationCell => dataGridUns.CurrentCell != null &&
			dataGridUns.CurrentCell.ColumnIndex == m_colTranslation.Index;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether the grid is in edit mode in a cell in the
		/// Translation column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool EditingTranslation => InTranslationCell &&
			dataGridUns.IsCurrentCellInEditMode;

		private string HelpHome => TxlPlugin.GetHelpFile("Home");
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
		/// Constructs a new instance of the <see cref="UNSQuestionsDialog"/> class.
		/// </summary>
		/// <param name="host">The application hosting the Transcelerator plugin</param>
		/// <param name="project">The project that Transcelerator is working with</param>
		/// /// <param name="startRef">The starting Scripture reference to filter on (in the English
		/// versification)</param>
		/// <param name="endRef">The ending Scripture reference to filter on (in the English
		/// versification)</param>
		/// <param name="selectKeyboard">The delegate to select vern/anal keyboard</param>
		/// <param name="sendReference">The callback to notify the host that Transcelerator is
		/// looking at a particular Scripture reference.</param>
		/// ------------------------------------------------------------------------------------
		public UNSQuestionsDialog(IPluginHost host, IProject project,
			IVerseRef startRef, IVerseRef endRef,
			Action<bool> selectKeyboard, Action<BCVRef> sendReference)
		{
			if (startRef != default && endRef != default && startRef.CompareTo(endRef) > 0)
				throw new ArgumentException("startRef must be before endRef");

			m_startRef = startRef;
			m_endRef = endRef;
			m_sendReference = sendReference;

			InitializeComponent();
			Icon = Resources.TXL_no_TXL;

			m_host = host;
			m_project = project;

			m_fileAccessor = new ParatextDataFileAccessor(m_project, HandleWriteLockReleased);

			m_getKeyTerms = () => m_host.GetBiblicalTermList(BiblicalTermListType.Major).Where(t => t.Occurrences.Any(o => o.BookNum < 67));

			m_selectKeyboard = m_fileAccessor.IsReadonly ? null : selectKeyboard;

			m_masterVersification = m_host.GetStandardVersification(StandardScrVersType.English);

			m_installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Empty;
            
			m_masterQuestionsFilename = Path.Combine(m_installDir, TxlCore.kQuestionsFilename);
			m_parsedQuestionsFilename = Path.Combine(s_programDataFolder, m_project.ShortName, TxlCore.kQuestionsFilename);

			var preferredUiLocale = LocalizationManager.UILanguageId;

			if (!IsNullOrEmpty(Properties.Settings.Default.OverrideDisplayLanguage))
			{
				preferredUiLocale = Properties.Settings.Default.OverrideDisplayLanguage;
				if (GetGeneralCode(preferredUiLocale) != GetGeneralCode(LocalizationManager.UILanguageId))
				{
					// Unless/until we ship UI strings for different variants of the same language,
					// there is no need to try to tell the LocalizationManager to load a different
					// variant. It's already smart enough to fallback to another variant of the
					// language anyway.
					LocalizationManager.SetUILanguage(preferredUiLocale, true);
				}
			}

			AddAvailableLocalizationsToMenu();
			SetLocalizer(preferredUiLocale);

			ClearBiblicalTermsPane();

			HelpButton = browseTopicsToolStripMenuItem.Enabled = !IsNullOrEmpty(HelpHome);

			mnuShowAllPhrases.Tag = PhraseTranslationHelper.KeyTermFilterType.All;
			mnuShowPhrasesWithKtRenderings.Tag = PhraseTranslationHelper.KeyTermFilterType.WithRenderings;
			mnuShowPhrasesWithMissingKtRenderings.Tag = PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings;

			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
			HandleStringsLocalized();

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
			SetScrForgeMenuIcon();
			mnuProduceScriptureForgeFiles.CheckedChanged += mnuProduceScriptureForgeFiles_CheckedChanged;
			mnuAutoSave.Checked = Properties.Settings.Default.AutoSave;
			mnuViewEditQuestionColumn.Checked = m_colEditQuestion.Visible = Properties.Settings.Default.ShowEditColumn;

			Margin = new Padding(Margin.Left, toolStrip1.Height, Margin.Right, Margin.Bottom);

#if DEBUG
			generateOutputForArloToolStripMenuItem.Visible = true;
			mnuLoadTranslationsFromTextFile.Visible = true;
#endif

			if (m_project.Language.Id == null)
				mnuGenerate.Enabled = false;

			TermRenderingCtrl.AppName = m_host.ApplicationName;
		}

		private Task HandleWriteLockReleased(DataFileAccessor.DataFileId fileType)
		{
			if (InvokeRequired)
				return new Task(() => Invoke(new Action(() => HandleWriteLockReleased(fileType))));

			if (fileType != m_lockToHold)
				return null;

			if (IsShowingModalForm)
			{
				m_forceSaveOnCloseModal = true;
				return new Task(WaitForSave);
			}

			if (SaveNeeded || dataGridUns.IsCurrentCellInEditMode)
				Save(true, false);

			return null;
		}

		void WaitForSave()
		{
			while (m_forceSaveOnCloseModal)
				Thread.Sleep(100);
		}

		void HandleDataGridUnsPopulatedFirstTime()
		{
			if (!dataGridUns.IsHandleCreated)
			{
				dataGridUns.HandleCreated += (sender, args) =>
				{
					var bcvRef = new BCVRef(m_host.ActiveWindowState.VerseRef.BBBCCCVVV);
					if (InvokeRequired)
						Invoke(new Action(() => { ProcessCurrentVerseRefChange(bcvRef); }));
					else
						ProcessCurrentVerseRefChange(bcvRef);
				};
			}
			else if (dataGridUns.RowCount > 0)
				ProcessCurrentVerseRefChange(new BCVRef(m_host.ActiveWindowState.VerseRef.BBBCCCVVV));
		}

		public void Show(TxlSplashScreen splashScreen)
		{
			ResetRowCount();

			// Now apply settings that have filtering or other side-effects
			CheckedKeyTermFilterType = (PhraseTranslationHelper.KeyTermFilterType)Properties.Settings.Default.KeyTermFilterType;
			btnSendScrReferences.Checked = Properties.Settings.Default.SendScrRefs;

			InitFromHostProject();

			m_project.ProjectDataChanged += OnProjectDataChanged;

			splashScreen?.Close();

			OnModalFormShown += delegate { m_biblicalTermsPane.Enabled = false; };
			OnModalFormClosed += OnOnModalFormClosed;

			Show();
		}

		private void ResetRowCount()
		{
			dataGridUns.RowCount = m_helper.Phrases.Count();
			m_lastRowEntered = -1;
		}

		private void OnOnModalFormClosed()
		{
			m_biblicalTermsPane.Enabled = true;
			if (m_forceSaveOnCloseModal)
			{
				Save(dataGridUns.IsCurrentCellInEditMode, false);
				m_forceSaveOnCloseModal = false;
			}
		}

		private void OnProjectDataChanged(IProject sender, ProjectDataChangeType details)
		{
			// REVIEW: Does this handle global changes from Send/Receive properly?
			InitFromHostProject(details);
			if (details == ProjectDataChangeType.WholeProject)
			{
				// REVIEW: If sort order changes and we are sorted on the Translation column, we should re-sort.
				// Does this code cover that?

				// This seems ideal, but we can't do this because a settings change does not request release of
				// Transcelerator's write lock(s); therefore, it does not force a save to happen before making
				// this change.
				// Debug.Assert(!SaveNeeded);

				Reload(false);
			}
		}

		private void InitFromHostProject(ProjectDataChangeType change = ProjectDataChangeType.WholeProject)
		{
			switch (change)
			{
				case ProjectDataChangeType.SettingVernacularKeyboard:
					if (InTranslationCell)
						m_selectKeyboard?.Invoke(true);
					return;
				case ProjectDataChangeType.DataBiblicalTerms:
				case ProjectDataChangeType.WholeProject:
					break;
				case ProjectDataChangeType.DataBiblicalTermsRenderings:
					var rowIndex = dataGridUns.CurrentCellAddress.Y;
					foreach (Control ctrl in m_biblicalTermsPane.Controls)
					{
						if (ctrl is TermRenderingCtrl ktRenderCtrl)
						{
							ktRenderCtrl.Reload();

							var sd = m_helper[rowIndex].FindTermRenderingInUse(ktRenderCtrl);
							if (sd != null)
								ktRenderCtrl.SelectedRendering = m_helper[rowIndex].Translation.Substring(sd.Start, sd.Length);
						}
					}
					return;
				default:
					return;
			}
			
			var fontInfo = m_project.Language.Font;
			m_vernFont?.Dispose();
			m_vernFont = new Font(fontInfo.FontFamily, fontInfo.Size);
			DataGridViewCellStyle translationCellStyle = new DataGridViewCellStyle();
			translationCellStyle.Font = m_vernFont;
			m_colTranslation.DefaultCellStyle = translationCellStyle;

			if (m_project.Language.IsRtoL)
				m_colTranslation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			dataGridUns.RowTemplate.MinimumHeight = dataGridUns.RowTemplate.Height = m_normalRowHeight =
				(int)Math.Ceiling(m_vernFont.Height * CreateGraphics().DpiY / 72) + 2;

			if (m_fileAccessor.IsReadonly)
			{
				SetWindowText();
				dataGridUns.ReadOnly = true;
				mnuAddQuestion.Enabled = false;
				mnuAutoSave.Checked = false;
				mnuAutoSave.Enabled = false;
				mnuCut.Enabled = false;
				mnuEditQuestion.Enabled = false;
				mnuExcludeQuestion.Enabled = false;
				mnuIncludeQuestion.Enabled = false;
				mnuPaste.Enabled = false;
				mnuShiftWordsLeft.Enabled = false;
				mnuShiftWordsRight.Enabled = false;
				mnuSave.Enabled = false;
				btnSave.Enabled = false;
				mnuLoadTranslationsFromTextFile.Enabled = false;
				mnuProduceScriptureForgeFiles.Enabled = false;
			}
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;
			SetControlTagsToFormatStringsAndFormatMenus();
			SetWindowText();
			UpdateCountsAndFilterStatus();
		}

		private void SetWindowText()
		{
			if (Tag == null)
				Tag = Text;
			Text = Format((string)Tag, m_project.ShortName) + ReadonlyAlert;
		}

		private void SetControlTagsToFormatStringsAndFormatMenus()
		{
			m_lblAnswerLabel.Tag = m_lblAnswerLabel.Text.Trim();
			m_lblCommentLabel.Tag = m_lblCommentLabel.Text.Trim();
			lblFilterIndicator.Tag = lblFilterIndicator.Text;
			lblRemainingWork.Tag = lblRemainingWork.Text;

			mnuProduceScriptureForgeFiles.Text = Format(mnuProduceScriptureForgeFiles.Text, kScriptureForgeProductName);
		}

		private static string GetLanguageNameWithDetails(string code)
		{
			try
			{
				var ci = CultureInfo.GetCultureInfo(code); // this may throw or produce worthless empty object
				if (!ci.EnglishName.StartsWith("Unknown Language"))	// Windows .Net behavior
					return ci.NativeName;
			}
			catch (Exception e)
			{
				Logger.WriteError(e);
			}

			return GetNativeLanguageNameWithEnglishSubtitle(code);
		}

		private void AddAvailableLocalizationsToMenu()
		{
			var locales = LocalizationsFileAccessor
				.GetAvailableLocales(m_installDir).Union(new[] { "en-US" }).ToList();
			var languagesNeedingDistinctionByLocale = locales
				.GroupBy(GetGeneralCode).Where(g => g.Count() > 1).Select(g => g.Key);

			mnuDisplayLanguage.InitializeWithAvailableUILocales(HandleDisplayLanguageSelected,
				TxlPlugin.PrimaryLocalizationManager, TxlPlugin.LocIncompleteViewModel,
				ShowMoreUiLanguagesDlg,
				locales.Where(l => languagesNeedingDistinctionByLocale.Contains(
					GetGeneralCode(l))).ToDictionary(GetLanguageNameWithDetails, l => l));
		}

		private void SetLocalizer(string preferredUiLocale)
		{
			m_dataLocalizer = GetDataLocalizer(preferredUiLocale);

			m_termLocalizer = m_dataLocalizer == null ||
				m_dataLocalizer.Locale == "en" || m_dataLocalizer.Locale == "en-GB" ? null :
				new BiblicalTermLocalizer(m_dataLocalizer.Locale, m_host.GetBiblicalTermList(BiblicalTermListType.Major));
		}

		private LocalizationsFileAccessor GetDataLocalizer(string localeId)
		{
			if (localeId == "en" || localeId == "en-US" || IsNullOrWhiteSpace(localeId))
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
			// REVIEW: XP is no longer supported. Is this code needed?
            // On Windows XP, TXL comes up underneath Paratext. See if this fixes it:
            TopMost = true;
            TopMost = false;
			Application.AddMessageFilter(this);
        }
		

		public void RequestClose(CancelEventArgs cancelEventArgs)
		{
			OnClosing(cancelEventArgs);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnClosing(CancelEventArgs e)
		{
			if (IsShowingModalForm)
			{
				Activate();
				e.Cancel = true;
				return;
			}

			var caption = Format(LocalizationManager.GetString("MainWindow.SaveChangesMessageCaption", "{0} - Save changes?",
				"This is a message box caption. The parameter is the information from the main window title to identify the plugin and project."),
				Text);
			bool confirmedSave = false;
			if (dataGridUns.IsCurrentCellInEditMode)
			{
				if (dataGridUns.IsCurrentCellDirty)
				{
					switch (MessageBox.Show(this,
						Format(LocalizationManager.GetString("MainWindow.CommitChangesBeforeClosingMessage",
							"You are currently editing the translation for a question in {0}. Do you wish to save this change before closing?",
							"Param is a Scripture reference."),
							m_helper[dataGridUns.CurrentCellAddress.Y].Reference),
						caption,
						MessageBoxButtons.YesNoCancel))
					{
						case DialogResult.Yes:
							dataGridUns.EndEdit();
							confirmedSave = true;
							break;
						case DialogResult.No:
							dataGridUns.CancelEdit();
							break;
						case DialogResult.Cancel:
							e.Cancel = true;
							base.OnClosing(e);
							return;
					}
				}
				else
					dataGridUns.CancelEdit();
			}

			if (SaveNeeded)
			{
				if (mnuAutoSave.Checked || confirmedSave)
				{
					Save(true, false);
					return;
				}

				switch (MessageBox.Show(this,
					LocalizationManager.GetString("MainWindow.SaveChangesBeforeClosingMessage",
						"You have made changes. Do you wish to save before closing?"),
					caption,
					MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						Save(true, false);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						base.OnClosing(e);
						return;
				}
			}

			Properties.Settings.Default.Save();

			if (Properties.Settings.Default.ProduceScriptureForgeFiles)
				ProduceScriptureForgeFiles();
			Application.RemoveMessageFilter(this);

			base.OnClosing(e);
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
				TrySetClipboardData(TextControl.SelectedText);
				TextControl.SelectedText = Empty;
            }
            else
            {
                CutToClipboard();
            }
		}

        private void mnuProduceScriptureForgeFiles_CheckedChanged(object sender, EventArgs e)
        {
	        SetScrForgeMenuIcon();
	        Properties.Settings.Default.ProduceScriptureForgeFiles = mnuProduceScriptureForgeFiles.Checked;

	        if (mnuProduceScriptureForgeFiles.Checked && mnuProduceScriptureForgeFiles.Tag == null)
	        {
		        mnuProduceScriptureForgeFiles.Tag = "shown";

				ShowModalChild(new ScriptureForgeInfoDlg(m_host.ApplicationName));
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
                    TrySetClipboardData(text);
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
                var text = TryGetClipboardText();
                if (text != null)
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

            CopyToClipboard(() =>
			{
	            // If copy succeeds, then make it a "cut" by clearing the selected cell
	            dataGridUns.SelectedCells[0].Value = Empty;
	            m_helper[dataGridUns.CurrentCell.RowIndex].HasUserTranslation = false;
	            SaveNeeded = true;
	            dataGridUns.InvalidateRow(dataGridUns.CurrentCell.RowIndex);
			});
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Puts the formatted values that represent the contents of the selected cells onto the
        /// <see cref="T:System.Windows.Forms.Clipboard"/>.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void CopyToClipboard(Action cutAction = null)
        {
            var dataObj = dataGridUns.GetClipboardContent();
			if (dataObj != null)
				TrySetClipboardData(dataObj, cutAction);
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

            var clipboardText = TryGetClipboardText();
			if (clipboardText == null)
				return;
            
			dataGridUns.CurrentCell.Value = clipboardText;
            SaveNeeded = true;
        }
		
		private void TrySetClipboardData(string data) => TrySetClipboardData(new DataObject(data));

		private void TrySetClipboardData(DataObject data, Action cutAction = null)
		{
			bool copySucceeded = false;
			try
			{
				Clipboard.SetDataObject(data, true, 2, 200);
				copySucceeded = true;
			}
			catch (Exception e)
			{
				Analytics.Track("ClipboardSetDataObjectException", new Dictionary<string, string>
					{{"Message", e.Message}});
				ShowModalChild(new MessageBoxForm(e.Message, TxlCore.kPluginName,
					MessageBoxButtons.RetryCancel), form =>
				{
					if (form.DialogResult == DialogResult.Retry)
						TrySetClipboardData(data, cutAction);
				});
			}
			if (copySucceeded)
				cutAction?.Invoke();
		}

		private static string TryGetClipboardText(bool allowNewlines = false)
		{
			string text;
			try
			{
				text = Clipboard.GetText();
			}
			catch (Exception e)
			{
				Analytics.Track("ClipboardGetTextException", new Dictionary<string, string>
					{{"Message", e.Message}});
				return null;
			}

			return !IsNullOrWhiteSpace(text) && (allowNewlines || !text.Contains('\n')) ? text : null;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Navigates to the next untranslated question in the grid. Beeps if all currently
        /// filtered questions have been translated
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void nextUntranslatedQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (dataGridUns.CurrentRow != null)
			{
				for (int iRow = dataGridUns.CurrentRow.Index + 1; iRow < m_helper.FilteredPhraseCount; iRow++)
				{
					if (!m_helper[iRow].HasUserTranslation)
					{
						SelectTranslationCellIn(iRow);
						return;
					}
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
			if (dataGridUns.CurrentRow != null)
			{
				for (int iRow = dataGridUns.CurrentRow.Index - 1; iRow >= 0; iRow--)
				{
					if (!m_helper[iRow].HasUserTranslation)
					{
						SelectTranslationCellIn(iRow);
						return;
					}
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
			{
				if (InvokeRequired)
					Invoke(new Action(() => { dataGridUns.Refresh(); }));
				else
					dataGridUns.Refresh();
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			m_selectKeyboard?.Invoke(InTranslationCell);
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			m_selectKeyboard?.Invoke(false);
		}

		private void dataGridUns_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colTranslation.Index)
			{
				m_selectKeyboard?.Invoke(true);
				if (dataGridUns.IsCurrentCellInEditMode)
					m_translationEditWasCommitted = false;
			}
			else if (e.RowIndex != m_lastTranslationSet)
				m_lastTranslationSet = -1;
		}

		private void dataGridUns_CellLeave(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colTranslation.Index)
				m_selectKeyboard?.Invoke(false);
		}

		private void mnuViewDebugInfo_CheckedChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			if (!item.Checked)
				dataGridUns.Columns.Remove(m_colDebugInfo);
			else
				dataGridUns.Columns.Add(m_colDebugInfo);
		}

		private void mnuViewEditQuestionColumn_CheckedChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			Properties.Settings.Default.ShowEditColumn = m_colEditQuestion.Visible = item.Checked;
		}

		private void mnuViewAnswersColumn_CheckedChanged(object sender, EventArgs e)
		{
			m_pnlAnswersAndComments.Visible = ShowAnswersAndComments;
		}

		private void dataGridUns_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			var tp = m_helper[e.RowIndex];
			if (e.ColumnIndex == m_colReference.Index)
				e.Value = tp.Reference;
			else if (e.ColumnIndex == m_colEnglish.Index)
				e.Value = m_dataLocalizer == null ? tp.PhraseInUse : m_dataLocalizer.GetLocalizedString(tp.ToUIDataString());
			else if (e.ColumnIndex == m_colEditQuestion.Index)
			{
				if (tp.ModifiedPhrase != null || tp.IsUserAdded)
					e.Value = Resources.iconfinder_edit_3855617___user_added;
				else if (tp.IsExcluded)
					e.Value = Resources.iconfinder_edit_3855617___excluded;
				else if (tp.AlternateForms != null)
					e.Value = Resources.iconfinder_edit_3855617___with_alternatives;
				else
					e.Value = Resources.iconfinder_edit_3855617;
			}
			else if (e.ColumnIndex == m_colTranslation.Index)
				e.Value = tp.Translation;
			else if (e.ColumnIndex == m_colUserTranslated.Index)
				e.Value = tp.HasUserTranslation;
			else if (e.ColumnIndex == m_colDebugInfo.Index)
				e.Value = tp.DebugInfo;
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
				m_translationEditWasCommitted = true;
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
			if (e.RowIndex < 0 || m_fileAccessor.IsReadonly)
				return;
			if (e.ColumnIndex == m_colTranslation.Index)
				dataGridUns.BeginEdit(true);
			if (e.ColumnIndex == m_colEditQuestion.Index)
			{
				var tp = m_helper[e.RowIndex];
				if (tp.IsExcluded)
					IncludeOrExcludeQuestion(false);
				else
					mnuEditQuestion_Click(sender, e);
			}
		}

		private void dataGridUns_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colUserTranslated.Index && e.RowIndex != m_lastTranslationSet &&
				m_helper[e.RowIndex].Translation.Any(IsLetter) && !m_fileAccessor.IsReadonly && !IsShowingModalForm)
			{
				if (m_helper[e.RowIndex].HasUserTranslation)
				{
					var otherIdenticalQuestions = m_helper[e.RowIndex].GetOtherIdenticalPhrasesWithSameTranslation();
					if (otherIdenticalQuestions.Any())
					{
						var msg = (otherIdenticalQuestions.Count == 1) ?
							LocalizationManager.GetString("MainWindow.ClearAllMatchingTranslations.Single",
								"There is another identical question that has this same translation. Clearing the translation for this question will also clear the translation for the other question.") :
							LocalizationManager.GetString("MainWindow.ClearAllMatchingTranslations.Multiple",
								"There are other identical questions that have this same translation. Clearing the translation for this question will also clear the translations for the other questions.");
						ShowModalChild(new MessageBoxForm(msg, TxlCore.kPluginName, MessageBoxButtons.OKCancel), form =>
						{
							if (form.DialogResult == DialogResult.OK)
							{
								foreach (var tp in otherIdenticalQuestions)
								{
									tp.HasUserTranslation = false;
									int index = m_helper.FindPhrase(tp.PhraseKey);
									if (index >= 0 && index < dataGridUns.RowCount)
										dataGridUns.InvalidateRow(index);
								}

								ToggleHasUserTranslation(e.RowIndex);
							}
						});
						return;
					}
				}

				ToggleHasUserTranslation(e.RowIndex);
			}
		}

		private void ToggleHasUserTranslation(int row)
		{
			m_helper[row].HasUserTranslation = !m_helper[row].HasUserTranslation;
			SaveNeeded = true;
			dataGridUns.InvalidateRow(row);
		}

		private void dataGridUns_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			int iClickedCol = e.ColumnIndex;
			var clickedColumn = dataGridUns.Columns[iClickedCol];
			if (clickedColumn.SortMode == DataGridViewColumnSortMode.NotSortable)
				return;

			// ENHANCE: Remember which cell we were in and try to find the same place in the newly sorted list.
			// We might only care about this in the case where the cell was being edited.

			// Not likely that a user would initiate a new sort while editing, but if they do (and if the
			// cell value was actually changed, let's just save it.
			if (dataGridUns.IsCurrentCellInEditMode)
				dataGridUns.EndEdit();

			// We want to sort it ascending unless it already was ascending.
			bool sortAscending = clickedColumn.HeaderCell.SortGlyphDirection != SortOrder.Ascending;
			if (!sortAscending)
			{
				clickedColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending;
			}
			else
			{
				for (int i = 0; i < dataGridUns.Columns.Count; i++)
				{
					dataGridUns.Columns[i].HeaderCell.SortGlyphDirection = (i == iClickedCol) ?
						SortOrder.Ascending : SortOrder.None;
				}
			}

			var refreshAction = new Action(() =>
			{
				UseWaitCursor = false;
				dataGridUns.Refresh();
				foreach (Control control in Controls)
					control.Enabled = true;
			});

			foreach (Control control in Controls)
				control.Enabled = false;

			UseWaitCursor = true;
			Task.Run(() => { SortByColumn(iClickedCol, sortAscending);}).ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					// REVIEW: If an exception gets thrown and we don't report it explicitly, what happens?
					foreach (var exception in t.Exception.InnerExceptions)
						ErrorReport.ReportFatalException(exception);
					// ???	Close();
				}

				if (InvokeRequired)
					Invoke(refreshAction);
				else
					refreshAction();
			});
		}

		private void SortByColumn(int iClickedCol, bool sortAscending)
		{
			if (iClickedCol == m_colReference.Index)
				m_helper.Sort(PhrasesSortedBy.Reference, sortAscending, true);
			else if (iClickedCol == m_colEnglish.Index)
				m_helper.Sort(PhrasesSortedBy.EnglishPhrase, sortAscending, true);
			else if (iClickedCol == m_colTranslation.Index)
				m_helper.Sort(PhrasesSortedBy.Translation, sortAscending, true);
			else if (iClickedCol == m_colUserTranslated.Index)
				m_helper.Sort(PhrasesSortedBy.Status, sortAscending, true);
			else if (iClickedCol == m_colDebugInfo.Index)
				m_helper.Sort(PhrasesSortedBy.Default, sortAscending, true);
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

		private void ApplyFilter(Action setCurrentCell = null)
		{
			bool clearCurrentSelection = false;
			
			if (m_currentPhrase == null)
			{
				RememberCurrentSelection();
				clearCurrentSelection = true;
			}
            var refFilter = m_startRef == null ? null :
				new Func<int, int, string, bool>((start, end, scrRef) => m_endRef.BBBCCCVVV >= start && m_startRef.BBBCCCVVV <= end);

			SetUiForLongTask(true, () =>
			{
				dataGridUns.RowEnter -= dataGridUns_RowEnter;
				dataGridUns.RowCount = 0;
				m_biblicalTermsPane.Hide();
			});
			
			if (setCurrentCell == null)
			{
				setCurrentCell = () =>
				{
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
					else
						HandleDataGridUnsPopulatedFirstTime();
				};
			}

			void CompletionAction()
			{
				void ResetGridCountsAndStatus()
				{
					ResetRowCount();

					dataGridUns.RowEnter += dataGridUns_RowEnter;

					int initialRow = dataGridUns.CurrentCell?.RowIndex ?? -1;

					setCurrentCell();

					if (dataGridUns.CurrentCell == null)
					{
						if (m_pnlAnswersAndComments.Visible)
							m_lblAnswerLabel.Visible = m_lblAnswers.Visible = m_lblCommentLabel.Visible = m_lblComments.Visible = false;
					}
					else if (initialRow == dataGridUns.CurrentCell.RowIndex)
					{
						dataGridUns_RowEnter(dataGridUns, new DataGridViewCellEventArgs(m_iCurrentColumn, initialRow));
					}

					UpdateCountsAndFilterStatus();
				}

				SetUiForLongTask(false, ResetGridCountsAndStatus);
			}

			Task.Run(() =>
			{
				Trace.WriteLine($"Filtering at {DateTime.Now}");
				m_helper.Filter(txtFilterByPart.Text, MatchWholeWords, CheckedKeyTermFilterType, refFilter,
					mnuViewExcludedQuestions.Checked, m_dataLocalizer == null ? null :
						(Func<TranslatablePhrase, string>)(tp => m_dataLocalizer.GetLocalizedString(tp.ToUIDataString())));

			}).ContinueWith(t =>
			{
				Trace.WriteLine($"Done Filtering at {DateTime.Now}");

				if (InvokeRequired)
					Invoke(new Action(CompletionAction));
				else
					CompletionAction();
			});
		}

		private void SetUiForLongTask(bool startingTask, Action doIfStartingOrEnding)
		{
			lock (this)
			{
				m_longTaskStackCount += startingTask ? 1 : -1;
				Debug.Assert(m_longTaskStackCount >= 0);

				if (startingTask && m_longTaskStackCount > 1 || !startingTask && m_longTaskStackCount > 0)
					return;

				doIfStartingOrEnding.Invoke();

				foreach (Control control in Controls)
				{
					if (control != toolStrip1 || !control.ContainsFocus)
						control.Enabled = !startingTask;
				}

				UseWaitCursor = startingTask;

				if (!startingTask)
				{
					if (m_queuedReference != null)
						ProcessQueuedReferenceChange();
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellDoubleClick event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == m_colDebugInfo.Index)
			{
				var sbldr = new StringBuilder();
				sbldr.AppendLine("Key Terms:");
				foreach (var keyTerm in m_helper[e.RowIndex].GetParts().OfType<KeyTerm>())
				{
					foreach (var sTermId in keyTerm.AllTermIds)
						sbldr.AppendLine(sTermId);
				}
				ShowModalChild(new MessageBoxForm(sbldr.ToString(), "More Key Term Debug Info", icon:MessageBoxIcon.None));
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
				foreach (ToolStripMenuItem menu in mnuFilterBiblicalTerms.DropDownItems)
				{
					if (menu != clickedMenu)
						menu.Checked = false;
				}
			}
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
			m_fileAccessor.Write(DataFileAccessor.DataFileId.Translations,
				(from translatablePhrase in m_helper.UnfilteredPhrases
				where translatablePhrase.HasUserTranslation
				select new XmlTranslation(translatablePhrase)).ToList());

			if (fSaveCustomizations)
			{
				List<PhraseCustomization> customizations = m_helper.CustomizedPhrases;

				if (customizations.Count > 0 || m_fileAccessor.Exists(DataFileAccessor.DataFileId.QuestionCustomizations))
				{
					m_fileAccessor.Write(DataFileAccessor.DataFileId.QuestionCustomizations,
						customizations);
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
            GenerateScript(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the mnuGenerate control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void GenerateScript(string defaultFolder)
		{
			var scriptGenerator = new HtmlScriptGenerator(m_project.Language.Id, f => m_helper.AllActivePhrasesWhere(f),
				m_colTranslation.DefaultCellStyle.Font.FontFamily.Name, p => m_sectionInfo.Find(p));
			scriptGenerator.DataLocalizerNeeded += (sender, id) => GetDataLocalizer(id);
			scriptGenerator.ChangeVersification = input => m_project.Versification.ChangeVersification(m_masterVersification.CreateReference(input)).BBBCCCVVV;
			scriptGenerator.AddProjectSpecificCssEntries = tw =>
			{
				var languageInfo = m_project.Language;
				var fontInfo = languageInfo.Font;

				tw.WriteLine(new UsfmStyleBasedCss(fontInfo.FontFamily, fontInfo.Features,
					languageInfo.Id, fontInfo.Size, languageInfo.IsRtoL, m_project.ScriptureMarkerInformation).CreateCSS());
			};

			var locales = new Dictionary<string, string>();
			foreach (var item in mnuDisplayLanguage.DropDownItems)
			{
				if (!(item is ToolStripButton menu))
					break;
				locales[(string)menu.Tag] = menu.Text;
			}
			
			GenerateScriptDlg generateScriptDlg = new GenerateScriptDlg(m_project.ShortName,
				defaultFolder, AvailableBookIds, m_sectionInfo.AllSections, locales,
				scriptGenerator);

			ShowModalChild(generateScriptDlg, dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					Task.Run(() =>
					{
						scriptGenerator.Extractor = new ScriptureExtractor(m_project, vRef => m_project.Versification.CreateReference(vRef));

						using (var sw = new StreamWriter(scriptGenerator.FileName, false, Encoding.UTF8))
							scriptGenerator.Generate(sw);

						Process.Start(scriptGenerator.FileName);
					}).ContinueWith(t =>
					{
						ErrorReport.ReportNonFatalException(t.Exception);
					}, TaskContinuationOptions.OnlyOnFaulted);
				}
			});
		}

		private void ProduceScriptureForgeFiles()
		{
			if (m_fileAccessor.IsReadonly)
				return;

			using (new WaitCursor(this))
			{
				var allAvailableLocalizers = LocalizationsFileAccessor.GetAvailableLocales(m_installDir).Select(GetDataLocalizer).ToList();

				// TXL-233: If Scripture Forge output files are created for a book but the user
				// later goes back and removes user-confirmed translations such that the book has
				// none, GetQuestionsForBooks needs to include those books with an empty list so
				// that the existing files are not left with the old content.
				var booksWithExistingSfFiles = new List<int>();
				for (var b = 1; b <= BCVRef.LastBook; b++)
				{
					var bookId = BCVRef.NumberToBookCode(b);
					if (m_fileAccessor.BookSpecificDataExists(DataFileAccessor.BookSpecificDataFileId.ScriptureForge, bookId))
						booksWithExistingSfFiles.Add(b);
				}

				foreach (var questionsForBook in m_helper.GetQuestionsForBooks(m_project.Language.Id, allAvailableLocalizers, booksWithExistingSfFiles))
				{
					m_fileAccessor.WriteBookSpecificData(DataFileAccessor.BookSpecificDataFileId.ScriptureForge,
						questionsForBook.BookId, questionsForBook);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes a file with the questions having verse numbers in parentheses and an
		/// line containing the answers.
		/// </summary>
		/// ------------------------------------------------------------------------------------		
		private void generateOutputForArloToolStripMenuItem_Click(object sender, EventArgs e)
		{
#if DEBUG
			using (var dlg = new SaveFileDialog())
			{
				dlg.DefaultExt = "txt";
				dlg.InitialDirectory = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					"SoftDev"), "Transcelerator");

				string sRef;
				Func<BCVRef, BCVRef, bool> InRange;
				if (m_startRef == null)
				{
					sRef = "- All";
					InRange = (bcvStart, bcvEnd) => true;
				}
				else
				{
					sRef = m_startRef.BookCode;
					if (m_startRef.BookNum != m_endRef.BookNum)
						sRef += "-" + m_endRef.BookCode;

					InRange = (bcvStart, bcvEnd) => bcvStart >= m_startRef.BBBCCCVVV && bcvEnd <= m_endRef.BBBCCCVVV;
				}

				string language = m_project.Language.Id;
				string sChapter = "Chapter";
				string sPsalm = "Psalm";
				if (language == "es")
				{
					language = "Spanish";
					sChapter = "Capi\u0301tulo";
					sPsalm = "Salmo";
				}

				dlg.FileName = $"Translations of {language} Questions {sRef}";
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
						throw new ParatextPluginException(ex);
					}
				}
			}
#endif
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes translations in a file having format: \rf BOOK C#, followed by questions
		/// with verse reference(s) in parentheses.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void mnuLoadTranslationsFromTextFile_Click(object sender, EventArgs e)
		{
#if DEBUG
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
					ShowModalChild(new MessageBoxForm($"Finished! See report in {reportFilename}", TxlCore.kPluginName, icon:MessageBoxIcon.None));
				}
			}
#endif
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
			if (!m_fileAccessor.IsReadonly)
				Properties.Settings.Default.AutoSave = mnuAutoSave.Checked;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the phraseSubstitutionsToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void phraseSubstitutionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PhraseSubstitutionsDlg dlg = new PhraseSubstitutionsDlg(PhraseSubstitutions,
				m_helper.Phrases.Where(tp => tp.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion).Select(p => p.PhraseInUse),
				dataGridUns.CurrentRow?.Index ?? 0);
			
			if (m_fileAccessor.IsReadonly)
				dlg.ReadonlyAlert = ReadonlyAlert;
			
			m_selectKeyboard?.Invoke(false);
			m_lockToHold = DataFileAccessor.DataFileId.PhraseSubstitutions;
			ShowModalChild(dlg, substitutionsDlg =>
			{
				if (substitutionsDlg.DialogResult == DialogResult.OK)
				{
					PhraseSubstitutions.Clear();
					PhraseSubstitutions.AddRange(dlg.Substitutions);
                    m_fileAccessor.Write(DataFileAccessor.DataFileId.PhraseSubstitutions,
						PhraseSubstitutions);

					Reload(false);
				}
				m_selectKeyboard?.Invoke(true);
				m_lockToHold = DataFileAccessor.DataFileId.Translations;
			});
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
			Reload(fForceSave, phrase?.PhraseKey, 0);
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
		/// <param name="fallBackRow">the index of the row to select if a question with the
		/// given key cannot be found.</param>
		/// ------------------------------------------------------------------------------------
		private void Reload(bool fForceSave, IQuestionKey key, int fallBackRow)
		{
			SetUiForLongTask(true, () =>
			{
				m_helper.TranslationsChanged -= m_helper_TranslationsChanged; 
				lblRemainingWork.Text = LocalizationManager.GetString("MainWindow.Reloading", "Reloading...");
			});

			int iCol = dataGridUns.CurrentCell?.ColumnIndex ?? m_colTranslation.Index;
			Save(fForceSave, fForceSave); // See comment above for fForceSave

			int iSortedCol = -1;
			bool sortAscending = true;
			for (int i = 0; i < dataGridUns.Columns.Count; i++)
			{
				switch (dataGridUns.Columns[i].HeaderCell.SortGlyphDirection)
				{
					case SortOrder.Ascending:
						iSortedCol = i;
						break;
					case SortOrder.Descending:
						iSortedCol = i;
						sortAscending = false;
						break;
					default:
						continue;
				}

				break;
			}

			dataGridUns.CurrentCell = null;
			dataGridUns.RowCount = 0;

			var refreshUi = new Action(() =>
			{
				SetUiForLongTask(false, () =>
					ApplyFilter(() =>
					{
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
					}));
			});

			Task.Run(() => { LoadTranslations(null); }).ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					// REVIEW: If an exception gets thrown and we don't report it explicitly, what happens?
					foreach (var exception in t.Exception.InnerExceptions)
						ErrorReport.ReportFatalException(exception);
					// ???	Close();
				}

				if (InvokeRequired)
					Invoke(refreshUi);
				else
					refreshUi();
			});
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
			// RowEnter frequently fires for the row we are already in. We only care about the
			// first time we're going to a new row. Setting ReadOnly (below) causes a crash if
			// it happens when this gets called as part of committing an edit.
			if (dataGridUns.CurrentRow?.Index == e.RowIndex && m_lastRowEntered >= 0)
				return;

			var phrase = m_helper[e.RowIndex];

			m_lastRowEntered = e.RowIndex;
			if (mnuViewBiblicalTermsPane.Checked)
				LoadKeyTermsPane(phrase);
			if (m_pnlAnswersAndComments.Visible)
				LoadAnswerAndComment(phrase);
			if (btnSendScrReferences.Checked)
				SendScrReference(e.RowIndex);

			try
			{
				DataGridViewRow row = dataGridUns.Rows[e.RowIndex];
				row.ReadOnly = m_helper[e.RowIndex].IsExcluded;

				m_normalRowHeight = row.Height;
				dataGridUns.AutoResizeRow(e.RowIndex);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
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

			IncludeOrExcludeQuestion(sender == mnuExcludeQuestion);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the current phrase to be included or excluded.
		/// </summary>
		/// <param name="exclude">Flag indicating whether this is a request to exclude (or
		/// include) the current question.</param>
		/// ------------------------------------------------------------------------------------
		private void IncludeOrExcludeQuestion(bool exclude)
		{
			CurrentPhrase.IsExcluded = exclude;
			Save(true, true);
			var addressToSelect = dataGridUns.CurrentCellAddress;
			ApplyFilter();
			ResetRowCount();
			if (dataGridUns.RowCount == addressToSelect.Y)
				addressToSelect.Y--;
			dataGridUns.CurrentCell = dataGridUns.Rows[addressToSelect.Y].Cells[addressToSelect.X];
			UpdateCountsAndFilterStatus();
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
			m_selectKeyboard?.Invoke(false);
			m_lockToHold = DataFileAccessor.DataFileId.QuestionCustomizations;
			ShowModalChild(new EditQuestionDlg(phrase,
				m_helper.GetMatchingPhrases(phrase.StartRef, phrase.EndRef)
					.Where(p => p != phrase && p.TypeOfPhrase != TypeOfPhrase.NoEnglishVersion)
					.Select(p => p.PhraseInUse).ToList(), m_dataLocalizer), dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
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

				m_lockToHold = DataFileAccessor.DataFileId.Translations;
			});
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
			m_selectKeyboard?.Invoke(false);
			string language = Format("{0} ({1})", m_project.LanguageName, m_project.Language.Id);
			var newQuestionDlg = new NewQuestionDlg(m_project, CurrentPhrase, language, m_sectionInfo,
				m_project.Versification, m_masterVersification, m_helper, m_selectKeyboard);

			m_lockToHold = DataFileAccessor.DataFileId.QuestionCustomizations;
			ShowModalChild(newQuestionDlg, dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					if (m_parser == null)
						m_parser = new MasterQuestionParser(GetQuestionWords(), m_getKeyTerms(), GetKeyTermRules(), PhraseSubstitutions);

					Question newQuestion = dlg.NewQuestion;
					var basePhrase = dlg.BasePhrase;
					if (basePhrase != null)
					{
						if (dlg.InsertBeforeBasePhrase)
							basePhrase.InsertedPhraseBefore = newQuestion;
						else
							basePhrase.AddedPhraseAfter = newQuestion;
					}

					var newPhrase = m_helper.AddQuestion(newQuestion, dlg.OwningSection,
						dlg.Category, dlg.SequenceNumber, m_parser);
					if (basePhrase == null)
						m_helper.AttachNewQuestionToAdjacentPhrase(newPhrase);

					if (dlg.Translation != Empty)
						newPhrase.Translation = dlg.Translation;

					Save(true, true);
					ResetRowCount();

					SelectTranslationCellIn(m_helper.FindPhrase(newPhrase.QuestionInfo));
					UpdateCountsAndFilterStatus();
				}

				m_lockToHold = DataFileAccessor.DataFileId.Translations;
			});
		}

		private void dataGridUns_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
		{
			e.ContextMenuStrip = (m_helper[e.RowIndex].Category == -1  || IsShowingModalForm) ? null : dataGridContextMenu;
		}

		private void dataGridContextMenu_Opening(object sender, CancelEventArgs e)
		{
			bool fExcluded = CurrentPhrase.IsExcluded;
			mnuExcludeQuestion.Visible = !fExcluded && !m_fileAccessor.IsReadonly && CurrentPhrase.Category != -1; // Can't exclude categories
			mnuIncludeQuestion.Visible = fExcluded && !m_fileAccessor.IsReadonly;
			mnuEditQuestion.Enabled = !fExcluded && !m_fileAccessor.IsReadonly;
			cutToolStripMenuItem.Enabled = !m_fileAccessor.IsReadonly;
			pasteToolStripMenuItem.Enabled = !m_fileAccessor.IsReadonly;
		}

		private void dataGridUns_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			try
			{
				dataGridUns.Rows[e.RowIndex].DefaultCellStyle.BackColor = m_helper[e.RowIndex].IsExcluded ?
					Color.LightCoral : dataGridUns.DefaultCellStyle.BackColor;
			}
			catch (Exception)
			{
				// Collection size is probably changing. Don't crash
				Debug.Fail("Probably changing filter while still trying to paint previously existing rows.");
			}
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
			m_selectKeyboard?.Invoke(false);
			ScrReferenceFilterDlg filterDlg = new ScrReferenceFilterDlg(m_project,
				m_startRef?.ChangeVersification(m_project.Versification),
				m_endRef?.ChangeVersification(m_project.Versification), m_availableBookIds);
			ShowModalChild(filterDlg, dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					// If nothing changed, don't waste time re-filtering.
					var newStartRefValue = dlg.FromRef;
					var newEndRefValue = dlg.ToRef;
					if (m_startRef != newStartRefValue || m_endRef != newEndRefValue)
					{
						var englishVrs = m_host.GetStandardVersification(StandardScrVersType.English);
						m_startRef = newStartRefValue?.ChangeVersification(englishVrs);
						m_endRef = newEndRefValue?.ChangeVersification(englishVrs);
						Properties.Settings.Default.FilterStartRef = m_startRef?.BBBCCCVVV ?? 0;
						Properties.Settings.Default.FilterEndRef = m_endRef?.BBBCCCVVV ?? 0;
						ApplyFilter();
					}
				}
			});
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

			var selection = TextControl == null ? null : new SubstringDescriptor(TextControl);

			var phrase = m_helper[rowIndex];

			var provisionalTranslation = phrase.InsertKeyTermRendering(
				TextControl?.Text ?? phrase.Translation, sender, sender.SelectedRendering, ref selection);
			if (TextControl == null)
			{
				if (phrase.HasUserTranslation)
				{
					phrase.Translation = provisionalTranslation;
					dataGridUns.InvalidateRow(rowIndex);
					SaveNeeded = true;
					return;
				}

				if (dataGridUns.CurrentCellAddress.X != m_colTranslation.Index)
					SelectTranslationCellIn(dataGridUns.CurrentCellAddress.Y);
				dataGridUns.BeginEdit(false);
				// Start and Length values may have been modified
				Debug.Assert(TextControl != null);
			}

			Debug.Assert(selection != null);

			TextControl.Text = provisionalTranslation;
			TextControl.SelectionStart = selection.Start;
			TextControl.SelectionLength = selection.Length;

			TextControl.Focus();
		}

		void SelectTranslationCellIn(int row) =>
			dataGridUns.CurrentCell = dataGridUns.Rows[row].Cells[m_colTranslation.Index];

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
			SaveRenderings();
        }

		private void SaveRenderings()
		{
			m_renderingsRepo.Save();
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
			RenderingSelectionRulesDlg dlg = new RenderingSelectionRulesDlg(
				m_helper.TermRenderingSelectionRules, m_selectKeyboard);

			if (m_fileAccessor.IsReadonly)
				dlg.ReadonlyAlert = ReadonlyAlert;

			m_lockToHold = DataFileAccessor.DataFileId.TermRenderingSelectionRules;

			ShowModalChild(dlg, rulesDlg =>
			{
				if (rulesDlg.DialogResult == DialogResult.OK)
				{
					m_helper.TermRenderingSelectionRules = new List<RenderingSelectionRule>(rulesDlg.Rules);
					KeyTermBestRenderingsChanged();
				}

				m_lockToHold = DataFileAccessor.DataFileId.Translations;
			});
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the EditingControlShowing event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridViewEditingControlShowingEventArgs"/>
		/// instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			m_translationEditWasCommitted = false;

			Debug.WriteLine($"dataGridUns_EditingControlShowing: m_lastTranslationSet = {m_lastTranslationSet}");
			if (TextControl == null)
				return;

			TextControl.UseWaitCursor = TextControl.Parent.UseWaitCursor = false;
			TextControl.AllowDrop = true;
			TextControl.DragDrop += TextControl_DragDrop;
			TextControl.DragEnter += TextControl_Drag;
			TextControl.DragOver += TextControl_Drag;
			TextControl.GiveFeedback += TextControl_GiveFeedback;
			TextControl.PreviewKeyDown += txtControl_PreviewKeyDown;
		}

		private void dataGridUns_HandleCreated(object sender, EventArgs e)
		{
			m_host.VerseRefChanged += OnHostOnVerseRefChanged;
		}

		private void OnHostOnVerseRefChanged(IPluginHost host, IVerseRef reference, SyncReferenceGroup @group)
		{
			lock (this)
			{
				if (!btnReceiveScrReferences.Checked || m_fSendingSyncMessage)
					return;

				var scrRef = new BCVRef(reference.ChangeVersification(m_masterVersification).BBBCCCVVV);

				if (m_fProcessingSyncMessage || !dataGridUns.Enabled)
					m_queuedReference = scrRef;
				else
					ProcessCurrentVerseRefChange(scrRef);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellEndEdit event of the dataGridUns control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void dataGridUns_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			Debug.WriteLine($"dataGridUns_CellEndEdit: m_lastTranslationSet = {m_lastTranslationSet};" +
				$"m_translationEditWasCommitted = {m_translationEditWasCommitted}");

			if (!m_translationEditWasCommitted && dataGridUns.CurrentRow != null)
				LoadKeyTermsPane(dataGridUns.CurrentRow.Index);

			if (TextControl != null)
			{
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

			TextControl.MoveSelectedWord(sender == (m_project.Language.IsRtoL ? mnuShiftWordsLeft : mnuShiftWordsRight));
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
			ShowModalChild(new HelpAboutDlg(Icon));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the browseTopicsToolStripMenuItem control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void browseTopicsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start(HelpHome);
		}

		private bool HandleDisplayLanguageSelected(string languageId)
		{
			var previousLocale = m_dataLocalizer?.Locale ?? "en-us";
			Analytics.Track("UI language chosen",
				new Dictionary<string, string> {
					{ "Previous", previousLocale },
					{ "New", languageId } });
			Logger.WriteEvent("UI language changed from " +
				$"{previousLocale} to {languageId}");
			SetLocalizer(languageId);
			TxlPlugin.UpdateUiLanguageForUser(languageId);

			Properties.Settings.Default.OverrideDisplayLanguage = languageId;
			if (txtFilterByPart.Text.Trim().Length > 0)
				ApplyFilter();
			else
				dataGridUns.Invalidate();
			LoadAnswersAndCommentsIfShowing(null, null);
			
			return true;
		}

		private bool ShowMoreUiLanguagesDlg()
		{
			Analytics.Track("Clicked More on UI locale menu");
			ShowModalChild(new MoreUiLanguagesDlg(mnuDisplayLanguage));
			return false;
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
			string sRef = null;
			try
			{
				sRef = dataGridUns.Rows[iRow].Cells[m_colReference.Index].Value as string;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			if (IsNullOrEmpty(sRef))
				return null;
			int ichDash = sRef.IndexOf('-');
			if (ichDash > 0)
				sRef = sRef.Substring(0, ichDash);
			return new BCVRef(sRef);
		}

		private void ReportMissingInstalledFile(string filename)
		{
			ErrorReport.NotifyUserOfProblem(GetMissingInstalledFileMessage(false, filename));
		}

		private string GetMissingInstalledFileMessage(bool required, string filename)
		{
			var fmt = required ?
				LocalizationManager.GetString("General.RequiredInstalledFileMissing",
					"Required file missing: {0}.",
					"Parameter is filename (for technical support)") :
				LocalizationManager.GetString("General.ExpectedInstalledFileMissing",
					"Expected file missing: {0}.",
					"Parameter is filename (for technical support)");
			var msg = Format(fmt, filename) + " " +
				Format(LocalizationManager.GetString("General.RerunInstaller",
					"Please re-run the {0} Installer to repair this problem.",
					"Parameter is \"Transcelerator\" (plugin name)"), TxlCore.kPluginName);
			return msg;
		}

		private void SetScrForgeMenuIcon()
		{
			mnuProduceScriptureForgeFiles.Image = mnuProduceScriptureForgeFiles.Checked ?
				Resources.sf_logo_medium___selected : Resources.sf_logo_medium;
		}

		private void UpdateSplashScreenMessage(IProgressMessage splashScreen, string fmt)
		{
			if (splashScreen != null)
				splashScreen.Message = Format(fmt, m_project.ShortName);
		}

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Loads the translations.
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
		public void LoadTranslations(IProgressMessage splashScreen)
		{
			UpdateSplashScreenMessage(splashScreen,
				LocalizationManager.GetString("SplashScreen.MsgLoadingQuestions",
					"Loading questions for {0}...", "Param is project name"));

			FileInfo finfoMasterQuestions = new FileInfo(m_masterQuestionsFilename);
			if (!finfoMasterQuestions.Exists)
				throw new FileNotFoundException(GetMissingInstalledFileMessage(true, m_masterQuestionsFilename));

			string keyTermRulesFilename = Path.Combine(m_installDir, TxlCore.kKeyTermRulesFilename);

            FileInfo finfoKtRules = new FileInfo(keyTermRulesFilename);
            if (!finfoKtRules.Exists)
				ReportMissingInstalledFile(keyTermRulesFilename);

			string questionWordsFilename = Path.Combine(m_installDir, TxlCore.kQuestionWordsFilename);

			FileInfo finfoQuestionWords = new FileInfo(questionWordsFilename);
			if (!finfoQuestionWords.Exists)
				ReportMissingInstalledFile(questionWordsFilename);

			FileInfo finfoParsedQuestions = new FileInfo(m_parsedQuestionsFilename);

	        FileInfo finfoTxlDll = new FileInfo(Assembly.GetExecutingAssembly().Location);

            Exception e;
	        ParsedQuestions parsedQuestions;
	        if (finfoParsedQuestions.Exists &&
                finfoMasterQuestions.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc &&
				finfoTxlDll.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc &&
				(!finfoKtRules.Exists || finfoKtRules.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc) &&
				(!finfoQuestionWords.Exists || finfoQuestionWords.LastWriteTimeUtc < finfoParsedQuestions.LastWriteTimeUtc) &&
                m_fileAccessor.ModifiedTime(DataFileAccessor.DataFileId.QuestionCustomizations).ToUniversalTime() < finfoParsedQuestions.LastWriteTimeUtc &&
                m_fileAccessor.ModifiedTime(DataFileAccessor.DataFileId.PhraseSubstitutions).ToUniversalTime() < finfoParsedQuestions.LastWriteTimeUtc)
	        {
	            parsedQuestions = XmlSerializationHelper.DeserializeFromFile<ParsedQuestions>(m_parsedQuestionsFilename);
	        }
	        else
	        {
				UpdateSplashScreenMessage(splashScreen,
					LocalizationManager.GetString("SplashScreen.MsgParsingQuestions",
		                "Processing questions for {0} using Major Biblical Terms list...",
		                "Param is the Paratext project name. If localizing into a language into which " +
		                "Paratext is also localized, the name of the list should exactly match " +
		                "the name used in Paratext"));

	            List<PhraseCustomization> customizations = null;
	            string phraseCustData = m_fileAccessor.Read(DataFileAccessor.DataFileId.QuestionCustomizations);
	            if (!IsNullOrEmpty(phraseCustData))
	            {
	                customizations = XmlSerializationHelper.DeserializeFromString<List<PhraseCustomization>>(phraseCustData, out e);
					if (e != null)
					{
						ErrorReport.NotifyUserOfProblem(e, LocalizationManager.GetString("General.LoadingCustomizationsFailed",
							"Unable to load customizations."));
					}
				}

		        m_parser = new MasterQuestionParser(m_masterQuestionsFilename, GetQuestionWords(),
                    m_getKeyTerms(), GetKeyTermRules(keyTermRulesFilename), customizations, PhraseSubstitutions);
	            parsedQuestions = m_parser.Result;
		        Directory.CreateDirectory(Path.GetDirectoryName(m_parsedQuestionsFilename));
	            XmlSerializationHelper.SerializeToFile(m_parsedQuestionsFilename, parsedQuestions);
	        }

			m_renderingsRepo = new ParatextTermRenderingsRepo(m_fileAccessor, m_project);
			var phrasePartManager = new PhrasePartManager(parsedQuestions.TranslatableParts, parsedQuestions.KeyTerms, m_renderingsRepo);
	        var qp = new QuestionProvider(parsedQuestions, phrasePartManager);
            m_helper = new PhraseTranslationHelper(qp);
			m_helper.VernacularStringComparer = m_project.Language.StringComparer;
		    m_helper.FileProxy = m_fileAccessor;
	        m_sectionInfo = qp.SectionInfo;
			m_availableBookIds = qp.AvailableBookIds;
		    string translationData = m_fileAccessor.Read(DataFileAccessor.DataFileId.Translations);
			if (!IsNullOrEmpty(translationData))
			{
				UpdateSplashScreenMessage(splashScreen,
					LocalizationManager.GetString("SplashScreen.MsgLoadingTranslations",
						"Loading translations for {0}...", "Param is project name"));

				List<XmlTranslation> translations = XmlSerializationHelper.DeserializeFromString<List<XmlTranslation>>(translationData, out e);
				if (e != null)
				{
					ErrorReport.NotifyUserOfProblem(e, LocalizationManager.GetString("General.LoadingTranslationsFailed",
						"Unable to load translations."));
				}
				else
				{
					foreach (XmlTranslation unsTranslation in translations.Where(t => !IsNullOrWhiteSpace(t.Translation)))
					{
						TranslatablePhrase phrase = m_helper.GetPhrase(unsTranslation.Reference, unsTranslation.PhraseKey);
						if (phrase != null && (!phrase.IsExcluded || phrase.IsUserAdded))
							phrase.Translation = unsTranslation.Translation;
					}
				}
			}
			m_helper.ProcessAllTranslations();
			m_helper.TranslationsChanged += m_helper_TranslationsChanged;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the phrase substitutions if not already loaded
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private List<Substitution> PhraseSubstitutions =>
			m_cachedPhraseSubstitutions ??
			(m_cachedPhraseSubstitutions = ListSerializationHelper.LoadOrCreateListFromString<Substitution>(
				m_fileAccessor.Read(DataFileAccessor.DataFileId.PhraseSubstitutions), true));

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads and initializes the key term rules
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private KeyTermRules GetKeyTermRules(string keyTermRulesFilename = null)
		{
			if (keyTermRulesFilename == null)
				keyTermRulesFilename = Path.Combine(m_installDir, TxlCore.kKeyTermRulesFilename);

			KeyTermRules rules = XmlSerializationHelper.DeserializeFromFile<KeyTermRules>(keyTermRulesFilename, out var e);
			if (e != null)
				throw new ParatextPluginException(e);

			rules.Initialize();
			return rules;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the built-in question words
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string[] GetQuestionWords()
		{
			var questionWordsFilename = Path.Combine(m_installDir, TxlCore.kQuestionWordsFilename);
			var questionWords = XmlSerializationHelper.DeserializeFromFile<QuestionWords>(questionWordsFilename, out var e);
			if (e != null)
				throw new ParatextPluginException(e);
			return questionWords.Items;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the key terms pane.
		/// </summary>
		/// <param name="rowIndex">Index of the row to load for.</param>
		/// ------------------------------------------------------------------------------------
		private void LoadKeyTermsPane(int rowIndex)
		{
			LoadKeyTermsPane(m_helper[rowIndex]);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the key terms pane.
		/// </summary>
		/// <param name="phrase">Phrase for which the pane is to be loaded.</param>
		/// ------------------------------------------------------------------------------------
		private void LoadKeyTermsPane(TranslatablePhrase phrase)
		{
			m_loadingBiblicalTermsPane = true;
			m_biblicalTermsPane.SuspendLayout();
			ClearBiblicalTermsPane();
			m_biblicalTermsPane.Height = MaximumHeightOfKeyTermsPane;
			int col = 0;
			int longestListHeight = 0;
			Dictionary<KeyTerm, int> previousKeyTermEndOfRenderingOffsets = new Dictionary<KeyTerm, int>();
			var majorTerms = m_host.GetBiblicalTermList(BiblicalTermListType.Major);

			foreach (KeyTerm keyTerm in phrase.GetParts().OfType<KeyTerm>())//.Where(ktm => ktm.Renderings.Any()))
			{
				previousKeyTermEndOfRenderingOffsets.TryGetValue(keyTerm, out var ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm);
				TermRenderingCtrl ktRenderCtrl = new TermRenderingCtrl(keyTerm,
					ichEndRenderingOfPreviousOccurrenceOfThisSameKeyTerm, DisplayExceptionMessage, LookupTerm,
					m_termLocalizer?.GetTermHeading(keyTerm), m_fileAccessor.IsReadonly);
				ktRenderCtrl.VernacularFont = m_vernFont;

				SubstringDescriptor sd = phrase.FindTermRenderingInUse(ktRenderCtrl);
				if (sd == null)
				{
					// Didn't find any renderings for this term in the translation, so don't select anything
					previousKeyTermEndOfRenderingOffsets[keyTerm] = phrase.Translation.Length;
				}
				else
				{
					previousKeyTermEndOfRenderingOffsets[keyTerm] = sd.EndOffset;
					ktRenderCtrl.SelectedRendering = phrase.Translation.Substring(sd.Start, sd.Length);
				}
				ktRenderCtrl.Dock = DockStyle.Fill;
				m_biblicalTermsPane.Controls.Add(ktRenderCtrl, col, 0);
				if (ktRenderCtrl.NaturalHeight > longestListHeight)
					longestListHeight = ktRenderCtrl.NaturalHeight;
				ktRenderCtrl.SelectedRenderingChanged += KeyTermRenderingSelected;
				ktRenderCtrl.BestRenderingsChanged += KeyTermBestRenderingsChanged;
				ktRenderCtrl.RenderingAddedOrDeleted += SaveRenderings;
				col++;
			}
			m_biblicalTermsPane.ColumnCount = col;
			ResizeKeyTermPaneColumns();
			m_biblicalTermsPane.Height = Math.Min(longestListHeight, MaximumHeightOfKeyTermsPane);
			m_biblicalTermsPane.Visible = m_biblicalTermsPane.ColumnCount > 0;
			m_biblicalTermsPane.ResumeLayout();
			m_loadingBiblicalTermsPane = false;
		}

		private void LookupTerm(IReadOnlyList<string> termIds)
		{
			var majorTerms = m_host.GetBiblicalTermList(BiblicalTermListType.Major);
		    List<IBiblicalTerm> termsToLoad = new List<IBiblicalTerm>();
		    int startRef = CurrentPhrase.StartRef;
		    int endRef = CurrentPhrase.EndRef;
			bool foundPriorityTerm = false;
            foreach (var term in majorTerms.Where(t => termIds.Contains(t.Lemma) ))
			{
				if (term.Occurrences.Select(o => o.BBBCCCVVV).Any(o => o >= startRef && o <= endRef))
				{
					if (!foundPriorityTerm)
					{
						termsToLoad.Clear();
						foundPriorityTerm = true;
					}
				}
				else if (foundPriorityTerm)
					continue;
				termsToLoad.Add(term);
            }

			m_host.BiblicalTermsWindow.LoadFilteredList(m_project, termsToLoad, majorTerms);
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
			if (m_helper == null)
				return; // Not ready yet. We'll get back here later...
			if (m_helper.FilteredPhraseCount == m_helper.UnfilteredPhraseCount)
			{
				lblFilterIndicator.Text = (string)lblFilterIndicator.Tag;
				lblFilterIndicator.Image = null;
			}
			else
			{
				lblFilterIndicator.Text = LocalizationManager.GetString("MainWindow.FilteredStatus", "Filtered");
				lblFilterIndicator.Image = Resources.Filtered;
			}
			lblRemainingWork.Text = Format((string)lblRemainingWork.Tag,
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
			{
				ctrl.SelectedRenderingChanged -= KeyTermRenderingSelected;
				ctrl.BestRenderingsChanged -= KeyTermBestRenderingsChanged;
				ctrl.Dispose();
			}

			m_biblicalTermsPane.Controls.Clear();
			m_biblicalTermsPane.ColumnCount = 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Notifies the host of the current (start) reference for the given row.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SendScrReference(int iRow)
		{
			lock (this)
			{
				if (m_fProcessingSyncMessage || m_queuedReference != null)
					return;
				var currentRef = GetScrRefOfRow(iRow);
				if (currentRef != null && currentRef.Valid)
				{
					m_fSendingSyncMessage = true;
					m_sendReference?.Invoke(currentRef);
					m_fSendingSyncMessage = false;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes the received request to update the current verse reference.
		/// </summary>
		/// <param name="reference">The reference (in English versification scheme).</param>
		/// ------------------------------------------------------------------------------------
		private void ProcessCurrentVerseRefChange(BCVRef reference)
		{
            if (dataGridUns.IsCurrentCellInEditMode)
                return;

			lock (this)
			{
				// While we process the given reference we might get additional synch events, the
				// most recent of which we store in m_queuedReference. If we're done
				// and we have a new reference in m_queuedReference we process that one, etc.
				for (; reference != null; reference = m_queuedReference)
				{
					m_queuedReference = null;
					m_fProcessingSyncMessage = true;

					int currentPhraseIndex = dataGridUns.CurrentCellAddress.Y;
					GoToReference(reference, currentPhraseIndex);
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Goes to the first row in the data grid corresponding to the given reference,
		/// preferably for a detail question.
		/// </summary>
        /// <param name="reference">The reference to check against</param>
        /// <param name="currentPhraseIndex">The index of the currently selected question</param>
        /// ------------------------------------------------------------------------------------
        private void GoToReference(BCVRef reference, int currentPhraseIndex)
		{
			Action<Task<int>> completionAction = t =>
			{
				lock (this)
				{
					if (m_queuedReference != null && !m_queuedReference.Equals(reference))
						ProcessQueuedReferenceChange();
					else
					{
						if (t.Result >= 0 && m_queuedReference == null && !dataGridUns.IsCurrentCellInEditMode &&
							dataGridUns.RowCount > t.Result)
							dataGridUns.CurrentCell = dataGridUns.Rows[t.Result].Cells[dataGridUns.CurrentCell?.ColumnIndex ??
								m_colTranslation.Index];

						m_fProcessingSyncMessage = false;
					}
				}
			};

			Task.Run(() =>
			{
				if (!reference.Valid)
					return -1;

				bool detailOnly = false;

				if (currentPhraseIndex >= 0)
				{
					var currentPhrase = m_helper.Phrases.ElementAt(currentPhraseIndex);

					if (currentPhrase.AppliesToReference(reference))
					{
						if (currentPhrase.IsDetail)
							return -1;

						// If we're already on a question for the current reference, we only
						// want to change to a different one if we are currently on an
						// overview question and we're able to find a detail question for the
						// new reference.
						detailOnly = true;
					}
				}

				int iFound = -1;
				int iRow = 0;
				foreach (var phrase in m_helper.Phrases)
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

				// This is not strictly necessary, but it gives Paratext a bit of a fighting chance to reclaim the
				// UI thread so it can finish its updates before Transcelerator, which is probably generally what
				// the user would expect.
				Thread.Sleep(2);

				return iFound;
			}
			).ContinueWith(t =>
			{
				if (InvokeRequired)
					Invoke(new Action(() => { completionAction(t); }));
				else
					completionAction(t);
			});
		}

		private void ProcessQueuedReferenceChange()
		{
			BCVRef reference = m_queuedReference;
			m_queuedReference = null;
			ProcessCurrentVerseRefChange(reference);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the answer and comment labels for the given row.
		/// </summary>
		/// <param name="rowIndex">Index of the row.</param>
		/// ------------------------------------------------------------------------------------
		private void LoadAnswerAndComment(int rowIndex)
		{
			LoadAnswerAndComment(m_helper[rowIndex]);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the answer and comment labels for the given row.
		/// </summary>
		/// <param name="phrase">Phrase for which the anwer(s) and comment(s) are to be loaded.
		/// </param>
		/// ------------------------------------------------------------------------------------
		private void LoadAnswerAndComment(TranslatablePhrase phrase)
		{
			var question = phrase.QuestionInfo;
			PopulateAnswerOrCommentLabel(question, question?.Answers, LocalizableStringType.Answer,
				m_lblAnswerLabel, m_lblAnswers, LocalizationManager.GetString("MainWindow.AnswersLabel", "Answers:"));
			PopulateAnswerOrCommentLabel(question, question?.Notes, LocalizableStringType.Note,
				m_lblCommentLabel, m_lblComments, LocalizationManager.GetString("MainWindow.CommentsLabel", "Comments:"));
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
			if (m.Msg == (int)Msg.WM_KEYDOWN)
			{
				var keys = (Keys)m.WParam;

				if (ModifierKeys != Keys.Control || keys == Keys.ControlKey)
					return false;

				var increaseFontSize = keys == Keys.Add || keys == Keys.Oemplus ? 1.1f :
					keys == Keys.Subtract || keys == Keys.OemMinus ? .9f : 0;

				if (increaseFontSize == 0)
					return false;

				Zoom(increaseFontSize);
			}
			return false;
		}

		private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Zoom(sender == zoomInToolStripMenuItem ? 1.1f : 0.9f);
		}

		private void Zoom(float increaseFontSize)
		{ 
			var existingFont = dataGridUns.DefaultCellStyle.Font;
			dataGridUns.DefaultCellStyle.Font = new Font(existingFont.FontFamily, existingFont.Size * increaseFontSize, existingFont.Style);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the given point is contained within the selected region of text
		/// in the text control that is currently being used to edit the Translation.
		/// </summary>
		/// <param name="position">Point (typically a mouse position) relative to screen</param>
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
			if (DragAndDropDisabled && e.Effect == DragDropEffects.Move)
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

			if (DragAndDropDisabled)
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

		public void ShowAddRenderingDlg(Action<string, string> addRendering)
		{
			Debug.Assert(!IsShowingModalForm);

			m_lockToHold = DataFileAccessor.DataFileId.KeyTermRenderingInfo;

			ShowModalChild(new AddRenderingDlg(m_selectKeyboard), dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
					addRendering(dlg.Rendering, dlg.Text);

				m_lockToHold = DataFileAccessor.DataFileId.Translations;
			});
		}

		private void DisplayExceptionMessage(Exception ex, string caption = null)
		{
			ShowModalChild(new MessageBoxForm(ex.Message, caption ?? TxlCore.kPluginName));
		}
	}
	#endregion

	#region class SubstringDescriptor
	#endregion
}