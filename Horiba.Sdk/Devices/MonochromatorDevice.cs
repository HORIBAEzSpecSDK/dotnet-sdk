using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public record MonochromatorDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator)
{
    public override Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Response> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Response> CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}