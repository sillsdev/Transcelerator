// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: AlternativeForm.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	[Serializable]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class AlternativeForm : ICloneable
	{
		/// <summary>
		/// Indicates whether this is an alternative whose purpose is merely for matching
		/// existing translations (as opposed to one that should be displayed to the user
		/// in the Edit question dialog box). This allows for changes to questions to fix
		/// typos, punctuation errors, etc. without invalidating translations the user may
		/// have already done.
		/// </summary>
		[XmlAttribute("hide")]
		public bool Hide { get; set; }

		/// <summary>
		/// Indicates whether this is the alternative which represents the original (as of
		/// October 1 2020) form of this question that can be treated as an immutable key.
		/// Only one AlternativeForm per question should have this flag set.
		/// </summary>
		[XmlAttribute("key")]
		public bool IsKey { get; set; }

		[XmlText]
		public string Text { get; set; }

		public AlternativeForm Clone() => (AlternativeForm)((ICloneable)this).Clone();

		object ICloneable.Clone()
		{
			var clone = (AlternativeForm)MemberwiseClone();
			return clone;
		}
	}
}
