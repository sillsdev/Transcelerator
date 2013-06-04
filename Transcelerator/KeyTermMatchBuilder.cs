// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2011' to='2011' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: KeyTermMatchBuilder.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AddInSideViews;
using SIL.Utils;

namespace SIL.Transcelerator
{
	public class KeyTermMatchBuilder
	{
		private readonly List<KeyTermMatch> m_list = new List<KeyTermMatch>();
		private List<Word> m_optionalPhraseWords;
		private bool m_fInOptionalPhrase;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyTermMatchBuilder"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public KeyTermMatchBuilder(IKeyTerm keyTerm) : this(keyTerm, null, null)
		{
		}

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Initializes a new instance of the <see cref="KeyTermMatchBuilder"/> class.
	    /// </summary>
	    /// <param name="keyTerm">The key term.</param>
	    /// <param name="rules">Optional dictionary of (English) key terms to rules indicating
	    /// special handling neeeded.</param>
        /// <param name="regexRules">regular-expression-based rules. If any term matches a
	    /// regular expression in this collection, terms will be extracted using the variable
	    /// "term". If the term variable is not present in the regular expression, this term
	    /// will be excluded.</param>
	    /// ------------------------------------------------------------------------------------
	    public KeyTermMatchBuilder(IKeyTerm keyTerm, IReadonlyDictionary<string, KeyTermRule> rules,
            IEnumerable<Regex> regexRules)
		{
			string normalizedLcTerm = keyTerm.Term.ToLowerInvariant().Normalize(NormalizationForm.FormC);
			KeyTermRule ktRule;
            bool fMatchForRefOnly = false;
            if (rules != null && rules.TryGetValue(normalizedLcTerm, out ktRule))
			{
                ktRule.Used = true;
				bool fExcludeMainTerm = false;
				if (ktRule.Rule != null)
				{
					switch (ktRule.Rule)
					{
						case KeyTermRule.RuleType.Exclude: fExcludeMainTerm = true; break;
                        case KeyTermRule.RuleType.MatchForRefOnly: fMatchForRefOnly = true; break;
					}
				}
				if (ktRule.Alternates != null)
				{
                    foreach (KeyTermRulesKeyTermRuleAlternate alt in ktRule.Alternates)
                        AddMatchesForPhrase(keyTerm, alt.Name, fMatchForRefOnly || alt.MatchForRefOnly, false, m_list.Count);
				}
				if (fExcludeMainTerm)
					return;
			}
            else if (regexRules != null)
            {
                foreach (Regex regexRule in regexRules)
                {
                    Match match = regexRule.Match(normalizedLcTerm);
                    while (match.Success)
                    {
                        string term = match.Result("${term}");
                        if (term == "${term}")
                            return; // No "term" variable found, so this rule excludes the term
                        foreach (string phrase in term.Split(new[] { ", or ", "," }, StringSplitOptions.RemoveEmptyEntries))
                            ProcessKeyTermPhrase(keyTerm, phrase, false); // for now, at least reg-ex based rules can't be reference-dependent
                        match = match.NextMatch();
                    }
                    if (m_list.Count > 0)
                        return;
                }
            }
            foreach (string phrase in normalizedLcTerm.Split(new[] { ", or ", ",", ";", "=" }, StringSplitOptions.RemoveEmptyEntries))
				ProcessKeyTermPhrase(keyTerm, phrase, fMatchForRefOnly);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes a single alternative word or phrase for a key term. Most key terms have a
		/// simple "source" (actually English) rendering that consists of a single word or
		/// phrase. But some have multiple alternative words or phrases; hence, this method.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void ProcessKeyTermPhrase(IKeyTerm keyTerm, string phrase, bool fMatchForRefOnly)
		{
			int startOfListForPhrase = m_list.Count;
			string[] orParts = phrase.Split(new [] {" or "}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (orParts.Length == 2)
			{
				int ichEndOfPreOrPhrase = orParts[0].Length;
				int ichStartOfPostOrPhrase = 0;
				int ichPre, ichPost;
				do
				{
					ichPre = orParts[0].LastIndexOf(' ', ichEndOfPreOrPhrase - 1);
					ichPost = orParts[1].IndexOf(' ', ichStartOfPostOrPhrase + 1);
					ichEndOfPreOrPhrase = (ichPre >= 0) ? ichPre : 0;
					ichStartOfPostOrPhrase = (ichPost >= 0) ? ichPost : orParts[1].Length;
				} while (ichEndOfPreOrPhrase > 0 && ichPost >= 0);

				if (ichEndOfPreOrPhrase > 0)
					ichEndOfPreOrPhrase++;
				ProcessKeyTermPhrase(keyTerm, orParts[0] + orParts[1].Substring(ichStartOfPostOrPhrase), fMatchForRefOnly);
                ProcessKeyTermPhrase(keyTerm, orParts[0].Substring(0, ichEndOfPreOrPhrase) + orParts[1], fMatchForRefOnly);
				return;
			}

            AddMatchesForPhrase(keyTerm, phrase, fMatchForRefOnly, true, startOfListForPhrase);

			if (m_fInOptionalPhrase)
				AddWordsToMatches(m_optionalPhraseWords, startOfListForPhrase);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Processes a (potentially multi-word) phrase for a key term, adding at least one new
        /// KeyTermMatch and appending the new word(s) to any existing match(es) that should
        /// include this phrase as part of the sequence of words used to form a match.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void AddMatchesForPhrase(IKeyTerm keyTerm, string phrase, bool fMatchForRefOnly,
	        bool createExtraMatchIfPhraseStartsWithTo, int startOfListForPhrase)
	    {
            // Initially, we add one empty list
	        m_list.Add(new KeyTermMatch(new Word[0], keyTerm, fMatchForRefOnly));
	        foreach (Word metaWord in phrase.Split(new[] {' ', '\u00a0'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('\'')))
	        {
	            List<Word> allWords = AllWords(metaWord, createExtraMatchIfPhraseStartsWithTo);
	            if (allWords.Count > 0)
	                AddWordsToMatches(allWords, startOfListForPhrase);
	            createExtraMatchIfPhraseStartsWithTo = false;
	        }
	    }

	    /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the words to matches. If adding more than one word, then this represents an
		/// optional word/phrase, which results in doubling the number of matches for the
		/// current phrase.
		/// </summary>
		/// <param name="words">The words to append to the matches' word lists.</param>
		/// <param name="startOfListForPhrase">The index of the position in m_list that
		/// corresponds to the start of the matches relevant to the current phrase.</param>
		/// ------------------------------------------------------------------------------------
		private void AddWordsToMatches(List<Word> words, int startOfListForPhrase)
		{
			int originalCount = m_list.Count;
			if (words.Count > 1)
			{
				// Spawn a new copy of each matching phrase for this metaword.
				m_list.AddRange(m_list.Skip(startOfListForPhrase).Select(k => new KeyTermMatch(k)).ToList());
			}

			Word word = words[0];
			for (int index = (word == null || m_fInOptionalPhrase) ? originalCount : startOfListForPhrase; index < m_list.Count; index++)
			{
				if (m_fInOptionalPhrase)
					m_list[index].AddWords(words);
				else
				{
					if (index == originalCount)
						word = words[1];
					m_list[index].AddWord(word);
				}
			}
			m_fInOptionalPhrase = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the matches.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<KeyTermMatch> Matches
		{
			get { return m_list; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the possible surface forms represented by this metaword (which could have
		/// an optional part, indicated by parentheses). If this is a completely optional word,
		/// this will include a null. If it is part of an optional phrase, it will return an
		/// empty list until it gets to the last word in the phrase, at which point it returns
		/// a list representing the whole phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private List<Word> AllWords(Word metaWord, bool createExtraMatchIfPhraseStartsWithTo)
		{
			List<Word> list = new List<Word>();
			int iOpenParen = (m_fInOptionalPhrase) ? 0 : metaWord.Text.IndexOf('(');
			if (iOpenParen >= 0)
			{
				int iCloseParen = metaWord.Text.IndexOf(')', iOpenParen);
				if (iCloseParen > iOpenParen)
				{
					if (m_fInOptionalPhrase)
					{
						list = m_optionalPhraseWords;
						list.Add(metaWord.Text.Remove(iCloseParen));
						m_optionalPhraseWords = null;
					}
					else
					{
						string shortOrOptionalWord = metaWord.Text.Remove(iOpenParen, iCloseParen - iOpenParen + 1);
                        if (shortOrOptionalWord.Length == 0)
                            shortOrOptionalWord = null;
                        string fullWord = metaWord.Text.Remove(iCloseParen, 1).Remove(iOpenParen, 1);
                        if (shortOrOptionalWord != null || fullWord.Length > 0)
                        {
                            list.Add(shortOrOptionalWord);
                            list.Add(fullWord);
                        }
					}
				}
				else if (m_fInOptionalPhrase)
				{
					m_optionalPhraseWords.Add(metaWord);
				}
				else if (iOpenParen == 0)
				{
					m_optionalPhraseWords = new List<Word>();
					m_optionalPhraseWords.Add(metaWord.Text.Remove(0, 1));
					m_fInOptionalPhrase = true;
				}
				else
				{
                    Debug.Fail("Found opening parenthesis with no closer: " + metaWord.Text);
				}
			}
			else
			{
				if (createExtraMatchIfPhraseStartsWithTo && metaWord == "to")
					list.Add(null);
				list.Add(metaWord);
			}

			return list;
		}
	}
}
