using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using Serilog;
using ScottPlot;
using HelperFunctions;

namespace Horiba.Sdk.Examples.Ccd
{
   public class CcdAcquisitionExample : IExample
   {
       public async Task MainAsync(bool showIclConsoleOutput= false)
       {
           DeviceManager deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
           await deviceManager.StartAsync();
   
           if (!deviceManager.ChargedCoupledDevices.Any())
           {
               Log.Error("Required CCD not found");
               await deviceManager.StopAsync();
               return;
           }
   
           var ccd = deviceManager.ChargedCoupledDevices.First();
           await ccd.OpenConnectionAsync();
              
           try
           {

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

                await ccd.SetGainAsync(0); //Least sensitive
                await ccd.SetSpeedAsync(0); //Slowest, but least read noise


                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(isShutterOpened: true);
                    while (true)
                    {
                        try
                        {
                            await Task.Delay(exposureTime * 2);

                            Log.Information("Trying for data...");

                            var data = await ccd.GetAcquisitionDataAsync();

                            Log.Information(data.ToString());
                            CsvParser.SaveCcdAcquisitionDataToCsv(data, "ccd_acquisition_data.csv");
                            Log.Information("Saved data to ccd_acquisition_data.csv");

                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error: {ex.Message}"); // This error is expected in this case

                            Log.Information("Data not ready yet...");

                        }
                    }
                }
            }
           finally
           {
               await ccd.CloseConnectionAsync();
               await Task.Delay(1000);
               await deviceManager.StopAsync();
           }
       }
       
   } 
    
} 
