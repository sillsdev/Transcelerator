// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2012, SIL International. All Rights Reserved.
// <copyright from='2011' to='2012' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: QuestionProvider.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SIL.Utils;
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
	#region class QuestionProvider
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Gets the questions from the file
	/// </summary>
	/// ------------------------------------------------------------------------------------
	public class QuestionProvider : IEnumerable<TranslatablePhrase>
    {
        #region Data members
        private QuestionSections m_sections;
        private readonly Dictionary<Word, List<KeyTerm>> m_keyTermsTable = new Dictionary<Word, List<KeyTerm>>();
        /// <summary>A double lookup table of all parts in all phrases managed by this class.
        /// For improved performance, outer lookup is by wordcount.</summary>
        private readonly SortedDictionary<int, Dictionary<Word, List<Part>>> m_partsTable = new SortedDictionary<int, Dictionary<Word, List<Part>>>();
		private IDictionary<string, string> m_sectionHeads;
		private int[] m_availableBookIds;
        #endregion

        #region Constructors
        /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Initializes a new instance of the <see cref="QuestionProvider"/> class.
	    /// </summary>
	    /// <param name="sections">Class representing the questions, organized by Scripture
	    /// section and category.</param>
	    /// <param name="keyTerms">(optional) key terms</param>
	    /// ------------------------------------------------------------------------------------
	    public QuestionProvider(QuestionSections sections, IEnumerable<KeyTermMatchSurrogate> keyTerms)
		{
			m_sections = sections;
            if (keyTerms != null)
                PopulateKeyTermsTable(keyTerms);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionProvider"/> class.
        /// </summary>
        /// <param name="parsedQuestions">The parsed questions object.</param>
        /// ------------------------------------------------------------------------------------
        public QuestionProvider(ParsedQuestions parsedQuestions)
        {
            m_sections = parsedQuestions.Sections;
            PopulateKeyTermsTable(parsedQuestions.KeyTerms);
            PopulatePartsTable(parsedQuestions.TranslatableParts);
        }
        #endregion

        #region Public Properties
        /// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets a dictionary that correlates (textual) Scripture references to
		/// corresponding section head text (note that these are not the section heads in
		/// the vernacular Scripture but rather from the master question file).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public IDictionary<string, string> SectionHeads
		{
			get
			{
				if (m_sectionHeads == null)
					m_sectionHeads = m_sections.Items.ToDictionary(s => s.ScriptureReference, s => s.Heading);
				return m_sectionHeads;
			}
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets an array of canonical book ids for which questions exist.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public int[] AvailableBookIds
		{
			get
			{
				if (m_availableBookIds == null)
					m_availableBookIds = m_sections.Items.Select(s => BCVRef.GetBookFromBcv(s.StartRef)).Distinct().ToArray();
				return m_availableBookIds;
			}
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Enumerates the full (unique) list of all translatable parts
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<Part> AllTranslatableParts
	    {
            get { return m_partsTable.Values.SelectMany(thing => thing.Values.SelectMany(parts => parts)); }
	    }

	    #endregion

		#region Private helper methods
	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Populates the key terms table. This method assumes that the incoming surrogates are
	    /// unique.
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    private void PopulateKeyTermsTable(IEnumerable<KeyTermMatchSurrogate> keyTermSurrogates)
	    {
            foreach (KeyTermMatchSurrogate surrogate in keyTermSurrogates)
            {
                Word firstWord = Word.FirstWord(surrogate.TermId);
                List<KeyTerm> termsStartingWithSameFirstWord;
	            if (!m_keyTermsTable.TryGetValue(firstWord, out termsStartingWithSameFirstWord))
	                m_keyTermsTable[firstWord] = termsStartingWithSameFirstWord = new List<KeyTerm>();

	            termsStartingWithSameFirstWord.Add(new KeyTerm(surrogate));
	        }
	    }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Populates the key terms table. This method assumes that the incoming surrogates are
	    /// unique.
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    private void PopulatePartsTable(IEnumerable<string> translatableParts)
	    {
	        foreach (string part in translatableParts)
	            CreatePart(part);
	    }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Creates a part for the given sub-phrase.
	    /// </summary>
	    /// <param name="phrase">The phrase containing the space-delimited words of the part
	    /// (assumed to be lowercase and free of punctuation).</param>
	    /// ------------------------------------------------------------------------------------
	    private void CreatePart(string phrase)
	    {
            GetOrCreatePart(phrase.Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries).Select(w => (Word)w).ToList(),
                null, true);
	    }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Gets or creates a part matching the given sub-phrase.
	    /// </summary>
	    /// <param name="words">The words of the part.</param>
	    /// <param name="owningPhraseOfPart">(optional) The owning phrase of the part to find or
	    /// create.</param>
	    /// <param name="unconditionallyCreate">Caller can pass true for added efficiency if it
	    /// knows for sure that the part doesn't already exist in the table.</param>
	    /// ------------------------------------------------------------------------------------
	    private Part GetOrCreatePart(List<Word> words, TranslatablePhrase owningPhraseOfPart,
            bool unconditionallyCreate)
        {
            Dictionary<Word, List<Part>> partsDictionary;
            List<Part> parts = null;
            Part part = null;
            if (m_partsTable.TryGetValue(words.Count(), out partsDictionary))
            {
                if (partsDictionary.TryGetValue(words.First(), out parts) && !unconditionallyCreate)
                    part = parts.FirstOrDefault(x => x.Words.SequenceEqual(words));
            }
            else
                m_partsTable[words.Count()] = partsDictionary = new Dictionary<Word, List<Part>>();

            if (parts == null)
                partsDictionary[words.First()] = parts = new List<Part>();

            if (part == null)
            {
                part = new Part(words);
                parts.Add(part);
            }

            if (owningPhraseOfPart != null)
                part.AddOwningPhrase(owningPhraseOfPart);

            return part;
        }

	    /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the questions/phrases
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private IEnumerable<TranslatablePhrase> GetPhrases()
		{
			HashSet<string> processedCategories = new HashSet<string>();
	        TranslatablePhrase phrase;
			foreach (Section section in m_sections.Items)
			{
				for (int iCat = 0; iCat < section.Categories.Length; iCat++)
				{
					Category category = section.Categories[iCat];

				    if (category.Type != null)
				    {
				        string lcCategory = category.Type.ToLowerInvariant();
				        if (!processedCategories.Contains(lcCategory))
				        {
				            phrase = new TranslatablePhrase(new SimpleQuestionKey(category.Type), -1, processedCategories.Count);
                            phrase.m_parts.Add(GetOrCreatePart(PhraseParser.GetWordsInString(lcCategory), phrase, false));
				            yield return phrase;
				            processedCategories.Add(lcCategory);
				        }
				    }

				    for (int iQuestion = 0; iQuestion < category.Questions.Count; iQuestion++)
					{
						Question q = category.Questions[iQuestion];
						if (q.ScriptureReference == null)
						{
							q.ScriptureReference = section.ScriptureReference;
							q.StartRef = section.StartRef;
							q.EndRef = section.EndRef;
						}
						phrase = new TranslatablePhrase(q, iCat, iQuestion + 1);
					    if (!phrase.IsExcluded)
					    {
					        foreach (ParsedPart part in q.ParsedParts)
					        {
					            if (part.Type == PartType.TranslatablePart)
					                phrase.m_parts.Add(GetOrCreatePart(part.Words, phrase, false));
					            else
					                phrase.m_parts.Add(FindKeyTerm(part.Words));
					        }
					    }
					    yield return phrase;
					}
				}
			}
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Finds the key term which matches the given list of words (assumed to exist)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private IPhrasePart FindKeyTerm(List<Word> words)
        {
            return (m_keyTermsTable[words[0]]).First(kt => kt.Words.SequenceEqual(words));
        }
	    #endregion

		#region Implementation of IEnumerable<TranslatablePhrase>
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns an enumerator that iterates through the collection of questions.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to
		/// iterate through the collection.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public IEnumerator<TranslatablePhrase> GetEnumerator()
		{
		    return GetPhrases().GetEnumerator();
		}
		#endregion

		#region Implementation of IEnumerable
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification = "Caller is responsible to dispose enumerator")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
	#endregion
}
