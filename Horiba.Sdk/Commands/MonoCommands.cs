using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Commands;

internal record MonoOpenCommand(int DeviceId) : MonochromatorDeviceCommand("mono_open", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}