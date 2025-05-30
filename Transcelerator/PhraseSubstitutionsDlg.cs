// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using L10NSharp.XLiffUtils;
using SIL.Windows.Forms.Extensions;
using SIL.Windows.Forms.Widgets;
using static System.String;
using static SIL.Transcelerator.SubstitutionMatchGroup;
using static SIL.Windows.Forms.Extensions.ControlExtensions.ErrorHandlingAction;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog box where user can specify adjustments to the original question for the purpose
	/// of parsing it into parts.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class PhraseSubstitutionsDlg : Form
	{
		protected ITextWithSelection TextControl { get; set; }

		protected TextBoxBase TextBoxControlImpl
		{
			get => m_textBoxControlImpl;
			set
			{
				TextControl = value == null ? null : new TextBoxWrapper(value);
				m_textBoxControlImpl = value;
			}
		}

		private readonly CustomDropDown m_regexMatchDropDown = new CustomDropDown();
		private readonly CustomDropDown m_regexReplaceDropDown = new CustomDropDown();
		private readonly string m_help;
		protected string RemoveItem { get; private set; }
		private string m_entireMatch;
		private TextBoxBase m_textBoxControlImpl;

		#region Constructor and initialization methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PhraseSubstitutionsDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public PhraseSubstitutionsDlg(IEnumerable<Substitution> phraseSubstitutions,
			IEnumerable<string> previewTestPhrases, int iDefaultTestPhrase)
		{
			InitializeComponent();
			
			m_regexMatchHelper.CreateControl();
			m_btnMatchSingleWord.Width = m_regexMatchHelper.Width - m_btnMatchSingleWord.Left * 2;
			m_dataGridView.Controls.Remove(m_regexMatchHelper);
			m_regexMatchDropDown.AutoClose = false;
			m_regexMatchDropDown.AutoCloseWhenMouseLeaves = false;
			m_regexReplaceDropDown.AutoClose = false;
			m_regexReplaceDropDown.AutoCloseWhenMouseLeaves = false;
			m_regexMatchDropDown.AddControl(m_regexMatchHelper);
			m_regexReplaceDropDown.AddControl(m_regexReplacementHelper);
			m_cboMatchGroup.Items.Clear();

			HandleStringsLocalized();
			LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;

			foreach (Substitution substitution in phraseSubstitutions)
			{
				m_dataGridView.Rows.Add(substitution.MatchingPattern, substitution.Replacement,
					substitution.IsRegex);
			}

			m_cboPreviewQuestion.Items.AddRange(previewTestPhrases.ToArray());
			if (m_cboPreviewQuestion.Items.Count > 0)
				m_cboPreviewQuestion.SelectedIndex = iDefaultTestPhrase;

			m_txtMatchPrefix.Tag = AffixType.Prefix;
			m_txtMatchSuffix.Tag = AffixType.Suffix;

			m_help = TxlPlugin.GetHelpFile("adjustments");
			HelpButton = !IsNullOrEmpty(m_help);
		}

		private void HandleStringsLocalized(ILocalizationManager lm = null)
		{
			if (lm != null && lm != TxlPlugin.PrimaryLocalizationManager)
				return;
			lblInstructions.Text = Format(lblInstructions.Text,
				colReplacement.HeaderText, colIsRegEx.HeaderText, colMatch.HeaderText);
			// The remove item will always be there in production, just not in tests.
			RemoveItem = m_cboMatchGroup.Items.Count > 0 ? m_cboMatchGroup.Items[0] as string : "Remove";
			m_entireMatch = LocalizationManager.GetString("PhraseSubstitutionDlg.EntireMatch",
				"Entire match");
		}
		#endregion

		#region Properties
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
		/// Gets the substitutions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal IEnumerable<Substitution> Substitutions => 
			m_dataGridView.Rows.Cast<DataGridViewRow>().Select(GetSubstitutionForRow)
				.Where(sub => !Substitution.IsNullOrEmpty(sub));

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match count value from the expression covered, partially covered, or
		/// immediately preceding the selected text in the edit control for the data grid view
		/// cell currently being edited. If there is no explicit match count expression, then
		/// this returns 1. Intended to be used only when the current column is the "Match"
		/// column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int ExistingMatchCountValue => GetExistingMatchCountValue(TextControl);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the prefix, if any, from the expression covered, partially covered, or
		/// immediately preceding the selected text in the edit control for the data grid view
		/// cell currently being edited. If there is no prefix expression, then this returns an
		/// empty string. Intended to be used only when the current column is the "Match" column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string ExistingPrefix =>
			GetExistingAffix((AffixType)m_txtMatchPrefix.Tag, TextControl);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the suffix, if any, from the expression covered, partially covered, or
		/// immediately preceding the selected text in the edit control for the data grid view
		/// cell currently being edited. If there is no suffix expression, then this returns an
		/// empty string. Intended to be used only when the current column is the "Match" column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string ExistingSuffix =>
			GetExistingAffix((AffixType)m_txtMatchSuffix.Tag, TextControl);
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match group, if any, from the expression covered, partially covered, or
		/// immediately preceding the selected text in the edit control for the data grid view
		/// cell currently being edited. If there is no match group expression, then this
		/// returns an empty string. Intended to be used only when the current column is the
		/// "Replacement" column.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal string ExistingMatchGroup
		{
			get
			{
				var group = GetExistingMatchGroup(TextControl.Text,
					TextControl.SelectionStart);
				return group == null ? Empty :
					group.Type == MatchGroupType.EntireMatch ? m_entireMatch :
					group.Group;
			}
		}

		#endregion

		#region Event handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the EditingControlShowing event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DataGridViewEditingControlShowingEventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			var currentRow = m_dataGridView.CurrentRow;
			if (currentRow == null)
				return;

			TextBoxControlImpl = IsRegEx(m_dataGridView.CurrentRow) ?
				e.Control as DataGridViewTextBoxEditingControl : null;

			if (TextBoxControlImpl == null)
				return;

			var cellDisplayRect = m_dataGridView.GetCellDisplayRectangle(
				m_dataGridView.CurrentCell.ColumnIndex, m_dataGridView.CurrentCell.RowIndex, true);

			if (m_dataGridView.CurrentCell.ColumnIndex == colMatch.Index)
			{
				m_regexMatchDropDown.Show(m_dataGridView, cellDisplayRect.Left, cellDisplayRect.Bottom + 1);
			}
			else
			{
				var matchGroups = GetMatchGroups((string)currentRow.Cells[colMatch.Index].Value);
				if (matchGroups.Length > 0)
				{
					m_cboMatchGroup.Items.AddRange(matchGroups);
					var sGroup = ExistingMatchGroup;
					m_cboMatchGroup.Items.Insert(0, sGroup.Length > 0 ? RemoveItem : Empty);
					m_regexReplaceDropDown.Show(m_dataGridView, cellDisplayRect.Left, cellDisplayRect.Bottom + 1);
				}
				else
					return;
			}

			TextBoxControlImpl.HideSelection = false;
			TextBoxControlImpl.KeyUp += txtControl_KeyUp;
			TextBoxControlImpl.TextChanged += txtControl_TextChanged;
			TextBoxControlImpl.TextChanged += UpdatePreview;
			TextBoxControlImpl.MouseClick += txtControl_MouseClick;
		}

		void txtControl_TextChanged(object sender, EventArgs e)
		{
			if (sender is TextBoxBase tb && tb.Focused)
				UpdateRegExHelperControls(nameof(txtControl_MouseClick));
		}

		private void txtControl_MouseClick(object sender, MouseEventArgs e)
		{
			UpdateRegExHelperControls(nameof(txtControl_MouseClick));
		}

		private void txtControl_KeyUp(object sender, KeyEventArgs e)
		{
			// ENHANCE: When the version of SIL.Windows.Forms that has KeyExtensions is released,
			// this should be replaced with a call to KeyExtensions.IsNavigationKey.
			if (IsNavigationKey(e.KeyCode, e.Control))
				UpdateRegExHelperControls($"{nameof(txtControl_KeyUp)} - {e.KeyData}");
		}

		/// <summary>
		/// Determines whether the specified key is considered a navigation key.
		/// </summary>
		/// <param name="key">The key (or key-combination) to examine.</param>
		/// <param name="ctrlPressed">If <c>true</c>, the letter <c>Keys.A</c> will be treated as a
		/// navigation key (even if the <paramref name="key"/> value does not explicitly include
		/// <c>Keys.Control</c>). In contexts where Ctrl-A is not a shortcut for Select All or that
		/// behavior is not relevant, pass false or omit this optional parameter.</param>
		/// <remarks>
		/// This is a TEMPORARY stand-in for SIL.Windows.Forms.KeyExtensions.IsNavigationKey (which
		/// is not yet available).
		/// If <paramref name="key"/> represents the combination Ctrl-A (i.e.,
		/// <c>Keys.Control | Keys.A</c>), but <paramref name="ctrlPressed"/> is <c>false</c>,
		/// this method will return <c>false</c>.
		/// </remarks>
		private static bool IsNavigationKey(Keys key, bool ctrlPressed = false)
		{
			switch (key & Keys.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
				case Keys.PageDown:
				case Keys.PageUp:
					return true;
				case Keys.A:
					return ctrlPressed;
				default:
					return false;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellEndEdit event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (m_regexMatchDropDown.Visible)
				m_regexMatchDropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
			else if (m_regexReplaceDropDown.Visible)
			{
				m_regexReplaceDropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
				m_cboMatchGroup.Items.Clear();
			}
			else
				return;
			
			TextBoxControlImpl.KeyUp -= txtControl_KeyUp;
			TextBoxControlImpl.TextChanged -= txtControl_TextChanged;
			TextBoxControlImpl.TextChanged -= UpdatePreview;
			TextBoxControlImpl.MouseClick -= txtControl_MouseClick;
			TextBoxControlImpl = null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the m_btnMatchSingleWord control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_btnMatchSingleWord_Click(object sender, EventArgs e)
		{
			int selStart = TextControl.SelectionStart;
			ReplaceSelectedText(kContiguousLettersMatchExpr, TextControl);
			TextControl.SelectionStart = selStart + kContiguousLettersMatchExpr.Length;
			TextControl.SelectionLength = 0;
			TextBoxControlImpl.Focus();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the TextChanged event for the suffixes or prefix TextBox control.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		protected void SuffixOrPrefixChanged(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			ResetTextAndSelectionForUpdatedAffix(textBox.Text, (AffixType)textBox.Tag, TextControl,
				out var existingAffixDeleted);
			if (!existingAffixDeleted)
				UpdateRegExHelperControls(nameof(SuffixOrPrefixChanged));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the ValueChanged event of the m_numTimesToMatch control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_numTimesToMatch_ValueChanged(object sender, EventArgs e)
		{
			TextBoxControlImpl.TextChanged -= txtControl_TextChanged;
			UpdateMatchCount((int)m_numTimesToMatch.Value, TextControl);
			TextBoxControlImpl.TextChanged += txtControl_TextChanged;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the SelectedIndexChanged event of the m_cboMatchGroup control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		void m_cboMatchGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedItemText = m_cboMatchGroup.Text;
			if (selectedItemText == Empty)
				return;
			TextBoxControlImpl.TextChanged -= txtControl_TextChanged;
			UpdateMatchGroup(selectedItemText);
			int i = m_cboMatchGroup.SelectedIndex;
			m_cboMatchGroup.SelectedIndexChanged -= m_cboMatchGroup_SelectedIndexChanged;
			m_cboMatchGroup.Items[0] = selectedItemText != RemoveItem ? RemoveItem : Empty;
			m_cboMatchGroup.SelectedIndex = i;
			m_cboMatchGroup.SelectedIndexChanged += m_cboMatchGroup_SelectedIndexChanged;
			TextBoxControlImpl.TextChanged += txtControl_TextChanged;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the RowEnter event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			btnUp.Enabled = e.RowIndex > 0;
			btnDown.Enabled = e.RowIndex < m_dataGridView.RowCount - 2; // Need to subtract one for the "New" row
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the preview.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void UpdatePreview(object sender, EventArgs e)
		{
			UpdatePreview();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the CellValueChanged event of the m_dataGridView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;
			UpdatePreview();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Swaps the current row's substitution rule with the previous one
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		private void btnUp_Click(object sender, EventArgs e)
		{
			if (m_dataGridView.CurrentRow == null)
				return;

			if (m_dataGridView.IsCurrentCellInEditMode)
				m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

			var prevRow = m_dataGridView.Rows[m_dataGridView.CurrentRow.Index - 1];
			Substitution prevSubstitution = GetSubstitutionForRow(prevRow);

			prevRow.Cells[colMatch.Index].Value = m_dataGridView.CurrentRow.Cells[colMatch.Index].Value;
			prevRow.Cells[colReplacement.Index].Value = m_dataGridView.CurrentRow.Cells[colReplacement.Index].Value;
			prevRow.Cells[colIsRegEx.Index].Value = m_dataGridView.CurrentRow.Cells[colIsRegEx.Index].Value;
			prevRow.Cells[colMatchCase.Index].Value = m_dataGridView.CurrentRow.Cells[colMatchCase.Index].Value;

			m_dataGridView.CurrentRow.Cells[colMatch.Index].Value = prevSubstitution.MatchingPattern;
			m_dataGridView.CurrentRow.Cells[colReplacement.Index].Value = prevSubstitution.Replacement;
			m_dataGridView.CurrentRow.Cells[colIsRegEx.Index].Value = prevSubstitution.IsRegex;
			m_dataGridView.CurrentRow.Cells[colMatchCase.Index].Value = prevSubstitution.MatchCase;

			m_dataGridView.CurrentCell = prevRow.Cells[m_dataGridView.CurrentCell.ColumnIndex];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Swaps the current row's substitution rule with the following one
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		private void btnDown_Click(object sender, EventArgs e)
		{
			if (m_dataGridView.CurrentRow == null)
				return;

			if (m_dataGridView.IsCurrentCellInEditMode)
				m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

			var nextRow = m_dataGridView.Rows[m_dataGridView.CurrentRow.Index + 1];
			Substitution nextSubstitution = GetSubstitutionForRow(nextRow);

			nextRow.Cells[colMatch.Index].Value = m_dataGridView.CurrentRow.Cells[colMatch.Index].Value;
			nextRow.Cells[colReplacement.Index].Value = m_dataGridView.CurrentRow.Cells[colReplacement.Index].Value;
			nextRow.Cells[colIsRegEx.Index].Value = m_dataGridView.CurrentRow.Cells[colIsRegEx.Index].Value;
			nextRow.Cells[colMatchCase.Index].Value = m_dataGridView.CurrentRow.Cells[colMatchCase.Index].Value;

			m_dataGridView.CurrentRow.Cells[colMatch.Index].Value = nextSubstitution.MatchingPattern;
			m_dataGridView.CurrentRow.Cells[colReplacement.Index].Value = nextSubstitution.Replacement;
			m_dataGridView.CurrentRow.Cells[colIsRegEx.Index].Value = nextSubstitution.IsRegex;
			m_dataGridView.CurrentRow.Cells[colMatchCase.Index].Value = nextSubstitution.MatchCase;

			m_dataGridView.CurrentCell = nextRow.Cells[m_dataGridView.CurrentCell.ColumnIndex];
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

		private void m_txtMatchSuffix_Enter(object sender, EventArgs e)
		{
			var selectedTextInGrid = TextControl?.SelectedText;
			if (m_txtMatchPrefix.Text.Length > 0 && selectedTextInGrid != null &&
			    selectedTextInGrid.Equals(m_txtMatchPrefix.Text))
			{
				TextControl.SelectionStart += selectedTextInGrid.Length;
				TextControl.SelectionLength = 0;
			}
		}

		private void m_txtMatchPrefix_Enter(object sender, EventArgs e)
		{
			var selectedTextInGrid = TextControl?.SelectedText;
			if (m_txtMatchSuffix.Text.Length > 0 && selectedTextInGrid != null &&
			    selectedTextInGrid.Equals(m_txtMatchSuffix.Text))
			{
				TextControl.SelectionLength = 0;
			}
		}
		#endregion

		#region Private/protected helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the regex preview for the given row.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdatePreview()
		{
			string sample = m_cboPreviewQuestion.Text;
			for (int index = 0; index < m_dataGridView.Rows.Count; index++)
			{
				var row = m_dataGridView.Rows[index];
				Substitution sub;
				if (TextControl == null || m_dataGridView.CurrentCellAddress.Y != index)
					sub = GetSubstitutionForRow(row);
				else
				{
					string match = (m_dataGridView.CurrentCell.ColumnIndex == colMatch.Index)
						? TextControl.Text
						: row.Cells[colMatch.Index].Value as string;
					string replacement = (m_dataGridView.CurrentCell.ColumnIndex == colReplacement.Index)
						? TextControl.Text
						: row.Cells[colReplacement.Index].Value as String;
					sub = GetSubstitutionForRow(match, replacement, row);
				}
				if (sub.Valid)
				{
					var sResult = sub.Apply(sample);
					if (sResult == sample)
					{
						row.Cells[colPreviewResult.Index].Style = new DataGridViewCellStyle
						{
							ForeColor = Color.Goldenrod
						};
						row.Cells[colPreviewResult.Index].Value = LocalizationManager.GetString(
							"QuestionAdjustmentsDlg.RuleDidNotChangeResult", "No Change");
					}
					else
					{
						row.Cells[colPreviewResult.Index].Style = null;
						row.Cells[colPreviewResult.Index].Value = sResult;
						sample = sResult;
					}
				}
				else
				{
					if ((index == m_dataGridView.RowCount - 1 && IsNullOrEmpty(sub.MatchingPattern)) ||
					    sub.ErrorType == Substitution.RegExErrorType.None)
					{
						// Don't display error message in the "New" row.
						row.Cells[colPreviewResult.Index].Value = Empty;
					}
					else
					{
						row.Cells[colPreviewResult.Index].Style = new DataGridViewCellStyle
						{
							ForeColor = Color.Red
						};
						switch (sub.ErrorType)
						{
							case Substitution.RegExErrorType.Empty:
								row.Cells[colPreviewResult.Index].Value = 
									LocalizationManager.GetString("PhraseSubstitutionsDlg.EmptyRegEx",
										"Nothing to match.");
								break;
							case Substitution.RegExErrorType.Exception:
								row.Cells[colPreviewResult.Index].Value = sub.RegExError.Message;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the max match count control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateRegExHelperControls(string context)
		{
			if (m_regexMatchDropDown.Visible)
			{
				this.SafeInvoke(() =>
				{
					UpdateMaxMatchCountControl();
					m_txtMatchPrefix.TextChanged -= SuffixOrPrefixChanged;
					if (!m_txtMatchPrefix.Text.Equals(ExistingPrefix, StringComparison.Ordinal))
						m_txtMatchPrefix.Text = ExistingPrefix;
					m_txtMatchPrefix.TextChanged += SuffixOrPrefixChanged;
					m_txtMatchSuffix.TextChanged -= SuffixOrPrefixChanged;
					if (!m_txtMatchSuffix.Text.Equals(ExistingSuffix, StringComparison.Ordinal))
						m_txtMatchSuffix.Text = ExistingSuffix;
					m_txtMatchSuffix.TextChanged += SuffixOrPrefixChanged;
				}, context, IgnoreAll);
			}
			else if (m_regexReplaceDropDown.Visible)
			{
				this.SafeInvoke(() =>
				{
					string sExisting = ExistingMatchGroup;
					m_cboMatchGroup.SelectedIndexChanged -= m_cboMatchGroup_SelectedIndexChanged;
					m_cboMatchGroup.Items[0] = sExisting.Length > 0 ? RemoveItem : Empty;
					m_cboMatchGroup.SelectedIndex = m_cboMatchGroup.FindStringExact(sExisting);
					m_cboMatchGroup.SelectedIndexChanged += m_cboMatchGroup_SelectedIndexChanged;
				}, context, IgnoreAll);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the max match count control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void UpdateMaxMatchCountControl()
		{
			m_numTimesToMatch.ValueChanged -= m_numTimesToMatch_ValueChanged;
			m_numTimesToMatch.Value = ExistingMatchCountValue;
			m_numTimesToMatch.ValueChanged += m_numTimesToMatch_ValueChanged;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the match group substitution expression currently selected (or near the
		/// selection) in the edit control for the data grid view cell currently being edited,
		/// or inserts a new match group substitution expression if there isn't one already.
		/// </summary>
		/// <param name="sGroup">The group.</param>
		/// ------------------------------------------------------------------------------------
		protected void UpdateMatchGroup(string sGroup)
		{
			var group = sGroup == m_entireMatch ? EntireMatch :
				sGroup == RemoveItem ? RemoveGroup : new SubstitutionMatchGroup(sGroup);

			int insertAt = TextControl.SelectionStart;
			string text = TextControl.Text;

			TextControl.Text = group.UpdateMatchGroup(text, ref insertAt,
				TextControl.SelectionLength, out int length);
			TextControl.SelectionStart = insertAt;
			TextControl.SelectionLength = length;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an array of objects that can be used to describe the regular
		/// expression match groups found in the given expression.
		/// </summary>
		/// <remarks>Actually, all the objects in the array are strings, but they are
		/// returned as an object array because that makes the caller happier.</remarks>
		/// ------------------------------------------------------------------------------------
		protected object[] GetMatchGroups(string matchExpression)
		{
			return SubstitutionMatchGroup.GetMatchGroups(matchExpression).Select(g =>
			{
				switch (g.Type)
				{
					case MatchGroupType.Normal: return (object)g.Group;
					case MatchGroupType.EntireMatch: return m_entireMatch;
					default: throw new ArgumentOutOfRangeException();
				}
			}).ToArray();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified row in the data grid view represents a regular
		/// expression.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool IsRegEx(DataGridViewRow row)
		{
			return (bool)(row.Cells[colIsRegEx.Index].Value ?? false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a Substitution object representing the current state of the given row.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private Substitution GetSubstitutionForRow(DataGridViewRow row)
		{
			return GetSubstitutionForRow(row.Cells[colMatch.Index].Value as string,
				row.Cells[colReplacement.Index].Value as string, row);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a Substitution object representing the current state of the given row.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private Substitution GetSubstitutionForRow(string matchingPattern, string replacement,
			DataGridViewRow row)
		{
			bool matchCase = (bool)(row.Cells[colMatchCase.Index].Value ?? false);
			return new Substitution(matchingPattern, replacement, IsRegEx(row), matchCase);
		}
		#endregion
	}
}