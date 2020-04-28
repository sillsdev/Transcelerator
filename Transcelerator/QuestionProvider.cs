// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
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
using SIL.Scripture;

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
        private readonly QuestionSections m_sections;
        private readonly PhrasePartManager m_manager;
        private SortedDictionary<int, SectionInfo> m_sectionInfo;
		private int[] m_availableBookIds;

	    #endregion

        #region Constructors
        /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Initializes a new instance of the <see cref="QuestionProvider"/> class. This version
	    /// is only for testing.
	    /// </summary>
	    /// <param name="sections">Class representing the questions, organized by Scripture
	    /// section and category.</param>
	    /// <param name="keyTerms">(optional) key terms</param>
	    /// ------------------------------------------------------------------------------------
	    internal QuestionProvider(QuestionSections sections, IEnumerable<KeyTermMatchSurrogate> keyTerms)
		{
			m_sections = sections;
            m_manager = new PhrasePartManager(new string[0], keyTerms);
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="QuestionProvider"/> class. This version
		/// is only for testing.
		/// </summary>
		/// <param name="parsedQuestions">The parsed questions object.</param>
		/// ------------------------------------------------------------------------------------
		internal QuestionProvider(ParsedQuestions parsedQuestions)
		{
			m_sections = parsedQuestions.Sections;
			m_manager = new PhrasePartManager(parsedQuestions.TranslatableParts, parsedQuestions.KeyTerms);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="QuestionProvider"/> class.
		/// </summary>
		/// <param name="parsedQuestions">The parsed questions object.</param>
		/// <param name="manager">The PhrasePartManager that keeps track of all known key terms
		/// and all translatable parts in the project.</param>
		/// ------------------------------------------------------------------------------------
		public QuestionProvider(ParsedQuestions parsedQuestions, PhrasePartManager manager)
		{
			m_sections = parsedQuestions.Sections;
			m_manager = manager;
		}
		#endregion

		#region Public Properties
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets a sorted (canonically) dictionary of all sections keyed by (integer
		/// representation of) the start reference. (Note that these are not the section
		/// heads in the vernacular Scripture but rather from the master question file.)
		/// </summary>
		/// --------------------------------------------------------------------------------
		public SortedDictionary<int, SectionInfo> SectionInfo
        {
            get
            {
                if (m_sectionInfo == null)
                {
	                m_sectionInfo = new SortedDictionary<int, SectionInfo>(
                        m_sections.Items.ToDictionary(s => s.StartRef, s => new SectionInfo(s)));
                }
                return m_sectionInfo;
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

		public PhrasePartManager PhrasePartManager
		{
			get { return m_manager; }
		}
	    #endregion

		#region Private helper methods
	    /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the questions/phrases
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private IEnumerable<TranslatablePhrase> GetPhrases()
		{
			HashSet<string> processedCategories = new HashSet<string>();
	        TranslatablePhrase phrase;
		    int categoryType;
			foreach (Section section in m_sections.Items)
			{
				for (int iCat = 0; iCat < section.Categories.Length; iCat++)
				{
					Category category = section.Categories[iCat];
					categoryType = iCat;
					if (iCat == 0 && !category.IsOverview) // 0 is reserved for "overview", so we can't use that for non-overview categories.
						categoryType++;

				    if (category.Type != null)
				    {
				        string lcCategory = category.Type.ToLowerInvariant();
				        if (!processedCategories.Contains(lcCategory))
				        {
				            phrase = new TranslatablePhrase(new SimpleQuestionKey(category.Type), -1, processedCategories.Count);
                            phrase.m_parts.Add(m_manager.GetOrCreatePart(PhraseParser.GetWordsInString(lcCategory), phrase, false));
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
						phrase = new TranslatablePhrase(q, categoryType, iQuestion + 1, q.IsSummaryQuestion);
					    if (!phrase.IsExcluded)
					        InitializePhraseParts(phrase);
					    yield return phrase;
					}
				}
			}
		}

		public void InitializePhraseParts(TranslatablePhrase phrase)
		{
			foreach (ParsedPart part in phrase.QuestionInfo.ParsedParts)
			{
				IPhrasePart newPart;
				if (part.Type == PartType.TranslatablePart)
					newPart = m_manager.GetOrCreatePart(part.Words, phrase, false);
				else if (part.Type == PartType.KeyTerm)
					newPart = m_manager.FindKeyTerm(part.Words);
				else
					newPart = new Number(part.NumericValue);
				phrase.m_parts.Add(newPart);
			}
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
