// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: RenderingSelectionRule.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using static System.String;
using SIL.Utils;

namespace SIL.Transcelerator
{
	#region class RenderingSelectionRule
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to hold rules that govern which term rendering is selected based on
	/// regular expression matching against the original English question.
	/// (supports XML serialization)
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("RenderingSelectionRule")]
	public class RenderingSelectionRule
	{
		#region Data members
		private string m_questionMatchingPattern;
		private string m_renderingMatchingPattern;
		private string m_qVariable, m_rVariable;
		private static readonly Regex s_qSuffixMatchPattern = new Regex(@"^\{0\}\\w\*(?<var>\w*)\\b$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex s_qPrefixMatchPattern = new Regex(@"^\\b(?<var>\w*)\\w\*\{0\}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex s_precedingWordMatchPattern = new Regex(@"^\\b(?<var>\w*) \{0\}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex s_followingWordMatchPattern = new Regex(@"^\{0\} (?<var>\w*)\\b$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex s_rSuffixMatchPattern = new Regex(@"^(?<var>\w*)\$$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex s_rPrefixMatchPattern = new Regex(@"^\^(?<var>\w*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		#endregion

		public enum QuestionMatchType
		{
			Undefined,
			Suffix,
			Prefix,
			PrecedingWord,
			FollowingWord,
			Custom,
		}

		public enum RenderingMatchType
		{
			Undefined,
			Suffix,
			Prefix,
			Custom,
		}

		#region Public (XML) properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name of the rule.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets whether this rule is disabled.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("disabled")]
		public bool Disabled { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the original phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("questionMatcher")]
		public string QuestionMatchingPattern
		{
			get => m_questionMatchingPattern;
			set
			{
				KeyTermPlaceHolderMissing = false;
				ErrorMessageQ = null;
				if (value == null)
				{
					m_questionMatchingPattern = null;
					m_qVariable = null;
					return;
				}
				m_questionMatchingPattern = value.Normalize(NormalizationForm.FormC);
				if (!m_questionMatchingPattern.Contains("{0}"))
					KeyTermPlaceHolderMissing = true;
				else
				{
					try
					{
						new Regex(Format(m_questionMatchingPattern, "term"), RegexOptions.CultureInvariant);
					}
					catch (Exception ex)
					{
						if (ex is ArgumentException || ex is FormatException)
						{
							KeyTermPlaceHolderMissing = false;
							ErrorMessageQ = ex.Message;
						}
						else
							throw;
					}
				}
			}
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the translation.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("renderingSelector")]
		public string RenderingMatchingPattern
		{
			get => m_renderingMatchingPattern;
			set
			{
				ErrorMessageR = null;
				if (value == null)
				{
					m_renderingMatchingPattern = null;
					m_rVariable = null;
					return;
				}
				m_renderingMatchingPattern = value.Normalize(NormalizationForm.FormC);
				if (!string.IsNullOrEmpty(m_questionMatchingPattern) && ErrorMessageQ == null)
				{
					try
					{
						Regex.Replace("term", string.Format(m_questionMatchingPattern, "term"),
							m_renderingMatchingPattern, RegexOptions.CultureInvariant);
					}
					catch (ArgumentException ex)
					{
						ErrorMessageR = ex.Message;
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a specific term to which this rule applies. If null, then this rule
		/// can apply to any term.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("term")]
		public string SpecificTerm { get; set; }
		#endregion

		#region Constructors
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderingSelectionRule"/> class,
		/// needed for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public RenderingSelectionRule()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderingSelectionRule"/> class
		/// </summary>
		/// --------------------------------------------------------------------------------
		public RenderingSelectionRule(string name)
		{
			Name = name;
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderingSelectionRule"/> class
		/// </summary>
		/// --------------------------------------------------------------------------------
		public RenderingSelectionRule(string questionMatchingPattern, string replacement)
		{
			QuestionMatchingPattern = questionMatchingPattern;
			RenderingMatchingPattern = replacement;
		}
		#endregion

		#region Public Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines if this rule applies to the given question, and if so, will attempt to
		/// select a rendering to use.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string ChooseRendering(string question, IEnumerable<Word> term, IEnumerable<string> renderings)
		{
			if (!Valid)
				return null;

			Regex regExQuestion = null;
			try
			{
				regExQuestion = new Regex(Format(m_questionMatchingPattern, "(?i:" + term.ToString(@"\W+") + ")"), RegexOptions.CultureInvariant);
			}
			catch (ArgumentException ex)
			{
				ErrorMessageQ = ex.Message;
			}

			if (regExQuestion != null && regExQuestion.IsMatch(question))
			{
				Regex regExRendering;
				try
				{
					regExRendering = new Regex(m_renderingMatchingPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
				}
				catch (ArgumentException ex)
				{
					ErrorMessageR = ex.Message;
					return null;
				}
				return renderings.FirstOrDefault(rendering => regExRendering.IsMatch(rendering.Normalize(NormalizationForm.FormC)));
			}
			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString() =>
			Name ?? m_questionMatchingPattern + ":" + m_renderingMatchingPattern;
		#endregion

		#region Non-serializable (XML) Properties
		public bool Valid => !KeyTermPlaceHolderMissing && ErrorMessageQ == null && ErrorMessageR == null;

		public bool KeyTermPlaceHolderMissing { get; private set; }
		public string ErrorMessageQ { get; private set; }
		public string ErrorMessageR { get; private set; }

		public QuestionMatchType QuestionMatchCriteriaType
		{
			get
			{
				m_qVariable = null;
				if (IsNullOrEmpty(m_questionMatchingPattern))
					return QuestionMatchType.Undefined;
				QuestionMatchType type;
				Match match = s_qSuffixMatchPattern.Match(m_questionMatchingPattern);
				if (match.Success)
					type = QuestionMatchType.Suffix;
				else
				{
					match = s_qPrefixMatchPattern.Match(m_questionMatchingPattern);
					if (match.Success)
						type = QuestionMatchType.Prefix;
					else
					{
						match = s_precedingWordMatchPattern.Match(m_questionMatchingPattern);
						if (match.Success)
							type = QuestionMatchType.PrecedingWord;
						else
						{
							match = s_followingWordMatchPattern.Match(m_questionMatchingPattern);
							if (match.Success)
								type = QuestionMatchType.FollowingWord;
							else if (m_questionMatchingPattern.Contains("{0}"))
							{
								m_qVariable = m_questionMatchingPattern;
								return QuestionMatchType.Custom;
							}
							else
								return QuestionMatchType.Undefined;
						}
					}
				}
				m_qVariable = match.Result("${var}");
				return type;
			}
		}

		public RenderingMatchType RenderingMatchCriteriaType
		{
			get
			{
				m_rVariable = null;
				if (IsNullOrEmpty(m_renderingMatchingPattern))
					return RenderingMatchType.Undefined;

				RenderingMatchType type;
				Match match = s_rSuffixMatchPattern.Match(m_renderingMatchingPattern);
				if (match.Success)
					type = RenderingMatchType.Suffix;
				else
				{
					match = s_rPrefixMatchPattern.Match(m_renderingMatchingPattern);
					if (match.Success)
						type = RenderingMatchType.Prefix;
					else
					{
						m_rVariable = m_renderingMatchingPattern;
						return RenderingMatchType.Custom;
					}
				}
				m_rVariable = match.Result("${var}");
				return type;
			}
		}

		public string QuestionVariable => m_qVariable;
		public string ResultVariable => m_rVariable;

		public string QuestionMatchSuffix
		{
			get => QuestionMatchCriteriaType == QuestionMatchType.Suffix ? m_qVariable : null;
			set
			{
				m_qVariable = value;
				QuestionMatchingPattern = IsNullOrEmpty(value) ? null : @"{0}\w*" + value + @"\b";
			}
		}

		public string QuestionMatchPrefix
		{
			get => QuestionMatchCriteriaType == QuestionMatchType.Prefix ? m_qVariable : null;
			set
			{
				m_qVariable = value;
				QuestionMatchingPattern = IsNullOrEmpty(value) ? null : @"\b" + value + @"\w*{0}";
			}
		}

		public string QuestionMatchPrecedingWord
		{
			get => QuestionMatchCriteriaType == QuestionMatchType.PrecedingWord ? m_qVariable : null;
			set
			{
				m_qVariable = value;
				QuestionMatchingPattern = IsNullOrEmpty(value) ? null : @"\b" + value + " {0}";
			}
		}

		public string QuestionMatchFollowingWord
		{
			get => QuestionMatchCriteriaType == QuestionMatchType.FollowingWord ? m_qVariable : null;
			set
			{
				m_qVariable = value;
				QuestionMatchingPattern = IsNullOrEmpty(value) ? null : "{0} " + value + @"\b";
			}
		}

		public string RenderingMatchSuffix
		{
			get => RenderingMatchCriteriaType == RenderingMatchType.Suffix ? m_rVariable : null;
			set
			{
				m_rVariable = value;
				RenderingMatchingPattern = IsNullOrEmpty(value) ? null : value + "$";
			}
		}

		public string RenderingMatchPrefix
		{
			get => RenderingMatchCriteriaType == RenderingMatchType.Prefix ? m_rVariable : null;
			set
			{
				m_rVariable = value;
				RenderingMatchingPattern = IsNullOrEmpty(value) ? null : "^" + value;
			}
		}
		#endregion
	}
	#endregion
}