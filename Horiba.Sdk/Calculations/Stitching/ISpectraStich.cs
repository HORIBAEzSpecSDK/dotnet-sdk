using System.Collections.Generic;

namespace Horiba.Sdk.Core.Stitching
{
    /// <summary>
    /// Stitches multiple spectra into a single spectrum.
    /// </summary>
    public interface ISpectraStitch
    {
        /// <summary>
        /// Stitches this stitch with another stitch.
        /// </summary>
        ISpectraStitch StitchWith(ISpectraStitch otherStitch);

        /// <summary>
        /// Returns the raw data of the stitched spectra.
        /// Python-compatible shape: [x, [y]]
        /// </summary>
        object StitchedSpectra();
    }
}