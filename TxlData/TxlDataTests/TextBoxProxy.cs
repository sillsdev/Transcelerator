// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2025' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;

namespace SIL.Transcelerator
{
	/// -------------------------------------------------------------------------------------------
	/// <summary>
	/// Test implementation (mimicking the behavior of a Windows TextBox) of ITextWithSelection.
	/// This facilitates testing of <see cref="SubstitutionMatchGroup"/>
	/// (<see cref="SubstitutionMatchGroupTests"/>) without depending on Windows Forms (or other UI
	/// implementation).
	/// </summary>
	/// <remarks>If/when some future UI toolkit/widget is used in place of Windows Forms, it would
	/// be worth verifying that this class behaves in a way that serves as an adequate proxy for
	/// that environment as well. It would need to be wrapped to adapt it to the interface, so it
	/// wouldn't matter if its native members were named differently, but if its actual behavior
	/// were fundamentally different, that could mean that agnostic methods would not actually
	/// work.
	/// </remarks>
	/// -------------------------------------------------------------------------------------------
	public class TextBoxProxy : ITextWithSelection
	{
		private int m_selectionStart;
		private string m_text = "";
		private int m_selectionLength;

		public string Text
		{
			get => m_text;
			set
			{
				m_text = value ?? "";
				SelectionStart = 0;
				SelectionLength = 0;
			}
		}

		public int SelectionStart
		{
			get => m_selectionStart;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("The assigned value is less than zero.");
				if (value > Text.Length)
					value = Text.Length;

				// Adjust selection length if it would now exceed bounds
				if (value + m_selectionLength > Text.Length)
					m_selectionLength = Text.Length - value;

				m_selectionStart = value;
			}
		}

		public int SelectionLength
		{
			get => m_selectionLength;
			set
			{
				if (value < 0)
					value = 0;

				if (m_selectionStart + value > Text.Length)
					value = Text.Length - m_selectionStart;

				m_selectionLength = value;
			}
		}

		public string SelectedText =>
			Text.Length == 0 || m_selectionLength == 0 ? string.Empty :
				Text.Substring(m_selectionStart, m_selectionLength);
	}
}