using Horiba.Sdk.Data;

namespace Horiba.Sdk.Calculations.DarkCountSubtraction;

public class DarkCountSubstraction
{
    public List<List<float>> SubtractData(List<List<float>> normalData, List<List<float>> darkData)
    {
        if (normalData == null) 
        {
            throw new ArgumentNullException(nameof(normalData));
        }

        if (darkData == null)
        {
            throw new ArgumentNullException(nameof(darkData));
        }

        if (normalData.Count != darkData.Count)
        {
            throw new ArgumentException("Data and dark data must be the same length.");
        }

        var result = new List<List<float>>();

        for (int i = 0; i < normalData.Count; i++)
        {
            var row = new List<float>();
            for (int j = 0; j < normalData[i].Count; j++)
            {
                float difference = normalData[i][j] - darkData[i][j];
                row.Add(difference);
            }
            result.Add(row);
        }

            return result;
    }
}
