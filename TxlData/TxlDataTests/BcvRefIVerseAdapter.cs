using System;
using System.Collections.Generic;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Transcelerator
{
	internal class BcvRefIVerseAdapter : IVerseRef
	{
		static readonly IVersification s_mockedVersification = new TestScrVers();

		private readonly BCVRef m_bcvRef;

		internal BcvRefIVerseAdapter(BCVRef bcvRef)
		{
			m_bcvRef = bcvRef ?? throw new ArgumentNullException(nameof(bcvRef));
		}

		public bool Equals(IVerseRef other)
		{
			return other != null && other.BBBCCCVVV.Equals(BBBCCCVVV);
		}

		public int CompareTo(IVerseRef other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));
			return BBBCCCVVV.CompareTo(other.BBBCCCVVV);
		}

		public IVerseRef ChangeVersification(IVersification newVersification)
		{
			if (newVersification != Versification)
				throw new NotImplementedException();
			return this;
		}

		public IVerseRef GetPreviousVerse(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextVerse(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousChapter(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextChapter(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousBook(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextBook(IProject project)
		{
			throw new NotImplementedException();
		}

		public string BookCode => BCVRef.NumberToBookCode(m_bcvRef.Book);
		public int BookNum => m_bcvRef.Book;
		public int ChapterNum => m_bcvRef.Chapter;
		public int VerseNum => m_bcvRef.Verse;
		public int BBBCCCVVV => m_bcvRef.BBCCCVVV;
		public IVersification Versification => s_mockedVersification;
		public bool RepresentsMultipleVerses => false;
		public IReadOnlyList<IVerseRef> AllVerses => new List<IVerseRef>(new [] { this});
	}
}
