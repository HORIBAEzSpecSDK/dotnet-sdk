using Newtonsoft.Json;

namespace Horiba.Sdk.Data;


public class ErrorEntry
{
    [JsonProperty("errors")] public string Error { get; set; }
}