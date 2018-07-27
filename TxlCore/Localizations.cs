// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: Localizations.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	public enum LocalizableStringType
	{
		Undefined,
		SectionHeading,
		Category,
		Question,
		Answer,
		Note,
	}

	#region Localizations
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("Localizations", Namespace = "", IsNullable = false)]
	public class Localizations
	{
		[XmlElement("String", typeof(LocalizableString), Form = XmlSchemaForm.Unqualified)]
		public LocalizableString[] Items { get; set; }
	}
	#endregion

	#region class LocalizableString
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class LocalizableString
	{
		// The type is currently just informative. It's not used for anything functional, but it
		// could be helpful in prioritizing strings for localization
		[XmlAttribute]
		[DefaultValue(LocalizableStringType.Undefined)]
		public LocalizableStringType Type { get; set; }

		[XmlElement("English")]
		public string English { get; set; }

		[XmlElement("Localization")]
		public Localization Localization { get; set; }
	}
	#endregion

	#region class Category
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Localization
	{
		public string Text => Occurrences?.FirstOrDefault()?.LocalizedString;

		[XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("Occurrence", typeof(Occurrence), IsNullable = false)]
		public List<Occurrence> Occurrences { get; set; }

		internal Occurrence GetMatchingOverrideIfAny(QuestionKey key)
		{
			return Occurrences?.SingleOrDefault(o => key.CompareRefs(o.StartRef, o.EndRef) == 0);
		}
	}

	#endregion

	#region class Category
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Occurrence
	{
		[XmlAttribute("startref")]
		public int StartRef { get; set; }

		[XmlAttribute("endref")]
		public int EndRef { get; set; }

		[XmlText]
		public string LocalizedString { get; set; }
	}
	#endregion
}
