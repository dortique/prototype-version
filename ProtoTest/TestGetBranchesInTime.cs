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
    public class TestGetBranchesInTime
    {

        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }

        [TestMethod]
        public void TestOneBranchBetweenDates()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 8 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 20);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 10 } }, 30);
            Thread.Sleep(2000);
            var changeTime = DateTime.Now;
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 6 } }, 5);
            Thread.Sleep(2000);

            Assert.AreEqual(5, Engine.GetCoverCollectionBranch(1, 0, 50, DateTime.Now).Count);
            Assert.AreEqual(4, Engine.GetCoverCollectionBranch(1, 0, 50, changeTime.AddMilliseconds(-1)).Count);

        }
        [TestMethod]
        public void TestOneBranchOnSameDate()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 8 } }, 10);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 9 } }, 20);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 10 } }, 30);
            Thread.Sleep(2000);

            var changeTime = DateTime.Now;
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 6 } }, 10);
            Thread.Sleep(2000);

            Assert.AreEqual(5, Engine.GetCoverCollectionBranch(1, 0, 50, DateTime.Now).Count);
            Assert.AreEqual(4, Engine.GetCoverCollectionBranch(1, 0, 50, changeTime.AddMilliseconds(-1)).Count);

        }

        [TestMethod]
        public void TestLinear()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 }, { "y", 3 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 5 }, { "a", 8 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 50 }, { "y", 4 } }, 20);

            var cc0 = Engine.GetCoverCollection(1, 0);
            var cc10 = Engine.GetCoverCollection(1, 10);
            var cc20 = Engine.GetCoverCollection(1, 20);

            var ccs = Engine.GetCoverCollections(1, 0, 21);
            var branch = Engine.GetCoverCollectionBranch(1, 0, 21, DateTime.Now.AddHours(1));

            Assert.AreEqual(2, cc0.Values["x"]);
            Assert.AreEqual(3, cc0.Values["y"]);
            Assert.AreEqual(7, cc0.Values["a"]);

            Assert.AreEqual(5, cc10.Values["x"]);
            Assert.AreEqual(3, cc10.Values["y"]);
            Assert.AreEqual(8, cc10.Values["a"]);

            Assert.AreEqual(5, cc20.Values["x"]);
            Assert.AreEqual(4, cc20.Values["y"]);
            Assert.AreEqual(8, cc20.Values["a"]);

            Assert.AreEqual(3, ccs.Count);
            Assert.AreEqual(cc0, ccs.SingleOrDefault(x => x.Valeur.Equals(0)));
            Assert.AreEqual(cc10, ccs.SingleOrDefault(x => x.Valeur.Equals(10)));
            Assert.AreEqual(cc20, ccs.SingleOrDefault(x => x.Valeur.Equals(20)));

            Assert.AreEqual(3, branch.Count);
            Assert.AreEqual(cc0, branch.First());
            Assert.AreEqual(cc10, branch.Skip(1).First());
            Assert.AreEqual(cc20, branch.Skip(2).First());
        }
    }
}
