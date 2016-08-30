using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtoVersion
{
    public class Agreement
    {

        public int Id { get; set; }
        public Dictionary<string, int> Values { get; }
        public int ValeurDate { get; }

        public Agreement(Dictionary<string, int> values, int valeur)
        {
            Values = values;
            ValeurDate = valeur;
        }

        public override string ToString()
        {
            return $"A{Id} {string.Join("", Values)} (valeur:{ValeurDate})";
        }

        public Agreement Get(int valeur)
        {
            var evts = Engine.AgreementEvents.Where(x => x.AgreementId.Equals(Id)).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);

            var values = new Dictionary<string, int>(Values);
            var result = new Agreement(values, ValeurDate)
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
            return result;

        }

        private Agreement ApplyChange(Agreement result, ChangeAgreementEvent evt)
        {
            result = new Agreement(new Dictionary<string, int>(result.Values), evt.ValeurDate)
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
            return result;
        }

        internal ICollection<Agreement> Get(int valeur1, int valeur2)
        {
            var result = new List<Agreement>();
            var agr = Get(valeur1);
            result.Add(agr);

            var events = Engine.AgreementEvents.Where(x => x.AgreementId.Equals(Id) && x.ValeurDate > valeur1 && x.ValeurDate < valeur2).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);
            foreach (var evt in events)
            {
                agr = ApplyChange(agr, evt);
                //var pc = Engine.GetProductCatalgoue(agr.ProductCatalogueId, agr.Valeur);
                //MergeValues(pc, agr);
                result.Add(agr);
            }

            return result;
        }
        public override bool Equals(object obj)
        {
            var result = obj != null && obj.GetType() == GetType();
            var a = (Agreement)obj;
            result = result && a.Id.Equals(Id) && a.ValeurDate.Equals(ValeurDate) && a.Values.Count.Equals(Values.Count);
            result = result && !a.Values.Except(Values).Any();
            return result;
        }
    }
}
