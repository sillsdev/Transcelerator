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
	/// <summary>Cross-platform OpenFile dialog. On Windows it displays .NET's WinForms
	/// OpenFileDialog, on Linux the GTK FileChooserDialog.</summary>
	public class OpenFileDialogAdapter: FileDialogAdapter, IOpenFileDialog
	{
		public OpenFileDialogAdapter()
		{
			m_dlg = Manager.CreateOpenFileDialog();
		}

		#region IOpenFileDialog implementation
		public Stream OpenFile()
		{
			return ((IOpenFileDialog)m_dlg).OpenFile();
		}

		public bool Multiselect
		{
			get { return ((IOpenFileDialog)m_dlg).Multiselect; }
			set { ((IOpenFileDialog)m_dlg).Multiselect = value; }
		}
		#endregion
	}
}
