// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2017, SIL International.
// <copyright from='2011' to='2017' company='SIL International'>
//		Copyright (c) 2017, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: EditQuestion.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

		internal string ModifiedPhrase
		{
			get { return m_txtModified.Text; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EditQuestion"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public EditQuestionDlg(TranslatablePhrase question, List<String> existingQuestionsForRef)
		{
			m_question = question;
			m_existingQuestionsForRef = existingQuestionsForRef;
			InitializeComponent();
			m_txtOriginal.Text = question.OriginalPhrase;
			m_txtModified.Text = question.PhraseInUse;
			Question q = question.QuestionInfo;
			if (q != null && q.AlternateForms != null)
			{
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditQuestionDlg));

				foreach (string alternateForm in q.AlternateForms.Skip(1))
				{
					RadioButton newBtn = new RadioButton();
					m_pnlAlternatives.Controls.Add(newBtn);
					resources.ApplyResources(newBtn, "m_rdoAlternative");
					m_pnlAlternatives.SetFlowBreak(newBtn, true);
					newBtn.Text = alternateForm;
					newBtn.CheckedChanged += m_rdoAlternative_CheckedChanged;
				}
				m_rdoAlternative.Text = q.AlternateForms.First();
				return;
			}
			m_pnlAlternatives.Hide();
		}

		private void m_txtModified_TextChanged(object sender, EventArgs e)
		{
			m_lblQuestionAlreadyExists.Visible = m_txtModified.Text.Length > 0 && m_existingQuestionsForRef.Contains(m_txtModified.Text);
			btnOk.Enabled = (m_txtModified.Text.Length > 0 || m_question.IsUserAdded) && m_txtModified.Text != m_question.PhraseInUse && !m_lblQuestionAlreadyExists.Visible;
			if (!m_pnlAlternatives.Visible)
				return;
			foreach (RadioButton rdoAlt in m_pnlAlternatives.Controls.OfType<RadioButton>())
				rdoAlt.Checked = (rdoAlt.Text == m_txtModified.Text);
		}

		private void m_rdoAlternative_CheckedChanged(object sender, EventArgs e)
		{
			var btn = ((RadioButton) sender);
			if (btn.Checked)
				m_txtModified.Text = btn.Text;
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			foreach (RadioButton rdoAlt in m_pnlAlternatives.Controls.OfType<RadioButton>())
				rdoAlt.Checked = false;
			m_txtModified.Text = m_txtOriginal.Text;
		}
	}
}