using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdParallelSpeedExample : IExample
    {
        public async Task MainAsync()
        {
            DeviceManager deviceManager = new DeviceManager();
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

    