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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using static System.String;

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
		private readonly string m_help;

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

			m_help = TxlPlugin.GetFileDistributedWithApplication("docs", "biblicalterms.htm");
			HelpButton = !IsNullOrEmpty(m_help);
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
	}
}