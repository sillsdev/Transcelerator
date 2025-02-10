// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global.
// <copyright from='2024' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
using System.Text;

namespace SIL.Transcelerator.Localization
{
	public class LocalizedDataString
	{
		private readonly bool m_needsNormalization;
		public string Data { get; }
		public string Normalized =>
			m_needsNormalization ? Data.Normalize(NormalizationForm.FormC) : Data;
		public string Lang { get; }
		public bool Omit => Data == "*Omit*";

		public LocalizedDataString(string data, string lang, bool needsNormalization = true)
		{
			m_needsNormalization = needsNormalization;
			Data = data;
			Lang = lang;
		}

		public override string ToString() => Data;

		public static implicit operator string(LocalizedDataString localizedDataString) =>
			localizedDataString?.ToString();
	}
}
