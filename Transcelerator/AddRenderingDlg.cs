// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: AddRenderingDlg.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog box to allow the user to add a rendering in Transcelerator only (i.e., does not
	/// propagate back to Paratext)
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class AddRenderingDlg : Form
	{
		private readonly Action<bool> m_selectKeyboard;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:AddRenderingDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AddRenderingDlg(Action<bool> selectKeyboard)
		{
			m_selectKeyboard = selectKeyboard;
			InitializeComponent();
			selectKeyboard?.Invoke(true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnClosed(EventArgs e)
		{
			m_selectKeyboard?.Invoke(false);
			 base.OnClosed(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Rendering => DialogResult == DialogResult.OK ? m_txtRendering.Text : null;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the TextChanged event of the m_txtRendering control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_txtRendering_TextChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = !string.IsNullOrEmpty(m_txtRendering.Text);
		}
	}
}