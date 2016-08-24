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

        public int AgreementIndependentValue { get; set; }
        public int CalculatedValue => Values.Sum();

        public CoverCollection(int agreementId, int[] values, int agreementIndependentValue)
        {
            Values = values;
            AgreementId = agreementId;
            AgreementIndependentValue = agreementIndependentValue;
            Id = Engine.CoverCollections.Count + 1;
        }

        public override string ToString()
        {
            return Get(Engine.Time);
        }

        public string Get(int valeurDate)
        {
            return $"CC{Id}[{string.Join("][", Values)}][cal:{CalculatedValue}] [agrId:{AgreementId}]";
        }
    }
}
