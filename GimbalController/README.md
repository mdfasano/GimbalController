### Installing gclib
After downloading the gclib files from [galil](https://www.galil.com/sw/pub/all/doc/global/install/windows/gclib/), I still did not have access to the nuget package needed to make this project work in c#.
It was necessary to download the nuget package directly from [here](https://www.galil.com/sw/pub/dotnet/gclib-dotnet.1.0.0.nupkg).
I placed this file in the galil\gclib\source folder, under a new folder I named dotnet.
I then had to point visual studio's NuGet package manager to that folder as a local package source before I could begin work.

PR 20000
BGA

### Assumptions
1. My program will be provided with an IP address 
	* I can scan the network for compatible devices if needed 
2. We never want to move the controller while movement is already in progress.