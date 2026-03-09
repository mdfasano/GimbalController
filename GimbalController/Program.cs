using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static gclib;
using GimbalController;

System.Diagnostics.Debug.WriteLine("testing1111");
// Swap to 'new RealGimbal()' when you are on-site
IGimbalController gimbal = new RealGimbal();

System.Diagnostics.Debug.WriteLine("Starting Galil Test...");

try 
{
    gimbal.Connect("192.168.1.5 -direct");
    
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
