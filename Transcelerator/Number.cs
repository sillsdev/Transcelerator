// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: Number.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SIL.Transcelerator
{
    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Class representing a numeric (integer) part (usually used for large numbers in Scripture)
    /// </summary>
    // ---------------------------------------------------------------------------------------------
    public sealed class Number : IPhrasePart
    {
		internal delegate void OnNumberFormattingChangedHandler();

        #region Data members
        private readonly int m_value;
        private string m_vernacular = null;
        #endregion

		#region Statics
	    private static NumberFormatInfo s_numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
		// 3000 vs. 3,000
	    private static bool s_fNoGroupPunctForShortNumbers = false;

		internal static event OnNumberFormattingChangedHandler OnNumberFormattingChanged;
		#endregion

		#region Construction and initialization
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Number"/> class.
		/// </summary>
		/// <param name="value">The numeric value.</param>
		/// ------------------------------------------------------------------------------------
		internal Number(int value)
		{
            m_value = value;
		}
        #endregion

        #region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the numeric value of the number
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int NumericValue
		{
		    get { return m_value; }
		}

	    /// ------------------------------------------------------------------------------------
        /// <summary>
        /// A number is always a single "Word"
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<Word> Words
        {
			get { yield return m_value.ToString(CultureInfo.InvariantCulture); }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
		/// Gets the "translation", which is just the number expressed in the vernacular script,
		/// formatted apprpriately.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string Translation
        {
            get
            {
	            if (m_vernacular == null)
	            {
		            m_vernacular = (s_fNoGroupPunctForShortNumbers &&
						s_numberFormatInfo.NumberGroupSizes.Length > 0 &&
						NumberOfDigits == s_numberFormatInfo.NumberGroupSizes[0] + 1) ?
						m_value.ToString(CultureInfo.InvariantCulture) :
						m_value.ToString("n0", s_numberFormatInfo);
		            if (s_numberFormatInfo.NativeDigits[0] != "0")
		            {
						StringBuilder sb = new StringBuilder(m_vernacular);
			            for (int i = 0; i < sb.Length; i++)
			            {
				            if (char.IsDigit(sb[i]))
					            sb[i] = s_numberFormatInfo.NativeDigits[(int) char.GetNumericValue(sb[i])][0];
			            }
			            m_vernacular = sb.ToString();
		            }
					OnNumberFormattingChanged += ClearProvisionalFormatting;
	            }
	            return m_vernacular;
            }
	        internal set
	        {
				if (!string.IsNullOrEmpty(m_vernacular))
					OnNumberFormattingChanged -= ClearProvisionalFormatting;

		        if (string.IsNullOrEmpty(value))
			        m_vernacular = null;
				else if (value != Translation)
				{
					m_vernacular = value;
					List<int> numberGroupSizes = new List<int>(2);
					int groupSize = 0;
					string sGroupPunct = string.Empty;
					foreach (char c in value.Reverse())
					{
						if (char.IsDigit(c))
							groupSize++;
						else
						{
							if (groupSize > 0)
							{
								numberGroupSizes.Add(groupSize);
								groupSize = 0;
							}
							if (numberGroupSizes.Count == 1)
								sGroupPunct = c + sGroupPunct;
						}
					}
					if (sGroupPunct == string.Empty)
						sGroupPunct = s_numberFormatInfo.NumberGroupSeparator;

					bool fNoGroupPunctForShortNumbers = s_fNoGroupPunctForShortNumbers;
					if (numberGroupSizes.Count == 0 && s_numberFormatInfo.NumberGroupSizes.Length > 0 &&
						groupSize == s_numberFormatInfo.NumberGroupSizes[0] + 1)
					{
						fNoGroupPunctForShortNumbers = true;
					}
		
					// Don't overwrite a previously determined set of number groupings unless this new
					// rendering definitely changes something by:
					// 1) increasing the number of groups (typically because this number has more digits
					//    than anything encountered previously.
					// 2) altering an existing grouping (e.g., 1,000,000 -> 10,00,000 changes the second
					//    group from 3 digits to 2 digits)
					// 3) removing an existing grouping. Removal must be prevented if this number doesn't
					//    have enough digits to "prove" that it represents an actual change. For example,
					//    if a previous number was formatted as 1,000,000,000 and now we're formatting
					//    100,000,000 (with only has two groups instead of 3), we don't remove the
					//    highest-level grouping because the number were formatting now is only 9 digits
					//    long, which does not exceed the number of digits already accounted for by the
					//    existing groupings: 3 + 3 + 3 = 9.
					if (s_numberFormatInfo.NumberGroupSizes.Length > numberGroupSizes.Count &&
						s_numberFormatInfo.NumberGroupSizes.Take(numberGroupSizes.Count).SequenceEqual(numberGroupSizes) &&
						s_numberFormatInfo.NumberGroupSizes.Sum() > NumberOfDigits)
					{
						numberGroupSizes = new List<int>(s_numberFormatInfo.NumberGroupSizes);
					}

					SetFormat(value[0], sGroupPunct, numberGroupSizes, fNoGroupPunctForShortNumbers);
				}
			}
        }

		/// ------------------------------------------------------------------------------------
		private void ClearProvisionalFormatting()
		{
			m_vernacular = null;
			OnNumberFormattingChanged -= ClearProvisionalFormatting;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a string that can be used to display this number in a way that is culturally
        /// appropriate in the current culture.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public override string ToString()
        {
	        return m_value.ToString(CultureInfo.CurrentCulture);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a string that can be displayed for "debugging" purposes (currently shown in the
        /// last column of the grid).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string DebugInfo
        {
            get { return ToString(); }
        }
        #endregion

        #region Public methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// The "best rendering" for a number is always just the number expressed in the
        /// vernacular script, formatted apprpriately.
        /// </summary>
        /// <remarks>If this term occurs more than once in the phrase, it is not possible to
        /// know which occurrence is which.</remarks>
        /// ------------------------------------------------------------------------------------
        public string GetBestRenderingInContext(TranslatablePhrase phrase)
        {
            return Translation;
        }
        #endregion

		#region Private helper methods/properties
		/// ------------------------------------------------------------------------------------
		private int NumberOfDigits
		{
			get { return (int) (Math.Log10(m_value) + 1); }
		}

	    /// ------------------------------------------------------------------------------------
		private static void SetFormat(char exampleDigit, string groupingPunctuation,
			IEnumerable<int> digitGroups, bool fNoGroupPunctForShortNumbers)
		{
			char nativeZero = (char)(exampleDigit - (int)char.GetNumericValue(exampleDigit));
			if (nativeZero.ToString(CultureInfo.InvariantCulture) != s_numberFormatInfo.NativeDigits[0] ||
				groupingPunctuation != s_numberFormatInfo.NumberGroupSeparator ||
				s_fNoGroupPunctForShortNumbers != fNoGroupPunctForShortNumbers ||
				(!s_numberFormatInfo.NumberGroupSizes.SequenceEqual(digitGroups)))
			{
				s_numberFormatInfo = new NumberFormatInfo();
				s_numberFormatInfo.DigitSubstitution = DigitShapes.NativeNational;
				var nativeDigits = new string[10];
				for (int i = 0; i <= 9; i++)
					nativeDigits[i] = ((char) (nativeZero + i)).ToString(CultureInfo.InvariantCulture);

				s_numberFormatInfo.NativeDigits = nativeDigits;

				s_numberFormatInfo.NumberGroupSeparator = groupingPunctuation;

				s_numberFormatInfo.NumberGroupSizes = digitGroups.ToArray();

				s_fNoGroupPunctForShortNumbers = fNoGroupPunctForShortNumbers;

				if (OnNumberFormattingChanged != null)
					OnNumberFormattingChanged();
			}
		}
		#endregion
	}
}


