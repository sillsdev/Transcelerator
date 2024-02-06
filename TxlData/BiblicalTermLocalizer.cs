// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.   
// <copyright from='2021' to='2023 company='SIL International'>
//		Copyright (c) 2023, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: BiblicalTermLocalizer.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Paratext.PluginInterfaces;

namespace SIL.Transcelerator
{
	public class BiblicalTermLocalizer
	{
		private readonly Dictionary<KeyTerm, string> m_dict = new Dictionary<KeyTerm, string>();
		private readonly string m_locale;
		private readonly IBiblicalTermList m_termsList;

		public BiblicalTermLocalizer(string locale, IBiblicalTermList biblicalTermList)
		{
			m_locale = locale;
			m_termsList = biblicalTermList;
		}

		public string GetTermHeading(KeyTerm keyTerm)
		{
			if (!m_dict.TryGetValue(keyTerm, out var heading))
			{
				var englishHeading = keyTerm.ToString();
				heading = m_termsList.FirstOrDefault(t => keyTerm.AllTermIds.Contains(t.Lemma) &&
						t.Gloss().Equals(englishHeading, StringComparison.OrdinalIgnoreCase))
					?.Gloss(m_locale);
				m_dict[keyTerm] = heading;
			}

			return heading;
		}
	}
}