using Serilog;

namespace Horiba.Sdk.Enums;

public static class SignalExtensions
{
    private const string AddressTokenName = "address";
    private const string EventTokenName = "event";
    private const string SignalTypeTokenName = "signalType";
    
    /// <summary>
    /// Converts the result of a command to a Signal.
    /// </summary>
    /// <param name="commandResult">The Results collection of a command response</param>
    /// <returns></returns>
    /// <exception cref="CommunicationException"></exception>
    public static Signal ToSignal(this Dictionary<string, object> commandResult)
    {
        if (!commandResult.TryGetValue(AddressTokenName, out var signalAddress) ||
            !commandResult.TryGetValue(EventTokenName, out var signalEvent) ||
            !commandResult.TryGetValue(SignalTypeTokenName, out var signalType))
            throw new CommunicationException("Command could not be parsed to a Trigger.");
        try
        {
            return new Signal(
                (SignalAddress)int.Parse(signalAddress.ToString()),
                (SignalEvent)int.Parse(signalEvent.ToString()),
                (SignalType)int.Parse(signalType.ToString()));
        }
        catch (Exception e)
        {
            Log.Error(e, "CommandResult did not contain the expected Trigger values");
                
            throw;
        }
    }
}

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