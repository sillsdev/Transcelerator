// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: ParsedPart.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using SIL.Utils;

namespace SIL.Transcelerator
{
    public enum PartType
    {
        TranslatablePart,
        KeyTerm,
    }

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to encapsulate a parsed portion (either a translatable part or a key term) of a
	/// Scripture checking question.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class ParsedPart
	{
	    #region Data members
	    private string m_text;
	    private List<Word> m_words;
	    private List<Question> m_owners;
	    #endregion

        #region Properties
        [XmlAttribute("type")]
        public PartType Type { get; set; }

        /// ----------------------------------------------------------------------------------------
        /// <summary>String of words (lowercase, space-separated) that comprise the part</summary>
        /// ----------------------------------------------------------------------------------------
        [XmlText]
        public string Text
        {
            get
            {
                if (m_text == null)
                    m_text = m_words.ToString(" ");
                return m_text;
            }
            set
            {
                if (m_words != null)
                    throw new InvalidOperationException("Can't set Text if Words has already been set.");
                m_text = value;
            }
        }

        /// ----------------------------------------------------------------------------------------
        /// <summary>List of Words</summary>
        /// ----------------------------------------------------------------------------------------
        [XmlIgnore]
        public List<Word> Words
        {
            get
            {
                if (m_words == null)
                {
                    string[] words = m_text.Split(' ');
                    m_words = new List<Word>(words.Length);
                    foreach (string word in words)
                        m_words.Add(word);
                }
                return m_words;
            }
            set
            {
                if (m_text != null)
                    throw new InvalidOperationException("Can't set Words if Text has already been set.");
                m_words = value;
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the questions/phrases that include this part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [XmlIgnore]
        public IEnumerable<Question> Owners
        {
            get { return m_owners ?? Enumerable.Empty<Question>(); }
        }
        #endregion

        #region Constructors
        /// ----------------------------------------------------------------------------------------
        /// <summary>
        /// Needed for XML deserialization
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        public ParsedPart()
	    {
        }

        /// ----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a translatable part
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        public ParsedPart(IEnumerable<Word> words)
        {
            m_words = words.ToList();
            Type = PartType.TranslatablePart;
        }

        /// ----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a key term part
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        public ParsedPart(KeyTermMatchSurrogate match)
        {
            m_text = match.TermId;
            Type = PartType.KeyTerm;
        }
	    #endregion

	    #region Public methods
        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return Type + ": " + Words;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a registered owner for this part (only relevant for translatable parts).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public void AddOwningPhrase(Question owner)
	    {
            if (Type != PartType.TranslatablePart)
                throw new InvalidOperationException("Key Term parts don't need to have their owners set.");
	        if (m_owners == null)
                m_owners = new List<Question>();
            m_owners.Add(owner);
	    }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets <paramref name="count"/> sub words from this part starting at <paramref name="i"/>.
        /// </summary>
        /// <param name="i">The index of the first word to get.</param>
        /// <param name="count">The count of words to get.</param>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<Word> GetSubWords(int i, int count)
        {
            int limit = i + count;
            for (; i < limit; i++)
                yield return m_words[i];
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the sub words from this part starting at <paramref name="i"/>.
        /// </summary>
        /// <param name="i">The index of the first word to get.</param>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<Word> GetSubWords(int i)
        {
            return GetSubWords(i, m_words.Count - i);
        }
        #endregion
    }
}
