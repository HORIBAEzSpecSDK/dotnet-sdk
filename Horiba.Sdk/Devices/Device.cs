using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract record Device(int DeviceId, WebSocketCommunicator Communicator)
{
    public int DeviceId { get; } = DeviceId;
    public WebSocketCommunicator Communicator { get; } = Communicator;
    public bool IsConnectionOpened { get; protected set; }
    public abstract Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default);
    public abstract Task<Response> OpenConnectionAsync(CancellationToken cancellationToken = default);
    public abstract Task<Response> CloseConnectionAsync(CancellationToken cancellationToken = default);
}