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
	public class SaveFileDialogAdapter: FileDialogAdapter, ISaveFileDialog
	{
		public SaveFileDialogAdapter()
		{
			m_dlg = Manager.CreateSaveFileDialog();
		}

		public bool CreatePrompt
		{
			get { return ((ISaveFileDialog)m_dlg).CreatePrompt; }
			set { ((ISaveFileDialog)m_dlg).CreatePrompt = value; }
		}

		public bool OverwritePrompt
		{
			get { return ((ISaveFileDialog)m_dlg).OverwritePrompt; }
			set { ((ISaveFileDialog)m_dlg).OverwritePrompt = value; }
		}

		public Stream OpenFile()
		{
			return ((ISaveFileDialog)m_dlg).OpenFile();
		}
	}
}
