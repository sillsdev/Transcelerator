// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International.
// <copyright from='2023' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: ListSerializationHelperTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class ListSerializationHelperTests
	{
		[Test]
		public void LoadOrCreateListFromString_ClassWithSinglePublicMember_LoadsList()
		{
			var result = 
				ListSerializationHelper.LoadOrCreateListFromString<SimpleClass>(
					"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
					"<ArrayOfSimple>\r\n" +
					"<Simple name=\"Monkey\" />\r\n" +
					"<Simple name=\"Soup\" />\r\n" +
					"</ArrayOfSimple>\r\n", out var ex);
			Assert.IsNull(ex);
			Assert.That(result.Select(s => s.Name), Is.EquivalentTo(new[] { "Monkey", "Soup" }));
		}

		[Test]
		public void LoadOrCreateListFromString_BogusString_CreatesEmptyList()
		{
			var result = 
				ListSerializationHelper.LoadOrCreateListFromString<SimpleClass>("This is garbage", out var ex);
			Assert.IsNotNull(ex);
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void LoadOrCreateListFromString_ClassWithPublicMemberButPrivateSetter_LoadsList()
		{
			var result = 
				ListSerializationHelper.LoadOrCreateListFromString<ClassWithPropSansPublicSetter>(
					"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
					"<ArrayOfComplex>\r\n" +
					"<Complex name=\"Frog\" />\r\n" +
					"<Complex name=\"Casserole\" />\r\n" +
					"</ArrayOfComplex>\r\n", out var ex);
			Assert.IsNull(ex);
			Assert.That(result.Select(s => s.Name), Is.EquivalentTo(new[] { "Frog", "Casserole" }));
		}

		/// <summary>
		/// This is the real-life test case. Although it is not really needed to demonstrate that
		/// LoadOrCreateListFromString works correctly, it is a helpful test case to make sure that
		/// Substitution is serializable as a list.
		/// </summary>
		/// <remarks>Perhaps it would make more sense to put this in a SubstitutionTests class.</remarks>
		[Test]
		public void LoadOrCreateListFromString_ListOfSubstitution_LoadsList()
		{
			var result = 
				ListSerializationHelper.LoadOrCreateListFromString<Substitution>("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
					"<ArrayOfSubstitution>\r\n" +
					"<Substitution pattern=\"does it mean\" replacement=\"means\" regex=\"false\" case=\"false\" />\r\n" +
					"<Substitution pattern=\" does \" replacement=\" \" regex=\"false\" case=\"true\" />\r\n" +
					"<Substitution pattern=\"How (long|often) did (.*) (last|stay)\" " +
					"replacement=\"How $1 $3ed $2\" regex=\"true\" case=\"false\" />\r\n" +
					"</ArrayOfSubstitution>\r\n", out var ex);
			Assert.IsNull(ex);
			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result.Where(s => s.IsRegex).Count, Is.EqualTo(1));
			Assert.That(result.Where(s => s.MatchCase).Count, Is.EqualTo(1));
			Assert.That(result.Where(s => s.Replacement != " ").Count, Is.EqualTo(2));
		}
	}

	[XmlType("Simple")]
	public class SimpleClass
	{
		[XmlAttribute("name")]
		public string Name { get; set; }
	}

	[XmlType("Complex")]
	public class ClassWithPropSansPublicSetter
	{
		public ClassWithPropSansPublicSetter()
		{
			Id = Guid.NewGuid().ToString();
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// This method is not really needed. Just illustrates case for having a private setter.
		/// </summary>
		[PublicAPI]
		public void RefreshId()
		{
			Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// I would not have thought that the XMLIgnore attribute would be needed
		/// (nor is the need for it particularly desirable), but it is!
		/// </summary>
		[XmlIgnore]
		public string Id { get; private set; }
	}
}
