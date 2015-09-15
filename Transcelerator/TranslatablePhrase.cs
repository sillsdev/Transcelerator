﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.   
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
using SIL.Scripture;
using SIL.Utils;

namespace SIL.Transcelerator
{
	public enum TypeOfPhrase : byte
	{
		Unknown,
		Question,
		StatementOrImperative,
		NoEnglishVersion,
	}

	public sealed class TranslatablePhrase : IComparable<TranslatablePhrase>
	{
		#region Data Members
		private readonly string m_sOrigPhrase;
		private string m_sModifiedPhrase;
		private readonly int m_category;
		internal readonly List<IPhrasePart> m_parts = new List<IPhrasePart>();
		private readonly TypeOfPhrase m_type;
		private int m_seqNumber;
		internal readonly QuestionKey m_questionInfo;
		private string m_sTranslation;
		private bool m_fHasUserTranslation;
		private bool m_allTermsAndNumbersMatch;
		internal static IPhraseTranslationHelper s_helper;
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="questionInfo">Information about the original question</param>
		/// <param name="category">The category (e.g. Overview vs. Detail question).</param>
		/// <param name="seqNumber">The sequence number (used to sort and/or uniquely identify
		/// a phrase within a particular category and reference).</param>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase(QuestionKey questionInfo, int category, int seqNumber)
			: this(questionInfo.Text,
			questionInfo is Question ? ((Question)questionInfo).ModifiedPhrase : null)
		{
			m_questionInfo = questionInfo;
			m_category = category;
			m_seqNumber = seqNumber;
			// The following is normally done by the ModifiedPhrase setter, but there's a
			// chicken-and-egg problem when constructing this, so we need to do it here.
			if (IsUserAdded && m_sModifiedPhrase != null)
				m_questionInfo.Text = m_sModifiedPhrase;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="phrase">The original phrase.</param>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase(string phrase) : this(phrase, null)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="TranslatablePhrase"/> class.
		/// </summary>
		/// <param name="originalPhrase">The original phrase. (Possibly a user-added phrase, in
		/// which case it will be a GUID, prefaced by Question.kGuidPrefix) </param>
		/// <param name="modifiedPhrase">The modified phrase.</param>
		/// ------------------------------------------------------------------------------------
		public TranslatablePhrase(string originalPhrase, string modifiedPhrase)
		{
			m_sOrigPhrase = originalPhrase.Normalize(NormalizationForm.FormC);
			if (!string.IsNullOrEmpty(modifiedPhrase))
			{
				m_sModifiedPhrase = modifiedPhrase.Normalize(NormalizationForm.FormC);

			}
			if (!String.IsNullOrEmpty(m_sOrigPhrase))
			{
				switch (PhraseInUse[PhraseInUse.Length - 1])
				{
					case '?': m_type = TypeOfPhrase.Question; break;
					case '.': m_type = TypeOfPhrase.StatementOrImperative; break;
					default: m_type = TypeOfPhrase.Unknown; break;
				}
				if (m_type == TypeOfPhrase.Unknown && m_sOrigPhrase.StartsWith(Question.kGuidPrefix))
				{
					m_sOrigPhrase = string.Empty;
					m_type = TypeOfPhrase.NoEnglishVersion;
				}
			}
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the category of this phrase (used to group phrases having the same reference).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int Category
		{
			get { return m_category; }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this is a Detail (as oppposed to Overview or category name)
        /// question/phrase.
        /// </summary>
        /// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsDetail
        {
            get { return m_category > 0; }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this is a category name (as oppposed to a Detail or Overview
        /// question/phrase.)
        /// </summary>
        /// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsCategoryName
        {
            get { return m_category < 0; }
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the "reference" that tells what this phrase pertains to.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string Reference
		{
			get { return (m_questionInfo == null) ? null : m_questionInfo.ScriptureReference; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the original phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		internal string OriginalPhrase
		{
			get { return m_sOrigPhrase; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrase to use for processing & comparison purposes (either the original
		/// phrase or a modified form of it).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string PhraseInUse
		{
			get { return m_sModifiedPhrase ?? m_sOrigPhrase; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the phrase as it is being presented to the user (the original phrase, a
		/// modified form of it, or a special UI string indicating a user-added question with
		/// no English equivalent).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string PhraseToDisplayInUI
		{
			get
			{
				return (m_type == TypeOfPhrase.NoEnglishVersion) ? Properties.Resources.kstidUserAddedEmptyPhrase :
					m_sModifiedPhrase ?? m_sOrigPhrase;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the key to use when saving or attempting to look up a specific translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public QuestionKey PhraseKey
		{
			get { return m_questionInfo; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a modified version of the phrase to use in place of the original.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string ModifiedPhrase
		{
			get { return m_sModifiedPhrase; }
			internal set
			{
				m_sModifiedPhrase = value.Normalize(NormalizationForm.FormC);
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
		internal Question InsertedPhraseBefore { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets information about a question that is added after this phrase in
		/// the list (when sorted in text order).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		internal Question AddedPhraseAfter { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this phrase is excluded (not available for
		/// translation).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsExcluded
		{
			get
			{
			    Question q = m_questionInfo as Question;
			    return q != null && q.IsExcluded;
			}
			internal set { ((Question)m_questionInfo).IsExcluded = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether this instance is excluded or customized.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		internal bool IsExcludedOrModified
		{
            get { return (IsExcluded || (m_sModifiedPhrase != null && !IsUserAdded)); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether this instance is user-supplied.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		internal bool IsUserAdded
		{
			get
			{
			    Question q = m_questionInfo as Question;
				return q != null && q.IsUserAdded;
			}
		}

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
				while (bldr.Length > 0 && Char.IsPunctuation(bldr[0]))
					bldr.Remove(0, 1);
				while (bldr.Length > 0 && Char.IsPunctuation(bldr[bldr.Length - 1]))
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
			get
			{
				if (m_fHasUserTranslation)
					return m_sTranslation;
				if (!string.IsNullOrEmpty(m_sTranslation))
					return String.Format(m_sTranslation, KeyTermRenderings.Union(NumberRenderings).ToArray());
				return s_helper.InitialPunctuationForType(TypeOfPhrase) +
					m_parts.ToString(true, " ", p => p.GetBestRenderingInContext(this)) +
					s_helper.FinalPunctuationForType(TypeOfPhrase);
			}
			set
			{
				if (IsExcluded)
					throw new InvalidOperationException("Translation can not be set for an excluded phrase.");
				m_fHasUserTranslation = !string.IsNullOrEmpty(value);
				SetTranslationInternal(value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the provisional translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void SetProvisionalTranslation(string value)
		{
			m_sTranslation = value;
			m_fHasUserTranslation = false;
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
			get { return m_fHasUserTranslation; }
			set
			{
				if (value)
					Translation = Translation; // This looks weird, but we want the side effects.
				else
				{
					m_sTranslation = null;
					m_fHasUserTranslation = false;

					Part firstPart = TranslatableParts.FirstOrDefault();
					if (firstPart != null)
					{
						foreach (TranslatablePhrase similarPhrase in firstPart.OwningPhrases.Where(phrase => phrase.HasUserTranslation && phrase.PartPatternMatches(this)))
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
				if (!string.IsNullOrEmpty(m_sTranslation) && m_sTranslation.Contains("{0}"))
				{
					var parameters = GetValuesForPartsOfType<KeyTerm>(t => "(" + t.DebugInfo + ")")
						.Union(GetValuesForPartsOfType<Number>(n => "#" + n.DebugInfo)).Cast<object>().ToArray();
					return String.Format(m_sTranslation, parameters);
				}
				return String.Empty;
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
		public IEnumerable<Part> TranslatableParts
		{
			get { return m_parts.OfType<Part>(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the key term renderings (i.e., tranlsations), ordered by their
		/// occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string[] KeyTermRenderings
		{
			get { return GetRenderingsOfType<KeyTerm>(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the numbers formatted appropriately for inserting into a
		/// tranlsations, ordered by their occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		private string[] NumberRenderings
		{
			get { return GetRenderingsOfType<Number>(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the parts formatted appropriately for inserting into a
		/// tranlsations, ordered by their occurrence in the phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string[] GetRenderingsOfType<T>() where T : IPhrasePart
		{
			return GetValuesForPartsOfType<T>(t => t.GetBestRenderingInContext(this)).ToArray();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an an array of the numbers formatted appropriately for inserting into a
		/// tranlsations, ordered by their occurrence in the phrase.
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
		public int StartRef
		{
			get { return (m_questionInfo == null) ? -1 : m_questionInfo.StartRef; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a numeric representation of the end reference in the form BBBCCCVVV.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int EndRef
		{
			get { return (m_questionInfo == null) ? -1 : m_questionInfo.EndRef; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the sequence number of this phrase (uniquely identifies this phrase within a
		/// given category and for a particular reference).
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public int SequenceNumber
		{
			get { return m_seqNumber; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Increments the sequence number of this phrase/question to allow for an inserted one.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void IncrementSequenceNumber()
		{
			m_seqNumber++;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translated name of the requested category; if not translated, use the
		/// English name.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public string CategoryName
		{
			get { return s_helper.GetCategoryName(Category); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets any additional information the creator of this phrase wanted to associate with
		/// it.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public Question QuestionInfo
		{
			get { return m_questionInfo as Question; }
		}

		[Browsable(false)]
		public TypeOfPhrase TypeOfPhrase
		{
			get { return (IsUserAdded && m_sModifiedPhrase == string.Empty) ? TypeOfPhrase.NoEnglishVersion : m_type; }
		}

		[Browsable(false)]
		public IEnumerable<string> AlternateForms
		{
			get
			{
				var qi = QuestionInfo;
				if (qi != null)
                    return qi.AlternateForms;
				return null;
			}
		}
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
		public IPhrasePart this[int index]
		{
			get { return m_parts[index]; }
		}

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
			int compare;
			// 1)
			// ENHANCE: Idea for a possible future optimization: 	compare = (TranslatableParts.Any() ? (-1) : m_parts[0] ) + (other.TranslatableParts.Any() ? 1 : -2);
			if (!other.TranslatableParts.Any())
				return TranslatableParts.Any() ? -1 : 0;
			if (!TranslatableParts.Any())
				return 1;
			compare = other.TranslatableParts.Min(p => p.OwningPhrases.Count()) * 100 / other.TranslatableParts.Count() - TranslatableParts.Min(p => p.OwningPhrases.Count()) * 100 / TranslatableParts.Count();
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
			compare = (Reference == null) ? (other.Reference == null? 0 : -1) : Reference.CompareTo(other.Reference);
			if (compare != 0)
				return compare;
			// 5)
			return PhraseInUse.CompareTo(other.PhraseInUse);
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
		/// Determines whether this phrase matches the criteria specified by the key term
		/// filter option.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool MatchesKeyTermFilter(PhraseTranslationHelper.KeyTermFilterType ktFilter)
		{
			switch (ktFilter)
			{
				case PhraseTranslationHelper.KeyTermFilterType.WithRenderings:
					return m_parts.OfType<KeyTerm>().All(kt => !string.IsNullOrEmpty(kt.Translation));
				case PhraseTranslationHelper.KeyTermFilterType.WithoutRenderings:
					bool temp = m_parts.OfType<KeyTerm>().Any(kt => string.IsNullOrEmpty(kt.Translation));
					return temp;
				default: // PhraseTranslationHelper.KeyTermFilterType.All
					return true;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translation template with placeholders for each of the key terms for which
		/// a matching rendering is found in the translation. As a side-effect, this also sets
		/// m_allTermsMatch.
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
					if (char.IsDigit(t) &&
						char.GetNumericValue(sNbr[iNbr]).Equals(char.GetNumericValue(t)))
					{
						if (ich < 0)
							ich = iTrans; // Found possible start of the number we're seeking.
						iNbr++;
						if (iNbr == sNbr.Length)
						{
							if (iTrans + 1 < translation.Length && char.IsDigit(translation[iTrans + 1]))
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
					else if (ich > -1 && !char.IsPunctuation(t) && !char.IsWhiteSpace(t))
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
		public SubstringDescriptor FindTermRenderingInUse(ITermRenderingInfo renderingInfo)
		{
			// This will almost always be 0, but if a term occurs more than once, this
			// will be the character offset following the occurrence of the rendering of
			// the preceding term in the translation.
			int ichStart = renderingInfo.EndOffsetOfRenderingOfPreviousOccurrenceOfThisTerm;
			int indexOfMatch = Int32.MaxValue;
			int lengthOfMatch = 0;
			foreach (string rendering in renderingInfo.Renderings)
			{
				int ich = Translation.IndexOf(rendering, ichStart, StringComparison.Ordinal);
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
		/// translation.
		/// </summary>
		/// <param name="renderingInfo">object that indicates all the renderings for the term
		/// and the offset of the end of the preceding occurrence of the rendering of the term
		/// (if any).</param>
		/// <param name="editingSelectionState">Substring descriptor (location & length) that
		/// indicates what part of the transaltion text the user had selected if this
		/// translation was being edited at the time the new rendering was chosen. If this is
		/// not null, we'll consider whether the new rendering should be inserted at that point.
		/// </param>
		/// <param name="newRendering">The selected rendering.</param>
		/// <returns>true if the rendering is inserted based on the editing state; false
		/// otehrwise (merely replaces an existing rendering or is inserted at the end).</returns>
		/// ------------------------------------------------------------------------------------
		public bool InsertKeyTermRendering(ITermRenderingInfo renderingInfo,
			SubstringDescriptor editingSelectionState, string newRendering)
		{
			var locationOfExistingRendering = FindTermRenderingInUse(renderingInfo);
			string text = Translation;
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
							if (start > 0 && Char.IsLetterOrDigit(text[start - 1]))
							{
								text = text.Insert(start, " ");
								start++;
							}
							if (start < text.Length && Char.IsLetterOrDigit(text[start]))
								text = text.Insert(start, " ");
						}
						text = text.Insert(start, newRendering);
						SetTranslationInternal(text);
						editingSelectionState.Start = start;
						editingSelectionState.Length = newRendering.Length;
						return true;
					}
				}
			}

			// Since we didn't insert the new term into the translation based on the editing
			// position, just replace the existing rendering (if any) or stick it at the end.
			var sd = locationOfExistingRendering;
			if (sd == null)
			{
				var finalPunct = s_helper.FinalPunctuationForType(TypeOfPhrase);
				int start = text.Length;
				if (finalPunct != null && text.EndsWith(finalPunct))
					start -= finalPunct.Length;
				sd = new SubstringDescriptor(start, 0);
				if (start > 0 && Char.IsLetterOrDigit(text[start - 1]))
					newRendering = " " + newRendering;
			}
			SetTranslationInternal(text.Remove(sd.Start, sd.Length).Insert(sd.Start, newRendering));
			return false;
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
			m_sTranslation = (value == null) ? null : value.Normalize(NormalizationForm.FormC);
			if (m_fHasUserTranslation && m_type != TypeOfPhrase.NoEnglishVersion)
			{
				m_allTermsAndNumbersMatch = false; // This will usually get updated in ProcessTranslation
				s_helper.ProcessTranslation(this);
			}
		}
		#endregion
    }
}
