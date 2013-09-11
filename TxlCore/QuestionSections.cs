// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: QuestionSections.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	#region class QuestionSections
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("ComprehensionCheckingQuestions", Namespace = "", IsNullable = false)]
	public class QuestionSections
	{
		[XmlElement("Section", typeof(Section), Form = XmlSchemaForm.Unqualified)]
		public Section[] Items { get; set; }
	}

	#endregion

	#region class Section
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Section
	{
		[XmlAttribute("heading")]
		public string Heading { get; set; }

		[XmlAttribute("scrref")]
		public string ScriptureReference { get; set; }

		[XmlAttribute("startref")]
		public int StartRef { get; set; }

		[XmlAttribute("endref")]
		public int EndRef { get; set; }

		[XmlArray(Form = XmlSchemaForm.Unqualified), XmlArrayItem("Category", typeof(Category), IsNullable = false)]
		public Category[] Categories { get; set; }
	}

	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Category
	{
		[XmlAttribute("overview")]
		public bool IsOverview { get; set; }

		[XmlAttribute("type")]
		public string Type { get; set; }

	    private List<Question> m_questions;
		[XmlElement]
        public List<Question> Questions { get { return m_questions ?? (m_questions = new List<Question>()); } }
	}
	#endregion
}
