// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TxlVerseRef.cs
// ---------------------------------------------------------------------------------------------
using Paratext.PluginInterfaces;
using SIL.Scripture;
using static System.Int32;

namespace SIL.Transcelerator
{
	internal class TxlVerseRef : IScrVerseRef
	{
		private readonly IVersification m_versification;
		private BCVRef m_bcvRef;
		private static BookSet TheCanon { get; }

		static TxlVerseRef()
		{
			TheCanon = new BookSet(1, BCVRef.LastBook);
		}

		public TxlVerseRef(int bbbcccvvv, IVersification versification)
		{
			m_versification = versification;
			m_bcvRef = new BCVRef(bbbcccvvv);
		}

		/// <summary>
		/// Get or set Book based on book number.
		/// </summary>
		/// <exception cref="VerseRefException">If BookNum is set to an invalid value</exception>
		public int BookNum
		{
			get => m_bcvRef.Book;
			set
			{
				if (value <= 0 || value > BCVRef.LastBook)
					throw new VerseRefException("Book number must be greater than zero and less than or equal to the number of books in the canon.");
				m_bcvRef.Book = value;
			}
		}
		
		/// <summary>
		/// Gets chapter number. -1 if not valid
		/// </summary>
		/// <exception cref="VerseRefException">If ChapterNum is negative</exception>
		public int ChapterNum
		{
			get => m_bcvRef.Chapter;
			set
			{
				if (value < 0)
					throw new VerseRefException("Chapter number can not be negative.");
				if (value > 999)
					throw new VerseRefException("Invalid chapter number: " + value);
				m_bcvRef.Chapter = value;
			}
		}

		/// <summary>
		/// Gets verse start number. -1 if not valid
		/// </summary>
		/// <exception cref="VerseRefException">If VerseNum is negative</exception>
		public int VerseNum
		{
			get => m_bcvRef.Verse;
			set
			{
				if (value < 0)
					throw new VerseRefException("Verse number can not be negative.");
				if (value > 999)
					throw new VerseRefException("Invalid verse number: " + value);
				m_bcvRef.Verse = value;
			}
		}

		/// <summary>
		/// Gets the book of the reference. This is the 
		/// three letter abbreviation in capital letters. e.g. "MAT"
		/// </summary>
		public string Book => BCVRef.NumberToBookCode(BookNum);

		/// <summary>
		/// Gets the chapter of the reference. e.g. "3"
		/// </summary>
		public string Chapter => ChapterNum <= 0 ? string.Empty : ChapterNum.ToString();

		/// <summary>
		///  Gets the verse of the reference e.g. "11"
		/// </summary>
		public string Verse => VerseNum <= 0 ? string.Empty : VerseNum.ToString();

		public int LastChapter => m_versification.GetLastChapter(BookNum);

		public int LastVerse => m_versification.GetLastVerse(BookNum, ChapterNum);

		/// <summary>
		/// Gets whether the versification of this reference has verse segments
		/// </summary>
		public bool VersificationHasVerseSegments => false;

		/// <summary>
		/// Returns verse ref with no bridging, but maintaining segments like "1a".
		/// </summary>
		public IScrVerseRef UnBridge() => this;

		/// <summary>
		/// Simplifies this verse ref so that it has no bridging of verses or 
		/// verse segments (no-op in this implementation).
		/// </summary>
		public void Simplify()
		{
		}

		/// <summary>
		/// Gets a new object having the specified book, chapter and verse values (with the
		/// same versification).
		/// </summary>
		public IScrVerseRef Create(string book, string chapter, string verse)
		{
			return new TxlVerseRef(new BCVRef(BCVRef.BookToNumber(book), Parse(chapter),
				Parse(verse)), m_versification);
		}

		/// <summary>
		/// Makes a clone of the reference
		/// </summary>
		public IScrVerseRef Clone()
		{
			return new TxlVerseRef(m_bcvRef, m_versification);
		}

		/// <summary>
		/// Tries to move to the next book among a set of books present.
		/// </summary>
		/// <param name="present">Set of books present or selected.</param>
		/// <returns>true if successful</returns>
		public bool NextBook(BookSet present)
		{
			int curBook = BookNum;
			int newBook = present.NextSelected(curBook);
			if (newBook == curBook)
				return false;
			BookNum = newBook;
			ChapterNum = 1;
			VerseNum = 1; // Transcelerator never deals with "verse 0"
			return true;
		}

		/// <summary>
		/// Tries to move to the next book in the canon.
		/// </summary>
		/// <returns>True if successful.</returns>
		public bool NextBook() => NextBook(TheCanon);

