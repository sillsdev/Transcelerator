// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2012, SIL International. All Rights Reserved.
// <copyright from='2011' to='2012' company='SIL International'>
//		Copyright (c) 2012, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: PhraseTranslationHelper.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AddInSideViews;
using SIL.Utils;
using System;

namespace SIL.Transcelerator
{
	/// --------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// --------------------------------------------------------------------------------------------
    internal class PhraseTranslationHelper : IPhraseTranslationHelper
	{
		#region Events
		public event Action TranslationsChanged;
		#endregion

		#region Data members
		private readonly List<TranslatablePhrase> m_phrases = new List<TranslatablePhrase>();
		private readonly List<Part> m_allParts;
		private List<TranslatablePhrase> m_filteredPhrases;
		private readonly Dictionary<int, TranslatablePhrase> m_categories = new Dictionary<int, TranslatablePhrase>(2);
		private readonly Dictionary<TypeOfPhrase, string> m_initialPunct = new Dictionary<TypeOfPhrase, string>();
		private readonly Dictionary<TypeOfPhrase, string> m_finalPunct = new Dictionary<TypeOfPhrase, string>();
		private bool m_justGettingStarted = true;
		private DataFileAccessor m_fileAccessor;
		private List<RenderingSelectionRule> m_termRenderingSelectionRules;
		private SortBy m_listSortCriterion = SortBy.Default;
		private bool m_listSortedAscending = true;
		/// <summary>Indicates whether the filtered list's sorting has been done</summary>
		private bool m_listSorted = false;

		private const int kAscending = 1;
		private const int kDescending = -1;
		#endregion

		#region SortBy enumeration
		public enum SortBy
		{
			Default,
			Reference,
			EnglishPhrase,
			Translation,
			Status,
		}
		#endregion

