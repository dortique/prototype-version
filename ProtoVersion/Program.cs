using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProtoVersion
{
    class Program
    {
        private static bool write = true;
        static void Main(string[] args)
        {
            var engine = new Engine();
            Console.WindowWidth = Console.LargestWindowWidth - (Console.LargestWindowWidth/10);
            Console.WindowHeight = Console.LargestWindowHeight - (Console.LargestWindowHeight / 10);

            if (args.Length > 0)
                RunConsoleApp();
            else
            {
                //a0: (x,y) = (1,2) valeur=0
                DoStuff(engine);

                Console.WriteLine("all is good");
                //RunConsoleApp(engine);
                ExitOnKey("Press any key to exit");
            }
        }

        private static void DoStuff(Engine engine)
        {
            var agr = engine.CreateAgreement(new Dictionary<string, int> {{"x", 1}, {"y", 2}});
            WriteMaybe($"New Agreeemnt: {agr}");
            WriteMaybe();

            //cc1: (x,y) = (1,2) a = 12 valeur=0
            var cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> {{"a", 12}}, 0);
            WriteMaybe($"new CoverCollections: {cc}");
            if (!CheckCoverCollection(cc, 1, 2, 12, 0)) FailAndExitOnKey("FAIL!");
            //cc2: (x,y) = (1,2) a = 23 valeur=0
            cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> {{"a", 23}}, 0);
            WriteMaybe($"new CoverCollections: {cc}");
            if (!CheckCoverCollection(cc, 1, 2, 23, 0)) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            //e1: x = 2 (valeur=20)
            //a1: (x,y) = (2,2) valuer=20
            var evt = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> {{"x", 2}}, 20);
            WriteMaybe($"Agreement changed: {evt}");
            agr = evt.Apply();
            WriteMaybe($"Agreement: {agr}");
            if (CheckAgreement(agr, 2, 2, 20)) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            //e2: y = 3 (valeur=20)
            //a2: (x,y) = (2,3) valuer=20
            evt = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> {{"y", 3}}, 20);
            WriteMaybe($"Agreement changed: {evt}");
            agr = evt.Apply();
            WriteMaybe($"Agreement: {agr}");
            if (CheckAgreement(agr, 2, 3, 20)) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            //cc3: (x,y) = (1,2) a = 7 valeur=0
            cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> {{"a", 7}}, 0);
            WriteMaybe($"new CoverCollections: {cc}");
            if (!CheckCoverCollection(cc, 1, 2, 7, 0)) ExitOnKey("FAIL!");
            //cc4: (x,y) = (2,3) a = 11 valeur=20
            cc = engine.CreateCoverCollection(agr.Id, new Dictionary<string, int> {{"a", 11}}, 20);
            WriteMaybe($"new CoverCollections: {cc}");
            if (!CheckCoverCollection(cc, 2, 3, 11, 20)) ExitOnKey("FAIL!");
            WriteMaybe();

            var ccs = engine.GetCoverCollections(0);
            if (ccs.Count > 3) ExitOnKey($"FAIL! Too many covercollections (expected 3, actual count is {ccs.Count}");

            //e3: y = 8 (valeur=10)
            //a3: (x,y) = (1,8) valuer=10
            evt = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> {{"y", 8}}, 10);
            WriteMaybe($"Agreement changed: {evt}");
            agr = evt.Apply();
            WriteMaybe($"Agreement: {agr}");
            if (CheckAgreement(agr, 1, 8, 10)) ExitOnKey("FAIL!");
            WriteMaybe();

            //a?: (x,y) = (2,3) valuer=20
            agr = Engine.GetAgreement(1,20);
            WriteMaybe($"Agreement: {agr}");
            if (agr.Values["x"] != 2 || agr.Values["y"] != 3) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            //e4: x = 6 (valeur=20)
            //a4: (x,y) = (6,3) valuer=20
            evt = engine.CreateChangeAgreementEvent(agr.Id, new Dictionary<string, int> {{"x", 6}}, 30);
            WriteMaybe($"Agreement changed: {evt}");
            agr = evt.Apply();
            WriteMaybe($"Agreement: {agr}");
            if (CheckAgreement(agr, 6, 3, 30)) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            agr = Engine.GetAgreement(1, 15);
            WriteMaybe($"Agrement.Get(15): {agr}");
            if (CheckAgreement(agr, 1, 8, 10)) FailAndExitOnKey("FAIL!");
            agr = Engine.GetAgreement(1, 10);
            WriteMaybe($"Agrement.Get(10): {agr}");
            if (CheckAgreement(agr, 1, 8, 10)) FailAndExitOnKey("FAIL!");
            agr = Engine.GetAgreement(1, 5);
            WriteMaybe($"Agrement.Get(5): {agr}");
            if (CheckAgreement(agr, 1, 2, 0)) FailAndExitOnKey("FAIL!");
            WriteMaybe();
            cc = Engine.GetCoverCollection(1,0);
            WriteMaybe($"CoverCollections[0].Get(0): {cc}");
            if (!CheckCoverCollection(cc, 1, 2, 12, 0)) FailAndExitOnKey("FAIL!");

            var ccevt = engine.CreateChangeCoverCollectionEvent(1, new Dictionary<string, int> {{"x", 19}}, 5);
            WriteMaybe($"CoverCollection changed: {ccevt}");
            cc = Engine.GetCoverCollection(1, 5);
            WriteMaybe($"CoverCollections[0].Get(5): {cc}");
            if (!CheckCoverCollection(cc, 19, 2, 12, 5)) FailAndExitOnKey("FAIL!");
            WriteMaybe();

            cc = Engine.GetCoverCollection(1, 10);
            WriteMaybe($"CoverCollections[0].Get(10): {cc}");
            if (!CheckCoverCollection(cc, 19, 8, 12, 10)) FailAndExitOnKey("FAIL!");
            WriteMaybe();
        }

        private static void WriteMaybe(string info = "")
        {
            if (write) Console.WriteLine(info);
        }

        private static bool CheckAgreement(Agreement agr, int x, int y, int valeur)
        {
            return !agr.Values.ContainsKey("x") || !agr.Values.ContainsKey("y") || agr.Values["x"] != x || agr.Values["y"] != y || agr.ValeurDate != valeur;
        }

        private static bool CheckCoverCollection(CoverCollection cc, int x, int y, int a, int valeur)
        {
            return cc.Values.ContainsKey("x") && cc.Values["x"] == x && cc.Values.ContainsKey("y") && cc.Values["y"] == y && cc.Values.ContainsKey("a") && cc.Values["a"] == a && cc.Valeur == valeur;
        }

        private static void ExitOnKey(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }

        private static void FailAndExitOnKey(string msg)
        {
            Console.WriteLine(msg);
            if (!write)
            {
                write = true;
                Engine.ClearAll();
                DoStuff(new Engine());
            }

            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }

        private static void RunConsoleApp()
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
            return new Engine().CreateCoverCollection(int.Parse(cmd[1]), new Dictionary<string, int> { { "a", int.Parse(cmd[2]) } },valeur).ToString();
        }

        private static string CreateAgreement(string[] cmd)
        {
//            return new Engine().CreateAgreement(cmd.Skip(1).Select(int.Parse).ToArray()).ToString();
            return "not implemented";
        }
        public static string ChangeAgreement(string[] cmd)
        {
            //var result = new Engine().CreateChangeAgreementEvent(int.Parse(cmd[1]), int.Parse(cmd[2]), int.Parse(cmd[3]), int.Parse(cmd[4]));
            //return result?.ToString() ?? $"no Agreement with id {cmd[1]}";
            return "not implemented";
        }
        public static string DisplayCoverCollections(string[] cmd)
        {
            if (Engine.CoverCollectionCount > 0)
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Engine.Time;
                    return Engine.GetCoverCollection(int.Parse(cmd[1]), valeur).ToString();
                }
                //return string.Join(Environment.NewLine, Engine.CoverCollections);
            }
            return "no covercollections in system";
        }

        public static string DisplayAgreements(string[] cmd)
        {
            if (Engine.AgreementCount > 0)
            {
                if (cmd.Length > 1)
                {
                    var valeur = cmd.Length > 2 ? int.Parse(cmd[2]) : Engine.Time;
                    return Engine.GetAgreement(int.Parse(cmd[1]),valeur).ToString();
                }
                //return string.Join(Environment.NewLine, Engine.Agreements);
                return "not implemented";
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
