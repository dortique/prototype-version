using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class Message
    {
        public int EventId { get; set; }

        public Message(int eventid)
        {
            EventId = eventid;
        }

    }
}
