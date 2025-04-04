using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Newtonsoft.Json.Linq;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdAbortExample
    {
         public static async Task MainAsync()
        {
            DeviceManager deviceManager = new DeviceManager();
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
                    Console.WriteLine("Aborting acquisition...");
                    await ccd.AcquisitionAbortAsync();
                }
                
                
                returnedCcDData = await ccd.GetAcquisitionDataAsync();
            }
            
            Console.WriteLine($"Data when aborted while waiting for a trigger: {returnedCcDData}");
    
            await ccd.CloseConnectionAsync();
            await deviceManager.StopAsync();
        }
    }
}

