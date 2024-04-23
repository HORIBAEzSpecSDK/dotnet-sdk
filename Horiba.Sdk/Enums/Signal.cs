namespace Horiba.Sdk.Enums;

public record Signal(SignalAddress SignalAddress, SignalEvent SignalEvent, SignalType SignalType)
{
    public static readonly Signal Default = new(SignalAddress.Input, SignalEvent.ShutterOpen, SignalType.ActiveHigh);
    
    public SignalAddress SignalAddress { get; } = SignalAddress;
    public SignalEvent SignalEvent { get; } = SignalEvent;
    public SignalType SignalType { get; } = SignalType;
}

/// <summary>
/// Token used to specify how the signal will cause the event.
/// </summary>
public enum SignalType
{
    ActiveHigh = 0,
    ActiveLow = 1
}

/// <summary>
///  Token used to specify when the signal event should occur.
/// </summary>
public enum SignalEvent
{
    StartExperiment = 0,
    ShutterOpen = 3
}

/// <summary>
/// Token used to specify where the signal is located.
/// </summary>
public enum SignalAddress
{
    Disabled = -1,
    Output = 0,
    Input = 1
}