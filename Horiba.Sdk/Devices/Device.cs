using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract record Device(int DeviceId, WebSocketCommunicator Communicator)
{
    public int DeviceId { get; } = DeviceId;
    public WebSocketCommunicator Communicator { get; } = Communicator;
}

public record ChargedCoupledDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator);
public record MonochromatorDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator);
public record SingleChannelDetectorDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator);
