// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2013' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: MasterQuestionParser.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AddInSideViews;
using SIL.Utils;
using System;
using System.Windows.Forms;
using SIL.Extensions;
using SIL.Scripture;
using SIL.Xml;

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
        // TODO: Don't hard-code this. Should be read from language-specific file along with leading question words.
        public static readonly List<Word> prepositionsAndArticles = new List<Word>(new Word[] { "in", "of", "the", "a", "an", "for", "by", "through", "on", "about", "to" });

	    private readonly QuestionSections m_sections;
	    private Dictionary<int, List<PhraseCustomization>> m_customizations; // One list per book
        private readonly Dictionary<Regex, string> m_phraseSubstitutions;
        /// <summary>A lookup table of the last word of all known English key terms to the
		/// actual key term objects.</summary>
		private readonly Dictionary<Word, List<KeyTermMatch>> m_keyTermsTable;
		/// <summary>A double lookup table of all parts in all phrases managed by this class.
		/// For improved performance, outer lookup is by wordcount.</summary>
        private readonly SortedDictionary<int, Dictionary<Word, List<ParsedPart>>> m_partsTable;

		private readonly IDictionary<int, List<List<Word>>> m_questionWordsLookupTable;
		private readonly IEnumerable<string> m_questionWords;

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
        public MasterQuestionParser(string filename, IEnumerable<string> questionWords,
            IEnumerable<IKeyTerm> keyTerms, KeyTermRules keyTermRules,
            IEnumerable<PhraseCustomization> customizations,
            IEnumerable<Substitution> phraseSubstitutions) :
            this(XmlSerializationHelper.DeserializeFromFile<QuestionSections>(filename),
            questionWords, keyTerms, keyTermRules, customizations, phraseSubstitutions)
        {
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="MasterQuestionParser"/> class. This
		/// version is useful when you just need a parser to use for ad-hoc questions, since
		/// it does not take an initial collection of sections with questions to be parsed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public MasterQuestionParser(IEnumerable<string> questionWords,
			IEnumerable<IKeyTerm> keyTerms, KeyTermRules keyTermRules,
			IEnumerable<Substitution> phraseSubstitutions) : this(default(QuestionSections), questionWords,
			keyTerms, keyTermRules, null, phraseSubstitutions)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="MasterQuestionParser"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
        public MasterQuestionParser(QuestionSections sections, IEnumerable<string> questionWords,
            IEnumerable<IKeyTerm> keyTerms, KeyTermRules keyTermRules,
            IEnumerable<PhraseCustomization> customizations,
            IEnumerable<Substitution> phraseSubstitutions)
		{
            m_sections = sections;
	        m_questionWords = questionWords;
	        if (questionWords != null)
	        {
		        m_questionWordsLookupTable = new Dictionary<int, List<List<Word>>>();
		        foreach (string questionWordPhrase in questionWords)
		        {
			        List<Word> listOfWordsInQuestion = questionWordPhrase.Split(' ').Select(w => (Word) w).ToList();
			        int count = listOfWordsInQuestion.Count;
			        List<List<Word>> listOfQuestionsForCount;
			        if (!m_questionWordsLookupTable.TryGetValue(count, out listOfQuestionsForCount))
				        m_questionWordsLookupTable[count] = listOfQuestionsForCount = new List<List<Word>>();
			        listOfQuestionsForCount.Add(listOfWordsInQuestion);
		        }
	        }
			if (customizations != null)
			{
				m_customizations = new Dictionary<int, List<PhraseCustomization>>();
				foreach (var customization in customizations)
				{
					var key = customization.ScrStartReference.Book;
					List<PhraseCustomization> list;
					if (!m_customizations.TryGetValue(key, out list))
						m_customizations[key] = list = new List<PhraseCustomization>();
					list.Add(customization);
				}
			}
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
			if (m_partsTable.Any())
				throw new InvalidOperationException("Parse called more than once.");

	        foreach (Question question in GetQuestions())
	            ParseQuestion(question);

	        for (int wordCount = m_partsTable.Keys.Max(); wordCount > 0; wordCount--)
	        {
	            Dictionary<Word, List<ParsedPart>> partsTable;
	            if (!m_partsTable.TryGetValue(wordCount, out partsTable))
	                continue;

	            int maxAllowableOccurrencesForSplitting = Math.Max(2, (26 - 2^wordCount)/2);

	            List<ParsedPart> partsToDelete = new List<ParsedPart>();

	            foreach (KeyValuePair<Word, List<ParsedPart>> phrasePartPair in partsTable)
	                // REVIEW: problem: won't be able to add a new part that starts with this word - Is this really a problem?
	            {
	                foreach (ParsedPart part in phrasePartPair.Value)
	                {
	                    int numberOfOccurrencesOfPart = part.Owners.Count();
                        if (numberOfOccurrencesOfPart > maxAllowableOccurrencesForSplitting)
                            continue;

	                    // Look to see if some other part is a sub-phrase of this part.
	                    SubPhraseMatch match = FindSubPhraseMatch(part);
                        // Should an uncommon match be able to break a common one? If not, should we keep looking for a better sub-phrase match?
                        if (match != null/* && NEEDS WORK: part.Owners.Count() < match.Part.Owners.Count() * 2*/)
	                    {
	                        foreach (var owningPhraseOfPart in part.Owners)
	                        {
	                            //Question owningPhraseOfPart = part.Owners.First();
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
	            }
	            foreach (ParsedPart partToDelete in partsToDelete)
	            {
	                partsTable[partToDelete.Words[0]].Remove(partToDelete);
	            }
	        }
	    }

		public void ParseQuestion(Question question)
		{
			if (question.IsParsable)
			{
				PhraseParser parser = new PhraseParser(m_keyTermsTable, m_phraseSubstitutions,
					m_questionWordsLookupTable, question, GetOrCreatePart);
				foreach (ParsedPart part in parser.Parse())
					question.ParsedParts.Add(part);
			}
		}

		public bool ParseNewOrModifiedQuestion(Question question, Action<KeyTermMatch> processKeyTermMatch)
		{
			if (question.IsParsable)
			{
				PhraseParser parser = new PhraseParser(m_keyTermsTable, m_phraseSubstitutions,
					m_questionWordsLookupTable, question, GetOrCreatePart);
				foreach (ParsedPart part in parser.Parse())
					question.ParsedParts.Add(part);
				foreach (var match in parser.KeyTermsUsedForPhrase)
					processKeyTermMatch(match);
				return true;
			}
			return false;
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
        /// or added (after) phrases. Also note any "Deletions" (i.e., exclusions).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private static IEnumerable<Question> GetCustomizations(Question q, Category category, int index,
			List<PhraseCustomization> customizations, int processAllAdditionsBeforeRef = 0)
        {
            var list = new List<PhraseCustomization>();
			int mismatchedCustomizationToRemove = -1;
		    int deletionsToIgnore = 0;
		    for (int i = 0; i < customizations.Count; i++)
		    {
			    var customization = customizations[i];
			    if ((customization.Type == PhraseCustomization.CustomizationType.AdditionAfter || customization.Type == PhraseCustomization.CustomizationType.InsertionBefore) &&
				    (customization.ScrStartReference < q.StartRef || (customization.ScrStartReference == q.StartRef && customization.ScrEndReference > q.EndRef)))
			    {
				    if (mismatchedCustomizationToRemove == -1 && !list.Any(c => c.Type == PhraseCustomization.CustomizationType.InsertionBefore))
				    {
					    mismatchedCustomizationToRemove = i;
					    list.Insert(0, customization);
				    }
				    else if (mismatchedCustomizationToRemove >= 0)
				    {
					    var existingInsertion = list[0];
					    // The existing insertion is also a mismatch. Should this one go ahead of it? 
					    if (existingInsertion.ScrStartReference > customization.ScrStartReference ||
						    (existingInsertion.Reference == customization.Reference && existingInsertion.ModifiedPhrase == customization.OriginalPhrase &&
							customization.Type == PhraseCustomization.CustomizationType.InsertionBefore))
					    {
						    list[0] = customization;
						    mismatchedCustomizationToRemove = i;
					    }
				    }
			    }
			    else if (q.Matches(customization.Reference, customization.OriginalPhrase))
			    {
				    int deletionsRemoved = list.RemoveAll(c => c.Type == PhraseCustomization.CustomizationType.Deletion);
					// Check for duplicate
					if (q.PhraseInUse == customization.ModifiedPhrase && customization.Type != PhraseCustomization.CustomizationType.Deletion && customization.OriginalPhrase == customization.ModifiedPhrase)
				    {
					    // Compare answers. If one is a substring of the other, keep the superstring version
					    // Otherwise, add another answer. Or maybe check for deletions and disregard...?
					    if (customization.Answer != null && (q.Answers == null || !q.Answers.Any(a => a.Contains(customization.Answer))))
					    {
						    int iAnswerWhichIsContainedByCustomizedAnswer = q.Answers?.IndexOf(a => customization.Answer.Contains(a)) ?? -1;
						    if (iAnswerWhichIsContainedByCustomizedAnswer >= 0)
						    {
								Debug.Assert(q.Answers != null); // This satisfies resharper and hopefully clarifies the intention of the above code.
							    q.Answers[iAnswerWhichIsContainedByCustomizedAnswer] = customization.Answer;
						    }
						    else
						    {
							    var answers = q.Answers?.ToList() ?? new List<string>();
							    answers.Add(customization.Answer);
							    q.Answers = answers.ToArray();
						    }
					    }
					    deletionsToIgnore += 1 - deletionsRemoved;
				    }
				    else if (customization.Type == PhraseCustomization.CustomizationType.Deletion && deletionsToIgnore > 0)
				    {
					    deletionsToIgnore--;
				    }
				    else
				    {
					    if (mismatchedCustomizationToRemove >= 0 && customization.Type == PhraseCustomization.CustomizationType.InsertionBefore)
					    {
						    // We found a "real" insertion before. Any prceeding ones will have to go before THAT one instead.
						    list[0] = customization;
						    mismatchedCustomizationToRemove = -1;
					    }
					    else if (customization.Type == PhraseCustomization.CustomizationType.Deletion && list.Any(c => c.Type == PhraseCustomization.CustomizationType.Deletion))
						    continue; // Can't delete the same thing twice. This will presumably exclude another copy of it.
						else
						    list.Add(customization);
				    }
				    customizations.RemoveAt(i--);
			    }
			    else if ((customization.Type == PhraseCustomization.CustomizationType.AdditionAfter || customization.Type == PhraseCustomization.CustomizationType.InsertionBefore) &&
				    customization.ScrStartReference < processAllAdditionsBeforeRef)
			    {
				    customizations.RemoveAt(i);
				    q.AddedQuestionAfter = new Question(customization.Reference, customization.ScrStartReference, customization.ScrEndReference,
					    customization.ModifiedPhrase, customization.Answer);
				    category.Questions.Insert(index, q.AddedQuestionAfter);
				    foreach (Question customQuestion in GetCustomizations(q.AddedQuestionAfter, category, index, customizations))
				    {
					    yield return customQuestion;
					    index++;
				    }
			    }
			}
		    if (mismatchedCustomizationToRemove >= 0)
		    {
			    // If we found an added question that didn't match the current question but needs to go before it,
			    // it is guaranteed to be the first one in the list and it must be treated as an insertion, unless we're
			    // processing trailing additions
			    customizations.RemoveAt(mismatchedCustomizationToRemove);
				list[0].Type = processAllAdditionsBeforeRef > 0 ? PhraseCustomization.CustomizationType.AdditionAfter :
					PhraseCustomization.CustomizationType.InsertionBefore;
		    }

	        foreach (var customization in list)
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
						// TODO: Support modifications (and additions?) of answers (and notes?) also.
                        break;
                    case PhraseCustomization.CustomizationType.InsertionBefore:
                        if (q.InsertedQuestionBefore != null)
                        {
                            throw new InvalidOperationException("Only one question/phrase is permitted to be inserted. Question/phrase '" + q.Text +
                                "' already has a question/phrase inserted before it: '" + q.InsertedQuestionBefore + "'. Value of subsequent insertion attempt was: '" +
                                customization.ModifiedPhrase + "'.");
                        }
                        q.InsertedQuestionBefore = new Question(customization.Reference, customization.ScrStartReference, customization.ScrEndReference,
                            customization.ModifiedPhrase, customization.Answer);
                        category.Questions.Insert(index, q.InsertedQuestionBefore);
                        foreach (Question customQuestion in GetCustomizations(q.InsertedQuestionBefore, category, index, customizations))
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
                        q.AddedQuestionAfter = new Question(customization.Reference, customization.ScrStartReference, customization.ScrEndReference,
                            customization.ModifiedPhrase, customization.Answer);
                        category.Questions.Insert(index + (processAllAdditionsBeforeRef > 0 ? 0 : 1), q.AddedQuestionAfter);
                        break;
                }
            }
	        if (processAllAdditionsBeforeRef == 0)
	        {
				if (q.InsertedQuestionBefore != null && q.ScriptureReference != q.InsertedQuestionBefore.ScriptureReference)
				{
					foreach (Question tpAdded in GetCustomizations(q.InsertedQuestionBefore, category, index, customizations, q.StartRef))
					{
						yield return tpAdded;
						index++;
					}
				}
		        yield return q;
	        }
	        if (processAllAdditionsBeforeRef == 0 && q.AddedQuestionAfter != null)
            {
	            foreach (Question tpAdded in GetCustomizations(q.AddedQuestionAfter, category, index + 1, customizations))
	            {
		            yield return tpAdded;
		            index++;
	            }
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
	        int currBook = -1;
	        List<PhraseCustomization> currCustomizations = null;
	        Category category = null;
			Question lastQuestionInBook = null;
	        int iQuestion = -1;
			foreach (Section section in m_sections.Items)
            {
	            if (m_customizations != null && BCVRef.GetBookFromBcv(section.StartRef) != currBook)
	            {
		            foreach (var question in GetTrailingCustomizations(currBook, currCustomizations, lastQuestionInBook,
						category, iQuestion))
			            yield return question;

		            currBook = BCVRef.GetBookFromBcv(section.StartRef);
		            m_customizations.TryGetValue(currBook, out currCustomizations);
				}
	            for (int iCat = 0; iCat < section.Categories.Length; iCat++)
                {
                    category = section.Categories[iCat];

                    for (iQuestion = 0; iQuestion < category.Questions.Count;)
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
                        if (currCustomizations != null)
                        {
                            foreach (var question in GetCustomizations(q, category, iQuestion, currCustomizations))
                            {
	                            lastQuestionInBook = question;
								yield return question;
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
	        if (m_customizations != null)
	        {
		        foreach (var question in GetTrailingCustomizations(currBook, currCustomizations, lastQuestionInBook,
			        category, iQuestion))
			        yield return question;
				m_customizations = null; // Allow this to be garbage collected (and prevent accidental future use)
	        }
		}

		private static IEnumerable<Question> GetTrailingCustomizations(int bookNum, List<PhraseCustomization> currCustomizations, Question lastQuestionInBook,
			Category category, int iQuestion)
		{
			if (currCustomizations != null && currCustomizations.Any())
			{
				Debug.Assert(lastQuestionInBook != null && iQuestion > 0 && category != null,
					$"Book {BCVRef.NumberToBookCode(bookNum)} has no built-in questions - cannot process customizations!");

				var maxRefForBook = new BCVRef(bookNum, 150, 999).BBCCCVVV; // This is an oversimplification, but we just need a usable limit. Could just use MaxInt.
				while (currCustomizations.Any())
				{
					foreach (var question in GetCustomizations(lastQuestionInBook, category, iQuestion, currCustomizations, maxRefForBook))
					{
						lastQuestionInBook = question;
						yield return question;
						iQuestion++;
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
		/// ------------------------------------------------------------------------------------
		private SubPhraseMatch FindSubPhraseMatch(ParsedPart part)
		{
			if (m_questionWords != null && m_questionWords.Contains(part.Text))
				return null;

			int partWordCount = part.Words.Count;
			for (int subPhraseWordCount = partWordCount - 1; subPhraseWordCount > 0; subPhraseWordCount--)
			{
				Dictionary<Word, List<ParsedPart>> subPhraseTable;
				if (!m_partsTable.TryGetValue(subPhraseWordCount, out subPhraseTable))
					continue;

				for (int iWord = 0; iWord < partWordCount; iWord++)
				{
					Word word = part.Words[iWord];
					if (iWord + subPhraseWordCount > partWordCount)
						break; // There aren't enough words left in this part to find a match
                    if (subPhraseWordCount == 1 && prepositionsAndArticles.Contains(word))
				        break; // Don't want to split a phrase using a part that consists of a single preposition or article.

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

		public bool ReparseModifiedQuestion(Question question, Action<KeyTermMatch> processKeyTermMatch)
		{
			var previousParts = new List<ParsedPart>(question.ParsedParts);
			question.ParsedParts.Clear();
			if (!ParseNewOrModifiedQuestion(question, processKeyTermMatch))
			{
				question.ParsedParts.AddRange(previousParts);
				return false;
			}
			var newParts = new List<ParsedPart>(question.ParsedParts);
			question.ParsedParts.Clear();

			foreach (var newPart in newParts)
			{
				if (newPart.Type != PartType.TranslatablePart)
					question.ParsedParts.Add(newPart);
				else
				{
					int iStartOfUnmatchedWordInPart = 0;
					for (int iWordInNewPart = 0; iWordInNewPart < newPart.Words.Count;)
					{
						var matchingPrevPart = previousParts.FirstOrDefault(pp => pp.Type == PartType.TranslatablePart &&
							newPart.Words.Skip(iWordInNewPart).Take(pp.Words.Count).SequenceEqual(pp.Words));
						if (matchingPrevPart != null)
						{
							if (iWordInNewPart > iStartOfUnmatchedWordInPart)
							{
								question.ParsedParts.Add(new ParsedPart(newPart.Words
									.Skip(iStartOfUnmatchedWordInPart)
									.Take(iWordInNewPart - iStartOfUnmatchedWordInPart)));	
							}
							question.ParsedParts.Add(matchingPrevPart);
							iWordInNewPart += matchingPrevPart.Words.Count;
							iStartOfUnmatchedWordInPart = iWordInNewPart;
						}
						else
							 iWordInNewPart++;
					}
					if (newPart.Words.Count > iStartOfUnmatchedWordInPart)
					{
						question.ParsedParts.Add(new ParsedPart(newPart.Words.Skip(iStartOfUnmatchedWordInPart).Take(newPart.Words.Count - iStartOfUnmatchedWordInPart)));
					}
				}
			}
			return true;
		}
	}
}
