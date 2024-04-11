using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

/// <summary>
/// Abstraction which defines the essential structure of the TCP communication.
/// It also takes care of generating sequential Ids for every command sent using the <see cref="WebSocketCommunicator"/>  
/// </summary>
public abstract record Command
{
    private static int _initialCommandId;

    protected Command(string commandName, Dictionary<string, object> parameters)
    {
        CommandName = commandName;
        Parameters = parameters;
        Id = ++_initialCommandId;
    }

    protected Command(string commandName) : this(commandName, new Dictionary<string, object>())
    {
    }

    [JsonProperty("id")] public int Id { get; protected set; }

    [JsonProperty("command")] public string CommandName { get; protected set; }

    [JsonProperty("parameters")] public Dictionary<string, object> Parameters { get; protected set; }
}