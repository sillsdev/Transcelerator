// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.   
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: UNSQuestionsDialog.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

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
			if (disposing && (components != null))
			{
				components.Dispose();
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
			System.Windows.Forms.ToolStripMenuItem mnuViewDebugInfo;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UNSQuestionsDialog));
			System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.mnuViewAnswers = new System.Windows.Forms.ToolStripMenuItem();
			this.dataGridUns = new System.Windows.Forms.DataGridView();
			this.m_colReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_colEnglish = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAutoSave = new System.Windows.Forms.ToolStripMenuItem();
			this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuGenerate = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuProduceScriptureForgeFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.generateOutputForArloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuLoadTranslationsFromTextFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.previousUntranslatedQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nextUntranslatedQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorShiftWords = new System.Windows.Forms.ToolStripSeparator();
			this.mnuShiftWordsRight = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShiftWordsLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuReferenceRange = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuKtFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowAllPhrases = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowPhrasesWithKtRenderings = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowPhrasesWithMissingKtRenderings = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuMatchWholeWords = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewBiblicalTermsPane = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuViewExcludedQuestions = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.displayLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.en_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.phraseSubstitutionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.biblicalTermsRenderingSelectionRulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			mnuViewDebugInfo = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.dataGridUns)).BeginInit();
			this.dataGridContextMenu.SuspendLayout();
			this.m_mainMenu.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.m_pnlAnswersAndComments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// mnuViewDebugInfo
			// 
			mnuViewDebugInfo.Checked = true;
			mnuViewDebugInfo.CheckOnClick = true;
			mnuViewDebugInfo.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(mnuViewDebugInfo, null);
			this.l10NSharpExtender1.SetLocalizationComment(mnuViewDebugInfo, null);
			this.l10NSharpExtender1.SetLocalizingId(mnuViewDebugInfo, "MainWindow.mnuViewDebugInfo");
			mnuViewDebugInfo.Name = "mnuViewDebugInfo";
			resources.ApplyResources(mnuViewDebugInfo, "mnuViewDebugInfo");
			mnuViewDebugInfo.CheckedChanged += new System.EventHandler(this.mnuViewDebugInfo_CheckedChanged);
			// 
			// toolStripSeparator5
			// 
			toolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
			// 
			// mnuViewAnswers
			// 
			this.mnuViewAnswers.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewAnswers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewAnswers, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewAnswers, "MainWindow.mnuViewAnswers");
			this.mnuViewAnswers.Name = "mnuViewAnswers";
			resources.ApplyResources(this.mnuViewAnswers, "mnuViewAnswers");
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
			resources.ApplyResources(this.dataGridUns, "dataGridUns");
			this.dataGridUns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.dataGridUns, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.dataGridUns, null);
			this.l10NSharpExtender1.SetLocalizingId(this.dataGridUns, "MainWindow.dataGridUns");
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
			this.dataGridUns.RowContextMenuStripNeeded += new System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler(this.dataGridUns_RowContextMenuStripNeeded);
			this.dataGridUns.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_RowEnter);
			this.dataGridUns.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridUns_RowLeave);
			this.dataGridUns.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridUns_RowPrePaint);
			this.dataGridUns.Resize += new System.EventHandler(this.dataGridUns_Resize);
			// 
			// m_colReference
			// 
			this.m_colReference.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			resources.ApplyResources(this.m_colReference, "m_colReference");
			this.m_colReference.Name = "m_colReference";
			this.m_colReference.ReadOnly = true;
			this.m_colReference.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.m_colReference.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// m_colEnglish
			// 
			this.m_colEnglish.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.m_colEnglish.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.m_colEnglish, "m_colEnglish");
			this.m_colEnglish.Name = "m_colEnglish";
			this.m_colEnglish.ReadOnly = true;
			// 
			// m_colTranslation
			// 
			this.m_colTranslation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.m_colTranslation, "m_colTranslation");
			this.m_colTranslation.Name = "m_colTranslation";
			// 
			// m_colUserTranslated
			// 
			this.m_colUserTranslated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			resources.ApplyResources(this.m_colUserTranslated, "m_colUserTranslated");
			this.m_colUserTranslated.Name = "m_colUserTranslated";
			this.m_colUserTranslated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// m_colDebugInfo
			// 
			this.m_colDebugInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.m_colDebugInfo, "m_colDebugInfo");
			this.m_colDebugInfo.Name = "m_colDebugInfo";
			this.m_colDebugInfo.ReadOnly = true;
			this.m_colDebugInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
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
			resources.ApplyResources(this.dataGridContextMenu, "dataGridContextMenu");
			this.dataGridContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dataGridContextMenu_Opening);
			// 
			// cutToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.cutToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.cutToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.cutToolStripMenuItem, ".cutToolStripMenuItem");
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.copyToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.copyToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.copyToolStripMenuItem, ".copyToolStripMenuItem");
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.pasteToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.pasteToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.pasteToolStripMenuItem, ".pasteToolStripMenuItem");
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
			// 
			// mnuExcludeQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuExcludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuExcludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuExcludeQuestion, ".mnuExcludeQuestion");
			this.mnuExcludeQuestion.Name = "mnuExcludeQuestion";
			resources.ApplyResources(this.mnuExcludeQuestion, "mnuExcludeQuestion");
			this.mnuExcludeQuestion.Click += new System.EventHandler(this.mnuIncludeOrExcludeQuestion_Click);
			// 
			// mnuIncludeQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuIncludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuIncludeQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuIncludeQuestion, ".mnuIncludeQuestion");
			this.mnuIncludeQuestion.Name = "mnuIncludeQuestion";
			resources.ApplyResources(this.mnuIncludeQuestion, "mnuIncludeQuestion");
			this.mnuIncludeQuestion.Click += new System.EventHandler(this.mnuIncludeOrExcludeQuestion_Click);
			// 
			// mnuEditQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuEditQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuEditQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuEditQuestion, ".mnuEditQuestion");
			this.mnuEditQuestion.Name = "mnuEditQuestion";
			resources.ApplyResources(this.mnuEditQuestion, "mnuEditQuestion");
			this.mnuEditQuestion.Click += new System.EventHandler(this.mnuEditQuestion_Click);
			// 
			// mnuAddQuestion
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAddQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAddQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAddQuestion, ".mnuAddQuestion");
			this.mnuAddQuestion.Name = "mnuAddQuestion";
			resources.ApplyResources(this.mnuAddQuestion, "mnuAddQuestion");
			this.mnuAddQuestion.Click += new System.EventHandler(this.AddNewQuestion);
			// 
			// m_mainMenu
			// 
			this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_mainMenu, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_mainMenu, "MainWindow.m_mainMenu");
			resources.ApplyResources(this.m_mainMenu, "m_mainMenu");
			this.m_mainMenu.Name = "m_mainMenu";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.mnuAutoSave,
            this.reloadToolStripMenuItem,
            this.toolStripSeparator2,
            this.mnuGenerate,
            this.mnuProduceScriptureForgeFiles,
            this.generateOutputForArloToolStripMenuItem,
            this.mnuLoadTranslationsFromTextFile,
            this.toolStripSeparator3,
            this.closeToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.fileToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.fileToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.fileToolStripMenuItem, "MainWindow.fileToolStripMenuItem");
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			// 
			// saveToolStripMenuItem
			// 
			resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.saveToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.saveToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.saveToolStripMenuItem, "MainWindow.saveToolStripMenuItem");
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.Save);
			// 
			// mnuAutoSave
			// 
			this.mnuAutoSave.Checked = true;
			this.mnuAutoSave.CheckOnClick = true;
			this.mnuAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuAutoSave, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuAutoSave, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuAutoSave, "MainWindow.mnuAutoSave");
			this.mnuAutoSave.Name = "mnuAutoSave";
			resources.ApplyResources(this.mnuAutoSave, "mnuAutoSave");
			this.mnuAutoSave.CheckedChanged += new System.EventHandler(this.mnuAutoSave_CheckedChanged);
			// 
			// reloadToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.reloadToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.reloadToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.reloadToolStripMenuItem, "MainWindow.reloadToolStripMenuItem");
			this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
			resources.ApplyResources(this.reloadToolStripMenuItem, "reloadToolStripMenuItem");
			this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// mnuGenerate
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuGenerate, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuGenerate, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuGenerate, "MainWindow.mnuGenerate");
			this.mnuGenerate.Name = "mnuGenerate";
			resources.ApplyResources(this.mnuGenerate, "mnuGenerate");
			this.mnuGenerate.Click += new System.EventHandler(this.mnuGenerate_Click);
			// 
			// mnuProduceScriptureForgeFiles
			// 
			this.mnuProduceScriptureForgeFiles.CheckOnClick = true;
			this.mnuProduceScriptureForgeFiles.Image = global::SIL.Transcelerator.Properties.Resources.sf_logo_medium;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuProduceScriptureForgeFiles, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuProduceScriptureForgeFiles, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuProduceScriptureForgeFiles, "MainWindow.mnuProduceScriptureForgeFiles");
			this.mnuProduceScriptureForgeFiles.Name = "mnuProduceScriptureForgeFiles";
			resources.ApplyResources(this.mnuProduceScriptureForgeFiles, "mnuProduceScriptureForgeFiles");
			this.mnuProduceScriptureForgeFiles.CheckedChanged += new System.EventHandler(this.mnuProduceScriptureForgeFiles_CheckedChanged);
			this.mnuProduceScriptureForgeFiles.Click += new System.EventHandler(this.mnuProduceScriptureForgeFiles_Clicked);
			// 
			// generateOutputForArloToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.generateOutputForArloToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.generateOutputForArloToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.generateOutputForArloToolStripMenuItem, "MainWindow.generateOutputForArloToolStripMenuItem");
			this.generateOutputForArloToolStripMenuItem.Name = "generateOutputForArloToolStripMenuItem";
			resources.ApplyResources(this.generateOutputForArloToolStripMenuItem, "generateOutputForArloToolStripMenuItem");
			this.generateOutputForArloToolStripMenuItem.Click += new System.EventHandler(this.generateOutputForArloToolStripMenuItem_Click);
			// 
			// mnuLoadTranslationsFromTextFile
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuLoadTranslationsFromTextFile, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuLoadTranslationsFromTextFile, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuLoadTranslationsFromTextFile, "MainWindow.mnuLoadTranslationsFromTextFile");
			this.mnuLoadTranslationsFromTextFile.Name = "mnuLoadTranslationsFromTextFile";
			resources.ApplyResources(this.mnuLoadTranslationsFromTextFile, "mnuLoadTranslationsFromTextFile");
			this.mnuLoadTranslationsFromTextFile.Click += new System.EventHandler(this.mnuLoadTranslationsFromTextFile_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// closeToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.closeToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.closeToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.closeToolStripMenuItem, "MainWindow.closeToolStripMenuItem");
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem1,
            this.mnuCopy,
            this.mnuPaste,
            this.toolStripSeparator8,
            this.previousUntranslatedQuestionToolStripMenuItem,
            this.nextUntranslatedQuestionToolStripMenuItem,
            this.toolStripSeparatorShiftWords,
            this.mnuShiftWordsRight,
            this.mnuShiftWordsLeft});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.editToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.editToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.editToolStripMenuItem, "MainWindow.editToolStripMenuItem");
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
			// 
			// cutToolStripMenuItem1
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.cutToolStripMenuItem1, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.cutToolStripMenuItem1, null);
			this.l10NSharpExtender1.SetLocalizingId(this.cutToolStripMenuItem1, "MainWindow.cutToolStripMenuItem1");
			this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
			resources.ApplyResources(this.cutToolStripMenuItem1, "cutToolStripMenuItem1");
			this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem1_Click);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Image = global::SIL.Transcelerator.Properties.Resources.Copy;
			resources.ApplyResources(this.mnuCopy, "mnuCopy");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuCopy, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuCopy, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuCopy, "MainWindow.mnuCopy");
			this.mnuCopy.Name = "mnuCopy";
			this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
			// 
			// mnuPaste
			// 
			this.mnuPaste.Image = global::SIL.Transcelerator.Properties.Resources.Paste;
			resources.ApplyResources(this.mnuPaste, "mnuPaste");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuPaste, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuPaste, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuPaste, "MainWindow.mnuPaste");
			this.mnuPaste.Name = "mnuPaste";
			this.mnuPaste.Click += new System.EventHandler(this.mnuPaste_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
			// 
			// previousUntranslatedQuestionToolStripMenuItem
			// 
			this.previousUntranslatedQuestionToolStripMenuItem.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Up_icon;
			resources.ApplyResources(this.previousUntranslatedQuestionToolStripMenuItem, "previousUntranslatedQuestionToolStripMenuItem");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.previousUntranslatedQuestionToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.previousUntranslatedQuestionToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.previousUntranslatedQuestionToolStripMenuItem, "MainWindow.previousUntranslatedQuestionToolStripMenuItem");
			this.previousUntranslatedQuestionToolStripMenuItem.Name = "previousUntranslatedQuestionToolStripMenuItem";
			this.previousUntranslatedQuestionToolStripMenuItem.Click += new System.EventHandler(this.prevUntranslatedQuestionToolStripMenuItem_Click);
			// 
			// nextUntranslatedQuestionToolStripMenuItem
			// 
			this.nextUntranslatedQuestionToolStripMenuItem.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Down_icon;
			resources.ApplyResources(this.nextUntranslatedQuestionToolStripMenuItem, "nextUntranslatedQuestionToolStripMenuItem");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.nextUntranslatedQuestionToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.nextUntranslatedQuestionToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.nextUntranslatedQuestionToolStripMenuItem, "MainWindow.nextUntranslatedQuestionToolStripMenuItem");
			this.nextUntranslatedQuestionToolStripMenuItem.Name = "nextUntranslatedQuestionToolStripMenuItem";
			this.nextUntranslatedQuestionToolStripMenuItem.Click += new System.EventHandler(this.nextUntranslatedQuestionToolStripMenuItem_Click);
			// 
			// toolStripSeparatorShiftWords
			// 
			this.toolStripSeparatorShiftWords.Name = "toolStripSeparatorShiftWords";
			resources.ApplyResources(this.toolStripSeparatorShiftWords, "toolStripSeparatorShiftWords");
			// 
			// mnuShiftWordsRight
			// 
			this.mnuShiftWordsRight.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Right_icon;
			resources.ApplyResources(this.mnuShiftWordsRight, "mnuShiftWordsRight");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShiftWordsRight, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShiftWordsRight, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShiftWordsRight, "MainWindow.mnuShiftWordsRight");
			this.mnuShiftWordsRight.Name = "mnuShiftWordsRight";
			this.mnuShiftWordsRight.Click += new System.EventHandler(this.HandleShiftWordsMenuClick);
			// 
			// mnuShiftWordsLeft
			// 
			this.mnuShiftWordsLeft.Image = global::SIL.Transcelerator.Properties.Resources.Arrow_Left_icon;
			resources.ApplyResources(this.mnuShiftWordsLeft, "mnuShiftWordsLeft");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShiftWordsLeft, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShiftWordsLeft, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShiftWordsLeft, "MainWindow.mnuShiftWordsLeft");
			this.mnuShiftWordsLeft.Name = "mnuShiftWordsLeft";
			this.mnuShiftWordsLeft.Click += new System.EventHandler(this.HandleShiftWordsMenuClick);
			// 
			// filterToolStripMenuItem
			// 
			this.filterToolStripMenuItem.CheckOnClick = true;
			this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuReferenceRange,
            this.mnuKtFilter,
            this.mnuMatchWholeWords});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.filterToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.filterToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.filterToolStripMenuItem, "MainWindow.filterToolStripMenuItem");
			this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
			resources.ApplyResources(this.filterToolStripMenuItem, "filterToolStripMenuItem");
			// 
			// mnuReferenceRange
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuReferenceRange, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuReferenceRange, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuReferenceRange, "MainWindow.mnuReferenceRange");
			this.mnuReferenceRange.Name = "mnuReferenceRange";
			resources.ApplyResources(this.mnuReferenceRange, "mnuReferenceRange");
			this.mnuReferenceRange.Click += new System.EventHandler(this.mnuReferenceRange_Click);
			// 
			// mnuKtFilter
			// 
			this.mnuKtFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowAllPhrases,
            this.mnuShowPhrasesWithKtRenderings,
            this.mnuShowPhrasesWithMissingKtRenderings});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuKtFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuKtFilter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuKtFilter, "MainWindow.mnuKtFilter");
			this.mnuKtFilter.Name = "mnuKtFilter";
			resources.ApplyResources(this.mnuKtFilter, "mnuKtFilter");
			// 
			// mnuShowAllPhrases
			// 
			this.mnuShowAllPhrases.Checked = true;
			this.mnuShowAllPhrases.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowAllPhrases, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowAllPhrases, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowAllPhrases, "MainWindow.mnuShowAllPhrases");
			this.mnuShowAllPhrases.Name = "mnuShowAllPhrases";
			resources.ApplyResources(this.mnuShowAllPhrases, "mnuShowAllPhrases");
			this.mnuShowAllPhrases.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowAllPhrases.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuShowPhrasesWithKtRenderings
			// 
			this.mnuShowPhrasesWithKtRenderings.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowPhrasesWithKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowPhrasesWithKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowPhrasesWithKtRenderings, "MainWindow.mnuShowPhrasesWithKtRenderings");
			this.mnuShowPhrasesWithKtRenderings.Name = "mnuShowPhrasesWithKtRenderings";
			resources.ApplyResources(this.mnuShowPhrasesWithKtRenderings, "mnuShowPhrasesWithKtRenderings");
			this.mnuShowPhrasesWithKtRenderings.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowPhrasesWithKtRenderings.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuShowPhrasesWithMissingKtRenderings
			// 
			this.mnuShowPhrasesWithMissingKtRenderings.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuShowPhrasesWithMissingKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuShowPhrasesWithMissingKtRenderings, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuShowPhrasesWithMissingKtRenderings, "MainWindow.mnuShowPhrasesWithMissingKtRenderings");
			this.mnuShowPhrasesWithMissingKtRenderings.Name = "mnuShowPhrasesWithMissingKtRenderings";
			resources.ApplyResources(this.mnuShowPhrasesWithMissingKtRenderings, "mnuShowPhrasesWithMissingKtRenderings");
			this.mnuShowPhrasesWithMissingKtRenderings.CheckedChanged += new System.EventHandler(this.OnKeyTermsFilterChecked);
			this.mnuShowPhrasesWithMissingKtRenderings.Click += new System.EventHandler(this.OnKeyTermsFilterChange);
			// 
			// mnuMatchWholeWords
			// 
			this.mnuMatchWholeWords.Checked = true;
			this.mnuMatchWholeWords.CheckOnClick = true;
			this.mnuMatchWholeWords.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuMatchWholeWords, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuMatchWholeWords, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuMatchWholeWords, "MainWindow.mnuMatchWholeWords");
			this.mnuMatchWholeWords.Name = "mnuMatchWholeWords";
			resources.ApplyResources(this.mnuMatchWholeWords, "mnuMatchWholeWords");
			this.mnuMatchWholeWords.CheckedChanged += new System.EventHandler(this.mnuMatchWholeWords_CheckChanged);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.Checked = true;
			this.viewToolStripMenuItem.CheckOnClick = true;
			this.viewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuViewDebugInfo,
            this.mnuViewAnswers,
            this.mnuViewToolbar,
            this.mnuViewBiblicalTermsPane,
            this.toolStripSeparator6,
            this.mnuViewExcludedQuestions,
            this.toolStripSeparator9,
            this.displayLanguageToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.viewToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.viewToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.viewToolStripMenuItem, "MainWindow.viewToolStripMenuItem");
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
			this.viewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// mnuViewToolbar
			// 
			this.mnuViewToolbar.Checked = true;
			this.mnuViewToolbar.CheckOnClick = true;
			this.mnuViewToolbar.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewToolbar, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewToolbar, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewToolbar, "MainWindow.mnuViewToolbar");
			this.mnuViewToolbar.Name = "mnuViewToolbar";
			resources.ApplyResources(this.mnuViewToolbar, "mnuViewToolbar");
			this.mnuViewToolbar.CheckStateChanged += new System.EventHandler(this.mnuViewToolbar_CheckedChanged);
			// 
			// mnuViewBiblicalTermsPane
			// 
			this.mnuViewBiblicalTermsPane.Checked = true;
			this.mnuViewBiblicalTermsPane.CheckOnClick = true;
			this.mnuViewBiblicalTermsPane.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewBiblicalTermsPane, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewBiblicalTermsPane, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewBiblicalTermsPane, "MainWindow.mnuViewBiblicalTermsPane");
			this.mnuViewBiblicalTermsPane.Name = "mnuViewBiblicalTermsPane";
			resources.ApplyResources(this.mnuViewBiblicalTermsPane, "mnuViewBiblicalTermsPane");
			this.mnuViewBiblicalTermsPane.CheckedChanged += new System.EventHandler(this.mnuViewBiblicalTermsPane_CheckedChanged);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			// 
			// mnuViewExcludedQuestions
			// 
			this.mnuViewExcludedQuestions.CheckOnClick = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuViewExcludedQuestions, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuViewExcludedQuestions, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuViewExcludedQuestions, "MainWindow.mnuViewExcludedQuestions");
			this.mnuViewExcludedQuestions.Name = "mnuViewExcludedQuestions";
			resources.ApplyResources(this.mnuViewExcludedQuestions, "mnuViewExcludedQuestions");
			this.mnuViewExcludedQuestions.Click += new System.EventHandler(this.ApplyFilter);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
			// 
			// displayLanguageToolStripMenuItem
			// 
			this.displayLanguageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.en_ToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.displayLanguageToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.displayLanguageToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.displayLanguageToolStripMenuItem, "MainWindow.displayLanguageToolStripMenuItem");
			this.displayLanguageToolStripMenuItem.Name = "displayLanguageToolStripMenuItem";
			resources.ApplyResources(this.displayLanguageToolStripMenuItem, "displayLanguageToolStripMenuItem");
			// 
			// en_ToolStripMenuItem
			// 
			this.en_ToolStripMenuItem.Checked = true;
			this.en_ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.en_ToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.en_ToolStripMenuItem, "MainWindow.en_ToolStripMenuItem");
			this.en_ToolStripMenuItem.Name = "en_ToolStripMenuItem";
			resources.ApplyResources(this.en_ToolStripMenuItem, "en_ToolStripMenuItem");
			this.en_ToolStripMenuItem.Tag = "en-US";
			this.en_ToolStripMenuItem.Click += new System.EventHandler(this.HandleDisplayLanguageSelected);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.phraseSubstitutionsToolStripMenuItem,
            this.biblicalTermsRenderingSelectionRulesToolStripMenuItem});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.optionsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.optionsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.optionsToolStripMenuItem, "MainWindow.optionsToolStripMenuItem");
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
			// 
			// phraseSubstitutionsToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.phraseSubstitutionsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.phraseSubstitutionsToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.phraseSubstitutionsToolStripMenuItem, "MainWindow.phraseSubstitutionsToolStripMenuItem");
			this.phraseSubstitutionsToolStripMenuItem.Name = "phraseSubstitutionsToolStripMenuItem";
			resources.ApplyResources(this.phraseSubstitutionsToolStripMenuItem, "phraseSubstitutionsToolStripMenuItem");
			this.phraseSubstitutionsToolStripMenuItem.Click += new System.EventHandler(this.phraseSubstitutionsToolStripMenuItem_Click);
			// 
			// biblicalTermsRenderingSelectionRulesToolStripMenuItem
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem, "MainWindow.biblicalTermsRenderingSelectionRulesToolStripMenuItem");
			this.biblicalTermsRenderingSelectionRulesToolStripMenuItem.Name = "biblicalTermsRenderingSelectionRulesToolStripMenuItem";
			resources.ApplyResources(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem, "biblicalTermsRenderingSelectionRulesToolStripMenuItem");
			this.biblicalTermsRenderingSelectionRulesToolStripMenuItem.Click += new System.EventHandler(this.biblicalTermsRenderingSelectionRulesToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.helpToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.helpToolStripMenuItem, null);
			this.l10NSharpExtender1.SetLocalizingId(this.helpToolStripMenuItem, "MainWindow.helpToolStripMenuItem");
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
			// 
			// mnuHelpAbout
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.mnuHelpAbout, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.mnuHelpAbout, null);
			this.l10NSharpExtender1.SetLocalizingId(this.mnuHelpAbout, "MainWindow.mnuHelpAbout");
			this.mnuHelpAbout.Name = "mnuHelpAbout";
			resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
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
			this.l10NSharpExtender1.SetLocalizingId(this.toolStrip1, "MainWindow.toolStrip1");
			resources.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.Name = "toolStrip1";
			// 
			// btnSave
			// 
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnSave, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnSave, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnSave, "MainWindow.btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.Click += new System.EventHandler(this.Save);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// toolStripLabelQuestionFilter
			// 
			this.l10NSharpExtender1.SetLocalizableToolTip(this.toolStripLabelQuestionFilter, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.toolStripLabelQuestionFilter, null);
			this.l10NSharpExtender1.SetLocalizingId(this.toolStripLabelQuestionFilter, "MainWindow.toolStripLabelQuestionFilter");
			this.toolStripLabelQuestionFilter.Name = "toolStripLabelQuestionFilter";
			resources.ApplyResources(this.toolStripLabelQuestionFilter, "toolStripLabelQuestionFilter");
			// 
			// txtFilterByPart
			// 
			this.txtFilterByPart.AcceptsReturn = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.txtFilterByPart, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.txtFilterByPart, null);
			this.l10NSharpExtender1.SetLocalizingId(this.txtFilterByPart, "MainWindow.txtFilterByPart");
			this.txtFilterByPart.Name = "txtFilterByPart";
			resources.ApplyResources(this.txtFilterByPart, "txtFilterByPart");
			this.txtFilterByPart.Enter += new System.EventHandler(this.txtFilterByPart_Enter);
			this.txtFilterByPart.Leave += new System.EventHandler(this.txtFilterByPart_Leave);
			this.txtFilterByPart.TextChanged += new System.EventHandler(this.ApplyFilter);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			// 
			// btnSendScrReferences
			// 
			this.btnSendScrReferences.Checked = true;
			this.btnSendScrReferences.CheckOnClick = true;
			this.btnSendScrReferences.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnSendScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.btnSendScrReferences, "btnSendScrReferences");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnSendScrReferences, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnSendScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnSendScrReferences, "MainWindow.btnSendScrReferences");
			this.btnSendScrReferences.Name = "btnSendScrReferences";
			this.btnSendScrReferences.CheckStateChanged += new System.EventHandler(this.btnSendScrReferences_CheckStateChanged);
			// 
			// btnReceiveScrReferences
			// 
			this.btnReceiveScrReferences.CheckOnClick = true;
			this.btnReceiveScrReferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.btnReceiveScrReferences, "btnReceiveScrReferences");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnReceiveScrReferences, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnReceiveScrReferences, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnReceiveScrReferences, "MainWindow.btnReceiveScrReferences");
			this.btnReceiveScrReferences.Name = "btnReceiveScrReferences";
			this.btnReceiveScrReferences.Click += new System.EventHandler(this.btnReceiveScrReferences_CheckStateChanged);
			// 
			// lblFilterIndicator
			// 
			this.lblFilterIndicator.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			resources.ApplyResources(this.lblFilterIndicator, "lblFilterIndicator");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblFilterIndicator, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblFilterIndicator, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblFilterIndicator, "MainWindow.lblFilterIndicator");
			this.lblFilterIndicator.Name = "lblFilterIndicator";
			// 
			// lblRemainingWork
			// 
			this.lblRemainingWork.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.lblRemainingWork, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.lblRemainingWork, null);
			this.l10NSharpExtender1.SetLocalizingId(this.lblRemainingWork, "MainWindow.lblRemainingWork");
			this.lblRemainingWork.Name = "lblRemainingWork";
			resources.ApplyResources(this.lblRemainingWork, "lblRemainingWork");
			// 
			// m_biblicalTermsPane
			// 
			resources.ApplyResources(this.m_biblicalTermsPane, "m_biblicalTermsPane");
			this.m_biblicalTermsPane.Name = "m_biblicalTermsPane";
			this.m_biblicalTermsPane.Resize += new System.EventHandler(this.m_biblicalTermsPane_Resize);
			// 
			// m_lblAnswerLabel
			// 
			resources.ApplyResources(this.m_lblAnswerLabel, "m_lblAnswerLabel");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAnswerLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAnswerLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAnswerLabel, "MainWindow.m_lblAnswerLabel");
			this.m_lblAnswerLabel.Name = "m_lblAnswerLabel";
			// 
			// m_lblAnswers
			// 
			resources.ApplyResources(this.m_lblAnswers, "m_lblAnswers");
			this.m_lblAnswers.BackColor = System.Drawing.SystemColors.Control;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblAnswers, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblAnswers, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblAnswers, "MainWindow.m_lblAnswers");
			this.m_lblAnswers.Name = "m_lblAnswers";
			// 
			// m_lblCommentLabel
			// 
			resources.ApplyResources(this.m_lblCommentLabel, "m_lblCommentLabel");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblCommentLabel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblCommentLabel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblCommentLabel, "MainWindow.m_lblCommentLabel");
			this.m_lblCommentLabel.Name = "m_lblCommentLabel";
			// 
			// m_lblComments
			// 
			resources.ApplyResources(this.m_lblComments, "m_lblComments");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblComments, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblComments, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblComments, "MainWindow.m_lblComments");
			this.m_lblComments.Name = "m_lblComments";
			// 
			// m_pnlAnswersAndComments
			// 
			resources.ApplyResources(this.m_pnlAnswersAndComments, "m_pnlAnswersAndComments");
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblComments, 1, 1);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblAnswers, 1, 0);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblAnswerLabel, 0, 0);
			this.m_pnlAnswersAndComments.Controls.Add(this.m_lblCommentLabel, 0, 1);
			this.m_pnlAnswersAndComments.Name = "m_pnlAnswersAndComments";
			this.m_pnlAnswersAndComments.VisibleChanged += new System.EventHandler(this.LoadAnswersAndCommentsIfShowing);
			// 
			// m_hSplitter
			// 
			this.m_hSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
			resources.ApplyResources(this.m_hSplitter, "m_hSplitter");
			this.m_hSplitter.Name = "m_hSplitter";
			this.m_hSplitter.TabStop = false;
			this.m_hSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.m_hSplitter_SplitterMoving);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = null;
			this.l10NSharpExtender1.PrefixForNewItems = "MainWindow";
			// 
			// UNSQuestionsDialog
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.dataGridUns);
			this.Controls.Add(this.m_hSplitter);
			this.Controls.Add(this.m_biblicalTermsPane);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.m_mainMenu);
			this.Controls.Add(this.m_pnlAnswersAndComments);
			this.HelpButton = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "MainWindow.WindowTitle");
			this.MainMenuStrip = this.m_mainMenu;
			this.Name = "UNSQuestionsDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.UNSQuestionsDialog_HelpButtonClicked);
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
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripMenuItem reloadToolStripMenuItem;
		private ToolStripMenuItem closeToolStripMenuItem;
		private ToolStripMenuItem filterToolStripMenuItem;
		private ToolStripMenuItem mnuKtFilter;
		private ToolStripMenuItem mnuShowAllPhrases;
		private ToolStripMenuItem mnuShowPhrasesWithKtRenderings;
		private ToolStripMenuItem mnuShowPhrasesWithMissingKtRenderings;
		private ToolStripMenuItem viewToolStripMenuItem;
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
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem phraseSubstitutionsToolStripMenuItem;
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
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem mnuHelpAbout;
		private ToolStripMenuItem biblicalTermsRenderingSelectionRulesToolStripMenuItem;
		private ToolStripLabel lblFilterIndicator;
		private ToolStripLabel lblRemainingWork;
		private ToolStripMenuItem mnuViewExcludedQuestions;
		private ToolStripSeparator toolStripSeparator6;
		private ContextMenuStrip dataGridContextMenu;
		private ToolStripMenuItem mnuExcludeQuestion;
		private ToolStripMenuItem mnuIncludeQuestion;
		private ToolStripMenuItem mnuEditQuestion;
		private ToolStripMenuItem mnuAddQuestion;
		private DataGridViewTextBoxColumn m_colReference;
		private DataGridViewTextBoxColumn m_colEnglish;
		private DataGridViewTextBoxColumn m_colTranslation;
		private DataGridViewCheckBoxColumn m_colUserTranslated;
		private DataGridViewTextBoxColumn m_colDebugInfo;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem mnuCopy;
		private ToolStripMenuItem mnuPaste;
        private Splitter m_hSplitter;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem cutToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem nextUntranslatedQuestionToolStripMenuItem;
        private ToolStripMenuItem previousUntranslatedQuestionToolStripMenuItem;
		private ToolStripMenuItem generateOutputForArloToolStripMenuItem;
		private ToolStripMenuItem mnuLoadTranslationsFromTextFile;
		private ToolStripMenuItem mnuShiftWordsRight;
		private ToolStripSeparator toolStripSeparatorShiftWords;
		private ToolStripMenuItem mnuShiftWordsLeft;
		private ToolStripSeparator toolStripSeparator9;
		private ToolStripMenuItem displayLanguageToolStripMenuItem;
		private ToolStripMenuItem en_ToolStripMenuItem;
		private ToolStripMenuItem mnuProduceScriptureForgeFiles;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
	}
}