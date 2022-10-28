// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2010' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: SerializationHelper.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIL.Xml;

namespace SIL.Utils
{
	public static class ListSerializationHelper
	{
		/// ------------------------------------------------------------------------------------
        /// <summary>
        /// Loads a list of objects of the specified type by deserializing from the given string.
        /// If an error occurs during deserialization, a new list is created.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public static List<T> LoadOrCreateListFromString<T>(string data, bool reportErrorToUser)
        {
            Exception e;
            List<T> list = XmlSerializationHelper.DeserializeFromString<List<T>>(data, out e);
            if (e != null && reportErrorToUser)
                MessageBox.Show(e.ToString());
            return list ?? new List<T>();
        }
	}
}
