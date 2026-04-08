using System;
using System.Collections.Generic;
using static gclib;

namespace GimbalController;

public enum Positions
{
    Load,       // 45 degree tilt up
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
    public event EventHandler? OperationCompleted;

    // COUNTS = DEGREES X 10000
    private const double COUNTS_PER_DEGREE = 10000.0;

    // Data structure holding our 6 defined positions
    // Key: The Enum choice, Value: (Axis A counts, Axis B counts)
    private readonly Dictionary<Positions, (double A, double B)> _positions = new()
    {
        // positions are represented here in degrees
        { Positions.Load, (45, 0) },
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
        Connect(ipAddress); // wrap in try/catch block?

        // move to a home location and set that to absolute zero
        Console.WriteLine("connection successful...running homing sequence");
        FindHome();
        Console.WriteLine("device ready"); // this should be a fired event instead
    }

    public void FindHome()
    {
        // After this, machine should be able to move 90 degrees
        // each way on the A axis, and 
        _gimbal.GCommand("XQ#HOME");
        StartMonitoringMotion();
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

        // -direct tells gclib not to look in the Windows Registry and appears to be standard
        // removed -direct after getting errors back from the device when running on lab setup
        // leaving comments here for potential debugging of future connection issues
        string connectionString = $"{address}";  

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

        // convert to a number that the controller understands
        int countA = DegreesToCounts(degreeA);
        int countB = DegreesToCounts(degreeB);

        // PA = Position Absolute
        // We use absolute moves so "Position 1" is always the same physical spot
        _gimbal.GCommand($"PAA={countA}"); // sets axisA target
        _gimbal.GCommand($"PAB={countB}"); // sets axisB target
        SetSpeed();
        _gimbal.GCommand("BGA B"); // begin motion on both A and B
        StartMonitoringMotion();
    }

    // this needs testing to confirm it returns what mark needs
    public string GetInfo()
    {
        string info = _gimbal.GInfo();
        return info;
    }

    // return true if the controller is moving or executing a program
    private bool IsGimbalBusy()
    {
        return IsGimbalMoving() || IsGimbalExecutingHomeProgram();
    }

    // returns true if the controller is moving on any axis
    private bool IsGimbalMoving()
    {
        // MG _BGm returns a 1 (as a string, along with other formatting characters)
        // if axis 'm' is moving, and a 0 if not.
        string axisAResponse = _gimbal.GCommand("MG _BGA");
        string axisBResponse = _gimbal.GCommand("MG _BGB");

        // response will either be 0.00 or 1.00, so we just 
        // need to check if > 0 to determine truthiness
        bool isAMoving = double.Parse(axisAResponse) > 0;
        bool isBMoving = double.Parse(axisBResponse) > 0;

        // return 0 iff both A and B axes are not moving
        return  isAMoving || isBMoving;
    }

    // returns true if controller is executing its home program
    private bool IsGimbalExecutingHomeProgram()
    {
        string responseXQ = _gimbal.GCommand("MG _XQ0");

        // MG _XQ returns -1.000 when it is done
        // returns 0 or a positive integer representing the program line
        // it is executing otherwise
        bool isGoingHome = double.Parse(responseXQ) >= 0;
        return isGoingHome;
    }

    // returns an array of strings
    // each string represents one Galil Ethernet controller, PCI controller, or COM port controller
    // returns empty array on error. 
    public string[] ScanNetwork ()
    {
        return _gimbal.GAddresses();
    }

    // interrupts all movement.
    // need to change implementation for movement to make this work
    // maybe instead of waiting for movement to finish before returning from movement function,
    // I could return immediatedly, but before sending any command I could run a check and if movment
    // is currently in progress return an error
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
    private static int DegreesToCounts(double degrees)
    {
        return (int)(Math.Round(degrees * COUNTS_PER_DEGREE));
    }

    // Helper to trigger event
    protected virtual void OnOperationCompleted()
    {
        OperationCompleted?.Invoke(this, EventArgs.Empty);
    }

    // spins up a thread that fires an event once activity is finished
    private void StartMonitoringMotion()
    {
        // this thread allows us to return control to parent program while
        // we wait for motion to finish
        Thread monitorThread = new(() =>
        {
            try
            {
                Thread.Sleep(200);  // Wait for "Begin" command to process
                while (IsGimbalBusy()) // Sleep thread until controller is not doing something
                {
                    Thread.Sleep(100);
                }
                OnOperationCompleted(); // event fires once we detect no motion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Background polling failed: {ex.Message}");
            }
        });
        monitorThread.IsBackground = true;
        monitorThread.Start();
    }
}
