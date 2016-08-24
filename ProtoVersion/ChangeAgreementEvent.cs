using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class ChangeAgreementEvent
    {
        public int Id;
        public int ValeurDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public int AgreementId { get; set; }
        public int Key { get; set; }
        public int Value { get; set; }
        public override string ToString() => $"CAE({Id})[agrId:{AgreementId}][key:{Key}][val:{Value}][valeur:{ValeurDate}]";

        public ChangeAgreementEvent(int agreementId, int key, int value, int valeur)
        {
            AgreementId = agreementId;
            Id = Engine.AgreementEvents.Count + 1;
            ValeurDate = valeur;
            Key = key;
            Value = value;
            RegisterDate = DateTime.Now;
        }

        public Agreement Apply()
        {
            return Engine.Agreements.Single(x => x.Id.Equals(AgreementId)).Get(ValeurDate);
        }

    }
}
