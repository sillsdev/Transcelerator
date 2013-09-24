// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermMatchTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using AddInSideViews;
using NUnit.Framework;
using System;
using Rhino.Mocks;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class KeyTermMatchTests
    {
        [Test]
        public void AppliesTo_TermAllowedToMatchAnywhere_ReturnsTrue()
        {
            IKeyTerm term = KeyTermMatchBuilderTests.AddMockedKeyTerm("tom");
            KeyTermMatch match = new KeyTermMatch(new List<Word>(new Word[] { "tom" }), term, false);
            Assert.IsTrue(match.AppliesTo(-1, -1));
        }

        [Test]
        public void AppliesTo_TermOccursInRange_ReturnsTrue()
        {
            IKeyTerm term = KeyTermMatchBuilderTests.AddMockedKeyTerm("tom", 002003002);
            KeyTermMatch match = new KeyTermMatch(new List<Word>(new Word[] { "tom"}), term, true);
            Assert.IsTrue(match.AppliesTo(002003002, 002003002));
            Assert.IsTrue(match.AppliesTo(002003002, 002003003));
            Assert.IsTrue(match.AppliesTo(002003001, 002003002));
            Assert.IsTrue(match.AppliesTo(002003001, 002003003));
        }

        [Test]
        public void AppliesTo_TermDoesNotOccurInRange_ReturnsFalse()
        {
            IKeyTerm term = KeyTermMatchBuilderTests.AddMockedKeyTerm("tom", 002003001,002003003);
            KeyTermMatch match = new KeyTermMatch(new List<Word>(new Word[] { "tom" }), term, true);
            Assert.IsFalse(match.AppliesTo(002003002, 002003002));
        }
    }
}
