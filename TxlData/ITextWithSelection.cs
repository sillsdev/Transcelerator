// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2025' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	public interface ITextWithSelection
	{
		string Text { get; set; }
		int SelectionStart { get; set; }
		int SelectionLength { get; set; }
		string SelectedText { get; }
	}
}