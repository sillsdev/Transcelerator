// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ITermRenderingInfo.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Interface to encapsulate what is known about a single occurrence of a key biblical term
	/// and its rendering in a string in the target language.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface ITermRenderingInfo
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the known renderings for the term in the target language.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		IEnumerable<string> Renderings { get; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This will almost always be 0, but if a term occurs more than once in a phrase, this
		/// will be the character offset of the end of the preceding occurrence of the rendering
		/// of the term in the translation string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		int EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm { get; set; }
	}
}
