using Newtonsoft.Json;

namespace Horiba.Sdk.Communication;

/// <summary>
/// Represents a response coming from the ICL
/// </summary>
/// <param name="Id">The id of the command this response is for</param>
/// <param name="CommandName">The name of the command this response is for</param>
/// <param name="Results">The results caused by triggering the command</param>
/// <param name="Errors">The errors the command caused</param>
public sealed record Response(int Id, string CommandName, Dictionary<string, object> Results, List<string> Errors)
{
    [JsonProperty("id")] public int Id { get; set; } = Id;

    [JsonProperty("command")] public string CommandName { get; set; } = CommandName;

    [JsonProperty("results")] public Dictionary<string, object> Results { get; set; } = Results;

    [JsonProperty("errors")] public List<string> Errors { get; set; } = Errors;
}