// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2024' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	/// <summary>
	/// Represents a phrase/question that the user modified (even if that modification was
	/// later superseded by a subsequent program change that added the modified version).
	/// </summary>
	/// <remarks>Technically, we wouldn't need this interface since the only class that
	/// implements it is in this same assembly, but it wouldn't have to be implemented like
	/// that, and using this interface clarifies the properties we actually care about.</remarks>
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
