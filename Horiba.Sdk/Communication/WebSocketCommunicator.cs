using System.Net;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Communication;

/// <summary>
/// Encapsulates the communication between ICL and the SDK. This class is responsible for sending, receiving and logging
/// all commands sent to and from the ICL.
/// This class should not be used on its own. <see cref="Horiba.Sdk.Devices.DeviceManager"/> Is responsible for creating it.
/// All devices are using the same instance of the <see cref="WebSocketCommunicator"/> once generated.
/// </summary>
/// <param name="ipAddress">The IP address of the machine running the ICL process. Defaults to 127.0.0.1</param>
/// <param name="port">The port on which the ICL expects communication to happen. Defaults to </param>
public sealed class WebSocketCommunicator(IPAddress ipAddress, int port)
{
    private readonly ClientWebSocket _wsClient = new();
    private readonly Uri _wsUri = new("ws://" + ipAddress + ":" + port);

    public bool IsConnectionOpened => _wsClient.State == WebSocketState.Open;

    public Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        Log.Debug("Opening WebSocket connection to: {@WsUri}", _wsUri);
        return _wsClient.ConnectAsync(_wsUri, cancellationToken);

        // TODO decide if we truly need to support asynchronous communication from the ICL
        // Start listening for messages from the server in a separate task
        // _ = Task.Run(() => ReceiveMessage(_wsClient, cancellationToken));
    }

    public Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        Log.Debug("Closing WebSocket connection");
        return _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "CloseConnectionAsync() method was invoked",
            cancellationToken);
    }

    public async Task<Response> SendWithResponseAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendInternalAsync(command, cancellationToken);


        var parsedResult = await ReceiveResponseAsync(cancellationToken);
        Log.Debug("Receiving response: {@Response}", parsedResult);

        if (parsedResult is null) throw new NullReferenceException("Deserialization of the response failed");
        if (parsedResult.Errors.Count != 0) throw new CommunicationException(parsedResult.Errors.First());

        return parsedResult;
    }

    public async Task SendAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendInternalAsync(command, cancellationToken);
        var echo = await ReceiveResponseAsync(cancellationToken);
        Log.Debug("ECHO sent command: {@Response}", echo);
    }

    private async Task SendInternalAsync(Command command, CancellationToken cancellationToken)
    {
        if (!IsConnectionOpened)
            throw new CommunicationException(
                "Connection is not established. Try opening connection before sending command");

        Log.Debug("Sending command: {@Command}", command);
        await _wsClient.SendAsync(new ArraySegment<byte>(command.ToByteArray()), WebSocketMessageType.Text, true,
            cancellationToken);
    }

    private async Task<Response?> ReceiveResponseAsync(CancellationToken cancellationToken)
    {
        var singleResponseBuffer = new byte[1024 * 4];
        await _wsClient.ReceiveAsync(new ArraySegment<byte>(singleResponseBuffer), cancellationToken);

        var res = Encoding.UTF8.GetString(singleResponseBuffer, 0, singleResponseBuffer.Length);
        var parsedResult = JsonConvert.DeserializeObject<Response>(res);
        return parsedResult;
    }

    private async Task ReceiveMessage(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new ArraySegment<byte>(new byte[1024]);
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
            // add result to threadSafe Queue
            // maybe add polling delay 
        }
    }
}