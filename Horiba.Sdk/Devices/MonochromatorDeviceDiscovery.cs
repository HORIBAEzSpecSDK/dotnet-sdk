using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public class MonochromatorDeviceDiscovery(WebSocketCommunicator communicator) : DeviceDiscovery<MonochromatorDevice>
{
    private readonly WebSocketCommunicator _communicator = communicator;

    public override Task<List<MonochromatorDevice>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}