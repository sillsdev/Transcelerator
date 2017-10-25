// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: PhraseCustomization.cs
// ---------------------------------------------------------------------------------------------
using System.Xml.Serialization;
using SIL.Scripture;

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
		/// Gets or sets the translation.
		/// </summary>
		/// --------------------------------------------------------------------------------
		[XmlAttribute("type")]
		public CustomizationType Type { get; set; }
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
		private readonly int m_startRef;
		private readonly int m_endRef;

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
