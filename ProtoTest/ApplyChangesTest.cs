using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoVersion;

namespace ProtoTest
{
    [TestClass]
    public class ApplyChangesTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            Engine.ClearAll();
        }
        [TestMethod]
        public void TestOne()
        {
            var engine = new Engine();
            engine.CreateAgreement(new Dictionary<string, int> { { "x", 2 } });
            engine.CreateCoverCollection(1, new Dictionary<string, int> { { "a", 7 } }, 0);

        }


    }

    
}
