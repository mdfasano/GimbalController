using System.Net;
using static gclib;

namespace GimbalController;

public class GController
{
    gclib _controller = new();
    private bool _isConnected = false;
    readonly int defaultSpeed = 100000;

    public void Connect(string addr)
    {
        if (_isConnected) return;

        try
        {
            // Format: "192.168.1.5 -direct"
            // -direct tells gclib not to look in the Windows Registry
            string address = $"{addr} -direct";

            Console.WriteLine($"Attempting to open connection to {address}...");
            _controller.GOpen(address);

            _isConnected = true;
            Console.WriteLine("Connection Established.");
        }
        catch (Exception ex)
        {
            _isConnected = false;
            throw new Exception($"Connection Failed: {ex.Message}");
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
            Console.WriteLine("Ethernet Link Closed.");
        }
    }

    public void SetZero(char axis)
    {
        // DP (Define Position) sets the current position to a specific value
        _controller.GCommand($"DP{axis}=0");
    }

    // we hard-code speed and acceleration into this function so the user does not have to worry about them
    // while we maintain consistency between operations
    public void RotateRelative(char axis, int counts)
    {
        //_controller.GCommand($"AC{axis}=100000");         // Acceleration
        //_controller.GCommand($"DC{axis}=100000");         // Deceleration
        _controller.GCommand($"SP{axis}={defaultSpeed}");   // Speed
        _controller.GCommand($"PR{axis}={counts}");         // Distance 
        _controller.GCommand($"BG{axis}");                  // Begin

        // This line SHOULD tells the C# code to pause until the controller 
        // confirms the move on that specific axis is complete.
        _controller.GCommand($"AM{axis};MG \"Done\"");

        // I think this can be removed, I added it to ensure the motor is 
        // settled after each move
        _controller.GCommand("WT 1000"); // 1 second
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
    public string[] ScanNetwork()
    {
        string[] addresses = _controller.GAddresses();
        Console.WriteLine("Available Controllers:");
        for (int i = 0; i < addresses.Length; i++)
            Console.WriteLine($"({i}) {addresses[i]}");
        return addresses;
    }

}