using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using Serilog;

namespace Horiba.Sdk.Calculations.Stitching;

//public abstract class SpectraStitch
//{
//    public abstract SpectraStitch StitchWith(SpectraStitch otherStitch);
//    public abstract List<List<float>> StitchedSpectra();
//}

public class LinearSpectraStitch : SpectraStitch
    {
        private readonly List<List<float>> _stitchedSpectrum;

        public LinearSpectraStitch(List<List<List<float>>> spectraList)
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

        private List<List<float>> StitchSpectra(List<List<float>> spectrum1, List<List<float>> spectrum2)
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

            // Calculate true overlap region
            var x1Min = x1.First();
            var x1Max = x1.Last();
            var x2Min = x2.First();
            var x2Max = x2.Last();

            var overlapStart = Math.Max(x1Min, x2Min);
            var overlapEnd = Math.Min(x1Max, x2Max);

            Log.Debug("Spectrum 1 range: {X1Min} to {X1Max}", x1Min, x1Max);
            Log.Debug("Spectrum 2 range: {X2Min} to {X2Max}", x2Min, x2Max);
            Log.Debug("Overlap region: {OverlapStart} to {OverlapEnd}", overlapStart, overlapEnd);

            if (overlapStart >= overlapEnd)
            {
                Log.Error($"No overlap between spectra: [{x1Min}, {x1Max}] and [{x2Min}, {x2Max}]");
                throw new Exception("No overlapping region between spectra");
            }

            // Create masks for overlapping regions using sorted arrays
            var mask1 = x1.Select(x => x >= overlapStart && x <= overlapEnd).ToArray();
            var mask2 = x2.Select(x => x >= overlapStart && x <= overlapEnd).ToArray();

            var x2Sorted = x2.Where((_, i) => mask2[i]).Select(v => (double)v).ToArray();
            var y2Sorted = y2.Where((_, i) => mask2[i]).Select(v => (double)v).ToArray();
            var x1Sorted = x1.Where((_, i) => mask1[i]).Select(v => (double)v).ToArray();
            
            // Interpolate second spectrum onto first spectrum's x points in overlap region
            var linearSpline = LinearSpline.InterpolateSorted(x2Sorted, y2Sorted);
            var y2Interp = x1Sorted.Select(x => linearSpline.Interpolate(x)).ToArray();
            
            // Average the overlapping region
            var yCombinedOverlap = y1.Where((_, i) => mask1[i]).Zip(y2Interp, (y1Val, y2Val) => (y1Val + (float)y2Val) / 2).ToArray();

            // Combine non-overlapping and overlapping regions
            var xCombined = x1.Where((_, i) => !mask1[i]).Concat<float>(x1.Where((_, i) => mask1[i])).Concat<float>(x2.Where((_, i) => !mask2[i])).ToArray();
            var yCombined = y1.Where((_, i) => !mask1[i]).Concat<float>(yCombinedOverlap).Concat<float>(y2.Where((_, i) => !mask2[i])).ToArray();

            // Ensure final result is sorted
            var sortIndices = Enumerable.Range(0, xCombined.Length).OrderBy(i => xCombined[i]).ToArray();
            xCombined = sortIndices.Select(i => xCombined[i]).ToArray();
            yCombined = sortIndices.Select(i => yCombined[i]).ToArray();

            return new List<List<float>> { xCombined.ToList(), yCombined.ToList() };
        }
    }