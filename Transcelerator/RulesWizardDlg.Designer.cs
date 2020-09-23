// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: RulesWizardDlg.Designer.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace SIL.Transcelerator
{
	partial class RulesWizardDlg
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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification="Controls get added to Controls collection and disposed there")]
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		// TODO-Linux: AutoCompletion is not implemented in Mono
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.m_lblUserDefinedOriginalQuestionCondition = new System.Windows.Forms.Label();
			this.m_lblUserDefinedRenderingCondition = new System.Windows.Forms.Label();
			this.grpSelectRendering = new System.Windows.Forms.GroupBox();
			this.m_pnlUserDefinedRenderingMatch = new System.Windows.Forms.Panel();
			this.m_txtRenderingMatchRegEx = new System.Windows.Forms.TextBox();
			this.m_rdoUserDefinedRenderingCriteria = new System.Windows.Forms.RadioButton();
			this.m_pnlVernacularPrefix = new System.Windows.Forms.Panel();
			this.m_lblRenderingConditionVernPrefix = new System.Windows.Forms.Label();
			this.m_txtVernacularPrefix = new System.Windows.Forms.TextBox();
			this.m_rdoRenderingHasPrefix = new System.Windows.Forms.RadioButton();
			this.m_pnlVernacularSuffix = new System.Windows.Forms.Panel();
			this.m_lblRenderingConditionVernSuffix = new System.Windows.Forms.Label();
			this.m_txtVernacularSuffix = new System.Windows.Forms.TextBox();
			this.m_rdoRenderingHasSuffix = new System.Windows.Forms.RadioButton();
			this.m_lblOriginalQuestionConditionFollowingWord = new System.Windows.Forms.Label();
			this.m_lblOriginalQuestionConditionPrecedingWord = new System.Windows.Forms.Label();
			this.m_lblOriginalQuestionConditionPrefix = new System.Windows.Forms.Label();
			this.m_lblOriginalQuestionConditionSuffix = new System.Windows.Forms.Label();
			this.m_lblRuleDescription = new System.Windows.Forms.Label();
			this.m_lblRuleName = new System.Windows.Forms.Label();
			this.grpMatchQuestion = new System.Windows.Forms.GroupBox();
			this.m_lblFollowingWordExample = new System.Windows.Forms.Label();
			this.m_lblPrecedingWordExample = new System.Windows.Forms.Label();
			this.m_lblPrefixExample = new System.Windows.Forms.Label();
			this.m_lblSuffixExample = new System.Windows.Forms.Label();
			this.m_rdoUserDefinedQuestionCriteria = new System.Windows.Forms.RadioButton();
			this.m_rdoFollowingWord = new System.Windows.Forms.RadioButton();
			this.m_rdoPreceedingWord = new System.Windows.Forms.RadioButton();
			this.m_rdoPrefix = new System.Windows.Forms.RadioButton();
			this.m_rdoSuffix = new System.Windows.Forms.RadioButton();
			this.m_pnlFollowingWordDetails = new System.Windows.Forms.Panel();
			this.m_cboFollowingWord = new System.Windows.Forms.ComboBox();
			this.m_pnlPrecedingWordDetails = new System.Windows.Forms.Panel();
			this.m_cboPrecedingWord = new System.Windows.Forms.ComboBox();
			this.m_pnlPrefixDetails = new System.Windows.Forms.Panel();
			this.m_cboPrefix = new System.Windows.Forms.ComboBox();
			this.m_pnlSuffixDetails = new System.Windows.Forms.Panel();
			this.m_cboSuffix = new System.Windows.Forms.ComboBox();
			this.m_pnlUserDefinedRuleDetails = new System.Windows.Forms.Panel();
			this.m_txtQuestionMatchRegEx = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.m_lblDescription = new System.Windows.Forms.Label();
			this.m_txtName = new System.Windows.Forms.TextBox();
			this.l10NSharpExtender1 = new L10NSharp.UI.L10NSharpExtender(this.components);
			this.grpSelectRendering.SuspendLayout();
			this.m_pnlUserDefinedRenderingMatch.SuspendLayout();
			this.m_pnlVernacularPrefix.SuspendLayout();
			this.m_pnlVernacularSuffix.SuspendLayout();
			this.grpMatchQuestion.SuspendLayout();
			this.m_pnlFollowingWordDetails.SuspendLayout();
			this.m_pnlPrecedingWordDetails.SuspendLayout();
			this.m_pnlPrefixDetails.SuspendLayout();
			this.m_pnlSuffixDetails.SuspendLayout();
			this.m_pnlUserDefinedRuleDetails.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).BeginInit();
			this.SuspendLayout();
			// 
			// m_lblUserDefinedOriginalQuestionCondition
			// 
			this.m_lblUserDefinedOriginalQuestionCondition.AutoSize = true;
			this.m_lblUserDefinedOriginalQuestionCondition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblUserDefinedOriginalQuestionCondition, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblUserDefinedOriginalQuestionCondition, "Note: {0} is literal text that will be displayed to the user. It must be preserve" +
        "d verbatim in the translation, but it will not be replaced by the program.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblUserDefinedOriginalQuestionCondition, "RulesWizardDlg.m_lblUserDefinedOriginalQuestionCondition");
			this.m_lblUserDefinedOriginalQuestionCondition.Location = new System.Drawing.Point(0, 0);
			this.m_lblUserDefinedOriginalQuestionCondition.Name = "m_lblUserDefinedOriginalQuestionCondition";
			this.m_lblUserDefinedOriginalQuestionCondition.Size = new System.Drawing.Size(412, 13);
			this.m_lblUserDefinedOriginalQuestionCondition.TabIndex = 0;
			this.m_lblUserDefinedOriginalQuestionCondition.Text = "Type a regular expression where {0} is the placeholder for the stem of the biblic" +
    "al term:";
			// 
			// m_lblUserDefinedRenderingCondition
			// 
			this.m_lblUserDefinedRenderingCondition.AutoSize = true;
			this.m_lblUserDefinedRenderingCondition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblUserDefinedRenderingCondition, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblUserDefinedRenderingCondition, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblUserDefinedRenderingCondition, "RulesWizardDlg.m_lblUserDefinedRenderingCondition");
			this.m_lblUserDefinedRenderingCondition.Location = new System.Drawing.Point(0, 0);
			this.m_lblUserDefinedRenderingCondition.Name = "m_lblUserDefinedRenderingCondition";
			this.m_lblUserDefinedRenderingCondition.Size = new System.Drawing.Size(393, 13);
			this.m_lblUserDefinedRenderingCondition.TabIndex = 0;
			this.m_lblUserDefinedRenderingCondition.Text = "Type a regular expression to match vernacular renderings that should be selected:" +
    "";
			// 
			// grpSelectRendering
			// 
			this.grpSelectRendering.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpSelectRendering.Controls.Add(this.m_pnlUserDefinedRenderingMatch);
			this.grpSelectRendering.Controls.Add(this.m_rdoUserDefinedRenderingCriteria);
			this.grpSelectRendering.Controls.Add(this.m_pnlVernacularPrefix);
			this.grpSelectRendering.Controls.Add(this.m_rdoRenderingHasPrefix);
			this.grpSelectRendering.Controls.Add(this.m_pnlVernacularSuffix);
			this.grpSelectRendering.Controls.Add(this.m_rdoRenderingHasSuffix);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.grpSelectRendering, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.grpSelectRendering, null);
			this.l10NSharpExtender1.SetLocalizingId(this.grpSelectRendering, "RulesWizardDlg.grpSelectRendering");
			this.grpSelectRendering.Location = new System.Drawing.Point(15, 320);
			this.grpSelectRendering.Name = "grpSelectRendering";
			this.grpSelectRendering.Size = new System.Drawing.Size(506, 134);
			this.grpSelectRendering.TabIndex = 3;
			this.grpSelectRendering.TabStop = false;
			this.grpSelectRendering.Text = "Select the rendering of the biblical that meets the following condition:";
			// 
			// m_pnlUserDefinedRenderingMatch
			// 
			this.m_pnlUserDefinedRenderingMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_pnlUserDefinedRenderingMatch.Controls.Add(this.m_lblUserDefinedRenderingCondition);
			this.m_pnlUserDefinedRenderingMatch.Controls.Add(this.m_txtRenderingMatchRegEx);
			this.m_pnlUserDefinedRenderingMatch.Location = new System.Drawing.Point(21, 87);
			this.m_pnlUserDefinedRenderingMatch.Name = "m_pnlUserDefinedRenderingMatch";
			this.m_pnlUserDefinedRenderingMatch.Size = new System.Drawing.Size(479, 37);
			this.m_pnlUserDefinedRenderingMatch.TabIndex = 15;
			this.m_pnlUserDefinedRenderingMatch.Visible = false;
			this.m_pnlUserDefinedRenderingMatch.VisibleChanged += new System.EventHandler(this.m_pnlUserDefinedRenderingMatch_VisibleChanged);
			// 
			// m_txtRenderingMatchRegEx
			// 
			this.m_txtRenderingMatchRegEx.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtRenderingMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtRenderingMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtRenderingMatchRegEx, "RulesWizardDlg.m_txtRenderingMatchRegEx");
			this.m_txtRenderingMatchRegEx.Location = new System.Drawing.Point(0, 17);
			this.m_txtRenderingMatchRegEx.Name = "m_txtRenderingMatchRegEx";
			this.m_txtRenderingMatchRegEx.Size = new System.Drawing.Size(479, 20);
			this.m_txtRenderingMatchRegEx.TabIndex = 1;
			this.m_txtRenderingMatchRegEx.TextChanged += new System.EventHandler(this.m_txtRenderingMatchRegEx_TextChanged);
			// 
			// m_rdoUserDefinedRenderingCriteria
			// 
			this.m_rdoUserDefinedRenderingCriteria.AutoSize = true;
			this.m_rdoUserDefinedRenderingCriteria.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUserDefinedRenderingCriteria, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUserDefinedRenderingCriteria, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUserDefinedRenderingCriteria, "RulesWizardDlg.m_rdoUserDefinedRenderingCriteria");
			this.m_rdoUserDefinedRenderingCriteria.Location = new System.Drawing.Point(7, 65);
			this.m_rdoUserDefinedRenderingCriteria.Name = "m_rdoUserDefinedRenderingCriteria";
			this.m_rdoUserDefinedRenderingCriteria.Size = new System.Drawing.Size(221, 17);
			this.m_rdoUserDefinedRenderingCriteria.TabIndex = 2;
			this.m_rdoUserDefinedRenderingCriteria.Text = "&Rendering meets a user-defined condition";
			this.m_rdoUserDefinedRenderingCriteria.UseVisualStyleBackColor = true;
			this.m_rdoUserDefinedRenderingCriteria.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlVernacularPrefix
			// 
			this.m_pnlVernacularPrefix.AutoSize = true;
			this.m_pnlVernacularPrefix.Controls.Add(this.m_lblRenderingConditionVernPrefix);
			this.m_pnlVernacularPrefix.Controls.Add(this.m_txtVernacularPrefix);
			this.m_pnlVernacularPrefix.Location = new System.Drawing.Point(214, 43);
			this.m_pnlVernacularPrefix.Name = "m_pnlVernacularPrefix";
			this.m_pnlVernacularPrefix.Size = new System.Drawing.Size(286, 25);
			this.m_pnlVernacularPrefix.TabIndex = 10;
			this.m_pnlVernacularPrefix.Visible = false;
			this.m_pnlVernacularPrefix.VisibleChanged += new System.EventHandler(this.m_pnlVernacularPrefix_VisibleChanged);
			// 
			// m_lblRenderingConditionVernPrefix
			// 
			this.m_lblRenderingConditionVernPrefix.AutoSize = true;
			this.m_lblRenderingConditionVernPrefix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblRenderingConditionVernPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblRenderingConditionVernPrefix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblRenderingConditionVernPrefix, "RulesWizardDlg.m_lblRenderingConditionVernPrefix");
			this.m_lblRenderingConditionVernPrefix.Location = new System.Drawing.Point(0, 1);
			this.m_lblRenderingConditionVernPrefix.Name = "m_lblRenderingConditionVernPrefix";
			this.m_lblRenderingConditionVernPrefix.Size = new System.Drawing.Size(89, 13);
			this.m_lblRenderingConditionVernPrefix.TabIndex = 0;
			this.m_lblRenderingConditionVernPrefix.Text = "&Vernacular prefix:";
			// 
			// m_txtVernacularPrefix
			// 
			this.m_txtVernacularPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtVernacularPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtVernacularPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtVernacularPrefix, "RulesWizardDlg.m_txtVernacularPrefix");
			this.m_txtVernacularPrefix.Location = new System.Drawing.Point(95, 0);
			this.m_txtVernacularPrefix.Name = "m_txtVernacularPrefix";
			this.m_txtVernacularPrefix.Size = new System.Drawing.Size(191, 20);
			this.m_txtVernacularPrefix.TabIndex = 1;
			this.m_txtVernacularPrefix.TextChanged += new System.EventHandler(this.m_txtVernacularPrefix_TextChanged);
			this.m_txtVernacularPrefix.Enter += new System.EventHandler(this.VernacularTextBox_Enter);
			this.m_txtVernacularPrefix.Leave += new System.EventHandler(this.VernacularTextBox_Leave);
			// 
			// m_rdoRenderingHasPrefix
			// 
			this.m_rdoRenderingHasPrefix.AutoSize = true;
			this.m_rdoRenderingHasPrefix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoRenderingHasPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoRenderingHasPrefix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoRenderingHasPrefix, "RulesWizardDlg.m_rdoRenderingHasPrefix");
			this.m_rdoRenderingHasPrefix.Location = new System.Drawing.Point(7, 42);
			this.m_rdoRenderingHasPrefix.Name = "m_rdoRenderingHasPrefix";
			this.m_rdoRenderingHasPrefix.Size = new System.Drawing.Size(170, 17);
			this.m_rdoRenderingHasPrefix.TabIndex = 1;
			this.m_rdoRenderingHasPrefix.Text = "Rendering has a specific p&refix";
			this.m_rdoRenderingHasPrefix.UseVisualStyleBackColor = true;
			this.m_rdoRenderingHasPrefix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlVernacularSuffix
			// 
			this.m_pnlVernacularSuffix.AutoSize = true;
			this.m_pnlVernacularSuffix.Controls.Add(this.m_lblRenderingConditionVernSuffix);
			this.m_pnlVernacularSuffix.Controls.Add(this.m_txtVernacularSuffix);
			this.m_pnlVernacularSuffix.Location = new System.Drawing.Point(214, 20);
			this.m_pnlVernacularSuffix.Name = "m_pnlVernacularSuffix";
			this.m_pnlVernacularSuffix.Size = new System.Drawing.Size(286, 25);
			this.m_pnlVernacularSuffix.TabIndex = 9;
			this.m_pnlVernacularSuffix.VisibleChanged += new System.EventHandler(this.m_pnlVernacularSuffix_VisibleChanged);
			// 
			// m_lblRenderingConditionVernSuffix
			// 
			this.m_lblRenderingConditionVernSuffix.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblRenderingConditionVernSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblRenderingConditionVernSuffix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblRenderingConditionVernSuffix, "RulesWizardDlg.m_lblRenderingConditionVernSuffix");
			this.m_lblRenderingConditionVernSuffix.Location = new System.Drawing.Point(0, 1);
			this.m_lblRenderingConditionVernSuffix.Name = "m_lblRenderingConditionVernSuffix";
			this.m_lblRenderingConditionVernSuffix.Size = new System.Drawing.Size(88, 13);
			this.m_lblRenderingConditionVernSuffix.TabIndex = 0;
			this.m_lblRenderingConditionVernSuffix.Text = "&Vernacular suffix:";
			// 
			// m_txtVernacularSuffix
			// 
			this.m_txtVernacularSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtVernacularSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtVernacularSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtVernacularSuffix, "RulesWizardDlg.m_txtVernacularSuffix");
			this.m_txtVernacularSuffix.Location = new System.Drawing.Point(95, 0);
			this.m_txtVernacularSuffix.Name = "m_txtVernacularSuffix";
			this.m_txtVernacularSuffix.Size = new System.Drawing.Size(191, 20);
			this.m_txtVernacularSuffix.TabIndex = 1;
			this.m_txtVernacularSuffix.TextChanged += new System.EventHandler(this.m_txtVernacularSuffix_TextChanged);
			this.m_txtVernacularSuffix.Enter += new System.EventHandler(this.VernacularTextBox_Enter);
			this.m_txtVernacularSuffix.Leave += new System.EventHandler(this.VernacularTextBox_Leave);
			// 
			// m_rdoRenderingHasSuffix
			// 
			this.m_rdoRenderingHasSuffix.AutoSize = true;
			this.m_rdoRenderingHasSuffix.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoRenderingHasSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoRenderingHasSuffix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoRenderingHasSuffix, "RulesWizardDlg.m_rdoRenderingHasSuffix");
			this.m_rdoRenderingHasSuffix.Location = new System.Drawing.Point(7, 19);
			this.m_rdoRenderingHasSuffix.Name = "m_rdoRenderingHasSuffix";
			this.m_rdoRenderingHasSuffix.Size = new System.Drawing.Size(169, 17);
			this.m_rdoRenderingHasSuffix.TabIndex = 0;
			this.m_rdoRenderingHasSuffix.TabStop = true;
			this.m_rdoRenderingHasSuffix.Text = "Rendering has a specific s&uffix";
			this.m_rdoRenderingHasSuffix.UseVisualStyleBackColor = true;
			this.m_rdoRenderingHasSuffix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_lblOriginalQuestionConditionFollowingWord
			// 
			this.m_lblOriginalQuestionConditionFollowingWord.AutoSize = true;
			this.m_lblOriginalQuestionConditionFollowingWord.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblOriginalQuestionConditionFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblOriginalQuestionConditionFollowingWord, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblOriginalQuestionConditionFollowingWord, "RulesWizardDlg.m_lblOriginalQuestionConditionFollowingWord");
			this.m_lblOriginalQuestionConditionFollowingWord.Location = new System.Drawing.Point(0, 1);
			this.m_lblOriginalQuestionConditionFollowingWord.Name = "m_lblOriginalQuestionConditionFollowingWord";
			this.m_lblOriginalQuestionConditionFollowingWord.Size = new System.Drawing.Size(80, 13);
			this.m_lblOriginalQuestionConditionFollowingWord.TabIndex = 0;
			this.m_lblOriginalQuestionConditionFollowingWord.Text = "Following &word:";
			// 
			// m_lblOriginalQuestionConditionPrecedingWord
			// 
			this.m_lblOriginalQuestionConditionPrecedingWord.AutoSize = true;
			this.m_lblOriginalQuestionConditionPrecedingWord.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblOriginalQuestionConditionPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblOriginalQuestionConditionPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblOriginalQuestionConditionPrecedingWord, "RulesWizardDlg.m_lblOriginalQuestionConditionPrecedingWord");
			this.m_lblOriginalQuestionConditionPrecedingWord.Location = new System.Drawing.Point(0, 1);
			this.m_lblOriginalQuestionConditionPrecedingWord.Name = "m_lblOriginalQuestionConditionPrecedingWord";
			this.m_lblOriginalQuestionConditionPrecedingWord.Size = new System.Drawing.Size(84, 13);
			this.m_lblOriginalQuestionConditionPrecedingWord.TabIndex = 0;
			this.m_lblOriginalQuestionConditionPrecedingWord.Text = "Preceding &word:";
			// 
			// m_lblOriginalQuestionConditionPrefix
			// 
			this.m_lblOriginalQuestionConditionPrefix.AutoSize = true;
			this.m_lblOriginalQuestionConditionPrefix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblOriginalQuestionConditionPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblOriginalQuestionConditionPrefix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblOriginalQuestionConditionPrefix, "RulesWizardDlg.m_lblOriginalQuestionConditionPrefix");
			this.m_lblOriginalQuestionConditionPrefix.Location = new System.Drawing.Point(0, 1);
			this.m_lblOriginalQuestionConditionPrefix.Name = "m_lblOriginalQuestionConditionPrefix";
			this.m_lblOriginalQuestionConditionPrefix.Size = new System.Drawing.Size(36, 13);
			this.m_lblOriginalQuestionConditionPrefix.TabIndex = 0;
			this.m_lblOriginalQuestionConditionPrefix.Text = "Prefi&x:";
			// 
			// m_lblOriginalQuestionConditionSuffix
			// 
			this.m_lblOriginalQuestionConditionSuffix.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblOriginalQuestionConditionSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblOriginalQuestionConditionSuffix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblOriginalQuestionConditionSuffix, "RulesWizardDlg.m_lblOriginalQuestionConditionSuffix");
			this.m_lblOriginalQuestionConditionSuffix.Location = new System.Drawing.Point(0, 1);
			this.m_lblOriginalQuestionConditionSuffix.Name = "m_lblOriginalQuestionConditionSuffix";
			this.m_lblOriginalQuestionConditionSuffix.Size = new System.Drawing.Size(36, 13);
			this.m_lblOriginalQuestionConditionSuffix.TabIndex = 0;
			this.m_lblOriginalQuestionConditionSuffix.Text = "Suffi&x:";
			// 
			// m_lblRuleDescription
			// 
			this.m_lblRuleDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblRuleDescription.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblRuleDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblRuleDescription, "");
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblRuleDescription, "RulesWizardDlg.m_lblRuleDescription");
			this.m_lblRuleDescription.Location = new System.Drawing.Point(12, 457);
			this.m_lblRuleDescription.Name = "m_lblRuleDescription";
			this.m_lblRuleDescription.Size = new System.Drawing.Size(86, 13);
			this.m_lblRuleDescription.TabIndex = 4;
			this.m_lblRuleDescription.Text = "Rule description:";
			// 
			// m_lblRuleName
			// 
			this.m_lblRuleName.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblRuleName, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblRuleName, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblRuleName, "RulesWizardDlg.m_lblRuleName");
			this.m_lblRuleName.Location = new System.Drawing.Point(12, 9);
			this.m_lblRuleName.Name = "m_lblRuleName";
			this.m_lblRuleName.Size = new System.Drawing.Size(61, 13);
			this.m_lblRuleName.TabIndex = 0;
			this.m_lblRuleName.Text = "Rule name:";
			// 
			// grpMatchQuestion
			// 
			this.grpMatchQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpMatchQuestion.Controls.Add(this.m_lblFollowingWordExample);
			this.grpMatchQuestion.Controls.Add(this.m_lblPrecedingWordExample);
			this.grpMatchQuestion.Controls.Add(this.m_lblPrefixExample);
			this.grpMatchQuestion.Controls.Add(this.m_lblSuffixExample);
			this.grpMatchQuestion.Controls.Add(this.m_rdoUserDefinedQuestionCriteria);
			this.grpMatchQuestion.Controls.Add(this.m_rdoFollowingWord);
			this.grpMatchQuestion.Controls.Add(this.m_rdoPreceedingWord);
			this.grpMatchQuestion.Controls.Add(this.m_rdoPrefix);
			this.grpMatchQuestion.Controls.Add(this.m_rdoSuffix);
			this.grpMatchQuestion.Controls.Add(this.m_pnlFollowingWordDetails);
			this.grpMatchQuestion.Controls.Add(this.m_pnlPrecedingWordDetails);
			this.grpMatchQuestion.Controls.Add(this.m_pnlPrefixDetails);
			this.grpMatchQuestion.Controls.Add(this.m_pnlSuffixDetails);
			this.grpMatchQuestion.Controls.Add(this.m_pnlUserDefinedRuleDetails);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.grpMatchQuestion, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.grpMatchQuestion, null);
			this.l10NSharpExtender1.SetLocalizingId(this.grpMatchQuestion, "RulesWizardDlg.grpMatchQuestion");
			this.grpMatchQuestion.Location = new System.Drawing.Point(15, 42);
			this.grpMatchQuestion.Name = "grpMatchQuestion";
			this.grpMatchQuestion.Size = new System.Drawing.Size(506, 265);
			this.grpMatchQuestion.TabIndex = 2;
			this.grpMatchQuestion.TabStop = false;
			this.grpMatchQuestion.Text = "This rule applies when the biblical term in the original question meets the follo" +
    "wing condition:";
			// 
			// m_lblFollowingWordExample
			// 
			this.m_lblFollowingWordExample.AutoSize = true;
			this.m_lblFollowingWordExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.m_lblFollowingWordExample.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblFollowingWordExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblFollowingWordExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblFollowingWordExample, "RulesWizardDlg.m_lblFollowingWordExample");
			this.m_lblFollowingWordExample.Location = new System.Drawing.Point(22, 168);
			this.m_lblFollowingWordExample.Name = "m_lblFollowingWordExample";
			this.m_lblFollowingWordExample.Size = new System.Drawing.Size(73, 18);
			this.m_lblFollowingWordExample.TabIndex = 22;
			this.m_lblFollowingWordExample.Tag = "slave \\b\\i girl";
			this.m_lblFollowingWordExample.Text = "Example: ";
			// 
			// m_lblPrecedingWordExample
			// 
			this.m_lblPrecedingWordExample.AutoSize = true;
			this.m_lblPrecedingWordExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.m_lblPrecedingWordExample.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblPrecedingWordExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblPrecedingWordExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblPrecedingWordExample, "RulesWizardDlg.m_lblPrecedingWordExample");
			this.m_lblPrecedingWordExample.Location = new System.Drawing.Point(22, 125);
			this.m_lblPrecedingWordExample.Name = "m_lblPrecedingWordExample";
			this.m_lblPrecedingWordExample.Size = new System.Drawing.Size(73, 18);
			this.m_lblPrecedingWordExample.TabIndex = 21;
			this.m_lblPrecedingWordExample.Tag = "\\b\\i for\\b0\\i0  Abraham";
			this.m_lblPrecedingWordExample.Text = "Example: ";
			// 
			// m_lblPrefixExample
			// 
			this.m_lblPrefixExample.AutoSize = true;
			this.m_lblPrefixExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.m_lblPrefixExample.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblPrefixExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblPrefixExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblPrefixExample, "RulesWizardDlg.m_lblPrefixExample");
			this.m_lblPrefixExample.Location = new System.Drawing.Point(22, 82);
			this.m_lblPrefixExample.Name = "m_lblPrefixExample";
			this.m_lblPrefixExample.Size = new System.Drawing.Size(73, 18);
			this.m_lblPrefixExample.TabIndex = 20;
			this.m_lblPrefixExample.Tag = "\\b\\i un\\b0\\i0 believer";
			this.m_lblPrefixExample.Text = "Example: ";
			// 
			// m_lblSuffixExample
			// 
			this.m_lblSuffixExample.AutoSize = true;
			this.m_lblSuffixExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblSuffixExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblSuffixExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblSuffixExample, "RulesWizardDlg.m_lblSuffixExample");
			this.m_lblSuffixExample.Location = new System.Drawing.Point(22, 39);
			this.m_lblSuffixExample.Name = "m_lblSuffixExample";
			this.m_lblSuffixExample.Size = new System.Drawing.Size(73, 18);
			this.m_lblSuffixExample.TabIndex = 19;
			this.m_lblSuffixExample.Tag = "bless\\b\\i ing";
			this.m_lblSuffixExample.Text = "Example: ";
			// 
			// m_rdoUserDefinedQuestionCriteria
			// 
			this.m_rdoUserDefinedQuestionCriteria.AutoSize = true;
			this.m_rdoUserDefinedQuestionCriteria.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUserDefinedQuestionCriteria, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUserDefinedQuestionCriteria, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUserDefinedQuestionCriteria, "RulesWizardDlg.m_rdoUserDefinedQuestionCriteria");
			this.m_rdoUserDefinedQuestionCriteria.Location = new System.Drawing.Point(7, 191);
			this.m_rdoUserDefinedQuestionCriteria.Name = "m_rdoUserDefinedQuestionCriteria";
			this.m_rdoUserDefinedQuestionCriteria.Size = new System.Drawing.Size(131, 17);
			this.m_rdoUserDefinedQuestionCriteria.TabIndex = 8;
			this.m_rdoUserDefinedQuestionCriteria.Text = "&User-defined condition";
			this.m_rdoUserDefinedQuestionCriteria.UseVisualStyleBackColor = true;
			this.m_rdoUserDefinedQuestionCriteria.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoFollowingWord
			// 
			this.m_rdoFollowingWord.AutoSize = true;
			this.m_rdoFollowingWord.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoFollowingWord, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoFollowingWord, "RulesWizardDlg.m_rdoFollowingWord");
			this.m_rdoFollowingWord.Location = new System.Drawing.Point(6, 148);
			this.m_rdoFollowingWord.Name = "m_rdoFollowingWord";
			this.m_rdoFollowingWord.Size = new System.Drawing.Size(246, 17);
			this.m_rdoFollowingWord.TabIndex = 6;
			this.m_rdoFollowingWord.Text = "Term is immediately &followed by a specific word";
			this.m_rdoFollowingWord.UseVisualStyleBackColor = true;
			this.m_rdoFollowingWord.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoPreceedingWord
			// 
			this.m_rdoPreceedingWord.AutoSize = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoPreceedingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoPreceedingWord, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoPreceedingWord, "RulesWizardDlg.m_rdoPreceedingWord");
			this.m_rdoPreceedingWord.Location = new System.Drawing.Point(6, 105);
			this.m_rdoPreceedingWord.Name = "m_rdoPreceedingWord";
			this.m_rdoPreceedingWord.Size = new System.Drawing.Size(252, 17);
			this.m_rdoPreceedingWord.TabIndex = 4;
			this.m_rdoPreceedingWord.Text = "Term is immediately &preceded by a specific word";
			this.m_rdoPreceedingWord.UseVisualStyleBackColor = true;
			this.m_rdoPreceedingWord.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoPrefix
			// 
			this.m_rdoPrefix.AutoSize = true;
			this.m_rdoPrefix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoPrefix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoPrefix, "RulesWizardDlg.m_rdoPrefix");
			this.m_rdoPrefix.Location = new System.Drawing.Point(6, 62);
			this.m_rdoPrefix.Name = "m_rdoPrefix";
			this.m_rdoPrefix.Size = new System.Drawing.Size(145, 17);
			this.m_rdoPrefix.TabIndex = 2;
			this.m_rdoPrefix.Text = "Term has a specific &prefix";
			this.m_rdoPrefix.UseVisualStyleBackColor = true;
			this.m_rdoPrefix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoSuffix
			// 
			this.m_rdoSuffix.AutoSize = true;
			this.m_rdoSuffix.Checked = true;
			this.m_rdoSuffix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSuffix, "To control which character will be the mnemonic key (underlined when the user pre" +
        "sses the ALT key), put the ampersand before the desired character.");
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSuffix, "RulesWizardDlg.m_rdoSuffix");
			this.m_rdoSuffix.Location = new System.Drawing.Point(7, 19);
			this.m_rdoSuffix.Name = "m_rdoSuffix";
			this.m_rdoSuffix.Size = new System.Drawing.Size(144, 17);
			this.m_rdoSuffix.TabIndex = 0;
			this.m_rdoSuffix.TabStop = true;
			this.m_rdoSuffix.Text = "Term has a specific &suffix";
			this.m_rdoSuffix.UseVisualStyleBackColor = true;
			this.m_rdoSuffix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlFollowingWordDetails
			// 
			this.m_pnlFollowingWordDetails.Controls.Add(this.m_cboFollowingWord);
			this.m_pnlFollowingWordDetails.Controls.Add(this.m_lblOriginalQuestionConditionFollowingWord);
			this.m_pnlFollowingWordDetails.Location = new System.Drawing.Point(283, 149);
			this.m_pnlFollowingWordDetails.Name = "m_pnlFollowingWordDetails";
			this.m_pnlFollowingWordDetails.Size = new System.Drawing.Size(217, 26);
			this.m_pnlFollowingWordDetails.TabIndex = 18;
			this.m_pnlFollowingWordDetails.Visible = false;
			this.m_pnlFollowingWordDetails.VisibleChanged += new System.EventHandler(this.m_pnlFollowingWordDetails_VisibleChanged);
			// 
			// m_cboFollowingWord
			// 
			this.m_cboFollowingWord.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboFollowingWord.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboFollowingWord.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboFollowingWord, "RulesWizardDlg.m_cboFollowingWord");
			this.m_cboFollowingWord.Location = new System.Drawing.Point(90, 0);
			this.m_cboFollowingWord.Name = "m_cboFollowingWord";
			this.m_cboFollowingWord.Size = new System.Drawing.Size(127, 21);
			this.m_cboFollowingWord.TabIndex = 1;
			this.m_cboFollowingWord.TextChanged += new System.EventHandler(this.m_cboFollowingWord_TextChanged);
			// 
			// m_pnlPrecedingWordDetails
			// 
			this.m_pnlPrecedingWordDetails.Controls.Add(this.m_cboPrecedingWord);
			this.m_pnlPrecedingWordDetails.Controls.Add(this.m_lblOriginalQuestionConditionPrecedingWord);
			this.m_pnlPrecedingWordDetails.Location = new System.Drawing.Point(283, 106);
			this.m_pnlPrecedingWordDetails.Name = "m_pnlPrecedingWordDetails";
			this.m_pnlPrecedingWordDetails.Size = new System.Drawing.Size(217, 26);
			this.m_pnlPrecedingWordDetails.TabIndex = 17;
			this.m_pnlPrecedingWordDetails.Visible = false;
			this.m_pnlPrecedingWordDetails.VisibleChanged += new System.EventHandler(this.m_pnlPrecedingWordDetails_VisibleChanged);
			// 
			// m_cboPrecedingWord
			// 
			this.m_cboPrecedingWord.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboPrecedingWord.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboPrecedingWord.FormattingEnabled = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboPrecedingWord, "RulesWizardDlg.m_cboPrecedingWord");
			this.m_cboPrecedingWord.Location = new System.Drawing.Point(90, 0);
			this.m_cboPrecedingWord.Name = "m_cboPrecedingWord";
			this.m_cboPrecedingWord.Size = new System.Drawing.Size(127, 21);
			this.m_cboPrecedingWord.TabIndex = 1;
			this.m_cboPrecedingWord.TextChanged += new System.EventHandler(this.m_cboPrecedingWord_TextChanged);
			// 
			// m_pnlPrefixDetails
			// 
			this.m_pnlPrefixDetails.Controls.Add(this.m_cboPrefix);
			this.m_pnlPrefixDetails.Controls.Add(this.m_lblOriginalQuestionConditionPrefix);
			this.m_pnlPrefixDetails.Location = new System.Drawing.Point(283, 63);
			this.m_pnlPrefixDetails.Name = "m_pnlPrefixDetails";
			this.m_pnlPrefixDetails.Size = new System.Drawing.Size(217, 26);
			this.m_pnlPrefixDetails.TabIndex = 16;
			this.m_pnlPrefixDetails.Visible = false;
			this.m_pnlPrefixDetails.VisibleChanged += new System.EventHandler(this.m_pnlPrefixDetails_VisibleChanged);
			// 
			// m_cboPrefix
			// 
			this.m_cboPrefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboPrefix.FormattingEnabled = true;
			this.m_cboPrefix.Items.AddRange(new object[] {
            "un",
            "inter"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboPrefix, "RulesWizardDlg.m_cboPrefix");
			this.m_cboPrefix.Location = new System.Drawing.Point(42, 0);
			this.m_cboPrefix.Name = "m_cboPrefix";
			this.m_cboPrefix.Size = new System.Drawing.Size(124, 21);
			this.m_cboPrefix.TabIndex = 1;
			this.m_cboPrefix.TextChanged += new System.EventHandler(this.m_cboPrefix_TextChanged);
			// 
			// m_pnlSuffixDetails
			// 
			this.m_pnlSuffixDetails.Controls.Add(this.m_cboSuffix);
			this.m_pnlSuffixDetails.Controls.Add(this.m_lblOriginalQuestionConditionSuffix);
			this.m_pnlSuffixDetails.Location = new System.Drawing.Point(283, 20);
			this.m_pnlSuffixDetails.Name = "m_pnlSuffixDetails";
			this.m_pnlSuffixDetails.Size = new System.Drawing.Size(217, 26);
			this.m_pnlSuffixDetails.TabIndex = 15;
			this.m_pnlSuffixDetails.VisibleChanged += new System.EventHandler(this.m_pnlSuffixDetails_VisibleChanged);
			// 
			// m_cboSuffix
			// 
			this.m_cboSuffix.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboSuffix.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboSuffix.FormattingEnabled = true;
			this.m_cboSuffix.Items.AddRange(new object[] {
            "al",
            "ate",
            "ble",
            "ed",
            "en",
            "er",
            "ful",
            "ic",
            "ing",
            "ive",
            "ize",
            "less",
            "ly",
            "ment",
            "ness",
            "ous",
            "s",
            "tion",
            "y"});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboSuffix, "RulesWizardDlg.m_cboSuffix");
			this.m_cboSuffix.Location = new System.Drawing.Point(42, 0);
			this.m_cboSuffix.Name = "m_cboSuffix";
			this.m_cboSuffix.Size = new System.Drawing.Size(124, 21);
			this.m_cboSuffix.TabIndex = 1;
			this.m_cboSuffix.TextChanged += new System.EventHandler(this.m_cboSuffix_TextChanged);
			// 
			// m_pnlUserDefinedRuleDetails
			// 
			this.m_pnlUserDefinedRuleDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_pnlUserDefinedRuleDetails.Controls.Add(this.m_lblUserDefinedOriginalQuestionCondition);
			this.m_pnlUserDefinedRuleDetails.Controls.Add(this.m_txtQuestionMatchRegEx);
			this.m_pnlUserDefinedRuleDetails.Location = new System.Drawing.Point(21, 214);
			this.m_pnlUserDefinedRuleDetails.Name = "m_pnlUserDefinedRuleDetails";
			this.m_pnlUserDefinedRuleDetails.Size = new System.Drawing.Size(479, 37);
			this.m_pnlUserDefinedRuleDetails.TabIndex = 14;
			this.m_pnlUserDefinedRuleDetails.Visible = false;
			this.m_pnlUserDefinedRuleDetails.VisibleChanged += new System.EventHandler(this.m_pnlUserDefinedRuleDetails_VisibleChanged);
			// 
			// m_txtQuestionMatchRegEx
			// 
			this.m_txtQuestionMatchRegEx.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtQuestionMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtQuestionMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtQuestionMatchRegEx, "RulesWizardDlg.m_txtQuestionMatchRegEx");
			this.m_txtQuestionMatchRegEx.Location = new System.Drawing.Point(0, 17);
			this.m_txtQuestionMatchRegEx.Name = "m_txtQuestionMatchRegEx";
			this.m_txtQuestionMatchRegEx.Size = new System.Drawing.Size(479, 20);
			this.m_txtQuestionMatchRegEx.TabIndex = 1;
			this.m_txtQuestionMatchRegEx.TextChanged += new System.EventHandler(this.m_txtQuestionMatchRegEx_TextChanged);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOk.Enabled = false;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Location = new System.Drawing.Point(188, 562);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Location = new System.Drawing.Point(269, 562);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_lblDescription
			// 
			this.m_lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblDescription.BackColor = System.Drawing.SystemColors.Window;
			this.m_lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblDescription, "RulesWizardDlg.m_lblDescription");
			this.m_lblDescription.Location = new System.Drawing.Point(12, 479);
			this.m_lblDescription.Name = "m_lblDescription";
			this.m_lblDescription.Size = new System.Drawing.Size(509, 69);
			this.m_lblDescription.TabIndex = 5;
			// 
			// m_txtName
			// 
			this.m_txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtName, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtName, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtName, "RulesWizardDlg.m_txtName");
			this.m_txtName.Location = new System.Drawing.Point(79, 9);
			this.m_txtName.Name = "m_txtName";
			this.m_txtName.Size = new System.Drawing.Size(442, 20);
			this.m_txtName.TabIndex = 1;
			this.m_txtName.Validating += new System.ComponentModel.CancelEventHandler(this.m_txtName_Validating);
			// 
			// l10NSharpExtender1
			// 
			this.l10NSharpExtender1.LocalizationManagerId = "Transcelerator";
			this.l10NSharpExtender1.PrefixForNewItems = null;
			// 
			// RulesWizardDlg
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(533, 597);
			this.Controls.Add(this.m_txtName);
			this.Controls.Add(this.m_lblDescription);
			this.Controls.Add(this.m_lblRuleName);
			this.Controls.Add(this.grpSelectRendering);
			this.Controls.Add(this.grpMatchQuestion);
			this.Controls.Add(this.m_lblRuleDescription);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.l10NSharpExtender1.SetLocalizableToolTip(this, null);
			this.l10NSharpExtender1.SetLocalizationComment(this, null);
			this.l10NSharpExtender1.SetLocalizingId(this, "RulesWizardDlg.WindowTitle");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RulesWizardDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create Rendering Selection Rule";
			this.grpSelectRendering.ResumeLayout(false);
			this.grpSelectRendering.PerformLayout();
			this.m_pnlUserDefinedRenderingMatch.ResumeLayout(false);
			this.m_pnlUserDefinedRenderingMatch.PerformLayout();
			this.m_pnlVernacularPrefix.ResumeLayout(false);
			this.m_pnlVernacularPrefix.PerformLayout();
			this.m_pnlVernacularSuffix.ResumeLayout(false);
			this.m_pnlVernacularSuffix.PerformLayout();
			this.grpMatchQuestion.ResumeLayout(false);
			this.grpMatchQuestion.PerformLayout();
			this.m_pnlFollowingWordDetails.ResumeLayout(false);
			this.m_pnlFollowingWordDetails.PerformLayout();
			this.m_pnlPrecedingWordDetails.ResumeLayout(false);
			this.m_pnlPrecedingWordDetails.PerformLayout();
			this.m_pnlPrefixDetails.ResumeLayout(false);
			this.m_pnlPrefixDetails.PerformLayout();
			this.m_pnlSuffixDetails.ResumeLayout(false);
			this.m_pnlSuffixDetails.PerformLayout();
			this.m_pnlUserDefinedRuleDetails.ResumeLayout(false);
			this.m_pnlUserDefinedRuleDetails.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.l10NSharpExtender1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton m_rdoPreceedingWord;
		private System.Windows.Forms.RadioButton m_rdoSuffix;
		private System.Windows.Forms.RadioButton m_rdoPrefix;
		private System.Windows.Forms.RadioButton m_rdoFollowingWord;
		private System.Windows.Forms.RadioButton m_rdoUserDefinedQuestionCriteria;
		private System.Windows.Forms.RadioButton m_rdoRenderingHasSuffix;
		private System.Windows.Forms.TextBox m_txtQuestionMatchRegEx;
		private System.Windows.Forms.Panel m_pnlUserDefinedRuleDetails;
		private System.Windows.Forms.Panel m_pnlSuffixDetails;
		private System.Windows.Forms.ComboBox m_cboSuffix;
		private System.Windows.Forms.Panel m_pnlPrefixDetails;
		private System.Windows.Forms.ComboBox m_cboPrefix;
		private System.Windows.Forms.Panel m_pnlPrecedingWordDetails;
		private System.Windows.Forms.ComboBox m_cboPrecedingWord;
		private System.Windows.Forms.Panel m_pnlFollowingWordDetails;
		private System.Windows.Forms.ComboBox m_cboFollowingWord;
		private System.Windows.Forms.TextBox m_txtVernacularSuffix;
		private System.Windows.Forms.Panel m_pnlVernacularSuffix;
		private System.Windows.Forms.RadioButton m_rdoRenderingHasPrefix;
		private System.Windows.Forms.Panel m_pnlVernacularPrefix;
		private System.Windows.Forms.TextBox m_txtVernacularPrefix;
		private System.Windows.Forms.Panel m_pnlUserDefinedRenderingMatch;
		private System.Windows.Forms.TextBox m_txtRenderingMatchRegEx;
		private System.Windows.Forms.RadioButton m_rdoUserDefinedRenderingCriteria;
		private System.Windows.Forms.Label m_lblDescription;
		private System.Windows.Forms.TextBox m_txtName;
        private System.Windows.Forms.GroupBox grpMatchQuestion;
        private System.Windows.Forms.Label m_lblSuffixExample;
        private System.Windows.Forms.Label m_lblPrefixExample;
        private System.Windows.Forms.Label m_lblFollowingWordExample;
        private System.Windows.Forms.Label m_lblPrecedingWordExample;
		private L10NSharp.UI.L10NSharpExtender l10NSharpExtender1;
		private System.Windows.Forms.Label m_lblRuleDescription;
		private System.Windows.Forms.Label m_lblUserDefinedOriginalQuestionCondition;
		private System.Windows.Forms.Label m_lblUserDefinedRenderingCondition;
		private System.Windows.Forms.GroupBox grpSelectRendering;
		private System.Windows.Forms.Label m_lblRenderingConditionVernPrefix;
		private System.Windows.Forms.Label m_lblRenderingConditionVernSuffix;
		private System.Windows.Forms.Label m_lblOriginalQuestionConditionFollowingWord;
		private System.Windows.Forms.Label m_lblOriginalQuestionConditionPrecedingWord;
		private System.Windows.Forms.Label m_lblOriginalQuestionConditionPrefix;
		private System.Windows.Forms.Label m_lblOriginalQuestionConditionSuffix;
		private System.Windows.Forms.Label m_lblRuleName;
	}
}