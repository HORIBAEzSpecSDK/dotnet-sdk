using System;
using System.Collections.Generic;
using System.Linq;
using Horiba.Sdk.Core.Internal;

namespace Horiba.Sdk.Core.Stitching
{
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
    public sealed class SimpleCutSpectraStitch : ISpectraStitch
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
        public SimpleCutSpectraStitch(List<List<object>> spectraList)
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
            if (otherStitch is not SimpleCutSpectraStitch other)
                throw new ArgumentException("otherStitch must be a SimpleCutSpectraStitch for exact parity.");

            var combined = StitchSpectra(
                (List<object>)this.StitchedSpectra(),
                (List<object>)other.StitchedSpectra()
            );

            return new SimpleCutSpectraStitch(new List<List<object>> { combined });
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
            return new List<object>
            {
                xStitched.ToList(),
                yStitched.ToList()
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
            // Use existing shim for 2-way concat to keep consistency
            return NumpyShims.Concat(NumpyShims.Concat(a, b), c);
        }
    }
}