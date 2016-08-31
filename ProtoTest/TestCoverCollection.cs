using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoVersion;

namespace ProtoTest
{
    [TestClass]
    public class TestCoverCollection
    {
        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }


        [TestMethod]
        public void TestOnlyAgrEventsLinearGet()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 10 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 8 } }, 0);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 2 }, { "y", 11 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 3 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 4 } }, 20);

            var agr = Engine.GetCoverCollection(1, 0);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(0, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 10);
            Assert.AreEqual(3, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(10, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 20);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(20, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 30);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(20, agr.Valeur);
        }

        [TestMethod]
        public void TestOnlyCollectionEventsLinearGet()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 10 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 8 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 2 }, { "a", 9 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 12 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "y", 100 } }, 20);

            var agr = Engine.GetCoverCollection(1, 0);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(0, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 10);
            Assert.AreEqual(2, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(12, agr.Values["a"]);
            Assert.AreEqual(10, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 20);
            Assert.AreEqual(2, agr.Values["x"]);
            Assert.AreEqual(100, agr.Values["y"]);
            Assert.AreEqual(12, agr.Values["a"]);
            Assert.AreEqual(20, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 30);
            Assert.AreEqual(2, agr.Values["x"]);
            Assert.AreEqual(100, agr.Values["y"]);
            Assert.AreEqual(12, agr.Values["a"]);
            Assert.AreEqual(20, agr.Valeur);
        }


        [TestMethod]
        public void TestAllEventsLinearGet()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 10 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 8 } }, 0);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 }, { "y", 8 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 2 }, { "y", 10 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 3 }, { "a", 9 } }, 20);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 4 }, { "y", 7 } }, 30);

            var agr = Engine.GetCoverCollection(1, 0);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(0, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 10);
            Assert.AreEqual(2, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(8, agr.Values["a"]);
            Assert.AreEqual(10, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 20);
            Assert.AreEqual(3, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(9, agr.Values["a"]);
            Assert.AreEqual(20, agr.Valeur);

            agr = Engine.GetCoverCollection(1, 30);
            Assert.AreEqual(3, agr.Values["x"]);
            Assert.AreEqual(7, agr.Values["y"]);
            Assert.AreEqual(9, agr.Values["a"]);
            Assert.AreEqual(30, agr.Valeur);

        }

        [TestMethod]

        public void TestBranchedGet()
        {
           
        }


        [TestMethod]
        public void TestGetAllCollectionChanges()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 7 } }, 20);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 35);

            Assert.AreEqual(3, Engine.GetCoverCollections(1, 0, 40).Count);
            Assert.AreEqual(1, Engine.GetCoverCollections(1, 0, 10).Count);
            Assert.AreEqual(1, Engine.GetCoverCollections(1, 10, 20).Count);
            Assert.AreEqual(2, Engine.GetCoverCollections(1, 30, 40).Count);
            Assert.AreEqual(1, Engine.GetCoverCollections(1, 30, 35).Count);
            Assert.AreEqual(2, Engine.GetCoverCollections(1, 30, 36).Count);
            Assert.AreEqual(3, Engine.GetCoverCollections(1, 10, 40).Count);
            Assert.AreEqual(2, Engine.GetCoverCollections(1, 20, 40).Count);

        }
        [TestMethod]

        public void TestLinearGetInterval()
        {
           
        }
        [TestMethod]

        public void TestBranchedGetInterval()
        {
          

        }
        [TestMethod]

        public void TestLinearGetBranch()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 10 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 8 } }, 0);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 }, { "y", 8 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 2 }, { "y", 10 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 3 }, { "a", 9 } }, 20);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 4 }, { "y", 7 } }, 30);

            var branch = Engine.GetCoverCollectionBranch(1, 10, 100, DateTime.Now.AddHours(1));
            Assert.AreEqual(4, branch.Count);

            var cc = branch.First();
            Assert.AreEqual(1, cc.Values["x"]);
            Assert.AreEqual(8, cc.Values["y"]);
            Assert.AreEqual(8, cc.Values["a"]);
            Assert.AreEqual(10, cc.Valeur);

            cc = branch.Skip(1).First();
            Assert.AreEqual(2, cc.Values["x"]);
            Assert.AreEqual(10, cc.Values["y"]);
            Assert.AreEqual(8, cc.Values["a"]);
            Assert.AreEqual(10, cc.Valeur);

            cc = branch.Skip(2).First();
            Assert.AreEqual(3, cc.Values["x"]);
            Assert.AreEqual(10, cc.Values["y"]);
            Assert.AreEqual(9, cc.Values["a"]);
            Assert.AreEqual(20, cc.Valeur);

            cc = branch.Skip(3).First();
            Assert.AreEqual(3, cc.Values["x"]);
            Assert.AreEqual(7, cc.Values["y"]);
            Assert.AreEqual(9, cc.Values["a"]);
            Assert.AreEqual(30, cc.Valeur);

        }
        [TestMethod]

        public void TestBranchedGetBranch()
        {
       
        }

   
    }
}
