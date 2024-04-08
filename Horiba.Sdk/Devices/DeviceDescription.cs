using Newtonsoft.Json;

namespace Horiba.Sdk.Devices;

internal sealed class DeviceDescription
{
    [JsonProperty("index")] public int Index { get; set; }
    [JsonProperty("deviceType")] public string DeviceType { get; set; }
    [JsonProperty("serialNumber")] public string SerialNumber { get; set; }
}