		#region KeyTermFilterType enumeration
		public enum KeyTermFilterType
		{
			All = 0,
			WithRenderings = 1,
			WithoutRenderings = 2,
		}
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="PhraseTranslationHelper"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public PhraseTranslationHelper(QuestionProvider qp)
		{
			TranslatablePhrase.s_helper = this;
		    m_allParts = qp.AllTranslatableParts.ToList();

			foreach (TranslatablePhrase phrase in qp.Where(p => !string.IsNullOrEmpty(p.PhraseToDisplayInUI)))
			{
				m_phrases.Add(phrase);
				if (phrase.Category == -1)
					m_categories[phrase.SequenceNumber] = phrase;
			}

			m_filteredPhrases = m_phrases;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sorts the list of phrases in the specified way.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Sort(SortBy by, bool ascending)
		{
			if (m_listSortCriterion != by)
			{
				m_listSortCriterion = by;
				m_listSortedAscending = ascending;
				m_listSorted = false;
			}
			else if (m_listSortedAscending != ascending)
			{
				if (m_listSorted)
					m_filteredPhrases.Reverse();
				else
					m_listSortedAscending = ascending;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sorts the specified list of phrases in the specified way.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void SortList(List<TranslatablePhrase> phrases, SortBy by, bool ascending)
		{
			if (by == SortBy.Default)
			{
				phrases.Sort();
				if (!ascending)
					phrases.Reverse();
				return;
			}

			Comparison<TranslatablePhrase> how;
			int direction = ascending ? kAscending : kDescending;
			switch (by)
			{
				case SortBy.Reference:
					how = PhraseReferenceComparison(direction);
					break;
				case SortBy.EnglishPhrase:
					how = (a, b) => a.PhraseInUse.CompareTo(b.PhraseInUse) * direction;
					break;
				case SortBy.Translation:
					how = (a, b) => a.Translation.CompareTo(b.Translation) * direction;
					break;
				case SortBy.Status:
					how = (a, b) => a.HasUserTranslation.CompareTo(b.HasUserTranslation) * direction;
					break;
				default:
					throw new ArgumentException("Unexpected sorting method", "by");
			}
			phrases.Sort(how);
		}

		private static Comparison<TranslatablePhrase> PhraseReferenceComparison(int direction)
		{
			return (a, b) =>
			{
				int val = a.StartRef.CompareTo(b.StartRef);
				if (val == 0)
				{
					val = a.Category.CompareTo(b.Category);
					if (val == 0)
					{
						val = a.EndRef.CompareTo(b.EndRef);
						if (val == 0)
						{
							val = a.SequenceNumber.CompareTo(b.SequenceNumber);
						}
					}
				}
				return val * direction;
			};
		}
		#endregion

		#region Public methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the (first) phrase in the collection that matches the given text for the given
		/// reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase GetPhrase(string reference, string englishPhrase)
		{
			englishPhrase = englishPhrase.Normalize(NormalizationForm.FormC);
			return m_phrases.FirstOrDefault(x => (reference == null || x.PhraseKey.ScriptureReference == reference) &&
				x.PhraseKey.Text == englishPhrase);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of the (first) phrase in the (filtered) collection that matches the
		/// given key.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int FindPhrase(QuestionKey phraseKey)
		{
			for (int i = 0; i < FilteredSortedPhrases.Count; i++)
			{
				TranslatablePhrase phrase = FilteredSortedPhrases[i];
				if (phrase.PhraseKey.Matches(phraseKey))
					return i;
			}
			return -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrases (filtered and sorted).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> Phrases
		{
			get { return FilteredSortedPhrases; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the filtered phrases, sorting them first if needed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private List<TranslatablePhrase> FilteredSortedPhrases
		{
			get
			{
				if (!m_listSorted)
				{
					SortList(m_filteredPhrases, m_listSortCriterion, m_listSortedAscending);
					m_listSorted = true;
				}
				return m_filteredPhrases;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the term-rendering selection rules.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public List<RenderingSelectionRule> TermRenderingSelectionRules
		{
			get { return m_termRenderingSelectionRules; }
			internal set
			{
				m_termRenderingSelectionRules = value;
				if (m_fileAccessor != null)
					m_fileAccessor.Write(DataFileAccessor.DataFileId.TermRenderingSelectionRules,
                        XmlSerializationHelper.SerializeToString(m_termRenderingSelectionRules));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the complete list of phrases sorted by reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal IEnumerable<TranslatablePhrase> UnfilteredPhrases
		{
			get
			{
				List<TranslatablePhrase> temp = m_phrases.GetRange(0, m_phrases.Count);
				temp.Sort(PhraseReferenceComparison(kAscending));
				return temp;
			}
		}

		internal DataFileAccessor FileProxy
		{
			set
			{
				m_fileAccessor = value;
				m_termRenderingSelectionRules =
                    XmlSerializationHelper.LoadOrCreateListFromString<RenderingSelectionRule>(
                    m_fileAccessor.Read(DataFileAccessor.DataFileId.TermRenderingSelectionRules), true);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of phrases/questions matching the applied filter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal int FilteredPhraseCount
		{
			get { return m_filteredPhrases.Count; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the total number of phrases/questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal int UnfilteredPhraseCount
		{
			get { return m_phrases.Count; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translation of the requested category; if not translated, use the English.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string GetCategoryName(int categoryId)
		{
			string catName = m_categories[categoryId].Translation;
			if (string.IsNullOrEmpty(catName))
				catName = m_categories[categoryId].PhraseToDisplayInUI;
			return catName;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the translatable phrase at the specified <paramref name="index"/>.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase this[int index]
		{
			get { return FilteredSortedPhrases[index]; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Filters the list of translatable phrases.
		/// </summary>
		/// <param name="partMatchString">String to filter "translatable" parts (not key term
		/// parts).</param>
		/// <param name="wholeWordMatch">If set to <c>true</c> the match string will only match
		/// complete words.</param>
		/// <param name="ktFilter">The type of Key Terms filter to apply.</param>
		/// <param name="refFilter">The reference filter delegate (params are startRef, endRef,
		/// and string representation of reference).</param>
		/// <param name="fShowExcludedQuestions">if set to <c>true</c> show excluded questions.
		/// </param>
		/// ------------------------------------------------------------------------------------
		public void Filter(string partMatchString, bool wholeWordMatch, KeyTermFilterType ktFilter,
			Func<int, int, string, bool> refFilter, bool fShowExcludedQuestions)
		{
			Func<int, int, string, bool> filterByRef = refFilter ?? new Func<int, int, string, bool>((start, end, sref) => true);
			
			m_listSorted = false;

			if (string.IsNullOrEmpty(partMatchString))
			{
				if (ktFilter != KeyTermFilterType.All)
					m_filteredPhrases = m_phrases.Where(phrase => phrase.MatchesKeyTermFilter(ktFilter) &&
						filterByRef(phrase.StartRef, phrase.EndRef, phrase.Reference) &&
						(fShowExcludedQuestions || !phrase.IsExcluded)).ToList();
				else if (refFilter != null)
					m_filteredPhrases = m_phrases.Where(phrase => filterByRef(phrase.StartRef, phrase.EndRef, phrase.Reference) &&
						(fShowExcludedQuestions || !phrase.IsExcluded)).ToList();
				else if (!fShowExcludedQuestions)
					m_filteredPhrases = m_phrases.Where(phrase => !phrase.IsExcluded).ToList();
				else
					m_filteredPhrases = m_phrases;
				return;
			}

			partMatchString = Regex.Escape(partMatchString.Normalize(NormalizationForm.FormC));
			if (wholeWordMatch)
				partMatchString = @"\b" + partMatchString + @"\b";
			Regex regexFilter = new Regex(partMatchString,
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			m_filteredPhrases = m_phrases.Where(phrase => regexFilter.IsMatch(phrase.PhraseInUse) &&
				phrase.MatchesKeyTermFilter(ktFilter) &&
				filterByRef(phrase.StartRef, phrase.EndRef, phrase.Reference) &&
				(fShowExcludedQuestions || !phrase.IsExcluded)).ToList();
		}
		#endregion

		#region Private and internal methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes a new translation on a phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void IPhraseTranslationHelper.ProcessTranslation(TranslatablePhrase tp)
		{
			string initialPunct, finalPunct;

			StringBuilder bldr = new StringBuilder();
			if (tp.Translation.StartsWith("\u00BF"))
				bldr.Append(tp.Translation[0]);
			if (bldr.Length > 0 && bldr.Length < tp.Translation.Length)
				m_initialPunct[tp.TypeOfPhrase] = initialPunct = bldr.ToString();
			else
				initialPunct = InitialPunctuationForType(tp.TypeOfPhrase);

			bldr.Length = 0;
			foreach (char t in tp.Translation.Reverse().TakeWhile(Char.IsPunctuation))
				bldr.Insert(0, t);
			if (bldr.Length > 0 && bldr.Length < tp.Translation.Length)
				m_finalPunct[tp.TypeOfPhrase] = finalPunct = bldr.ToString();
			else
				finalPunct = InitialPunctuationForType(tp.TypeOfPhrase);

			List<Part> tpParts = tp.TranslatableParts.ToList();
			if (tpParts.Count == 0)
				return;

			string translation = tp.GetTranslationTemplate();

			foreach (TranslatablePhrase similarPhrase in tpParts[0].OwningPhrases.Where(phrase => !phrase.HasUserTranslation && phrase.PartPatternMatches(tp)))
			{
				if (similarPhrase.PhraseInUse == tp.PhraseInUse)
					similarPhrase.Translation = tp.Translation;
				else if (tp.AllTermsMatch)
					similarPhrase.SetProvisionalTranslation(translation);
			}

			if (tp.AllTermsMatch)
			{
				if (tpParts.Count == 1)
				{
					if (translation.StartsWith(initialPunct))
						translation = translation.Remove(0, initialPunct.Length);
					if (translation.EndsWith(finalPunct))
						translation = translation.Substring(0, translation.Length - finalPunct.Length);

					tpParts[0].Translation = Regex.Replace(translation, @"\{.+\}", string.Empty).Trim();
					if (TranslationsChanged != null)
						TranslationsChanged();
					return;
				}
			}

			if (m_justGettingStarted)
				return;

			if (translation.StartsWith(initialPunct))
				translation = translation.Remove(0, initialPunct.Length);
			if (translation.EndsWith(finalPunct))
				translation = translation.Substring(0, translation.Length - finalPunct.Length);

			List<Part> unTranslatedParts = new List<Part>(tpParts);
			HashSet<Part> partsNeedingUpdating = new HashSet<Part>();
			foreach (Part part in tpParts)
			{
				partsNeedingUpdating.UnionWith(RecalculatePartTranslation(part).Where(p => !tpParts.Contains(p)));
				if (part.Translation.Length > 0)
				{
					int ichMatch = translation.IndexOf(part.Translation, StringComparison.Ordinal);
					if (ichMatch >= 0)
						translation = translation.Remove(ichMatch, part.Translation.Length);
					unTranslatedParts.Remove(part);
				}
			}
			if (unTranslatedParts.Count == 1)
				unTranslatedParts[0].Translation = Regex.Replace(translation, @"\{.+\}", string.Empty).Trim();

			foreach (Part partNeedingUpdating in partsNeedingUpdating)
				RecalculatePartTranslation(partNeedingUpdating);

			if (TranslationsChanged != null)
				TranslationsChanged();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Recalculates the part translation by considering all the owning phrases of the part
		/// and a probable translation based on what they have in common.
		/// </summary>
		/// <param name="part">The part.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		private static IEnumerable<Part> RecalculatePartTranslation(Part part)
		{
			string originalTranslation = part.Translation;

			List<string> userTranslations = new List<string>();
			foreach (TranslatablePhrase phrase in part.OwningPhrases.Where(op => op.HasUserTranslation))
			{
				string toAdd = phrase.UserTransSansOuterPunctuation;
				foreach (IPhrasePart otherPart in phrase.GetParts().Where(otherPart => otherPart != part))
				{
					if (otherPart is KeyTerm)
					{
						foreach (string ktTrans in ((KeyTerm)otherPart).Renderings)
						{
							int ich = toAdd.IndexOf(ktTrans, StringComparison.Ordinal);
							if (ich >= 0)
							{
								toAdd = toAdd.Remove(ich, ktTrans.Length).Insert(ich, StringUtils.kszObject);
								break;
							}
						}
					}
					else
					{
						if (otherPart.Translation.Length > 0)
						{
							int ichMatch = toAdd.IndexOf(otherPart.Translation, StringComparison.Ordinal);
							if (ichMatch >= 0)
								toAdd = toAdd.Remove(ichMatch, otherPart.Translation.Length).Insert(ichMatch, StringUtils.kszObject);
						}
					}
				}
				if (!string.IsNullOrEmpty(toAdd))
					userTranslations.Add(toAdd);
			}

			string commonTranslation = GetBestCommonPartTranslation(userTranslations);
			if (commonTranslation != null)
				part.Translation = commonTranslation;
			if (originalTranslation.Length > 0 && (part.Translation.Length == 0 || originalTranslation.Contains(part.Translation)))
			{
				// The translation of the part has shrunk
				return part.OwningPhrases.Where(phr => phr.HasUserTranslation).SelectMany(otherPhrases => otherPhrases.TranslatableParts).Distinct();
			}
			return new Part[0];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a common or frequently appearing substring of the given user translations that
		/// is deemed to be the most likely translation for a part that occurs in all the
		/// phrases these translations represent.
		/// </summary>
		/// <param name="userTranslations">The user translations of all the translated phrases
		/// which contain the part in question.</param>
		/// ------------------------------------------------------------------------------------
		private static string GetBestCommonPartTranslation(IList<string> userTranslations)
		{
			if (userTranslations.Count == 0)
				return null;

			string commonTranslation = GetSCommonSubstring(userTranslations);
			if (commonTranslation != null && commonTranslation.Length > 5) // 5 is a "magic number" - We don't want to accept really small words without considering a possibly better match statistically
				return commonTranslation;

			Dictionary<string, double> commonSubstrings = new Dictionary<string, double>(userTranslations.Count * 2);
			KeyValuePair<string, double> statisticallyBestSubstring = new KeyValuePair<string, double>(null, -1);
			bool fCommonSubstringIsWholeWord = false;
			bool fBestIsWholeWord = false;
			for (int i = 0; i < userTranslations.Count - 1; i++)
			{
				for (int j = i + 1; j < userTranslations.Count; j++)
				{
					string sCommonSubstring = StringUtils.LongestUsefulCommonSubstring(userTranslations[i], userTranslations[j],
						fCommonSubstringIsWholeWord, out fCommonSubstringIsWholeWord).Trim();
					if (sCommonSubstring.Length > 1 || (sCommonSubstring.Length == 1 && Char.IsLetter(sCommonSubstring[0])))
					{
						double val;
						commonSubstrings.TryGetValue(sCommonSubstring, out val);
						val += Math.Sqrt(sCommonSubstring.Length);
						commonSubstrings[sCommonSubstring] = val;
						// A whole-word match always trumps a partial-word match.
						if (val > statisticallyBestSubstring.Value || (!fBestIsWholeWord && fCommonSubstringIsWholeWord))
						{
							statisticallyBestSubstring = new KeyValuePair<string, double>(sCommonSubstring, val);
							fBestIsWholeWord = fCommonSubstringIsWholeWord;
						}
					}
				}
			}
			int totalComparisons = ((userTranslations.Count * userTranslations.Count) + userTranslations.Count) / 2;
			return (string.IsNullOrEmpty(commonTranslation) || statisticallyBestSubstring.Value > totalComparisons) ?
				statisticallyBestSubstring.Key : commonTranslation;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the longest substring common to all the given strings. This can't return less
		/// than a whole word.
		/// TODO: Handle agglutinative languages
		/// </summary>
		/// <param name="strings">The strings to consider.</param>
		/// ------------------------------------------------------------------------------------
		public static string GetSCommonSubstring(IList<string> strings)
		{
			string firstOne = strings[0];
			string sCommonSubstring;
			if (strings.Count == 1)
				sCommonSubstring = string.Empty;
			else
			{
				bool fCommonSubstringIsWholeWord;
				sCommonSubstring = StringUtils.LongestUsefulCommonSubstring(firstOne, strings[1],
					true, out fCommonSubstringIsWholeWord);
				for (int i = 2; i < strings.Count; i++)
				{
					string sPossibleCommonSubstring = StringUtils.LongestUsefulCommonSubstring(strings[i], sCommonSubstring,
						true, out fCommonSubstringIsWholeWord);
					if (sPossibleCommonSubstring.Length < sCommonSubstring.Length)
					{
						i = 1;
						int ichCommonPiece = sPossibleCommonSubstring.Length == 0 ? -1 :
							sCommonSubstring.IndexOf(sPossibleCommonSubstring, StringComparison.Ordinal);
						if (ichCommonPiece < 0)
							firstOne = firstOne.Replace(sCommonSubstring, StringUtils.kszObject);
						else
						{
							if (ichCommonPiece > 0)
								firstOne = firstOne.Replace(sCommonSubstring.Substring(0, ichCommonPiece), StringUtils.kszObject);
							if (ichCommonPiece + sPossibleCommonSubstring.Length < sCommonSubstring.Length)
							{
								firstOne = firstOne.Replace(sCommonSubstring.Substring(ichCommonPiece + sPossibleCommonSubstring.Length),
									StringUtils.kszObject);
							}
						}
						sCommonSubstring = StringUtils.LongestUsefulCommonSubstring(firstOne, strings[i],
							true, out fCommonSubstringIsWholeWord);
					}
				}
			}
			return sCommonSubstring;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the character(s) that normally occur at the beginning of a phrase/question of
        /// the specified type. Returns an empty string (never null) if no such punctuation has
        /// been determined.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string InitialPunctuationForType(TypeOfPhrase type)
		{
			string p;
			return m_initialPunct.TryGetValue(type, out p) ? p : string.Empty;
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the character(s) that normally occur at the end of a phrase/question of
        /// the specified type. Returns an empty string (never null) if no such punctuation has
        /// been determined.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string FinalPunctuationForType(TypeOfPhrase type)
		{
			string p;
			return m_finalPunct.TryGetValue(type, out p) ? p : string.Empty;
		}
		#endregion

		internal void ProcessAllTranslations()
		{
			if (!m_justGettingStarted)
				throw new InvalidOperationException("This method should only be called once, after all the saved translations have been loaded.");

			foreach (Part part in m_allParts)
			{
				if (part.OwningPhrases.Where(p => p.HasUserTranslation).Skip(1).Any()) // Must have at least 2 phrases with translations
					RecalculatePartTranslation(part);
			}

			m_justGettingStarted = false;
		}
	}

    public interface IPhraseTranslationHelper
    {
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Processes a new translation on a phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        void ProcessTranslation(TranslatablePhrase tp);

        string InitialPunctuationForType(TypeOfPhrase type);

        string FinalPunctuationForType(TypeOfPhrase type);

        string GetCategoryName(int category);

        List<RenderingSelectionRule> TermRenderingSelectionRules { get; }
    }
}
