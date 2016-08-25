using System;
using System.Diagnostics;
using System.Linq;

namespace ProtoVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine();
            if (args.Length > 0)
                RunConsoleApp(engine);
            else
            {

                //a0: (x,y) = (1,2) valeur=0
                var agr = engine.CreateAgreement(new[] { 1, 2 });
                Console.WriteLine($"New Agreeemnt: {agr}");
                Console.WriteLine();

                //cc1: (x,y) = (1,2) a = 12 valeur=0
                var cc = engine.CreateCoverCollection(agr.Id, 12,0);
                Console.WriteLine($"new CoverCollections: {cc}");
                if (cc.Values[0] != 1 || cc.Values[1] != 2 || cc.A != 12) ExitOnKey("FAIL!");
                //cc2: (x,y) = (1,2) a = 23 valeur=0
                cc = engine.CreateCoverCollection(agr.Id, 23, 0);
                Console.WriteLine($"new CoverCollections: {cc}");
                if (cc.Values[0] != 1 || cc.Values[1] != 2 || cc.A != 23) ExitOnKey("FAIL!");
                Console.WriteLine();


                //e1: x = 2 (valeur=20)
                //a1: (x,y) = (2,2) valuer=20
                var evt = engine.CreateChangeAgreementEvent(agr.Id, 0, 2, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 2) ExitOnKey("FAIL!");
                Console.WriteLine();

                //e2: y = 3 (valeur=20)
                //a2: (x,y) = (2,3) valuer=20
                evt = engine.CreateChangeAgreementEvent(agr.Id, 1, 3, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 3) ExitOnKey("FAIL!");
                Console.WriteLine();

                //cc1: (x,y) = (1,2) a = 7 valeur=0
                cc = engine.CreateCoverCollection(agr.Id, 7, 0);
                Console.WriteLine($"new CoverCollections: {cc}");
                if (cc.Values[0] != 1 || cc.Values[1] != 2 || cc.A != 7) ExitOnKey("FAIL!");
                //cc2: (x,y) = (2,3) a = 11 valeur=0
                cc = engine.CreateCoverCollection(agr.Id, 11, 20);
                Console.WriteLine($"new CoverCollections: {cc}");
                if (cc.Values[0] != 2 || cc.Values[1] != 3 || cc.A != 11) ExitOnKey("FAIL!");
                Console.WriteLine();


                //e3: y = 8 (valeur=10)
                //a3: (x,y) = (1,8) valuer=10
                evt = engine.CreateChangeAgreementEvent(agr.Id, 1, 8, 10);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 1 || agr.Values[1] != 8) ExitOnKey("FAIL!");
                Console.WriteLine();

                //a?: (x,y) = (2,3) valuer=20
                agr = Engine.Agreements.First().Get(20);
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 3) ExitOnKey("FAIL!");
                Console.WriteLine();

                //e4: x = 6 (valeur=20)
                //a4: (x,y) = (6,3) valuer=20
                evt = engine.CreateChangeAgreementEvent(agr.Id, 0, 6, 30);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 6 || agr.Values[1] != 3) ExitOnKey("FAIL!");
                Console.WriteLine();




                ExitOnKey("Press any key to exit");
            }
        }

        private static void ExitOnKey(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }

        private static void RunConsoleApp(Engine engine)
        {
            Console.WriteLine(DisplayCommands());

            string line;
            while ((line = Console.ReadLine()?.Trim()) != "q")
            {
                try
                {
                    if (line.Equals("cls"))
                    {
                        Console.Clear();
                    }
                    Console.WriteLine(ProcessCommand(line));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private static string ProcessCommand(string line)
        {
            try
            {
                var cmd = line.Contains(" ") ? line.Split(' ') : new[] { line };
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
                        return Engine.ClearAll();
                    case "cmd":
                        return DisplayCommands();
                    default:
                        return string.Join(Environment.NewLine, "Unknown Command", DisplayCommands());
                }
            }
            catch (Exception e)
            {
                return string.Join(Environment.NewLine, $"Error in command (idiot): {line} ({e.Message})", DisplayCommands());
            }
        }
        public static string CreateCoverCollection(string[] cmd)
        {
            var valeur = cmd.Length > 3 ? int.Parse(cmd[3]) : Engine.Time;
            return new Engine().CreateCoverCollection(int.Parse(cmd[1]), int.Parse(cmd[2]),valeur).ToString();
        }

        private static string CreateAgreement(string[] cmd)
        {
            return new Engine().CreateAgreement(cmd.Skip(1).ToArray().Select(int.Parse).ToArray()).ToString();
        }
        public static string ChangeAgreement(string[] cmd)
        {
            var result = new Engine().CreateChangeAgreementEvent(int.Parse(cmd[1]), int.Parse(cmd[2]), int.Parse(cmd[3]), int.Parse(cmd[4]));
            return result?.ToString() ?? $"no Agreement with id {cmd[1]}";
        }
        public static string DisplayCoverCollections(string[] cmd)
        {
            if (Engine.CoverCollections.Any())
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Engine.Time;
                    return Engine.CoverCollections.Single(x => x.Id.Equals(int.Parse(cmd[1]))).ToString();
                }
                return string.Join(Environment.NewLine, Engine.CoverCollections);
            }
            return "no covercollections in system";
        }

        public static string DisplayAgreements(string[] cmd)
        {
            if (Engine.Agreements.Any())
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Engine.Time;
                    return Engine.Agreements.Single(x => x.Id.Equals(int.Parse(cmd[1]))).Get(valeur).ToString();
                }
                return string.Join(Environment.NewLine, Engine.Agreements);
            }
            return "no agreements in system";
        }

        public static string DisplayCommands()
        {
            var cmds = new[]
            {
                "****************************************************************************",
                "q: quit",
                "cra  [x] .. [X]: Create Agreement with parameter values x .... X",
                "cha [agreementId] [key] [value] [valuer]",
                "da: Display Agreements [agreementId]? [valuer]?",
                "crcc [agrId] [extraValue] [valuer]?: Create CoverCollection from Agreement",
                "****************************************************************************"
            };
            return string.Join(Environment.NewLine, cmds);
        }
    }
}
