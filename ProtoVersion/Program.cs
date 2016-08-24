using System;
using System.Diagnostics;

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
                var agr = engine.CreateAgreement(new[] {1, 2, 3, 4});
                Console.WriteLine($"New Agreeemnt: {agr}");
                Console.WriteLine();

                var evt = engine.ChangeAgreement(agr.Id, 0, 2, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2) ExitOnKey("FAIL!");
                Console.WriteLine();

                evt = engine.ChangeAgreement(agr.Id, 1, 3, 20);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 2 || agr.Values[1] != 3) ExitOnKey("FAIL!");
                Console.WriteLine();

                evt = engine.ChangeAgreement(agr.Id, 1, 8, 10);
                Console.WriteLine($"Agreement changed: {evt}");
                agr = evt.Apply();
                Console.WriteLine($"Agreement: {agr}");
                if (agr.Values[0] != 1 || agr.Values[1] != 8) ExitOnKey("FAIL!");
                Console.WriteLine();


                //agr = engine.ChangeAgreement(agr.Id, 0, 8, 20);
                //Console.WriteLine($"Agreeemnt changed: {agr}");
                //agr = engine.ChangeAgreement(agr.Id, 2, 6, 20);
                //Console.WriteLine($"Agreeemnt changed: {agr}");
                //agr = engine.ChangeAgreement(agr.Id, 0, 2, 15);
                //Console.WriteLine($"Agreeemnt changed: {agr}");
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
