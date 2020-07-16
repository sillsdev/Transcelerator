// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2017, SIL International.
// <copyright from='2011' to='2017' company='SIL International'>
//		Copyright (c) 2017, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PhraseCustomization.cs
// ---------------------------------------------------------------------------------------------
using SIL.Scripture;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	#region class PhraseCustomization
	/// ------------------------------------------------------------------------------------
	/// <summary>
	/// Little class to support XML serialization of customizations (additions/changes/deletions
	/// </summary>
	/// ------------------------------------------------------------------------------------
	[XmlType("PhraseCustomization")]
	public class PhraseCustomization
	{
	    private BCVRef m_scrStartReference;
	    private BCVRef m_scrEndReference;

        #region CustomizationType enumeration
        public enum CustomizationType
		{
			Modification,
			Deletion,
			InsertionBefore,
			AdditionAfter,
		}
        #endregion

	    public BCVRef ScrStartReference
	    {
	        get
	        {
	            if (m_scrStartReference == null)
	                SetBcvRefs();
	            return m_scrStartReference;
	        }
	    }

	    public BCVRef ScrEndReference
	    {
	        get
	        {
	            if (m_scrEndReference == null)
	                SetBcvRefs();
                return m_scrEndReference;
	        }
	    }

		public QuestionKey Key => new CustomQuestionKey(this);

        private void SetBcvRefs()
        {
            m_scrStartReference = new BCVRef();
            m_scrEndReference = new BCVRef();
            BCVRef.ParseRefRange(Reference, ref m_scrStartReference, ref m_scrEndReference);
        }

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// --------------------------------------------------------------------------------
        [XmlAttribute("ref")]
		public string Reference { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the original phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public string OriginalPhrase { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the edited/customized phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public string ModifiedPhrase { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the answer (probably mostly used for added questions).
		/// </summary>
		/// --------------------------------------------------------------------------------
		public string Answer { get; set; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the type of customization.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("type")]
		public CustomizationType Type { get; set; }
		
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether this addition or insertion should result
		/// in the creation of a new category. This is only meaningful if Type is
		/// InsertionBefore or AdditionAfter (it will be ignored otherwise). If 0, then the
		/// addition or insertion occurs in the same category as the base. Otherwise, its
		/// value (which is always positive) indicates the number of categories before or
		/// after the base.
		/// </summary>
		/// <remarks>When processing customizations, if a category is found in that
		/// position and a matching question is not in that category, the added phrase will
		/// be added to the end of the existing ones if it is an InsertionBefore, or to the
		/// beginning of the existing ones if it is an AdditionAfter.</remarks>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("categoryOffset")]
		[DefaultValue(0)]
		public int AddedCategoryOffset { get; set; }

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseCustomization"/> class, needed
        /// for XML serialization.
        /// </summary>
        /// --------------------------------------------------------------------------------
        public PhraseCustomization()
		{
		}

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseCustomization"/> class.
        /// </summary>
        /// --------------------------------------------------------------------------------
        public PhraseCustomization(TranslatablePhrase tp)
		{
			Reference = tp.Reference;
			OriginalPhrase = tp.OriginalPhrase;
			ModifiedPhrase = tp.ModifiedPhrase;
			Type = tp.IsExcluded ? CustomizationType.Deletion : CustomizationType.Modification;
		}

        /// --------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseCustomization"/> class for an
        /// insertion or addition.
        /// </summary>
        /// --------------------------------------------------------------------------------
        public PhraseCustomization(string basePhrase, Question addedPhrase,
			CustomizationType type)
		{
			Reference = addedPhrase.ScriptureReference;
			OriginalPhrase = basePhrase;
			ModifiedPhrase = addedPhrase.Text;
			if (addedPhrase.Answers != null && addedPhrase.Answers.Length == 1)
				Answer = addedPhrase.Answers[0];
			Type = type;
		}
	}
	#endregion

	#region CustomQuestionKey
	internal class CustomQuestionKey : QuestionKey
	{
		public override string ScriptureReference { get; set; }
		public override int StartRef { get; set; }
		public override int EndRef { get; set; }
		internal CustomQuestionKey(PhraseCustomization pc)
		{
			Text = pc.OriginalPhrase;
			ScriptureReference = pc.Reference;
			StartRef = pc.ScrStartReference;
			EndRef = pc.ScrEndReference;
		}
	}
	#endregion
}
