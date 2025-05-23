// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2023' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;
using static System.Text.RegularExpressions.RegexOptions;

namespace SIL.Transcelerator
{
	public enum AffixType
	{
		Prefix,
		Suffix,
	}

	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Class to hold utility methods for working with RegEx match groups and patterns.
	/// </summary>
	/// ------------------------------------------------------------------------------------
	public class SubstitutionMatchGroup
	{
		public enum MatchGroupType
		{
			Normal,
			EntireMatch,
			Remove,
		}

		#region Data members
		public MatchGroupType Type { get; private set; }
		public string Group { get; private set; }
		private static readonly Regex s_matchNTimes = new Regex(@"(?<expressionToRepeat>\(.*\)|.)\{(?<minMatches>\d+,)?(?<maxMatches>\d+)\}");
		private static readonly Regex s_matchSubstGroup = new Regex(@"\$((?<numeric>(\d+)|&)|(\{(?<named>\w[a-zA-Z_0-9]*)\}))");
		private const string kMatchPrefix = @"\b{0}";
		private const string kMatchSuffix = @"{0}\b";
		public const string kContiguousLettersMatchExpr = @"(?<!\\)(\w+)";
		public static SubstitutionMatchGroup EntireMatch { get; } = new SubstitutionMatchGroup {
			Type = MatchGroupType.EntireMatch, Group = "&"};
		public static SubstitutionMatchGroup RemoveGroup { get; } = new SubstitutionMatchGroup {
			Type = MatchGroupType.Remove, Group = ""};
		#endregion

		#region private properties
		private bool IsNumberedGroup => char.IsNumber(Group[0]);
		#endregion

		#region Constructors
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstitutionMatchGroup"/> class. Private, for
		/// the purpose of creating the special static versions.
		/// </summary>
		/// --------------------------------------------------------------------------------
		private SubstitutionMatchGroup()
		{
		}
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstitutionMatchGroup"/> class representing a
		/// "Normal" (named or numbered) match group.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public SubstitutionMatchGroup(string group)
		{
			if ((IsNullOrEmpty(group) || group == "&"))
				throw new ArgumentException("For normal match groups, the group name or" +
					" number must be specified", nameof(group));

			Type = MatchGroupType.Normal;
			Group = group;
		}
		#endregion

