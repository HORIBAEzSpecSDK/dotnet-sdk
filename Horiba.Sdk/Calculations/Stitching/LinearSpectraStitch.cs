using System;
using System.Collections.Generic;
using System.Linq;
using Horiba.Sdk.Core.Internal;

namespace Horiba.Sdk.Core.Stitching
{
    /// <summary>
    /// Stitches a list of spectra using a linear model.
    /// 
    /// Input/Output shape parity with Python:
    /// - A single spectrum is represented as [x, [y]]
    ///   where x : List&lt;double&gt;, and [y] is a List containing one List&lt;double&gt;.
    /// - The stitched result is returned as the same shape: [x_combined, [y_combined]].
    /// 
    /// For convenience, the constructor accepts a list of such spectra.
    /// </summary>
    public sealed class LinearSpectraStitch : ISpectraStitch
    {
        // Stored as Python-compatible raw shape: [x, [y]]
        // x = List<double>, [y] = List<List<double>> with a single inner list
        private readonly List<object> _stitchedSpectrum;

        /// <summary>
        /// Construct from a list of spectra in Python-compatible shape: each spectrum = [x, [y]].
        /// Example (C#):
        /// var spectra = new List&lt;List&lt;object&gt;&gt;
        /// {
        ///     new() { x1, new List&lt;List&lt;double&gt;&gt; { y1 } },
        ///     new() { x2, new List&lt;List&lt;double&gt;&gt; { y2 } },
        /// };
        /// </summary>
        public LinearSpectraStitch(List<List<object>> spectraList)
        {
            if (spectraList == null || spectraList.Count == 0)
                throw new ArgumentException("spectraList must contain at least one spectrum.");

            // Start with the first spectrum as stitched base
            var stitched = spectraList[0];

            for (int i = 1; i < spectraList.Count; i++)
                stitched = StitchSpectra(stitched, spectraList[i]);

            _stitchedSpectrum = stitched;
        }

        public ISpectraStitch StitchWith(ISpectraStitch otherStitch)
        {
            if (otherStitch is not LinearSpectraStitch other)
                throw new ArgumentException("otherStitch must be a LinearSpectraStitch for exact parity.");

            var combined = StitchSpectra(
                (List<object>)this.StitchedSpectra(),
                (List<object>)other.StitchedSpectra()
            );

            return new LinearSpectraStitch(new List<List<object>> { combined });
        }

        public object StitchedSpectra() => _stitchedSpectrum;

        // ---- Implementation mirroring the Python algorithm ----

        private static List<object> StitchSpectra(List<object> spectrum1Raw, List<object> spectrum2Raw)
        {
            // Unpack Python-like shape: [x, [y]]
            var x1 = ToDoubleArray(spectrum1Raw[0]);
            var y1 = ToInnerYArray(spectrum1Raw[1]);

            var x2 = ToDoubleArray(spectrum2Raw[0]);
            var y2 = ToInnerYArray(spectrum2Raw[1]);

            // Sort while maintaining x-y correspondence
            var sort1 = NumpyShims.ArgSort(x1);
            var sort2 = NumpyShims.ArgSort(x2);

            var x1Sorted = NumpyShims.Take(x1, sort1);
            var y1Sorted = NumpyShims.Take(y1, sort1);

            var x2Sorted = NumpyShims.Take(x2, sort2);
            var y2Sorted = NumpyShims.Take(y2, sort2);

            // Overlap region
            double x1Min = x1Sorted[0], x1Max = x1Sorted[x1Sorted.Length - 1];
            double x2Min = x2Sorted[0], x2Max = x2Sorted[x2Sorted.Length - 1];

            double overlapStart = Math.Max(x1Min, x2Min);
            double overlapEnd   = Math.Min(x1Max, x2Max);

            if (overlapStart >= overlapEnd)
                throw new InvalidOperationException(
                    $"No overlapping region between spectra: [{x1Min}, {x1Max}] and [{x2Min}, {x2Max}]");

            // Masks on sorted arrays (inclusive)
            var mask1 = BuildInclusiveMask(x1Sorted, overlapStart, overlapEnd);
            var mask2 = BuildInclusiveMask(x2Sorted, overlapStart, overlapEnd);

            // Interpolate y2 onto x1 in overlap region
            var x1Overlap = NumpyShims.Mask(x1Sorted, mask1);
            var x2Overlap = NumpyShims.Mask(x2Sorted, mask2);
            var y2Overlap = NumpyShims.Mask(y2Sorted, mask2);

            var y2Interp = NumpyShims.Interp(x1Overlap, x2Overlap, y2Overlap);

            // Average overlap
            var y1Overlap = NumpyShims.Mask(y1Sorted, mask1);
            var yCombinedOverlap = new double[y1Overlap.Length];
            for (int i = 0; i < yCombinedOverlap.Length; i++)
                yCombinedOverlap[i] = 0.5 * (y1Overlap[i] + y2Interp[i]);

            // Combine non-overlap + overlap (order mirrors Python)
            var xNonOverlap1 = NumpyShims.Mask(x1Sorted, NumpyShims.Not(mask1));
            var yNonOverlap1 = NumpyShims.Mask(y1Sorted, NumpyShims.Not(mask1));

            var xNonOverlap2 = NumpyShims.Mask(x2Sorted, NumpyShims.Not(mask2));
            var yNonOverlap2 = NumpyShims.Mask(y2Sorted, NumpyShims.Not(mask2));

            var xCombined = NumpyShims.Concat(
                NumpyShims.Concat(xNonOverlap1, x1Overlap),
                xNonOverlap2
            );
            var yCombined = NumpyShims.Concat(
                NumpyShims.Concat(yNonOverlap1, yCombinedOverlap),
                yNonOverlap2
            );

            // Final sort by x
            var sortIdx = NumpyShims.ArgSort(xCombined);
            xCombined = NumpyShims.Take(xCombined, sortIdx);
            yCombined = NumpyShims.Take(yCombined, sortIdx);

            // Return in Python-compatible shape: [xCombined, [yCombined]]
            return new List<object>
            {
                xCombined.ToList(),
                new List<List<double>> { yCombined.ToList() }
            };
        }

        private static bool[] BuildInclusiveMask(double[] x, double start, double end)
        {
            var m = new bool[x.Length];
            for (int i = 0; i < x.Length; i++)
                m[i] = (x[i] >= start) && (x[i] <= end);
            return m;
        }

        // ---- Helpers to unpack Python-like shapes ----

        private static double[] ToDoubleArray(object o)
        {
            if (o is IEnumerable<double> ed) return ed.ToArray();
            throw new ArgumentException("Expected a List<double> for x.");
        }

        /// <summary>
        /// Expects [y] (a list containing one List<double>) and returns the inner List<double> as array.
        /// </summary>
        private static double[] ToInnerYArray(object o)
        {
            if (o is IEnumerable<IEnumerable<double>> outer)
            {
                var first = outer.FirstOrDefault();
                if (first is null) return Array.Empty<double>();
                return first.ToArray();
            }
            throw new ArgumentException("Expected a List<List<double>> for [y].");
        }
    }
}