// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021 to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: IHtmlScriptureExtractor.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	public interface IHtmlScriptureExtractor
	{
		bool IncludeVerseNumbers { get; set; }

		string GetAsHtmlFragment(int startRef, int endRef);
	}
}