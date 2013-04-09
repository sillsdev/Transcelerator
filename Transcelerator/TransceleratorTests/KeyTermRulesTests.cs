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
        /// Tests the ability to populate a KeyTermRules object from XML and initialize it based
        /// on a named key-terms list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void KeyTermRules_Initialize_GetsCorrectRules()
		{
            string xmlRules = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<KeyTermRules>" +
                "<KeyTermRule id=\"Ar\" rule=\"MatchForRefOnly\"/>" +
                "<KeyTermRule id=\"ask; pray\" rule=\"MatchForRefOnly\" appliesTo=\"Simple Biblical Terms\"/>" +
                "<KeyTermRule id=\"(pe quot) say\" rule=\"Exclude\" appliesTo=\"Advanced Biblical Terms\"/>" +
                "<KeyTermRule id=\"bless, praise\">" +
                "  <Alternate name=\"blesses\"/>" +
                "  <Alternate name=\"blessing\"/>" +
                "</KeyTermRule>" +
                "<KeyTermRule id=\"(?&lt;term&gt;.+): \\(\\d+\\).+; \" regex=\"true\"/>" +
                "<KeyTermRule id=\"\\(\\d+\\) (?&lt;term&gt;[^;]+): \\([^;]+\\)\" regex=\"true\" appliesTo=\"Advanced Biblical Terms\"/>" +
                "<KeyTermRule id=\"(?&lt;term&gt;.+): \\(\\d+\\).+; \" regex=\"true\" appliesTo=\"Simple Biblical Terms\"/>" +
                "</KeyTermRules>";
            KeyTermRules rules = XmlSerializationHelper.DeserializeFromString<KeyTermRules>(xmlRules);
            rules.Initialize("Advanced Biblical Terms");
            Assert.AreEqual(2, rules.RegexRules.Count());
            Assert.IsTrue(rules.RegexRules.Any(r => r.ToString() == "(?<term>.+): \\(\\d+\\).+; "));
            Assert.IsTrue(rules.RegexRules.Any(r => r.ToString() == "\\(\\d+\\) (?<term>[^;]+): \\([^;]+\\)"));
            Assert.IsTrue(rules.RulesDictionary["ar"].Rule == KeyTermRule.RuleType.MatchForRefOnly);
            KeyTermRule rule;
            Assert.IsFalse(rules.RulesDictionary.TryGetValue("ask; pray", out rule));
		}
	}
}
