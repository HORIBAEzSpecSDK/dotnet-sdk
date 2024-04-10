using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed record ChargedCoupledDevice(
    int DeviceId,
    string DeviceType,
    string SerialNumber,
    WebSocketCommunicator Communicator) :
    Device(DeviceId, DeviceType, SerialNumber, Communicator)
{
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdIsConnectionOpenedCommand(DeviceId), cancellationToken);

        if (result.Results.TryGetValue("open", out var bR)) return bool.Parse(bR.ToString());

        return false;
    }

    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendWithResponseAsync(new CcdOpenCommand(DeviceId), cancellationToken);
    }

    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdCloseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Waits for the device to complete an acquisition
    /// </summary>
    /// <param name="waitIntervalInMs">Defines how long will a waiting cycle lasts</param>
    /// <param name="initialWaitInMs">Defines the time before the waiting cycle begins</param>
    /// <param name="cancellationToken"></param>
    public override async Task WaitForDeviceNotBusy(int waitIntervalInMs = 500, int initialWaitInMs = 500, CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await GetAcquisitionBusyAsync(cancellationToken))
        {
            Log.Information("CCD: Waiting for device operation to complete");
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }

    public async Task<double> GetChipTemperatureAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTemperatureCommand(DeviceId), cancellationToken);
        return double.Parse(result.Results["temperature"].ToString());
    }

    public async Task<(int Width, int Height)> GetChipSizeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetChipSizeCommand(DeviceId), cancellationToken);
        return (int.Parse(result.Results["x"].ToString()), int.Parse(result.Results["y"].ToString()));
    }

    public async Task<Speed> GetSpeedAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetSpeedCommand(DeviceId), cancellationToken);
        return (Speed)int.Parse(result.Results["token"].ToString());
    }

    public async Task<int> GetExposureTimeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetExposureTimeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["time"].ToString());
    }

    public async Task SetExposureTimeAsync(int exposureTimeInMs, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetExposureTimeCommand(DeviceId, exposureTimeInMs),
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
        await Communicator.SendAsync(new CcdSetAcquisitionStartCommand(DeviceId, isShutterOpened),
            cancellationToken);
    }

    public async Task SetRegionOfInterestAsync(RegionOfInterest regionOfInterest,
        CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetRegionOfInterestCommand(DeviceId, regionOfInterest),
            cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetAcquisitionDataAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionDataCommand(DeviceId), cancellationToken);
        return result.Results;
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
        await Communicator.SendAsync(new CcdSetXAxisConversionTypeCommand(DeviceId, conversionType),
            cancellationToken);
    }

    public async Task<ConversionType> GetXAxisConversionTypeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetXAxisConversionTypeCommand(DeviceId), cancellationToken);
        return (ConversionType)int.Parse(result.Results["type"].ToString());
    }

    public async Task RestartDeviceAsync(CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdRestartCommand(DeviceId), cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetDeviceConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetConfigCommand(DeviceId), cancellationToken);
        return result.Results;
    }

    public async Task<int> GetNumberOfAveragesAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetNumberOfAveragesCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["count"].ToString());
    }

    public async Task SetNumberOfAveragesAsync(int numberOfAverages, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetNumberOfAveragesCommand(DeviceId, numberOfAverages), cancellationToken);
    }

    public async Task<Gain> GetGainAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetGainCommand(DeviceId), cancellationToken);
        return (Gain)int.Parse(result.Results["token"].ToString());
    }

    public async Task SetGainAsync(Gain gain, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetGainCommand(DeviceId, gain), cancellationToken);
    }

    public async Task<string> GetFitParametersAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetFitParametersCommand(DeviceId), cancellationToken);
        return result.Results["params"].ToString();
    }

    public async Task SetFitParametersAsync(string parameters, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetFitParametersCommand(DeviceId, parameters), cancellationToken);
    }

    public async Task<int> GetTimerResolutionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTimerResolutionCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["resolution"].ToString());
    }

    public async Task SetTimerResolutionAsync(int resolution, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetTimerResolutionCommand(DeviceId, resolution), cancellationToken);
    }

    public async Task SetAcquisitionFormatAsync(AcquisitionFormat format, int numberOfRois,
        CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetAcquisitionFormatCommand(DeviceId, format, numberOfRois),
            cancellationToken);
    }

    public async Task SetAcquisitionCountAsync(int count, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetAcquisitionCountCommand(DeviceId, count), cancellationToken);
    }

    public async Task<int> GetAcquisitionCountAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTimerResolutionCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["resolution"].ToString());
    }

    public async Task SetCleanCountAsync(int count, CleanCountMode mode, CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetCleanCountCommand(DeviceId, count, mode), cancellationToken);
    }

    public async Task SetAcquisitionAbortAsync(CancellationToken cancellationToken = default)
    {
        await Communicator.SendAsync(new CcdSetAcquisitionAbortCommand(DeviceId), cancellationToken);
    }

    public async Task<string> GetCleanCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetCleanCountCommand(DeviceId), cancellationToken);
        return $"count: {result.Results["count"]} mode: {result.Results["mode"]}";
    }

    public async Task<int> GetDataSizeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetDataSizeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["size"].ToString());
    }
}