using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static gclib;
using GimbalController;

System.Diagnostics.Debug.WriteLine("testing1111");

System.Diagnostics.Debug.WriteLine("Starting Galil Test...");


GController gimbal = new();
try
{
    gimbal.ScanNetwork();
    gimbal.Connect("192.168.1.10");

    // Example: Tell Information
    string info = gimbal.SendCommand("TI");
    System.Diagnostics.Debug.WriteLine($"Controller says: {info}");
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Critical Error: {ex.Message}");
}
finally
{
    gimbal.Disconnect();
}
