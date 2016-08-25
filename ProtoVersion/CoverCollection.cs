using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class CoverCollection
    {
        public int Id { get; set; }
        public int AgreementId { get; private set; }
        public int[] Values { get; }

        public int Valeur { get;  }

        public int A { get; set; }
        public int CalculatedValue => Values.Sum()+A;

        public CoverCollection(int agreementId, int[] values, int a, int valeur)
        {
            Values = values;
            AgreementId = agreementId;
            A = a;
            Valeur = valeur;
            Id = Engine.CoverCollections.Count + 1;
        }

        public override string ToString()
        {
            return Get(Engine.Time);
        }

        public string Get(int valeurDate)
        {
            return $"CC{Id}[{string.Join("][", Values)}] [A:{A}] [cal:{CalculatedValue}] [agrId:{AgreementId}]";
        }
    }
}
