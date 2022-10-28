using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	public static class VersificationExtensions
	{
		public static IVerseRef CreateReference(this IVersification versification, int bbbcccvvv)
		{
			var bcvRef = new BCVRef(bbbcccvvv);
			return versification.CreateReference(bcvRef.Book, bcvRef.Chapter, bcvRef.Verse);
		}
	}
}
