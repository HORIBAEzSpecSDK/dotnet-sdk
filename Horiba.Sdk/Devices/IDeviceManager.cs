using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public interface IDeviceManager
{
    Task StartAsync(bool startIcl);
    Task StopAsync();
    Task DiscoverDevicesAsync(CancellationToken cancellationToken);
    ICommunicator Communicator { get; }
    List<Monochromator> Monochromators { get; }
    List<ChargedCoupledDevice> ChargedCoupledDevices { get; }
}