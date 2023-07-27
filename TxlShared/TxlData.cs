// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2013' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using SIL.Scripture;

namespace SIL.Transcelerator
{
    public static class TxlData
    {
	    public const string kPluginName = "Transcelerator";
	    public const string kQuestionsFilename = "TxlQuestions.xml";
	    public const string kKeyTermRulesFilename = "keyTermRules.xml";
	    public const string kQuestionWordsFilename = "TxlQuestionWords.xml";
	    public const string kEmailAddress = "transcelerator_feedback@sil.org";

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
