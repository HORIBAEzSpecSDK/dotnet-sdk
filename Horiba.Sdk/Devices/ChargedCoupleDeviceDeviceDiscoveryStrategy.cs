using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public class ChargedCoupleDeviceDeviceDiscoveryStrategy(ICommunicator communicator)
    : DeviceDiscoveryStrategy<ChargedCoupledDevice>
{
    private readonly ICommunicator _communicator = communicator;

    public override Task<List<ChargedCoupledDevice>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}