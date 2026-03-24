using System;
using System.Collections.Generic;
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
        { Positions.Position_1, (0, 0) }, 
        { Positions.Position_2, (0, -90) }, 
        { Positions.Position_3, (0, 90) },
        { Positions.Position_4, (0, 180) },
        { Positions.Position_5, (-90, 0) },
        { Positions.Position_6, (90, 0) }
    };

    // takes a standard IP address
    public void Initialize(string ipAddress)
    {
        Console.WriteLine($"initializing...attempting to connect to {ipAddress}");
        Connect(ipAddress);

        // move to a home location and set that to absolute zero
        Console.WriteLine("connection successful...running homing sequence");
        FindHome();
        Console.WriteLine("device ready");
    }

    public void FindHome()
    {
        // After this, machine should be able to move 90 degrees
        // each way on the A axis, and 
        _gimbal.GCommand("XQ#HOME");
        while (double.Parse(_gimbal.GCommand("MG _XQ0")) >= 0)
        {
            Thread.Sleep(100); // Don't spam the processor
        }
        SetSpeed();
        return;
    }
    private void SetSpeed()
    {
        _gimbal.GCommand("AC 1000000, 1000000");
        _gimbal.GCommand("SP 100000, 100000");
    }

    // 
    private void Connect(string address)
    {
        // todo: validate ip string before this, throw error if bad string
        string connectionString = $"{address} -direct";  // -direct tells gclib not to look in the Windows Registry and appears to be standard

        try
        {
            _gimbal.GOpen(connectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Connection to {address} failed: {ex.Message}");
        }
    }

    // provided with a position from our positions enum
    // sends a move command, then waits to return until movement is finished
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

        //set timeout to a minute for both axes
        _gimbal.GTimeout((short)30000);
        // Begin motion on both axes
        _gimbal.GCommand("BGA B");

        // Wait for both to finish before returning control to the 3rd party
        _gimbal.GCommand("AMA;AMB; MG \"DONE\"");
    }

    // this needs testing to confirm it returns what mark needs
    public string GetInfo()
    {
        string info = _gimbal.GInfo();
        return info;
    }

    // returns an array of strings
    // each string represents one Galil Ethernet controller, PCI controller, or COM port controller
    // returns empty array on error. 
    public string[] ScanNetwork ()
    {
        return _gimbal.GAddresses();
    }

    // we use double for degrees for precision
    // convert to an int once it becomes a count
    private static int DegreesToCounts(double degrees)
    {
        return (int)(Math.Round(degrees * COUNTS_PER_DEGREE));
    }
}
