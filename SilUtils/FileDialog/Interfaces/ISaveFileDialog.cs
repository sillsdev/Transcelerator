// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
// --------------------------------------------------------------------------------------------
using System.IO;

namespace SIL.Utils.FileDialog
{
	public interface ISaveFileDialog: IFileDialog
	{
		bool CreatePrompt { get; set; }
		bool OverwritePrompt { get; set; }

		Stream OpenFile();
	}
}
