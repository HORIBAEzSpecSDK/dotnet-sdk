namespace Horiba.Sdk.Enums;

public static class RegionOfInterestExtensions
{
    public static Dictionary<string, object> ToDeviceParameters(this RegionOfInterest region, int deviceId)
    {
        return new Dictionary<string, object>()
        {
            { "index", deviceId },
            { "roiIndex", region.RoiIndex },
            { "xOrigin", region.Origin.X },
            { "yOrigin", region.Origin.Y },
            { "xSize", region.Size.Width },
            { "ySize", region.Size.Height },
            { "xBin", region.Bin.X },
            { "yBin", region.Bin.Y },
        };
    }
}

public record RegionOfInterest(int RoiIndex, (int X, int Y) Origin, (int X, int Y) Bin, (int Width, int Height) Size)
{
    public int RoiIndex { get; } = RoiIndex;
    public (int X, int Y) Origin { get; } = Origin;
    public (int X, int Y) Bin { get; } = Bin;
    public (int Width, int Height) Size { get; } = Size;
}