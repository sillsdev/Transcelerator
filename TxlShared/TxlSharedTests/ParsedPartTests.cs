// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ParsedPartTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SIL.Transcelerator
{
    [TestFixture]
    public class ParsedPartTests
	{
		[Test]
		public void Construct_FromWords_CreatesTranslatablePart()
		{
			var part = new ParsedPart(new Word[] { "cool", "beans" });
			Assert.AreEqual(PartType.TranslatablePart, part.Type);
			Assert.AreEqual(2, part.Words.Count);
			Assert.AreEqual("cool beans", part.Text);
		}

		[Test]
		public void Construct_FromKeyTermMatchSurrogate_CreatesKeyTermPart()
		{
			var part = new ParsedPart(new KeyTermMatchSurrogate("blah snerb", "bleh", "bloh"));
			Assert.AreEqual(PartType.KeyTerm, part.Type);
			Assert.AreEqual(2, part.Words.Count);
			Assert.AreEqual("blah snerb", part.Text);
			Assert.AreEqual(2, part.Words.Count);
			Assert.AreEqual("blah", part.Words[0].Text);
			Assert.AreEqual("snerb", part.Words[1].Text);
		}

		[Test]
		public void SetWords_TextNotPreviouslySet_SetsWords()
		{
			var part = new ParsedPart();
			part.Words = new List<Word>(new Word[] { "cool", "beans" });
			Assert.AreEqual(2, part.Words.Count);
			Assert.AreEqual("cool beans", part.Text);
		}

	    [Test]
	    public void SetWords_TextAlreadySet_ThrowsInvalidOperationException()
	    {
		    var part = new ParsedPart();
		    part.Words = new List<Word>(new Word[] {"cool", "beans"});
		    Assert.AreEqual("cool beans", part.Text);
		    Assert.Throws<InvalidOperationException>(() => {
				part.Words = new List<Word>(new Word[] {"flog", "legs"});
		    });
		}

		[Test]
		public void SetText_WordsNotPreviouslySet_SetsText()
		{
			var part = new ParsedPart();
			part.Text = "cool beans";
			Assert.AreEqual(2, part.Words.Count);
			Assert.AreEqual("cool beans", part.Text);
		}

		[Test]
		public void AddOwningPhrase_NullList_CreatesNewList()
		{
			var part = new ParsedPart(new Word[] { "cool", "beans" });
			Assert.AreEqual(PartType.TranslatablePart, part.Type);
			var q = new Question();
			q.Text = "Why?";
			part.AddOwningPhrase(q);
			Assert.AreEqual("Why?", part.Owners.Single().Text);
		}

		[Test]
		public void AddOwningPhrase_ExistingList_AddsToList()
		{
			var part = new ParsedPart(new Word[] { "beans" });
			var q = new Question();
			q.Text = "Why?";
			part.AddOwningPhrase(q);
			q = new Question();
			q.Text = "When?";
			part.AddOwningPhrase(q);
			Assert.AreEqual("Why?", part.Owners.First().Text);
			Assert.AreEqual("When?", part.Owners.Skip(1).First().Text);
		}

		[Test]
		public void AddOwningPhrase_KeyTermPart_ThrowsInvalidOperationException()
		{
			var part = new ParsedPart(new KeyTermMatchSurrogate("blah snerb", "bleh", "bloh"));
			Assert.AreEqual(PartType.KeyTerm, part.Type);
			Assert.Throws<InvalidOperationException>(() => part.AddOwningPhrase(new Question()));
		}
    }
}
