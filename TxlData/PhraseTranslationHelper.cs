// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: PhraseTranslationHelper.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Globalization;
using Paratext.PluginInterfaces;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using static System.String;
using static System.Char;

namespace SIL.Transcelerator
{
	#region SortBy enumeration
	public enum PhrasesSortedBy
	{
		Default,
		Reference,
		EnglishPhrase,
		Translation,
		Status,
	}
	#endregion

	#region VariantType enumeration
	public enum VariantType
	{
		/// <summary>
		/// Not a variant
		/// </summary>
		None,
		/// <summary>
		/// One or both of the variants in the pair consist of a series of questions (i.e., there
		/// are 3 or more total questions)
		/// </summary>
		SeriesOfQuestions,
		/// <summary>
		/// Each variant in the pair consists of a single question
		/// </summary>
		SingleQuestions
	}
	#endregion

	/// --------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// --------------------------------------------------------------------------------------------
    public class PhraseTranslationHelper : IPhraseTranslationHelper
	{
		private const string kDefaultLang = "en-US";

		private readonly ILocalizationsProvider m_dataLoc;

		#region Events
		public event Action TranslationsChanged;
		#endregion
		#region Data members
		private readonly List<TranslatablePhrase> m_phrases = new List<TranslatablePhrase>();
		private readonly PhrasePartManager m_phrasePartManager;
		private List<TranslatablePhrase> m_filteredPhrases;
		private readonly Dictionary<int, TranslatablePhrase> m_categories = new Dictionary<int, TranslatablePhrase>(2);
		private readonly Dictionary<TypeOfPhrase, string> m_initialPunct = new Dictionary<TypeOfPhrase, string>();
		private readonly Dictionary<TypeOfPhrase, string> m_finalPunct = new Dictionary<TypeOfPhrase, string>();
		private bool m_justGettingStarted = true;
		private DataFileAccessor m_fileAccessor;
		private List<RenderingSelectionRule> m_termRenderingSelectionRules;
		private PhrasesSortedBy m_listSortCriterion = PhrasesSortedBy.Default;
		private bool m_listSortedAscending = true;
		/// <summary>Indicates whether the filtered list's sorting has been done</summary>
		private bool m_listSorted = false;
		private bool m_sortIsDirty = false;

