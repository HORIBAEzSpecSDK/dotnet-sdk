using Newtonsoft.Json;

namespace Horiba.Sdk.Data;

public class SaqData
{
    [JsonProperty("data")] public List<DataItem> Data { get; set; } = [];
}

public class DataItem
{
    [JsonProperty("currentSignal")] public SaqSignal CurrentSignal { get; set; }

    [JsonProperty("elapsedTime")] public int ElapsedTime { get; set; }

    [JsonProperty("eventMarker")] public bool EventMarker { get; set; }

    [JsonProperty("overscaleCurrentChannel")]  public bool OverscaleCurrentChannel { get; set; }

    [JsonProperty("overscaleVoltageChannel")]  public bool OverscaleVoltageChannel { get; set; }

    [JsonProperty("pmtSignal")] public SaqSignal PmtSignal { get; set; }

    [JsonProperty("pointNumber")] public int PointNumber { get; set; }

    [JsonProperty("ppdSignal")] public SaqSignal PpdSignal { get; set; }

    [JsonProperty("voltageSignal")] public SaqSignal VoltageSignal { get; set; }
}

public class SaqSignal
{
    [JsonProperty("unit")] public string Unit { get; set; }

    [JsonProperty("value")] public float Value { get; set; }
}