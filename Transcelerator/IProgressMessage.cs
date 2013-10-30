// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: IProgressMessage.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Interface to allow a process to display messages showing progress.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface IProgressMessage
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the message to display.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		string Message { set; }
	}
}
