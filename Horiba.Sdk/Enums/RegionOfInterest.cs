namespace Horiba.Sdk.Enums;

public static class RegionOfInterestExtensions
{
    public static Dictionary<string, object> ToDeviceParameters(this RegionOfInterest region, int deviceId)
    {
        return new Dictionary<string, object>
        {
            { "index", deviceId },
            { "roiIndex", region.RoiIndex },
            { "xOrigin", region.Origin.X },
            { "yOrigin", region.Origin.Y },
            { "xSize", region.Size.Width },
            { "ySize", region.Size.Height },
            { "xBin", region.Bin.X },
            { "yBin", region.Bin.Y }
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
public record RegionOfInterest(int RoiIndex, (int X, int Y) Origin, (int X, int Y) Bin, (int Width, int Height) Size)
{
    /// <summary>
    /// Defines a region which covers the entire chip and create a histogram
    /// </summary>
    public static readonly RegionOfInterest Default = new RegionOfInterest(1, (0, 0), (1, 256), (1024, 256));
    
    public int RoiIndex { get; } = RoiIndex;
    public (int X, int Y) Origin { get; } = Origin;
    public (int X, int Y) Bin { get; } = Bin;
    public (int Width, int Height) Size { get; } = Size;
}