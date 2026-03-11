using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static gclib;
using GimbalController;
using System.Text.RegularExpressions;

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

        if (int.TryParse(input, out int choice) && choice > 0 && choice <= foundAddresses.Length)
        {
            // extract string from address list
            string selectedFullString = foundAddresses[choice];

            // Galil GAddresses returns "IP, Model, Serial". We only need the IP for GOpen.
            string selectedIp = selectedFullString.Split(',')[0].Trim();

            Console.WriteLine($"Selected: {selectedIp}");
            gimbal.Connect($"{selectedIp} -direct");
        }

        // Example: Tell Information
        info = gimbal.SendCommand("TP"); // TP = tell position
        Console.WriteLine($"Controller says: {info}"); // need to do some string parsing here, I think response isnt legible

        // rotate about axis A and back
        //gimbal.RotateRelative('A', 200000);
        info = gimbal.SendCommand("TP"); // prove we are connected

        gimbal.RotateRelative('A', -200000);
        Console.WriteLine($"Controller says: {info}"); // need to do some string parsing here, I think response isnt legible


        // rotate about axis B and back
        //gimbal.RotateRelative('B', 200000);
        //gimbal.RotateRelative('B', -200000);
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

bool IsValidAddress(string addr)
{
    // Regex for IPv4: Checks for 4 groups of 1-3 digits separated by dots
    string ipPattern = @"^(\d{1,3}\.){3}\d{1,3}";

    // Regex for COM: Checks for "COM" + digits + space + baud rate digits
    // Example: "COM3 115200"
    string comPattern = @"^COM\d+\s+\d+";

    if (Regex.IsMatch(addr, ipPattern))
    {
        Console.WriteLine("Format detected: IPv4");
        return true;
    }

    if (Regex.IsMatch(addr, comPattern))
    {
        Console.WriteLine("Format detected: Serial (COM)");
        return true;
    }

    return false;
}