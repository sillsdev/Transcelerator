// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2013' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTerm.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIL.Utils;

namespace SIL.Transcelerator
{
    // ---------------------------------------------------------------------------------------------
    /// <summary>
    /// Class representing the connection between a word or phrase in the language of wider
    /// communication (used for the master question list) and the biblical term(s)
    /// </summary>
    // ---------------------------------------------------------------------------------------------
    public sealed class KeyTerm : IPhrasePart
    {
        #region Events and Delegates
        public delegate void BestRenderingChangedHandler(KeyTerm sender);
        public event BestRenderingChangedHandler BestRenderingChanged;
        #endregion

        #region Data members
        private readonly ITermRenderingsRepo m_termRenderingRepo;
        private readonly KeyTermMatchSurrogate m_termSurrogate;
        private readonly List<Word> m_words;
        private HashSet<string> m_allRenderings;
        private string m_bestTranslation;
        #endregion

        #region Construction and initialization
        /// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyTermMatch"/> class.
		/// </summary>
		/// <param name="termSurrogate">The key term match surrogate (which represents one or
		/// more underlying actual biblical terms).</param>
		/// <param name="repository"></param>
		/// ------------------------------------------------------------------------------------
		internal KeyTerm(KeyTermMatchSurrogate termSurrogate, ITermRenderingsRepo repository)
		{
            m_words = termSurrogate.TermId.Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries).Select(w => (Word)w).ToList();
            m_termSurrogate = termSurrogate;
			m_termRenderingRepo = repository;
		}
		#endregion

