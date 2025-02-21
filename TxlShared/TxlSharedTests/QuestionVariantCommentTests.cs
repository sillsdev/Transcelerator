using System;
using NUnit.Framework;
using SIL.Transcelerator;

namespace TxlSharedTests
{
	/// <summary>
	/// Note that most of the functionality of this class is actually tested in
	/// <see cref="QuestionVariantsHelperTests"/>.
	/// </summary>
	[TestFixture]
	public class QuestionVariantCommentTests
	{
		[Test]
		public void Constructor_BasedOnEitherOrMatchOnly_ReturnsObjectRepresentingMatchInfo()
		{
			var matches = QuestionVariantComment.RegexUseOneVariantNote.Matches(
				"<Note>This is the comment with index 0</Note>" + Environment.NewLine +
				"<Note>Use either this question (D) or the preceding question (C). It would be redundant to ask both questions.</Note>" + Environment.NewLine +
				"<Note>question D (Lev 5:6-7)</Note>");

			Assert.That(matches.Count, Is.EqualTo(1));

			var comment = new QuestionVariantComment(matches[0]);
			Assert.That(comment.ScriptureReference, Is.Null);
			Assert.That(comment.Chapter, Is.Null);
			Assert.That(comment.Verses, Is.Null);
			Assert.IsFalse(comment.IsForVariantSeries);
			Assert.That(comment.NumberOfQuestionsInBothVariants, Is.EqualTo(2));
			Assert.That(comment.QuestionWithCommentAboutRedundancy, Is.Null);
			Assert.That(comment.Index, Is.EqualTo(-1));
			Assert.That(comment.FirstVariantLetter, Is.EqualTo('C'));
			Assert.That(comment.SecondVariantLetter, Is.EqualTo('D'));
			Assert.That(comment.ThisVariantLetter, Is.EqualTo('D'));
			Assert.That(comment.OtherVariantLetter, Is.EqualTo('C'));
		}

		[Test]
		public void Constructor_BasedOnGroupMatchOnly_ReturnsObjectRepresentingMatchInfo()
		{
			var matches = QuestionVariantComment.RegexUseOneVariantNote.Matches(
				"<Note>For Jonah 1:17, use either the group A questions or the group B questions. It would be redundant to ask all 3 questions.</Note>" + Environment.NewLine +
				"<Note>question A (Jon 1:17)</Note>");

			Assert.That(matches.Count, Is.EqualTo(1));

			var comment = new QuestionVariantComment(matches[0]);
			Assert.That(comment.ScriptureReference, Is.EqualTo("Jonah 1.17"));
			Assert.That(comment.Chapter, Is.EqualTo("1"));
			Assert.That(comment.Verses, Is.EqualTo("17"));
			Assert.IsTrue(comment.IsForVariantSeries);
			Assert.That(comment.NumberOfQuestionsInBothVariants, Is.EqualTo(3));
			Assert.That(comment.QuestionWithCommentAboutRedundancy, Is.Null);
			Assert.That(comment.Index, Is.EqualTo(-1));
			Assert.That(comment.FirstVariantLetter, Is.EqualTo('A'));
			Assert.That(comment.SecondVariantLetter, Is.EqualTo('B'));
		}
	}
}
