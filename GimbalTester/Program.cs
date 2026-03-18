using System;
using GimbalController;

class Program
{
    static void Main(string[] args)
    {
        string controllerIp = "192.168.1.10";
        GimbalController.GimbalController gimbal = new();

        try
        {
            // Connect and set up (Ensure CN -1, -1 is inside your Initialize!)
            gimbal.Initialize(controllerIp);

            Console.WriteLine("\n--- Gimbal Control Console ---");
            Console.WriteLine("  p1 - p6 : Move to Position (Blocks until arrival)");
            Console.WriteLine("  q       : Quit");
            Console.WriteLine("------------------------------");

            while (true)
            {
                Console.Write("\nEnter Command: ");
                string input = Console.ReadLine()?.ToLower().Trim();

                if (string.IsNullOrEmpty(input)) continue;
                if (input == "q") break;

                // Determine target position
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

                    try
                    {
                        // Direct call: The program stays on this line 
                        // until the motor stops at the destination.
                        gimbal.MoveGimbal(target.Value);

                        Console.WriteLine($"SUCCESS: Arrived at {target.Value}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"MOVE FAILED: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Use p1-p6 or q.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FATAL ERROR: {ex.Message}");
        }

        Console.WriteLine("Shutting down...");
    }
}