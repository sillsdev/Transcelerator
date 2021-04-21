// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2013' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: DataFileAccessor.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AddInSideViews;
using SIL.Xml;

namespace SIL.Transcelerator
{
    public abstract class DataFileAccessor
	{
		public enum DataFileId
		{
			Translations,
			QuestionCustomizations,
			PhraseSubstitutions,
			KeyTermRenderingInfo,
			TermRenderingSelectionRules,
		}

		public enum BookSpecificDataFileId
		{
			ScriptureForge,
		}

		protected static string GetFileName(DataFileId fileId)
        {
            switch (fileId)
            {
                case DataFileId.Translations: return "Translations of Checking Questions.xml";
                case DataFileId.QuestionCustomizations: return "Question Customizations.xml";
                case DataFileId.PhraseSubstitutions: return "Phrase substitutions.xml";
                case DataFileId.KeyTermRenderingInfo: return "Key term rendering info.xml";
                case DataFileId.TermRenderingSelectionRules: return "Term rendering selection rules.xml";
				default:
		            throw new ArgumentException("Bogus", nameof(fileId));
            }
		}

		protected static string GetBookSpecificFileName(BookSpecificDataFileId fileId, string bookId)
		{
			switch (fileId)
			{
				case BookSpecificDataFileId.ScriptureForge: return $"Translated Checking Questions for {bookId}.xml";
				default:
					throw new ArgumentException("Bogus", nameof(fileId));
			}
		}
		
		/// <summary>
		/// Since "string" is also an object, this fluent method checks whether
		/// data is a IConvertible object (which string is) and throws an exception.
		/// This prevents a caller accidentally serializing an object (perhaps with
		/// an encoding other than UTF-8) and then calling a Write method in this
		/// that is expecting a "normal" XML-serializable object with the serialized
		/// string and having it get re-serialized. Not sure if that would work or
		/// throw an exception, but by dealing with it here, it will be obvious what
		/// went wrong.
        /// </summary>
		private T CheckDataIsXmlSerializable<T>(T data)
        {
			if (data is IConvertible)
				throw new ArgumentException($"This method serializes the data provided to an XML string. Doing that for a {data.GetType()} does not make sense.", nameof(data));
			return data;
		}

		public void Write<T>(DataFileId fileId, T data) => 
			Write(fileId, XmlSerializationHelper.SerializeToString(CheckDataIsXmlSerializable(data), Encoding.UTF8));

		protected abstract void Write(DataFileId fileId, string data);

		public void WriteBookSpecificData<T>(BookSpecificDataFileId fileId, string bookId, T data)
		{
			var serializedData = XmlSerializationHelper.SerializeToString(
				CheckDataIsXmlSerializable(data), Encoding.UTF8);
            if (serializedData == null)
                throw new SerializationException($"An error occurred serializing {bookId} data for {fileId}.");
			WriteBookSpecificData(fileId, bookId, serializedData);
		}

		public abstract bool BookSpecificDataExists(BookSpecificDataFileId fileId, string bookId);

		protected abstract void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data);

		public abstract string Read(DataFileId fileId);

        public abstract bool Exists(DataFileId fileId);

        public abstract DateTime ModifiedTime(DataFileId fileId);
    }

    public class ParatextDataFileAccessor : DataFileAccessor
    {
        private readonly Func<string, string> m_getPlugInData;
        private readonly Action<string, string> m_putPlugInData;
        private readonly Func<string, DateTime> m_getPlugInDataModifiedTime;

        public ParatextDataFileAccessor(Func<string, string> getPlugInData,
            Action<string, string> putPlugInData,
            Func<string, DateTime> getPlugInDataModifiedTime)
        {
            m_getPlugInData = getPlugInData;
            m_putPlugInData = putPlugInData;
            m_getPlugInDataModifiedTime = getPlugInDataModifiedTime;
        }

        public static Dictionary<string, IPluginDataFileMergeInfo> GetDataFileKeySpecifications()
        {
            var specs = new Dictionary<string, IPluginDataFileMergeInfo>();

            specs[GetFileName(DataFileId.Translations)] = new PluginDataFileMergeInfo(
                new MergeLevel("/ArrayOfTranslation", "concat(@ref,'/',OriginalPhrase)"));

            specs[GetFileName(DataFileId.QuestionCustomizations)] = new PluginDataFileMergeInfo(
                new MergeLevel("/ArrayOfPhraseCustomization", "concat(@ref,'/',@type,'/',OriginalPhrase)"));

            specs[GetFileName(DataFileId.PhraseSubstitutions)] = new PluginDataFileMergeInfo(
                new MergeLevel("/ArrayOfSubstitution", "@pattern"));

            specs[GetFileName(DataFileId.KeyTermRenderingInfo)] = new PluginDataFileMergeInfo(
                new MergeLevel("/ArrayOfKeyTermRenderingInfo", "@id"),
                new MergeLevel("AdditionalRenderings", "."));

            specs[GetFileName(DataFileId.TermRenderingSelectionRules)] = new PluginDataFileMergeInfo(
                new MergeLevel("/ArrayOfRenderingSelectionRule", "@questionMatcher"));

            return specs;
        }

        protected override void Write(DataFileId fileId, string data)
        {
            m_putPlugInData(GetFileName(fileId), data);
        }

		protected override void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data)
		{
			m_putPlugInData(GetBookSpecificFileName(fileId, bookId), data);
		}

	    public override string Read(DataFileId fileId)
        {
            return m_getPlugInData(GetFileName(fileId)) ?? string.Empty;
        }

        public override bool Exists(DataFileId fileId)
        {
            return m_getPlugInDataModifiedTime(GetFileName(fileId)).Ticks > 0;
        }

		public override bool BookSpecificDataExists(BookSpecificDataFileId fileId, string bookId)
		{
			return m_getPlugInDataModifiedTime(GetBookSpecificFileName(fileId, bookId)).Ticks > 0;
		}

        public override DateTime ModifiedTime(DataFileId fileId)
        {
            return m_getPlugInDataModifiedTime(GetFileName(fileId));
        }
    }
}
