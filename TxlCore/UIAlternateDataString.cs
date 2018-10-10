// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIAlternateDataString.cs
// ---------------------------------------------------------------------------------------------
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UIAlternateDataString : UIDataString
	{
		private readonly Question m_baseQuestion;
		private readonly int m_index;

		public UIAlternateDataString(Question baseQuestion, int index, bool useAnyAlternate = true)
		{
			m_baseQuestion = baseQuestion;
			m_index = index;
			UseAnyAlternate = useAnyAlternate;
		}

		public override string SourceUIString => m_baseQuestion.AlternateForms[m_index];
		public override LocalizableStringType Type => LocalizableStringType.Alternate;
		public override int StartRef => m_baseQuestion.StartRef;
		public override int EndRef => m_baseQuestion.EndRef;
		public override string Question => m_baseQuestion.Text;
		public override bool UseAnyAlternate { get; }

		public override int GetHashCode()
		{
			return m_baseQuestion.GetHashCode() * 397 ^ m_index;
		}

		public override bool Equals(object obj)
		{
			if (obj is UIAlternateDataString other)
				return m_baseQuestion == other.m_baseQuestion && m_index == other.m_index && UseAnyAlternate == other.UseAnyAlternate;
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ScriptureReference + "-" + SourceUIString;
		}
	}
}