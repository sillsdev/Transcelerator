// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2018' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ITranslatablePhrase.cs
// ---------------------------------------------------------------------------------------------

namespace SIL.Transcelerator
{
	public enum TypeOfPhrase : byte
	{
		Unknown,
		Question,
		StatementOrImperative,
		NoEnglishVersion,
	}

	public interface ITranslatablePhrase
	{
		IQuestionKey PhraseKey { get; }
		string Translation { get; }
		TypeOfPhrase TypeOfPhrase { get; }
	}
}