		#region Public methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Looks for a prefix or suffix expression covered, partially covered, or immediately
		/// preceding the selected text. If one is found, the selection is set to cover the
		/// entire expression representing the prefix or suffix, i.e., including the regular
		/// expression marker.
		/// </summary>
		/// <param name="affixType">Value indicating whether to look for a prefix or suffix.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// ------------------------------------------------------------------------------------
		public static void ResetTextAndSelectionForUpdatedAffix(string sText, AffixType affixType,
			ITextWithSelection selectableText, out bool existingAffixDeleted)
		{
			sText = sText.Trim();
			if (sText.Length == 0)
			{
				Match match = FindAffixExpression(affixType, selectableText);
				if (match.Success)
				{
					selectableText.Text = match.Result("$`$'");
					selectableText.SelectionStart = match.Index;
					selectableText.SelectionLength = 0;
				}

				existingAffixDeleted = true;
				return;
			}
			SelectExistingPrefixOrSuffix(affixType, selectableText);
			int selRestore = selectableText.SelectionStart + GetAffixPlaceholderPosition(affixType);
			ReplaceSelectedText(FormatAffix(affixType, sText), selectableText);
			selectableText.SelectionStart = selRestore;
			selectableText.SelectionLength = sText.Length;
			existingAffixDeleted = false;
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Looks for a prefix or suffix expression covered, partially covered, or immediately
		/// preceding the selected text. If one is found, the selection is set to cover the
		/// entire expression representing the prefix or suffix, i.e., including the regular
		/// expression marker.
		/// </summary>
		/// <param name="affixType">Value indicating whether to look for a prefix or suffix.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// ------------------------------------------------------------------------------------
		public static void SelectExistingPrefixOrSuffix(AffixType affixType,
			ITextWithSelection selectableText)
		{
			Match match = FindAffixExpression(affixType, selectableText);
			if (!match.Success)
				return;
			selectableText.SelectionStart = match.Index;
			selectableText.SelectionLength = match.Length;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the currently selected text.
		/// </summary>
		/// <param name="textToInsert">The text to insert.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// ------------------------------------------------------------------------------------
		public static void ReplaceSelectedText(string textToInsert,
			ITextWithSelection selectableText)
		{
			string cellValue = selectableText.Text;
			if (cellValue == null)
				return;
			cellValue = cellValue.Remove(selectableText.SelectionStart, selectableText.SelectionLength);
			cellValue = cellValue.Insert(selectableText.SelectionStart, textToInsert);
			selectableText.Text = cellValue;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an existing prefix or suffix (as determined by the format string) covered,
		/// partially covered, or immediately preceding the selected text.
		/// </summary>
		/// <param name="affixType">Value indicating whether to look for a prefix or suffix.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// <returns>The (English) text portion of the prefix or suffix, i.e., without the
		/// regular expression marker</returns>
		/// ------------------------------------------------------------------------------------
		public static string GetExistingAffix(AffixType affixType, ITextWithSelection selectableText)
		{
			Match match = FindAffixExpression(affixType, selectableText);
			return match.Success ? match.Result("$1") : Empty;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the prefix or suffix expression covered, partially covered, or immediately
		/// preceding the selected text.
		/// </summary>
		/// <param name="affixType">Value indicating whether to look for a prefix or suffix.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// <returns>A Match object representing the regular expression for the prefix or
		/// suffix</returns>
		/// ------------------------------------------------------------------------------------
		public static Match FindAffixExpression(AffixType affixType,
			ITextWithSelection selectableText)
		{
			int pos = affixType == AffixType.Suffix ? selectableText.SelectionStart :
				selectableText.SelectionStart + selectableText.SelectionLength;
			return FindAffixExpressionAt(affixType, selectableText.Text, pos);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the prefix or suffix expression covered, partially covered, or immediately
		/// preceding the given position in the text. If the position is in some text that
		/// ambiguously defines something that could be a prefix and/or suffix, the prefix will be
		/// considered as the portion preceding <paramref name="pos"/> and the suffix will be the
		/// portion following.
		/// </summary>
		/// <param name="affixType">Value indicating whether to look for a prefix or suffix.</param>
		/// <param name="text">Test to search</param>
		/// <param name="pos">Character position in text to look for the affix expression</param>
		/// <returns>A Match object representing the regular expression for the prefix or
		/// suffix</returns>
		/// ------------------------------------------------------------------------------------
		public static Match FindAffixExpressionAt(AffixType affixType, string text, int pos)
		{
			const int lengthOfWordBreakExpression = 2; // (i.e., length of `\b`)
			var format = GetAffixFormat(affixType).Replace(@"\b", @"\\b");
			var regex = new Regex(Format(format, kContiguousLettersMatchExpr), Compiled);
			var match = FindMatchPatternAt(regex, text, pos);

			if (!match.Success)
				return Match.Empty;

			if (affixType == AffixType.Prefix)
			{
				// But if we're in the middle of this potential prefix and the following portion is
				// a valid suffix, then adjust to get only the preceding part.
				if (pos > match.Index + lengthOfWordBreakExpression && text.Length > pos)
				{
					var suffixMatch = FindAffixExpressionAt(AffixType.Suffix, text.Substring(pos), 0);
					if (suffixMatch.Success)
					{
						Debug.Assert(suffixMatch.Index == 0);
						match = regex.Match(text, match.Index, pos - match.Index);
					}
				}
			}
			else if (affixType == AffixType.Suffix)
			{
				// But if we're in the middle of this potential suffix and the preceding portion is
				// a valid prefix, then adjust to get only the following part.
				if (pos > 0 && pos < match.Index + match.Length - lengthOfWordBreakExpression)
				{
					var prefixMatch = FindAffixExpressionAt(AffixType.Prefix, text.Substring(0, pos), pos);
					if (prefixMatch.Success)
					{
						Debug.Assert(prefixMatch.Index == match.Index - lengthOfWordBreakExpression);
						match = regex.Match(text, pos);
					}
				}
			}

			return match;
		}

		public static string FormatAffix(AffixType affixType, string text)
		{
			return IsNullOrWhiteSpace(text) ? Empty : Format(GetAffixFormat(affixType), text);
		}

		public static int GetAffixPlaceholderPosition(AffixType affixType)
		{
			return GetAffixFormat(affixType).IndexOf("{0}", StringComparison.Ordinal);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match count value from the expression covered, partially covered, or
		/// immediately preceding the selected text. If there is no explicit match count
		/// expression, then this returns 1.
		/// </summary>
		/// <param name="selectableText">Text source that supports selection</param>
		/// ------------------------------------------------------------------------------------
		public static int GetExistingMatchCountValue(ITextWithSelection selectableText) =>
			GetRangeMax(selectableText.Text, selectableText.SelectionStart);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the maximum match count value from the range expression at, containing or
		/// immediately preceding the given position in the text. If no range
		/// expression is at the given position or that covers the character at that position,
		/// then 1 is returned (since that is the default max number of matches).
		/// </summary>
		/// <param name="text">Text to search</param>
		/// <param name="pos">Character position in text to look for the affix expression</param>
		/// ------------------------------------------------------------------------------------
		public static int GetRangeMax(string text, int pos)
		{
			var match = GetMatchForRangeExpression(text, pos);
			return match.Success? int.Parse(match.Result("${maxMatches}")) : 1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the range expression at, containing or immediately preceding the given position
		/// in the text.
		/// </summary>
		/// <param name="text">Text to search</param>
		/// <param name="pos">Character position in text to look for the affix expression</param>
		/// <returns>A Match object representing the text found.</returns>
		/// ------------------------------------------------------------------------------------
		private static Match GetMatchForRangeExpression(string text, int pos)
		{
			Match match = s_matchNTimes.Match(text);
			while (match.Success && match.Index + match.Length < pos)
				match = match.NextMatch();
			return (match.Success && match.Index <= pos) ? match : Match.Empty;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the expression covered, partially covered, or immediately preceding the
		/// given position in the text.
		/// </summary>
		/// <param name="matchPattern">The match pattern.</param>
		/// <param name="text">Text to search</param>
		/// <param name="pos">Character position in text to look for the affix expression</param>
		/// <returns>A Match object representing the text found.</returns>
		/// ------------------------------------------------------------------------------------
		private static Match FindMatchPatternAt(Regex matchPattern, string text, int pos)
		{
			Match match = matchPattern.Match(text);
			while (match.Success && match.Index + match.Length < pos)
				match = match.NextMatch();
			return (match.Success && match.Index <= pos) ? match : Match.Empty;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the match group, if any, from the replacement expression containing or preceding
		/// the given position. If there is no match group expression there, then this returns an
		/// empty string.
		/// </summary>
		/// <param name="replacementExpr">Replacement expression to search</param>
		/// <param name="pos">Character position in text to look for the affix expression</param>
		/// ------------------------------------------------------------------------------------
		public static SubstitutionMatchGroup GetExistingMatchGroup(string replacementExpr, int pos)
		{
			Match match = FindMatchPatternAt(s_matchSubstGroup, replacementExpr, pos);
			if (!match.Success)
				return null;
			string group = match.Result("${numeric}${named}");
			return (group == "0" || group == "&") ? EntireMatch: new SubstitutionMatchGroup(group);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a list of strings that can be used to describe the regular expression match
		/// groups found in the given expression. Note that the "entire match" group will
		/// always be the first entry in the returned array.
		/// </summary>
		/// <param name="matchExpression">RegEx pattern from which to harvest the match groups
		/// </param>
		/// ------------------------------------------------------------------------------------
		public static IEnumerable<SubstitutionMatchGroup> GetMatchGroups(string matchExpression)
		{
			string[] matchGroups;
			try
			{
				Regex r = new Regex(matchExpression);
				matchGroups = r.GetGroupNames();
			}
			catch (ArgumentException)
			{
				yield break;
			}
			yield return EntireMatch;
			foreach (var group in matchGroups.Skip(1).Select(s => new SubstitutionMatchGroup(s)))
				yield return group;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the match group substitution expression currently at (or near) the given
		/// position in the text, or inserts a new match group substitution expression if there
		/// isn't one already.
		/// </summary>
		/// <param name="text">Regular expression text in which count should be updated or
		/// inserted</param>
		/// <param name="insertAt">Character position in text to look for the existing  match
		/// group or where the new match group should be inserted. The supplied value may be
		/// adjusted to indicate the start of the actual group name/number.
		/// </param>
		/// <param name="charsToReplaceIfGroupNotFound">If an existing expression for the
		/// specified group is not found, this value indicates the number of existing characters
		/// in the regex to remove when inserting the group expression.</param>
		/// <param name="length">The number of characters (starting at the possibly
		/// updated insertAt location) in the updated regex text that cover the entire new or
		/// updated group specifier. If this was called to remove the existing group, this
		/// will always be 0</param>
		/// ------------------------------------------------------------------------------------
		public string UpdateMatchGroup(string text, ref int insertAt,
			int charsToReplaceIfGroupNotFound, out int length)
		{
			length = Group.Length;
			Match match = FindMatchPatternAt(s_matchSubstGroup, text, insertAt);
			if (match.Success)
			{
				if (Type != MatchGroupType.Remove)
				{
					text = match.Result("$`$$");
					insertAt = text.Length;

					if (!IsNumberedGroup && Type != MatchGroupType.EntireMatch)
					{
						text += "{" + Group + "}";
						insertAt++;
					}
					else
					{
						text += Group;
					}

					text += match.Result("$'");
				}
				else
				{
					text = match.Result("$`");
					insertAt = text.Length;
					text += match.Result("$'");
				}
			}
			else
			{
				if (Type == MatchGroupType.Remove)
				{
					// REVIEW: Should we throw an exception or just do nothing? Doing nothing should be harmless,
					// but there really is no sane reason why the caller should ever ask to remove a group that
					// does not exist.
					throw new InvalidOperationException($"Do not call ${nameof(UpdateMatchGroup)} to remove a " +
						$"match group unless there is a group in the text at ${nameof(insertAt)}. Call " +
						$"${nameof(GetExistingMatchGroup)} to check for the existence of a group.");
				}

				if (charsToReplaceIfGroupNotFound > 0)
					text = text.Remove(insertAt, charsToReplaceIfGroupNotFound);

				if (!IsNumberedGroup && Type != MatchGroupType.EntireMatch)
				{
					text = text.Insert(insertAt, "${" + Group + "}");
					insertAt++;
				}
				else
				{
					text = text.Insert(insertAt, "$" + Group);
				}
				insertAt++; // Don't want to select the $
			}

			return text;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the match count expression currently selected (or near the selection), or
		/// inserts a new match count expression if there isn't one already.
		/// </summary>
		/// <param name="numTimesToMatch">The num times to match.</param>
		/// <param name="selectableText">Text source that supports selection</param>
		/// ------------------------------------------------------------------------------------
		public static void UpdateMatchCount(int numTimesToMatch, ITextWithSelection selectableText)
		{
			var insertAt = selectableText.SelectionStart;
			var text = selectableText.Text;

			selectableText.Text = UpdateRangeMax(numTimesToMatch, text, ref insertAt,
				selectableText.SelectionLength, out int length);
			selectableText.SelectionStart = insertAt;
			selectableText.SelectionLength = length;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the max value of the range expression at (or near) the given position in
		/// the text, or inserts a new range expression if there isn't one already.
		/// </summary>
		/// <param name="newMaxValue">The desired maximum number of times to match.</param>
		/// <param name="text">Regular expression text in which count should be updated or
		/// inserted</param>
		/// <param name="insertAt">Character position in text to look for an existing expression
		/// indicating the number of times to match</param>
		/// <param name="numberOfCharsToIncludeInNewCountExpression">If an existing expression
		/// indicating the number of times to match is not found, this value will indicate the
		/// number of characters (starting at insertAt) to include in the newly created
		/// expression. Note that existing parentheses (if present) will be preserved.</param>
		/// <param name="length">The number of characters (starting at the possibly
		/// updated insertAt location) in the updated regex text that cover the entire new or
		/// updated count specifier</param>
		/// ------------------------------------------------------------------------------------
		public static string UpdateRangeMax(int newMaxValue, string text, ref int insertAt,
			int numberOfCharsToIncludeInNewCountExpression, out int length)
		{
			length = 0;

			if (insertAt == 0 && numberOfCharsToIncludeInNewCountExpression == 0)
				return text;

			Match match = GetMatchForRangeExpression(text, insertAt);
			if (match.Success)
			{
				if (newMaxValue > 1)
				{
					string minRange = match.Result("${minMatches}");
					if (IsNullOrEmpty(minRange))
						minRange = "1,";
					text = match.Result("$`${expressionToRepeat}");
					insertAt = text.Length;
					text += match.Result("{" + minRange + newMaxValue + "}");
					length = text.Length - insertAt;
					text += match.Result("$'");
				}
				else
				{
					text = match.Result("$`${expressionToRepeat}");
					insertAt = text.Length;
					length = 0;
					text += match.Result("$'");
				}
			}
			else
			{
				if (numberOfCharsToIncludeInNewCountExpression > 1)
				{
					if (text[insertAt + numberOfCharsToIncludeInNewCountExpression - 1] != ')' || text[insertAt] != '(')
					{
						text = text.Insert(insertAt + numberOfCharsToIncludeInNewCountExpression, ")");
						text = text.Insert(insertAt, "(");
						insertAt += numberOfCharsToIncludeInNewCountExpression + 2;
					}
					else
						insertAt += numberOfCharsToIncludeInNewCountExpression;
				}
				if (newMaxValue > 1)
				{
					string sTextToInsert = "{1," + newMaxValue + "}";
					length = sTextToInsert.Length;
					text = text.Insert(insertAt, sTextToInsert);
				}
			}

			return text;
		}
		#endregion
	
		#region Private helper methods
		private static string GetAffixFormat(AffixType affixType) =>
			affixType == AffixType.Prefix ? kMatchPrefix : kMatchSuffix;

		#endregion
	}
}