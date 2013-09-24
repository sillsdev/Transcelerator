// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
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
using AddInSideViews;
using SIL.Utils;

namespace SIL.Transcelerator
{
	public class KeyTermMatch
	{
		#region Data members
		private readonly List<Word> m_words;
		private readonly List<IKeyTerm> m_terms;
		private readonly bool m_matchForRefOnly;
		private HashSet<int> m_occurrences;
	    private bool m_isInUse;
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
        internal KeyTermMatch(IKeyTerm term, bool matchForRefOnly) :
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
		internal KeyTermMatch(IEnumerable<Word> words, IKeyTerm term, bool matchForRefOnly)
		{
			m_matchForRefOnly = matchForRefOnly;
			m_words = words.ToList();
			m_terms = new List<IKeyTerm>();
			m_terms.Add(term);
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
			if (obj is KeyTermMatch)
			{
				return m_words.SequenceEqual(((KeyTermMatch)obj).m_words);
			}
			if (obj is IEnumerable<Word>)
			{
				return m_words.SequenceEqual((IEnumerable<Word>)obj);
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
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
        {
            return m_words.ToString(" ");
        }
		#endregion

        #region Public properties
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Count of words this object matches on.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public int WordCount
        {
            get { return m_words.Count; }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the term Id.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string Id
        {
            get { return m_terms.First().Id; }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this match should only be considered for questions/phrases for one of
        /// the occurrences of the term(s).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public bool MatchForRefOnly
        {
            get { return m_matchForRefOnly; }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this resulted in an actual match for some question/phrase
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public bool InUse
	    {
            get { return m_isInUse; }
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the references of all occurences of this key term as integers in the form
		/// BBBCCCVVV.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IList<int> BcvOccurences
		{
			get { return m_terms.SelectMany(keyTerm => keyTerm.BcvOccurences).Distinct().ToList(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the key terms for this match.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<IKeyTerm> AllTerms
		{
			get { return m_terms; }
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the ith Word in the sequence of words on which this object matches
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public Word this[int i]
        {
            get { return m_words[i]; }
        }
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
			if (!m_matchForRefOnly)
				return true;
			if (m_occurrences == null)
				m_occurrences = new HashSet<int>(m_terms.SelectMany(term => term.BcvOccurences));
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
                    match.m_terms.Select(t => t.Id).ToArray());
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
        internal void AddTerm(IKeyTerm keyTerm)
		{
			if (keyTerm == null)
				throw new ArgumentNullException("keyTerm");
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
				throw new ArgumentNullException("word");
			m_words.Add(word);
            m_surrogate = null;
        }

		internal void AddWords(IEnumerable<Word> words)
		{
			m_words.AddRange(words);
            m_surrogate = null;
        }

        internal void MarkInUse()
        {
            m_isInUse = true;
        }
		#endregion
	}
}