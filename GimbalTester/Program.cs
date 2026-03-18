using System;
using GimbalController;

class Program
{
    static void Main(string[] args)
    {
        string controllerIp = "192.168.1.10"; // Update to your IP
        GimbalController.GimbalController gimbal = new();

        try
        {
            Console.WriteLine("--- Interactive Terminal ---");
            Console.WriteLine($"Connecting to {controllerIp}...");

            gimbal.Initialize(controllerIp);

            Console.WriteLine("\nControls:");
            Console.WriteLine("  p1, p2, p3, p4, p5, p6 : Move to Position n");
            Console.WriteLine("  q : Quit");
            Console.WriteLine("----------------------------------");

            while (true)
            {
                Console.Write("\nEnter Command: ");
                string input = Console.ReadLine()?.ToLower().Trim();

                if (input == "q") break;

                // Map string input to the Enum
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
                    Console.WriteLine($"Moving to {target.Value}...");
                    gimbal.MoveGimbal(target.Value);
                    Console.WriteLine("Movement Complete.");
                }
                else
                {
                    Console.WriteLine("Invalid command. Use p1-p6, or q.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[FATAL ERROR]: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("Shutting down...");
    }
}