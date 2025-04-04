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

namespace Horiba.Sdk.Examples.Ccd
{
    public class CcdRangeScanExample
    {
        public static async Task MainAsync()
        {
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
            await WaitForCcdAsync(ccd);

            var startWavelength = 400;
            var endWavelength = 600;
            var spectrum = new List<List<float>> { new List<float> { 0 }, new List<float> { 0 } };

            try
            {
                // Mono configuration
                if (!await mono.GetIsInitializedAsync())
                {
                    await mono.HomeAsync();
                    await WaitForMonoAsync(mono);
                }
                await mono.SetTurretGratingAsync(Grating.Second);
                await WaitForMonoAsync(mono);
                await mono.SetMirrorPositionAsync(Mirror.Entrance, MirrorPosition.Axial);
                await WaitForMonoAsync(mono);
                await mono.SetSlitPositionAsync(Slit.A, (float)0.5);
                await WaitForMonoAsync(mono);

                // CCD configuration
                await ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
                await ccd.SetExposureTimeAsync(100);
                await ccd.SetGainAsync(0); // High Light
                await ccd.SetSpeedAsync(2); // 1 MHz Ultra

                // Set center wavelength before setting x-axis conversion type
                await ccd.SetCenterWavelengthAsync(mono.DeviceId, 0);
                await ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
                await ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Image, 1);

                var ccdConfiguration = await ccd.GetDeviceConfigurationAsync();
                var chipX = Convert.ToInt32(ccdConfiguration["chipWidth"]);
                var chipY = Convert.ToInt32(ccdConfiguration["chipHeight"]);
                await ccd.SetRegionOfInterestAsync(new RegionOfInterest(1,0, 0, chipX, chipY, 1, chipY));

                var centerWavelengths = await ccd.CalculateRangePositionsAsync(mono.DeviceId, startWavelength, endWavelength, 100);
                Log.Information($"Number of captures: {centerWavelengths.Count()}");

                System.IO.File.WriteAllText("centerwavelengths.txt", string.Join(", ", centerWavelengths));

                var captures = new List<List<List<float>>>();
                foreach (var centerWavelength in centerWavelengths)
                {
                    await mono.MoveToWavelengthAsync(centerWavelength.WavelengthValue);
                    await WaitForMonoAsync(mono);
                    var monoWavelength = await mono.GetCurrentWavelengthAsync();
                    Log.Information($"Mono wavelength {monoWavelength}");

                    await ccd.SetCenterWavelengthAsync(mono.DeviceId, monoWavelength);

                    var xyData = await CaptureAsync(ccd);
                    Log.Debug($"Capture data structure: {xyData}");
                    captures.Add(xyData);
                }

                Log.Debug($"All captures before stitching: {captures}");
                var stitch = new LinearSpectraStitch(captures);
                spectrum = stitch.StitchedSpectra();
                var filteredSpectrum = await FilterValuesAsync(startWavelength, endWavelength, spectrum[0], spectrum[1]);
                System.IO.File.WriteAllText("plot_values.txt", string.Join(", ", spectrum.SelectMany(x => x)));

                await PlotValuesAsync(startWavelength, endWavelength, filteredSpectrum);
            }
            finally
            {
                await ccd.CloseConnectionAsync();
                Log.Information("Waiting before closing Monochromator");
                await Task.Delay(1000);
                await mono.CloseConnectionAsync();
                await deviceManager.StopAsync();
            }
        }

        private static async Task<List<List<float>>> CaptureAsync(ChargedCoupledDevice ccd)
        {
            var xyData = new List<List<float>> { new List<float> { 0 }, new List<float> { 0 } };
            if (await ccd.GetAcquisitionReadyAsync())
            {
                await ccd.AcquisitionStartAsync(true);
                await Task.Delay(1000); // Wait a short period for the acquisition to start
                await WaitForCcdAsync(ccd);

                var rawData = await ccd.GetAcquisitionDataAsync();
                Log.Information(rawData.ToString());
                xyData = new List<List<float>>
                {
                    rawData.Acquisition[0].Region[0].XData.Select(x => (float)x).ToList(),
                    rawData.Acquisition[0].Region[0].YData[0].Select(y => (float)y).ToList()
                };
                Log.Information(xyData.ToString());
            }
            else
            {
                throw new Exception("CCD not ready for acquisition");
            }
            return xyData;
        }

        private static async Task<List<List<float>>> FilterValuesAsync(double startWavelength, double endWavelength, List<float> wavelengthValues, List<float> intensityValues)
        {
            var filteredWavelengths = new List<float>();
            var filteredIntensities = new List<float>();
            for (var i = 0; i < wavelengthValues.Count; i++)
            {
                if (startWavelength <= wavelengthValues[i] && wavelengthValues[i] <= endWavelength)
                {
                    filteredWavelengths.Add(wavelengthValues[i]);
                    filteredIntensities.Add(intensityValues[i]);
                }
            }
            return new List<List<float>> { filteredWavelengths, filteredIntensities };
        }

        private static async Task PlotValuesAsync(double startWavelength, double endWavelength, List<List<float>> xyData)
        {
            var xValues = xyData[0];
            var yValues = xyData[1];

            // Plotting the data
            var plt = new ScottPlot.Plot();
            plt.Add.Scatter(xValues.ToArray(), yValues.ToArray());
            plt.Title($"Range Scan {startWavelength}-{endWavelength}[nm] vs. Intensity");
            plt.XLabel("Wavelength");
            plt.YLabel("Intensity");
            plt.SavePng($"ccdRangeScanPlot_{startWavelength}_{endWavelength}_nm.png", 400, 300);
        }

        private static async Task WaitForCcdAsync(ChargedCoupledDevice ccd)
        {
            var acquisitionBusy = true;
            while (acquisitionBusy)
            {
                acquisitionBusy = await ccd.GetAcquisitionBusyAsync();
                await Task.Delay(1000);
                Log.Information("Acquisition busy");
            }
        }

        private static async Task WaitForMonoAsync(MonochromatorDevice mono)
        {
            var monoBusy = true;
            while (monoBusy)
            {
                monoBusy = await mono.IsDeviceBusyAsync();
                await Task.Delay(1000);
                Log.Information("Mono busy...");
            }
        }
    }
}