// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UITestDataString.cs
// ---------------------------------------------------------------------------------------------
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UITestDataString : UIDataString
	{
		public UITestDataString(string source, LocalizableStringType type,
			int startRef = 0, int endRef = 0, string question = null, bool useAnyAlt = true)
		{
			SourceUIString = source;
			Type = type;
			StartRef = startRef;
			EndRef = endRef;
			Question = question ?? (type == LocalizableStringType.Question ? source : null);
			UseAnyAlternate = useAnyAlt;
		}

		public override string SourceUIString { get; }
		public override LocalizableStringType Type { get; }
		public override int StartRef { get; }
		public override int EndRef { get; }
		public override string Question { get; }
		public override bool UseAnyAlternate { get; }

		public override int GetHashCode()
		{
			return Type.GetHashCode() * 397 ^ SourceUIString.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is UITestDataString other)
				return SourceUIString == other.SourceUIString && Type == other.Type &&
					StartRef == other.StartRef && EndRef == other.EndRef && Question == other.Question;
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