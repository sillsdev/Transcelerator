// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: TranslationUnitTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace SIL.Transcelerator.Localization
{
    [TestFixture]
    public class TranslationUnitTests
    {
		#region set_Approved = false
		[TestCase(true)]
        [TestCase(false)]
        public void Approved_SetFalseAndThenSetTarget_NoEffect(bool isTargetLocalized)
        {
			var sut = new TranslationUnit();
			sut.Approved = false;
			Assert.IsFalse(sut.Approved);
			sut.Target = new Localization { Text = "Blah", IsLocalized = isTargetLocalized };
            Assert.AreEqual(isTargetLocalized, sut.Target.IsLocalized);
			Assert.IsFalse(sut.Approved);
		}

	    [Test]
	    public void Approved_SetFalseAndThenSetTargetNull_NoEffect()
	    {
		    var sut = new TranslationUnit();
		    sut.Target = new Localization { Text = "Blah", IsLocalized = true };
		    sut.Approved = false;
            Assert.IsTrue(sut.Target.IsLocalized);
		    Assert.IsFalse(sut.Approved);
		    sut.Target = null;
		    Assert.IsFalse(sut.Approved);
		}

	    [Test]
	    public void Approved_SetFalseAndThenSetTargetNotLocalized_NoEffect()
	    {
		    var sut = new TranslationUnit();
		    sut.Target = new Localization { Text = "Blah", IsLocalized = true };
		    sut.Approved = false;
		    Assert.IsFalse(sut.Approved);
		    sut.Target.IsLocalized = false;
		    Assert.IsFalse(sut.Approved);
	    }

	    [Test]
		public void Approved_SetTargeNotLocalizedAndThenSetFalse_NoEffect()
	    {
		    var sut = new TranslationUnit();
		    Assert.IsFalse(sut.Approved);
		    sut.Target = new Localization { Text = "Blah", IsLocalized = true };
		    sut.Approved = false;
		    Assert.IsTrue(sut.Target.IsLocalized);
		    Assert.IsFalse(sut.Approved);
	    }
		#endregion

		#region set_Approved = true
	    [Test]
	    public void Approved_SetTrueAndThenSetLocalizedTarget_ApprovedIsTrue()
	    {
		    var sut = new TranslationUnit();
		    sut.Approved = true;
		    Assert.IsFalse(sut.Approved);
		    sut.Target = new Localization { Text = "Blah", IsLocalized = true };
		    Assert.IsTrue(sut.Approved);
		    Assert.IsTrue(sut.Target.IsLocalized);
		}

	    [TestCase(null)]
	    [TestCase("")]
	    [TestCase("Blah")]
	    public void Approved_SetTrueAndThenSetNonLocalizedTarget_ApprovedIsFalse(string localizedString)
	    {
		    var sut = new TranslationUnit();
		    sut.Approved = true;
		    Assert.IsFalse(sut.Approved);
		    sut.Target = new Localization { Text = localizedString, IsLocalized = false };
		    Assert.IsFalse(sut.Approved);
		    Assert.IsFalse(sut.Target.IsLocalized);
		}

	    [Test]
		public void Approved_SetLocalizedTargetAndThenSetTrue_LocalizedBecomesApproved()
	    {
		    var sut = new TranslationUnit();
		    Assert.IsFalse(sut.Approved);
		    sut.Target = new Localization { Text = "Blah", IsLocalized = true };
		    sut.Approved = true;
		    Assert.IsTrue(sut.Target.IsLocalized);
		    Assert.IsTrue(sut.Approved);
		}
		#endregion
	}
}
