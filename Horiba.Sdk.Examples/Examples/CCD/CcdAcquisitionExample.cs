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
   public class CcdAcquisitionExample
   {
       public static async Task MainAsync()
       {
           DeviceManager deviceManager = new DeviceManager();
           await deviceManager.StartAsync();
   
           if (!deviceManager.ChargedCoupledDevices.Any())
           {
               Log.Error("Required CCD not found");
               await deviceManager.StopAsync();
               return;
           }
   
           var ccd = deviceManager.ChargedCoupledDevices.First();
           await ccd.OpenConnectionAsync();
           await WaitForCcdAsync(ccd);
           
   
           try
           {
               
               // CCD configuration
               await ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
               await ccd.SetExposureTimeAsync(100);
               await ccd.SetGainAsync(0); // High Light
               await ccd.SetSpeedAsync(2); // 1 MHz Ultra
               
               await ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
               await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Image, 1);
   
               var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
               var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
               var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);
               await ccd.SetRegionOfInterestAsync(new RegionOfInterest(1, 0, 0, chipX, chipY, 1, chipY));
               
               if (await ccd.GetAcquisitionReadyAsync())
               {
                   await ccd.AcquisitionStartAsync(true);
                   await Task.Delay(1000); // Wait a short period for the acquisition to start
                   await WaitForCcdAsync(ccd);
   
                   var rawData = await ccd.GetAcquisitionDataAsync();
                   Log.Information(rawData.ToString());
                   CsvParser.SaveCcdAcquisitionDataToCsv(rawData, "ccd_acquisition_data.csv");
               }
               else
               {
                   throw new Exception("CCD not ready for acquisition");
               }
               
           }
           finally
           {
               await ccd.CloseConnectionAsync();
               await Task.Delay(1000);
               await deviceManager.StopAsync();
           }
       }
   
       private static async Task WaitForCcdAsync(ChargedCoupledDevice ccd)
       {
           bool acquisitionBusy = true;
           while (acquisitionBusy)
           {
               acquisitionBusy = await ccd.GetAcquisitionBusyAsync();
               await Task.Delay(1000);
               Log.Information("Acquisition busy");
           }
       }
       
   } 
    
} 