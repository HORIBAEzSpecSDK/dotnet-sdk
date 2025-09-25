using System;
using System.Collections.Generic;
using System.Linq;
using Horiba.Sdk.Core.Internal;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using Serilog;



namespace Horiba.Sdk.Calculations.Stitching;

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
            // Overlap region (assumes x are ordered; no sorting in Python version)
            float overlapStart = Math.Max(x1[0], x2[0]);
            float overlapEnd = Math.Min(x1[x1.Length - 1], x2[x2.Length - 1]);

            if (overlapStart >= overlapEnd)
                throw new InvalidOperationException(
                    $"No overlapping region between spectra: [{x1[0]}, {x1[x1.Length - 1]}] and [{x2[0]}, {x2[x2.Length - 1]}]");

            // Masks on both spectra (inclusive)
            var (x1Overlap, y1Overlap) = WhereXY(x1, y1, v => v >= overlapStart && v <= overlapEnd);
            var (x2Overlap, y2Overlap) = WhereXY(x2, y2, v => v >= overlapStart && v <= overlapEnd);

            // The original NumPy code multiplies arrays elementwise.
            // That implies x1_overlap and x2_overlap have the same length in your data.
            if (x1Overlap.Length != x2Overlap.Length)
                throw new InvalidOperationException(
                    "Overlap sample counts differ between spectra, which the original NumPy code would fail on. " +
                    $"Got x1_overlap={x1Overlap.Length}, x2_overlap={x2Overlap.Length}.");

            // Compute weights A, B over the overlap [overlapStart, overlapEnd]
            float denom = (overlapEnd - overlapStart);
            if (denom == 0.0)
                throw new InvalidOperationException("Degenerate overlap (start equals end).");

            var A = new float[x1Overlap.Length];
            var B = new float[x2Overlap.Length];

            for (int i = 0; i < x1Overlap.Length; i++)
                A[i] = (x1Overlap[i] - overlapStart) / denom;

            for (int i = 0; i < x2Overlap.Length; i++)
                B[i] = (overlapEnd - x2Overlap[i]) / denom;

            // y_stitched = (y1_overlap * A + y2_overlap * B) / (A + B)
            var yStitchedOverlap = new float[x1Overlap.Length];
            for (int i = 0; i < yStitchedOverlap.Length; i++)
            {
                float sum = A[i] + B[i];
                // In theory sum > 0 inside the overlap; guard against float quirks.
                if (sum == 0.0)
                {
                    // Fallback: average (mirrors the intuitive limit case)
                    yStitchedOverlap[i] = (y1Overlap[i] + y2Overlap[i])/2;
                }
                else
                {
                    yStitchedOverlap[i] = (y1Overlap[i] * A[i] + y2Overlap[i] * B[i]) / sum;
                }
            }

            // Before overlap: take from spectrum 1 (strictly before start)
            var (xBefore, yBefore) = WhereXY(x1, y1, v => v < overlapStart);

            // After overlap: take from spectrum 2 (strictly after end)
            var (xAfter, yAfter) = WhereXY(x2, y2, v => v > overlapEnd);

            // Concatenate like Python:
            // x_stitched = [x_before, x1_overlap, x_after]
            // y_stitched = [y_before, y_stitched_overlap, y_after]
            var xStitched = Concat3(xBefore, x1Overlap, xAfter);
            var yStitchedFinal = Concat3(yBefore, yStitchedOverlap, yAfter);

            // Return in Python-compatible shape: [x_stitched, y_stitched]
            return new List<List<float>>
            {
                xStitched.ToList(),
                yStitchedFinal.ToList()
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

    