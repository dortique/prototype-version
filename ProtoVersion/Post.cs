
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class Post
    {
        public int Id { get; set; }
        public int CoverCollectionId { get; set; }
        public int Valeur { get; set; }
        public DateTime Stamp { get; set; }
        public int Value { get; set; }

        public Post(int id, int value, int valeur)
        {
            CoverCollectionId = id;
            Value = value;
            Valeur = valeur;
            Id = Engine.Posts.Count + 1;
            Stamp = DateTime.Now;
        }
    }
}
