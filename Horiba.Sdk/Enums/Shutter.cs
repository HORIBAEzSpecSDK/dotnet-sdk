using Newtonsoft.Json;

namespace Horiba.Sdk.Enums;

public enum Shutter
{
    [JsonProperty("shutter 1")]
    Shutter1 = 0,
    [JsonProperty("shutter 2")]
    Shutter2 = 1
}