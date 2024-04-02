using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public interface IDeviceManager
{
    WebSocketCommunicator Communicator { get; }
    List<MonochromatorDevice> Monochromators { get; }
    List<ChargedCoupledDevice> ChargedCoupledDevices { get; }
    Task StartAsync(bool startIcl, bool enableBinaryMessages);
    Task StopAsync();
    Task DiscoverDevicesAsync(CancellationToken cancellationToken);
}