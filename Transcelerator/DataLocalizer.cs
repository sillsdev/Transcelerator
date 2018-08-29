using System;
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

		public string GetLocalizedDataString(IQuestionKey key, LocalizableStringType type, string english, out string localeID)
		{
			return GetLocalizedDataString(new UIDataString(key, type, english), out localeID);
		}

		public string GetLocalizedDataString(UIDataString key, out string localeID)
		{
			localeID = m_dataLocFileAccessor.TryGetLocalizedString(key, out string localized) ? Locale : "en";
			return localized;
		}

		public string GetLocalizedDataString(TranslatablePhrase tp)
		{
			var key = tp.IsCategoryName ? new UIDataString(tp.PhraseInUse, LocalizableStringType.Category) :
				new UIDataString(tp.PhraseKey, LocalizableStringType.Question);
			return m_dataLocFileAccessor.GetLocalizedString(key);
		}
	}
}