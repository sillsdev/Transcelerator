using System;
using System.Collections.Generic;
using System.IO;

namespace SIL.Transcelerator
{
	/// <summary>
	/// Abstract base class for a script generator. 
	/// </summary>
	internal abstract class ScriptGenerator
	{
		protected string VernIcuLocale { get; }
		protected Func<Func<TranslatablePhrase, bool>, IEnumerable<TranslatablePhrase>> Source { get; }

		protected ScriptGenerator(string vernIcuLocale, Func<Func<TranslatablePhrase, bool>, IEnumerable<TranslatablePhrase>> source)
		{
			VernIcuLocale = vernIcuLocale;
			Source = source;
		}

		public abstract void Generate(TextWriter tw, string bookId = null);
	}
}
