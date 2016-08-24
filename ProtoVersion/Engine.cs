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

        public string ProcessCommand(string line)
        {
            try
            {
                var cmd = line.Contains(" ") ? line.Split(' ') : new[] {line};
                switch (cmd[0].ToLower())
                {
                    case "cra":
                        return CreateAgreement(cmd);
                    case "cha":
                        return ChangeAgreement(cmd);
                    case "da":
                        return DisplayAgreements(cmd);
                    case "crcc":
                        return CreateCoverCollection(cmd);
                    case "dcc":
                        return DisplayCoverCollections(cmd);
                    case "cls":
                        return ClearAll();
                    case "cmd":
                        return DisplayCommands();
                    default:
                        return string.Join(Environment.NewLine, "Unknown Command", DisplayCommands());
                }
            }
            catch (Exception e)
            {
                return string.Join(Environment.NewLine, $"Error in command: {line} ({e.Message})", DisplayCommands());
            }
        }


        public string CreateCoverCollection(string[] cmd)
        {
            var agr = Agreements.Single(x => x.Id.Equals(int.Parse(cmd[1])));
            var cc = new CoverCollection(agr.Id, agr.Values, int.Parse(cmd[2]));
            CoverCollections.Add(cc);
            return cc.ToString();
        }

        public string ClearAll()
        {
            Agreements = new List<Agreement>();
            AgreementEvents = new List<ChangeAgreementEvent>();
            Messages = new List<Message>();
            return "Data purged";
        }

        public ChangeAgreementEvent ChangeAgreement(int agrId, int index, int value, int valeur)
        {
            if (!Agreements.Any(x => x.Id.Equals(agrId)))
            {
                return null;
            }
            var ce = new ChangeAgreementEvent(agrId, index , value, valeur);
            AgreementEvents.Add(ce);
            return ce;
        }

        public string ChangeAgreement(string[] cmd)
        {
            var result = ChangeAgreement(int.Parse(cmd[1]), int.Parse(cmd[2]), int.Parse(cmd[3]), int.Parse(cmd[4]));
            return result?.ToString() ?? $"no Agreement with id {cmd[1]}";
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

        private string CreateAgreement(string[] cmd)
        {
            return CreateAgreement(cmd.Skip(1).ToArray().Select(int.Parse).ToArray()).ToString();
        }

        public string DisplayCoverCollections(string[] cmd)
        {
            if (CoverCollections.Any())
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Time;
                    return CoverCollections.Single(x => x.Id.Equals(int.Parse(cmd[1]))).ToString();
                }
                return string.Join(Environment.NewLine, CoverCollections);
            }
            return "no covercollections in system";
        }

        public string DisplayAgreements(string[] cmd)
        {
            if (Agreements.Any())
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Time;
                    return Agreements.Single(x => x.Id.Equals(int.Parse(cmd[1]))).Get(valeur).ToString();
                }
                return string.Join(Environment.NewLine, Agreements);
            }
            return "no agreements in system";
        }

        public string DisplayCommands()
        {
            var cmds = new[]
            {
                "****************************************************************************",
                "q: quit",
                "cra  [x] .. [X]: Create Agreement with parameter values x .... X",
                "cha [agreementId] [key] [value] [valuer]",
                "da: Display Agreements [agreementId]? [valuer]?",
                "crcc [agrId] [extraValue]: Create CoverCollection from Agreement",
                "****************************************************************************"
            };
            return string.Join(Environment.NewLine, cmds);
        }
    }
}
