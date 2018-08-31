// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2011' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
		public bool IsUserAdded
		{
			get => m_isUserAdded;
			set
			{
				if (!value && m_isUserAdded)
					throw new InvalidOperationException("An existing user-added question cannot be set back to a factory question. This setter is only designed to support deserialization and should not be used outside of this class or in tests.");
				m_isUserAdded = value;
			}
		}

		[XmlAttribute("modified")]
        public string ModifiedPhrase { get; set; }

        [XmlElement("Q", Form = XmlSchemaForm.Unqualified)]
        public override string Text
		{
			get => m_text; // Note: this is not base.m_text
			set
			{
			    if (String.IsNullOrWhiteSpace(value))
			    {
				    if (m_text == null)
				    {
					    // This is getting set for the very first time, presumably in the constructor.
						// This is only allowed in constructors that will be setting IsUserAdded to
						// true anyway.
					    IsUserAdded = true;
				    }
				    if (IsUserAdded)
			            m_text = kGuidPrefix + Guid.NewGuid();
                    else
                        throw new ArgumentException("Factory-supplied questions must not be empty.");
			    }
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
		private bool m_isUserAdded;

		[XmlElement]
        public List<ParsedPart> ParsedParts => m_parsedParts ?? (m_parsedParts = new List<ParsedPart>());

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
	    public bool IsParsable => !IsExcluded && !string.IsNullOrEmpty(m_text) && !m_text.StartsWith(kGuidPrefix);

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the question/phrase to use for processing & comparison purposes (either the
        /// original text or a modified form of it).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [XmlIgnore]
        public override string PhraseInUse => IsExcluded ? Text : (ModifiedPhrase ?? Text);

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
		public Question(Question baseQuestion, string newQuestion, string answer) :
			this(baseQuestion.ScriptureReference, baseQuestion.StartRef, 
			baseQuestion.EndRef, newQuestion, answer)
		{
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Constructor to make a new Question "from scratch" (used only in tests)
		/// </summary>
		/// --------------------------------------------------------------------------------
		protected Question(string scrRefAsString, int startRef, int endRef, string newQuestion) :
			base(newQuestion, scrRefAsString, startRef, endRef)
		{
			if (IsUserAdded)
				throw new InvalidOperationException("This constructor is only for use in tests and is not intended for creating user-added questions.");
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Constructor to make a new (user-added) Question.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Question(string scrRefAsString, int startRef, int endRef, string newQuestion, string answer) :
			base(newQuestion, scrRefAsString, startRef, endRef)
		{
			IsUserAdded = true;

			if (!string.IsNullOrEmpty(answer))
				Answers = new[] { answer };
		}
	}
	#endregion
}
