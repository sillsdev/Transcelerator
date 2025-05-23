// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2023' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Linq;
using System.Web.UI.WebControls;
using NUnit.Framework;
using static System.Int32;
using static SIL.Transcelerator.SubstitutionMatchGroup;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class SubstitutionMatchGroupTests
	{
		[TestCase("", 0, ExpectedResult = 1)]
		[TestCase("abc", 0, ExpectedResult = 1)]
		[TestCase("abc", 3, ExpectedResult = 1)]
		[TestCase("abc{1,2}", 3, ExpectedResult = 2)]
		[TestCase("abc{1,2}", 4, ExpectedResult = 2)]
		[TestCase("abc{1,3}", 5, ExpectedResult = 3)]
		[TestCase("abc{1,2}", 6, ExpectedResult = 2)]
		[TestCase("abc{1,4}", 7, ExpectedResult = 4)]
		[TestCase("(abc)", 0, ExpectedResult = 1)]
		[TestCase("(abc){1,2}", 5, ExpectedResult = 2)]
		[TestCase("(abc){1,2}", 6, ExpectedResult = 2)]
		[TestCase("(abc){1,4}", 0, ExpectedResult = 4)]
		[TestCase("(abc){1,3}", 1, ExpectedResult = 3)]
		[TestCase("abc{1,2}def{1,3}", 16, ExpectedResult = 3)]
		[TestCase("abc{1,2}def", 11, ExpectedResult = 1)]
		[TestCase("abc{1,2}def{1,3}", 9, ExpectedResult = 1)]
		[TestCase("abc{1,2}def{1,3}", 4, ExpectedResult = 2)]
		[TestCase("abc{1,2}def{1,3}", 3, ExpectedResult = 2)]
		[TestCase("abc{1,2}def{1,3}", 0, ExpectedResult = 1)]
		[TestCase("abc{2,100}", 10, ExpectedResult = 100)]
		[TestCase("abc{2}", 3, ExpectedResult = 2)]
		[TestCase("abc{2}", 4, ExpectedResult = 2)]
		[TestCase("abc{2}", 5, ExpectedResult = 2)]
		public int GetRangeMax_GetsExpectedCount(string text, int pos)
		{
			return GetRangeMax(text, pos);
		}

		#region UpdateRangeMax tests
		[TestCase(2, "", 0, 0, "", "Empty text => no-op", ExpectedResult = "")]
		[TestCase(2, "abc", 0, 0, "", "IP precedes all text => no-op", ExpectedResult = "abc")]
		[TestCase(2, "abc", 3, 0, "{1,2}", "When IP follows all text a new regex count should be " +
			"inserted for: 1-2 occurrences of preceding character.", ExpectedResult = "abc{1,2}")]
		[TestCase(2, "abc", 0, 3, "{1,2}", "When all text is selected, a new regex count should " +
			"be added for 1-2 occurrences of a new group containing all characters.",
			ExpectedResult = "(abc){1,2}")]
		[TestCase(1, "abc{1,2}", 3, 5, "", "When selected text covers part of the count " +
			"expression for a single preceding character, the expression should be removed when " +
			"going from 2 to 1.", ExpectedResult = "abc")]
		[TestCase(1, "abc{1,2}", 3, 8, "", "When selected text covers all of the count " +
			"expression for a single preceding character, the expression should be removed when " +
			"going from 2 to 1.", ExpectedResult = "abc")]
		[TestCase(3, "abc{1,2}", 4, 6, "{1,3}", "When selected text covers part of the count " +
			"expression for a single preceding character, the range expression max should be " +
			"incremented.", ExpectedResult = "abc{1,3}")]
		[TestCase(2, "(abc)", 0, 5, "{1,2}", "When selected text covers entire group" +
			"expression with no existing range expression, a new expression should be added" +
			"for 1-2 occurrences of a the group.", ExpectedResult = "(abc){1,2}")]
		[TestCase(3, "(abc){1,2}", 5, 5, "{1,3}", "When selected text covers entire count" +
			"expression for a group, the range expression max should be incremented.",
			ExpectedResult = "(abc){1,3}")]
		[TestCase(3, "(abc){1,2}", 5, 5, "{1,3}", "When selected text covers numbers in count" +
			"expression for a group, the range expression max should be incremented.",
			ExpectedResult = "(abc){1,3}")]
		[TestCase(3, "(abc){1,2}", 0, 10, "{1,3}", "When all text is selected, the max of the " +
			"existing range expression for the group should be incremented.",
			ExpectedResult = "(abc){1,3}")]
		[TestCase(3, "(abc){1,2}", 5, 5, "{1,3}", "When an existing group is selected, the " +
			"existing range expression max should be incremented.",
			ExpectedResult = "(abc){1,3}")]
		[TestCase(3, "(abc){1,2}", 1, 3, "{1,3}", "When the text of existing group is selected, " +
			"the existing range expression max should be incremented.",
			ExpectedResult = "(abc){1,3}")]
		[TestCase(6, "abc{1,2}def{1,3}", 16, 0, "{1,6}", "When IP is at the end of the text " +
			"ending with a range expression, that range expression max should be incremented.",
			ExpectedResult = "abc{1,2}def{1,6}")]
		[TestCase(99, "abc{2,100}", 10, 0, "{2,99}", "When IP is at the end of the text " +
			"ending with a range expression that does not start at 1, that range expression max " +
			"should be incremented.", ExpectedResult = "abc{2,99}")]
		[TestCase(3, "abc{2}", 3, 3, "{1,3}", "When a quantifier expression is selected that " +
			"specifies an exact number of matches greater than 1, that expression is replaced " +
			"by a range expression from 1-max.", ExpectedResult = "abc{1,3}")]
		public string UpdateRangeMax_GetsExpectedResult(int newValue, string text, int pos,
			int selLength, string expectedSelectedPortion, string explanation)
		{
			var result = UpdateRangeMax(newValue, text, ref pos, selLength, out var length);
			Assert.That(result.Substring(pos, length), Is.EqualTo(expectedSelectedPortion), explanation);
			return result;
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// This is similar to the above test, except it allows us to ensure the correct out/ref
		/// parameters when the expected selection occurs more than once in the result.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[TestCase(2, "abc{1,2}def", 11, 0, 11, 5, "When IP is at the end of the text not " +
			"ending with a range expression, a new range expression should be appended.",
			ExpectedResult = "abc{1,2}def{1,2}")]
		[TestCase(2, "abc{1,2}def{1,3}", 9, 0, 9, 5, "When IP is text not grouped so as to be" +
			"covered by either preceding or following range expressions, a new range expression " +
			"should be inserted to cover the preceding character.",
			ExpectedResult = "abc{1,2}d{1,2}ef{1,3}")]
		[TestCase(3, "abc{1,2}def{1,3}", 4, 0, 3, 5, "When IP is before the number in the first" +
			"of two existing range expressions, the max of the first range expression " +
			"should be updated (incremented).",
			ExpectedResult = "abc{1,3}def{1,3}")]
		[TestCase(3, "abc{1,4}def{1,3}", 4, 0, 3, 5, "When IP is before the number in the first" +
			"of two existing range expressions, the max of the first range expression " +
			"should be updated (decremented).",
			ExpectedResult = "abc{1,3}def{1,3}")]
		[TestCase(3, "abc{1,2}def{1,3}", 3, 1, 3, 5, "When a single character is selected and " +
			"the first of two existing range expressions applies it, the max of that count " +
			"expression should be updated.",
			ExpectedResult = "abc{1,3}def{1,3}")]
		[TestCase(3, "abc{1,2}def{1,3}", 2, 6, 3, 5, "When a character and its corresponding " +
			"range expression is selected, the max of that range expression should be updated.",
			ExpectedResult = "abc{1,3}def{1,3}")]
		[TestCase(3, "abc{1,2}def{1,3}", 3, 5, 3, 5, "When the range expression for a single " +
			"character is selected, the max of that range expression should be updated.",
			ExpectedResult = "abc{1,3}def{1,3}")]
		[TestCase(1, "win{1,2}now{1,3}", 2, 6, 3, 0, "When a character and its corresponding " +
			"range expression is selected and the count goes down to 1 that range expression " +
			"should be removed.",
			ExpectedResult = "winnow{1,3}")]
		[TestCase(1, "win{1,2}now{1,3}", 3, 5, 3, 0, "When a character and its corresponding " +
			"range expression is selected and the count goes down to 1 that range expression " +
			"should be removed.",
			ExpectedResult = "winnow{1,3}")]
		[TestCase(2, "abc{1,2}def{1,3}", 0, 16, 18, 5, "When the entire text is selected and " +
			"it contains multiple range expressions, a group should be created for entire text " +
			"and a new range expression should be appended to apply to the new group.",
			ExpectedResult = "(abc{1,2}def{1,3}){1,2}")]
		public string UpdateRangeMax_GetsExpectedResultAndSelection(int newValue, string text, int pos,
			int selLength, int expectedStart, int expectedLength, string explanation)
		{
			var result = UpdateRangeMax(newValue, text, ref pos, selLength, out var length);
			Assert.That(pos, Is.EqualTo(expectedStart), explanation);
			Assert.That(length, Is.EqualTo(expectedLength), explanation);
			return result;
		}
		#endregion

		#region MatchCount tests
		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the text box
		/// is empty - should do nothing.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EmptyTextBox_UpdateDoesNothing()
		{
			var t = new TextBoxProxy();
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));

			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.Empty);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the selection
		/// precedes all the text in the text box - should do nothing.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_IpPrecedingText_UpdateDoesNothing()
		{
			var t = new TextBoxProxy
			{
				Text = "abc",
				SelectionStart = 0,
				SelectionLength = 0
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));

			UpdateMatchCount(2, t);

			Assert.That(t.Text, Is.EqualTo("abc"));
			Assert.That(t.SelectionStart, Is.EqualTo(0));
			Assert.That(t.SelectionLength, Is.EqualTo(0));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the selection
		/// follows all the text in the text box, and there is no explicit number of occurrences
		/// specified.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_NoExistingMatchCount_IpAtEnd()
		{
			var t = new TextBoxProxy
			{
				Text = "abc",
				SelectionStart = 3,
				SelectionLength = 0
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,2}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the all the
		/// text in the text box is selected, and there is no explicit number of occurrences
		/// specified.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_AllTextSelected_NoExistingMatchCount()
		{
			var t = new TextBoxProxy
			{
				Text = "abc",
				SelectionStart = 0,
				SelectionLength = 3
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,2}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the final character in the text, and
		/// the entire expression indicating the number of matches is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_FinalExistingMatchCountExpressionSelected_NoGroup_DecrementTo1()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}",
				SelectionStart = 3,
				SelectionLength = 5
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(1, t);
			
			Assert.That(t.Text, Is.EqualTo("abc"));
			Assert.AreEqual(3, t.SelectionStart);
			Assert.AreEqual(0, t.SelectionLength);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the final character in the text, and
		/// the entire expression indicating the number of matches is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_FinalExistingMatchCountExpressionSelected_NoGroup_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}",
				SelectionStart = 3,
				SelectionLength = 5
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the entire
		/// text is grouped but there is no explicit number of occurrences specified for it, and
		/// the entire text is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EntireTextGroupSelected_AddMatchCount()
		{
			var t = new TextBoxProxy { Text = "(abc)" };
			t.SelectionLength = t.Text.Length;

			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,2}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the entire range of characters in the
		/// text, and the entire expression indicating the number of matches is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_FinalExistingMatchCountExpressionSelected_Group_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "(abc){1,2}",
				SelectionStart = 5,
				SelectionLength = 5
			};

			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the entire range of characters in the
		/// text, and just the numerals indicating the number of matches is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_FinalExistingMatchCountNumberSelected_Group_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "(abc){1,2}",
				SelectionStart = 6,
				SelectionLength = 3
			};

			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the entire range of characters in the
		/// text, and the entire text (including the match expression) is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EntireTextSelected_FinalExistingMatchCount_Group_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "(abc){1,2}",
			};
			t.SelectionLength = t.Text.Length;
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the entire range of characters in the
		/// text, and the text (but not the match expression) is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_TextGroupSelected_FinalExistingMatchCount_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "(abc){1,2}",
				SelectionLength = 5
			};

			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit number of occurrences specified for the entire range of characters in the
		/// text, and the text of the group (but not the parentheses that enclose the group) is
		/// selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_TextInGroupSelected_FinalExistingMatchCount_IncrementTo3()
		{
			var t = new TextBoxProxy
			{
				Text = "(abc){1,2}",
				SelectionStart = 1,
				SelectionLength = 3
			};

			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc){1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// insertion point is at the end of the string, following the second one.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EarlierExistingMatchCount_IpAtEnd_ChangeExistingMatchCount()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}def{1,3}",
				SelectionStart = 1,
			};
			t.SelectionStart = t.Text.Length;
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(3));
			
			UpdateMatchCount(6, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,2}def{1,6}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,6}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// earlier place in the text where an explicit number of occurrences is specified, and
		/// the insertion point is at the end of the string.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EarlierExistingMatchCount_IpAtEnd_AddMatchCount()
		{
			var t = new TextBoxProxy { Text = "abc{1,2}def" };
			t.SelectionStart = t.Text.Length;
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,2}def{1,2}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// insertion point is in the text between them.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_EarlierAndLaterExistingMatchCounts_IpInMiddle_AddMatchCount()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}def{1,3}",
				SelectionStart = 9,
				SelectionLength = 0
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,2}d{1,2}ef{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
			Assert.AreEqual(9, t.SelectionStart);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// insertion point is before the number in first one.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_LaterExistingMatchCount_IpBeforeNumberInExistingMatchCount()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}def{1,3}",
				SelectionStart = 4,
				SelectionLength = 0
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,3}def{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
			Assert.AreEqual(3, t.SelectionStart);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// character to which the first one applies is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_LaterExistingMatchCount_CharacterOfExistingMatchCountSelected()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}def{1,3}",
				SelectionStart = 3,
				SelectionLength = 1
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,3}def{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
			Assert.AreEqual(3, t.SelectionStart);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// character and match count expression of the first one is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_LaterExistingMatchCount_CharacterAndExistingMatchCountSelected()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{1,2}def{1,3}",
				SelectionStart = 3,
				SelectionLength = 6
			};
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			
			UpdateMatchCount(3, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{1,3}def{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
			Assert.AreEqual(3, t.SelectionStart);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there are two
		/// places where an explicit number of occurrences are specified in the text, and the
		/// entire text is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_TwoExistingMatchCounts_EntireTextSelected()
		{
			var t = new TextBoxProxy { Text = "abc{1,2}def{1,3}" };
			t.SelectionLength = t.Text.Length;
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(1));
			
			UpdateMatchCount(2, t);
			
			Assert.That(t.Text, Is.EqualTo("(abc{1,2}def{1,3}){1,2}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,2}"));
			Assert.AreEqual(t.Text.Length - t.SelectionLength, t.SelectionStart);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when the selection
		/// follows all the text in the text box, and there is an explicit range specified that
		/// does not start at 1.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_ExistingMatchCountStartsAt2_IpAtEnd_DecrementTo99()
		{
			var t = new TextBoxProxy { Text = "abc{2,100}" };
			t.SelectionStart = t.Text.Length;
			
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(100));
			
			UpdateMatchCount(99, t);
			
			Assert.That(t.Text, Is.EqualTo("abc{2,99}"));
			Assert.That(t.SelectedText, Is.EqualTo("{2,99}"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the GetExistingMatchCountValue and UpdateMatchCount methods when there is an
		/// explicit absolute match count specified and we increment it -- should convert it to
		/// a range.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void MatchCount_ConvertExistingAbsoluteMatchCountToRange()
		{
			var t = new TextBoxProxy
			{
				Text = "abc{2}",
				SelectionStart = 3,
				SelectionLength = 3
			};
			Assert.That(GetExistingMatchCountValue(t), Is.EqualTo(2));
			UpdateMatchCount(3, t);
			Assert.That(t.Text, Is.EqualTo("abc{1,3}"));
			Assert.That(t.SelectedText, Is.EqualTo("{1,3}"));
		}
		#endregion

		#region Prefix and Suffix tests
		[TestCase("", 0)]
		[TestCase("Blah", 0)]
		[TestCase("Blah", 1)]
		[TestCase("Blah", 4)]
		[TestCase(@"Bl\bah", 0)]
		[TestCase(@"Bl\bah", 1)]
		[TestCase(@"Bl\b", 0)]
		[TestCase(@"Bl\b", 1)]
		[TestCase(@"Bl\b", 2)]
		[TestCase(@"Bl\b", 3)]
		public void FindAffixExpressionAt_NoPrefixAtPos_ReturnsUnsuccessfulMatch(string text, int pos)
		{
			Assert.That(FindAffixExpressionAt(AffixType.Prefix, text, pos).Success, Is.False);
		}

		[TestCase("", 0)]
		[TestCase("Blah", 0)]
		[TestCase("Blah", 1)]
		[TestCase("Blah", 4)]
		[TestCase(@"Bl\bah", 5)]
		[TestCase(@"Bl\bah", 6)]
		[TestCase(@"\bpre", 0)]
		[TestCase(@"\bpre", 1)]
		[TestCase(@"\bpre", 2)]
		[TestCase(@"\bpre", 3)]
		public void FindAffixExpressionAt_NoSuffixAtPos_ReturnsUnsuccessfulMatch(string text, int pos)
		{
			Assert.That(FindAffixExpressionAt(AffixType.Suffix, text, pos).Success, Is.False);
		}

		[TestCase(@"\bpre fix", 0, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre fix", 1, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre fix", 3, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"Bl\bah", 2, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 3, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 4, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 6, 2, ExpectedResult = @"\bah")]
		[TestCase(@"\bpre\w+ful\b", 0, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre\w+ful\b", 1, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre\w+ful\b", 2, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre\w+ful\b", 4, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre\w+ful\b", 5, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpreful\b", 0, 0, ExpectedResult = @"\bpreful")]
		[TestCase(@"\bpreful\b", 1, 0, ExpectedResult = @"\bpreful")]
		[TestCase(@"\bpreful\b", 2, 0, ExpectedResult = @"\bpreful")]
		[TestCase(@"\bpreful\b", 3, 0, ExpectedResult = @"\bp")]
		[TestCase(@"\bpreful\b", 4, 0, ExpectedResult = @"\bpr")]
		[TestCase(@"\bpreful\b", 5, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpreful\b", 8, 0, ExpectedResult = @"\bpreful")]
		[TestCase(@"er\b\s+\bpreful\b", 7, 7, ExpectedResult = @"\bpreful")]
		[TestCase(@"er\b\s+\bpreful\b", 8, 7, ExpectedResult = @"\bpreful")]
		[TestCase(@"er\b\s+\bpreful\b", 9, 7, ExpectedResult = @"\bpreful")]
		[TestCase(@"er\b\s+\bpreful\b", 10, 7, ExpectedResult = @"\bp")]
		[TestCase(@"er\b\s+\bpreful\b", 11, 7, ExpectedResult = @"\bpr")]
		[TestCase(@"er\b\s+\bpreful\b", 12, 7, ExpectedResult = @"\bpre")]
		[TestCase(@"er\b\s+\bpreful\b", 15, 7, ExpectedResult = @"\bpreful")]
		public string FindAffixExpressionAt_PrefixAtPos_ReturnsSuccessfulMatch(string text, int pos,
			int expectedIndex)
		{
			var match = FindAffixExpressionAt(AffixType.Prefix, text, pos);
			Assert.True(match.Success);
			Assert.That(match.Index, Is.EqualTo(expectedIndex));
			return match.Value;
		}

		[TestCase(@"ment\b", 0, 0, ExpectedResult = @"ment\b")]
		[TestCase(@"ment\b", 4, 0, ExpectedResult = @"ment\b")]
		[TestCase(@"ment\b", 5, 0, ExpectedResult = @"ment\b")]
		[TestCase(@"ment\b", 6, 0, ExpectedResult = @"ment\b")]
		[TestCase(@"Blah ment\b", 5, 5, ExpectedResult = @"ment\b")]
		[TestCase(@"Blah ment\b", 6, 5, ExpectedResult = @"ment\b")]
		[TestCase(@"Blah ment\b", 10, 5, ExpectedResult = @"ment\b")]
		[TestCase(@"Blah ment\b", 11, 5, ExpectedResult = @"ment\b")]
		[TestCase(@"able\bah", 1, 0, ExpectedResult = @"able\b")]
		[TestCase(@"able\bah", 4, 0, ExpectedResult = @"able\b")]
		[TestCase(@"able\bah", 5, 0, ExpectedResult = @"able\b")]
		[TestCase(@"able\bah", 6, 0, ExpectedResult = @"able\b")]
		[TestCase(@"\bpre\w+ful\b", 8, 8, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpre\w+ful\b", 9, 8, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpre\w+ful\b", 10, 8, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpre\w+ful\b", 11, 8, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpre\w+ful\b", 12, 8, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpreful\b", 2, 2, ExpectedResult = @"preful\b")]
		[TestCase(@"\bpreful\b", 3, 3, ExpectedResult = @"reful\b")]
		[TestCase(@"\bpreful\b", 5, 5, ExpectedResult = @"ful\b")]
		[TestCase(@"\bpreful\b", 8, 2, ExpectedResult = @"preful\b")]
		[TestCase(@"\bpreful\b", 9, 2, ExpectedResult = @"preful\b")]
		[TestCase(@"\bpreful\b", 10, 2, ExpectedResult = @"preful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 9, 9, ExpectedResult = @"preful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 10, 10, ExpectedResult = @"reful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 12, 12, ExpectedResult = @"ful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 15, 9, ExpectedResult = @"preful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 16, 9, ExpectedResult = @"preful\b")]
		[TestCase(@"er\b\s+\bpreful\b \w+\b", 17, 9, ExpectedResult = @"preful\b")]
		public string FindAffixExpressionAt_SuffixAtPos_ReturnsSuccessfulMatch(string text, int pos,
			int expectedIndex)
		{
			var match = FindAffixExpressionAt(AffixType.Suffix, text, pos);
			Assert.True(match.Success);
			Assert.That(match.Index, Is.EqualTo(expectedIndex));
			return match.Value;
		}

		[TestCase(AffixType.Prefix, "", ExpectedResult = "")]
		[TestCase(AffixType.Prefix, " ", ExpectedResult = "")]
		[TestCase(AffixType.Prefix, "un", ExpectedResult = @"\bun")]
		[TestCase(AffixType.Prefix, "pre", ExpectedResult = @"\bpre")]
		[TestCase(AffixType.Suffix, "", ExpectedResult = "")]
		[TestCase(AffixType.Suffix, " ", ExpectedResult = "")]
		[TestCase(AffixType.Suffix, "ment", ExpectedResult = @"ment\b")]
		[TestCase(AffixType.Suffix, "tion", ExpectedResult = @"tion\b")]
		public string FormatAffix(AffixType affixType, string text)
		{
			return SubstitutionMatchGroup.FormatAffix(affixType, text);
		}

		[TestCase(AffixType.Prefix, ExpectedResult = 2)]
		[TestCase(AffixType.Suffix, ExpectedResult = 0)]
		public int GetAffixPlaceholderPosition_(AffixType affixType)
		{
			return GetAffixPlaceholderPosition(affixType);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box is initially empty.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingPrefix_EmptyTextBox()
		{
			var t = new TextBoxProxy();
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.Empty);
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.Empty);

			ResetTextAndSelectionForUpdatedAffix("pre", AffixType.Prefix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bpre"));
			Assert.That(t.SelectedText, Is.EqualTo("pre"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests starting a new (single-character) prefix at the end of some existing text in
		/// the text box.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_StartNewPrefixAtEndOfExistingText()
		{
			var t = new TextBoxProxy {Text = "Some words "};
			t.SelectionStart = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.Empty);
			
			ResetTextAndSelectionForUpdatedAffix("p", AffixType.Prefix, t, out var affixDeleted);
			
			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"Some words \bp"));
			Assert.That(t.SelectedText, Is.EqualTo("p"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("p"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box is initially empty.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_EmptyTextBox()
		{
			var t = new TextBoxProxy();
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.Empty);
			
			ResetTextAndSelectionForUpdatedAffix("suf", AffixType.Suffix, t, out var affixDeleted);
			
			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"suf\b"));
			Assert.That(t.SelectedText, Is.EqualTo("suf"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("suf"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box initially consists of nothing but
		/// a prefix, and the insertion point is at the start.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_EntireTextBoxIsPrefix_IpAtStart()
		{
			var t = new TextBoxProxy {Text = @"\bpre" };
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));

			ResetTextAndSelectionForUpdatedAffix("ante", AffixType.Prefix, t, out var affixDeleted);
			
			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bante"));
			Assert.That(t.SelectedText, Is.EqualTo("ante"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("ante"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box initially consists of nothing but
		/// a suffix, and the insertion point is at the start.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_EntireTextBoxIsSuffix_IpAtStart()
		{
			var t = new TextBoxProxy {Text = @"suf\b" };
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("suf"));

			ResetTextAndSelectionForUpdatedAffix("post", AffixType.Suffix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"post\b"));
			Assert.That(t.SelectedText, Is.EqualTo("post"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("post"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box initially consists of nothing but
		/// a prefix, and the insertion point is at the end.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_EntireTextBoxIsPrefix_IpAtEnd()
		{
			var t = new TextBoxProxy { Text = @"\bpre" };
			t.SelectionStart = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));

			ResetTextAndSelectionForUpdatedAffix("ante", AffixType.Prefix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bante"));
			Assert.That(t.SelectedText, Is.EqualTo("ante"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("ante"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box initially consists of nothing but
		/// a suffix, and the insertion point is at the end.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_EntireTextBoxIsSuffix_IpAtEnd()
		{
			var t = new TextBoxProxy { Text = @"suf\b" };
			t.SelectionStart = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("suf"));

			ResetTextAndSelectionForUpdatedAffix("post", AffixType.Suffix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"post\b"));
			Assert.That(t.SelectedText, Is.EqualTo("post"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("post"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box initially consists of nothing but
		/// a prefix, and the entire text box text is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_EntireTextBoxIsPrefix_EntireTextSelected()
		{
			var t = new TextBoxProxy { Text = @"\bpre" };
			t.SelectionLength = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));

			ResetTextAndSelectionForUpdatedAffix("ante", AffixType.Prefix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bante"));
			Assert.That(t.SelectedText, Is.EqualTo("ante"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("ante"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box initially consists of nothing but
		/// a suffix, and the entire text box text is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_EntireTextBoxIsSuffix_EntireTextSelected()
		{
			var t = new TextBoxProxy { Text = @"suf\b" };
			t.SelectionLength = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("suf"));

			ResetTextAndSelectionForUpdatedAffix("post", AffixType.Suffix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"post\b"));
			Assert.That(t.SelectedText, Is.EqualTo("post"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("post"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box initially consists of multiple
		/// words including a prefix (which is not the last thing in the text), and the insertion
		/// point is at the end.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_EarlierPrefix_InsertNewPrefixAtEnd()
		{
			var t = new TextBoxProxy { Text = @"ed\b \bpre(\w+) thing " };
			t.SelectionStart = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.Empty);

			ResetTextAndSelectionForUpdatedAffix("ante", AffixType.Prefix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"ed\b \bpre(\w+) thing \bante"));
			Assert.That(t.SelectedText, Is.EqualTo("ante"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("ante"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box initially consists of multiple
		/// words including a suffix (which is not the last thing in the text), and the insertion
		/// point is at the end.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_EarlierSuffix_InsertNewSuffixAtEnd()
		{
			var t = new TextBoxProxy { Text = @"ed\b \bpre(\w+) thing " };
			t.SelectionStart = t.Text.Length;
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.Empty);

			ResetTextAndSelectionForUpdatedAffix("post", AffixType.Suffix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"ed\b \bpre(\w+) thing post\b"));
			Assert.That(t.SelectedText, Is.EqualTo("post"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("post"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting prefix when the text box initially consists of multiple
		/// words including a prefix (which is not the first thing in the text), and the insertion
		/// point is at the beginning.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_LaterPrefix_InsertNewPrefixAtBeginning()
		{
			var t = new TextBoxProxy { Text = @" ed\b \bpre(\w+) thing" };
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.Empty);

			ResetTextAndSelectionForUpdatedAffix("ante", AffixType.Prefix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bante ed\b \bpre(\w+) thing"));
			Assert.That(t.SelectedText, Is.EqualTo("ante"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("ante"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and setting suffix when the text box initially consists of multiple
		/// words including a suffix (which is not the first thing in the text), and the insertion
		/// point is at the beginning.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_LaterSuffix_InsertNewSuffixAtBeginning()
		{
			var t = new TextBoxProxy { Text = @" \bpre(\w+) ed\b thing" };
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.Empty);

			ResetTextAndSelectionForUpdatedAffix("post", AffixType.Suffix, t, out var affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"post\b \bpre(\w+) ed\b thing"));
			Assert.That(t.SelectedText, Is.EqualTo("post"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("post"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and changing an existing prefix when the text box initially consists of
		/// multiple prefixes. The middle prefix is selected and replaced by newly entered text.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_ReplacePrefixInMiddle_EntirePrefixSelected()
		{
			var t = new TextBoxProxy
			{
				Text = @"\bpre(\w+) \bmid(\w+) \blast",
				SelectionStart = 11,
				SelectionLength = 5
			};
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("mid"));

			// Represents incremental changes during typing.
			ResetTextAndSelectionForUpdatedAffix("midd", AffixType.Prefix, t, out var affixDeleted);
			ResetTextAndSelectionForUpdatedAffix("middl", AffixType.Prefix, t, out affixDeleted);
			ResetTextAndSelectionForUpdatedAffix("middle", AffixType.Prefix, t, out affixDeleted);
			
			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"\bpre(\w+) \bmiddle(\w+) \blast"));
			Assert.That(t.SelectedText, Is.EqualTo("middle"));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("middle"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests getting and changing an existing suffix when the text box initially consists of
		/// multiple suffixes. The middle suffix is selected and replaced by newly entered text.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_ReplaceSuffixInMiddle_EntireSuffixSelected()
		{
			var t = new TextBoxProxy
			{
				Text = @"post\b(\w+) (\w+)mid\b (\w+)last\b",
				SelectionStart = 17,
				SelectionLength = 5
			};
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("mid"));

			// Represents incremental changes during typing.
			ResetTextAndSelectionForUpdatedAffix("midd", AffixType.Suffix, t, out var affixDeleted);
			ResetTextAndSelectionForUpdatedAffix("middl", AffixType.Suffix, t, out affixDeleted);
			ResetTextAndSelectionForUpdatedAffix("middle", AffixType.Suffix, t, out affixDeleted);

			Assert.That(affixDeleted, Is.False);
			Assert.That(t.Text, Is.EqualTo(@"post\b(\w+) (\w+)middle\b (\w+)last\b"));
			Assert.That(t.SelectedText, Is.EqualTo("middle"));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("middle"));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests removing an existing prefix.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_RemovePrefix()
		{
			var t = new TextBoxProxy
			{
				Text = @"Good \bpre",
				SelectionStart = 7,
				SelectionLength = 3
			};
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));

			// Represents incremental changes during deleting.
			ResetTextAndSelectionForUpdatedAffix("pr", AffixType.Prefix, t, out var affixDeleted);
			ResetTextAndSelectionForUpdatedAffix("p", AffixType.Prefix, t, out affixDeleted);
			ResetTextAndSelectionForUpdatedAffix(string.Empty, AffixType.Prefix, t, out affixDeleted);

			Assert.That(affixDeleted, Is.True);
			Assert.That(t.Text, Is.EqualTo("Good "));
			Assert.That(t.SelectedText, Is.EqualTo(string.Empty));
			Assert.That(t.SelectionStart, Is.EqualTo(5));
			Assert.That(t.SelectionLength, Is.EqualTo(0));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo(string.Empty));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests removing an existing suffix.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Suffix_RemoveSuffix()
		{
			var t = new TextBoxProxy
			{
				Text = @"ed\b here",
				SelectionStart = 0,
				SelectionLength = 2
			};
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo("ed"));

			// Represents incremental changes during deleting.
			ResetTextAndSelectionForUpdatedAffix("e", AffixType.Suffix, t, out var affixDeleted);
			ResetTextAndSelectionForUpdatedAffix(string.Empty, AffixType.Suffix, t, out affixDeleted);

			Assert.That(affixDeleted, Is.True);
			Assert.That(t.Text, Is.EqualTo(" here"));
			Assert.That(t.SelectedText, Is.EqualTo(string.Empty));
			Assert.That(t.SelectionStart, Is.EqualTo(0));
			Assert.That(t.SelectionLength, Is.EqualTo(0));
			Assert.That(GetExistingAffix(AffixType.Suffix, t), Is.EqualTo(string.Empty));
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests entering a space in the prefix text box when an existing prefix is selected.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ResetTextAndSelectionForUpdatedAffix_Prefix_EnterWhitespaceAsPrefix()
		{
			var t = new TextBoxProxy
			{
				Text = @"Good \bpre",
				SelectionStart = 7,
				SelectionLength = 3
			};
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo("pre"));

			ResetTextAndSelectionForUpdatedAffix(" ", AffixType.Prefix, t, out var affixDeleted);
			
			Assert.That(affixDeleted, Is.True);
			Assert.That(t.Text, Is.EqualTo("Good "));
			Assert.That(t.SelectedText, Is.EqualTo(string.Empty));
			Assert.That(t.SelectionStart, Is.EqualTo(5));
			Assert.That(t.SelectionLength, Is.EqualTo(0));
			Assert.That(GetExistingAffix(AffixType.Prefix, t), Is.EqualTo(string.Empty));
		}
		#endregion

		#region GetMatchGroups tests
		[TestCase("", "&")]
		[TestCase("No match groups here, folks!", "&")]
		[TestCase(@"No \(match groups\) here, folks!", "&")] // literal parentheses (escaped)
		[TestCase(@"invalid (?<named group>why (\\S+) not\\?) expression")]
		[TestCase("(1 unnamed match group here)", "&", "1")]
		[TestCase("before (one match group here) after", "&", "1")]
		[TestCase("before (?<grp1>1 match group here) after", "&", "grp1")]
		[TestCase(@"before (\S+) verb(\bed) after", "&", "1", "2")]
		[TestCase(@"before (?'word'\S+) verb(?<edSuffix>\bed) after", "&", "word", "edSuffix")]
		[TestCase(@"before (?<namedGroup>why (\S+) not\?) after", "&", "1", "namedGroup")]
		public void GetMatchGroups_GetsExpectedResults(string input, params string[] expectedMatches)
		{
			var results = GetMatchGroups(input).ToList();
			Assert.That(results.Select(m => m.Group), Is.EquivalentTo(expectedMatches));
		}
		#endregion

		#region GetExistingMatchGroup tests
		[TestCase("", 0)]
		[TestCase("abc", 0)]
		[TestCase("$5abc", 3)]
		[TestCase("Wow $&", 2)]
		[TestCase("Wow $4", 3)]
		[TestCase("$2 a $1", 3)]
		[TestCase("$2 a $1", 4)]
		public void GetExistingMatchGroup_None_ReturnsNull(string text, int pos)
		{
			Assert.IsNull(GetExistingMatchGroup(text, pos));
		}

		[TestCase("$4", 0, ExpectedResult = 4)]
		[TestCase("$2", 1, ExpectedResult = 2)]
		[TestCase("$3", 2, ExpectedResult = 3)]
		[TestCase("$3abc", 0, ExpectedResult = 3)]
		[TestCase("$2abc", 1, ExpectedResult = 2)]
		[TestCase("$2 $1", 0, ExpectedResult = 2)]
		[TestCase("$2 $1", 1, ExpectedResult = 2)]
		[TestCase("$2 $1", 2, ExpectedResult = 2)]
		[TestCase("$2 $1", 3, ExpectedResult = 1)]
		[TestCase("$2 $1", 4, ExpectedResult = 1)]
		[TestCase("$2 $1", 5, ExpectedResult = 1)]
		public int GetExistingMatchGroup_NumericMatchGroupAtPos_ReturnsNormalMatch(string text,
			int pos)
		{
			var match = GetExistingMatchGroup(text, pos);
			Assert.That(match.Type, Is.EqualTo(MatchGroupType.Normal));
			return Parse(match.Group);
		}

		[TestCase("${willy}", 0, ExpectedResult = "willy")]
		[TestCase("${billy}", 1, ExpectedResult = "billy")]
		[TestCase("${silly}", 2, ExpectedResult = "silly")]
		[TestCase("${hilly}", 7, ExpectedResult = "hilly")]
		[TestCase("${filly}", 8, ExpectedResult = "filly")]
		[TestCase("The ${myNamedMatch} thing", 4, ExpectedResult = "myNamedMatch")]
		[TestCase("The ${myNamedMatch} thing", 5, ExpectedResult = "myNamedMatch")]
		[TestCase("The ${myNamedMatch} thing", 6, ExpectedResult = "myNamedMatch")]
		[TestCase("The ${myNamedMatch} thing", 7, ExpectedResult = "myNamedMatch")]
		[TestCase("The ${myNamedMatch} thing", 18, ExpectedResult = "myNamedMatch")]
		[TestCase("The ${myNamedMatch} thing", 19, ExpectedResult = "myNamedMatch")]
		public string GetExistingMatchGroup_NamedMatchGroupAtPos_ReturnsNormalMatch(string text,
			int pos)
		{
			var match = GetExistingMatchGroup(text, pos);
			Assert.That(match.Type, Is.EqualTo(MatchGroupType.Normal));
			return match.Group;
		}

		[TestCase("$&", 0)]
		[TestCase("$0", 0)]
		[TestCase("$0", 1)]
		[TestCase("Wow $&", 4)]
		[TestCase("Wow $&", 5)]
		[TestCase("Wow $&", 6)]
		public void GetExistingMatchGroup_MatchGroupIsEntireMatch_ReturnsEntireMatch(string text,
			int pos)
		{
			var match = GetExistingMatchGroup(text, pos);
			Assert.That(match, Is.EqualTo(EntireMatch));
		}
		#endregion

		#region UpdateMatchGroup tests
		[TestCase("abc", 3, "2", 0, "IP follows all the text", ExpectedResult = "abc$2")]
		[TestCase("abc", 1, "frog", 0, "IP in the middle of the text", ExpectedResult = "a${frog}bc")]
		[TestCase("abc", 1, "soup", 1, "Selection covers part of the text", ExpectedResult = "a${soup}c")]
		public string UpdateMatchGroup_NoExistingMatchGroup_InsertsRequestedMatchGroup(
			string text, int pos, string groupNameOrNumber, int charsSelected,
			string caseDescription)
		{
			var groupToAdd = new SubstitutionMatchGroup(groupNameOrNumber);
			var result = groupToAdd.UpdateMatchGroup(text, ref pos, charsSelected, out int newSelLength);
			Assert.AreEqual(groupNameOrNumber, result.Substring(pos, newSelLength),
				caseDescription);
			return result;
		}

		[TestCase("abc$4", 5, "2", 0, "IP follows all the text, which ends with a different " +
			"numeric substitution expression", ExpectedResult = "abc$2")]
		[TestCase("$1$2$4", 3, "3", 0, "IP between dollar sign and number of a different " +
			"numeric substitution expression", ExpectedResult = "$1$3$4")]
		[TestCase("$1$2$4", 2, "3", 0, "IP between two numeric substitution expressions",
			ExpectedResult = "$3$2$4")]
		[TestCase("${verb} $1${noun}", 9, "adjective", 0, "IP between two numeric and named " +
			"substitution expressions", ExpectedResult = "${verb} ${adjective}${noun}")]
		[TestCase("${verb} $1${noun}", 8, "adjective", 2, "Selection covers numeric " +
			"substitution expression preceding a named substitution expression",
			ExpectedResult = "${verb} ${adjective}${noun}")]
		[TestCase("${verb} $1${noun}", 2, "adjective", 4, "Selection covers name portion of " +
			"a named substitution expression", ExpectedResult = "${adjective} $1${noun}")]
		public string UpdateMatchGroup_ExistingMatchGroup_ReplacesWithRequestedMatchGroup(
			string text, int pos, string groupNameOrNumber, int charsSelected,
			string caseDescription)
		{
			var groupToAdd = new SubstitutionMatchGroup(groupNameOrNumber);
			var result = groupToAdd.UpdateMatchGroup(text, ref pos, charsSelected, out int newSelLength);
			Assert.AreEqual(groupNameOrNumber, result.Substring(pos, newSelLength),
				caseDescription);
			return result;
		}

		[TestCase("abc $0", "Some text followed by a substitution expression for group 0 " +
			"(which is synonymous with $& - substitute entire match)", ExpectedResult = "abc ")]
		[TestCase("${yu}", "Text consists of a named substitution expression", ExpectedResult = "")]
		public string UpdateMatchGroup_IPAtEnd_Remove_RemovesMatchGroup(string text,
			string caseDescription)
		{
			int pos = text.Length;
			var result = RemoveGroup.UpdateMatchGroup(text, ref pos, 0,
				out int newSelLength);
			Assert.AreEqual(result.Length, pos, caseDescription);
			Assert.AreEqual(0, newSelLength, caseDescription);
			return result;
		}

		[TestCase("blah $2", 6, 1, 5, "Number part of numeric substitution expression selected",
			ExpectedResult = "blah ")]
		[TestCase("blah $2", 5, 2, 5, "Entire numeric substitution expression selected",
			ExpectedResult = "blah ")]
		[TestCase("blah$2", 4, 1, 4, "Dollar sign in numeric substitution expression selected",
			ExpectedResult = "blah")]
		[TestCase("blah $2", 5, 0, 5, "IP before dollar sign in numeric substitution",
			ExpectedResult = "blah ")]
		[TestCase("my $1 blah $2", 12, 0, 11, "IP between dollar sign and number in numeric substitution",
			ExpectedResult = "my $1 blah ")]
		[TestCase("froggy $2 soup", 9, 0, 7, "IP after number in numeric substitution",
			ExpectedResult = "froggy  soup")]
		[TestCase("${word}-${last}", 2, 4, 0, "Name part of named substitution expression selected",
			ExpectedResult = "-${last}")]
		[TestCase("${word}-${last} ", 0, 1, 0, "Dollar sign of named substitution expression selected",
			ExpectedResult = "-${last} ")]
		[TestCase("${word}-${last} ", 0, 2, 0, "Dollar sign and opening brace of named substitution expression selected",
			ExpectedResult = "-${last} ")]
		[TestCase("${word}-${last} ", 8, 7, 8, "Entire named substitution expression selected",
			ExpectedResult = "${word}- ")]
		[TestCase("${word}-${last} ", 8, 0, 8, "IP before dollar sign of named substitution expression",
			ExpectedResult = "${word}- ")]
		[TestCase("${word}-${last} ", 8, 0, 8, "IP in name of named substitution expression",
			ExpectedResult = "${word}- ")]
		[TestCase("${word}-${last} ", 15, 0, 8, "IP immediately following named substitution expression",
			ExpectedResult = "${word}- ")]
		public string UpdateMatchGroup_Remove_InsertsNewNumericMatchGroup(
			string text, int pos, int charsSelected, int expectedNewPos, string caseDescription)
		{
			var result = RemoveGroup.UpdateMatchGroup(text, ref pos,
				charsSelected, out int newSelLength);
			Assert.AreEqual(expectedNewPos, pos, caseDescription);
			Assert.AreEqual(0, newSelLength, caseDescription);
			return result;
		}

		[Test]
		public void UpdateMatchGroup_Remove_NoExistingMatchGroup_ThrowsInvalidOperationException()
		{
			int pos = 5;
			Assert.That(() =>
			{
				RemoveGroup.UpdateMatchGroup("$& My frog soup", ref pos,
					2, out _);
			}, Throws.InvalidOperationException);
		}
		#endregion
	}
}
