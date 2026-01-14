# Introduction

This SDK is created to streamline the configuration and usage of hardware produced by Horiba. This includes ChargetCoupleDevices and Monochromators.
On top of the hardware devices, Horiba develops propriatery communication layer based on WebSocket connection. This layer is encapsulated in a process called ICL. ICL needs to be licensed and installed on a PC which has USB connection to the hardware. Once this is set up and ready, this SDK comes to play.

C# developers can use this SDK to offload the complexity related to establishing and maintaining connection to both the ICL and the hardware devices. This will allow them to focus on building the solution they need from the get go.

---



⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️

> [!IMPORTANT]  
> This SDK is now released. For more information, contact your local HORIBA affiliate office, or visit https://www.horiba.com/int/scientific/products/detail/action/show/Product/ezspec-sdk-6853/. 
> For this dotNET code to work, the SDK from Horiba has to be purchased, installed and licensed.


⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️

---

## Prerequisites

* ICL.exe installed as part of the Horiba SDK, licensed and activated


## Compatibility

The SDK is built to support a wide range of target frameworks via multi-targeting. This means you can use the same NuGet package in both legacy and modern .NET environments:

*  .NET Framework 4.8
*  .NET Standard 2.0 and 2.1
*  .NET 7 and .NET 8

This compatibility is made possible by:

* Conditional dependencies (e.g. different Serilog/WebSocket packages per target)
* Avoidance of language/runtime features exclusive to .NET Core
* Centralized and modular design

# Getting started

This project is built and deployed to [Nuget.org](https://www.nuget.org/packages/Horiba.Sdk/). Installing it in a project can be done by the .NET CLI like so:

```dotnetcli
dotnet add package Horiba.Sdk
```

After having the package referenced in the project, you are ready to start using it.

## DeviceManager

This is the entry point of the SDK. It is responsible for:

* starting up the ICL process
* maintaining the WebSocket connection
* discoverty process for different device types

```csh
using var deviceManager = new DeviceManager();
await deviceManager.StartAsync();
```

Note the **using** declaration, this is needed to ensure proper disposal of all resources utilized by the **DeviceManager**. If the class is not properly disposed, multiple instances of the ICL.exe process might be left running. This can lead to inconsistent communication between available hardware and deviceManager.

After completion of the **StartAsync()** method, the collections of devices of the **DeviceManager** will be populated with concreat devices

## Access specific device

```csh
var ccd = deviceManager.ChargedCoupledDevices.First();
var mono = deviceManager.Monochromators.First();
```

By keeping reference to the concreate device you can access all functionalities it supports. The first step should be to establish the USB connection to the device.
This is done by invoking the **OpenConnectionAsync()** method

```csh
await ccd.OpenConnectionAsync();
await mono.OpenConnectionAsync();
```

If no **CommunicationException** is thrown, a connection will be established.

After establishing connection, you can start invoking the rest of the available commands

## Interacting with a device

```csh
var ccdConfig = await ccd.GetDeviceConfigurationAsync();
var monoConfig = await mono.GetDeviceConfigurationAsync();
```

Now you are ready to start implementing the functionality that best suites your case.

# Example Project Compatibility

The provided example project targets `.NET 8.0` for simplicity and to demonstrate all modern features and integrations (e.g. logging, charting, numerical packages). However, this may not work out of the box for users of legacy platforms like .NET Framework 4.8.

## Porting Example Code to .NET Framework 4.8

To adapt the example code for .NET Framework:

1. **Create a new Console App targeting .NET Framework 4.8** in Visual Studio.
2. **Replace .NET 6+ dependencies** with older or compatible alternatives:

   * Use `ScottPlot 4.x` instead of `5.x`
   * Use `MathNet.Numerics` versions that still support .NET Framework
   * Logging: Ensure you're using `Serilog 2.x`
3. **Replace `using var` declarations** with traditional `try/finally` or explicit `Dispose()` logic.
4. **Ensure you use `async Task Main()` only if supported**, otherwise wrap everything in a synchronous `Main()` and use `.GetAwaiter().GetResult()` to call async methods.

---

## How To?

### Send separate commands to supported devices

The [test project(https://github.com/HORIBAEzSpecSDK/dotnet-sdk/tree/main/Horiba.Sdk.Tests)] demonstrates how commands can be sent to the devices.
You can look around to see more detailed examples.

> NOTE: There are seemingly random delays in the tests. However, they are not random! These are the timeouts that the hardware needs to process the commands. They are set in empirical way so keep in mind that this is not exhausted list of all possible delays.

As a rule of thumb you can use the following pattern to send pairs of commands to the devices:

* send a SET command (e.g. setting a parameter on the device)
* wait for at least 300ms
* send a corresponding GET command

---

### Read actual data from CCD

* Create new ConsoleApplication

```dotnetcli
dotnet new console --framework net8.0
```

* Install the Nuget package

```dotnetcli
dotnet add package Horiba.Sdk
```

* Open the Program.cs file and update the implementation

```csh
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
namespace Example;

public class Example
{
	public static async Task homepageExample()
	{
        using var deviceManager = new DeviceManager();
        await deviceManager.StartAsync();
        var ccd = deviceManager.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var chipSize = await ccd.GetChipSizeAsync();

        await ccd.SetAcquisitionCountAsync(1);
        await ccd.SetExposureTimeAsync(1000);
        await ccd.SetXAxisConversionTypeAsync(ConversionType.None);

        //Set region of interest to full chip based on chip size
        RegionOfInterest ROI = new RegionOfInterest(1, 0, 0, chipSize.Width, chipSize.Height, 1, chipSize.Height);
        await ccd.SetRegionOfInterestAsync(ROI);

        CcdData data = new CcdData();
        AcquisitionDescription details = new AcquisitionDescription();

        if (await ccd.GetAcquisitionReadyAsync())
        {
            await ccd.AcquisitionStartAsync(true);

            await ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(300));

            data = await ccd.GetAcquisitionDataAsync();
        }

        details = data.Acquisition[0];

        Console.WriteLine("The following xData was retrieved");
        foreach (var xData in data.Acquisition[0].Region[0].XData)
        {
            Console.Write($"{xData.ToString()} ");
        }

        Console.WriteLine("\nThe following yData was retrieved");
        foreach (var yData in data.Acquisition[0].Region[0].YData[0])
        {
            Console.Write($"{yData.ToString()} ");
        }
    }
}
```

---

### Use different installation path of the ICL

By default, the SDK will look for the ICL.exe to be present in the following path:

> C:\Program Files\HORIBA Scientific\SDK\icl.exe

In cases where the local installation is in different folder, you can provide full path to the same executable in the constructor of the **DeviceManager** class.

```csh
using var deviceManager = new DeviceManager("C:\Path\To\ICL\icl.exe");
```

This path is used to start the ICL process prior connecting to it. The starting and stopping of this process is managed automatically by the DeviceManager. This is why it needs to be properly disposed. Otherwise, there might be multiple instances of the ICL process left running which will cause issues with the communication.

Fixing such issue boils down to manually stopping all locally running instances of the icl.exe and restarting the DeviceManager.

---

### Use local network to connect to the ICL

The SDK supports connecting to the ICL process over the local network. This can be done by providing the IP address and port of the machine where the ICL process is running.

> NOTE: If you are using this approach, you need to make sure that the ICL process is running on the remote PC prior creating the DeviceManager instance.

```csh
using var deviceManager = new DeviceManager(ipAddress: IPAddress.Parse("192.168.123.123"), port: 1111);
```
## To build the package locally
use the following command:
```dotnetcli
dotnet build -c Release ; dotnet pack -c Release
```

---