        #region Properties
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the words.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<Word> Words => m_words;

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the translation.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string Translation
        {
            get
            {
                if (m_bestTranslation == null)
                    LoadRenderings();
                return m_bestTranslation;
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a string that can be used to display word(s) corresponding to this key term
        /// in the LWC used for the master question list.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public override string ToString()
        {
            bool firstWord = true;
            return m_words.ToString(true, " ", w =>
                {
                    if (firstWord)
                    {
                        StringBuilder sb = new StringBuilder(w.Text);
                        sb[0] = (char.ToUpperInvariant(sb[0]));
                        firstWord = false;
                        return sb.ToString();
                    }
                    return w.Text;
                });
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all the underlying term IDs.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IReadOnlyList<string> AllTermIds => m_termSurrogate.BiblicalTermIds;

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a string that can be displayed for "debugging" purposes (currently shown in the
        /// last column of the grid).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string DebugInfo => "KT: " + Translation;

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all (distinct) renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public IEnumerable<string> Renderings
        {
            get
            {
                if (m_allRenderings == null)
                    LoadRenderings();
                return m_allRenderings;
            }
        }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the rendering info (if any) corresponding to this key term object
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private KeyTermRenderingInfo RenderingInfo =>
			m_termRenderingRepo.GetRenderingInfo(m_termSurrogate.TermId);

		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// The primary (best) rendering for the term in the target language (equivalent to the
        /// Translation).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public string BestRendering
        {
            get => Translation;
            set
            {
                m_bestTranslation = value.Normalize(NormalizationForm.FormC);
                KeyTermRenderingInfo info = RenderingInfo;
                if (info == null)
				{
                    info = new KeyTermRenderingInfo(m_termSurrogate.TermId, m_bestTranslation);
					m_termRenderingRepo.Add(info);
                }
                else
                    info.PreferredRendering = m_bestTranslation;
                
                BestRenderingChanged?.Invoke(this);
			}
        }
        #endregion

        #region Public methods
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a rendering.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public void AddRendering(string rendering)
        {
            string normalizedForm = rendering.Normalize(NormalizationForm.FormC);
            if (Renderings.Contains(normalizedForm))
                throw new ArgumentException("Rendering already exists.", nameof(rendering));
            KeyTermRenderingInfo info = RenderingInfo;
            if (info == null)
            {
                info = new KeyTermRenderingInfo(m_termSurrogate.TermId, BestRendering);
                m_termRenderingRepo.Add(info);
            }
            info.AddlRenderings.Add(rendering);
            m_allRenderings.Add(normalizedForm);
	        if (m_bestTranslation == string.Empty)
		        m_bestTranslation = rendering;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether the specified rendering can be deleted.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public bool CanRenderingBeDeleted(string rendering)
        {
            KeyTermRenderingInfo info = RenderingInfo;
            if (info == null)
                return false;
	        var normalized = rendering.Normalize(NormalizationForm.FormC);
            return info.AddlRenderings.Contains(normalized) && info.PreferredRendering != normalized;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the specified rendering.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public void DeleteRendering(string rendering)
        {
            rendering = rendering.Normalize(NormalizationForm.FormC);
            KeyTermRenderingInfo info = RenderingInfo;
            if (info == null || !info.AddlRenderings.Contains(rendering))
                throw new ArgumentException("Cannot delete non-existent rendering: " + rendering);

            if (info.AddlRenderings.Remove(rendering))
            {
                m_allRenderings.Remove(rendering);
	            if (m_bestTranslation == rendering)
		            m_bestTranslation = null; // New "best" will be re-determined when needed.
            }
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the best rendering for this term in when used in the context of the given
        /// phrase.
        /// </summary>
        /// <remarks>If this term occurs more than once in the phrase, it is not possible to
        /// know which occurrence is which.</remarks>
        /// ------------------------------------------------------------------------------------
        public string GetBestRenderingInContext(ITranslatablePhrase phrase, bool fast = false)
        {
            var renderings = (HashSet<string>)Renderings;
            if (!renderings.Any())
                return string.Empty;
            if (renderings.Count == 1 || fast || !(phrase is TranslatablePhrase tp))
	            return Translation;
            if (tp.Helper?.TermRenderingSelectionRules == null)
	            return Translation;

            List<string> renderingsList = null;
            foreach (RenderingSelectionRule rule in tp.Helper.TermRenderingSelectionRules.Where(r => !r.Disabled))
            {
                if (renderingsList == null)
                {
                    renderingsList = new List<string>();
                    foreach (string rendering in renderings)
                    {
                        if (rendering == Translation)
                            renderingsList.Insert(0, rendering);
                        else
                            renderingsList.Add(rendering);
                    }
                }
                string s = rule.ChooseRendering(tp.PhraseInUse, m_words, renderingsList);
                if (!string.IsNullOrEmpty(s))
                    return s;
            }
            return Translation;
        }
        #endregion

        #region Private helper methods
        // ---------------------------------------------------------------------------------------------
        /// <summary>
        /// Gather all renderings from the host and from any additions the user(s) might have made.
        /// Set the best rendering either based on the user's (previous) choice of based on the one that
        /// occurs most frequently.
        /// </summary>
        // ---------------------------------------------------------------------------------------------
        public void LoadRenderings()
        {
            m_allRenderings = new HashSet<string>();
	        m_bestTranslation = null;
            int max = -1;
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            foreach (var term in m_termSurrogate.BiblicalTermIds)
            {
                int value = 4; // First rendering for each term is considered the best, so it counts more.
                var renderings = m_termRenderingRepo.GetTermRenderings(term);
                if (renderings == null)
                    continue;
                foreach (string termRendering in renderings.Where(rendering => rendering != null).
                    Select(t => t.Normalize(NormalizationForm.FormC)))
                {
                    m_allRenderings.Add(termRendering);

					occurrences.TryGetValue(termRendering, out var num);
                    occurrences[termRendering] = num + value;
                    if (num > max)
                    {
                        m_bestTranslation = termRendering;
                        max = num;
                    }
                    value = 1;
                }
            }
            KeyTermRenderingInfo info = RenderingInfo;
            if (info != null)
            {
                m_allRenderings.UnionWith(info.AddlRenderings.Where(r => r != null));
				if (!string.IsNullOrEmpty(info.PreferredRendering))
					m_bestTranslation = info.PreferredRendering;
				else if (m_bestTranslation == null)
					m_bestTranslation = info.AddlRenderings.FirstOrDefault(r => r != null);
            }

            if (m_bestTranslation == null)
                m_bestTranslation = string.Empty;
        }
        #endregion
    }
}


