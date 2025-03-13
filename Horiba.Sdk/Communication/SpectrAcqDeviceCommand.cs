namespace Horiba.Sdk.Communication;

/// <summary>
/// A dedicated abstraction used to group device specific commands.
/// Only commands targeting a Spectracq3 should derive from this class.
/// </summary>
public abstract record SpectrAcqDeviceCommand : Command
{
    protected SpectrAcqDeviceCommand(string commandName, int deviceId) :
        base(commandName, new Dictionary<string, object> { { "index", deviceId } })
    {
    }    
    protected SpectrAcqDeviceCommand(string commandName, Dictionary<string, object> parameters) : base(commandName,
        parameters)
    {
    }
}