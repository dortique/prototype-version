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
                var agr = engine.CreateAgreement(new[] {1, 2});
                Console.WriteLine($"New Agreeemnt: {agr}");
                Console.WriteLine();

                //e1: x = 2 (valeur=20)
                //a1: (x,y) = (2,2) valuer=20
                var evt = engine.ChangeAgreement(agr.Id, 0, 2, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 2) ExitOnKey("FAIL!");
                Console.WriteLine();

                //e2: y = 3 (valeur=20)
                //a2: (x,y) = (2,3) valuer=20
                evt = engine.ChangeAgreement(agr.Id, 1, 3, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 3) ExitOnKey("FAIL!");
                Console.WriteLine();

                //e3: y = 8 (valeur=10)
                //a3: (x,y) = (1,8) valuer=10
                evt = engine.ChangeAgreement(agr.Id, 1, 8, 10);
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
                evt = engine.ChangeAgreement(agr.Id, 0, 6, 30);
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
            Console.WriteLine(engine.DisplayCommands());

            string line;
            while ((line = Console.ReadLine()?.Trim()) != "q")
            {
                try
                {
                    if (line.Equals("cls"))
                    {
                        Console.Clear();
                    }
                    Console.WriteLine(engine.ProcessCommand(line));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
