// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: RulesWizardDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using L10NSharp;
using SIL.IO;
using static System.String;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class RulesWizardDlg : Form
	{
		private RenderingSelectionRule m_rule;
		private readonly Action<bool> m_selectKeyboard;
		private readonly string m_help;

		private Func<string, bool> ValidateName { get; }

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:RulesWizardDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public RulesWizardDlg(RenderingSelectionRule rule, IEnumerable<string> allWordsInQuestions,
			Action<bool> selectKeyboard, Func<string, bool> nameValidator)
		{
			InitializeComponent();

            FormatExample(m_lblSuffixExample, m_pnlSuffixDetails);
            FormatExample(m_lblPrefixExample, m_pnlPrefixDetails);
            FormatExample(m_lblPrecedingWordExample, m_pnlPrecedingWordDetails);
            FormatExample(m_lblFollowingWordExample, m_pnlFollowingWordDetails);

		    m_rdoSuffix.Tag = m_pnlSuffixDetails;
			m_rdoPrefix.Tag = m_pnlPrefixDetails;
			m_rdoPreceedingWord.Tag = m_pnlPrecedingWordDetails;
			m_rdoFollowingWord.Tag = m_pnlFollowingWordDetails;
			m_rdoUserDefinedQuestionCriteria.Tag = m_pnlUserDefinedRuleDetails;
			m_rdoRenderingHasSuffix.Tag = m_pnlVernacularSuffix;
			m_rdoRenderingHasPrefix.Tag = m_pnlVernacularPrefix;
			m_rdoUserDefinedRenderingCriteria.Tag = m_pnlUserDefinedRenderingMatch;

			foreach (string word in allWordsInQuestions)
			{
				m_cboFollowingWord.Items.Add(word);
				m_cboPrecedingWord.Items.Add(word);
			}

			m_rule = rule;
			m_selectKeyboard = selectKeyboard;
			ValidateName = nameValidator;
			m_txtName.Text = m_rule.Name;

			m_help = FileLocationUtilities.GetFileDistributedWithApplication(true, "docs", "adjustments.htm");
			HelpButton = !IsNullOrEmpty(m_help);

			switch (m_rule.QuestionMatchCriteriaType)
			{
				case RenderingSelectionRule.QuestionMatchType.Undefined:
					Text = LocalizationManager.GetString("RulesWizardDlg.EditRuleCaption",
						"Edit Rendering Selection Rule");
					m_rdoSuffix.Checked = true; // default;
					//SetDetails(m_cboSuffix, string.Empty);
					return;
				case RenderingSelectionRule.QuestionMatchType.Suffix:
					m_rdoSuffix.Checked = true;
					SetDetails(m_cboSuffix, m_rule.QuestionMatchSuffix);
					break;
				case RenderingSelectionRule.QuestionMatchType.Prefix:
					m_rdoPrefix.Checked = true;
					SetDetails(m_cboPrefix, m_rule.QuestionMatchPrefix);
					break;
				case RenderingSelectionRule.QuestionMatchType.PrecedingWord:
					m_rdoPreceedingWord.Checked = true;
					SetDetails(m_cboPrecedingWord, m_rule.QuestionMatchPrecedingWord);
					break;
				case RenderingSelectionRule.QuestionMatchType.FollowingWord:
					m_rdoFollowingWord.Checked = true;
					SetDetails(m_cboFollowingWord, m_rule.QuestionMatchFollowingWord);
					break;
				case RenderingSelectionRule.QuestionMatchType.Custom:
					m_rdoUserDefinedQuestionCriteria.Checked = true;
					m_txtQuestionMatchRegEx.Text = m_rule.QuestionMatchingPattern;
					break;
			}

			switch (m_rule.RenderingMatchCriteriaType)
			{
				case RenderingSelectionRule.RenderingMatchType.Undefined: // default
				case RenderingSelectionRule.RenderingMatchType.Suffix:
					m_rdoRenderingHasSuffix.Checked = true;
					m_txtVernacularSuffix.Text = m_rule.RenderingMatchSuffix;
					break;
				case RenderingSelectionRule.RenderingMatchType.Prefix:
                    m_rdoRenderingHasPrefix.Checked = true;
                    m_txtVernacularPrefix.Text = m_rule.RenderingMatchPrefix;
					break;
				case RenderingSelectionRule.RenderingMatchType.Custom:
			        m_rdoUserDefinedRenderingCriteria.Checked = true;
					m_txtRenderingMatchRegEx.Text = m_rule.RenderingMatchingPattern;
					break;
			}
		}

        private static void FormatExample(Label lblExample, Control rightBoundingPanel)
	    {
	        RichTextBox rtfBoxExample = new RichTextBox();
	        rtfBoxExample.AutoWordSelection = false;
	        rtfBoxExample.BorderStyle = BorderStyle.None;
	        rtfBoxExample.CausesValidation = false;
	        rtfBoxExample.Cursor = Cursors.Arrow;
	        rtfBoxExample.EnableAutoDragDrop = false;
	        rtfBoxExample.Multiline = false;
	        rtfBoxExample.TabStop = false;
	        rtfBoxExample.AllowDrop = false;
	        rtfBoxExample.Margin = new Padding(0, 0, 0, 0);
	        rtfBoxExample.ScrollBars = RichTextBoxScrollBars.None;
	        rtfBoxExample.ShortcutsEnabled = false;
	        rtfBoxExample.WordWrap = false;
	        rtfBoxExample.BackColor = lblExample.BackColor;
	        rtfBoxExample.ForeColor = lblExample.ForeColor;
	        rtfBoxExample.Name = "rtfBoxExample1";
	        rtfBoxExample.Rtf = "{\\rtf1\\ansi\\deff0{\\fonttbl{\\f0\\fnil\\fcharset0 " +
	        lblExample.Font.Name + ";}}\r\n\\viewkind4\\uc1\\pard\\lang9\\f0\\fs" +
	        lblExample.Font.SizeInPoints * 2 + " " +
	        lblExample.Tag + "\\par}";
            ((GroupBox)lblExample.Parent).Controls.Add(rtfBoxExample);
	        rtfBoxExample.Location = new Point(lblExample.Bounds.Right, lblExample.Location.Y);
	        rtfBoxExample.Height = lblExample.Height;
            rtfBoxExample.Width = rightBoundingPanel.Location.X - rtfBoxExample.Location.X - 35;
	        rtfBoxExample.ReadOnly = true;
	    }

	    private static void SetDetails(ComboBox cbo, string details)
		{
			if (IsNullOrEmpty(details))
				cbo.SelectedIndex = -1;
			else
			{
				int index = cbo.FindStringExact(details);
				if (index >= 0 || cbo.DropDownStyle == ComboBoxStyle.DropDownList)
					cbo.SelectedIndex = index;
				cbo.Text = details;
			}
		}
		#endregion

		#region Event handlers
		private void OptionCheckedChanged(object sender, System.EventArgs e)
		{
			RadioButton btn = (RadioButton)sender;
			Panel panel = (Panel)btn.Tag;
			panel.Visible = btn.Checked;
			UpdateStatus();
		}

		private void m_cboSuffix_TextChanged(object sender, EventArgs e)
		{
			m_rule.QuestionMatchSuffix = m_cboSuffix.Text;
			UpdateStatus();
		}

		private void m_pnlSuffixDetails_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlSuffixDetails.Visible)
				m_cboSuffix_TextChanged(m_cboSuffix, e);
		}

		private void m_cboPrefix_TextChanged(object sender, EventArgs e)
		{
			m_rule.QuestionMatchPrefix = m_cboPrefix.Text;
			UpdateStatus();
		}

		private void m_pnlPrefixDetails_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlPrefixDetails.Visible)
				m_cboPrefix_TextChanged(m_cboSuffix, e);
		}

		private void m_cboPrecedingWord_TextChanged(object sender, EventArgs e)
		{
			m_rule.QuestionMatchPrecedingWord = m_cboPrecedingWord.Text;
			UpdateStatus();
		}

		private void m_pnlPrecedingWordDetails_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlPrecedingWordDetails.Visible)
				m_cboPrecedingWord_TextChanged(m_cboSuffix, e);
		}

		private void m_cboFollowingWord_TextChanged(object sender, EventArgs e)
		{
			m_rule.QuestionMatchFollowingWord = m_cboFollowingWord.Text;
			UpdateStatus();
		}

		private void m_pnlFollowingWordDetails_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlFollowingWordDetails.Visible)
				m_cboFollowingWord_TextChanged(m_cboSuffix, e);
		}

		private void m_txtQuestionMatchRegEx_TextChanged(object sender, EventArgs e)
		{
			m_rule.QuestionMatchingPattern = m_txtQuestionMatchRegEx.Text;
			UpdateStatus();
		}

		private void m_pnlUserDefinedRuleDetails_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlUserDefinedRuleDetails.Visible)
				m_txtQuestionMatchRegEx_TextChanged(m_txtQuestionMatchRegEx, e);
		}

		private void m_txtVernacularSuffix_TextChanged(object sender, EventArgs e)
		{
			m_rule.RenderingMatchSuffix = m_txtVernacularSuffix.Text;
			UpdateStatus();
		}

		private void m_pnlVernacularSuffix_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlVernacularSuffix.Visible)
				m_txtVernacularSuffix_TextChanged(m_txtVernacularSuffix, e);
		}

		private void m_txtVernacularPrefix_TextChanged(object sender, EventArgs e)
		{
			m_rule.RenderingMatchPrefix = m_txtVernacularPrefix.Text;
			UpdateStatus();
		}

		private void m_pnlVernacularPrefix_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlVernacularPrefix.Visible)
				m_txtVernacularPrefix_TextChanged(m_txtVernacularPrefix, e);
		}

		private void m_txtRenderingMatchRegEx_TextChanged(object sender, EventArgs e)
		{
			m_rule.RenderingMatchingPattern = m_txtRenderingMatchRegEx.Text;
			UpdateStatus();
		}

		private void m_pnlUserDefinedRenderingMatch_VisibleChanged(object sender, EventArgs e)
		{
			if (m_pnlUserDefinedRenderingMatch.Visible)
				m_txtRenderingMatchRegEx_TextChanged(m_txtRenderingMatchRegEx, e);
		}

		private void VernacularTextBox_Enter(object sender, EventArgs e)
		{
			m_selectKeyboard?.Invoke(true);
		}

		private void VernacularTextBox_Leave(object sender, EventArgs e)
		{
			m_selectKeyboard?.Invoke(false);
		}

		private void m_txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string name = m_txtName.Text.Trim();
			if (name.Length == 0)
			{
				MessageBox.Show(LocalizationManager.GetString("RulesWizardDlg.NameRequired", 
					"Name is required."), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Cancel = true;
			}
			else if (!ValidateName(name))
			{
				MessageBox.Show(Format(LocalizationManager.GetString("RulesWizardDlg.NameMustBeUnique",
					"There is already a rule named {0}. Rule names must be unique."), name),
					Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Cancel = true;
			}
			m_rule.Name = name;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (!m_rule.Valid)
			{
				string errorPartA = (m_rule.ErrorMessageQ != null) ?
					LocalizationManager.GetString("RulesWizardDlg.InvalidQuestionCondition",
						"Invalid condition for determining when rule applies.") +
						Environment.NewLine + m_rule.ErrorMessageQ :
					LocalizationManager.GetString("RulesWizardDlg.InvalidRenderingCondition",
						"Invalid condition for determining which rendering to select.") +
						Environment.NewLine + m_rule.ErrorMessageR;

				switch (MessageBox.Show(errorPartA + Environment.NewLine +
					LocalizationManager.GetString("RulesWizardDlg.FixConditionNow",
						"This rule can be saved but will not be used until the error is fixed. Would you like to fix it now?"),
					LocalizationManager.GetString("RulesWizardDlg.InvalidRegularExpressionCaption",
						"Regular Expression Invalid"),
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop))
				{
					case DialogResult.Yes:
						return;
					case DialogResult.No:
						DialogResult = DialogResult.OK; break;
					case DialogResult.Cancel:
						DialogResult = DialogResult.Cancel; break;
				}
			}
			else
				DialogResult = DialogResult.OK;
			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Process.Start(m_help);
		}
		#endregion

		#region Private helper methods
		private void UpdateStatus()
		{
			m_lblDescription.Text = m_rule.Description;
			btnOk.Enabled = m_txtName.Text != Empty &&
				m_rule.QuestionMatchCriteriaType != RenderingSelectionRule.QuestionMatchType.Undefined;
		}
		#endregion
	}
}