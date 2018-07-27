// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ITranslatablePhrase.cs
// ---------------------------------------------------------------------------------------------

namespace SIL.Transcelerator
{
	public interface ITranslatablePhrase
	{
		QuestionKey PhraseKey { get; }
		string Translation { get; }
	}
}