using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

public sealed record Response(int Id, string CommandName, Dictionary<string, object> Results, List<string> Errors)
{
    [JsonProperty("id")] public int Id { get; set; } = Id;

    [JsonProperty("command")] public string CommandName { get; set; } = CommandName;

    [JsonProperty("results")] public Dictionary<string, object> Results { get; set; } = Results;

    [JsonProperty("errors")] public List<string> Errors { get; set; } = Errors;
}