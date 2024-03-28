using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract class DeviceDiscovery<T> where T : Device
{
    public abstract Task<List<T>> DiscoverDevicesAsync();
}

public class ChargedCoupleDeviceDeviceDiscovery(WebSocketCommunicator communicator)
    : DeviceDiscovery<ChargedCoupledDevice>
{
    private readonly WebSocketCommunicator _communicator = communicator;

    public override Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}

public class MonochromatorDeviceDiscovery(WebSocketCommunicator communicator) : DeviceDiscovery<MonochromatorDevice>
{
    private readonly WebSocketCommunicator _communicator = communicator;

    public override Task<List<MonochromatorDevice>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}