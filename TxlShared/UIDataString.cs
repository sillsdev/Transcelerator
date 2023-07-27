// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIDataString.cs
// ---------------------------------------------------------------------------------------------
using SIL.Scripture;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public abstract class UIDataString : IRefRange
	{
		public abstract string SourceUIString { get; }
		public abstract LocalizableStringType Type { get; }
		public string ScriptureReference => StartRef > 0 ? BCVRef.MakeReferenceString(StartRef, EndRef, ".", "-") : null;
		public abstract int StartRef { get; }
		public abstract int EndRef { get; }
		public abstract string Question { get; }
		public virtual bool UseAnyAlternate => false;

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