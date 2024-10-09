// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.   
// <copyright from='2022' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
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
			// If we have LocalizedQuestions for the language, even if none of the UI is localized,
			// we should not show the dialog.
			if (LocalesWithLocalizedQuestions.Contains(languageId))
				return false;
			if (languageId.StartsWith("en-"))
				languageId = IetfLanguageTag.GetGeneralCode(languageId);
			return base.ShouldShowDialog(languageId);
		}

		public List<string> LocalesWithLocalizedQuestions { get; set; }
	}
}
