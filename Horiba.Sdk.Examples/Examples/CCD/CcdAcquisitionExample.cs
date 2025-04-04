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

namespace Horiba.Sdk.Examples.CcdExamples
{
   public class CcdAcquisitionExample
   {
       public static async Task MainAsync()
       {
           DeviceManager Dm = new DeviceManager();
           await Dm.StartAsync();
   
           if (!Dm.ChargedCoupledDevices.Any())
           {
               Log.Error("Required CCD not found");
               await Dm.StopAsync();
               return;
           }
   
           var Ccd = Dm.ChargedCoupledDevices.First();
           await Ccd.OpenConnectionAsync();
           await WaitForCcdAsync(Ccd);
           
   
           try
           {
               
               // CCD configuration
               await Ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
               await Ccd.SetExposureTimeAsync(100);
               await Ccd.SetGainAsync(0); // High Light
               await Ccd.SetSpeedAsync(2); // 1 MHz Ultra
               
               await Ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
               await Ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Image, 1);
   
               var ccdConfiguration = await Ccd.GetDeviceConfigurationAsync();
               var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
               var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);
               await Ccd.SetRegionOfInterestAsync(new RegionOfInterest(1, 0, 0, chipX, chipY, 1, chipY));
               
               if (await Ccd.GetAcquisitionReadyAsync())
               {
                   await Ccd.AcquisitionStartAsync(true);
                   await Task.Delay(1000); // Wait a short period for the acquisition to start
                   await WaitForCcdAsync(Ccd);
   
                   var rawData = await Ccd.GetAcquisitionDataAsync();
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
               await Ccd.CloseConnectionAsync();
               await Task.Delay(1000);
               await Dm.StopAsync();
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