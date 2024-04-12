namespace Horiba.Sdk.Communication;

/// <summary>
/// A dedicated abstraction used to group device specific commands.
/// Only commands targeting a MonochromatorDevice should derive from this class.
/// </summary>
public abstract record MonochromatorDeviceCommand : Command
{
    protected MonochromatorDeviceCommand(string commandName, int deviceId) :
        base(commandName, new Dictionary<string, object> { { "index", deviceId } })
    {
    }

    protected MonochromatorDeviceCommand(string commandName, Dictionary<string, object> parameters) : base(commandName,
        parameters)
    {
    }
}