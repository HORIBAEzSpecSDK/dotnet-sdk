using Horiba.Sdk.Core.Internal;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Horiba.Sdk.Examples;

/// <summary>
/// Stitches a list of spectra using a weighted average as in LabSpec6.
/// 
/// Python parity:
/// - Each spectrum is shaped as [x, y] (no extra nesting around y).
/// - The stitched spectrum is returned as [x_stitched, y_stitched].
/// - Assumes x arrays are ordered (the Python version does not sort).
/// - Throws if there is no overlap.
/// - Computes weights A and B linearly over the overlap and combines y elementwise.
///   (Assumes the two overlap ranges have the same number of samples, as in the Python.)
/// </summary>
public class LabSpec6SpectraStitch : SpectraStitch
{
    private readonly List<List<float>> _stitchedSpectrum;

    public LabSpec6SpectraStitch(List<List<List<float>>> spectraList)
    {
        if (spectraList == null || spectraList.Count == 0)
            throw new ArgumentException("Spectra list cannot be null or empty");

        _stitchedSpectrum = spectraList[0];

        for (var i = 1; i < spectraList.Count; i++)
        {
            _stitchedSpectrum = StitchSpectra(_stitchedSpectrum, spectraList[i]);
        }
    }

    public override SpectraStitch StitchWith(SpectraStitch otherStitch)
    {
        var newStitch = new LabSpec6SpectraStitch(new List<List<List<float>>>
        {
            StitchedSpectra(),
            otherStitch.StitchedSpectra()
        });
        return newStitch;
    }

    public override List<List<float>> StitchedSpectra()
    {
        return _stitchedSpectrum;
    }

    private List<List<float>> StitchSpectra(List<List<float>> spectrum1, List<List<float>> spectrum2)
    {

        // Unpack Python-like shape: [x, y]
        var fx1 = spectrum1[0];
        var fy1 = spectrum1[1];
        var fx2 = spectrum2[0];
        var fy2 = spectrum2[1];

        // Convert to arrays
        var x1 = fx1.ToArray();
        var x2 = fx2.ToArray();
        var y1 = fy1.ToArray();
        var y2 = fy2.ToArray();

        if (x1.Length == 0 || x2.Length == 0)
            throw new ArgumentException("Spectra must contain at least one x/y pair.");


        Array.Sort(x1, y1);
        Array.Sort(x2, y2);

        if (x1.Last() < x2[0])
        {
            throw new ArgumentException("No overlap between spectra, can concatenate here if enabled");
        }

        // Finds the index of the largest element in the first spectrum that is less than the first element in the second spectrum. Defines this as the start of the overlapping region.
        var x2Start = x2[0];
        var candidates1 = x1
            .Select((value, index) => new { value, index })
            .Where(x => x.value < x2Start);

        int overlapStartIndex = 0;

        if (candidates1.Any())
        {
            // Find the candidate with the max value
            var result1 = candidates1
                .OrderByDescending(x => x.value)
                .First();

            overlapStartIndex = result1.index;
        }
        else
        {
            throw new ArgumentException("No valid max value candidate");
        }

        var overlapStart = x1[overlapStartIndex];

        // Finds the index of the smallest element in the second spectrum that is greater than the last element in the first spectrum. Defines this as the end of the overlapping region.
        var x1End = x1.Last();
        var candidates2 = x2
            .Select((value, index) => new { value, index })
            .Where(x => x.value > x1End);

        int overlapEndIndex = 0;

        if (candidates2.Any())
        {
            var result2 = candidates2
                .OrderBy(x => x.value)
                .First();
            overlapEndIndex = result2.index;
        }
        else
        {
            throw new ArgumentException("No valid min value candidate");
        }
        var overlapEnd = x2[overlapEndIndex];

        var yOverlapWeightedAverage = new List<float>();

        for (int i = overlapStartIndex + 1; i < x1.Length; i++)
        {
            //converted argmin method from python
            int idx = ArgMin(x2, x1[i]);

            float weight1 = overlapEnd - x1[i];
            float weight2 = x2[idx] - overlapStart;
            float denom = (overlapEnd - overlapStart);

            float weightedAverage = (y1[i] * weight1 + y2[idx] * weight2) / denom;
            yOverlapWeightedAverage.Add(weightedAverage);
        }


        // Helper
        //Find index in x2 where x2 is closest to x1
        int ArgMin(float[] array, float value)
        {
            float minDiff = float.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < array.Length; i++)
            {
                float diff = Math.Abs(array[i] - value);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        float[] x2After = x2.Skip(overlapEndIndex).ToArray();
        float[] y1Before = y1.Take(overlapStartIndex + 1).ToArray();
        float[] y2After = y2.Skip(overlapEndIndex).ToArray();

        float[] xCombined = x1.Concat(x2After).ToArray();
        float[] yCombined = y1Before.Concat(yOverlapWeightedAverage).Concat(y2After).ToArray();

        return new List<List<float>>
        {
            xCombined.ToList(),
            yCombined.ToList()
        };

    }
}