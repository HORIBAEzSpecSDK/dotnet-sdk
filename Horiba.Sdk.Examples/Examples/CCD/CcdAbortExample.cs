using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Threading.Tasks;

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

            //ccd config

            var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
            var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
            var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);

            await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Spectra_Image, 1);
            RegionOfInterest myRegion = new RegionOfInterest(1, 0, 0, chipX, chipY, 1, chipY);
            await ccd.SetRegionOfInterestAsync(myRegion);

            await ccd.SetXAxisConversionTypeAsync(ConversionType.None);

            await ccd.SetAcquisitionCountAsync(1);

            int exposureTime = 1000;

            await ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
            await ccd.SetExposureTimeAsync(exposureTime);

            var trigger = new Trigger(TriggerAddress.Input, TriggerEvent.Once, TriggerSignalType.RisingEdge);
            await ccd.SetTriggerInAsync(trigger, true);

            CcdData returnedCcDData = null;
    
            if (await ccd.GetAcquisitionReadyAsync())
            {
                await ccd.AcquisitionStartAsync(isShutterOpened:true);
                while (true)
                {
                    try
                    {
                        await Task.Delay(exposureTime*2); 

                        Log.Information("Trying for data...");

                        var data = await ccd.GetAcquisitionDataAsync();  

                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error: {ex.Message}"); // This error is expected in this case

                        Log.Information("Data not acquired, aborting acquisition...");

                        await ccd.AcquisitionAbortAsync();
                    }
                }
                returnedCcDData = await ccd.GetAcquisitionDataAsync();
            }
            
            Log.Information($"XData when aborted while waiting for a trigger: {string.Join(", ", returnedCcDData.Acquisition[0].Region[0].XData)}");
            Log.Information($"YData when aborted while waiting for a trigger: {string.Join(", ", returnedCcDData.Acquisition[0].Region[0].YData[0])}");

            Log.Information("Restarting CCD to clear trigger commands...");
            await ccd.RestartDeviceAsync();
            Log.Information("Device restarting...");
            await Task.Delay(10000);

            await ccd.SetTriggerInAsync(trigger, false);
    
            await ccd.CloseConnectionAsync();
            await deviceManager.StopAsync();
        }
    }
}

