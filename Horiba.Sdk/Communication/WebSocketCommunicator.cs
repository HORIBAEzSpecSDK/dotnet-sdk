using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Serilog;
using Websocket.Client;

namespace Horiba.Sdk.Communication;

/// <summary>
/// Encapsulates the communication between ICL and the SDK. This class is responsible for sending, receiving and logging
/// all commands sent to and from the ICL.
/// This class should not be used on its own. <see cref="Horiba.Sdk.Devices.DeviceManager"/> Is responsible for creating it.
/// All devices are using the same instance of the <see cref="WebSocketCommunicator"/> once generated.
/// </summary>
/// <param name="ipAddress">The IP address of the machine running the ICL process. Defaults to 127.0.0.1</param>
/// <param name="port">The port on which the ICL expects communication to happen. Defaults to </param>
public sealed class WebSocketCommunicator(IPAddress ipAddress, int port) : IDisposable
{
    private readonly WebsocketClient _wsClient = new(new Uri("ws://" + ipAddress + ":" + port));
    private readonly Uri _wsUri = new("ws://" + ipAddress + ":" + port);
    private readonly BlockingCollection<string> _messageQueue = new();
    private IDisposable? _messageSubscription;

    public bool IsConnectionOpened => _wsClient.IsRunning;

    public async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        Log.Debug("Opening WebSocket connection to: {@WsUri}", _wsUri);

        // Set up message subscription before connecting
        _messageSubscription = _wsClient.MessageReceived.Subscribe(msg =>
        {
            if (msg.MessageType == System.Net.WebSockets.WebSocketMessageType.Text && !string.IsNullOrEmpty(msg.Text))
            {
                _messageQueue.Add(msg.Text);
            }
        });

        await _wsClient.Start();

        // TODO decide if we truly need to support asynchronous communication from the ICL
        // Start listening for messages from the server in a separate task
        // _ = Task.Run(() => ReceiveMessage(_wsClient, cancellationToken));
    }

    /// <summary>
    /// Terminates the connection to the ICL
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        Log.Debug("Closing WebSocket connection");
        _wsClient.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "CloseConnectionAsync() method was invoked");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends a command to the ICL and waits for a response. If the response contains errors, an exception is thrown.
    /// </summary>
    /// <param name="command">The command to be sent</param>
    /// <param name="cancellationToken">A token for cancelling all long lasting tasks</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Thrown when there is an issue with receiving a response</exception>
    /// <exception cref="CommunicationException">Thrown when the ICL responds with an error</exception>
    public async Task<Response> SendWithResponseAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendInternalAsync(command, cancellationToken);

        var parsedResult = await ReceiveResponseAsync(cancellationToken);

        if (parsedResult is null) throw new NullReferenceException("Deserialization of the response failed");
        if (parsedResult.Errors.Count != 0) throw new CommunicationException(parsedResult.Errors.First());

        return parsedResult;
    }

    /// <summary>
    /// Sends a command to the ICL without waiting for a response. Just logs the echo of the command.
    /// </summary>
    /// <param name="command">The command to be sent</param>
    /// <param name="cancellationToken">A token for cancelling all long lasting tasks</param>
    public async Task SendAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendInternalWithResponseAsync(command, cancellationToken);
        // Response is received but ignored
    }
    
    /// <summary>
    /// Sends a command to the ICL and waits for a response. If the response contains errors, an exception is thrown.
    /// This method is used internally by the SDK to send commands and receive responses.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Response?> SendInternalWithResponseAsync(Command command, CancellationToken cancellationToken)
    {
        await SendInternalAsync(command, cancellationToken);
        return await ReceiveResponseAsync(cancellationToken);
    }

    private async Task SendInternalAsync(Command command, CancellationToken cancellationToken)
    {
        if (!IsConnectionOpened)
            throw new CommunicationException(
                "Connection is not established. Try opening connection before sending command");

        Log.Debug("Sending command: {@Command}", command);
        var message = Encoding.UTF8.GetString(command.ToByteArray());
        _wsClient.Send(message);
        await Task.CompletedTask;
    }

    private async Task<Response?> ReceiveResponseAsync(CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Wait for a message with timeout
                if (_messageQueue.TryTake(out var message, 30000, cancellationToken))
                {
                    Log.Debug("The response string is: {@Response}", message);
                    var parsedResult = JsonConvert.DeserializeObject<Response>(message);
                    return parsedResult;
                }
                else
                {
                    throw new TimeoutException("Timeout waiting for WebSocket response");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to receive or parse response");
                throw;
            }
        }, cancellationToken);
    }

    private async Task ReceiveMessage(WebsocketClient webSocket, CancellationToken cancellationToken)
    {
        // TODO if this is required feature, decide on the mechanism for handling incoming messages
        while (webSocket.IsRunning && !cancellationToken.IsCancellationRequested)
        {
            // This method is for future use if async message handling is needed
            await Task.Delay(100, cancellationToken);
        }
    }

    public void Dispose()
    {
        _messageSubscription?.Dispose();
        _messageQueue?.Dispose();
        _wsClient?.Dispose();
    }
}