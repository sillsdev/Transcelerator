// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2012' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
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
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for word ending in "-ying"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void YingSuffix()
		{
			Assert.AreEqual("pray", PorterStemmer.StemTerm("praying"));
			Assert.AreEqual("buy", PorterStemmer.StemTerm("buying"));
			Assert.AreEqual("ly", PorterStemmer.StemTerm("lying"));
			Assert.AreEqual("obey", PorterStemmer.StemTerm("obeying"));
			Assert.AreEqual("repli", PorterStemmer.StemTerm("replying"));
			Assert.AreEqual("studi", PorterStemmer.StemTerm("studying"));
			Assert.AreEqual("tidi", PorterStemmer.StemTerm("tidying"));
			Assert.AreEqual("ying", PorterStemmer.StemTerm("ying"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for word ending in "-le"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void LeSuffix()
        {
            Assert.AreEqual("disciple", PorterStemmer.StemTerm("disciples"));
            Assert.AreEqual("temple", PorterStemmer.StemTerm("temples"));
            Assert.AreEqual("people", PorterStemmer.StemTerm("people"));
            Assert.AreEqual("cripple", PorterStemmer.StemTerm("cripple"));
            Assert.AreEqual("ample", PorterStemmer.StemTerm("ample"));
            Assert.AreEqual("battle", PorterStemmer.StemTerm("battles"));
            Assert.AreEqual("apostle", PorterStemmer.StemTerm("apostles"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the Stem method for short words ending in "e"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void ShortWordsEndingInE()
        {
            Assert.AreEqual("one", PorterStemmer.StemTerm("one"));
            Assert.AreEqual("one", PorterStemmer.StemTerm("ones"));
            Assert.AreEqual("ire", PorterStemmer.StemTerm("ire"));
            Assert.AreEqual("are", PorterStemmer.StemTerm("are"));
            Assert.AreEqual("ore", PorterStemmer.StemTerm("ore"));
            Assert.AreEqual("ore", PorterStemmer.StemTerm("ores"));
            Assert.AreEqual("core", PorterStemmer.StemTerm("core"));
            Assert.AreEqual("bare", PorterStemmer.StemTerm("bare"));
            Assert.AreEqual("there", PorterStemmer.StemTerm("there"));
            Assert.AreEqual("bye", PorterStemmer.StemTerm("bye"));
	        Assert.AreEqual("lie", PorterStemmer.StemTerm("lie"));
	        Assert.AreEqual("doe", PorterStemmer.StemTerm("doe"));
			// The stem of "does" could be either "do" or "doe". Although "do" is the more likely one,
			// it is better to have it stem to "doe" because we will never have "do" as a ley term. 
	        //Assert.AreEqual("do", PorterStemmer.StemTerm("does"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "ly"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInLy()
		{
			Assert.AreEqual("real", PorterStemmer.StemTerm("really"));
			Assert.AreEqual("final", PorterStemmer.StemTerm("finally"));
			Assert.AreEqual("full", PorterStemmer.StemTerm("fully"));
			Assert.AreEqual("bliss", PorterStemmer.StemTerm("blissfully"));
			Assert.AreEqual("pure", PorterStemmer.StemTerm("purely"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for words ending in "li"
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void WordsEndingInEli()
		{
			Assert.AreEqual("strang", PorterStemmer.StemTerm("strangely")); // "y" gets turned into "i"
			Assert.AreEqual("gingeli", PorterStemmer.StemTerm("gingeli"));
			Assert.AreEqual("gingeli", PorterStemmer.StemTerm("gingelis"));
			Assert.AreEqual("deli", PorterStemmer.StemTerm("deli"));
			Assert.AreEqual("deli", PorterStemmer.StemTerm("delis"));
			Assert.AreEqual("obel", PorterStemmer.StemTerm("obeli"));
			Assert.AreEqual("Areli", PorterStemmer.StemTerm("Areli"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the Stem method for special-case nouns
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SpecialCaseNouns()
		{
			Assert.AreEqual("morning", PorterStemmer.StemTerm("morning"));
			Assert.AreEqual("midmorning", PorterStemmer.StemTerm("midmorning"));
			Assert.AreEqual("even", PorterStemmer.StemTerm("evening")); // because this could be the verb, rather than the noun
			Assert.AreEqual("olive", PorterStemmer.StemTerm("olives"));

			Assert.AreEqual("someone", PorterStemmer.StemTerm("someone"));
			Assert.AreEqual("anyone", PorterStemmer.StemTerm("anyone"));
			Assert.AreEqual("everyone", PorterStemmer.StemTerm("everyone"));
			Assert.AreEqual("someone", PorterStemmer.StemTerm("someone's"));
			Assert.AreEqual("anyone", PorterStemmer.StemTerm("anyone's"));
			Assert.AreEqual("everyone", PorterStemmer.StemTerm("everyone's"));

			Assert.AreEqual("something", PorterStemmer.StemTerm("something"));
			Assert.AreEqual("anything", PorterStemmer.StemTerm("anything"));
			Assert.AreEqual("everything", PorterStemmer.StemTerm("everything"));
			Assert.AreEqual("something", PorterStemmer.StemTerm("something's"));
			Assert.AreEqual("anything", PorterStemmer.StemTerm("anything's"));
			Assert.AreEqual("everything", PorterStemmer.StemTerm("everything's"));

			Assert.AreEqual("somebody", PorterStemmer.StemTerm("somebody"));
			Assert.AreEqual("anybody", PorterStemmer.StemTerm("anybody"));
			Assert.AreEqual("everybody", PorterStemmer.StemTerm("everybody"));
			Assert.AreEqual("somebody", PorterStemmer.StemTerm("somebody's"));
			Assert.AreEqual("anybody", PorterStemmer.StemTerm("anybody's"));
			Assert.AreEqual("everybody", PorterStemmer.StemTerm("everybody's"));
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
			Assert.AreEqual("burial-place", PorterStemmer.StemTerm("burial-place"));
			Assert.AreEqual("burial-place", PorterStemmer.StemTerm("burial-places"));
			Assert.AreEqual("mid-morning", PorterStemmer.StemTerm("mid-morning"));
			Assert.AreEqual("fire-fly", PorterStemmer.StemTerm("fire-fly"));
			Assert.AreEqual("fire-fly", PorterStemmer.StemTerm("fire-fly's"));
			Assert.AreEqual("fire-fly", PorterStemmer.StemTerm("fire-flies"));
			Assert.AreEqual("raisin-cake", PorterStemmer.StemTerm("raisin-cake"));
			Assert.AreEqual("raisin-cake", PorterStemmer.StemTerm("raisin-cakes"));
			Assert.AreEqual("drink-offering", PorterStemmer.StemTerm("drink-offering"));
			Assert.AreEqual("drink-offering", PorterStemmer.StemTerm("drink-offerings"));
			Assert.AreEqual("drink-offering", PorterStemmer.StemTerm("drink-offerings'"));
			Assert.AreEqual("threshing-sledge", PorterStemmer.StemTerm("threshing-sledge"));
			Assert.AreEqual("threshing-sledge", PorterStemmer.StemTerm("threshing-sledges"));
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
			Assert.AreEqual("us", PorterStemmer.StemTerm("us"));
			Assert.AreEqual("bus", PorterStemmer.StemTerm("bus"));
			Assert.AreEqual("Jesus", PorterStemmer.StemTerm("Jesus"));
			Assert.AreEqual("mumu", PorterStemmer.StemTerm("mumus"));
			Assert.AreEqual("court", PorterStemmer.StemTerm("courteous")); // "court" is actually an archaic root
			Assert.AreEqual("gener", PorterStemmer.StemTerm("generous"));
			Assert.AreEqual("blasphem", PorterStemmer.StemTerm("blasphemous"));
			Assert.AreEqual("jabiru", PorterStemmer.StemTerm("jabirus"));
			Assert.AreEqual("vertu", PorterStemmer.StemTerm("vertus"));
			Assert.AreEqual("tutu", PorterStemmer.StemTerm("tutus"));
			Assert.AreEqual("Festus", PorterStemmer.StemTerm("Festus"));
			Assert.AreEqual("flu", PorterStemmer.StemTerm("flus"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ability to stem a word one step at a time, gradually peeling off suffixes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void StagedStemming()
		{
			Assert.That(PorterStemmer.GetStemmedForms("renderers'"),
				Is.EquivalentTo(new [] {"renderer", "render"}));

			Assert.That(PorterStemmer.GetStemmedForms("standardizations"),
				Is.EquivalentTo(new [] {"standardization", "standardize", "standard"}));

			Assert.That(PorterStemmer.GetStemmedForms("strangely"),
				Is.EquivalentTo(new [] {"strange", "strang"}));

			Assert.That(PorterStemmer.GetStemmedForms("realizing"),
				Is.EquivalentTo(new [] {"realize", "realiz"}));

			Assert.That(PorterStemmer.GetStemmedForms("nobody"), Is.Empty);

		}
	}
}
