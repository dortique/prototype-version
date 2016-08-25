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
        public Dictionary<string, int> Changes { get; set; }
        public override string ToString() => $"{GetType().Name}({Id})[{CoverCollectionId}]{string.Join(",", Changes.Select(x => $"({x.Key} => {x.Value})"))}(valeur = {ValeurDate}) (regdate = {RegisterDate})";

        public ChangeCoverCollectionEvent(int coverCollectionId, Dictionary<string, int> changes, int valeur)
        {
            CoverCollectionId = coverCollectionId;
            Id = Engine.CoverCollectionEvents.Count + 1;
            ValeurDate = valeur;
            Changes = changes;
            RegisterDate = DateTime.Now;
        }

        public CoverCollection Apply()
        {
            var cc = Engine.CoverCollections.Single(x => x.Id.Equals(CoverCollectionId)).Get(ValeurDate);
            Engine.Post(cc.Id, cc.CalculatedValue, ValeurDate);
            return cc;
        }

    }

}
