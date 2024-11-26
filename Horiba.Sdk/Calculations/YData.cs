namespace Horiba.Sdk.Data;

public record YData
{

    public YData(List<List<float>> data)
    {
        if (data == null)
        {
            throw new ArgumentException("Y data must not be null");
        }

        Y = data;
    }



    public List<List<float>> Y { get; set; }
}
