using Serilog;

namespace Horiba.Sdk.Enums;

public static class TriggerExtensions
{
    private const string AddressTokenName = "address";
    private const string EventTokenName = "event";
    private const string SignalTypeTokenName = "signalType";
    
    public static Trigger ToTrigger(this Dictionary<string, object> commandResult)
    {
        if (!commandResult.TryGetValue(AddressTokenName, out var triggerAddress) ||
            !commandResult.TryGetValue(EventTokenName, out var triggerEvent) ||
            !commandResult.TryGetValue(SignalTypeTokenName, out var triggerSignalType))
            throw new CommunicationException("Command could not be parsed to a Trigger.");
        try
        {
            return new Trigger(
                (TriggerAddress)int.Parse(triggerAddress.ToString()),
                (TriggerEvent)int.Parse(triggerEvent.ToString()),
                (TriggerSignalType)int.Parse(triggerSignalType.ToString()));
        }
        catch (Exception e)
        {
            Log.Error(e, "CommandResult did not contain the expected Trigger values");
                
            throw;
        }
    }
}

public record Trigger(TriggerAddress TriggerAddress, TriggerEvent TriggerEvent, TriggerSignalType TriggerSignalType)
{
    public static readonly Trigger Default = new(TriggerAddress.Input, TriggerEvent.Once, TriggerSignalType.FallingEdge);
    
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