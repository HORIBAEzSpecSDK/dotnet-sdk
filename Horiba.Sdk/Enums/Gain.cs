using System.Diagnostics.CodeAnalysis;

namespace Horiba.Sdk.Enums;

/// <summary>
/// Predefined gain values
/// </summary>
/// <param name="Value"></param>
public record Gain(int Value)
{
    public int Value { get; } = Value;
    public static explicit operator int(Gain gain) => gain.Value;
}