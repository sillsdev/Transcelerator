﻿// ---------------------------------------------------------------------------------------------
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
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class UIDataString : IRefRange
	{
		private bool m_useAnyAlternate;

		public UIDataString(IQuestionKey baseQuestionKey, LocalizableStringType type, string uiString = null)
		{
			ScriptureReference = baseQuestionKey.ScriptureReference;
			StartRef = baseQuestionKey.StartRef;
			EndRef = baseQuestionKey.EndRef;
			Type = type;
			switch (type)
			{
				case LocalizableStringType.SectionHeading:
				case LocalizableStringType.Category:
					throw new InvalidOperationException("This is not the constructor to use for this type of string.");
				case LocalizableStringType.Question:
					SourceUIString = String.IsNullOrWhiteSpace(baseQuestionKey.Text) ? uiString : baseQuestionKey.Text;
					break;
				default:
					SourceUIString = uiString ?? throw new ArgumentNullException(nameof(uiString));
					break;
			}
			Question = baseQuestionKey.Text;
		}

		public UIDataString(string uiString, LocalizableStringType type, string scrRef = null, int startRef = 0, int endRef = 0, string question = null)
		{
			ScriptureReference = scrRef;
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
		public string ScriptureReference { get; }
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