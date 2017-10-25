// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: QuestionKey.cs
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Simple base class to encapsulate the information needed to (more-or-less uniquely)
	/// identify a Scripture checking question.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public abstract class QuestionKey : IComparable
	{
		private string m_text;

		[XmlAttribute("scrref")]
		public abstract string ScriptureReference { get; set; }
		[XmlAttribute("startref")]
		public abstract int StartRef { get; set; }
		[XmlAttribute("endref")]
		public abstract int EndRef { get; set; }

		/// <summary>Text of the question in COMPOSED form</summary>
		[XmlElement("Q", Form = XmlSchemaForm.Unqualified)]
		public virtual string Text
		{
			get { return m_text; }
			set { m_text = value.Normalize(NormalizationForm.FormC); }
		}

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Gets the text to use for processing & comparison purposes
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [XmlIgnore]
	    public virtual string PhraseInUse { get { return m_text; } }

	    public bool Matches(QuestionKey other)
		{
			return CompareTo(other) == 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ScriptureReference + "-" + Text;
		}

		public int CompareRefs(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));
			if (this == obj)
				return 0;
			var other = obj as QuestionKey;
			if (other == null)
				throw new ArgumentException(nameof(obj), $"Attempt to compare question key ({this}) to an unexpected object ({obj}).");
			var startRefComparison = StartRef.CompareTo(other.StartRef);
			if (startRefComparison != 0)
				return startRefComparison;
			if (EndRef == StartRef)
				return other.EndRef == other.StartRef ? 0 : 1; // A key that represents a range always sorts before one that represents a single verse.
			if (other.EndRef == other.StartRef)
				return -1;
			return EndRef.CompareTo(other.EndRef);
		}

		public bool IsAtOrBeforeReference(QuestionKey keyToUseForReference, bool inclusive)
		{
			var compareResult = CompareRefs(keyToUseForReference);
			return inclusive ? compareResult <= 0 : compareResult < 0;
		}

		public int CompareTo(object obj)
		{
			var refsComparison = CompareRefs(obj);
			if (refsComparison != 0)
				return refsComparison;

			// REVIEW: If both keys are for the same single verse or reference range, we can't say they are equal (because their text is different), but the comparison
			// is a little bit arbitrary. For built-in questions or insertions/additions hanging off them, this shouldn't be a problem, since they stay in their natural order.
			// Only questions added that do not correspond to any existing question could be compared. But generally, only one such "free" addition can happen for any given
			// reference (range). Any subsequent additions would be an insertion before that one or an addition after. Possible exception would be if two users independently
			// added questions.
			return  String.Compare(Text, ((QuestionKey)obj).Text, StringComparison.Ordinal);
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Trivial implementation of QuestionKey for a question that is not pegged to a particular
	/// Scripture reference
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SimpleQuestionKey : QuestionKey
	{
	    private const int s_startRef = 01001001;
	    private const int s_endRef = 66022021;

	    #region Constructors

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SimpleQuestionKey(string text)
		{
			Text = text;
		}
		#endregion

		#region Overrides of QuestionKey
		public override string ScriptureReference
		{
			get { return string.Empty; }
			set { throw new NotImplementedException(); }
		}

		public override int StartRef
		{
			get { return s_startRef; }
			set { throw new NotImplementedException(); }
		}

		public override int EndRef
		{
			get { return s_endRef;}
			set { throw new NotImplementedException(); }
		}
		#endregion
	}
}
