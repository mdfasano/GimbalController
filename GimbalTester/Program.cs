using GimbalController;
using Microsoft.VisualBasic;
using System;

class Program
{
    // A flag to keep track of gimbal status in the console UI
    static bool _isMoving = false;

    static void Main(string[] args)
    {
        string controllerIp = "192.168.1.10";
        GimbalController.GimbalController gimbal = new();

        // --- Event Subscription ---
        // tests threading/monitoring logic
        gimbal.OperationCompleted += (sender, e) =>
        {
            _isMoving = false;
            Console.WriteLine("\n[EVENT] Movement/Homing Complete. Gimbal is now idle.");
            Console.Write("Enter Command: "); // Re-print prompt for UX
        };

        try
        {
            gimbal.Initialize(controllerIp);

            ShowMenu();

            while (true)
            {
                Console.Write("\nEnter Command: ");
                string input = Console.ReadLine()?.ToLower().Trim();

                if (string.IsNullOrEmpty(input)) continue;
                if (input == "q") break;

                switch (input)
                {
                    case "p1":
                    case "p2":
                    case "p3":
                    case "p4":
                    case "p5":
                    case "p6":
                        HandleMove(gimbal, input);
                        break;

                    case "home":
                        Console.WriteLine("Executing Home Sequence...");
                        _isMoving = true;
                        gimbal.FindHome();
                        break;

                    case "stop":
                        gimbal.Stop();
                        _isMoving = false;
                        Console.WriteLine("Emergency Stop Sent.");
                        break;

                    case "info":
                        Console.WriteLine("--- Device Info ---");
                        Console.WriteLine(gimbal.GetInfo());
                        break;

                    case "scan":
                        Console.WriteLine("Scanning Network for Controllers...");
                        var addresses = gimbal.ScanNetwork();
                        if (addresses.Length == 0) Console.WriteLine("No controllers found.");
                        foreach (var addr in addresses) Console.WriteLine($" Found: {addr}");
                        break;

                    case "help":
                        ShowMenu();
                        break;

                    default:
                        Console.WriteLine("Unknown command. Type 'help' for options.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FATAL ERROR: {ex.Message}");
        }

        Console.WriteLine("Shutting down...");
    }
    static void ShowMenu()
    {
        Console.WriteLine("\n--- Gimbal Control Console ---");
        Console.WriteLine("  p1 - p6 : Move to Position (Async/Threaded)");
        Console.WriteLine("  home    : Run homing sequence");
        Console.WriteLine("  stop    : Emergency Stop");
        Console.WriteLine("  info    : Get controller info");
        Console.WriteLine("  scan    : Scan network for Galil devices");
        Console.WriteLine("  q       : Quit");
        Console.WriteLine("------------------------------");
    }
    static void HandleMove(GimbalController.GimbalController gimbal, string input)
    {
        if (_isMoving)
        {
            Console.WriteLine("Wait! Gimbal is currently busy.");
            return;
        }

        Positions target = input switch
        {
            "p1" => Positions.Position_1,
            "p2" => Positions.Position_2,
            "p3" => Positions.Position_3,
            "p4" => Positions.Position_4,
            "p5" => Positions.Position_5,
            "p6" => Positions.Position_6,
            _ => Positions.Position_1
        };

        Console.WriteLine($"Commanding move to {target}...");
        _isMoving = true;

        // This returns immediately. The "SUCCESS" message in your 
        // old code was misleading because the motor hadn't finished yet.
        gimbal.MoveGimbal(target);
    }
}