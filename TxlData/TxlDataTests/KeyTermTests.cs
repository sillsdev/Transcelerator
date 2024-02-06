// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2013' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermTests.cs
// ---------------------------------------------------------------------------------------------
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Paratext.PluginInterfaces;
using Rhino.Mocks;
using Is = NUnit.Framework.Is;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class KeyTermTests
	{
		private ITermRenderingsRepo m_repo;

        [SetUp]
        public void TestSetup()
		{
			m_repo = MockRepository.GenerateStrictMock<ITermRenderingsRepo>();
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the ability to get the "externally supplied" renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetRenderings_SurrogateIsForSingleBiblicalTermWithTwoRenderings_ReturnsBothRenderings()
        {
            string[] renderings = { "abc", "xyz" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord")).Return(renderings);
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(null);
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"), m_repo);
            Assert.IsTrue(kt.Renderings.SequenceEqual(renderings));
            Assert.AreEqual("abc", kt.BestRendering);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the ability to get the "externally supplied" renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetRenderings_SurrogateIsForThreeBiblicalTermsOneOfWhichHasNoRenderings_ReturnsCombinedRenderings()
        {
            string[] renderings1 = { "abc", "xyz" };
            string[] renderings2 = { "xyz", "def", "qrs" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord1")).Return(renderings1);
			m_repo.Stub(r => r.GetTermRenderings("GreekWord2")).Return(new string[0]);
			m_repo.Stub(r => r.GetTermRenderings("HebrewWord")).Return(renderings2);
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(null);
			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord1",
				"GreekWord2", "HebrewWord"), m_repo);
            Assert.IsTrue(kt.Renderings.SequenceEqual(new [] { "abc", "xyz", "def", "qrs" }));
            Assert.AreEqual("xyz", kt.BestRendering);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the ability to add and remove additional renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void AddAndRemoveRenderings_NoExistingInfo()
		{
			MockRenderingRepoForGreekWordWithNoExistingInfo();
			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"), m_repo);
			kt.AddRendering("wunkyboo");
			Assert.AreEqual(3, kt.Renderings.Count());
			Assert.IsTrue(kt.Renderings.Contains("wunkyboo"));
            Assert.IsTrue(kt.CanRenderingBeDeleted("wunkyboo"));
            Assert.IsFalse(kt.CanRenderingBeDeleted("abc"));
            kt.DeleteRendering("wunkyboo");
            Assert.IsFalse(kt.Renderings.Contains("wunkyboo"));
		}

		private void MockRenderingRepoForGreekWordWithNoExistingInfo()
		{
			var project = MockRepository.GenerateMock<IProject>();
			var term = MockRepository.GenerateMock<IBiblicalTerm>();
			term.Stub(t => t.Lemma).Return("GreekWord");
			var renderings = MockRepository.GenerateMock<IBiblicalTermRenderings>();
			renderings.Stub(r => r.Renderings).Return(new List<string> {"abc", "xyz"});
			project.Stub(p => p.GetBiblicalTermRenderings(term, true)).Return(renderings);
			var termsList = new TestTermsList(new List<IBiblicalTerm> {term});
			project.Stub(p => p.BiblicalTermList).Return(termsList);
			m_repo = new ParatextTermRenderingsRepo(new TestKeyTermRenderingDataFileAccessor(), project);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the ability to add and remove additional renderings.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AddAndRemoveRenderings_ExistingInfo()
		{
			string[] renderings = { "abc", "xyz" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord")).Return(renderings);
			var info = new KeyTermRenderingInfo("diversion", "abc");
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(info);
			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"), m_repo);
			kt.AddRendering("wunkyboo");
			Assert.AreEqual(3, kt.Renderings.Count());
			Assert.That(info.AddlRenderings.Contains("wunkyboo"));
			Assert.IsTrue(kt.Renderings.Contains("wunkyboo"));
			Assert.IsTrue(kt.CanRenderingBeDeleted("wunkyboo"));
			Assert.IsFalse(kt.CanRenderingBeDeleted("abc"));
			kt.DeleteRendering("wunkyboo");
			Assert.IsFalse(kt.Renderings.Contains("wunkyboo"));
			Assert.That(info.AddlRenderings, Does.Not.Contain("wunkyboo"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that adding a rendering will cause it to be used (as the best/default) if
		/// there are no other renderings.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AddRendering_NoOtherRenderings_AddedRenderingIsDefault()
		{
			m_repo = new TermRenderingsRepo();
			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
			int renderingChangedHandlerCalls = 0;
			kt.BestRenderingChanged += delegate { renderingChangedHandlerCalls++; };
			kt.AddRendering("wunkyboo");
			Assert.AreEqual(1, kt.Renderings.Count());
			Assert.AreEqual("wunkyboo", kt.BestRendering);
			Assert.AreEqual("wunkyboo", kt.Translation);
			kt.LoadRenderings();
			Assert.AreEqual("wunkyboo", kt.BestRendering);
			Assert.AreEqual("wunkyboo", kt.Translation);
			kt.DeleteRendering("wunkyboo");
			Assert.AreEqual(string.Empty, kt.BestRendering);
			Assert.AreEqual(string.Empty, kt.Translation);
			Assert.That(renderingChangedHandlerCalls, Is.EqualTo(0));
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the KeyTerm.AddRendering method throws an exception if a duplicate
        /// is added.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void AddRenderingFailsToAddDuplicate()
        {
            string[] renderings = { "abc", "xyz" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord")).Return(renderings);
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(null);

            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
            Assert.Throws(typeof(ArgumentException), () => kt.AddRendering("abc"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for a
        /// rendering that is not in the list (should never happen in real life).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void CanRenderingBeDeleted_NonExistentRendering_ReturnsFalse()
        {
            string[] renderings = { "abc", "bwe" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord")).Return(renderings);
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(
				new KeyTermRenderingInfo("diversion", "bwe")
				{
					AddlRenderings = new List<string> {"fgj"}
				});

            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
            Assert.IsFalse(kt.CanRenderingBeDeleted("xyz"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for the
		/// default rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void CanRenderingBeDeleted_NotUserAdded_ReturnsFalse()
		{
			string[] renderings = { "abc", "xyz" };
			m_repo.Stub(r => r.GetTermRenderings("GreekWord")).Return(renderings);
			m_repo.Stub(r => r.GetRenderingInfo("diversion")).Return(null);

			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
			Assert.IsFalse(kt.CanRenderingBeDeleted("xyz"));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for the
		/// default rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void CanRenderingBeDeleted_ExplicitDefaultRendering_ReturnsFalse()
		{
			MockRenderingRepoForGreekWordWithNoExistingInfo();

			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
			int renderingChangedHandlerCalls = 0;
            kt.BestRenderingChanged += delegate(KeyTerm sender) 
			{
				Assert.That(sender, Is.EqualTo(kt));
				renderingChangedHandlerCalls++;
			};
			kt.AddRendering("bestest");
			kt.BestRendering = "bestest";
			Assert.IsFalse(kt.CanRenderingBeDeleted("bestest"));
            Assert.That(renderingChangedHandlerCalls, Is.EqualTo(1));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for the
		/// default rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void CanRenderingBeDeleted_ImplicitDefaultRendering_ReturnsTrue()
		{
			m_repo = new TermRenderingsRepo();

			var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"),
				m_repo);
			kt.AddRendering("bestest");
			Assert.IsTrue(kt.CanRenderingBeDeleted("bestest"));
		}
	}

    internal class TestKeyTermRenderingDataFileAccessor : DataFileAccessor
    {
        private string Data { get; set; }

        protected override void Write(DataFileId fileId, string data)
        {
            if (fileId != DataFileId.KeyTermRenderingInfo)
                throw new ArgumentException("Unexpected DataFileId '" + fileId + "'passed to Write method.", "fileId");
            Data = data;
        }

		public override bool BookSpecificDataExists(BookSpecificDataFileId fileId, string bookId)
		{
			throw new NotImplementedException();
		}

		protected override void WriteBookSpecificData(BookSpecificDataFileId fileId, string bookId, string data)
	    {
		    throw new NotImplementedException();
	    }

	    public override string Read(DataFileId fileId)
        {
            if (fileId != DataFileId.KeyTermRenderingInfo)
                throw new ArgumentException("Unexpected DataFileId '" + fileId + "'passed to Read method.", "fileId");
            return Data;
        }

        public override bool Exists(DataFileId fileId)
        {
            if (fileId != DataFileId.KeyTermRenderingInfo)
                throw new ArgumentException("Unexpected DataFileId '" + fileId + "'passed to Exists method.", "fileId");
            return Data != null;
        }

        public override DateTime ModifiedTime(DataFileId fileId)
        {
            throw new NotImplementedException();
        }
    }

	internal class TestTermsList : IBiblicalTermList
	{
		private readonly List<IBiblicalTerm> m_list;

		internal TestTermsList(List<IBiblicalTerm> list)
		{
			m_list = list;
		}

		public IEnumerator<IBiblicalTerm> GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Equals(IBiblicalTermList other) => false;

		public bool IsAvailable => true;
		public string LocalizedName => "Major Biblical Terms";
	}
}
