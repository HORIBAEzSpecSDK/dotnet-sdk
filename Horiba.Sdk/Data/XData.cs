using Newtonsoft.Json;

namespace Horiba.Sdk.Data;

public record XData
{
    [JsonConstructor]
    public XData(List<float> x)
    {
        X = x;
    }

    [JsonProperty("xData")]
    public List<float> X { get; }
}

