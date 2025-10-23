using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using MathNet.Numerics.Optimization;
using Serilog;

namespace Horiba.Sdk.Examples.CombinedDevices
{
    public class CcdRamanShiftExample : IExample
    {
        public async Task MainAsync(bool showIclConsoleOutput= false)
        {
            Console.Write("Enter the excitation wavelength (float value): ");
            double excitationWavelength;
            while (!double.TryParse(Console.ReadLine(), out excitationWavelength))
            {
                Console.WriteLine("Invalid input. Please enter a valid float value.");
                Console.Write("Enter the excitation wavelength (float value): ");
            }
            var deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
            await deviceManager.StartAsync();

            if (!deviceManager.ChargedCoupledDevices.Any() || !deviceManager.Monochromators.Any())
            {
                Log.Error("Required monochromator or CCD not found");
                await deviceManager.StopAsync();
                return;
            }
            
            var mono = deviceManager.Monochromators.First();
            await mono.OpenConnectionAsync();
            await WaitForMonoAsync(mono);
            
            
            var ccd = deviceManager.ChargedCoupledDevices.First();
            await ccd.OpenConnectionAsync();

            try
            {
                if (!await mono.GetIsInitializedAsync())
                {
                    await mono.HomeAsync();
                    await WaitForMonoAsync(mono);
                }
                await mono.SetTurretGratingAsync(Grating.Second);
                await WaitForMonoAsync(mono);
                
                
                Console.Write("Enter the center wavelength for the monochromator (float value): ");
                float targetWavelength;
                while (!float.TryParse(Console.ReadLine(), out targetWavelength))
                {
                    Console.WriteLine("Invalid input. Please enter a valid float value.");
                    Console.Write("Enter the center wavelength (float value): ");
                }
                await mono.MoveToWavelengthAsync(targetWavelength);
                Log.Information($"Moving to target wavelength {targetWavelength}...");
                await WaitForMonoAsync(mono);
                await mono.SetSlitPositionAsync(Slit.A, (float)0.0);
                await mono.SetMirrorPositionAsync(Mirror.Entrance, MirrorPosition.Axial);
                Log.Information("Setting slit position to A and mirror position to AXIAL...");
                await WaitForMonoAsync(mono);
                var monoWavelength = await mono.GetCurrentWavelengthAsync();
                Log.Information($"Mono wavelength: {monoWavelength}");

                //ccd config

                var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
                var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
                var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);

                await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Spectra_Image, 1);
                RegionOfInterest myRegion = new RegionOfInterest(1, 0, 0, chipX, chipY, 1, chipY);
                await ccd.SetRegionOfInterestAsync(myRegion);

                await ccd.SetCenterWavelengthAsync(mono.DeviceId, monoWavelength);

                await ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);

                await ccd.SetAcquisitionCountAsync(1);

                int exposureTime = 1000;

                await ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
                await ccd.SetExposureTimeAsync(exposureTime);

                await ccd.SetGainAsync(0); //Least sensitive
                await ccd.SetSpeedAsync(0); //Slowest, but least read noise

                var rawData = new CcdData();

                if (await ccd.GetAcquisitionReadyAsync())
                {
                    await ccd.AcquisitionStartAsync(isShutterOpened: true);
                    while (true)
                    {
                        try
                        {
                            await Task.Delay(exposureTime * 2);

                            Log.Information("Trying for data...");

                            rawData = await ccd.GetAcquisitionDataAsync();

                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error: {ex.Message}"); // This error is expected in this case

                            Log.Information("Data not ready yet...");

                        }
                    }

                    Log.Information($"Retrieved wavelengths: {string.Join(", ", rawData.Acquisition[0].Region[0].XData)}");

                    var wavelengths = rawData.Acquisition[0].Region[0].XData.Select(x => (float)x).ToList();
                    var ramanShift = await RamanConvertAsync(wavelengths, excitationWavelength);
                    var ramanShiftData = rawData;
                    ramanShiftData.Acquisition[0].Region[0].XData = ramanShift;

                    Log.Information($"Wavelengths converted to raman shift: {string.Join(", ", ramanShiftData.Acquisition[0].Region[0].XData)}");
                }
            }
            finally
            {
                await ccd.CloseConnectionAsync();
                await deviceManager.StopAsync();
            }
        }
        
        private static async Task WaitForMonoAsync(MonochromatorDevice mono)
        {
            bool monoBusy = true;
            while (monoBusy)
            {
                monoBusy = await mono.IsDeviceBusyAsync();
                await Task.Delay(1000);
                Log.Information("Mono busy...");
            }
        }
        
        private static async Task<List<float>> RamanConvertAsync(List<float> spectrum, double excitationWavelength)
        {
            return await Task.Run(() =>
            {
                var ramanValues = new List<float>();
                foreach (var waveLength in spectrum)
                {
                    var ramanShift = (1 / excitationWavelength - 1 / waveLength) * 1e7;
                    ramanValues.Add((float)ramanShift);
                }
                return ramanValues;
            });
        }
    }
}
