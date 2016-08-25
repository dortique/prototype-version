using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoVersion
{
    public class Engine
    {
        public Engine()
        {
            Time = 100;
            Agreements = Agreements ?? new List<Agreement>();
            AgreementEvents = AgreementEvents ?? new List<ChangeAgreementEvent>();
            CoverCollections = CoverCollections ?? new List<CoverCollection>();
            
            Messages = Messages ?? new List<Message>();
        }
        public static int Time { get; set; }
        public static ICollection<Agreement> Agreements { get; set; }
        public static ICollection<ChangeAgreementEvent> AgreementEvents { get; set; } 
        public static ICollection<CoverCollection> CoverCollections { get; set; }
        public static ICollection<Message> Messages { get; set; }

   



        public static string ClearAll()
        {
            Agreements = new List<Agreement>();
            AgreementEvents = new List<ChangeAgreementEvent>();
            Messages = new List<Message>();
            return "Data purged";
        }

        public ChangeAgreementEvent CreateChangeAgreementEvent(int agrId, int index, int value, int valeur)
        {
            if (!Agreements.Any(x => x.Id.Equals(agrId)))
            {
                return null;
            }
            var ce = new ChangeAgreementEvent(agrId, index , value, valeur);
            AgreementEvents.Add(ce);
            return ce;
        }



        public Agreement CreateAgreement(int[] values)
        {
            var a = new Agreement(values,0)
                {
                    Id = Agreements.Count + 1
                };
            Agreements.Add(a);
            return a;
        }

        public CoverCollection CreateCoverCollection(int agrId, int someValue, int valeur)
        {
            var agr = Engine.Agreements.Single(x => x.Id.Equals(agrId)).Get(valeur);
            var cc = new CoverCollection(agr.Id, agr.Values, someValue, valeur);
            Engine.CoverCollections.Add(cc);
            return cc;
        }

    }
}
