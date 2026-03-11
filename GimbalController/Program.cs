using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static gclib;
using GimbalController;

Console.WriteLine("Starting Galil Test...");

// this is written to be a canned demonstration of the movement/control of the gimbal machine
// it is not representative of the final product

GController gimbal = new();
try
{
    string info;

    string []foundAddresses = gimbal.ScanNetwork();
    if (foundAddresses.Length > 0)
    {
        Console.Write("\nEnter the number of the device you want to use: ");
        string input = Console.ReadLine() ?? string.Empty;

        if (int.TryParse(input, out int choice) && choice >= 0 && choice < foundAddresses.Length)
        {
            Console.WriteLine(choice);
            // extract string from address list
            string rawSelection = foundAddresses[choice];
            string finalAddress;

            // Check if the string contains "COM" (case-insensitive)
            if (rawSelection.Contains("COM", StringComparison.OrdinalIgnoreCase))
            {
                // For COM ports, we need: "COMn 19200 -direct"
                // We trim it just in case there are trailing spaces from the scan
                finalAddress = $"{rawSelection.Trim()} 19200";
            }
            else
            {
                // Galil GAddresses returns "IP, Model, Serial". We only need the IP for GOpen.
                finalAddress = rawSelection.Split(',')[0].Trim();
            }

            Console.WriteLine($"Selected: {finalAddress}");
            gimbal.Connect(finalAddress);

            // get positioning info
            info = gimbal.SendCommand("TP"); // TP = tell position
            Console.WriteLine($"Current position: {info}");

            // rotate about axis A and back
            gimbal.RotateRelative('A', 200000);
            info = gimbal.SendCommand("TP");
            Console.WriteLine($"Current position: {info}");
            gimbal.RotateRelative('A', -200000);
            info = gimbal.SendCommand("TP");
            Console.WriteLine($"Current position: {info}");

            // rotate about axis B and back
            gimbal.RotateRelative('B', 200000);
            info = gimbal.SendCommand("TP");
            Console.WriteLine($"Current position: {info}");
            gimbal.RotateRelative('B', -200000);
            info = gimbal.SendCommand("TP");
            Console.WriteLine($"Current position: {info}");
        }
    }
    else
    {
        Console.WriteLine("No controllers found on the network.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Critical Error: {ex.Message}");
}
finally
{
    gimbal.Disconnect();
    Console.WriteLine("\n========================================");
    Console.WriteLine("Execution ended. Press any key to close.");
    Console.ReadKey();
}