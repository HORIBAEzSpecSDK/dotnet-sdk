using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract record Device(int DeviceId, string DeviceType, string SerialNumber, WebSocketCommunicator Communicator)
{
    public string DeviceType { get; } = DeviceType;
    public string SerialNumber { get; } = SerialNumber;
    public int DeviceId { get; } = DeviceId;
    public WebSocketCommunicator Communicator { get; } = Communicator;
    public bool IsConnectionOpened { get; protected set; }
    public abstract Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default);
    public abstract Task OpenConnectionAsync(CancellationToken cancellationToken = default);
    public abstract Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    public abstract Task WaitForDeviceNotBusy(int waitIntervalInMs, int initialWaitInMs,
        CancellationToken cancellationToken = default);
}