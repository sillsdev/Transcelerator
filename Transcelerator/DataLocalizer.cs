using System;
using System.Diagnostics;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class DataLocalizer
	{
		private readonly LocalizationsFileAccessor m_dataLocFileAccessor;

		public string Locale => m_dataLocFileAccessor?.Locale;

		public DataLocalizer(LocalizationsFileAccessor localizationsFileAccessor)
		{
			m_dataLocFileAccessor = localizationsFileAccessor ?? throw new ArgumentNullException(nameof(localizationsFileAccessor));
		}

		public string GetLocalizedCommentOrAnswerString(IQuestionKey key, LocalizableStringType type, string english)
		{
			Debug.Assert(type == LocalizableStringType.Answer || type == LocalizableStringType.Note);
			return GetLocalizedDataString(new UIDataString(key, type, english), out string notUsed);
		}

		public string GetLocalizedDataString(UIDataString key, out string localeID)
		{
			localeID = m_dataLocFileAccessor.TryGetLocalizedString(key, out string localized) ? Locale : "en";
			return localized;
		}

		public string GetLocalizedDataString(TranslatablePhrase tp)
		{
			return  m_dataLocFileAccessor.GetLocalizedString(tp.ToUIDataString());
		}
	}
}