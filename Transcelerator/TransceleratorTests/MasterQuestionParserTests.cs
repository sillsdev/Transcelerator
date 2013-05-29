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
// File: MasterQuestionParserTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using AddInSideViews;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests the MasterQuestionParserBase implementation
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class MasterQuestionParserTests
	{
        private List<IKeyTerm> m_dummyKtList;
        private KeyTermRules m_keyTermRules;

        [SetUp]
        public void Setup()
        {
            m_dummyKtList = new List<IKeyTerm>();
            m_keyTermRules = null;
        }

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests enumerating overview and detail categories and questions with answers and
		/// comments.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_NoKeyTermsOrCustomizationsAndAllQuestionsUnique_EachQuestionHasOneTranslatablePart()
		{
		    MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                null, null, null, null);

		    ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        Assert.IsFalse(actQuestion.IsExcluded);
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                    }
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(5, pq.TranslatableParts.Length);
        }

	    /// ------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests parsing questions using a set of key terms
	    /// </summary>
	    /// ------------------------------------------------------------------------------------
	    [Test]
        public void GetResult_KeyTermsWithNoRules_KeyTermsBreakQuestionsIntoTranslatableParts()
	    {
            AddMockedKeyTerm("apostle");
            AddMockedKeyTerm("Luke");
            AddMockedKeyTerm("Jesus");

            MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                m_dummyKtList, m_keyTermRules, null, null);

            ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];

                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        Assert.IsFalse(actQuestion.IsExcluded);
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        switch (iQuestion)
                        {
                            case 0:
                                Assert.AreEqual(3, actQuestion.ParsedParts.Count);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                                Assert.AreEqual("what information did", actQuestion.ParsedParts[0].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[1].Type);
                                Assert.AreEqual("luke", actQuestion.ParsedParts[1].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[2].Type);
                                Assert.AreEqual("the writer of this book give in this introduction", actQuestion.ParsedParts[2].Text);
                                break;
                            case 1:
                                Assert.AreEqual(5, actQuestion.ParsedParts.Count);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                                Assert.AreEqual("what do you think an", actQuestion.ParsedParts[0].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[1].Type);
                                Assert.AreEqual("apostle", actQuestion.ParsedParts[1].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[2].Type);
                                Assert.AreEqual("of", actQuestion.ParsedParts[2].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[3].Type);
                                Assert.AreEqual("jesus", actQuestion.ParsedParts[3].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[4].Type);
                                Assert.AreEqual("is", actQuestion.ParsedParts[4].Text);
                                break;
                            case 4:
                                Assert.AreEqual(5, actQuestion.ParsedParts.Count);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                                Assert.AreEqual("what question did the", actQuestion.ParsedParts[0].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[1].Type);
                                Assert.AreEqual("apostle", actQuestion.ParsedParts[1].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[2].Type);
                                Assert.AreEqual("ask", actQuestion.ParsedParts[2].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[3].Type);
                                Assert.AreEqual("jesus", actQuestion.ParsedParts[3].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[4].Type);
                                Assert.AreEqual("about his kingdom", actQuestion.ParsedParts[4].Text);
                                break;
                            default:
                                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                                break;
                        }
                    }
                }
            }

            Assert.AreEqual(3, pq.KeyTerms.Length);
            Assert.AreEqual(10, pq.TranslatableParts.Length);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests parsing questions using a set of key terms
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_KeyTermsWithRulesLimitingMatchesByRef_KeyTermsAreUsedInCorrectQuestions()
        {
            AddMockedKeyTerm("God", 1, 4);
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("have", 2, 3, 4, 5, 6);
            AddMockedKeyTerm("say", 1, 2, 5);

            m_keyTermRules.Initialize();

            var qs = GenerateSimpleSectionWithSingleCategory(6);
            var cat = qs.Items[0].Categories[0];
            var q1 = cat.Questions[0];
            q1.Text = "What would God have me to say with respect to Paul?";
            q1.ScriptureReference = "A";
            q1.StartRef = 1;
            q1.EndRef = 1;
            var q2 = cat.Questions[1];
            q2.Text = "What is Paul asking me to say with respect to that dog?";
            q2.ScriptureReference = "B";
            q2.StartRef = 2;
            q2.EndRef = 2;
            var q3 = cat.Questions[2];
            q3.Text = "that dog";
            q3.ScriptureReference = "C";
            q3.StartRef = 3;
            q3.EndRef = 3;
            var q4 = cat.Questions[3];
            q4.Text = "Is it okay for Paul me to talk with respect to God today?";
            q4.ScriptureReference = "D";
            q4.StartRef = 4;
            q4.EndRef = 4;
            var q5 = cat.Questions[4];
            q5.Text = "that dog wishes this Paul and what is say radish";
            q5.ScriptureReference = "E";
            q5.StartRef = 5;
            q5.EndRef = 5;
            var q6 = cat.Questions[5];
            q6.Text = "What is that dog?";
            q6.ScriptureReference = "F";
            q6.StartRef = 6;
            q6.EndRef = 6;

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_dummyKtList, m_keyTermRules, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(3, pq.KeyTerms.Length);
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "god"));
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "paul"));
            Assert.IsFalse(pq.KeyTerms.Any(kt => kt.TermId == "have"));
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "say"));
            Assert.AreEqual(12, pq.TranslatableParts.Length);

            // Q1: Expected: "what would" /* 1 */, "kt:god", "have me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "kt:paul"
            Assert.AreEqual(6, q1.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[0].Type);
            Assert.AreEqual("what would", q1.ParsedParts[0].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[1].Type);
            Assert.AreEqual("god", q1.ParsedParts[1].Text);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[2].Type);
            Assert.AreEqual("have me to", q1.ParsedParts[2].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[3].Type);
            Assert.AreEqual("say", q1.ParsedParts[3].Text);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[4].Type);
            Assert.AreEqual("with respect to", q1.ParsedParts[4].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[5].Type);
            Assert.AreEqual("paul", q1.ParsedParts[5].Text);

            // Q2: Expected: "what is" /* 3 */, "kt:paul", "asking me to" /* 3 */, "kt:say", "with respect to" /* 3 */, "that dog" /* 4 */
            Assert.AreEqual(6, q2.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[0].Type);
            Assert.AreEqual("what is", q2.ParsedParts[0].Text);
            Assert.AreEqual(PartType.KeyTerm, q2.ParsedParts[1].Type);
            Assert.AreEqual("paul", q2.ParsedParts[1].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[2].Type);
            Assert.AreEqual("asking me to", q2.ParsedParts[2].Text);
            Assert.AreEqual(PartType.KeyTerm, q2.ParsedParts[3].Type);
            Assert.AreEqual("say", q2.ParsedParts[3].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[4].Type);
            Assert.AreEqual("with respect to", q2.ParsedParts[4].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[5].Type);
            Assert.AreEqual("that dog", q2.ParsedParts[5].Text);

            // Q3: Expected: "that dog" /* 4 */
            Assert.AreEqual(1, q3.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q3.ParsedParts[0].Type);
            Assert.AreEqual("that dog", q3.ParsedParts[0].Text);

            // Q4: Expected: "is it okay for" /* 1 */, "kt:paul", "me to talk" /* 1 */, "with respect to" /* 3 */, "kt:god", "today" /* 1 */
            Assert.AreEqual(6, q4.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[0].Type);
            Assert.AreEqual("is it okay for", q4.ParsedParts[0].Text);
            Assert.AreEqual(PartType.KeyTerm, q4.ParsedParts[1].Type);
            Assert.AreEqual("paul", q4.ParsedParts[1].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[2].Type);
            Assert.AreEqual("me to talk", q4.ParsedParts[2].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[3].Type);
            Assert.AreEqual("with respect to", q4.ParsedParts[3].Text);
            Assert.AreEqual(PartType.KeyTerm, q4.ParsedParts[4].Type);
            Assert.AreEqual("god", q4.ParsedParts[4].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[5].Type);
            Assert.AreEqual("today", q4.ParsedParts[5].Text);

            // Q5: Expected: "that dog" /* 4 */, "wishes this" /* 1 */, "kt:paul", "and" /* 1 */, "what is" /* 3 */, "kt:say", "radish" /* 1 */
            Assert.AreEqual(7, q5.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[0].Type);
            Assert.AreEqual("that dog", q5.ParsedParts[0].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[1].Type);
            Assert.AreEqual("wishes this", q5.ParsedParts[1].Text);
            Assert.AreEqual(PartType.KeyTerm, q5.ParsedParts[2].Type);
            Assert.AreEqual("paul", q5.ParsedParts[2].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[3].Type);
            Assert.AreEqual("and", q5.ParsedParts[3].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[4].Type);
            Assert.AreEqual("what is", q5.ParsedParts[4].Text);
            Assert.AreEqual(PartType.KeyTerm, q5.ParsedParts[5].Type);
            Assert.AreEqual("say", q5.ParsedParts[5].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[6].Type);
            Assert.AreEqual("radish", q5.ParsedParts[6].Text);

            // Q6: Expected: "what is" /* 3 */, "that dog" /* 4 */
            Assert.AreEqual(2, q6.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q6.ParsedParts[0].Type);
            Assert.AreEqual("what is", q6.ParsedParts[0].Text);
            Assert.AreEqual(PartType.TranslatablePart, q6.ParsedParts[1].Type);
            Assert.AreEqual("that dog", q6.ParsedParts[1].Text);
        }

	    ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that excluded questions are properly noted.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
		public void GetResult_TwoQuestionsDeleted_DeletedQuestionsMarkedAsExcluded()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "What do you think an apostle of Jesus is?";
            pc.Type = PhraseCustomization.CustomizationType.Deletion;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            pc.Type = PhraseCustomization.CustomizationType.Deletion;
            customizations.Add(pc);

            MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];

                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        if (iQuestion == 1 || iQuestion == 4)
                        {
                            Assert.IsTrue(actQuestion.IsExcluded);
                            Assert.AreEqual(0, actQuestion.ParsedParts.Count);
                        }
                        else
                        {
                            Assert.IsFalse(actQuestion.IsExcluded);
                            Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                            Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                            Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        }
                    }
                }
            }
            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(3, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions that have modifications are properly noted.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_ModifiedPhrases_ResultIncludesModifications()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "What do you think an apostle of Jesus is?";
            pc.ModifiedPhrase = "What do you think an apostle of Jesus Christ is?";
            pc.Type = PhraseCustomization.CustomizationType.Modification;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            pc.ModifiedPhrase = "What query did the apostles pose to Jesus about his realm?";
            pc.Type = PhraseCustomization.CustomizationType.Modification;
            customizations.Add(pc);

            MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];

                        Assert.IsFalse(actQuestion.IsExcluded);
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        switch (iQuestion)
                        {
                            case 1:
                                Assert.AreEqual("What do you think an apostle of Jesus Christ is?", actQuestion.ModifiedPhrase);
                                Assert.AreEqual(actQuestion.ModifiedPhrase, actQuestion.PhraseInUse);
                                break;
                            case 4:
                                Assert.AreEqual("What query did the apostles pose to Jesus about his realm?", actQuestion.ModifiedPhrase);
                                Assert.AreEqual(actQuestion.ModifiedPhrase, actQuestion.PhraseInUse);
                                break;
                            default:
                                Assert.IsNull(actQuestion.ModifiedPhrase);
                                Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                                break;
                        }
                    }
                }
            }
            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(5, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that user questions are properly added/inserted into the correct location.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_SimpleAdditionAndInsertionOfPhrases_PhrasesAreInCorrectOrder()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "What do you think an apostle of Jesus is?";
            pc.ModifiedPhrase = "Is this question before the one about the meaning of apostle?";
            pc.Answer = "Yup";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            pc.ModifiedPhrase = "What did He answer?";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);

            MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];

                        Assert.IsFalse(actQuestion.IsExcluded);
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        switch (iQuestion)
                        {
                            case 1:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("Is this question before the one about the meaning of apostle?", actQuestion.PhraseInUse);
                                break;
                            case 6:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("What did He answer?", actQuestion.PhraseInUse);
                                break;
                            default:
                                Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                                break;
                        }
                    }
                }
            }
            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(7, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions are properly ordered when the user has added some and then
        /// added some more to those.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_CompoundAdditionAndInsertionOfPhrases_PhrasesAreInCorrectOrder()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "What do you think an apostle of Jesus is?";
            pc.ModifiedPhrase = "Is this question before the one about the meaning of apostle?";
            pc.Answer = "Yup";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            pc.ModifiedPhrase = "What did He answer?";
            pc.Answer = "He told them to mind their own business.";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "Is this question before the one about the meaning of apostle?";
            pc.ModifiedPhrase = "Is this question before the one before the one about the meaning of apostle?";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.1-5";
            pc.OriginalPhrase = "Is this question before the one about the meaning of apostle?";
            pc.ModifiedPhrase = "This is going to hurt, isn't it?";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What did He answer?";
            pc.ModifiedPhrase = "I said, what did He answer?";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "I said, what did He answer?";
            pc.ModifiedPhrase = "Can I just go home now?";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);

            MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            VerifyQuestionSections(pq);

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            for (int iS = 0; iS < sections.Length; iS++)
            {
                Section actSection = sections[iS];
                for (int iC = 0; iC < actSection.Categories.Length; iC++)
                {
                    Category actCategory = actSection.Categories[iC];
                    for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
                    {
                        Question actQuestion = actCategory.Questions[iQ];

                        Assert.IsFalse(actQuestion.IsExcluded);
                        if (iQuestion == 9)
                        {
                            Assert.AreEqual(2, actQuestion.ParsedParts.Count);
                            Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                            Assert.AreEqual("i said", actQuestion.ParsedParts[0].Text);
                            Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[1].Type);
                            Assert.AreEqual("what did he answer", actQuestion.ParsedParts[1].Text);
                        }
                        else
                        {
                            Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                            Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        }
                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        switch (iQuestion)
                        {
                            case 1:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("Is this question before the one before the one about the meaning of apostle?", actQuestion.PhraseInUse);
                                break;
                            case 2:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("Is this question before the one about the meaning of apostle?", actQuestion.PhraseInUse);
                                break;
                            case 3:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("This is going to hurt, isn't it?", actQuestion.PhraseInUse);
                                break;
                            case 9:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("I said, what did He answer?", actQuestion.PhraseInUse);
                                break;
                            case 10:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("Can I just go home now?", actQuestion.PhraseInUse);
                                break;
                            default:
                                Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                                break;
                        }
                    }
                }
            }
            Assert.AreEqual(11, iQuestion);

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(11, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions are properly ordered when the user has added a question
        /// after a question that was inserted before a factory-supplied question. This case is
        /// interesting because it makes it hard to get the sequence numbers right.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_PhraseAddedAfterInsertionBefore_PhrasesAreInCorrectOrder()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            pc.ModifiedPhrase = "Is this question before the one about the meaning of apostle?";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "Is this question before the one about the meaning of apostle?";
            pc.ModifiedPhrase = "This is a phrase after the inserted question.";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);

            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[1];
            qs.Items[0] = CreateSection("ACT 1.1-6", "Acts 1:1-6 Introduction to the book.", 44001001,
                44001006, 0, 1);
            Question q = qs.Items[0].Categories[0].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "What question did the apostles ask Jesus about his kingdom?";
            q.Answers = new[] { "Stuff." };

            MasterQuestionParser qp = new MasterQuestionParser(qs, null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            List<Question> questions = pq.Sections.Items[0].Categories[0].Questions;
            Assert.AreEqual(3, questions.Count);
            for (int iQ = 0; iQ < questions.Count; iQ++)
            {
                Question actQuestion = questions[iQ];
                Assert.IsFalse(actQuestion.IsExcluded);
                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                Assert.IsNull(actQuestion.ModifiedPhrase);

                switch (iQ)
                {
                    case 0:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("Is this question before the one about the meaning of apostle?",
                                        actQuestion.PhraseInUse);
                        break;
                    case 1:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("This is a phrase after the inserted question.", actQuestion.PhraseInUse);
                        break;
                    case 2:
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?",
                                        actQuestion.PhraseInUse);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        break;
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(3, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions are properly ordered when the user has added a question
        /// before a question that was added after a question that was inserted before a
        /// factory-supplied question. This case is interesting because it makes it even harder
        /// to get the sequence numbers right.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_InsertAfterAdditionAfterInsertionBefore_PhrasesAreInCorrectOrder()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "Base question";
            pc.ModifiedPhrase = "AAA";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "AAA";
            pc.ModifiedPhrase = "CCC";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "CCC";
            pc.ModifiedPhrase = "BBB";
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);

            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[1];
            qs.Items[0] = CreateSection("ACT 1.1-6", "Acts 1:1-6 Introduction to the book.", 44001001,
                44001006, 0, 1);
            Question q = qs.Items[0].Categories[0].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "Base question";

            MasterQuestionParser qp = new MasterQuestionParser(qs, null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            List<Question> questions = pq.Sections.Items[0].Categories[0].Questions;
            Assert.AreEqual(4, questions.Count);
            for (int iQ = 0; iQ < questions.Count; iQ++)
            {
                Question actQuestion = questions[iQ];
                Assert.IsFalse(actQuestion.IsExcluded);
                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                Assert.IsNull(actQuestion.ModifiedPhrase);

                switch (iQ)
                {
                    case 0:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("AAA", actQuestion.PhraseInUse);
                        break;
                    case 1:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("BBB", actQuestion.PhraseInUse);
                        break;
                    case 2:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("CCC", actQuestion.PhraseInUse);
                        break;
                    case 3:
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual("Base question", actQuestion.PhraseInUse);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        break;
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(4, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions are properly enumerated when the user has added a question
        /// that doesn't have an English translation (just a GUID), and then added a question
        /// after that.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_AddedPhraseAfterAddedPhraseWithoutEnglishVersion_OrderIsCorrectAndPhraseWithoutEnglishVersionIsNotParsed()
        { 
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "Base question";
            Guid guidOfAddedQuestion = Guid.NewGuid();
            pc.ModifiedPhrase = Question.kGuidPrefix + guidOfAddedQuestion;
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);
            pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = Question.kGuidPrefix + guidOfAddedQuestion;
            pc.ModifiedPhrase = "Is this English, or what?";
            pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
            customizations.Add(pc);

            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[1];
            qs.Items[0] = CreateSection("ACT 1.1-6", "Acts 1:1-6 Introduction to the book.", 44001001,
                44001006, 0, 1);
            Question q = qs.Items[0].Categories[0].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "Base question";

            MasterQuestionParser qp = new MasterQuestionParser(qs, null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            List<Question> questions = pq.Sections.Items[0].Categories[0].Questions;
            Assert.AreEqual(3, questions.Count);
            for (int iQ = 0; iQ < questions.Count; iQ++)
            {
                Question actQuestion = questions[iQ];
                Assert.IsFalse(actQuestion.IsExcluded);
                Assert.IsNull(actQuestion.ModifiedPhrase);

                switch (iQ)
                {
                    case 0:
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual("Base question", actQuestion.PhraseInUse);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        break;
                    case 1:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual(Question.kGuidPrefix + guidOfAddedQuestion, actQuestion.PhraseInUse);
                        Assert.AreEqual(0, actQuestion.ParsedParts.Count);
                        break;
                    case 2:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual("Is this English, or what?", actQuestion.PhraseInUse);
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        break;
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(2, pq.TranslatableParts.Length);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that questions are properly enumerated when the user has inserted a question
        /// that doesn't have an English translation (just a GUID).
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetResult_InsertedQuestionWithoutEnglishVersion_OrderIsCorrectAndPhraseWithoutEnglishVersionIsNotParsed()
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc = new PhraseCustomization();
            pc.Reference = "ACT 1.6";
            pc.OriginalPhrase = "What question did the apostles ask Jesus about his kingdom?";
            Guid guidOfAddedQuestion = Guid.NewGuid();
            pc.ModifiedPhrase = Question.kGuidPrefix + guidOfAddedQuestion;
            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
            customizations.Add(pc);

            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[1];
            qs.Items[0] = CreateSection("ACT 1.1-6", "Acts 1:1-6 Introduction to the book.", 44001001,
                44001006, 0, 1);
            Question q = qs.Items[0].Categories[0].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "What question did the apostles ask Jesus about his kingdom?";
            q.Answers = new[] { "Stuff." };

            MasterQuestionParser qp = new MasterQuestionParser(qs, null, null, customizations, null);

            ParsedQuestions pq = qp.Result;
            List<Question> questions = pq.Sections.Items[0].Categories[0].Questions;
            Assert.AreEqual(2, questions.Count);
            for (int iQ = 0; iQ < questions.Count; iQ++)
            {
                Question actQuestion = questions[iQ];
                Assert.IsFalse(actQuestion.IsExcluded);
                Assert.IsNull(actQuestion.ModifiedPhrase);

                switch (iQ)
                {
                    case 0:
                        Assert.IsTrue(actQuestion.IsUserAdded);
                        Assert.AreEqual(Question.kGuidPrefix + guidOfAddedQuestion, actQuestion.PhraseInUse);
                        Assert.AreEqual(0, actQuestion.ParsedParts.Count);
                        break;
                    case 1:
                        Assert.IsFalse(actQuestion.IsUserAdded);
                        Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?", actQuestion.PhraseInUse);
                        Assert.AreEqual(actQuestion.Text, actQuestion.PhraseInUse);
                        Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        break;
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(1, pq.TranslatableParts.Length);
        }

        #region Private helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the mocked key term.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        private void AddMockedKeyTerm(string term, params int[] occurences)
        {
            if (occurences.Length > 0)
            {
                if (m_keyTermRules == null)
                {
                    m_keyTermRules = new KeyTermRules();
                    m_keyTermRules.Items = new List<KeyTermRule>();
                }
                KeyTermRule rule = new KeyTermRule();
                rule.id = term;
                rule.Rule = KeyTermRule.RuleType.MatchForRefOnly;
                m_keyTermRules.Items.Add(rule);
               // m_testFileAccessor.Data = 
            }
            IKeyTerm mockedKt = KeyTermMatchBuilderTests.AddMockedKeyTerm(term, occurences);
            m_dummyKtList.Add(mockedKt);
            //return mockedKt;
        }

        internal static QuestionSections GenerateStandardQuestionSections()
        {
            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[2];
            int iS = 0;
            qs.Items[iS] = CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
                                         44001005, 2, 1);
            int iC = 0;
            Question q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "What information did Luke, the writer of this book, give in this introduction?";
            q.Answers = new[] { "Luke reminded his readers that he was about to continue the true story about Jesus" };
            q = qs.Items[iS].Categories[iC].Questions[1];
            q.Text = "What do you think an apostle of Jesus is?";
            q.Answers = new[] { "Key Term Check: To be an apostle of Jesus means to be a messenger", "Can also be translated as \"sent one\"" };
            q.Notes = new[] { "Note: apostles can be real sweethearts sometimes" };

            iC = 1;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "To whom did the writer of Acts address this book?";
            q.Answers = new[] { "He addressed this book to Theophilus." };

            iS = 1;
            qs.Items[iS] = CreateSection("ACT 1.6-10", "Acts 1:6-10 The continuing saga.", 44001006, 44001010, 1, 1);
            iC = 0;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "What happened?";
            q.Answers = new[] { "Stuff" };

            iC = 1;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "What question did the apostles ask Jesus about his kingdom?";
            q.Answers = new[]
                {
                    "The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom."
                };
            return qs;
        }

        internal static QuestionSections GenerateSimpleSectionWithSingleCategory(int numberOfDetailQuestions)
        {
            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[1];
            int iS = 0;
            qs.Items[iS] = CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
                44001005, 0, numberOfDetailQuestions);
            return qs;
        }

		internal static Section CreateSection(string sRef, string heading, int startRef, int endRef, int cOverviewQuestions, int cDetailQuestions)
		{
			Section s = new Section();
			s.ScriptureReference = sRef;
			s.Heading = heading;
			s.StartRef = startRef;
			s.EndRef = endRef;
            s.Categories = new Category[(cOverviewQuestions > 0 ? 1 : 0) + (cDetailQuestions > 0 ? 1 : 0)];
		    int iC = 0;
		    if (cOverviewQuestions > 0)
		    {
		        s.Categories[iC] = new Category();
		        s.Categories[iC].Type = "Overview";
		        for (int i = 0; i < cOverviewQuestions; i++)
		            s.Categories[iC].Questions.Add(new Question());
		        iC++;
		    }

		    if (cDetailQuestions > 0)
		    {
		        s.Categories[iC] = new Category();
		        s.Categories[iC].Type = "Details";
		        for (int i = 0; i < cDetailQuestions; i++)
		            s.Categories[iC].Questions.Add(new Question());
		    }
            return s;
        }

        private void VerifyQuestionSections(ParsedQuestions pq)
        {
            QuestionSections actual = pq.Sections;
            QuestionSections expected = GenerateStandardQuestionSections();

            Assert.AreEqual(expected.Items.Length, actual.Items.Length);
            for (int iS = 0; iS < expected.Items.Length; iS++)
            {
                Section expSection = expected.Items[iS];
                Section actSection = actual.Items[iS];
                Assert.AreEqual(expSection.Heading, actSection.Heading);
                Assert.AreEqual(expSection.StartRef, actSection.StartRef);
                Assert.AreEqual(expSection.EndRef, actSection.EndRef);
                Assert.AreEqual(expSection.ScriptureReference, actSection.ScriptureReference);
                Assert.AreEqual(expSection.Categories.Length, actSection.Categories.Length);
                for (int iC = 0; iC < expSection.Categories.Length; iC++)
                {
                    Category expCategory = expSection.Categories[iC];
                    Category actCategory = actSection.Categories[iC];
                    Assert.AreEqual(expCategory.IsOverview, actCategory.IsOverview);
                    Assert.AreEqual(expCategory.Type, actCategory.Type);
                    Assert.IsTrue(expCategory.Questions.Count <= actCategory.Questions.Count);
                    int iExpQ = 0;
                    for (int iActQ = 0; iActQ < actCategory.Questions.Count; iActQ++)
                    {
                        Question actQuestion = actCategory.Questions[iActQ];
                        if (actQuestion.IsUserAdded)
                            continue;
                        Question expQuestion = expCategory.Questions[iExpQ];
                        Assert.AreEqual(expQuestion.Text, actQuestion.Text);
                        if (expQuestion.ScriptureReference != null)
                        {
                            Assert.AreEqual(expQuestion.StartRef, actQuestion.StartRef);
                            Assert.AreEqual(expQuestion.EndRef, actQuestion.EndRef);
                            Assert.AreEqual(expQuestion.ScriptureReference, actQuestion.ScriptureReference);
                        }
                        Assert.IsTrue((expQuestion.Answers == null && actQuestion.Answers == null) ||
                            expQuestion.Answers.SequenceEqual(actQuestion.Answers));
                        Assert.IsTrue((expQuestion.Notes == null && actQuestion.Notes == null) ||
                            expQuestion.Notes.SequenceEqual(actQuestion.Notes));
                        Assert.IsTrue((expQuestion.AlternateForms == null && actQuestion.AlternateForms == null) ||
                            expQuestion.AlternateForms.SequenceEqual(actQuestion.AlternateForms));
                        iExpQ++;

                        foreach (string part in actQuestion.ParsedParts.Where(pp => pp.Type == PartType.TranslatablePart).Select(pp => pp.Text))
                            Assert.IsTrue(pq.TranslatableParts.Contains(part));
                    }
                }
            }
        }
		#endregion
	}
}
