// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2013' company='SIL International'>
// 	Copyright (c) 2013, SIL International.
//
// 	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
// --------------------------------------------------------------------------------------------
#if __MonoCS__
using System;
using System.IO;
using Gtk;

namespace SIL.Utils.FileDialog.Linux
{
	internal class OpenFileDialogLinux: FileDialogLinux, IOpenFileDialog
	{
		public OpenFileDialogLinux()
		{
			Action = FileChooserAction.Open;
			LocalReset();
		}

		#region IOpenFileDialog implementation
		public Stream OpenFile()
		{
			return new FileStream(FileName, FileMode.Open);
		}
		#endregion

		protected override void ReportFileNotFound(string fileName)
		{
			ShowMessageBox(FileDialogStrings.FileNotFoundOpen, ButtonsType.Ok, MessageType.Warning,
				fileName);
		}

		private void LocalReset()
		{
			Title = FileDialogStrings.TitleOpen;
		}

		public override void Reset()
		{
			base.Reset();
			LocalReset();
		}
	}
}
#endif
