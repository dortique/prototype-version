using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public abstract class BaseEvent
    {
        public int Id;
        public int ValeurDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public Dictionary<string, int> Changes { get; set; }
    }
}
