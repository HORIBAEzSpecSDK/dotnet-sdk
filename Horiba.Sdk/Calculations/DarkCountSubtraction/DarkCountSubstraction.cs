using Horiba.Sdk.Data;

namespace Horiba.Sdk.Calculations.DarkCountSubtraction;

public class DarkCountSubstraction
{
    public YData SubtractData(YData normalData, YData darkData)
    {
        if (normalData == null) 
        {
            throw new ArgumentNullException(nameof(normalData));
        }

        if (darkData == null)
        {
            throw new ArgumentNullException(nameof(darkData));
        }

        if (normalData.Y.Count != darkData.Y.Count)
        {
            throw new ArgumentException("Data and dark data must be the same length.");
        }

        var result = new YData([[]]);
        result.Y = [];

        for (int i = 0; i < normalData.Y.Count; i++)
        {
            var row = new List<float>();
            for (int j = 0; j < normalData.Y[i].Count; j++)
            {
                float difference = normalData.Y[i][j] - darkData.Y[i][j];
                row.Add(difference);
            }
            result.Y.Add(row);
        }

            return result;
    }
}
