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

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "ly"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInLy()
		{
			Assert.AreEqual("real", s_stemmer.stemTerm("really"));
			Assert.AreEqual("final", s_stemmer.stemTerm("finally"));
			Assert.AreEqual("full", s_stemmer.stemTerm("fully"));
			Assert.AreEqual("bliss", s_stemmer.stemTerm("blissfully"));
			Assert.AreEqual("pure", s_stemmer.stemTerm("purely"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "li"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInEli()
		{
			Assert.AreEqual("strang", s_stemmer.stemTerm("strangely")); // "y" gets turned into "i"
			Assert.AreEqual("gingeli", s_stemmer.stemTerm("gingeli"));
			Assert.AreEqual("gingeli", s_stemmer.stemTerm("gingelis"));
			Assert.AreEqual("deli", s_stemmer.stemTerm("deli"));
			Assert.AreEqual("deli", s_stemmer.stemTerm("delis"));
			Assert.AreEqual("obel", s_stemmer.stemTerm("obeli"));
			Assert.AreEqual("Areli", s_stemmer.stemTerm("Areli"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for special-case nouns
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SpecialCaseNouns()
		{
			Assert.AreEqual("morning", s_stemmer.stemTerm("morning"));
			Assert.AreEqual("midmorning", s_stemmer.stemTerm("midmorning"));
			Assert.AreEqual("even", s_stemmer.stemTerm("evening")); // because this could be the verb, rather than the noun
			Assert.AreEqual("olive", s_stemmer.stemTerm("olives"));

			Assert.AreEqual("someone", s_stemmer.stemTerm("someone"));
			Assert.AreEqual("anyone", s_stemmer.stemTerm("anyone"));
			Assert.AreEqual("everyone", s_stemmer.stemTerm("everyone"));
			Assert.AreEqual("someone", s_stemmer.stemTerm("someone's"));
			Assert.AreEqual("anyone", s_stemmer.stemTerm("anyone's"));
			Assert.AreEqual("everyone", s_stemmer.stemTerm("everyone's"));

			Assert.AreEqual("something", s_stemmer.stemTerm("something"));
			Assert.AreEqual("anything", s_stemmer.stemTerm("anything"));
			Assert.AreEqual("everything", s_stemmer.stemTerm("everything"));
			Assert.AreEqual("something", s_stemmer.stemTerm("something's"));
			Assert.AreEqual("anything", s_stemmer.stemTerm("anything's"));
			Assert.AreEqual("everything", s_stemmer.stemTerm("everything's"));

			Assert.AreEqual("somebody", s_stemmer.stemTerm("somebody"));
			Assert.AreEqual("anybody", s_stemmer.stemTerm("anybody"));
			Assert.AreEqual("everybody", s_stemmer.stemTerm("everybody"));
			Assert.AreEqual("somebody", s_stemmer.stemTerm("somebody's"));
			Assert.AreEqual("anybody", s_stemmer.stemTerm("anybody's"));
			Assert.AreEqual("everybody", s_stemmer.stemTerm("everybody's"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for hyphenated nouns. The only stemming that should happen is
		/// to remove plurals and possessives.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void HyphenatedNouns()
		{
			Assert.AreEqual("burial-place", s_stemmer.stemTerm("burial-place"));
			Assert.AreEqual("burial-place", s_stemmer.stemTerm("burial-places"));
			Assert.AreEqual("mid-morning", s_stemmer.stemTerm("mid-morning"));
			Assert.AreEqual("fire-fly", s_stemmer.stemTerm("fire-fly"));
			Assert.AreEqual("fire-fly", s_stemmer.stemTerm("fire-fly's"));
			Assert.AreEqual("fire-fli", s_stemmer.stemTerm("fire-flies"));
			Assert.AreEqual("raisin-cake", s_stemmer.stemTerm("raisin-cake"));
			Assert.AreEqual("raisin-cake", s_stemmer.stemTerm("raisin-cakes"));
			Assert.AreEqual("drink-offering", s_stemmer.stemTerm("drink-offering"));
			Assert.AreEqual("drink-offering", s_stemmer.stemTerm("drink-offerings"));
			Assert.AreEqual("drink-offering", s_stemmer.stemTerm("drink-offerings'"));
			Assert.AreEqual("threshing-sledge", s_stemmer.stemTerm("threshing-sledge"));
			Assert.AreEqual("threshing-sledge", s_stemmer.stemTerm("threshing-sledges"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in us. Most words ending in us are not
		/// plurals, so the rule to remove the pluralizing suffix -s should not be applied in
		/// many situations.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInUs()
		{
			Assert.AreEqual("us", s_stemmer.stemTerm("us"));
			Assert.AreEqual("bus", s_stemmer.stemTerm("bus"));
			Assert.AreEqual("Jesus", s_stemmer.stemTerm("Jesus"));
			Assert.AreEqual("mumu", s_stemmer.stemTerm("mumus"));
			Assert.AreEqual("court", s_stemmer.stemTerm("courteous")); // "court" is actually an archaic root
			Assert.AreEqual("gener", s_stemmer.stemTerm("generous"));
			Assert.AreEqual("blasphem", s_stemmer.stemTerm("blasphemous"));
			Assert.AreEqual("jabiru", s_stemmer.stemTerm("jabirus"));
			Assert.AreEqual("vertu", s_stemmer.stemTerm("vertus"));
			Assert.AreEqual("tutu", s_stemmer.stemTerm("tutus"));
			Assert.AreEqual("Festus", s_stemmer.stemTerm("Festus"));
			Assert.AreEqual("flu", s_stemmer.stemTerm("flus"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ability to stem a word one step at a time, gradually peeling off suffixes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void StagedStemming()
		{
			s_stemmer.StagedStemming = true;
			try
			{
				Assert.AreEqual("renderer", s_stemmer.stemTerm("renderers'"));
				Assert.AreEqual("render", s_stemmer.stemTerm("renderer"));
				Assert.AreEqual("render", s_stemmer.stemTerm("render"));

				Assert.AreEqual("standardization", s_stemmer.stemTerm("standardizations"));
				Assert.AreEqual("standardize", s_stemmer.stemTerm("standardization"));
				Assert.AreEqual("standard", s_stemmer.stemTerm("standardize"));
				Assert.AreEqual("standard", s_stemmer.stemTerm("standard"));

				Assert.AreEqual("strange", s_stemmer.stemTerm("strangely"));
				Assert.AreEqual("strang", s_stemmer.stemTerm("strange"));
				Assert.AreEqual("strang", s_stemmer.stemTerm("strang"));

				Assert.AreEqual("realize", s_stemmer.stemTerm("realizing"));
				Assert.AreEqual("realiz", s_stemmer.stemTerm("realize"));
				Assert.AreEqual("realiz", s_stemmer.stemTerm("realiz"));
			}
			finally 
			{
				s_stemmer.StagedStemming = false;
			}
		}
	}
}
