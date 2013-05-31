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
// File: MasterQuestionParser.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AddInSideViews;
using SIL.Utils;
using System;

namespace SIL.Transcelerator
{
	/// --------------------------------------------------------------------------------------------
	/// <summary>
	/// Takes the "raw" questions (from TxlQuestions.xml), processes them using the customizations
	/// and phrase substitution rules, and then parses them using the key terms (as massaged by the
	/// key term rules) to generate a collection of pre-parsed questions that can be loaded quickly
	/// by the QuestionProvider for use by PhraseTranslationHelper.
	/// </summary>
	/// --------------------------------------------------------------------------------------------
	public class MasterQuestionParser
	{
	    #region Data members

	    private QuestionSections m_sections;
	    private readonly IEnumerable<PhraseCustomization> m_customizations;
        private readonly Dictionary<Regex, string> m_phraseSubstitutions;
        /// <summary>A lookup table of the last word of all known English key terms to the
		/// actual key term objects.</summary>
		private readonly Dictionary<Word, List<KeyTermMatch>> m_keyTermsTable;
		/// <summary>A double lookup table of all parts in all phrases managed by this class.
		/// For improved performance, outer lookup is by wordcount.</summary>
        private readonly SortedDictionary<int, Dictionary<Word, List<ParsedPart>>> m_partsTable;
		#endregion

        #region SubPhraseMatch class
        private class SubPhraseMatch
        {
            internal readonly int StartIndex;
            internal readonly ParsedPart Part;

