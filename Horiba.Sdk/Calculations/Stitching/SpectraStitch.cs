namespace Horiba.Sdk.Calculations.Stitching;
public abstract class SpectraStitch
{
    public abstract SpectraStitch StitchWith(SpectraStitch otherStitch);
    public abstract List<List<float>> StitchedSpectra();
}
