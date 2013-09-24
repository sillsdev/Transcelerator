// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: KeyTermTests.cs
// ---------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System;

namespace SIL.Transcelerator
{
	[TestFixture]
	public class KeyTermTests
	{
	    private TestKeyTermRenderingDataFileAccessor m_testFileAccessor;
	    private Dictionary<string, string[]> m_renderings = new Dictionary<string, string[]>();

        [SetUp]
        public void TestSetup()
        {
            KeyTerm.FileAccessor = m_testFileAccessor = new TestKeyTermRenderingDataFileAccessor();
            KeyTerm.GetTermRenderings = s =>
                {
                    string[] renderings;
                    return (m_renderings.TryGetValue(s, out renderings)) ? new List<string>(renderings) : null;
                };
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the ability to get the "externally supplied" renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void GetRenderings_SurrogateIsForSingleBiblicalTermWithTwoRenderings_ReturnsBothRenderings()
        {
            string[] renderings = new[] { "abc", "xyz" };
            m_renderings["GreekWord"] = renderings;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"));
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
            string[] renderings1 = new[] { "abc", "xyz" };
            string[] renderings2 = new[] { "xyz", "def", "qrs" };
            m_renderings["GreekWord1"] = renderings1;
            m_renderings["HebrewWord"] = renderings2;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord1", "GreekWord2", "HebrewWord"));
            Assert.IsTrue(kt.Renderings.SequenceEqual(new [] { "abc", "xyz", "def", "qrs" }));
            Assert.AreEqual("xyz", kt.BestRendering);
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests the ability to add and remove additional renderings.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void AddAndRemoveRenderings()
        {
            string[] renderings = new[] { "abc", "xyz" };
            m_renderings["GreekWord"] = renderings;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"));
            Assert.IsFalse(m_testFileAccessor.Exists(DataFileAccessor.DataFileId.KeyTermRenderingInfo));
            kt.AddRendering("wunkyboo");
            Assert.IsTrue(m_testFileAccessor.Exists(DataFileAccessor.DataFileId.KeyTermRenderingInfo));
            Assert.AreEqual(3, kt.Renderings.Count());
            Assert.IsTrue(kt.Renderings.Contains("wunkyboo"));
            Assert.IsTrue(kt.CanRenderingBeDeleted("wunkyboo"));
            Assert.IsFalse(kt.CanRenderingBeDeleted("abc"));
            kt.DeleteRendering("wunkyboo");
            Assert.IsFalse(kt.Renderings.Contains("wunkyboo"));
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
            string[] renderings = new[] { "abc", "xyz" };
            m_renderings["GreekWord"] = renderings;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"));
            Assert.Throws(typeof(ArgumentException), () => kt.AddRendering("abc"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for a
        /// rendering that is not in the list (should never happen in real life).
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void CanRenderingBeDeleted_NonExistentRendering()
        {
            string[] renderings = new[] { "abc", "xyz" };
            m_renderings["GreekWord"] = renderings;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"));
            Assert.IsFalse(kt.CanRenderingBeDeleted("xyz"));
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Tests that the KeyTerm.CanRenderingBeDeleted method returns false for the
        /// default rendering.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [Test]
        public void CanRenderingBeDeleted_DefaultRendering()
        {
            string[] renderings = new[] { "abc", "xyz" };
            m_renderings["GreekWord"] = renderings;
            var kt = new KeyTerm(new KeyTermMatchSurrogate("diversion", "GreekWord"));
            kt.AddRendering("bestest");
            kt.BestRendering = "bestest";
            Assert.IsFalse(kt.CanRenderingBeDeleted("bestest"));
        }
	}

    internal class TestKeyTermRenderingDataFileAccessor : DataFileAccessor
    {
        internal string Data { get; set; }

        public override void Write(DataFileId fileId, string data)
        {
            if (fileId != DataFileId.KeyTermRenderingInfo)
                throw new ArgumentException("Unexpected DataFileId '" + fileId + "'passed to Write method.", "fileId");
            Data = data;
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
}
