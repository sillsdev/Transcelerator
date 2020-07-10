using System.Collections.Generic;
using System.Linq;

namespace SIL.Transcelerator
{
	public class TransceleratorSections
	{
		private readonly SortedDictionary<int, SectionInfo> m_sections;

		public TransceleratorSections(QuestionSections sections)
		{
			m_sections = new SortedDictionary<int, SectionInfo>(
				sections.Items.ToDictionary(s => s.StartRef, s => new SectionInfo(s)));
		}

		public IEnumerable<SectionInfo> AllSections => m_sections.Values;

		#region needed for testing
		public int Count => m_sections.Count;

		/// <summary>Gets the element with the specified key.</summary>
		/// <param name="key">The key of the element to get or set.</param>
		/// <returns>The element with the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
		public SectionInfo this[int key] => m_sections[key];

		public IEnumerable<int> AllSectionStartRefs => m_sections.Keys;
		#endregion

		public SectionInfo GetSection(TranslatablePhrase phrase) =>
			GetSection(phrase.StartRef, phrase.EndRef);

		public SectionInfo GetSection(int startRef, int endRef)
		{
			if (!m_sections.TryGetValue(startRef, out var section))
				section = AllSections.FirstOrDefault(s => s.StartRef <= startRef && s.EndRef >= endRef);
			return section;
		}

		public IEnumerable<SectionInfo> GetSections(int scrRef) =>
			AllSections.Where(s => s.StartRef <= scrRef && s.EndRef >= scrRef);
	}
}
