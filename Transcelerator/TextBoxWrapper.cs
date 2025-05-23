// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2025' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	public class TextBoxWrapper<T> : ITextWithSelection where T : TextBoxBase
	{
		private readonly T m_textBoxControl;

		public TextBoxWrapper(T textBoxControl)
		{
			m_textBoxControl = textBoxControl;
		}

		public T Control => m_textBoxControl;

		public string Text
		{
			get => m_textBoxControl.Text;
			set => m_textBoxControl.Text = value;
		}

		public int SelectionStart
		{
			get => m_textBoxControl.SelectionStart;
			set => m_textBoxControl.SelectionStart = value;
		}

		public int SelectionLength
		{
			get => m_textBoxControl.SelectionLength;
			set => m_textBoxControl.SelectionLength = value;
		}

		public string SelectedText
		{
			get => m_textBoxControl.SelectedText;
		}
	}
}