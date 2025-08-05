using Horiba.Sdk.Communication;
using Horiba.Sdk.Commands;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Devices;

internal class SpectrAcqDeviceDiscovery(WebSocketCommunicator communicator) : DeviceDiscovery<SpectrAcqDevice>
{
    public override async Task<List<SpectrAcqDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken)
    {
        Response countResponse;
        try
        {
            countResponse = await communicator.SendWithResponseAsync(new IclDiscoverSpectrAcqCommand(), cancellationToken);
        }
        catch (CommunicationException e)
        {
            Log.Warning(e, "No SpectrAcq devices found");
            return [];
        }

        var devicesCount = (long)countResponse.Results["count"];
        if (devicesCount < 0)
        {
            return [];
        }

        var result = new List<SpectrAcqDevice>();
        var response =
            await communicator.SendWithResponseAsync(new IclSpectrAcqListCommand(), cancellationToken);

        foreach (var deviceDescriptions in response.Results.Select(ExtractDescription))
        {
            result.AddRange(deviceDescriptions.Select(deviceDescription =>
                new SpectrAcqDevice(deviceDescription.Index, deviceDescription.DeviceType,
                    deviceDescription.SerialNumber, communicator)));
        }

        return result;
    }
    private static DeviceDescription[] ExtractDescription(KeyValuePair<string, object> pair)
    {
        return JsonConvert.DeserializeObject<DeviceDescription[]>(pair.Value.ToString()) ??
               throw new CommunicationException("DeviceDescription mismatch");
    }
}