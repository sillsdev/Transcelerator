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
using System.Text.RegularExpressions;
using System.Threading;
using L10NSharp;
using Paratext.PluginInterfaces;
using SIL.Reporting;
using SIL.Xml;
using DateTime = System.DateTime;
using Task = System.Threading.Tasks.Task;

namespace SIL.Transcelerator
{
    public abstract class DataFileAccessor
	{
		protected const string kScrForgeTranslationsFilenamePrefix = "Translated Checking Questions for ";
		protected const string kScrForgeTranslationsExt = ".xml";
		protected static Regex s_regexScrForgeTranslationsFile = new Regex("^" +
			kScrForgeTranslationsFilenamePrefix + @"[1-3A-Z][A-Z]{2}\" +
			kScrForgeTranslationsExt + "$", RegexOptions.Compiled);
		private static Dictionary<DataFileId, string> s_dataFileIdMap;

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

		static DataFileAccessor()
		{
			s_dataFileIdMap = new Dictionary<DataFileId, string>(5);
			s_dataFileIdMap[DataFileId.Translations] = "Translations of Checking Questions.xml";
			s_dataFileIdMap[DataFileId.QuestionCustomizations] = "Question Customizations.xml";
			s_dataFileIdMap[DataFileId.PhraseSubstitutions] = "Phrase substitutions.xml";
			s_dataFileIdMap[DataFileId.KeyTermRenderingInfo] = "Key term rendering info.xml";
			s_dataFileIdMap[DataFileId.TermRenderingSelectionRules] = "Term rendering selection rules.xml";
		}

		protected static string GetFileName(DataFileId fileId)
        {
			return s_dataFileIdMap.TryGetValue(fileId, out var filename) ?
				filename : throw new ArgumentException("Bogus", nameof(fileId));
		}

		protected static DataFileId GetFileId(string fileName)
		{
			foreach (var pair in s_dataFileIdMap)
				if (fileName.Equals(pair.Value)) return pair.Key;

			throw new ArgumentException("Unexpected Filename", nameof(fileName));
		}

		protected static string GetBookSpecificFileName(BookSpecificDataFileId fileId, string bookId)
		{
			switch (fileId)
			{
				case BookSpecificDataFileId.ScriptureForge: return $"{kScrForgeTranslationsFilenamePrefix}{bookId}{kScrForgeTranslationsExt}";
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
		private readonly Func<DataFileId, Task> m_writeLockReleaseRequestHandler;
		// Dictionary of file names to their corresponding write locks
		private readonly Dictionary<string, IWriteLock> m_locks = new Dictionary<string, IWriteLock>();
		private readonly List<string> m_locksPendingRemoval = new List<string>();

		public bool IsReadonly
		{
			get 
			{
				lock(m_locks)
					return m_locks.Values.Any(l => l == null);
			}
		}

		public ParatextDataFileAccessor(IProject project, Func<DataFileId, Task> writeLockReleaseRequestHandler)
		{
			m_project = project;
			m_writeLockReleaseRequestHandler = writeLockReleaseRequestHandler;
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

			if (s_regexScrForgeTranslationsFile.IsMatch(pluginDataId))
				return new XMLDataMergeInfo(true,
					new XMLListKeyDefinition("/ComprehensionCheckingQuestionsForBook", "concat(@id,'/',@startChapter,'/',@startVerse)"));

			throw new ArgumentException($"Caller requested merge info for unexpected type of data: {pluginDataId}.", nameof(pluginDataId));
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

		public void EnsureLockForTranslationData()
		{
			EnsureLock(GetFileName(DataFileId.Translations), true);
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
							"Unable to obtain exclusive write access to data that belongs to Transcelerator: {0}",
							"Param 0: key that indicates the data file that was to be written"), fileName);
					}

					m_locks[fileName] = lockForFile;
				}

				return lockForFile;
			}
		}

		private void ReleaseRequested(IWriteLock lockToRelease)
		{
			bool pending = false;
			lock (m_locks)
			{
				var toRemove = m_locks.Where(kvp => kvp.Value == lockToRelease).Select(kvp => kvp.Key).ToList();
				foreach (var filename in toRemove)
				{
					if (m_writeLockReleaseRequestHandler == null)
						m_locks.Remove(filename);
					else
					{
						var task = filename.StartsWith(kScrForgeTranslationsFilenamePrefix) ? null :
							m_writeLockReleaseRequestHandler.Invoke(GetFileId(filename));
						if (task == null)
							m_locks.Remove(filename);
						else
						{
							m_locksPendingRemoval.Add(filename);
							pending = true;
							task.ContinueWith(t =>
							{
								lock (m_locks)
								{
									m_locksPendingRemoval.Remove(filename);
									m_locks.Remove(filename);
								}
							});
							task.Start();
						}
					}
				}
			}

			while (pending)
			{
				Thread.Sleep(100);
				lock (m_locks)
					pending = m_locksPendingRemoval.Any();
			}
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
