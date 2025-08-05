using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdAbortExample : IExample
    {
         public async Task MainAsync(bool showIclConsoleOutput= false)
        {
            DeviceManager deviceManager = new DeviceManager(showIclConsoleOutput: showIclConsoleOutput);
            await deviceManager.StartAsync();
            var ccd = deviceManager.ChargedCoupledDevices.First();
            await ccd.OpenConnectionAsync();
            
            await ccd.SetAcquisitionCountAsync(1);
            await ccd.SetXAxisConversionTypeAsync(ConversionType.None);
            RegionOfInterest myRegion = new RegionOfInterest(1, 0, 0, 16, 4, 1, 4);
            await ccd.SetRegionOfInterestAsync(myRegion);
            await ccd.SetExposureTimeAsync(100);
            var trigger = new Trigger(TriggerAddress.Input, TriggerEvent.Once, TriggerSignalType.RisingEdge);
            await ccd.SetTriggerInAsync(trigger, true);
            CcdData returnedCcDData = null;
    
            if (await ccd.GetAcquisitionReadyAsync())
            {
                await ccd.AcquisitionStartAsync(true);
                while (await ccd.GetAcquisitionBusyAsync())
                {
                    await Task.Delay(300);
                    Log.Information("Aborting acquisition...");
                    await ccd.AcquisitionAbortAsync();
                }
                
                
                returnedCcDData = await ccd.GetAcquisitionDataAsync();
            }
            
            Log.Information($"XData when aborted while waiting for a trigger: {string.Join(", ", returnedCcDData.Acquisition[0].Region[0].XData)}");
            Log.Information($"YData when aborted while waiting for a trigger: {string.Join(", ", returnedCcDData.Acquisition[0].Region[0].YData[0])}");

            Log.Information("Restarting CCD to clear trigger commands...");
            await ccd.RestartDeviceAsync();
            Log.Information("Device restarting...");
            await Task.Delay(10000);
    
            await ccd.CloseConnectionAsync();
            await deviceManager.StopAsync();
        }
    }
}

