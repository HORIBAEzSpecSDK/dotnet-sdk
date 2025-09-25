using System;
using System.Collections.Generic;
using System.Linq;
using Horiba.Sdk.Core.Internal;

namespace Horiba.Sdk.Calculations.Stitching;

/// <summary>
/// Stitches a list of spectra by always keeping the values from the next spectrum in the overlap region.
/// 
/// WARNING: Produces a stitched spectrum with "stairs".
/// 
/// Python parity:
/// - Each spectrum is shaped as [x, y] (NOTE: no extra nesting around y).
/// - The stitched spectrum is returned as [x_stitched, y_stitched].
/// - Assumes x arrays are ordered (Python version does not sort).
/// - Throws if there is no overlap.
/// </summary>
public class SimpleCutSpectraStitch : SpectraStitch
{
    // Stored as Python-compatible raw shape: [x, y]
    private readonly List<List<float>> _stitchedSpectrum;

    /// <summary>
    /// Construct from a list of spectra, each in Python-compatible shape: [x, y].
    /// Example (C#):
    /// var spectra = new List&lt;List&lt;object&gt;&gt;
    /// {
    ///     new() { x1, y1 },
    ///     new() { x2, y2 },
    /// };
    /// </summary>
    public SimpleCutSpectraStitch(List<List<List<float>>> spectraList)
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
        var newStitch = new LinearSpectraStitch(new List<List<List<float>>>
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

    private static List<List<float>> StitchSpectra(List<List<float>> spectrum1, List<List<float>> spectrum2)
    {
        var fx1 = spectrum1[0];
        var fy1 = spectrum1[1];
        var fx2 = spectrum2[0];
        var fy2 = spectrum2[1];

        // Convert to arrays
        var x1 = fx1.ToArray();
        var x2 = fx2.ToArray();
        var y1 = fy1.ToArray();
        var y2 = fy2.ToArray();

        // Sort spectra while maintaining x-y correspondence
        Array.Sort(x1, y1);
        Array.Sort(x2, y2);

        if (x1.Length == 0 || x2.Length == 0)
            throw new ArgumentException("Spectra must contain at least one x/y pair.");

        // Calculate true overlap region
        var x1Min = x1.First();
        var x1Max = x1.Last();
        var x2Min = x2.First();
        var x2Max = x2.Last();

        var overlapStart = Math.Max(x1Min, x2Min);
        var overlapEnd = Math.Min(x1Max, x2Max);

        if (overlapStart >= overlapEnd)
            throw new InvalidOperationException($"No overlapping region between spectra: [{x1[0]}, {x1[x1.Length - 1]}] and [{x2[0]}, {x2[x2.Length - 1]}]");

        // From spectrum 2, take the overlap region (mask2)
        var (x2Overlap, y2Overlap) = WhereXY(x2, y2, v => v >= overlapStart && v <= overlapEnd);

        // From spectrum 1, take values strictly before overlapStart
        var (x1Before, y1Before) = WhereXY(x1, y1, v => v < overlapStart);

        // From spectrum 2, take values strictly after overlapEnd
        var (x2After, y2After) = WhereXY(x2, y2, v => v > overlapEnd);

        // Concatenate in the same order as Python:
        // x_stitched = [x1_before_overlap, x2_overlap, x2_after_overlap]
        // y_stitched = [y1_before_overlap, y2_overlap, y2_after_overlap]
        var xStitched = Concat3(x1Before, x2Overlap, x2After);
        var yStitched = Concat3(y1Before, y2Overlap, y2After);

        // Return in Python-compatible shape: [x_stitched, y_stitched]
        return new List<List<float>>
        {
            xStitched.ToList(),
            yStitched.ToList()
        };
    }

    // ---------- Helpers ----------

    private static (float[] xs, float[] ys) WhereXY(float[] x, float[] y, Func<float, bool> predicate)
    {
        var xs = new List<float>();
        var ys = new List<float>();
        for (int i = 0; i < x.Length; i++)
        {
            if (predicate(x[i]))
            {
                xs.Add(x[i]);
                ys.Add(y[i]);
            }
        }
        return (xs.ToArray(), ys.ToArray());
    }

    private static float[] Concat3(float[] a, float[] b, float[] c)
    {
        return NumpyShims.Concat(NumpyShims.Concat(a, b), c);
    }
}
