using System.Windows.Forms;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Simple class to allow methods to pass an offset and a length in order to describe a
	/// substring.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SubstringDescriptor
	{
		public int Start { get; set; }
		public int Length { get; set; }

		public int EndOffset => Start + Length;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(int start, int length)
		{
			Start = start;
			Length = length;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="SubstringDescriptor"/> class based on
		/// the existing text selection in a text box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public SubstringDescriptor(TextBox textBoxCtrl)
		{
			Start = textBoxCtrl.SelectionStart;
			Length = textBoxCtrl.SelectionLength;
		}
	}
}