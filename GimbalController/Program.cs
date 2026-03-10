using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static gclib;
using GimbalController;

System.Diagnostics.Debug.WriteLine("testing1111");

System.Diagnostics.Debug.WriteLine("Starting Galil Test...");

// this is written to be a canned demonstration of the movement/control of the gimbal machine
// it is not representative of the final product

GController gimbal = new();
try
{
    gimbal.ScanNetwork();
    gimbal.Connect("192.168.1.5 -direct");

    // Example: Tell Information
    string info = gimbal.SendCommand("TI"); // prove we are connected
    System.Diagnostics.Debug.WriteLine($"Controller says: {info}"); // need to do some string parsing here, I think response isnt legible

    // rotate about axis A and back
    gimbal.RotateRelative('A', 5000);
    gimbal.RotateRelative('A', -5000);

    // rotate about axis B and back
    gimbal.RotateRelative('B', 5000);
    gimbal.RotateRelative('B', -5000);
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Critical Error: {ex.Message}");
}
finally
{
    gimbal.Disconnect();
}
