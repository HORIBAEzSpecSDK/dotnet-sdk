using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public record ChargedCoupledDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator)
{
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdIsConnectionOpenedCommand(DeviceId), cancellationToken);
        
        if (result.Results.TryGetValue("open", out var bR))
        {
            return bool.Parse(bR.ToString()); // ?? ToString() ??
        }

        return false;
    }

    public override async Task<Response> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await Communicator.SendWithResponseAsync(new CcdOpenCommand(DeviceId), cancellationToken);
    }

    public override async Task<Response> CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await Communicator.SendWithResponseAsync(new CcdCloseCommand(DeviceId), cancellationToken);
    }

    public async Task<int> GetChipTemperatureAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetTemperatureCommand(DeviceId), cancellationToken);
        return (int)result.Results["temperature"];
    }

    public async Task<(int Width, int Height)> GetChipSizeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetChipSizeCommand(DeviceId), cancellationToken);
        return ((int)result.Results["x"], (int)result.Results["y"]);
    }

    public async Task<int> GetSpeedAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetSpeedCommand(DeviceId), cancellationToken);
        return (int)result.Results["info"];
    }

    public async Task<int> GetExposureTimeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetExposureTimeCommand(DeviceId), cancellationToken);
        return (int)result.Results["time"];
    }

    public async Task SetExposureTimeAsync(int exposureTimeInMs, CancellationToken cancellationToken = default)
    {
        await Communicator.SendWithResponseAsync(new CcdSetExposureTimeCommand(DeviceId, exposureTimeInMs), cancellationToken);
    }
}