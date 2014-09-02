// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International.
// <copyright from='2012' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: EnglishStemmerTests.cs
// ---------------------------------------------------------------------------------------------
using NUnit.Framework;

namespace SIL.Stemmers
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TsStringUtils tests.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class EnglishStemmerTests
	{
		private static IStemmer s_stemmer = new EnglishStemmer();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for word ending in "-ying"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void YingSuffix()
		{
			Assert.AreEqual("pray", s_stemmer.Stem("praying"));
			Assert.AreEqual("buy", s_stemmer.Stem("buying"));
			Assert.AreEqual("ly", s_stemmer.Stem("lying"));
			Assert.AreEqual("obey", s_stemmer.Stem("obeying"));
			Assert.AreEqual("repli", s_stemmer.Stem("replying"));
			Assert.AreEqual("studi", s_stemmer.Stem("studying"));
			Assert.AreEqual("tidi", s_stemmer.Stem("tidying"));
			Assert.AreEqual("ying", s_stemmer.Stem("ying"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for word ending in "-le"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void LeSuffix()
        {
            Assert.AreEqual("disciple", s_stemmer.Stem("disciples"));
            Assert.AreEqual("temple", s_stemmer.Stem("temples"));
            Assert.AreEqual("people", s_stemmer.Stem("people"));
            Assert.AreEqual("cripple", s_stemmer.Stem("cripple"));
            Assert.AreEqual("ample", s_stemmer.Stem("ample"));
            Assert.AreEqual("battle", s_stemmer.Stem("battles"));
            Assert.AreEqual("apostle", s_stemmer.Stem("apostles"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for short words ending in "e"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ShortWordsEndingInE()
        {
            Assert.AreEqual("one", s_stemmer.Stem("one"));
            Assert.AreEqual("one", s_stemmer.Stem("ones"));
            Assert.AreEqual("ire", s_stemmer.Stem("ire"));
            Assert.AreEqual("are", s_stemmer.Stem("are"));
            Assert.AreEqual("ore", s_stemmer.Stem("ore"));
            Assert.AreEqual("ore", s_stemmer.Stem("ores"));
            Assert.AreEqual("core", s_stemmer.Stem("core"));
            Assert.AreEqual("bare", s_stemmer.Stem("bare"));
            Assert.AreEqual("there", s_stemmer.Stem("there"));
            Assert.AreEqual("bye", s_stemmer.Stem("bye"));
            Assert.AreEqual("lie", s_stemmer.Stem("lie"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "ly"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInLy()
		{
			Assert.AreEqual("real", s_stemmer.Stem("really"));
			Assert.AreEqual("final", s_stemmer.Stem("finally"));
			Assert.AreEqual("full", s_stemmer.Stem("fully"));
			Assert.AreEqual("bliss", s_stemmer.Stem("blissfully"));
			Assert.AreEqual("pure", s_stemmer.Stem("purely"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "li"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInEli()
		{
			Assert.AreEqual("strang", s_stemmer.Stem("strangely")); // "y" gets turned into "i"
			Assert.AreEqual("gingeli", s_stemmer.Stem("gingeli"));
			Assert.AreEqual("gingeli", s_stemmer.Stem("gingelis"));
			Assert.AreEqual("deli", s_stemmer.Stem("deli"));
			Assert.AreEqual("deli", s_stemmer.Stem("delis"));
			Assert.AreEqual("obel", s_stemmer.Stem("obeli"));
			Assert.AreEqual("Areli", s_stemmer.Stem("Areli"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for special-case nouns
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SpecialCaseNouns()
		{
			Assert.AreEqual("morning", s_stemmer.Stem("morning"));
			Assert.AreEqual("midmorning", s_stemmer.Stem("midmorning"));
			Assert.AreEqual("even", s_stemmer.Stem("evening")); // because this could be the verb, rather than the noun
			Assert.AreEqual("olive", s_stemmer.Stem("olives"));

			Assert.AreEqual("someone", s_stemmer.Stem("someone"));
			Assert.AreEqual("anyone", s_stemmer.Stem("anyone"));
			Assert.AreEqual("everyone", s_stemmer.Stem("everyone"));
			Assert.AreEqual("someone", s_stemmer.Stem("someone's"));
			Assert.AreEqual("anyone", s_stemmer.Stem("anyone's"));
			Assert.AreEqual("everyone", s_stemmer.Stem("everyone's"));

			Assert.AreEqual("something", s_stemmer.Stem("something"));
			Assert.AreEqual("anything", s_stemmer.Stem("anything"));
			Assert.AreEqual("everything", s_stemmer.Stem("everything"));
			Assert.AreEqual("something", s_stemmer.Stem("something's"));
			Assert.AreEqual("anything", s_stemmer.Stem("anything's"));
			Assert.AreEqual("everything", s_stemmer.Stem("everything's"));

			Assert.AreEqual("somebody", s_stemmer.Stem("somebody"));
			Assert.AreEqual("anybody", s_stemmer.Stem("anybody"));
			Assert.AreEqual("everybody", s_stemmer.Stem("everybody"));
			Assert.AreEqual("somebody", s_stemmer.Stem("somebody's"));
			Assert.AreEqual("anybody", s_stemmer.Stem("anybody's"));
			Assert.AreEqual("everybody", s_stemmer.Stem("everybody's"));
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
			Assert.AreEqual("burial-place", s_stemmer.Stem("burial-place"));
			Assert.AreEqual("burial-place", s_stemmer.Stem("burial-places"));
			Assert.AreEqual("mid-morning", s_stemmer.Stem("mid-morning"));
			Assert.AreEqual("fire-fly", s_stemmer.Stem("fire-fly"));
			Assert.AreEqual("fire-fly", s_stemmer.Stem("fire-fly's"));
			Assert.AreEqual("fire-fly", s_stemmer.Stem("fire-flies"));
			Assert.AreEqual("raisin-cake", s_stemmer.Stem("raisin-cake"));
			Assert.AreEqual("raisin-cake", s_stemmer.Stem("raisin-cakes"));
			Assert.AreEqual("drink-offering", s_stemmer.Stem("drink-offering"));
			Assert.AreEqual("drink-offering", s_stemmer.Stem("drink-offerings"));
			Assert.AreEqual("drink-offering", s_stemmer.Stem("drink-offerings'"));
			Assert.AreEqual("threshing-sledge", s_stemmer.Stem("threshing-sledge"));
			Assert.AreEqual("threshing-sledge", s_stemmer.Stem("threshing-sledges"));
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
			Assert.AreEqual("us", s_stemmer.Stem("us"));
			Assert.AreEqual("bus", s_stemmer.Stem("bus"));
			Assert.AreEqual("Jesus", s_stemmer.Stem("Jesus"));
			Assert.AreEqual("mumu", s_stemmer.Stem("mumus"));
			Assert.AreEqual("court", s_stemmer.Stem("courteous")); // "court" is actually an archaic root
			Assert.AreEqual("gener", s_stemmer.Stem("generous"));
			Assert.AreEqual("blasphem", s_stemmer.Stem("blasphemous"));
			Assert.AreEqual("jabiru", s_stemmer.Stem("jabirus"));
			Assert.AreEqual("vertu", s_stemmer.Stem("vertus"));
			Assert.AreEqual("tutu", s_stemmer.Stem("tutus"));
			Assert.AreEqual("Festus", s_stemmer.Stem("Festus"));
			Assert.AreEqual("flu", s_stemmer.Stem("flus"));
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
				Assert.AreEqual("renderer", s_stemmer.Stem("renderers'"));
				Assert.AreEqual("render", s_stemmer.Stem("renderer"));
				Assert.AreEqual("render", s_stemmer.Stem("render"));

				Assert.AreEqual("standardization", s_stemmer.Stem("standardizations"));
				Assert.AreEqual("standardize", s_stemmer.Stem("standardization"));
				Assert.AreEqual("standard", s_stemmer.Stem("standardize"));
				Assert.AreEqual("standard", s_stemmer.Stem("standard"));

				Assert.AreEqual("strange", s_stemmer.Stem("strangely"));
				Assert.AreEqual("strang", s_stemmer.Stem("strange"));
				Assert.AreEqual("strang", s_stemmer.Stem("strang"));

				Assert.AreEqual("realize", s_stemmer.Stem("realizing"));
				Assert.AreEqual("realiz", s_stemmer.Stem("realize"));
				Assert.AreEqual("realiz", s_stemmer.Stem("realiz"));
			}
			finally
			{
				s_stemmer.StagedStemming = false;
			}
		}
	}
}
