// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2011' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International (derivitive work only)
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
//		This file incorporates work covered by the following permission notice:
//		The software is completely free for any purpose, unless notes at the head of
//		the program text indicates otherwise (which is rare). In any case, the notes
//		about licensing are never more restrictive than the BSD License.
// </copyright>
#endregion
//
// File: PorterStemmer.cs
//
//Porter stemmer in CSharp, based on the Java port. The original paper is in
//
//    Porter, 1980, An algorithm for suffix stripping, Program, Vol. 14,
//    no. 3, pp 130-137,
//
//See also http://www.tartarus.org/~martin/PorterStemmer
//

//
//History:
//
//Release 1
//
//Bug 1 (reported by Gonzalo Parra 16/10/99) fixed as marked below.
//The words 'aed', 'eed', 'oed' leave k at 'a' for step 3, and b[k-1]
//is then out outside the bounds of b.
//
//Release 2
//
//Similarly,
//
//Bug 2 (reported by Steve Dyrdahl 22/2/00) fixed as marked below.
//'ion' by itself leaves j = -1 in the test for 'ion' in step 5, and
//b[j] is then outside the bounds of b.
//
//Release 3
//
//Considerably revised 4/9/00 in the light of many helpful suggestions
//from Brian Goetz of Quiotix Corporation (brian@quiotix.com).
//
//Release 4
// Made more suitable for use in .Net
//
//Cheers Leif
//
// Release 5
// Added ability to strip off possessives & prevent removal of final "ee" (TomB)
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Stemmer, implementing the Porter Stemming Algorithm
	/// The Stemmer class transforms a word into its root form. The input word can be provided
	/// a character at time (by calling add()), or at once by calling one of the various
	/// stem(something) methods.
	/// </summary>
	/// ----------------------------------------------------------------------------------------  
	[ClassInterface( ClassInterfaceType.None )]
	public class PorterStemmer
	{
		private static Dictionary<string, string> s_dictionaryStaged = new Dictionary<string, string>();
		private static Dictionary<string, string> s_dictionaryFull = new Dictionary<string, string>();
		private readonly char[] b;
		private int i,     /* offset into b */
			iEnd, /* offset to end of stemmed word */
			j, k;
		protected bool changed;

		protected PorterStemmer(string s) 
		{
			i = s.Length;
			b = new char[i];
			for (int c = 0; c < i; c++)
				b[c] = s[c];

			changed = false;
			iEnd = 0;
		}

		/// <summary>
		/// Gets the stem for the given term, with fast dictionary lookup if
		/// it is a term that has been previously stemmed.
		/// </summary>
		/// <param name="term">The term whose stem is to be found</param>
		public static string StemTerm(string term)
		{
			lock (s_dictionaryFull)
			{
				if (s_dictionaryFull.TryGetValue(term, out var stem))
					return stem;

				stem = new PorterStemmer(term).Stem();
				s_dictionaryFull[term] = stem;
				return stem;
			}
		}

		/// <summary>
		/// For the given term, gets all unique stages of stemming (with fast dictionary lookup) by
		/// recursively peeling off affixes.
		/// </summary>
		/// <param name="term">The term whose stemmed forms are to be found</param>
		/// <returns></returns>
		public static IEnumerable<string> GetStemmedForms(string term)
		{
			string stem;
			lock (s_dictionaryStaged)
			{
				if (!s_dictionaryStaged.TryGetValue(term, out stem))
				{
					stem = new PorterStemmer(term).Stem(true);
					s_dictionaryStaged[term] = stem;
				}
			}

			if (stem != term)
			{
				yield return stem;
				foreach (var subStem in GetStemmedForms(stem))
					yield return subStem;
			}
		}

		private string GetTerm()
		{
			return new string(b, 0, iEnd);
		}

		public override string ToString() => GetTerm();

		/// <summary>
		/// True if b[i] is a consonant.
		/// </summary>
		private bool cons(int i) 
		{
			switch (b[i]) 
			{
				case 'a': case 'e': case 'i': case 'o': case 'u': return false;
				case 'y': return i == 0 || !cons(i - 1);
				default: return true;
			}
		}

		/// <summary>
		/// Measures the number of consonant sequences between 0 and j. if c is
		/// a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
		/// presence,
		///
		/// <c><v>       gives 0
		/// <c>vc<v>     gives 1
		/// <c>vcvc<v>   gives 2
		/// <c>vcvcvc<v> gives 3
		/// ....
		/// </summary>
		private int m() 
		{
			int n = 0;
			int i = 0;
			while(true) 
			{
				if (i > j) return n;
				if (! cons(i)) break; i++;
			}
			i++;
			while(true) 
			{
				while(true) 
				{
					if (i > j) return n;
					if (cons(i)) break;
					i++;
				}
				i++;
				n++;
				while(true) 
				{
					if (i > j) return n;
					if (! cons(i)) break;
					i++;
				}
				i++;
			}
		}

		/// <summary>
		/// True if 0,...j contains a vowel
		/// </summary>
		private bool VowelInStem() 
		{
			int i;
			for (i = 0; i <= j; i++)
				if (! cons(i))
					return true;
			return false;
		}

		/// <summary>
		/// True if j,(j-1) contain a double consonant.
		/// </summary>
		private bool IsDoubleCons(int j) 
		{
			if (j < 1)
				return false;
			if (b[j] != b[j-1])
				return false;
			return cons(j);
		}

		/// <summary>
		/// True if i-2,i-1,i has the form consonant - vowel - consonant
		/// and also if the second c is not w,x or y. this is used when trying to
		/// restore an e at the end of a short word. e.g.
		///
		/// 	cav(e), lov(e), hop(e), crim(e), but
		/// 	snow, box, tray.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		private bool cvc(int i) 
		{
			if (i < 2 || !cons(i) || cons(i-1) || !cons(i-2))
				return false;
			int ch = b[i];
			if (ch == 'w' || ch == 'x' || ch == 'y')
				return false;
			return true;
		}

		private bool EndsWith(String s) 
		{
			int l = s.Length;
			int o = k-l+1;
			if (o < 0)
				return false;
			char[] sc = s.ToCharArray();
			for (int i = 0; i < l; i++)
				if (b[o+i] != sc[i])
					return false;
			j = k-l;
			return true;
		}

		/// <summary>
		/// Sets (j+1),...k to the characters in the string s, readjusting k.
		/// </summary>
		private void SetTo(string s) 
		{
			int l = s.Length;
			int o = j+1;
			char[] sc = s.ToCharArray();
			for (int i = 0; i < l; i++)
				b[o+i] = sc[i];
			k = j+l;
			changed = true;
		}

		/* r(s) is used further down. */
		private void r(string s) 
		{
			if (m() > 0)
				SetTo(s);
		}

		/* Step1 gets rid of plurals and -ed or -ing. e.g.
			   caresses  ->  caress
			   ponies    ->  poni
			   ties      ->  ti
			   caress    ->  caress
			   cats      ->  cat

			   feed      ->  feed
			   agreed    ->  agree
			   disabled  ->  disable

			   matting   ->  mat
			   mating    ->  mate
			   meeting   ->  meet
			   milling   ->  mill
			   messing   ->  mess

			   meetings  ->  meet

		*/
		private void Step1A()
		{
			if (EndsWith("s'"))
			{
				k--;
			}
			if (b[k] == 's')
			{
				if (EndsWith("sses"))
					k -= 2;
				else if (EndsWith("ies"))
					SetTo("y");
				else if (EndsWith("'s"))
					k -= 2;
				else if (b[k - 1] == 'u')
				{
					if (b.Length > 3)
					{
						if (EndsWith("ous"))
							k -= 3;
						else if (cons(k - 2) && b[k - 2] != 's' && !EndsWith("stus"))
						{
							k--;
						}
					}
				}
				else if (b[k - 1] != 's')
					k--;
			}
		}

		private void Step1B()
		{

			if (EndsWith("eed")) 
			{
				if (m() > 0)
					k--;
			}
			else if ((EndsWith("ed") || EndsWith("ing")) && VowelInStem()) 
			{
				k = j;
				if (EndsWith("at"))
					SetTo("ate");
				else if (EndsWith("bl"))
					SetTo("ble");
				else if (EndsWith("iz"))
					SetTo("ize");
				else if (IsDoubleCons(k)) 
				{
					k--;
					int ch = b[k];
					if (ch == 'l' || ch == 's' || ch == 'z')
						k++;
				}
				else if (m() == 1 && cvc(k)) SetTo("e");
			}
		}

		/// <summary>
		/// Step2 turns terminal y to i when there is another vowel in the stem, unless the
		/// immediately preceding letter is a vowel.
		/// </summary>
		private void Step2() 
		{
			if (EndsWith("y") && (k > 0 && cons(k - 1)) && VowelInStem())
			{
				changed = true;
				b[k] = 'i';
			}
		}

		/// <summary>
		/// Step3 maps double suffixes to single ones. so -ization ( = -ize plus
		/// -ation) maps to -ize etc. note that the string before the suffix must give
		/// m() > 0.
		/// </summary>
		private void Step3() 
		{
			if (k == 0)
				return;
			
			/* For Bug 1 */
			switch (b[k-1]) 
			{
				case 'a':
					if (EndsWith("ational")) { r("ate"); break; }
					if (EndsWith("tional")) { r("tion"); break; }
					break;
				case 'c':
					if (EndsWith("enci")) { r("ence"); break; }
					if (EndsWith("anci")) { r("ance"); break; }
					break;
				case 'e':
					if (EndsWith("izer")) { r("ize"); break; }
					break;
				case 'l':
					if (EndsWith("bli")) { r("ble"); break; }
					if (EndsWith("ealli")) { SetTo("eal"); break; }
					if (EndsWith("alli")) { r("al"); break; }
					if (EndsWith("ulli")) { SetTo((j == 0) ? "ull" : "ul"); break; }
					if (EndsWith("entli")) { r("ent"); break; }
					if (EndsWith("eli") && (changed)) { r("e"); break; }
					if (EndsWith("obeli")) { SetTo("obel"); break; }
					if (EndsWith("ousli")) { r("ous"); break; }
					break;
				case 'o':
					if (EndsWith("ization")) { r("ize"); break; }
					if (EndsWith("ation")) { r("ate"); break; }
					if (EndsWith("ator")) { r("ate"); break; }
					break;
				case 's':
					if (EndsWith("alism")) { r("al"); break; }
					if (EndsWith("iveness")) { r("ive"); break; }
					if (EndsWith("fulness")) { r("ful"); break; }
					if (EndsWith("ousness")) { r("ous"); break; }
					break;
				case 't':
					if (EndsWith("aliti")) { r("al"); break; }
					if (EndsWith("iviti")) { r("ive"); break; }
					if (EndsWith("biliti")) { r("ble"); break; }
					break;
				case 'g':
					if (EndsWith("logi")) { r("log"); break; }
					break;
				default :
					break;
			}
		}

		/* Step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
		private void Step4() 
		{
			switch (b[k]) 
			{
				case 'e':
					if (EndsWith("icate")) { r("ic"); break; }
					if (EndsWith("ative")) { r(""); break; }
					if (EndsWith("alize")) { r("al"); break; }
					break;
				case 'i':
					if (EndsWith("iciti")) { r("ic"); break; }
					break;
				case 'l':
					if (EndsWith("ical")) { r("ic"); break; }
					if (EndsWith("ful")) { r(""); break; }
					break;
				case 's':
					if (EndsWith("ness")) { r(""); break; }
					break;
			}
		}

		/* Step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
		private void Step5() 
		{
			if (k == 0)
				return;

			/* for Bug 1 */
			switch ( b[k-1] ) 
			{
				case 'a':
					if (EndsWith("al")) break; return;
				case 'c':
					if (EndsWith("ance")) break;
					if (EndsWith("ence")) break; return;
				case 'e':
					if (EndsWith("er")) break; return;
				case 'i':
					if (EndsWith("ic")) break; return;
				case 'l':
					if (EndsWith("able")) break;
					if (EndsWith("ible")) break;
					if (EndsWith("ple")) break; return;
				case 'n':
					if (EndsWith("ant")) break;
					if (EndsWith("ement")) break;
					if (EndsWith("ment")) break;
					/* element etc. not stripped before the m */
					if (EndsWith("ent")) break; return;
				case 'o':
					if (EndsWith("ion") && j >= 0 && (b[j] == 's' || b[j] == 't')) break;
					/* j >= 0 fixes Bug 2 */
					if (EndsWith("ou")) break; return;
					/* takes care of -ous */
				case 's':
					if (EndsWith("ism")) break; return;
				case 't':
					if (EndsWith("ate")) break;
					if (EndsWith("iti")) break; return;
				case 'u':
					if (EndsWith("ous")) break; return;
				case 'v':
					if (EndsWith("ive")) break; return;
				case 'z':
					if (EndsWith("ize")) break; return;
				default:
					return;
			}
			if (m() > 1)
				k = j;
		}

		/* Step6() removes a final -e if m() > 1. */
		private void Step6() 
		{
			j = k;
			
			if (b[k] == 'e') 
			{
				if (EndsWith("ple") || EndsWith("tle") || k < 3)
					return;
				int a = m();
				if ((a > 1 || a == 1 && !cvc(k-1)) && b[k-1] != 'e')
					k--;
			}
			if (b[k] == 'l' && IsDoubleCons(k) && m() > 1)
				k--;
		}

		/// <summary>
		/// Stem the word in the Stemmer buffer and return the result.
		/// </summary>
		protected string Stem(bool stagedStemming = false) 
		{
			k = i - 1;
			if (k > 1) 
			{
				Step1A();
				if (!IsSpecialCaseNoun() && !b.Contains('-'))
				{
					if ((!changed && k == i - 1) || !stagedStemming)
					{
						Step1B();
						if ((!changed && k == i - 1) || !stagedStemming)
						{
							Step2();
							Step3();
							if (!changed || !stagedStemming)
							{
								Step4();
								if (!changed || !stagedStemming)
								{
									Step5();
									if ((!changed && k == i - 1) || !stagedStemming)
										Step6();
								}
							}
						}
					}
				}
			}
			iEnd = k+1;
			i = 0;

			return GetTerm();
		}

		private bool IsSpecialCaseNoun()
		{
			return (EndsWith("morning") || EndsWith("olive") ||
				EndsWith("someone") || EndsWith("anyone") || EndsWith("everyone") ||
				EndsWith("something") || EndsWith("anything") || EndsWith("everything") || EndsWith("nothing") ||
				EndsWith("somebody") || EndsWith("anybody") || EndsWith("everybody") || EndsWith("nobody"));
		}
	}
}
