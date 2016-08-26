using System.Collections.Generic;
using System.Linq;

namespace ProtoVersion
{
    public class CoverCollection
    {
        public int Id { get; set; }
        public int AgreementId { get; private set; }
        public int Valeur { get; private set; }

        public Dictionary<string, int> Values { get; }
  
        public int CalculatedValue => Values.Sum(x => x.Value);

        public Dictionary<string, int> Bogus(Dictionary<string, int> a, Dictionary<string, int> b)
        {
            var c = a.Concat(b.Where(x => !a.ContainsKey(x.Key)));
            return c.ToDictionary(x => x.Key, y => y.Value);
        }


        public CoverCollection(int agreementId, Dictionary<string, int> values, int valeur)
        {
            Values = values;
            AgreementId = agreementId;
            Valeur = valeur;
            Id = Engine.CoverCollectionCount + 1;
            //Engine.Posts.Add(new Post(A*Values[0], Valeur));
        }

        public override string ToString()
        {
            return $"CC{Id}[{string.Join(" ",Values)} [cal:{CalculatedValue}] [agrId:{AgreementId}] [valeur:{Valeur}]";
        }

        public CoverCollection Get(int valeur)
        {
            if (Valeur > valeur) return null;

            var evts = Engine.CoverCollectionEvents.Where(x => x.CoverCollectionId.Equals(Id)).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);

            var values = new Dictionary<string,int>(Values);

            var result = new CoverCollection(AgreementId,values, Valeur)
            {
                Id = Id
            };

            foreach (var evt in evts)
            {
                if (evt.ValeurDate > valeur)
                {
                    break;
                }
                result = new CoverCollection(AgreementId, result.Values, evt.ValeurDate)
                {
                    Id = Id
                };

                foreach (var change in evt.Changes)
                {

                    if (result.Values.ContainsKey(change.Key))
                    {
                        result.Values[change.Key] = change.Value;
                    }
                    else
                    {
                        result.Values.Add(change.Key, change.Value);
                    }
                }
            }
            var agr = Engine.GetAgreement(AgreementId,valeur);
            if (agr.ValeurDate > result.Valeur) result.Valeur = agr.ValeurDate;
            foreach (var val in agr.Values)
            {
                if (!result.Values.ContainsKey(val.Key))
                {
                    result.Values.Add(val.Key, val.Value);
                }
            }


            return result;

        }
    }
}
