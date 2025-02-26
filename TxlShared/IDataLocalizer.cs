// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2024' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion

namespace SIL.Transcelerator.Localization
{
	public interface IDataLocalizer : ILocalizationsProvider
	{
		LocalizedDataString GetLocalizedDataString(UIDataString key);
	}
}
