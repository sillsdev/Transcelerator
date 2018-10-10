// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UISimpleDataString.cs
// ---------------------------------------------------------------------------------------------
using System.Diagnostics;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UISimpleDataString : UIDataString
	{
		public UISimpleDataString(string source, LocalizableStringType type)
		{
			Debug.Assert(type == LocalizableStringType.Category || type == LocalizableStringType.NonLocalizable);
			SourceUIString = source;
			Type = type;
		}

		public override string SourceUIString { get; }
		public override LocalizableStringType Type { get; }
		public override int StartRef => 0;
		public override int EndRef => 0;
		public override string Question => null;

		public override int GetHashCode()
		{
			return Type.GetHashCode() * 397 ^ SourceUIString.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is UISimpleDataString other)
				return SourceUIString == other.SourceUIString && Type == other.Type;
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