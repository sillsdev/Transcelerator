// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PorterStemmerEnhancementTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TsStringUtils tests.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class PorterStemmerEnhancementTests
	{
		private static PorterStemmer s_stemmer = new PorterStemmer();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for word ending in "-ying"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void YingSuffix()
		{
			Assert.AreEqual("pray", s_stemmer.stemTerm("praying"));
			Assert.AreEqual("buy", s_stemmer.stemTerm("buying"));
			Assert.AreEqual("ly", s_stemmer.stemTerm("lying"));
			Assert.AreEqual("obey", s_stemmer.stemTerm("obeying"));
			Assert.AreEqual("repli", s_stemmer.stemTerm("replying"));
			Assert.AreEqual("studi", s_stemmer.stemTerm("studying"));
			Assert.AreEqual("tidi", s_stemmer.stemTerm("tidying"));
			Assert.AreEqual("ying", s_stemmer.stemTerm("ying"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for word ending in "-le"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void LeSuffix()
        {
            Assert.AreEqual("disciple", s_stemmer.stemTerm("disciples"));
            Assert.AreEqual("temple", s_stemmer.stemTerm("temples"));
            Assert.AreEqual("people", s_stemmer.stemTerm("people"));
            Assert.AreEqual("cripple", s_stemmer.stemTerm("cripple"));
            Assert.AreEqual("ample", s_stemmer.stemTerm("ample"));
            Assert.AreEqual("battle", s_stemmer.stemTerm("battles"));
            Assert.AreEqual("apostle", s_stemmer.stemTerm("apostles"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for short words ending in "e"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ShortWordsEndingInE()
        {
            Assert.AreEqual("one", s_stemmer.stemTerm("one"));
            Assert.AreEqual("one", s_stemmer.stemTerm("ones"));
            Assert.AreEqual("ire", s_stemmer.stemTerm("ire"));
            Assert.AreEqual("are", s_stemmer.stemTerm("are"));
            Assert.AreEqual("ore", s_stemmer.stemTerm("ore"));
            Assert.AreEqual("ore", s_stemmer.stemTerm("ores"));
            Assert.AreEqual("core", s_stemmer.stemTerm("core"));
            Assert.AreEqual("bare", s_stemmer.stemTerm("bare"));
            Assert.AreEqual("there", s_stemmer.stemTerm("there"));
            Assert.AreEqual("bye", s_stemmer.stemTerm("bye"));
            Assert.AreEqual("lie", s_stemmer.stemTerm("lie"));
        }
	}
}
