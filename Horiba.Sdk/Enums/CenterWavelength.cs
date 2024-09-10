using System.Linq;

namespace Horiba.Sdk.Enums;

public record CenterWavelength(float WavelengthValue)
{
    public float WavelengthValue { get; } = WavelengthValue;
}
