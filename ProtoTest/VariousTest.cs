using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtoTest
{
    [TestClass]
    public class VariousTest
    {

        [TestMethod]
        public void TestCeiling()
        {
            var test = new []  {-19, -11, -10, -2, -1,  0,  1,   2,  9, 10, 13, 102};
            var result = new[] {-10, -10,   0,  0,  0, 10, 10,  10, 10, 20, 20, 110};
            for (var i=0; i<test.Length; i++) 
            {
                Assert.AreEqual(result[i], RoundUp(test[i]));
            }
        }


        private int RoundUp(int x)
        {
            return (int)Math.Ceiling( (x+1)/10.0)*10;
        }

    }

    
}
