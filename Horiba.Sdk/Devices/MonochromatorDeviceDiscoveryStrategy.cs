using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public class MonochromatorDeviceDiscoveryStrategy(ICommunicator communicator) : DeviceDiscoveryStrategy<Monochromator>
{
    private readonly ICommunicator _communicator = communicator;

    public override Task<List<Monochromator>> DiscoverDevicesAsync()
    {
        throw new NotImplementedException();
    }
}