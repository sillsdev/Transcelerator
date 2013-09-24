// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: QuestionProviderTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests the QuestionProviderBase implementation
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class QuestionProviderTests
	{
		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests enumerating overview and detail categories and questions with answers and
		/// comments.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void EnumeratePhrases_Basic()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[2];
			int iS= 0;
			qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
				44001005, 2, 1);
			int iC = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What information did Luke, the writer of this book, give in this introduction?";
			q.Answers = new [] { "Luke reminded his readers that he was about to continue the true story about Jesus" };
			q = qs.Items[iS].Categories[iC].Questions[1];
			q.Text = "What do you think an apostle of Jesus is?";
			q.Answers = new [] { "Key Term Check: To be an apostle of Jesus means to be a messenger", "Can also be translated as \"sent one\"" };
			q.Notes = new [] {"Note: apostles can be real sweethearts sometimes"};

			iC = 1;
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "To whom did the writer of Acts address this book?";
			q.Answers = new [] { "He addressed this book to Theophilus." };

			iS= 1;
			qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.6-10", "Acts 1:6-10 The continuing saga.", 44001006, 44001010, 1, 1);
			iC = 0;
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What happened?";
			q.Answers = new [] { "Stuff" };

			iC = 1;
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.ScriptureReference = "ACT 1.6";
			q.StartRef = 44001006;
			q.EndRef = 44001006;
			q.Text = "What question did the apostles ask Jesus about his kingdom?";
			q.Answers = new [] { "The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom." };

			QuestionProvider qp = new QuestionProvider(qs, null);

			Assert.AreEqual(2, qp.SectionHeads.Count);
			Assert.AreEqual("Acts 1:1-5 Introduction to the book.", qp.SectionHeads["ACT 1.1-5"]);
			Assert.AreEqual("Acts 1:6-10 The continuing saga.", qp.SectionHeads["ACT 1.6-10"]);
			Assert.AreEqual(1, qp.AvailableBookIds.Length);
			Assert.AreEqual(44, qp.AvailableBookIds[0]);

			List<TranslatablePhrase> phrases = qp.ToList();
			Assert.AreEqual(7, phrases.Count);

			TranslatablePhrase phrase = phrases[0];
			Assert.AreEqual("Overview", phrase.PhraseInUse);
			Assert.AreEqual(-1, phrase.Category);
			Assert.AreEqual(string.Empty, phrase.Reference);
			Assert.AreEqual(001001001, phrase.StartRef);
			Assert.AreEqual(066022021, phrase.EndRef);
			Assert.AreEqual(0, phrase.SequenceNumber);
			Assert.IsNull(phrase.QuestionInfo);
            VerifyTranslatablePhrase(phrase, "overview", 1);

			phrase = phrases[1];
			Assert.AreEqual("What information did Luke, the writer of this book, give in this introduction?",
				phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ACT 1.1-5", phrase.Reference);
			Assert.AreEqual(44001001, phrase.StartRef);
			Assert.AreEqual(44001005, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.IsNull(phrase.QuestionInfo.Notes);
			Assert.AreEqual("Luke reminded his readers that he was about to continue the true story about Jesus",
				phrase.QuestionInfo.Answers.First());

			phrase = phrases[2];
			Assert.AreEqual("What do you think an apostle of Jesus is?", phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ACT 1.1-5", phrase.Reference);
			Assert.AreEqual(44001001, phrase.StartRef);
			Assert.AreEqual(44001005, phrase.EndRef);
			Assert.AreEqual(phrases[1].SequenceNumber + 1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(2, phrase.QuestionInfo.Answers.Count());
			Assert.AreEqual(1, phrase.QuestionInfo.Notes.Count());
			Assert.AreEqual("Key Term Check: To be an apostle of Jesus means to be a messenger",
				phrase.QuestionInfo.Answers.ElementAt(0));
			Assert.AreEqual("Can also be translated as \"sent one\"",
				phrase.QuestionInfo.Answers.ElementAt(1));
			Assert.AreEqual("Note: apostles can be real sweethearts sometimes",
				phrase.QuestionInfo.Notes.ElementAt(0));

			phrase = phrases[3];
			Assert.AreEqual("Details", phrase.PhraseInUse);
			Assert.AreEqual(-1, phrase.Category);
			Assert.AreEqual(string.Empty, phrase.Reference);
			Assert.AreEqual(001001001, phrase.StartRef);
			Assert.AreEqual(066022021, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNull(phrase.QuestionInfo);
            VerifyTranslatablePhrase(phrase, "details", 1);

			phrase = phrases[4];
			Assert.AreEqual("To whom did the writer of Acts address this book?",
				phrase.PhraseInUse);
			Assert.AreEqual(1, phrase.Category);
			Assert.AreEqual("ACT 1.1-5", phrase.Reference);
			Assert.AreEqual(44001001, phrase.StartRef);
			Assert.AreEqual(44001005, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.IsNull(phrase.QuestionInfo.Notes);
			Assert.AreEqual("He addressed this book to Theophilus.",
				phrase.QuestionInfo.Answers.First());

			phrase = phrases[5];
			Assert.AreEqual("What happened?", phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ACT 1.6-10", phrase.Reference);
			Assert.AreEqual(44001006, phrase.StartRef);
			Assert.AreEqual(44001010, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.IsNull(phrase.QuestionInfo.Notes);
			Assert.AreEqual("Stuff", phrase.QuestionInfo.Answers.First());

			phrase = phrases[6];
			Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?",
				phrase.PhraseInUse);
			Assert.AreEqual(1, phrase.Category);
			Assert.AreEqual("ACT 1.6", phrase.Reference);
			Assert.AreEqual(44001006, phrase.StartRef);
			Assert.AreEqual(44001006, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.IsNull(phrase.QuestionInfo.Notes);
			Assert.AreEqual("The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom.",
				phrase.QuestionInfo.Answers.First());
		}

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests enumerating overview and detail categories and questions with answers and
        /// comments.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void GetSectionHeads_InCanonicalOrder()
        {
            QuestionSections qs = new QuestionSections();
            qs.Items = new Section[4];
            int iS = 0;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 2.6-10", "Acts 2:6-10 Preaching.", 44002006,
                44002010, 0, 1);
            Question q = qs.Items[iS].Categories[0].Questions[0];
            q.Text = "q1";

            iS++;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 2.1-5", "Acts 2:1-5 Stuff.", 44002001,
                44002005, 0, 1);
            q = qs.Items[iS].Categories[0].Questions[0];
            q.Text = "q2";

            iS++;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.1-18", "Acts 1:1-18 Jesus Leaves.", 44001001,
                44001018, 0, 1);
            q = qs.Items[iS].Categories[0].Questions[0];
            q.Text = "q3";

            iS++;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("MAT 13.1-7", "Matthew 13:1-7 Parable.", 40013001,
                44013007, 0, 1);
            q = qs.Items[iS].Categories[0].Questions[0];
            q.Text = "q4";

            QuestionProvider qp = new QuestionProvider(qs, null);

            Assert.AreEqual(4, qp.SectionHeads.Count);
            IEnumerable<string> keys = qp.SectionReferences;
            string key = keys.ElementAt(0);
            Assert.AreEqual("MAT 13.1-7", key);
            Assert.AreEqual("Matthew 13:1-7 Parable.", qp.SectionHeads[key]);
            key = keys.ElementAt(1);
            Assert.AreEqual("ACT 1.1-18", key);
            Assert.AreEqual("Acts 1:1-18 Jesus Leaves.", qp.SectionHeads[key]);
            key = keys.ElementAt(2);
            Assert.AreEqual("ACT 2.1-5", key);
            Assert.AreEqual("Acts 2:1-5 Stuff.", qp.SectionHeads[key]);
            key = keys.ElementAt(3);
            Assert.AreEqual("ACT 2.6-10", key);
            Assert.AreEqual("Acts 2:6-10 Preaching.", qp.SectionHeads[key]);
        }

        ///--------------------------------------------------------------------------------------
        /// <summary>
        /// Tests enumerating overview and detail categories and questions with answers and
        /// comments.
        /// </summary>
        ///--------------------------------------------------------------------------------------
        [Test]
        public void EnumeratePhrases_EnsureSharedPartsAreHookedUpCorrectly()
        {
            ParsedQuestions pq = new ParsedQuestions();
            pq.KeyTerms = new [] {
                new KeyTermMatchSurrogate("luke", "lucas"),
                new KeyTermMatchSurrogate("jesus", "jesu"),
                new KeyTermMatchSurrogate("disciple", "dicipulo")
            };
            QuestionSections qs = pq.Sections = new QuestionSections();
            qs.Items = new Section[2];
            int iS = 0;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
                44001005, 2, 1);
            int iC = 0;
            Question q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "What information did Luke give?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("what information did")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[0]));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("give")));
            q = qs.Items[iS].Categories[iC].Questions[1];
            q.Text = "What is a disciple of Jesus?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("what is a")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[2]));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("of")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[1]));

            iC = 1;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "What is a book?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("what is a")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("book")));

            iS = 1;
            qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.6-10", "Acts 1:6-10 The continuing saga.", 44001006, 44001010, 1, 2);
            iC = 0;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.Text = "Which of the disciples wanted to give away land?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("which")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("of")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("the")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[2]));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("wanted to")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("give")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("away land")));

            iC = 1;
            q = qs.Items[iS].Categories[iC].Questions[0];
            q.ScriptureReference = "ACT 1.6";
            q.StartRef = 44001006;
            q.EndRef = 44001006;
            q.Text = "What information did jesus want to give the disciples?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("what information did")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[1]));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("want to")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("give")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("the")));
            q.ParsedParts.Add(new ParsedPart(pq.KeyTerms[2]));

            q = qs.Items[iS].Categories[iC].Questions[1];
            q.ScriptureReference = "ACT 1.7";
            q.StartRef = 44001007;
            q.EndRef = 44001007;
            q.Text = "Do you want to read this book again?";
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("do you")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("want to")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("read this")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("book")));
            q.ParsedParts.Add(new ParsedPart(PhraseParser.GetWordsInString("again")));

            pq.TranslatableParts = new []
            {
                "what information did",
                "give",
                "what is a",
                "of",
                "book",
                "which",
                "the",
                "wanted to",
                "away land",
                "do you",
                "want to",
                "read this",
                "again"                
            };
            QuestionProvider qp = new QuestionProvider(pq);
            List<TranslatablePhrase> phrases = qp.ToList();
            Assert.AreEqual(8, phrases.Count);

            TranslatablePhrase phrase = phrases[0];
            Assert.AreEqual("Overview", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "overview", 1);

            phrase = phrases[1];
            Assert.AreEqual("What information did Luke give?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "what information did", 2, "give", 3);

            phrase = phrases[2];
            Assert.AreEqual("What is a disciple of Jesus?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "what is a", 2, "of", 2);

            phrase = phrases[3];
            Assert.AreEqual("Details", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "details", 1);

            phrase = phrases[4];
            Assert.AreEqual("What is a book?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "what is a", 2, "book", 2);

            phrase = phrases[5];
            Assert.AreEqual("Which of the disciples wanted to give away land?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "which", 1, "of", 2, "the", 2, "wanted to", 1, "give", 3, "away land", 1);

            phrase = phrases[6];
            Assert.AreEqual("What information did jesus want to give the disciples?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "what information did", 2, "want to", 2, "give", 3, "the", 2);

            phrase = phrases[7];
            Assert.AreEqual("Do you want to read this book again?", phrase.PhraseInUse);
            VerifyTranslatablePhrase(phrase, "do you", 1, "want to", 2, "read this", 1, "book", 2, "again", 1);
        }
        
        ///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a category that doesn't have a Type specified is skipped when enumerating.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void EnumeratePhrases_UnnamedCategory()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[1];
			int iS= 0;
			Section s = new Section();
			qs.Items[iS] = s;
							
			s.ScriptureReference = "ROM 1.1-17";
			s.Heading = "Romans 1:1-17 Introduction to the book.";
			s.StartRef = 45001001;
			s.EndRef = 45001017;
			s.Categories = new Category[1];
			s.Categories[0] = new Category();
		    Question q = new Question();
            s.Categories[0].Questions.Add(q);
			q.Text = "Who wrote this book?";
			q.Answers = new[] { "Paul." };

			QuestionProvider qp = new QuestionProvider(qs, null);

			Assert.AreEqual(1, qp.SectionHeads.Count);
			Assert.AreEqual("Romans 1:1-17 Introduction to the book.", qp.SectionHeads["ROM 1.1-17"]);
			Assert.AreEqual(1, qp.AvailableBookIds.Length);
			Assert.AreEqual(45, qp.AvailableBookIds[0]);
			List<TranslatablePhrase> phrases = qp.ToList();
			Assert.AreEqual(1, phrases.Count);

			TranslatablePhrase phrase = phrases[0];
			Assert.AreEqual("Who wrote this book?", phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ROM 1.1-17", phrase.Reference);
			Assert.AreEqual(45001001, phrase.StartRef);
			Assert.AreEqual(45001017, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.IsNull(phrase.QuestionInfo.Notes);
			Assert.AreEqual("Paul.", phrase.QuestionInfo.Answers.First());
		}


		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that two categories whose types differ only by case are not enumerated
		/// distinctly.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void EnumeratePhrases_CategoriesDifferOnlyByCase()
		{
			QuestionSections qs = new QuestionSections();
			qs.Items = new Section[2];
			int iS = 0;
			qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.1-5", "Acts 1:1-5 Introduction to the book.", 44001001,
				44001005, 1, 1);
			int iC = 0;
			Question q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What information did Luke, the writer of this book, give in this introduction?";
			q.Answers = new[] { "Luke reminded his readers that he was about to continue the true story about Jesus" };

			iC = 1;
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "To whom did the writer of Acts address this book?";
			q.Answers = new[] { "He addressed this book to Theophilus." };

			iS = 1;
			qs.Items[iS] = MasterQuestionParserTests.CreateSection("ACT 1.6-10", "Acts 1:6-10 The continuing saga.", 44001006, 44001010, 1, 1);
			iC = 0;
			qs.Items[iS].Categories[iC].Type = "overview";
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.Text = "What happened?";
			q.Answers = new[] { "Stuff" };

			iC = 1;
			qs.Items[iS].Categories[iC].Type = "details";
			q = qs.Items[iS].Categories[iC].Questions[0];
			q.ScriptureReference = "ACT 1.6";
			q.StartRef = 44001006;
			q.EndRef = 44001006;
			q.Text = "What question did the apostles ask Jesus about his kingdom?";
			q.Answers = new[] { "The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom." };

			QuestionProvider qp = new QuestionProvider(qs, null);

			List<TranslatablePhrase> phrases = qp.ToList();
			Assert.AreEqual(6, phrases.Count);
			Assert.AreEqual(1, qp.AvailableBookIds.Length);
			Assert.AreEqual(44, qp.AvailableBookIds[0]);
			Assert.AreEqual(2, qp.SectionHeads.Count);
			Assert.AreEqual("Acts 1:1-5 Introduction to the book.", qp.SectionHeads["ACT 1.1-5"]);
			Assert.AreEqual("Acts 1:6-10 The continuing saga.", qp.SectionHeads["ACT 1.6-10"]);

			TranslatablePhrase phrase = phrases[0];
			Assert.AreEqual("Overview", phrase.PhraseInUse);
			Assert.AreEqual(-1, phrase.Category);
			Assert.AreEqual(string.Empty, phrase.Reference);
			Assert.AreEqual(001001001, phrase.StartRef);
			Assert.AreEqual(066022021, phrase.EndRef);
			Assert.AreEqual(0, phrase.SequenceNumber);
			Assert.IsNull(phrase.QuestionInfo);

			phrase = phrases[1];
			Assert.AreEqual("What information did Luke, the writer of this book, give in this introduction?",
				phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ACT 1.1-5", phrase.Reference);
			Assert.AreEqual(44001001, phrase.StartRef);
			Assert.AreEqual(44001005, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.AreEqual("Luke reminded his readers that he was about to continue the true story about Jesus",
				phrase.QuestionInfo.Answers.First());

			phrase = phrases[2];
			Assert.AreEqual("Details", phrase.PhraseInUse);
			Assert.AreEqual(-1, phrase.Category);
			Assert.AreEqual(string.Empty, phrase.Reference);
			Assert.AreEqual(001001001, phrase.StartRef);
			Assert.AreEqual(066022021, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNull(phrase.QuestionInfo);

			phrase = phrases[3];
			Assert.AreEqual("To whom did the writer of Acts address this book?",
				phrase.PhraseInUse);
			Assert.AreEqual(1, phrase.Category);
			Assert.AreEqual("ACT 1.1-5", phrase.Reference);
			Assert.AreEqual(44001001, phrase.StartRef);
			Assert.AreEqual(44001005, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.AreEqual("He addressed this book to Theophilus.",
				phrase.QuestionInfo.Answers.First());

			phrase = phrases[4];
			Assert.AreEqual("What happened?", phrase.PhraseInUse);
			Assert.AreEqual(0, phrase.Category);
			Assert.AreEqual("ACT 1.6-10", phrase.Reference);
			Assert.AreEqual(44001006, phrase.StartRef);
			Assert.AreEqual(44001010, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.AreEqual("Stuff", phrase.QuestionInfo.Answers.First());

			phrase = phrases[5];
			Assert.AreEqual("What question did the apostles ask Jesus about his kingdom?",
				phrase.PhraseInUse);
			Assert.AreEqual(1, phrase.Category);
			Assert.AreEqual("ACT 1.6", phrase.Reference);
			Assert.AreEqual(44001006, phrase.StartRef);
			Assert.AreEqual(44001006, phrase.EndRef);
			Assert.AreEqual(1, phrase.SequenceNumber);
			Assert.IsNotNull(phrase.QuestionInfo);
			Assert.AreEqual(1, phrase.QuestionInfo.Answers.Count());
			Assert.AreEqual("The apostles asked Jesus whether he was soon going to set up his kingdom in a way that everybody could see and cause the people of Israel to have power in that kingdom.",
				phrase.QuestionInfo.Answers.First());
        }

        #region Private helper methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the translatable phrase.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="parts">Parts information, with alternating sub-phrases and their
        /// occurrence counts (across all phrases in the test).</param>
        /// ------------------------------------------------------------------------------------
        private static void VerifyTranslatablePhrase(TranslatablePhrase phrase,
            params object[] parts)
        {
            Assert.AreEqual(parts.Length / 2, phrase.TranslatableParts.Count(), "Phrase \"" + phrase +
                "\" was split into the wrong number of parts.");
            int i = 0;
            foreach (Part part in phrase.TranslatableParts)
            {
                Assert.AreEqual(parts[i++], part.Text, "Unexpected part");
                Assert.AreEqual(GetWordCount(part.Text), part.Words.Count(),
                                "Unexpected word count for part \"" + part.Text + "\"");
                Assert.AreEqual(parts[i++], part.OwningPhrases.Count(),
                                "Unexpected number of owning phrases for part \"" + part.Text + "\"");
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the word count of the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// ------------------------------------------------------------------------------------
        private static int GetWordCount(IEnumerable<char> text)
        {
            return 1 + text.Count(c => c == ' ');
        }
        #endregion
    }
}
