using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdDarkCountSubtractionExample : IExample
    {
        public async Task MainAsync(bool showIclConsoleOutput= false)
        {
            var deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
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

                var rawDataShutterClosed = new CcdData();
                List<float> intensityDataShutterClosed = new List<float>();
                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(isShutterOpened: false);
                    while (true)
                    {
                        try
                        {
                            await Task.Delay(exposureTime * 2);

                            Log.Information("Trying for data...");

                            rawDataShutterClosed = await ccd.GetAcquisitionDataAsync();

                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error: {ex.Message}"); // This error is expected in this case

                            Log.Information("Data not ready yet...");

                        }
                    }
                    intensityDataShutterClosed = rawDataShutterClosed.Acquisition[0].Region[0].YData[0].Select(y => (float)y).ToList();
                    Log.Information($"Data with closed shutter: {string.Join(", ", intensityDataShutterClosed)}");
                }
                
                List<float> intensityDataShutterOpen = new List<float>();
                var rawDataShutterOpen = new CcdData();
                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(isShutterOpened: true);
                    while (true)
                    {
                        try
                        {
                            await Task.Delay(exposureTime * 2);

                            Log.Information("Trying for data...");

                            rawDataShutterOpen = await ccd.GetAcquisitionDataAsync();

                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error: {ex.Message}"); // This error is expected in this case

                            Log.Information("Data not ready yet...");

                        }
                    }

                    intensityDataShutterOpen = rawDataShutterOpen.Acquisition[0].Region[0].YData[0].Select(y => (float)y).ToList();
                    Log.Information($"Data with open shutter: {string.Join(", ", intensityDataShutterOpen)}");
                }

                var dataWithoutNoise = await SubtractDarkCountAsync(intensityDataShutterOpen, intensityDataShutterClosed);
                var dataSubtracted = rawDataShutterOpen;
                dataSubtracted.Acquisition[0].Region[0].YData[0] = dataWithoutNoise;

                Log.Information($"Dark subtracted data: {string.Join(", ", dataWithoutNoise)}");
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
