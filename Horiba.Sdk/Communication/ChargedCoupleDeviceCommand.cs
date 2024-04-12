namespace Horiba.Sdk.Communication;

/// <summary>
/// A dedicated abstraction used to group device specific commands.
/// Only commands targeting a ChargedCoupleDevice should derive from this class.
/// </summary>
public abstract record ChargedCoupleDeviceCommand : Command
{
    protected ChargedCoupleDeviceCommand(string commandName, int deviceId) :
        base(commandName, new Dictionary<string, object> { { "index", deviceId } })
    {
    }

    protected ChargedCoupleDeviceCommand(string commandName, Dictionary<string, object> parameters) : base(commandName,
        parameters)
    {
    }
}