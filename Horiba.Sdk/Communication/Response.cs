using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

public record Response(int Id, string CommandName, Dictionary<string, object> Results)
{
    public int Id { get; set; } = Id;

    [JsonProperty("command")]
    public string CommandName { get; set; } = CommandName;

    public Dictionary<string, object> Results { get; set; } = Results;
}