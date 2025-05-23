// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	#region class DummyPhraseSubstitutionsDlg
	internal class DummyPhraseSubstitutionsDlg : PhraseSubstitutionsDlg
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="DummyPhraseSubstitutionsDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal DummyPhraseSubstitutionsDlg() : base(new Substitution[] { }, new string[] { }, 0)
		{
			TextControl = new TextBoxProxy();
		}

		internal ITextWithSelection FauxEditedTextControl => TextControl;

		public void ChangeMatchGroup(string s)
		{
			if (s == string.Empty)
				s = RemoveItem;
			UpdateMatchGroup(s);
		}
	}
	#endregion

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture, Apartment(ApartmentState.STA)]
	[SuppressMessage("Gendarme.Rules.Design", "TypesWithDisposableFieldsShouldBeDisposableRule",
		Justification="Unit test - m_dlg gets disposed in Teardown(); m_textBox is a reference")]
	public class PhraseSubstitutionsDlgTests
	{
		#region Data Members
		private DummyPhraseSubstitutionsDlg m_dlg;
		#endregion

		#region Properties
		private ITextWithSelection TextBox => m_dlg.FauxEditedTextControl;
		#endregion

		#region Setup and Teardown
		[SetUp]
		public void Setup()
		{
			m_dlg = new DummyPhraseSubstitutionsDlg();
		}

		[TearDown]
		public void Teardown()
		{
			m_dlg.Dispose();
		}
		#endregion

		#region MatchGroup tests
		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ExistingMatchGroup when the text box is empty.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_GetExisting_EmptyTextBox()
		{
			Assert.AreEqual(string.Empty, m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests inserting a numeric match group in an empty text box.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_InsertNumeric_EmptyTextBox()
		{
			m_dlg.ChangeMatchGroup("2");
			Assert.AreEqual("$2", TextBox.Text);
			Assert.AreEqual("2", m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests inserting a named match group in an empty text box.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_InsertNamed_EmptyTextBox()
		{
			m_dlg.ChangeMatchGroup("willy");
			Assert.AreEqual("${willy}", TextBox.Text);
			Assert.AreEqual("willy", m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests inserting a match group corresponding to the entire match in an empty text box.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_InsertEntireMatch_EmptyTextBox()
		{
			m_dlg.ChangeMatchGroup("Entire match");
			Assert.AreEqual("$&", TextBox.Text);
			Assert.AreEqual("&", TextBox.SelectedText);
			Assert.AreEqual("Entire match", m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ExistingMatchGroup and UpdateMatchGroup methods when the selection
		/// precedes all the text in the text box - should do nothing.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_IpPrecedingText_InsertNumericMatchGroup()
		{
			TextBox.Text = "abc";
			TextBox.SelectionStart = 0;
			TextBox.SelectionLength = 0;
			Assert.AreEqual(string.Empty, m_dlg.ExistingMatchGroup);
			m_dlg.ChangeMatchGroup("2");
			Assert.AreEqual("$2abc", TextBox.Text);
			Assert.AreEqual("2", TextBox.SelectedText);
			Assert.AreEqual(1, TextBox.SelectionStart);
			Assert.AreEqual("2", m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ExistingMatchGroup and UpdateMatchGroup methods when the selection
		/// follows all the text in the text box, and there is no match group.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_NoExistingMatchGroup_IpAtEnd_InsertNumericMatchGroup()
		{
			TextBox.Text = "abc";
			TextBox.SelectionStart = 3;
			TextBox.SelectionLength = 0;
			Assert.AreEqual(string.Empty, m_dlg.ExistingMatchGroup);
			m_dlg.ChangeMatchGroup("2");
			Assert.AreEqual("abc$2", TextBox.Text);
			Assert.AreEqual("2", TextBox.SelectedText);
			Assert.AreEqual("2", m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ExistingMatchGroup method when the text box contains some text followed by
		/// a substitution expression for group 0 (which is synonymous with $& - substitute
		/// entire match), and the insertion point is at the end. Then remove that group
		/// substitution expression.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_GetAndRemoveExisting_IpAfterGroupZero()
		{
			TextBox.Text = "abc $0";
			TextBox.SelectionStart = TextBox.Text.Length;
			TextBox.SelectionLength = 0;
			Assert.AreEqual("Entire match", m_dlg.ExistingMatchGroup);
			m_dlg.ChangeMatchGroup(string.Empty);
			Assert.AreEqual("abc ", TextBox.Text);
			Assert.AreEqual(4, TextBox.SelectionStart);
			Assert.AreEqual(0, TextBox.SelectionLength);
			Assert.AreEqual(string.Empty, m_dlg.ExistingMatchGroup);
		}

		///--------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ExistingMatchGroup method when the text box contains a named substitution
		/// expression, and the insertion point is at the end. Then remove that group
		/// substitution expression.
		/// </summary>
		///--------------------------------------------------------------------------------------
		[Test]
		public void ExistingMatchGroup_GetAndRemoveExisting_IpAfterNamedGroup()
		{
			TextBox.Text = "${yu}";
			TextBox.SelectionStart = TextBox.Text.Length;
			TextBox.SelectionLength = 0;
			Assert.AreEqual("yu", m_dlg.ExistingMatchGroup);
			m_dlg.ChangeMatchGroup(string.Empty);
			Assert.AreEqual("", TextBox.Text);
			Assert.AreEqual(0, TextBox.SelectionStart);
			Assert.AreEqual(0, TextBox.SelectionLength);
			Assert.AreEqual(string.Empty, m_dlg.ExistingMatchGroup);
		}
		#endregion
	}
}
