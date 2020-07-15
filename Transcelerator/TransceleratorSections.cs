using System;
using System.Collections.Generic;
using System.Linq;
using SIL.Extensions;

namespace SIL.Transcelerator
{
	public class TransceleratorSections
	{
		private readonly SortedList<int, ISectionInfo> m_sections;

		public TransceleratorSections(QuestionSections sections)
		{
			m_sections = new SortedList<int, ISectionInfo>(sections.Items.Length);
			m_sections.AddRange(sections.Items.Select(s => new KeyValuePair<int, ISectionInfo>(s.StartRef, s)));
		}

		public IList<ISectionInfo> AllSections => m_sections.Values;

		#region needed for testing
		public int Count => m_sections.Count;
		public IEnumerable<int> AllSectionStartRefs => m_sections.Keys;
		#endregion

		/// <summary>Gets the section with the specified start reference.</summary>
		/// <param name="startRef">The start reference (in BBBCCCVVV form) of the section to get.</param>
		/// <returns>The section with the specified start reference, or null if no matching section is found.</returns>
		public ISectionInfo Find(int startRef)
		{
			return m_sections.TryGetValue(startRef, out ISectionInfo value) ? value : null;
		}

		/// <summary>Gets the section corresponding to the specified question/phrase.</summary>
		/// <param name="phrase">The phrase/question whose section is the be retrieved.</param>
		/// <exception cref="ArgumentException">An invalid phrase was supplied, which does
		/// not correspond to any section.</exception>
		/// <exception cref="T:System.InvalidOperationException">No section started with
		/// the given phrase's StartRef and yet more than one section contained it.</exception>
		public ISectionInfo Find(ITranslatablePhrase phrase)
		{
			var section = Find(phrase.PhraseKey.StartRef);
			if (section != null)
				return section;

			section = GetSectionsThatContainRange(phrase.PhraseKey.StartRef, phrase.PhraseKey.EndRef)
				.SingleOrDefault().Value;
			if (section != null)
				return section;
			throw new ArgumentException("An invalid phrase was supplied, which does not correspond to any section.",
				nameof(phrase));
		}

		public IEnumerable<KeyValuePair<int, ISectionInfo>> GetSectionsThatContainRange(int startRef, int endRef)
		{
			if (startRef > endRef)
				throw new ArgumentException("End reference must be greater than or equal to start reference.");
			if (startRef < m_sections.Values[0].StartRef)
				throw new ArgumentOutOfRangeException("Invalid start reference");
			if (endRef > m_sections.Values.Last().EndRef)
				throw new ArgumentOutOfRangeException("Invalid end reference");
			var index = m_sections.Keys.BinarySearch(startRef);
			foreach (var section in m_sections.Select(s => s.Value)
				.TakeWhile(s => s.StartRef <= startRef && s.EndRef >= endRef))
			{
				yield return new KeyValuePair<int, ISectionInfo>(index++, section);
			}
		}
	}
}
