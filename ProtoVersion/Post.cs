
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class Post
    {
        public int Valeur { get; set; }
        public DateTime Stamp { get; set; }
        public int Value { get; set; }

        public Post(int value, int valeur)
        {
            Value = value;
            Valeur = valeur;
            Stamp = DateTime.Now;
        }
    }
}
