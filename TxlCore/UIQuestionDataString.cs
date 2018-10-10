// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIQuestionDataString.cs
// ---------------------------------------------------------------------------------------------
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UIQuestionDataString : UIDataString
	{
		private readonly IQuestionKey m_questionKey;
		private readonly string m_modifiedString;

		public UIQuestionDataString(IQuestionKey questionKey, bool original, bool useAnyAlternate)
		{
			m_questionKey = questionKey;
			m_modifiedString = original || questionKey.PhraseInUse == questionKey.Text ? null : questionKey.PhraseInUse;
			UseAnyAlternate = useAnyAlternate;
		}

		public override string SourceUIString => m_modifiedString ?? Question;
		public override LocalizableStringType Type => LocalizableStringType.Question;
		public override int StartRef => m_questionKey.StartRef;
		public override int EndRef => m_questionKey.EndRef;
		public override string Question => m_questionKey.Text;
		public override bool UseAnyAlternate { get; }

		public override int GetHashCode()
		{
			var result = m_questionKey.GetHashCode();
			if (m_modifiedString != null)
				result = result * 397 ^ m_modifiedString.GetHashCode();
			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is UIQuestionDataString other)
				return m_questionKey == other.m_questionKey && m_modifiedString == other.m_modifiedString && UseAnyAlternate == other.UseAnyAlternate;
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