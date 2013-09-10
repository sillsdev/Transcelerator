// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermRulesTests.cs
// ---------------------------------------------------------------------------------------------
using System.Linq;
using AddInSideViews;
using NUnit.Framework;
using System;
using Rhino.Mocks;
using SIL.Utils;

namespace SIL.Transcelerator
{
	[TestFixture]
    public class KeyTermRulesTests
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
        /// Tests the ability to populate a KeyTermRules object from XML and initialize it.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void KeyTermRules_Initialize_GetsCorrectRules()
		{
            string xmlRules = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<KeyTermRules>" +
                "<KeyTermRule id=\"Ar\" rule=\"MatchForRefOnly\"/>" +
                "<KeyTermRule id=\"ask; pray\" rule=\"MatchForRefOnly\"/>" +
                "<KeyTermRule id=\"(pe quot) say\" rule=\"Exclude\"/>" +
                "<KeyTermRule id=\"bless, praise\">" +
                "  <Alternate name=\"blesses\"/>" +
                "  <Alternate name=\"blessing\"/>" +
                "</KeyTermRule>" +
                "<KeyTermRule id=\"(?&lt;term&gt;.+): \\(\\d+\\).+; \" regex=\"true\"/>" +
                "<KeyTermRule id=\"\\(\\d+\\) (?&lt;term&gt;[^;]+): \\([^;]+\\)\" regex=\"true\"/>" +
                "</KeyTermRules>";
            KeyTermRules rules = XmlSerializationHelper.DeserializeFromString<KeyTermRules>(xmlRules);
            rules.Initialize();
            Assert.AreEqual(2, rules.RegexRules.Count());
            Assert.IsTrue(rules.RegexRules.Any(r => r.ToString() == "(?<term>.+): \\(\\d+\\).+; "));
            Assert.IsTrue(rules.RegexRules.Any(r => r.ToString() == "\\(\\d+\\) (?<term>[^;]+): \\([^;]+\\)"));
            Assert.IsTrue(rules.RulesDictionary["ar"].Rule == KeyTermRule.RuleType.MatchForRefOnly);
            KeyTermRule rule;
            Assert.IsTrue(rules.RulesDictionary.TryGetValue("ask; pray", out rule));
		}
	}
}
