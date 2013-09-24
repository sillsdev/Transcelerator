﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
    public static class TxlCore
    {
        public const string englishVersificationName = "English";
        public const string questionsFilename = "TxlQuestions.xml";

      	/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Parses the given string to get a starting and ending Scripture reference.
		/// </summary>
		/// <param name="sReference">The string-representation of the Scripture reference.</param>
		/// <param name="startRef">The start reference.</param>
		/// <param name="endRef">The end reference.</param>
		/// ------------------------------------------------------------------------------------
		public static void ParseRefRange(this string sReference, out BCVRef startRef, out BCVRef endRef)
        {
            startRef = new BCVRef();
            endRef = new BCVRef();
            BCVRef.ParseRefRange(sReference, ref startRef, ref endRef);
        }
    }
}
