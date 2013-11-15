﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: Word.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;

namespace SIL.Transcelerator
{
	public sealed class Word
	{
		#region Data Members
		private readonly string m_sText;
		private static readonly Dictionary<string, Word> s_words = new Dictionary<string, Word>(1000);
		private static readonly Dictionary<Word, HashSet<Word>> s_inflectedWords = new Dictionary<Word, HashSet<Word>>();
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Word"/> class.
		/// </summary>
		/// <param name="text">The text of the word.</param>
		/// ------------------------------------------------------------------------------------
		private Word(string text)
		{
			Debug.Assert(!string.IsNullOrEmpty(text));
			m_sText = text;
			s_words[text] = this;
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the text of the word.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Text
		{
			get { return m_sText; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Enumerates all "known" words (i.e., those found in the English questions).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static IEnumerable<string> AllWords
		{
			get { return s_words.Keys; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether this word is actually a number (sequence of digits).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsNumber
		{
			get { return char.IsDigit(Text[0]); }
		}
		#endregion

		#region Public methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the given alternate (inflected) form of this word to the collection of words
		/// that will be considered as equivalent words.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void AddAlternateForm(Word inflectedForm)
		{
			HashSet<Word> inflectedForms;
			if (!s_inflectedWords.TryGetValue(this, out inflectedForms))
				s_inflectedWords[this] = inflectedForms = new HashSet<Word>();
			inflectedForms.Add(inflectedForm);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified other word is equivalent to this word (either the
		/// same word or an inflected form of it).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsEquivalent(Word otherWord)
		{
			if (this == otherWord)
				return true;
			HashSet<Word> inflectedForms;
			return (s_inflectedWords.TryGetValue(this, out inflectedForms) && inflectedForms.Contains(otherWord));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether or not a Word for the specified text exists.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool Exists(string word)
		{
			return s_words.ContainsKey(word);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The text of this word.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			 return m_sText;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs an implicit conversion from <see cref="SIL.Transcelerator.Word"/>
		/// to <see cref="System.String"/>.
		/// </summary>
		/// <param name="word">The word.</param>
		/// <returns>The result of the conversion.</returns>
		/// ------------------------------------------------------------------------------------
		public static implicit operator string(Word word)
		{
			return word == null ? null : word.m_sText;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs an implicit conversion from <see cref="SIL.Transcelerator.Word"/>
		/// to <see cref="System.String"/>.
		/// </summary>
		/// <param name="text">The word.</param>
		/// <returns>The result of the conversion.</returns>
		/// ------------------------------------------------------------------------------------
		public static implicit operator Word(string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;
			Word word;
			if (s_words.TryGetValue(text, out word))
				return word;
			return new Word(text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the Word representing the first (space-delimited) word in the text. Text is
		/// assumed to be a punctuation-free string (and probably lowercase).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Word FirstWord(string text)
		{
		    if (string.IsNullOrEmpty(text))
		        return null;

			int ichMin;
		    int length = 0;
			for (ichMin = 0; ichMin + length < text.Length;)
			{
			    if (text[ichMin + length] == ' ')
			    {
			        if (length > 0)
			            break;
			        ichMin++;
			    }
			    else
			        length++;
			}
			return text.Substring(ichMin, length);
		}
		#endregion
	}
}
