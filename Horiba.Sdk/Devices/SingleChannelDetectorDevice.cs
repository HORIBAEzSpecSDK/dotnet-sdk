using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public sealed record SingleChannelDetectorDevice(
    int DeviceId,
    string DeviceType,
    string SerialNumber,
    WebSocketCommunicator Communicator)
    : Device(DeviceId, DeviceType, SerialNumber, Communicator)
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

    public override Task WaitForDeviceBusy(int waitIntervalInMs, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}