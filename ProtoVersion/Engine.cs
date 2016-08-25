using System.Collections.Generic;
using System.Linq;

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
            CoverCollectionEvents = CoverCollectionEvents ?? new List<ChangeCoverCollectionEvent>();
            Posts = Posts ?? new List<Post>();
            
            Messages = Messages ?? new List<Message>();
        }
        public static int Time { get; set; }
        public static ICollection<Agreement> Agreements { get; set; }
        public static ICollection<ChangeAgreementEvent> AgreementEvents { get; set; } 
        public static ICollection<CoverCollection> CoverCollections { get; set; }
        public static ICollection<ChangeCoverCollectionEvent> CoverCollectionEvents { get; set; } 
        public static ICollection<Message> Messages { get; set; }
        public static ICollection<Post>  Posts { get; set; }



        public static string ClearAll()
        {
            Agreements = new List<Agreement>();
            AgreementEvents = new List<ChangeAgreementEvent>();
            CoverCollections = new List<CoverCollection>();
            CoverCollectionEvents = new List<ChangeCoverCollectionEvent>();
            Messages = new List<Message>();
            Posts = new List<Post>();
            return "Data purged";
        }

        public ChangeAgreementEvent CreateChangeAgreementEvent(int agrId, Dictionary<string, int> changes, int valeur)
        {
            if (!Agreements.Any(x => x.Id.Equals(agrId)))
            {
                return null;
            }
            var ce = new ChangeAgreementEvent(agrId, changes, valeur);
            AgreementEvents.Add(ce);
            return ce;
        }

        public ChangeCoverCollectionEvent CreateChangeCoverCollectionEvent(int ccId, Dictionary<string, int> changes, int valeur)
        {
            if (!CoverCollections.Any(x => x.Id.Equals(ccId)))
            {
                return null;
            }
            var ce = new ChangeCoverCollectionEvent(ccId, changes, valeur);
            CoverCollectionEvents.Add(ce);
            return ce;
        }

        public Agreement CreateAgreement(Dictionary<string, int> values)
        {
            var a = new Agreement(values,0)
                {
                    Id = Agreements.Count + 1
                };
            Agreements.Add(a);
            return a;
        }

        public CoverCollection CreateCoverCollection(int agrId, Dictionary<string, int> values , int valeur)
        {
            var agr = Engine.Agreements.Single(x => x.Id.Equals(agrId)).Get(valeur);
            var ccValues = values.Where(val => !agr.Values.ContainsKey(val.Key) || !agr.Values[val.Key].Equals(val.Value)).ToDictionary(val => val.Key, val => val.Value);

            var cc = new CoverCollection(agr.Id, ccValues, valeur);
            Engine.CoverCollections.Add(cc);
            return cc.Get(valeur);
        }

        public ICollection<CoverCollection> GetCoverCollections(int valeur)
        {
            var result = CoverCollections.Where(x => x.Get(valeur) != null);
            return result.ToList();
        }

    }
}
