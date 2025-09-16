using System;
using System.Collections.Generic;
using System.Linq;
using Horiba.Sdk.Core.Internal;

namespace Horiba.Sdk.Core.Stitching
{
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
    public sealed class LabSpec6SpectraStitch : ISpectraStitch
    {
        // Stored as Python-compatible raw shape: [x, y]
        private readonly List<object> _stitchedSpectrum;

        /// <summary>
        /// Construct from a list of spectra, each in Python-compatible shape: [x, y].
        /// Example (C#):
        /// var spectra = new List&lt;List&lt;object&gt;&gt;
        /// {
        ///     new() { x1, y1 },
        ///     new() { x2, y2 },
        /// };
        /// </summary>
        public LabSpec6SpectraStitch(List<List<object>> spectraList)
        {
            if (spectraList == null || spectraList.Count == 0)
                throw new ArgumentException("spectraList must contain at least one spectrum.");

            var stitched = spectraList[0];

            for (int i = 1; i < spectraList.Count; i++)
                stitched = StitchSpectra(stitched, spectraList[i]);

            _stitchedSpectrum = stitched;
        }

        public ISpectraStitch StitchWith(ISpectraStitch otherStitch)
        {
            if (otherStitch is not LabSpec6SpectraStitch other)
                throw new ArgumentException("otherStitch must be a LabSpec6SpectraStitch for exact parity.");

            var combined = StitchSpectra(
                (List<object>)this.StitchedSpectra(),
                (List<object>)other.StitchedSpectra()
            );

            return new LabSpec6SpectraStitch(new List<List<object>> { combined });
        }

        public object StitchedSpectra() => _stitchedSpectrum;

        // ---------- Core algorithm (parity with Python) ----------

        private static List<object> StitchSpectra(List<object> spectrum1Raw, List<object> spectrum2Raw)
        {
            // Unpack Python-like shape: [x, y]
            var x1 = ToDoubleArray(spectrum1Raw[0]);
            var y1 = ToDoubleArray(spectrum1Raw[1]);

            var x2 = ToDoubleArray(spectrum2Raw[0]);
            var y2 = ToDoubleArray(spectrum2Raw[1]);

            if (x1.Length == 0 || x2.Length == 0)
                throw new ArgumentException("Spectra must contain at least one x/y pair.");

            // Overlap region (assumes x are ordered; no sorting in Python version)
            double overlapStart = Math.Max(x1[0], x2[0]);
            double overlapEnd   = Math.Min(x1[x1.Length - 1], x2[x2.Length - 1]);

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
            double denom = (overlapEnd - overlapStart);
            if (denom == 0.0)
                throw new InvalidOperationException("Degenerate overlap (start equals end).");

            var A = new double[x1Overlap.Length];
            var B = new double[x2Overlap.Length];

            for (int i = 0; i < x1Overlap.Length; i++)
                A[i] = (x1Overlap[i] - overlapStart) / denom;

            for (int i = 0; i < x2Overlap.Length; i++)
                B[i] = (overlapEnd - x2Overlap[i]) / denom;

            // y_stitched = (y1_overlap * A + y2_overlap * B) / (A + B)
            var yStitchedOverlap = new double[x1Overlap.Length];
            for (int i = 0; i < yStitchedOverlap.Length; i++)
            {
                double sum = A[i] + B[i];
                // In theory sum > 0 inside the overlap; guard against float quirks.
                if (sum == 0.0)
                {
                    // Fallback: average (mirrors the intuitive limit case)
                    yStitchedOverlap[i] = 0.5 * (y1Overlap[i] + y2Overlap[i]);
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
            return new List<object>
            {
                xStitched.ToList(),
                yStitchedFinal.ToList()
            };
        }

        // ---------- Helpers ----------

        private static double[] ToDoubleArray(object o)
        {
            if (o is IEnumerable<double> ed) return ed.ToArray();
            throw new ArgumentException("Expected a List<double> for x or y.");
        }

        private static (double[] xs, double[] ys) WhereXY(double[] x, double[] y, Func<double, bool> predicate)
        {
            var xs = new List<double>();
            var ys = new List<double>();
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

        private static double[] Concat3(double[] a, double[] b, double[] c)
        {
            return NumpyShims.Concat(NumpyShims.Concat(a, b), c);
        }
    }
}