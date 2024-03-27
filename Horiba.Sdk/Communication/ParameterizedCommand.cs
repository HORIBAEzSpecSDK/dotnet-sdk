namespace Horiba.Sdk.Communication;

public record ParameterizedCommand : Command
{
    public Dictionary<string, object> Parameters { get; private set; }

    public ParameterizedCommand(string commandName, Dictionary<string, object> parameters) : base(commandName)
    {
        Parameters = parameters;
    }
}