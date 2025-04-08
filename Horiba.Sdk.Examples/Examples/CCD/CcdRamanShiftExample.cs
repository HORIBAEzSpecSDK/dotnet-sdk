using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Data;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using MathNet.Numerics.Optimization;
using Serilog;

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdRamanShiftExample : IExample
    {
        public async Task MainAsync()
        {
            Console.Write("Enter the excitation wavelength (float value): ");
            double excitationWavelength;
            while (!double.TryParse(Console.ReadLine(), out excitationWavelength))
            {
                Console.WriteLine("Invalid input. Please enter a valid float value.");
                Console.Write("Enter the excitation wavelength (float value): ");
            }
            var acquisitionFormat = AcquisitionFormat.Spectra;
            var deviceManager = new DeviceManager();
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
                
                await ccd.SetAcquisitionCountAsync(1);
                await ccd.SetCenterWavelengthAsync(mono.DeviceId, monoWavelength);
                await ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
                await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Spectra, 1);
                await ccd.SetExposureTimeAsync(new Random().Next(1, 5));
                var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
                var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
                var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);
                await ccd.SetRegionOfInterestAsync(new RegionOfInterest(1,0, 0, chipX, chipY, 1, chipY));

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

                    var rawData = await ccd.GetAcquisitionDataAsync();
                    Log.Information($"Retrieved wavelengths: {string.Join(", ", rawData.Acquisition[0].Region[0].XData)}");

                    var wavelengths = rawData.Acquisition[0].Region[0].XData.Select(x => (float)x).ToList();
                    var ramanShift = await RamanConvertAsync(wavelengths, excitationWavelength);
                    var ramanShiftData =rawData;
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
                    var ramanShift = ((1 / excitationWavelength) - (1 / waveLength)) * 1e7;
                    ramanValues.Add((float)ramanShift);
                }
                return ramanValues;
            });
        }
    }
}