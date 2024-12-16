using Newtonsoft.Json;

namespace Horiba.Sdk.Data;

public class CcdData
{
    [JsonProperty("acquisition")]
    public List<AcquisitionDescription>? Acquisition { get; set; } = new List<AcquisitionDescription>();

    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; } = string.Empty;
}

public class AcquisitionDescription
{
    [JsonProperty("acqIndex")]
    public string Index { get; set; } = string.Empty;

    [JsonProperty("roi")]
    public List<RegionOfInterestDescription> Region { get; set; } = new List<RegionOfInterestDescription>();
}


public class RegionOfInterestDescription
{
    [JsonProperty("roiIndex")]
    public int Index { get; set; }
    [JsonProperty("xBinning")]
    public int XBinning { get; set; }
    [JsonProperty("xOrigin")]
    public int XOrigin { get; set; }
    [JsonProperty("xSize")]
    public int XSize { get; set; }
    [JsonProperty("yBinning")]
    public int YBinning { get; set; }
    [JsonProperty("yOrigin")]
    public int YOrigin { get; set; }
    [JsonProperty("ySize")]
    public int YSize { get; set; }
    [JsonProperty("xData")]
    public List<float> XData { get; set; }
    [JsonProperty("yData")]
    public List<List<float>> YData { get; set; }
}

