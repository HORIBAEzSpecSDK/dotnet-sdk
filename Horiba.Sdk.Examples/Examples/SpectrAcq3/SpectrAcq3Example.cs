using Horiba.Sdk.Devices;

namespace Horiba.Sdk.Examples.SpectrAcq3;
using Microsoft.Extensions.Logging;


class SpectrAcq3Programm : IExample
{
    /// <summary>
    /// This example initializes the DeviceManager, discovers SpectrAcq3 devices, retrieves their serial numbers,
    /// logs the information, and then closes the connections.
    /// </summary>
    /// <returns></returns>
    public async Task MainAsync(bool showIclConsoleOutput= false)
    {
        // Creating logger 
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        ILogger logger = loggerFactory.CreateLogger<MainProgram>();
        
        //Initialize the DeviceManager
        DeviceManager deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
        try
        {
            // Start the DeviceManager
            await deviceManager.StartAsync();
            
            // Discover SpectrAcq3 devices
            var saqDevices = deviceManager.SpectrAcqDevices;

            // Iterate over discovered SpectrAcq3 devices
            foreach (var spectracq3 in saqDevices)
            {
                // Open the device
                await spectracq3.OpenConnectionAsync();

                // Retrieve and log the serial number
                var serialNumber = await spectracq3.GetSerialNumberAsync();
                logger.LogInformation($"Discovered SpectrAcq3 with Serial Number: {serialNumber}");

                // Close the device
                await spectracq3.CloseConnectionAsync();
            }
        }
        finally
        {
            // Stop the DeviceManager
            await deviceManager.StopAsync();
        }
        
    }
}
