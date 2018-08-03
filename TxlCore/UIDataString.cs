// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2018, SIL International.
// <copyright from='2018' to='2018' company='SIL International'>
//		Copyright (c) 2018, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: UIDataString.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Transcelerator
{
	public class UIDataString : IQuestionKey
	{
		// ENHANCE: Consider changing this to a private constructor and adding a CreateKey that returns
		// and IQuestionKey and only constructs a new key if needed.
		public UIDataString(IQuestionKey baseQuestionKey, string uiString, LocalizableStringType type)
		{
			ScriptureReference = baseQuestionKey.ScriptureReference;
			StartRef = baseQuestionKey.StartRef;
			EndRef = baseQuestionKey.EndRef;
			PhraseInUse = uiString;
			Text = type == LocalizableStringType.Question && !string.IsNullOrWhiteSpace(baseQuestionKey.Text) ?
				baseQuestionKey.Text : uiString;
		}

		public string PhraseInUse { get; }
		public string ScriptureReference { get; }
		public int StartRef { get; }
		public int EndRef { get; }
		public string Text { get; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return ScriptureReference + "-" + Text;
		}
	}
}