            public SubPhraseMatch(int startIndex, ParsedPart part)
            {
                StartIndex = startIndex;
                Part = part;
            }
        }
        #endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
        /// Initializes a new instance of the <see cref="MasterQuestionParser"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
        public MasterQuestionParser(string filename, IEnumerable<IKeyTerm> keyTerms,
            KeyTermRules keyTermRules, IEnumerable<PhraseCustomization> customizations,
            IEnumerable<Substitution> phraseSubstitutions) :
            this(XmlSerializationHelper.DeserializeFromFile<QuestionSections>(filename), keyTerms,
            keyTermRules, customizations, phraseSubstitutions)
        {
        }
        
        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="PhraseTranslationHelper"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
        public MasterQuestionParser(QuestionSections sections, IEnumerable<IKeyTerm> keyTerms,
            KeyTermRules keyTermRules, IEnumerable<PhraseCustomization> customizations,
            IEnumerable<Substitution> phraseSubstitutions)
		{
            m_sections = sections;
            m_customizations = customizations;
            if (keyTerms != null)
            {
                m_keyTermsTable = new Dictionary<Word, List<KeyTermMatch>>(keyTerms.Count());
                PopulateKeyTermsTable(keyTerms, keyTermRules);
            }

            if (phraseSubstitutions != null)
            {
                m_phraseSubstitutions = new Dictionary<Regex, string>(phraseSubstitutions.Count());
                foreach (Substitution substitutePhrase in phraseSubstitutions)
                    m_phraseSubstitutions[substitutePhrase.RegEx] = substitutePhrase.RegExReplacementString;
            }

            m_partsTable = new SortedDictionary<int, Dictionary<Word, List<ParsedPart>>>();
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Performs the parsing logic to divide question text into translatable parts and key term parts.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void Parse()
	    {
	        foreach (Question question in GetQuestions())
	        {
	            if (question.IsParsable)
	            {
	                PhraseParser parser = new PhraseParser(m_keyTermsTable, m_phraseSubstitutions, question, GetOrCreatePart);
	                foreach (ParsedPart part in parser.Parse())
	                    question.ParsedParts.Add(part);
	            }
	        }

	        for (int wordCount = m_partsTable.Keys.Max(); wordCount > 1; wordCount--)
	        {
	            Dictionary<Word, List<ParsedPart>> partsTable;
	            if (!m_partsTable.TryGetValue(wordCount, out partsTable))
	                continue;

	            List<ParsedPart> partsToDelete = new List<ParsedPart>();

	            foreach (KeyValuePair<Word, List<ParsedPart>> phrasePartPair in partsTable)
	                // REVIEW: problem: won't be able to add a new part that starts with this word
	            {
	                foreach (ParsedPart part in phrasePartPair.Value)
	                {
	                    if (part.Owners.Count() != 1)
	                        continue;

	                    // Look to see if some other part is a sub-phrase of this part.
	                    SubPhraseMatch match = FindSubPhraseMatch(part);
	                    if (match != null)
	                    {
	                        Question owningPhraseOfPart = part.Owners.First();
	                        int iPart = owningPhraseOfPart.ParsedParts.IndexOf(part);
	                        // Deal with any preceding remainder
	                        if (match.StartIndex > 0)
	                        {
	                            ParsedPart preceedingPart = GetOrCreatePart(part.GetSubWords(0, match.StartIndex),
	                                                                        owningPhraseOfPart);
	                            owningPhraseOfPart.ParsedParts.Insert(iPart++, preceedingPart);
	                        }
	                        match.Part.AddOwningPhrase(owningPhraseOfPart);
	                        owningPhraseOfPart.ParsedParts[iPart++] = match.Part;
	                        // Deal with any following remainder
	                        // Breaks this part at the given position because an existing part was found to be a
	                        // substring of this part. Any text before the part being excluded will be broken off
	                        // as a new part and returned. Any text following the part being excluded will be kept
	                        // as this part's contents.
	                        if (match.StartIndex + match.Part.Words.Count < part.Words.Count)
	                        {
	                            ParsedPart followingPart =
	                                GetOrCreatePart(part.GetSubWords(match.StartIndex + match.Part.Words.Count),
	                                                owningPhraseOfPart);
	                            owningPhraseOfPart.ParsedParts.Insert(iPart, followingPart);
	                        }
	                        partsToDelete.Add(part);
	                    }
	                }
	            }
	            foreach (ParsedPart partToDelete in partsToDelete)
	            {
	                partsTable[partToDelete.Words[0]].Remove(partToDelete);
	            }
	        }
	    }

	    #endregion

        #region Public properties
	    public ParsedQuestions Result
	    {
	        get
	        {
                Parse();

                ParsedQuestions result = new ParsedQuestions();
                if (m_keyTermsTable != null)
                {
                    result.KeyTerms = m_keyTermsTable.Values.SelectMany(l => l).Where(k => k.InUse).Select(i => (KeyTermMatchSurrogate)i).ToArray();
                    //var matchesInUse = m_keyTermsTable.Values.SelectMany(l => l).Where(k => k.InUse).ToList();
                    //result.KeyTerms = new KeyTermMatchSurrogate[matchesInUse.Count];
                    //for (int i = 0; i < matchesInUse.Count; i++)
                    //    result.KeyTerms[i] = matchesInUse[i];
                }
	            result.TranslatableParts = m_partsTable.Values.SelectMany(d => d.Values).SelectMany(l => l).Select(p => p.Words.ToString(" ")).ToArray();
	            result.Sections = m_sections;
	            return result;
	        }
	    }
        #endregion

        #region Private helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets (possibly modified form of) the given phrase along with any inserted (before)
        /// or added (after) phrases. 
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private IEnumerable<Question> GetCustomizations(Question q, Category category, int index)
        {
            Question added = null;
            var list = m_customizations.Where(c => q.Matches(c.Reference, c.OriginalPhrase)).ToList();
            Debug.WriteLine(list);
            foreach (PhraseCustomization customization in m_customizations.Where(c => q.Matches(c.Reference, c.OriginalPhrase)))
            {
                switch (customization.Type)
                {

                    case PhraseCustomization.CustomizationType.Deletion:
                        q.IsExcluded = true;
                        break;
                    case PhraseCustomization.CustomizationType.Modification:
                        if (q.ModifiedPhrase != null)
                        {
                            throw new InvalidOperationException("Only one modified version of a question/phrase is permitted. Question/phrase '" + q.Text +
                                "' has already been modified as '" + q.ModifiedPhrase + "'. Value of subsequent modification attempt was: '" +
                                customization.ModifiedPhrase + "'.");
                        }
                        q.ModifiedPhrase = customization.ModifiedPhrase;
                        break;
                    case PhraseCustomization.CustomizationType.InsertionBefore:
                        if (q.InsertedQuestionBefore != null)
                        {
                            throw new InvalidOperationException("Only one question/phrase is permitted to be inserted. Question/phrase '" + q.Text +
                                "' already has a question/phrase inserted before it: '" + q.InsertedQuestionBefore + "'. Value of subsequent insertion attempt was: '" +
                                customization.ModifiedPhrase + "'.");
                        }
                        q.InsertedQuestionBefore = new Question(q, customization.ModifiedPhrase, customization.Answer);
                        category.Questions.Insert(index, q.InsertedQuestionBefore);
                        foreach (Question customQuestion in GetCustomizations(q.InsertedQuestionBefore, category, index))
                        {
                            yield return customQuestion;
                            index++;
                        }
                        break;
                    case PhraseCustomization.CustomizationType.AdditionAfter:
                        if (q.AddedQuestionAfter != null)
                        {
                            throw new InvalidOperationException("Only one question/phrase is permitted to be added. Question/phrase '" + q.Text +
                                "' already has a question/phrase added after it: '" + q.AddedQuestionAfter + "'. Value of of subsequent addition attempt was: '" +
                                customization.ModifiedPhrase + "'.");
                        }
                        added = q.AddedQuestionAfter = new Question(q, customization.ModifiedPhrase, customization.Answer);
                        category.Questions.Insert(index + 1, q.AddedQuestionAfter);
                        break;
                }
            }
            yield return q;
            if (added != null)
            {
                foreach (Question tpAdded in GetCustomizations(added, category, index + 1))
                    yield return tpAdded;
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all the questions/phrases from each section, including any customized or added
        /// ones. Any additions are actually added to the collection of questions for the
        /// appropriate section & category.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private IEnumerable<Question> GetQuestions()
        {
            //HashSet<string> processedCategories = new HashSet<string>();
            foreach (Section section in m_sections.Items)
            {
                for (int iCat = 0; iCat < section.Categories.Length; iCat++)
                {
                    Category category = section.Categories[iCat];

                    for (int iQuestion = 0; iQuestion < category.Questions.Count;)
                    {
                        Question q = category.Questions[iQuestion];

                        if (string.IsNullOrEmpty(q.Text))
                        {
                            category.Questions.RemoveAt(iQuestion);
                            continue;
                        }

                        if (q.ScriptureReference == null)
                        {
                            q.ScriptureReference = section.ScriptureReference;
                            q.StartRef = section.StartRef;
                            q.EndRef = section.EndRef;
                        }
                        if (m_customizations != null)
                        {
                            foreach (Question inserted in GetCustomizations(q, category, iQuestion))
                            {
                                yield return inserted;
                                iQuestion++;
                            }
                        }
                        else
                        {
                            yield return q;
                            iQuestion++;
                        }
                    }
                }
            }
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Populates the key terms table.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void PopulateKeyTermsTable(IEnumerable<IKeyTerm> keyTerms, KeyTermRules rules)
		{
			KeyTermMatchBuilder matchBuilder;

			foreach (IKeyTerm keyTerm in keyTerms)
			{
			    matchBuilder = new KeyTermMatchBuilder(keyTerm,
                    rules == null ? null : rules.RulesDictionary, rules == null ? null : rules.RegexRules);

			    foreach (KeyTermMatch matcher in matchBuilder.Matches.Where(matcher => matcher.WordCount != 0))
			    {
			        List<KeyTermMatch> foundMatchers;
			        Word firstWord = matcher[0];
			        if (!m_keyTermsTable.TryGetValue(firstWord, out foundMatchers))
			            m_keyTermsTable[firstWord] = foundMatchers = new List<KeyTermMatch>();

			        KeyTermMatch existingMatcher = foundMatchers.FirstOrDefault(m => m.Equals(matcher));
			        if (existingMatcher == null)
			            foundMatchers.Add(matcher);
			        else
			            existingMatcher.AddTerm(keyTerm);
			    }
			}

#if DEBUG
            if (rules != null)
            {
                string unUsedRules = rules.RulesDictionary.Values.Where(r => !r.Used).ToString(Environment.NewLine);
                if (unUsedRules.Length > 0)
                {
                    MessageBox.Show("Unused KeyTerm Rules: \n" + unUsedRules, "Transcelerator");
                }
            }
#endif
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or creates a part matching the given sub-phrase.
        /// </summary>
        /// <param name="words">The words of the sub-phrase.</param>
        /// <param name="owningPhraseOfPart">The owning phrase of the part to find or create.</param>
        /// ------------------------------------------------------------------------------------
        private ParsedPart GetOrCreatePart(IEnumerable<Word> words, Question owningPhraseOfPart)
        {
			Debug.Assert(words.Any());
            ParsedPart part = null;

			Dictionary<Word, List<ParsedPart>> partsTable;
			List<ParsedPart> parts = null;
			if (m_partsTable.TryGetValue(words.Count(), out partsTable))
			{
				if (partsTable.TryGetValue(words.First(), out parts))
					part = parts.FirstOrDefault(x => x.Words.SequenceEqual(words));
			}
			else
				m_partsTable[words.Count()] = partsTable = new Dictionary<Word, List<ParsedPart>>();

			if (parts == null)
				partsTable[words.First()] = parts = new List<ParsedPart>();

			if (part == null)
			{
				part = new ParsedPart(words);
				parts.Add(part);
			}

            part.AddOwningPhrase(owningPhraseOfPart);

			return part;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the longest phrase that is a sub-phrase of the specified part.
		/// </summary>
		/// <param name="part">The part.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		private SubPhraseMatch FindSubPhraseMatch(ParsedPart part)
		{
			int partWordCount = part.Words.Count;
			for (int subPhraseWordCount = partWordCount - 1; subPhraseWordCount > 1; subPhraseWordCount--)
			{
				Dictionary<Word, List<ParsedPart>> subPhraseTable;
				if (!m_partsTable.TryGetValue(subPhraseWordCount, out subPhraseTable))
					continue;

				for (int iWord = 0; iWord < partWordCount; iWord++)
				{
					Word word = part.Words[iWord];
					if (iWord + subPhraseWordCount > partWordCount)
						break; // There aren't enough words left in this part to find a match
					List<ParsedPart> possibleSubParts;
					if (subPhraseTable.TryGetValue(word, out possibleSubParts))
					{
						foreach (ParsedPart possibleSubPart in possibleSubParts)
						{
							int iWordTemp = iWord + 1;
							int isubWord = 1;
							int possiblePartWordCount = possibleSubPart.Words.Count;
							while (isubWord < possiblePartWordCount && possibleSubPart.Words[isubWord] == part.Words[iWordTemp++])
								isubWord++;
							if (isubWord == possiblePartWordCount)
								return new SubPhraseMatch(iWord, possibleSubPart);
						}
					}
				}
			}
			return null;
		}
		#endregion
	}
}
