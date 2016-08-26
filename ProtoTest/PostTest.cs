using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoVersion;

namespace ProtoTest
{
    /// <summary>
    /// Summary description for PostTest
    /// </summary>
    [TestClass]
    public class PostTest
    {

        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }



        [TestMethod]
        public void TestLinearPost()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> {{"x", 2}});
            engine.CreateCoverCollection(1, new Dictionary<string, int> {{"a", 5}}, 0);
            Engine.DoCalculations(1, 0, 50);
            Assert.AreEqual(5, Engine.Posts.Count);
            Assert.AreEqual(7, Engine.Posts.First(x => x.Valeur.Equals(10)).Value);
            Assert.AreEqual(7, Engine.Posts.First(x => x.Valeur.Equals(50)).Value);
        }

        [TestMethod]
        public void TestChangeCoverEvent()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> {{"x", 2}});
            engine.CreateCoverCollection(1, new Dictionary<string, int> {{"a", 5}}, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> {{"a", 7}}, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(9, Engine.Posts.Sum(x => x.Value));


        }

        [TestMethod]
        public void TestMultipleChangeCoverEvent()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> {{"x", 2}});
            engine.CreateCoverCollection(1, new Dictionary<string, int> {{"a", 5}}, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> {{"a", 7}}, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(9, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 0).Apply();
            Assert.AreEqual(5, Engine.Posts.Count);
            Assert.AreEqual(11, Engine.Posts.Sum(x => x.Value));
        }
    }
}
