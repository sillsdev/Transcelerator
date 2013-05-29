// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: KeyTermMatchSurrogate.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Class to represent one or more key terms that have a common (possibly partial) gloss
	/// (supports XML serialization)
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("KeyTerm")]
	public class KeyTermMatchSurrogate
	{
		#region XML attributes
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the id, which is the (space-separated) list of (lowercase) words
		/// which comprise the gloss (in the language of the master list of comprehension
		/// questions) that is common to all key terms united under this surrogate.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("id")]
		public string TermId { get; set; }
		#endregion

		#region XML elements
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// List of underlying term IDs in the original biblical languages, covered by this
		/// surrogate
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlElement("BiblicalTermIds")]
        public List<string> BiblicalTermIds = new List<string>();
		#endregion

		#region Constructors
		/// --------------------------------------------------------------------------------
		/// <summary>
        /// Initializes a new instance of the <see cref="KeyTermMatchSurrogate"/> class, 
        /// needed for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public KeyTermMatchSurrogate()
		{
		}

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTermMatchSurrogate"/> class.
        /// </summary>
        /// --------------------------------------------------------------------------------
        public KeyTermMatchSurrogate(string termId, params string[] biblicalTermIds)
        {
            TermId = termId;
            BiblicalTermIds.AddRange(biblicalTermIds);
        }
		#endregion
	}
}