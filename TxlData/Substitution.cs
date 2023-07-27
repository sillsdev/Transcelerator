// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: Substitution.cs
// ---------------------------------------------------------------------------------------------
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System;
using static System.String;

namespace SIL.Transcelerator
{
	#region class Substitution
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to hold rules about phrases that are substituted before parsing
	/// (supports XML serialization)
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("Substitution")]
	public class Substitution
	{
		#region Data members
		private Regex m_regEx;
		private string m_matchingPattern;
		private bool m_isRegex;
		private bool m_matchCase;
		#endregion

		#region Public (XML) properties
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the original phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("pattern")]
		public string MatchingPattern
		{
			get => m_matchingPattern;
			set
			{
				m_matchingPattern = value;
				m_regEx = null;
			}
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the translation.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("replacement")]
		public string Replacement { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the reference.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("regex")]
		public bool IsRegex
		{
			get => m_isRegex;
			set
			{
				m_isRegex = value;
				m_regEx = null;
			}
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets whether the match is case-sensitive.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("case")]
		public bool MatchCase
		{
			get => m_matchCase;
			set
			{
				m_matchCase = value;
				m_regEx = null;
			}
		}
		#endregion

		#region Constructors
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Substitution"/> class, needed
		/// for XML serialization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Substitution()
		{
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Substitution"/> class.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public Substitution(string matchingPattern, string replacement, bool regEx,
			bool matchCase)
		{
			MatchingPattern = matchingPattern;
			Replacement = replacement;
			IsRegex = regEx;
			MatchCase = matchCase;
		}
		#endregion

		#region Public methods
		public string Apply(string sample) => RegEx.Replace(sample, RegExReplacementString);
		#endregion

		#region Non-serializable (as XML) Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a regular expression object representing this substitution (regardless of
		/// whether this substitution is marked as a regular expression).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal Regex RegEx
		{
			get
			{
				ErrorType = RegExErrorType.None;
				if (m_regEx == null)
				{
					if (String.IsNullOrEmpty(MatchingPattern))
					{
						m_regEx = new Regex(Empty);
						ErrorType = RegExErrorType.Empty;
					}
					else
					{
						string pattern = MatchingPattern.Normalize(NormalizationForm.FormC);
						if (!IsRegex)
							pattern = Regex.Escape(pattern);
						RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant;
						if (!MatchCase)
							options |= RegexOptions.IgnoreCase;
						try
						{
							m_regEx = new Regex(pattern, options);
						}
						catch (ArgumentException ex)
						{
							if (!IsRegex)
								throw; // Not sure what else to do - hopefully this can't happen.

							ErrorType = RegExErrorType.Exception;
							RegExError = ex;
							m_regEx = new Regex(Empty);
						}
					}
				}
				return m_regEx;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a replacement string suitable for using in a regular expression replacement.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal string RegExReplacementString =>
			(String.IsNullOrEmpty(Replacement)) ? Empty :
				Replacement.Normalize(NormalizationForm.FormD); // ToLowerInvariant()?

		/// ------------------------------------------------------------------------------------
		public bool Valid => RegEx.ToString().Length > 0;

		/// ------------------------------------------------------------------------------------
		public static bool IsNullOrEmpty(Substitution sub)
		{
			return sub == null || (String.IsNullOrEmpty(sub.MatchingPattern) &&
				String.IsNullOrEmpty(sub.Replacement));
		}

		public enum RegExErrorType
		{
			None,
			Empty,
			Exception,
		}

		public Exception RegExError { get; private set; }
		public RegExErrorType ErrorType { get; private set; }
		#endregion
	}
	#endregion
}