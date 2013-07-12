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
	    private List<Word> m_questionWords;
            
        [SetUp]
        public void Setup()
        {
            m_dummyKtList = new List<IKeyTerm>();
            m_keyTermRules = null;
            m_questionWords = new List<Word>(new Word[] { "who", "what", "when", "why", "how", "where", "which" });
        }

        #region Parsing tests
        ///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests enumerating overview and detail categories and questions with answers and
		/// comments. Note: this test does not use the standard question words; otherwise, some
		/// questions would be broken into 2 parts.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_NoKeyTermsOrCustomizationsAndAllQuestionsUnique_EachQuestionHasOneTranslatablePart()
		{
		    MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
                new List<Word>(), null, null, null, null);

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
                m_questionWords, m_dummyKtList, m_keyTermRules, null, null);

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
                        int pp;
                        switch (iQuestion)
                        {
                            case 0:
                                Assert.AreEqual(4, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("information did", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("luke", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("the writer of this book give in this introduction", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 1:
                                Assert.AreEqual(6, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("do you think an", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("apostle", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("of", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("jesus", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("is", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 2:
                                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                                break;
                            case 3:
                                Assert.AreEqual(2, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("happened", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 4:
                                Assert.AreEqual(6, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("question did the", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("apostle", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("ask", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("jesus", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("about his kingdom", actQuestion.ParsedParts[pp++].Text);
                                break;
                        }
                    }
                }
            }

            Assert.AreEqual(3, pq.KeyTerms.Length);
            Assert.AreEqual(11, pq.TranslatableParts.Length);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests parsing questions using a set of key terms
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_KeyTermsWithNoRules_KeyTermsAndPartsBreakQuestionsIntoTranslatableParts()
        {
            AddMockedKeyTerm("isaac");
            AddMockedKeyTerm("paul");

            var qs = GenerateSimpleSectionWithSingleCategory(9);
            var cat = qs.Items[0].Categories[0];
            var q1 = cat.Questions[0];
            q1.Text = "Now what?";
            q1.ScriptureReference = "A";
            q1.StartRef = 1;
            q1.EndRef = 1;
            var q2 = cat.Questions[1];
            q2.Text = "What did Isaac say?";
            q2.ScriptureReference = "B";
            q2.StartRef = 2;
            q2.EndRef = 2;
            var q3 = cat.Questions[2];
            q3.Text = "What could Isaac say?";
            q3.ScriptureReference = "C";
            q3.StartRef = 3;
            q3.EndRef = 3;
            var q4 = cat.Questions[3];
            q4.Text = "So now what did those two brothers do?";
            q4.ScriptureReference = "D";
            q4.StartRef = 4;
            q4.EndRef = 4;
            var q5 = cat.Questions[4];
            q5.Text = "So what could they do about the problem?";
            q5.ScriptureReference = "E";
            q5.StartRef = 5;
            q5.EndRef = 5;
            var q6 = cat.Questions[5];
            q6.Text = "So what did he do?";
            q6.ScriptureReference = "F";
            q6.StartRef = 6;
            q6.EndRef = 6;
            var q7 = cat.Questions[6];
            q7.Text = "So now what was Isaac complaining about?";
            q7.ScriptureReference = "G";
            q7.StartRef = 7;
            q7.EndRef = 7;
            var q8 = cat.Questions[7];
            q8.Text = "So what did the Apostle Paul say about that?";
            q8.ScriptureReference = "H";
            q8.StartRef = 8;
            q8.EndRef = 8;
            var q9 = cat.Questions[8];
            q9.Text = "Why did they treat the Apostle Paul so?";
            q9.ScriptureReference = "I";
            q9.StartRef = 9;
            q9.EndRef = 9;

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, null, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(2, pq.KeyTerms.Length);
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "isaac"));
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "paul"));
            Assert.AreEqual(15, pq.TranslatableParts.Length);

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
                        int pp;
                        switch (iQuestion)
                        {
                            case 0:
                                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("now what", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 1:
                                Assert.AreEqual(4, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("did", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("isaac", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("say", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 2:
                                Assert.AreEqual(4, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("could", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("isaac", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("say", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 3:
                                Assert.AreEqual(3, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("now what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("did those two brothers do", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 4:
                                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so what could they do about the problem", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 5:
                                Assert.AreEqual(1, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so what did he do", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 6:
                                Assert.AreEqual(5, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("now what", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("was", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("isaac", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("complaining about", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 7:
                                Assert.AreEqual(3, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so what did the apostle", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("paul", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("say about that", actQuestion.ParsedParts[pp++].Text);
                                break;
                            case 8:
                                Assert.AreEqual(4, actQuestion.ParsedParts.Count);
                                pp = 0;
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("why", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("did they treat the apostle", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.KeyTerm, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("paul", actQuestion.ParsedParts[pp++].Text);
                                Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[pp].Type);
                                Assert.AreEqual("so", actQuestion.ParsedParts[pp++].Text);
                                break;
                        }
                    }
                }
            }
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

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, m_keyTermRules, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(3, pq.KeyTerms.Length);
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "god"));
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "paul"));
            Assert.IsFalse(pq.KeyTerms.Any(kt => kt.TermId == "have"));
            Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "say"));
            Assert.AreEqual(13, pq.TranslatableParts.Length);

            Assert.AreEqual(7, q1.ParsedParts.Count);
            int pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[pp].Type);
            Assert.AreEqual("what", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[pp].Type);
            Assert.AreEqual("would", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[pp].Type);
            Assert.AreEqual("god", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[pp].Type);
            Assert.AreEqual("have me to", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[pp].Type);
            Assert.AreEqual("say", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[pp].Type);
            Assert.AreEqual("with respect to", q1.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q1.ParsedParts[pp].Type);
            Assert.AreEqual("paul", q1.ParsedParts[pp++].Text);

            Assert.AreEqual(7, q2.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("what", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("is", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q2.ParsedParts[pp].Type);
            Assert.AreEqual("paul", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("asking me to", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q2.ParsedParts[pp].Type);
            Assert.AreEqual("say", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("with respect to", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("that dog", q2.ParsedParts[pp++].Text);

            Assert.AreEqual(1, q3.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q3.ParsedParts[0].Type);
            Assert.AreEqual("that dog", q3.ParsedParts[0].Text);

            Assert.AreEqual(6, q4.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("is it okay for", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q4.ParsedParts[pp].Type);
            Assert.AreEqual("paul", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("me to talk", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("with respect to", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q4.ParsedParts[pp].Type);
            Assert.AreEqual("god", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("today", q4.ParsedParts[pp++].Text);

            Assert.AreEqual(6, q5.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[pp].Type);
            Assert.AreEqual("that dog", q5.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[pp].Type);
            Assert.AreEqual("wishes this", q5.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q5.ParsedParts[pp].Type);
            Assert.AreEqual("paul", q5.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[pp].Type);
            Assert.AreEqual("and what is", q5.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q5.ParsedParts[pp].Type);
            Assert.AreEqual("say", q5.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q5.ParsedParts[pp].Type);
            Assert.AreEqual("radish", q5.ParsedParts[pp++].Text);

            Assert.AreEqual(3, q6.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q6.ParsedParts[pp].Type);
            Assert.AreEqual("what", q6.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q6.ParsedParts[pp].Type);
            Assert.AreEqual("is", q6.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q6.ParsedParts[pp].Type);
            Assert.AreEqual("that dog", q6.ParsedParts[pp++].Text);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests breaking up phrases into parts where an empty key terms list is supplied.
        /// One phrase (with punctuation and extra spaces removed) is a substring of others.
        /// One phrase is an empty string.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_NoKeyTermsButOnePhraseIsSubPhraseOfOthers_PhrasesBrokenIntoParts()
        {
            var qs = GenerateSimpleSectionWithSingleCategory(4);
            var cat = qs.Items[0].Categories[0];
            var q1 = cat.Questions[0];
            q1.Text = " What do  you think?";
            q1.ScriptureReference = "A";
            q1.StartRef = 1;
            q1.EndRef = 1;
            var q2 = cat.Questions[1];
            q2.Text = "What  do you think it means to forgive?";
            q2.ScriptureReference = "B";
            q2.StartRef = 2;
            q2.EndRef = 2;
            var q3 = cat.Questions[2];
            q3.Text = "Did he ask, \"What do you think about this?\"";
            q3.ScriptureReference = "C";
            q3.StartRef = 3;
            q3.EndRef = 3;
            var q4 = cat.Questions[3];
            q4.Text = "What do you think  it means to bless someone? ";
            q4.ScriptureReference = "D";
            q4.StartRef = 4;
            q4.EndRef = 4;

            MasterQuestionParser qp = new MasterQuestionParser(qs, new List<Word>(), m_dummyKtList, m_keyTermRules, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(0, pq.KeyTerms.Length);
            Assert.AreEqual(5, pq.TranslatableParts.Length);

            Assert.AreEqual(1, q1.ParsedParts.Count);
            int pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[pp].Type);
            Assert.AreEqual("what do you think", q1.ParsedParts[pp++].Text);

            Assert.AreEqual(2, q2.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("what do you think", q2.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[pp].Type);
            Assert.AreEqual("it means to forgive", q2.ParsedParts[pp++].Text);

            Assert.AreEqual(3, q3.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q3.ParsedParts[pp].Type);
            Assert.AreEqual("did he ask", q3.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q3.ParsedParts[pp].Type);
            Assert.AreEqual("what do you think", q3.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q3.ParsedParts[pp].Type);
            Assert.AreEqual("about this", q3.ParsedParts[pp++].Text);

            Assert.AreEqual(2, q4.ParsedParts.Count);
            pp = 0;
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("what do you think", q4.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q4.ParsedParts[pp].Type);
            Assert.AreEqual("it means to bless someone", q4.ParsedParts[pp++].Text);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests breaking up phrases into parts where an empty key terms list is supplied.
        /// One phrase is an empty string.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_EmptyString_Omitted()
        {
            var qs = GenerateSimpleSectionWithSingleCategory(3);
            var cat = qs.Items[0].Categories[0];
            var q1 = cat.Questions[0];
            q1.Text = "First of all, this is not even a question.";
            q1.ScriptureReference = "A";
            q1.StartRef = 1;
            q1.EndRef = 1;
            var q2 = cat.Questions[1];
            q2.ScriptureReference = "B";
            q2.StartRef = 2;
            q2.EndRef = 2;
            var q3 = cat.Questions[2];
            q3.Text = "Why so few questions?";
            q3.ScriptureReference = "C";
            q3.StartRef = 3;
            q3.EndRef = 3;

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, m_keyTermRules, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(0, pq.KeyTerms.Length);
            Assert.AreEqual(3, pq.TranslatableParts.Length);

            Assert.AreEqual(2, pq.Sections.Items[0].Categories[0].Questions.Count);
            q1 = pq.Sections.Items[0].Categories[0].Questions[0];
            Assert.AreEqual("First of all, this is not even a question.", q1.Text);
            Assert.AreEqual(1, q1.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q1.ParsedParts[0].Type);
            Assert.AreEqual("first of all this is not even a question", q1.ParsedParts[0].Text);

            q2 = pq.Sections.Items[0].Categories[0].Questions[1];
            Assert.AreEqual("Why so few questions?", q2.Text);
            Assert.AreEqual(2, q2.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[0].Type);
            Assert.AreEqual("why", q2.ParsedParts[0].Text);
            Assert.AreEqual(PartType.TranslatablePart, q2.ParsedParts[1].Type);
            Assert.AreEqual("so few questions", q2.ParsedParts[1].Text);
        }

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where a non-empty key terms list is supplied
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_KeyTermsList_NoPhraseSubstitutions()
        //{
        //    AddMockedKeyTerm("John"); // The apostle
        //    AddMockedKeyTerm("John"); // The Baptist
        //    AddMockedKeyTerm("Paul");
        //    AddMockedKeyTerm("Mary");
        //    AddMockedKeyTerm("temple");
        //    AddMockedKeyTerm("forgive");
        //    AddMockedKeyTerm("bless");
        //    AddMockedKeyTerm("God");
        //    AddMockedKeyTerm("Jesus");
        //    AddMockedKeyTerm("sin");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Who was John?"), 
        //        new TranslatablePhrase("Who was Paul?"),
        //        new TranslatablePhrase("Who was Mary?"),
        //        new TranslatablePhrase("Who went to the well?"),
        //        new TranslatablePhrase("Who went to the temple?"),
        //        new TranslatablePhrase("What do you think it means to forgive?"),
        //        new TranslatablePhrase("What do you think it means to bless someone?"),
        //        new TranslatablePhrase("What do you think God wants you to do?"),
        //        new TranslatablePhrase("Why do you think God created man?"),
        //        new TranslatablePhrase("Why do you think God  sent Jesus to earth?"),
        //        new TranslatablePhrase("Who went to the well with Jesus?"),
        //        new TranslatablePhrase("Do you think God could forgive someone who sins?"),
        //        new TranslatablePhrase("What do you think it means to serve two masters?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(13, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Who was John?", "who was", 3);
        //    VerifyTranslatablePhrase(pth, "Who was Paul?", "who was", 3);
        //    VerifyTranslatablePhrase(pth, "Who was Mary?", "who was", 3);
        //    VerifyTranslatablePhrase(pth, "Who went to the well?", "who went to the well", 2);
        //    VerifyTranslatablePhrase(pth, "Who went to the temple?", "who went to the", 1);
        //    VerifyTranslatablePhrase(pth, "What do you think it means to forgive?", "what do you think it means to", 3);
        //    VerifyTranslatablePhrase(pth, "What do you think it means to bless someone?", "what do you think it means to", 3, "someone", 1);
        //    VerifyTranslatablePhrase(pth, "What do you think God wants you to do?", "what", 1, "do you think", 2, "wants you to do", 1);
        //    VerifyTranslatablePhrase(pth, "Why do you think God created man?", "why do you think", 2, "created man", 1);
        //    VerifyTranslatablePhrase(pth, "Why do you think God  sent Jesus to earth?", "why do you think", 2, "sent", 1, "to earth", 1);
        //    VerifyTranslatablePhrase(pth, "Who went to the well with Jesus?", "who went to the well", 2, "with", 1);
        //    VerifyTranslatablePhrase(pth, "Do you think God could forgive someone who sins?", "do you think", 2, "could", 1, "someone who", 1);
        //    VerifyTranslatablePhrase(pth, "What do you think it means to serve two masters?", "what do you think it means to", 3, "serve two masters", 1);
        //}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests breaking up phrases into parts where an empty key terms list is supplied and
        /// there are phrases ignored/substituted.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_EmptyKeyTermsList_PhraseSubstitutions()
        {
            List<Substitution> substitutions = new List<Substitution>();
            substitutions.Add(new Substitution("What do you think it means", "What means", false, false));
            substitutions.Add(new Substitution("tHe", null, false, false));
            substitutions.Add(new Substitution("do", string.Empty, false, false));

            var qs = GenerateSimpleSectionWithSingleCategory(6);
            var cat = qs.Items[0].Categories[0];
            int i = 0;
            cat.Questions[0].Text = "What do you think it means?";
            cat.Questions[1].Text = "What do you think it means to forgive?";
            //cat.Questions[2]; -- leave empty!
            cat.Questions[3].Text = "How could I have forgotten the question mark";
            cat.Questions[4].Text = "What do you think it means to bless someone? ";
            cat.Questions[5].Text = "What means of support do disciples have?";

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, m_keyTermRules, null, substitutions);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(0, pq.KeyTerms.Length);
            Assert.AreEqual(7, pq.TranslatableParts.Length);
            Assert.AreEqual(5, pq.Sections.Items[0].Categories[0].Questions.Count);

            i = 0;
            int pp = 0;
            Question q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means to forgive?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means to forgive", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("How could I have forgotten the question mark", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("how", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("could i have forgotten question mark", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means to bless someone?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means to bless someone", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What means of support do disciples have?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means of support disciples have", q.ParsedParts[pp++].Text);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests breaking up phrases into parts where a non-empty key terms list is supplied,
        /// with some more complicated muli-word terms, including some with apostrophes. 
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_KeyTermsList_Apostrophes()
        {
            AddMockedKeyTerm("(God described as the) Most High");
            AddMockedKeyTerm("(God of) hosts");
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("heavenly creature, symbol of God's majesty and associated with his presence");
            AddMockedKeyTerm("(one's own) burial-place");
            AddMockedKeyTerm("to make straight (one's way)");
            AddMockedKeyTerm("love for one's fellow believer");

            var qs = GenerateSimpleSectionWithSingleCategory(9);
            var cat = qs.Items[0].Categories[0];
            int i = 0;
            cat.Questions[0].Text = "If one gives one's word and doesn't keep it, is it sin?";
            cat.Questions[1].Text = "Is there one way to heaven?";
            cat.Questions[2].Text = "Is the Bible God's Word?";
            cat.Questions[3].Text = "Who is God's one and only Son?";
            cat.Questions[4].Text = "Can the God of hosts also be called the Most High God?";
            cat.Questions[5].Text = "Should one be buried in one's own burial-place?";
            cat.Questions[6].Text = "Do wise people make straight paths for themselves?";
            cat.Questions[7].Text = "How can you tell if one has love for one's fellow believer?";
            cat.Questions[8].Text = "Is the earth God's?";

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, m_keyTermRules, null, null);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(6, pq.KeyTerms.Length);
            Assert.AreEqual(15, pq.TranslatableParts.Length);
            Assert.AreEqual(9, pq.Sections.Items[0].Categories[0].Questions.Count);

            i = 0;
            int pp = 0;
            Question q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("If one gives one's word and doesn't keep it, is it sin?", q.Text);
            Assert.AreEqual(1, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("if one gives one's word and doesn't keep it is it sin", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Is there one way to heaven?", q.Text);
            Assert.AreEqual(1, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("is there one way to heaven", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Is the Bible God's Word?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("is the bible", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("word", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who is God's one and only Son?", q.Text);
            Assert.AreEqual(4, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("is", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("one and only son", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Can the God of hosts also be called the Most High God?", q.Text);
            Assert.AreEqual(5, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("can the", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god of hosts", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("also be called the", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("most high", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Should one be buried in one's own burial-place?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("should one be buried in", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("one's own burial-place", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Do wise people make straight paths for themselves?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("do wise people", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("make straight", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("paths for themselves", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("How can you tell if one has love for one's fellow believer?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("how", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("can you tell if one has", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("love for one's fellow believer", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Is the earth God's?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("is the earth", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests breaking up phrases into parts where a non-empty key terms list is supplied
        /// and there are substitutions defined with regular expressions and case-sensitive
        /// matching.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetResult_KeyTermsList_SimplePhraseSubstitutions()
        {
            AddMockedKeyTerm("John"); // The apostle
            AddMockedKeyTerm("John"); // The Baptist
            AddMockedKeyTerm("Paul");
            AddMockedKeyTerm("Mary");
            AddMockedKeyTerm("temple");
            AddMockedKeyTerm("forgive");
            AddMockedKeyTerm("bless");
            AddMockedKeyTerm("God");
            AddMockedKeyTerm("Jesus");
            AddMockedKeyTerm("sin");

            List<Substitution> substitutions = new List<Substitution>();
            substitutions.Add(new Substitution("What do you think it means", "What means", false, false));
            substitutions.Add(new Substitution("the", null, false, false));
            substitutions.Add(new Substitution("do", string.Empty, false, false));

            var qs = GenerateSimpleSectionWithSingleCategory(13);
            var cat = qs.Items[0].Categories[0];
            int i = 0;
            cat.Questions[i++].Text = "Who was John?";
            cat.Questions[i++].Text = "Who was Paul?";
            cat.Questions[i++].Text = "Who was Mary?";
            cat.Questions[i++].Text = "Who went to the well?";
            cat.Questions[i++].Text = "Who went to the temple?";
            cat.Questions[i++].Text = "What do you think it means to forgive?";
            cat.Questions[i++].Text = "What do you think it means to bless someone?";
            cat.Questions[i++].Text = "What do you think God wants you to do?";
            cat.Questions[i++].Text = "Why do you think God created man?";
            cat.Questions[i++].Text = "Why do you think God  sent Jesus to the earth?";
            cat.Questions[i++].Text = "Who went to the well with Jesus?";
            cat.Questions[i++].Text = "Do you think God could forgive someone who sins?";
            cat.Questions[i].Text = "What do you think it means to serve two masters?";

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, m_dummyKtList, m_keyTermRules, null, substitutions);
            ParsedQuestions pq = qp.Result;
            Assert.AreEqual(9, pq.KeyTerms.Length);
            Assert.AreEqual(17, pq.TranslatableParts.Length);
            Assert.AreEqual(13, pq.Sections.Items[0].Categories[0].Questions.Count);

            i = 0;
            int pp = 0;
            Question q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who was John?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("was", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("john", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who was Paul?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("was", q.ParsedParts[pp++].Text);
            Assert.AreEqual("paul", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who was Mary?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("was", q.ParsedParts[pp++].Text);
            Assert.AreEqual("mary", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who went to the well?", q.Text);
            Assert.AreEqual(2, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("went to well", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who went to the temple?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("went to", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("temple", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means to forgive?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means to", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("forgive", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means to bless someone?", q.Text);
            Assert.AreEqual(4, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means to", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("bless", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("someone", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think God wants you to do?", q.Text);
            Assert.AreEqual(4, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("you think", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("wants you to", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Why do you think God created man?", q.Text);
            Assert.AreEqual(4, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("why", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("you think", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("created man", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Why do you think God  sent Jesus to the earth?", q.Text);
            Assert.AreEqual(6, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("why", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("you think", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("sent", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("jesus", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("to earth", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Who went to the well with Jesus?", q.Text);
            Assert.AreEqual(4, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("went to well", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("with", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("jesus", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("Do you think God could forgive someone who sins?", q.Text);
            Assert.AreEqual(6, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("you think", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("god", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("could", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("forgive", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("someone who", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.KeyTerm, q.ParsedParts[pp].Type);
            Assert.AreEqual("sin", q.ParsedParts[pp++].Text);

            pp = 0;
            q = pq.Sections.Items[0].Categories[0].Questions[i++];
            Assert.AreEqual("What do you think it means to serve two masters?", q.Text);
            Assert.AreEqual(3, q.ParsedParts.Count);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("what", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("means to", q.ParsedParts[pp++].Text);
            Assert.AreEqual(PartType.TranslatablePart, q.ParsedParts[pp].Type);
            Assert.AreEqual("serve two masters", q.ParsedParts[pp++].Text);
        }

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where a non-empty key terms list is supplied
        ///// and there are phrases ignored/substituted.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_KeyTermsList_ComplexPhraseSubstitutions()
        //{
        //    AddMockedKeyTerm("John"); // The apostle
        //    AddMockedKeyTerm("John"); // The Baptist
        //    AddMockedKeyTerm("Paul");
        //    AddMockedKeyTerm("Mary");
        //    AddMockedKeyTerm("altar");
        //    AddMockedKeyTerm("forgive");
        //    AddMockedKeyTerm("bless");
        //    AddMockedKeyTerm("God");
        //    AddMockedKeyTerm("Jesus");
        //    AddMockedKeyTerm("sin");

        //    List<Substitution> substitutions = new List<Substitution>();
        //    substitutions.Add(new Substitution("what do you think it means", "what means", false, true));
        //    substitutions.Add(new Substitution(@"\ban\b", "a", true, false));
        //    substitutions.Add(new Substitution(@"did (\S+) do", @"did $1", true, true));
        //    substitutions.Add(new Substitution(@"ed\b", null, true, true));

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("What did John do?"), 
        //        new TranslatablePhrase("What did Paul do?"), 
        //        new TranslatablePhrase("Who was Mary?"),
        //        new TranslatablePhrase("Who walked on a wall?"),
        //        new TranslatablePhrase("Who walked on an altar?"),
        //        new TranslatablePhrase("What do you think it means to forgive?"),
        //        new TranslatablePhrase("what do you think it means to bless someone?"),
        //        new TranslatablePhrase("Did Mary do the right thing?")},
        //        m_dummyKtList, m_keyTermRules, substitutions);

        //    Assert.AreEqual(8, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "What did John do?", "what did", 2);
        //    VerifyTranslatablePhrase(pth, "What did Paul do?", "what did", 2);
        //    VerifyTranslatablePhrase(pth, "Who was Mary?", "who was", 1);
        //    VerifyTranslatablePhrase(pth, "Who walked on a wall?", "who walk on a", 2, "wall", 1);
        //    VerifyTranslatablePhrase(pth, "Who walked on an altar?", "who walk on a", 2);
        //    VerifyTranslatablePhrase(pth, "What do you think it means to forgive?", "what do you think it means to", 1);
        //    VerifyTranslatablePhrase(pth, "what do you think it means to bless someone?", "what means to", 1, "someone", 1);
        //    VerifyTranslatablePhrase(pth, "Did Mary do the right thing?", "did", 1, "do the right thing", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where the key terms list contains some terms
        ///// consisting of more than one word
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_MultiWordKeyTermsList()
        //{
        //    AddMockedKeyTerm("to forgive (flamboyantly)");
        //    AddMockedKeyTerm("to forgive always and forever");
        //    AddMockedKeyTerm("high priest");
        //    AddMockedKeyTerm("God");
        //    AddMockedKeyTerm("sentence that is seven words long");
        //    AddMockedKeyTerm("sentence");
        //    AddMockedKeyTerm("seven");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("What do you think it means to forgive?"),
        //        new TranslatablePhrase("Bla bla bla to forgive always?"),
        //        new TranslatablePhrase("Please forgive!"),
        //        new TranslatablePhrase("Who do you think God wants you to forgive and why?"),
        //        new TranslatablePhrase("Can you say a sentence that is seven words long?"),
        //        new TranslatablePhrase("high priest"),
        //        new TranslatablePhrase("If the high priest wants you to forgive God, can he ask you using a sentence that is seven words long or not?"),
        //        new TranslatablePhrase("Is this sentence that is seven dwarves?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(8, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "What do you think it means to forgive?", "what do you think it means", 1);
        //    VerifyTranslatablePhrase(pth, "Bla bla bla to forgive always?", "bla bla bla", 1, "always", 1);
        //    VerifyTranslatablePhrase(pth, "Please forgive!", "please", 1);
        //    VerifyTranslatablePhrase(pth, "Who do you think God wants you to forgive and why?", "who do you think", 1, "wants you", 2, "and why", 1);
        //    VerifyTranslatablePhrase(pth, "Can you say a sentence that is seven words long?", "can you say a", 1);
        //    VerifyTranslatablePhrase(pth, "high priest");
        //    VerifyTranslatablePhrase(pth, "If the high priest wants you to forgive God, can he ask you using a sentence that is seven words long or not?",
        //        "if the", 1, "wants you", 2, "can he ask you using a", 1, "or not", 1);
        //    VerifyTranslatablePhrase(pth, "Is this sentence that is seven dwarves?", "is this", 1, "that is", 1, "dwarves", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where the phrase contains the word "Pharisees"
        ///// This deals with a weakness in the original (v1) Porter Stemmer algortihm. (TXL-52)
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_MatchPluralFormOfMultiSyllabicWordEndingInDoubleE()
        //{
        //    AddMockedKeyTerm("pharisee");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("What did the pharisee want?"),
        //        new TranslatablePhrase("What did the pharisees eat?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "What did the pharisee want?", "what did the", 2, "want", 1);
        //    VerifyTranslatablePhrase(pth, "What did the pharisees eat?", "what did the", 2, "eat", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where the key terms list contains a term
        ///// consisting of more than one word where there is a partial match that fails at the
        ///// end of the phrase.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_MultiWordKeyTermsList_AvoidFalseMatchAtEnd()
        //{
        //    AddMockedKeyTerm("John");
        //    AddMockedKeyTerm("tell the good news");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("What did John tell the Christians?"),
        //        new TranslatablePhrase("Why should you tell the good news?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "What did John tell the Christians?", "what did", 1, "tell the christians", 1);
        //    VerifyTranslatablePhrase(pth, "Why should you tell the good news?", "why should you", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where a two consectutive key terms appear in a
        ///// phrase
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_TwoConsecutiveKeyTerms()
        //{
        //    AddMockedKeyTerm("John");
        //    AddMockedKeyTerm("sin");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Did John sin when he told Herod that it was unlawful to marry Herodius?")}, 
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(1, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Did John sin when he told Herod that it was unlawful to marry Herodius?", "did", 1, "when he told herod that it was unlawful to marry herodius", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where a terms list is supplied that contains
        ///// words or morphemes that are optional (either explicitly indicated using parentheses
        ///// or implicitly optional words, such as the word "to" followed by a verb.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_KeyTermsWithOptionalWords()
        //{
        //    AddMockedKeyTerm("ask for (earnestly)");
        //    AddMockedKeyTerm("to sin");
        //    AddMockedKeyTerm("(things of) this life");
        //    AddMockedKeyTerm("(loving)kindness");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Did Herod ask for John's head because he wanted to sin?"),
        //        new TranslatablePhrase("Did Jambres sin when he clung to the things of this life?"),
        //        new TranslatablePhrase("Whose lovingkindness is everlasting?"),
        //        new TranslatablePhrase("What did John ask for earnestly?"),
        //        new TranslatablePhrase("Is showing kindness in this life a way to earn salvation?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(5, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Did Herod ask for John's head because he wanted to sin?", "did herod", 1, "john's head because he wanted", 1);
        //    VerifyTranslatablePhrase(pth, "Did Jambres sin when he clung to the things of this life?", "did jambres", 1, "when he clung to the", 1);
        //    VerifyTranslatablePhrase(pth, "Whose lovingkindness is everlasting?", "whose", 1, "is everlasting", 1);
        //    VerifyTranslatablePhrase(pth, "What did John ask for earnestly?", "what did john", 1);
        //    VerifyTranslatablePhrase(pth, "Is showing kindness in this life a way to earn salvation?", "is showing", 1, "in", 1, "a way to earn salvation", 1);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests breaking up phrases into parts where a terms list is supplied that contains
        ///// a phrase that begins with an inflected form of a verb and a term that contains a
        ///// one-word uninflected form of that word. Phrases that contain the inflected form of
        ///// the verb but do not macth the whole phrase should match the uninflected term.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_KeyTermsWithMultipleWordsAndPhrasesWithWordsWhichNeedToBeStemmed()
        //{
        //    m_keyTermRules = new KeyTermRules();
        //    m_keyTermRules.Items = new List<KeyTermRule>();

        //    AddMockedKeyTerm("Blessed One", 1);
        //    AddMockedKeyTerm("bless; praise", "bendito");
        //    KeyTermRule rule = new KeyTermRule();
        //    rule.id = "bless; praise";
        //    rule.Alternates = new[] { new KeyTermRulesKeyTermRuleAlternate { Name = "blessed" } };
        //    m_keyTermRules.Items.Add(rule);
        //    AddMockedKeyTerm("blessed; worthy of praise", "bienaventurado");
        //    AddMockedKeyTerm("Jacob");

        //    m_keyTermRules.Initialize();
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase(new TestQ("Who was the Blessed One?", "A", 1, 1), 1, 0),
        //        new TranslatablePhrase(new TestQ("Who is blessed?", "A", 1, 1), 1, 0),
        //        new TranslatablePhrase(new TestQ("Who was present when Jacob blessed one of his sons?", "B", 2, 2), 1, 0)},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(3, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Who was the Blessed One?", "who was the", 1);
        //    VerifyTranslatablePhrase(pth, "Who is blessed?", "who is", 1);
        //    VerifyTranslatablePhrase(pth, "Who was present when Jacob blessed one of his sons?", "who was present when", 1, "one of his sons", 1);
        //    TranslatablePhrase phr = pth.UnfilteredPhrases.FirstOrDefault(x => x.OriginalPhrase == "Who is blessed?");
        //    Assert.AreEqual(2, phr.GetParts().OfType<KeyTermMatch>().ElementAt(0).AllTerms.Count());
        //    Assert.IsTrue(rule.Used);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests sub-part matching logic in case where breaking a phrase into smaller subparts
        ///// causes a remainder which is an existing part (in use in another phrase).
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_SubPartBreakingCausesRemainderWhichIsAnExistingPart()
        //{
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Who was the man who went to the store?"),
        //        new TranslatablePhrase("Who was the man?"),
        //        new TranslatablePhrase("Who went to the store?"),
        //        new TranslatablePhrase("Who was the man with the goatee who went to the store?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(4, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Who was the man who went to the store?", "who was the man", 3, "who went to the store", 3);
        //    VerifyTranslatablePhrase(pth, "Who was the man?", "who was the man", 3);
        //    VerifyTranslatablePhrase(pth, "Who went to the store?", "who went to the store", 3);
        //    VerifyTranslatablePhrase(pth, "Who was the man with the goatee who went to the store?", "who was the man", 3, "with the goatee", 1, "who went to the store", 3);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests sub-part matching logic in case where breaking a phrase into smaller subparts
        ///// causes both a preceeding and a following remainder.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_SubPartMatchInTheMiddle()
        //{
        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Are you the one who knows the man who ate the monkey?"),
        //        new TranslatablePhrase("Who knows the man?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Are you the one who knows the man who ate the monkey?",
        //        "are you the one", 1, "who knows the man", 2, "who ate the monkey", 1);
        //    VerifyTranslatablePhrase(pth, "Who knows the man?", "who knows the man", 2);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests sub-part matching logic in case where a phrase could theoretically match a
        ///// sub-phrase  on smoething other than a word boundary.
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void GetResult_PreventMatchOfPartialWords()
        //{
        //    AddMockedKeyTerm("think");

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase("Was a man happy?"),
        //        new TranslatablePhrase("As a man thinks in his heart, how is he?")},
        //        m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(2, pth.Phrases.Count(), "Wrong number of phrases in helper");

        //    VerifyTranslatablePhrase(pth, "Was a man happy?", "was a man happy", 1);
        //    VerifyTranslatablePhrase(pth, "As a man thinks in his heart, how is he?", "as a man", 1, "in his heart how is he", 1);
        //}
        #endregion

        #region Constrain Key Terms to References tests
        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests constraining the use of key terms to only the applicable "verses"
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ConstrainByRef_Simple()
        //{
        //    AddMockedKeyTerm("God", 4);
        //    AddMockedKeyTerm("Paul", 1, 5);
        //    AddMockedKeyTerm("have", 99);
        //    AddMockedKeyTerm("say", 2, 5);

        //    m_keyTermRules.Initialize();

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase(new TestQ("What would God have me to say with respect to Paul?", "A", 1, 1), 1, 0),
        //        new TranslatablePhrase(new TestQ("What is Paul asking me to say with respect to that dog?", "B", 2, 2), 1, 0),
        //        new TranslatablePhrase(new TestQ("that dog", "C", 3, 3), 1, 0),
        //        new TranslatablePhrase(new TestQ("Is it okay for Paul to talk with respect to God today?", "D", 4, 4), 1, 0),
        //        new TranslatablePhrase(new TestQ("that dog wishes this Paul what is say radish", "E", 5, 5), 1, 0),
        //        new TranslatablePhrase(new TestQ("What is that dog?", "F", 6, 6), 1, 0)}, m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");
        //    VerifyTranslatablePhrase(pth, "What would God have me to say with respect to Paul?", "what would god have me to say with respect to", 1);
        //    VerifyTranslatablePhrase(pth, "What is Paul asking me to say with respect to that dog?", "what is", 3, "paul asking me to", 1, "with respect to", 1, "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "that dog", "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "Is it okay for Paul to talk with respect to God today?", "is it okay for paul to talk with respect to", 1, "today", 1);
        //    VerifyTranslatablePhrase(pth, "that dog wishes this Paul what is say radish", "that dog", 4, "wishes this", 1, "what is", 3, "radish", 1);
        //    VerifyTranslatablePhrase(pth, "What is that dog?", "what is", 3, "that dog", 4);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests constraining the use of key terms to only the applicable "verses"
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ConstrainByRef_RefRanges()
        //{
        //    AddMockedKeyTerm("God", 4);
        //    AddMockedKeyTerm("Paul", 1, 3, 5);
        //    AddMockedKeyTerm("have", 99);
        //    AddMockedKeyTerm("say", 2, 5);

        //    m_keyTermRules.Initialize();

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase(new TestQ("What would God have me to say with respect to Paul?", "A-D", 1, 4), 0, 0),
        //        new TranslatablePhrase(new TestQ("What is Paul asking me to say with respect to that dog?", "B", 2, 2), 1, 0),
        //        new TranslatablePhrase(new TestQ("that dog", "C", 3, 3), 1, 0),
        //        new TranslatablePhrase(new TestQ("Is it okay for Paul to talk with respect to God today?", "B-D", 2, 4), 0, 0),
        //        new TranslatablePhrase(new TestQ("that dog wishes this Paul what is say radish", "E", 5, 5), 1, 0),
        //        new TranslatablePhrase(new TestQ("What is that dog?", "E-F", 5, 6), 0, 0)}, m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");
        //    VerifyTranslatablePhrase(pth, "What would God have me to say with respect to Paul?", "what would", 1, "have me to", 1, "with respect to", 3);
        //    VerifyTranslatablePhrase(pth, "What is Paul asking me to say with respect to that dog?", "what is", 3, "paul asking me to", 1, "with respect to", 3, "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "that dog", "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "Is it okay for Paul to talk with respect to God today?", "is it okay for", 1, "to talk", 1, "with respect to", 3, "today", 1);
        //    VerifyTranslatablePhrase(pth, "that dog wishes this Paul what is say radish", "that dog", 4, "wishes this", 1, "what is", 3, "radish", 1);
        //    VerifyTranslatablePhrase(pth, "What is that dog?", "what is", 3, "that dog", 4);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests constraining the use of key terms to only the applicable "verses"
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ConstrainByRef_GodMatchesAnywhere()
        //{
        //    AddMockedKeyTerm("God");
        //    AddMockedKeyTerm("Paul", 1, 3, 5);
        //    AddMockedKeyTerm("have", 99);
        //    AddMockedKeyTerm("say", 2, 5);

        //    m_keyTermRules.Initialize();

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase(new TestQ("What would God have me to say with respect to Paul?", "A-D", 1, 4), 0, 0),
        //        new TranslatablePhrase(new TestQ("What is Paul asking me to say with respect to that dog?", "B", 2, 2), 1, 0),
        //        new TranslatablePhrase(new TestQ("that dog", "C", 3, 3), 1, 0),
        //        new TranslatablePhrase(new TestQ("Is it okay for Paul to talk with respect to God today?", "B-D", 2, 4), 0, 0),
        //        new TranslatablePhrase(new TestQ("that dog wishes this Paul what is say radish", "E", 5, 5), 1, 0),
        //        new TranslatablePhrase(new TestQ("What is that dog?", "E-F", 5, 6), 0, 0)}, m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");
        //    VerifyTranslatablePhrase(pth, "What would God have me to say with respect to Paul?", "what would", 1, "have me to", 1, "with respect to", 3);
        //    VerifyTranslatablePhrase(pth, "What is Paul asking me to say with respect to that dog?", "what is", 3, "paul asking me to", 1, "with respect to", 3, "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "that dog", "that dog", 4);
        //    VerifyTranslatablePhrase(pth, "Is it okay for Paul to talk with respect to God today?", "is it okay for", 1, "to talk", 1, "with respect to", 3, "today", 1);
        //    VerifyTranslatablePhrase(pth, "that dog wishes this Paul what is say radish", "that dog", 4, "wishes this", 1, "what is", 3, "radish", 1);
        //    VerifyTranslatablePhrase(pth, "What is that dog?", "what is", 3, "that dog", 4);
        //}

        ///// ------------------------------------------------------------------------------------
        ///// <summary>
        ///// Tests constraining the use of key terms to only the applicable "verses"
        ///// </summary>
        ///// ------------------------------------------------------------------------------------
        //[Test]
        //public void ConstrainByRef_ComplexKeyTerms()
        //{
        //    AddMockedKeyTerm("high priest", 1);
        //    AddMockedKeyTerm("high", 1, 2);
        //    AddMockedKeyTerm("radish", 1, 2);
        //    AddMockedKeyTerm("(to have) eaten or drunk", 2, 3);
        //    AddMockedKeyTerm("high or drunk sailor", 2, 4);

        //    m_keyTermRules.Initialize();

        //    PhraseTranslationHelper pth = new PhraseTranslationHelper(new[] {
        //        new TranslatablePhrase(new TestQ("Was the high priest on his high horse?", "A", 1, 1), 1, 0),
        //        new TranslatablePhrase(new TestQ("Who was the high priest?", "B", 2, 2), 1, 0),
        //        new TranslatablePhrase(new TestQ("I have eaten the horse.", "A", 1, 1), 1, 0),
        //        new TranslatablePhrase(new TestQ("How high is this?", "C", 3, 3), 1, 0),
        //        new TranslatablePhrase(new TestQ("That drunk sailor has eaten a radish", "C-D", 3, 4), 0, 0),
        //        new TranslatablePhrase(new TestQ("That high sailor was to have drunk some radish juice", "A-B", 1, 2), 0, 0)}, m_dummyKtList, m_keyTermRules, new List<Substitution>());

        //    Assert.AreEqual(6, pth.Phrases.Count(), "Wrong number of phrases in helper");
        //    VerifyTranslatablePhrase(pth, "Was the high priest on his high horse?", "was the", 2, "on his", 1, "horse", 1);
        //    VerifyTranslatablePhrase(pth, "Who was the high priest?", "who", 1, "was the", 2, "priest", 1);
        //    VerifyTranslatablePhrase(pth, "I have eaten the horse.", "i have eaten the horse", 1);
        //    VerifyTranslatablePhrase(pth, "How high is this?", "how high is this", 1);
        //    VerifyTranslatablePhrase(pth, "That drunk sailor has eaten a radish", "that", 2, "has", 1, "a radish", 1);
        //    VerifyTranslatablePhrase(pth, "That high sailor was to have drunk some radish juice", "that", 2, "was", 1, "some", 1, "juice", 1);
        //}
        #endregion

        #region Phrase-customization (additions, exclusions, modifications) tests
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
                new List<Word>(), null, null, customizations, null);

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
                new List<Word>(), null, null, customizations, null);

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
                new List<Word>(), null, null, customizations, null);

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
                new List<Word>(), null, null, customizations, null);

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
                            Assert.IsFalse(actQuestion.ParsedParts.Any(pp => pp.Type != PartType.TranslatablePart));
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

            MasterQuestionParser qp = new MasterQuestionParser(qs, new List<Word>(), null, null, customizations, null);

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

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, null, null, customizations, null);

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

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, null, null, customizations, null);

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

            MasterQuestionParser qp = new MasterQuestionParser(qs, m_questionWords, null, null, customizations, null);

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
                        Assert.AreEqual(2, actQuestion.ParsedParts.Count);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[1].Type);
                        break;
                }
            }

            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(2, pq.TranslatableParts.Length);
        }
        #endregion

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
            }
            IKeyTerm mockedKt = KeyTermMatchBuilderTests.AddMockedKeyTerm(term, occurences);
            m_dummyKtList.Add(mockedKt);
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
