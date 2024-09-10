namespace Horiba.Sdk.Stitching;

public record XYData
{
    public XYData(float x, float y)
    {
        X = x;
        Y = y;
    }

    public XYData(List<float> data)
    {
        if (data == null || data.Count != 2) 
        {
            throw new ArgumentException("List must contain of exactly two elements to be converted to XYData");
        }

        X = data[0];
        Y = data[1];
    }

    public float X { get; }
    public float Y { get; }
}
