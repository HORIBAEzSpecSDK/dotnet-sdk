# Getting Started with HORIBA EzSpec SDK

This guide will help you get up and running with the HORIBA EzSpec SDK for .NET.

## Prerequisites

- .NET Framework 4.8 or .NET Standard 2.0+ or .NET 7.0+
- HORIBA EzSpec software installed and running
- Compatible HORIBA instruments (Monochromator, CCD, SpectrAcq)

## Installation

Install the SDK via NuGet Package Manager:

```bash
dotnet add package Horiba.Sdk
```

Or via Package Manager Console:

```powershell
Install-Package Horiba.Sdk
```

## Basic Setup

### 1. Initialize the Device Manager

The `DeviceManager` is the central hub for discovering and managing all HORIBA devices:

```csharp
using Horiba.Sdk.Devices;

// Create and start the device manager
var deviceManager = new DeviceManager();
await deviceManager.StartAsync();
```

### 2. Discover Available Devices

```csharp
// Discover all connected devices
var devices = await deviceManager.DiscoverDevicesAsync();

// Filter by device type
var monochromators = devices.OfType<MonochromatorDevice>();
var ccds = devices.OfType<ChargedCoupledDevice>();
var spectrAcq = devices.OfType<SpectrAcqDevice>();
```

## Working with Monochromators

### Basic Operations

```csharp
// Get the first available monochromator
var mono = devices.OfType<MonochromatorDevice>().FirstOrDefault();
if (mono != null)
{
    // Open connection
    await mono.OpenAsync();
    
    // Initialize the device
    await mono.InitializeAsync();
    
    // Set wavelength to 550nm
    await mono.SetWavelengthAsync(550.0);
    
    // Get current position
    var currentWavelength = await mono.GetCurrentWavelengthAsync();
    Console.WriteLine($"Current wavelength: {currentWavelength} nm");
    
    // Move grating
    await mono.MoveGratingAsync(Grating.First);
    
    // Close connection when done
    await mono.CloseAsync();
}
```

### Advanced Monochromator Control

```csharp
// Configure slits
await mono.MoveSlitAsync(Slit.A, 1.0); // 1mm opening

// Mirror control
await mono.MoveMirrorAsync(Mirror.Entrance, MirrorPosition.Axial);

// Shutter control (if supported)
await mono.OpenShutterAsync(Shutter.First);
await mono.CloseShutterAsync(Shutter.First);
```

## Working with CCDs

### Image Acquisition

```csharp
var ccd = devices.OfType<ChargedCoupledDevice>().FirstOrDefault();
if (ccd != null)
{
    await ccd.OpenAsync();
    
    // Configure acquisition parameters
    await ccd.SetExposureTimeAsync(1000); // 1 second
    await ccd.SetGainAsync(1);
    await ccd.SetAcquisitionCountAsync(1);
    
    // Set region of interest (optional)
    await ccd.SetRegionOfInterestAsync(new RegionOfInterest
    {
        X1 = 0,
        Y1 = 0,
        X2 = 1024,
        Y2 = 1024
    });
    
    // Start acquisition
    await ccd.StartAcquisitionAsync();
    
    // Wait for completion
    while (await ccd.GetAcquisitionBusyAsync())
    {
        await Task.Delay(100);
    }
    
    // Get the data
    var data = await ccd.GetAcquisitionDataAsync();
    
    await ccd.CloseAsync();
}
```

## Working with SpectrAcq

```csharp
var spectrAcq = devices.OfType<SpectrAcqDevice>().FirstOrDefault();
if (spectrAcq != null)
{
    await spectrAcq.OpenAsync();
    
    // Configure measurement
    // Implementation depends on your specific requirements
    
    await spectrAcq.CloseAsync();
}
```

## Error Handling

Always wrap device operations in try-catch blocks to handle communication errors:

```csharp
try
{
    await mono.SetWavelengthAsync(550.0);
}
catch (CommunicationException ex)
{
    Console.WriteLine($"Communication error: {ex.Message}");
    // Handle error appropriately
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Thread Safety

The SDK is designed to be thread-safe. You can safely call methods from multiple threads:

```csharp
// Multiple operations can be performed concurrently
var tasks = new[]
{
    mono.SetWavelengthAsync(500.0),
    ccd.SetExposureTimeAsync(2000),
    // ... other operations
};

await Task.WhenAll(tasks);
```

## Best Practices

1. **Always dispose of resources**: Use `using` statements or call `CloseAsync()` explicitly
2. **Check device status**: Use `IsOpenAsync()` and `IsBusyAsync()` before operations
3. **Handle timeouts**: Set appropriate timeouts for long-running operations
4. **Use async/await**: All SDK operations are asynchronous for better performance
5. **Error handling**: Always wrap operations in try-catch blocks

## Complete Example

```csharp
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;

class Program
{
    static async Task Main(string[] args)
    {
        var deviceManager = new DeviceManager();
        
        try
        {
            await deviceManager.StartAsync();
            var devices = await deviceManager.DiscoverDevicesAsync();
            
            var mono = devices.OfType<MonochromatorDevice>().FirstOrDefault();
            var ccd = devices.OfType<ChargedCoupledDevice>().FirstOrDefault();
            
            if (mono != null && ccd != null)
            {
                // Open devices
                await Task.WhenAll(mono.OpenAsync(), ccd.OpenAsync());
                
                // Set up measurement
                await mono.SetWavelengthAsync(500.0);
                await ccd.SetExposureTimeAsync(1000);
                
                // Perform measurement
                await ccd.StartAcquisitionAsync();
                var data = await ccd.GetAcquisitionDataAsync();
                
                Console.WriteLine($"Acquired {data.Length} data points");
                
                // Clean up
                await Task.WhenAll(mono.CloseAsync(), ccd.CloseAsync());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            await deviceManager.StopAsync();
        }
    }
}
```

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed method documentation
- Check out the Examples project in the repository
- Read about advanced features like data stitching and dark count subtraction
