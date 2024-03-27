namespace Horiba.Sdk.Communication;

public interface ICommunicator
{
    Task OpenConnectionAsync(CancellationToken cancellationToken);
    Task CloseConnectionAsync(CancellationToken cancellationToken);
    Task<Response> SendAsync(Command command, CancellationToken cancellationToken);
    bool IsConnectionOpened { get; }
}