// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: Part.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using SIL.Utils;

namespace SIL.Transcelerator
{
	public interface IPhrasePart
	{
        IEnumerable<Word> Words { get; }
		string Translation { get; }
		string DebugInfo { get; }
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the best rendering for this part in when used in the context of the given
		/// phrase.
		/// </summary>
		/// <remarks>If this part occurs more than once in the phrase, it is not possible to
		/// know which occurrence is which.</remarks>
		/// ------------------------------------------------------------------------------------
		string GetBestRenderingInContext(TranslatablePhrase phrase);
	}

    public sealed class Part : IPhrasePart
	{
		#region Data Members
		internal readonly List<Word> m_words;
		private readonly List<TranslatablePhrase> m_owningPhrases = new List<TranslatablePhrase>();
		private string m_translation = string.Empty;
		#endregion

		#region Constructors
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Part"/> class.
		/// </summary>
		/// <param name="words">The collection of words that make up the sub-phrase represented
		/// by this part.</param>
		/// ------------------------------------------------------------------------------------
		internal Part(IEnumerable<Word> words)
		{
			m_words = words.ToList();
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the words.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<Word> Words
		{
			get { return m_words; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the owning phrases.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<TranslatablePhrase> OwningPhrases
		{
			get { return m_owningPhrases; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the text of the sub-phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Text
		{
			get { return m_words.ToString(" "); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the translation.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Translation
		{
			get { return m_translation; }
			internal set {  m_translation = value ?? string.Empty; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a string with some debug info.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string DebugInfo
		{
			get { return OwningPhrases.Count() + ", " + Translation; }
		}
		#endregion

		#region Internal methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the spcified phrase as an owner of this Part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal void AddOwningPhrase(TranslatablePhrase phrase)
		{
			m_owningPhrases.Add(phrase);
		}
		#endregion

		#region Public methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The text of this part.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			 return Text;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Performs an implicit conversion from <see cref="SIL.Transcelerator.Part"/>
		/// to <see cref="System.String"/>.
		/// </summary>
		/// <param name="part">The part.</param>
		/// <returns>The result of the conversion.</returns>
		/// ------------------------------------------------------------------------------------
		public static implicit operator string(Part part)
		{
			return part == null ? null : part.Text;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the best rendering for this part in when used in the context of the given
		/// phrase.
		/// </summary>
		/// <remarks>If this part occurs more than once in the phrase, it is not possible to
		/// know which occurrence is which.</remarks>
		/// ------------------------------------------------------------------------------------
		public string GetBestRenderingInContext(TranslatablePhrase phrase)
		{
			return Translation;
		}
		#endregion
	}
}