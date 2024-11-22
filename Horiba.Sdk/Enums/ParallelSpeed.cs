namespace Horiba.Sdk.Enums;

/// <summary> 
/// Predefined working parallel speeds
/// </summary>
public record ParallelSpeed(int Value)
    {
        public int Value { get; } = Value;
        public static explicit operator int(ParallelSpeed parallelSpeed) => parallelSpeed.Value;
    }
