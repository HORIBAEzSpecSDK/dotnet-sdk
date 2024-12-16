using Newtonsoft.Json;

namespace Horiba.Sdk.Data;

public record YData
{
    [JsonConstructor]
    public YData(List<List<float>> y)
    {
        Y = y;
    }

    [JsonProperty("yData")]
    public List<List<float>> Y { get; set; }
}
