// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: NewQuestionDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class NewQuestionDlg : Form
	{
		private readonly Question m_baseQuestion;
		internal Question NewQuestion
		{
			get
			{
				return new Question(m_baseQuestion, chkNoEnglish.Checked ? null : m_txtEnglish.Text,
					m_txtAnswer.Text);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NewQuestionDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public NewQuestionDlg(Question baseQuestion)
		{
			m_baseQuestion = baseQuestion;
			InitializeComponent();

			lblReference.Text = String.Format(lblReference.Text, m_baseQuestion.ScriptureReference);
		}

		private void m_txtEnglish_TextChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = (m_txtEnglish.Text.Length > 0 || chkNoEnglish.Checked);
		}

		private void chkNoEnglish_CheckedChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = (m_txtEnglish.Text.Length > 0 || chkNoEnglish.Checked);
			m_txtEnglish.Enabled = !chkNoEnglish.Checked;
		}
	}
}