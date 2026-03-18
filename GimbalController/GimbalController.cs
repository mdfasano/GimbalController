using System.Net;
using System.Runtime.CompilerServices;
using static gclib;

namespace GimbalController;

public enum Positions
{
    Position_1, // vertical gnd down
    Position_2, // horizontal gnd right
    Position_3, // vertical gnd left
    Position_4, // vertical gnd up
    Position_5, // flat face down
    Position_6  // flat face up
}

public class GimbalController
{
    private gclib _gimbal = new();

    // COUNTS = DEGREES X 10000
    private const double COUNTS_PER_DEGREE = 10000.0;

    // Data structure holding our 6 defined positions
    // Key: The Enum choice, Value: (Axis A counts, Axis B counts)
    private readonly Dictionary<Positions, (double A, double B)> _positions = new()
    {
        // positions are represented here in degrees
        { Positions.Position_1, (90, 0) }, 
        { Positions.Position_2, (90, 90) }, 
        { Positions.Position_3, (90, 270) },
        { Positions.Position_4, (90, 180) },
        { Positions.Position_5, (0, 0) },
        { Positions.Position_6, (180, 0) }
    };

    // takes a standard IP address
    public void Initialize(string ipAddress)
    {
        Connect(ipAddress); // wrap in try/catch block?

        // move to a home location and set that to absolute zero
        FindHome();
        // move to position1 here (unless home works as position1
    }

    // this will move both axes to the reverse limit(RL)
    // it will stop automatically when it hits the limit switches
    // we are using the limit switches to derive absolute positional consistency
    // NOTE: THIS POINT WILL BE ABSOLUTE ZERO, ALL MOVEMENTS WILL BE RELATIVE TO 
    // THESE REVERSE LIMIT POINTS
    private void FindHome()
    {
        //'jog' slowly until we hit the reverse limit on both axes
        _gimbal.GCommand("JGA=-20000; JGB=-20000; BGA B");

        // Poll the limit status bits
        // _RL (Reverse Limit) is 0 when the switch is hit
        while (_gimbal.GCommand("MG _RLA").Trim() == "1.0000" ||
               _gimbal.GCommand("MG _RLB").Trim() == "1.0000")
        {
            Thread.Sleep(50);
        }
        _gimbal.GCommand("ST A B");   // Stop and reset the registers
        _gimbal.GCommand("AMA; AMB"); // Wait for full deceleration

        _gimbal.GCommand("DPA=0; DPB=0"); // define this position as 0
        _gimbal.GCommand("DEA=0; DEB=0"); // todo: learn more about the difference between dp and de commands
    }

    private void Connect(string address)
    {
        // todo: validate ip string before this, throw error if bad string
        string connectionString = $"{address} -direct";  // -direct tells gclib not to look in the Windows Registry and appears to be standard

        try
        {
            _gimbal.GOpen(connectionString);
            _gimbal.GCommand("SH A B"); // "servo here" ensure the motor is on and ready
        }
        catch (Exception ex)
        {
            throw new Exception($"Connection to {address} failed: {ex.Message}");
        }
    }

    public void MoveGimbal(Positions targetPosition)
    {
        if (!_positions.ContainsKey(targetPosition))
            throw new ArgumentException("Invalid position requested.");

        var (degreeA, degreeB) = _positions[targetPosition];
        int countA = DegreesToCounts(degreeA);
        int countB = DegreesToCounts(degreeB);

        // PA = Position Absolute
        // We use absolute moves so "Position 1" is always the same physical spot
        _gimbal.GCommand($"PAA={countA}");
        _gimbal.GCommand($"PAB={countB}");

        // Begin motion on both axes
        _gimbal.GCommand("BGA B");

        // Wait for both to finish before returning control to the 3rd party
        _gimbal.GCommand("AMA;AMB;MG \"Done\"");
    }

    public void ScanNetwork ()
    {

    }

    public void Stop()
    {
        try
        {
            // ST A B stops motion on both axes immediately
            _gimbal.GCommand("ST A B");
            Console.WriteLine("EMERGENCY STOP TRIGGERED");
        }
        catch (Exception ex)
        {
            // Even if the command fails, we want to know
            throw new Exception($"Stop command failed: {ex.Message}");
        }
    }

    // we use double for degrees for precision
    // convert to an int once it becomes a count
    private int DegreesToCounts(double degrees)
    {
        return (int)(Math.Round(degrees * COUNTS_PER_DEGREE));
    }
}
