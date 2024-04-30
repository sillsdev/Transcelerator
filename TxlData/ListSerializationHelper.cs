// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2010' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: SerializationHelper.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using SIL.Xml;

namespace SIL.Transcelerator
{
	public static class ListSerializationHelper
	{
		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Loads a list of objects of the specified type by deserializing from the given string.
        /// If an error occurs during deserialization, a new list is created.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public static List<T> LoadOrCreateListFromString<T>(string data, out Exception exception)
        {
            return XmlSerializationHelper.DeserializeFromString<List<T>>(data, out exception) ??
	            new List<T>();
        }
	}
}
