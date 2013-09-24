// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International.
//
// 	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
// --------------------------------------------------------------------------------------------
using System.IO;

namespace SIL.Utils.FileDialog
{
	/// <summary>
	/// Interface to the OpenFileDialog
	/// </summary>
	public interface IOpenFileDialog: IFileDialog
	{
		bool Multiselect { get; set; }

		Stream OpenFile();
	}
}
