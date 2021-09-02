// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2013' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: DataFileAccessor.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using L10NSharp;
using Paratext.PluginInterfaces;
using SIL.Extensions;
using SIL.Reporting;
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

    public class ParatextDataFileAccessor : DataFileAccessor, IPluginObject
    {
		private readonly IProject m_project;
		private readonly Dictionary<string, IWriteLock> m_locks = new Dictionary<string, IWriteLock>();
		public bool IsReadonly
		{
			get 
			{
				lock(m_locks)
					return m_locks.Values.Any(l => l == null);
			}
		}

		public ParatextDataFileAccessor(IProject project)
        {
            m_project = project;
        }

		public static XMLDataMergeInfo GetXMLDataMergeInfo(string pluginDataId)
        {
			if (pluginDataId == GetFileName(DataFileId.Translations))
				return new XMLDataMergeInfo(false,
				new XMLListKeyDefinition("/ArrayOfTranslation", "concat(@ref,'/',OriginalPhrase)"));

			if (pluginDataId == GetFileName(DataFileId.QuestionCustomizations))
				return new XMLDataMergeInfo(false,
                new XMLListKeyDefinition("/ArrayOfPhraseCustomization", "concat(@ref,'/',@type,'/',OriginalPhrase)"));

			if (pluginDataId == GetFileName(DataFileId.PhraseSubstitutions))
				return new XMLDataMergeInfo(true,
                new XMLListKeyDefinition("/ArrayOfSubstitution", "@pattern"));

			if (pluginDataId == GetFileName(DataFileId.KeyTermRenderingInfo))
				return new XMLDataMergeInfo(false,
                new XMLListKeyDefinition("/ArrayOfKeyTermRenderingInfo", "@id"),
                new XMLListKeyDefinition("AdditionalRenderings", "."));

			if (pluginDataId == GetFileName(DataFileId.TermRenderingSelectionRules))
				return new XMLDataMergeInfo(true,
                new XMLListKeyDefinition("/ArrayOfRenderingSelectionRule", "@questionMatcher"));

			throw new NotImplementedException("Caller requested merge info for unexpected type of data.");
		}

        protected override void Write(DataFileId fileId, string data)
        {
			lock (m_locks)
			{
				var fileName = GetFileName(fileId);
				m_project.PutPluginData(EnsureLock(fileName, true), this, fileName,
					writer => { writer.Write(data); });
			}
		}

		protected override void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data)
		{
			lock (m_locks)
			{
				var fileName = GetBookSpecificFileName(fileId, bookId);
				m_project.PutPluginData(EnsureLock(fileName, true), this, fileName,
					writer => { writer.Write(data); });
			}
		}

	    public override string Read(DataFileId fileId)
		{
			var fileName = GetFileName(fileId);
			EnsureLock(fileName, false); // Lock is not needed for reading, but this way we'll find out of something else tries to access a file we've already read.

			using (var reader = m_project.GetPluginData(this, fileName))
				return reader?.ReadToEnd();
		}

		private IWriteLock EnsureLock(string fileName, bool required)
		{
			lock (m_locks)
			{
				if (!m_locks.TryGetValue(fileName, out var lockForFile) || lockForFile == null)
				{
					lockForFile = m_project.RequestWriteLock(this, ReleaseRequested, fileName);
					if (lockForFile == null && required)
					{
						ErrorReport.NotifyUserOfProblem(LocalizationManager.GetString("General.RequestLockError",
							"Unable to obtain exclusive access to Txl"));
					}

					m_locks[fileName] = lockForFile;
				}

				return lockForFile;
			}
		}

		private void ReleaseRequested(IWriteLock lockToRelease)
		{
			lock (m_locks)
				m_locks.RemoveAll(kvp => kvp.Value == lockToRelease);
		}

		public override bool Exists(DataFileId fileId) =>
			Exists(m_project.GetPluginData(this, GetFileName(fileId)));

		public override bool BookSpecificDataExists(BookSpecificDataFileId fileId, string bookId) => 
			Exists(m_project.GetPluginData(this, GetBookSpecificFileName(fileId, bookId)));

		private bool Exists(TextReader reader)
		{
			if (reader == null)
				return false;
			reader.Dispose();
			return true;
		}

        public override DateTime ModifiedTime(DataFileId fileId)
		{
			return m_project.GetPluginDataModifiedTime(this, GetFileName(fileId));
        }
    }
}
