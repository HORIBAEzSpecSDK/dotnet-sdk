using System;
using System.Collections.Generic;
using System.Linq;

namespace Horiba.Sdk.Core.Internal
{
    internal static class NumpyShims
    {
        /// <summary>
        /// Returns indices that would sort the array ascending (stable).
        /// </summary>
        public static int[] ArgSort(double[] values)
        {
            return Enumerable.Range(0, values.Length)
                             .OrderBy(i => values[i])
                             .ToArray();
        }

        /// <summary>
        /// Take array elements by index array.
        /// </summary>
        public static double[] Take(double[] values, int[] indices)
        {
            var r = new double[indices.Length];
            for (int i = 0; i < indices.Length; i++)
                r[i] = values[indices[i]];
            return r;
        }

        /// <summary>
        /// Boolean mask selection (true keeps element).
        /// </summary>
        public static double[] Mask(double[] values, bool[] mask)
        {
            var list = new List<double>(values.Length);
            for (int i = 0; i < values.Length; i++)
                if (mask[i]) list.Add(values[i]);
            return list.ToArray();
        }

        /// <summary>
        /// Logical NOT of mask.
        /// </summary>
        public static bool[] Not(bool[] mask)
        {
            var r = new bool[mask.Length];
            for (int i = 0; i < mask.Length; i++) r[i] = !mask[i];
            return r;
        }

        /// <summary>
        /// Concatenate two arrays.
        /// </summary>
        public static float[] Concat(float[] a, float[] b)
        {
            var r = new float[a.Length + b.Length];
            Array.Copy(a, 0, r, 0, a.Length);
            Array.Copy(b, 0, r, a.Length, b.Length);
            return r;
        }

        /// <summary>
        /// NumPy-like interp: piecewise linear, clamps outside xp to fp edges.
        /// Assumes xp is strictly increasing.
        /// </summary>
        public static double[] Interp(double[] x, double[] xp, double[] fp)
        {
            var y = new double[x.Length];

            // Edge clamps
            double xp0 = xp[0], xpN = xp[xp.Length - 1];
            double fp0 = fp[0], fpN = fp[fp.Length - 1];


            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];

                if (xi <= xp0) { y[i] = fp0; continue; }
                if (xi >= xpN) { y[i] = fpN; continue; }

                // Binary search for right interval [j-1, j]
                int j = Array.BinarySearch(xp, xi);
                if (j >= 0)
                {
                    // Exact match
                    y[i] = fp[j];
                    continue;
                }

                j = ~j; // first index greater than xi
                int j0 = j - 1;
                int j1 = j;

                double x0 = xp[j0], x1 = xp[j1];
                double y0 = fp[j0], y1 = fp[j1];

                double t = (xi - x0) / (x1 - x0);
                y[i] = y0 + t * (y1 - y0);
            }

            return y;
        }
    }
}