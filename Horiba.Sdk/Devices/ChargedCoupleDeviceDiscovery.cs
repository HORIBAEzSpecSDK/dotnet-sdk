using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Devices;

internal class ChargedCoupleDeviceDiscovery(WebSocketCommunicator communicator) :
    DeviceDiscovery<ChargedCoupledDevice>
{
    public override async Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        Response countResponse;
        try
        {
            countResponse = await communicator.SendWithResponseAsync(new IclDiscoverCcdCommand(), cancellationToken);
        }
        catch (CommunicationException e)
        {
            Log.Warning(e, "No CCD devices found");
            return [];
        }
        var devicesCount = (long)countResponse.Results["count"];
        if (devicesCount < 0)
        {
            return [];
        }
        var result = new List<ChargedCoupledDevice>();
        var response =
            await communicator.SendWithResponseAsync(new IclCcdListCommand(), cancellationToken);
        
        foreach (var deviceDescriptions in response.Results.Select(ExtractDescription))
        {
            result.AddRange(deviceDescriptions.Select(deviceDescription => 
                new ChargedCoupledDevice(deviceDescription.Index, deviceDescription.DeviceType, deviceDescription.SerialNumber, communicator)));
        }

        return result;
    }

    private static DeviceDescription[] ExtractDescription(KeyValuePair<string, object> pair)
    {
        return JsonConvert.DeserializeObject<DeviceDescription[]>(pair.Value.ToString()) ??
               throw new CommunicationException("DeviceDescription mismatch");
    }
}