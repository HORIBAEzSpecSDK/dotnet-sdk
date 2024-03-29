using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

public abstract record Command
{
    private static int _initialCommandId;
    
    [JsonProperty("id")]
    public int Id { get; protected set; }
    [JsonProperty("command")]
    public string CommandName { get; protected set; }
    [JsonProperty("parameters")]
    public Dictionary<string, object> Parameters { get; protected set; }

    protected Command(string commandName, Dictionary<string, object> parameters)
    {
        CommandName = commandName;
        Parameters = parameters;
        Id = ++_initialCommandId;
    }

    protected Command(string commandName) : this(commandName, new Dictionary<string, object>())
    {
    }
}

public abstract record ChargedCoupleDeviceCommand : Command
{
    protected ChargedCoupleDeviceCommand(int deviceId, string commandName) : 
        base(commandName, new Dictionary<string, object> { { "index", deviceId } })
    {
        
    }
    // protected ChargedCoupleDeviceCommand(int deviceId, string commandName, params ) : 
    //     base(commandName, new Dictionary<string, object> { { "index", deviceId } })
    // {
    //     
    // }
}

public abstract record MonochromatorDeviceCommand : Command
{
    protected MonochromatorDeviceCommand(string commandName, Dictionary<string, object> parameters) : base(commandName, parameters)
    {
    }
}

public abstract record SingleChanelDetectorCommand : Command
{
    protected SingleChanelDetectorCommand(string commandName, Dictionary<string, object> parameters) : base(commandName, parameters)
    {
    }
}