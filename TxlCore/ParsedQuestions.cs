// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ParsedQuestions.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
    [Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
    [XmlRoot("ParsedQuestions", Namespace = "", IsNullable = false)]
    public class ParsedQuestions
	{
        [XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("KeyTermMatchSurrogate", typeof(KeyTermMatchSurrogate), IsNullable = false)]
        public KeyTermMatchSurrogate[] KeyTerms { get; set; }

        [XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("TranslatablePart", typeof(string), IsNullable = false)]
        public string[] TranslatableParts { get; set; }

        [XmlElement("Sections", typeof(QuestionSections), Form = XmlSchemaForm.Unqualified)]
		public QuestionSections Sections { get; set; }
	}
}
