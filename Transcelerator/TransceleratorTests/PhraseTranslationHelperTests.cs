// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: PhraseTranslationHelperTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using AddInSideViews;
using NUnit.Framework;
using Rhino.Mocks;

namespace SIL.Transcelerator
{
    #region class PhraseTranslationTestBase
    public abstract class PhraseTranslationTestBase
    {
        protected QuestionSections m_sections;
        private List<IKeyTerm> m_dummyKtList;
        protected Dictionary<string, List<string>> m_dummyKtRenderings;
        private Dictionary<string, KeyTermMatchSurrogate> m_keyTermsDictionary;
        private Dictionary<string, ParsedPart> m_translatablePartsDictionary;

        [SetUp]
        public virtual void Setup()
        {
            m_sections = new QuestionSections();
            m_sections.Items = new Section[1];
            m_sections.Items[0] = new Section();
            m_sections.Items[0].Categories = new Category[1];
            m_sections.Items[0].Categories[0] = new Category();

            m_dummyKtList = new List<IKeyTerm>();
            m_dummyKtRenderings = new Dictionary<string, List<string>>();
            KeyTerm.GetTermRenderings = s =>
                {
                    List<string> renderings;
                    m_dummyKtRenderings.TryGetValue(s, out renderings);
                    return renderings;
                };

            KeyTerm.FileAccessor = new TestKeyTermRenderingDataFileAccessor();
            m_keyTermsDictionary = new Dictionary<string, KeyTermMatchSurrogate>();
            m_translatablePartsDictionary = new Dictionary<string, ParsedPart>();
        }

