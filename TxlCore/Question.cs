// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: Question.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	#region class Question
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to support XML serialization
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class Question : QuestionKey
	{
		public const string kGuidPrefix = "GUID: ";
		private string m_text;

		[XmlAttribute("scrref")]
		public override string ScriptureReference { get; set; }

		[XmlAttribute("startref")]
		public override int StartRef { get; set; }

        [XmlAttribute("endref")]
        public override int EndRef { get; set; }

        [XmlAttribute("exclude")]
        public bool IsExcluded { get; set; }

        [XmlAttribute("user")]
        public bool IsUserAdded { get; set; }

        [XmlAttribute("modified")]
        public string ModifiedPhrase { get; set; }

        [XmlElement("Q", Form = XmlSchemaForm.Unqualified)]
        public override string Text
		{
			get
            {
                return m_text; // Note: this is not base.m_text
            }
			set
			{
				if (String.IsNullOrEmpty(value))
					m_text = kGuidPrefix + Guid.NewGuid();
				else
					m_text = value.Trim();
			}
		}

        [XmlElement("A", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public string[] Answers { get; set; }

        [XmlElement("Note", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
		public string[] Notes { get; set; }

        [XmlElement("Alternative", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public string[] AlternateForms { get; set; }

        protected List<ParsedPart> m_parsedParts;
        [XmlElement]
        public List<ParsedPart> ParsedParts { get { return m_parsedParts ?? (m_parsedParts = new List<ParsedPart>()); } }

        [XmlIgnore]
        public Question InsertedQuestionBefore { get; set; }

        [XmlIgnore]
        public Question AddedQuestionAfter { get; set; }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
        /// Gets whether the question/phrase can be parsed. If it is excluded or Text is
        /// empty/null or this is a question/phrase with no LWC version (i.e., text is merely a
        /// GUID), then it should not be parsed. 
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [XmlIgnore]
	    public bool IsParsable
	    {
            get
            {
                return !IsExcluded && !string.IsNullOrEmpty(m_text) && !m_text.StartsWith(kGuidPrefix);
            }
	    }

	    /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the question/phrase to use for processing & comparison purposes (either the
        /// original text or a modified form of it).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [XmlIgnore]
        public string PhraseInUse
        {
            get
            {
                if (IsExcluded)
                    throw new InvalidOperationException("Cannot access PhraseInUseFor an excluded question.");
                return ModifiedPhrase ?? Text;
            }
        }

        /// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Question"/> class, needed
		/// for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Question()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Constructor to make a new (user-added) Question.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Question(Question baseQuestion, string newQuestion, string answer)
		{
			ScriptureReference = baseQuestion.ScriptureReference;
			StartRef = baseQuestion.StartRef;
			EndRef = baseQuestion.EndRef;
			Text = newQuestion;
		    IsUserAdded = true;

			if (!string.IsNullOrEmpty(answer))
				Answers = new [] { answer };
		}
	}
	#endregion
}
