using System.Text.RegularExpressions;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

internal class ChargedCoupleDeviceDiscovery(WebSocketCommunicator communicator) :
    DeviceDiscovery<ChargedCoupledDevice>
{
    public override async Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        var countResponse = await communicator.SendWithResponseAsync(new IclDiscoverCcdCommand(), cancellationToken);
        var devicesCount = (long)countResponse.Results["count"];
        if (devicesCount < 0)
        {
            return new List<ChargedCoupledDevice>();
        }
        var result = new List<ChargedCoupledDevice>();
        var response =
            await communicator.SendWithResponseAsync(new IclListCcdCommand(), cancellationToken);
        
        foreach (var rawDescription in response.Results)
        {
            var deviceDescription = ExtractDescription(rawDescription);
            result.Add(new ChargedCoupledDevice(deviceDescription.ProductId, deviceDescription.DeviceType,
                deviceDescription.SerialNumber, communicator));
        }

        return result;
    }

    internal DeviceDescription ExtractDescription(KeyValuePair<string, object> pair)
    {
        var ccdTypeMatch = Regex.Match(pair.Value.ToString(), @"deviceType: (.*?),");
        var ccdIdMatch = Regex.Match(pair.Key, @"index(.*?):");
        var ccdSerialNumberMatch = Regex.Match(pair.Value.ToString(), @"serialNumber: (.*?)}");

        var id = ccdIdMatch.Groups[1].Value.Trim();
        var ccdType = ccdTypeMatch.Groups[1].Value.Trim();
        var serialNumber = ccdSerialNumberMatch.Groups[1].Value.Trim();

        return new DeviceDescription { ProductId = int.Parse(id), DeviceType = ccdType, SerialNumber = serialNumber };
    }
}