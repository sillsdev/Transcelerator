using System.Linq;
using NUnit.Framework;
using SIL.Transcelerator;

namespace TxlSharedTests
{
	[TestFixture]
	public class QuestionGroupHelperTests
	{
		[Test]
		public void GetCommentAboutAvoidingRedundantQuestions_UserAddedQuestion_ReturnsNull()
		{
			var q = new Question("EXO 1.1", 2001001, 2001001, "Why no comments?", "Because this is a user question.");
			Assert.That(new [] {q}.GetCommentAboutAvoidingRedundantQuestions(), Is.Null);
		}

		[Test]
		public void GetCommentAboutAvoidingRedundantQuestions_QuestionsWithNoGroupComments_ReturnsNull()
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
			Assert.IsFalse(comment.IsForGroup);
			Assert.That(comment.NumberOfQuestionsInBothGroups, Is.EqualTo(2));
			Assert.That(comment.QuestionWithCommentAboutRedundancy, Is.EqualTo(q));
			Assert.That(comment.Index, Is.EqualTo(1));
			Assert.That(comment.FirstGroupLetter, Is.EqualTo('C'));
			Assert.That(comment.SecondGroupLetter, Is.EqualTo('D'));
			Assert.That(comment.ThisGroupLetter, Is.EqualTo('D'));
			Assert.That(comment.OtherGroupLetter, Is.EqualTo('C'));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void GetCommentAboutAvoidingRedundantQuestions_QuestionsWithGroupComments_ReturnsObjectRepresentingCorrectQuestion(bool reverse)
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
			Assert.IsTrue(comment.IsForGroup);
			Assert.That(comment.NumberOfQuestionsInBothGroups, Is.EqualTo(129));
			Assert.That(comment.QuestionWithCommentAboutRedundancy.Text, Is.EqualTo("Who sent a great fish to swallow up Jonah?"));
			Assert.That(comment.Index, Is.EqualTo(0));
			Assert.That(comment.FirstGroupLetter, Is.EqualTo('A'));
			Assert.That(comment.SecondGroupLetter, Is.EqualTo('B'));
			Assert.That(comment.ThisGroupLetter, Is.EqualTo('A'));
			Assert.That(comment.OtherGroupLetter, Is.EqualTo('B'));

		}

		[Test]
		public void AlternativeGroup_Null_ThrowsArgumentNullException()
		{
			Assert.That(() => {((string)null).AlternativeGroup(); }, Throws.ArgumentNullException);
		}

		[TestCase("")]
		[TestCase("5-6")]
		[TestCase("A")]
		[TestCase("2z")]
		public void AlternativeGroup_InvalidGroup_ThrowsArgumentException(string group)
		{
			Assert.That(() => { group.AlternativeGroup(); }, Throws.ArgumentException);
		}

		[TestCase("5-6A", ExpectedResult = "5-6B")]
		[TestCase("2D", ExpectedResult = "2C")]
		[TestCase("43Z", ExpectedResult = "43Y")]
		public string AlternativeGroup_ValidGroup_ThrowsArgumentException(string group)
		{
			return group.AlternativeGroup();
		}
	}
}
