// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ListUtilsTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class ListUtilsTests
	{
		#region ToString extension method tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of integers with no special
		/// function to convert them to strings and a null separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_ValueType_NoFuncNullSeparator()
		{
			IEnumerable<int> list = new[] { 5, 6, 2, 3 };
			Assert.AreEqual("5623", list.ToString(null));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of integers with no special
		/// function to convert them to strings and a specified separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_ValueType_NoFunc_IgnoreEmpty()
		{
			IEnumerable<int> list = new[] { 5, 0, 2, 3 };
			Assert.AreEqual("5,0,2,3", list.ToString(true, ","));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of chars with no special
		/// function to convert them to strings and a comma and space as the separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_NoFuncCommaSeparator()
		{
			IEnumerable<char> list = new[] { '#', 'r', 'p', '3' };
			Assert.AreEqual("#, r, p, 3", list.ToString(", "));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of strings with a special
		/// function to convert the strings to lowercase and the newline as the separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_Func()
		{
			IEnumerable<string> list = new[] { "ABC", "XYz", "p", "3w", "ml" };
			Assert.AreEqual("abc" + Environment.NewLine + "xyz" + Environment.NewLine + "p" + Environment.NewLine + "3w" + Environment.NewLine + "ml",
				list.ToString(Environment.NewLine, item => item.ToLowerInvariant()));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for an empty dictionary.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_EmptyList()
		{
			Dictionary<string, int> list = new Dictionary<string, int>();
			Assert.AreEqual(string.Empty, list.ToString(Environment.NewLine, item => item.Key));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of strings that has nulls
		/// and empty strings which must be excluded, with a special function to convert the
		/// strings to lowercase and a space-padded ampersand as the separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_ExcludeNullAndEmptyItems()
		{
			IEnumerable<string> list = new[] { "ABC", null, "p", string.Empty };
			Assert.AreEqual("abc & p", list.ToString(" & ", item => item.ToLowerInvariant()));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of strings that has nulls
		/// and empty strings whose positions must be preserved in the list, with a special
		/// function to convert the strings to lowercase and a space-padded ampersand as the
		/// separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_IncludeNullAndEmptyItems()
		{
			IEnumerable<string> list = new[] { string.Empty, "ABC", null, "p", string.Empty };
			Assert.AreEqual("|ABC||p|", list.ToString(false, "|"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of strings that has nulls
		/// and empty strings whose positions must be preserved in the list, with a special
		/// function to convert the strings to lowercase and a space-padded ampersand as the
		/// separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_IgnoreNullAndEmptyItems()
		{
			IEnumerable<string> list = new[] { string.Empty, "ABC", null, "p", string.Empty };
			Assert.AreEqual("ABC|p", list.ToString(true, "|"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of objects of both class and
		/// value types that includes nulls and empty strings, with no special function to
		/// convert them to strings and a space-padded ampersand as the separator string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_ObjectsOfValueAndClassTypes()
		{
			IEnumerable<object> list = new object[] { "ABC", null, 0, string.Empty, 'r' };
			Assert.AreEqual("ABC & 0 & r", list.ToString(" & "));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ToString extension method for a collection of objects of both class and
		/// value types that includes nulls and empty strings, with a method that adds the
		/// strings plus the accumulated length of the builder and a comma as the separator
		/// string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void EnumerableToString_FuncThatTakesStrBuilder()
		{
			IEnumerable<string> list = new[] { "A", "BC", "XYZ" };
			const string kSep = ",";
			int kcchSep = kSep.Length;
			Assert.AreEqual("A1,BC5,XYZ10", list.ToString(kSep, (item, bldr) =>
			{
				int cch = bldr.Length > 0 ? kcchSep : 0;
				bldr.Append(item);
				bldr.Append(bldr.Length + cch);
			}));
		}
		#endregion
	}
}
