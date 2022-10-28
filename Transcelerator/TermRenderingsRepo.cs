// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: TermRenderingsRepo.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Paratext.PluginInterfaces;
using SIL.Utils;

namespace SIL.Transcelerator
{
	class TermRenderingsRepo : ITermRenderingsRepo
	{
		protected readonly List<KeyTermRenderingInfo> m_data;

		public TermRenderingsRepo(List<KeyTermRenderingInfo> data = null)
		{
			m_data = data ?? new List<KeyTermRenderingInfo>();
		}

		#region ITermRenderingsRepo implementation
		public KeyTermRenderingInfo GetRenderingInfo(string termId) =>
			m_data.FirstOrDefault(i => i.TermId == termId);

		public void Add(KeyTermRenderingInfo info) => m_data.Add(info);

		public virtual IEnumerable<string> GetTermRenderings(string termId)
		{
			yield break;
		}
		#endregion
	}

	class ParatextTermRenderingsRepo : TermRenderingsRepo
	{
		private readonly DataFileAccessor m_fileAccessor;
		private readonly IProject m_project;

		public ParatextTermRenderingsRepo(DataFileAccessor fileAccessor, IProject project) :
			base(ListSerializationHelper.LoadOrCreateListFromString<KeyTermRenderingInfo>(
				fileAccessor.Read(DataFileAccessor.DataFileId.KeyTermRenderingInfo), true))
		{
			m_fileAccessor = fileAccessor;
			m_project = project;
		}

		public void Save()
		{
			m_fileAccessor.Write(DataFileAccessor.DataFileId.KeyTermRenderingInfo, m_data);
		}

		#region ITermRenderingsRepo overrides
		public override IEnumerable<string> GetTermRenderings(string termId)
		{
			return m_project.BiblicalTermList.Where(term => term.Lemma == termId)
				.SelectMany(term => m_project.GetBiblicalTermRenderings(term, true).Renderings);
		}
		#endregion
	}
}
