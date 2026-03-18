using System;
using System.Threading.Tasks;
using GimbalController;

class Program
{
    static async Task Main(string[] args)
    {
        string controllerIp = "192.168.1.10";
        GimbalController.GimbalController gimbal = new();

        try
        {
            gimbal.Initialize(controllerIp);

            Console.WriteLine("\n--- Command Console ---");
            Console.WriteLine("  p1 - p6 : Move to Position");
            Console.WriteLine("  ESC     : EMERGENCY STOP");
            Console.WriteLine("  q       : Quit");
            Console.WriteLine("------------------------------");

            bool running = true;
            while (running)
            {
                // 1. Check for the Escape Key at the start of every loop iteration
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        gimbal.Stop();
                        continue;
                    }
                }

                // 2. Handle Commands
                Console.Write("\rCommand: "); // \r keeps the line clean
                string input = Console.ReadLine()?.ToLower().Trim();

                if (string.IsNullOrEmpty(input)) continue;
                if (input == "q") break;

                Positions? target = input switch
                {
                    "p1" => Positions.Position_1,
                    "p2" => Positions.Position_2,
                    "p3" => Positions.Position_3,
                    "p4" => Positions.Position_4,
                    "p5" => Positions.Position_5,
                    "p6" => Positions.Position_6,
                    _ => null
                };

                if (target.HasValue)
                {
                    // Run the move in a Task so the loop can keep 
                    // listening for the ESC key during the transit!
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            gimbal.MoveGimbal(target.Value);
                            Console.WriteLine($"\nArrived at {target.Value}.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nMove error: {ex.Message}");
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal Error: {ex.Message}");
        }
    }
}