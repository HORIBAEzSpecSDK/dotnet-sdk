# horiba-dotnet-sdk
This is the c# .NET repository for the Horiba SDK components.

___

⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️

> [!WARNING]  
> This SDK is under development and not yet released.

> [!IMPORTANT]  
> For this .NET code to work, the SDK from Horiba has to be purchased, installed and licensed.
> The code in this repo and the SDK are under development and not yet released for public use!

⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️

___

**📦 Prerequisites**

* .NET Standard or .NET 6+
* ICL.exe installed as part of the Horiba SDK, licensed and activated

## API reference is available at

https://thatstheend.github.io/horiba-dotnet-sdk/docs/api/Horiba.Sdk.html

## ICL Commands Status available at

https://thatstheend.github.io/horiba-dotnet-sdk

# Getting started

This project is built and deployed to [Nuget.org](https://www.nuget.org/packages/Horiba.Sdk/). Installing it in a project can be done by the .NET CLI like so:

```dotnetcli
dotnet add package Horiba.Sdk
```

After having the package referenced in the project, you are ready to start using it.

#### DeviceManager

This is the entry point of the SDK. It is responsible for:

* starting up the ICL process  
* maintaining the WebSocket connection
* discoverty process for different device types

```csh
using var deviceManager = new DeviceManager();
await deviceManager.StartAsync();
```

Note the **using** declaration, this is needed to ensure proper disposal of all resources utilized by the **DeviceManager**

After completion of the **StartAsync()** method, the collections of devices of the **DeviceManager** will be populated with concreat devices

#### Access specific device

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

After establishing connection to a device, you can start invoking the rest of the available commands

#### Interacting with a device

```csh
var ccdConfig = await ccd.GetDeviceConfigurationAsync();
var monoConfig = await mono.GetDeviceConfigurationAsync();
```

Now you are ready to start implementing the functionality that best suites your case.

# How To?

#### Read actual data from CCD

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
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;

var deviceManager = new DeviceManager();
await deviceManager.StartAsync();
var ccd = deviceManager.ChargedCoupledDevices.First();
await ccd.OpenConnectionAsync();

await ccd.SetAcquisitionCountAsync(1);
await ccd.SetExposureTimeAsync(1500);
await ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
await ccd.SetXAxisConversionTypeAsync(ConversionType.None);

Dictionary<string, object> data = [];
if (await ccd.GetAcquisitionReadyAsync())
{
    await ccd.SetAcquisitionStartAsync(true);
    
    // This method will start a polling procedure after 1s initial delay
    // this initial delay is needed to allow the device to start the acquisition.
    // The interval of the polling procedure is set to be 300ms
    // every iteration of the polling procedure will check if the device is busy
    // by sending a request to the device.
    // The method will return when the device is not busy anymore
    await ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(300));
    
    data = await ccd.GetAcquisitionDataAsync();
}
        
var acquisitionRawData = data.GetValueOrDefault("acquisition");
var timestamp = data.GetValueOrDefault("timestamp");
```

The raw data will be in the shape of **Dictionary<string, object>** you will be able to extract the interesting data as per your needs.

However, if you want to just be able to use JSON deserialization functionality to work with typed object, you can take a look at the **Horiba.Sdk.Tests.AcquisitionDescription.cs** class and use it to deserialize the data into.

```csh
var parsedData = JsonConvert.DeserializeObject<List<AcquisitionDescription>>(acquisitionRawData.ToString());
```
