using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Examples
{
    public class CcdParallelSpeedExample
    {
        public static async Task MainAsync()
        {
            DeviceManager Dm = new DeviceManager();
            await Dm.StartAsync();
            var Ccd = Dm.ChargedCoupledDevices.First();
            await Ccd.OpenConnectionAsync();



            var currentParallelSpeed = await Ccd.GetParallelSpeedAsync();
            Log.Information($"Current Parallel Speed: {currentParallelSpeed}");
            
            await Ccd.SetParallelSpeedAsync(1);
            var setParallelSpeed = await Ccd.GetParallelSpeedAsync();
            Log.Information($"Set Parallel Speed: {setParallelSpeed}");

    
            await Ccd.CloseConnectionAsync();
            await Dm.StopAsync();
        }
    }
}

