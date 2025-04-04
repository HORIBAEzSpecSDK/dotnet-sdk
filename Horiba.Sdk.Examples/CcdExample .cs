using Horiba.Sdk.Devices;
using Horiba.Sdk.Data;
using Horiba.Sdk.Enums;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Serilog.Sinks.SystemConsole.Themes;




namespace Horiba.Sdk.Examples.CcdExamples
{
    class CcdProgram
    
    {
    
        public static async Task CcdExample()
        {
            DeviceManager Dm = new DeviceManager();
            await Dm.StartAsync();
            var ccd = Dm.ChargedCoupledDevices.First();
            await ccd.OpenConnectionAsync();
    
            var rawConfig = await ccd.GetDeviceConfigurationAsync();
    
            var ChipHSpacing = (long)rawConfig["chipHSpacing"];
            var ChipHeight = (long)rawConfig["chipHeight"];
            var ChipName = (string)rawConfig["chipName"];
            var ChipSerialNumber = (string)rawConfig["chipSerialNumber"];
            var ChipVSpacing = (long)rawConfig["chipVSpacing"];
            var ChipWidth = (long)rawConfig["chipWidth"];
            var DeviceType = (string)rawConfig["deviceType"];
            var HardwareAvgAvailable = (bool)rawConfig["hardwareAvgAvailable"];
            var IsLinescan = (bool)rawConfig["lineScan"];
            var ProductId = (string)rawConfig["productId"];
            var SerialNumber = (string)rawConfig["serialNumber"];
            var FirmwareVersion = (string)rawConfig["version"];
            var ChipXSize = ChipWidth;
            var ChipYSize = ChipHeight;
    
            JArray jsonStringArray = (JArray)rawConfig["speeds"];
            RegionOfInterest myRegion = new RegionOfInterest(1, 0, 0, 16, 4, 1, 4);
            await ccd.SetAcquisitionCountAsync(1);
            await ccd.SetExposureTimeAsync(100);
            await ccd.SetRegionOfInterestAsync(myRegion);
            await ccd.SetXAxisConversionTypeAsync(ConversionType.None);
    
            int dsize = await ccd.GetDataSizeAsync();
    
            await ccd.SetXAxisConversionTypeAsync(ConversionType.None);
            CcdData returnedCcDData = null;
    
            if (await ccd.GetAcquisitionReadyAsync())
            {
                await ccd.AcquisitionStartAsync(true);
    
                // This method will start a polling procedure after 1s initial delay
                // this initial delay is needed to allow the device to start the acquisition.
                // The interval of the polling procedure is set to be 300ms
                // every iteration of the polling procedure will check if the device is busy
                // by sending a request to the device.
                // The method will return when the device is not busy anymore
                await ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(300));
    
                returnedCcDData = await ccd.GetAcquisitionDataAsync();
            }
            Console.WriteLine("The following xData was retrieved");
            foreach (var xData in returnedCcDData.Acquisition[0].Region[0].XData)
            {
                Console.WriteLine(xData.ToString());
            }
    
            await ccd.CloseConnectionAsync();
            await Dm.StopAsync();
        }
    
    }
}



