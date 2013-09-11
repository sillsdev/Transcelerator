// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: XmlTranslation.cs
// ---------------------------------------------------------------------------------------------
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	#region class XmlTranslation
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to support XML serialization
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("Translation")]
	public class XmlTranslation
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the reference.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("ref")]
		public string Reference { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the phrase key (typically the text of the question in English.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlElement("OriginalPhrase")]
		public string PhraseKey { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the translation.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public string Translation { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlTranslation"/> class, needed
		/// for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public XmlTranslation()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlTranslation"/> class.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public XmlTranslation(TranslatablePhrase tp)
		{
			Reference = tp.PhraseKey.ScriptureReference;
			PhraseKey = tp.PhraseKey.Text;
			Translation = tp.Translation;
		}
	}
	#endregion
}
