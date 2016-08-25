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
        public Dictionary<string, int> Changes { get; set; }
        public override string ToString() => $"CAE({Id})[agrId:{AgreementId}]{Changes.Select(x => $"[{x.Key}={x.Value}]")}[valeur:{ValeurDate}]";

        public ChangeAgreementEvent(int agreementId, Dictionary<string, int> changes, int valeur)
        {
            AgreementId = agreementId;
            Id = Engine.AgreementEvents.Count + 1;
            ValeurDate = valeur;
            Changes = changes;
            RegisterDate = DateTime.Now;
        }

        public Agreement Apply()
        {
            Engine.Messages.Add(new Message(Id));
            return Engine.Agreements.Single(x => x.Id.Equals(AgreementId)).Get(ValeurDate);
        }

    }
}
