// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIAnswerOrNoteDataString.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UIAnswerOrNoteDataString : UIDataString
	{
		private readonly Question m_baseQuestion;
		private readonly int m_index;
		public UIAnswerOrNoteDataString(Question baseQuestion, LocalizableStringType type, int index)
		{
			Debug.Assert(type == LocalizableStringType.Answer || type == LocalizableStringType.Note);
			m_baseQuestion = baseQuestion;
			Type = type;
			m_index = index;
		}

		public override string SourceUIString => Type == LocalizableStringType.Answer ?
			m_baseQuestion.Answers[m_index] :
			m_baseQuestion.Notes[m_index];
		public override LocalizableStringType Type { get; }
		public override int StartRef => m_baseQuestion.StartRef;
		public override int EndRef => m_baseQuestion.EndRef;
		public override string Question => m_baseQuestion.Text;

		public override int GetHashCode()
		{
			var result = Type.GetHashCode();
			result = result * 397 ^ m_baseQuestion.GetHashCode();
			return result * 397 ^ m_index;
		}

		public override bool Equals(object obj)
		{
			if (obj is UIAnswerOrNoteDataString other)
				return Type == other.Type && m_baseQuestion == other.m_baseQuestion && m_index == other.m_index;
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