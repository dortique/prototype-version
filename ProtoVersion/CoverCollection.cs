using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class CoverCollection
    {
        public int Id { get; set; }
        public int AgreementId { get; private set; }
        public int Valeur { get; }


        private Dictionary<string, int> _values;
        public Dictionary<string, int> Values {
            get { return _values.Union(Engine.Agreements.Single(x => x.Id.Equals(AgreementId)).Get(Valeur).Values).ToDictionary(x => x.Key, y => y.Value); }
            //get { return Bogus(_values, Engine.Agreements.Single(x => x.Id.Equals(AgreementId)).Get(ValeurDate).Values); }
            private set { _values = value; }
        }

        //public int A { get; set; }
        public int CalculatedValue => Values.Sum(x => x.Value);


        public CoverCollection(int agreementId, Dictionary<string, int> values, int valeur)
        {
            Values = values;
            AgreementId = agreementId;
            Valeur = valeur;
            Id = Engine.CoverCollections.Count + 1;
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

            foreach (var evt in evts)
            {
                if (evt.ValeurDate > valeur)
                {
                    break;
                }
            }
            return new CoverCollection(AgreementId, values, Valeur)
            {
                Id = Id
            };

        }
    }
}
