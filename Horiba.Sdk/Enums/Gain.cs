using System.Diagnostics.CodeAnalysis;

namespace Horiba.Sdk.Enums;

/// <summary>
/// Predefined gain values
/// </summary>
/// <param name="Value"></param>
public abstract record Gain(int Value)
{
    public int Value { get; } = Value;
    public static explicit operator int(Gain gain) => gain.Value;
}

/// <summary>
/// Represents predefined gain values for SyncerityOE devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityOEGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SyncerityOEGain HighLight = new(0);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SyncerityOEGain BestDynamicRange = new(1);
    
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SyncerityOEGain HighSensitivity = new(2);
    
    public static explicit operator SyncerityOEGain(int v)
    {
        return v switch
        {
            0 => HighLight,
            1 => BestDynamicRange,
            2 => HighSensitivity,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for SyncerityNIR devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityNIRGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SyncerityNIRGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SyncerityNIRGain BestDynamicRange = new(1);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SyncerityNIRGain HighLight = new(2);
    
    public static explicit operator SyncerityNIRGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => BestDynamicRange,
            2 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for SyncerityUVVis devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SyncerityUVVisGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SyncerityUVVisGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SyncerityUVVisGain BestDynamicRange = new(1);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SyncerityUVVisGain HighLight = new(2);
    
    public static explicit operator SyncerityUVVisGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => BestDynamicRange,
            2 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for SynapseCCD devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseCCDGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SynapseCCDGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SynapseCCDGain BestDynamicRange = new(1);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SynapseCCDGain HighLight = new(2);
    
public static explicit operator SynapseCCDGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => BestDynamicRange,
            2 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for Symphony2CCD devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Symphony2CCDGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly Symphony2CCDGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly Symphony2CCDGain BestDynamicRange = new(1);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly Symphony2CCDGain HighLight = new(2);
    
    public static explicit operator Symphony2CCDGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => BestDynamicRange,
            2 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for SynapseIGA devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseIGAGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SynapseIGAGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of HighDynamic
    /// </summary>
    public static readonly SynapseIGAGain HighDynamic = new(1);
    
    public static explicit operator SynapseIGAGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => HighDynamic,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for Symphony2IGA devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Symphony2IGAGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly Symphony2IGAGain HighSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of HighDynamic
    /// </summary>
    public static readonly Symphony2IGAGain HighDynamic = new(1);
    
    public static explicit operator Symphony2IGAGain(int v)
    {
        return v switch
        {
            0 => HighSensitivity,
            1 => HighDynamic,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for Synapse Plus devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapsePlusGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of UltimateSensitivity
    /// </summary>
    public static readonly SynapsePlusGain UltimateSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SynapsePlusGain HighSensitivity = new(1);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SynapsePlusGain BestDynamicRange = new(2);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SynapsePlusGain HighLight = new(3);
    
    public static explicit operator SynapsePlusGain(int v)
    {
        return v switch
        {
            0 => UltimateSensitivity,
            1 => HighSensitivity,
            2 => BestDynamicRange,
            3 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}

/// <summary>
/// Represents predefined gain values for Synapse Plus devices
/// </summary>
/// <param name="Value"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record SynapseEMGain(int Value) : Gain(Value)
{
    /// <summary>
    /// Defines gain of UltimateSensitivity
    /// </summary>
    public static readonly SynapseEMGain UltimateSensitivity = new(0);
    
    /// <summary>
    /// Defines gain of HighSensitivity
    /// </summary>
    public static readonly SynapseEMGain HighSensitivity = new(1);
    
    /// <summary>
    /// Defines gain of BestDynamicRange
    /// </summary>
    public static readonly SynapseEMGain BestDynamicRange = new(2);
    
    /// <summary>
    /// Defines gain of HighLight
    /// </summary>
    public static readonly SynapseEMGain HighLight = new(3);
    
    public static explicit operator SynapseEMGain(int v)
    {
        return v switch
        {
            0 => UltimateSensitivity,
            1 => HighSensitivity,
            2 => BestDynamicRange,
            3 => HighLight,
            _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
        }; 
    }
}