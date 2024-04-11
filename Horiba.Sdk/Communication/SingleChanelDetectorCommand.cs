namespace Horiba.Sdk.Communication;

/// <summary>
/// A dedicated abstraction used to group device specific commands.
/// Only commands targeting a SingleChanelDetector should derive from this class.
/// </summary>
public abstract record SingleChanelDetectorCommand : Command
{
    protected SingleChanelDetectorCommand(string commandName, Dictionary<string, object> parameters) : base(commandName,
        parameters)
    {
    }
}