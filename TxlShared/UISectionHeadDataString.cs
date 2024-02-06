// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UISectionHeadDataString.cs
// ---------------------------------------------------------------------------------------------
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UISectionHeadDataString : UIDataString
	{
		private readonly ISectionInfo m_section;
		public UISectionHeadDataString(ISectionInfo section)
		{
			m_section = section;
		}
		
		public override string SourceUIString => m_section.Heading;
		public override LocalizableStringType Type => LocalizableStringType.SectionHeading;
		public override int StartRef => m_section.StartRef;
		public override int EndRef => m_section.EndRef;
		public override string Question => null;

		public override int GetHashCode()
		{
			return m_section.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is UISectionHeadDataString other)
				return SourceUIString == other.SourceUIString;
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