// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2013' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: LocalizationsFileAccessor.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SIL.Transcelerator.Localization
{
	public class LocalizationsFileAccessor
	{
		private Localizations m_xliffRoot;
		protected Localizations XliffRoot => m_xliffRoot;
		protected FileBody Localizations { get; private set; } // Exposed for sake of subclass used in unit tests
		private string DirectoryName { get; }
		public string Locale { get; }
		private const string klocaleFilenamePrefix = "LocalizedPhrases-";
		private const string kLocaleFilenameExtension = ".xlf";
		private Dictionary<UIDataString, Tuple<string, bool>> m_fastLookup;

		private XmlSerializer Serializer => new XmlSerializer(typeof(Localizations),
			"urn:oasis:names:tc:xliff:document:1.2");

		public LocalizationsFileAccessor(string directory, string locale)
		{
			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException("Attempt to initialize LocalizationsFileGenerator with non-existent directory failed.");
			DirectoryName = directory;
			Locale = locale;
			if (Exists)
			{
				try
				{
					using (var reader = new XmlTextReader(new StreamReader(FileName)))
						m_xliffRoot = (Localizations)Serializer.Deserialize(reader);
				}
				catch (Exception ex)
				{
					throw new DataException($"File {FileName} could not be deserialized.", ex);
				}

				if (!m_xliffRoot.IsValid(out string error))
					throw new DataException(error);
				// Crowdin insists on using "es-ES" (Spanish as spoke in Spain), rather than supporting generic Spanish.
				if (String.IsNullOrWhiteSpace(m_xliffRoot.File.TargetLanguage) || (m_xliffRoot.File.TargetLanguage == "es-ES" && locale == "es"))
					m_xliffRoot.File.TargetLanguage = locale;
				else if (m_xliffRoot.File.TargetLanguage != locale)
					throw new DataException($"The target language ({m_xliffRoot.File.TargetLanguage}) specified in the data does not match the locale indicated by the file name: {FileName}");
				Localizations = m_xliffRoot.File.Body;
				InitializeLookupTable();
			}
			else
			{
				InitializeLocalizations();
			}
		}

		protected virtual void InitializeLookupTable()
		{
			// ENHANCE: For better speed/memory conservation, set capacity
			m_fastLookup = new Dictionary<UIDataString, Tuple<string, bool>>();
		}

		// For testing only
		protected internal LocalizationsFileAccessor()
		{
			InitializeLocalizations();
			InitializeLookupTable();
		}

		protected void InitializeLocalizations()
		{
			m_xliffRoot = new Localizations();
			m_xliffRoot.Initialize();
			Localizations = m_xliffRoot.File.Body;
		}

		public static IEnumerable<string> GetAvailableLocales(string directory)
		{
			try
			{
				DirectoryInfo dirInfo = new DirectoryInfo(directory);
				return dirInfo.GetFiles($"{klocaleFilenamePrefix}*{kLocaleFilenameExtension}")
					.Select(fi => fi.Name)
					.Select(n => n.Substring(klocaleFilenamePrefix.Length))
					.Select(l => l.Substring(0, l.Length - kLocaleFilenameExtension.Length));
			}
			catch (Exception)
			{
				// Bad path or something
				return new string[0];
			}
		}

		public string FileName => Path.Combine(DirectoryName, $"{klocaleFilenamePrefix}{Locale}{kLocaleFilenameExtension}");
		public FileInfo FileInfo => new FileInfo(FileName);
		public bool Exists => FileInfo.Exists;

		public string GetLocalizedString(UIDataString key, bool failoverToEnglish = true)
		{
			return TryGetLocalizedString(key, out string localized) ? localized : (failoverToEnglish ? key.SourceUIString : null);
		}

		public string GetLocalizedDataString(UIDataString key, out string localeID)
		{
			localeID = TryGetLocalizedString(key, out string localized) ? Locale : "en";
			return localized;
		}

		public virtual bool TryGetLocalizedString(UIDataString key, out string localized)
		{
			if (m_fastLookup.TryGetValue(key, out Tuple<string, bool> value))
			{
				localized = value.Item1;
				return value.Item2;
			}
			var info = Localizations.GetStringLocalization(key);
			if (info != null && info.Target.IsLocalized)
			{
				localized = info.Target.Text;
				m_fastLookup.Add(key, new Tuple<string, bool>(localized, true));
				return true;
			}
			localized = key.SourceUIString;
			m_fastLookup.Add(key, new Tuple<string, bool>(localized, false));
			return false;
		}
	}
}