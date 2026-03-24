## Usage

`public void Initialize(string ipAddress)`
- Connects to provided IP, then runs the controller's homing function. 
- returns once the homing is complete

`public string[] ScanNetwork()`
- this can help to find your controller or troubleshoot connection issues
- returns an array of strings
- each string in returned array represents one Galil Ethernet controller, PCI controller, or COM port controller

`public void MoveGimbal(Positions targetPosition)`
- moves to provided position
- returns once the movement is finished
- this library exposes the following enum for use with this function
```
enum Positions {
	Positions.Position_1, // vertical gnd down, used as home
	Positions.Position_2, // horizontal gnd right
	Positions.Position_3, // vertical gnd left
	Positions.Position_4, // vertical gnd up
	Positions.Position_5, // flat face down
	Positions.Position_6  // flat face up
}
```

`public string GetInfo()`
- ***need to confirm this is returning the correct info with mark***

`public void FindHome()`
- rerun the homing function that runs on initialization
- exposed in case some kind of reset is necessary
  - it will be faster to run `MoveGimbal(Positions.Position_1)`
- returns once movement is complete

### Assumptions
1. We never want to move the controller while movement is already in progress.
2. We only need to move to the 6 defined positions
  - those positions will never change and a user does not need to be able to redefine them

### Installing
- nuget package
- The physical machine running the software must have the Galil C++ driver framework installed (gclib.dll or GDK).
  - Client must install the Galil Tools/GDK installer on the host PC so the underlying drivers exist in C:\Program Files (x86)\Galil\gclib\.