namespace SIL.Transcelerator
{
	public interface IHtmlScriptureExtractor
	{
		bool IncludeVerseNumbers { get; set; }

		string GetAsHtmlFragment(int startRef, int endRef);
	}
}