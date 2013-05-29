// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2012, SIL International. All Rights Reserved.
// <copyright from='2011' to='2012' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: QuestionProviderTests.cs
// ---------------------------------------------------------------------------------------------
using System;
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
	}
}
