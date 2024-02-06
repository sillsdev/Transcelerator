using System;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class QuestionTests
	{
		[TestCase("")]
		[TestCase("   ")]
		[TestCase(null)]
		public void Text_SetNullOrEmptyForNonUserAddedQuestion_QuestionMarkedAsUserAdded(string question)
		{
			var q = new Question {Text = question};
			Assert.IsTrue(q.IsUserAdded);
		}

		[Test]
		public void Constructor_NullTextAndKey_IdSetToGuidWithPrefix()
		{
			var q = new Question("TST 2:3", 500002003, 500002003, null, "Not provided", null);
			Assert.IsTrue(q.Id.StartsWith(Question.kGuidPrefix));
			Assert.IsTrue(Guid.TryParse(q.Id.Substring(Question.kGuidPrefix.Length), out _));
		}

		[Test]
		public void Constructor_NullTextWithKeySpecified_IdAndTextSetToSpecifiedKey()
		{
			var explicitKey = Question.kGuidPrefix + Guid.NewGuid();
			var q = new Question("TST 2:3", 500002003, 500002003, null, null, explicitKey);
			Assert.AreEqual(explicitKey, q.Id);
			Assert.AreEqual(explicitKey, q.Text);
		}

		[Test]
		public void Constructor_TextAndKeySpecified_IdAndTextSetToSpecifiedKey()
		{
			var explicitKey = Question.kGuidPrefix + Guid.NewGuid();
			var q = new Question("TST 2:3", 500002003, 500002003, "English version of question?", null, explicitKey);
			Assert.AreEqual(explicitKey, q.Id);
			Assert.AreEqual("English version of question?", q.Text);
		}
	}
}