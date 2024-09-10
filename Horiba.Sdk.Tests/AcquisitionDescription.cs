using Newtonsoft.Json;

namespace Horiba.Sdk.Tests;

public class AcquisitionDescription
{
    [JsonProperty("acqIndex")]
    public string Index { get; set; }
        
    [JsonProperty("roi")]
    public List<RegionOfInterestDescription> Region { get; set; }
        
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
        [JsonProperty("xyData")]
        public List<List<float>> Data { get; set; }
    }
}