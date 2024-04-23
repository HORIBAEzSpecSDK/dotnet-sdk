namespace Horiba.Sdk.Enums;

public record Trigger(TriggerAddress TriggerAddress, TriggerEvent TriggerEvent, TriggerSignalType TriggerSignalType)
{
    public static Trigger Default = new(TriggerAddress.Input, TriggerEvent.Once, TriggerSignalType.FallingEdge);
    
    public TriggerAddress TriggerAddress { get; } = TriggerAddress;
    public TriggerEvent TriggerEvent { get; } = TriggerEvent;
    public TriggerSignalType TriggerSignalType { get; } = TriggerSignalType;
}

/// <summary>
/// Token used to specify how the signal will cause the input trigger.
/// </summary>
public enum TriggerSignalType
{
    /// <summary>
    /// TTL Falling Edge
    /// </summary>
    FallingEdge = 0,
    
    /// <summary>
    /// TTL Rising Edge
    /// </summary>
    RisingEdge = 1
}

/// <summary>
/// Token used to specify when the trigger event should occur.
/// </summary>
public enum TriggerEvent
{
    /// <summary>
    /// For all acquisitions only once
    /// </summary>
    Once = 0,
    
    /// <summary>
    /// For each acquisition
    /// </summary>
    Each = 1
}

/// <summary>
/// Token used to specify where the trigger is located.
/// </summary>
public enum TriggerAddress
{
    Disabled = -1,
    Input = 0,
    Output = 1
}