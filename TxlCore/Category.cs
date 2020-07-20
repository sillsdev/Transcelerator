// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: Category.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SIL.Transcelerator
{
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class Category : ICloneable
	{
		[XmlAttribute("overview")]
		public bool IsOverview { get; set; }

		[XmlAttribute("type")]
		public string Type { get; set; }

		private List<Question> m_questions;
		[XmlElement]
		public List<Question> Questions => m_questions ?? (m_questions = new List<Question>());

		public Category Clone() => (Category)((ICloneable)this).Clone();

		object ICloneable.Clone()
		{
			var clone = (Category)MemberwiseClone();
			if (m_questions != null)
			{
				clone.m_questions = new List<Question>(m_questions.Count);
				foreach (var q in m_questions)
					clone.m_questions.Add(q.Clone());
			}

			return clone;
		}
	}

	public class CategoryComparer : IEqualityComparer<Category>
	{
		public static IEqualityComparer<Category> AreSame = new CategoryComparer();

		public bool Equals(Category x, Category y) =>
			x.IsOverview.Equals(y.IsOverview) && x.Type.Equals(y.Type);

		public int GetHashCode(Category obj)
		{
			unchecked
			{
				var hashCode = obj.IsOverview.GetHashCode();
				hashCode = (hashCode * 397) ^ (obj.Type != null ? obj.Type.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}