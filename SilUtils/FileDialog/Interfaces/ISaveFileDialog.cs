// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
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
