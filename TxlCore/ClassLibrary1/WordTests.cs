// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: WordTests.cs
// ---------------------------------------------------------------------------------------------
using NUnit.Framework;

namespace SIL.Transcelerator
{
    [TestFixture]
    public class WordTests
    {
        [Test]
        public void FirstWord_DifferentStringsWithSameFirstWord_ReturnsSameWord()
        {
            Assert.AreEqual(Word.FirstWord("tom is nice"), Word.FirstWord("tom has a frog"));
        }

        [Test]
        public void FirstWord_StringsWithDifferentFirstWords_ReturnsDifferentWords()
        {
            Assert.AreNotEqual(Word.FirstWord("tim is nice"), Word.FirstWord("tom has a frog"));
        }

        [Test]
        public void FirstWord_NullString_ReturnsNull()
        {
            Assert.IsNull(Word.FirstWord(null));
        }

        [Test]
        public void FirstWord_EmptyString_ReturnsNull()
        {
            Assert.IsNull(Word.FirstWord(""));
        }

        [Test]
        public void FirstWord_StringWithOnlySpaces_ReturnsNull()
        {
            Assert.IsNull(Word.FirstWord("    "));
        }

        [Test]
        public void FirstWord_StringWithLeadingSpaces_ReturnsCorrectWord()
        {
            Word word = "tom";
            Assert.AreEqual(word, Word.FirstWord("  tom is a dude"));
        }

        [Test]
        public void IsEquivalent_SameWord_ReturnsTrue()
        {
            Word word = "monkey";
            Assert.IsTrue(word.IsEquivalent(word));
        }

        [Test]
        public void IsEquivalent_DifferentWord_ReturnsFalse()
        {
            Word word = "monkey";
            Assert.IsFalse(word.IsEquivalent("frog"));
        }

        [Test]
        public void IsEquivalent_InflectedFormOfWord_ReturnsTrue()
        {
            Word word = "monkey";
            word.AddAlternateForm("monkeys");
            Assert.IsTrue(word.IsEquivalent("monkeys"));
        }
    }
}
