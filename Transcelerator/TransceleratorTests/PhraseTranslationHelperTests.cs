// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: PhraseTranslationHelperTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AddInSideViews;
using NUnit.Framework;
using Rhino.Mocks;
using SIL.Extensions;
using SIL.Reflection;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using static System.Int32;

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

		public IEnumerable<IKeyTerm> KeyTerms { get { return m_dummyKtList; } } 

        #region Helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a list of ParsedParts based on a string array representing key terms and parts
        /// to dictionaries used by GetParsedQuestions. Note that strings in the parts array will
        /// be treated as translatable parts unless prefixed with "kt:", in which case they
        /// will be treated as key terms (corresponding key terms must be added by calling
        /// AddMockedKeyTerm). Integers will be treated as numeric parts.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        protected List<ParsedPart> GetParsedParts(object[] parts)
        {
            List<ParsedPart> parsedParts = new List<ParsedPart>();
            foreach (object part in parts)
            {
				ParsedPart parsedPart;

	            if (part is int)
	            {
		            parsedPart = new ParsedPart((int) part);
	            }
	            else
	            {
		            var sPart = (string) part;
					if (sPart.StartsWith("kt:"))
		            {
						string sWords = sPart.Substring(3);
			            KeyTermMatchSurrogate kt;
			            if (!m_keyTermsDictionary.TryGetValue(sWords, out kt))
				            m_keyTermsDictionary[sWords] =
					            kt = new KeyTermMatchSurrogate(sWords, new string(sWords.Reverse().ToArray()));
			            parsedPart = new ParsedPart(kt);
		            }
                    else if (sPart.StartsWith("num:"))
					{
						var number = Parse(sPart.Substring(4));
						parsedPart = new ParsedPart(number);
					}
		            else
		            {
						if (!m_translatablePartsDictionary.TryGetValue(sPart, out parsedPart))
			            {
							m_translatablePartsDictionary[sPart] = parsedPart = new ParsedPart(sPart.Split(new[] { ' ' },
					            StringSplitOptions.RemoveEmptyEntries).Select(w => (Word) w));
			            }
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
                List<string> listOfRenderings;
                if (!m_dummyKtRenderings.TryGetValue(underlyingTermId, out listOfRenderings))
                    m_dummyKtRenderings[underlyingTermId] = listOfRenderings = new List<string>();
                if (otherRenderings != null)
                    listOfRenderings.AddRange(otherRenderings.Where(r => !listOfRenderings.Contains(r)));
                listOfRenderings.Insert(0, bestRendering);
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

    [TestFixture, Apartment(ApartmentState.STA)]
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
		    AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
		        "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
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

            pth.Sort(PhrasesSortedBy.EnglishPhrase, true);

            Assert.AreEqual("D", pth[0].Reference);
            Assert.AreEqual("C", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
            Assert.AreEqual("B", pth[3].Reference);
            Assert.AreEqual("F", pth[4].Reference);
            Assert.AreEqual("A", pth[5].Reference);

            pth.Sort(PhrasesSortedBy.EnglishPhrase, false);

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

            pth.Sort(PhrasesSortedBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("C", pth[2].Reference);
            Assert.AreEqual("D", pth[3].Reference);
            Assert.AreEqual("E", pth[4].Reference);
            Assert.AreEqual("F", pth[5].Reference);

            pth.Sort(PhrasesSortedBy.Reference, false);

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

            var cat = section.Categories[0] = new Category { IsOverview = true };
	        cat.IsOverview = true;
            AddTestQuestion(cat, "What is the meaning of life?", "A-D", 1, 4);
            AddTestQuestion(cat, "Why is there evil?", "E-F", 5, 6);
            AddTestQuestion(cat, "Why am I here?", "A-D", 1, 4);

            cat = section.Categories[1] = new Category { IsOverview = false };
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

            pth.Sort(PhrasesSortedBy.Reference, true);

            Assert.AreEqual("What is the meaning of life?", pth[0].PhraseInUse);
            Assert.AreEqual("A-D", pth[0].Reference);
            Assert.AreEqual(0, pth[0].SequenceNumber);
            Assert.AreEqual("Why am I here?", pth[1].PhraseInUse);
            Assert.AreEqual("A-D", pth[1].Reference);
            Assert.AreEqual(2, pth[1].SequenceNumber);
            Assert.AreEqual("What would God do?", pth[2].PhraseInUse);
            Assert.AreEqual("A", pth[2].Reference);
            Assert.AreEqual(0, pth[2].SequenceNumber);
            Assert.AreEqual("What is Paul asking that man?", pth[3].PhraseInUse);
            Assert.AreEqual("A", pth[3].Reference);
            Assert.AreEqual(6, pth[3].SequenceNumber);
            Assert.AreEqual("C", pth[4].Reference);
            Assert.AreEqual("Is it okay for Paul to talk to God today?", pth[5].PhraseInUse);
            Assert.AreEqual("D", pth[5].Reference);
            Assert.AreEqual(2, pth[5].SequenceNumber);
            Assert.AreEqual("Is a dog man's best friend?", pth[6].PhraseInUse);
            Assert.AreEqual("D", pth[6].Reference);
            Assert.AreEqual(7, pth[6].SequenceNumber);
            Assert.AreEqual("E-F", pth[7].Reference);
            Assert.AreEqual(0, pth[7].Category);
            Assert.AreEqual(1, pth[7].SequenceNumber);
            Assert.AreEqual("E", pth[8].Reference);
            Assert.AreEqual("E-F", pth[9].Reference);
            Assert.AreEqual("E-G", pth[10].Reference);
            Assert.AreEqual(1, pth[10].Category);

            pth.Sort(PhrasesSortedBy.Reference, false);

            Assert.AreEqual("E-G", pth[0].Reference);
            Assert.AreEqual(1, pth[0].Category);
            Assert.AreEqual("E-F", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
            Assert.AreEqual("E-F", pth[3].Reference);
            Assert.AreEqual(0, pth[3].Category);
            Assert.AreEqual("D", pth[4].Reference);
            Assert.AreEqual(7, pth[4].SequenceNumber);
            Assert.AreEqual("D", pth[5].Reference);
            Assert.AreEqual(2, pth[5].SequenceNumber);
            Assert.AreEqual("C", pth[6].Reference);
            Assert.AreEqual("A", pth[7].Reference);
            Assert.AreEqual(6, pth[7].SequenceNumber);
            Assert.AreEqual("A", pth[8].Reference);
            Assert.AreEqual(0, pth[8].SequenceNumber);
            Assert.AreEqual("A-D", pth[9].Reference);
            Assert.AreEqual(2, pth[9].SequenceNumber);
            Assert.AreEqual("A-D", pth[10].Reference);
            Assert.AreEqual(0, pth[10].SequenceNumber);
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

            pth.Sort(PhrasesSortedBy.Translation, true);

            Assert.AreEqual(q3, pth[0].QuestionInfo);
            Assert.AreEqual(q2, pth[1].QuestionInfo);
            Assert.AreEqual(q4, pth[2].QuestionInfo);
            Assert.AreEqual(q5, pth[3].QuestionInfo);
            Assert.AreEqual(q6, pth[4].QuestionInfo);
            Assert.AreEqual(q1, pth[5].QuestionInfo);

            pth.Sort(PhrasesSortedBy.Translation, false);

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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, true, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, false, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
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
                "this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 2 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "kt:say", "radish");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 2 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("is", false, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
            Assert.AreEqual(5, pth.Phrases.Count(), "Wrong number of phrases in helper");
            pth.Sort(PhrasesSortedBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("D", pth[2].Reference);
            Assert.AreEqual("E", pth[3].Reference);
            Assert.AreEqual("F", pth[4].Reference);
		}

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests filtering localized phrases textually
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [TestCase("Paul", false)]
	    [TestCase("Pablo", true)]
		public void GetPhrasesFilteredTextually_Localized(string filterPhrase, bool useLocalization)
	    {
		    AddMockedKeyTerm("God");
		    AddMockedKeyTerm("Paul");
		    AddMockedKeyTerm("have");
		    AddMockedKeyTerm("say");

		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
			    "this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
		    AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
			    "what is" /* 2 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
		    AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
		    AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
			    "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
		    AddTestQuestion(cat, "that dog wishes this Paul and say radish", "E", 5, 5,
			    "that dog" /* 4 */, "wishes this", "kt:paul", "and", "kt:say", "radish");
		    AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 2 */, "that dog" /* 4 */);

		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    Func<TranslatablePhrase, string> localizer = tp =>
		    {
			    var s = tp.PhraseInUse;
			    if (useLocalization)
				    s = s.Replace("Paul", "Pablo");
			    return s;
		    };

		    Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

		    pth.Filter(filterPhrase, false, PhraseTranslationHelper.KeyTermFilterType.All, null, false, localizer);
		    Assert.AreEqual(4, pth.Phrases.Count(), "Wrong number of phrases in helper");
		    pth.Sort(PhrasesSortedBy.Reference, true);

		    Assert.AreEqual("A", pth[0].Reference);
		    Assert.AreEqual("B", pth[1].Reference);
		    Assert.AreEqual("D", pth[2].Reference);
		    Assert.AreEqual("E", pth[3].Reference);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All,
                ((start, end, sref) => start >= 2 && end <= 5 && sref != "C"), false);
            pth.Sort(PhrasesSortedBy.Reference, true);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, false, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, true, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, true, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, false, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.All,
                ((start, end, sref) => start >= 2 && end <= 5 && sref != "C"), false);
            pth.Sort(PhrasesSortedBy.Reference, true);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, false);
            pth.Sort(PhrasesSortedBy.Reference, true);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, true, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, false, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, true, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, false, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithRenderings, null, false);
            pth.Sort(PhrasesSortedBy.Reference, true);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter(null, false, PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings, null, false);
            pth.Sort(PhrasesSortedBy.Reference, true);
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is have radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:have", "radish");
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
                "what would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is have radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:have", "radish");
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
                "this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
            AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
                "what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
            AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
            AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
                "is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
            AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
                "that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
            AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

            Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");

            pth.Filter("is", false, PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings, null, false);
            Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper");
            pth.Sort(PhrasesSortedBy.Reference, true);

            Assert.AreEqual("A", pth[0].Reference);
            Assert.AreEqual("B", pth[1].Reference);
            Assert.AreEqual("E", pth[2].Reference);
        }
        #endregion

		#region GetMatchingPhrases Tests
		[Test]
		public void GetMatchingPhrases_NoMatches_ReturnsNoResults()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "MAT 2:2", 2, 2, "Q1");
			AddTestQuestion(cat, "Q2", "MAT 2:2", 2, 2, "Q2");
			AddTestQuestion(cat, "Q3", "REV 6:4-5", 4, 5, "Q2");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			Assert.IsFalse(pth.GetMatchingPhrases(4, 6).Any());
		}

		[Test]
		[TestCase(PhrasesSortedBy.Reference, true)]
		[TestCase(PhrasesSortedBy.Reference, false)]
		[TestCase(PhrasesSortedBy.Default, true)]
		[TestCase(PhrasesSortedBy.EnglishPhrase, false)]
		public void GetMatchingPhrases_TwoMatches_ReturnsInProperSequenceOrder(PhrasesSortedBy listSortedBy, bool ascending)
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "MAT 2:2", 2, 2, "Q1");
			AddTestQuestion(cat, "Q2", "MAT 2:2", 2, 2, "Q2");
			AddTestQuestion(cat, "Q3", "REV 6:4-5", 4, 5, "Q2");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			pth.Sort(listSortedBy, ascending);

			var matches = pth.GetMatchingPhrases(2, 2);
			Assert.AreEqual(2, matches.Count);
			Assert.That(matches.Select(m => m.PhraseInUse).Contains("Q1"));
			Assert.That(matches.Select(m => m.PhraseInUse).Contains("Q2"));
		}

		[Test]
		public void GetMatchingPhrases_ExcludedQuestion_ResultIncludesExcludedQuestion()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "MAT 2:2", 2, 2, "Q1");
			AddTestQuestion(cat, "Q2", "MAT 2:2", 2, 2, "Q2");
			AddTestQuestion(cat, "Q3", "REV 6:4-5", 4, 5, "Q2");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			pth[0].IsExcluded = true;

			var matches = pth.GetMatchingPhrases(2, 2);
			Assert.AreEqual(2, matches.Count);
			Assert.That(matches.Select(m => m.PhraseInUse).Contains("Q1"));
			Assert.That(matches.Select(m => m.PhraseInUse).Contains("Q1"));
		}
		#endregion

		#region CustomizedPhrases tests
		[Test]
	    public void CustomizedPhrases_NoCustomizations_EmptyList()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "What would God have me to say with respect to Paul?", "A", 1, 1);
		    AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2);

		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			Assert.IsFalse(pth.CustomizedPhrases.Any());
		}

	    [Test]
	    public void CustomizedPhrases_ModifiedAndDeletedQuestions_ModificationsAndDeletionsInCorrectOrder()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "Question 1?", "A", 1, 1);
		    var q = AddTestQuestion(cat, "Question 2? (deleted)", "B", 2, 2);
		    q.IsExcluded = true;
		    AddTestQuestion(cat, "Question 3?", "B", 2, 2);
		    q = AddTestQuestion(cat, "Question 4?", "C", 3, 3);
		    q.ModifiedPhrase = "Question 4? (modified)";
		    q = AddTestQuestion(cat, "Question 5?", "C", 3, 3);
		    q.ModifiedPhrase = "Question 5? (modified)";
		    q = AddTestQuestion(cat, "Question 6? (deleted)", "D", 4, 4);
		    q.IsExcluded = true;

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
			Assert.AreEqual(4, customizations.Count);
		    int i = 0;
		    Assert.AreEqual("Question 2? (deleted)", customizations[i].OriginalPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Deletion, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 4?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 4? (modified)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Modification, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 5?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 5? (modified)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Modification, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 6? (deleted)", customizations[i].OriginalPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Deletion, customizations[i].Type);
		}

	    [Test]
	    public void CustomizedPhrases_NewlyInsertedAndAddedQuestions_NewQuestionsAreInCorrectOrder()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "Question 1? (replaced)", "A", 1, 1);
			AddTestQuestion(cat, "Question 4?", "C", 3, 3);
			
		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

			pth.Sort(PhrasesSortedBy.Reference, true);
			var qBase1 = pth[0];
			Assert.AreEqual("Question 1? (replaced)", qBase1.OriginalPhrase, "Sanity check to make sure we grabbed the correct base question.");
		    qBase1.IsExcluded = true;
			var addedQuestion = new Question(qBase1.QuestionInfo, "Question 2? (added)", "Yes");
		    pth.AddQuestion(addedQuestion, qBase1.SectionId, 1, qBase1.SequenceNumber + 1, mp);
		    qBase1.AddedPhraseAfter = addedQuestion;

		    var qBase4 = pth[2];
		    Assert.AreEqual("Question 4?", qBase4.OriginalPhrase, "Sanity check to make sure we grabbed the correct base question.");
		    addedQuestion = new Question(qBase4.QuestionInfo, "Question 3? (inserted)", "No");
		    pth.AddQuestion(addedQuestion, qBase4.SectionId, 1, 2, mp);
		    qBase4.InsertedPhraseBefore = addedQuestion;

			var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(3, customizations.Count);
		    int i = 0;
		    Assert.AreEqual("Question 1? (replaced)", customizations[i].OriginalPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Deletion, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 1? (replaced)", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 2? (added)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("Yes", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 4?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 3? (inserted)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("No", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[i].Type);
		}

	    /// <summary>
	    /// TXL-207: A question should be able to be added after an inserted question without causing them
	    /// to get cross-linked such that a duplicate results.
	    /// </summary>
	    // ENHANCE(TXL-218): [TestCase(0)]
	    [TestCase(1)]
	    public void CustomizedPhrases_NewlyInsertedQuestionBeforeFirstExistingQuestionWithSubsequentAddition_NewQuestionsAreInCorrectOrder(
		    int categoryForNewQuestions)
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "Is this just a question?", "GEN 22:13", 1022013, 1022013);
			
		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

			var qBase1 = pth[0];
			var insertedQuestionBefore = new Question(qBase1.QuestionInfo, "Was this question inserted before only question in Genesis 22:13?", "Yes");
			pth.AddQuestion(insertedQuestionBefore, qBase1.SectionId, categoryForNewQuestions, qBase1.SequenceNumber, mp); 
			var customizations = pth.CustomizedPhrases;
			Assert.AreEqual(1, customizations.Count);

		    var qBase2 = pth[0];
		    Assert.AreEqual("Was this question inserted before only question in Genesis 22:13?",
			    qBase2.OriginalPhrase, "Sanity check to make sure we grabbed the correct base question.");
		    var addedQuestion = new Question(qBase2.QuestionInfo, "Was this question added after the inserted one?", "You bet");
		    pth.AddQuestion(addedQuestion, qBase2.SectionId, categoryForNewQuestions, qBase2.SequenceNumber + 1, mp);

		    Assert.AreEqual(3, pth.UnfilteredPhraseCount);

			customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(2, customizations.Count);
            int i = 0;
            Assert.AreEqual("Is this just a question?", customizations[i].OriginalPhrase);
            Assert.AreEqual("Was this question inserted before only question in Genesis 22:13?", customizations[i].ModifiedPhrase);
            Assert.AreEqual("Yes", customizations[i].Answer);
            Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[i].Type);
            i++;
            Assert.AreEqual("Was this question inserted before only question in Genesis 22:13?", customizations[i].OriginalPhrase);
            Assert.AreEqual("Was this question added after the inserted one?", customizations[i].ModifiedPhrase);
            Assert.AreEqual("You bet", customizations[i].Answer);
            Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[i].Type);
	    }

	    /// <summary>
	    /// TXL-221: A question should be able to be inserted after an added question without causing them
	    /// to get cross-linked such that a duplicate results.
	    /// </summary>
	    [Test]
	    public void CustomizedPhrases_NewlyInsertedBeforeAddition_NewQuestionsAreInCorrectOrderWithoutDuplicate()
	    {
            // SETUP
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "What do you understand it is that makes something be called \"a miracle\"?", "LUK 10:13", 42010013, 42010013);
		    AddTestQuestion(cat, "How did Jesus compare these two towns, Chorazin and Bethsaida with other cities?", "LUK 10:13-14", 42010013, 42010014);
		    AddTestQuestion(cat, "What do you think it would have meant if the people of Chorazin and Bethsaida had sat in sackcloth and ashes?", "LUK 10:13", 42010013, 42010013);
			
		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

			var qBase1 = pth[1];
			var newQuestion = new Question("LUK 10.13-14", 42010013, 42010014, "What happenes if I cange this base question later?", "No one knows the future.");
			qBase1.AddedPhraseAfter = newQuestion;
			var newPhrase = pth.AddQuestion(newQuestion, qBase1.SectionId, qBase1.Category, qBase1.SequenceNumber + 1, mp);
			newPhrase.Translation = "¿Qué sucede si cambio esta pregunta de base luego?";

			Assert.AreEqual(1, pth.CustomizedPhrases.Count);

		    var qBase2 = pth.UnfilteredPhrases[2];
			Assert.AreEqual("What happenes if I cange this base question later?",
				qBase2.OriginalPhrase, "Sanity check to make sure we grabbed the correct base question.");
            // And the last shall be first...
			newQuestion = new Question("LUK 10.13-14", 42010013, 42010014, null, null);
			var immutableKeyOfSecondAddedQuestion = newQuestion.Id;
			qBase2.InsertedPhraseBefore = newQuestion;
			newPhrase = pth.AddQuestion(newQuestion, qBase2.SectionId, qBase2.Category, qBase2.SequenceNumber, mp);
			newPhrase.Translation = "Añadí esta frase antes de cambiar la pregunta de base.";

			Assert.AreEqual(5, pth.UnfilteredPhraseCount);

            // SUT
			var customizations = pth.CustomizedPhrases;

            // VERIFY
		    Assert.AreEqual(2, customizations.Count);

			var cust = customizations.Single(c => c.ModifiedPhrase == immutableKeyOfSecondAddedQuestion);
            Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type,
				"Although it was inserted before the following question, we actually want it to be" +
				"persisted as an addition after the following one. (This makes the logic simpler and less error prone.)");
            Assert.AreEqual("How did Jesus compare these two towns, Chorazin and Bethsaida with other cities?", cust.OriginalPhrase);
            Assert.IsNull(cust.Answer);
			Assert.AreEqual(immutableKeyOfSecondAddedQuestion, cust.ImmutableKey);

			cust = customizations.Single(c => c.ModifiedPhrase == "What happenes if I cange this base question later?");
            Assert.AreEqual(immutableKeyOfSecondAddedQuestion, cust.OriginalPhrase);
            Assert.AreEqual("No one knows the future.", cust.Answer);
            Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type);

			Assert.AreEqual(pth.UnfilteredPhrases.IndexOf(p => p.QuestionInfo.Id == immutableKeyOfSecondAddedQuestion) + 1,
				pth.UnfilteredPhrases.IndexOf(p => p.QuestionInfo.Text == "What happenes if I cange this base question later?"));
        }

	    /// <summary>
	    /// TXL-221: A question should be able to be inserted after an added question without causing them
	    /// to get cross-linked such that a duplicate results.
	    /// </summary>
	    [Test]
	    public void CustomizedPhrases_NewAdditionsAndInsertionsHangingOffPrevAdditionAndOrig_NewQuestionsAreInCorrectOrderWithoutCircularReferences()
	    {
            // SETUP
            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "In what ways was John not like a man in fine clothes?", "LUK 7:25", 42007025, 42007025);
            var insertedBefore = AddTestQuestion(cat, "Are we going to cook anything besides just roasting marshmallows?", "LUK 7:27", 42007027, 42007027);
            var immutableKeyOfQuestion1_1 = insertedBefore.Id;
            insertedBefore.IsUserAdded = true;
            insertedBefore.Answers = new[] { "Depende de la lluvia." };
            AddTestQuestion(cat, "What was the purpose of John's ministry?", "LUK 7:27", 42007027, 42007027);
            var addedAfter = AddTestQuestion(cat, "Does this have an English version?", "LUK 7:27", 42007027, 42007027);
            var immutableKeyOfQuestion1_2 = addedAfter.Id;
            addedAfter.IsUserAdded = true;
            addedAfter.Answers = new[] { "Claro qu sí" };
            AddTestQuestion(cat, "What do you think was the reason that Jesus said that John was greater than anyone else ever born?", "LUK 7:28", 42007028, 42007028);

            var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

			pth.UnfilteredPhrases.Single(p => p.QuestionInfo.Id == immutableKeyOfQuestion1_1)
				.Translation = "¿Vamos a cocinar comida aparte de los besitos?";
			var baseFor2_1And2 = pth.UnfilteredPhrases.Single(p => p.QuestionInfo.Id == immutableKeyOfQuestion1_2);
			baseFor2_1And2.Translation = "¿Tiene ésta una versión en inglés?";
			var baseFor2_3 = pth.UnfilteredPhrases.Single(p => p.QuestionInfo.Text == "What was the purpose of John's ministry?");

			var customizations = pth.CustomizedPhrases;
			Assert.AreEqual(2, customizations.Count, "Sanity check");
			Assert.AreEqual("Are we going to cook anything besides just roasting marshmallows?",
				customizations[0].ModifiedPhrase, "Sanity check");
			Assert.AreEqual("Does this have an English version?",
				customizations[1].ModifiedPhrase, "Sanity check");

			var newQuestion = new Question("LUK 7.27", 42007027, 42007027, "Is this an English question after the other one?", "Parece que sí");
			var immutableKeyOfQuestion2_1 = newQuestion.Id;
			baseFor2_1And2.AddedPhraseAfter = newQuestion;
			pth.AddQuestion(newQuestion, baseFor2_1And2.SectionId, baseFor2_1And2.Category, baseFor2_1And2.SequenceNumber + 1, mp)
				.Translation = "¿Es ésta una pregunta en inglés que viene después de la otra?";

			newQuestion = new Question("LUK 7.27", 42007027, 42007027, "Could this perhaps be a preceding English question then?", "Mesmamente");
			var immutableKeyOfQuestion2_2 = newQuestion.Id;
			baseFor2_1And2.InsertedPhraseBefore = newQuestion;
			pth.AddQuestion(newQuestion, baseFor2_1And2.SectionId, baseFor2_1And2.Category, baseFor2_1And2.SequenceNumber, mp)
				.Translation = "¿Acaso sería posible que ésta sea una pregunta anterior en inglés entonces?";

			newQuestion = new Question("LUK 7.27", 42007027, 42007027, null, "Quien sabe");
			var immutableKeyOfQuestion2_3 = newQuestion.Id;
			baseFor2_3.AddedPhraseAfter = newQuestion;
			pth.AddQuestion(newQuestion, baseFor2_3.SectionId, baseFor2_3.Category, baseFor2_3.SequenceNumber + 1, mp)
				.Translation = "¿Funcionaría pegarle otra pregunta sin inglés después de la pregunta original?";

			Assert.AreEqual(8, pth.UnfilteredPhraseCount, "Sanity check");

            // SUT
			customizations = pth.CustomizedPhrases;

            // VERIFY
		    Assert.AreEqual(5, customizations.Count);

			var i = 0;
            var cust = customizations[i];
			Assert.AreEqual(immutableKeyOfQuestion1_1, cust.ImmutableKey);
			Assert.AreEqual("Are we going to cook anything besides just roasting marshmallows?", cust.ModifiedPhrase);
			Assert.AreEqual("What was the purpose of John's ministry?", cust.OriginalPhrase);
			Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, cust.Type);
			
			cust = customizations[++i];
			Assert.AreEqual(immutableKeyOfQuestion2_3, cust.ImmutableKey);
			Assert.AreEqual("Quien sabe", cust.Answer);
			Assert.AreEqual("What was the purpose of John's ministry?", cust.OriginalPhrase);
			Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type);

			cust = customizations[++i];
			Assert.AreEqual(immutableKeyOfQuestion2_2, cust.ImmutableKey);
			Assert.AreEqual("Could this perhaps be a preceding English question then?", cust.ModifiedPhrase);
			Assert.AreEqual("Mesmamente", cust.Answer);
			Assert.AreEqual(customizations[i - 1].ModifiedPhrase, cust.OriginalPhrase);
			Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type);

			cust = customizations[++i];
			Assert.AreEqual(immutableKeyOfQuestion1_2, cust.ImmutableKey);
			Assert.AreEqual("Does this have an English version?", cust.ModifiedPhrase);
			Assert.AreEqual("Claro qu sí", cust.Answer);
			Assert.AreEqual(customizations[i - 1].ModifiedPhrase, cust.OriginalPhrase);
			Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type);

			cust = customizations[++i];
			Assert.AreEqual(immutableKeyOfQuestion2_1, cust.ImmutableKey);
			Assert.AreEqual("Is this an English question after the other one?", cust.ModifiedPhrase);
			Assert.AreEqual("Parece que sí", cust.Answer);
			Assert.AreEqual(customizations[i - 1].ModifiedPhrase, cust.OriginalPhrase);
			Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, cust.Type);

			foreach (var customization in customizations)
			{
				Assert.IsFalse(customizations.Any(c => c.ModifiedPhrase == customization.OriginalPhrase &&
					c.OriginalPhrase == customization.ModifiedPhrase), $"Circular dependency between {customization.ModifiedPhrase} and {customization.OriginalPhrase}");
			}
		}

	    [Test]
	    public void CustomizedPhrases_InsertedQuestionWithLaterReference_NewQuestionComesBeforeBase()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "One man in the crowd was shouting to Jesus. About what was he shouting?", "LUK 9.38-40", 42009038, 42009040);
			AddTestQuestion(cat, "Had anyone else tried to help this man and his boy?", "LUK 9.40", 42009040, 42009040);
			AddTestQuestion(cat, "What does it mean to \"cast out a spirit/demon\" from a person?", "LUK 9.37-40", 42009037, 42009040);
			AddTestQuestion(cat, "What did all the crowd think about this?", "LUK 9.43", 42009043, 42009043);
			
		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

			pth.Sort(PhrasesSortedBy.Reference, true);
		    var qBase = pth[2];
		    Assert.AreEqual("Had anyone else tried to help this man and his boy?", qBase.OriginalPhrase,
			    "Sanity check to make sure we grabbed the correct base question.");
		    var addedQuestion = new Question("LUK 9.43", 42009043, 42009043, "Is this in order?", "No");
		    pth.AddQuestion(addedQuestion, qBase.SectionId, qBase.Category, qBase.SequenceNumber, mp);
		    qBase.InsertedPhraseBefore = addedQuestion;

			var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[0].Type,
                "Although it was inserted before the following question, we actually want it to be" +
				"persisted as an addition after the following one. (This makes the logic simpler and less error prone.)");
		    Assert.AreEqual("One man in the crowd was shouting to Jesus. About what was he shouting?",
				customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this in order?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("No", customizations[0].Answer);

            Assert.AreEqual(pth.UnfilteredPhrases.IndexOf(p => p.QuestionInfo.Text == "Had anyone else tried to help this man and his boy?") - 1,
				pth.UnfilteredPhrases.IndexOf(p => p.QuestionInfo.Text == "Is this in order?"));
		}

	    [Test]
	    public void CustomizedPhrases_AddedQuestionWithEarlierReference_NewQuestionComesAfterBase()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    AddTestQuestion(cat, "One man in the crowd was shouting to Jesus. About what was he shouting?", "LUK 9.38-40", 42009038, 42009040);
		    AddTestQuestion(cat, "Had anyone else tried to help this man and his boy?", "LUK 9.40", 42009040, 42009040);
		    AddTestQuestion(cat, "What does it mean to \"cast out a spirit/demon\" from a person?", "LUK 9.37-40", 42009037, 42009040);
		    AddTestQuestion(cat, "What did all the crowd think about this?", "LUK 9.43", 42009043, 42009043);
			
		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);

		    pth.Sort(PhrasesSortedBy.Reference, true);
		    var qBase = pth[2];
		    Assert.AreEqual("Had anyone else tried to help this man and his boy?", qBase.OriginalPhrase,
			    "Sanity check to make sure we grabbed the correct base question.");
		    var addedQuestion = new Question("LUK 9.38", 42009038, 42009038, "Is this in order?", "No");
		    pth.AddQuestion(addedQuestion, qBase.SectionId, qBase.Category, qBase.SequenceNumber + 1, mp);
		    qBase.AddedPhraseAfter = addedQuestion;

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual("Had anyone else tried to help this man and his boy?", customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this in order?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("No", customizations[0].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[0].Type);
	    }

	    [Test]
	    public void CustomizedPhrases_PreviouslyInsertedAndAddedQuestionsMatchSurroundingAdjacentRefs_AddedQuestionsAreInCorrectOrder()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    var qBase1 = AddTestQuestion(cat, "Question 1? (replaced)", "A", 1, 1);
		    qBase1.IsExcluded = true;
		    var qAdded = AddTestQuestion(cat, "Question 2? (added)", "A", 1, 1);
		    qAdded.IsUserAdded = true;
		    qAdded.Answers = new[] { "Yes" };
		    qBase1.AddedQuestionAfter = qAdded; // This will be ignored.
		    var qInserted = AddTestQuestion(cat, "Question 3? (inserted)", "B", 2, 2);
		    qInserted.IsUserAdded = true;
		    qInserted.Answers = new[] { "No" };
		    var qBase4 = AddTestQuestion(cat, "Question 4?", "B", 2, 2);
		    qBase4.InsertedQuestionBefore = qInserted; // This will be ignored.

		    var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(4, pth.UnfilteredPhraseCount);
		    Assert.AreEqual(3, customizations.Count);
		    int i = 0;
		    Assert.AreEqual("Question 1? (replaced)", customizations[i].OriginalPhrase);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.Deletion, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 1? (replaced)", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 2? (added)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("Yes", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 4?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 3? (inserted)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("No", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[i].Type);
	    }

		[Test]
	    public void CustomizedPhrases_PreviouslyInsertedAndAddedQuestionsForDifferentRefs_AddedQuestionsAreInCorrectOrder()
	    {
		    var cat = m_sections.Items[0].Categories[0];
		    var q = AddTestQuestion(cat, "Question 1? (inserted)", "A", 1, 1);
		    q.IsUserAdded = true;
		    q.Answers = new[] {"Yes"};
			AddTestQuestion(cat, "Question 2?", "B", 2, 2);
			q = AddTestQuestion(cat, "Question 3? (added)", "C", 3, 3);
		    q.IsUserAdded = true;
		    q.Answers = new[] { "No" };
		    q = AddTestQuestion(cat, "Question 4? (added)", "D", 4, 4);
		    q.IsUserAdded = true;
		    q.Answers = new[] { "Maybe" };

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(4, pth.UnfilteredPhraseCount);
		    Assert.AreEqual(3, customizations.Count);
			int i = 0;
		    Assert.AreEqual("Question 2?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 1? (inserted)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("Yes", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 2?", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 3? (added)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("No", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[i].Type);
		    i++;
		    Assert.AreEqual("Question 3? (added)", customizations[i].OriginalPhrase);
		    Assert.AreEqual("Question 4? (added)", customizations[i].ModifiedPhrase);
		    Assert.AreEqual("Maybe", customizations[i].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[i].Type);
		}
        
        #region TXL-216
		[TestCase(37)]
		[TestCase(38)]
		[TestCase(43)]
	    public void CustomizedPhrases_PreviouslyAddedQuestionsWithDifferentReferencesInCategoryOfPrecedingQuestion_AddedQuestionsAreAssociatedWithBaseInSameCategory(
		    int verseForAddedQuestion)
	    {
		    m_sections.Items[0].Categories = new Category[2];
		    var cat = m_sections.Items[0].Categories[0] = new Category {Type = "Overview", IsOverview = true};
		    var q = AddTestQuestion(cat, "What happened when Jesus and the three disciples came down from the mountain?",
			    "LUK 9.37-43", 42009037, 42009043);
		    q.IsUserAdded = false;
		    q.Answers = new[] {"A crowd met them.", 
			    "A man in that crowd had a son whom a spirit seized, caused him to have convulsions and would not leave him alone."};
		    var bbCccVvv = 42009000 + verseForAddedQuestion;
		    q = AddTestQuestion(cat, "Is this added to the overview category?", $"LUK 9:{verseForAddedQuestion}", bbCccVvv, bbCccVvv);
			q.IsUserAdded = true;
		    q.Answers = new[] { "I hope so." };
		    cat = m_sections.Items[0].Categories[1] = new Category {Type = "Minutia", IsOverview = false};
		    q = AddTestQuestion(cat, "One man in the crowd was shouting to Jesus. About what was he shouting?", "LUK 9.38-40", 42009038, 42009040);
		    q.IsUserAdded = false;
		    q.Answers = new[] { "He needed help." };

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(5, pth.UnfilteredPhraseCount, "Two for the categories, and three for the questions.");
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual("What happened when Jesus and the three disciples came down from the mountain?", customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this added to the overview category?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("I hope so.", customizations[0].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.AdditionAfter, customizations[0].Type);
		}
        
	    [TestCase(37, 37)]
	    [TestCase(38, 38)]
	    [TestCase(38, 43)]
	    [TestCase(43, 43)]
	    [TestCase(41, 43)]
	    [TestCase(41, 42)]
	    public void CustomizedPhrases_PreviouslyAddedQuestionsWithDifferentReferencesInSectionOfPrecedingQuestion_AddedQuestionsAreAssociatedWithBaseInSameSectionAndCategory(
		    int startVerseForAddedQuestion, int endVerseForAddedQuestion)
	    {
		    m_sections.Items = new Section[2];
		    m_sections.Items[0] = new Section
			{
				Categories = new Category[1],
			    Heading = "Luke 9:37-43a Jesus ordered an evil spirit to leave a boy, and it did.",
			    StartRef = 42009037,
			    EndRef = 4200943
		    };
		    m_sections.Items[1] = new Section
		    {
			    Categories = new Category[1],
			    Heading = "Luke 9:43b-45 Jesus says someone will betray him to his enemies.",
			    StartRef = 42009043,
			    EndRef = 4200945
		    };
		    var cat = m_sections.Items[0].Categories[0] = new Category {Type = "Minutia", IsOverview = false};
		    var q = AddTestQuestion(cat, "What did all the crowd think about this?",
			    "LUK 9.43", 42009043, 42009043);
		    q.IsUserAdded = false;
		    q.Answers = new[] {"They were all astounded/amazed at the great power of God to cast out such a powerful demon. (43a)"};
		    cat = m_sections.Items[1].Categories[0] = new Category {Type = "Minutia", IsOverview = false};
		    var verseBridgeSuffix = startVerseForAddedQuestion == endVerseForAddedQuestion ? null : "-" + endVerseForAddedQuestion; 
		    q = AddTestQuestion(cat, "Is this inserted into the Minutia category of the second section?",
			    $"LUK 9:{startVerseForAddedQuestion}{verseBridgeSuffix}",
			    42009000 + startVerseForAddedQuestion, 42009000 + endVerseForAddedQuestion);
			q.IsUserAdded = true;
		    q.Answers = new[] { "I hope so." };
		    q = AddTestQuestion(cat, "What new information did Jesus tell his disciples?", "LUK 9.43-44", 42009043, 42009044);
		    q.IsUserAdded = false;
		    q.Answers = new[] { "He needed help." };

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(4, pth.UnfilteredPhraseCount, "One for the Minutia category, and three for the questions.");
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual("What new information did Jesus tell his disciples?", customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this inserted into the Minutia category of the second section?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("I hope so.", customizations[0].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[0].Type);
		}
        
		[TestCase(37)]
		[TestCase(38)]
		[TestCase(43)]
	    public void CustomizedPhrases_PreviouslyInsertedQuestionsWithDifferentReferencesInCategoryOfFollowingQuestion_InsertedQuestionsAreAssociatedWithBaseInSameCategory(
		    int verseForInsertedQuestion)
	    {
		    m_sections.Items[0].Categories = new Category[2];
		    var cat = m_sections.Items[0].Categories[0] = new Category {Type = "Overview", IsOverview = true};
		    var q = AddTestQuestion(cat, "What happened when Jesus and the three disciples came down from the mountain?",
			    "LUK 9.37-43", 42009037, 42009043);
		    q.IsUserAdded = false;
		    q.Answers = new[] {"A crowd met them.", 
			    "A man in that crowd had a son whom a spirit seized, caused him to have convulsions and would not leave him alone."};
		    cat = m_sections.Items[0].Categories[1] = new Category {Type = "Minutia", IsOverview = false};
		    var bbCccVvv = 42009000 + verseForInsertedQuestion;
		    q = AddTestQuestion(cat, "Is this inserted into the Minutia category?", $"LUK 9:{verseForInsertedQuestion}", bbCccVvv, bbCccVvv);
			q.IsUserAdded = true;
		    q.Answers = new[] { "I hope so." };
		    q = AddTestQuestion(cat, "One man in the crowd was shouting to Jesus. About what was he shouting?", "LUK 9.38-40", 42009038, 42009040);
		    q.IsUserAdded = false;
		    q.Answers = new[] { "He needed help." };

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(5, pth.UnfilteredPhraseCount, "Two for the categories, and three for the questions.");
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual("One man in the crowd was shouting to Jesus. About what was he shouting?", customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this inserted into the Minutia category?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("I hope so.", customizations[0].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[0].Type);
		}
        
		[TestCase(43)]
		[TestCase(44)]
		[TestCase(45)]
	    public void CustomizedPhrases_PreviouslyInsertedQuestionsWithDifferentReferencesInSectionOfFollowingQuestion_InsertedQuestionsAreAssociatedWithBaseInSameSectionAndCategory(
		    int verseForInsertedQuestion)
	    {
		    m_sections.Items = new Section[2];
		    m_sections.Items[0] = new Section
			{
				Categories = new Category[1],
			    Heading = "Luke 9:37-43a Jesus ordered an evil spirit to leave a boy, and it did.",
			    StartRef = 42009037,
			    EndRef = 4200943
		    };
		    m_sections.Items[1] = new Section
		    {
			    Categories = new Category[1],
			    Heading = "Luke 9:43b-45 Jesus says someone will betray him to his enemies.",
			    StartRef = 42009043,
			    EndRef = 4200945
		    };
		    var cat = m_sections.Items[0].Categories[0] = new Category {Type = "Minutia", IsOverview = false};
		    var q = AddTestQuestion(cat, "What did all the crowd think about this?",
			    "LUK 9.43", 42009043, 42009043);
		    q.IsUserAdded = false;
		    q.Answers = new[] {"They were all astounded/amazed at the great power of God to cast out such a powerful demon. (43a)"};
		    cat = m_sections.Items[1].Categories[0] = new Category {Type = "Minutia", IsOverview = false};
		    var bbCccVvv = 42009000 + verseForInsertedQuestion;
		    q = AddTestQuestion(cat, "Is this inserted into the Minutia category of the second section?",
			    $"LUK 9:{verseForInsertedQuestion}", bbCccVvv, bbCccVvv);
			q.IsUserAdded = true;
		    q.Answers = new[] { "I hope so." };
		    q = AddTestQuestion(cat, "What new information did Jesus tell his disciples?", "LUK 9.43-44", 42009043, 42009044);
		    q.IsUserAdded = false;
		    q.Answers = new[] { "He needed help." };

			var qp = new QuestionProvider(GetParsedQuestions());
		    PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

		    var customizations = pth.CustomizedPhrases;
		    Assert.AreEqual(4, pth.UnfilteredPhraseCount, "One for the Minutia category, and three for the questions.");
		    Assert.AreEqual(1, customizations.Count);
		    Assert.AreEqual("What new information did Jesus tell his disciples?", customizations[0].OriginalPhrase);
		    Assert.AreEqual("Is this inserted into the Minutia category of the second section?", customizations[0].ModifiedPhrase);
		    Assert.AreEqual("I hope so.", customizations[0].Answer);
		    Assert.AreEqual(PhraseCustomization.CustomizationType.InsertionBefore, customizations[0].Type);
		}
        #endregion
        #endregion

        #region AddQuestion Tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests adding a totally unique question (no words corresponding to existing parts)
        /// to a PhraseTranslationHelper with no filter set and sorted by reference.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
		public void AddQuestion_NoFilterSet_SortedByReference_NoKeyTerms_OneNewPart_NewPhraseWithOneNewPartAdded()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say", null);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
			AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "A", 1, 1,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
			AddTestQuestion(cat, "that dog", "B", 2, 2, "that dog" /* 4 */);
			AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "B", 2, 2,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
			AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "B", 2, 2,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
			AddTestQuestion(cat, "What is that dog?", "B", 2, 2, "what is" /* 3 */, "that dog" /* 4 */);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			pth.Sort(PhrasesSortedBy.Reference, true);
			var originalCount = pth.UnfilteredPhraseCount;

			var basedOnQuestion = pth[3];
			var sequenceNumberOfQuestionBeforeNewQuestion = basedOnQuestion.SequenceNumber;
			var textOfQuestionBeforeNewQuestion = basedOnQuestion.QuestionInfo.Text;
			var textOfQuestionFollowingNewQuestion = pth[4].QuestionInfo.Text;

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			TranslatablePhrase newPhrase = pth.AddQuestion(new Question(basedOnQuestion.QuestionInfo, "Wherefore cometh thou?", "I'm just bored, I guess."),
				basedOnQuestion.SectionId, 1, sequenceNumberOfQuestionBeforeNewQuestion + 1, mp);

			Assert.AreEqual("Wherefore cometh thou?", newPhrase.QuestionInfo.Text);
			Assert.AreEqual(basedOnQuestion, pth[3], "Based on question should still be in same position in list.");
			Assert.AreEqual(textOfQuestionBeforeNewQuestion, basedOnQuestion.QuestionInfo.Text);
			Assert.AreEqual(sequenceNumberOfQuestionBeforeNewQuestion, basedOnQuestion.SequenceNumber, "For a new question added after based-on question, sequence number of based-on question should not change.");
			Assert.AreEqual(newPhrase, pth[4]);
			Assert.AreEqual(sequenceNumberOfQuestionBeforeNewQuestion + 1, newPhrase.SequenceNumber);
			Assert.AreEqual(textOfQuestionFollowingNewQuestion, pth[5].QuestionInfo.Text);
			Assert.AreEqual(newPhrase.SequenceNumber + 1, pth[5].SequenceNumber);
			Assert.AreEqual(newPhrase.SequenceNumber + 2, pth[6].SequenceNumber);
			Assert.AreEqual(originalCount + 1, pth.UnfilteredPhraseCount);
			Assert.AreEqual(originalCount + 1, pth.FilteredPhraseCount);

			for (int i = 0; i < pth.FilteredPhraseCount; i++)
				Assert.IsTrue(pth.UnfilteredPhrases.Contains(pth[i]));

			var onlyPart = newPhrase.TranslatableParts.Single();
			Assert.AreEqual(0, newPhrase.KeyTermRenderings.Length);
			Assert.AreEqual("wherefore cometh thou", onlyPart.Text);
			Assert.AreEqual(newPhrase, onlyPart.OwningPhrases.Single());
		}
		
		/// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests adding a question with some words already identified as existing parts and
	    /// key terms to a PhraseTranslationHelper with no filter set and default sorting.
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [Test]
		public void AddQuestion_Insert_NoFilterSet_DefaultSorting_ExistingKeyTermsAndParts_NewPhraseAdded()
	    {
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say", null);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
			AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
			AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
			AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
			AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
			AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
		    var originalCount = pth.UnfilteredPhraseCount;

			var basedOnQuestion = pth[3];
			var sequenceNumberOfNewQuestion = basedOnQuestion.SequenceNumber;

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			TranslatablePhrase newPhrase = pth.AddQuestion(new Question(basedOnQuestion.QuestionInfo, "What is Paul saying?", null),
				basedOnQuestion.SectionId, 1, sequenceNumberOfNewQuestion, mp);

			Assert.AreEqual("What is Paul saying?", newPhrase.QuestionInfo.Text);
			Assert.AreEqual(newPhrase.SequenceNumber + 1, basedOnQuestion.SequenceNumber);
			Assert.AreEqual(sequenceNumberOfNewQuestion, newPhrase.SequenceNumber);
			Assert.AreEqual(originalCount + 1, pth.UnfilteredPhraseCount);
			Assert.AreEqual(originalCount + 1, pth.FilteredPhraseCount);

			var expectedSeq = 0;
			foreach (var tp in pth.UnfilteredPhrases)
				Assert.AreEqual(expectedSeq++, tp.SequenceNumber);
			foreach (var tp in pth.Phrases)
				Assert.IsTrue(pth.UnfilteredPhrases.Contains(tp));

			var translatableParts = newPhrase.TranslatableParts.ToList();
			Assert.AreEqual(2, translatableParts.Count);
			Assert.AreEqual(2, newPhrase.KeyTermRenderings.Length);
			Assert.AreEqual("what", translatableParts[0].Text);
			Assert.AreEqual("is", translatableParts[1].Text);
			Assert.AreEqual(newPhrase, translatableParts[0].OwningPhrases.Single());
			Assert.AreEqual(newPhrase, translatableParts[1].OwningPhrases.Single());
	    }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests adding a question with some words already identified as existing parts and
		/// key terms to a PhraseTranslationHelper with no filter set and default sorting.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AddQuestion_Insert_FilterSet_SortedByQuestion_ExistingMultiwordParts_NewPhraseAdded()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say", null);

			var cat = m_sections.Items[0].Categories[0];
			var otherQuestions = new List<Question>(6);
			otherQuestions.Add(AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul"));
			otherQuestions.Add(AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */));
			otherQuestions.Add(AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */));
			otherQuestions.Add(AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today"));
			otherQuestions.Add(AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish"));
			otherQuestions.Add(AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */));

			var qp = new QuestionProvider(GetParsedQuestions());
			var pth = new PhraseTranslationHelper(qp);
			pth.Filter("this", true, PhraseTranslationHelper.KeyTermFilterType.All, null, false);
			pth.Sort(PhrasesSortedBy.EnglishPhrase, true);

			var originalCount = pth.UnfilteredPhraseCount;

			var basedOnQuestion = pth.Phrases.Last();
			var sequenceNumberOfNewQuestion = basedOnQuestion.SequenceNumber;
			otherQuestions.Remove(basedOnQuestion.QuestionInfo);

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			TranslatablePhrase newPhrase = pth.AddQuestion(new Question(basedOnQuestion.QuestionInfo, "Explain this with respect to that.", null),
				basedOnQuestion.SectionId, 1, sequenceNumberOfNewQuestion, mp);

			Assert.AreEqual("Explain this with respect to that.", newPhrase.QuestionInfo.Text);
			Assert.AreEqual(newPhrase, pth[0]);
			Assert.AreEqual(newPhrase.SequenceNumber + 1, basedOnQuestion.SequenceNumber);
			Assert.AreEqual(sequenceNumberOfNewQuestion, newPhrase.SequenceNumber);
			Assert.AreEqual(originalCount + 1, pth.UnfilteredPhraseCount);
			Assert.AreEqual(3, pth.FilteredPhraseCount);

			foreach (var tp in pth.Phrases)
			{
				Assert.IsTrue(pth.UnfilteredPhrases.Contains(tp));
				if (otherQuestions.Contains(tp.QuestionInfo))
					Assert.AreEqual(tp.Reference[0] - 'A' + 1, tp.SequenceNumber);
			}
			for (int i = 0; i < pth.FilteredPhraseCount; i++)
				Assert.IsTrue(pth.UnfilteredPhrases.Contains(pth[i]));

			var translatableParts = newPhrase.TranslatableParts.ToList();
			Assert.AreEqual(1, translatableParts.Count);
			Assert.AreEqual("explain this with respect to that", translatableParts[0].Text);
			Assert.AreEqual(newPhrase, translatableParts[0].OwningPhrases.Single());
			// TODO: The following would be preferable, but it is fairly tricky to implement.
			// See commented out code in PhrasePartManager
			//Assert.AreEqual(3, translatableParts.Count);
			//Assert.AreEqual("explain this", translatableParts[0].Text);
			//Assert.AreEqual("with respect to", translatableParts[1].Text);
			//Assert.AreEqual("that", translatableParts[2].Text);
			//Assert.AreEqual(newPhrase, translatableParts[0].OwningPhrases.Single());
			//Assert.AreEqual(newPhrase, translatableParts[1].OwningPhrases.Single());
			//Assert.AreEqual(newPhrase, translatableParts[2].OwningPhrases.Single());
		}
	    #endregion

		#region AttachNewQuestionToAdjacentPhrase Tests
        // This is a test scenario that does not correspond to something that could really happen,
        // because the actual TXL data has Overview questions for the first section in Genesis.
		[Test]
		public void AttachNewQuestionToAdjacentPhrase_NewQuestionIsFirstInList_AttachedAsInsertBefore()
		{
			var cat = m_sections.Items[0].Categories[0];
            Assert.IsFalse(m_sections.Items[0].Categories.Single().IsOverview,
	            "Setup problem: This test want to insert into a (non-existent) category with a " +
	            "lower index than that of the first test question");

			AddTestQuestion(cat, "Q1", "GEN 1:1", 001001001, 001001001, "Q1");
			AddTestQuestion(cat, "Q2", "GEN 2:2", 001002002, 001002002, "Q2");
			AddTestQuestion(cat, "Q3", "GEN 2:2-3", 001002002, 001002003, "Q3");

			var qp = new QuestionProvider(GetParsedQuestions());
			var pth = new PhraseTranslationHelper(qp);

			var newQuestion = new Question("GEN 1:1", 001001001, 001001001, "Why is this the first question?", null);
			var newPhrase = new TranslatablePhrase(newQuestion, 0, 0, 0);
			pth.AttachNewQuestionToAdjacentPhrase(newPhrase);

			var q1 = pth.Phrases.Single(p => p.InsertedPhraseBefore != null);
			Assert.AreEqual("Q1", q1.PhraseInUse);
			Assert.AreEqual(q1.InsertedPhraseBefore, newQuestion);
			Assert.IsTrue(pth.Phrases.All(p => p.AddedPhraseAfter == null));
		}

		[Test]
		public void AttachNewQuestionToAdjacentPhrase_NewQuestionIsLastInList_AttachedAsAddedAfter()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "REV 20:9", 66020009, 66020009, "Q1");
			AddTestQuestion(cat, "Q2", "REV 20:9", 66020009, 66020009, "Q2");
			AddTestQuestion(cat, "Q3", "REV 20:11-12", 66020011, 66020012, "Q3");

			var qp = new QuestionProvider(GetParsedQuestions());
			var pth = new PhraseTranslationHelper(qp);
			foreach (var phrase in pth.UnfilteredPhrases)
			{
				Assert.AreEqual(0, phrase.SectionId, "Setup sanity check");
				Assert.AreEqual(1, phrase.Category, "Setup sanity check");
			}

			var newQuestion = new Question("REV 20:10-12", 66020010, 66020012, "Why is this the last question?", null);
			var newPhrase = new TranslatablePhrase(newQuestion, 0, 2 /* new category for this section*/, 0);
			pth.AttachNewQuestionToAdjacentPhrase(newPhrase);

			var q3 = pth.Phrases.Single(p => p.AddedPhraseAfter != null);
			Assert.AreEqual("Q3", q3.PhraseInUse);
			Assert.AreEqual(q3.AddedPhraseAfter, newQuestion);
			Assert.IsTrue(pth.Phrases.All(p => p.InsertedPhraseBefore == null));
		}
		#endregion

		#region ModifyQuestion Tests
		[Test]
		public void ModifyQuestion_CapitalizationAndPunctuationOnlyChange_PartsAndKeyTermsSequencesUnchanged()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say", null);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
			AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
			AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
			AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
			var questionToModify = AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
			AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phraseToModify = pth.Phrases.ElementAt(pth.FindPhrase(questionToModify));
			var originalTranslatablePartsSequence = phraseToModify.TranslatableParts.ToList();
			var originalKeyTermsSequence = phraseToModify.KeyTermRenderings;

			pth.Sort(PhrasesSortedBy.EnglishPhrase, true);

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			var origId = phraseToModify.QuestionInfo.Id;
			pth.ModifyQuestion(phraseToModify, "That dog wishes this Paul, and what is say radish?", mp);

			Assert.AreEqual("That dog wishes this Paul, and what is say radish?", phraseToModify.PhraseInUse);
			Assert.AreEqual(origId, phraseToModify.QuestionInfo.Id);
			Assert.AreEqual(phraseToModify, pth[2]);

			Assert.IsTrue(phraseToModify.TranslatableParts.SequenceEqual(originalTranslatablePartsSequence));
			Assert.IsTrue(phraseToModify.KeyTermRenderings.SequenceEqual(originalKeyTermsSequence));
		}

		[Test]
		public void ModifyQuestion_OnlyKeyTermsChanges_PartsSequenceUnchanged()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("fur");
			AddMockedKeyTerm("say", null);
			AddMockedKeyTerm("radish");

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
			AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
			AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
			AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
			var questionToModify = AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "kt:radish");
			AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phraseToModify = pth.Phrases.ElementAt(pth.FindPhrase(questionToModify));
			var originalTranslatablePartsSequence = phraseToModify.TranslatableParts.ToList();

			pth.Sort(PhrasesSortedBy.EnglishPhrase, true);

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			var origId = phraseToModify.QuestionInfo.Id;
			pth.ModifyQuestion(phraseToModify, "that dog fur wishes this have and what is say radishes", mp);

			Assert.AreEqual("that dog fur wishes this have and what is say radishes", phraseToModify.PhraseInUse);
			Assert.AreEqual(origId, phraseToModify.QuestionInfo.Id);
			Assert.AreEqual(phraseToModify, pth[2]);

			Assert.IsTrue(phraseToModify.TranslatableParts.SequenceEqual(originalTranslatablePartsSequence));
			var keyTerms = phraseToModify.KeyTermRenderings;
			Assert.AreEqual(4, keyTerms.Length);
			Assert.AreEqual("FUR", keyTerms[0]);
			Assert.AreEqual("HAVE", keyTerms[1]);
			Assert.AreEqual(String.Empty, keyTerms[2]);
			Assert.AreEqual("RADISH", keyTerms[3]);
		}

		[Test]
		public void ModifyQuestion_KeyTermsAndLastPartUnchanged_PartsSequenceChanged()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("have");
			AddMockedKeyTerm("say", null);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "This would God have me to say with respect to Paul?", "A", 1, 1,
				"this would", "kt:god", "kt:have", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul");
			AddTestQuestion(cat, "What is Paul asking me to say with respect to that dog?", "B", 2, 2,
				"what is" /* 3 */, "kt:paul", "asking", "me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */);
			AddTestQuestion(cat, "that dog", "C", 3, 3, "that dog" /* 4 */);
			AddTestQuestion(cat, "Is it okay for Paul me to talk with respect to God today?", "D", 4, 4,
				"is it okay for", "kt:paul", "me to" /* 3 */, "talk", "with respect to" /* 3 */, "kt:god", "today");
			var questionToModify = AddTestQuestion(cat, "that dog wishes this Paul and what is say radish", "E", 5, 5,
				"that dog" /* 4 */, "wishes this", "kt:paul", "and", "what is" /* 3 */, "kt:say", "radish");
			AddTestQuestion(cat, "What is that dog?", "F", 6, 6, "what is" /* 3 */, "that dog" /* 4 */);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phraseToModify = pth.Phrases.ElementAt(pth.FindPhrase(questionToModify));
			var origId = phraseToModify.QuestionInfo.Id;

			pth.Sort(PhrasesSortedBy.EnglishPhrase, true);

			var mp = new MasterQuestionParser(MasterQuestionParserTests.s_questionWords, KeyTerms, null, null);
			pth.ModifyQuestion(phraseToModify, "What did that dog wish Paul would say to the radish?", mp);

			Assert.AreEqual("What did that dog wish Paul would say to the radish?", phraseToModify.QuestionInfo.Text);
			Assert.AreEqual(origId, phraseToModify.QuestionInfo.Id);
			Assert.AreEqual(phraseToModify, pth[3]);

			var translatableParts = phraseToModify.TranslatableParts.ToList();
			Assert.AreEqual(7, translatableParts.Count);
			int i = 0;
			Assert.AreEqual("what", translatableParts[i++].Text);
			Assert.AreEqual("did", translatableParts[i++].Text);
			Assert.AreEqual("that dog", translatableParts[i++].Text);
			Assert.AreEqual("wish", translatableParts[i++].Text);
			Assert.AreEqual("would", translatableParts[i++].Text);
			Assert.AreEqual("to the", translatableParts[i++].Text);
			Assert.AreEqual("radish", translatableParts[i].Text);
		}
		#endregion

		#region Translation tests
		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation to null for a phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetNewTranslation_Null()
        {
            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            var phrase = pth.GetPhrase("A", "Who was the man?");
            phrase.Translation = null;

            Assert.AreEqual(0, phrase.Translation.Length);
            Assert.IsFalse(phrase.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase when that whole phrase matches part of
        /// another phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetNewTranslation_AutoAcceptTranslationForAllIdenticalPhrases()
        {
            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man");
            Question q1 = AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man" /* 2 */);
            Question q2 = AddTestQuestion(cat, "Where was the woman?", "A", 1, 1, "where was the woman" /* 2 */);
            Question q3 = AddTestQuestion(cat, "Who was the man?", "B", 2, 2, "who was the man" /* 2 */);
            Question q4 = AddTestQuestion(cat, "Where was the woman?", "C", 3, 3, "where was the woman" /* 2 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";
            phrase2.Translation = "\u00BFDo\u0301nde estaba la mujer?";

            Assert.AreEqual(phrase1.Translation, phrase3.Translation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.AreEqual(phrase2.Translation, phrase4.Translation);
            Assert.IsTrue(phrase4.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase when that whole phrase matches part of
        /// another phrase. TXL-108
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetNewTranslation_AcceptTranslationConsistingOnlyOfInitialAndFinalPunctuation()
        {
            AddMockedKeyTerm("Paul", null);
            AddMockedKeyTerm("Judas", null);

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was Paul talking to?", "A", 1, 1, "who was" /* 2 */, "kt:paul", "talking to" /* 2 */);
            Question q2 = AddTestQuestion(cat, "Who was Judas kissing?", "A", 1, 1, "who was" /* 2 */, "kt:judas", "kissing");
            Question q3 = AddTestQuestion(cat, "Why was Judas talking to Paul?", "B", 2, 2, "why was", "kt:judas", "talking to" /* 2*/, "kt:paul");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            phrase1.Translation = "\u00BFCon quie\u0301n estaba hablando Pablo?";
            phrase2.HasUserTranslation = true;

            Assert.AreEqual("\u00BF?", phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.AreEqual(phrase2.Translation, phrase3.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase when that whole phrase matches part of
        /// another phrase, even if it has an untranslated key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetNewTranslation_AutoAcceptTranslationForAllIdenticalPhrases_WithUntranslatedKeyTerm()
        {
            AddMockedKeyTerm("man", null);

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man" /* 2 */);
            Question q2 = AddTestQuestion(cat, "Who was the man?", "B", 2, 2, "who was the man" /* 2 */);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";

            Assert.AreEqual(phrase1.Translation, phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase when that whole phrase matches part of
        /// another phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetNewTranslation_WholePhraseMatchesPartOfAnotherPhrase()
        {
            var cat = m_sections.Items[0].Categories[0];
            Question shortQ = AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man" /* 2 */);
            Question longQ = AddTestQuestion(cat, "Who was the man with the jar?", "B", 2, 2, "who was the man" /* 2 */, "with the jar" /*1*/);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase shortPhrase = pth.GetPhrase(shortQ.ScriptureReference, shortQ.Text);
            TranslatablePhrase longPhrase = pth.GetPhrase(longQ.ScriptureReference, longQ.Text);

            Assert.AreEqual(1, shortPhrase.TranslatableParts.Count());
            Assert.AreEqual(2, longPhrase.TranslatableParts.Count());

            string partTrans = "Quie\u0301n era el hombre";
            shortPhrase.Translation = partTrans + "?";

            Assert.AreEqual((partTrans + "?").Normalize(NormalizationForm.FormC), shortPhrase.Translation);
            Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), shortPhrase[0].Translation);
            Assert.AreEqual((partTrans + "?").Normalize(NormalizationForm.FormC), longPhrase.Translation);
            Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), longPhrase[0].Translation);
            Assert.AreEqual(0, longPhrase[1].Translation.Length);
            Assert.IsTrue(shortPhrase.HasUserTranslation);
            Assert.IsFalse(longPhrase.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase when that whole phrase matches part of
        /// another phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_WholePhraseMatchesPartOfAnotherPhrase()
        {
            var cat = m_sections.Items[0].Categories[0];
            Question shortQ = AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man" /* 2 */);
            Question longQ = AddTestQuestion(cat, "Who was the man with the jar?", "B", 2, 2, "who was the man" /* 2 */, "with the jar" /*1*/);

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase shortPhrase = pth.GetPhrase(shortQ.ScriptureReference, shortQ.Text);
            TranslatablePhrase longPhrase = pth.GetPhrase(longQ.ScriptureReference, longQ.Text);

            Assert.AreEqual(1, shortPhrase.TranslatableParts.Count());
            Assert.AreEqual(2, longPhrase.TranslatableParts.Count());

            shortPhrase.Translation = "Quiem fue el hambre?";
            string partTrans = "Quie\u0301n era el hombre";
            string trans = "\u00BF" + partTrans + "?";
            shortPhrase.Translation = trans;

            Assert.AreEqual(trans.Normalize(NormalizationForm.FormC), shortPhrase.Translation);
            Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), shortPhrase[0].Translation);
            Assert.AreEqual(trans.Normalize(NormalizationForm.FormC), longPhrase.Translation);
            Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), longPhrase[0].Translation);
            Assert.AreEqual(0, longPhrase[1].Translation.Length);
            Assert.IsTrue(shortPhrase.HasUserTranslation);
            Assert.IsFalse(longPhrase.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for two phrases that have a common part and verify
        /// that a third phrase that has that part shows the translation of the translated part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_GuessAtPhraseTranslationBasedOnTriangulation()
        {
            AddMockedKeyTerm("Jesus");
            AddMockedKeyTerm("lion");
            AddMockedKeyTerm("jar");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was the man in the lion's den?", "A", 1, 1, "who was the man" /* 2 */,
                "in the", "kt:lion", "den");
            Question q2 = AddTestQuestion(cat, "Who was the man with the jar?", "B", 2, 2, "who was the man" /* 2 */,
                "with the", "kt:jar");
            Question q3 = AddTestQuestion(cat, "Who was the man Jesus healed?", "C", 3, 3, "who was the man" /* 2 */,
                "kt:jesus", "healed");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            Assert.AreEqual(3, phrase1.TranslatableParts.Count());
            Assert.AreEqual(2, phrase2.TranslatableParts.Count());
            Assert.AreEqual(2, phrase3.TranslatableParts.Count());

            string transPart = "Quie\u0301n era el hombre";
            string transCommon = "\u00BF" + transPart;
            phrase1.Translation = transCommon + " en la fosa de leones?";
            phrase2.Translation = transCommon + " con el jarro?";

            Assert.AreEqual((transCommon + " en la fosa de leones?").Normalize(NormalizationForm.FormC), phrase1.Translation);
            Assert.AreEqual((transCommon + " con el jarro?").Normalize(NormalizationForm.FormC), phrase2.Translation);
            Assert.AreEqual((transCommon + " JESUS?").Normalize(NormalizationForm.FormC), phrase3.Translation);
            Assert.AreEqual(transPart.Normalize(NormalizationForm.FormC), phrase3[0].Translation);
            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsFalse(phrase3.HasUserTranslation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests setting the translation for two phrases that have a common part and verify
		/// that a third phrase that has that part shows the translation of the translated part.
		/// In this test, there is a long partial-word match for a pair of sentences, but the
		/// partial word also contains a short word that happens to be a match for a different
		/// part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_PreventShortProvisionalTranslationFromObscuringLongerMatchForAnotherPart()
		{
			AddMockedKeyTerm("Paul", "Pablo");
			AddMockedKeyTerm("power","poder");
			AddMockedKeyTerm("Lord", "Senor");
			AddMockedKeyTerm("God", "Dios");
			AddMockedKeyTerm("favor", "favor");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "What had Paul described to them?", "A", 1, 1, "what",
				"had", "kt:paul", "described", "to them");
			Question q2 = AddTestQuestion(cat, "How is the power of the Lord described?", "B", 2, 2, "how",
				"is", "the", "kt:power", "of", "the", "kt:lord", "described");
			Question q3 = AddTestQuestion(cat, "How is God described?", "C", 3, 3, "how",
				"is", "kt:god", "described");
			Question q4 = AddTestQuestion(cat, "of", "D", 4, 4, "of");
			Question q5 = AddTestQuestion(cat, "Where is the Lord?", "E", 5, 5, "where", "is the", "kt:Lord");
			Question q6 = AddTestQuestion(cat, "How can man obtain the favor of God?", "F", 6, 6, "how",
				"can man obtain", "the", "kt:favor", "of", "kt:god");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
			TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
			TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
			TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);
			TranslatablePhrase phrase6 = pth.GetPhrase(q6.ScriptureReference, q6.Text);

			phrase4.Translation = "de";
			phrase5.Translation = "¿Do\u0301nde esta\u0301 el Senor?";
			phrase6.Translation = "¿Co\u0301mo puede el hombre obtener el favor de Dios?";
			phrase2.Translation = "¿Co\u0301mo se describe el poder del Senor?";
			phrase1.Translation = "¿Que\u0301 les habi\u0301a descrito Pablo?";

			Assert.AreEqual("¿Co\u0301mo Dios descri?".Normalize(NormalizationForm.FormC), phrase3.Translation);
			Assert.IsFalse(phrase3.HasUserTranslation);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for two phrases that have a common part and verify
        /// that a third phrase that has that part shows the translation of the translated part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_FindKeyTermRenderingWhenKtHasMultiplesTranslations()
        {
            AddMockedKeyTerm("arrow", "flecha");
            AddMockedKeyTerm("arrow", "dardo");
            AddMockedKeyTerm("arrow", "dardos");
            AddMockedKeyTerm("lion", "noil", "leo\u0301n", "noil");
            AddMockedKeyTerm("boat", "nave");
            AddMockedKeyTerm("boat", "barco");
            AddMockedKeyTerm("boat", "barca");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "I shot the lion with the arrow.", "A", 1, 1, "i shot the", "kt:lion", "with the", "kt:arrow");
            Question q2 = AddTestQuestion(cat, "Who put the lion in the boat?", "A", 1, 1, "who put the", "kt:lion", "in the", "kt:boat");
            Question q3 = AddTestQuestion(cat, "Does the boat belong to the boat?", "A", 1, 1, "does the", "kt:boat", "belong to the", "kt:boat");
            Question q4 = AddTestQuestion(cat, "I shot the boat with the arrow.", "A", 1, 1, "i shot the", "kt:boat", "with the", "kt:arrow");
            Question q5 = AddTestQuestion(cat, "Who put the arrow in the boat?", "A", 1, 1, "who put the", "kt:arrow", "in the", "kt:boat");
            Question q6 = AddTestQuestion(cat, "Who put the arrow in the lion?", "A", 1, 1, "who put the", "kt:arrow", "in the", "kt:lion");
            Question q7 = AddTestQuestion(cat, "I shot the arrow with the lion.", "A", 1, 1, "i shot the", "kt:arrow", "with the", "kt:lion");
            Question q8 = AddTestQuestion(cat, "Does the arrow belong to the lion?", "A", 1, 1, "does the", "kt:arrow", "belong to the", "kt:lion");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);
            TranslatablePhrase phrase6 = pth.GetPhrase(q6.ScriptureReference, q6.Text);
            TranslatablePhrase phrase7 = pth.GetPhrase(q7.ScriptureReference, q7.Text);
            TranslatablePhrase phrase8 = pth.GetPhrase(q8.ScriptureReference, q8.Text);

            foreach (TranslatablePhrase phrase in pth.Phrases)
            {
                Assert.AreEqual(4, phrase.GetParts().Count(), "Wrong number of parts for phrase: " + phrase.OriginalPhrase);
                Assert.AreEqual(2, phrase.TranslatableParts.Count(), "Wrong number of translatable parts for phrase: " + phrase.OriginalPhrase);
            }

            phrase1.Translation = "Yo le pegue\u0301 un tiro al noil con un dardo.";
            Assert.AreEqual("Yo le pegue\u0301 un tiro al barca con un dardos.".Normalize(NormalizationForm.FormC),
                phrase4.Translation);
            Assert.AreEqual("Yo le pegue\u0301 un tiro al dardos con un leo\u0301n.".Normalize(NormalizationForm.FormC),
                phrase7.Translation);

            phrase2.Translation = "\u00BFQuie\u0301n puso el leo\u0301n en la barca?";
            Assert.AreEqual("\u00BFQuie\u0301n puso el dardos en la barca?".Normalize(NormalizationForm.FormC),
                phrase5.Translation);
            Assert.AreEqual("\u00BFQuie\u0301n puso el dardos en la leo\u0301n?".Normalize(NormalizationForm.FormC),
                phrase6.Translation);

            phrase3.Translation = "\u00BFEl boat le pertenece al barco?";
            // This is a bizarre special case where the original question has the same key term twice and
            // the user has translated it differently. Internal details of the logic (specifically, it finds
            // the longer rendering first) dictate the order in which the key terms are considered to have
            // been found. For the purposes of this test case, we don't care in which order the terms of the
            // untranslated question get substituted.
            Assert.IsTrue("\u00BFEl leo\u0301n le pertenece al dardos?".Normalize(NormalizationForm.FormC) == phrase8.Translation ||
                "\u00BFEl dardos le pertenece al leo\u0301n?".Normalize(NormalizationForm.FormC) == phrase8.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsFalse(phrase4.HasUserTranslation);
            Assert.IsFalse(phrase5.HasUserTranslation);
            Assert.IsFalse(phrase6.HasUserTranslation);
            Assert.IsFalse(phrase7.HasUserTranslation);
            Assert.IsFalse(phrase8.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for one phrase and then repeatedly accepting and
        /// un-accepting the generated translation for the other phrase that differs only by
        /// key terms. (TXL-51)
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_PreventAddedSpacesWhenAcceptingGeneratedTranslation()
        {
            AddMockedKeyTerm("Jesus", "Jesu\u0301s");
            AddMockedKeyTerm("Phillip", "Felipe");
            AddMockedKeyTerm("Matthew", "Mateo");

            var cat = m_sections.Items[0].Categories[0];
            Question q0 = AddTestQuestion(cat, "What asked Jesus Matthew?", "A", 1, 1, "what asked", "kt:jesus", "kt:matthew");
            Question q1 = AddTestQuestion(cat, "What asked Jesus Phillip?", "A", 1, 1, "what asked", "kt:jesus", "kt:phillip");
            Question q2 = AddTestQuestion(cat, "What asked Phillip Matthew?", "A", 1, 1, "what asked", "kt:phillip", "kt:matthew");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase0 = pth.GetPhrase(q0.ScriptureReference, q0.Text);
            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            phrase0.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s Mateo?";
            phrase1.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s a Felipe?";
            string expectedTranslation = "\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC);
            Assert.AreEqual(expectedTranslation, phrase2.Translation);
            Assert.IsFalse(phrase2.HasUserTranslation);

            phrase2.HasUserTranslation = true;
            Assert.AreEqual(expectedTranslation, phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);

            expectedTranslation = "\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC);
            phrase2.HasUserTranslation = false;
            Assert.AreEqual(expectedTranslation, phrase2.Translation);
            Assert.IsFalse(phrase2.HasUserTranslation);

            phrase2.HasUserTranslation = true;
            Assert.AreEqual(expectedTranslation, phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that unconfirming a user translation will go back to a template-based
        /// translation if one is available. Specifically, it will revert to the template of the
        /// first translated question it finds.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_ClearingUserTranslationFlagRestoresTemplateBasedTranslation()
        {
            AddMockedKeyTerm("Jesus", "Jesu\u0301s");
            AddMockedKeyTerm("Phillip", "Felipe");
            AddMockedKeyTerm("Matthew", "Mateo");

            var cat = m_sections.Items[0].Categories[0];
            Question q0 = AddTestQuestion(cat, "What asked Jesus Matthew?", "A", 1, 1, "what asked", "kt:jesus", "kt:matthew");
            Question q1 = AddTestQuestion(cat, "What asked Jesus Phillip?", "A", 1, 1, "what asked", "kt:jesus", "kt:phillip");
            Question q2 = AddTestQuestion(cat, "What asked Phillip Matthew?", "A", 1, 1, "what asked", "kt:phillip", "kt:matthew");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase0 = pth.GetPhrase(q0.ScriptureReference, q0.Text);
            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            phrase0.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s a Mateo?";
            phrase1.Translation = "\u00BFQue\u0301 le pregunto\u0301 Jesu\u0301s Felipe?";
            Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC),
                phrase2.Translation);
            Assert.IsFalse(phrase2.HasUserTranslation);

            phrase2.HasUserTranslation = true;
            Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe Mateo?".Normalize(NormalizationForm.FormC),
                phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);

            phrase2.HasUserTranslation = false;
            Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC),
                phrase2.Translation);
            Assert.IsFalse(phrase2.HasUserTranslation);

            phrase2.HasUserTranslation = true;
            Assert.AreEqual("\u00BFQue\u0301 le pregunto\u0301 Felipe a Mateo?".Normalize(NormalizationForm.FormC),
                phrase2.Translation);
            Assert.IsTrue(phrase2.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for two phrases that have a common part and verify
        /// that a third phrase that has that part shows the translation of the translated part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_GuessAtPhraseTranslationBasedOnTriangulation()
        {
            AddMockedKeyTerm("Jesus");
            AddMockedKeyTerm("lion");
            AddMockedKeyTerm("jar");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was the man in the lions' den?", "A", 1, 1, "who was the man", "in the", "kt:lions", "den");
            Question q2 = AddTestQuestion(cat, "Who was the man with the jar?", "A", 1, 1, "who was the man", "with the", "kt:jar");
            Question q3 = AddTestQuestion(cat, "Who was the man Jesus healed?", "A", 1, 1, "who was the man", "kt:jesus", " healed");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            Assert.AreEqual(3, phrase1.TranslatableParts.Count());
            Assert.AreEqual(2, phrase2.TranslatableParts.Count());
            Assert.AreEqual(2, phrase3.TranslatableParts.Count());

            string partTrans = "Quie\u0301n era el hombre";
            string transCommon = "\u00BF" + partTrans;
            phrase1.Translation = "Quien fue lo hambre en la fosa de leones?";
            phrase2.Translation = transCommon + " con el jarro?";
            Assert.AreEqual("\u00BFmbre JESUS?", phrase3.Translation);
            Assert.AreEqual("mbre", phrase3[0].Translation);

            phrase1.Translation = transCommon + " en la fosa de leones?";

            Assert.AreEqual((transCommon + " en la fosa de leones?").Normalize(NormalizationForm.FormC), phrase1.Translation);
            Assert.AreEqual((transCommon + " con el jarro?").Normalize(NormalizationForm.FormC), phrase2.Translation);
            Assert.AreEqual((transCommon + " JESUS?").Normalize(NormalizationForm.FormC), phrase3.Translation);
            Assert.AreEqual(partTrans.Normalize(NormalizationForm.FormC), phrase3[0].Translation);
            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsFalse(phrase3.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase with only one translatable part when
        /// another phrase differs only by a key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_GuessAtOnePartPhraseThatDiffersBySingleKeyTerm()
        {
            AddMockedKeyTerm("Timothy", "Timoteo");
            AddMockedKeyTerm("Euticus", "Eutico");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was Timothy?", "A", 1, 1, "who was", "kt:timothy");
            Question q2 = AddTestQuestion(cat, "Who was Euticus?", "A", 1, 1, "who was", "kt:euticus");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            Assert.AreEqual(1, phrase1.TranslatableParts.Count());
            Assert.AreEqual(2, phrase1.GetParts().Count());
            Assert.AreEqual(1, phrase2.TranslatableParts.Count());
            Assert.AreEqual(2, phrase2.GetParts().Count());

            const string frame = "\u00BFQuie\u0301n era {0}?";
            phrase1.Translation = string.Format(frame, "Timoteo");

            Assert.AreEqual(string.Format(frame, "Timoteo").Normalize(NormalizationForm.FormC), phrase1.Translation);
            Assert.AreEqual(string.Format(frame, "Eutico").Normalize(NormalizationForm.FormC), phrase2.Translation);
            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsFalse(phrase2.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for two phrases that have a common part and verify
        /// that a third phrase that has that part shows the translation of the translated part.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_GuessAtTwoPartPhraseThatDiffersBySingleKeyTerm()
        {
            AddMockedKeyTerm("Jacob", "Jacobo");
            AddMockedKeyTerm("Matthew", "Mateo");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Was Jacob one of the disciples?", "A", 1, 1, "was", "kt:jacob", "one of the disciples");
            Question q2 = AddTestQuestion(cat, "Was Matthew one of the disciples?", "A", 1, 1, "was", "kt:matthew", " one of the disciples");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            Assert.AreEqual(2, phrase1.TranslatableParts.Count());
            Assert.AreEqual(3, phrase1.GetParts().Count());
            Assert.AreEqual(2, phrase2.TranslatableParts.Count());
            Assert.AreEqual(3, phrase2.GetParts().Count());

            const string frame = "\u00BFFue {0} uno de los discipulos?";
            phrase1.Translation = string.Format(frame, "Jacobo");

            Assert.AreEqual(string.Format(frame, "Jacobo"), phrase1.Translation);
            Assert.AreEqual(string.Format(frame, "Mateo"), phrase2.Translation);
            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsFalse(phrase2.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for multiple phrases. Possible part translations
        /// should be assigned to parts according to length and numbers of occurrences, but no
        /// portion of a translation should be used as the translation for two parts of the same
        /// owning phrase
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_PreventTranslationFromBeingUsedForMultipleParts()
        {
            AddMockedKeyTerm("Jacob", "Jacob");
            AddMockedKeyTerm("John", "Juan");
            AddMockedKeyTerm("Jesus", "Jesu\u0301s");
            AddMockedKeyTerm("Mary", "Mari\u0301a");
            AddMockedKeyTerm("Moses", "Moise\u0301s");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "So what did Jacob do?", "A", 1, 1, "so what did", "kt:jacob", "do");
            Question q2 = AddTestQuestion(cat, "So what did Jesus do?", "A", 1, 1, "so what did", "kt:jesus", " do");
            Question q3 = AddTestQuestion(cat, "What did Jacob do?", "A", 1, 1, "what did", "kt:jacob", "do");
            Question q4 = AddTestQuestion(cat, "What did Moses ask?", "A", 1, 1, "what did", "kt:moses", "ask");
            Question q5 = AddTestQuestion(cat, "So what did John ask?", "A", 1, 1, "so what did", "kt:john", "ask");
            Question q6 = AddTestQuestion(cat, "So what did Mary want?", "A", 1, 1, "so what did", "kt:mary", "want");
            Question q7 = AddTestQuestion(cat, "What did Moses do?", "A", 1, 1, "what did", "kt:moses", "do");
            Question q8 = AddTestQuestion(cat, "Did Moses ask, \"What did Jacob do?\"", "a", 1, 1, "did", "kt:moses", "ask", "what did", "kt:jacob", "do");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);
            TranslatablePhrase phrase6 = pth.GetPhrase(q6.ScriptureReference, q6.Text);
            TranslatablePhrase phrase7 = pth.GetPhrase(q7.ScriptureReference, q7.Text);
            TranslatablePhrase phrase8 = pth.GetPhrase(q8.ScriptureReference, q8.Text);

            Assert.AreEqual(2, phrase1.TranslatableParts.Count());
            Assert.AreEqual(3, phrase1.GetParts().Count());
            Assert.AreEqual(2, phrase2.TranslatableParts.Count());
            Assert.AreEqual(3, phrase2.GetParts().Count());
            Assert.AreEqual(2, phrase3.TranslatableParts.Count());
            Assert.AreEqual(3, phrase3.GetParts().Count());
            Assert.AreEqual(2, phrase4.TranslatableParts.Count());
            Assert.AreEqual(3, phrase4.GetParts().Count());
            Assert.AreEqual(2, phrase5.TranslatableParts.Count());
            Assert.AreEqual(3, phrase5.GetParts().Count());
            Assert.AreEqual(2, phrase6.TranslatableParts.Count());
            Assert.AreEqual(3, phrase6.GetParts().Count());
            Assert.AreEqual(2, phrase7.TranslatableParts.Count());
            Assert.AreEqual(3, phrase7.GetParts().Count());
            Assert.AreEqual(4, phrase8.TranslatableParts.Count());
            Assert.AreEqual(6, phrase8.GetParts().Count());

            phrase1.Translation = "\u00BFEntonces que\u0301 hizo Jacob?";
            phrase2.Translation = "\u00BFEntonces que\u0301 hizo Jesu\u0301s?";
            phrase3.Translation = "\u00BFQue\u0301 hizo Jacob?";
            phrase4.Translation = "\u00BFQue\u0301 pregunto\u0301 Moise\u0301s?";
            phrase5.Translation = "\u00BFEntonces que\u0301 pregunto\u0301 Juan?";

            Assert.AreEqual("\u00BFEntonces que\u0301 Mari\u0301a?".Normalize(NormalizationForm.FormC), phrase6.Translation);
            Assert.AreEqual("\u00BFQue\u0301 hizo Moise\u0301s?".Normalize(NormalizationForm.FormC), phrase7.Translation);
            Assert.AreEqual("Moise\u0301s pregunto\u0301 Que\u0301 Jacob hizo".Normalize(NormalizationForm.FormC), phrase8.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for multiple phrases. Possible part translations
        /// should be assigned to parts according to length and numbers of occurrences, but no
        /// portion of a translation should be used as the translation for two parts of the same
        /// owning phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_UseStatsAndConfidenceToDeterminePartTranslations()
        {
            AddMockedKeyTerm("ask");
            AddMockedKeyTerm("give");
            AddMockedKeyTerm("want");
            AddMockedKeyTerm("whatever");
            AddMockedKeyTerm("thing");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "ABC ask DEF", "A", 1, 1, "abc", "kt:ask", "def");
            Question q2 = AddTestQuestion(cat, "ABC give XYZ", "A", 1, 1, "abc", "kt:give", "xyz");
            Question q3 = AddTestQuestion(cat, "XYZ want ABC whatever EFG", "a", 1, 1, "xyz", "kt:want", "abc", "kt:whatever", "efg");
            Question q4 = AddTestQuestion(cat, "EFG thing ABC", "A", 1, 1, "efg", "kt:thing", "abc");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            Assert.AreEqual(2, phrase1.TranslatableParts.Count());
            Assert.AreEqual(3, phrase1.GetParts().Count());
            Assert.AreEqual(2, phrase2.TranslatableParts.Count());
            Assert.AreEqual(3, phrase2.GetParts().Count());
            Assert.AreEqual(3, phrase3.TranslatableParts.Count());
            Assert.AreEqual(5, phrase3.GetParts().Count());
            Assert.AreEqual(2, phrase4.TranslatableParts.Count());
            Assert.AreEqual(3, phrase4.GetParts().Count());

            phrase1.Translation = "def ASK abc";
            phrase2.Translation = "abc xyz GIVE";
            phrase3.Translation = "WANT xyz abc WHATEVER efg";

            Assert.AreEqual("efg THING abc", phrase4.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the code to determine the best translation for a part of a phrase will
        /// not take a substring common to all phrases if it would mean selecting less than a
        /// whole word instead of a statistically viable substring that consists of whole
        /// words.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_UseStatisticalBestPartTranslations()
        {
            AddMockedKeyTerm("Isaac", "Isaac");
            AddMockedKeyTerm("Paul", "Pablo");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "Now what?", "A", 1, 1, "now what");
            AddTestQuestion(cat, "What did Isaac say?", "A", 1, 1, "what did", "kt:isaac", "say");
            Question q1 = AddTestQuestion(cat, "So now what did those two brothers do?", "A", 1, 1, "so", "now what", "did those two brothers do");
            Question q2 = AddTestQuestion(cat, "So what did they do about the problem?", "A", 1, 1, "so", "what did", "they do about the problem");
            Question q3 = AddTestQuestion(cat, "So what did he do?", "A", 1, 1, "so", "what did", "he do");
            Question q4 = AddTestQuestion(cat, "So now what was Isaac complaining about?", "A", 1, 1, "so", "now what", "was", "kt:isaac", "complaining about");
            Question q5 = AddTestQuestion(cat, "So what did the Apostle Paul say about that?", "A", 1, 1, "so", "what did", "the apostle", "kt:paul", "say about that");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);

            Assert.AreEqual(3, phrase1.TranslatableParts.Count());
            Assert.AreEqual(3, phrase1.GetParts().Count());
            Assert.AreEqual(3, phrase2.TranslatableParts.Count());
            Assert.AreEqual(3, phrase2.GetParts().Count());
            Assert.AreEqual(3, phrase3.TranslatableParts.Count());
            Assert.AreEqual(3, phrase3.GetParts().Count());
            Assert.AreEqual(4, phrase4.TranslatableParts.Count());
            Assert.AreEqual(5, phrase4.GetParts().Count());
            Assert.AreEqual(4, phrase5.TranslatableParts.Count());
            Assert.AreEqual(5, phrase5.GetParts().Count());

            phrase1.Translation = "\u00BFEntonces ahora que\u0301 hicieron esos dos hermanos?";
            phrase2.Translation = "\u00BFEntonces que\u0301 hicieron acerca del problema?";
            phrase3.Translation = "\u00BFEntonces que\u0301 hizo?";
            phrase5.Translation = "\u00BFEntonces que\u0301 dijo Pablo acerca de eso?";

            Assert.AreEqual("\u00BFEntonces Isaac?", phrase4.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the code to determine the best translation for a part of a phrase will
        /// not take a substring common to all phrases if it would mean selecting less than a
        /// whole word instead of a statistically viable substring that consists of whole
        /// words.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SetTranslation_UseStatisticalBestPartTranslationsRatherThanCommonPartialWord()
        {
            AddMockedKeyTerm("Isaac", "Isaac");
            AddMockedKeyTerm("Paul", "Pablo");

            var cat = m_sections.Items[0].Categories[0];
            AddTestQuestion(cat, "Now what?", "A", 1, 1, "now what");
            AddTestQuestion(cat, "What did Isaac say?", "A", 1, 1, "what did", "kt:isaac", "say");
            AddTestQuestion(cat, "What could Isaac say?", "A", 1, 1, "what could", "kt:isaac", "say");
            Question q1 = AddTestQuestion(cat, "So now what did those two brothers do?", "A", 1, 1, "so", "now what", "did those two brothers do");
            Question q2 = AddTestQuestion(cat, "So what could they do about the problem?", "A", 1, 1, "so", "what could", "they do about the problem");
            Question q3 = AddTestQuestion(cat, "So what did he do?", "A", 1, 1, "so", "what did", "he do");
            Question q4 = AddTestQuestion(cat, "So now what was Isaac complaining about?", "A", 1, 1, "so", "now what", "was", "kt:isaac", "complaining about");
            Question q5 = AddTestQuestion(cat, "So what did the Apostle Paul say about that?", "A", 1, 1, "so", "what did", "the apostle", "kt:paul", "say about that");
            Question q6 = AddTestQuestion(cat, "Why did they treat the Apostle Paul so?", "A", 1, 1, "why did they treat the apostle", "kt:paul", "so");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);
            TranslatablePhrase phrase6 = pth.GetPhrase(q6.ScriptureReference, q6.Text);

            Assert.AreEqual(3, phrase1.TranslatableParts.Count());
            Assert.AreEqual(3, phrase1.GetParts().Count());
            Assert.AreEqual(3, phrase2.TranslatableParts.Count());
            Assert.AreEqual(3, phrase2.GetParts().Count());
            Assert.AreEqual(3, phrase3.TranslatableParts.Count());
            Assert.AreEqual(3, phrase3.GetParts().Count());
            Assert.AreEqual(4, phrase4.TranslatableParts.Count());
            Assert.AreEqual(5, phrase4.GetParts().Count());
            Assert.AreEqual(4, phrase5.TranslatableParts.Count());
            Assert.AreEqual(5, phrase5.GetParts().Count());
            Assert.AreEqual(2, phrase6.TranslatableParts.Count());
            Assert.AreEqual(3, phrase6.GetParts().Count());

            phrase1.Translation = "Entonces AB Zuxelopitmyfor CD EF GH";
            phrase2.Translation = "Entonces Vuxelopitmyfor IJ KL MN OP QR";
            phrase3.Translation = "Entonces Wuxelopitmyfor ST";
            phrase5.Translation = "Entonces Xuxelopitmyfor dijo Pablo UV WX YZ";
            phrase6.Translation = "BG LP Yuxelopitmyfor DW MR Pablo";

            Assert.AreEqual("Entonces Isaac", phrase4.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests setting the translation for a group of phrases such that the only common
		/// character for a part they have in common is a punctuation character.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_DoNotTreatNormalLeadingPuncAsOpeningQuestionMark()
		{
			AddMockedKeyTerm("Isaiah", "Isai\u0301as");
			AddMockedKeyTerm("Paul", "Pablo");
			AddMockedKeyTerm("Silas", "Silas");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "What did Paul and Silas do in jail?", "A", 1, 1, "what", "did", "kt:paul", "and", "kt:silas", "do in jail");
			Question q2 = AddTestQuestion(cat, "Were Isaiah and Paul prophets?", "A", 1, 1, "were", "kt:isaiah", "and", "kt:paul", "prophets");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

			phrase1.Translation = "*\u00BFQue\u0301 hicieron Pablo y Silas en la carcel?";
			Assert.AreEqual("Isai\u0301as Pablo?".Normalize(NormalizationForm.FormC), phrase2.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests setting the translation for a group of phrases such that the only common
		/// character for a part they have in common is a punctuation character.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[Ignore("Maybe this is okay, after all.")]
		public void SetTranslation_DoNotUseNumbersAsGuessedTranslations()
		{
			AddMockedKeyTerm("Jesus", "Jesu\u0301s");
			AddMockedKeyTerm("Paul", "Pablo");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "Was Paul one of Jesus' twelve disciples?", "A", 1, 1, "was", "kt:paul", "one", "of", "kt:jesus", "twelve disciples");
			Question q2 = AddTestQuestion(cat, "How far did the twelve disciples of Jesus go?", "A", 1, 1, "how", " far did the", "twelve disciples", "of", "kt:jesus", "go");
			Question q3 = AddTestQuestion(cat, "Tell one way Paul helped Jesus.", "A", 1, 1, "tell", "one", "way", "kt:paul", "helped", "kt:jesus");
			Question q4 = AddTestQuestion(cat, "Were the twelve disciples of Jesus in one room?", "A", 1, 1, "were", "the", "twelve disciples", "of", "kt:jesus", "in", "one", "room");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
			TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
			TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

			phrase1.Translation = "\u00BFEra Pablo 1 de los 12 disci\u0301pulos de Jesu\u0301s?";
			phrase2.Translation = "\u00BFCuanta distancia caminaron los 12 disci\u0301pulos de Jesu\u0301s?";
			phrase3.Translation = "Cuenta 1 manera en que Pablo le ayudo\u0301 a Jesu\u0301s?";
			Assert.AreEqual("\u00BFdisci\u0301pulos de Jesu\u0301s?".Normalize(NormalizationForm.FormC), phrase4.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests setting the translation for a phrase with a pattern of parts, terms and
		/// numbers that matches another phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_GuessAtTranslationForSimilarPartWithNumbers()
		{
			AddMockedKeyTerm("Benjamin", "Benjami\u0301n");
			AddMockedKeyTerm("Judah", "Juda\u0301");
			AddMockedKeyTerm("talent", "talento");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "The 4000 men of Benjamin donated 2000 talents of gold.", "A", 1, 1, "the", 4000,
				"men of", "kt:benjamin", "donated", 2000, "kt:talent", "of gold");
			Question q2 = AddTestQuestion(cat, "The 2000 men of Judah donated 1000 talents of gold.", "A", 1, 1, "the", 2000,
				"men of", "kt:judah", "donated", 1000, "kt:talent", "of gold");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

			phrase1.Translation = "De los hombres de Benjami\u0301n, 2 000 talentos de oro, donado por sus 4 000 hombres.";
			Assert.AreEqual("De los hombres de Juda\u0301, 1 000 talentos de oro, donado por sus 2 000 hombres.".Normalize(NormalizationForm.FormC),
				phrase2.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests setting the translation for a phrase with a pattern of parts, terms and
		/// numbers that matches another phrase. In this case the user-supplied translation
		/// has an error in that it uses the incorrect number, so the guess translation is
		/// only based on the parts and not the (bogus) template.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_IncorrectNumberPreventsTemplateBasedGuessing()
		{
			AddMockedKeyTerm("Benjamin", "Benjami\u0301n");
			AddMockedKeyTerm("Judah", "Juda\u0301");
			AddMockedKeyTerm("talent", "talento");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "The 4000 men of Benjamin donated 2000 talents of gold.", "A", 1, 1, "the", 4000,
				"men of", "kt:benjamin", "donated", 2000, "kt:talent", "of gold");
			Question q2 = AddTestQuestion(cat, "The 2000 men of Judah donated 1000 talents of gold.", "A", 1, 1, "the", 2000,
				"men of", "kt:judah", "donated", 1000, "kt:talent", "of gold");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

			phrase1.Translation = "Los 40.000 hombres de Benjami\u0301n donaron 2.000 talentos de oro.";
			Assert.AreEqual("2.000 Juda\u0301 1.000 talento.".Normalize(NormalizationForm.FormC),
				phrase2.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the guessed translation for a phrases that have numerals include those
		/// numerals.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_NoUserTranslations_GuessedTranslationsIncludeNumbers()
		{
			//This just sets the statics in the Number class to a known setting for a predictable test result. 
			Number n = new Number(200000);
			n.Translation = "200,000";

			AddMockedKeyTerm("Jesus", "Jesu\u0301s");
			AddMockedKeyTerm("Paul", "Pablo");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "Was Paul one of Jesus' 12 disciples?", "A", 1, 1, "was", "kt:paul", "one", "of", "kt:jesus", 12, "disciples");
			Question q2 = AddTestQuestion(cat, "Who sealed the 144,000?", "A", 1, 1, "who", " sealed the", 144000);

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

			Assert.AreEqual("Pablo Jesu\u0301s 12".Normalize(NormalizationForm.FormC), phrase1.Translation);
			Assert.AreEqual("144,000".Normalize(NormalizationForm.FormC), phrase2.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the guessed translation for a phrases that have numerals include those
		/// numerals, formatted according to the pattern suggested by existing user
		/// translation(s).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void
			SetTranslation_UserTranslationsIncludeNumbers_GuessedTranslationsFormatNumbersBasedOnFromattingInUserTranslations()
		{
			AddMockedKeyTerm("Jesus", "Jesu\u0301s");
			AddMockedKeyTerm("Paul", "Pablo");
			AddMockedKeyTerm("Jew", "judio");
			AddMockedKeyTerm("Gentile", "gentil");
			AddMockedKeyTerm("Egypt", "Egipto");
			AddMockedKeyTerm("David", "David");
			AddMockedKeyTerm("Canaan", "Canaa\u0301n");
			AddMockedKeyTerm("Gideon", "Gedeo\u0301n");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "Were the 3000 new church members Jews or Gentiles?", "A", 1, 1, "were the",
				3000, "new church members", "kt:jew", "or", "kt:gentile");
			Question q2 = AddTestQuestion(cat, "Who sealed the 144,000?", "A", 1, 1, "who", " sealed the", 144000);
			Question q3 = AddTestQuestion(cat, "Did David really slay more than 10,000 men?", "A", 1, 1, "did", "kt:david",
				"really slay more than", 10000, "men");
			Question q4 = AddTestQuestion(cat, "Of the 657,000 Jews who left Egypt, how many entered Canaan?", "A", 1, 1,
				"of the", 657000, "kt:jew", "who left", "kt:egypt", "how many", "entered", "kt:canaan");
			Question q5 = AddTestQuestion(cat, "Was Paul one of Jesus' 12 disciples?", "A", 1, 1, "was", "kt:paul",
				"one", "of", "kt:jesus", 12, "disciples");
			Question q6 = AddTestQuestion(cat, "Was Gideon happy to have only 300 soldiers left?", "A", 1, 1, "was", "kt:gideon",
				"happy to have only", 300, "soldiers left");
			Question q7 = AddTestQuestion(cat, "Should 3,500 have a comma?", "A", 1, 1, "should", 3500, "have a comma");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
			TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
			TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
			TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);
			TranslatablePhrase phrase6 = pth.GetPhrase(q6.ScriptureReference, q6.Text);
			TranslatablePhrase phrase7 = pth.GetPhrase(q7.ScriptureReference, q7.Text);

			phrase2.Translation = "\u00BFQuie\u0301n sello\u0301 a los 1.44.000?";

			Assert.AreEqual("\u00BF3.000 judio gentil?".Normalize(NormalizationForm.FormC), phrase1.Translation);
			Assert.AreEqual("\u00BFDavid 10.000?".Normalize(NormalizationForm.FormC), phrase3.Translation);
			Assert.AreEqual("\u00BF6.57.000 judio Egipto Canaa\u0301n?".Normalize(NormalizationForm.FormC), phrase4.Translation);
			Assert.AreEqual("\u00BFPablo Jesu\u0301s 12?".Normalize(NormalizationForm.FormC), phrase5.Translation);
			Assert.AreEqual("\u00BFGedeo\u0301n 300?".Normalize(NormalizationForm.FormC), phrase6.Translation);
			Assert.AreEqual("\u00BF3.500?".Normalize(NormalizationForm.FormC), phrase7.Translation);

			phrase1.Translation = "\u00BF3000 judio gentil?";
			Assert.AreEqual("\u00BF3500?".Normalize(NormalizationForm.FormC), phrase7.Translation);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the guessed translation for a phrases that have numerals include those
		/// numerals using the range of script (non Arabic-Indic) digits suggested by existing
		/// user translation(s).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslation_UserTranslationsIncludeScriptDigit_GuessedTranslationsUseCorrectRangeOfScriptDigits()
		{
			//Number n = new Number(200000);
			//n.Translation
			AddMockedKeyTerm("Jew", "judio");
			AddMockedKeyTerm("Egypt", "Egipto");
			AddMockedKeyTerm("Canaan", "Canaa\u0301n");

			var cat = m_sections.Items[0].Categories[0];
			Question q1 = AddTestQuestion(cat, "Of the 657,000 Jews who left Egypt, how many entered Canaan?", "A", 1, 1,
				"of the", 657000, "kt:jew", "who left", "kt:egypt", "how many", "entered", "kt:canaan");
			Question q2 = AddTestQuestion(cat, "Who sealed the 144,000?", "A", 1, 1, "who", " sealed the", 144000);


			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
			TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

			phrase2.Translation = "\u00BFQuie\u0301n sello\u0301 a los \u0967\u096A\u096A\u060C\u0966\u0966\u0966?";

			Assert.AreEqual("\u00BF\u096C\u096B\u096D\u060C\u0966\u0966\u0966 judio Egipto Canaa\u0301n?".Normalize(NormalizationForm.FormC), phrase1.Translation);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a group of phrases such that the only common
        /// character for a part they have in common is a punctuation character.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_PreventPartTranslationFromBeingSetToPunctuation()
        {
            AddMockedKeyTerm("Isaiah", "Isai\u0301as");
            AddMockedKeyTerm("Paul", "Pablo");
            AddMockedKeyTerm("Silas", "Silas");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "What did Paul and Silas do in jail?", "A", 1, 1, "what did", "kt:paul", "and", "kt:silas", "do in jail");
            Question q2 = AddTestQuestion(cat, "Were Isaiah and Paul prophets?", "A", 1, 1, "were", "kt:isaiah", "and", "kt:paul", "prophets");
            Question q3 = AddTestQuestion(cat, "Did Paul and Silas run away?", "A", 1, 1, "did", "kt:paul", "and", "kt:silas", "run away");
            Question q4 = AddTestQuestion(cat, "And what did Paul do next?", "A", 1, 1, "and", "what did", "kt:paul", "do next");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            phrase1.Translation = "\u00BFQue\u0301 hicieron Pablo y Silas en la carcel?";
            phrase2.Translation = "\u00BFEran profetas Pablo e Isai\u0301as?";
            phrase3.Translation = "\u00BFSe corrieron Pablo y Silas?";
            Assert.AreEqual("\u00BFy Pablo?", phrase4.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase that has parts that are also in another
        /// phrase that does not have a user translation but does have parts that do have a
        /// translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_PreventTrashingPartTranslationsWhenReCalculating()
        {
            AddMockedKeyTerm("Mary", "Mari\u0301a");
            AddMockedKeyTerm("Jesus");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "When?", "A", 1, 1, "when");
            Question q2 = AddTestQuestion(cat, "Where did Mary find Jesus?", "A", 1, 1, "where did", "kt:mary", "find", "kt:jesus");
            Question q3 = AddTestQuestion(cat, "Where did Jesus find a rock?", "A", 1, 1, "where did", "kt:jesus", " find a rock");
            Question q4 = AddTestQuestion(cat, "Where did Mary eat?", "A", 1, 1, "where did", "kt:mary", "eat");
            Question q5 = AddTestQuestion(cat, "When Mary went to the tomb, where did Jesus meet her?", "A", 1, 1, "when", "kt:mary", "went to the tomb", "where did", "kt:jesus", " meet her");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);

            phrase1.Translation = "\u00BFCua\u0301ndo?";
            phrase2.Translation = "\u00BFDo\u0301nde encontro\u0301 Mari\u0301a a JESUS?";
            phrase3.Translation = "\u00BFDo\u0301nde encontro\u0301 JESUS una piedra?";
            phrase4.Translation = "\u00BFDo\u0301nde comio\u0301 Mari\u0301a?";
            Assert.AreEqual("\u00BFCua\u0301ndo Mari\u0301a Do\u0301nde JESUS?".Normalize(NormalizationForm.FormC), phrase5.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsFalse(phrase5.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a group of phrases that have a common part such
        /// that phrases A & B have a common substring that is longer than the substring that
        /// all three share in common.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_FallbackToSmallerCommonSubstring()
        {
            AddMockedKeyTerm("the squirrel", "la ardilla");
            AddMockedKeyTerm("donkey", "asno");
            AddMockedKeyTerm("Balaam", "Balaam");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "When did the donkey and the squirrel fight?", "A", 1, 1, "when", "did the", "kt:donkey", "and", "kt:the squirrel", "fight");
            Question q2 = AddTestQuestion(cat, "What did the donkey and Balaam do?", "A", 1, 1, "what", "did the", "kt:donkey", "and", "kt:balaam", "do");
            Question q3 = AddTestQuestion(cat, "Where are Balaam and the squirrel?", "A", 1, 1, "where", "are", "kt:balaam", "and", "kt:the squirrel");
            Question q4 = AddTestQuestion(cat, "and?", "A", 1, 1, "and");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno y la ardilla?";
            phrase2.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
            phrase3.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam y la ardilla?";
            Assert.AreEqual("\u00BFy?", phrase4.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsFalse(phrase4.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a group of phrases that have a common part such
        /// that phrases A & B have a common substring that is longer than the substring that
        /// all three share in common.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_FallbackToSmallerCommonSubstring_EndingInLargerSubstring()
        {
            AddMockedKeyTerm("the squirrel", "ardilla");
            AddMockedKeyTerm("donkey", "asno");
            AddMockedKeyTerm("Balaam", "Balaam");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "When did the donkey and the squirrel fight?", "A", 1, 1, "when", "did the", "kt:donkey", "and", "kt:the squirrel", "fight");
            Question q2 = AddTestQuestion(cat, "Where are Balaam and the squirrel?", "A", 1, 1, "where", "are", "kt:balaam", "and", "kt:the squirrel");
            Question q3 = AddTestQuestion(cat, "What did the donkey and Balaam do?", "A", 1, 1, "what", "did the", "kt:donkey", "and", "kt:balaam", "do");
            Question q4 = AddTestQuestion(cat, "and?", "A", 1, 1, "and");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno loco y ardilla?";
            phrase2.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam loco y ardilla?";
            phrase3.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
            Assert.AreEqual("\u00BFy?", phrase4.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsFalse(phrase4.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a group of phrases that have a common part such
        /// that phrases A & B have a common substring that is longer than the substring that
        /// all three share in common.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_FallbackToSmallerCommonSubstring_StartingInLargerSubstring()
        {
            AddMockedKeyTerm("the squirrel", "ardilla");
            AddMockedKeyTerm("donkey", "asno");
            AddMockedKeyTerm("Balaam", "Balaam");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "When did the donkey and the squirrel fight?", "A", 1, 1, "when did the", "kt:donkey", "and", "kt:the squirrel", "fight");
            Question q2 = AddTestQuestion(cat, "Where are Balaam and the squirrel?", "A", 1, 1, "where are", "kt:balaam", "and", "kt:the squirrel");
            Question q3 = AddTestQuestion(cat, "What did the donkey and Balaam do?", "A", 1, 1, "what did the", "kt:donkey", "and", "kt:balaam", "do");
            Question q4 = AddTestQuestion(cat, "and?", "A", 1, 1, "and");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);

            phrase1.Translation = "\u00BFCua\u0301ndo pelearon el asno y la ardilla?";
            phrase2.Translation = "\u00BFDo\u0301nde esta\u0301n Balaam y la ardilla?";
            phrase3.Translation = "\u00BFQue\u0301 hicieron el asno y Balaam?";
            Assert.AreEqual("\u00BFy?", phrase4.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsFalse(phrase4.HasUserTranslation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests setting the translation for a phrase such that there is a single part whose
        /// rendering does not match the statistically best rendering for that part. The
        /// statistically best part should win.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ChangeTranslation_PreventUpdatedTranslationFromChangingGoodPartTranslation()
        {
            AddMockedKeyTerm("donkey", "asno");
            AddMockedKeyTerm("Balaam", "Balaam");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "When?", "A", 1, 1, "When");
            Question q2 = AddTestQuestion(cat, "When Balaam eats donkey.", "A", 1, 1, "when", "kt:balaam", "eats", "kt:donkey");
            Question q3 = AddTestQuestion(cat, "What donkey eats?", "A", 1, 1, "what", "kt:donkey", "eats");
            Question q4 = AddTestQuestion(cat, "What Balaam eats?", "A", 1, 1, "what", "kt:balaam", "eats");
            Question q5 = AddTestQuestion(cat, "Donkey eats?", "A", 1, 1, "kt:donkey", "eats");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);
            TranslatablePhrase phrase4 = pth.GetPhrase(q4.ScriptureReference, q4.Text);
            TranslatablePhrase phrase5 = pth.GetPhrase(q5.ScriptureReference, q5.Text);

            phrase1.Translation = "\u00BFCua\u0301ndo?";
            phrase2.Translation = "\u00BFCua\u0301ndo come Balaam al asno.";
            phrase3.Translation = "\u00BFQue\u0301 come el asno?";
            phrase4.Translation = "\u00BFQue\u0301 ingiere Balaam?";
            Assert.AreEqual("\u00BFasno come?", phrase5.Translation);

            Assert.IsTrue(phrase1.HasUserTranslation);
            Assert.IsTrue(phrase2.HasUserTranslation);
            Assert.IsTrue(phrase3.HasUserTranslation);
            Assert.IsTrue(phrase4.HasUserTranslation);
            Assert.IsFalse(phrase5.HasUserTranslation);
        }
        #endregion

        #region Rendering Selection Rules tests
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that adding a rendering selection rule based on the preceding (English) word
        /// causes the correct vernacular rendering to be selected.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SelectCorrectTermRendering_NoPartsTranslated_BasedOnPrecedingWordRule()
        {
            AddMockedKeyTerm("Jesus", "susej", "Cristo", "Jesucristo", "Jesus", "Cristo Jesus");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was the man Jesus healed?", "A", 1, 1, "who was the man", "kt:jesus", " healed");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);

            Assert.AreEqual(2, phrase1.TranslatableParts.Count());

            Assert.IsFalse(phrase1.HasUserTranslation);
            Assert.AreEqual("Cristo", phrase1.Translation);

            pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] { new RenderingSelectionRule(@"\bman {0}", @"ucristo\b") });

            Assert.AreEqual("Jesucristo", phrase1.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that adding a rendering selection rule based on the following (English) word
        /// causes the correct vernacular rendering to be inserted into the partial translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SelectCorrectTermRendering_SomePartsTranslated_BasedOnFollowingWordRule()
        {
            AddMockedKeyTerm("Stephen", "Esteban");
            AddMockedKeyTerm("Mary", "Mari\u0301a");
            AddMockedKeyTerm("look", "kool", "mirar", "pareci\u0301a", "buscaba", "busca", "busco");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "What did Stephen look like to the priests and elders and other people present?", "A", 1, 1, "what", "did", "kt:stephen", "kt:look", "like to the priests and elders and other people present");
            Question q2 = AddTestQuestion(cat, "What did Stephen do?", "A", 1, 1, "what", "did", "kt:stephen", "do");
            Question q3 = AddTestQuestion(cat, "What did Mary look for?", "A", 1, 1, "what", "did", "kt:mary", "kt:look", "for");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            phrase1.Translation = "\u00BFCo\u0301mo pareci\u0301a Esteban a los sacerdotes y ancianos y a los dema\u0301s?";
            phrase2.Translation = "\u00BFCo\u0301mo hizo Esteban?";

            Assert.IsFalse(phrase3.HasUserTranslation);
            Assert.AreEqual("\u00BFCo\u0301mo i Mari\u0301a mirar?".Normalize(NormalizationForm.FormC), phrase3.Translation);

            pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
                new RenderingSelectionRule(@"{0} like\b", @"\bparec"),
                new RenderingSelectionRule(@"{0} for\b", @"\bbusc")});

            Assert.AreEqual("\u00BFCo\u0301mo i Mari\u0301a buscaba?".Normalize(NormalizationForm.FormC), phrase3.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that adding a rendering selection rule based on the (English) suffix causes
        /// the correct vernacular rendering to be inserted into the translation template.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SelectCorrectTermRendering_FillInTemplate_BasedOnSuffixRule()
        {
            AddMockedKeyTerm("magician", "naicigam", "mago", "brujo");
            AddMockedKeyTerm("servant", "tnavres", "criado", "siervo");
            AddMockedKeyTerm("heal", "laeh", "sanar", "curada", "sanada", "sanara\u0301", "sanas", "curan", "cura", "sana", "sanado");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Was the servant healed?", "A", 1, 1, "was the", "kt:servant", "kt:heal");
            Question q2 = AddTestQuestion(cat, "Was the magician healed?", "A", 1, 1, "was the", "kt:magician", "kt:heal");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            phrase1.Translation = "\u00BFFue sanado el siervo?";

            Assert.IsFalse(phrase2.HasUserTranslation);
            Assert.AreEqual("\u00BFFue sanar el mago?", phrase2.Translation);

            pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
                new RenderingSelectionRule(@"{0}\w*ed\b", @"o$")});

            Assert.AreEqual("\u00BFFue sanado el mago?", phrase2.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// TXL-208: Tests that getting the Translation (and also the DebugInfo) of a phrase
        /// that has a pattern does not crash (and correctly uses an empty string) when there
        /// are no non-empty key term renderings set for the key terms used in an untranslated
        /// question.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetTranslation_MatchesPatternOfAnotherQuestionButKeyTermRenderingsAreUnknown_EmptyKeyTermRenderingsAreUsed()
        {
            AddMockedKeyTerm("magician", "naicigam", "");
            AddMockedKeyTerm("servant", "tnavres", "criado", "siervo");
            AddMockedKeyTerm("man", "nam", "hombre");
            AddMockedKeyTerm("heal", "laeh", "sanar", "curada", "sanado", "sanara\u0301", "sanas", "curan", "cura", "sana");
            AddMockedKeyTerm("help", "pleh", "ayudado", "ayudar", "auxiliado", "ayudara\u0301", "ayudas", "auxilian", "auxilia", "ayuda");
            AddMockedKeyTerm("blind", "dnilb", "");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Was the servant healed?", "A", 1, 1, "was the", "kt:servant", "kt:heal");
            Question q2 = AddTestQuestion(cat, "Was the magician blinded?", "A", 1, 1, "was the", "kt:magician", "kt:blind");
            Question q3 = AddTestQuestion(cat, "Was the man helped?", "B", 2, 2, "was the", "kt:man", "kt:help");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            phrase1.Translation = "\u00BFFue sanado el siervo?";
            Assert.AreEqual("\u00BFFue ayudado el hombre?", phrase3.Translation,
	            "Sanity check: If this fails, the pattern was probably not detected based on setting the preceding translation.");

            Assert.IsFalse(phrase2.HasUserTranslation);

            Assert.AreEqual("\u00BFFue  el ?", phrase2.Translation);
            Assert.AreEqual("\u00BFFue (KT: ) el (KT: )?  ---  was the (3, Fue) | Magician (KT: ) | Blind (KT: )", phrase2.DebugInfo);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// TXL-208: Tests that getting the Translation (and also the DebugInfo) of a phrase
        /// that has a pattern does not crash (and correctly uses renderings and numbers.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [TestCase("magos", "enciegados")]
        [TestCase("magos", "")]
        [TestCase("", "ciegos")]
        [TestCase("", "")]
        public void GetTranslation_MatchesPatternOfAnotherQuestionWithNumbers_EmptyKeyTermRenderingsAreUsed(string magicianRendering, string blindedRendering)
        {
            AddMockedKeyTerm("magician", "naicigam", magicianRendering);
            AddMockedKeyTerm("servant", "tnavres", "criados", "criado");
            AddMockedKeyTerm("man", "nam", "hombre", "hombres");
            AddMockedKeyTerm("heal", "laeh", "sanar", "curada", "sanado", "sanados", "sanara\u0301", "sanas", "curan", "cura", "sana");
            AddMockedKeyTerm("help", "pleh", "ayudar", "ayudado", "ayudados", "auxiliado", "ayudara\u0301", "ayudas", "auxilian", "auxilia", "ayuda");
            AddMockedKeyTerm("blind", "dnilb", blindedRendering);

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Were the 2 servants healed by the 2 men?", "A", 1, 1, "were the", "num:2", "kt:servant", "kt:heal", "by the", "num:2", "kt:man");
            Question q2 = AddTestQuestion(cat, "Were the 4 magicians blinded by the 4 servants?", "A", 1, 1, "were the", "num:4", "kt:magician", "kt:blind", "by the", "num:4", "kt:servant");
            Question q3 = AddTestQuestion(cat, "Were the 12 men helped by the 6 servants?", "B", 2, 2, "were the", "num:12", "kt:man", "kt:help", "by the", "num:6", "kt:servant");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);
            TranslatablePhrase phrase3 = pth.GetPhrase(q3.ScriptureReference, q3.Text);

            phrase1.Translation = "\u00BFFueron sanados los 2 criados por los 2 hombres?";
            phrase3.Translation = "\u00BFFueron ayudados los 12 hombres por los 6 siervos?";

            Assert.IsFalse(phrase2.HasUserTranslation);

            Assert.AreEqual($"\u00BFFueron {blindedRendering} los 4 {magicianRendering} por los 4 criados?", phrase2.Translation);
            Assert.That(phrase2.DebugInfo.StartsWith($"\u00BFFueron (KT: {blindedRendering}) los #4 (KT: {magicianRendering}) por los #4 (KT: criados)?  ---  "));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that adding a rendering selection rule based on the (English) suffix causes
        /// the correct vernacular rendering to be inserted into the translation template. In
        /// this test, there are multiple renderings which conform to the rendering selection
        /// rule -- we want the one that is also the default rendering for the term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SelectCorrectTermRendering_FillInTemplate_BasedOnSuffixRule_PreferDefault()
        {
            AddMockedKeyTerm("magician", "naicigam", "mago", "brujo");
            AddMockedKeyTerm("servant", "tnavres", "criado", "siervo");
            AddMockedKeyTerm("heal", "laeh", "sanara\u0301", "curada", "sanada", "sanar", "curara\u0301", "sanas", "curan", "cura", "sana", "sanado");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Will the servant be healed?", "A", 1, 1, "will the", "kt:servant", "be", "kt:heal");
            Question q2 = AddTestQuestion(cat, "Will the magician be healed?", "A", 1, 1, "will the", "kt:magician", "be", "kt:heal");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);
            TranslatablePhrase phrase2 = pth.GetPhrase(q2.ScriptureReference, q2.Text);

            phrase1.Translation = "\u00BFSe curara\u0301 el siervo?";

            Assert.IsFalse(phrase2.HasUserTranslation);
            Assert.AreEqual("\u00BFSe sanara\u0301 el mago?".Normalize(NormalizationForm.FormC), phrase2.Translation);

            pth.TermRenderingSelectionRules = new List<RenderingSelectionRule>(new[] {
                new RenderingSelectionRule(@"Will .* {0}\w*\b", "ra\u0301$")});

            m_dummyKtRenderings["laeh"].Remove("curara\u0301");
            m_dummyKtRenderings["laeh"].Insert(0, "curara\u0301");

            Dictionary<Word, List<KeyTerm>> keyTermsTable =
                (Dictionary<Word, List<KeyTerm>>)ReflectionHelper.GetField(qp.PhrasePartManager, "m_keyTermsTable");
            keyTermsTable["heal"].First().LoadRenderings();

            Assert.AreEqual("\u00BFSe curara\u0301 el mago?".Normalize(NormalizationForm.FormC), phrase2.Translation);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that adding a disabled rendering selection rule does not change the resulting
        /// translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void SelectCorrectTermRendering_RuleDisabled()
        {
            AddMockedKeyTerm("Jesus", "susej", "Cristo", "Jesucristo", "Jesus", "Cristo Jesus");

            var cat = m_sections.Items[0].Categories[0];
            Question q1 = AddTestQuestion(cat, "Who was the man Jesus healed?", "A", 1, 1, "who was the man", "kt:jesus",
                "healed");

            var qp = new QuestionProvider(GetParsedQuestions());
            PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
            ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

            TranslatablePhrase phrase1 = pth.GetPhrase(q1.ScriptureReference, q1.Text);

            Assert.IsFalse(phrase1.HasUserTranslation);
            Assert.AreEqual("Cristo", phrase1.Translation);

            pth.TermRenderingSelectionRules =
                new List<RenderingSelectionRule>(new[] {new RenderingSelectionRule(@"\bman {0}", @"ucristo\b")});
            pth.TermRenderingSelectionRules[0].Disabled = true;

            Assert.AreEqual("Cristo", phrase1.Translation);
        }

        #endregion

		#region SetTranslationsFromText tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has no lines of text.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_EmptyFile_NoTranslationsSet()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase = pth.GetPhrase("A", "Who was the man?");

			using (TextReader reader = new StringReader(""))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "empty", new TestScrVers(), writer);
				Assert.AreEqual(0, phrase.Translation.Length);
				Assert.IsFalse(phrase.HasUserTranslation);
				Assert.AreEqual("Processing file: empty\r\nSummary\r\n=======\r\n" +
					"Read: 0, Matched: 0, Set: 0, Unparsable Lines: 0\r\n",
					writer.ToString());
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has some bogus lines.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_UnparsableLines_ProblemsReported()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "GEN 1.1", 001001001, 001001001, "who was the man");
			AddTestQuestion(cat, "Who is the girl?", "GEN 1.2", 001001002, 001001002, "who is the girl");
			AddTestQuestion(cat, "Why do you ask?", "GEN 2.2", 001002002, 001002002, "why do you ask");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase1 = pth.GetPhrase("GEN 1:1", "Who was the man?");

			var translations = "Genesis es un libro muy especial.\r\n" +
				"\\rf GEN 1\r\n" +
				"Quien es el unico creador de todas las cosas? (1)\r\n" +
				"Translated by Jennifer";

			using (TextReader reader = new StringReader(translations))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "Genesis.txt", new TestScrVers(), writer);

				Assert.AreEqual("Quien es el unico creador de todas las cosas?", phrase1.Translation);
				Assert.IsTrue(phrase1.HasUserTranslation);

				Assert.AreEqual("Processing file: Genesis.txt\r\n" +
					"   ***Parsing problem at line 1: \"Genesis es un libro muy especial.\"\r\n" +
					"   GEN 1:1 - \"Who was the man?\" ---> \"Quien es el unico creador de todas las cosas?\"\r\n" +
					"   ***Parsing problem at line 4: \"Translated by Jennifer\"\r\n" +
					"Summary\r\n=======\r\n" +
					"Read: 1, Matched: 1, Set: 1, Unparsable Lines: 2\r\n",
					writer.ToString());
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has exactly one matching
		/// translation for each question (none of which are translated).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_ExactMatchesNoExistingTranslations_AllTranslationsSet()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "GEN 1.1", 001001001, 001001001, "who was the man");
			AddTestQuestion(cat, "Who is the girl?", "GEN 1.2", 001001002, 001001002, "who is the girl");
			AddTestQuestion(cat, "Why do you ask?", "GEN 2.2", 001002002, 001002002, "why do you ask");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase1 = pth.GetPhrase("GEN 1.1", "Who was the man?");
			var phrase2 = pth.GetPhrase("GEN 1.2", "Who is the girl?");
			var phrase3 = pth.GetPhrase("GEN 2.2", "Why do you ask?");

			var translations = "\\rf GEN 1\r\n" +
				"\r\n" +
				"Quien es el unico creador de todas las cosas? (1)\r\n" +
				"Cual era la condicion de la tierra en el principio? (2)\r\n" +
				"\r\n" +
				"\\rf GEN 2\r\n" +
				"\r\n" +
				"Que hizo Dios en el septimo dia? (2)";

			using (TextReader reader = new StringReader(translations))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "Genesis.txt", new TestScrVers(), writer);

				Assert.AreEqual("Quien es el unico creador de todas las cosas?", phrase1.Translation);
				Assert.IsTrue(phrase1.HasUserTranslation);
				Assert.AreEqual("Cual era la condicion de la tierra en el principio?", phrase2.Translation);
				Assert.IsTrue(phrase2.HasUserTranslation);
				Assert.AreEqual("Que hizo Dios en el septimo dia?", phrase3.Translation);
				Assert.IsTrue(phrase3.HasUserTranslation);
				
				Assert.AreEqual("Processing file: Genesis.txt\r\n" +
					"   GEN 1:1 - \"Who was the man?\" ---> \"Quien es el unico creador de todas las cosas?\"\r\n" +
					"   GEN 1:2 - \"Who is the girl?\" ---> \"Cual era la condicion de la tierra en el principio?\"\r\n" +
					"   GEN 2:2 - \"Why do you ask?\" ---> \"Que hizo Dios en el septimo dia?\"\r\n" +
					"Summary\r\n=======\r\n" +
					"Read: 3, Matched: 3, Set: 3, Unparsable Lines: 0\r\n",
					writer.ToString());
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has exactly one matching
		/// translation for each question (some of which are already translated).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_ExactMatchesSomeExistingTranslations_UntranslatedQuestionsHaveTranslationsSet()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "GEN 1.1", 001001001, 001001001, "who was the man");
			AddTestQuestion(cat, "Who is the girl?", "GEN 1.2", 001001002, 001001002, "who is the girl");
			AddTestQuestion(cat, "Why do you ask?", "GEN 2.2", 001002002, 001002002, "why do you ask");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase1 = pth.GetPhrase("GEN 1:1", "Who was the man?");
			phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";
			var phrase2 = pth.GetPhrase("GEN 1:2", "Who is the girl?");
			var phrase3 = pth.GetPhrase("GEN 2:2", "Why do you ask?");
			phrase3.Translation = "\u00BFPor que\u0301 preguntas?";

			var translations = "\\rf GEN 1\r\n" +
				"\r\n" +
				"Quien es el unico creador de todas las cosas? (1)\r\n" +
				"Cual era la condicion de la tierra en el principio? (2)\r\n" +
				"\r\n" +
				"\\rf GEN 2\r\n" +
				"\r\n" +
				"Que hizo Dios en el septimo dia? (2)";

			using (TextReader reader = new StringReader(translations))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "Genesis.txt", new TestScrVers(), writer);

				Assert.AreEqual("\u00BFQuie\u0301n era el hombre?".Normalize(NormalizationForm.FormC), phrase1.Translation);
				Assert.AreEqual("Cual era la condicion de la tierra en el principio?".Normalize(NormalizationForm.FormC), phrase2.Translation);
				Assert.IsTrue(phrase2.HasUserTranslation);
				Assert.AreEqual("\u00BFPor que\u0301 preguntas?".Normalize(NormalizationForm.FormC), phrase3.Translation);

				Assert.AreEqual("Processing file: Genesis.txt\r\n" +
					"   GEN 1:1 - \"Who was the man?\" ***Already translated as: \"\u00BFQuie\u0301n era el hombre?\" ***Not set to: \"Quien es el unico creador de todas las cosas?\"\r\n" +
					"   GEN 1:2 - \"Who is the girl?\" ---> \"Cual era la condicion de la tierra en el principio?\"\r\n" +
					"   GEN 2:2 - \"Why do you ask?\" ***Already translated as: \"\u00BFPor que\u0301 preguntas?\" ***Not set to: \"Que hizo Dios en el septimo dia?\"\r\n" +
					"Summary\r\n=======\r\n" +
					"Read: 3, Matched: 3, Set: 1, Unparsable Lines: 0\r\n".Normalize(NormalizationForm.FormD),
					writer.ToString().Normalize(NormalizationForm.FormD));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has some questions that
		/// don't match or that match multiple original questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_MultipleAndMissingQuestions_ExactMatchesHaveTranslationsSet()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "GEN 1.1-31", 001001001, 001001031, "who was the man");
			AddTestQuestion(cat, "Who is the girl?", "GEN 2.2-13", 001002002, 001002013, "who is the girl");
			AddTestQuestion(cat, "Why do you ask?", "GEN 2.2-13", 001002002, 001002013, "why do you ask");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase1 = pth.GetPhrase("GEN 1:1-31", "Who was the man?");
			var phrase2 = pth.GetPhrase("GEN 2:2-13", "Who is the girl?");
			var phrase3 = pth.GetPhrase("GEN 2:2-13", "Why do you ask?");
			phrase3.Translation = "\u00BFPor que\u0301 preguntas?";

			var translations = "\\rf GEN 1\r\n" +
				"\r\n" +
				"\u00BFQuie\u0301n es el unico creador de todas las cosas?\r\n" + // No explicit ref - should match whole chapter
				"Despue\u0301s de que Zorobabel los rechazo, que tres cosas hicieron estos adversarios? (10-12)\r\n" +
				"\r\n" +
				"\\rf GEN 2\r\n" +
				"\r\n" +
				"Cual era la condicion de la tierra en el principio? (2-13)\r\n" +
				"Que hizo Dios en el septimo dia? (2-13)";

			using (TextReader reader = new StringReader(translations))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "Genesis.txt", new TestScrVers(), writer);

				Assert.AreEqual("\u00BFQuie\u0301n es el unico creador de todas las cosas?".Normalize(NormalizationForm.FormC), phrase1.Translation);
				Assert.IsTrue(phrase1.HasUserTranslation);
				Assert.IsFalse(phrase2.HasUserTranslation);
				Assert.AreEqual("\u00BFPor que\u0301 preguntas?".Normalize(NormalizationForm.FormC), phrase3.Translation);
				Assert.IsTrue(phrase3.HasUserTranslation);

				Assert.AreEqual("Processing file: Genesis.txt\r\n" +
					"   GEN 1:1-31 - \"Who was the man?\" ---> \"\u00BFQuie\u0301n es el unico creador de todas las cosas?\"\r\n" +
					"   GEN 1:10-12 - ***No matching question: \"Despue\u0301s de que Zorobabel los rechazo, que tres cosas hicieron estos adversarios?\"\r\n" +
					"   GEN 2:2-13 - ***Multiple matching questions: \"Cual era la condicion de la tierra en el principio?\"\r\n" +
					"   GEN 2:2-13 - ***Multiple matching questions: \"Que hizo Dios en el septimo dia?\"\r\n" +
					"Summary\r\n=======\r\n" +
					"Read: 4, Matched: 1, Set: 1, Unparsable Lines: 0\r\n".Normalize(NormalizationForm.FormD),
					writer.ToString().Normalize(NormalizationForm.FormD));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling SetTranslationsFromText with a reader that has some questions that
		/// match multiple original questions, but where all matches already have translations
		/// set.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SetTranslationsFromText_MultipleQuestionsAllTranslated_NotFlaggedAsErrorNoTranslationsSet()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "GEN 2.2-13", 001002002, 001002013, "who was the man");
			AddTestQuestion(cat, "Who is the girl?", "GEN 2.2-13", 001002002, 001002013, "who is the girl");
			AddTestQuestion(cat, "Why do you ask?", "GEN 2.2-13", 001002002, 001002013, "why do you ask");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			ReflectionHelper.SetField(pth, "m_justGettingStarted", false);

			var phrase1 = pth.GetPhrase("GEN 2:2-13", "Who was the man?");
			var phrase2 = pth.GetPhrase("GEN 2:2-13", "Who is the girl?");
			var phrase3 = pth.GetPhrase("GEN 2:2-13", "Why do you ask?");
			phrase1.Translation = "\u00BFQuie\u0301n era el hombre?";
			phrase2.Translation = "\u00BFQuie\u0301n es la chica?";
			phrase3.Translation = "\u00BFPor que\u0301 preguntas?";

			var translations = "\\rf GEN 2\r\n" +
				"\u00BFQuie\u0301n es el unico creador de todas las cosas? (2-13)\r\n" +
				"\u00BFCual era la condicion de la tierra en el principio? (2-13)\r\n" +
				"\u00BFQue hizo Dios en el septimo dia? (2-13)";

			using (TextReader reader = new StringReader(translations))
			using (TextWriter writer = new StringWriter())
			{
				pth.SetTranslationsFromText(reader, "Genesis.txt", new TestScrVers(), writer);

				Assert.AreEqual("\u00BFQuie\u0301n era el hombre?".Normalize(NormalizationForm.FormC), phrase1.Translation);
				Assert.IsTrue(phrase1.HasUserTranslation);
				Assert.AreEqual("\u00BFQuie\u0301n es la chica?".Normalize(NormalizationForm.FormC), phrase2.Translation);
				Assert.IsTrue(phrase2.HasUserTranslation);
				Assert.AreEqual("\u00BFPor que\u0301 preguntas?".Normalize(NormalizationForm.FormC), phrase3.Translation);
				Assert.IsTrue(phrase3.HasUserTranslation);

				Assert.AreEqual("Processing file: Genesis.txt\r\n" +
					"   GEN 2:2-13 - \"\u00BFQuie\u0301n es el unico creador de todas las cosas?\" ***All matching questions have translations\r\n" +
					"   GEN 2:2-13 - \"\u00BFCual era la condicion de la tierra en el principio?\" ***All matching questions have translations\r\n" +
					"   GEN 2:2-13 - \"\u00BFQue hizo Dios en el septimo dia?\" ***All matching questions have translations\r\n" +
					"Summary\r\n=======\r\n" +
					"Read: 3, Matched: 0, Set: 0, Unparsable Lines: 0\r\n".Normalize(NormalizationForm.FormD),
					writer.ToString().Normalize(NormalizationForm.FormD));
			}
		}
		#endregion

		#region GetPhrase tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when there is an exact match.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_ExactMatchExists_GetsIt()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man");
			AddTestQuestion(cat, "Who was the woman?", "A", 1, 1, "who was the woman");
			AddTestQuestion(cat, "Who was the man?", "B", 2, 2, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phrase = pth.GetPhrase("A", "Who was the man?");
			Assert.AreEqual("A", phrase.Reference);
			Assert.AreEqual(1, phrase.StartRef);
			Assert.AreEqual(1, phrase.EndRef);
			Assert.AreEqual("Who was the man?", phrase.PhraseInUse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when there is not an exact match with any of the questions
		/// in use, but there is an exact match with the original phrase or one of the alternate
		/// forms.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TestCase(true)]
		[TestCase(false)]
		public void GetPhrase_MatchesAlternate_GetsIt(bool hiddenAlternative)
		{
			var cat = m_sections.Items[0].Categories[0];
			var q1 = AddTestQuestion(cat, "Who was the man?", "A", 1, 1, "who was the man");
			q1.Alternatives = new[]
			{ 
				new AlternativeForm {Text = "Who was the adult male person?"}, 
				new AlternativeForm {Text = "Who was that man?", Hide = hiddenAlternative},
				new AlternativeForm {Text = "Who was the gentleman?"}
			};
			AddTestQuestion(cat, "Who was the woman?", "A", 1, 1, "who was the woman");
			AddTestQuestion(cat, "Who was that man?", "B", 2, 2, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phrase = pth.GetPhrase("A", "Who was that man?");
			Assert.AreEqual("A", phrase.Reference);
			Assert.AreEqual(1, phrase.StartRef);
			Assert.AreEqual(1, phrase.EndRef);
			Assert.AreEqual("Who was the man?", phrase.PhraseInUse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when there is exactly one match in the same chapter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_OneMatchExistsForDifferentReferenceInSameChapter_GetsIt()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "LEV 2.15", 003002015, 003002015, "who was the man");
			AddTestQuestion(cat, "Who was the mannequin?", "LEV 2.1-13", 003002001, 003002013, "who was the mannequin");
			AddTestQuestion(cat, "Who was the man?", "LEV 3.15", 003003015, 003003015, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phrase = pth.GetPhrase("LEV 2:1-13", "Who was the man?");
			Assert.AreEqual("LEV 2.15", phrase.Reference);
			Assert.AreEqual(003002015, phrase.StartRef);
			Assert.AreEqual(003002015, phrase.EndRef);
			Assert.AreEqual("Who was the man?", phrase.PhraseInUse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when there are multiple matches in the same chapter, but
		/// none for the exact reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_TwoMatchesExistForDifferentReferencesInSameChapter_ReturnsNull()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "LEV 2.15", 003002015, 003002015, "who was the man");
			AddTestQuestion(cat, "Who was the mannequin?", "LEV 2.1-13", 003002001, 003002013, "who was the mannequin");
			AddTestQuestion(cat, "Who was the man?", "LEV 2.1-14", 003002001, 003002014, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			Assert.IsNull(pth.GetPhrase("LEV 2.1-13", "Who was the man?"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when the reference being sought is specified with a colon
		/// instead of a period (which is what is used in the master question list).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_MatchColonWithPeriod_GetsIt()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "LEV 2.1", 003002001, 003002001, "who was the man");
			AddTestQuestion(cat, "Who was the mannequin?", "LEV 2.1-13", 003002001, 003002013, "who was the mannequin");
			AddTestQuestion(cat, "Who was the man?", "LUK 3.13-15", 042003013, 042003015, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phrase = pth.GetPhrase("LEV 2:1", "Who was the man?");
			Assert.AreEqual("LEV 2.1", phrase.Reference);
			Assert.AreEqual(003002001, phrase.StartRef);
			Assert.AreEqual(003002001, phrase.EndRef);
			Assert.AreEqual("Who was the man?", phrase.PhraseInUse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when no reference is specified and there are multiple exact
		/// textual matches for the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_NullReferenceExactMatchesExist_GetsFirst()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "LEV 2.1", 003002001, 003002001, "who was the man");
			AddTestQuestion(cat, "Who was the mannequin?", "LEV 2.1-13", 003002001, 003002013, "who was the mannequin");
			AddTestQuestion(cat, "Who was the man?", "LUK 3.13-15", 042003013, 042003015, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			var phrase = pth.GetPhrase(null, "Who was the man?");
			Assert.AreEqual("LEV 2.1", phrase.Reference);
			Assert.AreEqual(003002001, phrase.StartRef);
			Assert.AreEqual(003002001, phrase.EndRef);
			Assert.AreEqual("Who was the man?", phrase.PhraseInUse);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests calling GetPhrase when no reference is specified and there is no exact textual
		/// match for the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetPhrase_NullReferenceNoExactMatchExists_ReturnsNull()
		{
			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Who was the man?", "LEV 2.1", 003002001, 003002001, "who was the man");
			AddTestQuestion(cat, "Who was the mannequin?", "LEV 2.1-13", 003002001, 003002013, "who was the mannequin");
			AddTestQuestion(cat, "Who was the man?", "LUK 3.13-15", 042003013, 042003015, "who was the man");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);

			Assert.IsNull(pth.GetPhrase(null, "Who was the manager?"));
		}
		#endregion

        #region ComparePhraseReferences tests
        [TestCase(true)]
        [TestCase(false)]
        public void ComparePhraseReferences_QuestionComparedToItself_ReturnsZero(bool ascending)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this the first question?", null), 1, 1, 1);
	        var result = PhraseTranslationHelper.ComparePhraseReferences(a, a, ascending ? 1 : -1);
		    Assert.AreEqual(0, result);
        }

        private static void VerifyComparePhraseReferencesALessThanB(bool ascending, TranslatablePhrase a, TranslatablePhrase b)
        {
	        var result = PhraseTranslationHelper.ComparePhraseReferences(a, b, ascending ? 1 : -1);
	        if (ascending)
		        Assert.That(result < 0);
	        else
		        Assert.That(result > 0);
	        // Repeat with parameters switched
	        result = PhraseTranslationHelper.ComparePhraseReferences(b, a, ascending ? 1 : -1);
	        if (ascending)
		        Assert.That(result > 0);
	        else
		        Assert.That(result < 0);
        }

        [TestCase(1, 1, 1, 2, 1, 1, true)]
        [TestCase(1, 1, 1, 2, 1, 1, false)]
        [TestCase(1, 1, 1, 1, 2, 1, true)]
        [TestCase(1, 1, 1, 1, 2, 1, false)]
        [TestCase(1, 1, 1, 1, 1, 2, true)]
        [TestCase(1, 1, 1, 1, 1, 2, false)]
        public void ComparePhraseReferences_QuestionsWithIdenticalReferencesButDifferentIndices_SortedByIndex(
	        int aSection, int aCategory, int aSequence,
	        int bSection, int bCategory, int bSequence,
	        bool ascending)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this the first question?", null),
		        aSection, aCategory, aSequence);
	        var b = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this different text but the same key?", null),
		        bSection, bCategory, bSequence);
	        VerifyComparePhraseReferencesALessThanB(ascending, a, b);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ComparePhraseReferences_QuestionsWithDifferentStartRefs_SortedByStartRef(bool ascending)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:1-2", 1, 2, "Why is this the first question?", null), 1, 1, 2);
	        var b = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Is this an overview question?", null), 1, 0, 1);
	        VerifyComparePhraseReferencesALessThanB(ascending, a, b);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ComparePhraseReferences_QuestionsInDifferentSectionsSameStartRefs_SortedByEndRef(bool ascending)
        {
	        var a = new TranslatablePhrase(new Question("GEN 29:14", 102914, 102914, "What did Laban say then?", null), 1, 1, 21);
	        var b = new TranslatablePhrase(new Question("GEN 29:14", 102914, 102915, "About how long had Jacob worked for Laban, when Laban asked Jacob about wages?", null), 2, 1, 2);
	        VerifyComparePhraseReferencesALessThanB(ascending, a, b);
        }
        #endregion

        #region ComparePhrasesByIndexedOrder tests

        [TestCase(true)]
        [TestCase(false)]
        public void ComparePhrasesByIndexedOrder_QuestionComparedToItself_ReturnsZero(bool ascending)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this the first question?", null), 1, 1, 1);
	        var result = PhraseTranslationHelper.ComparePhrasesByIndexedOrder(a, a);
	        Assert.AreEqual(0, result);
        }

        private static void VerifyComparePhrasesByIndexedOrderALessThanB(TranslatablePhrase a, TranslatablePhrase b)
        {
	        var result = PhraseTranslationHelper.ComparePhrasesByIndexedOrder(a, b);
	        Assert.That(result < 0);
	        // Repeat with parameters switched
	        result = PhraseTranslationHelper.ComparePhrasesByIndexedOrder(b, a);
	        Assert.That(result > 0);
        }

        [TestCase(1, 1, 1, 2, 1, 1)]
        [TestCase(1, 1, 1, 1, 2, 1)]
        [TestCase(1, 1, 1, 1, 1, 2)]
        public void ComparePhrasesByIndexedOrder_QuestionsWithIdenticalReferencesButDifferentIndices_SortedByIndex(
	        int aSection, int aCategory, int aSequence,
	        int bSection, int bCategory, int bSequence)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this the first question?", null),
		        aSection, aCategory, aSequence);
	        var b = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this different text but the same key?", null),
		        bSection, bCategory, bSequence);
	        VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }

        [TestCase(1, 1, 1, 2, 1, 1)]
        [TestCase(1, 1, 1, 1, 2, 1)]
        [TestCase(1, 1, 1, 1, 1, 2)]
        public void ComparePhrasesByIndexedOrder_QuestionsWithDifferentStartRefs_SortedByIndex(
	        int aSection, int aCategory, int aSequence,
	        int bSection, int bCategory, int bSequence)
        {
	        var a = new TranslatablePhrase(new Question("GEN 1:2", 2, 2, "Why is this question out of verse order?", null),
		        aSection, aCategory, aSequence);
	        var b = new TranslatablePhrase(new Question("GEN 1:1-2", 1, 2, "Is this a summary question?", null),
		        bSection, bCategory, bSequence);
	        VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }

        [Test]
        public void ComparePhrasesByIndexedOrder_QuestionsInDifferentSectionsSameStartRefs_SortedBySection()
        {
	        var a = new TranslatablePhrase(new Question("GEN 29:14", 102914, 102914, "What did Laban say then?", null), 1, 1, 21);
	        var b = new TranslatablePhrase(new Question("GEN 29:14", 102914, 102915, "About how long had Jacob worked for Laban, when Laban asked Jacob about wages?", null), 2, 1, 2);
	        VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }

        [TestCase(001001001, 001001002, 001001002, 001001002, true)]
        [TestCase(001001001, 001001002, 001001002, 001001002, false)]
        [TestCase(001001002, 001001002, 001001001, 001001002, true)]
        [TestCase(001001002, 001001002, 001001001, 001001002, false)]
        [TestCase(001001001, 001001005, 001001002, 001001002, true)]
        [TestCase(001001001, 001001005, 001001002, 001001002, false)]
        [TestCase(001001002, 001001003, 001001002, 001001006, true)]
        [TestCase(001001002, 001001003, 001001002, 001001006, false)]
        [TestCase(001001005, 001001007, 001001002, 001001006, true)]
        [TestCase(001001005, 001001007, 001001002, 001001006, false)]
        [TestCase(001001006, 001001008, 001001002, 001001006, true)]
        [TestCase(001001006, 001001008, 001001002, 001001006, false)]
        public void ComparePhrasesByIndexedOrder_QuestionsWithOverlappingRefs_SortedBySequenceNumber(
	        int startRefA, int endRefA, int startRefB, int endRefB, bool differByCategory)
        {
	        int bCategory = differByCategory ? 1 : 0;
	        int bSequence = differByCategory ? 1 : 2;
	        var strRefA = BCVRef.MakeReferenceString("GEN", startRefA, endRefA, ":", "-");
	        var strRefB = BCVRef.MakeReferenceString("GEN", startRefB, endRefB, ":", "-");
	        var a = new TranslatablePhrase(new Question(strRefA, startRefA, endRefA, "Why is this the first question?", null), 6, 0, 1);
	        var b = new TranslatablePhrase(new Question(strRefB, startRefB, endRefB, "Why is this the second question?", null), 6, bCategory, bSequence);
            VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }

        [TestCase(001001001, 001001002, 001001003, 001001003, true)]
        [TestCase(001001001, 001001002, 001001003, 001001003, false)]
        [TestCase(001001003, 001001003, 001001001, 001001002, true)]
        [TestCase(001001003, 001001003, 001001001, 001001002, false)]
        [TestCase(001001001, 001001005, 001001006, 001001006, true)]
        [TestCase(001001001, 001001005, 001001006, 001001006, false)]
        [TestCase(001001002, 001001003, 001001004, 001001006, true)]
        [TestCase(001001002, 001001003, 001001004, 001001006, false)]
        public void ComparePhrasesByIndexedOrder_NonOverlappingRefs_SortedBySequenceNumber(
	        int startRefA, int endRefA, int startRefB, int endRefB, bool differByCategory)
        {
	        int bCategory = differByCategory ? 1 : 0;
	        int bSequence = differByCategory ? 1 : 2;
	        var strRefA = BCVRef.MakeReferenceString("GEN", startRefA, endRefA, ":", "-");
	        var strRefB = BCVRef.MakeReferenceString("GEN", startRefB, endRefB, ":", "-");
	        var a = new TranslatablePhrase(new Question(strRefA, startRefA, endRefA, "Why is this the first question?", null), 4, 0, 1);
	        var b = new TranslatablePhrase(new Question(strRefB, startRefB, endRefB, "Why is this the second question?", null), 4, bCategory, bSequence);
            VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }

        [Test]
        public void ComparePhrasesByIndexedOrder_DifferentBooks_SortedByBookNumber()
        {
	        var a = new TranslatablePhrase(new Question("GEN 1.1", 1001001, 1001001,
		        "At what time did these events happen?", null), 4, 1, 0);
	        var b = new TranslatablePhrase(new Question("ACT 1:1-2", 44001001, 44001002, 
			    "To whom did the writer of Acts address this book?", null), 0, 1, 0);
	        VerifyComparePhrasesByIndexedOrderALessThanB(a, b);
        }
        #endregion

        [TestCase("en-US")]
        [TestCase("es")]
        [TestCase("oth")]
		public void GetQuestionsForBooks_TwoBooks_QuestionsReturnedForCorrectBooksAndIdsAreBasedOnOriginalQuestion(string vernLocale)
		{
			var frenchLocalizations = MockRepository.GenerateMock<ILocalizationsProvider>();
			frenchLocalizations.Stub(l => l.Locale).Return("fr");
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q1"),
				out Arg<string>.Out("frQ1").Dummy)).Return(true);
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q2"),
				out Arg<string>.Out("frQ2").Dummy)).Return(true);
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q3"),
				out Arg<string>.Out("frQ3").Dummy)).Return(true);
            
			var spanishLocalizations = MockRepository.GenerateMock<ILocalizationsProvider>();
			spanishLocalizations.Stub(l => l.Locale).Return("es");
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q1"),
				out Arg<string>.Out("esQ1").Dummy)).Return(true);
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q2"),
				out Arg<string>.Out("esQ2").Dummy)).Return(true);
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q3"),
				out Arg<string>.Out("esQ3").Dummy)).Return(true);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "MAT 2.2", 40002002, 40002002, "Q1");
			AddTestQuestion(cat, "Q2", "MAT 2.2", 40002002, 40002002, "Q2");
			AddTestQuestion(cat, "Q3", "REV 6.4-5", 66006004, 66006005, "Q2");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			pth.GetPhrase("MAT 2:2", "Q1").Translation = $"{vernLocale} Why so many frogs?";
			pth.GetPhrase("MAT 2:2", "Q2").Translation = $"{vernLocale} What was her name?";
			pth.GetPhrase("REV 6:4-5", "Q3").Translation = $"{vernLocale} Who ate the pizza?";

			var result = pth.GetQuestionsForBooks(vernLocale, new [] {spanishLocalizations, frenchLocalizations}, new int[0]).ToList();
            Assert.AreEqual(2, result.Count);

            // Verify MAT questions
			var mat = result[0];
            Assert.AreEqual("MAT", mat.BookId);
            Assert.AreEqual(2, mat.Questions.Count);
			var q1 = mat.Questions[0];
            Assert.AreEqual("Q1", q1.Id);
            Assert.AreEqual($"{vernLocale} Why so many frogs?", q1.Question.Single(q => q.Lang == vernLocale).Text);
			var q2 = mat.Questions[1];
            Assert.AreEqual("Q2", q2.Id);
			pth.GetPhrase("MAT 2:2", "Q2").ModifiedPhrase = "This should not be the ID";
			Assert.AreEqual("Q2", q2.Id);
            Assert.AreEqual($"{vernLocale} What was her name?", q2.Question.Single(q => q.Lang == vernLocale).Text);
			Assert.IsTrue(mat.Questions.All(q => q.Question.Length == (vernLocale == "oth" ? 4 : 3)));

            // Verify REV questions
			var rev = result[1];
			Assert.AreEqual("REV", rev.BookId);
			var q3 = rev.Questions[0];
            Assert.AreEqual("Q3", q3.Id);
            Assert.AreEqual($"{vernLocale} Who ate the pizza?", q3.Question.Single(q => q.Lang == vernLocale).Text);
            Assert.AreEqual(vernLocale == "oth" ? 4 : 3, rev.Questions.Single().Question.Length);
		}
		
        // TXL-233
		[Test]
		public void GetQuestionsForBooks_TwoBooksPlusThreeThatNoLongerHaveUserConfirmedTranslations_BooksWithoutTranslationsReturnEmptyList()
		{
			var vernLocale = "en-US";
			var frenchLocalizations = MockRepository.GenerateMock<ILocalizationsProvider>();
			frenchLocalizations.Stub(l => l.Locale).Return("fr");
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q1"),
				out Arg<string>.Out("frQ1").Dummy)).Return(true);
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q2"),
				out Arg<string>.Out("frQ2").Dummy)).Return(true);
			frenchLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q3"),
				out Arg<string>.Out("frQ3").Dummy)).Return(true);
            
			var spanishLocalizations = MockRepository.GenerateMock<ILocalizationsProvider>();
			spanishLocalizations.Stub(l => l.Locale).Return("es");
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q1"),
				out Arg<string>.Out("esQ1").Dummy)).Return(true);
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q2"),
				out Arg<string>.Out("esQ2").Dummy)).Return(true);
			spanishLocalizations.Stub(l => l.TryGetLocalizedString(Arg<UIDataString>.Matches(d => d.Question == "Q3"),
				out Arg<string>.Out("esQ3").Dummy)).Return(true);

			var cat = m_sections.Items[0].Categories[0];
			AddTestQuestion(cat, "Q1", "MAT 2.2", 40002002, 40002002, "Q1");
			AddTestQuestion(cat, "Q2", "MAT 2.2", 40002002, 40002002, "Q2");
			AddTestQuestion(cat, "Q3", "JUD 1.4-5", 65001004, 65001005, "Q2");

			var qp = new QuestionProvider(GetParsedQuestions());
			PhraseTranslationHelper pth = new PhraseTranslationHelper(qp);
			pth.GetPhrase("MAT 2:2", "Q1").Translation = $"{vernLocale} Why so many frogs?";
			pth.GetPhrase("MAT 2:2", "Q2").Translation = $"{vernLocale} What was her name?";
			pth.GetPhrase("JUD 1:4-5", "Q3").Translation = $"{vernLocale} Who ate the pizza?";

			var result = pth.GetQuestionsForBooks(vernLocale, new [] {spanishLocalizations, frenchLocalizations}, new int[] {2, 44, 66}).ToList();
            Assert.AreEqual(5, result.Count);

			int i = 0;
			// Verify EXO questions
			var r = result[i];
			Assert.AreEqual("EXO", r.BookId);
			Assert.AreEqual(0, r.Questions.Count);

            // Verify MAT questions
			r = result[++i];
            Assert.AreEqual("MAT", r.BookId);
            Assert.AreEqual(2, r.Questions.Count);
			var q1 = r.Questions[0];
            Assert.AreEqual("Q1", q1.Id);
            Assert.AreEqual($"{vernLocale} Why so many frogs?", q1.Question.Single(q => q.Lang == vernLocale).Text);
			var q2 = r.Questions[1];
            Assert.AreEqual("Q2", q2.Id);
			pth.GetPhrase("MAT 2:2", "Q2").ModifiedPhrase = "This should not be the ID";
			Assert.AreEqual("Q2", q2.Id);
            Assert.AreEqual($"{vernLocale} What was her name?", q2.Question.Single(q => q.Lang == vernLocale).Text);
			Assert.IsTrue(r.Questions.All(q => q.Question.Length == (vernLocale == "oth" ? 4 : 3)));
            
			// Verify ACT questions
			r = result[++i];
			Assert.AreEqual("ACT", r.BookId);
			Assert.AreEqual(0, r.Questions.Count);

			// Verify JUD questions
			r = result[++i];
			Assert.AreEqual("JUD", r.BookId);
			var q3 = r.Questions[0];
			Assert.AreEqual("Q3", q3.Id);
			Assert.AreEqual($"{vernLocale} Who ate the pizza?", q3.Question.Single(q => q.Lang == vernLocale).Text);
			Assert.AreEqual(vernLocale == "oth" ? 4 : 3, r.Questions.Single().Question.Length);

            // Verify REV questions
			r = result[++i];
			Assert.AreEqual("REV", r.BookId);
			Assert.AreEqual(0, r.Questions.Count);
		}

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
            int startRef, int endRef, params object[] parts)
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
            int startRef, int endRef, params object[] parts)
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
		public TestQ(string text, string sRef, int startRef, int endRef, List<ParsedPart> parts) :
			base(sRef, startRef, endRef, text)
		{
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
