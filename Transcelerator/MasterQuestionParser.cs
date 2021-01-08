// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2013' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
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

		private class Customizations // All customizations that share a key (used to match to a question)
		{
			private bool m_isResolved = true;
			private List<PhraseCustomization> AdditionsAndInsertions { get; set; }
			private List<PhraseCustomization> Deletions { get; set; }
			private PhraseCustomization Modification { get; set; }
			private List<string> AllAnswers;

			private string ModifiedPhrase => Modification?.ModifiedPhrase;

			public bool IsAdditionOrInsertion => AdditionsAndInsertions.Any();
			/// If the text hasn't changed, it's not a real addition; user just changed the
			/// reference or answer.
			public bool IsAdditionOfDifferentPhrase => AdditionsAndInsertions.Any(a => a.OriginalPhrase != a.ModifiedPhrase);

			public Customizations()
			{
				AdditionsAndInsertions = new List<PhraseCustomization>();
				Deletions = new List<PhraseCustomization>();
			}

			public void Add(PhraseCustomization pc)
			{
				switch (pc.Type)
				{
					case PhraseCustomization.CustomizationType.AdditionAfter:
					case PhraseCustomization.CustomizationType.InsertionBefore:
						if (AdditionsAndInsertions.Any(a => a.ModifiedPhrase == pc.ModifiedPhrase || a.Type == pc.Type))
							m_isResolved = false;
						AdditionsAndInsertions.Add(pc);
						break;
					case PhraseCustomization.CustomizationType.Deletion:
						Deletions.Add(pc);
						if (Deletions.Count > 1)
							m_isResolved = false;
						break;
					case PhraseCustomization.CustomizationType.Modification:
						if (Modification != null)
						{
							throw new InvalidOperationException("Only one modified version of a question/phrase is permitted. Question/phrase '" + pc.OriginalPhrase +
								"' has already been modified as '" + Modification.ModifiedPhrase + "'. Value of subsequent modification attempt was: '" +
								pc.ModifiedPhrase + "'.");
						}
						Modification = pc;
						break;
				}
			}

			private void SetExcludedAndModified(Question question)
			{
				question.IsExcluded = Deletions.SingleOrDefault() != null;
				Deletions.Clear();
				if (ModifiedPhrase != null)
				{
					question.ModifiedPhrase = ModifiedPhrase;
					Modification = null;
				}
			}

			public void ApplyToQuestion(Question question)
			{
				ResolveDeletionsAndAdditions();

				SetExcludedAndModified(question);

				var duplicate = AdditionsAndInsertions.SingleOrDefault(q => q.OriginalPhrase == q.ModifiedPhrase && q.OriginalPhrase == question.PhraseInUse);

				if (duplicate != null)
				{
					var newAnswer = duplicate.Answer;
					if (newAnswer.Length == 0)
						newAnswer = null;
					// Adding an exactly identical question.
					if (question.IsExcluded)
					{
						// Okay, so this is a true replacement, but if the replacement doesn't have an answer, it probably doesn't make sense, so let's ignore it
						if (newAnswer == null)
						{
							question.IsExcluded = false;
							AdditionsAndInsertions.Clear();
						}
					}
					else
					{
						if (newAnswer != null)
						{
							// We don't allow exact duplicate questions, so this can't be treated as a real addition. We're just changing the answer.
							if (question.Answers.Length == 0)
								question.Answers = new[] {newAnswer};
							else
								question.Answers[0] = newAnswer;
						}
						AdditionsAndInsertions.Remove(duplicate);
					}
				}
				// TODO: Support adding (not just replacing) answers?
				// TODO Support modifying (and replacing?) notes.

				var insertion = AdditionsAndInsertions.SingleOrDefault(a => a.Type == PhraseCustomization.CustomizationType.InsertionBefore);
				if (insertion != null)
				{
					question.InsertedQuestionBefore = new Question(insertion.Reference, insertion.ScrStartReference,
						insertion.ScrEndReference, insertion.ModifiedPhrase, insertion.Answer, insertion.ImmutableKey);
				}

				var addition = AdditionsAndInsertions.SingleOrDefault(a => a.Type == PhraseCustomization.CustomizationType.AdditionAfter);
				if (addition != null)
				{
					question.AddedQuestionAfter = new Question(addition.Reference, addition.ScrStartReference,
						addition.ScrEndReference, addition.ModifiedPhrase, addition.Answer, addition.ImmutableKey);
				}
			}

			private void ResolveDeletionsAndAdditions()
			{
				if (m_isResolved)
					return;

				AllAnswers = new List<string>();

				FinishResolvingIfNoMorePairsCanBeDeleted();

				int iDel = 0;
				// Pass 1: Exact match between deletion and addition on ModifiedPhrase
				while (!m_isResolved && iDel < Deletions.Count)
				{
					iDel = Deletions.Skip(iDel).IndexOf(d => !String.IsNullOrEmpty(d.ModifiedPhrase));
					if (iDel < 0)
						break;
					// Prefer to delete additions that don't have answers
					int iAdditionToRemove = AdditionsAndInsertions.IndexOf(a => a.ModifiedPhrase == Deletions[iDel].ModifiedPhrase && String.IsNullOrEmpty(a.Answer));
					if (iAdditionToRemove < 0)
						iAdditionToRemove = AdditionsAndInsertions.IndexOf(a => a.ModifiedPhrase == Deletions[iDel].ModifiedPhrase);
					if (iAdditionToRemove >= 0)
						RemoveDeletionAndAdditionPair(iDel, iAdditionToRemove);
				}
				iDel = 0;
				var iDelOrig = iDel;
				// Pass 2: Neither deletion nor addition have ModifiedPhrase set
				while (!m_isResolved && iDel < Deletions.Count)
				{
					iDel = Deletions.Skip(iDel).IndexOf(d => String.IsNullOrEmpty(d.ModifiedPhrase));
					if (iDel < 0)
						break;
					// Prefer to delete additions that don't have answers
					int iAdditionToRemove = AdditionsAndInsertions.IndexOf(a => (String.IsNullOrEmpty(a.ModifiedPhrase) || a.ModifiedPhrase == a.OriginalPhrase) && String.IsNullOrEmpty(a.Answer));
					if (iAdditionToRemove < 0)
						iAdditionToRemove = AdditionsAndInsertions.IndexOf(a => String.IsNullOrEmpty(a.ModifiedPhrase) || a.ModifiedPhrase == a.OriginalPhrase);
					if (iAdditionToRemove >= 0)
						RemoveDeletionAndAdditionPair(iDel, iAdditionToRemove);
					else if (iDel == iDelOrig)
						break;
					iDelOrig = iDel;
				}
				// Pass 3: Pair 'em up and blow 'em away
				while (!m_isResolved)
				{
					// Prefer to delete additions that don't have answers
					int iAdditionToRemove = AdditionsAndInsertions.IndexOf(a => String.IsNullOrEmpty(a.Answer));
					if (iAdditionToRemove < 0)
						iAdditionToRemove = 0;
					RemoveDeletionAndAdditionPair(0, iAdditionToRemove);
				}
			}

			private void RemoveDeletionAndAdditionPair(int iDeletion, int iAddition)
			{
				RemoveAddition(iAddition);
				Deletions.RemoveAt(iDeletion);

				FinishResolvingIfNoMorePairsCanBeDeleted();
			}

			private void FinishResolvingIfNoMorePairsCanBeDeleted()
			{
				if (Deletions.Any() && AdditionsAndInsertions.Count > Deletions.Count)
					return;
				if (AdditionsAndInsertions.Count <= 1)
				{
					if (Deletions.Count > 1)
					{
						// This should probably be an exception, but maybe we can recover...
						Deletions.RemoveRange(1, Deletions.Count - 1);
						Debug.Fail($"There were more deletions than additions for {Deletions.Single().Key}.");
					}
				}
				else if (Deletions.Count <= 1)
				{
					// REVIEW: We're assuming that the first (remaining) addition is the "base" one (i.e., any other non-duplicates will
					// be hanging off of it as an insertion or addition). If this is not true, we'll need to look through the list to find
					// the first one whose OriginalPhrase is not the ModifiedPhrase of any other addition/insertion in the list.
					var baseAddition = AdditionsAndInsertions[0].ModifiedPhrase;
					var i = 0;
					while (i + 1 < AdditionsAndInsertions.Count)
					{
						// We assume that earlier ones in the list are older versions whose answers are less likely to be the most desirable
						// one, so we delete ealier ones first so that the last one survives (and its answer will be inserted first in the
						// list. Sadly, this is probably the best we can do.
						var iNewBase = AdditionsAndInsertions.FindIndex(i + 1, a => a.ModifiedPhrase == baseAddition);
						if (iNewBase < 0)
							break;
						RemoveAddition(i);
						i = iNewBase;
					}
				}
				else
					return;

				if (AllAnswers.Any())
				{
					var bestAnswer = AdditionsAndInsertions.First().Answer;
					if (!String.IsNullOrWhiteSpace(bestAnswer) && !AllAnswers.Any(a => a.Contains(bestAnswer)))
					{
						AllAnswers.Insert(0, bestAnswer);
					}
					else if (AllAnswers.Count == 1)
					{
						AdditionsAndInsertions.First().Answer = AllAnswers[0];
						AllAnswers = null;
					}
				}

				m_isResolved = true;
			}

			private void RemoveAddition(int iAdditionToRemove)
			{
				var answer = AdditionsAndInsertions[iAdditionToRemove].Answer;
				AdditionsAndInsertions.RemoveAt(iAdditionToRemove);
				if (!String.IsNullOrWhiteSpace(answer) && !AllAnswers.Any(a => a.Contains(answer)) && !AdditionsAndInsertions.Where(b => b.Answer != null).Any(c => c.Answer.Contains(answer)))
					AllAnswers.Add(answer);
			}

			public bool IsOrphanedInsertionAtOrBeforeReference(IQuestionKey keyToUseForReference, IEnumerable<Question> existingQuestionsInCategory, bool inclusive)
			{
				if (!AdditionsAndInsertions.Any(a => a.OriginalPhrase == a.ModifiedPhrase))
					return false; // Not an orphan - even if it is for an earlier reference, we expect to find it hanging off a real question.
				var addition = AdditionsAndInsertions.FirstOrDefault();
				return addition != null && addition.Key.IsAtOrBeforeReference(keyToUseForReference, inclusive) &&
					!existingQuestionsInCategory.Any(q => q.Matches(addition.Key));
			}

			public Question PopQuestion(IQuestionKey keyToUseForReference)
			{
				ResolveDeletionsAndAdditions();

				PhraseCustomization questionToInsert;
				try
				{
					questionToInsert = AdditionsAndInsertions.First();
				}
				catch (Exception e)
				{
					throw new InvalidOperationException("PopQuestion should only be called for a customization that is not tied to any other question and has " +
						"at least one insertion or addition.", e);
				}
				AdditionsAndInsertions.RemoveAt(0);
				var newQ = new Question(keyToUseForReference.ScriptureReference, keyToUseForReference.StartRef, keyToUseForReference.EndRef,
					questionToInsert.ModifiedPhrase ?? questionToInsert.OriginalPhrase, questionToInsert.Answer, questionToInsert.ImmutableKey);
				SetExcludedAndModified(newQ);
				if (AllAnswers != null && AllAnswers.Any()) // Note: If there are any, there are at least 2
					newQ.Answers = AllAnswers.ToArray();
				return newQ;
			}
		}

		// TODO: Don't hard-code this. Should be read from language-specific file along with leading question words.
        public static readonly List<Word> prepositionsAndArticles = new List<Word>(new Word[] { "in", "of", "the", "a", "an", "for", "by", "through", "on", "about", "to" });

	    private readonly QuestionSections m_sections;
	    private Dictionary<int, SortedDictionary<QuestionKey, Customizations>> m_customizations; // One dictionary per book num
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
				m_customizations = new Dictionary<int, SortedDictionary<QuestionKey, Customizations>>();
				foreach (var customization in customizations)
				{
					var bookKey = customization.ScrStartReference.Book;
					SortedDictionary<QuestionKey, Customizations> customizationsForBook;
					if (!m_customizations.TryGetValue(bookKey, out customizationsForBook))
						m_customizations[bookKey] = customizationsForBook = new SortedDictionary<QuestionKey, Customizations>();

					var customizationsKey = customization.Key;
					Customizations customizationsForKey;
					if (!customizationsForBook.TryGetValue(customizationsKey, out customizationsForKey))
					{
						customizationsForBook[customizationsKey] = customizationsForKey = new Customizations();
					}
					customizationsForKey.Add(customization);
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
        private static IEnumerable<Question> GetCustomizations(Question q,
	        IRefRange sectionRange, Category category, int index,
	        SortedDictionary<QuestionKey, Customizations> customizations,
	        bool processAllAdditionsForRef = false)
        {
	        Customizations customizationsForQuestion;
			if (TryPopCustomizationForQuestion(customizations, q, sectionRange, out customizationsForQuestion))
			{
				customizationsForQuestion.ApplyToQuestion(q);
				if (q.InsertedQuestionBefore != null)
				{
					category.Questions.Insert(index, q.InsertedQuestionBefore);
					foreach (Question customQuestion in GetCustomizations(q.InsertedQuestionBefore, sectionRange,
						category, index, customizations))
					{
						yield return customQuestion;
						index++;
					}
				}
			}
			if (q.InsertedQuestionBefore == null)
			{
				var insertionForPreviousReference = customizations.LastOrDefault(c => c.Value.IsOrphanedInsertionAtOrBeforeReference(q, category.Questions, processAllAdditionsForRef) &&
					customizations.All(other => other.Value == c.Value || !other.Key.Matches(c.Key)));
				var key = insertionForPreviousReference.Key;
				Debug.Assert(key != null || !customizations.Any(c => c.Value.IsOrphanedInsertionAtOrBeforeReference(q, category.Questions, processAllAdditionsForRef)));
				if (key != null)
				{
					var newQ = q.InsertedQuestionBefore = insertionForPreviousReference.Value.PopQuestion(key);
					category.Questions.Insert(index, newQ);
					// We now want to remove this, but only if it doesn't have additional pending insertions or deletions hanging off it.
					if (!insertionForPreviousReference.Value.IsAdditionOrInsertion)
						customizations.Remove(key);
					foreach (Question customQuestion in GetCustomizations(newQ, sectionRange,
						category, index, customizations, true))
					{
						yield return customQuestion;
						index++;
					}
				}
			}

			yield return q;
			
	        if (q.AddedQuestionAfter != null)
	        {
		        category.Questions.Insert(index + 1, q.AddedQuestionAfter);
				foreach (Question tpAdded in GetCustomizations(q.AddedQuestionAfter, sectionRange, 
					category, index + 1, customizations))
		        {
			        yield return tpAdded;
			        index++;
		        }
	        }
		}

        private static bool TryPopCustomizationForQuestion(SortedDictionary<QuestionKey,
	        Customizations> customizations, Question question, IRefRange sectionRange,
	        out Customizations customizationsForQuestion)
        {
	        if (customizations.TryGetValue(question, out customizationsForQuestion))
	        {
				customizations.Remove(question);
		        return true;
	        }

			// In theory questions cannot be added that span multiple sections, but it used to
			// be possible (and may become possible again in the future?). In any case, if we
			// find a question that does, we want to keep it with the section that contains the
			// base question rather than having it get left to be processed totally out of order
			// in GetTrailingCustomizations. Therefore, rather than strictly checking that the
			// custom question is wholly contained in the sectionRange (using &&), we just make
			// sure it at least partially pertains to the sectionRange (using ||).
	        foreach (var kvpCustomization in customizations
		        .TakeWhile(c => c.Key.EndRef <= sectionRange.EndRef || c.Key.StartRef >= sectionRange.StartRef)
		        .Where(c => c.Value.IsAdditionOfDifferentPhrase)) // If user just changed the reference, it gets dealt with elsewhere.
			{
				if (kvpCustomization.Key.Text == question.PhraseInUse)
				{
					customizationsForQuestion = kvpCustomization.Value;
					customizations.Remove(kvpCustomization.Key);
					return true;
				}
			}

			return false;
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
	        SortedDictionary<QuestionKey, Customizations> currBookCustomizations = null;
			Category category = null;
			Question lastQuestionInBook = null;
			Section lastSectionInBook = null;
	        int iQuestion = -1;
			foreach (Section section in m_sections.Items)
            {
	            if (m_customizations != null && BCVRef.GetBookFromBcv(section.StartRef) != currBook)
	            {
		            if (lastSectionInBook != null)
		            {
			            foreach (var question in GetTrailingCustomizations(currBook, lastSectionInBook, currBookCustomizations, lastQuestionInBook, category, iQuestion))
				            yield return question;
		            }

		            currBook = BCVRef.GetBookFromBcv(section.StartRef);
		            m_customizations.TryGetValue(currBook, out currBookCustomizations);
	            }

	            lastSectionInBook = section;
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
						if (q.StartRef == 0 || q.EndRef == 0)
							Debug.WriteLine(q.Text);
                        if (currBookCustomizations != null)
                        {
                            foreach (var question in GetCustomizations(q, section, category, iQuestion, currBookCustomizations))
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
	        foreach (var question in GetTrailingCustomizations(currBook, lastSectionInBook, currBookCustomizations, lastQuestionInBook, category, iQuestion))
		        yield return question;
			m_customizations = null; // Allow this to be garbage collected (and prevent accidental future use)
		}

		private static IEnumerable<Question> GetTrailingCustomizations(int bookNum, IRefRange finalSection, SortedDictionary<QuestionKey, Customizations> customizations, Question lastQuestionInBook,
			Category category, int iQuestion)
		{
			if (customizations != null)
			{
				Debug.Assert(lastQuestionInBook != null && iQuestion > 0 && category != null,
					$"Book {BCVRef.NumberToBookCode(bookNum)} has no built-in questions - cannot process customizations!");

				while (customizations.Any(c => c.Value.IsAdditionOrInsertion))
				{
					var insertionForPreviousReference = customizations.FirstOrDefault(c => c.Value.IsAdditionOrInsertion && customizations.All(other => c.Value == other.Value || !other.Key.Matches(c.Key)));
					var key = insertionForPreviousReference.Key;
					if (key == null)
					{
						Debug.Assert(!customizations.Any(), $"Detected circular chain of customizations for book: {BCVRef.NumberToBookCode(bookNum)}. There were {customizations.Count} customizations that could not be processed!");
						break;
					}

					var newQ = lastQuestionInBook.AddedQuestionAfter = insertionForPreviousReference.Value.PopQuestion(key);
					if (!insertionForPreviousReference.Value.IsAdditionOrInsertion)
						customizations.Remove(key);
					category.Questions.Add(newQ);
					foreach (Question question in GetCustomizations(newQ, finalSection, category, iQuestion, customizations))
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
                    MessageBox.Show("Unused KeyTerm Rules: \n" + unUsedRules, TxlPlugin.pluginName);
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
