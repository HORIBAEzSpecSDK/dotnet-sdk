using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public class ChargedCoupleDeviceDeviceDiscovery(WebSocketCommunicator communicator)
    : DeviceDiscovery<ChargedCoupledDevice>
{
    private readonly WebSocketCommunicator _communicator = communicator;

    public override Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}