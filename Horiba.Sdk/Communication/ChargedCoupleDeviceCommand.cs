namespace Horiba.Sdk.Communication;

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