// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2008' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: IScrVers.cs
// --------------------------------------------------------------------------------------------
namespace SILUBS.SharedScrUtils
{
    public interface IScrVers
    {
        int GetLastChapter(int bookNum);

        int GetLastVerse(int bookNum, int chapterNum);

        int ChangeVersification(int reference, IScrVers scrVersSource);
    }
}