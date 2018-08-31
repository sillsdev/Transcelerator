// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIDataString.cs
// ---------------------------------------------------------------------------------------------
using System;
using SIL.Scripture;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UIDataString : IRefRange
	{
		private bool m_useAnyAlternate;

		public UIDataString(IQuestionKey baseQuestionKey, LocalizableStringType type, string uiString = null)
		{
			StartRef = baseQuestionKey.StartRef;
			EndRef = baseQuestionKey.EndRef;
			Type = type;
			switch (type)
			{
				case LocalizableStringType.SectionHeading:
				case LocalizableStringType.Category:
					throw new InvalidOperationException("This is not the constructor to use for this type of string.");
				case LocalizableStringType.Question:
					m_useAnyAlternate = true;
					SourceUIString = baseQuestionKey.PhraseInUse;
					if (uiString != null && uiString != SourceUIString)
						throw new ArgumentException("It is invalid to create a UIDataString from a Question using a form other than that represented by the question. Perhaps the intention was to get an alternate form instead.");
					break;
				default:
					SourceUIString = uiString ?? throw new ArgumentNullException(nameof(uiString));
					break;
			}
			Question = baseQuestionKey.Text;
		}

		public UIDataString(SectionInfo section) : this(section.Heading, LocalizableStringType.SectionHeading, section.StartRef, section.EndRef)
		{
		}

		public UIDataString(string uiString, LocalizableStringType type, int startRef = 0, int endRef = 0, string question = null)
		{
			if (type != LocalizableStringType.Category && type != LocalizableStringType.NonLocalizable && startRef <= 0)
				throw new ArgumentException("Scripture reference must be specified for all localizable types other than categories");
			StartRef = startRef;
			EndRef = endRef;
			SourceUIString = uiString ?? throw new ArgumentNullException(nameof(uiString));
			Type = type;
			switch (Type)
			{
				case LocalizableStringType.Question:
					Question = uiString;
					m_useAnyAlternate = true;
					break;
				case LocalizableStringType.Alternate:
					m_useAnyAlternate = true;
					goto case LocalizableStringType.Answer;
				case LocalizableStringType.Answer:
				case LocalizableStringType.Note:
					if (String.IsNullOrWhiteSpace(question))
						throw new ArgumentNullException(nameof(question), "Question is required for UI strings that are associated with a specific question.");
					Question = question;
					break;
			}
		}

		public string SourceUIString { get; }
		public LocalizableStringType Type { get; }
		public string ScriptureReference => BCVRef.MakeReferenceString(StartRef, EndRef, ".", "-");
		public int StartRef { get; }
		public int EndRef { get; }
		public string Question { get; }

		public bool UseAnyAlternate
		{
			get => m_useAnyAlternate;
			set
			{
				if (Type != LocalizableStringType.Question && Type != LocalizableStringType.Alternate)
					throw new InvalidOperationException("Setting UseAnyAlternate to true only makes sense for questions and alternates.");
				m_useAnyAlternate = value;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ScriptureReference + "-" + SourceUIString;
		}
	}
}