using System;
using System.Collections.Generic;
using System.IO;

namespace SILUBS.Transcelerator
{
    public abstract class DataFileProxy
    {
        public enum DataFileId
        {
            Translations,
            QuestionCustomizations,
            PhraseSubstitutions,
            KeyTermRenderingInfo,
            TermRenderingSelectionRules,
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
            }
            throw new ArgumentException("Bogus", "fileId");
        }

        public abstract void Write(DataFileId fileId, string data);

        public abstract string Read(DataFileId fileId);

        public abstract bool Exists(DataFileId fileId);
    }

    public class ParatextDataFileProxy : DataFileProxy
    {
        private readonly Func<string, TextReader> m_getPlugInData;
        private readonly Action<string, TextReader> m_putPlugInData;

        public ParatextDataFileProxy(Func<string, TextReader> getPlugInData,
            Action<string, TextReader> putPlugInData)
        {
            m_getPlugInData = getPlugInData;
            m_putPlugInData = putPlugInData;
        }

        public static Dictionary<string, List<KeyValuePair<string, string>>> GetDataFileKeySpecifications()
        {
            var specs = new Dictionary<string, List<KeyValuePair<string, string>>>();
            specs[GetFileName(DataFileId.Translations)] = new List<KeyValuePair<string, string>>(
                new[] { new KeyValuePair<string, string>("/ArrayOfTranslation", "concat(@ref,'/',OriginalPhrase)") });
            specs[GetFileName(DataFileId.QuestionCustomizations)] = new List<KeyValuePair<string, string>>(
                new[] { new KeyValuePair<string, string>("/ArrayOfPhraseCustomization", "concat(@ref,'/',@type,'/',OriginalPhrase)") });
            specs[GetFileName(DataFileId.PhraseSubstitutions)] = new List<KeyValuePair<string, string>>(
                new[] { new KeyValuePair<string, string>("/ArrayOfSubstitution", "@pattern") });
            specs[GetFileName(DataFileId.KeyTermRenderingInfo)] = new List<KeyValuePair<string, string>>(
                new[] { new KeyValuePair<string, string>("/ArrayOfKeyTermRenderingInfo", "@id"),
                new KeyValuePair<string, string>("AdditionalRenderings", ".")});
            specs[GetFileName(DataFileId.TermRenderingSelectionRules)] = new List<KeyValuePair<string, string>>(
                new[] { new KeyValuePair<string, string>("/ArrayOfRenderingSelectionRule", "@questionMatcher") });
            return specs;
        }

        public override void Write(DataFileId fileId, string data)
        {
            m_putPlugInData(GetFileName(fileId), new StringReader(data));
        }

        public override string Read(DataFileId fileId)
        {
            var reader = m_getPlugInData(GetFileName(fileId));
            return reader == null ? string.Empty : reader.ReadToEnd();
        }

        public override bool Exists(DataFileId fileId)
        {
            return m_getPlugInData(GetFileName(fileId)) != null;
        }
    }
}
