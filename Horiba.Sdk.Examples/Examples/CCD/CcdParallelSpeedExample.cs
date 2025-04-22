using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdParallelSpeedExample : IExample
    {
        public async Task MainAsync(bool showIclConsoleOutput= false)
        {
            Log.Information("This example shows how to set the parallel speed of a CCD. The way this example works can be used to set other parameters as well.");
            DeviceManager deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
            await deviceManager.StartAsync();
            var ccd = deviceManager.ChargedCoupledDevices.First();
            await ccd.OpenConnectionAsync();

            var currentParallelSpeed = await ccd.GetParallelSpeedAsync();
            Log.Information($"Current Parallel Speed: {currentParallelSpeed}");
            
            await ccd.SetParallelSpeedAsync(1);
            var setParallelSpeed = await ccd.GetParallelSpeedAsync();
            Log.Information($"Set Parallel Speed: {setParallelSpeed}");

    
            await ccd.CloseConnectionAsync();
            await deviceManager.StopAsync();
        }
    }
}

    