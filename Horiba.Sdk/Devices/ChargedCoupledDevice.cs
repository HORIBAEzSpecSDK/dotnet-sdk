using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public record ChargedCoupledDevice(int DeviceId, WebSocketCommunicator Communicator) : Device(DeviceId, Communicator)
{
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdIsConnectionOpenedCommand(DeviceId), cancellationToken);

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
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTemperatureCommand(DeviceId), cancellationToken);
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
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetExposureTimeCommand(DeviceId), cancellationToken);
        return (int)result.Results["time"];
    }

    public async Task SetExposureTimeAsync(int exposureTimeInMs, CancellationToken cancellationToken = default)
    {
        await Communicator.SendWithResponseAsync(new CcdSetExposureTimeCommand(DeviceId, exposureTimeInMs),
            cancellationToken);
    }

    public async Task<bool> GetAcquisitionReadyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionReadyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["ready"];
    }

    public async Task SetAcquisitionStartAsync(bool isShutterOpened, CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdSetAcquisitionStartCommand(DeviceId, isShutterOpened),
                cancellationToken);
    }

    public async Task SetRegionOfInterestAsync(RegionOfInterest regionOfInterest,
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdSetRegionOfInterestCommand(DeviceId, regionOfInterest),
                cancellationToken);
    }

    public async Task<Response> GetAcquisitionDataAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionDataCommand(DeviceId), cancellationToken);
        return result;
    }

    public async Task<bool> GetAcquisitionBusyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionBusyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isBusy"];
    }

    public async Task SetXAxisConversionTypeAsync(ConversionType conversionType,
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdSetXAxisConversionTypeCommand(DeviceId, conversionType),
                cancellationToken);
    }

    public async Task<ConversionType> GetXAxisConversionTypeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetXAxisConversionTypeCommand(DeviceId), cancellationToken);
        return (ConversionType)result.Results["type"];
    }
}