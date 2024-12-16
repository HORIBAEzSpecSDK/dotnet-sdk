//namespace Horiba.Sdk.Calculations.Stitching;

//public class LinearSpectraStitch(List<XYData>[] data) : SpectraStitch(data)
//{
//    public override List<XYData> Stitch()
//    {
//        var stitched = new List<XYData>();

//        var orderedData = Data.Select(d => d.OrderBy(xy => xy.X).ToList()).ToArray();

//        for (var i = 0; i < orderedData.Length - 1; i++)
//        {
//            var spectrum1 = i == 0 ? orderedData[i] : stitched;
//            var spectrum2 = orderedData[i + 1];

//            var overlapStart = Math.Max(spectrum1[0].X, spectrum2[0].X);
//            var overlapEnd = Math.Min(spectrum1[^1].X, spectrum2[^1].X);

//            var newStitched = new List<XYData>();

//            newStitched.AddRange(spectrum1.Where(d => d.X < overlapStart));

//            var spectrum1Overlapped = spectrum1.Where(d => d.X >= overlapStart).ToArray();
//            var spectrum2Overlapped = spectrum2.Where(d => d.X <= overlapEnd).ToArray();

//            newStitched.AddRange(spectrum1Overlapped.Select((d, index) => new XYData(d.X, (d.Y + spectrum2Overlapped[index].Y) / 2)));

//            newStitched.AddRange(spectrum2.Where(d => d.X > overlapEnd));

//            stitched = newStitched;
//        }

//        return stitched;
//    }
//}
