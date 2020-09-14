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
			System.Windows.Forms.Label m_lblUserDefinedOriginalQuestionCondition;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RulesWizardDlg));
			System.Windows.Forms.Label m_lblUserDefinedRenderingCondition;
			System.Windows.Forms.GroupBox grpSelectRendering;
			System.Windows.Forms.Label m_lblRenderingConditionVernPrefix;
			System.Windows.Forms.Label m_lblRenderingConditionVernSuffix;
			System.Windows.Forms.Label m_lblOriginalQuestionConditionFollowingWord;
			System.Windows.Forms.Label m_lblOriginalQuestionConditionPrecedingWord;
			System.Windows.Forms.Label m_lblOriginalQuestionConditionPrefix;
			System.Windows.Forms.Label m_lblOriginalQuestionConditionSuffix;
			System.Windows.Forms.Label m_lblRuleDescription;
			System.Windows.Forms.Label m_lblRuleName;
			this.m_pnlUserDefinedRenderingMatch = new System.Windows.Forms.Panel();
			this.m_txtRenderingMatchRegEx = new System.Windows.Forms.TextBox();
			this.m_rdoUserDefinedRenderingCriteria = new System.Windows.Forms.RadioButton();
			this.m_pnlVernacularPrefix = new System.Windows.Forms.Panel();
			this.m_txtVernacularPrefix = new System.Windows.Forms.TextBox();
			this.m_rdoRenderingHasPrefix = new System.Windows.Forms.RadioButton();
			this.m_pnlVernacularSuffix = new System.Windows.Forms.Panel();
			this.m_txtVernacularSuffix = new System.Windows.Forms.TextBox();
			this.m_rdoRenderingHasSuffix = new System.Windows.Forms.RadioButton();
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
			m_lblUserDefinedOriginalQuestionCondition = new System.Windows.Forms.Label();
			m_lblUserDefinedRenderingCondition = new System.Windows.Forms.Label();
			grpSelectRendering = new System.Windows.Forms.GroupBox();
			m_lblRenderingConditionVernPrefix = new System.Windows.Forms.Label();
			m_lblRenderingConditionVernSuffix = new System.Windows.Forms.Label();
			m_lblOriginalQuestionConditionFollowingWord = new System.Windows.Forms.Label();
			m_lblOriginalQuestionConditionPrecedingWord = new System.Windows.Forms.Label();
			m_lblOriginalQuestionConditionPrefix = new System.Windows.Forms.Label();
			m_lblOriginalQuestionConditionSuffix = new System.Windows.Forms.Label();
			m_lblRuleDescription = new System.Windows.Forms.Label();
			m_lblRuleName = new System.Windows.Forms.Label();
			grpSelectRendering.SuspendLayout();
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
			resources.ApplyResources(m_lblUserDefinedOriginalQuestionCondition, "m_lblUserDefinedOriginalQuestionCondition");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblUserDefinedOriginalQuestionCondition, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblUserDefinedOriginalQuestionCondition, "Note: {0} is literal text that will be displayed to the user. It must be preserve" +
        "d verbatim in the translation, but it will not be replaced by the program.");
			this.l10NSharpExtender1.SetLocalizingId(m_lblUserDefinedOriginalQuestionCondition, "RulesWizardDlg.m_lblUserDefinedOriginalQuestionCondition");
			m_lblUserDefinedOriginalQuestionCondition.Name = "m_lblUserDefinedOriginalQuestionCondition";
			// 
			// m_lblUserDefinedRenderingCondition
			// 
			resources.ApplyResources(m_lblUserDefinedRenderingCondition, "m_lblUserDefinedRenderingCondition");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblUserDefinedRenderingCondition, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblUserDefinedRenderingCondition, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblUserDefinedRenderingCondition, "RulesWizardDlg.m_lblUserDefinedRenderingCondition");
			m_lblUserDefinedRenderingCondition.Name = "m_lblUserDefinedRenderingCondition";
			// 
			// grpSelectRendering
			// 
			resources.ApplyResources(grpSelectRendering, "grpSelectRendering");
			grpSelectRendering.Controls.Add(this.m_pnlUserDefinedRenderingMatch);
			grpSelectRendering.Controls.Add(this.m_rdoUserDefinedRenderingCriteria);
			grpSelectRendering.Controls.Add(this.m_pnlVernacularPrefix);
			grpSelectRendering.Controls.Add(this.m_rdoRenderingHasPrefix);
			grpSelectRendering.Controls.Add(this.m_pnlVernacularSuffix);
			grpSelectRendering.Controls.Add(this.m_rdoRenderingHasSuffix);
			this.l10NSharpExtender1.SetLocalizableToolTip(grpSelectRendering, null);
			this.l10NSharpExtender1.SetLocalizationComment(grpSelectRendering, null);
			this.l10NSharpExtender1.SetLocalizingId(grpSelectRendering, "RulesWizardDlg.grpSelectRendering");
			grpSelectRendering.Name = "grpSelectRendering";
			grpSelectRendering.TabStop = false;
			// 
			// m_pnlUserDefinedRenderingMatch
			// 
			resources.ApplyResources(this.m_pnlUserDefinedRenderingMatch, "m_pnlUserDefinedRenderingMatch");
			this.m_pnlUserDefinedRenderingMatch.Controls.Add(m_lblUserDefinedRenderingCondition);
			this.m_pnlUserDefinedRenderingMatch.Controls.Add(this.m_txtRenderingMatchRegEx);
			this.m_pnlUserDefinedRenderingMatch.Name = "m_pnlUserDefinedRenderingMatch";
			this.m_pnlUserDefinedRenderingMatch.VisibleChanged += new System.EventHandler(this.m_pnlUserDefinedRenderingMatch_VisibleChanged);
			// 
			// m_txtRenderingMatchRegEx
			// 
			resources.ApplyResources(this.m_txtRenderingMatchRegEx, "m_txtRenderingMatchRegEx");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtRenderingMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtRenderingMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtRenderingMatchRegEx, "RulesWizardDlg.m_txtRenderingMatchRegEx");
			this.m_txtRenderingMatchRegEx.Name = "m_txtRenderingMatchRegEx";
			this.m_txtRenderingMatchRegEx.TextChanged += new System.EventHandler(this.m_txtRenderingMatchRegEx_TextChanged);
			// 
			// m_rdoUserDefinedRenderingCriteria
			// 
			resources.ApplyResources(this.m_rdoUserDefinedRenderingCriteria, "m_rdoUserDefinedRenderingCriteria");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUserDefinedRenderingCriteria, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUserDefinedRenderingCriteria, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUserDefinedRenderingCriteria, "RulesWizardDlg.m_rdoUserDefinedRenderingCriteria");
			this.m_rdoUserDefinedRenderingCriteria.Name = "m_rdoUserDefinedRenderingCriteria";
			this.m_rdoUserDefinedRenderingCriteria.UseVisualStyleBackColor = true;
			this.m_rdoUserDefinedRenderingCriteria.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlVernacularPrefix
			// 
			resources.ApplyResources(this.m_pnlVernacularPrefix, "m_pnlVernacularPrefix");
			this.m_pnlVernacularPrefix.Controls.Add(m_lblRenderingConditionVernPrefix);
			this.m_pnlVernacularPrefix.Controls.Add(this.m_txtVernacularPrefix);
			this.m_pnlVernacularPrefix.Name = "m_pnlVernacularPrefix";
			this.m_pnlVernacularPrefix.VisibleChanged += new System.EventHandler(this.m_pnlVernacularPrefix_VisibleChanged);
			// 
			// m_lblRenderingConditionVernPrefix
			// 
			resources.ApplyResources(m_lblRenderingConditionVernPrefix, "m_lblRenderingConditionVernPrefix");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblRenderingConditionVernPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblRenderingConditionVernPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblRenderingConditionVernPrefix, "RulesWizardDlg.m_lblRenderingConditionVernPrefix");
			m_lblRenderingConditionVernPrefix.Name = "m_lblRenderingConditionVernPrefix";
			// 
			// m_txtVernacularPrefix
			// 
			resources.ApplyResources(this.m_txtVernacularPrefix, "m_txtVernacularPrefix");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtVernacularPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtVernacularPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtVernacularPrefix, "RulesWizardDlg.m_txtVernacularPrefix");
			this.m_txtVernacularPrefix.Name = "m_txtVernacularPrefix";
			this.m_txtVernacularPrefix.TextChanged += new System.EventHandler(this.m_txtVernacularPrefix_TextChanged);
			this.m_txtVernacularPrefix.Enter += new System.EventHandler(this.VernacularTextBox_Enter);
			this.m_txtVernacularPrefix.Leave += new System.EventHandler(this.VernacularTextBox_Leave);
			// 
			// m_rdoRenderingHasPrefix
			// 
			resources.ApplyResources(this.m_rdoRenderingHasPrefix, "m_rdoRenderingHasPrefix");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoRenderingHasPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoRenderingHasPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoRenderingHasPrefix, "RulesWizardDlg.m_rdoRenderingHasPrefix");
			this.m_rdoRenderingHasPrefix.Name = "m_rdoRenderingHasPrefix";
			this.m_rdoRenderingHasPrefix.UseVisualStyleBackColor = true;
			this.m_rdoRenderingHasPrefix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlVernacularSuffix
			// 
			resources.ApplyResources(this.m_pnlVernacularSuffix, "m_pnlVernacularSuffix");
			this.m_pnlVernacularSuffix.Controls.Add(m_lblRenderingConditionVernSuffix);
			this.m_pnlVernacularSuffix.Controls.Add(this.m_txtVernacularSuffix);
			this.m_pnlVernacularSuffix.Name = "m_pnlVernacularSuffix";
			this.m_pnlVernacularSuffix.VisibleChanged += new System.EventHandler(this.m_pnlVernacularSuffix_VisibleChanged);
			// 
			// m_lblRenderingConditionVernSuffix
			// 
			resources.ApplyResources(m_lblRenderingConditionVernSuffix, "m_lblRenderingConditionVernSuffix");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblRenderingConditionVernSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblRenderingConditionVernSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblRenderingConditionVernSuffix, "RulesWizardDlg.m_lblRenderingConditionVernSuffix");
			m_lblRenderingConditionVernSuffix.Name = "m_lblRenderingConditionVernSuffix";
			// 
			// m_txtVernacularSuffix
			// 
			resources.ApplyResources(this.m_txtVernacularSuffix, "m_txtVernacularSuffix");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtVernacularSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtVernacularSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtVernacularSuffix, "RulesWizardDlg.m_txtVernacularSuffix");
			this.m_txtVernacularSuffix.Name = "m_txtVernacularSuffix";
			this.m_txtVernacularSuffix.TextChanged += new System.EventHandler(this.m_txtVernacularSuffix_TextChanged);
			this.m_txtVernacularSuffix.Enter += new System.EventHandler(this.VernacularTextBox_Enter);
			this.m_txtVernacularSuffix.Leave += new System.EventHandler(this.VernacularTextBox_Leave);
			// 
			// m_rdoRenderingHasSuffix
			// 
			resources.ApplyResources(this.m_rdoRenderingHasSuffix, "m_rdoRenderingHasSuffix");
			this.m_rdoRenderingHasSuffix.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoRenderingHasSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoRenderingHasSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoRenderingHasSuffix, "RulesWizardDlg.m_rdoRenderingHasSuffix");
			this.m_rdoRenderingHasSuffix.Name = "m_rdoRenderingHasSuffix";
			this.m_rdoRenderingHasSuffix.TabStop = true;
			this.m_rdoRenderingHasSuffix.UseVisualStyleBackColor = true;
			this.m_rdoRenderingHasSuffix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// grpMatchQuestion
			// 
			resources.ApplyResources(this.grpMatchQuestion, "grpMatchQuestion");
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
			this.grpMatchQuestion.Name = "grpMatchQuestion";
			this.grpMatchQuestion.TabStop = false;
			// 
			// m_lblFollowingWordExample
			// 
			resources.ApplyResources(this.m_lblFollowingWordExample, "m_lblFollowingWordExample");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblFollowingWordExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblFollowingWordExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblFollowingWordExample, "RulesWizardDlg.m_lblFollowingWordExample");
			this.m_lblFollowingWordExample.Name = "m_lblFollowingWordExample";
			this.m_lblFollowingWordExample.Tag = "slave \\b\\i girl";
			// 
			// m_lblPrecedingWordExample
			// 
			resources.ApplyResources(this.m_lblPrecedingWordExample, "m_lblPrecedingWordExample");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblPrecedingWordExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblPrecedingWordExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblPrecedingWordExample, "RulesWizardDlg.m_lblPrecedingWordExample");
			this.m_lblPrecedingWordExample.Name = "m_lblPrecedingWordExample";
			this.m_lblPrecedingWordExample.Tag = "\\b\\i for\\b0\\i0  Abraham";
			// 
			// m_lblPrefixExample
			// 
			resources.ApplyResources(this.m_lblPrefixExample, "m_lblPrefixExample");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblPrefixExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblPrefixExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblPrefixExample, "RulesWizardDlg.m_lblPrefixExample");
			this.m_lblPrefixExample.Name = "m_lblPrefixExample";
			this.m_lblPrefixExample.Tag = "\\b\\i un\\b0\\i0 believer";
			// 
			// m_lblSuffixExample
			// 
			resources.ApplyResources(this.m_lblSuffixExample, "m_lblSuffixExample");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblSuffixExample, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblSuffixExample, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblSuffixExample, "RulesWizardDlg.m_lblSuffixExample");
			this.m_lblSuffixExample.Name = "m_lblSuffixExample";
			this.m_lblSuffixExample.Tag = "bless\\b\\i ing";
			// 
			// m_rdoUserDefinedQuestionCriteria
			// 
			resources.ApplyResources(this.m_rdoUserDefinedQuestionCriteria, "m_rdoUserDefinedQuestionCriteria");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoUserDefinedQuestionCriteria, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoUserDefinedQuestionCriteria, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoUserDefinedQuestionCriteria, "RulesWizardDlg.m_rdoUserDefinedQuestionCriteria");
			this.m_rdoUserDefinedQuestionCriteria.Name = "m_rdoUserDefinedQuestionCriteria";
			this.m_rdoUserDefinedQuestionCriteria.UseVisualStyleBackColor = true;
			this.m_rdoUserDefinedQuestionCriteria.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoFollowingWord
			// 
			resources.ApplyResources(this.m_rdoFollowingWord, "m_rdoFollowingWord");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoFollowingWord, "RulesWizardDlg.m_rdoFollowingWord");
			this.m_rdoFollowingWord.Name = "m_rdoFollowingWord";
			this.m_rdoFollowingWord.UseVisualStyleBackColor = true;
			this.m_rdoFollowingWord.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoPreceedingWord
			// 
			resources.ApplyResources(this.m_rdoPreceedingWord, "m_rdoPreceedingWord");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoPreceedingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoPreceedingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoPreceedingWord, "RulesWizardDlg.m_rdoPreceedingWord");
			this.m_rdoPreceedingWord.Name = "m_rdoPreceedingWord";
			this.m_rdoPreceedingWord.UseVisualStyleBackColor = true;
			this.m_rdoPreceedingWord.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoPrefix
			// 
			resources.ApplyResources(this.m_rdoPrefix, "m_rdoPrefix");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoPrefix, "RulesWizardDlg.m_rdoPrefix");
			this.m_rdoPrefix.Name = "m_rdoPrefix";
			this.m_rdoPrefix.UseVisualStyleBackColor = true;
			this.m_rdoPrefix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_rdoSuffix
			// 
			resources.ApplyResources(this.m_rdoSuffix, "m_rdoSuffix");
			this.m_rdoSuffix.Checked = true;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_rdoSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_rdoSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_rdoSuffix, "RulesWizardDlg.m_rdoSuffix");
			this.m_rdoSuffix.Name = "m_rdoSuffix";
			this.m_rdoSuffix.TabStop = true;
			this.m_rdoSuffix.UseVisualStyleBackColor = true;
			this.m_rdoSuffix.CheckedChanged += new System.EventHandler(this.OptionCheckedChanged);
			// 
			// m_pnlFollowingWordDetails
			// 
			this.m_pnlFollowingWordDetails.Controls.Add(this.m_cboFollowingWord);
			this.m_pnlFollowingWordDetails.Controls.Add(m_lblOriginalQuestionConditionFollowingWord);
			resources.ApplyResources(this.m_pnlFollowingWordDetails, "m_pnlFollowingWordDetails");
			this.m_pnlFollowingWordDetails.Name = "m_pnlFollowingWordDetails";
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
			resources.ApplyResources(this.m_cboFollowingWord, "m_cboFollowingWord");
			this.m_cboFollowingWord.Name = "m_cboFollowingWord";
			this.m_cboFollowingWord.TextChanged += new System.EventHandler(this.m_cboFollowingWord_TextChanged);
			// 
			// m_lblOriginalQuestionConditionFollowingWord
			// 
			resources.ApplyResources(m_lblOriginalQuestionConditionFollowingWord, "m_lblOriginalQuestionConditionFollowingWord");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalQuestionConditionFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalQuestionConditionFollowingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalQuestionConditionFollowingWord, "RulesWizardDlg.m_lblOriginalQuestionConditionFollowingWord");
			m_lblOriginalQuestionConditionFollowingWord.Name = "m_lblOriginalQuestionConditionFollowingWord";
			// 
			// m_pnlPrecedingWordDetails
			// 
			this.m_pnlPrecedingWordDetails.Controls.Add(this.m_cboPrecedingWord);
			this.m_pnlPrecedingWordDetails.Controls.Add(m_lblOriginalQuestionConditionPrecedingWord);
			resources.ApplyResources(this.m_pnlPrecedingWordDetails, "m_pnlPrecedingWordDetails");
			this.m_pnlPrecedingWordDetails.Name = "m_pnlPrecedingWordDetails";
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
			resources.ApplyResources(this.m_cboPrecedingWord, "m_cboPrecedingWord");
			this.m_cboPrecedingWord.Name = "m_cboPrecedingWord";
			this.m_cboPrecedingWord.TextChanged += new System.EventHandler(this.m_cboPrecedingWord_TextChanged);
			// 
			// m_lblOriginalQuestionConditionPrecedingWord
			// 
			resources.ApplyResources(m_lblOriginalQuestionConditionPrecedingWord, "m_lblOriginalQuestionConditionPrecedingWord");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalQuestionConditionPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalQuestionConditionPrecedingWord, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalQuestionConditionPrecedingWord, "RulesWizardDlg.m_lblOriginalQuestionConditionPrecedingWord");
			m_lblOriginalQuestionConditionPrecedingWord.Name = "m_lblOriginalQuestionConditionPrecedingWord";
			// 
			// m_pnlPrefixDetails
			// 
			this.m_pnlPrefixDetails.Controls.Add(this.m_cboPrefix);
			this.m_pnlPrefixDetails.Controls.Add(m_lblOriginalQuestionConditionPrefix);
			resources.ApplyResources(this.m_pnlPrefixDetails, "m_pnlPrefixDetails");
			this.m_pnlPrefixDetails.Name = "m_pnlPrefixDetails";
			this.m_pnlPrefixDetails.VisibleChanged += new System.EventHandler(this.m_pnlPrefixDetails_VisibleChanged);
			// 
			// m_cboPrefix
			// 
			this.m_cboPrefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboPrefix.FormattingEnabled = true;
			this.m_cboPrefix.Items.AddRange(new object[] {
            resources.GetString("m_cboPrefix.Items"),
            resources.GetString("m_cboPrefix.Items1")});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboPrefix, "RulesWizardDlg.m_cboPrefix");
			resources.ApplyResources(this.m_cboPrefix, "m_cboPrefix");
			this.m_cboPrefix.Name = "m_cboPrefix";
			this.m_cboPrefix.TextChanged += new System.EventHandler(this.m_cboPrefix_TextChanged);
			// 
			// m_lblOriginalQuestionConditionPrefix
			// 
			resources.ApplyResources(m_lblOriginalQuestionConditionPrefix, "m_lblOriginalQuestionConditionPrefix");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalQuestionConditionPrefix, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalQuestionConditionPrefix, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalQuestionConditionPrefix, "RulesWizardDlg.m_lblOriginalQuestionConditionPrefix");
			m_lblOriginalQuestionConditionPrefix.Name = "m_lblOriginalQuestionConditionPrefix";
			// 
			// m_pnlSuffixDetails
			// 
			this.m_pnlSuffixDetails.Controls.Add(this.m_cboSuffix);
			this.m_pnlSuffixDetails.Controls.Add(m_lblOriginalQuestionConditionSuffix);
			resources.ApplyResources(this.m_pnlSuffixDetails, "m_pnlSuffixDetails");
			this.m_pnlSuffixDetails.Name = "m_pnlSuffixDetails";
			this.m_pnlSuffixDetails.VisibleChanged += new System.EventHandler(this.m_pnlSuffixDetails_VisibleChanged);
			// 
			// m_cboSuffix
			// 
			this.m_cboSuffix.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.m_cboSuffix.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.m_cboSuffix.FormattingEnabled = true;
			this.m_cboSuffix.Items.AddRange(new object[] {
            resources.GetString("m_cboSuffix.Items"),
            resources.GetString("m_cboSuffix.Items1"),
            resources.GetString("m_cboSuffix.Items2"),
            resources.GetString("m_cboSuffix.Items3"),
            resources.GetString("m_cboSuffix.Items4"),
            resources.GetString("m_cboSuffix.Items5"),
            resources.GetString("m_cboSuffix.Items6"),
            resources.GetString("m_cboSuffix.Items7"),
            resources.GetString("m_cboSuffix.Items8"),
            resources.GetString("m_cboSuffix.Items9"),
            resources.GetString("m_cboSuffix.Items10"),
            resources.GetString("m_cboSuffix.Items11"),
            resources.GetString("m_cboSuffix.Items12"),
            resources.GetString("m_cboSuffix.Items13"),
            resources.GetString("m_cboSuffix.Items14"),
            resources.GetString("m_cboSuffix.Items15"),
            resources.GetString("m_cboSuffix.Items16"),
            resources.GetString("m_cboSuffix.Items17"),
            resources.GetString("m_cboSuffix.Items18")});
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_cboSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_cboSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_cboSuffix, "RulesWizardDlg.m_cboSuffix");
			resources.ApplyResources(this.m_cboSuffix, "m_cboSuffix");
			this.m_cboSuffix.Name = "m_cboSuffix";
			this.m_cboSuffix.TextChanged += new System.EventHandler(this.m_cboSuffix_TextChanged);
			// 
			// m_lblOriginalQuestionConditionSuffix
			// 
			resources.ApplyResources(m_lblOriginalQuestionConditionSuffix, "m_lblOriginalQuestionConditionSuffix");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblOriginalQuestionConditionSuffix, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblOriginalQuestionConditionSuffix, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblOriginalQuestionConditionSuffix, "RulesWizardDlg.m_lblOriginalQuestionConditionSuffix");
			m_lblOriginalQuestionConditionSuffix.Name = "m_lblOriginalQuestionConditionSuffix";
			// 
			// m_pnlUserDefinedRuleDetails
			// 
			resources.ApplyResources(this.m_pnlUserDefinedRuleDetails, "m_pnlUserDefinedRuleDetails");
			this.m_pnlUserDefinedRuleDetails.Controls.Add(m_lblUserDefinedOriginalQuestionCondition);
			this.m_pnlUserDefinedRuleDetails.Controls.Add(this.m_txtQuestionMatchRegEx);
			this.m_pnlUserDefinedRuleDetails.Name = "m_pnlUserDefinedRuleDetails";
			this.m_pnlUserDefinedRuleDetails.VisibleChanged += new System.EventHandler(this.m_pnlUserDefinedRuleDetails_VisibleChanged);
			// 
			// m_txtQuestionMatchRegEx
			// 
			resources.ApplyResources(this.m_txtQuestionMatchRegEx, "m_txtQuestionMatchRegEx");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtQuestionMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtQuestionMatchRegEx, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtQuestionMatchRegEx, "RulesWizardDlg.m_txtQuestionMatchRegEx");
			this.m_txtQuestionMatchRegEx.Name = "m_txtQuestionMatchRegEx";
			this.m_txtQuestionMatchRegEx.TextChanged += new System.EventHandler(this.m_txtQuestionMatchRegEx_TextChanged);
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnOk, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnOk, "Common.OK");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.btnCancel, null);
			this.l10NSharpExtender1.SetLocalizingId(this.btnCancel, "Common.Cancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_lblRuleDescription
			// 
			resources.ApplyResources(m_lblRuleDescription, "m_lblRuleDescription");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblRuleDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblRuleDescription, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblRuleDescription, "RulesWizardDlg.m_lblRuleDescription");
			m_lblRuleDescription.Name = "m_lblRuleDescription";
			// 
			// m_lblRuleName
			// 
			resources.ApplyResources(m_lblRuleName, "m_lblRuleName");
			this.l10NSharpExtender1.SetLocalizableToolTip(m_lblRuleName, null);
			this.l10NSharpExtender1.SetLocalizationComment(m_lblRuleName, null);
			this.l10NSharpExtender1.SetLocalizingId(m_lblRuleName, "RulesWizardDlg.m_lblRuleName");
			m_lblRuleName.Name = "m_lblRuleName";
			// 
			// m_lblDescription
			// 
			resources.ApplyResources(this.m_lblDescription, "m_lblDescription");
			this.m_lblDescription.BackColor = System.Drawing.SystemColors.Window;
			this.m_lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_lblDescription, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_lblDescription, "RulesWizardDlg.m_lblDescription");
			this.m_lblDescription.Name = "m_lblDescription";
			// 
			// m_txtName
			// 
			resources.ApplyResources(this.m_txtName, "m_txtName");
			this.l10NSharpExtender1.SetLocalizableToolTip(this.m_txtName, null);
			this.l10NSharpExtender1.SetLocalizationComment(this.m_txtName, null);
			this.l10NSharpExtender1.SetLocalizingId(this.m_txtName, "RulesWizardDlg.m_txtName");
			this.m_txtName.Name = "m_txtName";
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
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.m_txtName);
			this.Controls.Add(this.m_lblDescription);
			this.Controls.Add(m_lblRuleName);
			this.Controls.Add(grpSelectRendering);
			this.Controls.Add(this.grpMatchQuestion);
			this.Controls.Add(m_lblRuleDescription);
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
			grpSelectRendering.ResumeLayout(false);
			grpSelectRendering.PerformLayout();
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
	}
}