using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public abstract record Device(int DeviceId, ICommunicator Communicator)
{
    public int DeviceId { get; } = DeviceId;
    public ICommunicator Communicator { get; } = Communicator;
}

public record ChargedCoupledDevice(int DeviceId, ICommunicator Communicator) : Device(DeviceId, Communicator);
public record Monochromator(int DeviceId, ICommunicator Communicator) : Device(DeviceId, Communicator);