using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public interface IDeviceManager
{
    Task StartAsync(bool startIcl, bool enableBinaryMessages);
    Task StopAsync();
    Task DiscoverDevicesAsync(CancellationToken cancellationToken);
    WebSocketCommunicator Communicator { get; }
    List<MonochromatorDevice> Monochromators { get; }
    List<ChargedCoupledDevice> ChargedCoupledDevices { get; }
}