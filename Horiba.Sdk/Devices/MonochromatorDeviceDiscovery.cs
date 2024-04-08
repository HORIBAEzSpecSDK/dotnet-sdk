using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Newtonsoft.Json;

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
            var deviceDescriptions = ExtractDescription(rawDescription.Value.ToString());
            foreach (var deviceDescription in deviceDescriptions)
            {
                result.Add(new MonochromatorDevice(deviceDescription.Index, deviceDescription.DeviceType,
                    deviceDescription.SerialNumber, communicator));
            }
        }

        return result;
    }

    internal DeviceDescription[] ExtractDescription(string rawDescription)
    {
        return JsonConvert.DeserializeObject<DeviceDescription[]>(rawDescription) ??
               throw new CommunicationException("DeviceDescription mismatch");
    }
}