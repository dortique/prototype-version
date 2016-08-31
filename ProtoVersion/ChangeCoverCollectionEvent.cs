using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class ChangeCoverCollectionEvent : BaseEvent, IHaveValeur
    {
        public int CoverCollectionId { get; set; }
        public override string ToString() => $"{GetType().Name}({Id})[{CoverCollectionId}]{string.Join(",", Changes.Select(x => $"({x.Key} => {x.Value})"))}(valeur = {ValeurDate}) (regdate = {RegisterDate})";
        public int GetValeur()
        {
            return ValeurDate;
        }

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
            Engine.DoCalculations(CoverCollectionId, ValeurDate);
            return Engine.GetCoverCollection(CoverCollectionId,ValeurDate);
        }

    }

}
