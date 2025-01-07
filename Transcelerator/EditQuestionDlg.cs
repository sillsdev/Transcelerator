// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: EditQuestion.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;
using SIL.Transcelerator.Localization;
using static System.String;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog box for choosing an alternative or typing a custom version of a question.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class EditQuestionDlg : Form
	{
		private readonly TranslatablePhrase m_question;
		private readonly List<string> m_existingQuestionsForRef;
		private readonly string m_help;

		internal string ModifiedPhrase
		{
			get
			{
				if (m_rdoCustom.Checked)
					return m_txtModified.Text;

				return m_tableLayoutPanelMain.Controls
					.OfType<RadioButton>().FirstOrDefault(c => c.Checked)?.Tag as string ??
					m_txtModified.Text;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EditQuestion"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public EditQuestionDlg(TranslatablePhrase question,
			IReadOnlyCollection<TranslatablePhrase> existingQuestionsForRef,
			LocalizationsFileAccessor dataLocalizer, QuestionGroupType groupType)
		{
			m_question = question;
			m_existingQuestionsForRef = existingQuestionsForRef.Select(p => p.PhraseInUse)
				.Union(existingQuestionsForRef.Select(p => dataLocalizer.GetLocalizedDataString(new UIQuestionDataString(p.PhraseKey, true, false)).Data)).ToList();
			InitializeComponent();

			if (groupType != QuestionGroupType.AlternativeSetOfQuestions)
				m_lblGroupedSetQuestionWarning.Hide();
			if (groupType != QuestionGroupType.AlternativeSingleQuestions)
				m_lblAltQuestionWarning.Hide();

			if (dataLocalizer == null)
			{
				m_rdoOriginal.Text = question.OriginalPhrase;
				m_txtModified.Text = question.PhraseInUse;
			}
			else
			{
				m_rdoOriginal.Tag = question.OriginalPhrase;
				m_rdoOriginal.Text = dataLocalizer.GetLocalizedDataString(new UIQuestionDataString(question.PhraseKey, true, false));
				m_txtModified.Text = question.OriginalPhrase == question.PhraseInUse ? m_rdoOriginal.Text : question.PhraseInUse;
			}

			if (m_txtModified.Text != m_rdoOriginal.Text)
			{
				// For now, assume custom. Below, we might discover it actually matches a known
				// alternative.
				m_rdoCustom.Checked = true;
			}

			ActiveControl = m_txtModified;
			var alternativeForms = question.QuestionInfo?.Alternatives;
			if (alternativeForms != null && alternativeForms.Any(f => !f.Hide))
			{
				var resources = new ComponentResourceManager(typeof(EditQuestionDlg));
				Tuple<string, LocalizedDataString> firstOneToShow = null;
				int insertAt = m_tableLayoutPanelMain.GetRow(m_rdoAlternative) + 1;
				int colForAltBtn = m_tableLayoutPanelMain.GetColumn(m_rdoAlternative);
				for (var i = 0; i < alternativeForms.Length; i++)
				{
					if (alternativeForms[i].Hide)
						continue;
					var alternateForm = alternativeForms[i].Text;
					var localizedAlt = dataLocalizer == null ? new LocalizedDataString(alternateForm, LocalizationManager.kDefaultLang, false) :
						dataLocalizer.GetLocalizedDataString(new UIAlternateDataString(m_question.QuestionInfo, i, false));
					if (localizedAlt.Omit)
						continue;
					if (firstOneToShow == null)
					{
						firstOneToShow = new Tuple<string, LocalizedDataString>(alternateForm, localizedAlt);
						continue;
					}
					var newBtn = new RadioButton
					{
						Padding = m_rdoAlternative.Padding,
						Margin = m_rdoAlternative.Margin,
						AutoSize = true
					};
					m_tableLayoutPanelMain.RowCount++;
					ShiftExistingControlsDown(insertAt);
					m_tableLayoutPanelMain.LayoutSettings.RowStyles.Insert(insertAt,
						new RowStyle(SizeType.AutoSize));
					m_tableLayoutPanelMain.Controls.Add(newBtn, colForAltBtn, insertAt);
					resources.ApplyResources(newBtn, "m_rdoAlternative");
					InitializeRadioButton(newBtn, alternateForm, localizedAlt);
					newBtn.CheckedChanged += HandleOriginalOrAlternativeCheckedChanged;
				}
				Debug.Assert(firstOneToShow != null);
				InitializeRadioButton(m_rdoAlternative, firstOneToShow.Item1, firstOneToShow.Item2);
			}
			else
			{
				m_lblAlternatives.Hide();
				m_rdoAlternative.Hide();
			}

			m_help = TxlPlugin.GetHelpFile("editingquestions");
			HelpButton = !IsNullOrEmpty(m_help);

			// Don't do these things in Designer. The first two prevent GetControlFromPosition from
			// finding them. The event handler causes unnecessary work to be done prematurely.
			m_lblQuestionAlreadyExists.Visible = false;
			m_txtModified.Enabled = m_rdoCustom.Checked;
			m_txtModified.TextChanged += HandleModifiedQuestionTextChanged;
		}

		private void ShiftExistingControlsDown(int insertAt)
		{
			for (int iRow = m_tableLayoutPanelMain.RowCount - 2; iRow > insertAt; iRow--)
			{
				for (int iCol = 0; iCol < m_tableLayoutPanelMain.ColumnCount; iCol++)
				{
					var ctrl = m_tableLayoutPanelMain.GetControlFromPosition(iCol, iRow);
					if (ctrl != null)
						m_tableLayoutPanelMain.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(iCol, iRow + 1));
				}
			}
		}

		private void InitializeRadioButton(RadioButton btn, string englishAltText, LocalizedDataString alt)
		{
			btn.Text = alt;
			btn.Tag = englishAltText;
			btn.Checked = m_txtModified.Text == btn.Text;
		}

		private void HandleModifiedQuestionTextChanged(object sender, EventArgs e)
		{
			m_lblQuestionAlreadyExists.Visible = m_txtModified.Text.Length > 0 && m_existingQuestionsForRef.Contains(m_txtModified.Text);
			m_btnOk.Enabled = (m_txtModified.Text.Length > 0 || m_question.IsUserAdded) && m_txtModified.Text != m_question.PhraseInUse && !m_lblQuestionAlreadyExists.Visible;
			var rdoAlt = m_lblAlternatives.Visible ? m_tableLayoutPanelMain.Controls
				.OfType<RadioButton>().FirstOrDefault(c => c != m_rdoCustom &&
					(c.Text == m_txtModified.Text || m_txtModified.Text.Equals(c.Tag))) : null;
			if (rdoAlt != null)
				rdoAlt.Checked = true;
		}

		private void HandleOriginalOrAlternativeCheckedChanged(object sender, EventArgs e)
		{
			var btn = (RadioButton)sender;
			if (btn.Checked)
			{
				SetModifiedTextBoxAvailability(false);
				m_txtModified.TextChanged -= HandleModifiedQuestionTextChanged;
				m_txtModified.Text = btn.Text;
				m_txtModified.TextChanged += HandleModifiedQuestionTextChanged;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the Click event of the Help button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleHelpButtonClick(object sender, CancelEventArgs e)
		{
			HandleHelpRequest(sender, new HelpEventArgs(MousePosition));
		}

		private void HandleHelpRequest(object sender, HelpEventArgs args)
		{
			if (!IsNullOrEmpty(m_help))
				Process.Start(m_help);
		}

		private void HandleCustomCheckedChanged(object sender, EventArgs e)
		{
			SetModifiedTextBoxAvailability(m_rdoCustom.Checked);
		}

		private void SetModifiedTextBoxAvailability(bool enable)
		{
			if (enable)
				m_txtModified.Enabled = true;
			else if (IsHandleCreated && !m_txtModified.ContainsFocus)
				m_txtModified.Enabled = false;
		}

		private void HandlePanelLayout(object sender, LayoutEventArgs e)
		{
			AdjustLayout();
		}

		private void AdjustLayout()
		{
			m_tableLayoutPanelMain.SuspendLayout();

			// Get the total height of the TableLayoutPanel's content
			if (m_tableLayoutPanelMain.AutoSize)
				m_txtModified.Height = m_txtModified.MinimumSize.Height;
			// I know that in theory, m_pnlVScroll.ClientSize.Width has already excluded the
			// vertical scrollbar width, but without that (or some kind of small fudge factor),
			// m_txtModified remains too wide and the horizontal scrollbar appears.
			if (m_pnlVScroll.VerticalScroll.Visible &&
			    m_txtModified.Width + m_txtModified.Margin.Horizontal +
			    SystemInformation.VerticalScrollBarWidth > m_pnlVScroll.ClientSize.Width)
			{
				m_txtModified.Width = m_pnlVScroll.ClientSize.Width -
					m_txtModified.Margin.Horizontal - SystemInformation.VerticalScrollBarWidth;
			}

			int contentHeight = m_tableLayoutPanelMain.GetPreferredSize(new Size(m_tableLayoutPanelMain.Width, 0)).Height;

			if (contentHeight > m_pnlVScroll.ClientSize.Height)
			{
				// Switch to AutoSize mode and use scrollbars
				m_tableLayoutPanelMain.AutoSize = true;
				m_tableLayoutPanelMain.Dock = DockStyle.None;
			}
			else
			{
				// Switch to Fill mode
				m_tableLayoutPanelMain.AutoSize = false;
				m_tableLayoutPanelMain.Dock = DockStyle.Fill;
			}

			m_tableLayoutPanelMain.ResumeLayout();
		}
	}
}