using System;
using System.Collections.Generic;
using AddInSideViews;

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

		public abstract void Write(DataFileId fileId, string data);

		public abstract void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data);

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

        public override void Write(DataFileId fileId, string data)
        {
            m_putPlugInData(GetFileName(fileId), data);
        }

	    public override void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data)
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

        public override DateTime ModifiedTime(DataFileId fileId)
        {
            return m_getPlugInDataModifiedTime(GetFileName(fileId));
        }
    }
}