		/// <summary>
		/// Tries to move to the previous book among a set of books present.
		/// </summary>
		/// <param name="present">Set of books present or selected.</param>
		/// <returns>true if successful</returns>
		public bool PreviousBook(BookSet present)
		{
			int curBook = BookNum;
			int newBook = present.PreviousSelected(curBook);
			if (newBook == curBook)
				return false; //no previous book
			BookNum = newBook;
			ChapterNum = 1;
			VerseNum = 1;
			return true;
		}

		/// <summary>
		/// Tries to move to the previous book in the entire canon superset.
		/// </summary>
		/// <returns>true if successful</returns>
		public bool PreviousBook() => PreviousBook(TheCanon);

		/// <summary>
		/// Tries to move to the next chapter.
		/// </summary>
		/// <returns>true if successful</returns>
		public bool NextChapter(BookSet present)
		{
			// If current book doesn't exist, try jump to next.
			if (!present.IsSelected(BookNum))
				return NextBook(present);
			int newPosition = ChapterNum + 1;
			if (newPosition > LastChapter)
				return NextBook(present);

			ChapterNum = newPosition;
			VerseNum = 1;

			return true;
		}

		/// <summary>
		/// Tries to move to the next chapter.
		/// </summary>
		/// <returns>true if successful</returns>
		public bool NextChapter() => NextChapter(TheCanon);

		/// <summary>
		/// Tries to move to the previous chapter.
		/// </summary>
		/// <returns>true if successful</returns>
		public bool PreviousChapter(BookSet present)
		{
			// current ref doesn't exist? try find an existing one prior
			if (!present.IsSelected(BookNum))
				return PreviousBookLastChapter(present);
			int newPosition = ChapterNum - 1;
			if (newPosition < 1)
				return PreviousBookLastChapter(present);
			VerseNum = 1; // Use property to reset verse string
			ChapterNum = newPosition;
			return true;
		}

		bool PreviousBookLastChapter(BookSet present)
		{
			bool result = PreviousBook(present);
			if (result)
				ChapterNum = LastChapter != Versification.NonCanonicalLastChapterOrVerse ? LastChapter : 1;
			return result;
		}

		/// <summary>
		/// Tries to move to the previous chapter.
		/// </summary>
		/// <returns>true if successful</returns>
		public bool PreviousChapter() => PreviousChapter(TheCanon);

		/// <summary>
		/// Tries to move to the next verse (or verse segment, if available in the current versification).
		/// </summary>
		/// <returns>true if successful, false if at end of scripture</returns>
		public bool NextVerse(BookSet present)
		{
			// avoid incrementing through a blank book
			if (!present.IsSelected(BookNum))
				return NextBook(present);

			if (VerseNum >= m_versification.GetLastVerse(BookNum, ChapterNum))
					return NextChapter(present);
			VerseNum++;
			return true;
		}

		/// <summary>
		/// Tries to move to the next verse (or verse segment, if available in the current versification).
		/// </summary>
		/// <returns>true if successful, false if at end of scripture</returns>
		public bool NextVerse() => NextVerse(TheCanon);

		/// <summary>
		/// Tries to move to the previous verse (or verse segment, if available in the current versification).
		/// </summary>
		/// <returns>true if successful, false if at beginning of scripture</returns>
		public bool PreviousVerse(BookSet present)
		{
			// avoid moving through nonexistent books.
			if (!present.IsSelected(BookNum))
				return PreviousChapterLastVerse(present);

			if (VerseNum == 1)
				return PreviousChapterLastVerse(present);
			VerseNum--; // Use property to reset verse string
			return true;
		}

		bool PreviousChapterLastVerse(BookSet present)
		{
			bool result;
			// This logic prevents simple nav to chapter 0:
			// Book doesn't exist or we just asked for the chapter before #1
			if (!present.IsSelected(BookNum) || ChapterNum <= 1)
				result = PreviousBookLastChapter(present);
			else
			{
				result = true;
				ChapterNum--;
			}
			if (result)
				VerseNum = LastVerse;
			return result;
		}

		/// <summary>
		/// Tries to move to the previous verse (or verse segment, if available in the current versification).
		/// </summary>
		/// <returns>true if successful, false if at beginning of scripture</returns>
		public bool PreviousVerse() => PreviousVerse(TheCanon);

		/// <summary>
		/// Advances to the last segment associated with this verse (no-op in this implementation).
		/// </summary>
		public void AdvanceToLastSegment()
		{
		}
	}
}
