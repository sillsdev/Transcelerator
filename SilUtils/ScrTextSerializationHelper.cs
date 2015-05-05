// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ScrTextSerializationHelper.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;

namespace SIL.Utils
{
	public static class ScrTextSerializationHelper
	{
		#region XmlScrTextReader class
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Custom XmlTextReader that can preserve whitespace characters (spaces, tabs, etc.) 
		/// that are in XML elements. This allows us to properly handle deserialization of
		/// paragraph runs that contain runs that contain only whitespace characters.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private class XmlScrTextReader : XmlTextReader
		{
			private bool m_fKeepWhitespaceInElements;

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Initializes a new instance of the <see cref="XmlScrTextReader"/> class.
			/// </summary>
			/// <param name="reader">The stream reader.</param>
			/// <param name="fKeepWhitespaceInElements">if set to <c>true</c>, the reader
			/// will preserve and return elements that contain only whitespace, otherwise
			/// these elements will be ignored during a deserialization.</param>
			/// --------------------------------------------------------------------------------
			public XmlScrTextReader(TextReader reader, bool fKeepWhitespaceInElements) :
				base(reader)
			{
				m_fKeepWhitespaceInElements = fKeepWhitespaceInElements;
			}

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Initializes a new instance of the <see cref="XmlScrTextReader"/> class.
			/// </summary>
			/// <param name="filename">The filename.</param>
			/// <param name="fKeepWhitespaceInElements">if set to <c>true</c>, the reader
			/// will preserve and return elements that contain only whitespace, otherwise
			/// these elements will be ignored during a deserialization.</param>
			/// --------------------------------------------------------------------------------
			public XmlScrTextReader(string filename, bool fKeepWhitespaceInElements) :
				base(new StreamReader(filename))
			{
				m_fKeepWhitespaceInElements = fKeepWhitespaceInElements;
			}

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Gets the namespace URI (as defined in the W3C Namespace specification) of the 
			/// node on which the reader is positioned.
			/// </summary>
			/// <value></value>
			/// <returns>The namespace URI of the current node; otherwise an empty string.</returns>
			/// --------------------------------------------------------------------------------
			public override string NamespaceURI
			{
				get { return string.Empty; }
			}

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Reads the next node from the stream.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public override bool Read()
			{
				// Since we use this class only for deserialization, catch file not found
				// exceptions for the case when the XML file contains a !DOCTYPE declearation
				// and the specified DTD file is not found. (This is because the base class
				// attempts to open the DTD by merely reading the !DOCTYPE node from the 
				// current directory instead of relative to the XML document location.)
				try
				{
					return base.Read();
				}
				catch (FileNotFoundException)
				{
					return true;
				}
			}

			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Gets the type of the current node.
			/// </summary>
			/// --------------------------------------------------------------------------------
			public override XmlNodeType NodeType
			{
				get
				{
					if (m_fKeepWhitespaceInElements && 
						(base.NodeType == XmlNodeType.Whitespace || base.NodeType == XmlNodeType.SignificantWhitespace) && 
						Value != null && Value.IndexOf('\n') < 0 && Value.Trim().Length == 0)
					{
						// We found some whitespace that was most
						// likely whitespace we want to keep.
						return XmlNodeType.Text;
					}

					return base.NodeType;
				}
			}
		}

		#endregion

		#region Methods for XML serializing and deserializing data
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes XML from the specified string to an object of the specified type.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static T DeserializeFromString<T>(string input, out Exception e) where T : class
		{
			return DeserializeFromString<T>(input, false, out e);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes XML from the specified string to an object of the specified type.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static T DeserializeFromString<T>(string input, bool fKeepWhitespaceInElements,
			out Exception e) where T : class
		{
			T data = null;
			e = null;

			try
			{
				if (string.IsNullOrEmpty(input))
					return null;

				// Whitespace is not allowed before the XML declaration,
				// so get rid of any that exists.
				input = input.TrimStart();

				using (XmlScrTextReader reader = new XmlScrTextReader(
					new StringReader(input), fKeepWhitespaceInElements))
				{
					data = DeserializeInternal<T>(reader);
				}
			}
			catch (Exception outEx)
			{
				e = outEx;
			}

			return data;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes an object using the specified reader.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize</typeparam>
		/// <param name="reader">The reader.</param>
		/// <returns>The deserialized object</returns>
		/// ------------------------------------------------------------------------------------
		private static T DeserializeInternal<T>(XmlScrTextReader reader)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(T));
			deserializer.UnknownAttribute += deserializer_UnknownAttribute;
			return (T)deserializer.Deserialize(reader);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the UnknownAttribute event of the deserializer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Xml.Serialization.XmlAttributeEventArgs"/>
		/// instance containing the event data.</param>
		/// ------------------------------------------------------------------------------------
		static void deserializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			if (e.Attr.LocalName == "lang")
			{
				// This is special handling for the xml:lang attribute that is used to specify
				// the WS for the current paragraph, run in a paragraph, etc. The XmlTextReader
				// treats xml:lang as a special case and basically skips over it (but it does
				// set the current XmlLang to the specified value). This keeps the deserializer
				// from getting xml:lang as an attribute which keeps us from getting these values.
				// The fix for this is to look at the object that is being deserialized and,
				// using reflection, see if it has any fields that have an XmlAttribute looking
				// for the xml:lang and setting it to the value we get here. (TE-8328)
				object obj = e.ObjectBeingDeserialized;
				Type type = obj.GetType();
				foreach (FieldInfo field in type.GetFields())
				{
					object[] bla = field.GetCustomAttributes(typeof(XmlAttributeAttribute), false);
					if (bla.Length == 1 && ((XmlAttributeAttribute)bla[0]).AttributeName == "xml:lang")
					{
						field.SetValue(obj, e.Attr.Value);
					}
				}
			}
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Loads a list of objects of the specified type by deserializing from the given string.
        /// If an error occurs during deserialization, a new list is created.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public static List<T> LoadOrCreateListFromString<T>(string data, bool reportErrorToUser)
        {
            Exception e;
            List<T> list = DeserializeFromString<List<T>>(data, out e);
            if (e != null && reportErrorToUser)
                MessageBox.Show(e.ToString());
            return list ?? new List<T>();
        }
		#endregion
	}
}
