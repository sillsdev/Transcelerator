// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2018' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ComprehensionCheckingQuestions.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SIL.ComprehensionCheckingData
{
	#region class ComprehensionCheckingQuestions

	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to support XML serialization
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class ComprehensionCheckingQuestionsForBook
	{
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }

		[XmlAttribute("version")]
		public string Version
		{
			get => "1.1";
			set // Setter only used for deserialization
			{
				if (value != null && value != "1.1" && value != "1.0")
					throw new XmlException($"Unexpected version number in localization file: {value}");
			}
		}

		[XmlAttribute("book")]
		public string BookId { get; set; }

		[XmlElement("Question")]
		public List<ComprehensionCheckingQuestion> Questions { get; set; }
	}

	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class ComprehensionCheckingQuestion
	{
		private int m_endChapter;
		private int m_endVerse;
		private string m_id;

		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("id")]
		public string Id
		{
			get => m_id ?? Question.Single(q => q.Lang == "en-US" || q.Lang == "en").Text;
			set => m_id = value; 
		}

		[XmlAttribute("overview")]
		[DefaultValue(false)]
		public bool IsOverview { get; set; }

		[XmlAttribute("startChapter")]
		public int Chapter { get; set; }

		[XmlAttribute("endChapter")]
		public int EndChapter
		{
			get => m_endChapter == 0 ? Chapter : m_endChapter;
			set => m_endChapter = value;
		}

		[XmlIgnore]
		public bool EndChapterSpecified => m_endChapter > 0;

		[XmlAttribute("startVerse")]
		public int StartVerse { get; set; }

		[XmlAttribute("endVerse")]
		public int EndVerse
		{
			get => m_endVerse == 0 ? StartVerse : m_endVerse;
			set => m_endVerse = value;
		}

		[XmlIgnore]
		public bool EndVerseSpecified => m_endVerse > 0;

		[XmlArray("Q")]
		[XmlArrayItem(typeof(StringAlt))]
		public StringAlt[] Question { get; set; }

		[XmlArray("Answers")]
		[XmlArrayItem(typeof(StringAlt[]), ElementName = "A")]
        public StringAlt[][] Answers { get; set; }

		[XmlArray("Notes")]
		[XmlArrayItem(typeof(StringAlt[]), ElementName = "N")]
		public StringAlt[][] Notes { get; set; }
	}

	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlRoot(ElementName = "Alt")]
	public class StringAlt
	{
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	#endregion
}
