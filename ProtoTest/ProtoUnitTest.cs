using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoVersion;

namespace ProtoTest
{
    [TestClass]
    public class AgreementCoverCollectionAndEventTest
    {
        private Engine engine;

        [TestInitialize]
        public void Initialize()
        {
            Engine.ClearAll();
            engine = new Engine();
        }

        [TestMethod]
        public void TestAgreementCreation()
        {
            var agr = engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } });
            Assert.IsNotNull(agr);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(2, agr.Values["y"]);
            Assert.AreEqual(0, agr.ValeurDate);
            Assert.AreEqual(1, Engine.AgreementCount);

        }

        [TestMethod]
        public void TestGetAgreement()
        {
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } });
            var agr = Engine.GetAgreement(1, Engine.Time);
            Assert.IsNotNull(agr);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(2, agr.Values["y"]);
            Assert.AreEqual(0, agr.ValeurDate);
            Assert.AreEqual(1, Engine.AgreementCount);
        }



        [TestMethod]
        public void TestCreateCoverCollections()
        {
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } });
            var agr = Engine.GetAgreement(1, Engine.Time);


            //cc1: (x,y) = (1,2) a = 12 valeur=0
            var cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> { { "a", 12 } }, 0);
            Assert.IsNotNull(cc);
            Assert.AreEqual(1, cc.Values["x"]);
            Assert.AreEqual(2, cc.Values["y"]);
            Assert.AreEqual(12, cc.Values["a"]);
            Assert.AreEqual(0, cc.Valeur);

            ////cc2: (x,y) = (1,2) a = 23 valeur=0
            cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> { { "a", 23 } }, 0);
            Assert.IsNotNull(cc);
            Assert.AreEqual(1, cc.Values["x"]);
            Assert.AreEqual(2, cc.Values["y"]);
            Assert.AreEqual(23, cc.Values["a"]);
            Assert.AreEqual(0, cc.Valeur);

            Assert.AreEqual(2, Engine.CoverCollectionCount);


        }


        [TestMethod]
        public void TestChangeAgreement()
        {
            //x -> 2 -> 7 -> 9
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });

            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 } }, 10).Apply();
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 9 } }, 20).Apply();
            Assert.AreEqual(1, Engine.AgreementCount);

            var agr = Engine.GetAgreement(1, 5);

            Assert.IsNotNull(agr);
            Assert.AreEqual(2, agr.Values["x"]);
            Assert.AreEqual(0, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 15);

            Assert.IsNotNull(agr);
            Assert.AreEqual(7, agr.Values["x"]);
            Assert.AreEqual(10, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 25);

            Assert.IsNotNull(agr);
            Assert.AreEqual(9, agr.Values["x"]);
            Assert.AreEqual(20, agr.ValeurDate);

        }


        [TestMethod]
        public void TestChangeAgreementMultipleValues()
        {
            //x -> 2 -> 7 -> 9
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 }, { "y", 3 } });
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 }, { "y", 11 } }, 10).Apply();

            var agr = Engine.GetAgreement(1, 11);
            Assert.AreEqual(7, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
        }


        [TestMethod]
        public void TestChangeAgreementWithCollections()
        {
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } });
            var agr = Engine.GetAgreement(1, Engine.Time);

            engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> { { "a", 12 } }, 0);
            engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> { { "a", 23 } }, 0);

            const int x = 2;
            var y = agr.Values["y"];
            const int valeur = 20;

            //e1: x = 2 (valeur=20)
            //a1: (x,y) = (2,2) valuer=20
            agr = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> { { "x", x } }, valeur).Apply();


            var cc1V0 = Engine.GetCoverCollection(1, 0);
            var cc1V50 = Engine.GetCoverCollection(1, 50);

            Assert.AreNotEqual(cc1V0.Values["x"], cc1V50.Values["x"]);
            Assert.AreEqual(x, cc1V50.Values["x"]);
            Assert.AreEqual(y, cc1V50.Values["y"]);
            Assert.AreEqual(cc1V0.Values["a"], cc1V50.Values["a"]);
            Assert.AreEqual(valeur, cc1V50.Valeur);

            //e2: y = 3 (valeur=20)
            //a2: (x,y) = (2,3) valuer=20
            y = 3;

            agr = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> { { "y", y } }, valeur).Apply();
            Assert.IsNotNull(agr);
            Assert.AreEqual(x, agr.Values["x"]);
            Assert.AreEqual(y, agr.Values["y"]);
            Assert.AreEqual(20, agr.ValeurDate);
            Assert.AreEqual(1, Engine.AgreementCount);


            cc1V50 = Engine.GetCoverCollection(1, 50);

            Assert.AreNotEqual(cc1V0.Values["x"], cc1V50.Values["x"]);
            Assert.AreNotEqual(cc1V0.Values["y"], cc1V50.Values["y"]);
            Assert.AreEqual(x, cc1V50.Values["x"]);
            Assert.AreEqual(y, cc1V50.Values["y"]);
            Assert.AreEqual(cc1V0.Values["a"], cc1V50.Values["a"]);
            Assert.AreEqual(valeur, cc1V50.Valeur);

        }


        [TestMethod]
        public void TestCreateCoverCollectionWithAgreementEvent()
        {
            var valeur = 20;
            var x = 10;

            engine.CreateAgreement(new Dictionary<string, int> { { "x", x - 1 } });
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", x } }, valeur).Apply();

            var ccBeforeEvent = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, valeur - 1);
            var ccOnEvent = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 11 } }, valeur);
            var ccAfterEvent = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 11 } }, valeur + 1);

            Assert.IsNotNull(ccBeforeEvent);
            Assert.IsNotNull(ccOnEvent);
            Assert.IsNotNull(ccAfterEvent);

            Assert.AreEqual(valeur - 1, ccBeforeEvent.Valeur);
            Assert.AreEqual(valeur, ccOnEvent.Valeur);
            Assert.AreEqual(valeur + 1, ccAfterEvent.Valeur);

            Assert.AreNotEqual(x, ccBeforeEvent.Values["x"]);
            Assert.AreEqual(x, ccOnEvent.Values["x"]);
            Assert.AreEqual(x, ccAfterEvent.Values["x"]);

            ccBeforeEvent = Engine.GetCoverCollection(ccBeforeEvent.Id, valeur);
            Assert.AreEqual(x, ccBeforeEvent.Values["x"]);
        }


        [TestMethod]
        public void TestCreateCoverCollectionDifferentValeurs()
        {
            var vt = 20;

            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });

            var cc0 = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, vt - vt);
            var cc20 = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 11 } }, vt);
            var cc80 = engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 13 } }, vt * 4);

            Assert.AreEqual(1, engine.GetCoverCollections(vt - vt).Count);
            Assert.AreEqual(2, engine.GetCoverCollections(vt).Count);
            Assert.AreEqual(3, engine.GetCoverCollections(vt * 4).Count);
            Assert.AreEqual(3, engine.GetCoverCollections(vt * 5).Count);
        }


        [TestMethod]
        public void TestChangeCoverCollection()
        {
            var vt = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            var agr = Engine.GetAgreement(1, 20);
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, vt - vt);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "a", 19 } }, vt).Apply();
            var cc0 = Engine.GetCoverCollection(1, 0);
            var cc1 = Engine.GetCoverCollection(1, vt * 2);

            Assert.AreNotEqual(cc0.Values["a"], cc1.Values["a"]);

        }


        [TestMethod]
        public void TestChangeCoverCollectionAndAgreement()
        {
            var vt = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            var agr = Engine.GetAgreement(1, 20);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 } }, vt * 2).Apply();

            engine.CreateCoverCollection(1, new Dictionary<string, int>(), vt - vt);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 5 } }, vt).Apply();

            var cc0 = Engine.GetCoverCollection(1, 0);
            var cc1 = Engine.GetCoverCollection(1, vt);
            var cc2 = Engine.GetCoverCollection(1, vt * 3);

            Assert.AreEqual(2, cc0.Values["x"]);
            Assert.AreEqual(5, cc1.Values["x"]);
            Assert.AreEqual(5, cc2.Values["x"]);

        }


        [TestMethod]
        public void TestCoverCollectionAndChangeAgreementMultipleValues()
        {
            var vt = 10;
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 }, { "y", 3 } });
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 7 }, { "y", 9 } }, vt * 2).Apply();

            engine.CreateCoverCollection(1, new Dictionary<string, int>(), vt - vt);
            engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> { { "x", 5 } }, vt).Apply();

            var cc = Engine.GetCoverCollection(1, 0);
            Assert.AreEqual(2, cc.Values["x"]);
            Assert.AreEqual(3, cc.Values["y"]);

            cc = Engine.GetCoverCollection(1, vt + 1);
            Assert.AreEqual(5, cc.Values["x"]);
            Assert.AreEqual(3, cc.Values["y"]);

            cc = Engine.GetCoverCollection(1, vt * 2 + 1);
            Assert.AreEqual(5, cc.Values["x"]);
            Assert.AreEqual(9, cc.Values["y"]);
        }


    }
}
