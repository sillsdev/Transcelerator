using AddInSideViews;
using SILUBS.SharedScrUtils;

namespace SIL.Transcelerator
{
    public class ScrVers : IScrVers
    {
        private readonly IHost host;
        private readonly string versificationName;

        public ScrVers(IHost host, string versificationName)
        {
            this.host = host;
            this.versificationName = versificationName;
        }

        public int GetLastChapter(int bookNum)
        {
            return host.GetLastChapter(bookNum, versificationName);
        }

        public int GetLastVerse(int bookNum, int chapterNum)
        {
            return host.GetLastVerse(bookNum, chapterNum, versificationName);
        }

        public int ChangeVersification(int reference, IScrVers scrVersSource)
        {
            return this == scrVersSource ? reference :
                host.ChangeVersification(reference, ((ScrVers)scrVersSource).versificationName, versificationName);
        }
    }
}
