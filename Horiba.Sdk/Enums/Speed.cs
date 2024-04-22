using System.Diagnostics.CodeAnalysis;

namespace Horiba.Sdk.Enums;

/// <summary> 
/// Predefined working speeds
/// </summary>
public abstract record Speed(int Value)
{
    public int Value { get; } = Value;
    public static explicit operator int(Speed speed) => speed.Value;
}

/// <summary>
/// Represents predefined Speed values for SyncerityOE devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityOESpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 45 kHz 
    /// </summary>
    public static readonly SyncerityOESpeed kHz45 = new(0);
    
    /// <summary>
    /// Defines speed of 1 MHz
    /// </summary>
    public static readonly SyncerityOESpeed MHz1 = new(1);
    
    /// <summary>
    /// Defines speed of 1 MHz Ultra
    /// </summary>
    public static readonly SyncerityOESpeed MHz1U = new(2);
    
    public static explicit operator SyncerityOESpeed(int v)
    {
        return v switch
        {
            0 => kHz45,
            1 => MHz1,
            2 => MHz1U,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for SyncerityNIR devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityNIRSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 45 kHz 
    /// </summary>
    public static readonly SyncerityNIRSpeed kHz45 = new(0);
    
    /// <summary>
    /// Defines speed of 500 kHz
    /// </summary>
    public static readonly SyncerityNIRSpeed kHz500 = new(1);
    
    /// <summary>
    /// Defines speed of 500 kHz Ultra
    /// </summary>
    public static readonly SyncerityNIRSpeed kHz500U = new(2);
    
    public static explicit operator SyncerityNIRSpeed(int v)
    {
        return v switch
        {
            0 => kHz45,
            1 => kHz500,
            2 => kHz500U,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for SyncerityUVVis devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityUVVisSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 45 kHz 
    /// </summary>
    public static readonly SyncerityUVVisSpeed kHz45 = new(0);
    
    /// <summary>
    /// Defines speed of 500 kHz
    /// </summary>
    public static readonly SyncerityUVVisSpeed kHz500 = new(1);
    
    /// <summary>
    /// Defines speed of 500 kHz Ultra
    /// </summary>
    public static readonly SyncerityUVVisSpeed kHz500U = new(2);
    
    public static explicit operator SyncerityUVVisSpeed(int v)
    {
        return v switch
        {
            0 => kHz45,
            1 => kHz500,
            2 => kHz500U,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for SynapseCCD devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseCCDSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 20 kHz 
    /// </summary>
    public static readonly SynapseCCDSpeed kHz20 = new(0);
    
    /// <summary>
    /// Defines speed of 1 MHz
    /// </summary>
    public static readonly SynapseCCDSpeed MHz1 = new(1);
    
    /// <summary>
    /// Defines speed of 1 MHz Ultra
    /// </summary>
    public static readonly SynapseCCDSpeed MHz1U = new(2);
    
    public static explicit operator SynapseCCDSpeed(int v)
    {
        return v switch
        {
            0 => kHz20,
            1 => MHz1,
            2 => MHz1U,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for Symphony2 CCD devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Symphony2CCDSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 20 kHz 
    /// </summary>
    public static readonly Symphony2CCDSpeed kHz20 = new(0);
    
    /// <summary>
    /// Defines speed of 1 MHz
    /// </summary>
    public static readonly Symphony2CCDSpeed MHz1 = new(1);
    
    /// <summary>
    /// Defines speed of 1 MHz Ultra
    /// </summary>
    public static readonly Symphony2CCDSpeed MHz1U = new(2);
    
    public static explicit operator Symphony2CCDSpeed(int v)
    {
        return v switch
        {
            0 => kHz20,
            1 => MHz1,
            2 => MHz1U,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for SynapseIGA devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseIGASpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 300 kHz 
    /// </summary>
    public static readonly SynapseIGASpeed kHz300 = new(0);
    
    /// <summary>
    /// Defines speed of 1.5 MHz
    /// </summary>
    public static readonly SynapseIGASpeed MHz1_5 = new(1);
    
    public static explicit operator SynapseIGASpeed(int v)
    {
        return v switch
        {
            0 => kHz300,
            1 => MHz1_5,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for Symphony2 IGA devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Symphony2IGASpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 300 kHz 
    /// </summary>
    public static readonly Symphony2IGASpeed kHz300 = new(0);
    
    /// <summary>
    /// Defines speed of 1.5 MHz
    /// </summary>
    public static readonly Symphony2IGASpeed MHz1_5 = new(1);
    
    public static explicit operator Symphony2IGASpeed(int v)
    {
        return v switch
        {
            0 => kHz300,
            1 => MHz1_5,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for Synapse Plus devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapsePlusSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 50 kHz 
    /// </summary>
    public static readonly SynapsePlusSpeed kHz50 = new(0);
    
    /// <summary>
    /// Defines speed of 1 MHz
    /// </summary>
    public static readonly SynapsePlusSpeed MHz1 = new(1);
    
    /// <summary>
    /// Defines speed of 3 MHz
    /// </summary>
    public static readonly SynapsePlusSpeed MHz3 = new(2);
    
    public static explicit operator SynapsePlusSpeed(int v)
    {
        return v switch
        {
            0 => kHz50,
            1 => MHz1,
            2 => MHz3,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined Speed values for Synapse EM devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseEMSpeed(int Value) : Speed(Value)
{
    /// <summary>
    /// Defines speed of 50 kHz HS
    /// </summary>
    public static readonly SynapseEMSpeed kHz50HS = new(0);
    
    /// <summary>
    /// Defines speed of 1 MHz HS
    /// </summary>
    public static readonly SynapseEMSpeed MHz1HS = new(1);
    
    /// <summary>
    /// Defines speed of 3 MHz HS
    /// </summary>
    public static readonly SynapseEMSpeed MHz3HS = new(2);
    
    /// <summary>
    /// Defines speed of 50 kHz EM
    /// </summary>
    public static readonly SynapseEMSpeed KHz50EmSpeed = new(3);
    
    /// <summary>
    /// Defines speed of 1 MHz EM
    /// </summary>
    public static readonly SynapseEMSpeed MHz1EmSpeed = new(4);
    
    /// <summary>
    /// Defines speed of 3 MHz EM
    /// </summary>
    public static readonly SynapseEMSpeed MHz3EmSpeed = new(5);
    
    public static explicit operator SynapseEMSpeed(int v)
    {
        return v switch
        {
            0 => kHz50HS,
            1 => MHz1HS,
            2 => MHz3HS,
            3 => KHz50EmSpeed,
            4 => MHz1EmSpeed,
            5 => MHz3EmSpeed,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}
