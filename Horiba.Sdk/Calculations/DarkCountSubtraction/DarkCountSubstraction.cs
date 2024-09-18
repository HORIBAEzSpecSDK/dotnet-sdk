namespace Horiba.Sdk.Calculations.DarkCountSubtraction;

public class DarkCountSubstraction
{
    public List<XYData> SubtractData(List<XYData> data, List<XYData> darkData)
    {
        if (data == null) 
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (darkData == null)
        {
            throw new ArgumentNullException(nameof(darkData));
        }

        if (data.Count != darkData.Count)
        {
            throw new ArgumentException("Data and dark data must be the same length.");
        }

        var result = new List<XYData>();
        for (var i = 0; i < data.Count; i++)
        {
            result.Add(new XYData(data[i].X, data[i].Y - darkData[i].Y));
        }

        return result;
    }
}
