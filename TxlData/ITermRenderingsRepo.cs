// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ITermRenderingsRepo.cs
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SIL.Transcelerator
{
	public interface ITermRenderingsRepo
	{
		KeyTermRenderingInfo GetRenderingInfo(string termId);

		void Add(KeyTermRenderingInfo info);

		IEnumerable<string> GetTermRenderings(string termId);
	}
}
