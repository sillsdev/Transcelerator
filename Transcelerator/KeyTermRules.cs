﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: KeyTermRule.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Xml.Serialization;
using SIL.Utils;

namespace SIL.Transcelerator
{
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to support XML serialization and provide key terms in a usable form to
	/// PhraseTranslationHelper
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[Serializable]
	[XmlType(AnonymousType=true)]
	[XmlRoot(Namespace="", IsNullable=false)]
	public class KeyTermRules
	{
	    private List<Regex> m_regexRules;
        private ReadonlyDictionary<string, KeyTermRule> m_rulesDictionary;

	    [XmlElement("KeyTermRule", Form=XmlSchemaForm.Unqualified)]
		public List<KeyTermRule> Items { get; set; }

        /// <summary>
        /// Regular-expression-based rules. If a regular expression contains the variable
        /// "term", that variable can be used (potentially for multiple matches) to extract
        /// terms from a key-term's English gloss. If the term variable is not present in
        /// the regular expression, its function is to indicate that the matching key-term
        /// is to be excluded.</summary>
        [XmlIgnore]
        public IEnumerable<Regex> RegexRules
	    {
	        get
            {
                if (m_rulesDictionary == null)
                    throw new InvalidOperationException("Must call Initialize before accessing RegexRules");
                return m_regexRules;
            }
	    }

        /// <summary>
        /// dictionary of (English) key terms to rules
        /// </summary>
        /// <remarks>Wanted to make this class actually BE a ReadonlyDictionary, but that prevented
        /// it from being XML Serializable</remarks>
        [XmlIgnore]
        public ReadonlyDictionary<string, KeyTermRule> RulesDictionary
        {
	        get
            {
                if (m_rulesDictionary == null)
                    throw new InvalidOperationException("Must call Initialize before accessing RulesDictionary");
                return m_rulesDictionary;
            }
        }

	    public void Initialize()
	    {
            Dictionary<string, KeyTermRule> dictionary = new Dictionary<string, KeyTermRule>();
            m_regexRules = null;
            foreach (KeyTermRule keyTermRule in Items.Where(rule => !String.IsNullOrEmpty(rule.id)))
            {
                if (keyTermRule.IsRegEx)
                {
                    if (m_regexRules == null)
                        m_regexRules = new List<Regex>();
                    m_regexRules.Add(new Regex(keyTermRule.id, RegexOptions.Compiled));
                }
                else
                    dictionary[keyTermRule.id] = keyTermRule;
            }
            m_rulesDictionary = new ReadonlyDictionary<string, KeyTermRule>(dictionary);
	    }
	}
	
	[Serializable]
	[XmlType(AnonymousType=true)]
	public class KeyTermRule
	{

		[Serializable]
		public enum RuleType
		{
			MatchForRefOnly,
			Exclude,
		}

		private string m_id;

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the list of alternate forms of the terms.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlElement("Alternate", Form=XmlSchemaForm.Unqualified)]
		public KeyTermRulesKeyTermRuleAlternate[] Alternates { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute]
		public string id
		{
			get { return m_id; }
			set { m_id = value.ToLowerInvariant().Normalize(NormalizationForm.FormC); }
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the rule.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("rule")]
		public string RuleStr
		{
			get
			{
				return Rule == null ? null : Rule.ToString();
			}
			set
			{
				if (value == null || !Enum.IsDefined(typeof(RuleType), value))
					Rule = null;
				else
					Rule = (RuleType)Enum.Parse(typeof(RuleType), value, true);
			}
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets whether this rule's id should be interpreted as a regular
		/// expresssion, rather than an exact string match.
		/// </summary>
		/// --------------------------------------------------------------------------------
        [XmlAttribute("regex")]
        public bool IsRegEx { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the rule.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlIgnore]
        public RuleType? Rule { get; set; }

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this rule was actually used during
        /// processing of the key terms. If no matching term was found, the rule is not
        /// used.
        /// </summary>
        /// --------------------------------------------------------------------------------
        [XmlIgnore]
        public bool Used { get; set; }

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Returns rule id
        /// </summary>
        /// --------------------------------------------------------------------------------
        public override string ToString()
        {
            return id;
        }
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to represent an alternate form of a key term
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[Serializable]
	[XmlType(AnonymousType=true)]
	public class KeyTermRulesKeyTermRuleAlternate
	{
		private string m_name;

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("name")]
		public string Name
		{
			get { return m_name; }
			set { m_name = value.ToLowerInvariant().Normalize(NormalizationForm.FormC); }
		}

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets whether this alternate should only be considered a match when the
        /// term occurs in a question whose Scripture reference is known to contain the
        /// term. If false or unspecified, this has no effect. If true, and the owning rule
        /// has not specified the MatchForRefOnly rule, this overrides the parent rule for
        /// this alternate. (See the "ask; pray" rule for an example of correct usage.)
        /// </summary>
        /// --------------------------------------------------------------------------------
        [XmlAttribute("matchForRefOnly")]
        public bool MatchForRefOnly { get; set; }
    }
}