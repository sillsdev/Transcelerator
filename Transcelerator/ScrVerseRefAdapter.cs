// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2021' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ScrVerseRefAdapter.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Linq;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	class ScrVerseRefAdapter : IScrVerseRef, IEquatable<IVerseRef>
	{
		private IVerseRef m_verseRef;
		private readonly IProject m_project;

		public ScrVerseRefAdapter(IVerseRef verseRef, IProject project)
		{
			m_verseRef = verseRef ?? throw new ArgumentNullException(nameof(verseRef));
			m_project = project ?? throw new ArgumentNullException(nameof(project));
		}

		private IVersification Versification => m_verseRef.Versification;
		
		public int BookNum
		{
			get => m_verseRef.BookNum;
			set => m_verseRef = Versification.CreateReference(value, m_verseRef.ChapterNum, m_verseRef.VerseNum);
		}
		
		public int ChapterNum
		{
			get => m_verseRef.ChapterNum;
			set => m_verseRef = Versification.CreateReference(m_verseRef.BookNum, value, m_verseRef.VerseNum);
		}

		public int VerseNum
		{
			get => m_verseRef.VerseNum;
			set => m_verseRef = Versification.CreateReference(m_verseRef.BookNum, m_verseRef.ChapterNum, value);
		}

		public string Book => m_verseRef.BookCode;
		public string Chapter => m_verseRef.ChapterNum.ToString();
		public string Verse => m_verseRef.VerseNum.ToString();
		public int LastChapter => Versification.GetLastChapter(BookNum);
		public int LastVerse => Versification.GetLastVerse(BookNum, ChapterNum);
		public bool VersificationHasVerseSegments => false;

		public IScrVerseRef UnBridge() => this;

		public void Simplify()
		{
		}

		public IScrVerseRef Create(string book, string chapter, string verse)
        {
            var verseRef = m_verseRef.Versification.CreateReference($"{book} {chapter}:{verse}");
            if (verseRef == null)
            {
                // In practice, the book (coming from the VerseControl) should always be valid.
                if (string.IsNullOrEmpty(book) || BCVRef.BookToNumber(book) < 0)
                    book = "MAT";
                if (string.IsNullOrEmpty(chapter))
                    chapter = "1";
                if (string.IsNullOrEmpty(verse))
                    verse = "1";
                verseRef = m_verseRef.Versification.CreateReference($"{book} {chapter}:{verse}");
            }

            return new ScrVerseRefAdapter(verseRef, m_project);
        }

        public IScrVerseRef Clone() => new ScrVerseRefAdapter(m_verseRef, m_project);

		public bool NextBook(BookSet present) => GetAdjacent(v => v.GetNextBook(m_project), present);

		public bool NextBook() => GetAdjacent(v => v.GetNextBook(m_project));

		public bool PreviousBook(BookSet present) => GetAdjacent(v => v.GetPreviousBook(m_project), present);

		public bool PreviousBook() => GetAdjacent(v => v.GetPreviousBook(m_project));

		public bool NextChapter(BookSet present) => GetAdjacent(v => v.GetNextChapter(m_project), present);

		public bool NextChapter() => GetAdjacent(v => v.GetNextChapter(m_project));

		public bool PreviousChapter(BookSet present) => GetAdjacent(v => v.GetPreviousChapter(m_project), present);

		public bool PreviousChapter() => GetAdjacent(v => v.GetPreviousChapter(m_project));

		public bool NextVerse(BookSet present) => GetAdjacent(v => v.GetNextVerse(m_project), present);

		public bool NextVerse() => GetAdjacent(v => v.GetNextVerse(m_project));

		public bool PreviousVerse(BookSet present) => GetAdjacent(v => v.GetPreviousVerse(m_project), present);

		public bool PreviousVerse() => GetAdjacent(v => v.GetPreviousVerse(m_project));

		public void AdvanceToLastSegment()
		{
		}

		#region Private helper methods
		private bool GetAdjacent(Func<IVerseRef, IVerseRef> getFunc, BookSet present)
		{
			IVerseRef adj = m_verseRef;
			do
			{
				adj = getFunc(adj);
				if (adj == null)
					return false;				
			} while (!present.SelectedBookNumbers.Contains(adj.BookNum));

			m_verseRef = adj;
			return true;
		}

		public bool GetAdjacent(Func<IVerseRef, IVerseRef> getFunc)
		{
			IVerseRef adj = getFunc(m_verseRef);
			if (adj == null)
				return false;				

			m_verseRef = adj;
			return true;
		}
		#endregion

		public bool Equals(IVerseRef other)
		{
			return other != null && m_verseRef.Equals(other);
		}
	}
}
