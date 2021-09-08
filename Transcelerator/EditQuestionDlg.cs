// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
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
using System.Linq;
using System.Windows.Forms;
using SIL.IO;
using SIL.Transcelerator.Localization;
using static System.String;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
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
				if (m_txtModified.Text == m_txtOriginal.Text)
					return m_txtOriginal.Tag as string ?? m_txtModified.Text;
				if (m_pnlAlternatives.Visible)
				{
					var selectedAlt = m_pnlAlternatives.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
					if (selectedAlt != null)
						return (string)selectedAlt.Tag;
				}
				return m_txtModified.Text;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EditQuestion"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public EditQuestionDlg(TranslatablePhrase question, List<string> existingQuestionsForRef,
			LocalizationsFileAccessor dataLocalizer)
		{
			m_question = question;
			m_existingQuestionsForRef = existingQuestionsForRef;
			InitializeComponent();
			if (dataLocalizer == null)
			{
				m_txtOriginal.Text = question.OriginalPhrase;
				m_txtModified.Text = question.PhraseInUse;
			}
			else
			{
				m_txtOriginal.Tag = question.OriginalPhrase;
				m_txtOriginal.Text = dataLocalizer.GetLocalizedDataString(new UIQuestionDataString(question.PhraseKey, true, false), out _);
				m_txtModified.Text = question.OriginalPhrase == question.PhraseInUse ? m_txtOriginal.Text : question.PhraseInUse;
			}
			var alternativeForms = question.QuestionInfo?.Alternatives;
			if (alternativeForms != null && alternativeForms.Any(f => !f.Hide))
			{
				var resources = new System.ComponentModel.ComponentResourceManager(typeof(EditQuestionDlg));
				var firstOneToShow = -1;
				for (var i = 0; i < alternativeForms.Length; i++)
				{
					if (alternativeForms[i].Hide)
						continue;
					if (firstOneToShow == -1)
					{
						firstOneToShow = i;
						continue;
					}
					var newBtn = new RadioButton();
					m_pnlAlternatives.Controls.Add(newBtn);
					resources.ApplyResources(newBtn, "m_rdoAlternative");
					m_pnlAlternatives.SetFlowBreak(newBtn, true);
					newBtn.Padding = m_rdoAlternative.Padding;
					newBtn.AutoSize = true;
					InitializeRadioButton(newBtn, i, dataLocalizer);
					newBtn.CheckedChanged += m_rdoAlternative_CheckedChanged;
				}
				Debug.Assert(firstOneToShow >= 0);
				InitializeRadioButton(m_rdoAlternative, firstOneToShow, dataLocalizer);
			}
			else
				m_pnlAlternatives.Hide();

			m_help = FileLocationUtilities.GetFileDistributedWithApplication(true, "docs", "editingquestions.htm");
			HelpButton = !IsNullOrEmpty(m_help);
		}

		private void InitializeRadioButton(RadioButton btn, int index, LocalizationsFileAccessor dataLocalizer)
		{
			var alternateForm = m_question.QuestionInfo.Alternatives[index].Text;
			if (dataLocalizer == null)
				btn.Text = alternateForm;
			else
			{
				btn.Text = dataLocalizer.GetLocalizedDataString(new UIAlternateDataString(m_question.QuestionInfo, index, false), out _);
				btn.Tag = alternateForm;

			}
			btn.Checked = m_txtModified.Text == btn.Text;
		}

		private void m_txtModified_TextChanged(object sender, EventArgs e)
		{
			m_lblQuestionAlreadyExists.Visible = m_txtModified.Text.Length > 0 && m_existingQuestionsForRef.Contains(m_txtModified.Text);
			btnOk.Enabled = (m_txtModified.Text.Length > 0 || m_question.IsUserAdded) && m_txtModified.Text != m_question.PhraseInUse && !m_lblQuestionAlreadyExists.Visible;
			if (!m_pnlAlternatives.Visible)
				return;
			foreach (RadioButton rdoAlt in m_pnlAlternatives.Controls.OfType<RadioButton>())
				rdoAlt.Checked = (rdoAlt.Text == m_txtModified.Text || m_txtModified.Text.Equals(rdoAlt.Tag));
		}

		private void m_rdoAlternative_CheckedChanged(object sender, EventArgs e)
		{
			var btn = (RadioButton)sender;
			if (btn.Checked)
				m_txtModified.Text = btn.Text;
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			foreach (RadioButton rdoAlt in m_pnlAlternatives.Controls.OfType<RadioButton>())
				rdoAlt.Checked = false;
			m_txtModified.Text = m_txtOriginal.Text;
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
			Process.Start(m_help);
		}
	}
}