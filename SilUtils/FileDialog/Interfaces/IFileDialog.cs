// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SIL.Utils.FileDialog
{
	/// <summary>
	/// Interface to the FileDialog
	/// </summary>
	public interface IFileDialog
	{
		bool AddExtension { get; set; }
		bool CheckFileExists { get; set; }
		bool CheckPathExists { get; set; }
		string DefaultExt { get; set; }
		string FileName { get; set; }
		string[] FileNames { get; }
		string Filter { get; set; }
		int FilterIndex { get; set; }
		string InitialDirectory { get; set; }
		bool RestoreDirectory { get; set; }
		bool ShowHelp { get; set; }
		bool SupportMultiDottedExtensions { get; set; }
		string Title { get; set; }
		bool ValidateNames { get; set; }

		void Reset();
		DialogResult ShowDialog();
		DialogResult ShowDialog(IWin32Window owner);

		event EventHandler Disposed;
		event CancelEventHandler FileOk;
		event EventHandler HelpRequest;
	}
}
