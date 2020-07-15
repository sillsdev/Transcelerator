// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 201, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: SectionInfo.cs
// ---------------------------------------------------------------------------------------------

namespace SIL.Transcelerator
{
	public interface ISectionInfo
	{
		string Heading { get; }
		int StartRef { get; }
		int EndRef { get; }
		string ToString();
	}
}
