using System.Text.RegularExpressions;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Newtonsoft.Json;

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
            await communicator.SendWithResponseAsync(new IclCcdListCommand(), cancellationToken);
        
        foreach (var rawDescription in response.Results)
        {
            var deviceDescriptions = ExtractDescription(rawDescription);
            foreach (var deviceDescription in deviceDescriptions)
            {
                result.Add(new ChargedCoupledDevice(deviceDescription.Index, deviceDescription.DeviceType,
                    deviceDescription.SerialNumber, communicator));
            }
        }

        return result;
    }

    internal DeviceDescription[] ExtractDescription(KeyValuePair<string, object> pair)
    {
        return JsonConvert.DeserializeObject<DeviceDescription[]>(pair.Value.ToString()) ??
               throw new CommunicationException("DeviceDescription mismatch");
    }
}