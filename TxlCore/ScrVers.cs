// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2014' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using AddInSideViews;
using SIL.Scripture;

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
