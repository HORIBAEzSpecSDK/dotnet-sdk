using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public record SingleChannelDetectorDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator)
{
    public override Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}