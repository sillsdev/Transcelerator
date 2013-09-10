// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
// --------------------------------------------------------------------------------------------
#if !__MonoCS__
using System.IO;
using System.Windows.Forms;

namespace SIL.Utils.FileDialog.Windows
{
	internal class OpenFileDialogWindows: FileDialogWindows, IOpenFileDialog
	{
		public OpenFileDialogWindows()
		{
			m_dlg = new OpenFileDialog();
		}

		#region IOpenFileDialog implementation
		public Stream OpenFile()
		{
			return ((OpenFileDialog)m_dlg).OpenFile();
		}

		public bool Multiselect
		{
			get { return ((OpenFileDialog)m_dlg).Multiselect; }
			set { ((OpenFileDialog)m_dlg).Multiselect = value; }
		}
		#endregion
	}
}
#endif
