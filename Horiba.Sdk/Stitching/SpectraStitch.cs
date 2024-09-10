namespace Horiba.Sdk.Stitching;

public abstract class SpectraStitch(List<XYData>[] data)
{
    protected readonly List<XYData>[] Data = data;

    public abstract List<XYData> Stitch();
}
