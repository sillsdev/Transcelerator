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
		public void Question_SetNullOrEmptyTextOfNonUserAddedQuestion_MarkedAsUserAdded(string question)
		{
			var q = new Question {Text = question};
			Assert.IsTrue(q.IsUserAdded);
		}
	}
}