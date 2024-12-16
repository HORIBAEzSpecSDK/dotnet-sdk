namespace Horiba.Sdk.Enums;

public static class RegionOfInterestExtensions
{
    public static Dictionary<string, object> ToDeviceParameters(this RegionOfInterest region, int deviceId)
    {
        return new Dictionary<string, object>
        {
            { "index", deviceId },
            { "roiIndex", region.RoiIndex },
            { "xOrigin", region.XOrigin },
            { "yOrigin", region.YOrigin },
            { "xSize", region.XSize },
            { "ySize", region.YSize },
            { "xBin", region.XBin },
            { "yBin", region.YBin }
        };
    }
}

/// <summary>
/// Represents the region which the device will read data from
/// </summary>
/// <param name="RoiIndex"></param>
/// <param name="Origin">X,Y coordinates setting the starting point of the region</param>
/// <param name="Bin"></param>
/// <param name="Size">Width,Height of the region</param>
public record RegionOfInterest(int RoiIndex, int XOrigin, int YOrigin, int XSize, int YSize, int XBin, int YBin)
{
    /// <summary>
    /// Defines a region which covers the entire chip and create a histogram
    /// </summary>
    public static readonly RegionOfInterest Default = new RegionOfInterest(1, 0, 0, 1024, 256, 1, 256);
    
    public int RoiIndex { get; } = RoiIndex;
    public int XOrigin { get; } = XOrigin;
    public int YOrigin { get; } = YOrigin;
    public int XSize { get; } = XSize;
    public int YSize { get; } = YSize;
    public int XBin { get; } = XBin;
    public int YBin { get; } = YBin;

}