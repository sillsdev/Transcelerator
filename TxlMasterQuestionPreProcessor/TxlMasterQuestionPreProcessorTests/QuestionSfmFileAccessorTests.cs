// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2012, SIL International. All Rights Reserved.
// <copyright from='2011' to='2012' company='SIL International'>
//		Copyright (c) 2012, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: QuestionSfmFileAccessorTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SIL.TxlMasterQuestionPreProcessor;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests the QuestionProviderBase implementation
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class QuestionSfmFileAccessorTests
	{
		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests basic parsing of overview and detail questions, answers, and comments.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseBasicQuestions()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Acts 1:1-5 Introduction to the book.",
				@"\oh Overview",
				@"\tqref ACT 1.1-5",
				@"\bttq What information did Luke, the writer of this book, give in this introduction?",
				@"\tqe Luke reminded his readers that he was about to continue the true story about Jesus",
				@"\bttq What do you think an apostle of Jesus is?",
				@"\tqe Key Term Check: To be an apostle of Jesus means to be a messenger",
				"\\tqe Can also be translated as \"sent one\"",
				@"\an Note: apostles can be real sweethearts sometimes",
				@"\dh Details",
				@"\bttq To whom did the writer of Acts address this book?",
				@"\tqe He addressed this book to Theophilus.",
				@"\rf Acts 1:6-10 The continuing saga.",
				@"\oh Overview",
				@"\tqref ACT 1.6-10",
				@"\bttq What happened?",
				@"\tqe Stuff",
				@"\dh Details",
				@"\tqref ACT 1.6",
				@"\bttq What question did the apostles ask Jesus about his kingdom?",
				@"\tqe The apostles asked Jesus whether he was soon going to set up his",
				@"kingdom in a way that everybody could see and cause the",
				@"people of Israel to have power in that kingdom."}, null);

			Assert.AreEqual(2, sections.Items.Length);

			Section section = sections.Items[0];
			Assert.AreEqual("Acts 1:1-5 Introduction to the book.", section.Heading);

			Assert.AreEqual(2, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Overview", category.Type);
			
			Assert.AreEqual(2, category.Questions.Count);

			Question question = category.Questions[0];
			Assert.AreEqual("What information did Luke, the writer of this book, give in this introduction?",
				question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Luke reminded his readers that he was about to continue the true story about Jesus",
				question.Answers[0]);
			Assert.IsNull(question.Notes);

			question = category.Questions[1];
			Assert.AreEqual("What do you think an apostle of Jesus is?", question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(2, question.Answers.Length);
			Assert.AreEqual("Key Term Check: To be an apostle of Jesus means to be a messenger",
				question.Answers[0]);
			Assert.AreEqual("Can also be translated as \"sent one\"", question.Answers[1]);
			Assert.AreEqual(1, question.Notes.Length);
			Assert.AreEqual("Note: apostles can be real sweethearts sometimes", question.Notes[0]);

			category = section.Categories[1];
			Assert.AreEqual("Details", category.Type);
			
			Assert.AreEqual(1, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("To whom did the writer of Acts address this book?", question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.IsNull(question.Notes);
			Assert.AreEqual("He addressed this book to Theophilus.", question.Answers[0]);

			section = sections.Items[1];
			Assert.AreEqual("Acts 1:6-10 The continuing saga.", section.Heading);

			Assert.AreEqual(2, section.Categories.Length);

			category = section.Categories[0];
			Assert.AreEqual("Overview", category.Type);
			
			Assert.AreEqual(1, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("What happened?", question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Stuff", question.Answers[0]);
			Assert.IsNull(question.Notes);

			category = section.Categories[1];
			Assert.AreEqual("Details", category.Type);
			
			Assert.AreEqual(1, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?", question.Text);
			Assert.AreEqual("ACT 1.6", question.ScriptureReference);
			Assert.AreEqual(44001006, question.StartRef);
			Assert.AreEqual(44001006, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom.",
				question.Answers[0]);
			Assert.IsNull(question.Notes);
		}

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// TXL-116: Ensure references for single-chapter books are parsed correctly.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void ParseQuestionsForSingleChapterBook()
        {
            QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Philemon 1-3",
				@"\oh Overview",
				@"\tqref PHM 1-3",
				@"\bttq Tell in your own words what Paul said.",
				@"\tqe Paul is writing a letter to Philemon.",
				@"\dh Details",
				@"\tqref PHM 1-2",
				@"\bttq Who is writing this letter?",
				@"\tqe Paul. Timothy is not a co-author, but joined Paul in sending the greeting.",
				@"\bttq Where is Paul when he is writing this letter?",
				@"\tqe In prison.",
				@"\tqref PHM 3",
				@"\bttq What does Paul want God to do for these people?",
				@"\tqe He wants God to bless them and give them inner peace.",

				@"\rf Philemon 4-7",
				@"\oh Overview",
				@"\tqref PHM 4-7",
				@"\bttq Tell in your own words what Paul said.",
				@"\tqe He thanks God for Philemon.",
				@"\dh Details",
				@"\tqref PHM 4-5",
				@"\bttq Why does Paul thank God?",
				@"\tqe Because of what he has heard about Philemon.",
				@"\bttq What has he heard about him?",
				@"\tqe That Philemon loves all believers.",
				@"\tqref PHM 6",
				@"\bttq What does Paul pray for in this verse?",
				@"\tqe That their fellowship will result in knowing more.",
				@"\tqref PHM 7",
				@"\bttq What gives Paul much joy?",
				@"\tqe The love that Philemon shows to others.",
				@"\bttq What was the evidence of that love?",
				@"\tqe The help and encouragement he has given to the believers.",
            
				@"\rf Philemon 23-25",
				@"\oh Overview",
				@"\tqref PHM 23-25",
				@"\bttq Tell in your own words what Paul said.",
				@"\tqe Paul sends greetings.",
				@"\dh Details",
				@"\tqref PHM 23-24",
				@"\bttq To whom does Paul send greetings?",
				@"\tqe To Philemon and the believers with him (2)."}, null);

            Assert.AreEqual(3, sections.Items.Length);

            // SECTION 0 : PHM 1-3
            Section section = sections.Items[0];
            Assert.AreEqual("Philemon 1-3", section.Heading);
            Assert.AreEqual(57001001, section.StartRef);
            Assert.AreEqual(57001003, section.EndRef);

            Assert.AreEqual(2, section.Categories.Length);

            Category category = section.Categories[0];
            Assert.AreEqual("Overview", category.Type);

            Assert.AreEqual(1, category.Questions.Count);

            Question question = category.Questions[0];
            Assert.AreEqual("Tell in your own words what Paul said.", question.Text);
            Assert.IsNull(question.ScriptureReference);

            category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            const int numberOfDetailQuestionsS0 = 3;
            Assert.AreEqual(numberOfDetailQuestionsS0, category.Questions.Count);

            Assert.AreEqual("Who is writing this letter?", category.Questions[0].Text);
            Assert.AreEqual("Where is Paul when he is writing this letter?", category.Questions[1].Text);

            for (int i = 0; i < numberOfDetailQuestionsS0 - 1; i++)
            {
                question = category.Questions[i];
                Assert.AreEqual("PHM 1-2", question.ScriptureReference);
                Assert.AreEqual(57001001, question.StartRef);
                Assert.AreEqual(57001002, question.EndRef);                
            }
            question = category.Questions[numberOfDetailQuestionsS0 - 1];

            Assert.AreEqual("What does Paul want God to do for these people?", question.Text);
            Assert.AreEqual("PHM 3", question.ScriptureReference);
            Assert.AreEqual(57001003, question.StartRef);
            Assert.AreEqual(57001003, question.EndRef);

            // SECTION 1 : PHM 4-7
            section = sections.Items[1];
            Assert.AreEqual("Philemon 4-7", section.Heading);
            Assert.AreEqual(57001004, section.StartRef);
            Assert.AreEqual(57001007, section.EndRef);

            Assert.AreEqual(2, section.Categories.Length);

            category = section.Categories[0];
            Assert.AreEqual("Overview", category.Type);

            Assert.AreEqual(1, category.Questions.Count);

            question = category.Questions[0];
            Assert.AreEqual("Tell in your own words what Paul said.", question.Text);
            Assert.IsNull(question.ScriptureReference);

            category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(5, category.Questions.Count);

            question = category.Questions[0];
            Assert.AreEqual("Why does Paul thank God?", question.Text);
            Assert.AreEqual("PHM 4-5", question.ScriptureReference);
            Assert.AreEqual(57001004, question.StartRef);
            Assert.AreEqual(57001005, question.EndRef);

            question = category.Questions[1];
            Assert.AreEqual("What has he heard about him?", question.Text);
            Assert.AreEqual("PHM 4-5", question.ScriptureReference);
            Assert.AreEqual(57001004, question.StartRef);
            Assert.AreEqual(57001005, question.EndRef);

            question = category.Questions[2];
            Assert.AreEqual("What does Paul pray for in this verse?", question.Text);
            Assert.AreEqual("PHM 6", question.ScriptureReference);
            Assert.AreEqual(57001006, question.StartRef);
            Assert.AreEqual(57001006, question.EndRef);

            question = category.Questions[3];
            Assert.AreEqual("What gives Paul much joy?", question.Text);
            Assert.AreEqual("PHM 7", question.ScriptureReference);
            Assert.AreEqual(57001007, question.StartRef);
            Assert.AreEqual(57001007, question.EndRef);

            question = category.Questions[4];
            Assert.AreEqual("What was the evidence of that love?", question.Text);
            Assert.AreEqual("PHM 7", question.ScriptureReference);
            Assert.AreEqual(57001007, question.StartRef);
            Assert.AreEqual(57001007, question.EndRef);

            // SECTION 2 : PHM 23-25",
            section = sections.Items[2];
            Assert.AreEqual("Philemon 23-25", section.Heading);
            Assert.AreEqual(57001023, section.StartRef);
            Assert.AreEqual(57001025, section.EndRef);

            Assert.AreEqual(2, section.Categories.Length);

            category = section.Categories[0];
            Assert.AreEqual("Overview", category.Type);

            Assert.AreEqual(1, category.Questions.Count);

            question = category.Questions[0];
            Assert.AreEqual("Tell in your own words what Paul said.", question.Text);
            Assert.IsNull(question.ScriptureReference);

            category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(1, category.Questions.Count);

            question = category.Questions[0];
            Assert.AreEqual("To whom does Paul send greetings?", question.Text);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("To Philemon and the believers with him (2).", question.Answers[0]);
            Assert.AreEqual("PHM 23-24", question.ScriptureReference);
            Assert.AreEqual(57001023, question.StartRef);
            Assert.AreEqual(57001024, question.EndRef);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests parsing of detail questions whose answers contain a verse number in parentheses.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void ParseBasicQuestions_InterpretVerseNumbersInAnswers_Simple()
        {
            QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Acts 1:1-5 Introduction to the book.",
				@"\oh Overview",
				@"\tqref ACT 1.1-5",
				@"\bttq What information did Luke, the writer of this book, give in this introduction?",
				@"\tqe Luke reminded his readers that he was about to continue the true story about Jesus and his apostles that Luke had written in his first book. (1)",
				@"\dh Details",
				@"\bttq To whom did the writer of Acts address this book?",
				@"\tqe He addressed this book to Theophilus. (1)",
				@"\bttq What happened?",
				@"\tqe Stuff (2-3)",
				@"\tqref ACT 1.4",
				@"\bttq What question did the apostles ask Jesus about his kingdom?",
				@"\tqe The apostles asked Jesus what was happening.",
				@"\bttq Where were they?",
				@"\tqe On a mountain. (4)",
				@"\tqe Outside. (5)"}, null);

            Assert.AreEqual(1, sections.Items.Length);

            // Acts
            Section section = sections.Items[0];
            Assert.AreEqual("Acts 1:1-5 Introduction to the book.", section.Heading);
            Assert.AreEqual("ACT 1.1-5", section.ScriptureReference);

            Assert.AreEqual(2, section.Categories.Length);

            Category category = section.Categories[0];
            Assert.AreEqual("Overview", category.Type);

            Assert.AreEqual(1, category.Questions.Count);
            Question question = category.Questions[0];

            Assert.AreEqual("What information did Luke, the writer of this book, give in this introduction?", question.Text);
            Assert.IsNull(question.ScriptureReference);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.IsNull(question.Notes);
            Assert.AreEqual("Luke reminded his readers that he was about to continue the true story about Jesus and his apostles that Luke had written in his first book. (1)", question.Answers[0]);

            category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(4, category.Questions.Count);
            question = category.Questions[0];

            Assert.AreEqual("To whom did the writer of Acts address this book?", question.Text);
            Assert.AreEqual("ACT 1.1", question.ScriptureReference);
            Assert.AreEqual(44001001, question.StartRef);
            Assert.AreEqual(44001001, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.IsNull(question.Notes);
            Assert.AreEqual("He addressed this book to Theophilus. (1)", question.Answers[0]);

            question = category.Questions[1];

            Assert.AreEqual("What happened?", question.Text);
            Assert.AreEqual("ACT 1.2-3", question.ScriptureReference);
            Assert.AreEqual(44001002, question.StartRef);
            Assert.AreEqual(44001003, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("Stuff (2-3)", question.Answers[0]);
            Assert.IsNull(question.Notes);

            question = category.Questions[2];

            Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?", question.Text);
            Assert.AreEqual("ACT 1.4", question.ScriptureReference);
            Assert.AreEqual(44001004, question.StartRef);
            Assert.AreEqual(44001004, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("The apostles asked Jesus what was happening.", question.Answers[0]);
            Assert.IsNull(question.Notes);

            question = category.Questions[3];

            Assert.AreEqual("Where were they?", question.Text);
            Assert.AreEqual("ACT 1.4-5", question.ScriptureReference);
            Assert.AreEqual(44001004, question.StartRef);
            Assert.AreEqual(44001005, question.EndRef);
            Assert.AreEqual(2, question.Answers.Length);
            Assert.AreEqual("On a mountain. (4)", question.Answers[0]);
            Assert.AreEqual("Outside. (5)", question.Answers[1]);
            Assert.IsNull(question.Notes);
        }


        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests parsing of detail questions whose answers contain a chapter and verse number(s)
        /// in parentheses and/or whose sections cross chapter boundaries.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void ParseBasicQuestions_InterpretVerseNumbersInAnswers_WithChapterNumbers()
        {
            QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
                @"\rf Genesis 27:41-28:5 Jacob fled (from) Esau.",
                @"\oh Overview",
                @"\tqref GEN 27.41-28.5",
                @"\bttq What was happening at the time of this incident?",
                @"\tqe Esau was plotting to take revenge on Jacob by killing him, so Rebekah told Jacob that he must flee. Rebekah told Isaac that she did not want Jacob to marry a Canaanite woman. Therefore Isaac told Jacob to go to his uncle Laban and to marry one of Laban's daughters. Then Isaac prayed a blessing for/asked God to bless/help Jacob.",
                @"\dh Details",
				@"\tqref GEN 27.46-28.5",
				@"\bttq What did Rebekah say to her husband, Isaac?",
				@"\tqe She said she was upset with Esau's Hittite wives and that she would die if Jacob took a Hittite wife. (46)",
				@"\bttq After she told this to Isaac, what did he do?",
				@"\tqe He summoned Jacob. (28:1)",
				@"\rf Luke 2:39-40 The family returned to Nazareth.",
				@"\oh Overview",
				@"\tqref LUK 2.39-40",
				@"\bttq What do you think that we are to learn from what Luke wrote about the family returning to Nazareth?",
				@"\tqe Luke told Theophilus (1:1-4) that he wanted to present a true account of Jesus's life.",
				@"\dh Details",
				@"\bttq When were Joseph and Mary ready to return to their home in Nazareth?",
				@"\tqe They were ready when they had finished doing everything that the Law of the Lord commanded them to do. (2:39)",
				@"\rf Luke 9:57-62 People who want to follow Jesus must let go of everything else they think is important.",
				@"\dh Details",
				@"\tqref LUK 9.57-58",
				@"\bttq Where were Jesus and his disciples going?",
				@"\tqe They were still headed towards Jerusalem. (9:51)",
				@"\bttq A man approached Jesus along the road. What did this man say to Jesus?",
				@"\tqe He said that he would follow Jesus wherever Jesus went. (57)"}, null);

            Assert.AreEqual(3, sections.Items.Length);

            // Genesis
            Section section = sections.Items[0];
            Assert.AreEqual("Genesis 27:41-28:5 Jacob fled (from) Esau.", section.Heading);
            Assert.AreEqual("GEN 27.41-28.5", section.ScriptureReference);
            Assert.AreEqual(2, section.Categories.Length);

            Category category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(2, category.Questions.Count);
            Question question = category.Questions[0];
            Assert.AreEqual("What did Rebekah say to her husband, Isaac?", question.Text);
            Assert.AreEqual("GEN 27.46", question.ScriptureReference);
            Assert.AreEqual(001027046, question.StartRef);
            Assert.AreEqual(001027046, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("She said she was upset with Esau's Hittite wives and that she would die if Jacob took a Hittite wife. (46)", question.Answers[0]);

            question = category.Questions[1];
            Assert.AreEqual("After she told this to Isaac, what did he do?", question.Text);
            Assert.AreEqual("GEN 28.1", question.ScriptureReference);
            Assert.AreEqual(001028001, question.StartRef);
            Assert.AreEqual(001028001, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("He summoned Jacob. (28:1)", question.Answers[0]);

            //Luke
            section = sections.Items[1];
            Assert.AreEqual("Luke 2:39-40 The family returned to Nazareth.", section.Heading);
            Assert.AreEqual("LUK 2.39-40", section.ScriptureReference);
            Assert.AreEqual(2, section.Categories.Length);

            category = section.Categories[0];
            Assert.AreEqual("Overview", category.Type);

            Assert.AreEqual(1, category.Questions.Count);
            question = category.Questions[0];
            Assert.AreEqual("What do you think that we are to learn from what Luke wrote about the family returning to Nazareth?", question.Text);
            Assert.IsNull(question.ScriptureReference);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("Luke told Theophilus (1:1-4) that he wanted to present a true account of Jesus's life.", question.Answers[0]);

            category = section.Categories[1];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(1, category.Questions.Count);
            question = category.Questions[0];
            Assert.AreEqual("When were Joseph and Mary ready to return to their home in Nazareth?", question.Text);
            Assert.AreEqual("LUK 2.39", question.ScriptureReference);
            Assert.AreEqual(042002039, question.StartRef);
            Assert.AreEqual(042002039, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("They were ready when they had finished doing everything that the Law of the Lord commanded them to do. (2:39)", question.Answers[0]);

            section = sections.Items[2];
            Assert.AreEqual("Luke 9:57-62 People who want to follow Jesus must let go of everything else they think is important.", section.Heading);
            Assert.AreEqual("LUK 9.57-58", section.ScriptureReference);
            Assert.AreEqual(1, section.Categories.Length);

            category = section.Categories[0];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(2, category.Questions.Count);
            question = category.Questions[0];
            Assert.AreEqual("Where were Jesus and his disciples going?", question.Text);
            Assert.IsNull(question.ScriptureReference);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("They were still headed towards Jerusalem. (9:51)", question.Answers[0]);

            question = category.Questions[1];
            Assert.AreEqual("A man approached Jesus along the road. What did this man say to Jesus?", question.Text);
            Assert.AreEqual("LUK 9.57", question.ScriptureReference);
            Assert.AreEqual(042009057, question.StartRef);
            Assert.AreEqual(042009057, question.EndRef);
            Assert.AreEqual(1, question.Answers.Length);
            Assert.AreEqual("He said that he would follow Jesus wherever Jesus went. (57)", question.Answers[0]);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests parsing of detail questions whose answers contain a verse number in parentheses.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void ParseBasicQuestions_InterpretVerseNumbersInAnswers_WithSubverseLetters()
        {
            QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
                @"\rf Luke 11:37-54 Jesus warned the religious leaders that God would punish their hypocrisy.",
                @"\dh Details",
                @"\tqref LUK 11.53-54",
                @"\bttq What happened after Jesus finished talking and left that place?",
                @"\tqe The Pharisees and scribes began to oppose Jesus fiercely. (53a)",
                @"\tqe They raised a lot of questions (53b), just waiting until he said something (54)."}, null);

            Assert.AreEqual(1, sections.Items.Length);

            Section section = sections.Items[0];
            Assert.AreEqual("Luke 11:37-54 Jesus warned the religious leaders that God would punish their hypocrisy.", section.Heading);
            Assert.AreEqual("LUK 11.53-54", section.ScriptureReference);
            Assert.AreEqual(1, section.Categories.Length);

            Category category = section.Categories[0];
            Assert.AreEqual("Details", category.Type);

            Assert.AreEqual(1, category.Questions.Count);
            Question question = category.Questions[0];
            Assert.AreEqual("What happened after Jesus finished talking and left that place?", question.Text);
            Assert.AreEqual("LUK 11.53-54", question.ScriptureReference);
            Assert.AreEqual(042011053, question.StartRef);
            Assert.AreEqual(042011054, question.EndRef);
            Assert.AreEqual(2, question.Answers.Length);
            Assert.AreEqual("The Pharisees and scribes began to oppose Jesus fiercely. (53a)", question.Answers[0]);
            Assert.AreEqual("They raised a lot of questions (53b), just waiting until he said something (54).", question.Answers[1]);
        }

        ///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions which ahppen to be split across 2 or more consecutive
		/// \bttq fields.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseBasicQuestions_QuestionsSplitAcrossConsecutiveFields()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Luke 1:1-79 Introduction to the book.",
				@"\dh Details",
				@"\tqref LUK 1.1-79",
				@"\bttq For what reason did Paul remind the people in the synagogue that after David had died and ",
				@"\bttq [people] had buried his body, his body had decayed?",
				@"\tqe Paul wanted the people in the synagogue to recognize that David was not speaking about himself...",
				@"\bttq [IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK:]",
				@"\bttq Tell me what the picture language about the sun/darkness/shadows means to you?",
				@"\tqe [THE SUN:] The sun is a picture of the savior coming from heaven to start a new day, a new period in our lives. He makes it clear to us how he saves us. Now we are encouraged about what is going to happen to us.",
				@"\tqe [DARKNESS/SHADOWS:] The time of not knowing about the true God and of being afraid that something, perhaps God or evil spirits, might soon make us die, ends. (78-79)"}, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Details", category.Type);

			Assert.AreEqual(2, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("For what reason did Paul remind the people in the synagogue that after David had died and [people] had buried his body, his body had decayed?", question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Paul wanted the people in the synagogue to recognize that David was not speaking about himself...", question.Answers[0]);

			question = category.Questions[1];

			Assert.AreEqual("[IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK:] Tell me what the picture language about the sun/darkness/shadows means to you?", question.Text);
			Assert.AreEqual("LUK 1.78-79", question.ScriptureReference);
			Assert.AreEqual(2, question.Answers.Length);
			Assert.AreEqual("[THE SUN:] The sun is a picture of the savior coming from heaven to start a new day, a new period in our lives. He makes it clear to us how he saves us. Now we are encouraged about what is going to happen to us.", question.Answers[0]);
			Assert.AreEqual("[DARKNESS/SHADOWS:] The time of not knowing about the true God and of being afraid that something, perhaps God or evil spirits, might soon make us die, ends. (78-79)", question.Answers[1]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that have alternatives pairs of words or phrases indicated
		/// by a forward slash.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_Slashes()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
		            @"\rf Acts 1:1-14",
		            @"\oh Overview",
		            @"\tqref ACT 1.1-14",
		            @"\bttq What do you think it means if/that someone condemns someone else?",
		            @"\tqe Answer A",
		            @"\bttq In the letter that Claudius Lysias wrote, what charges/accusations did he say/write that the members of the Sanhedrin made against Paul?",
		            @"\tqe Answer B",
		         	@"\bttq Tell me what the picture language about the sun/darkness/shadows means to you?",
					@"\tqe Nothing",
					@"\bttq Pretend you are the angel and this table/chair/bench/tree is the altar. Show me where the angel was standing according to this verse.",
					@"\tqe Here!",
					@"\bttq What does he say about teaching/lessons?",
					@"\tqe Not much",
					@"\bttq Where was Paul when he wrote/was writing this letter?",
		            @"\tqe Answer C",
		            @"\bttq What is the reason that there is no real value in people carefully obeying/for people to carefully obey such rules?",
		            @"\tqe Answer D",
		            @"\bttq What does Christ do for the body/the church/the believers?",
		            @"\tqe Answer E",
		            @"\bttq What did the prophets write about Jesus/the Messiah long ago?",
		            @"\tqe Answer F"}, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];
			Assert.AreEqual("Acts 1:1-14", section.Heading);

			Assert.AreEqual(1, section.Categories.Length);
			Category category = section.Categories[0];

			Assert.AreEqual(9, category.Questions.Count);

			Question question = category.Questions[0];
			Assert.AreEqual("What do you think it means if/that someone condemns someone else?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What do you think it means if someone condemns someone else?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What do you think it means that someone condemns someone else?", question.AlternateForms.ElementAt(1));

			question = category.Questions[1];
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges/accusations did he say/write that the members of the Sanhedrin made against Paul?", question.Text);
			Assert.AreEqual(4, question.AlternateForms.Count());
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges did he say that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges did he write that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what accusations did he say that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(2));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what accusations did he write that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(3));

			question = category.Questions[2];
			Assert.AreEqual("Tell me what the picture language about the sun/darkness/shadows means to you?", question.Text);
			Assert.AreEqual(3, question.AlternateForms.Count());
			Assert.AreEqual("Tell me what the picture language about the sun means to you?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("Tell me what the picture language about the darkness means to you?", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("Tell me what the picture language about the shadows means to you?", question.AlternateForms.ElementAt(2));

			question = category.Questions[3];
			Assert.AreEqual("Pretend you are the angel and this table/chair/bench/tree is the altar. Show me where the angel was standing according to this verse.", question.Text);
			Assert.AreEqual(4, question.AlternateForms.Count());
			Assert.AreEqual("Pretend you are the angel and this table is the altar. Show me where the angel was standing according to this verse.", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("Pretend you are the angel and this chair is the altar. Show me where the angel was standing according to this verse.", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("Pretend you are the angel and this bench is the altar. Show me where the angel was standing according to this verse.", question.AlternateForms.ElementAt(2));
			Assert.AreEqual("Pretend you are the angel and this tree is the altar. Show me where the angel was standing according to this verse.", question.AlternateForms.ElementAt(3));

			question = category.Questions[4];
			Assert.AreEqual("What does he say about teaching/lessons?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What does he say about teaching?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What does he say about lessons?", question.AlternateForms.ElementAt(1));

			// For the remaining cases the results are not ideal, but this is the best we can do without a helper file to clarify the intended meaning

			question = category.Questions[5];
			Assert.AreEqual("Where was Paul when he wrote/was writing this letter?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("Where was Paul when he wrote writing this letter?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("Where was Paul when he was writing this letter?", question.AlternateForms.ElementAt(1));

			question = category.Questions[6];
			Assert.AreEqual("What is the reason that there is no real value in people carefully obeying/for people to carefully obey such rules?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What is the reason that there is no real value in people carefully obeying people to carefully obey such rules?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What is the reason that there is no real value in people carefully for people to carefully obey such rules?", question.AlternateForms.ElementAt(1));

			question = category.Questions[7];
			Assert.AreEqual("What does Christ do for the body/the church/the believers?", question.Text);
			Assert.AreEqual(4, question.AlternateForms.Count());
			Assert.AreEqual("What does Christ do for the body church believers?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What does Christ do for the body the believers?", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("What does Christ do for the the church believers?", question.AlternateForms.ElementAt(2));
			Assert.AreEqual("What does Christ do for the the the believers?", question.AlternateForms.ElementAt(3));

			question = category.Questions[8];
			Assert.AreEqual("What did the prophets write about Jesus/the Messiah long ago?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What did the prophets write about Jesus Messiah long ago?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What did the prophets write about the Messiah long ago?", question.AlternateForms.ElementAt(1));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that have alternatives pairs of words or phrases indicated
		/// by a forward slash. This version uses some helper information to deal with problem
		/// cases.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_Slashes_UsingAlternativeOverrides()
		{
			Dictionary<string, string[]> alternatives = new Dictionary<string, string[]>();
			alternatives["Where was Paul when he wrote/was writing this letter?"] = new[] { "Where was Paul when he wrote this letter?", "Where was Paul when he was writing this letter?" };
			alternatives["What is the reason that there is no real value in people carefully obeying/for people to carefully obey such rules?"] = new[] { "What is the reason that there is no real value in people carefully obeying such rules?", "What is the reason that there is no real value for people to carefully obey such rules?" };
			alternatives["What does Christ do for the body/the church/the believers?"] = new[] { "What does Christ do for the body?", "What does Christ do for the church?", "What does Christ do for the believers?" };
			alternatives["What did the prophets write about Jesus/the Messiah long ago?"] = new[] { "What did the prophets write about Jesus long ago?", "What did the prophets write about the Messiah long ago?" };
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
		            @"\rf Acts 1:1-14",
		            @"\oh Overview",
		            @"\tqref ACT 1.1-14",
		            @"\bttq What do you think it means if/that someone condemns someone else?",
		            @"\tqe Answer A",
		            @"\bttq In the letter that Claudius Lysias wrote, what charges/accusations did he say/write that the members of the Sanhedrin made against Paul?",
		            @"\tqe Answer B",
		            @"\bttq Where was Paul when he wrote/was writing this letter?",
		            @"\tqe Answer C",
		            @"\bttq What is the reason that there is no real value in people carefully obeying/for people to carefully obey such rules?",
		            @"\tqe Answer D",
		            @"\bttq What does Christ do for the body/the church/the believers?",
		            @"\tqe Answer E",
		            @"\bttq What did the prophets write about Jesus/the Messiah long ago?",
		            @"\tqe Answer F"}, alternatives);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];
			Assert.AreEqual("Acts 1:1-14", section.Heading);

			Assert.AreEqual(1, section.Categories.Length);
			Category category = section.Categories[0];

			Assert.AreEqual(6, category.Questions.Count);

			Question question = category.Questions[0];
			Assert.AreEqual("What do you think it means if/that someone condemns someone else?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What do you think it means if someone condemns someone else?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What do you think it means that someone condemns someone else?", question.AlternateForms.ElementAt(1));

			question = category.Questions[1];
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges/accusations did he say/write that the members of the Sanhedrin made against Paul?", question.Text);
			Assert.AreEqual(4, question.AlternateForms.Count());
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges did he say that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what charges did he write that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what accusations did he say that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(2));
			Assert.AreEqual("In the letter that Claudius Lysias wrote, what accusations did he write that the members of the Sanhedrin made against Paul?", question.AlternateForms.ElementAt(3));

			question = category.Questions[2];
			Assert.AreEqual("Where was Paul when he wrote/was writing this letter?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("Where was Paul when he wrote this letter?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("Where was Paul when he was writing this letter?", question.AlternateForms.ElementAt(1));

			question = category.Questions[3];
			Assert.AreEqual("What is the reason that there is no real value in people carefully obeying/for people to carefully obey such rules?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What is the reason that there is no real value in people carefully obeying such rules?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What is the reason that there is no real value for people to carefully obey such rules?", question.AlternateForms.ElementAt(1));

			question = category.Questions[4];
			Assert.AreEqual("What does Christ do for the body/the church/the believers?", question.Text);
			Assert.AreEqual(3, question.AlternateForms.Count());
			Assert.AreEqual("What does Christ do for the body?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What does Christ do for the church?", question.AlternateForms.ElementAt(1));
			Assert.AreEqual("What does Christ do for the believers?", question.AlternateForms.ElementAt(2));

			question = category.Questions[5];
			Assert.AreEqual("What did the prophets write about Jesus/the Messiah long ago?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Count());
			Assert.AreEqual("What did the prophets write about Jesus long ago?", question.AlternateForms.ElementAt(0));
			Assert.AreEqual("What did the prophets write about the Messiah long ago?", question.AlternateForms.ElementAt(1));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions with words or phrases in brackets, indicating that they
		/// are optional.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_OptionalWordsInBrackets()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Luke 1:1-79 Introduction to the book.",
				@"\dh Details",
				@"\tqref LUK 1.1-79",
				@"\bttq [Read verses 22-23 again. Then ask this question:] How do those words of the important man in your language sound to you? Do the words make you think that he was happy or frustrated or sad or angry?",
				@"\tqe He was frustrated and angry.",
				@"\bttq For what reason did Paul remind the people in the synagogue that after David had died and ",
				@"\bttq [people] had buried his body, his body had decayed?",
				@"\tqe Paul wanted the people in the synagogue to recognize that David was not speaking about himself...",
				@"\bttq [IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK:]",
				@"\bttq Tell me what the picture language about the sun/darkness/shadows means to you?",
				@"\tqe [THE SUN:] The sun is a picture of the savior coming from heaven to start a new day, a new period in our lives. He makes it clear to us how he saves us. Now we are encouraged about what is going to happen to us.",
				@"\tqe [DARKNESS/SHADOWS:] The time of not knowing about the true God and of being afraid that something, perhaps God or evil spirits, might soon make us die, ends. (78-79)"}, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Details", category.Type);

			Assert.AreEqual(3, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("[Read verses 22-23 again. Then ask this question:] How do those words of the important man in your language sound to you? Do the words make you think that he was happy or frustrated or sad or angry?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Length);
			Assert.AreEqual("Read verses 22-23 again. Then ask this question: How do those words of the important man in your language sound to you? Do the words make you think that he was happy or frustrated or sad or angry?", question.AlternateForms[0]);
			Assert.AreEqual("How do those words of the important man in your language sound to you? Do the words make you think that he was happy or frustrated or sad or angry?", question.AlternateForms[1]);

			question = category.Questions[1];

			Assert.AreEqual("For what reason did Paul remind the people in the synagogue that after David had died and [people] had buried his body, his body had decayed?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Length);
			Assert.AreEqual("For what reason did Paul remind the people in the synagogue that after David had died and people had buried his body, his body had decayed?", question.AlternateForms[0]);
			Assert.AreEqual("For what reason did Paul remind the people in the synagogue that after David had died and had buried his body, his body had decayed?", question.AlternateForms[1]);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Paul wanted the people in the synagogue to recognize that David was not speaking about himself...", question.Answers[0]);

			question = category.Questions[2];

			Assert.AreEqual("[IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK:] Tell me what the picture language about the sun/darkness/shadows means to you?", question.Text);
			Assert.AreEqual(6, question.AlternateForms.Length);
			Assert.AreEqual("IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK: Tell me what the picture language about the sun means to you?", question.AlternateForms[0]);
			Assert.AreEqual("IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK: Tell me what the picture language about the darkness means to you?", question.AlternateForms[1]);
			Assert.AreEqual("IF THE TRANSLATION OF VERSES 78-79 HAS PICTURE LANGUAGE ABOUT THE SUN OR DARKNESS OR SHADOWS, ASK: Tell me what the picture language about the shadows means to you?", question.AlternateForms[2]);
			Assert.AreEqual("Tell me what the picture language about the sun means to you?", question.AlternateForms[3]);
			Assert.AreEqual("Tell me what the picture language about the darkness means to you?", question.AlternateForms[4]);
			Assert.AreEqual("Tell me what the picture language about the shadows means to you?", question.AlternateForms[5]);
			Assert.AreEqual("LUK 1.78-79", question.ScriptureReference);
			Assert.AreEqual(2, question.Answers.Length);
			Assert.AreEqual("[THE SUN:] The sun is a picture of the savior coming from heaven to start a new day, a new period in our lives. He makes it clear to us how he saves us. Now we are encouraged about what is going to happen to us.", question.Answers[0]);
			Assert.AreEqual("[DARKNESS/SHADOWS:] The time of not knowing about the true God and of being afraid that something, perhaps God or evil spirits, might soon make us die, ends. (78-79)", question.Answers[1]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions with optional (bracketed) phrases that contain alternative
		/// parts.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_OptionalWordsWithAlternatePhrases()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Galatians 6:1-10 All believers in Christ should help each other when they suffer or are tempted.",
				@"\oh Overview",
				@"\tqref GAL 6.1-10",
				@"\bttq What do you think it means to be tempted [by someone/something] to do something bad/wrong?",
				@"\tqe Key Term Check: It means to try to persuade somebody to do something sinful."}, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Overview", category.Type);

			Assert.AreEqual(1, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("What do you think it means to be tempted [by someone/something] to do something bad/wrong?", question.Text);
			Assert.AreEqual(6, question.AlternateForms.Length);
			Assert.AreEqual("What do you think it means to be tempted by someone to do something bad?", question.AlternateForms[0]);
			Assert.AreEqual("What do you think it means to be tempted by someone to do something wrong?", question.AlternateForms[1]);
			Assert.AreEqual("What do you think it means to be tempted by something to do something bad?", question.AlternateForms[2]);
			Assert.AreEqual("What do you think it means to be tempted by something to do something wrong?", question.AlternateForms[3]);
			Assert.AreEqual("What do you think it means to be tempted to do something bad?", question.AlternateForms[4]);
			Assert.AreEqual("What do you think it means to be tempted to do something wrong?", question.AlternateForms[5]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that have two complete alternate forms of the question in
		/// a single \bttq field (alternates separated by an "OR" or by a double-slash (//).
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_CompleteQuestionsInLine()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Luke 1:1-79 Introduction to the book.",
				@"\dh Details",
				@"\tqref LUK 1.1-79",
				@"\bttq Who rules the believers now? // To whom do the believers now belong?",
				@"\tqe God has now caused his Son, Jesus, whom he loves, to rule the believers. // God has now caused the believers to become Christ's people. (13)",
				@"\bttq According to God, for what reason would his Spirit not remain with people/mankind forever/for a long time? -OR- According to God, for what reason would he not allow people/mankind to live forever/for a long time?",
				@"\tqe God's Spirit would not remain with people/mankind forever/for a long time. // God would not allow people/mankind to live forever/for a long time, because people chose to give in to the temptations of the flesh. (3)",
				@"\bttq What does he say about teaching/lessons? (or, What does the author want/tell the readers to do?)",
				@"\tqe Any answer is fine." }, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Details", category.Type);

			Assert.AreEqual(3, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("Who rules the believers now? // To whom do the believers now belong?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Length);
			Assert.AreEqual("Who rules the believers now?", question.AlternateForms[0]);
			Assert.AreEqual("To whom do the believers now belong?", question.AlternateForms[1]);

			question = category.Questions[1];

			Assert.AreEqual("According to God, for what reason would his Spirit not remain with people/mankind forever/for a long time? -OR- According to God, for what reason would he not allow people/mankind to live forever/for a long time?", question.Text);
			Assert.AreEqual(8, question.AlternateForms.Length);
			Assert.AreEqual("According to God, for what reason would his Spirit not remain with people forever a long time?", question.AlternateForms[0]);
			Assert.AreEqual("According to God, for what reason would his Spirit not remain with people for a long time?", question.AlternateForms[1]);
			Assert.AreEqual("According to God, for what reason would his Spirit not remain with mankind forever a long time?", question.AlternateForms[2]);
			Assert.AreEqual("According to God, for what reason would his Spirit not remain with mankind for a long time?", question.AlternateForms[3]);
			Assert.AreEqual("According to God, for what reason would he not allow people to live forever a long time?", question.AlternateForms[4]);
			Assert.AreEqual("According to God, for what reason would he not allow people to live for a long time?", question.AlternateForms[5]);
			Assert.AreEqual("According to God, for what reason would he not allow mankind to live forever a long time?", question.AlternateForms[6]);
			Assert.AreEqual("According to God, for what reason would he not allow mankind to live for a long time?", question.AlternateForms[7]);

			question = category.Questions[2];

			Assert.AreEqual("What does he say about teaching/lessons? (or, What does the author want/tell the readers to do?)", question.Text);
			Assert.AreEqual(4, question.AlternateForms.Length);
			Assert.AreEqual("What does he say about teaching?", question.AlternateForms[0]);
			Assert.AreEqual("What does he say about lessons?", question.AlternateForms[1]);
			Assert.AreEqual("What does the author want the readers to do?", question.AlternateForms[2]);
			Assert.AreEqual("What does the author tell the readers to do?", question.AlternateForms[3]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that have two complete alternate forms of the question in
		/// separate \bttq fields, with another \bttq field in between that just contains the
		/// word "OR" or "-OR-".
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestionsWithAlternatives_OrInSeparateField()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Luke 1:1-79 Introduction to the book.",
				@"\dh Details",
				@"\tqref LUK 1.1-79",
				@"\bttq What would the shepherds see which would prove to them that what the angel said was true?",
				@"\bttq -OR-",
				@"\bttq How would the shepherds know if they had found the baby which the angel was talking about?",
				@"\tqe They would find the baby wrapped in pieces of cloth (according to their Jewish custom) (12b)",
				@"\tqe and lying in a manger, that is, an animal's food trough/box. (12c)"}, null);

			Assert.AreEqual(1, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.AreEqual("Details", category.Type);

			Assert.AreEqual(1, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("What would the shepherds see which would prove to them that what the angel said was true? -OR- How would the shepherds know if they had found the baby which the angel was talking about?", question.Text);
			Assert.AreEqual(2, question.AlternateForms.Length);
			Assert.AreEqual("What would the shepherds see which would prove to them that what the angel said was true?", question.AlternateForms[0]);
			Assert.AreEqual("How would the shepherds know if they had found the baby which the angel was talking about?", question.AlternateForms[1]);
			Assert.AreEqual(2, question.Answers.Length);
			Assert.AreEqual("They would find the baby wrapped in pieces of cloth (according to their Jewish custom) (12b)", question.Answers[0]);
			Assert.AreEqual("and lying in a manger, that is, an animal's food trough/box. (12c)", question.Answers[1]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that are in a section that does not have any \oh or \dh
		/// markers
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestions_NoExplicitCategories()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Romans 1:1-7 Introduction.",
				@"\tqref ROM 1.1-7",
				@"\tqref ROM 1.1",
				@"\tqref ROM 1.6-7",
				@"\bttq (a) What has God called Paul to become?",
				@"\tqe God has called Paul to be one of Christ's servants and representatives of God in telling the good news.",
				@"\bttq (b) What has God called the people in Rome to become?",
				@"\tqe God called the people of Rome to be God's own people, whom he loves.",
				@"\tqref ROM 1.2-3",
				@"\bttq What two things is Christ?",
				@"\tqe He is a human being, and he is the Son of God.",
				@"\rf Romans 1:8-17 Paul prays that God will give him the chance to go to Rome to preach the gospel.",
				@"\tqref ROM 1.8-17",
				@"\tqref ROM 1.8",
				@"\bttq What news makes Paul happy?",
				@"\tqe He has heard that everyone everywhere is speaking of how the people in Rome to whom he is writing have believed in Christ.",
				@"\tqref ROM 1.9-10",
				@"\bttq What good example does Paul show us?",
				@"\tqe He prays for them every time he prays to God."}, null);

			Assert.AreEqual(2, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual("Romans 1:1-7 Introduction.", section.Heading);
			Assert.AreEqual("ROM 1.1-7", section.ScriptureReference);
			Assert.AreEqual(45001001, section.StartRef);
			Assert.AreEqual(45001007, section.EndRef);
			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.IsNull(category.Type);

			Assert.AreEqual(3, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("(a) What has God called Paul to become?", question.Text);
			Assert.AreEqual("ROM 1.6-7", question.ScriptureReference);
			Assert.AreEqual(45001006, question.StartRef);
			Assert.AreEqual(45001007, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("God has called Paul to be one of Christ's servants and representatives of God in telling the good news.", question.Answers[0]);

			question = category.Questions[1];

			Assert.AreEqual("(b) What has God called the people in Rome to become?", question.Text);
			Assert.AreEqual("ROM 1.6-7", question.ScriptureReference);
			Assert.AreEqual(45001006, question.StartRef);
			Assert.AreEqual(45001007, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("God called the people of Rome to be God's own people, whom he loves.", question.Answers[0]);

			question = category.Questions[2];

			Assert.AreEqual("What two things is Christ?", question.Text);
			Assert.AreEqual("ROM 1.2-3", question.ScriptureReference);
			Assert.AreEqual(45001002, question.StartRef);
			Assert.AreEqual(45001003, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("He is a human being, and he is the Son of God.", question.Answers[0]);

			section = sections.Items[1];

			Assert.AreEqual("Romans 1:8-17 Paul prays that God will give him the chance to go to Rome to preach the gospel.", section.Heading);
			Assert.AreEqual("ROM 1.8-17", section.ScriptureReference);
			Assert.AreEqual(45001008, section.StartRef);
			Assert.AreEqual(45001017, section.EndRef);
			Assert.AreEqual(1, section.Categories.Length);

			category = section.Categories[0];
			Assert.IsNull(category.Type);

			Assert.AreEqual(2, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("What news makes Paul happy?", question.Text);
			Assert.AreEqual("ROM 1.8", question.ScriptureReference);
			Assert.AreEqual(45001008, question.StartRef);
			Assert.AreEqual(45001008, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("He has heard that everyone everywhere is speaking of how the people in Rome to whom he is writing have believed in Christ.", question.Answers[0]);

			question = category.Questions[1];

			Assert.AreEqual("What good example does Paul show us?", question.Text);
			Assert.AreEqual("ROM 1.9-10", question.ScriptureReference);
			Assert.AreEqual(45001009, question.StartRef);
			Assert.AreEqual(45001010, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("He prays for them every time he prays to God.", question.Answers[0]);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests parsing of questions that are in a section that does not have any \oh or \dh
		/// markers, followed by a section (in a different book) that does have explicit category
		/// markers
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ParseQuestions_TransitionFromImplicitToExplicitCategories()
		{
			QuestionSections sections = QuestionSfmFileAccessor.Generate(new[] {
				@"\rf Romans 16:17-27 Paul's last words.",
				@"\tqref ROM 16.17-27",
				@"\tqref ROM 16.17-18",
				@"\bttq What are the greatest dangers to a church?",
				@"\tqe Those who are church members who cause disunity, and confusion and who teach things that are against what we have already learned.",
				@"\tqref ROM 16.25-27",
				@"\bttq What must be done with this Good News?",
				@"\tqe It must be preached to all peoples of this world so that they might believe and follow it.",
				@"\rf Titus 1:1-4",
				@"\oh Overview",
				@"\tqref TIT 1.1-4",
				@"\bttq Tell in your own words what Paul said.",
				@"\tqe Paul is writing a letter to Titus.",
				@"\dh Details",
				@"\tqref TIT 1.1",
				@"\bttq Who wrote this letter?",
				@"\tqe Paul."}, null);

			Assert.AreEqual(2, sections.Items.Length);

			Section section = sections.Items[0];

			Assert.AreEqual("Romans 16:17-27 Paul's last words.", section.Heading);
			Assert.AreEqual("ROM 16.17-27", section.ScriptureReference);
			Assert.AreEqual(45016017, section.StartRef);
			Assert.AreEqual(45016027, section.EndRef);
			Assert.AreEqual(1, section.Categories.Length);

			Category category = section.Categories[0];
			Assert.IsNull(category.Type);

			Assert.AreEqual(2, category.Questions.Count);

			Question question = category.Questions[0];

			Assert.AreEqual("What are the greatest dangers to a church?", question.Text);
			Assert.AreEqual("ROM 16.17-18", question.ScriptureReference);
			Assert.AreEqual(45016017, question.StartRef);
			Assert.AreEqual(45016018, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Those who are church members who cause disunity, and confusion and who teach things that are against what we have already learned.", question.Answers[0]);

			question = category.Questions[1];

			Assert.AreEqual("What must be done with this Good News?", question.Text);
			Assert.AreEqual("ROM 16.25-27", question.ScriptureReference);
			Assert.AreEqual(45016025, question.StartRef);
			Assert.AreEqual(45016027, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("It must be preached to all peoples of this world so that they might believe and follow it.", question.Answers[0]);

			section = sections.Items[1];

			Assert.AreEqual("Titus 1:1-4", section.Heading);
			Assert.AreEqual("TIT 1.1-4", section.ScriptureReference);
			Assert.AreEqual(56001001, section.StartRef);
			Assert.AreEqual(56001004, section.EndRef);
			Assert.AreEqual(2, section.Categories.Length);

			category = section.Categories[0];
			Assert.AreEqual("Overview", category.Type);

			Assert.AreEqual(1, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("Tell in your own words what Paul said.", question.Text);
			Assert.IsNull(question.ScriptureReference);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Paul is writing a letter to Titus.", question.Answers[0]);

			category = section.Categories[1];
			Assert.AreEqual("Details", category.Type);

			Assert.AreEqual(1, category.Questions.Count);

			question = category.Questions[0];

			Assert.AreEqual("Who wrote this letter?", question.Text);
			Assert.AreEqual("TIT 1.1", question.ScriptureReference);
			Assert.AreEqual(56001001, question.StartRef);
			Assert.AreEqual(56001001, question.EndRef);
			Assert.AreEqual(1, question.Answers.Length);
			Assert.AreEqual("Paul.", question.Answers[0]);
		}
	}
}
