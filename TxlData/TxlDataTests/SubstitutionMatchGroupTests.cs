// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2023' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Linq;
using NUnit.Framework;
using static System.Int32;

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
			return SubstitutionMatchGroup.GetRangeMax(text, pos);
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
			var result = SubstitutionMatchGroup.UpdateRangeMax(newValue, text, ref pos, selLength, out var length);
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
			var result = SubstitutionMatchGroup.UpdateRangeMax(newValue, text, ref pos, selLength, out var length);
			Assert.That(pos, Is.EqualTo(expectedStart), explanation);
			Assert.That(length, Is.EqualTo(expectedLength), explanation);
			return result;
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
			Assert.False(SubstitutionMatchGroup.FindAffixExpressionAt(AffixType.Prefix, text, pos).Success);
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
			Assert.False(SubstitutionMatchGroup.FindAffixExpressionAt(AffixType.Suffix, text, pos).Success);
		}

		[TestCase(@"\bpre fix", 0, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre fix", 1, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"\bpre fix", 3, 0, ExpectedResult = @"\bpre")]
		[TestCase(@"Bl\bah", 2, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 3, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 4, 2, ExpectedResult = @"\bah")]
		[TestCase(@"Bl\bah", 6, 2, ExpectedResult = @"\bah")]
		public string FindAffixExpressionAt_PrefixAtPos_ReturnsSuccessfulMatch(string text, int pos,
			int expectedIndex)
		{
			var match = SubstitutionMatchGroup.FindAffixExpressionAt(AffixType.Prefix, text, pos);
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
		public string FindAffixExpressionAt_SuffixAtPos_ReturnsSuccessfulMatch(string text, int pos,
			int expectedIndex)
		{
			var match = SubstitutionMatchGroup.FindAffixExpressionAt(AffixType.Suffix, text, pos);
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
			return SubstitutionMatchGroup.GetAffixPlaceholderPosition(affixType);
		}

		// TODO: Depending on whether/how I can get the logic moved from
		// PhraseSubstitutionDlg.SuffixOrPrefixChanged into SubstitutionMatchGroup, these tests
		// might be able to be done here.
		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests starting a new (single-character) prefix at the end of some existing text in
		///// the text box.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_StartNewPrefixAtEndOfExistingText()
		//{
		//	m_textBox.Text = @"Some words ";
		//	m_textBox.SelectionStart = m_textBox.Text.Length;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"p");
		//	Assert.AreEqual(@"Some words \bp", m_textBox.Text);
		//	Assert.AreEqual(@"p", m_textBox.SelectedText);
		//	Assert.AreEqual(@"p", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box is initially empty.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_EmptyTextBox()
		//{
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"suf");
		//	Assert.AreEqual(@"suf\b", m_textBox.Text);
		//	Assert.AreEqual(@"suf", m_textBox.SelectedText);
		//	Assert.AreEqual(@"suf", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting prefix when the text box initially consists of nothing but
		///// a prefix, and the insertion point is at the start.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_EntireTextBoxIsPrefix_IpAtStart()
		//{
		//	m_textBox.Text = @"\bpre";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(@"pre", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"ante");
		//	Assert.AreEqual(@"\bante", m_textBox.Text);
		//	Assert.AreEqual(@"ante", m_textBox.SelectedText);
		//	Assert.AreEqual(@"ante", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box initially consists of nothing but
		///// a suffix, and the insertion point is at the start.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_EntireTextBoxIsPrefix_IpAtStart()
		//{
		//	m_textBox.Text = @"suf\b";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(@"suf", m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"post");
		//	Assert.AreEqual(@"post\b", m_textBox.Text);
		//	Assert.AreEqual(@"post", m_textBox.SelectedText);
		//	Assert.AreEqual(@"post", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting prefix when the text box initially consists of nothing but
		///// a prefix, and the insertion point is at the end.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_EntireTextBoxIsPrefix_IpAtEnd()
		//{
		//	m_textBox.Text = @"\bpre";
		//	m_textBox.SelectionStart = m_textBox.Text.Length;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(@"pre", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"ante");
		//	Assert.AreEqual(@"\bante", m_textBox.Text);
		//	Assert.AreEqual(@"ante", m_textBox.SelectedText);
		//	Assert.AreEqual(@"ante", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box initially consists of nothing but
		///// a suffix, and the insertion point is at the end.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_EntireTextBoxIsPrefix_IpAtEnd()
		//{
		//	m_textBox.Text = @"suf\b";
		//	m_textBox.SelectionStart = m_textBox.Text.Length;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(@"suf", m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"post");
		//	Assert.AreEqual(@"post\b", m_textBox.Text);
		//	Assert.AreEqual(@"post", m_textBox.SelectedText);
		//	Assert.AreEqual(@"post", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting prefix when the text box initially consists of nothing but
		///// a prefix, and the entire text box text is selected.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_EntireTextBoxIsPrefix_EntireTextSelected()
		//{
		//	m_textBox.Text = @"\bpre";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = m_textBox.Text.Length;
		//	Assert.AreEqual(@"pre", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"ante");
		//	Assert.AreEqual(@"\bante", m_textBox.Text);
		//	Assert.AreEqual(@"ante", m_textBox.SelectedText);
		//	Assert.AreEqual(@"ante", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box initially consists of nothing but
		///// a suffix, and the entire text box text is selected.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_EntireTextBoxIsPrefix_EntireTextSelected()
		//{
		//	m_textBox.Text = @"suf\b";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = m_textBox.Text.Length;
		//	Assert.AreEqual(@"suf", m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"post");
		//	Assert.AreEqual(@"post\b", m_textBox.Text);
		//	Assert.AreEqual(@"post", m_textBox.SelectedText);
		//	Assert.AreEqual(@"post", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting prefix when the text box initially consists of multiple
		///// words including a prefix (which is not the last thing in the text), and the insertion
		///// point is at the end.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_EarlierPrefix_InsertNewPrefixAtEnd()
		//{
		//	m_textBox.Text = @"ed\b \bpre(\w+) thing ";
		//	m_textBox.SelectionStart = m_textBox.Text.Length;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"ante");
		//	Assert.AreEqual(@"ed\b \bpre(\w+) thing \bante", m_textBox.Text);
		//	Assert.AreEqual(@"ante", m_textBox.SelectedText);
		//	Assert.AreEqual(@"ante", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box initially consists of multiple
		///// words including a suffix (which is not the last thing in the text), and the insertion
		///// point is at the end.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_EarlierSuffix_InsertNewSuffixAtEnd()
		//{
		//	m_textBox.Text = @"ed\b \bpre(\w+) thing ";
		//	m_textBox.SelectionStart = m_textBox.Text.Length;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"post");
		//	Assert.AreEqual(@"ed\b \bpre(\w+) thing post\b", m_textBox.Text);
		//	Assert.AreEqual(@"post", m_textBox.SelectedText);
		//	Assert.AreEqual(@"post", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting prefix when the text box initially consists of multiple
		///// words including a prefix (which is not the first thing in the text), and the insertion
		///// point is at the beginning.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_LaterPrefix_InsertNewPrefixAtBeginning()
		//{
		//	m_textBox.Text = @" ed\b \bpre(\w+) thing";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"ante");
		//	Assert.AreEqual(@"\bante ed\b \bpre(\w+) thing", m_textBox.Text);
		//	Assert.AreEqual(@"ante", m_textBox.SelectedText);
		//	Assert.AreEqual(@"ante", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and setting suffix when the text box initially consists of multiple
		///// words including a suffix (which is not the first thing in the text), and the insertion
		///// point is at the beginning.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_LaterSuffix_InsertNewSuffixAtBeginning()
		//{
		//	m_textBox.Text = @" \bpre(\w+) ed\b thing";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = 0;
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"post");
		//	Assert.AreEqual(@"post\b \bpre(\w+) ed\b thing", m_textBox.Text);
		//	Assert.AreEqual(@"post", m_textBox.SelectedText);
		//	Assert.AreEqual(@"post", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and changing an existing prefix when the text box initially consists of
		///// multiple prefixes. The middle prefix is selected and replaced by newly entered text.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_ReplacePrefixInMiddle_EntirePrefixSelected()
		//{
		//	m_textBox.Text = @"\bpre(\w+) \bmid(\w+) \blast";
		//	m_textBox.SelectionStart = 11;
		//	m_textBox.SelectionLength = 5;
		//	Assert.AreEqual("mid", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"midd");
		//	Assert.AreEqual(@"\bpre(\w+) \bmidd(\w+) \blast", m_textBox.Text);
		//	m_dlg.ChangePrefix(@"middl");
		//	Assert.AreEqual(@"\bpre(\w+) \bmiddl(\w+) \blast", m_textBox.Text);
		//	m_dlg.ChangePrefix(@"middle");
		//	Assert.AreEqual(@"\bpre(\w+) \bmiddle(\w+) \blast", m_textBox.Text);
		//	Assert.AreEqual(@"middle", m_textBox.SelectedText);
		//	Assert.AreEqual(@"middle", m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests getting and changing an existing suffix when the text box initially consists of
		///// multiple suffixes. The middle suffix is selected and replaced by newly entered text.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_ReplaceSuffixInMiddle_EntireSuffixSelected()
		//{
		//	m_textBox.Text = @"post\b(\w+) (\w+)mid\b (\w+)last\b";
		//	m_textBox.SelectionStart = 17;
		//	m_textBox.SelectionLength = 5;
		//	Assert.AreEqual("mid", m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"midd");
		//	Assert.AreEqual(@"post\b(\w+) (\w+)midd\b (\w+)last\b", m_textBox.Text);
		//	m_dlg.ChangeSuffix(@"middl");
		//	Assert.AreEqual(@"post\b(\w+) (\w+)middl\b (\w+)last\b", m_textBox.Text);
		//	m_dlg.ChangeSuffix(@"middle");
		//	Assert.AreEqual(@"post\b(\w+) (\w+)middle\b (\w+)last\b", m_textBox.Text);
		//	Assert.AreEqual(@"middle", m_textBox.SelectedText);
		//	Assert.AreEqual(@"middle", m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests removing an existing prefix.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_RemovePrefix()
		//{
		//	m_textBox.Text = @"Good \bpre";
		//	m_textBox.SelectionStart = 7;
		//	m_textBox.SelectionLength = 3;
		//	Assert.AreEqual("pre", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@"pr");
		//	Assert.AreEqual(@"Good \bpr", m_textBox.Text);
		//	m_dlg.ChangePrefix(@"p");
		//	Assert.AreEqual(@"Good \bp", m_textBox.Text);
		//	m_dlg.ChangePrefix(string.Empty);
		//	Assert.AreEqual(@"Good ", m_textBox.Text);
		//	Assert.AreEqual(string.Empty, m_textBox.SelectedText);
		//	Assert.AreEqual(5, m_textBox.SelectionStart);
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingPrefix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests removing an existing suffix.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Suffix_RemoveSuffix()
		//{
		//	m_textBox.Text = @"ed\b here";
		//	m_textBox.SelectionStart = 0;
		//	m_textBox.SelectionLength = 2;
		//	Assert.AreEqual("ed", m_dlg.ExistingSuffix);
		//	m_dlg.ChangeSuffix(@"e");
		//	Assert.AreEqual(@"e\b here", m_textBox.Text);
		//	m_dlg.ChangeSuffix(string.Empty);
		//	Assert.AreEqual(@" here", m_textBox.Text);
		//	Assert.AreEqual(string.Empty, m_textBox.SelectedText);
		//	Assert.AreEqual(0, m_textBox.SelectionStart);
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingSuffix);
		//}

		/////--------------------------------------------------------------------------------------
		///// <summary>
		///// Tests entering a space in the prefix text box when an existing prefix is selected.
		///// </summary>
		/////--------------------------------------------------------------------------------------
		//[Test]
		//public void Prefix_EnterWhitespaceAsPrefix()
		//{
		//	m_textBox.Text = @"Good \bpre";
		//	m_textBox.SelectionStart = 7;
		//	m_textBox.SelectionLength = 3;
		//	Assert.AreEqual("pre", m_dlg.ExistingPrefix);
		//	m_dlg.ChangePrefix(@" ");
		//	Assert.AreEqual(@"Good ", m_textBox.Text);
		//	Assert.AreEqual(string.Empty, m_textBox.SelectedText);
		//	Assert.AreEqual(5, m_textBox.SelectionStart);
		//	Assert.AreEqual(string.Empty, m_dlg.ExistingPrefix);
		//}
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
			var results = SubstitutionMatchGroup.GetMatchGroups(input).ToList();
			Assert.That(results.Select(m => m.Group), Is.EquivalentTo(expectedMatches));
		}
		#endregion

		#region GetExistingMatchGroup tests
		[TestCase("", 0)]
		[TestCase("abc", 0)]
		[TestCase("$5abc", 3)]
		[TestCase("Wow $&", 2)]
		[TestCase("Wow $4", 3)]
		public void GetExistingMatchGroup_None_ReturnsNull(string text, int pos)
		{
			Assert.IsNull(SubstitutionMatchGroup.GetExistingMatchGroup(text, pos));
		}

		[TestCase("$4", 0, ExpectedResult = 4)]
		[TestCase("$2", 1, ExpectedResult = 2)]
		[TestCase("$3", 2, ExpectedResult = 3)]
		[TestCase("$3abc", 0, ExpectedResult = 3)]
		[TestCase("$2abc", 1, ExpectedResult = 2)]
		public int GetExistingMatchGroup_NumericMatchGroupAtPos_ReturnsNormalMatch(string text,
			int pos)
		{
			var match = SubstitutionMatchGroup.GetExistingMatchGroup(text, pos);
			Assert.That(match.Type, Is.EqualTo(SubstitutionMatchGroup.MatchGroupType.Normal));
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
			var match = SubstitutionMatchGroup.GetExistingMatchGroup(text, pos);
			Assert.That(match.Type, Is.EqualTo(SubstitutionMatchGroup.MatchGroupType.Normal));
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
			var match = SubstitutionMatchGroup.GetExistingMatchGroup(text, pos);
			Assert.That(match, Is.EqualTo(SubstitutionMatchGroup.EntireMatch));
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
			var result = SubstitutionMatchGroup.RemoveGroup.UpdateMatchGroup(text, ref pos, 0,
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
			var result = SubstitutionMatchGroup.RemoveGroup.UpdateMatchGroup(text, ref pos,
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
				SubstitutionMatchGroup.RemoveGroup.UpdateMatchGroup("$& My frog soup", ref pos,
					2, out _);
			}, Throws.InvalidOperationException);
		}
		#endregion
	}
}
