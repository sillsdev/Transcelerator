// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2012' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: QuestionKey.cs
// ---------------------------------------------------------------------------------------------

using System;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	public interface IQuestionKey : IRefRange
	{
		string ScriptureReference { get; }
		/// <summary>Text of the question in COMPOSED form</summary>
		string Text { get; }
		string PhraseInUse { get; }
	}

	public static class QuestionKeyExtensions
	{
		public static int CompareRefs(this IQuestionKey me, object other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));
			if (me == other)
				return 0;
			var otherKey = other as IQuestionKey;
			if (otherKey == null)
				throw new ArgumentException(nameof(other), $"Attempt to compare question key ({me}) to an unexpected object ({other}).");

			return me.CompareRefs(otherKey.StartRef, otherKey.EndRef);
		}

		public static int CompareRefs(this IQuestionKey me, int startRef, int endRef)
		{
			var startRefComparison = me.StartRef.CompareTo(startRef);
			if (startRefComparison != 0)
				return startRefComparison;
			if (me.EndRef == me.StartRef)
				return endRef == startRef ? 0 : 1; // A key that represents a range always sorts before one that represents a single verse.
			if (endRef == startRef)
				return -1;
			return me.EndRef.CompareTo(endRef);
		}

		public static bool Matches(this IQuestionKey me, IQuestionKey other)
		{
			return me.CompareTo(other) == 0;
		}


		public static int CompareTo(this IQuestionKey me, object obj)
		{
			var refsComparison = me.CompareRefs(obj);
			if (refsComparison != 0)
				return refsComparison;

			// REVIEW: If both keys are for the same single verse or reference range, we can't say they are equal (because their text is different), but the comparison
			// is a little bit arbitrary. For built-in questions or insertions/additions hanging off them, this shouldn't be a problem, since they stay in their natural order.
			// Only questions added that do not correspond to any existing question could be compared. But generally, only one such "free" addition can happen for any given
			// reference (range). Any subsequent additions would be an insertion before that one or an addition after. Possible exception would be if two users independently
			// added questions.
			return String.Compare(me.Text, ((IQuestionKey)obj).Text, StringComparison.Ordinal);
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Simple base class to encapsulate the information needed to (more-or-less uniquely)
	/// identify a Scripture checking question.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public abstract class QuestionKey : IQuestionKey, IComparable
	{
		private string m_text;

		[XmlAttribute("scrref")]
		public abstract string ScriptureReference { get; set; }
		[XmlAttribute("startref")]
		public abstract int StartRef { get; set; }
		[XmlAttribute("endref")]
		public abstract int EndRef { get; set; }

		[XmlElement("Q", Form = XmlSchemaForm.Unqualified)]
		public virtual string Text
		{
			get => m_text;
			set => m_text = value.Normalize(NormalizationForm.FormC);
		}

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Gets the text to use for processing & comparison purposes
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [XmlIgnore]
	    public virtual string PhraseInUse => m_text;

		protected QuestionKey()
		{
		}

		protected QuestionKey(string text, string scriptureReference, int startRef, int endRef)
		{
			Text = text;
			ScriptureReference = scriptureReference;
			StartRef = startRef;
			EndRef = endRef;
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

		public bool IsAtOrBeforeReference(IQuestionKey keyToUseForReference, bool inclusive)
		{
			var compareResult = this.CompareRefs(keyToUseForReference);
			return inclusive ? compareResult <= 0 : compareResult < 0;
		}

		public int CompareTo(object obj)
		{
			return QuestionKeyExtensions.CompareTo(this, obj);
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Trivial implementation of QuestionKey for a question that is not pegged to a particular
	/// Scripture reference
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SimpleQuestionKey : IQuestionKey
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SimpleQuestionKey(string text)
		{
			Text = text.Normalize(NormalizationForm.FormC);
		}

		public string ScriptureReference => string.Empty;
		public int StartRef => 01001001;
		public int EndRef => 66022021;
		public string Text { get; }
		public string PhraseInUse => Text;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ScriptureReference + "-" + Text;
		}
	}
}
