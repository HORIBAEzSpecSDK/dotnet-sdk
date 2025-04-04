using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Horiba.Sdk.Devices;
using Horiba.Sdk.Enums;
using Newtonsoft.Json.Linq;
using Serilog;
using HelperFunctions;

namespace Horiba.Sdk.Examples.SpectrAcq3
{
    public class SpectrAcq3AcquisitionExample
    {
        public static async Task MainAsync()
        {
            var deviceManager = new DeviceManager();
            await deviceManager.StartAsync();

            if (!deviceManager.Monochromators.Any())
            {
                Log.Error("No monochromators found, exiting...");
                await deviceManager.StopAsync();
                return;
            }

            if (!deviceManager.SpectrAcqDevices.Any())
            {
                Log.Error("No SpectrAcq3 devices found, exiting...");
                await deviceManager.StopAsync();
                return;
            }

            var mono = deviceManager.Monochromators.First();
            var spectracq3 = deviceManager.SpectrAcqDevices.First();

            await mono.OpenConnectionAsync();
            await spectracq3.OpenConnectionAsync();

            var wavelengths = new List<int> { 500, 501, 502 };

            try
            {
                foreach (var wavelength in wavelengths)
                {
                    await mono.MoveToWavelengthAsync(wavelength);
                    while (await mono.IsDeviceBusyAsync())
                    {
                        await Task.Delay(100);
                    }
                    Log.Information($"Monochromator set to {wavelength}nm");

                    await spectracq3.SetAcqSetAsync(1, 0, 1, 0);
                    await spectracq3.StartAcquisitionAsync(ScanStartMode.TriggerAndInterval);
                    await Task.Delay(3000);
                    var data = await spectracq3.GetAvailableDataAsync();
                    Log.Information($"Acquired data at {wavelength}nm: {data}");
                    var fileName = $"acquisition_data_{wavelength}.csv";
                    CsvParser.SaveSpectrAcq3DataToCsv(data.Data[0], fileName);
                }
            }
            finally
            {
                await mono.CloseConnectionAsync();
                await spectracq3.CloseConnectionAsync();
                await deviceManager.StopAsync();
            }
        }
    }
}