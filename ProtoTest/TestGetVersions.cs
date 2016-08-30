using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoVersion;

namespace ProtoTest
{
    /// <summary>
    /// Summary description for TestGetVersions
    /// </summary>
    [TestClass]
    public class TestGetVersions
    {

        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }


        [TestMethod]
        public void TestGetAllWithCoverCollectionChanges()
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
        public void TestGetAllAgreements()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 }, { "y", 0 } });
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 } }, 5);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "y", 4 } }, 5);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "z", 9 } }, 10);

            var a0T5 = Engine.GetAgreements(1, 0, 5);
            var a0T10 = Engine.GetAgreements(1, 0, 10);
            var a0T40 = Engine.GetAgreements(1, 0, 40);

            Assert.AreEqual(1, a0T5?.Count);
            Assert.AreEqual(3, a0T10?.Count);
            Assert.AreEqual(4, a0T40?.Count);

            var a0 = Engine.GetAgreement(1, 0);
            var a5 = Engine.GetAgreement(1, 5);
            var a10 = Engine.GetAgreement(1, 10);

            Assert.AreEqual(a0, a0T5?.Single(x => x.ValeurDate.Equals(0)));
            Assert.AreEqual(a5, a0T10?.Last(x => x.ValeurDate.Equals(5)));
            Assert.AreEqual(a10, a0T40?.Single(x => x.ValeurDate.Equals(10)));

        }


        [TestMethod]
        public void TestGetAllWithAgreementChanges()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 } }, 5);

            Assert.AreEqual(2, Engine.GetCoverCollections(1, 0, 40).Count);

        }

        [TestMethod]
        public void TestValuesGetAll()
        {
            var engine = new Engine();
            Engine.Time = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 2 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 4 } }, 5);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 6 } }, 8);


            var cc0T10 = Engine.GetCoverCollections(1, 0, 10);
            Assert.AreEqual(3, cc0T10.Count);
            var cc0 = cc0T10.SingleOrDefault(x => x.Valeur == 0);
            var cc5 = cc0T10.SingleOrDefault(x => x.Valeur == 5);
            var cc8 = cc0T10.SingleOrDefault(x => x.Valeur == 8);

            Assert.AreEqual(0, cc0?.Values["x"]);
            Assert.AreEqual(2, cc0?.Values["a"]);

            Assert.AreEqual(0, cc5?.Values["x"]);
            Assert.AreEqual(4, cc5?.Values["a"]);

            Assert.AreEqual(6, cc8?.Values["x"]);
            Assert.AreEqual(4, cc8?.Values["a"]);


        }
    }
}
