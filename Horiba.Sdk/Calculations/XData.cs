namespace Horiba.Sdk.Data;

public record XData
{

    public XData(List<float> data)
    {
        if (data == null)
        {
            throw new ArgumentException("X data must not be null");
        }

        X = data;
    }



    public List<float> X { get; }
}

