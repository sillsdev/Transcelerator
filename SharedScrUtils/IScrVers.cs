namespace SILUBS.SharedScrUtils
{
    public interface IScrVers
    {
        int GetLastChapter(int bookNum);

        int GetLastVerse(int bookNum, int chapterNum);

        int ChangeVersification(int reference, IScrVers scrVersSource);
    }
}