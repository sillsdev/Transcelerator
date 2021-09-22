// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.   
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: UNSQuestionsDialog.Designer.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;

namespace SIL.Transcelerator
{
	partial class UNSQuestionsDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (disposing)
			{
				components?.Dispose();
				m_host.VerseRefChanged -= OnHostOnVerseRefChanged;
				m_project.ProjectDataChanged -= OnProjectDataChanged;

				LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Forms designer method
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		// TODO-Linux: VirtualMode is not supported on Mono
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UNSQuestionsDialog));
			this.mnuViewDebugInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewAnswers = new System.Windows.Forms.ToolStripMenuItem();
			this.dataGridUns = new System.Windows.Forms.DataGridView();
			this.m_colReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_colEnglish = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_colEditQuestion = new System.Windows.Forms.DataGridViewImageColumn();
			this.m_colUserTranslated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.m_colDebugInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExcludeQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuIncludeQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEditQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAddQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.m_mainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAutoSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuGenerate = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuProduceScriptureForgeFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.generateOutputForArloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuLoadTranslationsFromTextFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCut = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuPreviousUntranslatedQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNextUntranslatedQuestion = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorShiftWords = new System.Windows.Forms.ToolStripSeparator();
			this.mnuShiftWordsRight = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShiftWordsLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuReferenceRange = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFilterBiblicalTerms = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowAllPhrases = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowPhrasesWithKtRenderings = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowPhrasesWithMissingKtRenderings = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuMatchWholeWords = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewEditQuestionColumn = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewBiblicalTermsPane = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuViewExcludedQuestions = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuDisplayLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.en_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemMoreLanguages = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAdvanced = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuPhraseSubstitutions = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuBiblicalTermsRenderingSelectionRules = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.browseTopicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabelQuestionFilter = new System.Windows.Forms.ToolStripLabel();
			this.txtFilterByPart = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.btnSendScrReferences = new System.Windows.Forms.ToolStripButton();
			this.btnReceiveScrReferences = new System.Windows.Forms.ToolStripButton();
			this.lblFilterIndicator = new System.Windows.Forms.ToolStripLabel();
			this.lblRemainingWork = new System.Windows.Forms.ToolStripLabel();
			this.m_biblicalTermsPane = new System.Windows.Forms.TableLayoutPanel();
			this.m_lblAnswerLabel = new System.Windows.Forms.Label();
			this.m_lblAnswers = new System.Windows.Forms.Label();
			this.m_lblCommentLabel = new System.Windows.Forms.Label();
			this.m_lblComments = new System.Windows.Forms.Label();
			this.m_pnlAnswersAndComments = new System.Windows.Forms.TableLayoutPanel();
			this.m_hSplitter = new System.Windows.Forms.Splitter();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.dataGridUns)).BeginInit();
			this.dataGridContextMenu.SuspendLayout();
			this.m_mainMenu.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.m_pnlAnswersAndComments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// mnuViewDebugInfo
			// 
			this.mnuViewDebugInfo.Checked = true;
			this.mnuViewDebugInfo.CheckOnClick = true;
			this.mnuViewDebugInfo.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewDebugInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewDebugInfo, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewDebugInfo, "MainWindow.Menu.GeneratedTranslationDetails");
			this.mnuViewDebugInfo.Name = "mnuViewDebugInfo";
			this.mnuViewDebugInfo.Size = new System.Drawing.Size(226, 22);
			this.mnuViewDebugInfo.Text = "&Generated Translation Details";
			this.mnuViewDebugInfo.CheckedChanged += new System.EventHandler(this.mnuViewDebugInfo_CheckedChanged);
			// 
			// mnuViewAnswers
			// 
			this.mnuViewAnswers.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewAnswers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewAnswers, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewAnswers, "MainWindow.Menu.View.AnswersAndComments");
			this.mnuViewAnswers.Name = "mnuViewAnswers";
			this.mnuViewAnswers.Size = new System.Drawing.Size(226, 22);
			this.mnuViewAnswers.Text = "&Answers and Comments";
			this.mnuViewAnswers.CheckedChanged += new System.EventHandler(this.mnuViewAnswersColumn_CheckedChanged);
			// 
			// dataGridUns
			// 
			this.dataGridUns.AllowUserToAddRows = false;
			this.dataGridUns.AllowUserToDeleteRows = false;
			this.dataGridUns.AllowUserToResizeRows = false;
			this.dataGridUns.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridUns.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridUns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridUns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colReference,
            this.m_colEnglish,
            this.m_colTranslation,
            this.m_colEditQuestion,
            this.m_colUserTranslated,
            this.m_colDebugInfo});
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridUns.DefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridUns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridUns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.dataGridUns, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.dataGridUns, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.dataGridUns, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.dataGridUns, "MainWindow.QuestionsGrid");
			this.dataGridUns.Location = new System.Drawing.Point(3, 52);
			this.dataGridUns.Margin = new System.Windows.Forms.Padding(8);
			this.dataGridUns.MultiSelect = false;
			this.dataGridUns.Name = "dataGridUns";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridUns.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dataGridUns.RowHeadersVisible = false;
			this.dataGridUns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGridUns.Size = new System.Drawing.Size(792, 227);
			this.dataGridUns.TabIndex = 7;
			this.dataGridUns.VirtualMode = true;
			this.dataGridUns.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellClick);
			this.dataGridUns.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellContentClick);
			this.dataGridUns.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellDoubleClick);
			this.dataGridUns.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellEndEdit);
			this.dataGridUns.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellEnter);
			this.dataGridUns.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_CellLeave);
			this.dataGridUns.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridUns_CellMouseDown);
			this.dataGridUns.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridUns_CellValueNeeded);
			this.dataGridUns.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridUns_CellValuePushed);
			this.dataGridUns.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridUns_ColumnHeaderMouseClick);
			this.dataGridUns.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridUns_EditingControlShowing);
			this.dataGridUns.HandleCreated += new System.EventHandler(this.dataGridUns_HandleCreated);
			this.dataGridUns.RowContextMenuStripNeeded += new System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler(this.dataGridUns_RowContextMenuStripNeeded);
			this.dataGridUns.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_RowEnter);
			this.dataGridUns.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_RowLeave);
			this.dataGridUns.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridUns_RowPrePaint);
			this.dataGridUns.Resize += new System.EventHandler(this.dataGridUns_Resize);
			// 
			// m_colReference
			// 
			this.m_colReference.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.m_colReference.HeaderText = "_L10N_:MainWindow.QuestionsGrid.Reference!Reference";
			this.m_colReference.Name = "m_colReference";
			this.m_colReference.ReadOnly = true;
			this.m_colReference.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.m_colReference.Width = 310;
			// 
			// m_colEnglish
			// 
			this.m_colEnglish.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.m_colEnglish.DefaultCellStyle = dataGridViewCellStyle2;
			this.m_colEnglish.HeaderText = "_L10N_:MainWindow.QuestionsGrid.EnglishQuestion!English Question";
			this.m_colEnglish.MinimumWidth = 100;
			this.m_colEnglish.Name = "m_colEnglish";
			this.m_colEnglish.ReadOnly = true;
			// 
			// m_colTranslation
			// 
			this.m_colTranslation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.m_colTranslation.HeaderText = "_L10N_:MainWindow.QuestionsGrid.Translation!Translation";
			this.m_colTranslation.MinimumWidth = 100;
			this.m_colTranslation.Name = "m_colTranslation";
			// 
			// m_colEditQuestion
			// 
			this.m_colEditQuestion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.m_colEditQuestion.HeaderText = "_L10N_:MainWindow.QuestionsGrid.Edit!Edit";
			this.m_colEditQuestion.MinimumWidth = 20;
			this.m_colEditQuestion.Name = "m_colEditQuestion";
			this.m_colEditQuestion.Width = 227;
			// 
			// m_colUserTranslated
			// 
			this.m_colUserTranslated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.m_colUserTranslated.HeaderText = "_L10N_:MainWindow.QuestionsGrid.Confirmed!Confirmed";
			this.m_colUserTranslated.Name = "m_colUserTranslated";
			this.m_colUserTranslated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.m_colUserTranslated.Width = 304;
			// 
			// m_colDebugInfo
			// 
			this.m_colDebugInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.m_colDebugInfo.HeaderText = "_L10N_:MainWindow.QuestionsGrid.GeneratedTranslationDetails!Generated Translation" +
    " Details";
			this.m_colDebugInfo.MinimumWidth = 100;
			this.m_colDebugInfo.Name = "m_colDebugInfo";
			this.m_colDebugInfo.ReadOnly = true;
			// 
			// dataGridContextMenu
			// 
			this.dataGridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator7,
            this.mnuExcludeQuestion,
            this.mnuIncludeQuestion,
            this.mnuEditQuestion,
            this.mnuAddQuestion});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.dataGridContextMenu, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.dataGridContextMenu, null);
			this.l10NSharpExtender1.SetLocalizingId(this.dataGridContextMenu, "dataGridContextMenu");
			this.dataGridContextMenu.Name = "dataGridContextMenu";
			this.dataGridContextMenu.Size = new System.Drawing.Size(167, 164);
			this.dataGridContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dataGridContextMenu_Opening);
			// 
			// cutToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.cutToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.cutToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.cutToolStripMenuItem, "MainWindow.dataGridContextMenu.cutToolStripMenuItem");
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.copyToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.copyToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.copyToolStripMenuItem, "MainWindow.dataGridContextMenu.copyToolStripMenuItem");
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pasteToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pasteToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pasteToolStripMenuItem, "MainWindow.dataGridContextMenu.pasteToolStripMenuItem");
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(163, 6);
			// 
			// mnuExcludeQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuExcludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuExcludeQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuExcludeQuestion, "MainWindow.dataGridContextMenu.mnuExcludeQuestion");
			this.mnuExcludeQuestion.Name = "mnuExcludeQuestion";
			this.mnuExcludeQuestion.Size = new System.Drawing.Size(166, 22);
			this.mnuExcludeQuestion.Text = "E&xclude Question";
			this.mnuExcludeQuestion.Click += new System.EventHandler(this.mnuIncludeOrExcludeQuestion_Click);
			// 
			// mnuIncludeQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuIncludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuIncludeQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuIncludeQuestion, "MainWindow.dataGridContextMenu.mnuIncludeQuestion");
			this.mnuIncludeQuestion.Name = "mnuIncludeQuestion";
			this.mnuIncludeQuestion.Size = new System.Drawing.Size(166, 22);
			this.mnuIncludeQuestion.Text = "&Include Question";
			this.mnuIncludeQuestion.Visible = false;
			this.mnuIncludeQuestion.Click += new System.EventHandler(this.mnuIncludeOrExcludeQuestion_Click);
			// 
			// mnuEditQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuEditQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuEditQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuEditQuestion, "MainWindow.dataGridContextMenu.mnuEditQuestion");
			this.mnuEditQuestion.Name = "mnuEditQuestion";
			this.mnuEditQuestion.Size = new System.Drawing.Size(166, 22);
			this.mnuEditQuestion.Text = "&Edit Question...";
			this.mnuEditQuestion.Click += new System.EventHandler(this.mnuEditQuestion_Click);
			// 
			// mnuAddQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAddQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAddQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAddQuestion, "MainWindow.dataGridContextMenu.mnuAddQuestion");
			this.mnuAddQuestion.Name = "mnuAddQuestion";
			this.mnuAddQuestion.Size = new System.Drawing.Size(166, 22);
			this.mnuAddQuestion.Text = "&New Question...";
			this.mnuAddQuestion.Click += new System.EventHandler(this.AddNewQuestion);
			// 
			// m_mainMenu
			// 
			this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuFilter,
            this.mnuView,
            this.mnuAdvanced,
            this.mnuHelp});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_mainMenu, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_mainMenu, "MainWindow.m_mainMenu");
			this.m_mainMenu.Location = new System.Drawing.Point(3, 3);
			this.m_mainMenu.Name = "m_mainMenu";
			this.m_mainMenu.Size = new System.Drawing.Size(792, 24);
			this.m_mainMenu.TabIndex = 15;
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuAutoSave,
            this.mnuReload,
            this.toolStripSeparator2,
            this.mnuGenerate,
            this.mnuProduceScriptureForgeFiles,
            this.generateOutputForArloToolStripMenuItem,
            this.mnuLoadTranslationsFromTextFile,
            this.toolStripSeparator3,
            this.mnuClose});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuFile, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuFile, "MainWindow.Menu.File");
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size(37, 20);
			this.mnuFile.Text = "&File";
			// 
			// mnuSave
			// 
			this.mnuSave.Enabled = false;
			this.mnuSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuSave.Image")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuSave, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuSave, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuSave, "MainWindow.Menu.File.Save");
			this.mnuSave.Name = "mnuSave";
			this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mnuSave.Size = new System.Drawing.Size(277, 22);
			this.mnuSave.Text = "&Save";
			this.mnuSave.Click += new System.EventHandler(this.Save);
			// 
			// mnuAutoSave
			// 
			this.mnuAutoSave.Checked = true;
			this.mnuAutoSave.CheckOnClick = true;
			this.mnuAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAutoSave, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAutoSave, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAutoSave, "MainWindow.File.mnuAutoSave");
			this.mnuAutoSave.Name = "mnuAutoSave";
			this.mnuAutoSave.Size = new System.Drawing.Size(277, 22);
			this.mnuAutoSave.Text = "&Auto Save";
			this.mnuAutoSave.CheckedChanged += new System.EventHandler(this.mnuAutoSave_CheckedChanged);
			// 
			// mnuReload
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuReload, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuReload, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuReload, "MainWindow.Menu.File.Reload");
			this.mnuReload.Name = "mnuReload";
			this.mnuReload.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.mnuReload.Size = new System.Drawing.Size(277, 22);
			this.mnuReload.Text = "&Reload";
			this.mnuReload.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(274, 6);
			// 
			// mnuGenerate
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuGenerate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuGenerate, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuGenerate, "MainWindow.Menu.File.GenerateCheckingScript");
			this.mnuGenerate.Name = "mnuGenerate";
			this.mnuGenerate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.mnuGenerate.Size = new System.Drawing.Size(277, 22);
			this.mnuGenerate.Text = "&Generate Checking Script...";
			this.mnuGenerate.Click += new System.EventHandler(this.mnuGenerate_Click);
			// 
			// mnuProduceScriptureForgeFiles
			// 
			this.mnuProduceScriptureForgeFiles.CheckOnClick = true;
			this.mnuProduceScriptureForgeFiles.Image = global::SIL.Transcelerator.Properties.Resources.sf_logo_medium;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuProduceScriptureForgeFiles, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuProduceScriptureForgeFiles, "Param is \"Scripture Forge\" (product name)");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuProduceScriptureForgeFiles, "MainWindow.Menu.File.ProduceScriptureForgeFiles");
			this.mnuProduceScriptureForgeFiles.Name = "mnuProduceScriptureForgeFiles";
			this.mnuProduceScriptureForgeFiles.Size = new System.Drawing.Size(277, 22);
			this.mnuProduceScriptureForgeFiles.Text = "Produce {0} Files";
			// 
			// generateOutputForArloToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.generateOutputForArloToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.generateOutputForArloToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.generateOutputForArloToolStripMenuItem, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.generateOutputForArloToolStripMenuItem, "MainWindow.Menu.File.GenerateOutputForArlo");
			this.generateOutputForArloToolStripMenuItem.Name = "generateOutputForArloToolStripMenuItem";
			this.generateOutputForArloToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
			this.generateOutputForArloToolStripMenuItem.Text = "Generate output for Arlo...";
			this.generateOutputForArloToolStripMenuItem.Visible = false;
			this.generateOutputForArloToolStripMenuItem.Click += new System.EventHandler(this.generateOutputForArloToolStripMenuItem_Click);
			// 
			// mnuLoadTranslationsFromTextFile
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuLoadTranslationsFromTextFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuLoadTranslationsFromTextFile, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuLoadTranslationsFromTextFile, "MainWindow.Menu.File.LoadTranslationsFromTextFile");
			this.mnuLoadTranslationsFromTextFile.Name = "mnuLoadTranslationsFromTextFile";
			this.mnuLoadTranslationsFromTextFile.Size = new System.Drawing.Size(277, 22);
			this.mnuLoadTranslationsFromTextFile.Text = "Load Translations from Plain Text File...";
			this.mnuLoadTranslationsFromTextFile.Visible = false;
			this.mnuLoadTranslationsFromTextFile.Click += new System.EventHandler(this.mnuLoadTranslationsFromTextFile_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(274, 6);
			// 
			// mnuClose
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuClose, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuClose, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuClose, "MainWindow.Menu.File.Close");
			this.mnuClose.Name = "mnuClose";
			this.mnuClose.Size = new System.Drawing.Size(277, 22);
			this.mnuClose.Text = "&Close";
			this.mnuClose.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// mnuEdit
			// 
			this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCut,
            this.mnuCopy,
            this.mnuPaste,
            this.toolStripSeparator8,
            this.mnuPreviousUntranslatedQuestion,
            this.mnuNextUntranslatedQuestion,
            this.toolStripSeparatorShiftWords,
            this.mnuShiftWordsRight,
            this.mnuShiftWordsLeft});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuEdit, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuEdit, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuEdit, "MainWindow.Menu.Edit");
			this.mnuEdit.Name = "mnuEdit";
			this.mnuEdit.Size = new System.Drawing.Size(39, 20);
			this.mnuEdit.Text = "&Edit";
			this.mnuEdit.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
			// 
			// mnuCut
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuCut, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuCut, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuCut, "MainWindow.Menu.Edit.Cut");
			this.mnuCut.Name = "mnuCut";
			this.mnuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.mnuCut.Size = new System.Drawing.Size(289, 22);
			this.mnuCut.Text = "Cu&t";
			this.mnuCut.Click += new System.EventHandler(this.cutToolStripMenuItem1_Click);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Image = global::SIL.Transcelerator.Properties.Resources.Copy;
			this.mnuCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuCopy, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuCopy, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuCopy, "MainWindow.Menu.Edit.Copy");
			this.mnuCopy.Name = "mnuCopy";
			this.mnuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mnuCopy.Size = new System.Drawing.Size(289, 22);
			this.mnuCopy.Text = "&Copy";
			this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
			// 
			// mnuPaste
			// 
			this.mnuPaste.Image = global::SIL.Transcelerator.Properties.Resources.Paste;
			this.mnuPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuPaste, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuPaste, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuPaste, "MainWindow.Menu.Edit.Paste");
			this.mnuPaste.Name = "mnuPaste";
			this.mnuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.mnuPaste.Size = new System.Drawing.Size(289, 22);
			this.mnuPaste.Text = "&Paste";
			this.mnuPaste.Click += new System.EventHandler(this.mnuPaste_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(286, 6);
			// 
			// mnuPreviousUntranslatedQuestion
			// 
			this.mnuPreviousUntranslatedQuestion.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon;
			this.mnuPreviousUntranslatedQuestion.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuPreviousUntranslatedQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuPreviousUntranslatedQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuPreviousUntranslatedQuestion, "MainWindow.Menu.Edit.PreviousUntranslatedQuestion");
			this.mnuPreviousUntranslatedQuestion.Name = "mnuPreviousUntranslatedQuestion";
			this.mnuPreviousUntranslatedQuestion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
			this.mnuPreviousUntranslatedQuestion.Size = new System.Drawing.Size(289, 22);
			this.mnuPreviousUntranslatedQuestion.Text = "Previous &Untranslated Question";
			this.mnuPreviousUntranslatedQuestion.Click += new System.EventHandler(this.prevUntranslatedQuestionToolStripMenuItem_Click);
			// 
			// mnuNextUntranslatedQuestion
			// 
			this.mnuNextUntranslatedQuestion.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon;
			this.mnuNextUntranslatedQuestion.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuNextUntranslatedQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuNextUntranslatedQuestion, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuNextUntranslatedQuestion, "MainWindow.Menu.Edit.NextUntranslatedQuestion");
			this.mnuNextUntranslatedQuestion.Name = "mnuNextUntranslatedQuestion";
			this.mnuNextUntranslatedQuestion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
			this.mnuNextUntranslatedQuestion.Size = new System.Drawing.Size(289, 22);
			this.mnuNextUntranslatedQuestion.Text = "&Next Untranslated Question";
			this.mnuNextUntranslatedQuestion.Click += new System.EventHandler(this.nextUntranslatedQuestionToolStripMenuItem_Click);
			// 
			// toolStripSeparatorShiftWords
			// 
			this.toolStripSeparatorShiftWords.Name = "toolStripSeparatorShiftWords";
			this.toolStripSeparatorShiftWords.Size = new System.Drawing.Size(286, 6);
			this.toolStripSeparatorShiftWords.Visible = false;
			// 
			// mnuShiftWordsRight
			// 
			this.mnuShiftWordsRight.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Right_icon;
			this.mnuShiftWordsRight.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShiftWordsRight, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShiftWordsRight, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShiftWordsRight, "MainWindow.Menu.Edit.ShiftWordsRight");
			this.mnuShiftWordsRight.Name = "mnuShiftWordsRight";
			this.mnuShiftWordsRight.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Right)));
			this.mnuShiftWordsRight.Size = new System.Drawing.Size(289, 22);
			this.mnuShiftWordsRight.Text = "Shift Word(s) &Right";
			this.mnuShiftWordsRight.Visible = false;
			this.mnuShiftWordsRight.Click += new System.EventHandler(this.HandleShiftWordsMenuClick);
			// 
			// mnuShiftWordsLeft
			// 
			this.mnuShiftWordsLeft.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Left_icon;
			this.mnuShiftWordsLeft.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShiftWordsLeft, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShiftWordsLeft, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShiftWordsLeft, "MainWindow.Menu.Edit.ShiftWordsLeft");
			this.mnuShiftWordsLeft.Name = "mnuShiftWordsLeft";
			this.mnuShiftWordsLeft.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Left)));
			this.mnuShiftWordsLeft.Size = new System.Drawing.Size(289, 22);
			this.mnuShiftWordsLeft.Text = "Shift Word(s) &Left";
			this.mnuShiftWordsLeft.Visible = false;
			this.mnuShiftWordsLeft.Click += new System.EventHandler(this.HandleShiftWordsMenuClick);
			// 
			// mnuFilter
			// 
			this.mnuFilter.CheckOnClick = true;
			this.mnuFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuReferenceRange,
            this.mnuFilterBiblicalTerms,
            this.mnuMatchWholeWords});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuFilter, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuFilter, "MainWindow.Menu.Filter");
			this.mnuFilter.Name = "mnuFilter";
			this.mnuFilter.Size = new System.Drawing.Size(45, 20);
			this.mnuFilter.Text = "&Filter";
			// 
			// mnuReferenceRange
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuReferenceRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuReferenceRange, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuReferenceRange, "MainWindow.Menu.Filter.ReferenceRange");
			this.mnuReferenceRange.Name = "mnuReferenceRange";
			this.mnuReferenceRange.Size = new System.Drawing.Size(307, 22);
			this.mnuReferenceRange.Text = "&Set Reference Range...";
			this.mnuReferenceRange.Click += new System.EventHandler(this.mnuReferenceRange_Click);
			// 
			// mnuFilterBiblicalTerms
			// 
			this.mnuFilterBiblicalTerms.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowAllPhrases,
            this.mnuShowPhrasesWithKtRenderings,
            this.mnuShowPhrasesWithMissingKtRenderings});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuFilterBiblicalTerms, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuFilterBiblicalTerms, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuFilterBiblicalTerms, "MainWindow.Menu.Filter.BiblicalTerms");
			this.mnuFilterBiblicalTerms.Name = "mnuFilterBiblicalTerms";
			this.mnuFilterBiblicalTerms.Size = new System.Drawing.Size(307, 22);
			this.mnuFilterBiblicalTerms.Text = "Biblical &Terms";
			// 
			// mnuShowAllPhrases
			// 
			this.mnuShowAllPhrases.Checked = true;
			this.mnuShowAllPhrases.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowAllPhrases, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowAllPhrases, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowAllPhrases, "MainWindow.mnuShowAllPhrases");
			this.mnuShowAllPhrases.Name = "mnuShowAllPhrases";
			this.mnuShowAllPhrases.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.mnuShowAllPhrases.Size = new System.Drawing.Size(380, 22);
			this.mnuShowAllPhrases.Text = "Show &All (no filter)";
			this.mnuShowAllPhrases.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowAllPhrases.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuShowPhrasesWithKtRenderings
			// 
			this.mnuShowPhrasesWithKtRenderings.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowPhrasesWithKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowPhrasesWithKtRenderings, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowPhrasesWithKtRenderings, "MainWindow.mnuShowPhrasesWithKtRenderings");
			this.mnuShowPhrasesWithKtRenderings.Name = "mnuShowPhrasesWithKtRenderings";
			this.mnuShowPhrasesWithKtRenderings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.mnuShowPhrasesWithKtRenderings.Size = new System.Drawing.Size(380, 22);
			this.mnuShowPhrasesWithKtRenderings.Text = "Show Questions Where All Terms Have &Renderings";
			this.mnuShowPhrasesWithKtRenderings.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowPhrasesWithKtRenderings.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuShowPhrasesWithMissingKtRenderings
			// 
			this.mnuShowPhrasesWithMissingKtRenderings.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowPhrasesWithMissingKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowPhrasesWithMissingKtRenderings, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowPhrasesWithMissingKtRenderings, "MainWindow.mnuShowPhrasesWithMissingKtRenderings");
			this.mnuShowPhrasesWithMissingKtRenderings.Name = "mnuShowPhrasesWithMissingKtRenderings";
			this.mnuShowPhrasesWithMissingKtRenderings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
			this.mnuShowPhrasesWithMissingKtRenderings.Size = new System.Drawing.Size(380, 22);
			this.mnuShowPhrasesWithMissingKtRenderings.Text = "Show Questions with Terms &Missing Renderings";
			this.mnuShowPhrasesWithMissingKtRenderings.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowPhrasesWithMissingKtRenderings.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuMatchWholeWords
			// 
			this.mnuMatchWholeWords.Checked = true;
			this.mnuMatchWholeWords.CheckOnClick = true;
			this.mnuMatchWholeWords.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuMatchWholeWords, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuMatchWholeWords, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuMatchWholeWords, "MainWindow.Menu.Filter.MatchWholeWords");
			this.mnuMatchWholeWords.Name = "mnuMatchWholeWords";
			this.mnuMatchWholeWords.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.mnuMatchWholeWords.Size = new System.Drawing.Size(307, 22);
			this.mnuMatchWholeWords.Text = "Match &Whole Words When Filtering";
			this.mnuMatchWholeWords.CheckedChanged += new System.EventHandler(this.mnuMatchWholeWords_CheckChanged);
			// 
			// mnuView
			// 
			this.mnuView.Checked = true;
			this.mnuView.CheckOnClick = true;
			this.mnuView.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewDebugInfo,
            this.mnuViewEditQuestionColumn,
            this.mnuViewAnswers,
            this.mnuViewToolbar,
            this.mnuViewBiblicalTermsPane,
            this.toolStripSeparator6,
            this.mnuViewExcludedQuestions,
            this.toolStripSeparator9,
            this.mnuDisplayLanguage});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuView, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuView, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuView, "MainWindow.Menu.View");
			this.mnuView.Name = "mnuView";
			this.mnuView.Size = new System.Drawing.Size(44, 20);
			this.mnuView.Text = "&View";
			this.mnuView.CheckedChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// mnuViewEditQuestionColumn
			// 
			this.mnuViewEditQuestionColumn.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewEditQuestionColumn, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewEditQuestionColumn, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewEditQuestionColumn, "MainWindow.UNSQuestionsDialog.editQuestionColumnToolStripMenuItem");
			this.mnuViewEditQuestionColumn.Name = "mnuViewEditQuestionColumn";
			this.mnuViewEditQuestionColumn.Size = new System.Drawing.Size(226, 22);
			this.mnuViewEditQuestionColumn.Text = "Edit &Question Column";
			this.mnuViewEditQuestionColumn.Click += new System.EventHandler(this.mnuViewEditQuestionColumn_CheckedChanged);
			// 
			// mnuViewToolbar
			// 
			this.mnuViewToolbar.Checked = true;
			this.mnuViewToolbar.CheckOnClick = true;
			this.mnuViewToolbar.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewToolbar, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewToolbar, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewToolbar, "MainWindow.Menu.View.Toolbar");
			this.mnuViewToolbar.Name = "mnuViewToolbar";
			this.mnuViewToolbar.Size = new System.Drawing.Size(226, 22);
			this.mnuViewToolbar.Text = "&Toolbar";
			this.mnuViewToolbar.CheckStateChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// mnuViewBiblicalTermsPane
			// 
			this.mnuViewBiblicalTermsPane.Checked = true;
			this.mnuViewBiblicalTermsPane.CheckOnClick = true;
			this.mnuViewBiblicalTermsPane.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewBiblicalTermsPane, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewBiblicalTermsPane, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewBiblicalTermsPane, "MainWindow.Menu.View.BiblicalTermsPane");
			this.mnuViewBiblicalTermsPane.Name = "mnuViewBiblicalTermsPane";
			this.mnuViewBiblicalTermsPane.Size = new System.Drawing.Size(226, 22);
			this.mnuViewBiblicalTermsPane.Text = "&Biblical Terms Pane";
			this.mnuViewBiblicalTermsPane.CheckedChanged += new System.EventHandler(this.mnuViewBiblicalTermsPane_CheckedChanged);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(223, 6);
			// 
			// mnuViewExcludedQuestions
			// 
			this.mnuViewExcludedQuestions.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewExcludedQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewExcludedQuestions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewExcludedQuestions, "MainWindow.Menu.View.ExcludedQuestions");
			this.mnuViewExcludedQuestions.Name = "mnuViewExcludedQuestions";
			this.mnuViewExcludedQuestions.Size = new System.Drawing.Size(226, 22);
			this.mnuViewExcludedQuestions.Text = "&Excluded Questions";
			this.mnuViewExcludedQuestions.Click += new System.EventHandler(this.ApplyFilter);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(223, 6);
			// 
			// mnuDisplayLanguage
			// 
			this.mnuDisplayLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.en_ToolStripMenuItem,
            this.toolStripSeparator10,
            this.toolStripMenuItemMoreLanguages});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuDisplayLanguage, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuDisplayLanguage, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuDisplayLanguage, "MainWindow.Menu.View.DisplayLanguage");
			this.mnuDisplayLanguage.Name = "mnuDisplayLanguage";
			this.mnuDisplayLanguage.Size = new System.Drawing.Size(226, 22);
			this.mnuDisplayLanguage.Text = "&Display Language";
			// 
			// en_ToolStripMenuItem
			// 
			this.en_ToolStripMenuItem.Checked = true;
			this.en_ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.en_ToolStripMenuItem, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.en_ToolStripMenuItem, "MainWindow.en_ToolStripMenuItem");
			this.en_ToolStripMenuItem.Name = "en_ToolStripMenuItem";
			this.en_ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.en_ToolStripMenuItem.Tag = "en-US";
			this.en_ToolStripMenuItem.Text = "American English";
			this.en_ToolStripMenuItem.Click += new System.EventHandler(this.HandleDisplayLanguageSelected);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(163, 6);
			// 
			// toolStripMenuItemMoreLanguages
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripMenuItemMoreLanguages, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripMenuItemMoreLanguages, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripMenuItemMoreLanguages, "MainWindow.UNSQuestionsDialog.toolStripMenuItemMoreLanguages");
			this.toolStripMenuItemMoreLanguages.Name = "toolStripMenuItemMoreLanguages";
			this.toolStripMenuItemMoreLanguages.Size = new System.Drawing.Size(166, 22);
			this.toolStripMenuItemMoreLanguages.Text = "&More...";
			this.toolStripMenuItemMoreLanguages.Click += new System.EventHandler(this.toolStripMenuItemMoreLanguages_Click);
			// 
			// mnuAdvanced
			// 
			this.mnuAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPhraseSubstitutions,
            this.mnuBiblicalTermsRenderingSelectionRules});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAdvanced, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAdvanced, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAdvanced, "MainWindow.Menu.Advanced");
			this.mnuAdvanced.Name = "mnuAdvanced";
			this.mnuAdvanced.Size = new System.Drawing.Size(72, 20);
			this.mnuAdvanced.Text = "&Advanced";
			// 
			// mnuPhraseSubstitutions
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuPhraseSubstitutions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuPhraseSubstitutions, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuPhraseSubstitutions, "MainWindow.Menu.Advanced.PreprocessingQuestionAdjustments");
			this.mnuPhraseSubstitutions.Name = "mnuPhraseSubstitutions";
			this.mnuPhraseSubstitutions.Size = new System.Drawing.Size(294, 22);
			this.mnuPhraseSubstitutions.Text = "Preprocessing &Question Adjustments...";
			this.mnuPhraseSubstitutions.Click += new System.EventHandler(this.phraseSubstitutionsToolStripMenuItem_Click);
			// 
			// mnuBiblicalTermsRenderingSelectionRules
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuBiblicalTermsRenderingSelectionRules, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuBiblicalTermsRenderingSelectionRules, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuBiblicalTermsRenderingSelectionRules, "MainWindow.Menu.Advanced.BiblicalTermsRenderingSelectionRules");
			this.mnuBiblicalTermsRenderingSelectionRules.Name = "mnuBiblicalTermsRenderingSelectionRules";
			this.mnuBiblicalTermsRenderingSelectionRules.Size = new System.Drawing.Size(294, 22);
			this.mnuBiblicalTermsRenderingSelectionRules.Text = "Biblical Terms &Rendering Selection Rules...";
			this.mnuBiblicalTermsRenderingSelectionRules.Click += new System.EventHandler(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseTopicsToolStripMenuItem,
            this.mnuHelpAbout});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuHelp, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuHelp, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuHelp, "MainWindow.Menu.Help");
			this.mnuHelp.Name = "mnuHelp";
			this.mnuHelp.Size = new System.Drawing.Size(44, 20);
			this.mnuHelp.Text = "&Help";
			// 
			// browseTopicsToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.browseTopicsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.browseTopicsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.browseTopicsToolStripMenuItem, "MainWindow.UNSQuestionsDialog.browseTopicsToolStripMenuItem");
			this.browseTopicsToolStripMenuItem.Name = "browseTopicsToolStripMenuItem";
			this.browseTopicsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.browseTopicsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.browseTopicsToolStripMenuItem.Text = "Browse Topics...";
			this.browseTopicsToolStripMenuItem.Click += new System.EventHandler(this.browseTopicsToolStripMenuItem_Click);
			// 
			// mnuHelpAbout
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuHelpAbout, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuHelpAbout, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.mnuHelpAbout, "MainWindow.Menu.Help.About");
			this.mnuHelpAbout.Name = "mnuHelpAbout";
			this.mnuHelpAbout.Size = new System.Drawing.Size(183, 22);
			this.mnuHelpAbout.Text = "&About Transcelerator";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripSeparator1,
            this.toolStripLabelQuestionFilter,
            this.txtFilterByPart,
            this.toolStripSeparator4,
            this.btnSendScrReferences,
            this.btnReceiveScrReferences,
            this.lblFilterIndicator,
            toolStripSeparator5,
            this.lblRemainingWork});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStrip1, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.toolStrip1, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStrip1, "MainWindow.toolStrip1");
			this.toolStrip1.Location = new System.Drawing.Point(3, 27);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(792, 25);
			this.toolStrip1.TabIndex = 16;
			// 
			// btnSave
			// 
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSave.Enabled = false;
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnSave, "Save (Ctrl+S)");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnSave, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnSave, "MainWindow.btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(23, 22);
			this.btnSave.Click += new System.EventHandler(this.Save);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabelQuestionFilter
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripLabelQuestionFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripLabelQuestionFilter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripLabelQuestionFilter, "MainWindow.toolStripLabelQuestionFilter");
			this.toolStripLabelQuestionFilter.Name = "toolStripLabelQuestionFilter";
			this.toolStripLabelQuestionFilter.Size = new System.Drawing.Size(87, 22);
			this.toolStripLabelQuestionFilter.Text = "Question Filter:";
			// 
			// txtFilterByPart
			// 
			this.txtFilterByPart.AcceptsReturn = true;
			this.txtFilterByPart.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.txtFilterByPart, "Type an English word or phrase to filter the list of questions.");
			this.l10NSharpExtender1.SetLocalizationComment(this.txtFilterByPart, null);
			this.l10NSharpExtender1.SetLocalizingId(this.txtFilterByPart, "MainWindow.txtFilterByPart");
			this.txtFilterByPart.Name = "txtFilterByPart";
			this.txtFilterByPart.Size = new System.Drawing.Size(200, 25);
			this.txtFilterByPart.Enter += new System.EventHandler(this.txtFilterByPart_Enter);
			this.txtFilterByPart.Leave += new System.EventHandler(this.txtFilterByPart_Leave);
			this.txtFilterByPart.TextChanged += new System.EventHandler(this.ApplyFilter);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// btnSendScrReferences
			// 
			this.btnSendScrReferences.Checked = true;
			this.btnSendScrReferences.CheckOnClick = true;
			this.btnSendScrReferences.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnSendScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSendScrReferences.Image = ((System.Drawing.Image)(resources.GetObject("btnSendScrReferences.Image")));
			this.btnSendScrReferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnSendScrReferences, "Send Scripture References");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnSendScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnSendScrReferences, "MainWindow.btnSendScrReferences");
			this.btnSendScrReferences.Name = "btnSendScrReferences";
			this.btnSendScrReferences.Size = new System.Drawing.Size(23, 22);
			this.btnSendScrReferences.Text = "Send Scripture References";
			this.btnSendScrReferences.CheckStateChanged += new System.EventHandler(this.btnSendScrReferences_CheckStateChanged);
			// 
			// btnReceiveScrReferences
			// 
			this.btnReceiveScrReferences.CheckOnClick = true;
			this.btnReceiveScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnReceiveScrReferences.Image = ((System.Drawing.Image)(resources.GetObject("btnReceiveScrReferences.Image")));
			this.btnReceiveScrReferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnReceiveScrReferences, "Receive Scripture References");
			this.l10NSharpExtender1.SetLocalizationComment(this.btnReceiveScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnReceiveScrReferences, "MainWindow.btnReceiveScrReferences");
			this.btnReceiveScrReferences.Name = "btnReceiveScrReferences";
			this.btnReceiveScrReferences.Size = new System.Drawing.Size(23, 22);
			this.btnReceiveScrReferences.Text = "Receive Scripture References";
			this.btnReceiveScrReferences.Click += new System.EventHandler(this.btnReceiveScrReferences_CheckStateChanged);
			// 
			// lblFilterIndicator
			// 
			this.lblFilterIndicator.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.lblFilterIndicator.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblFilterIndicator.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblFilterIndicator, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblFilterIndicator, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblFilterIndicator, "MainWindow.lblFilterIndicator");
			this.lblFilterIndicator.Name = "lblFilterIndicator";
			this.lblFilterIndicator.Size = new System.Drawing.Size(59, 22);
			this.lblFilterIndicator.Text = "Unfiltered";
			// 
			// lblRemainingWork
			// 
			this.lblRemainingWork.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblRemainingWork, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblRemainingWork, "Parameters are numbers: untranslated/total");
			this.l10NSharpExtender1.SetLocalizingId(this.lblRemainingWork, "MainWindow.lblRemainingWork");
			this.lblRemainingWork.Name = "lblRemainingWork";
			this.lblRemainingWork.Size = new System.Drawing.Size(169, 22);
			this.lblRemainingWork.Text = "Untranslated Questions: {0}/{1}";
			// 
			// m_biblicalTermsPane
			// 
			this.m_biblicalTermsPane.AutoScroll = true;
			this.m_biblicalTermsPane.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_biblicalTermsPane.ColumnCount = 1;
			this.m_biblicalTermsPane.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_biblicalTermsPane.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_biblicalTermsPane.Location = new System.Drawing.Point(3, 282);
			this.m_biblicalTermsPane.MinimumSize = new System.Drawing.Size(100, 40);
			this.m_biblicalTermsPane.Name = "m_biblicalTermsPane";
			this.m_biblicalTermsPane.RowCount = 1;
			this.m_biblicalTermsPane.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_biblicalTermsPane.Size = new System.Drawing.Size(792, 117);
			this.m_biblicalTermsPane.TabIndex = 17;
			this.m_biblicalTermsPane.Resize += new System.EventHandler(this.m_biblicalTermsPane_Resize);
			// 
			// m_lblAnswerLabel
			// 
			this.m_lblAnswerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAnswerLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAnswerLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAnswerLabel, "MainWindow.m_lblAnswerLabel");
			this.m_lblAnswerLabel.Location = new System.Drawing.Point(3, 0);
			this.m_lblAnswerLabel.Name = "m_lblAnswerLabel";
			this.m_lblAnswerLabel.Padding = new System.Windows.Forms.Padding(1, 3, 1, 3);
			this.m_lblAnswerLabel.Size = new System.Drawing.Size(67, 19);
			this.m_lblAnswerLabel.TabIndex = 0;
			this.m_lblAnswerLabel.Text = "Answer:";
			// 
			// m_lblAnswers
			// 
			this.m_lblAnswers.AutoSize = true;
			this.m_lblAnswers.BackColor = System.Drawing.SystemColors.Control;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAnswers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAnswers, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblAnswers, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAnswers, "MainWindow.m_lblAnswers");
			this.m_lblAnswers.Location = new System.Drawing.Point(76, 0);
			this.m_lblAnswers.Name = "m_lblAnswers";
			this.m_lblAnswers.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.m_lblAnswers.Size = new System.Drawing.Size(14, 19);
			this.m_lblAnswers.TabIndex = 2;
			this.m_lblAnswers.Text = "#";
			// 
			// m_lblCommentLabel
			// 
			this.m_lblCommentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCommentLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCommentLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblCommentLabel, "MainWindow.m_lblCommentLabel");
			this.m_lblCommentLabel.Location = new System.Drawing.Point(3, 19);
			this.m_lblCommentLabel.Name = "m_lblCommentLabel";
			this.m_lblCommentLabel.Padding = new System.Windows.Forms.Padding(1, 3, 1, 3);
			this.m_lblCommentLabel.Size = new System.Drawing.Size(67, 19);
			this.m_lblCommentLabel.TabIndex = 1;
			this.m_lblCommentLabel.Text = "Comment:";
			// 
			// m_lblComments
			// 
			this.m_lblComments.AutoSize = true;
			this.m_lblComments.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblComments, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblComments, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this.m_lblComments, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblComments, "MainWindow.m_lblComments");
			this.m_lblComments.Location = new System.Drawing.Point(76, 19);
			this.m_lblComments.Name = "m_lblComments";
			this.m_lblComments.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.m_lblComments.Size = new System.Drawing.Size(14, 19);
			this.m_lblComments.TabIndex = 3;
			this.m_lblComments.Text = "#";
			// 
			// m_pnlAnswersAndComments
			// 
			this.m_pnlAnswersAndComments.AutoSize = true;
			this.m_pnlAnswersAndComments.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_pnlAnswersAndComments.ColumnCount = 2;
			this.m_pnlAnswersAndComments.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.m_pnlAnswersAndComments.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblComments, 1, 1);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblAnswers, 1, 0);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblAnswerLabel, 0, 0);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblCommentLabel, 0, 1);
			this.m_pnlAnswersAndComments.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_pnlAnswersAndComments.Location = new System.Drawing.Point(3, 399);
			this.m_pnlAnswersAndComments.Name = "m_pnlAnswersAndComments";
			this.m_pnlAnswersAndComments.RowCount = 2;
			this.m_pnlAnswersAndComments.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_pnlAnswersAndComments.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.m_pnlAnswersAndComments.Size = new System.Drawing.Size(792, 38);
			this.m_pnlAnswersAndComments.TabIndex = 19;
			this.m_pnlAnswersAndComments.Visible = false;
			this.m_pnlAnswersAndComments.VisibleChanged += new System.EventHandler(this.LoadAnswersAndCommentsIfShowing);
			// 
			// m_hSplitter
			// 
			this.m_hSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.m_hSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_hSplitter.Location = new System.Drawing.Point(3, 279);
			this.m_hSplitter.Name = "m_hSplitter";
			this.m_hSplitter.Size = new System.Drawing.Size(792, 3);
			this.m_hSplitter.TabIndex = 20;
			this.m_hSplitter.TabStop = false;
			this.m_hSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.m_hSplitter_SplitterMoving);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = "MainWindow";
			// 
			// UNSQuestionsDialog
			// 
			this.AccessibleName = "Transcelerator Main Window";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(798, 440);
			this.Controls.Add(this.dataGridUns);
			this.Controls.Add(this.m_hSplitter);
			this.Controls.Add(this.m_biblicalTermsPane);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.m_mainMenu);
			this.Controls.Add(this.m_pnlAnswersAndComments);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizationPriority(this, L10NSharp.LocalizationPriority.NotLocalizable);
			this.l10NSharpExtender1.SetLocalizingId(this, "MainWindow.WindowTitle");
			this.MainMenuStrip = this.m_mainMenu;
			this.MinimumSize = new System.Drawing.Size(180, 150);
			this.Name = "UNSQuestionsDialog";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "{0} - Transcelerator";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.browseTopicsToolStripMenuItem_Click);
			this.Activated += new System.EventHandler(this.UNSQuestionsDialog_Activated);
			this.Resize += new System.EventHandler(this.UNSQuestionsDialog_Resize);
			((System.ComponentModel.ISupportInitialize)(this.dataGridUns)).EndInit();
			this.dataGridContextMenu.ResumeLayout(false);
			this.m_mainMenu.ResumeLayout(false);
			this.m_mainMenu.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.m_pnlAnswersAndComments.ResumeLayout(false);
			this.m_pnlAnswersAndComments.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private DataGridView dataGridUns;
		private MenuStrip m_mainMenu;
		private ToolStripMenuItem mnuFile;
		private ToolStripMenuItem mnuSave;
		private ToolStripMenuItem mnuReload;
		private ToolStripMenuItem mnuClose;
		private ToolStripMenuItem mnuFilter;
		private ToolStripMenuItem mnuFilterBiblicalTerms;
		private ToolStripMenuItem mnuShowAllPhrases;
		private ToolStripMenuItem mnuShowPhrasesWithKtRenderings;
		private ToolStripMenuItem mnuShowPhrasesWithMissingKtRenderings;
		private ToolStripMenuItem mnuView;
		private ToolStrip toolStrip1;
		private ToolStripLabel toolStripLabelQuestionFilter;
		private ToolStripTextBox txtFilterByPart;
		private ToolStripMenuItem mnuMatchWholeWords;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mnuGenerate;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem mnuViewToolbar;
		private ToolStripMenuItem mnuAutoSave;
		private ToolStripMenuItem mnuAdvanced;
		private ToolStripMenuItem mnuPhraseSubstitutions;
		private ToolStripButton btnSave;
		private ToolStripMenuItem mnuReferenceRange;
		private TableLayoutPanel m_biblicalTermsPane;
		private ToolStripMenuItem mnuViewBiblicalTermsPane;
		private ToolStripButton btnSendScrReferences;
		private ToolStripButton btnReceiveScrReferences;
		private ToolStripSeparator toolStripSeparator4;
		private Label m_lblAnswerLabel;
		private Label m_lblCommentLabel;
		private Label m_lblAnswers;
		private Label m_lblComments;
		private TableLayoutPanel m_pnlAnswersAndComments;
		private ToolStripMenuItem mnuViewAnswers;
		private ToolStripMenuItem mnuHelp;
		private ToolStripMenuItem mnuHelpAbout;
		private ToolStripMenuItem mnuBiblicalTermsRenderingSelectionRules;
		private ToolStripLabel lblFilterIndicator;
		private ToolStripLabel lblRemainingWork;
		private ToolStripMenuItem mnuViewExcludedQuestions;
		private ToolStripSeparator toolStripSeparator6;
		private ContextMenuStrip dataGridContextMenu;
		private ToolStripMenuItem mnuExcludeQuestion;
		private ToolStripMenuItem mnuIncludeQuestion;
		private ToolStripMenuItem mnuEditQuestion;
		private ToolStripMenuItem mnuAddQuestion;
		private ToolStripMenuItem mnuEdit;
		private ToolStripMenuItem mnuCopy;
		private ToolStripMenuItem mnuPaste;
        private Splitter m_hSplitter;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem mnuCut;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem mnuNextUntranslatedQuestion;
        private ToolStripMenuItem mnuPreviousUntranslatedQuestion;
		private ToolStripMenuItem generateOutputForArloToolStripMenuItem;
		private ToolStripMenuItem mnuLoadTranslationsFromTextFile;
		private ToolStripMenuItem mnuShiftWordsRight;
		private ToolStripSeparator toolStripSeparatorShiftWords;
		private ToolStripMenuItem mnuShiftWordsLeft;
		private ToolStripSeparator toolStripSeparator9;
		private ToolStripMenuItem mnuDisplayLanguage;
		private ToolStripMenuItem en_ToolStripMenuItem;
		private ToolStripMenuItem mnuProduceScriptureForgeFiles;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private ToolStripMenuItem mnuViewDebugInfo;
		private ToolStripSeparator toolStripSeparator10;
		private ToolStripMenuItem browseTopicsToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItemMoreLanguages;
		private ToolStripMenuItem mnuViewEditQuestionColumn;
		private DataGridViewTextBoxColumn m_colReference;
		private DataGridViewTextBoxColumn m_colEnglish;
		private DataGridViewTextBoxColumn m_colTranslation;
		private DataGridViewImageColumn m_colEditQuestion;
		private DataGridViewCheckBoxColumn m_colUserTranslated;
		private DataGridViewTextBoxColumn m_colDebugInfo;
	}
}