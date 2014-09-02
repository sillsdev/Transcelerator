// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International (derivitive work only)
//
//	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
//	This file incorporates work covered by the following permission notice:
//  Port of Snowball stemmers on C#
//  Original stemmers can be found on http://snowball.tartarus.org
//  Licence still BSD: http://snowball.tartarus.org/license.php
//  
//  Most of stemmers are ported from Java by Iveonik Systems ltd. (www.iveonik.com)
// </copyright>
#endregion
//
//	History:
//
//	18 March 2014
//
//	Changed namespace and added StagedStemming property. Also added StemmerAttribute. (TomB)
// ---------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace SIL.Stemmers
{
	public interface IStemmer
	{
		bool StagedStemming { get; set; }

		string Stem(string s);
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	internal sealed class StemmerAttribute : Attribute
	{
		// This is a positional argument
		public StemmerAttribute(string icuLocale)
		{
			IcuLocale = icuLocale;
		}

		public string IcuLocale { get; private set; }
	}

	public static class StemmerExtensions
	{
		public static string GetIcuLocale(this IStemmer stemmer)
		{
			return ((StemmerAttribute) stemmer.GetType().GetCustomAttributes(typeof (StemmerAttribute), true).Single()).IcuLocale;
		}
	}
}
