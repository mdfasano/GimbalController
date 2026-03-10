using System.Net;
using static gclib;

namespace GimbalController;

public class GController
{
    private readonly gclib _controller = new();
    private bool _isConnected = false;

    public void Connect(string ipAddress)
    {
        if (_isConnected) return;

        try
        {
            // Format: "192.168.1.5 -direct"
            // -direct tells gclib not to look in the Windows Registry
            string address = $"{ipAddress} -direct";

            System.Diagnostics.Debug.WriteLine($"Attempting to open Ethernet handle to {address}...");
            _controller.GOpen(address);

            _isConnected = true;
            System.Diagnostics.Debug.WriteLine("Ethernet Link Established.");
        }
        catch (Exception ex)
        {
            _isConnected = false;
            throw new Exception($"Ethernet Connection Failed: {ex.Message}");
        }


    }

    // wrapper for the static GCommand
    // may need a StringBuilder here depending on the wrapper's signature
    public string SendCommand(string command) => _controller.GCommand(command);

    public void Disconnect()
    {
        if (_isConnected)
        {
            _controller.GClose();

            _isConnected = false;
            System.Diagnostics.Debug.WriteLine("Ethernet Link Closed.");
        }
    }

    public void SetZero(char axis)
    {
        // DP (Define Position) sets the current position to a specific value
        _controller.GCommand($"DP{axis}=0");
    }

    public void RotateAbsolute(char axis, int targetPosition, int speed)
    {
        /* uncomment theses if they are needed for execution
        _controller.GCommand($"AC{axis}=100000"); //acceleration
        _controller.GCommand($"DC{axis}=100000"); //deceleration
        _controller.GCommand($"SP{axis}={speed}");//speed
        */

        // PA (Position Absolute) tells the motor exactly where to go on the map
        _controller.GCommand($"PA{axis}={targetPosition}"); 

        // Start motion/Begin
        _controller.GCommand($"BG{axis}");
    }

    // to help determine how to access connected devices
    public void ScanNetwork()
    {
        string[] addresses = _controller.GAddresses();
        System.Diagnostics.Debug.WriteLine("Available Controllers:");
        foreach (var addr in addresses) System.Diagnostics.Debug.WriteLine($" - {addr}");
    }
}