using System;
using System.Linq;

namespace ProtoVersion
{
    public class Agreement
    {

        public int Id { get; set; }
        public int[] Values { get; }
        public Agreement(int[] values, int valeur)
        {
            Values = values;
            ValeurDate = valeur;
        }
        public int ValeurDate { get;  }

        public override string ToString()
        {
            return $"A{Id} [{string.Join("] [", Values)}] (valeur:{ValeurDate})";
        }

        public Agreement Get(int valeur)
        {
            var evts = Engine.AgreementEvents.Where(x => x.AgreementId.Equals(Id)).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);

            var values = new int[Values.Length];
            Values.CopyTo(values, 0);
            foreach (var evt in evts)
            {
                if (evt.ValeurDate > valeur)
                {
                    break;
                }
                if (evt.Key >= values.Length)
                {
                    Array.Resize(ref values, evt.Key + 1);
                }
                values[evt.Key] = evt.Value;
            }
            return new Agreement(values, valeur)
            {
                Id = Id
            };

        }
    }
}
