using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class ChangeCoverCollectionEvent
    {
        public int Id;
        public int ValeurDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public int CoverCollectionId { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
        public override string ToString() => $"{GetType().Name}({Id})[{CoverCollectionId}:key {Key}:val {Value}:valeur {ValeurDate}] (regdate = {RegisterDate})";

        public ChangeCoverCollectionEvent(int coverCollectionId, string key, int value, int valeur, int id)
        {
            CoverCollectionId = coverCollectionId;
            Id = id;
            ValeurDate = valeur;
            Key = key;
            Value = value;
            RegisterDate = DateTime.Now;
        }

        public CoverCollection Apply()
        {
            return Engine.CoverCollections.Single(x => x.Id.Equals(CoverCollectionId)).Get(ValeurDate);
        }

    }
}
