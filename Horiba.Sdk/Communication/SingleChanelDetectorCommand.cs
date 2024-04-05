namespace Horiba.Sdk.Communication;

public abstract record SingleChanelDetectorCommand : Command
{
    protected SingleChanelDetectorCommand(string commandName, Dictionary<string, object> parameters) : base(commandName,
        parameters)
    {
    }
}