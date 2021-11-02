using System.Collections.Generic;

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
		string GetBestRenderingInContext(TranslatablePhrase phrase, bool fast = false);
	}
}