// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2022' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace SIL.Transcelerator
{
	/// <summary>
	/// Abstract base class for a script generator. 
	/// </summary>
	public abstract class ScriptGenerator
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
