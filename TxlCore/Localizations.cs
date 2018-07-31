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

	#region class AlternateForm
	[Serializable]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class LocalizableStringForm
	{
		[XmlElement("English")]
		public string English { get; set; }

		[XmlElement("Localization")]
		public Localization Localization { get; set; }

		// Note: There is always the slight possibility that the actual localization happens to be an
		// exact match on the English (e.g., if locale is a dialect/creole of English or if it is a
		// 1- or 2-word string and just happens to be an exact cognate). In that case, we can't really
		// distinguish between a true localization and an English string copied over into the localized
		// text field. But this should be extraordinarily unlikely for any string that actually has
		// alternate forms.
		public bool HasBeenLocalized(QuestionKey key) => English != Localization.GetLocalizedString(key);
	}
	#endregion

	#region class LocalizableString
	[Serializable]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class LocalizableString : LocalizableStringForm
	{
		// The type is currently just informative. It's not used for anything functional, but it
		// could be helpful in prioritizing strings for localization
		[XmlAttribute]
		[DefaultValue(LocalizableStringType.Undefined)]
		public LocalizableStringType Type { get; set; }

		[XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("Alt", typeof(LocalizableStringForm), IsNullable = true)]
		public List<LocalizableStringForm> Alternates { get; set; }

		internal String GetLocalizedString(QuestionKey key)
		{
			if (Localization?.Text == null)
				return null;

			bool foundLocalizedVersion = HasBeenLocalized(key);
			Localization loc = key.PhraseInUse == key.Text || foundLocalizedVersion ? Localization : null;
			if (Alternates!= null && Alternates.Any())
			{
				LocalizableStringForm matchingAlt = null;
				foreach (var alternate in Alternates)
				{
					if (!foundLocalizedVersion && alternate.HasBeenLocalized(key))
					{
						loc = alternate.Localization;
						if (alternate.English == key.PhraseInUse)
							break;
						foundLocalizedVersion = true;
					}
					if (matchingAlt == null && alternate.English == key.PhraseInUse)
					{
						if (alternate.HasBeenLocalized(key))
						{
							loc = alternate.Localization;
							break;
						}
						matchingAlt = alternate;
					}
				}
				// If we didn't find an exact match that was localized, we prefer a localized one over an
				// exact match. But if none of the alternates was localized, then we go with the exact match, if any.
				if (!foundLocalizedVersion && matchingAlt != null)
					loc = matchingAlt.Localization;
			}

			return loc?.GetLocalizedString(key);
		}
	}
	#endregion

	#region class Category
	[Serializable]
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
		
		internal string GetLocalizedString(QuestionKey key)
		{
			return GetMatchingOverrideIfAny(key)?.LocalizedString ?? Text;
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
