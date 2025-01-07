// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2013' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
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
using SIL.WritingSystems;
using static System.String;

namespace SIL.Transcelerator.Localization
{
	public class LocalizationsFileAccessor : IDataLocalizer
	{
		private Localizations m_xliffRoot;
		protected Localizations XliffRoot => m_xliffRoot;
		protected FileBody Localizations { get; private set; } // Exposed for sake of subclass used in unit tests
		private string DirectoryName { get; }
		public string Locale { get; }
		public string FileName { get; }
		private const string kBaseFilename = "LocalizedPhrases";
		private const string kLocaleFilenamePrefix = kBaseFilename + "-";
		private const string kLocaleFilenameExtension = ".xlf";
		private Dictionary<UIDataString, Tuple<string, bool>> m_fastLookup;

		private XmlSerializer Serializer => new XmlSerializer(typeof(Localizations),
			"urn:oasis:names:tc:xliff:document:1.2");

		public static bool AreEquivalentLocales(string fullLocale, string shortLocale)
		{
			if (fullLocale == shortLocale)
				return true;

			switch (shortLocale)
			{
				case "es":
					return fullLocale == "es-ES";
				default:
					return false;
			}
		}

		public LocalizationsFileAccessor(string directory, string locale)
		{
			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException("Attempt to initialize LocalizationsFileGenerator with non-existent directory failed.");
			DirectoryName = directory;
			Locale = locale;
			FileName = locale == "en" ?
				Path.Combine(DirectoryName, $"{kBaseFilename}{kLocaleFilenameExtension}") :
				Path.Combine(DirectoryName, $"{kLocaleFilenamePrefix}{Locale}{kLocaleFilenameExtension}");

			if (!Exists)
			{
				var langCode = IetfLanguageTag.GetGeneralCode(locale);
				if (langCode != locale)
				{
					Locale = langCode;
					FileName = Path.Combine(DirectoryName,
						$"{kLocaleFilenamePrefix}{Locale}{kLocaleFilenameExtension}");
				}
			}

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
				// Crowdin now supports Language Mapping but used to insist on using "es-ES"
				// (Spanish as spoken in Spain), rather than supporting generic Spanish.
				if (IsNullOrWhiteSpace(m_xliffRoot.File.TargetLanguage) || AreEquivalentLocales(m_xliffRoot.File.TargetLanguage, locale))
					m_xliffRoot.File.TargetLanguage = locale;
				else
					throw new DataException($"The target language ({m_xliffRoot.File.TargetLanguage}) specified in the data does not match the locale indicated by the file name: {FileName}");
				Localizations = m_xliffRoot.File.Body;
				InitializeLookupTable();
			}
			else
			{
				if (locale == "en")
				{
					throw new FileNotFoundException("I don't think you want to regenerate the English xml file from scratch. You will lose the magic numbers assigned by Crowdin. If you need this file, re-get it from Crowdin.",
						FileName);
				}

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
				return dirInfo.GetFiles($"{kLocaleFilenamePrefix}*{kLocaleFilenameExtension}")
					.Select(fi => fi.Name)
					.Select(n => n.Substring(kLocaleFilenamePrefix.Length))
					.Select(l => l.Substring(0, l.Length - kLocaleFilenameExtension.Length));
			}
			catch (Exception)
			{
				// Bad path or something
				return Array.Empty<string>();
			}
		}

		public FileInfo FileInfo => new FileInfo(FileName);
		public bool Exists => FileInfo.Exists;

		public string GetLocalizedString(UIDataString key, bool failOverToEnglish = true)
		{
			return TryGetLocalizedString(key, out string localized) ? localized : (failOverToEnglish ? key.SourceUIString : null);
		}

		public LocalizedDataString GetLocalizedDataString(UIDataString key)
		{
			var localeID = TryGetLocalizedString(key, out string localized) ? Locale : "en";
			return new LocalizedDataString(localized, localeID);
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