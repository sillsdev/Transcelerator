// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PhraseParser.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Method class for parsing a translatable phrase into translatable parts and key terms.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class PhraseParser
	{
		private readonly Dictionary<Word, List<KeyTermMatch>> m_keyTermsTable;
		private readonly IDictionary<int, List<List<Word>>> m_questionWords;
	    private readonly Question m_phrase;
        private readonly Func<IEnumerable<Word>, Question, ParsedPart> YieldTranslatablePart;
		private readonly List<Word> m_words;
		/// <summary>The index of the current (potential) match</summary>
		private int m_iStartMatch;
		private int m_iNextWord;
		private List<KeyTermMatch> m_matches;
		private readonly List<KeyTermMatch> m_keyTermsUsedForPhrase = new List<KeyTermMatch>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="PhraseParser"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal PhraseParser(Dictionary<Word, List<KeyTermMatch>> keyTermsTable,
            Dictionary<Regex, string> substituteStrings, IDictionary<int, List<List<Word>>> questionWords,
            Question phrase, Func<IEnumerable<Word>, Question, ParsedPart> yieldPart)
		{
			m_keyTermsTable = keyTermsTable;
			m_questionWords = questionWords;
			YieldTranslatablePart = yieldPart;
			m_phrase = phrase;

			string phraseToParse = m_phrase.PhraseInUse;
		    if (substituteStrings != null)
		    {
		        foreach (KeyValuePair<Regex, string> substituteString in substituteStrings)
		            phraseToParse = substituteString.Key.Replace(phraseToParse, substituteString.Value);
		    }

		    m_words = GetWordsInString(phraseToParse.ToLowerInvariant());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a list of all of the words in the specified string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static List<Word> GetWordsInString(string phrase)
		{
			List<Word> words = new List<Word>();
			StringBuilder wordBldr = new StringBuilder();
			bool processingNumber = false;
			foreach (char ch in phrase)
			{
				if (Char.IsLetter(ch) || ch == '\'' || ch == '-')
				{
					if (processingNumber)
					{
						words.Add(wordBldr.ToString());
						wordBldr.Length = 0;
						processingNumber = false;
						if (ch == '\'' || ch == '-')
							continue;
					}
					wordBldr.Append(ch);
				}
				else if (Char.IsDigit(ch))
				{
					wordBldr.Append(ch);
					processingNumber = true;
				}
				else if (wordBldr.Length > 0 && (!processingNumber || (ch != ',' && ch != '.')))
				{
					words.Add(wordBldr.ToString());
					wordBldr.Length = 0;
					processingNumber = false;
				}
			}
            if (wordBldr.Length > 0)
				words.Add(wordBldr.ToString());
			return words;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Cleans up the given source-language phrase (makes it all lowercase and removes
		/// punctuation and extraneous whitespace) and returns a collection of phrase parts,
		/// broken up by key terms.
		/// </summary>
		/// <returns>Collection of phrase parts</returns>
		/// ------------------------------------------------------------------------------------
		internal IEnumerable<ParsedPart> Parse()
		{
			int minUnhandled = m_iStartMatch = m_iNextWord = 0;

			if (m_questionWords != null)
			{
				foreach (int count in m_questionWords.Keys.OrderByDescending(k => k))
				{
					foreach (List<Word> questionPhrase in m_questionWords[count])
					{
						bool match = true;
						int i;
						for (i = 0; i < count; i++)
						{
							if (m_iNextWord + i >= m_words.Count || questionPhrase[i] != m_words[m_iNextWord + i])
							{
								match = false;
								break;
							}
						}
						if (match)
						{
							yield return YieldTranslatablePart(m_words.Take(count), m_phrase);
							m_iStartMatch = m_iNextWord = minUnhandled = count;
							break;
						}
					}
				}
			}

			for (; m_iNextWord < m_words.Count; )
			{
				var bestKeyTerm = FindBestKeyTerm();
				if (bestKeyTerm == null)
				{
					if (m_words[m_iNextWord].IsNumber)
					{
						if (m_iNextWord > minUnhandled)
							yield return YieldTranslatablePart(m_words.Skip(minUnhandled).Take(m_iNextWord - minUnhandled), m_phrase);
						yield return new ParsedPart(Int32.Parse(m_words[m_iNextWord].Text));
						m_iStartMatch = minUnhandled = ++m_iNextWord;
					}
					else
						m_iNextWord++;
				}
				else
				{
					// We've found the best key term we're going to find.
					int keyTermWordCount = bestKeyTerm.WordCount;
					if (m_iStartMatch > minUnhandled)
						yield return YieldTranslatablePart(m_words.Skip(minUnhandled).Take(m_iStartMatch - minUnhandled), m_phrase);
					m_keyTermsUsedForPhrase.Add(bestKeyTerm);
					bestKeyTerm.InUse = true;
					yield return new ParsedPart(bestKeyTerm);
					m_iStartMatch = m_iNextWord = minUnhandled = m_iStartMatch + keyTermWordCount;
				}
			}

			if (minUnhandled < m_words.Count)
			{
				yield return YieldTranslatablePart(m_words.Skip(minUnhandled), m_phrase);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the best key term in the list of words starting at m_iStartMatch and including
		/// up to m_iNextWord. As new words are considered, the list of possible matches
		/// (m_matches) is reduced by any that no longer match until there is exactly one match
		/// that exactly equals the words in the key term or the list is empty.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private KeyTermMatch FindBestKeyTerm()
		{
		    if (m_keyTermsTable == null)
		        return null;

			Word nextWord = m_words[m_iNextWord];

			if (m_iStartMatch == m_iNextWord)
			{
				m_matches = null;
				if (m_keyTermsTable.TryGetValue(nextWord, out var matches))
                {
				    m_matches = new List<KeyTermMatch>(matches.Where(m => m.AppliesTo(m_phrase.StartRef, m_phrase.EndRef)));
                }
                if (m_matches == null || m_matches.All(m => m.WordCount > 1))
                {
					foreach (var form in PorterStemmer.GetStemmedForms(nextWord.Text))
					{
						Word stem = form;

                        if (m_keyTermsTable.TryGetValue(stem, out matches))
                        {
                            stem.AddAlternateForm(nextWord);
                            matches = new List<KeyTermMatch>(matches.Where(m => m.AppliesTo(m_phrase.StartRef, m_phrase.EndRef)));
                            if (m_matches == null)
                                m_matches = matches;
                            else
                                m_matches.AddRange(matches);
                        }
                    }
                    if (m_matches == null || m_matches.Count == 0)
                    {
                        m_iStartMatch++;
                        return null;
                    }
				}

				// If we found a one-word exact match and there are no other key terms that start
				// with that word, then we return it. The code below would handle this, but it's such
				// a common case, we want it to be fast. If there are one or more multi-word key
				// terms that start with this word, we need to keep looking.
				if (m_matches.Count == 1 && m_matches[0].WordCount == 1)
					return m_matches[0];
			}

			int cMatchingWordsInTermSoFar = m_iNextWord - m_iStartMatch + 1;
			int lengthOfBestMatch = 0;
			KeyTermMatch longestMatch = null;

			// Remove from the possible matches any that don't match so far
			for (int iTerm = 0; iTerm < m_matches.Count; iTerm++)
			{
				KeyTermMatch term = m_matches[iTerm];
				if (!PhraseEqualsKeyTermSoFar(term, cMatchingWordsInTermSoFar) ||
					(AtEndOfPhrase && term.WordCount > cMatchingWordsInTermSoFar))
					m_matches.RemoveAt(iTerm--);
				else if (term.WordCount > lengthOfBestMatch)
				{
					lengthOfBestMatch = term.WordCount;
					longestMatch = term;
				}
			}

			if (m_matches.Count == 0)
			{
				// The only matches we had were multi-word matches, and the addition of the current
				// word made it so that none of them matched. Therefore, we don't have a key term
				// starting at iStartMatch.
				m_iNextWord = m_iStartMatch; // The for loop in Parse will increment this.
				m_iStartMatch++;
				return null;
			}

			if ((m_matches.Count == 1 && lengthOfBestMatch < cMatchingWordsInTermSoFar) || (lengthOfBestMatch == cMatchingWordsInTermSoFar))
				return longestMatch;

			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether we're working on the last word in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool AtEndOfPhrase => m_iNextWord == m_words.Count - 1;

		public List<KeyTermMatch> KeyTermsUsedForPhrase => m_keyTermsUsedForPhrase;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the part of the phrase we're considering matches the key term so
		/// far.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool PhraseEqualsKeyTermSoFar(KeyTermMatch term, int cMatchingWordsInTermSoFar)
		{
			int cCompare = Math.Min(term.WordCount, cMatchingWordsInTermSoFar);
			for (int iWord = m_iStartMatch; iWord < cCompare + m_iStartMatch; iWord++)
			{
				if (!term[iWord - m_iStartMatch].IsEquivalent(m_words[iWord]))
					return false;
			}
			return true;
		}
	}
}
