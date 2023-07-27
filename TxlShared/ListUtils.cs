// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2011' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ListUtils.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public static class ListUtils
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumeration, formatted as a string that contains the items, separated
		/// by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The enumeration.</param>
		/// <param name="separator">The separator.</param>
		/// ------------------------------------------------------------------------------------
		[Obsolete("Use version in SIL.Extensions in SIL.Core DLL")]
		public static string ToString<T>(this IEnumerable<T> list, string separator)
		{
			return list.ToString(separator, item => item.ToString());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumeration, formatted as a string that contains the items, separated
		/// by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The enumeration.</param>
		/// <param name="ignoreEmptyItems">True to ignore any items in the list that are empty
		/// or null, false to include them in the returned string.</param>
		/// <param name="separator">The separator.</param>
		/// ------------------------------------------------------------------------------------
		[Obsolete("Use version in SIL.Extensions in SIL.Core DLL")]
		public static string ToString<T>(this IEnumerable<T> list, bool ignoreEmptyItems,
			string separator)
		{
			return list.ToString(ignoreEmptyItems, separator, item => item == null ? string.Empty : item.ToString());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumeration, formatted as a string that contains the items, separated
		/// by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The enumeration.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="itemToStringFunction">A function that is applied to each item to turn
		/// it into a string.</param>
		/// ------------------------------------------------------------------------------------
		[Obsolete("Use version in SIL.Extensions in SIL.Core DLL")]
		public static string ToString<T>(this IEnumerable<T> list, string separator,
			Func<T, string> itemToStringFunction)
		{
			return list.ToString(true, separator, itemToStringFunction);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumeration, formatted as a string that contains the items, separated
		/// by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The enumeration.</param>
		/// <param name="ignoreEmptyItems">True to ignore any items in the list that are empty
		/// or null, false to include them in the returned string.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="itemToStringFunction">A function that is applied to each item to turn
		/// it into a string.</param>
		/// ------------------------------------------------------------------------------------
		[Obsolete("Use version in SIL.Extensions in SIL.Core DLL")]
		public static string ToString<T>(this IEnumerable<T> list, bool ignoreEmptyItems,
			string separator, Func<T, string> itemToStringFunction)
		{
			var bldr = new StringBuilder();
			bool fFirstTime = true;
			foreach (T item in list)
			{
				if (!ignoreEmptyItems || item != null)
				{
					string sItem = itemToStringFunction(item);
					if (!fFirstTime && (!string.IsNullOrEmpty(sItem) || !ignoreEmptyItems))
						bldr.Append(separator);
					bldr.Append(sItem);
				}
				fFirstTime = ignoreEmptyItems && bldr.Length == 0;
			}
			return bldr.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the enumeration, formatted as a string that contains the items, separated
		/// by the specified separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The enumeration.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="addToBuilderDelegate">Delegate to add the item to the string builder.</param>
		/// ------------------------------------------------------------------------------------
		[Obsolete("Use version in SIL.Extensions in SIL.Core DLL")]
		public static string ToString<T>(this IEnumerable<T> list, string separator,
			Action<T, StringBuilder> addToBuilderDelegate)
		{
			var bldr = new StringBuilder();
			foreach (T item in list)
			{
				if (item == null)
					continue;
				int cchBefore = bldr.Length;
				addToBuilderDelegate(item, bldr);
				if (cchBefore > 0 && bldr.Length > cchBefore)
					bldr.Insert(cchBefore, separator);
			}
			return bldr.ToString();
		}
	}
}
