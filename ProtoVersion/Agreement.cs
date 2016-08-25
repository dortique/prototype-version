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


        public Agreement(Dictionary<string,int> values, int valeur)
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

                result = new Agreement(values, evt.ValeurDate)
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
            return result;

        }
    }
}
