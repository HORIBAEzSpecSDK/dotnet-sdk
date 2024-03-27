using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

public abstract record Command
{
    private static int _initialCommandId;
    public int Id { get; protected set; }
    [JsonProperty("command")]
    public string CommandName { get; protected set; }

    protected Command(string commandName)
    {
        CommandName = commandName;
        Id = ++_initialCommandId;
    }
}