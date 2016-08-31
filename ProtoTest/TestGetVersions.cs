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
            Assert.AreEqual(2, a0T10?.Count);
            Assert.AreEqual(3, a0T40?.Count);

            var a0 = Engine.GetAgreement(1, 0);
            var a5 = Engine.GetAgreement(1, 5);
            var a10 = Engine.GetAgreement(1, 10);

            Assert.AreEqual(a0, a0T5?.SingleOrDefault(x => x.ValeurDate.Equals(0)));
            Assert.AreEqual(a5, a0T10?.SingleOrDefault(x => x.ValeurDate.Equals(5)));
            Assert.AreEqual(a10, a0T40?.SingleOrDefault(x => x.ValeurDate.Equals(10)));

        }

        [TestMethod]
        public void TestGetAllWithAgreementChanges()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 } }, 5);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 2 } }, 10);

            var ccs = Engine.GetCoverCollections(1, 0, 40);
            Assert.AreEqual(3, ccs.Count);
            Assert.AreEqual(Engine.GetCoverCollection(1, 0), ccs.SingleOrDefault(x => x.Valeur.Equals(0)));
            Assert.AreEqual(Engine.GetCoverCollection(1, 5), ccs.SingleOrDefault(x => x.Valeur.Equals(5)));
            Assert.AreEqual(Engine.GetCoverCollection(1, 10), ccs.SingleOrDefault(x => x.Valeur.Equals(10)));

        }

        [TestMethod]
        public void TestGetAllWithChangesOfBothKind()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 0 }, { "y", 1 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 5 } }, 0);

            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 5 }, { "a", 2 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }, 15);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 6 } }, 15);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "y", 8 } }, 20);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 4 } }, 0);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 9 } }, 15);

            var ccs = Engine.GetCoverCollections(1, 0, 40);
            Assert.AreEqual(4, ccs.Count);
            Assert.AreEqual(Engine.GetCoverCollection(1, 0), ccs.SingleOrDefault(x => x.Valeur.Equals(0)));
            Assert.AreEqual(Engine.GetCoverCollection(1, 10), ccs.SingleOrDefault(x => x.Valeur.Equals(10)));
            Assert.AreEqual(Engine.GetCoverCollection(1, 15), ccs.Last(x => x.Valeur.Equals(15)));
            Assert.AreEqual(Engine.GetCoverCollection(1, 20), ccs.SingleOrDefault(x => x.Valeur.Equals(20)));
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
