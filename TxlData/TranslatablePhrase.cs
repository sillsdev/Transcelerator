// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2011' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: TranslatablePhrase.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SIL.Data;
using SIL.Extensions;
using SIL.Scripture;
using SIL.Transcelerator.Localization;
using static System.Char;
using static System.String;

namespace SIL.Transcelerator
{
	public sealed class TranslatablePhrase : IComparable<TranslatablePhrase>, ITranslatablePhrase
	{
		#region Data Members
		private string m_sModifiedPhrase;
		internal readonly List<IPhrasePart> m_parts = new List<IPhrasePart>();
		private readonly TypeOfPhrase m_type;
		private readonly IQuestionKey m_questionInfo;
		private string m_sTranslation;
		private bool m_fHasUserTranslation;
		private bool m_allTermsAndNumbersMatch;
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="questionInfo">Information about the original question</param>
		/// <param name="section">The id of the section to which this question pertains
		/// (or -1 if this is a category name).</param>
		/// <param name="iCategory">The index of the category (e.g. Overview vs. Detail question)
		/// within the section (or -1 if this is a category name).</param>
		/// <param name="seqNumber">The sequence number (used to sort and/or uniquely identify
		/// a phrase within a particular section and category).</param>
		/// <param name="helper">Helper object that knows about other TranslatablePhrase objects
		/// and can manage the relationship between them as translations change.</param>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase(IQuestionKey questionInfo, int section, int iCategory,
			int seqNumber, IPhraseTranslationHelper helper)
			: this(questionInfo.Text, (questionInfo as Question)?.ModifiedPhrase, helper)
		{
			m_questionInfo = questionInfo;
			SectionId = section;
			Category = iCategory;
			SequenceNumber = seqNumber;
			// The following is normally done by the ModifiedPhrase setter, but there's a
			// chicken-and-egg problem when constructing this, so we need to do it here.
			if (IsUserAdded && m_sModifiedPhrase != null)
			{
				// This cast is entirely safe. Note above that for m_sModifiedPhrase to be non-null,
				// the questionInfo object has to be a Question
				((Question)questionInfo).Text = m_sModifiedPhrase;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="phrase">The original phrase.</param>
		/// <param name="helper">Helper object that knows about other TranslatablePhrase objects
		/// and can manage the relationship between them as translations change.</param>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase(string phrase, IPhraseTranslationHelper helper) :
			this(phrase, null, helper)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="originalPhrase">The original phrase. (Possibly a user-added phrase, in
		/// which case it will be a GUID, prefaced by Question.kGuidPrefix) </param>
		/// <param name="modifiedPhrase">The modified phrase.</param>
		/// <param name="helper">Helper object that knows about other TranslatablePhrase objects
		/// and can manage the relationship between them as translations change.</param>
		/// ------------------------------------------------------------------------------------
		private TranslatablePhrase(string originalPhrase, string modifiedPhrase, IPhraseTranslationHelper helper)
		{
			OriginalPhrase = originalPhrase.Normalize(NormalizationForm.FormC);
			if (!IsNullOrEmpty(modifiedPhrase))
			{
				m_sModifiedPhrase = modifiedPhrase.Normalize(NormalizationForm.FormC);
			}
			if (!IsNullOrEmpty(OriginalPhrase))
			{
				switch (PhraseInUse[PhraseInUse.Length - 1])
				{
					case '?': m_type = TypeOfPhrase.Question; break;
					case '.': m_type = TypeOfPhrase.StatementOrImperative; break;
					default: m_type = TypeOfPhrase.Unknown; break;
				}
				if (m_type == TypeOfPhrase.Unknown && OriginalPhrase.StartsWith(Question.kGuidPrefix))
				{
					OriginalPhrase = Empty;
					m_type = TypeOfPhrase.NoEnglishVersion;
				}
			}

			Helper = helper;
		}
		#endregion

		#region Properties
		internal IPhraseTranslationHelper Helper { get; }
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the category of this phrase (used to group phrases within the same section).
		/// Returns -1 if this is a category name.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int Category { get; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the id of the section to which this question pertains (or -1 if this
		/// is a category name).
		/// </summary>
		/// <remarks>Within a book, section IDs are sequential. Unfortunately, however, they
		/// are not sequential throughout Scripture (books are out of canonical order)</remarks>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int SectionId { get; }

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this is a Detail (as opposed to Overview or category name)
        /// question/phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsDetail => Category > 0;

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this is a category name (as opposed to a Detail or Overview
        /// question/phrase.)
        /// </summary>
        /// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsCategoryName => Category < 0;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the "reference" that tells what this phrase pertains to.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string Reference => m_questionInfo?.ScriptureReference;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the original phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string OriginalPhrase { get; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrase to use for processing & comparison purposes (either the original
		/// phrase or a modified form of it).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string PhraseInUse => m_sModifiedPhrase ?? OriginalPhrase;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrase as it is being presented to the user (the original phrase, a
		/// modified form of it, or a special UI string indicating a user-added question with
		/// no English equivalent). Despite its name, this is NOT the localized form of the
		/// question (if the UI is being presented in a locale other than U.S. English.)
		/// <seealso cref="NoEnglishVersionExplanation"/>
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[PublicAPI("Used via reflection as part of DataSource for grid in New Question dialog")]
		public string PhraseToDisplayInUI =>
			(m_type == TypeOfPhrase.NoEnglishVersion) ? NoEnglishVersionExplanation: PhraseInUse;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clients that want user-added questions to display with a meaningful (potentially
		/// localized) explanation instead of an empty string (e.g., "User-added question") can
		/// set this static property accordingly.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string NoEnglishVersionExplanation { get; set; } = "";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the key to use when saving or attempting to look up a specific translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public IQuestionKey PhraseKey => m_questionInfo;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a modified version of the phrase to use in place of the original.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string ModifiedPhrase
		{
			get => m_sModifiedPhrase;
			internal set
			{
				m_sModifiedPhrase = value.Normalize(NormalizationForm.FormC);
				if (m_sModifiedPhrase.Equals(OriginalPhrase))
					m_sModifiedPhrase = null;
				QuestionInfo.Text = value;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets information about a question that is inserted ahead of this phrase in
		/// the list (when sorted in text order).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public Question InsertedPhraseBefore { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets information about a question that is added after this phrase in
		/// the list (when sorted in text order).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public Question AddedPhraseAfter { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this phrase is excluded (not available for
		/// translation).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsExcluded
		{
			get => m_questionInfo is Question q && q.IsExcluded;
			set => ((Question)m_questionInfo).IsExcluded = value;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether this instance is excluded or customized.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		internal bool IsExcludedOrModified => (IsExcluded || (m_sModifiedPhrase != null && !IsUserAdded));

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether this instance is user-supplied.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsUserAdded => m_questionInfo is Question q && q.IsUserAdded;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the user translation with any initial and final punctuation removed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string UserTransSansOuterPunctuation
		{
			get
			{
				Debug.Assert(m_fHasUserTranslation);

				StringBuilder bldr = new StringBuilder(m_sTranslation);
				while (bldr.Length > 0 && IsPunctuation(bldr[0]))
					bldr.Remove(0, 1);
				while (bldr.Length > 0 && IsPunctuation(bldr[bldr.Length - 1]))
					bldr.Length--;
				return bldr.ToString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Translation
		{
			get => GetTranslation();
			set
			{
				if (IsExcluded)
				{
					if (IsUserAdded)
						m_sTranslation = value?.Normalize(NormalizationForm.FormC);
					else
						throw new InvalidOperationException("Translation can not be set for an excluded phrase.");
				}
				else
				{
					SetHasUserTranslationInternal(!IsNullOrEmpty(value));
					SetTranslationInternal(value);
				}
			}
		}

		public string GetTranslation(bool fast = false)
		{
			if (m_fHasUserTranslation)
				return m_sTranslation;
			if (!IsNullOrEmpty(m_sTranslation))
				return Format(m_sTranslation, GetRenderingsOfType<KeyTerm>(fast).Concat(GetRenderingsOfType<Number>(fast)).Cast<object>().ToArray());
			return Helper.InitialPunctuationForType(TypeOfPhrase) +
				m_parts.ToString(true, " ", p => p.GetBestRenderingInContext(this, fast)) +
				Helper.FinalPunctuationForType(TypeOfPhrase);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the provisional translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void SetProvisionalTranslation(string value)
		{
			m_sTranslation = value;
			SetHasUserTranslationInternal(false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the flag indicating whether the translation represents a user translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetHasUserTranslationInternal(bool value)
		{
			if (m_fHasUserTranslation != value)
			{
				m_fHasUserTranslation = value;
				Helper.ProcessChangeInUserTranslationState();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this phrase has a translation that was
		/// supplied by the user).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool HasUserTranslation
		{
			get => m_fHasUserTranslation;
			set
			{
				if (value)
					Translation = Translation; // This looks weird, but we want the side effects.
				else
				{
					m_sTranslation = null;
					m_fHasUserTranslation = false;

					foreach (TranslatablePhrase similarPhrase in GetTranslatedPhrasesWithSameInitialTranslatablePart().Where(phrase => phrase.PartPatternMatches(this)))
					{
						if (similarPhrase.PhraseInUse == PhraseInUse)
						{
							m_sTranslation = similarPhrase.Translation;
							return;
						}

						if (similarPhrase.m_allTermsAndNumbersMatch)
						{
							SetProvisionalTranslation(similarPhrase.GetTranslationTemplate());
							return;
						}
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether the user-supplied translation contains a known
		/// rendering for each of the key terms in the original phrase and equivalent numeric
		/// representation for any numbers expressed as digits.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool AllTermsAndNumbersMatch
		{
			get 
			{
				Debug.Assert(m_fHasUserTranslation);
				return m_allTermsAndNumbersMatch;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a string that attempts to show the parts or matching pattern that were used to
		/// arrive at the translation (for debugging purposes).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string DebugInfo
		{
			get
			{
				var sb = new StringBuilder(PatternBasedDebugInfo);
				if (sb.Length > 0)
					sb.Append("  ---  ");
				sb.Append(PartsBasedDebugInfo);
				return sb.ToString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a string that attempts to show how the matching pattern would be used to
		/// arrive at the translation (for debugging purposes). Note: If this returns a
		/// non-empty string, then the Translation being shown is actually based on this
		/// pattern.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string PatternBasedDebugInfo
		{
			get
			{
				if (!IsNullOrEmpty(m_sTranslation) && m_sTranslation.Contains("{0}"))
				{
					var parameters = GetValuesForPartsOfType<KeyTerm>(t => "(" + t.DebugInfo + ")")
						.Concat(GetValuesForPartsOfType<Number>(n => "#" + n.DebugInfo)).Cast<object>().ToArray();
					return Format(m_sTranslation, parameters);
				}
				return Empty;
			}
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a string that attempts to show how the parts would be used to generate the
		/// translation (for debugging purposes). Note: If PatternBasedDebugInfo returns a non-
		/// empty string, the generated Translation being show is based on the pattern, not on
		/// the translations of the parts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string PartsBasedDebugInfo
		{
			get
			{
				return m_parts.ToString(" | ", (part, bldr) =>
				{
					bldr.Append(part);
					bldr.Append(" (");
					bldr.Append(part.DebugInfo);
					bldr.Append(")");
				});
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translatable Parts (i.e., the parts that are not Key Terms or leading/
		/// trailing punctuation).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public IEnumerable<Part> TranslatableParts => m_parts.OfType<Part>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the key term renderings (i.e., translations), ordered by their
		/// occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string[] KeyTermRenderings => GetRenderingsOfType<KeyTerm>();

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the parts formatted appropriately for inserting into a
		/// translations, ordered by their occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string[] GetRenderingsOfType<T>(bool fast = false) where T : IPhrasePart
		{
			return GetValuesForPartsOfType<T>(t => t.GetBestRenderingInContext(this, fast)).ToArray();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the numbers formatted appropriately for inserting into a
		/// translations, ordered by their occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private IEnumerable<string> GetValuesForPartsOfType<T>(Func<T, string> selector) where T : IPhrasePart
		{
			return m_parts.OfType<T>().Select(selector);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a numeric representation of the start reference in the form BBBCCCVVV.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int StartRef => m_questionInfo?.StartRef ?? -1;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a numeric representation of the end reference in the form BBBCCCVVV.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int EndRef => m_questionInfo?.EndRef ?? -1;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the (0-based) sequence number of this phrase (uniquely identifies this phrase
		/// within a given category of a particular section).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int SequenceNumber { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Increments the sequence number of this phrase/question to allow for an inserted one.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void IncrementSequenceNumber()
		{
			SequenceNumber++;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translated name of the requested category; if not translated, use the
		/// English name.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string CategoryName => Helper.GetCategoryName(Category);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets any additional information the creator of this phrase wanted to associate with
		/// it.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public Question QuestionInfo => m_questionInfo as Question;

		[Browsable(false)]
		public TypeOfPhrase TypeOfPhrase => (IsUserAdded && m_sModifiedPhrase == Empty) ? TypeOfPhrase.NoEnglishVersion : m_type;

		[Browsable(false)]
		public IEnumerable<string> AlternateForms => QuestionInfo?.AlternativeForms;

		public string ImmutableKey => IsUserAdded ? QuestionInfo.Id :
			QuestionInfo?.Alternatives?.FirstOrDefault(a => a.IsKey)?.Text ?? OriginalPhrase;
		#endregion

		#region Public/internal methods (and the indexer which is really more like a property, but Tim wants it in this region)
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return PhraseKey.ToString();
		}

		public UIDataString ToUIDataString()
		{
			if (IsCategoryName)
				return new UISimpleDataString(PhraseInUse, LocalizableStringType.Category);

			if (m_sModifiedPhrase != null || IsUserAdded)
			{
				if (AlternateForms != null)
				{
					var iAlt = AlternateForms.IndexOf(m_sModifiedPhrase);
					if (iAlt >= 0)
						return new UIAlternateDataString(QuestionInfo, iAlt);
				}
				return new UISimpleDataString(PhraseInUse, LocalizableStringType.NonLocalizable);
			}
			return new UIQuestionDataString(PhraseKey, true, true);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the parts, including the key terms (maybe never needed?).
		/// </summary>
		/// <remarks>This is a method rather than a property to prevent it from being displayed
		/// in the data grid.</remarks>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<IPhrasePart> GetParts()
		{
			return m_parts;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the part at the specified <paramref name="index"/>.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public IPhrasePart this[int index] => m_parts[index];

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Compares two phrases based on the following:
		/// 1) Compare the translatable parts with the fewest number of owning phrases. The
		/// phrase whose least prolific part has the most owning phrases sorts first.
		/// 2) Fewest translatable parts
		/// 3) Translatable part with the maximum number of owning phrases.
		/// 4) Reference (alphabetically)
		/// 5) Alphabetically by original phrase
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
		/// Value
		/// Meaning
		/// Less than zero
		/// This object is less than the <paramref name="other"/> parameter.
		/// Zero
		/// This object is equal to <paramref name="other"/>.
		/// Greater than zero
		/// This object is greater than <paramref name="other"/>.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public int CompareTo(TranslatablePhrase other)
		{
			// 1)
			// ENHANCE: Idea for a possible future optimization: 	compare = (TranslatableParts.Any() ? (-1) : m_parts[0] ) + (other.TranslatableParts.Any() ? 1 : -2);
			if (!other.TranslatableParts.Any())
				return TranslatableParts.Any() ? -1 : 0;
			if (!TranslatableParts.Any())
				return 1;
			var compare = other.TranslatableParts.Min(p => p.OwningPhrases.Count()) * 100 / other.TranslatableParts.Count() - TranslatableParts.Min(p => p.OwningPhrases.Count()) * 100 / TranslatableParts.Count();
			if (compare != 0)
				return compare;
			// 2)
			//compare = TranslatableParts.Count() - other.TranslatableParts.Count();
			//if (compare != 0)
			//    return compare;
			// 3)
			compare = other.TranslatableParts.Max(p => p.OwningPhrases.Count()) - TranslatableParts.Max(p => p.OwningPhrases.Count());
			if (compare != 0)
				return compare;
			// 4)
			compare = (Reference == null) ? (other.Reference == null? 0 : -1) : 
				Compare(Reference, other.Reference, StringComparison.Ordinal);
			if (compare != 0)
				return compare;
			// 5)
			return Compare(PhraseInUse, other.PhraseInUse, StringComparison.Ordinal);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether this phrase's parts matches those of the given phrase. A "match"
		/// is when the Translatable Parts are in the exact same sequence and the number and
		/// sequence of key terms is also the same (but not necessarily the same key terms).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool PartPatternMatches(TranslatablePhrase tp)
		{
			if (m_parts.Count != tp.m_parts.Count)
				return false;
			for (int i = 0; i < m_parts.Count; i++)
			{
				if ((m_parts[i] as Part) != tp.m_parts[i] as Part)
					return false;
			}
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the other phrases that have a translation and which have the same initial part
		/// as the given phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> GetTranslatedPhrasesWithSameInitialTranslatablePart()
		{
			var firstPart = TranslatableParts.FirstOrDefault();
			if (firstPart != null)
				return firstPart.OwningPhrases.Where(phrase => phrase != this && phrase.HasUserTranslation);
			return Array.Empty<TranslatablePhrase>();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the other identical phrases that have the same translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IReadOnlyCollection<TranslatablePhrase> GetOtherIdenticalPhrasesWithSameTranslation()
		{
			return GetTranslatedPhrasesWithSameInitialTranslatablePart().Where(phrase =>
				phrase.PhraseInUse == PhraseInUse &&
				phrase.Translation == Translation).ToList();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether this phrase matches the criteria specified by the key term
		/// filter option.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool MatchesKeyTermFilter(PhraseTranslationHelper.KeyTermFilterType ktFilter)
		{
			switch (ktFilter)
			{
				case PhraseTranslationHelper.KeyTermFilterType.WithRenderings:
					return m_parts.OfType<KeyTerm>().All(kt => !IsNullOrEmpty(kt.Translation));
				case PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings:
					bool temp = m_parts.OfType<KeyTerm>().Any(kt => IsNullOrEmpty(kt.Translation));
					return temp;
				default: // PhraseTranslationHelper.KeyTermFilterType.All
					return true;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translation template with placeholders for each of the key terms for which
		/// a matching rendering is found in the translation. As a side-effect, this also sets
		/// m_allTermsAndNumbersMatch.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal string GetTranslationTemplate()
		{
			Debug.Assert(m_fHasUserTranslation);
			int iSubstitutionParam = 0;
			string translation = Translation;
			m_allTermsAndNumbersMatch = true;
			foreach (KeyTerm term in m_parts.OfType<KeyTerm>())
			{
				int ich = -1;
				foreach (string ktTrans in term.Renderings.OrderBy(r => r, new StrLengthComparer(false)))
				{
					ich = translation.IndexOf(ktTrans, StringComparison.Ordinal);
					if (ich >= 0)
					{
						translation = translation.Remove(ich, ktTrans.Length);
						translation = translation.Insert(ich, "{" + iSubstitutionParam++ + "}");
						break;
					}
				}
				if (ich == -1)
					m_allTermsAndNumbersMatch = false;
			}

			foreach (Number number in m_parts.OfType<Number>())
			{
				string sNbr = number.NumericValue.ToString("d");
				int iNbr = 0;
				int ich = -1;
				for (int iTrans = 0; iTrans < translation.Length; iTrans++)
				{
					char t = translation[iTrans];
					if (IsDigit(t) &&
						GetNumericValue(sNbr[iNbr]).Equals(GetNumericValue(t)))
					{
						if (ich < 0)
							ich = iTrans; // Found possible start of the number we're seeking.
						iNbr++;
						if (iNbr == sNbr.Length)
						{
							if (iTrans + 1 < translation.Length && IsDigit(translation[iTrans + 1]))
							{
								// Number in the translation has more (unaccounted for) digits - not a match
								ich = -1;
								iNbr = 0;
							}
							else
							{
								int len = iTrans - ich + 1;
								number.Translation = translation.Substring(ich, len);
								translation = translation.Remove(ich, len);
								translation = translation.Insert(ich, "{" + iSubstitutionParam++ + "}");
								break;
							}
						}
					}
					else if (ich > -1 && !IsPunctuation(t) && !IsWhiteSpace(t))
					{
						// Not a complete match. Start over.
						ich = -1;
						iNbr = 0;
					}
				}
				if (iNbr != sNbr.Length)
					m_allTermsAndNumbersMatch = false;
			}
			return translation;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the term rendering (from the known ones in the renderingInfo) in use in
		/// the current translation.
		/// </summary>
		/// <param name="renderingInfo">The information about a single occurrence of a key
		/// biblical term and its rendering in a string in the target language.</param>
		/// <returns>An object that indicates where in the translation string the match was
		/// found (offset and length)</returns>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor FindTermRenderingInUse(ITermRenderingInfo renderingInfo) =>
			FindTermRenderingInUse(renderingInfo, Translation);

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Finds the term rendering (from the known ones in the renderingInfo) in use in
		/// the given translation.
		/// </summary>
		/// <param name="renderingInfo">The information about a single occurrence of a key
		/// biblical term and its rendering in a string in the target language.</param>
		/// <param name="translation">If provided, a provisional translation to use instead
		/// of the current <see cref="Translation"/></param>
		/// <returns>An object that indicates where in the translation string the match was
		/// found (offset and length)</returns>
		/// ------------------------------------------------------------------------------------
		public static SubstringDescriptor FindTermRenderingInUse(ITermRenderingInfo renderingInfo,
			string translation)
		{
			// This will almost always be 0, but if a term occurs more than once, this
			// will be the character offset following the occurrence of the rendering of
			// the preceding term in the translation.
			int ichStart = renderingInfo.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm;
			int indexOfMatch = int.MaxValue;
			int lengthOfMatch = 0;
			foreach (string rendering in renderingInfo.Renderings)
			{
				if (ichStart >= translation.Length)
					return null; // Translation was shortened/deleted
				int ich = translation.IndexOf(rendering, ichStart, StringComparison.Ordinal);
				if (ich >= 0 && (ich < indexOfMatch || (ich == indexOfMatch && rendering.Length > lengthOfMatch)))
				{
					// Found an earlier or longer match.
					indexOfMatch = ich;
					lengthOfMatch = rendering.Length;
				}
			}
			if (lengthOfMatch == 0)
				return null;

			return new SubstringDescriptor(indexOfMatch, lengthOfMatch);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Inserts a newly selected key term rendering into the appropriate place in the
		/// (provisional) translation.
		/// </summary>
		/// <param name="text">The initial value of the translation. This may or may not be the
		/// actual value of the translation for this phrase since it may be a "dirty" value if
		/// the user is in the middle of editing it.
		/// </param>
		/// <param name="renderingInfo">object that indicates all the renderings for the term
		/// and the offset of the end of the preceding occurrence of the rendering of the term
		/// (if any).</param>
		/// <param name="editingSelectionState">Substring descriptor (location & length) that
		/// indicates what part of the translation text the user had selected if this
		/// translation was being edited at the time the new rendering was chosen. If this is
		/// not null, we'll consider whether the new rendering should be inserted at that point.
		/// </param>
		/// <param name="newRendering">The selected rendering.</param>
		/// <returns>The new (provisional) value of the translation with the new rendering
		/// inserted in the best possible place in the text.</returns>
		/// ------------------------------------------------------------------------------------
		public string InsertKeyTermRendering(string text, ITermRenderingInfo renderingInfo,
			string newRendering, ref SubstringDescriptor editingSelectionState)
		{
			var locationOfExistingRendering = FindTermRenderingInUse(renderingInfo, text);
			if (editingSelectionState != null)
			{
				// If the user had selected the entire translation, we treat it as if they weren't
				// editing at all, since we can't make any reasonable guess as to what part of the
				// translation should be replaced based on the selection.
				if (editingSelectionState.Length < text.Length)
				{
					int start = editingSelectionState.Start;
					int length = editingSelectionState.Length;
					bool fInsertAtSelectionStart = false;
					// Before blindly replacing the (first) existing rendering, we want to look to see if
					// one of the following conditions exists:
					// 1) The existing text of the translation which the user has been editing does not
					//    contain any rendering for this term at all. In this case, we insert the rendering
					//    at the insertion point or replace the existing selection.
					// 2) The selected word or phrase is a (known) rendering for the same term.
					//    In this case, we replace the selection with the newly chosen rendering. This
					//    gives the user a way to explicitly control which instance of a term is to be
					//    replaced in those cases where the same term (or another term with some of the
					//    same renderings) occurs more than once in a question.
					if (length > 0)
					{
						var existingSelectedText = text.Substring(start, length).TrimEnd();
						if (existingSelectedText.Length > 0)
						{
							length = existingSelectedText.Length;
							existingSelectedText = existingSelectedText.TrimStart();
							if (length > existingSelectedText.Length)
							{
								start += length - existingSelectedText.Length;
								length = existingSelectedText.Length;
							}
						}
						else
						{
							// selection consists entirely of spaces. Leave one leading and one trailing space. 
							start++;
							length--;
							if (length > 0)
								length--;
						}

						if (locationOfExistingRendering == null || (length > 0 && renderingInfo.Renderings.Contains(existingSelectedText)))
						{
							text = text.Remove(start, length);
							fInsertAtSelectionStart = true;
						}
					}
					else
						fInsertAtSelectionStart = locationOfExistingRendering == null;

					if (fInsertAtSelectionStart)
					{
						if (length == 0)
						{
							if (start > 0 && IsLetterOrDigit(text[start - 1]))
							{
								text = text.Insert(start, " ");
								start++;
							}
							if (start < text.Length && IsLetterOrDigit(text[start]))
								text = text.Insert(start, " ");
						}
						editingSelectionState.Start = start;
						editingSelectionState.Length = newRendering.Length;
						return text.Insert(start, newRendering);
					}
				}
			}

			// Since we didn't insert the new term into the translation based on the editing
			// position, just replace the existing rendering (if any) or stick it at the end.
			var sd = locationOfExistingRendering;
			if (sd == null)
			{
				var finalPunct = Helper.FinalPunctuationForType(TypeOfPhrase);
				int start = text.Length;
				if (finalPunct != null && text.EndsWith(finalPunct))
					start -= finalPunct.Length;
				if (start > 0 && IsLetterOrDigit(text[start - 1]))
					text = text.Insert(start++, " ");
				sd = new SubstringDescriptor(start, 0);
			}

			editingSelectionState = new SubstringDescriptor(sd.Start, newRendering.Length);
			return text.Remove(sd.Start, sd.Length).Insert(sd.Start, newRendering);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this phrase/question applies to the given Scripture reference.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public bool AppliesToReference(BCVRef reference)
        {
            return !IsCategoryName && reference >= StartRef && reference <= EndRef;
        }
		#endregion

		#region Private Helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the translation and processes it if it is a user translation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// ------------------------------------------------------------------------------------
		private void SetTranslationInternal(string value)
		{
			m_sTranslation = value?.Normalize(NormalizationForm.FormC);
			if (m_fHasUserTranslation && m_type != TypeOfPhrase.NoEnglishVersion)
			{
				m_allTermsAndNumbersMatch = false; // This will usually get updated in ProcessTranslation
				Helper.ProcessTranslation(this);
			}
		}
		#endregion
    }
}
