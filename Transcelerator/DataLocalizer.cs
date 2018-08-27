using System;
using SIL.Transcelerator.Localization;

namespace SIL.Transcelerator
{
	public class DataLocalizer
	{
		private readonly LocalizationsFileAccessor m_dataLocalizer;

		public string Locale => m_dataLocalizer?.Locale;

		public DataLocalizer(LocalizationsFileAccessor localizationsFileAccessor)
		{
			m_dataLocalizer = localizationsFileAccessor ?? throw new ArgumentNullException(nameof(localizationsFileAccessor));
		}

		public string GetLocalizedDataString(IQuestionKey key, LocalizableStringType type, string english, out string localeID)
		{
			localeID = m_dataLocalizer.TryGetLocalizedString(new UIDataString(key, english, type), out string localized) ? Locale : "en";
			return localized;
		}

		public string GetLocalizedDataString(TranslatablePhrase tp)
		{
			return GetLocalizedDataString(tp.PhraseKey,
				tp.IsCategoryName ? LocalizableStringType.Category : LocalizableStringType.Question,
				tp.PhraseInUse, out string notUsed);
		}
	}
}