﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermMatchBuilderTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Paratext.PluginInterfaces;
using Rhino.Mocks;
using SIL.ObjectModel;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class KeyTermMatchBuilderTests
	{
		#region Sanitized data Tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the simple case of a key term consisting of
		/// a single required word with no optional parts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SingleWordKeyTerm()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("fun"));
			Assert.AreEqual(1, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "fun");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term consisting of
		/// two required words with no optional parts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TwoWordKeyTerm()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("fun stuff"));
			Assert.AreEqual(1, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "fun", "stuff");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term consisting of
		/// a verb with the implicitly optional word "to".
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TwoWordKeyTermWithImplicitOptionalInfinitiveMarker()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to cry"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "cry");
			VerifyKeyTermMatch(bldr, 1, "to", "cry");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with an optional
		/// leading word.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalLeadingWord()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(fun) stuff"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "stuff");
			VerifyKeyTermMatch(bldr, 1, "fun", "stuff");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with an optional
		/// middle word.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalMiddleWord()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("really (fun) stuff"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "really", "stuff");
			VerifyKeyTermMatch(bldr, 1, "really", "fun", "stuff");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with an optional
		/// trailing word.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalTrailingWord()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("morning (star)"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "morning");
			VerifyKeyTermMatch(bldr, 1, "morning", "star");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with an optional
		/// leading phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalPhrase()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(things of this) life"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "life");
			VerifyKeyTermMatch(bldr, 1, "things", "of", "this", "life");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with a single required
		/// word with an optional initial part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalInitialPart()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(loving)kindness"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "kindness");
			VerifyKeyTermMatch(bldr, 1, "lovingkindness");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with a single required
		/// word with an optional middle part. (It's unlikely that there is any actual data like
		/// this.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalMiddlePart()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("anti(dis)establishment"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "antiestablishment");
			VerifyKeyTermMatch(bldr, 1, "antidisestablishment");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with a single required
		/// word with an optional final part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalFinalPart()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("kind(ness)"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "kind");
			VerifyKeyTermMatch(bldr, 1, "kindness");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term with a single required
		/// word with an optional final part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void OptionalFinal()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("kind(ness)"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "kind");
			VerifyKeyTermMatch(bldr, 1, "kindness");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term where "or" separates
		/// two three-word phrases, with more text preceding
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ThreeWordPhrasesSeparatedWithOr()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to remove the state of guilt or uncleanness from oneself"));
			Assert.AreEqual(4, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, true, "remove", "the", "state", "of", "guilt");
			VerifyKeyTermMatch(bldr, 1, "to", "remove", "the", "state", "of", "guilt");
			VerifyKeyTermMatch(bldr, 2, "remove", "the", "uncleanness", "from", "oneself");
			VerifyKeyTermMatch(bldr, 3, "to", "remove", "the", "uncleanness", "from", "oneself");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a complex multi-word key term
		/// consisting of weird optional parts and phrases.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void NastyBeyondBelief()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to (have) beg(ged) for (loving)kindness (and mercy)"));
			Assert.AreEqual(32, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "beg", "for", "kindness");
			VerifyKeyTermMatch(bldr, 1, "to", "beg", "for", "kindness");
			VerifyKeyTermMatch(bldr, 2, "have", "beg", "for", "kindness");
			VerifyKeyTermMatch(bldr, 3, "to", "have", "beg", "for", "kindness");
			VerifyKeyTermMatch(bldr, 4, "begged", "for", "kindness");
			VerifyKeyTermMatch(bldr, 5, "to", "begged", "for", "kindness");
			VerifyKeyTermMatch(bldr, 6, "have", "begged", "for", "kindness");
			VerifyKeyTermMatch(bldr, 7, "to", "have", "begged", "for", "kindness");
			VerifyKeyTermMatch(bldr, 8, "beg", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 9, "to", "beg", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 10, "have", "beg", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 11, "to", "have", "beg", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 12, "begged", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 13, "to", "begged", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 14, "have", "begged", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 15, "to", "have", "begged", "for", "lovingkindness");
			VerifyKeyTermMatch(bldr, 16, "beg", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 17, "to", "beg", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 18, "have", "beg", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 19, "to", "have", "beg", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 20, "begged", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 21, "to", "begged", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 22, "have", "begged", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 23, "to", "have", "begged", "for", "kindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 24, "beg", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 25, "to", "beg", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 26, "have", "beg", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 27, "to", "have", "beg", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 28, "begged", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 29, "to", "begged", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 30, "have", "begged", "for", "lovingkindness", "and", "mercy");
			VerifyKeyTermMatch(bldr, 31, "to", "have", "begged", "for", "lovingkindness", "and", "mercy");
		}
		#endregion

		#region Rules tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
		/// include both the original term and add an alternate.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RuleToKeepOriginalTermAndAddAnAlternate()
		{
			Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
			KeyTermRule rule = new KeyTermRule();
			rule.id = "Jesus";
			rule.Alternates = new [] {new KeyTermRulesKeyTermRuleAlternate()};
			rule.Alternates[0].Name = "Jesus Christ";
			rules[rule.id] = rule;
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("Jesus"),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "jesus", "christ");
			VerifyKeyTermMatch(bldr, 1, "jesus");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
		/// exclude it (no alternates).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RuleToExcludeTermCompletely()
		{
			Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
			KeyTermRule rule = new KeyTermRule();
			rule.id = "Jesus";
			rule.Rule = KeyTermRule.RuleType.Exclude;
			rules[rule.id] = rule;
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("Jesus"),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
			Assert.AreEqual(0, bldr.Matches.Count());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
		/// restrict it to match only to certain references.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RuleToLimitMatchToTermRefs()
		{
			Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
			KeyTermRule rule = new KeyTermRule();
			rule.id = "ask";
			rule.Rule = KeyTermRule.RuleType.MatchForRefOnly;
			rules[rule.id] = rule;
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm(rule.id, 34),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
			Assert.AreEqual(1, bldr.Matches.Count());
			KeyTermMatch ktm = VerifyKeyTermMatch(bldr, 0, false, "ask");
			Assert.IsFalse(ktm.AppliesTo(30, 33));
			Assert.IsTrue(ktm.AppliesTo(34, 34));
			Assert.IsFalse(ktm.AppliesTo(35, 39));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
		/// exclude it, using alternates instead.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RuleToReplaceOriginalTermWithAlternates_Basic()
		{
			Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
			KeyTermRule rule = new KeyTermRule();
			rule.id = "to lift up (one's hand, heart, or soul) = to worship, pray";
			rule.Rule = KeyTermRule.RuleType.Exclude;
			rule.Alternates = new[] { new KeyTermRulesKeyTermRuleAlternate(), new KeyTermRulesKeyTermRuleAlternate(), new KeyTermRulesKeyTermRuleAlternate() };
			rule.Alternates[0].Name = "worship";
			rule.Alternates[1].Name = "praise exuberantly";
			rule.Alternates[2].Name = "pray";
			rules[rule.id] = rule;
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm(rule.id),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
			Assert.AreEqual(3, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "worship");
			VerifyKeyTermMatch(bldr, 1, "praise", "exuberantly");
			VerifyKeyTermMatch(bldr, 2, "pray");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
        /// exclude it, using alternates instead. One of the alternates has the matchForRefOnly
        /// attribute set to true.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RuleToReplaceOriginalTermWithAlternates_AltWithMatchForRefOnly()
        {
            Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
            KeyTermRule rule = new KeyTermRule();
            rule.id = "ask; pray";
            rule.Rule = KeyTermRule.RuleType.Exclude;
            rule.Alternates = new[] { new KeyTermRulesKeyTermRuleAlternate(), new KeyTermRulesKeyTermRuleAlternate() };
            rule.Alternates[0].Name = "ask";
            rule.Alternates[0].MatchForRefOnly = true;
            rule.Alternates[1].Name = "pray";
            rules[rule.id] = rule;
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm(rule.id),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
            Assert.AreEqual(2, bldr.Matches.Count());
            VerifyKeyTermMatch(bldr, 0, false, "ask");
            VerifyKeyTermMatch(bldr, 1, true, "pray");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a term which has a rule to
        /// exclude it, using alternates instead. Ensure that each alternate is used exactly as
        /// it is to create a single match, not parsed for internal "or"s or leading "to"s.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RuleToReplaceOriginalTermWithAlternates_PreventFurtherParsingOfAlts()
        {
            Dictionary<string, KeyTermRule> rules = new Dictionary<string, KeyTermRule>();
            KeyTermRule rule = new KeyTermRule();
            rule.id = "fast; fasting";
            rule.Rule = KeyTermRule.RuleType.Exclude;
            rule.Alternates = new[] { new KeyTermRulesKeyTermRuleAlternate(), new KeyTermRulesKeyTermRuleAlternate() };
            rule.Alternates[0].Name = "to fast";
            rule.Alternates[1].Name = "fast or pray";
            rules[rule.id] = rule;
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm(rule.id),
                new ReadOnlyDictionary<string, KeyTermRule>(rules));
            Assert.AreEqual(2, bldr.Matches.Count());
            VerifyKeyTermMatch(bldr, 0, true, "to", "fast");
            VerifyKeyTermMatch(bldr, 1, true, "fast", "or", "pray");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a term which has a regular-
        /// expression-based rule to exclude it.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RuleToExcludeTermBasedOnRegularExpression()
        {
            Regex regexRule = new Regex(@".*\(div1 type=", RegexOptions.Compiled);
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(
                AddMockedKeyTerm("(qal) crush; (pi) crush to pieces; (pu) be crushed to pieces; (hi) scatter; (ho) be crushed to pieces, be scattered.(div1 type=letter id=LET.12)(head)ל"), null,
                new[] { regexRule });
            Assert.AreEqual(0, bldr.Matches.Count());

            bldr = new KeyTermMatchBuilder(
                 AddMockedKeyTerm("(qal) crumble.(div1 type=letter id=LET.18)(head)ץ"), null,
                 new[] { regexRule });
            Assert.AreEqual(0, bldr.Matches.Count());
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a term which has a regular-
        /// expression-based rule to extract terms.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RuleToExtractMatchesBasedOnRegularExpression()
        {
            Regex regexSingleProperName = new Regex(@"(?<term>.+): \(\d+\).+; ", RegexOptions.Compiled);
            Regex regexMultipleProperNames = new Regex(@"\(\d+\) (?<term>[^;]+): \([^;]+\)", RegexOptions.Compiled);
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(
                AddMockedKeyTerm("(1) Judah: (a) son of Jacob, his tribe, his territory; (b)" +
                " person in the genealogy of Jesus; (2) Judas: (a) the betrayer of Jesus; (b)" +
                " a brother of Jesus; (c) an apostle, the son of James; (d) member of the" +
                " Jerusalem church, called Barsabbas; (3) a disciple in Damascus; (f)" +
                " revolutionary leader"), null,
                new[] { regexSingleProperName, regexMultipleProperNames });
            Assert.AreEqual(2, bldr.Matches.Count());

            bldr = new KeyTermMatchBuilder(
                 AddMockedKeyTerm("Barsabbas: (1) Joseph; (2) Judas"), null,
                 new[] { regexSingleProperName, regexMultipleProperNames });
            Assert.AreEqual(1, bldr.Matches.Count());
        }
		#endregion

		#region Really hard Real Data tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData1()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("worm, maggot"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "worm");
			VerifyKeyTermMatch(bldr, 1, "maggot");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData2()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("castor oil plant (FF 106, 107)"));
			Assert.LessOrEqual(1, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "castor", "oil", "plant");
			// Ideally, we don't want to get anything for the junk in parentheses, but it
			// shouldn't really hurt anything, so we'll live with it.
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData3()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(loving)kindness, solidarity, joint liability, grace"));
			Assert.AreEqual(5, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "kindness");
			VerifyKeyTermMatch(bldr, 1, "lovingkindness");
			VerifyKeyTermMatch(bldr, 2, "solidarity");
			VerifyKeyTermMatch(bldr, 3, "joint", "liability");
			VerifyKeyTermMatch(bldr, 4, "grace");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData4()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("Canaanean = Zealot"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "canaanean");
			VerifyKeyTermMatch(bldr, 1, "zealot");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data. This is a term whose "gloss" is so long that the matcher gets
		/// discarded because in real life we never manage to match a term over 6 words.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData5()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("dreadful event or sight"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "dreadful", "event");
			VerifyKeyTermMatch(bldr, 1, "dreadful", "sight");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData6()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("exempt, free from"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "exempt");
			VerifyKeyTermMatch(bldr, 1, "free", "from");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData7_ExceedsMaxWordCount()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("someone who sins against someone else and therefore 'owes' that person"));
			Assert.AreEqual(0, bldr.Matches.Count());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData8()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("state of fearing, standing in awe"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "state", "of", "fearing");
			VerifyKeyTermMatch(bldr, 1, "standing", "in", "awe");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData9()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to be favorably disposed to someone, or to experience an emotion of compassion towards other people"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "be", "favorably", "disposed", "to", "someone");
			VerifyKeyTermMatch(bldr, 1, "to", "be", "favorably", "disposed", "to", "someone");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData10()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to recompense, to reward, to pay"));
			Assert.AreEqual(6, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "recompense");
			VerifyKeyTermMatch(bldr, 1, "to", "recompense");
			VerifyKeyTermMatch(bldr, 2, "reward");
			VerifyKeyTermMatch(bldr, 3, "to", "reward");
			VerifyKeyTermMatch(bldr, 4, "pay");
			VerifyKeyTermMatch(bldr, 5, "to", "pay");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: complex insanity.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[Ignore("We're still trying to figure out what to do with this.")]
		public void RealData11()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to lift up (one's hand, heart, or soul) = to worship, pray"));
			Assert.AreEqual(7, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "lift", "up");
			VerifyKeyTermMatch(bldr, 1, "to", "lift", "up");
			VerifyKeyTermMatch(bldr, 2, "lift", "up", "one's", "hand", "heart", "soul");
			VerifyKeyTermMatch(bldr, 3, "to", "lift", "up", "one's", "hand", "heart", "soul");
			VerifyKeyTermMatch(bldr, 4, "worship");
			VerifyKeyTermMatch(bldr, 5, "to", "worship");
			VerifyKeyTermMatch(bldr, 6, "pray");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: missing closing parenthesis for optional phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData12()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("olive oil (used as food"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "olive", "oil");
			VerifyKeyTermMatch(bldr, 1, "olive", "oil", "used", "as", "food");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: optional comma-separated parts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[Ignore("We're still trying to figure out what to do with this.")]
		public void RealData13()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(Lord, LORD, God of) hosts"));
			Assert.AreEqual(3, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "hosts");
			VerifyKeyTermMatch(bldr, 1, "lord", "of", "hosts");
			VerifyKeyTermMatch(bldr, 2, "god", "of", "hosts");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
        /// real evil data: optional comma-separated parts.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RealData14()
        {
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("(one's own) burial-place"));
            Assert.AreEqual(2, bldr.Matches.Count());
            VerifyKeyTermMatch(bldr, 0, "burial-place");
            VerifyKeyTermMatch(bldr, 1, "one's", "own", "burial-place");
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
        /// real evil data: parentheses.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void RealData15()
        {
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("bridal chamber (marriage bed)"));
            Assert.AreEqual(2, bldr.Matches.Count());
            VerifyKeyTermMatch(bldr, 0, "bridal", "chamber");
            VerifyKeyTermMatch(bldr, 1, "bridal", "chamber", "marriage", "bed");
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealData16()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("laurustinus; or, pine tree"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "laurustinus");
			VerifyKeyTermMatch(bldr, 1, "pine", "tree");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: "or" separating two words
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealDataWithOr1()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("courtyard or sheepfold"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "courtyard");
			VerifyKeyTermMatch(bldr, 1, "sheepfold");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: "or" separating two two-word phrases, with more text following
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealDataWithOr2()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("give up or lay aside what one possesses"));
			Assert.AreEqual(2, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "give", "up", "what", "one", "possesses");
			VerifyKeyTermMatch(bldr, 1, "lay", "aside", "what", "one", "possesses");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: "or" separating two three-word phrases, with more text preceeding
		/// Result is too long and is discarded.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealDataWithOr3()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("to perform the ritual of removing the state of guilt or uncleanness from oneself"));
			Assert.AreEqual(0, bldr.Matches.Count());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: "or" separating two three-word phrases, with more text preceeding.
		/// Results are all too long and get discarded.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealDataWithOr4()
		{
			KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("and the flowers are white or pink. The whole plant gives off an agreeable odour"));
			Assert.AreEqual(0, bldr.Matches.Count());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the KeyTermMatchBuilder class in the case of a key term from the world of
		/// real evil data: "or" separating two three-word phrases, with more text preceeding
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void RealDataWithEmptyParentheses()
		{
            KeyTermMatchBuilder bldr = new KeyTermMatchBuilder(AddMockedKeyTerm("receive, welcome; pay attention to, recognize ()"));
			Assert.AreEqual(4, bldr.Matches.Count());
			VerifyKeyTermMatch(bldr, 0, "receive");
            VerifyKeyTermMatch(bldr, 1, "welcome");
            VerifyKeyTermMatch(bldr, 2, "pay", "attention", "to");
            VerifyKeyTermMatch(bldr, 3, "recognize");
        }
		#endregion

		#region private helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the mocked key term.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal static IBiblicalTerm AddMockedKeyTerm(string term, params int[] occurrences)
		{
			IBiblicalTerm mockedKt = MockRepository.GenerateStub<IBiblicalTerm>();
			mockedKt.Stub(kt => kt.Gloss(Arg<string>.Is.Anything)).Return(term);
			mockedKt.Stub(kt => kt.Occurrences).Return(occurrences.Length > 0 ? occurrences.Select(o => (IVerseRef)new BcvRefIVerseAdapter(new BCVRef(o))).ToList() : new List<IVerseRef>());
		    mockedKt.Stub(kt => kt.Lemma).Return(new string(term.Reverse().ToArray()));
			return mockedKt;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Verifies the key term match.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void VerifyKeyTermMatch(KeyTermMatchBuilder bldr, int iMatch,
			params string[] words)
		{
			VerifyKeyTermMatch(bldr, iMatch, true, words);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Verifies the key term match.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static KeyTermMatch VerifyKeyTermMatch(KeyTermMatchBuilder bldr, int iMatch,
			bool matchAnywhere, params string[] words)
		{
			KeyTermMatch ktm = bldr.Matches.ElementAt(iMatch);
			Assert.AreEqual(words.Length, ktm.WordCount);
			for (int i = 0; i < words.Length; i++)
				Assert.AreEqual(words[i], ktm[i].Text);
            Assert.IsTrue(ktm.MatchForRefOnly != matchAnywhere);
            // The following is really a test of the KeyTermMatch.AppliesTo method:
			if (matchAnywhere)
			{
				Random r = new Random(DateTime.Now.Millisecond);
				Assert.IsTrue(ktm.AppliesTo(r.Next(), r.Next()));
			}
			return ktm;
		}
		#endregion
	}
}
