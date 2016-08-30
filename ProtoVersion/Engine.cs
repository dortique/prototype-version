using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

            if (Messages == null)
            {
                Messages = new ObservableCollection<Message>();
                Messages.CollectionChanged += HandleMessage;
            }
        }

        public static int Time { get; set; }
        private static ICollection<Agreement> Agreements { get; set; }
        public static ICollection<ChangeAgreementEvent> AgreementEvents { get; set; }
        private static ICollection<CoverCollection> CoverCollections { get; set; }
        public static ICollection<ChangeCoverCollectionEvent> CoverCollectionEvents { get; set; }
        public static ObservableCollection<Message> Messages { get; set; }
        public static ICollection<Post> Posts { get; set; }
        public static int AgreementCount => Agreements.Count;
        public static int CoverCollectionCount => CoverCollections.Count;


        public static Agreement GetAgreement(int id, int valeur)
        {
            return Agreements.SingleOrDefault(x => x.Id.Equals(id))?.Get(valeur);
        }

        public static ICollection<Agreement> GetAgreements(int id, int vt1, int vt2)
        {
            return Agreements.SingleOrDefault(x => x.Id.Equals(id))?.Get(vt1, vt2);
        }

        public static CoverCollection GetCoverCollection(int id, int valeur)
        {
            return CoverCollections.SingleOrDefault(x => x.Id.Equals(id))?.Get(valeur);
        }
        public static ICollection<CoverCollection> GetCoverCollections(int id, int vt1, int vt2)
        {
            return CoverCollections.SingleOrDefault(x => x.Id.Equals(id))?.Get(vt1, vt2);
        }

        public static string ClearAll()
        {
            Agreements = new List<Agreement>();
            AgreementEvents = new List<ChangeAgreementEvent>();
            CoverCollections = new List<CoverCollection>();
            CoverCollectionEvents = new List<ChangeCoverCollectionEvent>();
            Messages = new ObservableCollection<Message>();
            Messages.CollectionChanged += HandleMessage;
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
            var a = new Agreement(values, 0)
            {
                Id = Agreements.Count + 1
            };
            Agreements.Add(a);
            return a;
        }

        public CoverCollection CreateCoverCollection(int agrId, Dictionary<string, int> values, int valeur)
        {
            var agr = Engine.GetAgreement(agrId, valeur);
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

        public static void DoCalculations(int id, int startValeur)
        {
            var end = Posts.Any(x => x.CoverCollectionId.Equals(id)) ? Posts.Where(x => x.CoverCollectionId.Equals(id)).Max(y => y.Valeur) : Engine.Time;
            DoCalculations(id, startValeur, end);
        }

        public static void DoCalculations(int id, int startValeur, int endValeur)
        {
            if (endValeur <= startValeur) return;

            //var vt = startValeur < 10 ? 10 : startValeur;
            var vt = NextCalculationTime(startValeur);
            //Round up to nearest multiple of ten (mimick first of month)

            while (vt <= endValeur)
            {
                var ccs = GetCoverCollections(id, vt - 10, vt);
                //var cc = GetCoverCollection(id, vt - 10);
                var previous = Posts.Where(x => x.CoverCollectionId.Equals(id) && x.Valeur.Equals(vt)).OrderByDescending(o => o.Id).FirstOrDefault();
                if (previous != null)
                    Posts.Add(new Post(id, previous.Value * -1, vt));
                // handle many versions during calculation interval



                var n = 0.0m;
                var prevVt = vt;
                foreach (var cc in ccs.OrderByDescending(o => o.Valeur))
                {
                    var m = cc.CalculatedValue*1.0m;
                    m = m/10.0m;
                    var p = prevVt - (cc.Valeur < vt-10 ? vt - 10 : cc.Valeur) ;
                    prevVt = cc.Valeur;
                    m = m*p;
                    n += m;
                }
                //var result = ccs.Sum(cc => cc.CalculatedValue/10*(cc.Valeur%10));
                var result = n;
                Posts.Add(new Post(id, result, vt));
                vt = vt + 10;
            }

        }


        private static void HandleMessage(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var msg in e.NewItems)
            {
                var changeEvent = AgreementEvents.Single(x => x.Id.Equals(((Message)msg).EventId));
                var ccs = CoverCollections.Where(x => x.AgreementId.Equals(changeEvent.AgreementId));
                foreach (var cc in ccs)
                {
                    DoCalculations(cc.Id, changeEvent.ValeurDate);
                }
            }
        }

        public static int NextCalculationTime(int x)
        {
            return (int)Math.Ceiling((x+1) / 10.0) * 10;
        }
    }
}
