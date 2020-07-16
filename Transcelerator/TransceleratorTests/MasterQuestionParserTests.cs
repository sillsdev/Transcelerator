// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2013' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//	
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: MasterQuestionParserTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddInSideViews;
using NUnit.Framework;
using SIL.Extensions;

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
		internal static List<string> s_questionWords = new List<string>(new[] { "who", "what", "when", "why", "how", "where", "which", "how many", "how much", "how long", "how old" });

		[SetUp]
		public void Setup()
		{
			m_dummyKtList = new List<IKeyTerm>();
			m_keyTermRules = null;
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
				new List<string>(), null, null, null, null);

			ParsedQuestions pq = qp.Result;
			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
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
				s_questionWords, m_dummyKtList, m_keyTermRules, null, null);

			ParsedQuestions pq = qp.Result;
			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
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
								VerifyParts(actQuestion, actQuestion.Text,
									"what",
									"information did",
									new KeyTermPart("luke"),
									"the writer of this book give in this introduction");
								break;
							case 1:
								VerifyParts(actQuestion, actQuestion.Text,
									"what",
									"do you think an",
									new KeyTermPart("apostle"),
									"of",
									new KeyTermPart("jesus"),
									"is");
								break;
							case 2:
								Assert.AreEqual(1, actQuestion.ParsedParts.Count);
								Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts[0].Type);
								break;
							case 3:
								VerifyParts(actQuestion, actQuestion.Text,
									"what",
									"happened");
								break;
							case 4:
								VerifyParts(actQuestion, actQuestion.Text,
									"what",
									"question did the",
									new KeyTermPart("apostle"),
									"ask",
									new KeyTermPart("jesus"),
									"about his kingdom");
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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, null, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(2, pq.KeyTerms.Length);
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "isaac"));
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "paul"));
			Assert.AreEqual(15, pq.TranslatableParts.Length);

			Section[] sections = pq.Sections.Items;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
					{
						Question actQuestion = actCategory.Questions[iQ];

						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.IsFalse(actQuestion.IsExcluded);
						Assert.IsFalse(actQuestion.IsUserAdded);
						switch (iQ)
						{
							case 0:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"now",
									"what");
								break;
							case 1:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"what",
									"did",
									new KeyTermPart("isaac"),
									"say");
								break;
							case 2:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"what",
									"could",
									new KeyTermPart("isaac"),
									"say");
								break;
							case 3:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"so",
									"now",
									"what",
									"did",
									"those two brothers do");
								break;
							case 4:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"so",
									"what",
									"could",
									"they do about the problem");
								break;
							case 5:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"so",
									"what",
									"did",
									"he do");
								break;
							case 6:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"so",
									"now",
									"what",
									"was",
									new KeyTermPart("isaac"),
									"complaining about");
								break;
							case 7:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"so",
									"what",
									"did",
									"the apostle",
									new KeyTermPart("paul"),
									"say",
									"about that");
								break;
							case 8:
								VerifyParts(actQuestion, actQuestion.PhraseInUse,
									"why",
									"did",
									"they treat the apostle",
									new KeyTermPart("paul"),
									"so");
								break;
							default:
								Assert.Fail("Unexpected question!");
								break;
						}
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing questions using a set of key terms where the questions have words that
		/// need to be stemmed to find matches
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_PossessiveFormsOfKeyTerms_StemmingResultsInAppropriateMatches()
		{
			AddMockedKeyTerm("apostle");
			AddMockedKeyTerm("Luke");
			AddMockedKeyTerm("believer");
			AddMockedKeyTerm("believe");

			var qs = GenerateSimpleSectionWithSingleCategory(3);
			var cat = qs.Items[0].Categories[0];
			var q1 = cat.Questions[0];
			q1.Text = "What was Luke's last name?";
			q1.ScriptureReference = "A";
			q1.StartRef = 1;
			q1.EndRef = 1;
			var q2 = cat.Questions[1];
			q2.Text = "Who should believers' friends be?";
			q2.ScriptureReference = "B";
			q2.StartRef = 2;
			q2.EndRef = 2;
			var q3 = cat.Questions[2];
			q3.Text = "What did the apostles say?";
			q3.ScriptureReference = "C";
			q3.StartRef = 3;
			q3.EndRef = 3;

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "apostle"));
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "luke"));
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "believer"));
			Assert.IsFalse(pq.KeyTerms.Any(kt => kt.TermId == "believe"));

			VerifyParts(q1, "What was Luke's last name?",
				"what",
				"was",
				new KeyTermPart("luke"),
				"last name");

			VerifyParts(q2, "Who should believers' friends be?",
				"who",
				"should",
				new KeyTermPart("believer"),
				"friends be");

			VerifyParts(q3, "What did the apostles say?",
				"what",
				"did the",
				new KeyTermPart("apostle"),
				"say");
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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "god"));
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "paul"));
			Assert.IsFalse(pq.KeyTerms.Any(kt => kt.TermId == "have"));
			Assert.IsTrue(pq.KeyTerms.Any(kt => kt.TermId == "say"));
			Assert.AreEqual(13, pq.TranslatableParts.Length);

			VerifyParts(q1, "What would God have me to say with respect to Paul?",
				"what",
				"would",
				new KeyTermPart("god"),
				"have me to",
				new KeyTermPart("say"),
				"with respect to",
				new KeyTermPart("paul"));

			VerifyParts(q2, "What is Paul asking me to say with respect to that dog?",
				"what",
				"is",
				new KeyTermPart("paul"),
				"asking me to",
				new KeyTermPart("say"),
				"with respect to",
				"that dog");

			VerifyParts(q3, "that dog",
				"that dog");

			VerifyParts(q4, "Is it okay for Paul me to talk with respect to God today?",
				"is",
				"it okay for",
				new KeyTermPart("paul"),
				"me to talk",
				"with respect to",
				new KeyTermPart("god"),
				"today");

			VerifyParts(q5, "that dog wishes this Paul and what is say radish",
				"that dog",
				"wishes this",
				new KeyTermPart("paul"),
				"and",
				"what",
				"is",
				new KeyTermPart("say"),
				"radish");

			VerifyParts(q6, "What is that dog?",
				"what",
				"is",
				"that dog");
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests enumerating questions with numbers.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_IncludesDigits_NumericPartsParsedCorrectly()
		{
			AddMockedKeyTerm("christian");
			AddMockedKeyTerm("jesus");
			AddMockedKeyTerm("jesus christ");

			var qs = GenerateQuestions(
				"Why did 11,000 people die?",
				"Were the 144.000 Christians or Jews?",
				"What did they do with Jesus' 12 baskets of leftover food?",
				"What does Jesus say in verses 4-6?");

			MasterQuestionParser qp = new MasterQuestionParser(qs, new List<string>(), m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(2, pq.KeyTerms.Length);
			Assert.AreEqual(8, pq.TranslatableParts.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Why did 11,000 people die?",
				"why did", 11000, "people die");

			VerifyParts(questions[i++], "Were the 144.000 Christians or Jews?",
				"were the", 144000, new KeyTermPart("christian"), "or jews");

			VerifyParts(questions[i++], "What did they do with Jesus' 12 baskets of leftover food?",
				"what did they do with", new KeyTermPart("jesus"), 12, "baskets of leftover food");

			VerifyParts(questions[i++], "What does Jesus say in verses 4-6?",
				"what does", new KeyTermPart("jesus"), "say in verses", 4, 6);
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
			var qs = GenerateQuestions(
				" What do  you think?",
				"What  do you think it means to forgive?",
				"Did he ask, \"What do you think about this?\"",
				"What do you think  it means to bless someone? ");

			MasterQuestionParser qp = new MasterQuestionParser(qs, new List<string>(), m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(0, pq.KeyTerms.Length);
			Assert.AreEqual(5, pq.TranslatableParts.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What do  you think?",
				"what do you think");

			VerifyParts(questions[i++], "What  do you think it means to forgive?",
				"what do you think",
				"it means to forgive");

			VerifyParts(questions[i++], "Did he ask, \"What do you think about this?\"",
				"did he ask",
				"what do you think",
				"about this");

			VerifyParts(questions[i++], "What do you think  it means to bless someone?",
				"what do you think",
				"it means to bless someone");
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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(0, pq.KeyTerms.Length);
			Assert.AreEqual(3, pq.TranslatableParts.Length);
			Assert.AreEqual(2, pq.Sections.Items[0].Categories[0].Questions.Count);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "First of all, this is not even a question.",
				"first of all this is not even a question");

			VerifyParts(questions[i++], "Why so few questions?",
				"why",
				"so few questions");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where a non-empty key terms list is supplied
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_KeyTermsList_NoPhraseSubstitutions()
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

			var pq = ParseQuestions(
				"Who was John?",
				"Who was Paul?",
				"Who was Mary?",
				"Who went to the well?",
				"Who went to the temple?",
				"What do you think it means to forgive?",
				"What do you think it means to bless someone?",
				"What do you think God wants you to do?",
				"Why do you think God created man?",
				"Why do you think God  sent Jesus to earth?",
				"Who went to the well with Jesus?",
				"Do you think God could forgive someone who sins?",
				"What do you think it means to serve two masters?");

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Who was John?",
				"who",
				"was",
				new KeyTermPart("john"));
			VerifyParts(pq.Sections.Items[0].Categories[0].Questions[i++], "Who was Paul?",
				"who",
				"was",
				new KeyTermPart("paul"));
			VerifyParts(pq.Sections.Items[0].Categories[0].Questions[i++], "Who was Mary?",
				"who",
				"was",
				new KeyTermPart("mary"));

			VerifyParts(questions[i++], "Who went to the well?",
				"who",
				"went to the",
				"well");
			
			VerifyParts(questions[i++], "Who went to the temple?",
				"who",
				"went to the",
				new KeyTermPart("temple"));

			VerifyParts(questions[i++], "What do you think it means to forgive?",
				"what",
				"do you think",
				"it means to",
				new KeyTermPart("forgive"));

			VerifyParts(questions[i++], "What do you think it means to bless someone?",
				"what",
				"do you think",
				"it means to",
				new KeyTermPart("bless"),
				"someone");

			VerifyParts(questions[i++], "What do you think God wants you to do?",
				"what",
				"do you think",
				new KeyTermPart("god"),
				"wants you to do");

			VerifyParts(questions[i++], "Why do you think God created man?",
				"why",
				"do you think",
				new KeyTermPart("god"),
				"created man");

			VerifyParts(questions[i++], "Why do you think God  sent Jesus to earth?",
				"why",
				"do you think",
				new KeyTermPart("god"),
				"sent",
				new KeyTermPart("jesus"),
				"to earth");

			VerifyParts(questions[i++], "Who went to the well with Jesus?",
				"who",
				"went to the",
				"well",
				"with",
				new KeyTermPart("jesus"));

			VerifyParts(questions[i++], "Do you think God could forgive someone who sins?",
				"do you think",
				new KeyTermPart("god"),
				"could",
				new KeyTermPart("forgive"),
				"someone",
				"who",
				new KeyTermPart("sin"));

			VerifyParts(questions[i++], "What do you think it means to serve two masters?",
				"what",
				"do you think",
				"it means to",
				"serve two masters");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where an empty key terms list is supplied and
		/// there are phrases ignored/substituted.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_EmptyKeyTermsList_PhraseSubstitutions()
		{
			List<Substitution> substitutions = new List<Substitution>
				{
					new Substitution("What do you think it means", "What means", false, false),
					new Substitution("tHe", null, false, false),
					new Substitution("do", string.Empty, false, false)
				};

			var qs = GenerateSimpleSectionWithSingleCategory(6);
			var cat = qs.Items[0].Categories[0];
			int i = 0;
			cat.Questions[i++].Text = "What do you think it means?";
			cat.Questions[i++].Text = "What do you think it means to forgive?";
			i++; //cat.Questions[2]; -- leave empty!
			cat.Questions[i++].Text = "How could I have forgotten the question mark";
			cat.Questions[i++].Text = "What do you think it means to bless someone? ";
			cat.Questions[i++].Text = "What means of support do disciples have?";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, substitutions);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(0, pq.KeyTerms.Length);
			Assert.AreEqual(7, pq.TranslatableParts.Length);
			Assert.AreEqual(5, pq.Sections.Items[0].Categories[0].Questions.Count);

			i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What do you think it means?",
				"what",
				"means");

			VerifyParts(questions[i++], "What do you think it means to forgive?",
				"what",
				"means",
				"to forgive");

			VerifyParts(questions[i++], "How could I have forgotten the question mark",
				"how",
				"could i have forgotten question mark");

			VerifyParts(questions[i++], "What do you think it means to bless someone?",
				"what",
				"means",
				"to bless someone");

			VerifyParts(questions[i++], "What means of support do disciples have?",
				"what",
				"means",
				"of support disciples have");
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

			var pq = ParseQuestions(
				"If one gives one's word and doesn't keep it, is it sin?",
				"Is there one way to heaven?",
				"Is the Bible God's Word?",
				"Who is God's one and only Son?",
				"Can the God of hosts also be called the Most High God?",
				"Should one be buried in one's own burial-place?",
				"Do wise people make straight paths for themselves?",
				"How can you tell if one has love for one's fellow believer?",
				"Is the earth God's?");

			Assert.AreEqual(6, pq.KeyTerms.Length);
			Assert.AreEqual(17, pq.TranslatableParts.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "If one gives one's word and doesn't keep it, is it sin?",
				"if one gives one's",
				"word",
				"and doesn't keep it",
				"is",
				"it sin");

			VerifyParts(questions[i++], "Is there one way to heaven?",
				"is",
				"there one way to heaven");

			VerifyParts(questions[i++], "Is the Bible God's Word?",
				"is",
				"the bible",
				new KeyTermPart("god"),
				"word");

			VerifyParts(questions[i++], "Who is God's one and only Son?",
				"who",
				"is",
				new KeyTermPart("god"),
				"one and only son");

			VerifyParts(questions[i++], "Can the God of hosts also be called the Most High God?",
				"can the",
				new KeyTermPart("god of hosts"),
				"also be called the",
				new KeyTermPart("most high"),
				new KeyTermPart("god"));

			VerifyParts(questions[i++], "Should one be buried in one's own burial-place?",
				"should one be buried in",
				new KeyTermPart("one's own burial-place"));

			VerifyParts(questions[i++], "Do wise people make straight paths for themselves?",
				"do wise people",
				new KeyTermPart("make straight"),
				"paths for themselves");

			VerifyParts(questions[i++], "How can you tell if one has love for one's fellow believer?",
				"how",
				"can you tell if one has",
				new KeyTermPart("love for one's fellow believer"));

			VerifyParts(questions[i++], "Is the earth God's?",
				"is",
				"the earth",
				new KeyTermPart("god"));
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

			List<Substitution> substitutions = new List<Substitution>
				{
					new Substitution("What do you think it means", "What means", false, false),
					new Substitution("the", null, false, false),
					new Substitution("do", string.Empty, false, false)
				};

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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, substitutions);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(9, pq.KeyTerms.Length);
			Assert.AreEqual(13, pq.Sections.Items[0].Categories[0].Questions.Count);

			i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Who was John?",
				"who",
				"was",
				new KeyTermPart("john"));

			VerifyParts(questions[i++], "Who was Paul?",
				"who",
				"was",
				new KeyTermPart("paul"));

			VerifyParts(questions[i++], "Who was Mary?",
				"who",
				"was",
				new KeyTermPart("mary"));

			VerifyParts(questions[i++], "Who went to the well?",
				"who",
				"went to",
				"well");

			VerifyParts(questions[i++], "Who went to the temple?",
				"who",
				"went to",
				new KeyTermPart("temple"));

			VerifyParts(questions[i++], "What do you think it means to forgive?",
				"what",
				"means to",
				new KeyTermPart("forgive"));

			VerifyParts(questions[i++], "What do you think it means to bless someone?",
				"what",
				"means to",
				new KeyTermPart("bless"),
				"someone");

			VerifyParts(questions[i++], "What do you think God wants you to do?",
				"what",
				"you think",
				new KeyTermPart("god"),
				"wants you to");

			VerifyParts(questions[i++], "Why do you think God created man?",
				"why",
				"you think",
				new KeyTermPart("god"),
				"created man");

			VerifyParts(questions[i++], "Why do you think God  sent Jesus to the earth?",
				"why",
				"you think",
				new KeyTermPart("god"),
				"sent",
				new KeyTermPart("jesus"),
				"to earth");

			VerifyParts(questions[i++], "Who went to the well with Jesus?",
				"who",
				"went to",
				"well",
				"with",
				new KeyTermPart("jesus"));

			VerifyParts(questions[i++], "Do you think God could forgive someone who sins?",
				"you think",
				new KeyTermPart("god"),
				"could",
				new KeyTermPart("forgive"),
				"someone",
				"who",
				new KeyTermPart("sin"));

			VerifyParts(questions[i++], "What do you think it means to serve two masters?",
				"what",
				"means to",
				"serve two masters");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where a non-empty key terms list is supplied
		/// and there are phrases ignored/substituted.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_KeyTermsList_ComplexPhraseSubstitutions()
		{
			AddMockedKeyTerm("John"); // The apostle
			AddMockedKeyTerm("John"); // The Baptist
			AddMockedKeyTerm("Paul");
			AddMockedKeyTerm("Mary");
			AddMockedKeyTerm("altar");
			AddMockedKeyTerm("forgive");
			AddMockedKeyTerm("bless");
			AddMockedKeyTerm("God"); // not used
			AddMockedKeyTerm("Jesus"); // not used
			AddMockedKeyTerm("sin"); // not used

			List<Substitution> substitutions = new List<Substitution>
				{
					new Substitution("what do you think it means", "what means", false, true),
					new Substitution(@"\ban\b", "a", true, false),
					new Substitution(@"did (\S+) do", @"did $1", true, true),
					new Substitution(@"ed\b", null, true, true)
				};

			var qs = GenerateSimpleSectionWithSingleCategory(13);
			var cat = qs.Items[0].Categories[0];
			int i = 0;
			cat.Questions[i++].Text = "What did John do?";
			cat.Questions[i++].Text = "What did Paul do?";
			cat.Questions[i++].Text = "Who was Mary?";
			cat.Questions[i++].Text = "Who walked on a wall?";
			cat.Questions[i++].Text = "Who walked on an altar?";
			cat.Questions[i++].Text = "What do you think it means to forgive?";
			cat.Questions[i++].Text = "what do you think it means to bless someone?";
			cat.Questions[i].Text = "Did Mary do the right thing?";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, substitutions);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(6, pq.KeyTerms.Length, "Only the key terms actually used should be counted. (The two John's get combined into a single match.)");
			Assert.AreEqual(10, pq.TranslatableParts.Length);
			Assert.AreEqual(8, pq.Sections.Items[0].Categories[0].Questions.Count);

			i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What did John do?",
				"what",
				"did",
				new KeyTermPart("john"));

			VerifyParts(questions[i++], "What did Paul do?",
				"what",
				"did",
				new KeyTermPart("paul"));

			VerifyParts(questions[i++], "Who was Mary?",
				"who",
				"was",
				new KeyTermPart("mary"));

			VerifyParts(questions[i++], "Who walked on a wall?",
				"who",
				"walk on a",
				"wall");

			VerifyParts(questions[i++], "Who walked on an altar?",
				"who",
				"walk on a",
				new KeyTermPart("altar"));

			VerifyParts(questions[i++], "What do you think it means to forgive?",
				"what",
				"do you think it",
				"means to",
				new KeyTermPart("forgive"));

			VerifyParts(questions[i++], "what do you think it means to bless someone?",
				"what",
				"means to",
				new KeyTermPart("bless"),
				"someone");

			VerifyParts(questions[i++], "Did Mary do the right thing?",
				"did",
				new KeyTermPart("mary"),
				"do the right thing");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where the key terms list contains some terms
		/// consisting of more than one word
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_MultiWordKeyTermsList()
		{
			AddMockedKeyTerm("to forgive (flamboyantly)"); // Generates 2 used matches: "to forgive" and "forgive"
			AddMockedKeyTerm("to forgive always and forever"); // not used
			AddMockedKeyTerm("high priest");
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("sentence that is seven words long");
			AddMockedKeyTerm("sentence");
			AddMockedKeyTerm("seven");

			var pq = ParseQuestions(
				"What do you think it means to forgive?",
				"Bla bla bla to forgive always?",
				"Please forgive!",
				"Who do you think God wants you to forgive and why?",
				"Can you say a sentence that is seven words long?",
				"high priest",
				"If the high priest wants you to forgive God, can he ask you using a sentence that is seven words long or not?",
				"Is this sentence that is seven dwarves?"); 
			
			Assert.AreEqual(7, pq.KeyTerms.Length, "Only the key terms actually used should be counted.");
			
			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What do you think it means to forgive?",
				"what",
				"do you think",
				"it means",
				new KeyTermPart("to forgive"));

			VerifyParts(questions[i++], "Bla bla bla to forgive always?",
				"bla bla bla",
				new KeyTermPart("to forgive"),
				"always");

			VerifyParts(questions[i++], "Please forgive!",
				"please",
				new KeyTermPart("forgive"));

			VerifyParts(questions[i++], "Who do you think God wants you to forgive and why?",
				"who",
				"do you think",
				new KeyTermPart("god"),
				"wants you",
				new KeyTermPart("to forgive"),
				"and why");

			VerifyParts(questions[i++], "Can you say a sentence that is seven words long?",
				"can you say a",
				new KeyTermPart("sentence that is seven words long"));

			VerifyParts(questions[i++], "high priest",
				new KeyTermPart("high priest"));

			VerifyParts(questions[i++], "If the high priest wants you to forgive God, can he ask you using a sentence that is seven words long or not?",
				"if the",
				new KeyTermPart("high priest"),
				"wants you",
				new KeyTermPart("to forgive"),
				new KeyTermPart("god"),
				"can he ask you using a",
				new KeyTermPart("sentence that is seven words long"),
				"or not");

			VerifyParts(questions[i++], "Is this sentence that is seven dwarves?",
				"is this",
				new KeyTermPart("sentence"),
				"that is",
				new KeyTermPart("seven"),
				"dwarves");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where questions begin with two-word phrases
		/// that are treated as special "question words".
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_MultiWordQuestionWords_QuestionWordPhrasesKeptTogether()
		{
			var qs = GenerateSimpleSectionWithSingleCategory(4);
			var cat = qs.Items[0].Categories[0];
			int i = 0;
			cat.Questions[i++].Text = "How do you forgive?";
			cat.Questions[i++].Text = "How long is your nose?";
			cat.Questions[i++].Text = "How much did it cost?";
			cat.Questions[i++].Text = "How many frogs are there?";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(4, pq.Sections.Items[0].Categories[0].Questions.Count);

			i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "How do you forgive?",
				"how",
				"do you forgive");

			VerifyParts(questions[i++], "How long is your nose?",
				"how long",
				"is your nose");

			VerifyParts(questions[i++], "How much did it cost?",
				"how much",
				"did it cost");

			VerifyParts(questions[i++], "How many frogs are there?",
				"how many",
				"frogs are there");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where the phrase contains the word "Pharisees"
		/// This deals with a weakness in the original (v1) Porter Stemmer algortihm. (TXL-52)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_MatchPluralFormOfMultiSyllabicWordEndingInDoubleE()
		{
			AddMockedKeyTerm("pharisee");

			var pq = ParseQuestions(
				"What did the pharisee want?",
				"What did the pharisees eat?");

			Assert.AreEqual(1, pq.KeyTerms.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What did the pharisee want?",
				"what",
				"did the",
				new KeyTermPart("pharisee"),
				"want");

			VerifyParts(questions[i++], "What did the pharisees eat?",
				"what",
				"did the",
				new KeyTermPart("pharisee"),
				"eat");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where the key terms list contains a term
		/// consisting of more than one word where there is a partial match that fails at the
		/// end of the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_MultiWordKeyTermsList_AvoidFalseMatchAtEnd()
		{
			AddMockedKeyTerm("John");
			AddMockedKeyTerm("tell the good news");

			var pq = ParseQuestions(
				"What did John tell the Christians?",
				"Why should you tell the good news?");

			Assert.AreEqual(2, pq.KeyTerms.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What did John tell the Christians?",
				"what",
				"did",
				new KeyTermPart("john"),
				"tell the christians");

			VerifyParts(questions[i++], "Why should you tell the good news?",
				"why",
				"should you",
				new KeyTermPart("tell the good news"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where a two consectutive key terms appear in a
		/// phrase
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_TwoConsecutiveKeyTerms()
		{
			AddMockedKeyTerm("John");
			AddMockedKeyTerm("sin");

			var pq = ParseQuestions("Did John sin when he told Herod that it was unlawful to marry Herodius?");

			Assert.AreEqual(2, pq.KeyTerms.Length);

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Did John sin when he told Herod that it was unlawful to marry Herodius?",
				"did",
				new KeyTermPart("john"),
				new KeyTermPart("sin"),
				"when he told herod that it was unlawful to marry herodius");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where a terms list is supplied that contains
		/// words or morphemes that are optional (either explicitly indicated using parentheses
		/// or implicitly optional words, such as the word "to" followed by a verb.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_KeyTermsWithOptionalWords()
		{
			AddMockedKeyTerm("ask for (earnestly)");
			AddMockedKeyTerm("to sin");
			AddMockedKeyTerm("(things of) this life");
			AddMockedKeyTerm("(loving)kindness");

			var pq =ParseQuestions(
				"Did Herod ask for John's head because he wanted to sin?",
				"Did Jambres sin when he clung to the things of this life?",
				"Whose lovingkindness is everlasting?",
				"What did John ask for earnestly?",
				"Is showing kindness in this life a way to earn salvation?");

			Assert.AreEqual(8, pq.KeyTerms.Length, "Each used variation of key terms counted separately.");

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Did Herod ask for John's head because he wanted to sin?",
				"did herod",
				new KeyTermPart("ask for"),
				"john's head because he wanted",
				new KeyTermPart("to sin"));

			VerifyParts(questions[i++], "Did Jambres sin when he clung to the things of this life?",
				"did jambres",
				new KeyTermPart("sin"),
				"when he clung to the",
				new KeyTermPart("things of this life"));

			VerifyParts(questions[i++], "Whose lovingkindness is everlasting?",
				"whose",
				new KeyTermPart("lovingkindness"),
				"is everlasting");

			VerifyParts(questions[i++], "What did John ask for earnestly?",
				"what",
				"did john",
				new KeyTermPart("ask for earnestly"));

			VerifyParts(questions[i++], "Is showing kindness in this life a way to earn salvation?",
				"is showing",
				new KeyTermPart("kindness"),
				"in",
				new KeyTermPart("this life"),
				"a way to earn salvation");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests breaking up phrases into parts where a terms list is supplied that contains
		/// a phrase that begins with an inflected form of a verb and a term that contains a
		/// one-word uninflected form of that word. Phrases that contain the inflected form of
		/// the verb but do not macth the whole phrase should match the uninflected term.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_KeyTermsWithMultipleWordsAndPhrasesWithWordsWhichNeedToBeStemmed()
		{
			m_keyTermRules = new KeyTermRules();
			m_keyTermRules.Items = new List<KeyTermRule>();

			AddMockedKeyTerm("Blessed One", 1);
			AddMockedKeyTerm("bless; praise");
			KeyTermRule rule = new KeyTermRule();
			rule.id = "bless; praise";
			rule.Alternates = new[] { new KeyTermRulesKeyTermRuleAlternate { Name = "blessed" } };
			m_keyTermRules.Items.Add(rule);
			AddMockedKeyTerm("blessed; worthy of praise");
			AddMockedKeyTerm("Jacob");

			m_keyTermRules.Initialize();

			Assert.IsFalse(rule.Used);

			var qs = GenerateQuestions(
				"Who was the Blessed One?",
				"Who is blessed?",
				"Who was present when Jacob blessed one of his sons?");

			int i = 0;

			Question q = qs.Items[0].Categories[0].Questions[i++];
			q.StartRef = 1;
			q.EndRef = 1;
			q.ScriptureReference = "A";
			q = qs.Items[0].Categories[0].Questions[i++];
			q.StartRef = 1;
			q.EndRef = 1;
			q.ScriptureReference = "A";
			q = qs.Items[0].Categories[0].Questions[i++];
			q.StartRef = 2;
			q.EndRef = 2;
			q.ScriptureReference = "B";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);

			i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Who was the Blessed One?",
				"who",
				"was the",
				new KeyTermPart("blessed one"));
			VerifyParts(questions[i++], "Who is blessed?",
				"who",
				"is",
				new KeyTermPart("blessed"));
			VerifyParts(questions[i++], "Who was present when Jacob blessed one of his sons?",
				"who",
				"was present when",
				new KeyTermPart("jacob"),
				new KeyTermPart("blessed"),
				"one of his sons");

			Assert.IsTrue(rule.Used);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sub-part matching logic in case where breaking a phrase into smaller subparts
		/// causes a remainder which is an existing part (in use in another phrase).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_SubPartBreakingCausesRemainderWhichIsAnExistingPart()
		{
			var qs = GenerateQuestions(
				"Who was the man who went to the store?",
				"Who was the man?",
				"Who went to the store?",
				"Who was the man with the goatee who went to the store?");

			MasterQuestionParser qp = new MasterQuestionParser(qs, new List<string>(), m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Who was the man who went to the store?",
				"who was the man",
				"who went to the store");
			VerifyParts(questions[i++], "Who was the man?",
				"who was the man");
			VerifyParts(questions[i++], "Who went to the store?",
				"who went to the store");
			VerifyParts(questions[i++], "Who was the man with the goatee who went to the store?",
				"who was the man",
				"with the goatee",
				"who went to the store");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sub-part matching logic in case where breaking a phrase into smaller subparts
		/// causes both a preceeding and a following remainder.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_SubPartMatchInTheMiddle()
		{
			var qs = GenerateQuestions(
				"Are you the one who knows the man who ate the monkey?",
				"Who knows the man?");

			MasterQuestionParser qp = new MasterQuestionParser(qs, new List<string>(), m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Are you the one who knows the man who ate the monkey?",
				"are you the one",
				"who knows the man",
				"who ate the monkey");
			VerifyParts(questions[i++], "Who knows the man?",
				"who knows the man");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests sub-part matching logic in case where a phrase could theoretically match a
		/// sub-phrase  on smoething other than a word boundary.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetResult_PreventMatchOfPartialWords()
		{
			AddMockedKeyTerm("think");

			var pq = ParseQuestions(
				"Was a man happy?",
				"As a man thinks in his heart, how is he?");

			int i = 0;
			var questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Was a man happy?",
				"was a man happy");
			VerifyParts(questions[i++], "As a man thinks in his heart, how is he?",
				"as a man",
				new KeyTermPart("think"),
				"in his heart how is he");
		}
		#endregion

		#region Constrain Key Terms to References tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests constraining the use of key terms to only the applicable "verses"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConstrainByRef_Simple()
		{
			AddMockedKeyTerm("God", 4);
			AddMockedKeyTerm("Paul", 1, 5);
			AddMockedKeyTerm("have", 99);
			AddMockedKeyTerm("say", 2, 5);

			m_keyTermRules.Initialize();

			var qs = GenerateQuestions(
				"What would God have me to say with respect to Paul?",
				"What is Paul asking me to say with respect to that dog?",
				"that dog",
				"Is it okay for Paul to talk with respect to God today?",
				"that dog wishes this Paul what is say radish",
				"What is that dog?");

			var questions = qs.Items[0].Categories[0].Questions;
			for (int iQ = 0; iQ < questions.Count; iQ++)
			{
				Question q = questions[iQ];
				q.StartRef = iQ + 1;
				q.EndRef = iQ + 1;
				q.ScriptureReference = ((char)('A' + iQ)).ToString();
			}

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);

			// Note that in the second and fourth questions, the word "is" is not broken off as a separate part because
			// the search for subparts starts with long parts and works down to shorter parts. At the time these longer
			// parts are being checked, "is" has not yet been broken off into its own part. Later, when the short part
			// "what is" gets broken, then "is" becomes its own part, but we don't search for subparts iteratively. I'm
			// not 100% sure this is the correct design, but it does help prevent excessive fragmentation (and it's
			// more efficient).
			int i = 0;
			questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What would God have me to say with respect to Paul?",
				"what",
				"would god have me to say with respect to",
				new KeyTermPart("paul"));
			VerifyParts(questions[i++], "What is Paul asking me to say with respect to that dog?",
				"what",
				"is paul asking me to",
				new KeyTermPart("say"),
				"with respect to",
				"that dog");
			VerifyParts(questions[i++], "that dog",
				"that dog");
			VerifyParts(questions[i++], "Is it okay for Paul to talk with respect to God today?",
				"is it okay for paul to talk with respect to",
				new KeyTermPart("god"),
				"today");
			VerifyParts(questions[i++], "that dog wishes this Paul what is say radish",
				"that dog",
				"wishes this",
				new KeyTermPart("paul"),
				"what",
				"is",
				new KeyTermPart("say"),
				"radish");
			VerifyParts(questions[i++], "What is that dog?",
				"what",
				"is",
				"that dog");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests constraining the use of key terms to only the applicable "verses"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConstrainByRef_RefRanges()
		{
			AddMockedKeyTerm("God", 4);
			AddMockedKeyTerm("Paul", 1, 3, 5);
			AddMockedKeyTerm("have", 99);
			AddMockedKeyTerm("say", 2, 5);

			m_keyTermRules.Initialize();

			var qs = GenerateQuestions(
				"What would God have me to say with respect to Paul?",
				"What is Paul asking me to say with respect to that dog?",
				"that dog",
				"Is it okay for Paul to talk with respect to God today?",
				"that dog wishes this Paul what is say radish",
				"What is that dog?");

			int i = 0;
			var questions = qs.Items[0].Categories[0].Questions;

			Question q = questions[i++];
			q.StartRef = 1;
			q.EndRef = 4;
			q.ScriptureReference = "A-D";
			q = questions[i++];
			q.StartRef = 2;
			q.EndRef = 2;
			q.ScriptureReference = "B";
			q = questions[i++];
			q.StartRef = 3;
			q.EndRef = 3;
			q.ScriptureReference = "C";
			q = questions[i++];
			q.StartRef = 2;
			q.EndRef = 4;
			q.ScriptureReference = "B-D";
			q = questions[i++];
			q.StartRef = 5;
			q.EndRef = 5;
			q.ScriptureReference = "E";
			q = questions[i++];
			q.StartRef = 5;
			q.EndRef = 6;
			q.ScriptureReference = "E-F";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);

			i = 0;
			questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What would God have me to say with respect to Paul?",
				"what",
				"would",
				new KeyTermPart("god"),
				"have me to",
				new KeyTermPart("say"),
				"with respect to",
				new KeyTermPart("paul"));
			VerifyParts(questions[i++], "What is Paul asking me to say with respect to that dog?",
				"what",
				"is paul asking me to",
				new KeyTermPart("say"),
				"with respect to",
				"that dog");
			VerifyParts(questions[i++], "that dog",
				"that dog");
			VerifyParts(questions[i++], "Is it okay for Paul to talk with respect to God today?",
				"is it okay for",
				new KeyTermPart("paul"),
				"to talk",
				"with respect to",
				new KeyTermPart("god"),
				"today");
			VerifyParts(questions[i++], "that dog wishes this Paul what is say radish",
				"that dog",
				"wishes this",
				new KeyTermPart("paul"),
				"what",
				"is",
				new KeyTermPart("say"),
				"radish");
			VerifyParts(questions[i++], "What is that dog?",
				"what",
				"is",
				"that dog");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests constraining the use of key terms to only the applicable "verses"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConstrainByRef_GodMatchesAnywhere()
		{
			AddMockedKeyTerm("God");
			AddMockedKeyTerm("Paul", 1, 3, 5);
			AddMockedKeyTerm("have", 99);
			AddMockedKeyTerm("say", 2, 5);

			m_keyTermRules.Initialize();

			var qs = GenerateQuestions(
				"What would God have me to say with respect to Paul?",
				"What is Paul asking me to say with respect to that dog?",
				"that dog",
				"Is it okay for Paul to talk with respect to God today?",
				"that dog wishes this Paul what is say radish",
				"What is that dog?");

			int i = 0;
			var questions = qs.Items[0].Categories[0].Questions;

			Question q = questions[i++];
			q.StartRef = 1;
			q.EndRef = 4;
			q.ScriptureReference = "A";
			q = questions[i++];
			q.StartRef = 2;
			q.EndRef = 2;
			q.ScriptureReference = "B";
			q = questions[i++];
			q.StartRef = 3;
			q.EndRef = 3;
			q.ScriptureReference = "C";
			q = questions[i++];
			q.StartRef = 2;
			q.EndRef = 4;
			q.ScriptureReference = "B-D";
			q = questions[i++];
			q.StartRef = 5;
			q.EndRef = 5;
			q.ScriptureReference = "E";
			q = questions[i++];
			q.StartRef = 5;
			q.EndRef = 6;
			q.ScriptureReference = "E-F";

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(3, pq.KeyTerms.Length);

			i = 0;
			questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "What would God have me to say with respect to Paul?",
				"what",
				"would",
				new KeyTermPart("god"),
				"have me to",
				new KeyTermPart("say"),
				"with respect to",
				new KeyTermPart("paul"));
			VerifyParts(questions[i++], "What is Paul asking me to say with respect to that dog?",
				"what",
				"is paul asking me to",
				new KeyTermPart("say"),
				"with respect to",
				"that dog");
			VerifyParts(questions[i++], "that dog",
				"that dog");
			VerifyParts(questions[i++], "Is it okay for Paul to talk with respect to God today?",
				"is it okay for",
				new KeyTermPart("paul"),
				"to talk",
				"with respect to",
				new KeyTermPart("god"),
				"today");
			VerifyParts(questions[i++], "that dog wishes this Paul what is say radish",
				"that dog",
				"wishes this",
				new KeyTermPart("paul"),
				"what",
				"is",
				new KeyTermPart("say"),
				"radish");
			VerifyParts(questions[i++], "What is that dog?",
				"what",
				"is",
				"that dog");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests constraining the use of key terms to only the applicable "verses"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ConstrainByRef_ComplexKeyTerms()
		{
			AddMockedKeyTerm("high priest", 1);
			AddMockedKeyTerm("high", 1, 2);
			AddMockedKeyTerm("radish", 1, 2);
			AddMockedKeyTerm("(to have) eaten or drunk", 2, 3);
			AddMockedKeyTerm("high or drunk sailor", 2, 4);

			m_keyTermRules.Initialize();

			var qs = GenerateSimpleSectionWithSingleCategory(9);
			int i = 0;
			var questions = qs.Items[0].Categories[0].Questions;
			var q = questions[i++];
			q.Text = "Was the high priest on his high horse?";
			q.ScriptureReference = "A";
			q.StartRef = 1;
			q.EndRef = 1;

			q = questions[i++];
			q.Text = "Who was the high priest?";
			q.ScriptureReference = "B";
			q.StartRef = 2;
			q.EndRef = 2;

			q = questions[i++];
			q.Text = "I have eaten the horse.";
			q.ScriptureReference = "A";
			q.StartRef = 1;
			q.EndRef = 1;
			
			q = questions[i++];
			q.Text = "How high is this?";
			q.ScriptureReference = "C";
			q.StartRef = 3;
			q.EndRef = 3;
			
			q = questions[i++];
			q.Text = "That drunk sailor has eaten a radish";
			q.ScriptureReference = "C-D";
			q.StartRef = 3;
			q.EndRef = 4;
			
			q = questions[i++];
			q.Text = "That high sailor was to have drunk some radish juice";
			q.ScriptureReference = "A-B";
			q.StartRef = 1;
			q.EndRef = 2;

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null, null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(7, pq.KeyTerms.Length);

			i = 0;
			questions = pq.Sections.Items[0].Categories[0].Questions;
			VerifyParts(questions[i++], "Was the high priest on his high horse?",
				"was",
				"the",
				new KeyTermPart("high priest"),
				"on his",
				new KeyTermPart("high"),
				"horse");
			VerifyParts(questions[i++], "Who was the high priest?",
				"who",
				"was",
				"the",
				new KeyTermPart("high"),
				"priest");
			VerifyParts(questions[i++], "I have eaten the horse.",
				"i have eaten the horse");
			VerifyParts(questions[i++], "How high is this?",
				"how",
				"high is this");
			VerifyParts(questions[i++], "That drunk sailor has eaten a radish",
				"that",
				new KeyTermPart("drunk sailor"),
				"has",
				new KeyTermPart("eaten"),
				"a radish");
			VerifyParts(questions[i++], "That high sailor was to have drunk some radish juice",
				"that",
				new KeyTermPart("high sailor"),
				"was",
				new KeyTermPart("to have drunk"),
				"some",
				new KeyTermPart("radish"),
				"juice");
		}
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
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;
			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
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
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;
			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Category actCategory in sections.SelectMany(actSection => actSection.Categories))
			{
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
				new List<string>(), null, null, customizations, null);

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

		#region TXL-187: Custom questions added for a verse (range) that does not correspond to any existing question do not get loaded
		// Essentially, the tests in this region are primarily testing certain parts of the logic in MasterQuestionParser.GetCustomizations,
		// Customizations.ResolveDeletionsAndAdditions, and MasterQuestionParser.GetTrailingCustomizations (and the logic in GetQuestions
		// that calls that method).

		private static QuestionSections GenerateProverbsQuestionSections(bool includeBeneficialQuestionAt313_14 = false)
	    {
	        QuestionSections qs = new QuestionSections();
	        qs.Items = new Section[1];
	        int iS = 0;
	        qs.Items[iS] = CreateSection("PRO 3.1-35", "Proverbs 3:1-35 The Rewards of Wisdom.", 20003001,
	            20003035, 1, includeBeneficialQuestionAt313_14 ? 4 : 3);
	        int iC = 0;
	        Question q = qs.Items[iS].Categories[iC].Questions[0];
	        q.Text = "What is wisdom?";
	        q.Answers = new[] { "Smartness on steroids" };

	        iC = 1;
	        int iQ = 0;
	        q = qs.Items[iS].Categories[iC].Questions[iQ];
	        q.StartRef = 20003012;
	        q.EndRef = 20003014;
	        q.ScriptureReference = "PRO 3.12-14";
	        q.Text = "How many words are there?";
	        q.Answers = new[] { "A bunch" };

	        q = qs.Items[iS].Categories[iC].Questions[++iQ];
	        q.StartRef = 20003013;
	        q.EndRef = 20003013;
	        q.ScriptureReference = "PRO 3.13";
	        q.Text = "What man is happy?";
	        q.Answers = new[] { "The one who is smiling" };

			if (includeBeneficialQuestionAt313_14)
			{
				q = qs.Items[iS].Categories[iC].Questions[++iQ];
				q.StartRef = 20003013;
				q.EndRef = 20003014;
				q.ScriptureReference = "PRO 3.13-14";
				q.Text = "What is much more beneficial than gold or silver?";
				q.Answers = new[] { "Wisdom, understanding (13-14)" };
			}

	        q = qs.Items[iS].Categories[iC].Questions[++iQ];
	        q.StartRef = 20003015;
	        q.EndRef = 20003016;
	        q.ScriptureReference = "PRO 3.15-16";
	        q.Text = "What pictures describe wisdom?";
	        q.Answers = new[] { "Jewels, life, riches, etc." };

	        return qs;
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests that user questions are properly added/inserted into the correct location when
	    /// they are for a verse (or range) that does not match any existing question.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_SomeAddedQuestionsReplaceQuestionsWithDifferentVerseRangeButAreAlsoDeleted_PhrasesAreInCorrectOrderWithNoAdditions()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        // Note that the following two customizations really should have been done as a modification
	        // rather than as a deletion and addition, but a really user did it this way, so just checking
	        // to be sure it works.
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.ModifiedPhrase = "What person is happy?";
		    pc.Answer = "The person that finds wisdom and gets understanding (13)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What person is happy?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        // The following customization is deleted by the preceding one
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);

	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int excludedQuestions = 0;
	        int iQuestion = 0;

	        for (int iS = 0; iS < sections.Length; iS++)
	        {
	            Section actSection = sections[iS];
	            for (int iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                Category actCategory = actSection.Categories[iC];
	                for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    Question actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
	                                break;
	                            case 2:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
	                                break;
	                            case 3:
	                                Assert.IsTrue(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What person is happy?", actQuestion.PhraseInUse);
	                                Assert.AreEqual("The person that finds wisdom and gets understanding (13)", actQuestion.Answers.Single());
	                                break;
	                            case 4:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(4, pq.TranslatableParts.Length);
	        Assert.AreEqual(4, iQuestion);
	        Assert.AreEqual(3, excludedQuestions);
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests that user questions are properly added/inserted into the correct location when
	    /// they are for a verse (or range) that does not match any existing question.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_AddedQuestionReplacesQuestionWithDifferentVerseRange_PhrasesAreInCorrectOrder()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);

	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int excludedQuestions = 0;
	        int iQuestion = 0;

	        for (int iS = 0; iS < sections.Length; iS++)
	        {
	            Section actSection = sections[iS];
	            for (int iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                Category actCategory = actSection.Categories[iC];
	                for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    Question actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
	                            case 2:
	                                Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
	                            case 3:
	                                Assert.AreEqual("Are there any words in this section whose meaning is not clear?", actQuestion.PhraseInUse);
	                                Assert.AreEqual("[Make a list of the words not understood](13 - 20)", actQuestion.Answers.Single());
		                            Assert.IsTrue(actQuestion.IsUserAdded);
		                            Assert.AreEqual("PRO 3.13-20", actQuestion.ScriptureReference);
	                                break;
	                            case 4:
	                                Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
                                case 5:
	                                Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(5, pq.TranslatableParts.Length);
	        Assert.AreEqual(5, iQuestion);
	        Assert.AreEqual(1, excludedQuestions);
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests that user questions are properly added/inserted into the correct location when
	    /// they are for a verse (or range) that does not match any existing question.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_InsertedQuestionWithVerseRangeThatDoesNotMatchExistingQuestion_InsertedPhraseIncludedInCorrectPlace()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);

	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int excludedQuestions = 0;
	        int iQuestion = 0;

	        for (int iS = 0; iS < sections.Length; iS++)
	        {
	            Section actSection = sections[iS];
	            for (int iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                Category actCategory = actSection.Categories[iC];
	                for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    Question actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
	                                break;
	                            case 2:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
	                                break;
	                            case 3:
	                                Assert.IsTrue(actQuestion.IsUserAdded);
	                                Assert.AreEqual("PRO 3.13-20", actQuestion.ScriptureReference);
	                                Assert.AreEqual("Are there any words in this section whose meaning is not clear?", actQuestion.PhraseInUse);
	                                Assert.AreEqual("[Make a list of the words not understood](13 - 20)", actQuestion.Answers.Single());
	                                break;
	                            case 4:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
	                                break;
	                            case 5:
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(5, pq.TranslatableParts.Length);
	        Assert.AreEqual(5, iQuestion);
	        Assert.AreEqual(0, excludedQuestions);
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests that user questions are properly added/inserted into the correct location when
	    /// they are for a verse (or range) that does not match any existing question.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_AddedQuestionWithDifferentSingleVerseReplacesDeletedQuestionWithDifferentVerseRange_PhrasesAreInCorrectOrder()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        // Note that the following two customizations really should have been done as a modification
	        // rather than as a deletion and addition, but a really user did it this way, so just checking
	        // to be sure it works.
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.ModifiedPhrase = "What person is happy?";
		    pc.Answer = "The person over there";
	        pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What person is happy?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);
	        // The following customization is deleted by the preceding one
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13-20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20) - Added by Tom to prove this answer is not even being considered.";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);
			// And just to keep things confusing, this one is an "insertion before" (because that
			// is apparently the default if there are no matching questions*), but it actually
			// comes after all the others (by virtue of its start reference), including all other
			// built-in questions in the book.
			// *The dialog was later changed to show all questions in the selected category for the
			// section containing the selected reference, so if they were to do this today, there
			// would be matching questions.
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.20";
	        pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
	        pc.Answer = "[Make a list of the words not understood](13 - 20)";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);

	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int excludedQuestions = 0;
	        int iQuestion = 0;

	        for (int iS = 0; iS < sections.Length; iS++)
	        {
	            Section actSection = sections[iS];
	            for (int iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                Category actCategory = actSection.Categories[iC];
	                for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    Question actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
								case 2:
	                                Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
								case 3:
	                                Assert.AreEqual("What person is happy?", actQuestion.PhraseInUse);
									Assert.AreEqual("The person over there", actQuestion.Answers.Single());
	                                Assert.IsTrue(actQuestion.IsUserAdded);
	                                break;
								case 4:
	                                Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                break;
								case 5:
									Assert.AreEqual("Are there any words in this section whose meaning is not clear?", actQuestion.PhraseInUse);
									Assert.AreEqual("[Make a list of the words not understood](13 - 20)", actQuestion.Answers.Single());
									Assert.IsTrue(actQuestion.IsUserAdded);
									break;
								default:
	                                throw new Exception($"Unexpected question: {actQuestion.PhraseInUse}");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(5, pq.TranslatableParts.Length);
	        Assert.AreEqual(5, iQuestion);
	        Assert.AreEqual(3, excludedQuestions);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_NewQuestionsWithDifferentVerses_PhrasesNotAttachedToQuestionsForDifferentBook()
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			pc = new PhraseCustomization();
			pc.Reference = "PRO 1.13";
			pc.OriginalPhrase = "What does a person gain by wisdom?";
			pc.ModifiedPhrase = "What does a person gain by wisdom?";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13-20";
			pc.OriginalPhrase = "Are there any words in this section whose meaning is not clear?";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);

			MasterQuestionParser qp = new MasterQuestionParser(GenerateStandardQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.IsFalse(actQuestion.IsUserAdded);
					}
				}
			}
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(true, true, PhraseCustomization.CustomizationType.InsertionBefore)]
	    [TestCase(true, true, PhraseCustomization.CustomizationType.AdditionAfter)]
        [TestCase(false, true, PhraseCustomization.CustomizationType.AdditionAfter)]   // Not sure these two cases make sense. Why would you ever replace
        [TestCase(false, true, PhraseCustomization.CustomizationType.InsertionBefore)] // and existing question with an identical one w/o an answer?
	    [TestCase(true, false, PhraseCustomization.CustomizationType.InsertionBefore)]
	    [TestCase(true, false, PhraseCustomization.CustomizationType.AdditionAfter)]
	    [TestCase(false, false, PhraseCustomization.CustomizationType.AdditionAfter)]
	    [TestCase(false, false, PhraseCustomization.CustomizationType.InsertionBefore)]
	    public void GetResult_NewQuestionIdenticalToBuiltInQuestionWithSameAnswerOrNoAnswer_KeepsBuiltInQuestion(
	        bool sameAnswer, bool originalQuestionExcluded, PhraseCustomization.CustomizationType type)
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        if (originalQuestionExcluded)
	        {
	            pc.Reference = "PRO 3.13";
	            pc.OriginalPhrase = "What man is happy?";
	            pc.Type = PhraseCustomization.CustomizationType.Deletion;
	            customizations.Add(pc);
	        }
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.13";
	        pc.OriginalPhrase = "What man is happy?";
	        pc.ModifiedPhrase = "What man is happy?";
	        pc.Answer = sameAnswer ? "The one who is smiling" : "";
            pc.Type = type;
	        customizations.Add(pc);
	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int iQuestion = 0;

	        foreach (Section actSection in sections)
	        {
	            foreach (Category actCategory in actSection.Categories)
	            {
	                foreach (Question actQuestion in actCategory.Questions)
	                {
		                Assert.IsNull(actQuestion.ModifiedPhrase);

		                if (actQuestion.IsExcluded)
		                {
			                Assert.IsTrue(originalQuestionExcluded && sameAnswer);
	                        Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
							continue;
						}
						iQuestion++;
	                    Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                    switch (iQuestion)
	                    {
	                        case 1:
	                            Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
		                        Assert.IsFalse(actQuestion.IsUserAdded);
	                            break;
	                        case 2:
	                            Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
	                            break;
	                        case 3:
	                            Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
	                            Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
								Assert.AreEqual(originalQuestionExcluded && sameAnswer, actQuestion.IsUserAdded);
	                            break;
	                        case 4:
	                            Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
	                            break;
	                        default:
	                            throw new Exception("More included questions than expected.");
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(4, pq.TranslatableParts.Length);
	        Assert.AreEqual(4, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_NonMatchingDeletionsOrModifications_Ignored()
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			pc.Reference = "PRO 3.9";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.20";
			pc.OriginalPhrase = "Why doesn't this question exist?";
			pc.ModifiedPhrase = "Why should it?";
			pc.Type = PhraseCustomization.CustomizationType.Modification;
			customizations.Add(pc);
			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.IsFalse(actQuestion.IsExcluded);
						Assert.IsFalse(actQuestion.IsUserAdded);
						iQuestion++;
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								break;
							case 2:
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								break;
							case 3:
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								break;
							case 4:
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(4, pq.TranslatableParts.Length);
			Assert.AreEqual(4, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(true)]
		[TestCase(false)]
		public void GetResult_AddedQuestionForVerseWithNoExistingQuestionsHasDependentAddedQuestion_PhrasesAreInCorrectOrder(
			bool includeExtraAdditionAndDeletion)
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc;
			if (includeExtraAdditionAndDeletion)
			{
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.12";
				pc.OriginalPhrase = "To what can we compare the LORD's treatment of those he loves?";
				pc.ModifiedPhrase = "How does the LORD treat the one He loves?";
				pc.Type = PhraseCustomization.CustomizationType.Deletion;
				customizations.Add(pc);
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.12";
				pc.OriginalPhrase = "How does the LORD treat the one He loves?";
				pc.ModifiedPhrase = "How does the LORD treat the one He loves?";
				pc.Answer = "A father who disciplines the son in whom he delights (12)";
				pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
				customizations.Add(pc);
			}
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.12";
			pc.OriginalPhrase = "How does the LORD treat the one He loves?";
			pc.ModifiedPhrase = "How does the LORD treat the one He loves?";
			pc.Answer = "Like a father who disciplines his dear and accepted son (12)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.12";
			pc.OriginalPhrase = "How does the LORD treat the one He loves?";
			pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
			pc.Answer = "[Make a list of the words not understood] (7-12)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.IsFalse(actQuestion.IsExcluded);
						iQuestion++;
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 2:
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 3:
								Assert.AreEqual("How does the LORD treat the one He loves?", actQuestion.PhraseInUse);
								if (includeExtraAdditionAndDeletion)
									Assert.IsTrue(actQuestion.Answers.SequenceEqual(new [] { "Like a father who disciplines his dear and accepted son (12)",
										"A father who disciplines the son in whom he delights (12)"}));
								else
									Assert.AreEqual("Like a father who disciplines his dear and accepted son (12)", actQuestion.Answers.Single());
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							case 4:
								Assert.AreEqual("Are there any words in this section whose meaning is not clear?", actQuestion.PhraseInUse);
								Assert.AreEqual("[Make a list of the words not understood] (7-12)", actQuestion.Answers.Single());
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							case 5:
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 6:
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(6, pq.TranslatableParts.Length);
			Assert.AreEqual(6, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// UI no longer permits this, but it used to, so we need to be sure we handle this
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(true, PhraseCustomization.CustomizationType.InsertionBefore)]
		[TestCase(false, PhraseCustomization.CustomizationType.AdditionAfter)]
		[TestCase(false, PhraseCustomization.CustomizationType.AdditionAfter)]
		[TestCase(true, PhraseCustomization.CustomizationType.InsertionBefore)]
		public void GetResult_NewQuestionIdenticalToBuiltInQuestionWithAnswerThatContainsOriginalAnswer_KeepsQuestionWithLongerAnswer(
			bool originalQuestionExcluded, PhraseCustomization.CustomizationType type)
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			if (originalQuestionExcluded)
			{
				pc.Reference = "PRO 3.13";
				pc.OriginalPhrase = "What man is happy?";
				pc.Type = PhraseCustomization.CustomizationType.Deletion;
				customizations.Add(pc);
			}
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13";
			pc.OriginalPhrase = "What man is happy?";
			pc.ModifiedPhrase = "What man is happy?";
			pc.Answer = "The one who is smiling. (13)";
			pc.Type = type;
			customizations.Add(pc);
			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						Assert.IsNull(actQuestion.ModifiedPhrase);
						if (actQuestion.IsExcluded)
						{
							Assert.IsTrue(originalQuestionExcluded);
							continue;
						}
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						iQuestion++;
						switch (iQuestion)
						{
							case 1:
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 2:
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 3:
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.AreEqual("The one who is smiling. (13)", actQuestion.Answers.Single());
								Assert.AreEqual(originalQuestionExcluded, actQuestion.IsUserAdded);
								break;
							case 4:
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(4, pq.TranslatableParts.Length);
			Assert.AreEqual(4, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(true)]
		[TestCase(false)]
		public void GetResult_IdenticalAddedQuestionsWithSameAnswers_KeepsOnlyOne(bool oneQuestionExcluded)
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc;
			for (int i = 0; i < 2; i++)
			{
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.13-20";
				pc.OriginalPhrase = "What is the deal here?";
				pc.ModifiedPhrase = "What is the deal here?";
				pc.Answer = "No one knows";
				// This matches what I saw in real user data, but it probably doesn't matter.
				pc.Type = i == 0 ? PhraseCustomization.CustomizationType.InsertionBefore : PhraseCustomization.CustomizationType.AdditionAfter;
				customizations.Add(pc);
			}
			if (oneQuestionExcluded)
			{
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.13-20";
				pc.OriginalPhrase = "What is the deal here?";
				pc.Type = PhraseCustomization.CustomizationType.Deletion;
				customizations.Add(pc);
			}
			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						iQuestion++;
						Assert.IsFalse(actQuestion.IsExcluded);
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								break;
							case 2:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								break;
							case 3:
								Assert.IsTrue(actQuestion.IsUserAdded);
								Assert.AreEqual("What is the deal here?", actQuestion.PhraseInUse);
								Assert.AreEqual("No one knows", actQuestion.Answers.Single());
								break;
							case 4:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
								break;
							case 5:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(5, pq.TranslatableParts.Length);
			Assert.AreEqual(5, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(0)]
		[TestCase(1)]
		//[TestCase(2)]
		public void GetResult_IdenticalAddedQuestionsWithOneAnswerThatContainsOthers_KeepsOnlyTheOneWhoAnswerIsTheSuperset(int numberOfDeletions)
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc;
			StringBuilder bldr = new StringBuilder("This is the base answer.");
			for (int i = 0; i < 3; i++)
			{
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.14";
				pc.OriginalPhrase = "What is the deal here?";
				pc.ModifiedPhrase = "What is the deal here?";
				pc.Answer = bldr.ToString();
				bldr.Append(" More.");
				// This matches what I saw in real user data, but it probably doesn't matter.
				pc.Type = i == 0 ? PhraseCustomization.CustomizationType.InsertionBefore : PhraseCustomization.CustomizationType.AdditionAfter;
				customizations.Add(pc);
			}
			for (int i = 0; i < numberOfDeletions; i++)
			{
				pc = new PhraseCustomization();
				pc.Reference = "PRO 3.14";
				pc.OriginalPhrase = "What is the deal here?";
				pc.Type = PhraseCustomization.CustomizationType.Deletion;
				customizations.Add(pc);
			}
			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						iQuestion++;
						Assert.IsFalse(actQuestion.IsExcluded);
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 2:
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 3:
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 4:
								Assert.AreEqual("What is the deal here?", actQuestion.PhraseInUse);
								Assert.AreEqual("This is the base answer. More. More.", actQuestion.Answers.Single());
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							case 5:
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							default:
								throw new Exception("Unexpected question:" + actQuestion.PhraseInUse + "(answer: " + actQuestion.Answers.First() + ")");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(5, pq.TranslatableParts.Length);
			Assert.AreEqual(5, iQuestion);
		}

	    ///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
		/// data condition.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
	    public void GetResult_IdenticalAddedQuestionsAllExcluded_KeepsOnlyOneButExcludesIt()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc;
	        for (int i = 0; i < 2; i++)
	        {
	            pc = new PhraseCustomization();
	            pc.Reference = "PRO 3.13-20";
	            pc.OriginalPhrase = "What is the deal here?";
	            pc.ModifiedPhrase = "What is the deal here?";
	            pc.Answer = "No one knows";
	            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	            customizations.Add(pc);

	            pc = new PhraseCustomization();
	            pc.Reference = "PRO 3.13-20";
	            pc.OriginalPhrase = "What is the deal here?";
	            pc.Type = PhraseCustomization.CustomizationType.Deletion;
	            customizations.Add(pc);
	        }
	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int iQuestion = 0;

	        foreach (Section actSection in sections)
	        {
	            foreach (Category actCategory in actSection.Categories)
	            {
	                foreach (Question actQuestion in actCategory.Questions)
	                {
	                    iQuestion++;
	                    Assert.IsNull(actQuestion.ModifiedPhrase);
						if (!actQuestion.IsExcluded)
							Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                    switch (iQuestion)
	                    {
	                        case 1:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
	                            break;
	                        case 2:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
	                            break;
                            case 3:
                                Assert.IsTrue(actQuestion.IsExcluded);
                                Assert.IsTrue(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What is the deal here?", actQuestion.PhraseInUse);
	                            Assert.AreEqual("No one knows", actQuestion.Answers.Single());
                                break;
                            case 4:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
	                            Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
	                            break;
	                        case 5:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                            break;
	                        default:
	                            throw new Exception("More included questions than expected.");
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(4, pq.TranslatableParts.Length);
	        Assert.AreEqual(5, iQuestion);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that user questions are properly added/inserted into the correct location when
        /// they are for a verse (or range) that does not match any existing question.
        /// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
        /// data condition.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [TestCase(true, true)]
        [TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(false, false)]
	    public void GetResult_IdenticalAddedQuestionsWithDifferentAnswers_KeepsSingleQuestionWithAllAnswers(
			bool oneQuestionExcluded, bool questionWithShortAnswerAddedFirst)
        {
            List<PhraseCustomization> customizations = new List<PhraseCustomization>();
            PhraseCustomization pc;
	        int iShortAnswer = questionWithShortAnswerAddedFirst ? 0 : 1;
			for (int i = 0; i < 2; i++)
            {
                pc = new PhraseCustomization();
                pc.Reference = "PRO 3.14-16";
                pc.OriginalPhrase = "What is the deal here?";
                pc.ModifiedPhrase = "What is the deal here?";
                pc.Answer = i == iShortAnswer ? "Short" : "This is a long answer!!!";
                pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
                customizations.Add(pc);
            }
            if (oneQuestionExcluded)
            {
                pc = new PhraseCustomization();
                pc.Reference = "PRO 3.14-16";
                pc.OriginalPhrase = "What is the deal here?";
                pc.Type = PhraseCustomization.CustomizationType.Deletion;
                customizations.Add(pc);
            }
            MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
                new List<string>(), null, null, customizations, null);

            ParsedQuestions pq = qp.Result;

            Section[] sections = pq.Sections.Items;

            int iQuestion = 0;

            foreach (Section actSection in sections)
            {
                foreach (Category actCategory in actSection.Categories)
                {
                    foreach (Question actQuestion in actCategory.Questions)
                    {
                        iQuestion++;
                        Assert.IsFalse(actQuestion.IsExcluded);
                        Assert.IsNull(actQuestion.ModifiedPhrase);
                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
                        switch (iQuestion)
                        {
                            case 1:
                                Assert.IsFalse(actQuestion.IsUserAdded);
                                Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
                                break;
                            case 2:
                                Assert.IsFalse(actQuestion.IsUserAdded);
                                Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
                                break;
	                        case 3:
		                        Assert.IsFalse(actQuestion.IsUserAdded);
		                        Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
		                        Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
		                        break;
							case 4:
                                Assert.IsTrue(actQuestion.IsUserAdded);
                                Assert.AreEqual("What is the deal here?", actQuestion.PhraseInUse);
                                Assert.IsTrue(actQuestion.Answers.SetEquals(new [] { "Short", "This is a long answer!!!" }));
                                break;
                            case 5:
                                Assert.IsFalse(actQuestion.IsUserAdded);
                                Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
                                break;
                            default:
                                throw new Exception("More included questions than expected.");
                        }
                    }
                }
            }
            Assert.IsNull(pq.KeyTerms);
            Assert.AreEqual(5, pq.TranslatableParts.Length);
            Assert.AreEqual(5, iQuestion);
        }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// Tests that user questions are properly added/inserted into the correct location when
	    /// they are for a verse (or range) that does not match any existing question.
	    /// UI no longer permits this, but it used to, so we need to be sure we handle this bogus
	    /// data condition.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [TestCase(true, true)]
	    [TestCase(true, false)]
	    [TestCase(false, true)]
	    [TestCase(false, false)]
	    public void GetResult_IdenticalAddedQuestionsWithDifferentAnswersAllExcluded_KeepsSingleQuestionWithAllAnswersButExcluded(
			bool questionWithShortAnswerAddedFirst, bool includeQuestionWithSubstringAnswer)
	    {
		    List<PhraseCustomization> customizations = new List<PhraseCustomization>();
		    PhraseCustomization pc;
		    int iShortAnswer = questionWithShortAnswerAddedFirst ? 0 : 1;
		    int numberOfAdditions = includeQuestionWithSubstringAnswer ? 3 : 2;
			for (int i = 0; i < numberOfAdditions; i++)
	        {
	            pc = new PhraseCustomization();
	            pc.Reference = "PRO 3.14";
	            pc.OriginalPhrase = "What is the deal here?";
	            pc.ModifiedPhrase = "What is the deal here?";
	            pc.Answer = i == iShortAnswer ? "Short" : "This is a long answer!!!";
		        if (i == 2)
			        pc.Answer = pc.Answer.Remove(0, 5).Replace("!!!", "!");
	            pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	            customizations.Add(pc);

	            pc = new PhraseCustomization();
	            pc.Reference = "PRO 3.14";
	            pc.OriginalPhrase = "What is the deal here?";
	            pc.Type = PhraseCustomization.CustomizationType.Deletion;
	            customizations.Add(pc);
	        }
	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int iQuestion = 0;

	        foreach (Section actSection in sections)
	        {
	            foreach (Category actCategory in actSection.Categories)
	            {
	                foreach (Question actQuestion in actCategory.Questions)
	                {
	                    iQuestion++;
	                    Assert.IsNull(actQuestion.ModifiedPhrase);
						if (!actQuestion.IsExcluded)
							Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                    switch (iQuestion)
	                    {
	                        case 1:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
	                            break;
	                        case 2:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
	                            break;
	                        case 3:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
	                            Assert.AreEqual("The one who is smiling", actQuestion.Answers.Single());
	                            break;
		                    case 4:
			                    Assert.IsTrue(actQuestion.IsExcluded);
			                    Assert.IsTrue(actQuestion.IsUserAdded);
			                    Assert.AreEqual("What is the deal here?", actQuestion.PhraseInUse);
			                    Assert.IsTrue(actQuestion.Answers.SetEquals(new[] { "Short", "This is a long answer!!!" }));
								break;
							case 5:
	                            Assert.IsFalse(actQuestion.IsExcluded);
	                            Assert.IsFalse(actQuestion.IsUserAdded);
	                            Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
	                            break;
	                        default:
	                            throw new Exception("More included questions than expected.");
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(4, pq.TranslatableParts.Length);
	        Assert.AreEqual(5, iQuestion);
	    }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that user questions are properly added/inserted into the correct location when
        /// they are for a verse (or range) that does not match any existing question.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
	    public void GetResult_AddedQuestionsWithDifferentVerseRangesFollowingLastBuiltInQuestionInBook_AddedQuestionsIncludedAtEnd()
	    {
	        List<PhraseCustomization> customizations = new List<PhraseCustomization>();
	        PhraseCustomization pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.16-20";
	        pc.OriginalPhrase = "Is this a question?";
	        pc.ModifiedPhrase = "Is this a question?";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.16";
	        pc.OriginalPhrase = "Is this a question?";
	        pc.ModifiedPhrase = "Will this be in the right order?";
	        pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.16";
	        pc.OriginalPhrase = "Will this be in the right order?";
	        pc.ModifiedPhrase = "Will this be second to last?";
	        pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
	        customizations.Add(pc);
	        pc = new PhraseCustomization();
	        pc.Reference = "PRO 3.16";
	        pc.OriginalPhrase = "Will this be second to last?";
	        pc.Type = PhraseCustomization.CustomizationType.Deletion;
	        customizations.Add(pc);

	        MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
	            new List<string>(), null, null, customizations, null);

	        ParsedQuestions pq = qp.Result;

	        Section[] sections = pq.Sections.Items;

	        int iQuestion = 0;

	        foreach (Section actSection in sections)
	        {
		        foreach (Category actCategory in actSection.Categories)
		        {
			        foreach (Question actQuestion in actCategory.Questions)
			        {
				        iQuestion++;
				        Assert.IsNull(actQuestion.ModifiedPhrase);
						if (!actQuestion.IsExcluded)
							Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
				        switch (iQuestion)
				        {
					        case 1:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsFalse(actQuestion.IsUserAdded);
						        Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
						        break;
					        case 2:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsFalse(actQuestion.IsUserAdded);
						        Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
						        break;
					        case 3:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsFalse(actQuestion.IsUserAdded);
						        Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
						        break;
					        case 4:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsFalse(actQuestion.IsUserAdded);
						        Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
						        break;
					        case 5:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsTrue(actQuestion.IsUserAdded);
						        Assert.AreEqual("Is this a question?", actQuestion.PhraseInUse);
						        break;
					        case 6:
						        Assert.IsTrue(actQuestion.IsExcluded);
						        Assert.IsTrue(actQuestion.IsUserAdded);
						        Assert.AreEqual("Will this be second to last?", actQuestion.PhraseInUse);
						        break;
					        case 7:
						        Assert.IsFalse(actQuestion.IsExcluded);
						        Assert.IsTrue(actQuestion.IsUserAdded);
						        Assert.AreEqual("Will this be in the right order?", actQuestion.PhraseInUse);
						        break;
							default:
						        throw new Exception("More included questions than expected.");
				        }
			        }
		        }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(6, pq.TranslatableParts.Length);
	        Assert.AreEqual(7, iQuestion);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_LayersOfInsertedAndDeletedQuestionsWithDifferentVerseRanges_PhrasesAreInCorrectOrder()
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			pc.Reference = "PRO 3.4";
			pc.OriginalPhrase = "What kind of favor should you seek in the sight of God and people?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.4";
			pc.OriginalPhrase = "What kind of favor should you seek in the sight of God and people?";
			pc.ModifiedPhrase = "What kind of favor should you seek in the sight of God and people?";
			pc.Answer = "\"The favor of good understanding,\" or \"a reputation for good understanding\" (NET note)";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.4";
			pc.OriginalPhrase = "What kind of favor should you seek in the sight of God and people?";
			pc.ModifiedPhrase = "What kind of favor should you seek in the sight of God and people?";
			pc.Answer = "\"The favor of good understanding,\" or \"a reputation for good understanding\" (NET note) (4)";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.12-14";
			pc.OriginalPhrase = "How many words are there?";
			pc.ModifiedPhrase = "How many words were there?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.12-14";
			pc.OriginalPhrase = "How many words are there?";
			pc.ModifiedPhrase = "How many words were there?";
			pc.Answer = "Enough to make you crazy";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);

			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int excludedQuestions = 0;
			int iQuestion = 0;

			for (int iS = 0; iS < sections.Length; iS++)
			{
				Section actSection = sections[iS];
				for (int iC = 0; iC < actSection.Categories.Length; iC++)
				{
					Category actCategory = actSection.Categories[iC];
					for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++)
					{
						Question actQuestion = actCategory.Questions[iQ];

						if (actQuestion.IsExcluded)
						{
							excludedQuestions++;
						}
						else
						{
							iQuestion++;
							Assert.IsNull(actQuestion.ModifiedPhrase);
							Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
							switch (iQuestion)
							{
								case 1:
									Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
									Assert.IsFalse(actQuestion.IsUserAdded);
									break;
								case 2:
									Assert.AreEqual("What kind of favor should you seek in the sight of God and people?", actQuestion.PhraseInUse);
									Assert.IsTrue(actQuestion.IsUserAdded);
									break;
								case 3:
									Assert.AreEqual("How many words were there?", actQuestion.PhraseInUse);
									Assert.IsTrue(actQuestion.IsUserAdded);
									break;
								case 4:
									Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
									Assert.IsFalse(actQuestion.IsUserAdded);
									break;
								case 5:
									Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
									Assert.IsFalse(actQuestion.IsUserAdded);
									break;
								default:
									throw new Exception("More included questions than expected.");
							}
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(5, pq.TranslatableParts.Length);
			Assert.AreEqual(5, iQuestion);
			Assert.AreEqual(1, excludedQuestions);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that user questions are properly added/inserted into the correct location when
		/// they are for a verse (or range) that does not match any existing question.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_AddedQuestionReplacesAddedQuestionWithNullAnswer_SingleQuestionAddedInCorrectOrderWithCorrectAnswer()
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.ModifiedPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.ModifiedPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.Answer = "Fill your barns with grain and make your vats overflow with new wine";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.ModifiedPhrase = "What will you gain as a result of honoring the LORD?";
			pc.Answer = "Fill your barns with grain and make your vats overflow with new wine (10)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will the LORD do for you as a result of honoring Him?";
			pc.ModifiedPhrase = "What will you gain as a result of honoring the LORD?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.10";
			pc.OriginalPhrase = "What will you gain as a result of honoring the LORD?";
			pc.ModifiedPhrase = "What will you gain as a result of honoring the LORD?";
			pc.Answer = "Barns completely filled with grain and vats overflowing with new wine (10)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.25";
			pc.OriginalPhrase = "What pictures describe wisdom?";
			pc.ModifiedPhrase = "Is this a later addition to prove that it didn't get processed too early?";
			pc.Answer = "Yes";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);

			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						iQuestion++;
						Assert.IsNull(actQuestion.ModifiedPhrase);
						if (!actQuestion.IsExcluded)
							Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsExcluded);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 2:
								Assert.AreEqual("What will the LORD do for you as a result of honoring Him?", actQuestion.PhraseInUse);
								Assert.AreEqual("Fill your barns with grain and make your vats overflow with new wine (10)", actQuestion.Answers.Single());
								Assert.IsTrue(actQuestion.IsExcluded);
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							case 3:
								Assert.AreEqual("What will you gain as a result of honoring the LORD?", actQuestion.PhraseInUse);
								Assert.AreEqual("Barns completely filled with grain and vats overflowing with new wine (10)", actQuestion.Answers.Single());
								Assert.IsFalse(actQuestion.IsExcluded);
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							case 4:
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 5:
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 6:
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								Assert.IsFalse(actQuestion.IsUserAdded);
								break;
							case 7:
								Assert.AreEqual("Is this a later addition to prove that it didn't get processed too early?", actQuestion.PhraseInUse);
								Assert.IsTrue(actQuestion.IsUserAdded);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(6, pq.TranslatableParts.Length);
			Assert.AreEqual(7, iQuestion);
		}
		#endregion

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// TXL-188: Tests that if a user-added question has been added to the master list of
		/// questions, it will be ignored and not cause the program to crash or skip other
		/// added questions which hang off of it.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void GetResult_AddedQuestionsNowInMasterList_AdditionsIgnored()
		{
			List<PhraseCustomization> customizations = new List<PhraseCustomization>();
			PhraseCustomization pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13";
			pc.OriginalPhrase = "What shows that a man might have been happy?";
			pc.ModifiedPhrase = "What man is happy?";
			pc.Type = PhraseCustomization.CustomizationType.Deletion;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13";
			pc.OriginalPhrase = "What man is happy?";
			pc.ModifiedPhrase = "What man is happy?";
			pc.Answer = "Sorry, but this answer gets ignored.";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13";
			pc.OriginalPhrase = "What man is happy?";
			pc.ModifiedPhrase = "What man is happy?";
			pc.Answer = "I guess we'll use this one.";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13";
			pc.OriginalPhrase = "What man is happy?";
			pc.ModifiedPhrase = "Are there any words in this section whose meaning is not clear?";
			pc.Answer = "[Make a list of the words not understood](13 - 20)";
			pc.Type = PhraseCustomization.CustomizationType.AdditionAfter;
			customizations.Add(pc);
			pc = new PhraseCustomization();
			pc.Reference = "PRO 3.13-14";
			pc.OriginalPhrase = "What is much more beneficial than gold or silver?";
			pc.ModifiedPhrase = "What is much more beneficial than gold or silver?";
			pc.Answer = "Wisdom, understanding (13-14)";
			pc.Type = PhraseCustomization.CustomizationType.InsertionBefore;
			customizations.Add(pc);

			MasterQuestionParser qp = new MasterQuestionParser(GenerateProverbsQuestionSections(true),
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Section actSection in sections)
			{
				foreach (Category actCategory in actSection.Categories)
				{
					foreach (Question actQuestion in actCategory.Questions)
					{
						Assert.IsFalse(actQuestion.IsExcluded);
						iQuestion++;
						Assert.IsNull(actQuestion.ModifiedPhrase);
						Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
						switch (iQuestion)
						{
							case 1:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What is wisdom?", actQuestion.PhraseInUse);
								break;
							case 2:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("How many words are there?", actQuestion.PhraseInUse);
								break;
							case 3:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What man is happy?", actQuestion.PhraseInUse);
								Assert.AreEqual("I guess we'll use this one.", actQuestion.Answers.Single());
								break;
							case 4:
								Assert.IsTrue(actQuestion.IsUserAdded);
								Assert.AreEqual("Are there any words in this section whose meaning is not clear?", actQuestion.PhraseInUse);
								Assert.AreEqual("[Make a list of the words not understood](13 - 20)", actQuestion.Answers.Single());
								break;
							case 5:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What is much more beneficial than gold or silver?", actQuestion.PhraseInUse);
								Assert.AreEqual("Wisdom, understanding (13-14)", actQuestion.Answers.Single());
								break;
							case 6:
								Assert.IsFalse(actQuestion.IsUserAdded);
								Assert.AreEqual("What pictures describe wisdom?", actQuestion.PhraseInUse);
								break;
							default:
								throw new Exception("More included questions than expected.");
						}
					}
				}
			}
			Assert.IsNull(pq.KeyTerms);
			Assert.AreEqual(6, pq.TranslatableParts.Length);
			Assert.AreEqual(6, iQuestion);
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
				new List<string>(), null, null, customizations, null);

			ParsedQuestions pq = qp.Result;
			VerifyQuestionSections(pq);

			Section[] sections = pq.Sections.Items;

			int iQuestion = 0;

			foreach (Category actCategory in sections.SelectMany(actSection => actSection.Categories))
			{
				for (int iQ = 0; iQ < actCategory.Questions.Count; iQ++, iQuestion++)
				{
					Question actQuestion = actCategory.Questions[iQ];

					Assert.IsFalse(actQuestion.IsExcluded);
					if (iQuestion == 9)
					{
						VerifyParts(actQuestion, actQuestion.Text,
							"i said",
							"what did he answer");
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

			MasterQuestionParser qp = new MasterQuestionParser(qs, new List<string>(), null, null, customizations, null);

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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, null, null, customizations, null);

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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, null, null, customizations, null);

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

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, null, null, customizations, null);

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

		#region TXL-216 tests
	    private static QuestionSections CreateLuke9V37Thru45TestSections(string overviewQuestionForSecondSection = null)
	    {
		    var qs = new QuestionSections {Items = new Section[2]};
		    var iS = 0;
		    qs.Items[iS] = CreateSection("LUK 9.37-43a", "Luke 9:37-43a Jesus ordered an evil spirit to leave a boy, and it did.", 42009037,
			    42009043, 1, 2);
		    var iC = 0;
		    var q = qs.Items[iS].Categories[iC].Questions[0];
		    q.Text = "What do you think was the reason that Jesus was so upset?";
		    q.Answers = new[] {"He had given all twelve disciples power and authority over these things earlier (9:1)."};

		    iC = 1;
		    var iQ = 0;
		    q = qs.Items[iS].Categories[iC].Questions[iQ];
		    q.StartRef = 42009038;
		    q.EndRef = 42009040;
		    q.ScriptureReference = "LUK 9.38-40";
		    q.Text = "One man in the crowd was shouting to Jesus. About what was he shouting?";
		    q.Answers = new[] {"He wanted Jesus to take a look at his son, (who was his only child). (38)"};

		    q = qs.Items[iS].Categories[iC].Questions[++iQ];
		    q.StartRef = 42009043;
		    q.EndRef = 42009043;
		    q.ScriptureReference = "LUK 9.43";
		    q.Text = "What did all the crowd think about this?";
		    q.Answers = new[] {"They were all astounded/amazed at the great power of God to cast out such a powerful demon. (43a)"};

		    qs.Items[++iS] = CreateSection("LUK 9.43-45", "Luke 9:43b-45 Jesus says someone will betray him to his enemies.", 42009043,
			    42009045, overviewQuestionForSecondSection == null ? 0 : 1, 2);
		    iC = 0;
		    iQ = 0;
			if (overviewQuestionForSecondSection != null)
			{
				q = qs.Items[iS].Categories[iC].Questions[iQ];
				q.StartRef = 42009043;
				q.EndRef = 42009045;
				q.ScriptureReference = "LUK 9.43-45";
				q.Text = overviewQuestionForSecondSection;
				iC++;
			}

		    q = qs.Items[iS].Categories[iC].Questions[iQ];
		    q.StartRef = 42009043;
		    q.EndRef = 42009044;
		    q.ScriptureReference = "LUK 9.43-44";
		    q.Text = "What new information did Jesus tell his disciples?";
		    q.Answers = new[] {"He told them that someone would betray [him,] the Son of Man, into the hands/control of other men. (44)"};

		    q = qs.Items[iS].Categories[iC].Questions[++iQ];
		    q.StartRef = 42009044;
		    q.EndRef = 42009044;
		    q.ScriptureReference = "LUK 9.44";
		    q.Text = "What do you think it means to betray someone into other people's hands/control?";
		    q.Answers = new[] {"Key Term Check: It means to deliver that person into the control of someone whom that person regards as an enemy."};
		    return qs;
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// TXL-216: Tests that a user question is properly inserted into the correct location
	    /// and associated with the correct section when it is for a verse that spans two
	    /// sections and the first detail question of the second section applies to multiple
	    /// verses.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_InsertedQuestionHasSameStartVerseAsBaseButCoversSingleVerse_PhrasesAreInCorrectOrderAndSection()
	    {
	        var customizations = new List<PhraseCustomization>();
	        var pc = new PhraseCustomization
	        {
		        Reference = "LUK 9.43",
		        OriginalPhrase = "What new information did Jesus tell his disciples?",
		        ModifiedPhrase = "Why did Jesus use this precise moment, in the midst of the hoopla and emotion, to bring his disciples back down to the cold reality?",
		        Answer = "Sometimes in moments of ecstasy, that's what's needed.",
		        Type = PhraseCustomization.CustomizationType.InsertionBefore
	        };
	        customizations.Add(pc);

	        var qs = CreateLuke9V37Thru45TestSections();
	        var qp = new MasterQuestionParser(qs, new List<string>(), null, null, customizations, null);

	        var pq = qp.Result;
	        var sections = pq.Sections.Items;

	        var excludedQuestions = 0;
	        var iQuestion = 0;

	        for (var iS = 0; iS < sections.Length; iS++)
	        {
	            var actSection = sections[iS];
	            for (var iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                var actCategory = actSection.Categories[iC];
	                for (var iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    var actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.AreEqual("What do you think was the reason that Jesus was so upset?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
									Assert.AreEqual(0, iS);
									Assert.AreEqual(0, iC);
	                                break;
	                            case 2:
	                                Assert.AreEqual("One man in the crowd was shouting to Jesus. About what was he shouting?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(0, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
	                            case 3:
		                            Assert.AreEqual("What did all the crowd think about this?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
		                            Assert.AreEqual(0, iS);
		                            Assert.AreEqual(1, iC);
		                            break;
	                            case 4:
		                            Assert.AreEqual("Why did Jesus use this precise moment, in the midst of the hoopla and emotion, to bring his disciples back down to the cold reality?", actQuestion.PhraseInUse);
		                            Assert.AreEqual("LUK 9.43", actQuestion.ScriptureReference);
		                            Assert.AreEqual(42009043, actQuestion.StartRef);
		                            Assert.AreEqual(42009043, actQuestion.EndRef);
		                            Assert.IsTrue(actQuestion.IsUserAdded);
		                            Assert.AreEqual(1, iS);
		                            Assert.AreEqual(0, iC);
		                            break;
	                            case 5:
	                                Assert.AreEqual("What new information did Jesus tell his disciples?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(0, iC);
	                                break;
                                case 6:
	                                Assert.AreEqual("What do you think it means to betray someone into other people's hands/control?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(0, iC);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(6, iQuestion);
	        Assert.AreEqual(0, excludedQuestions);
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// TXL-216: Tests that a user question is properly inserted into a new (overview)
	    /// category for the correct (existing) section.
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_InsertedQuestionInNewCategory_PhrasesAreInCorrectCategoryAndSection()
	    {
	        var customizations = new List<PhraseCustomization>();
	        var pc = new PhraseCustomization
	        {
		        Reference = "LUK 9.43-45",
		        OriginalPhrase = "What new information did Jesus tell his disciples?",
		        ModifiedPhrase = "What happened in this passage?",
		        Answer = "Various things.",
		        Type = PhraseCustomization.CustomizationType.InsertionBefore,
				AddedCategoryOffset = 1
	        };
	        customizations.Add(pc);

	        var qs = CreateLuke9V37Thru45TestSections();
	        var qp = new MasterQuestionParser(qs, new List<string>(), null, null, customizations, null);

	        var pq = qp.Result;
	        var sections = pq.Sections.Items;

	        var excludedQuestions = 0;
	        var iQuestion = 0;

	        for (var iS = 0; iS < sections.Length; iS++)
	        {
	            var actSection = sections[iS];
	            for (var iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                var actCategory = actSection.Categories[iC];
	                for (var iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    var actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.AreEqual("What do you think was the reason that Jesus was so upset?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
									Assert.AreEqual(0, iS);
									Assert.AreEqual(0, iC);
	                                break;
	                            case 2:
	                                Assert.AreEqual("One man in the crowd was shouting to Jesus. About what was he shouting?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(0, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
	                            case 3:
		                            Assert.AreEqual("What did all the crowd think about this?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
		                            Assert.AreEqual(0, iS);
		                            Assert.AreEqual(1, iC);
		                            break;
	                            case 4:
		                            Assert.IsTrue(actCategory.IsOverview);
		                            Assert.AreEqual("What happened in this passage?", actQuestion.PhraseInUse);
		                            Assert.AreEqual("LUK 9.43-45", actQuestion.ScriptureReference);
		                            Assert.AreEqual(42009043, actQuestion.StartRef);
		                            Assert.AreEqual(42009045, actQuestion.EndRef);
		                            Assert.IsTrue(actQuestion.IsUserAdded);
									Assert.AreEqual("Various things.", actQuestion.Answers.Single());
		                            Assert.AreEqual(1, iS);
		                            Assert.AreEqual(0, iC);
		                            break;
	                            case 5:
		                            Assert.IsFalse(actCategory.IsOverview);
	                                Assert.AreEqual("What new information did Jesus tell his disciples?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
                                case 6:
	                                Assert.AreEqual("What do you think it means to betray someone into other people's hands/control?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(6, iQuestion);
	        Assert.AreEqual(0, excludedQuestions);
	    }

	    ///--------------------------------------------------------------------------------------
	    /// <summary>
	    /// TXL-216: Tests that a user question that was inserted before the first detail
	    /// question, intended to create the overview category, is properly added to the end of
	    /// the existing overview category (which can happen if the overview category is later
	    /// added to the list of canned questions).
	    /// </summary>
	    ///--------------------------------------------------------------------------------------
	    [Test]
	    public void GetResult_InsertedQuestionInNewCategoryThatNowExists_PhrasesAreInCorrectCategoryAndSection()
	    {
	        var customizations = new List<PhraseCustomization>();
	        var pc = new PhraseCustomization
	        {
		        Reference = "LUK 9.43-45",
		        OriginalPhrase = "What new information did Jesus tell his disciples?",
		        ModifiedPhrase = "What happened in this passage?",
		        Answer = "Various things.",
		        Type = PhraseCustomization.CustomizationType.InsertionBefore,
				AddedCategoryOffset = 1
	        };
	        customizations.Add(pc);

	        var qs = CreateLuke9V37Thru45TestSections("Please summarize what you remember.");
	        var qp = new MasterQuestionParser(qs, new List<string>(), null, null, customizations, null);

	        var pq = qp.Result;
	        var sections = pq.Sections.Items;

	        var excludedQuestions = 0;
	        var iQuestion = 0;

	        for (var iS = 0; iS < sections.Length; iS++)
	        {
	            var actSection = sections[iS];
	            for (var iC = 0; iC < actSection.Categories.Length; iC++)
	            {
	                var actCategory = actSection.Categories[iC];
	                for (var iQ = 0; iQ < actCategory.Questions.Count; iQ++)
	                {
	                    var actQuestion = actCategory.Questions[iQ];

	                    if (actQuestion.IsExcluded)
	                    {
	                        excludedQuestions++;
	                    }
	                    else
	                    {
	                        iQuestion++;
	                        Assert.IsNull(actQuestion.ModifiedPhrase);
	                        Assert.AreEqual(PartType.TranslatablePart, actQuestion.ParsedParts.Single().Type);
	                        switch (iQuestion)
	                        {
	                            case 1:
	                                Assert.AreEqual("What do you think was the reason that Jesus was so upset?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
									Assert.AreEqual(0, iS);
									Assert.AreEqual(0, iC);
	                                break;
	                            case 2:
	                                Assert.AreEqual("One man in the crowd was shouting to Jesus. About what was he shouting?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(0, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
	                            case 3:
		                            Assert.AreEqual("What did all the crowd think about this?", actQuestion.PhraseInUse);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
		                            Assert.AreEqual(0, iS);
		                            Assert.AreEqual(1, iC);
		                            break;
	                            case 4:
		                            Assert.IsTrue(actCategory.IsOverview);
		                            Assert.AreEqual("Please summarize what you remember.", actQuestion.PhraseInUse);
		                            Assert.AreEqual("LUK 9.43-45", actQuestion.ScriptureReference);
		                            Assert.IsFalse(actQuestion.IsUserAdded);
		                            Assert.AreEqual(1, iS);
		                            Assert.AreEqual(0, iC);
		                            break;
	                            case 5:
		                            Assert.IsTrue(actCategory.IsOverview);
		                            Assert.AreEqual("What happened in this passage?", actQuestion.PhraseInUse);
		                            Assert.AreEqual("LUK 9.43-45", actQuestion.ScriptureReference);
		                            Assert.AreEqual(42009043, actQuestion.StartRef);
		                            Assert.AreEqual(42009045, actQuestion.EndRef);
		                            Assert.IsTrue(actQuestion.IsUserAdded);
		                            Assert.AreEqual("Various things.", actQuestion.Answers.Single());
		                            Assert.AreEqual(1, iS);
		                            Assert.AreEqual(0, iC);
		                            break;
	                            case 6:
		                            Assert.IsFalse(actCategory.IsOverview);
	                                Assert.AreEqual("What new information did Jesus tell his disciples?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
                                case 7:
	                                Assert.AreEqual("What do you think it means to betray someone into other people's hands/control?", actQuestion.PhraseInUse);
	                                Assert.IsFalse(actQuestion.IsUserAdded);
	                                Assert.AreEqual(1, iS);
	                                Assert.AreEqual(1, iC);
	                                break;
	                            default:
	                                throw new Exception("More included questions than expected.");
	                        }
	                    }
	                }
	            }
	        }
	        Assert.IsNull(pq.KeyTerms);
	        Assert.AreEqual(7, iQuestion);
	        Assert.AreEqual(0, excludedQuestions);
	    }
		#endregion
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

		/// ------------------------------------------------------------------------------------
		internal static QuestionSections GenerateSimpleSectionWithSingleCategory(int numberOfDetailQuestions)
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS = 0;
			qs.Items[iS] = CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
				44001005, 0, numberOfDetailQuestions);
			return qs;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a MasterQuestionParser and retrieves the ParsedQuestions for the given
		/// questions. Uses the m_questionWords, m_dummyKtList, and m_keyTermRules, but does not
		/// supply any customizations or substitutions. Asserts that the correct number
		/// of questions is parsed (but doesn't check the text of each question).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal ParsedQuestions ParseQuestions(params string[] questions)
		{
			var qs = GenerateQuestions(questions);

			MasterQuestionParser qp = new MasterQuestionParser(qs, s_questionWords, m_dummyKtList, m_keyTermRules, null,
				null);
			ParsedQuestions pq = qp.Result;
			Assert.AreEqual(questions.Length, pq.Sections.Items[0].Categories[0].Questions.Count);
			return pq;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a QuestionSections object with a single section and single category with
		/// all the questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static QuestionSections GenerateQuestions(params string[] questions)
		{
			var qs = GenerateSimpleSectionWithSingleCategory(questions.Length);
			var cat = qs.Items[0].Categories[0];
			int i = 0;
			foreach (string question in questions)
				cat.Questions[i++].Text = question;
			return qs;
		}

		/// ------------------------------------------------------------------------------------
		internal static Section CreateSection(string sRef, string heading, int startRef, int endRef,
			int cOverviewQuestions, int cDetailQuestions)
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
				s.Categories[iC].IsOverview = true;
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

		/// ------------------------------------------------------------------------------------
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
					foreach (Question actQuestion in actCategory.Questions)
					{
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
						CollectionAssert.AreEqual(expQuestion.Answers, actQuestion.Answers);
						CollectionAssert.AreEqual(expQuestion.Notes, actQuestion.Notes);
						CollectionAssert.AreEqual(expQuestion.Alternatives, actQuestion.Alternatives);
						iExpQ++;

						foreach (string part in actQuestion.ParsedParts.Where(pp => pp.Type == PartType.TranslatablePart).Select(pp => pp.Text))
							Assert.IsTrue(pq.TranslatableParts.Contains(part));
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Little class to support identifying parts as key-term parts for the purpose of
		/// verification using VerifyParts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private class KeyTermPart
		{
			private string m_term;
			public KeyTermPart(string term)
			{
				m_term = term;
			}

			public override string ToString()
			{
				return m_term;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Verifies the text and parts of the given question.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static void VerifyParts(Question q, string text, params object[] parts)
		{
			Assert.AreEqual(text, q.Text);
			for (int pp = 0; pp < parts.Length && pp < q.ParsedParts.Count; pp++)
			{
				object part = parts[pp];
				if (part is int)
					Assert.AreEqual(part, q.ParsedParts[pp].NumericValue);
				else
					Assert.AreEqual(part.ToString(), q.ParsedParts[pp].Text);
				Assert.AreEqual(part is KeyTermPart ? PartType.KeyTerm :
					(part is int ? PartType.Number : PartType.TranslatablePart),
					q.ParsedParts[pp].Type,
					"Part " + q.ParsedParts[pp].Text + "(" + pp + ") has wrong type.");
			}
			Assert.AreEqual(parts.Length, q.ParsedParts.Count);
		}
		#endregion
	}
}
