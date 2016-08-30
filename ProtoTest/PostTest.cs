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
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
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
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 7 } }, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(9, Engine.Posts.Sum(x => x.Value));


        }

        [TestMethod]
        public void TestMultipleChangeCoverEvent()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 7 } }, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(9, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 0).Apply();
            Assert.AreEqual(5, Engine.Posts.Count);
            Assert.AreEqual(11, Engine.Posts.Sum(x => x.Value));
        }

        [TestMethod]
        public void TestChangeAgreementEvent()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 } }, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(12, Engine.Posts.Sum(x => x.Value));
        }

        [TestMethod]
        public void TestMultipleChangeAgreementEvent()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 } }, 0).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(12, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 9 } }, 0).Apply();
            Assert.AreEqual(5, Engine.Posts.Count);
            Assert.AreEqual(14, Engine.Posts.Sum(x => x.Value));
        }

        [TestMethod]
        public void TestMultipleChanges()
        {
            var engine = new Engine();
            Engine.Time = 45;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning
            Assert.AreEqual(4, Engine.Posts.Count);
            Assert.IsTrue(Engine.Posts.All(x => x.Value.Equals(7)));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 7 } }, 20).Apply();
            Assert.AreEqual(8, Engine.Posts.Count);
            Assert.AreEqual(32, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 10).Apply();
            Assert.AreEqual(14, Engine.Posts.Count);
            Assert.AreEqual(36, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 } }, 10).Apply();
            Assert.AreEqual(20, Engine.Posts.Count);
            Assert.AreEqual(33, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 11 } }, 0).Apply();
            Assert.AreEqual(28, Engine.Posts.Count);
            Assert.AreEqual(72, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 19 } }, 30).Apply();
            Assert.AreEqual(30, Engine.Posts.Count);
            Assert.AreEqual(72, Engine.Posts.Sum(x => x.Value));
        }

        [TestMethod]
        public void TestChangeMiddleOfMonth()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 2} }, 0);

            Engine.DoCalculations(1, 0, Engine.Time); //"normal" fremregning
            Assert.AreEqual(1, Engine.Posts.Count);
            Assert.IsTrue(Engine.Posts.All(x => x.Value.Equals(2)));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 4 } }, 5).Apply();
            Assert.AreEqual(3, Engine.Posts.Count);
            Assert.AreEqual(3m, Engine.Posts.Sum(x => x.Value));

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 6 } }, 8).Apply();
            Assert.AreEqual(5, Engine.Posts.Count);
            Assert.AreEqual(4.2m, Engine.Posts.Sum(x => x.Value));
        }


    }
}
