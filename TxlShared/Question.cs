// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
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
using System.Linq;
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
	public class Question : QuestionKey, ICloneable
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
		[DefaultValue(false)]
        public bool IsExcluded { get; set; }

		[XmlAttribute("user")]
		[DefaultValue(false)]
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
		
        [XmlAttribute("group")]
        public string Group { get; set; }

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
						m_text = Id;
					else
                        throw new ArgumentException("Factory-supplied questions must not be empty.");
			    }
				else
				{
					EnsureKeySetWhenChangingText(value);
					m_text = value.Trim();
				}
			}
		}

        [XmlElement("A", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public string[] Answers { get; set; }

        [XmlElement("Note", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
		public string[] Notes { get; set; }

        [XmlElement("Alternative", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public AlternativeForm[] Alternatives { get; set; }

        public IEnumerable<string> AlternativeForms => Alternatives?.Select(a => a.Text);

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
		/// for XML serialization, tests, and the question pre-processor. Never use this for
		/// user-added questions
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Question()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Constructor to make a new user-added Question (used only in tests).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Question(IQuestionKey baseQuestion, string newQuestion, string answer) :
			this(baseQuestion.ScriptureReference, baseQuestion.StartRef, 
			baseQuestion.EndRef, newQuestion, answer, newQuestion)
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
		public Question(string scrRefAsString, int startRef, int endRef, string newQuestion, string answer, string key = null) :
			base(newQuestion, scrRefAsString, startRef, endRef, key ?? newQuestion ?? kGuidPrefix + Guid.NewGuid())
		{
			IsUserAdded = true;
			if (m_text == null)
				m_text = Id;

			if (!string.IsNullOrEmpty(answer))
				Answers = new[] { answer };
		}

		public Question Clone() => (Question)((ICloneable)this).Clone();

		object ICloneable.Clone()
		{
			var clone = (Question)MemberwiseClone();
			if (Answers != null)
			{
				clone.Answers = new string[Answers.Length];
				for (var index = 0; index < Answers.Length; index++)
					clone.Answers[index] = Answers[index];
			}
			if (Notes != null)
			{
				clone.Answers = new string[Notes.Length];
				for (var index = 0; index < Notes.Length; index++)
					clone.Notes[index] = Notes[index];
			}
			if (AlternativeForms != null)
			{
				clone.Alternatives = new AlternativeForm[Alternatives.Length];
				for (var index = 0; index < Alternatives.Length; index++)
					clone.Alternatives[index] = Alternatives[index].Clone();
			}
			
			if (InsertedQuestionBefore != null)
				clone.InsertedQuestionBefore = InsertedQuestionBefore.Clone();
			if (AddedQuestionAfter != null)
				clone.AddedQuestionAfter = AddedQuestionAfter.Clone();

			return clone;
		}
	}
	#endregion
}
