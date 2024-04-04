using System.Net;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Communication;

public sealed class WebSocketCommunicator
{
    private const int Port = 25010;
    private readonly IPAddress _ip = IPAddress.Loopback;
    private readonly ClientWebSocket _wsClient = new();
    private readonly Uri _wsUri;

    public WebSocketCommunicator()
    {
        _wsUri = new Uri("ws://" + _ip + ":" + Port);
    }

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

        // TODO this is hard limit! Check what is the maximum info that can be sent and its configuration options
        var parsedResult = await ReceiveResponseAsync(cancellationToken);
        Log.Debug("Receiving response: {@Response}", parsedResult);

        if (parsedResult is null) throw new NullReferenceException("Deserialization of the response failed");
        if (parsedResult.Errors.Count != 0)
        {
            throw new CommunicationException(parsedResult.Errors.First());
        }

        return parsedResult;
    }

    public async Task SendAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendInternalAsync(command, cancellationToken);
        var echo = await ReceiveResponseAsync(cancellationToken);
        Log.Debug("ECHO set command: {@Response}", echo);
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
        var singleResponseBuffer = new byte[1024*4];
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