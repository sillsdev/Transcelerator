// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: RenderingSelectionRulesDlg.cs
//
// Some icons used in this dialog box were downloaded from http://www.iconfinder.com
// The Add Rule icon was developed by Yusuke Kamiyamane and is covered by this Creative Commons
// License: http://creativecommons.org/licenses/by/3.0/
// The Copy Rule icon was developed by Momenticons and is covered by this Creative Commons
// License: http://creativecommons.org/licenses/by/3.0/
// The Delete Rule icon was downloaded from http://www.easyicon.net and was developed by
// Creative Freedom (http://www.creativefreedom.co.uk/free-icons/free-icons-funktional). It is
// covered by the Creative Commons Attribution 3.0 Unported License:
// http://creativecommons.org/licenses/by/3.0/deed.en_GB
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;
using SIL.Windows.Forms;
using static System.String;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class RenderingSelectionRulesDlg : ParentFormBase
	{
		private readonly Action<bool> m_selectKeyboard;
		private readonly string m_help;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:RenderingSelectionRulesDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public RenderingSelectionRulesDlg(IEnumerable<RenderingSelectionRule> rules,
			Action<bool> selectKeyboard)
		{
			m_selectKeyboard = selectKeyboard;
			InitializeComponent();

			toolStrip1.Renderer = new NoToolStripBorderRenderer();

			if (rules != null && rules.Any())
			{
				foreach (RenderingSelectionRule rule in rules)
					m_listRules.Items.Add(rule, !rule.Disabled);
				m_listRules.SelectedIndex = 0;
			}
			btnEdit.Enabled = btnCopy.Enabled = btnDelete.Enabled = (m_listRules.SelectedIndex >= 0);

			m_help = TxlPlugin.GetFileDistributedWithApplication("docs", "renderingselectionrules.htm");
			HelpButton = !IsNullOrEmpty(m_help);
		}

		public IEnumerable<RenderingSelectionRule> Rules
		{
			get { return m_listRules.Items.Cast<RenderingSelectionRule>(); }
		}

		public string ReadonlyAlert
		{
			set
			{
				Text += value;
				if (value != null)
					btnOk.Enabled = false;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnNew control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnNew_Click(object sender, EventArgs e)
		{
			int i = 1;
			var newSelectionRuleNameTemplate = LocalizationManager.GetString(
				"RenderingSelectionRulesDlg.NewSelectionRuleNameTemplate",
				"Selection Rule - {0}", "Param is an integer");
			string name = Format(newSelectionRuleNameTemplate, i);

			Func<string, bool> nameIsUnique = n => !m_listRules.Items.Cast<RenderingSelectionRule>().Any(r => r.Name == n);
			while (!nameIsUnique(name))
				name = Format(newSelectionRuleNameTemplate, ++i);

			RenderingSelectionRule rule = new RenderingSelectionRule(name);

			ShowModalChild(new RulesWizardDlg(rule, true, Word.AllWords, m_selectKeyboard, nameIsUnique), dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					m_listRules.SelectedIndex = m_listRules.Items.Add(rule);
					if (rule.Valid)
						m_listRules.SetItemChecked(m_listRules.SelectedIndex, true);
				}
			});
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnEdit control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnEdit_Click(object sender, EventArgs e)
		{
			RenderingSelectionRule rule = (RenderingSelectionRule)m_listRules.SelectedItem;
			string origName = rule.Name;
			string origQ = rule.QuestionMatchingPattern;
			string origR = rule.RenderingMatchingPattern;
			Func<string, bool> nameIsUnique = n => !m_listRules.Items.Cast<RenderingSelectionRule>().Where(r => r != rule).Any(r => r.Name == n);
			ShowModalChild(new RulesWizardDlg(rule, false, Word.AllWords, m_selectKeyboard, nameIsUnique), dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					if (!rule.Valid)
						m_listRules.SetItemChecked(m_listRules.SelectedIndex, false);

					m_listRules.Invalidate();
				}
				else
				{
					rule.Name = origName;
					rule.QuestionMatchingPattern = origQ;
					rule.RenderingMatchingPattern = origR;
				}
			});
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnCopy control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnCopy_Click(object sender, EventArgs e)
		{
			int iOrigRule = m_listRules.SelectedIndex;
			RenderingSelectionRule origRule = m_listRules.SelectedItem as RenderingSelectionRule;
			RenderingSelectionRule newRule = new RenderingSelectionRule(origRule.QuestionMatchingPattern, origRule.RenderingMatchingPattern);

			var copiedSelectionRuleNameTemplate = LocalizationManager.GetString("RenderingSelectionRulesDlg.CopiedSelectionRuleNameTemplate",
				"{0} - Copy{1}", "Param 0: the original rule name; Param 1: an optional numeric suffix to prevent duplicates (if needed)");
			int i = 1;
			var name = Format(copiedSelectionRuleNameTemplate, origRule.Name, string.Empty);

			Func<string, bool> nameIsUnique = n => !m_listRules.Items.Cast<RenderingSelectionRule>().Any(r => r.Name == n);
			while (!nameIsUnique(name))
				name = Format(copiedSelectionRuleNameTemplate, origRule.Name, "(" + i++ + ")");

			newRule.Name = name;

			ShowModalChild(new RulesWizardDlg(newRule, true, Word.AllWords, m_selectKeyboard, nameIsUnique), dlg =>
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					m_listRules.SelectedIndex = m_listRules.Items.Add(newRule);
					if (newRule.Valid)
						m_listRules.SetItemChecked(m_listRules.SelectedIndex, m_listRules.GetItemChecked(iOrigRule));
				}
			});
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the btnDelete control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnDelete_Click(object sender, EventArgs e)
		{
			int i = m_listRules.SelectedIndex;
			m_listRules.Items.RemoveAt(i);
			if (m_listRules.Items.Count > 0)
				m_listRules.SelectedIndex = m_listRules.Items.Count > i ? i : i - 1;
		}

		private void m_listRules_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RenderingSelectionRule rule = m_listRules.SelectedItem as RenderingSelectionRule;
			btnEdit.Enabled = btnCopy.Enabled = btnDelete.Enabled = (rule != null);
			if (rule != null)
				m_lblDescription.Text = rule.Description;
		}

		private void m_listRules_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			((RenderingSelectionRule)m_listRules.Items[e.Index]).Disabled = e.NewValue == CheckState.Unchecked;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, System.ComponentModel.CancelEventArgs e)
		{
			HandleHelpRequest(sender, new HelpEventArgs(MousePosition));
		}

		private void HandleHelpRequest(object sender, HelpEventArgs args)
		{
			if (!IsNullOrEmpty(m_help))
				Process.Start(m_help);
		}
	}

	public class NoToolStripBorderRenderer : ToolStripProfessionalRenderer
	{
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			// Eat this event.
		}
	}

}