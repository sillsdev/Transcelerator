// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2008' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: MultilingScrBooks.cs
// --------------------------------------------------------------------------------------------
namespace SILUBS.SharedScrUtils
{
	#region class BookLabel
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to associate a book label (name or abbreviation) with a canonical book number.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class BookLabel
	{
		/// <summary></summary>
		public string Label;
		/// <summary></summary>
		public int BookNum;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sLabel">The s label.</param>
		/// <param name="nBookNum">The n book num.</param>
		/// ------------------------------------------------------------------------------------
		public BookLabel(string sLabel, int nBookNum)
		{
			Label = sLabel;
			BookNum = nBookNum;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return Label;
		}
	}
	#endregion
} 