		private const int kAscending = 1;
		private const int kDescending = -1;
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
		public PhraseTranslationHelper(QuestionProvider qp, ILocalizationsProvider dataLoc = null)
		{
			m_dataLoc = dataLoc;
			m_phrasePartManager = qp.PhrasePartManager;
			qp.Helper = this;

			foreach (TranslatablePhrase phrase in qp.Where(p => p.TypeOfPhrase == TypeOfPhrase.NoEnglishVersion || !IsNullOrEmpty(p.PhraseInUse)))
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
		public void Sort(PhrasesSortedBy by, bool ascending, bool immediate = false)
		{
			if (m_sortIsDirty)
				m_listSorted = false;

			if (m_listSortCriterion != by)
			{
				m_listSortCriterion = by;
				m_listSortedAscending = ascending;
				m_listSorted = false;
			}
			else if (m_listSortedAscending != ascending)
			{
				if (m_listSorted)
				{
					m_filteredPhrases.Reverse();
					return;
				}
				m_listSortedAscending = ascending;
			}

			if (immediate)
				SortList();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Applies the currently specified sort criterion to the list of (filtered) phrases).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SortList()
		{
			if (m_listSortCriterion == PhrasesSortedBy.Default)
			{
				m_filteredPhrases.Sort();
				if (!m_listSortedAscending)
					m_filteredPhrases.Reverse();
				return;
			}

			Comparison<TranslatablePhrase> how;
			int direction = m_listSortedAscending ? kAscending : kDescending;
			switch (m_listSortCriterion)
			{
				case PhrasesSortedBy.Reference:
					how = PhraseReferenceComparison(direction);
					break;
				case PhrasesSortedBy.EnglishPhrase:
					how = (a, b) => Compare(a.PhraseInUse, b.PhraseInUse, StringComparison.InvariantCulture) * direction;
					break;
				case PhrasesSortedBy.Translation:
					if (VernacularStringComparer == null)
						how = (a, b) => Compare(a.GetTranslation(true), b.GetTranslation(true), StringComparison.InvariantCulture) * direction;
					else
						how = (a, b) => VernacularStringComparer.Compare(a.GetTranslation(true), b.GetTranslation(true)) * direction;
					break;
				case PhrasesSortedBy.Status:
					how = (a, b) => a.HasUserTranslation.CompareTo(b.HasUserTranslation) * direction;
					break;
				default:
					throw new InvalidOperationException("Unexpected sorting method");
			}
			m_filteredPhrases.Sort(how);
		}

		private static Comparison<TranslatablePhrase> NaturalOrderComparison() => ComparePhrasesByIndexedOrder;

		private static Comparison<TranslatablePhrase> PhraseReferenceComparison(int direction) =>
			(a, b) => ComparePhraseReferences(a, b, direction);

		public static int ComparePhraseReferences(TranslatablePhrase a, TranslatablePhrase b, int direction = kAscending)
		{
			if (a == b)
				return 0;
			int val = a.StartRef.CompareTo(b.StartRef);
			if (val == 0)
			{
				{
					val = a.SectionId.CompareTo(b.SectionId);
					if (val == 0)
					{
						val = a.Category.CompareTo(b.Category);
						if (val == 0)
						{
							val = a.SequenceNumber.CompareTo(b.SequenceNumber);
						}
					}
				}
			}

			return val * direction;
		}

		public static int ComparePhrasesByIndexedOrder(TranslatablePhrase a, TranslatablePhrase b)
		{
			if (a == b)
				return 0;
			var val = BCVRef.GetBookFromBcv(a.StartRef).CompareTo(BCVRef.GetBookFromBcv(b.StartRef));
			if (val != 0)
				return val;
			val = a.SectionId.CompareTo(b.SectionId);
			if (val != 0)
				return val;
			val = a.Category.CompareTo(b.Category);
			if (val != 0)
				return val; 
			val = a.SequenceNumber.CompareTo(b.SequenceNumber);
			Debug.Assert(val != 0);
			return val;
		}
		#endregion

		#region Public methods and properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the (first) phrase in the collection that matches the given text for the given
		/// reference. If no exact match, it will try to find one for the same book and chapter
		/// if there is exactly one.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase GetPhrase(string reference, string englishPhrase)
		{
			if (reference != null)
				reference = reference.Replace(':', '.');
			englishPhrase = englishPhrase.Normalize(NormalizationForm.FormC);
			var phrase = m_phrases.FirstOrDefault(x => (reference == null || x.PhraseKey.ScriptureReference == reference) &&
				x.PhraseKey.Text == englishPhrase);
			if (phrase == null)
			{
				foreach (TranslatablePhrase x in m_phrases.Where(p => p.AlternateForms != null))
				{
					if ((reference == null || x.PhraseKey.ScriptureReference == reference) && x.AlternateForms.Contains(englishPhrase))
					{
						phrase = x;
						break;
					}
				}

				if (phrase == null && reference != null)
				{
					var iEndOfChapter = reference.IndexOf(".", StringComparison.InvariantCulture);
					//                                                  0123456789
					// "Magic numbers" based on a reference in the form ABC 0.0
					// TO                                               ABC 000.000
					if (iEndOfChapter >= 5 && iEndOfChapter <= 7)
					{
						reference = reference.Substring(0, iEndOfChapter + 1);
						try
						{
							phrase = m_phrases.SingleOrDefault(x => (x.PhraseKey.ScriptureReference.StartsWith(reference)) &&
								x.PhraseInUse == englishPhrase && !x.IsExcluded);
						}
						catch (InvalidOperationException)
						{
							phrase = null;
						}

					}
				}
			}
			return phrase;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of the (first) phrase in the (filtered) collection that matches the
		/// given key.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int FindPhrase(IQuestionKey phraseKey)
		{
			return FilteredSortedPhrases.FindIndex(p => p.PhraseKey.Matches(phraseKey));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the questions/phrases for the given reference.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public List<TranslatablePhrase> GetMatchingPhrases(int startRef, int endRef)
		{
			return m_phrases.Where(p => p.PhraseKey.StartRef == startRef && p.PhraseKey.EndRef == endRef).ToList();
		}

		#region Question group stuff
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the questions/phrases that belong to the specified variant (group).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> GetPhrasesInVariant(string scrRef, string variantId)
		{
			return m_phrases.Where(p => p.Reference == scrRef && p.VariantId == variantId);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the questions/phrases that belong to the alternate variant of the one
		/// specified.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> GetPhrasesInOtherVariant(string scrRef,
			string variantId)
		{
			return m_phrases.Where(p => p.Reference == scrRef &&
				p.VariantId == variantId.OtherVariantId());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether the given phrase/question is part of a variant for which the user
		/// has not yet made a decision to exclude one of the two variants in the pair.
		/// </summary>
		/// <param name="phrase">The phrase/question to check</param>
		/// ------------------------------------------------------------------------------------
		public bool IsInVariantPairPendingUserDecision(TranslatablePhrase phrase)
		{
			return !phrase.IsExcluded && phrase.IsPartOfVariant &&
				GetPhrasesInOtherVariant(phrase.Reference, phrase.VariantId)
					.Any(p => !p.IsExcluded);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets whether the two questions pertain to the same variant pair.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool InSameVariantPair(TranslatablePhrase a, TranslatablePhrase b)
		{
			return (a.VariantId == b.VariantId || a.VariantId == b.VariantId?.OtherVariantId()) &&
				a.Reference == b.Reference;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// For the given phrase/question, gets whether it is not part of a variant at all, it is
		/// part of a variant pair where one or both consists of a series of questions (i.e., 3
		/// or more total questions), or it is part of a pair of variants consisting of just two
		/// individual questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public VariantType GetVariantType(TranslatablePhrase question)
		{
			if (!question.IsPartOfVariant)
				return VariantType.None;
			if (GetPhrasesInVariant(question.Reference, question.VariantId).Count() > 1 ||
			    GetPhrasesInOtherVariant(question.Reference, question.VariantId).Count() > 1)
				return VariantType.SeriesOfQuestions;
			return VariantType.SingleQuestions;
		}
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrases (filtered and sorted).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> Phrases => FilteredSortedPhrases;

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
					SortList();
					m_listSorted = true;
					m_sortIsDirty = false;
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
			get => m_termRenderingSelectionRules;
			set
			{
				m_termRenderingSelectionRules = value;
				m_fileAccessor?.Write(DataFileAccessor.DataFileId.TermRenderingSelectionRules,
					m_termRenderingSelectionRules);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the complete list of phrases in the natural order in which they should occur in
		/// the script. (Note: This is mostly in order by reference, but there are occasionally
		/// questions which are intentionally asked out of order.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IReadOnlyList<TranslatablePhrase> UnfilteredPhrases
		{
			get
			{
				var temp = m_phrases.GetRange(0, m_phrases.Count);
				temp.Sort(NaturalOrderComparison());
				return temp;
			}
		}

		public IEnumerable<TranslatablePhrase> AllActivePhrasesWhere(Func<TranslatablePhrase, bool> include)
		{
			return UnfilteredPhrases.Where(tp => tp.Category > -1 && include(tp) && !tp.IsExcluded);
		}

		public IEnumerable<ComprehensionCheckingQuestionsForBook> GetQuestionsForBooks(
			string vernIcuLocale, IReadOnlyCollection<ILocalizationsProvider> localizations,
			IEnumerable<int> booksWithExistingSfFiles)
		{
			var e = booksWithExistingSfFiles.GetEnumerator();

			void AdvanceEnumerator()
			{
				if (!e.MoveNext())
				{
					e.Dispose();
					e = null;
				}
			}

			AdvanceEnumerator();

			var prevBook = -1;
			ComprehensionCheckingQuestionsForBook currentBookQuestions = null; 

			foreach (TranslatablePhrase phrase in AllActivePhrasesWhere(p => p.HasUserTranslation))
			{
				var question = phrase.QuestionInfo;
				var startRef = new BCVRef(phrase.StartRef);
				var currBook = startRef.Book;
				if (currBook != prevBook)
				{
					if (currentBookQuestions != null)
						yield return currentBookQuestions;

					while (e != null && currBook > e.Current)
					{
						yield return new ComprehensionCheckingQuestionsForBook
						{
							Lang = vernIcuLocale,
							BookId = BCVRef.NumberToBookCode(e.Current),
							Questions = new List<ComprehensionCheckingQuestion>()
						};
						AdvanceEnumerator();
					}
					if (e != null && currBook == e.Current)
						AdvanceEnumerator();
					
					currentBookQuestions = new ComprehensionCheckingQuestionsForBook
					{
						Lang = vernIcuLocale,
						BookId = BCVRef.NumberToBookCode(currBook),
						Questions = new List<ComprehensionCheckingQuestion>()
					};
					prevBook = currBook;
				}

				var q = new ComprehensionCheckingQuestion
				{
					Id = phrase.ImmutableKey,
					Question = GetQuestionAlternates(phrase, vernIcuLocale, localizations),
					IsOverview = !phrase.IsDetail,
					Chapter = startRef.Chapter,
					StartVerse = startRef.Verse,
					Answers = GetMultilingualStrings(question, LocalizableStringType.Answer, localizations),
					Notes = GetMultilingualStrings(question, LocalizableStringType.Note, localizations)
				};

				if (phrase.StartRef != phrase.EndRef)
				{
					var endRef = new BCVRef(phrase.EndRef);
					if (startRef.Chapter != endRef.Chapter)
						q.EndChapter = endRef.Chapter;
					q.EndVerse = endRef.Verse;
				}
				
				currentBookQuestions.Questions.Add(q);
			}

			if (currentBookQuestions != null)
				yield return currentBookQuestions;
			
			while (e != null)
			{
				yield return new ComprehensionCheckingQuestionsForBook
				{
					Lang = vernIcuLocale,
					BookId = BCVRef.NumberToBookCode(e.Current),
					Questions = new List<ComprehensionCheckingQuestion>()
				};
				AdvanceEnumerator();
			}
		}

		private StringAlt[] GetQuestionAlternates(TranslatablePhrase question, string vernIcuLocale,
			IReadOnlyCollection<ILocalizationsProvider> localizers)
		{
			var list = new List<StringAlt> {new StringAlt {Lang = vernIcuLocale, Text = question.Translation}};
			string variant = null;
			var locKey = question.ToUIDataString();
			if (vernIcuLocale != kDefaultLang)
				list.Add(new StringAlt { Lang = kDefaultLang, Text = locKey.SourceUIString });
			list.AddRange(from loc in localizers.Where(l => l.Locale != vernIcuLocale)
				where loc.TryGetLocalizedString(locKey, out variant)
				select new StringAlt { Lang = loc.Locale, Text = variant });
				
			return list.ToArray();
		}
		
		private static StringAlt[][] GetMultilingualStrings(Question question, LocalizableStringType type,
			IReadOnlyCollection<ILocalizationsProvider> localizers)
		{
			string[] sourceStrings = type == LocalizableStringType.Answer ? question.Answers : question.Notes;
			if (sourceStrings == null)
				return null;
			string variant;
			var ms = new StringAlt[sourceStrings.Length][];
			for (var i = 0; i < sourceStrings.Length; i++)
			{
				var locKey = new UIAnswerOrNoteDataString(question, type, i);
				variant = null;
				List<StringAlt> list = new List<StringAlt> {new StringAlt {Lang = kDefaultLang, Text = sourceStrings[i]}};
				list.AddRange(from loc in localizers
					where loc.TryGetLocalizedString(locKey, out variant)
					select new StringAlt {Lang = loc.Locale, Text = variant});
				ms[i] = list.ToArray();
			}
			return ms;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the list of customized (added, inserted, modified, deleted) phrases, in the
		/// order in which they should occur in the script.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public List<PhraseCustomization> CustomizedPhrases
		{
			get
			{
				var customizations = new HashSet<PhraseCustomization>(new DuplicateCustomizationPreventer());
				var allPhrases = UnfilteredPhrases;
				for (var i = 0; i < allPhrases.Count; i++)
				{
					var translatablePhrase = allPhrases[i];
					if (translatablePhrase.IsCategoryName)
						continue;
					if (translatablePhrase.IsExcludedOrModified)
						customizations.Add(new PhraseCustomization(translatablePhrase));
					if (translatablePhrase.InsertedPhraseBefore != null && !translatablePhrase.IsUserAdded)
					{
						customizations.Add(new PhraseCustomization(translatablePhrase.QuestionInfo.Text,
							translatablePhrase.InsertedPhraseBefore,
							PhraseCustomization.CustomizationType.InsertionBefore));
					}
					if (translatablePhrase.IsUserAdded)
					{
						var precedingPhrase = i > 0 ? allPhrases[i - 1] : null;
						if (precedingPhrase?.IsCategoryName == true)
							precedingPhrase = null;
						//i < allPhrases.Count - 1 && !allPhrases[i + 1].IsUserAdded && allPhrases[i + 1].InsertedPhraseBefore == translatablePhrase.QuestionInfo
						customizations.Add(GetPhraseCustomization(translatablePhrase, precedingPhrase,
							allPhrases.Skip(i + 1).FirstOrDefault(q => !q.IsUserAdded)));
					}
				}
				return customizations.ToList();
			}
		}

		/// <summary>
		/// Gets a PhraseCustomization for the addedPhrase by determining whether to hang it as
		/// an addition off the phraseBefore or as an insertion off the phraseAfter. (TXL-218: Or
		/// in rare cases, hang it off itself?) We make this determination by first looking at
		/// the section/category, then by considering the reference. If both of the adjacent phrases
		/// have the same section and category, but neither has the same reference, we arbitrarily
		/// treat it as an addition hanging off the phraseBefore.
		/// </summary>
		private static PhraseCustomization GetPhraseCustomization(TranslatablePhrase addedPhrase,
			TranslatablePhrase phraseBefore, TranslatablePhrase nonUserPhraseAfter)
		{
			bool AreInSameSectionAndCategory(TranslatablePhrase a, TranslatablePhrase b) =>
				a.SectionId == b.SectionId && a.Category == b.Category;

			if (phraseBefore == null)
			{
				Debug.Assert(nonUserPhraseAfter != null);
				if (AreInSameSectionAndCategory(nonUserPhraseAfter, addedPhrase))
				{
					return new PhraseCustomization(nonUserPhraseAfter.QuestionInfo.Text, addedPhrase.QuestionInfo,
						PhraseCustomization.CustomizationType.InsertionBefore);
				}

				Debug.Fail("TXL-218: For now, this shouldn't happen.");
				// Hang the question off itself
				return new PhraseCustomization(addedPhrase.OriginalPhrase,
					addedPhrase.QuestionInfo, PhraseCustomization.CustomizationType.InsertionBefore /* arbitrary */);
			}

			if (nonUserPhraseAfter == null)
			{
				Debug.Assert(phraseBefore != null);
				if (AreInSameSectionAndCategory(phraseBefore, addedPhrase))
				{
					return new PhraseCustomization(phraseBefore.QuestionInfo.Text, addedPhrase.QuestionInfo,
						PhraseCustomization.CustomizationType.AdditionAfter);
				}

				Debug.Fail("TXL-218: For now, this shouldn't happen.");
				// Hang the question off itself
				return new PhraseCustomization(addedPhrase.OriginalPhrase,
					addedPhrase.QuestionInfo, PhraseCustomization.CustomizationType.AdditionAfter /* arbitrary */);
			}

			if (AreInSameSectionAndCategory(phraseBefore, addedPhrase))
			{
				if (AreInSameSectionAndCategory(nonUserPhraseAfter, addedPhrase))
				{
					// All three questions are in the same section and category, so attach it to the
					// one for the same reference, if any.
					if (phraseBefore.QuestionInfo.CompareRefs(addedPhrase.QuestionInfo) == 0 ||
						(nonUserPhraseAfter.QuestionInfo.CompareRefs(addedPhrase.QuestionInfo) != 0))
					{
						return new PhraseCustomization(phraseBefore.QuestionInfo.Text,
							addedPhrase.QuestionInfo,
							PhraseCustomization.CustomizationType.AdditionAfter);
					}

					return new PhraseCustomization(nonUserPhraseAfter.QuestionInfo.Text,
						addedPhrase.QuestionInfo,
						PhraseCustomization.CustomizationType.InsertionBefore);
				}

				return new PhraseCustomization(phraseBefore.QuestionInfo.Text, addedPhrase.QuestionInfo,
					PhraseCustomization.CustomizationType.AdditionAfter);
			}

			if (AreInSameSectionAndCategory(nonUserPhraseAfter, addedPhrase))
			{
				return new PhraseCustomization(nonUserPhraseAfter.QuestionInfo.Text, addedPhrase.QuestionInfo,
					PhraseCustomization.CustomizationType.InsertionBefore);
			}

			// REVIEW (TXL-218): For now, this probably can't happen, but when we implement TXL-218,
			// we want to hang the question off itself.
			Debug.Fail("TXL-218: For now, this shouldn't happen.");
			return new PhraseCustomization(addedPhrase.OriginalPhrase,
				addedPhrase.QuestionInfo, PhraseCustomization.CustomizationType.AdditionAfter /* arbitrary */);
		}
		
		public IComparer<string> VernacularStringComparer { get; set; }

		public void SetFileProxy(DataFileAccessor value, out Exception termRenderingSelectionRulesException)
		{
			m_fileAccessor = value;
			m_termRenderingSelectionRules =
				ListSerializationHelper.LoadOrCreateListFromString<RenderingSelectionRule>(
					m_fileAccessor.Read(DataFileAccessor.DataFileId.TermRenderingSelectionRules), out termRenderingSelectionRulesException);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of phrases/questions matching the applied filter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int FilteredPhraseCount => m_filteredPhrases.Count;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the total number of phrases/questions.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int UnfilteredPhraseCount => m_phrases.Count;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translation of the requested category; if not translated, use the current
		/// UI localization (falling back to English). If translated, lang will be null;
		/// otherwise, it will be the UI locale used.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string GetCategoryName(int categoryId, out string lang)
		{
			string catName = m_categories[categoryId].Translation;
			if (IsNullOrEmpty(catName))
			{
				var englishCategoryName = m_categories[categoryId].OriginalPhrase;
				if (m_dataLoc == null)
				{
					lang = kDefaultLang;
					catName = englishCategoryName;
				}
				else
				{
					var key = new UISimpleDataString(englishCategoryName, LocalizableStringType.Category);
					lang = m_dataLoc.TryGetLocalizedString(key, out catName) ? m_dataLoc.Locale : "en";
				}
			}
			else
			{
				lang = null;
			}
			return catName;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets (UI-compatible) category names in order by ID (overview = 0, etc.)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<string> AllCategories
		{
			get
			{
				for (var i = 0; i < m_categories.Count; i++)
					yield return GetCategoryName(i, out _);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the translatable phrase at the specified <paramref name="index"/>.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase this[int index] => FilteredSortedPhrases[index];

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Filters the list of translatable phrases.
		/// </summary>
		/// <param name="partMatchString">String to filter (localized) question text (not key term
		/// parts).</param>
		/// <param name="wholeWordMatch">If set to <c>true</c> the match string will only match
		/// complete words.</param>
		/// <param name="ktFilter">The type of Key Terms filter to apply.</param>
		/// <param name="refFilter">The reference filter delegate (params are startRef, endRef,
		/// and string representation of reference).</param>
		/// <param name="fShowExcludedQuestions">if set to <c>true</c> show excluded questions.
		/// </param>
		/// <param name="getLocalizedPhraseInUse">Function for getting the localized version of a
		/// question's PhraseInUse</param>
		/// ------------------------------------------------------------------------------------
		public void Filter(string partMatchString, bool wholeWordMatch, KeyTermFilterType ktFilter,
			Func<int, int, string, bool> refFilter, bool fShowExcludedQuestions,
			Func<TranslatablePhrase, string> getLocalizedPhraseInUse = null)
		{
			Func<int, int, string, bool> filterByRef = refFilter ?? new Func<int, int, string, bool>((start, end, sref) => true);
			
			m_listSorted = false;

			if (IsNullOrEmpty(partMatchString))
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
			// We probably don't want the extra overhead of a do-nothing localization lookup if we're
			// doing English, so we do the check for a null localization lookup function.
			if (getLocalizedPhraseInUse == null)
			{
				m_filteredPhrases = m_phrases.Where(phrase =>
					regexFilter.IsMatch(phrase.PhraseInUse) &&
					phrase.MatchesKeyTermFilter(ktFilter) &&
					filterByRef(phrase.StartRef, phrase.EndRef, phrase.Reference) &&
					(fShowExcludedQuestions || !phrase.IsExcluded)).ToList();
			}
			else
			{
				m_filteredPhrases = m_phrases.Where(phrase =>
					regexFilter.IsMatch(getLocalizedPhraseInUse(phrase)) &&
					phrase.MatchesKeyTermFilter(ktFilter) &&
					filterByRef(phrase.StartRef, phrase.EndRef, phrase.Reference) &&
					(fShowExcludedQuestions || !phrase.IsExcluded)).ToList();
			}
		}

		public TranslatablePhrase AddQuestion(Question question, int section, int category, int seqNumber, MasterQuestionParser parser)
		{
			// Advance sequence numbers of any other questions in this category of this section that
			// have a sequence number >= the new one.
			var phrasesToAdvance = m_phrases.Where(p => p.SectionId == section &&
				p.Category == category && p.SequenceNumber >= seqNumber).ToArray();
			foreach (var phrase in phrasesToAdvance)
				phrase.IncrementSequenceNumber();
			var newPhrase = new TranslatablePhrase(question, section, category, seqNumber, this);
			m_phrases.Add(newPhrase);
			if (m_filteredPhrases != m_phrases)
				m_filteredPhrases.Add(newPhrase);
			if (parser.ParseNewOrModifiedQuestion(question, keyTermMatch => m_phrasePartManager.AddKeyTermMatch(keyTermMatch)))
				m_phrasePartManager.InitializePhraseParts(newPhrase);
			m_listSorted = false;
			return newPhrase;
		}

		public void ModifyQuestion(TranslatablePhrase phrase, string modifiedPhrase, MasterQuestionParser parser)
		{
			phrase.ModifiedPhrase = modifiedPhrase;
			if (parser.ReparseModifiedQuestion(phrase.QuestionInfo, keyTermMatch => m_phrasePartManager.AddKeyTermMatch(keyTermMatch)))
			{
				phrase.m_parts.Clear();
				m_phrasePartManager.InitializePhraseParts(phrase);
			}
			if (phrase.HasUserTranslation)
				((IPhraseTranslationHelper)this).ProcessTranslation(phrase);
			if (m_listSortCriterion != PhrasesSortedBy.Reference)
				m_listSorted = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Parses lines from given stream reader to get translated questions for a particular
		/// reference and attempts to find the corresponding master question and set the
		/// translation if there is a unique match and the question is not already translated. 
		/// </summary>
		/// <param name="reader">Stream reader with \rf lines to indicate book and chapter and
		/// untagged lines with questions followed by verse references in parentheses.</param>
		/// <param name="filename">Name of the file being processed (for reporting purposes)
		/// </param>
		/// <param name="vers">Master versification system used</param>
		/// <param name="reportWriter">writable stream to report results</param>
		/// ------------------------------------------------------------------------------------
		public void SetTranslationsFromText(TextReader reader, string filename, IVersification vers,
			TextWriter reportWriter)
		{
			reportWriter.WriteLine("Processing file: " + filename);

			int questionsRead = 0;
			int questionsMatched = 0;
			int translationsSet = 0;
			int problemsFound = 0;
			MultilingScrBooks multilingScrBooks = new MultilingScrBooks();
			Regex regexChapterBreak = new Regex(@"\\rf (?<bookAndChapter>.+ \d+)", RegexOptions.Compiled);
			Regex regexTranslation = new Regex(@" *(?<translation>.+[?.])( *\((?<versesCovered>\d+(-\d+)?)\))?", RegexOptions.Compiled);

			string sLine;
			int lineNbr = 0;
			string sBookAndChapter = null;
			while ((sLine = reader.ReadLine()) != null)
			{
				lineNbr++;
				sLine = sLine.Replace("  ", " ").Trim();

				if (sLine.Length == 0)
					continue;

				Match match = regexChapterBreak.Match(sLine);
				if (match.Success)
				{
					sBookAndChapter = match.Result("${bookAndChapter}");
					continue;
				}

				if (sBookAndChapter == null)
				{
					reportWriter.WriteLine("   ***Parsing problem at line {0}: \"{1}\"", lineNbr, sLine);
					problemsFound++;
					continue;
				}

				match = regexTranslation.Match(sLine);
				if (match.Success)
				{
					questionsRead++;
					string sVersesCoveredByQuestion = match.Result("${versesCovered}");
					int startVerse = 0;
					BCVRef startRef = multilingScrBooks.ParseRefString(sBookAndChapter + ":1");
					foreach (char c in sVersesCoveredByQuestion)
					{
						if (!IsDigit(c))
							break;
						startVerse *= 10;
						startVerse += (int)GetNumericValue(c);
					}
					if (startVerse > 0)
						startRef.Verse = startVerse;

					BCVRef endRef = new BCVRef(startRef);
					int factor = 1;
					int endVerse = 0;
					for (int i = sVersesCoveredByQuestion.Length - 1; i >= 0; i--)
					{
						char c = sVersesCoveredByQuestion[i];
						if (!IsDigit(c))
							break;
						endVerse += (int)GetNumericValue(c) * factor;
						factor *= 10;
					}
					if (endVerse > 0)
						endRef.Verse = endVerse;
					else if (startVerse == 0)
						endRef.Verse = vers.GetLastVerse(endRef.Book, endRef.Chapter);

					Func<int, int, string, bool> refFilter = (start, end, sref) => endRef == end && startRef == start;

					var matches = m_phrases.Where(phrase => refFilter(phrase.StartRef, phrase.EndRef, phrase.Reference)).ToList();

					string sTranslation = match.Result("${translation}");

					if (matches.Count == 1)
					{
						questionsMatched++;

						var uniqueMatch = matches[0];
						if (!uniqueMatch.HasUserTranslation)
						{
							uniqueMatch.Translation = sTranslation;
							reportWriter.WriteLine("   {0} - \"{1}\" ---> \"{2}\"",
								BCVRef.MakeReferenceString(startRef, endRef, ":", "-"),
								uniqueMatch.OriginalPhrase, uniqueMatch.Translation);
							translationsSet++;
						}
						else
						{
							reportWriter.WriteLine("   {0} - \"{1}\" ***Already translated as: \"{2}\" ***Not set to: \"{3}\"",
								BCVRef.MakeReferenceString(startRef, endRef, ":", "-"),
								uniqueMatch.OriginalPhrase, uniqueMatch.Translation, sTranslation);
						}
					}
					else if (matches.Count == 0)
					{
						reportWriter.WriteLine("   {0} - ***No matching question: \"{1}\"",
							BCVRef.MakeReferenceString(startRef, endRef, ":", "-"), sTranslation);
					}
					else
					{
						if (matches.All(tp => tp.HasUserTranslation))
						{
							reportWriter.WriteLine("   {0} - \"{1}\" ***All matching questions have translations",
								BCVRef.MakeReferenceString(startRef, endRef, ":", "-"), sTranslation);
						}
						else
						{
							reportWriter.WriteLine("   {0} - ***Multiple matching questions: \"{1}\"",
								BCVRef.MakeReferenceString(startRef, endRef, ":", "-"), sTranslation);
						}
					}
				}
				else
				{
					reportWriter.WriteLine("   ***Parsing problem at line {0}: \"{1}\"", lineNbr, sLine);
					problemsFound++;
				}
			}

			reportWriter.WriteLine("Summary");
			reportWriter.WriteLine("=======");
			reportWriter.WriteLine("Read: {0}, Matched: {1}, Set: {2}, Unparsable Lines: {3}", questionsRead, questionsMatched,
				translationsSet, problemsFound);
		}
		#endregion

		#region Implementation of IPhraseTranslationHelper and associated private helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes a new translation on a phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void IPhraseTranslationHelper.ProcessTranslation(TranslatablePhrase tp)
		{
			if (m_listSortCriterion == PhrasesSortedBy.Translation)
				m_sortIsDirty = true;

			string initialPunct, finalPunct;

			StringBuilder bldr = new StringBuilder();
			if (tp.Translation.StartsWith("\u00BF"))
				bldr.Append(tp.Translation[0]);
			if (bldr.Length > 0 && bldr.Length < tp.Translation.Length)
				m_initialPunct[tp.TypeOfPhrase] = initialPunct = bldr.ToString();
			else
				initialPunct = InitialPunctuationForType(tp.TypeOfPhrase);

			bldr.Length = 0;
			foreach (char t in tp.Translation.Reverse().TakeWhile(IsPunctuation))
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
				else if (tp.AllTermsAndNumbersMatch)
					similarPhrase.SetProvisionalTranslation(translation);
			}

			if (tp.AllTermsAndNumbersMatch)
			{
				if (tpParts.Count == 1)
				{
					if (translation.StartsWith(initialPunct))
						translation = translation.Remove(0, initialPunct.Length);
					if (translation.EndsWith(finalPunct))
						translation = translation.Substring(0, translation.Length - finalPunct.Length);

					tpParts[0].Translation = Regex.Replace(translation, @"\{.+\}", Empty).Trim();
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
				unTranslatedParts[0].Translation = Regex.Replace(translation, @"\{.+\}", Empty).Trim();

			foreach (Part partNeedingUpdating in partsNeedingUpdating.OrderBy(p => -p.Words.Count()))
				RecalculatePartTranslation(partNeedingUpdating);

			if (TranslationsChanged != null)
				TranslationsChanged();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// If list is sorted by status, note that it is no longer sorted correctly.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void ProcessChangeInUserTranslationState()
		{
			if (m_listSortCriterion == PhrasesSortedBy.Status)
				m_sortIsDirty = true;
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

			var userTranslations = new List<string>();
			foreach (TranslatablePhrase phrase in part.OwningPhrases.Where(op => op.HasUserTranslation))
			{
				StringBuilder toAdd = new StringBuilder(phrase.UserTransSansOuterPunctuation);
				foreach (IPhrasePart otherPart in phrase.GetParts().Where(otherPart => otherPart != part))
				{
					if (otherPart is KeyTerm term)
					{
						foreach (string ktTrans in term.Renderings)
						{
							int ich = toAdd.ToString().IndexOf(ktTrans, StringComparison.Ordinal);
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
							toAdd.ReplaceUniqueOrWholeWordSubstring(otherPart.Translation, StringUtils.kszObject, 3);
					}
				}
				if (toAdd.Length > 0)
					userTranslations.Add(toAdd.ToString());
			}

			string commonTranslation = GetBestCommonPartTranslation(userTranslations);
		    if (commonTranslation != null)
		    {
                if (commonTranslation.Contains(StringUtils.kszObject))
                    Debug.WriteLine("ORC in part translation");
		        part.Translation = commonTranslation;
		    }
		    if (originalTranslation.Length > 0 && (part.Translation.Length == 0 ||
				(originalTranslation.Length > part.Translation.Length && originalTranslation.Contains(part.Translation))))
			{
				// The translation of the part has shrunk
				return part.OwningPhrases.Where(phr => phr.HasUserTranslation).SelectMany(otherPhrases => otherPhrases.TranslatableParts).Distinct();
			}
			return Array.Empty<Part>();
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

			string commonTranslation = GetCommonSubstring(userTranslations);
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
					if (sCommonSubstring.Length > 1 || (sCommonSubstring.Length == 1 && IsLetter(sCommonSubstring[0])))
					{
						commonSubstrings.TryGetValue(sCommonSubstring, out var val);
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
			return (IsNullOrEmpty(commonTranslation) || statisticallyBestSubstring.Value > totalComparisons) ?
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
		public static string GetCommonSubstring(IList<string> strings)
		{
			string firstOne = strings[0];
			string sCommonSubstring;
			if (strings.Count == 1)
				sCommonSubstring = Empty;
			else
			{
				sCommonSubstring = StringUtils.LongestUsefulCommonSubstring(firstOne, strings[1],
					true, out _);
				for (int i = 2; i < strings.Count; i++)
				{
					string sPossibleCommonSubstring = StringUtils.LongestUsefulCommonSubstring(strings[i], sCommonSubstring,
						true, out _);
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
							true, out _);
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
			return m_initialPunct.TryGetValue(type, out var p) ? p : Empty;
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
			return m_finalPunct.TryGetValue(type, out var p) ? p : Empty;
		}

		public void ProcessAllTranslations()
		{
			if (!m_justGettingStarted)
				throw new InvalidOperationException("This method should only be called once, after all the saved translations have been loaded.");

			foreach (Part part in m_phrasePartManager.AllTranslatableParts)
			{
				if (part.OwningPhrases.Where(p => p.HasUserTranslation).Skip(1).Any()) // Must have at least 2 phrases with translations
					RecalculatePartTranslation(part);
			}

			m_justGettingStarted = false;
		}

		/// <summary>
		/// In (rare) case where a custom question is added and there are no other
		/// questions in the selected category for this section, this method finds
		/// an adjacent question (in a different section and/or category) to attach
		/// the new question to.
		/// </summary>
		/// <param name="newPhrase"></param>
		public void AttachNewQuestionToAdjacentPhrase(TranslatablePhrase newPhrase)
		{
			TranslatablePhrase phraseBefore = null, phraseAfter = null;
			foreach (var tp in m_phrases.Where(p => p != newPhrase))
			{
				var result = ComparePhrasesByIndexedOrder(tp, newPhrase);
				if (result < 0)
				{
					if (phraseBefore == null || ComparePhrasesByIndexedOrder(phraseBefore, tp) < 0)
						phraseBefore = tp;
				}
				else
				{
					Debug.Assert(result != 0);
					if (phraseAfter == null || ComparePhrasesByIndexedOrder(tp, phraseAfter) < 0)
						phraseAfter = tp;
				}
			}
			if (phraseAfter == null)
			{
				Debug.Assert(phraseBefore != null);
				phraseBefore.AddedPhraseAfter = newPhrase.QuestionInfo;
			}
			else
			{
				if (phraseBefore == null || newPhrase.StartRef - phraseBefore.StartRef > phraseAfter.StartRef - newPhrase.EndRef)
					phraseAfter.InsertedPhraseBefore = newPhrase.QuestionInfo;
				else
					phraseBefore.AddedPhraseAfter = newPhrase.QuestionInfo;
			}
		}

		#region Number formatting stuff
		public event OnNumberFormattingChangedHandler OnNumberFormattingChanged;

		/// ------------------------------------------------------------------------------------
		public void SetNumericFormat(char exampleDigit, string groupingPunctuation,
			IReadOnlyList<int> digitGroups, bool fNoGroupPunctForShortNumbers)
		{
			char nativeZero = (char)(exampleDigit - (int)GetNumericValue(exampleDigit));
			if (nativeZero.ToString(CultureInfo.InvariantCulture) != NumberFormatInfo.NativeDigits[0] ||
				groupingPunctuation != NumberFormatInfo.NumberGroupSeparator ||
				NoGroupPunctForShortNumbers != fNoGroupPunctForShortNumbers ||
				!NumberFormatInfo.NumberGroupSizes.SequenceEqual(digitGroups))
			{
				NumberFormatInfo = new NumberFormatInfo
				{
					DigitSubstitution = DigitShapes.NativeNational
				};
				var nativeDigits = new string[10];
				for (int i = 0; i <= 9; i++)
					nativeDigits[i] = ((char) (nativeZero + i)).ToString(CultureInfo.InvariantCulture);

				NumberFormatInfo.NativeDigits = nativeDigits;

				NumberFormatInfo.NumberGroupSeparator = groupingPunctuation;

				NumberFormatInfo.NumberGroupSizes = digitGroups.ToArray();

				NoGroupPunctForShortNumbers = fNoGroupPunctForShortNumbers;

				OnNumberFormattingChanged?.Invoke();
			}
		}

		public NumberFormatInfo NumberFormatInfo { get; private set; } = CultureInfo.CurrentCulture.NumberFormat;
		public bool NoGroupPunctForShortNumbers { get; private set; }

		#endregion
		#endregion
	}
}
