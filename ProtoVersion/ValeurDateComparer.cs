using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class ValeurDateComparer : IComparer<IHaveValeur>
    {
        public int Compare(IHaveValeur o1, IHaveValeur o2)
        {
            return o1.GetValeur().CompareTo(o2.GetValeur());
        }
    }
}
