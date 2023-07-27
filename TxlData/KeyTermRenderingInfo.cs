// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: KeyTermRenderingInfo.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	#region class KeyTermRenderingInfo
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to hold information about how to select the best rendering for a key
	/// term (supports XML serialization)
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("KeyTermRenderingInfo")]
	public class KeyTermRenderingInfo
	{
		#region XML attributes
		/// --------------------------------------------------------------------------------
		/// <summary>
        /// Gets or sets the id (see KeyTermSurrogate.id).
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("id")]
		public string TermId { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the original phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("default")]
		public string PreferredRendering { get; set; }
		#endregion

		#region XML elements
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// List of additional renderings (i.e., not supplied from external source)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlElement("AdditionalRenderings")]
		public List<string> AddlRenderings = new List<string>();
		#endregion

		#region Constructors
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyTermRenderingInfo"/> class, needed
		/// for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public KeyTermRenderingInfo()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyTermRenderingInfo"/> class.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public KeyTermRenderingInfo(string termId, string bestRendering)
		{
			TermId = termId;
			PreferredRendering = bestRendering;
		}
		#endregion
	}
	#endregion
}