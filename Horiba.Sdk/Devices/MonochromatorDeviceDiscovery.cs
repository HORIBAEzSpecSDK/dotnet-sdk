using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

internal class MonochromatorDeviceDiscovery(WebSocketCommunicator communicator) :
    DeviceDiscovery<MonochromatorDevice>
{
    public override async Task<List<MonochromatorDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        var countResponse =
            await communicator.SendWithResponseAsync(new IclDiscoverMonochromatorDevicesCommand(), cancellationToken);
        var devicesCount = (long)countResponse.Results["count"];
        if (devicesCount < 0)
        {
            return new List<MonochromatorDevice>();
        }
        var result = new List<MonochromatorDevice>();
        var response =
            await communicator.SendWithResponseAsync(new IclListMonochromatorDevicesCommand(), cancellationToken);
        
        foreach (var rawDescription in response.Results)
        {
            var deviceDescription = ExtractDescription(rawDescription.Value.ToString());
            result.Add(new MonochromatorDevice(deviceDescription.ProductId, deviceDescription.DeviceType,
                deviceDescription.SerialNumber, communicator));
        }

        return result;
    }

    internal DeviceDescription ExtractDescription(string rawDescription)
    {
        var splitDescription = rawDescription.Split(";");

        var id = splitDescription[0].Replace("[\r\n  \"", "").Trim();
        var ccdType = splitDescription[1];
        var serialNumber = splitDescription[2];

        return new DeviceDescription { ProductId = int.Parse(id), DeviceType = ccdType, SerialNumber = serialNumber };
    }
}