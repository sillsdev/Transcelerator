using System.Linq;
using NUnit.Framework;
using SIL.Transcelerator;

namespace TxlSharedTests
{
	[TestFixture]
	public class QuestionVariantsHelperTests
	{
		[Test]
		public void GetCommentAboutAvoidingRedundantQuestions_UserAddedQuestion_ReturnsNull()
		{
			var q = new Question("EXO 1.1", 2001001, 2001001, "Why no comments?", "Because this is a user question.");
			Assert.That(new [] {q}.GetCommentAboutAvoidingRedundantQuestions(), Is.Null);
		}

		[Test]
		public void GetCommentAboutAvoidingRedundantQuestions_QuestionsWithNoVariantComments_ReturnsNull()
		{
			var questions = new [] {
					new Question
					{
						Text = "What does the fox say?",
						Notes = new[] 
						{
							"Do not ask this question.",
							"It is a test question."
						}
					},
					new Question
					{
						Text = "Tell me more.",
						Notes = new[] 
						{
							"This is a command, not a question.",
							"This is not part of a group, so it is not redundant."
						}
					}
				};
			Assert.That(questions.GetCommentAboutAvoidingRedundantQuestions(), Is.Null);
		}

		[Test]
		public void GetCommentAboutAvoidingRedundantQuestions_QuestionsWithEitherOrComments_ReturnsObjectRepresentingCorrectQuestion()
		{
			var q = new Question
			{
				Text = "What does the fox say?",
				ScriptureReference = "LEV 5.6-7",
				StartRef = 3005006,
				EndRef = 3005007,
				Group = "6-7D",
				Notes = new[]
				{
					"This is the comment with index 0",
					"Use either this question (D) or the preceding question (C). It would be redundant to ask both questions.",
					"question D (Lev 5:6-7)"
				}
			};
			var comment = new[] { q }.GetCommentAboutAvoidingRedundantQuestions();
			Assert.That(comment.ScriptureReference, Is.EqualTo("LEV 5.6-7"));
			Assert.That(comment.Chapter, Is.EqualTo("5"));
			Assert.That(comment.Verses, Is.EqualTo("6-7"));
			Assert.IsFalse(comment.IsForVariantSeries);
			Assert.That(comment.NumberOfQuestionsInBothVariants, Is.EqualTo(2));
			Assert.That(comment.QuestionWithCommentAboutRedundancy, Is.EqualTo(q));
			Assert.That(comment.Index, Is.EqualTo(1));
			Assert.That(comment.FirstVariantLetter, Is.EqualTo('C'));
			Assert.That(comment.SecondVariantLetter, Is.EqualTo('D'));
			Assert.That(comment.ThisVariantLetter, Is.EqualTo('D'));
			Assert.That(comment.OtherVariantLetter, Is.EqualTo('C'));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void GetCommentAboutAvoidingRedundantQuestions_QuestionsWithVariantComments_ReturnsObjectRepresentingCorrectQuestion(bool reverse)
		{
			var questions = new [] {
				new Question
				{
					Text = "Who sent a great fish to swallow up Jonah?",
					ScriptureReference = "JON 1.17",
					StartRef = 32001017,
					EndRef = 32001017,
					Group = "17A",
					Notes = new[] 
					{
						"For Jonah 1:17, use either the group A questions or the group B questions. It would be redundant to ask all 129 questions.",
						"group A (Jon 1:17)"
					}
				},
				new Question
				{
					Text = "Tell me more.",
					Notes = new[] 
					{
						"group A (Jon 1:17)"
					}
				}
			};
			if (reverse)
				questions = questions.Reverse().ToArray();
			var comment = questions.GetCommentAboutAvoidingRedundantQuestions();
			Assert.That(comment.ScriptureReference, Is.EqualTo("JON 1.17"));
			Assert.That(comment.Chapter, Is.EqualTo("1"));
			Assert.That(comment.Verses, Is.EqualTo("17"));
			Assert.IsTrue(comment.IsForVariantSeries);
			Assert.That(comment.NumberOfQuestionsInBothVariants, Is.EqualTo(129));
			Assert.That(comment.QuestionWithCommentAboutRedundancy.Text, Is.EqualTo("Who sent a great fish to swallow up Jonah?"));
			Assert.That(comment.Index, Is.EqualTo(0));
			Assert.That(comment.FirstVariantLetter, Is.EqualTo('A'));
			Assert.That(comment.SecondVariantLetter, Is.EqualTo('B'));
			Assert.That(comment.ThisVariantLetter, Is.EqualTo('A'));
			Assert.That(comment.OtherVariantLetter, Is.EqualTo('B'));
		}

		[Test]
		public void OtherVariantId_Null_ThrowsArgumentNullException()
		{
			Assert.That(() => {((string)null).OtherVariantId(); }, Throws.ArgumentNullException);
		}

		[TestCase("")]
		[TestCase("5-6")]
		[TestCase("A")]
		[TestCase("2z")]
		public void OtherVariantId_InvalidVariantId_ThrowsArgumentException(string invalidId)
		{
			Assert.That(() => { invalidId.OtherVariantId(); }, Throws.ArgumentException);
		}

		[TestCase("5-6A", ExpectedResult = "5-6B")]
		[TestCase("2D", ExpectedResult = "2C")]
		[TestCase("43Z", ExpectedResult = "43Y")]
		public string OtherVariantId_ValidVariantId_ReturnsIdOfOtherVariant(string id)
		{
			return id.OtherVariantId();
		}
	}
}
