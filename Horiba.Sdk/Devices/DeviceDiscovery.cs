using System.Text.RegularExpressions;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract class DeviceDiscovery<T> where T : Device
{
    public abstract Task<List<T>> DiscoverDevicesAsync(CancellationToken cancellationToken);
}

public class ChargedCoupleDeviceDeviceDiscovery(WebSocketCommunicator communicator)
    : DeviceDiscovery<ChargedCoupledDevice>
{
    public override async Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        var response = await communicator.SendWithResponseAsync(new IclDiscoverCcdCommand(), cancellationToken);
        var result = new List<ChargedCoupledDevice>();
        foreach (var rawDescription in response.Results)
        {
            var deviceDescription = ExtractDescription(rawDescription.Value.ToString());
            result.Add(new ChargedCoupledDevice(deviceDescription.ProductId, deviceDescription.DeviceType,
                deviceDescription.SerialNumber, communicator));
        }

        return result;
    }

    internal ChargedCoupledDeviceDescription ExtractDescription(string rawDescription)
    {
        var ccdTypeMatch = Regex.Match(rawDescription, @"deviceType: (.*?),");
        var ccdIdMatch = Regex.Match(rawDescription, @"productId: (.*?),");
        var ccdSerialNumberMatch = Regex.Match(rawDescription, @"serialNumber: (.*?)}");

        var id = ccdIdMatch.Groups[1].Value.Trim();
        var ccdType = ccdTypeMatch.Groups[1].Value.Trim();
        var serialNumber = ccdSerialNumberMatch.Groups[1].Value.Trim();
        
        return new ChargedCoupledDeviceDescription { ProductId = int.Parse(id), DeviceType = ccdType, SerialNumber = serialNumber };
    }
}

public class MonochromatorDeviceDiscovery(WebSocketCommunicator communicator) : DeviceDiscovery<MonochromatorDevice>
{
    public override async Task<List<MonochromatorDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        var response = await communicator.SendWithResponseAsync(new IclDiscoverMonochromatorDevicesCommand(), cancellationToken);
        var result = new List<MonochromatorDevice>();
        foreach (var rawDescription in response.Results)
        {
            var deviceDescription = ExtractDescription(rawDescription.Value.ToString());
            result.Add(new MonochromatorDevice(deviceDescription.ProductId, deviceDescription.DeviceType,
                deviceDescription.SerialNumber, communicator));
        }

        return result;
    }

    internal ChargedCoupledDeviceDescription ExtractDescription(string rawDescription)
    {
        var splitDescription = rawDescription.Split(";");

        var id = splitDescription[0].Replace("[\r\n  \"","").Trim();
        var ccdType = splitDescription[1];
        var serialNumber = splitDescription[2];
        
        return new ChargedCoupledDeviceDescription { ProductId = int.Parse(id), DeviceType = ccdType, SerialNumber = serialNumber };
    }
}