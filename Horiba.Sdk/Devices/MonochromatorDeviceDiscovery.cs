using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Devices;

internal class MonochromatorDeviceDiscovery(WebSocketCommunicator communicator) :
    DeviceDiscovery<MonochromatorDevice>
{
    public override async Task<List<MonochromatorDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        Response countResponse;
        try
        {
            countResponse = await communicator.SendWithResponseAsync(new IclDiscoverMonochromatorDevicesCommand(),
                cancellationToken);
        }
        catch (CommunicationException e)
        {
            Log.Warning(e, "No Monochromator devices found.");
            return [];
        }
        var devicesCount = (long)countResponse.Results["count"];
        if (devicesCount < 0)
        {
            return [];
        }
        var result = new List<MonochromatorDevice>();
        var response =
            await communicator.SendWithResponseAsync(new IclMonochromatorListCommand(), cancellationToken);
        
        foreach (var deviceDescriptions in response.Results.Select(rawDescription => ExtractDescription(rawDescription.Value.ToString())))
        {
            result.AddRange(deviceDescriptions.Select(deviceDescription =>
                new MonochromatorDevice(deviceDescription.Index, deviceDescription.DeviceType, deviceDescription.SerialNumber, communicator)));
        }

        return result;
    }

    private static DeviceDescription[] ExtractDescription(string rawDescription)
    {
        return JsonConvert.DeserializeObject<DeviceDescription[]>(rawDescription) ??
               throw new CommunicationException("DeviceDescription mismatch");
    }
}