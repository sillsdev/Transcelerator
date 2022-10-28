// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.   
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using L10NSharp;
using SIL.Windows.Forms.LocalizationIncompleteDlg;
using SIL.WritingSystems;

namespace SIL.Transcelerator
{
	internal class TxlLocalizationIncompleteViewModel : LocalizationIncompleteViewModel
	{
		public TxlLocalizationIncompleteViewModel(ILocalizationManager appLm,
			string crowdinProjectName, Action issueRequestForLocalization) :
			base(appLm, crowdinProjectName, issueRequestForLocalization)
		{
		}

		public override bool ShouldShowDialog(string languageId)
		{
			if (languageId.StartsWith("en-"))
				languageId = IetfLanguageTag.GetGeneralCode(languageId);
			return base.ShouldShowDialog(languageId);
		}
	}
}
