using System.Net;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

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
    
    public Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return _wsClient.ConnectAsync(_wsUri, cancellationToken);
    }

    public Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
    }

    public async Task<Response> SendWithResponseAsync(Command command, CancellationToken cancellationToken = default)
    {
        await SendAsync(command, cancellationToken);

        // TODO this is hard limit! Check what is the maximum info that can be sent and its configuration options
        var responseBuffer = new byte[1024]; //2048*512 // we need to wait for at least 300ms - 500ms for a flag to be set
        var wsResponse = await _wsClient.ReceiveAsync(new ArraySegment<byte>(responseBuffer), cancellationToken);
        
        var res = Encoding.UTF8.GetString(responseBuffer, 0, wsResponse.Count);
        var parsedResult = JsonConvert.DeserializeObject<Response>(res);

        if (parsedResult is null)
        {
            throw new NullReferenceException("Deserialization of the response failed");
        }

        return parsedResult;
    }

    public Task SendAsync(Command command, CancellationToken cancellationToken = default)
    {
        if (!IsConnectionOpened)
        {
            throw new WebSocketException("Connection is not established. Try connecting before sending command");
        }

        return _wsClient.SendAsync(new ArraySegment<byte>(command.ToByteArray()), WebSocketMessageType.Text, true,
            cancellationToken);
    }

    public bool IsConnectionOpened => _wsClient.State == WebSocketState.Open;
}