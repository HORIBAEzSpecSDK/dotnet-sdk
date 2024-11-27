using System.Diagnostics.CodeAnalysis;

namespace Horiba.Sdk.Enums;

/// <summary> 
/// Predefined working speeds
/// </summary>
public record Speed(int Value)
{
    public int Value { get; } = Value;
    public static explicit operator int(Speed speed) => speed.Value;
}
