namespace SIL.Transcelerator.Localization
{
	public interface ILocalizationsProvider
	{
		string Locale { get; }
		bool TryGetLocalizedString(UIDataString key, out string localized);
	}
}