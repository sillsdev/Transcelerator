// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: QuestionAlternativeOverrides.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.TxlMasterQuestionPreProcessor
{
	#region class QuestionAlternativeOverrides
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("QuestionAlternativeOverrides", Namespace = "", IsNullable = false)]
	public class QuestionAlternativeOverrides
	{
		[XmlElement("Alternative", typeof(Alternative), Form = XmlSchemaForm.Unqualified)]
		public Alternative[] Items { get; set; }
	}

	#endregion

	#region class Alternative
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Alternative
	{
		[XmlElement("Original", Form = XmlSchemaForm.Unqualified)]
		public string Text { get; set; }

		[XmlElement("Alt", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
		public string[] AlternateForms { get; set; }
	}
	#endregion
}
