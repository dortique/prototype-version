using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace ProtoVersion
{
    public class CoverCollection
    {
        public int Id { get; set; }
        public int AgreementId { get; private set; }
        public int Valeur { get; private set; }

        public Dictionary<string, int> Values { get; }

        public int CalculatedValue => Values.Sum(x => x.Value);

        public Dictionary<string, int> Bogus(Dictionary<string, int> a, Dictionary<string, int> b)
        {
            var c = a.Concat(b.Where(x => !a.ContainsKey(x.Key)));
            return c.ToDictionary(x => x.Key, y => y.Value);
        }


        public CoverCollection(int agreementId, Dictionary<string, int> values, int valeur, int id = 0)
        {
            Values = values;
            AgreementId = agreementId;
            Valeur = valeur;
            Id = id > 0 ? id : Engine.CoverCollectionCount + 1;
            //Engine.Posts.Add(new Post(A*Values[0], Valeur));
        }

        public override string ToString()
        {
            return $"CC{Id}[{string.Join(" ", Values)} [cal:{CalculatedValue}] [agrId:{AgreementId}] [valeur:{Valeur}]";
        }

        public ICollection<CoverCollection> Get(int valeur1, int valeur2)
        {
            var result = new List<CoverCollection>();
            var cc = Get(valeur1);
            result.Add(cc);

            var agrs = Engine.GetAgreements(cc.AgreementId, valeur1, valeur2).Skip(1); //skipping first agreement version (createevent - which is not implemented yet)
            var events = Engine.CoverCollectionEvents.Where(x => x.CoverCollectionId.Equals(Id) && x.ValeurDate > valeur1 && x.ValeurDate < valeur2).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);
            var allDates = new List<int>(agrs.Select(x => x.ValeurDate));
            allDates.AddRange(events.Select(x => x.ValeurDate));
            foreach (var valeur in allDates.Distinct().OrderBy(o => o))
            {
                cc = Get(valeur);
                result.Add(cc);
            }

            return result;
        }

        private CoverCollection Clone()
        {
            return new CoverCollection(AgreementId, new Dictionary<string, int>(Values), Valeur, Id);
        }


        public CoverCollection Get(int valeur)
        {
            if (Valeur > valeur) return null;

            var evts = Engine.CoverCollectionEvents.Where(x => x.CoverCollectionId.Equals(Id)).OrderBy(o => o.ValeurDate).ThenBy(t => t.RegisterDate);

            var values = new Dictionary<string, int>(Values);

            var result = new CoverCollection(AgreementId, values, Valeur, Id);

            foreach (var evt in evts)
            {
                if (evt.ValeurDate > valeur)
                {
                    break;
                }
                result = ApplyChange(result, evt);
            }
            var agr = Engine.GetAgreement(AgreementId, valeur);
            MergeValues(agr, result);
            if (agr.ValeurDate > result.Valeur) result.Valeur = agr.ValeurDate;


            return result;

        }

        private static void MergeValues(Agreement agr, CoverCollection result)
        {
            foreach (var val in agr.Values)
            {
                if (!result.Values.ContainsKey(val.Key))
                {
                    result.Values.Add(val.Key, val.Value);
                }
            }
        }

        private CoverCollection ApplyChange(CoverCollection result, BaseEvent evt)
        {

            var cc = new CoverCollection(AgreementId, new Dictionary<string, int>(result.Values), evt.ValeurDate)
            {
                Id = Id
            };

            foreach (var change in evt.Changes)
            {
                if (cc.Values.ContainsKey(change.Key))
                {
                    if (evt is ChangeCoverCollectionEvent)
                    {
                        cc.Values[change.Key] = change.Value;
                    }
                }
                else
                {
                    cc.Values.Add(change.Key, change.Value);
                }
            }
            return cc;
        }

        public override bool Equals(object obj)
        {
            var result = obj != null && obj.GetType() == GetType();
            if (!result) return false;
            var cc = (CoverCollection)obj;
            result = cc.AgreementId.Equals(AgreementId) && cc.Id.Equals(Id) && cc.Valeur.Equals(Valeur) &&
                     cc.CalculatedValue.Equals(CalculatedValue) && cc.Values.Count.Equals(Values.Count);
            result = result && !cc.Values.Except(Values).Any();
            return result;
        }

        public ICollection<CoverCollection> GetBranch(int valeur1, int valeur2, DateTime realTime)
        {
            var result = new List<CoverCollection>();
            var ccEvents = Engine.CoverCollectionEvents.Where(x => x.CoverCollectionId.Equals(Id) &&/* x.ValeurDate >= valeur1 &&*/ x.ValeurDate < valeur2 && x.RegisterDate < realTime);
            var all = new List<IHaveValeur>(ccEvents);
            var agrs = Engine.GetAgreementBranch(AgreementId, 0, valeur2, realTime);
            all.AddRange(agrs.Skip(1));

            var ccOnly = this;
            var ccMerged = this.Clone();
            var agreement = agrs.First();
            MergeValues(agreement, ccMerged);
            result.Add(ccMerged);
            foreach (var e in all.OrderBy(o => o.GetValeur())) //todo: should do thenby(registertime)
            {
                if (e is ChangeCoverCollectionEvent)
                {
                    ccOnly = ApplyChange(ccOnly, (ChangeCoverCollectionEvent) e);
                }
                else //assume Agreement "version"
                {
                    agreement = (Agreement) e;
                }
                ccMerged = ccOnly.Clone();
                MergeValues(agreement, ccMerged);
                ccMerged.Valeur = e.GetValeur();
                result.Add(ccMerged);

            }
            return result.Where(x => x.Valeur >= valeur1).ToList();
        }




    }
}
