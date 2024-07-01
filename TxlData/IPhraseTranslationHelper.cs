// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2021' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: IPhraseTranslationHelper.cs
// ---------------------------------------------------------------------------------------------
using SIL.Transcelerator.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace SIL.Transcelerator
{
	public delegate void OnNumberFormattingChangedHandler();

	/// <summary>
	/// Helper that knows about other TranslatablePhrase objects
	/// and can manage the relationship between them as translations change.
	/// </summary>
	public interface IPhraseTranslationHelper
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Processes a new translation on a phrase.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void ProcessTranslation(TranslatablePhrase tp);

		void ProcessChangeInUserTranslationState();

		string InitialPunctuationForType(TypeOfPhrase type);

		string FinalPunctuationForType(TypeOfPhrase type);

		string GetCategoryName(int category, out string lang);

		List<RenderingSelectionRule> TermRenderingSelectionRules { get; }

		event OnNumberFormattingChangedHandler OnNumberFormattingChanged;

		void SetNumericFormat(char exampleDigit, string groupingPunctuation,
			IReadOnlyList<int> digitGroups, bool fNoGroupPunctForShortNumbers);

		NumberFormatInfo NumberFormatInfo { get; }

		// 3000 vs. 3,000
		bool NoGroupPunctForShortNumbers { get; }
	}
}