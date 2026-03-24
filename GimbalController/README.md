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

### Installing gclib
After downloading the gclib files from [galil](https://www.galil.com/sw/pub/all/doc/global/install/windows/gclib/), I still did not have access to the nuget package needed to make this project work in c#.
It was necessary to download the nuget package directly from [here](https://www.galil.com/sw/pub/dotnet/gclib-dotnet.1.0.0.nupkg).
I placed this file in the galil\gclib\source folder, under a new folder I named dotnet.
I then had to point visual studio's NuGet package manager to that folder as a local package source before I could begin work.
