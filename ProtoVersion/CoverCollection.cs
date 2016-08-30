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

        public ICollection<CoverCollection> Get(int valeur1, int valeur2)
        {
            var result = new List<CoverCollection>();
            var cc = Get(valeur1);
            result.Add(cc);

            var agrs = Engine.GetAgreements(cc.AgreementId, valeur1, valeur2);

            var events = Engine.CoverCollectionEvents.Where(x => x.CoverCollectionId.Equals(Id) && x.ValeurDate > valeur1 && x.ValeurDate < valeur2).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);
            foreach (var evt in events)
            {
                cc = ApplyChange(cc, evt);
                var agr = Engine.GetAgreement(cc.AgreementId, cc.Valeur);
                MergeValues(agr, cc);
                result.Add(cc);
            }


            return result;
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
                result = ApplyChange(result, evt);
            }
            var agr = Engine.GetAgreement(AgreementId,valeur);
            MergeValues(agr, result);
            if (agr.ValeurDate > result.Valeur) result.Valeur = agr.ValeurDate;


            return result;

        }

        private static void MergeValues(Agreement agr, CoverCollection result)
        {
            foreach (var val in agr.Values)
            {
                if (!result.Values.ContainsKey(val.Key))
                {
                    result.Values.Add(val.Key, val.Value);
                }
            }
        }

        private CoverCollection ApplyChange(CoverCollection result, ChangeCoverCollectionEvent evt)
        {
            var cc = new CoverCollection(AgreementId, new Dictionary<string, int>(result.Values), evt.ValeurDate)
            {
                Id = Id
            };

            foreach (var change in evt.Changes)
            {
                if (cc.Values.ContainsKey(change.Key))
                {
                    cc.Values[change.Key] = change.Value;
                }
                else
                {
                    cc.Values.Add(change.Key, change.Value);
                }
            }
            return cc;
        }

        public override bool Equals(object obj)
        {
            var result = obj != null && obj.GetType() == GetType();
            var cc = (CoverCollection) obj;
            result = result && cc.AgreementId.Equals(AgreementId) && cc.Id.Equals(Id) && cc.Valeur.Equals(Valeur) &&
                     cc.CalculatedValue.Equals(CalculatedValue) && cc.Values.Count.Equals(Values.Count);
             result = result && !cc.Values.Except(Values).Any();
            return result;
        }

    }
}
