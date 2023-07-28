// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermMatch.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Paratext.PluginInterfaces;
using SIL.Extensions;

namespace SIL.Transcelerator
{
	public class KeyTermMatch
	{
		#region Data members
		private readonly List<Word> m_words;
		private readonly List<IBiblicalTerm> m_terms;
		private HashSet<int> m_occurrences;
		private KeyTermMatchSurrogate m_surrogate; // Cached for efficiency
		#endregion

		#region Constructors
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTermMatch"/> class that doesn't
        /// (yet) match on any words.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="matchForRefOnly">if set to <c>true</c> [match for ref only].</param>
        /// ------------------------------------------------------------------------------------
        internal KeyTermMatch(IBiblicalTerm term, bool matchForRefOnly) :
            this(new Word[0], term, matchForRefOnly)
        {
        }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Copy constructor (only valid for an object having a single term).
	    /// </summary>
	    /// <param name="matchBase">The base match from which this match will be created.</param>
	    /// ------------------------------------------------------------------------------------
	    internal KeyTermMatch(KeyTermMatch matchBase) :
	        this(matchBase.m_words, matchBase.m_terms[0], matchBase.MatchForRefOnly)
	    {
	        if (matchBase.m_terms.Count != 1)
	        {
	            throw new ArgumentException("KeyTermMatch copy constructor only valid for making" +
	                " copies of a new in-progress match with a single underlying key term.");
	        }
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyTermMatch"/> class.
		/// </summary>
		/// <param name="words">The words.</param>
		/// <param name="term">The term.</param>
		/// <param name="matchForRefOnly">if set to <c>true</c> [match for ref only].</param>
		/// ------------------------------------------------------------------------------------
		internal KeyTermMatch(IEnumerable<Word> words, IBiblicalTerm term, bool matchForRefOnly)
		{
			MatchForRefOnly = matchForRefOnly;
			m_words = words.ToList();
			m_terms = new List<IBiblicalTerm> { term };
		}
		#endregion

		#region Overridden methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		/// ------------------------------------------------------------------------------------
		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case KeyTermMatch match:
					return m_words.SequenceEqual(match.m_words);
				case IEnumerable<Word> words:
					return m_words.SequenceEqual(words);
			}
			return base.Equals(obj);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data
		/// structures like a hash table. 
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public override int GetHashCode() => base.GetHashCode();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public override string ToString() => m_words.ToString(" ");
		#endregion

		#region Public properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Count of words this object matches on.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int WordCount => m_words.Count;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the term Id.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Id => m_terms.First().Lemma;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether this match should only be considered for questions/phrases for one of
		/// the occurrences of the term(s).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool MatchForRefOnly { get; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether this resulted in an actual match for some question/phrase
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool InUse { get; internal set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the ith Word in the sequence of words on which this object matches
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Word this[int i] => m_words[i];

		#endregion

        #region Public methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Indicates whether this key term match should be considered when parsing a phrase
        /// for the given reference range
        /// </summary>
        /// <param name="startRef">Start reference in BBBCCCVVV format</param>
        /// <param name="endRef">End reference in BBBCCCVVV format</param>
        /// ------------------------------------------------------------------------------------
        public bool AppliesTo(int startRef, int endRef)
		{
			if (!MatchForRefOnly)
				return true;
			if (m_occurrences == null)
				m_occurrences = new HashSet<int>(m_terms.SelectMany(term => term.Occurrences.Select(o => o.BBBCCCVVV)));
			return m_occurrences.Any(o => startRef <= o && endRef >= o);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Implicitly casts this as a KeyTermMatchSurrogate
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public static implicit operator KeyTermMatchSurrogate(KeyTermMatch match)
        {
            if (match == null)
                return null;

            // Typically, we'll only want to do this cast once the terms and words of the object
            // have been fully set, but then we'll want to do it more than once. It's expensive
            // enough that it's probably worth caching.
            if (match.m_surrogate == null)
            {
                match.m_surrogate = new KeyTermMatchSurrogate(match.ToString(),
                    match.m_terms.Select(t => t.Lemma).ToArray());
            }
            return match.m_surrogate;
        }
        #endregion

        #region Internal methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds another underlying key term with the same (possibly partial) gloss as the
        /// other(s)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        internal void AddTerm(IBiblicalTerm keyTerm)
		{
			if (keyTerm == null)
				throw new ArgumentNullException(nameof(keyTerm));
			m_terms.Add(keyTerm);
            m_surrogate = null;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds another Word to the sequence that constitute the phrase on which this matches.
        /// This is intended to be  
        /// </summary>
        /// ------------------------------------------------------------------------------------
        internal void AddWord(Word word)
		{
			if (word == null)
				throw new ArgumentNullException(nameof(word));
			m_words.Add(word);
            m_surrogate = null;
        }

		internal void AddWords(IEnumerable<Word> words)
		{
			m_words.AddRange(words);
            m_surrogate = null;
        }
		#endregion
	}
}