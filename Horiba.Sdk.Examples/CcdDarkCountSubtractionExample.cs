using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Examples.CcdExamples
{
    public class CcdDarkCountSubtractionExample
    {
        public static async Task MainAsync()
        {
            var acquisitionFormat = AcquisitionFormat.Image;
            var deviceManager = new DeviceManager();
            await deviceManager.StartAsync();

            if (!deviceManager.ChargedCoupledDevices.Any())
            {
                Log.Error("No CCDs found, exiting...");
                await deviceManager.StopAsync();
                return;
            }

            var ccd = deviceManager.ChargedCoupledDevices.First();
            await ccd.OpenConnectionAsync();

            try
            {
                await ccd.SetAcquisitionCountAsync(1);
                await ccd.SetXAxisConversionTypeAsync(ConversionType.None);
                await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Image, 1);
                Log.Information((await ccd.GetAcquisitionCountAsync()).ToString());
                Log.Information((await ccd.GetCleanCountAsync()).ToString());
                Log.Information((await ccd.GetTimerResolutionAsync()).ToString());
                Log.Information((await ccd.GetGainAsync()).ToString());
                Log.Information((await ccd.GetChipSizeAsync()).ToString());
                Log.Information((await ccd.GetExposureTimeAsync()).ToString());
                await ccd.SetExposureTimeAsync(new Random().Next(1, 5));
                Log.Information((await ccd.GetExposureTimeAsync()).ToString());
                Log.Information((await ccd.GetChipTemperatureAsync()).ToString());
                Log.Information((await ccd.GetSpeedAsync()).ToString());
                
                var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
                var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
                var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);
                await ccd.SetRegionOfInterestAsync(new RegionOfInterest(1,0, 0, chipX, chipY, 1, chipY));

                var rawDataShutterClosed = new CcdData();
                List<float> intensityDataShutterClosed = new List<float>();
                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(false);
                    await Task.Delay(1000); // Wait a short period for the acquisition to start
                    bool acquisitionBusy = true;
                    while (acquisitionBusy)
                    {
                        acquisitionBusy = await ccd.GetAcquisitionBusyAsync();
                        await Task.Delay(300);
                        Log.Information("Acquisition busy");
                    }

                    rawDataShutterClosed = await ccd.GetAcquisitionDataAsync();
                    intensityDataShutterClosed = rawDataShutterClosed.Acquisition[0].Region[0].YData[0].Select(y => (float)y).ToList();
                    Log.Information($"Data with closed shutter: {string.Join(", ", intensityDataShutterClosed)}");
                }
                
                List<float> intensityDataShutterOpen = new List<float>();
                var rawDataShutterOpen = new CcdData();
                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(true);
                    await Task.Delay(1000); // Wait a short period for the acquisition to start
                    bool acquisitionBusy = true;
                    while (acquisitionBusy)
                    {
                        acquisitionBusy = await ccd.GetAcquisitionBusyAsync();
                        await Task.Delay(300);
                        Log.Information("Acquisition busy");
                    }

                    rawDataShutterOpen = await ccd.GetAcquisitionDataAsync();
                    intensityDataShutterOpen = rawDataShutterOpen.Acquisition[0].Region[0].YData[0].Select(y => (float)y).ToList();
                    Log.Information($"Data with open shutter: {string.Join(", ", intensityDataShutterOpen)}");
                }

                var dataWithoutNoise = await SubtractDarkCountAsync(intensityDataShutterOpen, intensityDataShutterClosed);
                var dataSubtracted = rawDataShutterOpen;
                dataSubtracted.Acquisition[0].Region[0].YData[0] = dataWithoutNoise;

                Log.Information($"Data without noise: {string.Join(", ", dataWithoutNoise)}");
            }
            finally
            {
                await ccd.CloseConnectionAsync();
                await deviceManager.StopAsync();
            }
        }

        private static async Task<List<float>> SubtractDarkCountAsync(List<float> shutterOpenData, List<float> shutterClosedData)
        {
            var valuesNoiseFree = new List<float>();
            var zippedData = shutterOpenData.Zip(shutterClosedData, (open, closed) => open - closed);
            valuesNoiseFree.AddRange(zippedData);
            return valuesNoiseFree;
        }
    }
}