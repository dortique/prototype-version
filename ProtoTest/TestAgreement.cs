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
    public class TestAgreement
    {
        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }


        [TestMethod]
        public void TestLinearGet()
        {
            CreateLinear();

            var agr = Engine.GetAgreement(1, 0);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(0, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 10);
            Assert.AreEqual(3, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(10, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 20);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(20, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 30);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(11, agr.Values["y"]);
            Assert.AreEqual(20, agr.ValeurDate);
        }
        [TestMethod]

        public void TestBranchedGet()
        {
            CreateBranch();
            var agr = Engine.GetAgreement(1, 0);
            Assert.AreEqual(1, agr.Values["x"]);
            Assert.AreEqual(10, agr.Values["y"]);
            Assert.AreEqual(0, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 10);
            Assert.AreEqual(3, agr.Values["x"]);
            Assert.AreEqual(12, agr.Values["y"]);
            Assert.AreEqual(10, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 20);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(12, agr.Values["y"]);
            Assert.AreEqual(20, agr.ValeurDate);

            agr = Engine.GetAgreement(1, 30);
            Assert.AreEqual(4, agr.Values["x"]);
            Assert.AreEqual(13, agr.Values["y"]);
            Assert.AreEqual(30, agr.ValeurDate);
        }

        [TestMethod]

        public void TestLinearGetInterval()
        {
            CreateLinear();
            var ccs = Engine.GetAgreements(1, 0, 30);
            Assert.AreEqual(3, ccs.Count);
            for (var i = 0; i < ccs.Count; i++)
            {
                Assert.AreEqual(Engine.GetAgreement(1, i * 10), ccs.SingleOrDefault(x => x.ValeurDate.Equals(i * 10)));
            }
        }
        [TestMethod]

        public void TestBranchedGetInterval()
        {
            CreateBranch();
            var ccs = Engine.GetAgreements(1, 0, 40);
            Assert.AreEqual(4, ccs.Count);
            for (var i = 0; i < ccs.Count; i++)
            {
                Assert.AreEqual(Engine.GetAgreement(1, i * 10), ccs.SingleOrDefault(x => x.ValeurDate.Equals(i * 10)));
            }

        }
        [TestMethod]

        public void TestLinearGetBranch()
        {
            CreateLinear();
            var branch = Engine.GetAgreementBranch(1, 0, 30, DateTime.Now.AddHours(1));
            Assert.AreEqual(4, branch.Count);
            Assert.AreEqual(Engine.GetAgreement(1, 0), branch.First());
            Assert.AreNotEqual(Engine.GetAgreement(1, 10), branch.Skip(1).First());
            Assert.AreEqual(2, branch.Skip(1).First().Values["x"]);
            Assert.AreEqual(11, branch.Skip(1).First().Values["y"]);
            Assert.AreEqual(10, branch.Skip(1).First().ValeurDate);
            Assert.AreEqual(Engine.GetAgreement(1, 10), branch.Skip(2).First());
            Assert.AreEqual(Engine.GetAgreement(1, 20), branch.Skip(3).First());
        }
        [TestMethod]

        public void TestBranchedGetBranch()
        {
            var changeTime = CreateBranch();
            var branch = Engine.GetAgreementBranch(1, 0, 100, changeTime);
            Assert.AreEqual(6, branch.Count);
            Assert.AreEqual(Engine.GetAgreement(1, 0), branch.First());

            Assert.AreNotEqual(Engine.GetAgreement(1, 10), branch.Skip(1).First());
            Assert.AreEqual(2, branch.Skip(1).First().Values["x"]);
            Assert.AreEqual(11, branch.Skip(1).First().Values["y"]);
            Assert.AreEqual(10, branch.Skip(1).First().ValeurDate);

            Assert.AreNotEqual(Engine.GetAgreement(1, 10), branch.Skip(2).First());
            Assert.AreEqual(3, branch.Skip(2).First().Values["x"]);
            Assert.AreEqual(11, branch.Skip(2).First().Values["y"]);
            Assert.AreEqual(10, branch.Skip(2).First().ValeurDate);

            Assert.AreEqual(Engine.GetAgreement(1, 10), branch.Skip(3).First());
            Assert.AreEqual(Engine.GetAgreement(1, 20), branch.Skip(4).First());
        }
        private static void CreateLinear()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 1 }, { "y", 10 } });
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 2 }, { "y", 11 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 3 } }, 10);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "x", 4 } }, 20);
        }
        private static DateTime CreateBranch()
        {
            CreateLinear();
            var engine = new Engine();
            var t = DateTime.Now;
            Thread.Sleep(2000);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "y", 12 } }, 10);
            Thread.Sleep(2000);
            engine.CreateChangeAgreementEvent(1, new Dictionary<string, int> { { "y", 13 } }, 30);
            return t;
        }
    }
}
