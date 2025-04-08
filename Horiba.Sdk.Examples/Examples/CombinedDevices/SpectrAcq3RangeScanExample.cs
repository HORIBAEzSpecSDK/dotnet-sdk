using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Serilog;
using ScottPlot;

namespace Horiba.Sdk.Examples.CombinedDevices
{
    public class SpectrAcq3RangeScanExample : IExample
    {
        public async Task MainAsync()
        {
            var deviceManager = new DeviceManager();
            await deviceManager.StartAsync();

            if (!deviceManager.SpectrAcqDevices.Any() || !deviceManager.Monochromators.Any())
            {
                Log.Error("Required monochromator or SpectrAcq3 not found");
                await deviceManager.StopAsync();
                return;
            }

            var mono = deviceManager.Monochromators.First();
            await mono.OpenConnectionAsync();
            await WaitForMonoAsync(mono);

            var spectracq3 = deviceManager.SpectrAcqDevices.First();
            await spectracq3.OpenConnectionAsync();

            var startWavelength = 490;
            var endWavelength = 520;
            var incrementWavelength = 3;
            var wavelengths = new List<int>();
            for (int wavelength = startWavelength; wavelength <= endWavelength; wavelength += incrementWavelength)
            {
                wavelengths.Add(wavelength);
            }
            Log.Information($"Wavelengths: {string.Join(", ", wavelengths)}");
            var xData = new List<int>();
            var yDataCurrent = new List<float>();
            var yDataVoltage = new List<float>();
            var yDataCounts = new List<float>();

            try
            {
                await spectracq3.SetAcqSetAsync(1, 0, 1, 0);

                foreach (var wavelength in wavelengths)
                {
                    await mono.MoveToWavelengthAsync(wavelength);
                    while (await mono.IsDeviceBusyAsync())
                    {
                        await Task.Delay(100);
                    }
                    Log.Information($"Monochromator set to {wavelength}nm");

                    await spectracq3.StartAcquisitionAsync(ScanStartMode.TriggerAndInterval);
                    await Task.Delay(3000);
                    var data = await spectracq3.GetAvailableDataAsync();
                    Log.Information($"Acquired data at {wavelength}nm: {data}");
                    xData.Add(wavelength);
                    yDataCurrent.Add(data.Data[0].CurrentSignal.Value);
                    yDataVoltage.Add(data.Data[0].VoltageSignal.Value);
                    yDataCounts.Add(data.Data[0].PpdSignal.Value);
                }
            }
            finally
            {
                await mono.CloseConnectionAsync();
                await spectracq3.CloseConnectionAsync();
                await deviceManager.StopAsync();
            }

            await PlotValuesAsync(startWavelength, endWavelength, xData, yDataCurrent);
        }

        private static async Task PlotValuesAsync(int startWavelength, int endWavelength, List<int> xData, List<float> yData)
        {
            var plt = new ScottPlot.Plot();
            plt.Add.Scatter(xData.Select(x => (double)x).ToArray(), yData.Select(y => (double)y).ToArray());
            plt.Title($"Range Scan {startWavelength}-{endWavelength}[nm] vs. Intensity");
            plt.XLabel("Wavelength");
            plt.YLabel("Intensity");
            var fileName = $"SpectrAcq3RangeScan_{startWavelength}-{endWavelength}.png";
            plt.SavePng(fileName, 400, 300);
            Log.Information($"Range scan plot has been saved as {fileName}");
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
    }
}