// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: IModifiedPhrase.cs.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	public interface IModifiedPhrase
	{
		string Reference { get; }
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets the original phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		string OriginalPhrase { get; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets the edited/customized phrase.
		/// </summary>
		/// --------------------------------------------------------------------------------
		string ModifiedPhrase { get; }
	}
}
