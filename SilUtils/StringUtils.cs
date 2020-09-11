// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: StringUtils.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// StringUtils is a collection of static methods for working with strings and characters.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public static class StringUtils
	{
		/// <summary>Character is interpreted as object</summary>
		public const char kChObject = '\uFFFC';

		/// <summary>String is interpreted as object</summary>
		public static readonly string kszObject = "\uFFFC";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Find the portion of two strings which is different.
		/// </summary>
		/// <param name="str1">first string to compare</param>
		/// <param name="str2">second string to compare</param>
		/// <param name="ichMin">The index of the first difference or -1 if no difference is
		/// found</param>
		/// <param name="ichLim1">The index of the limit of the difference in the 1st string or
		/// -1 if no difference is found</param>
		/// <param name="ichLim2">The index of the limit of the difference in the 2nd string or
		/// -1 if no difference is found</param>
		/// <returns><c>true</c> if a difference is found; <c>false</c> otherwise</returns>
		/// ------------------------------------------------------------------------------------
		public static bool FindStringDifference(string str1, string str2, out int ichMin, out int ichLim1, out int ichLim2)
		{
			ichMin = FindStringDifference_(str1, str2);
			if (ichMin >= 0)
			{
				// Search from the end to find the end of the text difference
				FindStringDifferenceLimits(str1, str2, ichMin, out ichLim1, out ichLim2);
				return true;
			}
			ichLim1 = -1;
			ichLim2 = -1;
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Find a difference in two strings.
		/// </summary>
		/// <param name="str1">first string to compare</param>
		/// <param name="str2">second string to compare</param>
		/// <returns>The index of the first difference or -1 if not found</returns>
		/// TODO-Linux FWNX-150: mono compiler was have problems thinking calls to FindStringDifference was
		/// ambiguous. So renambed to FindStringDifference_
		/// ------------------------------------------------------------------------------------
		private static int FindStringDifference_(string str1, string str2)
		{
			for (int i = 0; i < str1.Length; )
			{
				// If we pass the end of string 2 before string 1 then the difference is
				// the remainder of string 1.
				if (i >= str2.Length)
					return i;

				// get the next character with its combining modifiers
				string next1 = GetNextCharWithModifiers(str1, i);
				string next2 = GetNextCharWithModifiers(str2, i);

				if (next1 != next2)
					return i;
				i += next1.Length;
			}

			// If the second string is longer than the first string, then return the difference as the
			// last portion of the second string.
			if (str2.Length > str1.Length)
				return str1.Length;

			// If we get this far, the strings are the same.
			return -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Produce a version of the given name that can be used as a file name. This is done
		/// by replacing characters that the current OS does not allow with underscores '_'.
		/// </summary>
		/// <param name="sName">Name to be filtered</param>
		/// <returns>the filtered name</returns>
		/// ------------------------------------------------------------------------------------
		public static string FilterForFileName(string sName)
		{
			return FilterForFileName(sName, new string(Path.GetInvalidFileNameChars()));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Produce a version of the given name that can be used as a file name. This is done
		/// by replacing invalid characters with underscores '_'.
		/// </summary>
		/// <param name="sName">Name to be filtered</param>
		/// <param name="invalidChars">characters to filter out</param>
		/// <param name="replaceNbsp"></param>
		/// <returns>the filtered name</returns>
		/// ------------------------------------------------------------------------------------
		public static string FilterForFileName(string sName, string invalidChars, bool replaceNbsp = true)
		{
			StringBuilder cleanName = new StringBuilder(sName);

			// replace all invalid characters with an '_'
			for (int i = 0; i < sName.Length; i++)
			{
				if (invalidChars.IndexOf(sName[i]) >= 0 || sName[i] < ' ') // eliminate all control characters too
					cleanName[i] = '_';
			}
			if (replaceNbsp)
				cleanName.Replace('\u00A0', ' ');
			return cleanName.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the next character sequence that represents a character with any modifiers.
		/// This allows us to treat characters with diacritics as a unit.
		/// </summary>
		/// <param name="source">string to get character from</param>
		/// <param name="start">index to start extracting from</param>
		/// <returns>a character string with the base character and all modifiers</returns>
		/// ------------------------------------------------------------------------------------
		private static string GetNextCharWithModifiers(string source, int start)
		{
			int end = start + 1;
			while (end < source.Length && Char.GetUnicodeCategory(source, end) == UnicodeCategory.NonSpacingMark)
				end++;
			return source.Substring(start, end - start);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Find a difference in two strings.
		/// </summary>
		/// <param name="str1">first string to compare</param>
		/// <param name="str2">second string to compare</param>
		/// <param name="ichMin">the min of the difference that has already been found</param>
		/// <param name="ichLim1">The index of the limit of the difference in the 1st string</param>
		/// <param name="ichLim2">The index of the limit of the difference in the 2nd string</param>
		/// ------------------------------------------------------------------------------------
		private static void FindStringDifferenceLimits(string str1, string str2, int ichMin, out int ichLim1, out int ichLim2)
		{
			ichLim1 = str1.Length;
			ichLim2 = str2.Length;
			// look backwards through the strings for the end of the difference. Do not
			// look past the start of the difference which has already been found.
			while (ichLim1 > ichMin && ichLim2 > ichMin)
			{
				if (str1[ichLim1 - 1] != str2[ichLim2 - 1])
					return;
				--ichLim1;
				--ichLim2;
			}
		}

		/// <summary>
		/// Remove all whitespace from a string.
		/// </summary>
		public static string StripWhitespace(string s)
		{
			if (s == null)
				return s;
			int len = s.Length;
			if (len <= 0)
				return s;
			StringBuilder sb = new StringBuilder(len);
			foreach (char c in s)
			{
				if (!char.IsWhiteSpace(c))
					sb.Append(c);
			}
			return sb.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the given string from the string builder if it is unique (only occurs once
		/// as a substring) or is a whole-word match (surrounded by white-space, punctuation, or
		/// start/end of string).
		/// </summary>
		/// <param name="bldr">The string builder.</param>
		/// <param name="toRemove">The string to look for and remove.</param>
		/// ------------------------------------------------------------------------------------
		public static int RemoveUniqueOrWholeWordSubstring(this StringBuilder bldr,
			string toRemove)
		{
			return bldr.ReplaceUniqueOrWholeWordSubstring(toRemove, string.Empty, 1);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the given string from the string builder if it is unique (only occurs once
		/// as a substring) or is a whole-word match (surrounded by white-space, punctuation, or
		/// start/end of string).
		/// </summary>
		/// <param name="bldr">The string builder.</param>
		/// <param name="toRemove">The string to look for and remove.</param>
		/// <param name="replacement">The string to insert in place of the removed text.</param>
		/// <param name="minLengthForPartialMatch">Partial-word matches will only be performed
		/// if toRemove is at least minLengthForPartialMatch characters long</param>
		/// ------------------------------------------------------------------------------------
		public static int ReplaceUniqueOrWholeWordSubstring(this StringBuilder bldr,
			string toRemove, string replacement, int minLengthForPartialMatch)
		{
			string s = bldr.ToString();
			int useThisMatch;
			int ichMatch = useThisMatch = s.IndexOf(toRemove, StringComparison.Ordinal);
			while (ichMatch >= 0)
			{
				bool wordBreakAtStart = (ichMatch == 0 || !Char.IsLetter(s[ichMatch - 1]));
				int ichLim = ichMatch + toRemove.Length;
				bool wordBreakAtEnd = (ichLim == s.Length || !Char.IsLetter(s[ichLim]));
				if (wordBreakAtStart && wordBreakAtEnd)
				{
					useThisMatch = ichMatch;
					break;
				}

				if (useThisMatch != ichMatch || toRemove.Length < minLengthForPartialMatch)
					useThisMatch = -1; // This isn't the first match, so the only way we'll do the replacement is if we find a whole-word match.

				// Found a partial-word match. Keep looking to see if there's another (possibly a whole-word one)
				ichMatch = s.IndexOf(toRemove, ichMatch + 1, StringComparison.Ordinal);
			}

			if (useThisMatch >= 0)
				bldr.Remove(useThisMatch, toRemove.Length).Insert(useThisMatch, replacement);
			return useThisMatch;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the longest substring that two strings have in common. The substring returned
		/// will either be one or more contiguous whole words or a sustring that is part of a
		/// single word (if so requested by the caller). In the latter case, the returned
		/// substring must be at least 15% of the total string length to be considered useful.
		/// </summary>
		/// <param name="s1">The first string.</param>
		/// <param name="s2">The other string.</param>
		/// <param name="wholeWordOnly">if set to <c>true</c> only whole-word substrings will be
		/// considered useful.</param>
		/// <param name="foundWholeWords">Indicates whether the substring being returned is one
		/// or more whole words (undefined if no useful substring is found)</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static string LongestUsefulCommonSubstring(string s1, string s2, bool wholeWordOnly,
			out bool foundWholeWords)
		{
			foundWholeWords = true;

			if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
				return string.Empty;

			string bestMatch = string.Empty;
			for (int ich = 0; ich + bestMatch.Length < s1.Length; ich++)
			{
				if (s1[ich] == kChObject || char.IsWhiteSpace(s1[ich]))
					continue;

				int cchMatch = bestMatch.Trim().Length;
				string bestCandidate = string.Empty;

				do
				{
					cchMatch++;
				}
				while (ich + cchMatch < s1.Length && char.IsLetter(s1[ich + cchMatch])); // Need CPE?

				//if (cchMatch > maxLength)
				//{
				//    ich += cchMatch;
				//    continue;
				//}
				string candidate = s1.Substring(ich, cchMatch);
				int ichOrc = candidate.IndexOf(kszObject, StringComparison.Ordinal);
				if (ichOrc >= 0)
				{
					ich += ichOrc;
					continue;
				}
				int ichMatch = 0;
				do
				{
					ichMatch = s2.IndexOf(candidate, ichMatch, StringComparison.Ordinal);
					if (ichMatch < 0)
						break;
					bestCandidate = candidate;
					if (ich + cchMatch == s1.Length || s1[ich + cchMatch] == kChObject)
						break;
					if (!Char.IsLetter(s1[ich + cchMatch]))
					{
						if (!Char.IsWhiteSpace(s1[ich + cchMatch]))
							candidate = s1.Substring(ich, cchMatch + 1); // include punctuation
						cchMatch++;
						//if (cchMatch > maxLength)
						//    break;
					}
					else
					{
						do
						{
							cchMatch++;
						}
						while (ich + cchMatch < s1.Length && char.IsLetter(s1[ich + cchMatch])); // Need CPE?
						//if (cchMatch > maxLength)
						//    break;
						candidate = s1.Substring(ich, cchMatch);
					}
				} while (true);
				if (bestCandidate.Trim().Length > bestMatch.Trim().Length)
					bestMatch = bestCandidate;
				if (char.IsLetter(s1[ich]))
				{
					ich = s1.IndexOf(" ", ich, StringComparison.Ordinal);
					if (ich < 0)
						break;
				}
			}

			if (bestMatch.Length > 0 || wholeWordOnly)
				return bestMatch;

			foundWholeWords = false;

			string longestStr, shortestStr;
			if (s1.Length > s2.Length)
			{
				longestStr = s1;
				shortestStr = s2;
			}
			else
			{
				longestStr = s2;
				shortestStr = s1;
			}
			int cchMinUsefulMatch = (int)(.15 * shortestStr.Length);
			int shortestLen = shortestStr.Length;
			int cchBestMatch = 0;
			for (int ich = 0; ich < shortestLen - cchMinUsefulMatch; ich++)
			{
				int cchMatch = cchMinUsefulMatch;
				string bestCandidate = string.Empty;
				string candidate = shortestStr.Substring(ich, cchMatch);
				int ichOrc = candidate.IndexOf(kszObject, StringComparison.Ordinal);
				if (ichOrc >= 0)
				{
					ich += ichOrc;
					continue;
				}
				int ichMatch = 0;
				do
				{
					ichMatch = longestStr.IndexOf(candidate, ichMatch, StringComparison.Ordinal);
					if (ichMatch < 0 || ichMatch < shortestLen && shortestStr[ichMatch] == kChObject)
						break;
					bestCandidate = candidate;
					if (ich + cchMatch == shortestLen)
						break;
                    if (shortestStr[ich + cchMatch] == kChObject)
                        break;
					candidate = shortestStr.Substring(ich, ++cchMatch);
				} while (true);
				if (cchMatch > cchBestMatch && bestCandidate.Any(c => !Char.IsWhiteSpace(c)))
				{
					cchMinUsefulMatch = cchBestMatch = cchMatch;
					bestMatch = bestCandidate;
				}
			}

			return bestMatch;
		}
	}

	#region StrLengthComparer class
	/// -----------------------------------------------------------------------------------------
	/// <summary>
	/// Class to compare strings and sort by length
	/// </summary>
	/// -----------------------------------------------------------------------------------------
	public class StrLengthComparer : IComparer<string>
	{
		private readonly int m_asc;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="StrLengthComparer"/> class that sorts
		/// from shortest to longest.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public StrLengthComparer() : this(true)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="StrLengthComparer"/> class.
		/// </summary>
		/// <param name="ascending">if set to <c>true</c>, strings will be sorted from shortest
		/// to longest; otherwise, from longest to shortest.</param>
		/// ------------------------------------------------------------------------------------
		public StrLengthComparer(bool ascending)
		{
			m_asc = ascending ? 1 : -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Comparison method
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int Compare(string obj1, string obj2)
		{
			return m_asc * (obj1.Length - obj2.Length);
		}
	}
	#endregion
}
