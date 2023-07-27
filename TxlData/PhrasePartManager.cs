// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2015' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PhrasePartManager.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SIL.Transcelerator
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Gets the questions from the file
	/// </summary>
	/// ------------------------------------------------------------------------------------
	public class PhrasePartManager
    {
		private readonly ITermRenderingsRepo m_renderingsRepository;

		#region Data members
        private readonly Dictionary<Word, List<KeyTerm>> m_keyTermsTable = new Dictionary<Word, List<KeyTerm>>();
        /// <summary>A double lookup table of all parts in all phrases managed by this class.
        /// For improved performance, outer lookup is by word count.</summary>
        private readonly SortedDictionary<int, Dictionary<Word, List<Part>>> m_partsTable = new SortedDictionary<int, Dictionary<Word, List<Part>>>();
		private List<Part> m_allPartsList; 
	    #endregion

        #region Constructor
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="QuestionProvider"/> class.
		/// </summary>
		/// <param name="translatableParts">parts (that are not key terms) to add to the master
		/// parts table</param>
		/// <param name="keyTerms">key term match surrogates</param>
		/// <param name="renderingsRepository">Repository of term rendering info</param>
		/// ------------------------------------------------------------------------------------
		public PhrasePartManager(IEnumerable<string> translatableParts,
			IEnumerable<KeyTermMatchSurrogate> keyTerms,
			ITermRenderingsRepo renderingsRepository)
		{
			m_renderingsRepository = renderingsRepository;
			PopulatePartsTable(translatableParts);
            if (keyTerms != null)
                PopulateKeyTermsTable(keyTerms);
        }
        #endregion

        #region Public Properties
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Enumerates the full (unique) list of all translatable parts
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public List<Part> AllTranslatableParts
	    {
            get
            {
	            if (m_allPartsList == null)
					m_allPartsList = m_partsTable.Values.SelectMany(thing => thing.Values.SelectMany(parts => parts)).ToList();
				return m_allPartsList;
			}
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
			if (keyTermSurrogates == null)
				return;

            foreach (KeyTermMatchSurrogate surrogate in keyTermSurrogates)
                AddKeyTermMatch(surrogate);
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the given match to the existing key terms table if it is not already present.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void AddKeyTermMatch(KeyTermMatchSurrogate matchSurrogate)
		{
			Word firstWord = Word.FirstWord(matchSurrogate.TermId);
			if (!m_keyTermsTable.TryGetValue(firstWord, out var termsStartingWithSameFirstWord))
				m_keyTermsTable[firstWord] = termsStartingWithSameFirstWord = new List<KeyTerm>();

			termsStartingWithSameFirstWord.Add(new KeyTerm(matchSurrogate, m_renderingsRepository));
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
		#endregion

		#region Public methods
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
	    public Part GetOrCreatePart(List<Word> words, TranslatablePhrase owningPhraseOfPart,
            bool unconditionallyCreate)
        {
			List<Part> parts = null;
            Part part = null;
            if (m_partsTable.TryGetValue(words.Count, out var partsDictionary))
            {
                if (partsDictionary.TryGetValue(words.First(), out parts) && !unconditionallyCreate)
                    part = parts.FirstOrDefault(x => x.Words.SequenceEqual(words));
            }
            else
                m_partsTable[words.Count] = partsDictionary = new Dictionary<Word, List<Part>>();

            if (parts == null)
                partsDictionary[words.First()] = parts = new List<Part>();

            if (part == null)
            {
                part = new Part(words);
                parts.Add(part);
				m_allPartsList?.Add(part);
			}

            if (owningPhraseOfPart != null)
                part.AddOwningPhrase(owningPhraseOfPart);

            return part;
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// For each "ParsedPart" in the phrase/question, populate the phrase's Parts with the
		/// appropriate key term or "Part" (adding it to the master repo of parts, if needed).
		/// </summary>
		/// <param name="phrase">A translatable phrase which has already been parsed but not
		/// yet initialized with real IPhraseParts</param>
		/// ------------------------------------------------------------------------------------
		public void InitializePhraseParts(TranslatablePhrase phrase)
		{
			Debug.Assert(phrase.QuestionInfo.ParsedParts.Any());
			Debug.Assert(!phrase.m_parts.Any());

			foreach (ParsedPart part in phrase.QuestionInfo.ParsedParts)
			{
				IPhrasePart newPart;
				if (part.Type == PartType.TranslatablePart)
					newPart = GetOrCreatePart(part.Words, phrase, false);
				else if (part.Type == PartType.KeyTerm)
					newPart = FindKeyTerm(part.Words);
				else
					newPart = new Number(part.NumericValue, phrase.Helper);
				phrase.m_parts.Add(newPart);
			}
		}

		// TODO: The following was an attempt to copy and modify code from MasterQuestionParser to
		// enable parts to be further divided based on existing smaller parts. But it's not trivial
		// and if we have to do it, it would be better to find a way without so much (almost) code
		// duplication.
		// Perhaps the solution is to make it possible to initialize the MasterQuestionParser's parts
		// table (which holds ParsedParts) from PhrasePartManager's parts table (which uses actual
		// Parts).
		
		//public void ParseAddedQuestion(TranslatablePhrase phrase)
		//{
		//	var question = phrase.QuestionInfo;
		//	List<ParsedPart> partsToDelete = new List<ParsedPart>();

		//	// Any newly created parts should be subdivided into smaller component parts if possible
		//	for (int iPart = 0; iPart < question.ParsedParts.Count; )
		//	{
		//		var part = question.ParsedParts[iPart];
		//		if (part.Type != PartType.TranslatablePart || part.Owners.Count() > 1 || part.Words.Count == 1)
		//		{
		//			iPart++;
		//			continue;
		//		}
		//		SubPhraseMatch match = FindSubPhraseMatch(part);
		//		if (match != null)
		//		{
		//			if (match.StartIndex > 0)
		//			{
		//				Part precedingPart = GetOrCreatePart(part.GetSubWords(0, match.StartIndex).ToList(),
		//					phrase, false);
		//				question.ParsedParts.Insert(iPart++, precedingPart);
		//			}
		//			question.ParsedParts.Insert(iPart++, match.Part);
		//			if (match.StartIndex + match.Part.Words.Count < part.Words.Count)
		//			{
		//				ParsedPart followingPart = GetOrCreatePart(part.GetSubWords(match.StartIndex + match.Part.Words.Count), question);
		//				question.ParsedParts.Insert(iPart, followingPart);
		//			}
		//			if (match.StartIndex > 0)
		//				iPart -= 2; // Need to come back around and deal with possibility of further breaking up preceding part.
		//			partsToDelete.Add(part);
		//		}
		//		else
		//		{
		//			iPart++;
		//		}
		//	}

		//	foreach (ParsedPart partToDelete in partsToDelete)
		//	{
		//		Dictionary<Word, List<ParsedPart>> partsTable;
		//		if (!m_partsTable.TryGetValue(partToDelete.Words.Count, out partsTable))
		//			continue;
		//		partsTable[partToDelete.Words[0]].Remove(partToDelete);
		//	}
		//}

		//#region SubPhraseMatch class
		//private class SubPhraseMatch
		//{
		//	internal readonly int StartIndex;
		//	internal readonly Part Part;

		//	public SubPhraseMatch(int startIndex, Part part)
		//	{
		//		StartIndex = startIndex;
		//		Part = part;
		//	}
		//}
		//#endregion

		///// ------------------------------------------------------------------------------------
		///// <summary>
		///// Finds the longest phrase that is a sub-phrase of the specified part.
		///// </summary>
		///// <param name="part">The part.</param>
		///// ------------------------------------------------------------------------------------
		//private SubPhraseMatch FindSubPhraseMatch(ParsedPart part)
		//{
		//	int partWordCount = part.Words.Count();
		//	for (int subPhraseWordCount = partWordCount - 1; subPhraseWordCount > 0; subPhraseWordCount--)
		//	{
		//		Dictionary<Word, List<Part>> subPhraseTable;
		//		if (!m_partsTable.TryGetValue(subPhraseWordCount, out subPhraseTable))
		//			continue;

		//		for (int iWord = 0; iWord < partWordCount; iWord++)
		//		{
		//			Word word = part.Words.ElementAt(iWord);
		//			if (iWord + subPhraseWordCount > partWordCount)
		//				break; // There aren't enough words left in this part to find a match
		//			if (subPhraseWordCount == 1 && MasterQuestionParser.prepositionsAndArticles.Contains(word))
		//				break; // Don't want to split a phrase using a part that consists of a single preposition or article.

		//			List<Part> possibleSubParts;
		//			if (subPhraseTable.TryGetValue(word, out possibleSubParts))
		//			{
		//				foreach (Part possibleSubPart in possibleSubParts)
		//				{
		//					int iWordTemp = iWord + 1;
		//					int iSubWord = 1;
		//					int possiblePartWordCount = possibleSubPart.Words.Count();
		//					while (iSubWord < possiblePartWordCount && possibleSubPart.Words.ElementAt(iSubWord) == part.Words.ElementAt(iWordTemp++))
		//						iSubWord++;
		//					if (iSubWord == possiblePartWordCount)
		//						return new SubPhraseMatch(iWord, possibleSubPart);
		//				}
		//			}
		//		}
		//	}
		//	return null;
		//}

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Finds the key term which matches the given list of words (assumed to exist)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IPhrasePart FindKeyTerm(List<Word> words)
        {
            return (m_keyTermsTable[words[0]]).First(kt => kt.Words.SequenceEqual(words));
        }
	    #endregion
	}
}
