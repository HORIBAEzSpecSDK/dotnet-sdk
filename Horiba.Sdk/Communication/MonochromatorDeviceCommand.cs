namespace Horiba.Sdk.Communication;

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