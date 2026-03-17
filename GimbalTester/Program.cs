using System;
using System.Threading;

using GimbalController; // Ensure this matches your library's namespace

class Program
{
    static void Main(string[] args)
    {
        string controllerIp = "192.168.1.10"; // ensure this is an accurate IP

        GimbalController.GimbalController gimbal = new();

        try
        {
            Console.WriteLine("--- Gimbal Functional Test Starting ---");

            // 1. Initialize (Connects and Homes)
            Console.WriteLine($"Connecting to {controllerIp}...");
            gimbal.Initialize(controllerIp);
            Console.WriteLine("Initialization and Homing Complete.\n");

            // 2. Iterate through all positions in the Enum
            foreach (Positions pos in Enum.GetValues(typeof(Positions)))
            {
                Console.WriteLine($">>> Moving to {pos}...");

                gimbal.MoveGimbal(pos);

                Console.WriteLine($"Reached {pos}. Waiting 5 seconds...");
                Thread.Sleep(5000); // Pause so you can visually verify the position
            }

            Console.WriteLine("\n--- Sequential Test Complete ---");
            Console.WriteLine("Returning to Position_1 (Home)...");
            gimbal.MoveGimbal(Positions.Position_1);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[TEST FAILURE]: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }
}