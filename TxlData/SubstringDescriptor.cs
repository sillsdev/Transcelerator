// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2021' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: SubstringDescriptor.cs
// --------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Simple class to allow methods to pass an offset and a length in order to describe a
	/// substring.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SubstringDescriptor
	{
		public int Start { get; set; }
		public int Length { get; set; }

		public int EndOffset => Start + Length;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(int start, int length)
		{
			Start = start;
			Length = length;
		}
	}
}