        #region Helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a list of ParsedParts based on a string array representing key terms and parts
        /// to dictionaries used by GetParsedQuestions. Note that items in the parts array will
        /// be treated as translatable parts unless prefixed with "kt:", in which case they
        /// will be treated as key terms (corresponding key terms must be added by calling
        /// AddMockedKeyTerm.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected List<ParsedPart> GetParsedParts(string[] parts)
        {
            List<ParsedPart> parsedParts = new List<ParsedPart>();
            foreach (string part in parts)
            {
                ParsedPart parsedPart;
                if (part.StartsWith("kt:"))
                {
                    string sWords = part.Substring(3);
                    KeyTermMatchSurrogate kt;
                    if (!m_keyTermsDictionary.TryGetValue(sWords, out kt))
                        m_keyTermsDictionary[sWords] = kt = new KeyTermMatchSurrogate(sWords, new string(sWords.Reverse().ToArray()));
                    parsedPart = new ParsedPart(kt);
                }
                else
                {
                    if (!m_translatablePartsDictionary.TryGetValue(part, out parsedPart))
                    {
                        m_translatablePartsDictionary[part] = parsedPart = new ParsedPart(part.Split(new[] {' '},
                             StringSplitOptions.RemoveEmptyEntries).Select(w => (Word) w));
                    }
                }
                parsedParts.Add(parsedPart);
            }
            return parsedParts;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a ParsedQuestions object.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected ParsedQuestions GetParsedQuestions()
        {
            ParsedQuestions pq = new ParsedQuestions();
            pq.Sections = m_sections;
            pq.KeyTerms = m_keyTermsDictionary.Values.ToArray();
            pq.TranslatableParts = m_translatablePartsDictionary.Keys.ToArray();
            return pq;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the mocked key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected IKeyTerm AddMockedKeyTerm(string englishGlossOfTerm)
        {
            return AddMockedKeyTerm(englishGlossOfTerm, englishGlossOfTerm.ToUpper());
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the mocked key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected IKeyTerm AddMockedKeyTerm(string englishGlossOfTerm, string bestRendering)
        {
            return AddMockedKeyTerm(englishGlossOfTerm, new string(englishGlossOfTerm.ToLowerInvariant().Reverse().ToArray()),
                bestRendering, (bestRendering != null) ? new[] { englishGlossOfTerm } : new string[0]);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the mocked key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected IKeyTerm AddMockedKeyTerm(string englishGlossOfTerm, string underlyingTermId,
            string bestRendering, params string[] otherRenderings)
        {
            if (bestRendering != null)
            {
                m_dummyKtRenderings[underlyingTermId] = (otherRenderings == null) ? new List<string>() : new List<string>(otherRenderings);
                m_dummyKtRenderings[underlyingTermId].Insert(0, bestRendering);
            }

            IKeyTerm mockedKt = MockRepository.GenerateStub<IKeyTerm>();
            mockedKt.Stub(kt => kt.Term).Return(englishGlossOfTerm);
            mockedKt.Stub(kt => kt.Id).Return(underlyingTermId);

            m_dummyKtList.Add(mockedKt);
            return mockedKt;
        }
        #endregion
    }
    #endregion

    [TestFixture]
	public class PhraseTranslationHelperTests : PhraseTranslationTestBase
    {
        #region List sorting tests
        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting phrases, where the phrases that have parts that are used by lots of
		/// other phrases sort first in the list. Specifically, based on the number of owning
		/// phrases of the part that has the fewest. If they have the same min, then the phrase
		/// with the fewest parts should sort first. If they are still equal, the one with a
		/// part that has a maximum number of owning phrases should sort first. Otherwise, sort
		/// by reference. (Hard to explain)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrasesSortedByDefault()
		{
          	AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say");

		    var cat = m_sections.Items[0].Categories[0];

            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
		    AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
		        "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

		    var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

			Assert.AreEqual("C", pth[0].Reference);
			Assert.AreEqual("F", pth[1].Reference);
			Assert.AreEqual("A", pth[2].Reference);
			Assert.AreEqual("B", pth[3].Reference);
			Assert.AreEqual("E", pth[4].Reference);
			Assert.AreEqual("D", pth[5].Reference);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases sorted alphabetically by the English original.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesSortedByOriginalPhrase()
        {
            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1);
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2);
            AddTestQuestion(cat, "that dog", "C", 3, 3);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            pth.Sort(PhraseTranslationHelper.SortBy.EnglishPhrase, true);

            Assert.AreEqual("D", pth[0].Reference);
            Assert.AreEqual("C", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
            Assert.AreEqual("B", pth[3].Reference);
            Assert.AreEqual("F", pth[4].Reference);
            Assert.AreEqual("A", pth[5].Reference);

            pth.Sort(PhraseTranslationHelper.SortBy.EnglishPhrase, false);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("F", pth[1].Reference);
            Assert.AreEqual("B", pth[2].Reference);
            Assert.AreEqual("E", pth[3].Reference);
            Assert.AreEqual("C", pth[4].Reference);
            Assert.AreEqual("D", pth[5].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases sorted by reference (all unique, all in the same category)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesSortedByReference()
        {
            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1);
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2);
            AddTestQuestion(cat, "that dog", "C", 3, 3);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("C", pth[2].Reference);
            Assert.AreEqual("D", pth[3].Reference);
            Assert.AreEqual("E", pth[4].Reference);
            Assert.AreEqual("F", pth[5].Reference);

            pth.Sort(PhraseTranslationHelper.SortBy.Reference, false);

            Assert.AreEqual("F", pth[0].Reference);
            Assert.AreEqual("E", pth[1].Reference);
            Assert.AreEqual("D", pth[2].Reference);
            Assert.AreEqual("C", pth[3].Reference);
            Assert.AreEqual("B", pth[4].Reference);
            Assert.AreEqual("A", pth[5].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases sorted by category, reference and sequence number
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesSortedByReferenceCategoryAndSequenceNum()
        {
            var section = m_sections.Items[0];
            section.Categories = new Category[2];

            var cat = m_sections.Items[0].Categories[0] = new Category();
            AddTestQuestion(cat, "What is the meaning of life?", "A-D", 1, 4);
            AddTestQuestion(cat, "Why is there evil?", "E-G", 5, 6);
            AddTestQuestion(cat, "Why am I here?", "A-D", 1, 4);

            cat = section.Categories[1] = new Category();
            AddTestQuestion(cat, "What would God do?", "A", 1, 1);
            AddTestQuestion(cat, "When is the best time for ice cream?", "C", 3, 3);
            AddTestQuestion(cat, "Is it okay for Paul to talk to God today?", "D", 4, 4);
            AddTestQuestion(cat, "What is that dog?", "E", 5, 5);
            AddTestQuestion(cat, "What is that dog?", "E-F", 5, 6);
            AddTestQuestion(cat, "What is that dog?", "E-G", 5, 7);
            AddTestQuestion(cat, "What is Paul asking that man?", "A", 1, 1);
            AddTestQuestion(cat, "Is a dog man's best friend?", "D", 4, 4);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);

            Assert.AreEqual("What is the meaning of life?", pth[0].PhraseInUse);
            Assert.AreEqual("A-D", pth[0].Reference);
            Assert.AreEqual(1, pth[0].SequenceNumber);
            Assert.AreEqual("Why am I here?", pth[1].PhraseInUse);
            Assert.AreEqual("A-D", pth[1].Reference);
            Assert.AreEqual(3, pth[1].SequenceNumber);
            Assert.AreEqual("What would God do?", pth[2].PhraseInUse);
            Assert.AreEqual("A", pth[2].Reference);
            Assert.AreEqual(1, pth[2].SequenceNumber);
            Assert.AreEqual("What is Paul asking that man?", pth[3].PhraseInUse);
            Assert.AreEqual("A", pth[3].Reference);
            Assert.AreEqual(7, pth[3].SequenceNumber);
            Assert.AreEqual("C", pth[4].Reference);
            Assert.AreEqual("Is it okay for Paul to talk to God today?", pth[5].PhraseInUse);
            Assert.AreEqual("D", pth[5].Reference);
            Assert.AreEqual(3, pth[5].SequenceNumber);
            Assert.AreEqual("Is a dog man's best friend?", pth[6].PhraseInUse);
            Assert.AreEqual("D", pth[6].Reference);
            Assert.AreEqual(8, pth[6].SequenceNumber);
            Assert.AreEqual("E-G", pth[7].Reference);
            Assert.AreEqual(0, pth[7].Category);
            Assert.AreEqual(2, pth[7].SequenceNumber);
            Assert.AreEqual("E", pth[8].Reference);
            Assert.AreEqual("E-F", pth[9].Reference);
            Assert.AreEqual("E-G", pth[10].Reference);
            Assert.AreEqual(1, pth[10].Category);

            pth.Sort(PhraseTranslationHelper.SortBy.Reference, false);

            Assert.AreEqual("E-G", pth[0].Reference);
            Assert.AreEqual(1, pth[0].Category);
            Assert.AreEqual("E-F", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
            Assert.AreEqual("E-G", pth[3].Reference);
            Assert.AreEqual(0, pth[3].Category);
            Assert.AreEqual("D", pth[4].Reference);
            Assert.AreEqual(8, pth[4].SequenceNumber);
            Assert.AreEqual("D", pth[5].Reference);
            Assert.AreEqual(3, pth[5].SequenceNumber);
            Assert.AreEqual("C", pth[6].Reference);
            Assert.AreEqual("A", pth[7].Reference);
            Assert.AreEqual(7, pth[7].SequenceNumber);
            Assert.AreEqual("A", pth[8].Reference);
            Assert.AreEqual(1, pth[8].SequenceNumber);
            Assert.AreEqual("A-D", pth[9].Reference);
            Assert.AreEqual(3, pth[9].SequenceNumber);
            Assert.AreEqual("A-D", pth[10].Reference);
            Assert.AreEqual(1, pth[10].SequenceNumber);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases sorted alphabetically by the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesSortedByTranslation()
        {
            var cat = m_sections.Items[0].Categories[0];
            var q1 = AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1);
            var q2 = AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2);
            var q3 = AddTestQuestion(cat, "that dog", "C", 3, 3);
            var q4 = AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4);
            var q5 = AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5);
            var q6 = AddTestQuestion(cat, "What is that dog?", "F", 6, 6);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            pth[pth.FindPhrase(q1)].Translation = "Z";
            pth[pth.FindPhrase(q2)].Translation = "B";
            pth[pth.FindPhrase(q3)].Translation = "alligator";
            pth[pth.FindPhrase(q4)].Translation = "D";
            pth[pth.FindPhrase(q5)].Translation = "e";
            pth[pth.FindPhrase(q6)].Translation = "E";

            pth.Sort(PhraseTranslationHelper.SortBy.Translation, true);

            Assert.AreEqual(q3, pth[0].QuestionInfo);
            Assert.AreEqual(q2, pth[1].QuestionInfo);
            Assert.AreEqual(q4, pth[2].QuestionInfo);
            Assert.AreEqual(q5, pth[3].QuestionInfo);
            Assert.AreEqual(q6, pth[4].QuestionInfo);
            Assert.AreEqual(q1, pth[5].QuestionInfo);

            pth.Sort(PhraseTranslationHelper.SortBy.Translation, false);

            Assert.AreEqual(q1, pth[0].QuestionInfo);
            Assert.AreEqual(q6, pth[1].QuestionInfo);
            Assert.AreEqual(q5, pth[2].QuestionInfo);
            Assert.AreEqual(q4, pth[3].QuestionInfo);
            Assert.AreEqual(q2, pth[4].QuestionInfo);
            Assert.AreEqual(q3, pth[5].QuestionInfo);
        }
	    #endregion

        #region List filtering tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering phrases textually with the whole-word-match option
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_WholeWordMatch()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("what is", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("F", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);

            pth.Filter("what is Pau", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(0, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("what is Paul", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in helper");
            Assert.AreEqual("B", pth[0].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering phrases textually with the whole-word-match option
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_WholeWordMatch_IncludeExcluded()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, false, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, true, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, false, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, true, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper (unfiltered)");

            pth.Filter("what is", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in helper (filtered on 'what is', excluding deleted questions)");

            Assert.AreEqual("that dog wishes this Paul and what is say radish", pth[0].PhraseInUse);

            pth.Filter("what is", true, PhraseTranslationHelper.KeyTermFilterType.All, null, true);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper (filtered on 'what is', including deleted questions)");

            Assert.AreEqual("that dog wishes this Paul and what is say radish", pth[0].PhraseInUse);
            Assert.AreEqual("What is Paul asking me to say with respect to that dog?", pth[1].PhraseInUse);
            Assert.AreEqual("What is that dog?", pth[2].PhraseInUse);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering phrases textually without the whole-word-match option
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_PartialWordMatch()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
                "this would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 2 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 2 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("is", false, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(5, pth.Phrases.Count(), "Wrong number of phrases in helper");
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("D", pth[2].Reference);
            Assert.AreEqual("E", pth[3].Reference);
            Assert.AreEqual("F", pth[4].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering phrases textually using a string that would result in a bogus
        /// regular expression if not properly escaped.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_PreventBogusRegExCrash()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "This would God have me [to] say with respect to Paul?", "A", 1, 1,
                "this would", "kt:god", "kt:have", "me to", "kt:say", "with respect to", "kt:paul");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is that dog");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("[", false, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in helper");
            Assert.AreEqual("A", pth[0].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering by part using a string that doesn't even come close to matching any part
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_NoMatches()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("finkelsteins", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(0, pth.Phrases.Count(), "Wrong number of phrases in helper");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering by ref
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredOnlyByRef()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All,
                ((start, end, sref) => start >= 2 && end <= 5 && sref != "C"), false);
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper");
            Assert.AreEqual("B", pth[0].Reference);
            Assert.AreEqual("D", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);

            // Now remove the ref filter
            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering by ref with excluded phrases
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredOnlyByRef_IncludeExcluded()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, false, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, false, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, true, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, true, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, false, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All,
                ((start, end, sref) => start >= 2 && end <= 5 && sref != "C"), false);
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);
            Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in filtered list");
            Assert.AreEqual("What is Paul asking me to say with respect to that dog?", pth[0].PhraseInUse);

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All,
                ((start, end, sref) => start >= 2 && end <= 5 && sref != "C"), true);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in filtered list");
            Assert.AreEqual("What is Paul asking me to say with respect to that dog?", pth[0].PhraseInUse);
            Assert.AreEqual("Is it okay for Paul me to talk with respect to God today?", pth[1].PhraseInUse);
            Assert.AreEqual("that dog wishes this Paul and what is say radish", pth[2].PhraseInUse);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering for phrases where all key terms have renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredByKeyTerms_WithRenderings()
        {
            AddMockedKeyTerm("God", null);
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have", null);
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, false);
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);
            Assert.AreEqual(4, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("B", pth[0].Reference);
            Assert.AreEqual("C", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
            Assert.AreEqual("F", pth[3].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering for phrases where all key terms have renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredByKeyTerms_WithRenderings_IncludeExcluded()
        {
            AddMockedKeyTerm("God", null);
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have", null);
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, true, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, true, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, true, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, false, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, false);
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);
            Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("C", pth[0].Reference);
            Assert.AreEqual("F", pth[1].Reference);

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, true);
            Assert.AreEqual(5, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("A", pth[0].Reference); // Even though there is an unrendered term, this phrase gets included because we don't actually parse excluded phrases to look for key term matches.
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("C", pth[2].Reference);
            Assert.AreEqual("E", pth[3].Reference);
            Assert.AreEqual("F", pth[4].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering for phrases where any key terms don't have renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredByKeyTerms_WithoutRenderings()
        {
            AddMockedKeyTerm("God", null);
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have", null);
            AddMockedKeyTerm("say");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "kt:paul", "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings, null, false);
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);
            Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("D", pth[1].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases filtered by part, retrieving only phrases whose key terms have
        /// renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_WholeWordMatch_KeyTermsWithRenderings()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say", null);

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "paul" /* 1 */, "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is have radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:have", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("what is", true, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, false);
            Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("F", pth[0].Reference);
            Assert.AreEqual("E", pth[1].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests getting phrases filtered by part and reference, retrieving only phrases whose
        /// key terms have renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextuallyAndByRef_WholeWordMatch_KeyTermsWithRenderings()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say", null);

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1,
                "what would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "paul" /* 1 */, "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is have radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:have", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("what is", true, PhraseTranslationHelper.KeyTermFilterType.WithRenderings,
                ((start, end, sref) => start < 6), false);
            Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in helper");

            Assert.AreEqual("E", pth[0].Reference);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests filtering by part doing a partial match, retrieving only phrases whose key
        /// terms do NOT have renderings
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetPhrasesFilteredTextually_PartialMatchWithoutRenderings()
        {
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have");
            AddMockedKeyTerm("say", null);

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
                "this would" /* 1 */, "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking" /* 1 */, "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for" /* 1 */, "paul" /* 1 */, "me to" /* 3 */, "talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */);
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */);
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("is", false, PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings, null, false);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper");
            pth.Sort(PhraseTranslationHelper.SortBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
        }
        #endregion

        #region Translation tests
        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation to null for a phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetNewTranslation_Null()
        //{
        //    TranslatablePhrase phrase = new TranslatablePhrase(new TestQ("Who was the man?", "A", 1, 1), 1, 0);
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase}, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    phrase.Translation = null;

        //    Assert.AreEqual(0, phrase.Translation.Length);
        //    Assert.IsFalse(phrase.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase when that whole phrase matches part of
        ///// another phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetNewTranslation_AutoAcceptTranslationForAllIdenticalPhrases()
        //{
        //    TranslatablePhrase phrase1 = new TranslatablePhrase(new TestQ("Who was the man?", "A", 1, 1), 1, 0);
        //    TranslatablePhrase phrase2 = new TranslatablePhrase(new TestQ("Where was the woman?", "A", 1, 1), 1, 0);
        //    TranslatablePhrase phrase3 = new TranslatablePhrase(new TestQ("Who was the man?", "B", 2, 2), 1, 0);
        //    TranslatablePhrase phrase4 = new TranslatablePhrase(new TestQ("Where was the woman?", "C", 3, 3), 1, 0);
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2, phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";
        //    phrase2.Translation = "\u00BFDo\u0301nde estaba la mujer?";

        //    Assert.AreEqual(phrase1.Translation, phrase3.Translation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.AreEqual(phrase2.Translation, phrase4.Translation);
        //    Assert.IsTrue(phrase4.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase when that whole phrase matches part of
        ///// another phrase. TXL-108
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetNewTranslation_AcceptTranslationConsistingOnlyOfInitialAndFinalPunctuation()
        //{
        //    AddMockedKeyTerm("Paul", null);
        //    AddMockedKeyTerm("Judas", null);
        //    TranslatablePhrase phrase1 = new TranslatablePhrase(new TestQ("Who was Paul talking to?", "A", 1, 1), 1, 0);
        //    TranslatablePhrase phrase2 = new TranslatablePhrase(new TestQ("Who was Judas kissing?", "A", 1, 1), 1, 0);
        //    TranslatablePhrase phrase3 = new TranslatablePhrase(new TestQ("Why was Judas talking to Paul?", "B", 2, 2), 1, 0);
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2, phrase3 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    phrase1.Translation = "\u00BFCon quie\u0301n estaba hablando Pablo?";
        //    phrase2.HasUserTranslation = true;

        //    Assert.AreEqual("\u00BF?", phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.AreEqual(phrase2.Translation, phrase3.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase when that whole phrase matches part of
        ///// another phrase, even if it has an untranslated key term.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetNewTranslation_AutoAcceptTranslationForAllIdenticalPhrases_WithUntranslatedKeyTerm()
        //{
        //    AddMockedKeyTerm("man", null);

        //    TranslatablePhrase phrase1 = new TranslatablePhrase(new TestQ("Who was the man?", "A", 1, 1), 1, 0);
        //    TranslatablePhrase phrase2 = new TranslatablePhrase(new TestQ("Who was the man?", "B", 2, 2), 1, 0);
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";

        //    Assert.AreEqual(phrase1.Translation, phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase when that whole phrase matches part of
        ///// another phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetNewTranslation_WholePhraseMatchesPartOfAnotherPhrase()
        //{
        //    TranslatablePhrase shortPhrase = new TranslatablePhrase("Who was the man?");
        //    TranslatablePhrase longPhrase = new TranslatablePhrase("Who was the man with the jar?");
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        shortPhrase, longPhrase }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, shortPhrase.TranslatableParts.Count());
        //    Assert.AreEqual(2, longPhrase.TranslatableParts.Count());

        //    string partTrans = "Quie\u0301n era el hombre";
        //    shortPhrase.Translation = partTrans + "?";

        //    Assert.AreEqual((partTrans + "?").Normalize(NormalizationForm.FormC), shortPhrase.Translation);
        //    Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), shortPhrase[0].Translation);
        //    Assert.AreEqual((partTrans + "?").Normalize(NormalizationForm.FormC), longPhrase.Translation);
        //    Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), longPhrase[0].Translation);
        //    Assert.AreEqual(0, longPhrase[1].Translation.Length);
        //    Assert.IsTrue(shortPhrase.HasUserTranslation);
        //    Assert.IsFalse(longPhrase.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase when that whole phrase matches part of
        ///// another phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_WholePhraseMatchesPartOfAnotherPhrase()
        //{
        //    TranslatablePhrase shortPhrase = new TranslatablePhrase("Who was the man?");
        //    TranslatablePhrase longPhrase = new TranslatablePhrase("Who was the man with the jar?");
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        shortPhrase, longPhrase }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, shortPhrase.TranslatableParts.Count());
        //    Assert.AreEqual(2, longPhrase.TranslatableParts.Count());

        //    shortPhrase.Translation = "Quiem fue el hambre?";
        //    string partTrans = "Quie\u0301n era el hombre";
        //    string trans = "\u00BF" + partTrans + "?";
        //    shortPhrase.Translation = trans;

        //    Assert.AreEqual(trans.Normalize(NormalizationForm.FormC), shortPhrase.Translation);
        //    Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), shortPhrase[0].Translation);
        //    Assert.AreEqual(trans.Normalize(NormalizationForm.FormC), longPhrase.Translation);
        //    Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), longPhrase[0].Translation);
        //    Assert.AreEqual(0, longPhrase[1].Translation.Length);
        //    Assert.IsTrue(shortPhrase.HasUserTranslation);
        //    Assert.IsFalse(longPhrase.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for two phrases that have a common part and verify
        ///// that a third phrase that has that part shows the translation of the translated part.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_GuessAtPhraseTranslationBasedOnTriangulation()
        //{
        //    AddMockedKeyTerm("Jesus");
        //    AddMockedKeyTerm("lion");
        //    AddMockedKeyTerm("jar");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Who was the man in the lion's den?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Who was the man with the jar?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Who was the man Jesus healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2, phrase3 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());

        //    string transPart = "Quie\u0301n era el hombre";
        //    string transCommon = "\u00BF" + transPart;
        //    phrase1.Translation = transCommon + " en la fosa de leones?";
        //    phrase2.Translation = transCommon + " con el jarro?";

        //    Assert.AreEqual((transCommon + " en la fosa de leones?").Normalize(NormalizationForm.FormC), phrase1.Translation);
        //    Assert.AreEqual((transCommon + " con el jarro?").Normalize(NormalizationForm.FormC), phrase2.Translation);
        //    Assert.AreEqual((transCommon + " JESUS?").Normalize(NormalizationForm.FormC), phrase3.Translation);
        //    Assert.AreEqual(transPart.Normalize(NormalizationForm.FormC), phrase3[0].Translation);
        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsFalse(phrase3.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for two phrases that have a common part and verify
        ///// that a third phrase that has that part shows the translation of the translated part.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_FindKeyTermRenderingWhenKtHasMultiplesTranslations()
        //{
        //    AddMockedKeyTerm("arrow", "flecha");
        //    AddMockedKeyTerm("arrow", "dardo");
        //    AddMockedKeyTerm("arrow", "dardos");
        //    AddMockedKeyTerm("lion", "leo\u0301n");
        //    AddMockedKeyTerm("boat", "nave");
        //    AddMockedKeyTerm("boat", "barco");
        //    AddMockedKeyTerm("boat", "barca");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("I shot the lion with the arrow.");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Who put the lion in the boat?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Does the boat belong to the boat?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("I shot the boat with the arrow.");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("Who put the arrow in the boat?");
        //    TranslatablePhrase phrase6 = new TranslatablePhrase("Who put the arrow in the lion?");
        //    TranslatablePhrase phrase7 = new TranslatablePhrase("I shot the arrow with the lion.");
        //    TranslatablePhrase phrase8 = new TranslatablePhrase("Does the arrow belong to the lion?");

        //    PhraseTranslationHelper helper = new PhraseTranslationHelper(
        //        new[] { phrase1, phrase2, phrase3, phrase4, phrase5, phrase6, phrase7, phrase8 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(helper, "m_justGettingStarted", false);

        //    foreach (TranslatablePhrase phrase in helper.Phrases)
        //    {
        //        Assert.AreEqual(4, phrase.GetParts().Count(), "Wrong number of parts for phrase: " + phrase.OriginalPhrase);
        //        Assert.AreEqual(2, phrase.TranslatableParts.Count(), "Wrong number of translatable parts for phrase: " + phrase.OriginalPhrase);
        //    }

        //    phrase1.Translation = "Yo le pegue\u0301 un tiro al noil con un dardo.";
        //    Assert.AreEqual("Yo le pegue\u0301 un tiro al nave con un flecha.".Normalize(NormalizationForm.FormC),
        //        phrase4.Translation);
        //    Assert.AreEqual("Yo le pegue\u0301 un tiro al flecha con un leo\u0301n.".Normalize(NormalizationForm.FormC),
        //        phrase7.Translation);

        //    phrase2.Translation = "\u00BFQuie\u0301n puso el leo\u0301n en la barca?";
        //    Assert.AreEqual("\u00BFQuie\u0301n puso el flecha en la nave?".Normalize(NormalizationForm.FormC),
        //        phrase5.Translation);
        //    Assert.AreEqual("\u00BFQuie\u0301n puso el flecha en la leo\u0301n?".Normalize(NormalizationForm.FormC),
        //        phrase6.Translation);

        //    phrase3.Translation = "\u00BFEl taob le pertenece al barco?";
        //    // This is a bizarre special case where the original question has the same key term twice and
        //    // the user has translated it differently. Internal details of the logic (specifically, it finds
        //    // the longer rendering first) dictate the order in which the key terms are considered to have
        //    // been found. For the purposes of this test case, we don't care in which order the terms of the
        //    // untranslated question get substituted.
        //    Assert.IsTrue("\u00BFEl leo\u0301n le pertenece al flecha?".Normalize(NormalizationForm.FormC) == phrase8.Translation ||
        //        "\u00BFEl flecha le pertenece al leo\u0301n?".Normalize(NormalizationForm.FormC) == phrase8.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsFalse(phrase4.HasUserTranslation);
        //    Assert.IsFalse(phrase5.HasUserTranslation);
        //    Assert.IsFalse(phrase6.HasUserTranslation);
        //    Assert.IsFalse(phrase7.HasUserTranslation);
        //    Assert.IsFalse(phrase8.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for one phrase and then repeatedly accepting and
        ///// un-accepting the generated translation for the other phrase that differs only by
        ///// key terms. (TXL-51)
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_PreventAddedSpacesWhenAcceptingGeneratedTranslation()
        //{
        //    AddMockedKeyTerm("Jesus", "Jesu\u0301s");
        //    AddMockedKeyTerm("Phillip", "Felipe");
        //    AddMockedKeyTerm("Matthew", "Mateo");

        //    TranslatablePhrase phrase0 = new TranslatablePhrase("What asked Jesus Matthew?");
        //    TranslatablePhrase phrase1 = new TranslatablePhrase("What asked Jesus Phillip?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("What asked Phillip Matthew?");

        //    PhraseTranslationHelper helper = new PhraseTranslationHelper(
        //        new[] { phrase0, phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(helper, "m_justGettingStarted", false);


        //    phrase0.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s Mateo?";
        //    phrase1.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s a Felipe?";
        //    string expectedTranslation = "\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC);
        //    Assert.AreEqual(expectedTranslation,phrase2.Translation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);

        //    phrase2.HasUserTranslation = true;
        //    Assert.AreEqual(expectedTranslation, phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);

        //    expectedTranslation = "\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC);
        //    phrase2.HasUserTranslation = false;
        //    Assert.AreEqual(expectedTranslation, phrase2.Translation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);

        //    phrase2.HasUserTranslation = true;
        //    Assert.AreEqual(expectedTranslation, phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that unconfirming a user translation will go back to a template-based
        ///// translation if one is available. Specifically, it will revert to the template of the
        ///// first translated question it finds.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_ClearingUserTranslationFlagRestoresTemplateBasedTranslation()
        //{
        //    AddMockedKeyTerm("Jesus", "Jesu\u0301s");
        //    AddMockedKeyTerm("Phillip", "Felipe");
        //    AddMockedKeyTerm("Matthew", "Mateo");

        //    TranslatablePhrase phrase0 = new TranslatablePhrase("What asked Jesus Matthew?");
        //    TranslatablePhrase phrase1 = new TranslatablePhrase("What asked Jesus Phillip?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("What asked Phillip Matthew?");

        //    PhraseTranslationHelper helper = new PhraseTranslationHelper(
        //        new[] { phrase0, phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(helper, "m_justGettingStarted", false);


        //    phrase0.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s a Mateo?";
        //    phrase1.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s Felipe?";
        //    Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC),
        //        phrase2.Translation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);

        //    phrase2.HasUserTranslation = true;
        //    Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC),
        //        phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);

        //    phrase2.HasUserTranslation = false;
        //    Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC),
        //        phrase2.Translation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);

        //    phrase2.HasUserTranslation = true;
        //    Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC),
        //        phrase2.Translation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for two phrases that have a common part and verify
        ///// that a third phrase that has that part shows the translation of the translated part.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_GuessAtPhraseTranslationBasedOnTriangulation()
        //{
        //    AddMockedKeyTerm("Jesus");
        //    AddMockedKeyTerm("lion");
        //    AddMockedKeyTerm("jar");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Who was the man in the lions' den?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Who was the man with the jar?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Who was the man Jesus healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2, phrase3 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());

        //    string partTrans = "Quie\u0301n era el hombre";
        //    string transCommon = "\u00BF" + partTrans;
        //    phrase1.Translation = "Quien fue lo hambre en la fosa de leones?";
        //    phrase2.Translation = transCommon + " con el jarro?";
        //    Assert.AreEqual("\u00BFmbre JESUS?", phrase3.Translation);
        //    Assert.AreEqual("mbre", phrase3[0].Translation);

        //    phrase1.Translation = transCommon + " en la fosa de leones?";

        //    Assert.AreEqual((transCommon + " en la fosa de leones?").Normalize(NormalizationForm.FormC), phrase1.Translation);
        //    Assert.AreEqual((transCommon + " con el jarro?").Normalize(NormalizationForm.FormC), phrase2.Translation);
        //    Assert.AreEqual((transCommon + " JESUS?").Normalize(NormalizationForm.FormC), phrase3.Translation);
        //    Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), phrase3[0].Translation);
        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsFalse(phrase3.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase with only one translatable part when
        ///// another phrase differs only by a key term.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_GuessAtOnePartPhraseThatDiffersBySingleKeyTerm()
        //{
        //    AddMockedKeyTerm("Timothy", "Timoteo");
        //    AddMockedKeyTerm("Euticus", "Eutico");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Who was Timothy?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Who was Euticus?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase1.GetParts().Count());
        //    Assert.AreEqual(1, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.GetParts().Count());

        //    const string frame = "\u00BFQuie\u0301n era {0}?";
        //    phrase1.Translation = string.Format(frame, "Timoteo");

        //    Assert.AreEqual(string.Format(frame, "Timoteo").Normalize(NormalizationForm.FormC), phrase1.Translation);
        //    Assert.AreEqual(string.Format(frame, "Eutico").Normalize(NormalizationForm.FormC), phrase2.Translation);
        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for two phrases that have a common part and verify
        ///// that a third phrase that has that part shows the translation of the translated part.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_GuessAtTwoPartPhraseThatDiffersBySingleKeyTerm()
        //{
        //    AddMockedKeyTerm("Jacob", "Jacobo");
        //    AddMockedKeyTerm("Matthew", "Mateo");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Was Jacob one of the disciples?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Was Matthew one of the disciples?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase1.GetParts().Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.GetParts().Count());

        //    const string frame = "\u00BFFue {0} uno de los discipulos?";
        //    phrase1.Translation = string.Format(frame, "Jacobo");

        //    Assert.AreEqual(string.Format(frame, "Jacobo"), phrase1.Translation);
        //    Assert.AreEqual(string.Format(frame, "Mateo"), phrase2.Translation);
        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsFalse(phrase2.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for multiple phrases. Possible part translations
        ///// should be assigned to parts according to length and numbers of occurrences, but no
        ///// portion of a translation should be used as the translation for two parts of the same
        ///// owning phrase
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_PreventTranslationFromBeingUsedForMultipleParts()
        //{
        //    AddMockedKeyTerm("Jacob", "Jacob");
        //    AddMockedKeyTerm("John", "Juan");
        //    AddMockedKeyTerm("Jesus", "Jesu\u0301s");
        //    AddMockedKeyTerm("Mary", "Mari\u0301a");
        //    AddMockedKeyTerm("Moses", "Moise\u0301s");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("So what did Jacob do?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("So what did Jesus do?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("What did Jacob do?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("What did Moses ask?");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("So what did John ask?");
        //    TranslatablePhrase phrase6 = new TranslatablePhrase("So what did Mary want?");
        //    TranslatablePhrase phrase7 = new TranslatablePhrase("What did Moses do?");
        //    TranslatablePhrase phrase8 = new TranslatablePhrase("Did Moses ask, \"What did Jacob do?\"");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2, phrase3,
        //        phrase4, phrase5, phrase6, phrase7, phrase8 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase1.GetParts().Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.GetParts().Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.GetParts().Count());
        //    Assert.AreEqual(2, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase4.GetParts().Count());
        //    Assert.AreEqual(2, phrase5.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase5.GetParts().Count());
        //    Assert.AreEqual(2, phrase6.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase6.GetParts().Count());
        //    Assert.AreEqual(2, phrase7.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase7.GetParts().Count());
        //    Assert.AreEqual(4, phrase8.TranslatableParts.Count());
        //    Assert.AreEqual(6, phrase8.GetParts().Count());

        //    phrase1.Translation = "\u00BFEntonces que\u0301 hizo Jacob?";
        //    phrase2.Translation = "\u00BFEntonces que\u0301 hizo Jesu\u0301s?";
        //    phrase3.Translation = "\u00BFQue\u0301 hizo Jacob?";
        //    phrase4.Translation = "\u00BFQue\u0301 pregunto\u0301 Moise\u0301s?";
        //    phrase5.Translation = "\u00BFEntonces que\u0301 pregunto\u0301 Juan?";

        //    Assert.AreEqual("\u00BFEntonces que\u0301 Mari\u0301a?".Normalize(NormalizationForm.FormC), phrase6.Translation);
        //    Assert.AreEqual("\u00BFQue\u0301 hizo Moise\u0301s?".Normalize(NormalizationForm.FormC), phrase7.Translation);
        //    Assert.AreEqual("Moise\u0301s pregunto\u0301 Que\u0301 Jacob hizo".Normalize(NormalizationForm.FormC), phrase8.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for multiple phrases. Possible part translations
        ///// should be assigned to parts according to length and numbers of occurrences, but no
        ///// portion of a translation should be used as the translation for two parts of the same
        ///// owning phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_UseStatsAndConfidenceToDeterminePartTranslations()
        //{
        //    AddMockedKeyTerm("ask");
        //    AddMockedKeyTerm("give");
        //    AddMockedKeyTerm("want");
        //    AddMockedKeyTerm("whatever");
        //    AddMockedKeyTerm("thing");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("ABC ask DEF");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("ABC give XYZ");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("XYZ want ABC whatever EFG");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("EFG thing ABC");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2,
        //        phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase1.GetParts().Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.GetParts().Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(5, phrase3.GetParts().Count());
        //    Assert.AreEqual(2, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase4.GetParts().Count());

        //    phrase1.Translation = "def ASK abc";
        //    phrase2.Translation = "abc xyz GIVE";
        //    phrase3.Translation = "WANT xyz abc WHATEVER efg";

        //    Assert.AreEqual("efg THING abc", phrase4.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that the code to determine the best translation for a part of a phrase will
        ///// not take a substring common to all phrases if it would mean selecting less than a
        ///// whole word instead of a statistically viable substring that consists of whole
        ///// words.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_UseStatisticalBestPartTranslations()
        //{
        //    AddMockedKeyTerm("Isaac", "Isaac");
        //    AddMockedKeyTerm("Paul", "Pablo");

        //    TranslatablePhrase phraseBreakerA = new TranslatablePhrase("Now what?");
        //    TranslatablePhrase phraseBreakerB = new TranslatablePhrase("What did Isaac say?");
        //    TranslatablePhrase phrase1 = new TranslatablePhrase("So now what did those two brothers do?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("So what did they do about the problem?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("So what did he do?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("So now what was Isaac complaining about?");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("So what did the Apostle Paul say about that?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phraseBreakerA, phraseBreakerB,
        //        phrase1, phrase2, phrase3, phrase4, phrase5 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase1.GetParts().Count());
        //    Assert.AreEqual(3, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.GetParts().Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.GetParts().Count());
        //    Assert.AreEqual(4, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(5, phrase4.GetParts().Count());
        //    Assert.AreEqual(4, phrase5.TranslatableParts.Count());
        //    Assert.AreEqual(5, phrase5.GetParts().Count());

        //    phrase1.Translation = "\u00BFEntonces ahora que\u0301 hicieron esos dos hermanos?";
        //    phrase2.Translation = "\u00BFEntonces que\u0301 hicieron acerca del problema?";
        //    phrase3.Translation = "\u00BFEntonces que\u0301 hizo?";
        //    phrase5.Translation = "\u00BFEntonces que\u0301 dijo Pablo acerca de eso?";

        //    Assert.AreEqual("\u00BFEntonces Isaac?", phrase4.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that the code to determine the best translation for a part of a phrase will
        ///// not take a substring common to all phrases if it would mean selecting less than a
        ///// whole word instead of a statistically viable substring that consists of whole
        ///// words.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_UseStatisticalBestPartTranslationsRatherThanCommonPartialWord()
        //{
        //    AddMockedKeyTerm("Isaac", "Isaac");
        //    AddMockedKeyTerm("Paul", "Pablo");

        //    TranslatablePhrase phraseBreakerA = new TranslatablePhrase("Now what?");
        //    TranslatablePhrase phraseBreakerB = new TranslatablePhrase("What did Isaac say?");
        //    TranslatablePhrase phraseBreakerC = new TranslatablePhrase("What could Isaac say?");
        //    TranslatablePhrase phrase1 = new TranslatablePhrase("So now what did those two brothers do?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("So what could they do about the problem?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("So what did he do?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("So now what was Isaac complaining about?");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("So what did the Apostle Paul say about that?");
        //    TranslatablePhrase phrase6 = new TranslatablePhrase("Why did they treat the Apostle Paul so?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phraseBreakerA, phraseBreakerB, phraseBreakerC,
        //        phrase1, phrase2, phrase3, phrase4, phrase5, phrase6 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase1.GetParts().Count());
        //    Assert.AreEqual(3, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.GetParts().Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.GetParts().Count());
        //    Assert.AreEqual(4, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(5, phrase4.GetParts().Count());
        //    Assert.AreEqual(4, phrase5.TranslatableParts.Count());
        //    Assert.AreEqual(5, phrase5.GetParts().Count());
        //    Assert.AreEqual(2, phrase6.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase6.GetParts().Count());

        //    phrase1.Translation = "Entonces AB Zuxelopitmyfor CD EF GH";
        //    phrase2.Translation = "Entonces Vuxelopitmyfor IJ KL MN OP QR";
        //    phrase3.Translation = "Entonces Wuxelopitmyfor ST";
        //    phrase5.Translation = "Entonces Xuxelopitmyfor dijo Pablo UV WX YZ";
        //    phrase6.Translation = "BG LP Yuxelopitmyfor DW MR Pablo";

        //    Assert.AreEqual("Entonces Isaac", phrase4.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a group of phrases such that the only common
        ///// character for a part they have in common is a punctuation character.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SetTranslation_DoNotTreatNormalLeadingPuncAsOpeningQuestionMark()
        //{
        //    AddMockedKeyTerm("Isaiah", "Isai\u0301as");
        //    AddMockedKeyTerm("Paul", "Pablo");
        //    AddMockedKeyTerm("Silas", "Silas");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("What did Paul and Silas do in jail?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Were Isaiah and Paul prophets?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2 },
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.TranslatableParts.Count());

        //    phrase1.Translation = "*\u00BFQue\u0301 hicieron Pablo y Silas en la carcel?";
        //    Assert.AreEqual("Isai\u0301as Pablo?".Normalize(NormalizationForm.FormC), phrase2.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a group of phrases such that the only common
        ///// character for a part they have in common is a punctuation character.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_PreventPartTranslationFromBeingSetToPunctuation()
        //{
        //    AddMockedKeyTerm("Isaiah", "Isai\u0301as");
        //    AddMockedKeyTerm("Paul", "Pablo");
        //    AddMockedKeyTerm("Silas", "Silas");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("What did Paul and Silas do in jail?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Were Isaiah and Paul prophets?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Did Paul and Silas run away?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("And what did Paul do next?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2,
        //        phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase4.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFQue\u0301 hicieron Pablo y Silas en la carcel?";
        //    phrase2.Translation = "\u00BFEran profetas Pablo e Isai\u0301as?";
        //    phrase3.Translation = "\u00BFSe corrieron Pablo y Silas?";
        //    Assert.AreEqual("\u00BFy Pablo?", phrase4.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase that has parts that are also in another
        ///// phrase that does not have a user translation but does have parts that do have a
        ///// translation.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_PreventTrashingPartTranslationsWhenReCalculating()
        //{
        //    AddMockedKeyTerm("Mary", "Mari\u0301a");
        //    AddMockedKeyTerm("Jesus");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("When?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Where did Mary find Jesus?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Where did Jesus find a rock?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("Where did Mary eat?");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("When Mary went to the tomb, where did Jesus meet her?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2,
        //        phrase3, phrase4, phrase5 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(4, phrase5.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCua\u0301ndo?";
        //    phrase2.Translation = "\u00BFDo\u0301nde encontro\u0301 Mari\u0301a a JESUS?";
        //    phrase3.Translation = "\u00BFDo\u0301nde encontro\u0301 JESUS una piedra?";
        //    phrase4.Translation = "\u00BFDo\u0301nde comio\u0301 Mari\u0301a?";
        //    Assert.AreEqual("\u00BFCua\u0301ndo Mari\u0301a Do\u0301nde JESUS?".Normalize(NormalizationForm.FormC), phrase5.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsFalse(phrase5.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a group of phrases that have a common part such
        ///// that phrases A & B have a common substring that is longer than the substring that
        ///// all three share in common.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_FallbackToSmallerCommonSubstring()
        //{
        //    AddMockedKeyTerm("the squirrel", "la ardilla");
        //    AddMockedKeyTerm("donkey", "asno");
        //    AddMockedKeyTerm("Balaam", "Balaam");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("When did the donkey and the squirrel fight?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("What did the donkey and Balaam do?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("Where are Balaam and the squirrel?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("and?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2,
        //        phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(1, phrase4.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno y la ardilla?";
        //    phrase2.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
        //    phrase3.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam y la ardilla?";
        //    Assert.AreEqual("\u00BFy?", phrase4.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsFalse(phrase4.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a group of phrases that have a common part such
        ///// that phrases A & B have a common substring that is longer than the substring that
        ///// all three share in common.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_FallbackToSmallerCommonSubstring_EndingInLargerSubstring()
        //{
        //    AddMockedKeyTerm("the squirrel", "ardilla");
        //    AddMockedKeyTerm("donkey", "asno");
        //    AddMockedKeyTerm("Balaam", "Balaam");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("When did the donkey and the squirrel fight?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Where are Balaam and the squirrel?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("What did the donkey and Balaam do?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("and?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2,
        //        phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(1, phrase4.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno loco y ardilla?";
        //    phrase2.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam loco y ardilla?";
        //    phrase3.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
        //    Assert.AreEqual("\u00BFy?", phrase4.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsFalse(phrase4.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a group of phrases that have a common part such
        ///// that phrases A & B have a common substring that is longer than the substring that
        ///// all three share in common.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_FallbackToSmallerCommonSubstring_StartingInLargerSubstring()
        //{
        //    AddMockedKeyTerm("the squirrel", "ardilla");
        //    AddMockedKeyTerm("donkey", "asno");
        //    AddMockedKeyTerm("Balaam", "Balaam");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("When did the donkey and the squirrel fight?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Where are Balaam and the squirrel?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("What did the donkey and Balaam do?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("and?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2, phrase3, phrase4 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(3, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(3, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(1, phrase4.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno y la ardilla?";
        //    phrase2.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam y la ardilla?";
        //    phrase3.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
        //    Assert.AreEqual("\u00BFy?", phrase4.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsFalse(phrase4.HasUserTranslation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests setting the translation for a phrase such that there is a single part whose
        ///// rendering does not match the statistically best rendering for that part. The
        ///// statistically best part should win.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ChangeTranslation_PreventUpdatedTranslationFromChangingGoodPartTranslation()
        //{
        //    AddMockedKeyTerm("donkey", "asno");
        //    AddMockedKeyTerm("Balaam", "Balaam");

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("When?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("When Balaam eats donkey.");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("What donkey eats?");
        //    TranslatablePhrase phrase4 = new TranslatablePhrase("What Balaam eats?");
        //    TranslatablePhrase phrase5 = new TranslatablePhrase("Donkey eats?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] { phrase1, phrase2, phrase3, phrase4, phrase5 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase4.TranslatableParts.Count());
        //    Assert.AreEqual(1, phrase5.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCua\u0301ndo?";
        //    phrase2.Translation = "\u00BFCua\u0301ndo come Balaam al asno.";
        //    phrase3.Translation = "\u00BFQue\u0301 come el asno?";
        //    phrase4.Translation = "\u00BFQue\u0301 ingiere Balaam?";
        //    Assert.AreEqual("\u00BFasno come?", phrase5.Translation);

        //    Assert.IsTrue(phrase1.HasUserTranslation);
        //    Assert.IsTrue(phrase2.HasUserTranslation);
        //    Assert.IsTrue(phrase3.HasUserTranslation);
        //    Assert.IsTrue(phrase4.HasUserTranslation);
        //    Assert.IsFalse(phrase5.HasUserTranslation);
        //}
        #endregion

        #region Rendering Selection Rules tests
        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that adding a rendering selection rule based on the preceding (English) word
        ///// causes the correct vernacular rendering to be selected.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SelectCorrectTermRendering_NoPartsTranslated_BasedOnPrecedingWordRule()
        //{
        //    AddMockedKeyTerm("Jesus", "Cristo", new [] {"Jesucristo", "Jesus", "Cristo Jesus"});

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Who was the man Jesus healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());

        //    Assert.IsFalse(phrase1.HasUserTranslation);
        //    Assert.AreEqual("Cristo", phrase1.Translation);

        //    pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new [] {new RenderingSelectionRule(@"\bman {0}", @"ucristo\b") });

        //    Assert.AreEqual("Jesucristo", phrase1.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that adding a rendering selection rule based on the following (English) word
        ///// causes the correct vernacular rendering to be inserted into the partial translation.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SelectCorrectTermRendering_SomePartsTranslated_BasedOnFollowingWordRule()
        //{
        //    AddMockedKeyTerm("Stephen", "Esteban");
        //    AddMockedKeyTerm("Mary", "Mari\u0301a");
        //    AddMockedKeyTerm("look", "mirar", new[] { "pareci\u0301a", "buscaba", "busca", "busco" });

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("What did Stephen look like to the priests and elders and other people present?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("What did Stephen do?");
        //    TranslatablePhrase phrase3 = new TranslatablePhrase("What did Mary look for?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2, phrase3 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase3.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFCo\u0301mo pareci\u0301a Esteban a los sacerdotes y ancianos y a los dema\u0301s?";
        //    phrase2.Translation = "\u00BFCo\u0301mo hizo Esteban?";

        //    Assert.IsFalse(phrase3.HasUserTranslation);
        //    Assert.AreEqual("\u00BFCo\u0301mo Mari\u0301a mirar?".Normalize(NormalizationForm.FormC), phrase3.Translation);

        //    pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
        //        new RenderingSelectionRule(@"{0} like\b", @"\bparec"),
        //        new RenderingSelectionRule(@"{0} for\b", @"\bbusc")});

        //    Assert.AreEqual("\u00BFCo\u0301mo Mari\u0301a buscaba?".Normalize(NormalizationForm.FormC), phrase3.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that adding a rendering selection rule based on the (English) suffix causes
        ///// the correct vernacular rendering to be inserted into the translation template.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SelectCorrectTermRendering_FillInTemplate_BasedOnSuffixRule()
        //{
        //    AddMockedKeyTerm("magician", "mago", new[] { "brujo" });
        //    AddMockedKeyTerm("servant", "criado", new[] { "siervo" });
        //    AddMockedKeyTerm("heal", "sanar", new[] { "curada", "sanada", "sanara\u0301", "sanas", "curan", "cura", "sana", "sanado" });

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Was the servant healed?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Was the magician healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(1, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(1, phrase2.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFFue sanado el siervo?";

        //    Assert.IsFalse(phrase2.HasUserTranslation);
        //    Assert.AreEqual("\u00BFFue sanar el mago?", phrase2.Translation);

        //    pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
        //        new RenderingSelectionRule(@"{0}\w*ed\b", @"o$")});

        //    Assert.AreEqual("\u00BFFue sanado el mago?", phrase2.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that adding a rendering selection rule based on the (English) suffix causes
        ///// the correct vernacular rendering to be inserted into the translation template. In
        ///// this test, there are multiple renderings which conform to the rendering selection
        ///// rule -- we want the one that is also the default rendering for the term.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SelectCorrectTermRendering_FillInTemplate_BasedOnSuffixRule_PreferDefault()
        //{
        //    AddMockedKeyTerm("magician", "mago", new[] { "brujo" });
        //    AddMockedKeyTerm("servant", "criado", new[] { "siervo" });
        //    AddMockedKeyTerm("heal", "sanara\u0301", new[] { "curada", "sanada", "sanar", "curara\u0301", "sanas", "curan", "cura", "sana", "sanado" });

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Will the servant be healed?");
        //    TranslatablePhrase phrase2 = new TranslatablePhrase("Will the magician be healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1, phrase2 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());
        //    Assert.AreEqual(2, phrase2.TranslatableParts.Count());

        //    phrase1.Translation = "\u00BFSe curara\u0301 el siervo?";

        //    Assert.IsFalse(phrase2.HasUserTranslation);
        //    Assert.AreEqual("\u00BFSe sanara\u0301 el mago?".Normalize(NormalizationForm.FormC), phrase2.Translation);

        //    pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
        //        new RenderingSelectionRule(@"Will .* {0}\w*\b", "ra\u0301$")});

        //    Dictionary<Word, List<KeyTermMatch>> keyTermsTable =
        //        (Dictionary<Word, List<KeyTermMatch>>)ReflectionHelper.GetField(pth, "m_keyTermsTable");

        //    keyTermsTable["heal"].First().BestRendering = "curara\u0301";

        //    Assert.AreEqual("\u00BFSe curara\u0301 el mago?".Normalize(NormalizationForm.FormC), phrase2.Translation);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests that adding a disabled rendering selection rule does not change the resulting
        ///// translation.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void SelectCorrectTermRendering_RuleDisabled()
        //{
        //    AddMockedKeyTerm("Jesus", "Cristo", new[] { "Jesucristo", "Jesus", "Cristo Jesus" });

        //    TranslatablePhrase phrase1 = new TranslatablePhrase("Who was the man Jesus healed?");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        phrase1 }, m_dummyKtList, m_keyTermRules, new List<Substitution>());
        //    ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

        //    Assert.AreEqual(2, phrase1.TranslatableParts.Count());

        //    Assert.IsFalse(phrase1.HasUserTranslation);
        //    Assert.AreEqual("Cristo", phrase1.Translation);

        //    pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] { new RenderingSelectionRule(@"\bman {0}", @"ucristo\b") });
        //    pth.TermRenderingSelectionRules[0].Disabled = true;

        //    Assert.AreEqual("Cristo", phrase1.Translation);
        //}
        #endregion

        #region Private helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a test question to the given category and adds info about key terms and parts
        /// to dictionaries used by GetParsedQuestions. Note that items in the parts array will
        /// be treated as translatable parts unless prefixed with "kt:", in which case they
        /// will be treated as key terms (corresponding key terms must be added by calling
        /// AddMockedKeyTerm.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private Question AddTestQuestion(Category cat, string text, string sRef,
            int startRef, int endRef, params string[] parts)
        {
            return AddTestQuestion(cat, false, text, sRef, startRef, endRef, parts);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a test question to the given category and adds info about key terms and parts
        /// to dictionaries used by GetParsedQuestions. Note that items in the parts array will
        /// be treated as translatable parts unless prefixed with "kt:", in which case they
        /// will be treated as key terms (corresponding key terms must be added by calling
        /// AddMockedKeyTerm.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private Question AddTestQuestion(Category cat, bool excluded, string text, string sRef,
            int startRef, int endRef, params string[] parts)
        {
            var q = new TestQ(text, sRef, startRef, endRef, excluded ? null : GetParsedParts(parts));
            q.IsExcluded = excluded;
            cat.Questions.Add(q);
            return q;
        }
        #endregion
    }

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Trivial implementation of QuestionKey for test questions (not real Scripture references)
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TestQ : Question
	{
		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TestQ(string text, string sRef, int startRef, int endRef, List<ParsedPart> parts)
		{
			Text = text;
			ScriptureReference = sRef;
			StartRef = startRef;
			EndRef = endRef;
		    m_parsedParts = parts;
		}
		#endregion

		#region Overrides of QuestionKey
		public override string ScriptureReference { get; set; }
		public override int StartRef { get; set; }
		public override int EndRef { get; set; }
		#endregion
	}
}
