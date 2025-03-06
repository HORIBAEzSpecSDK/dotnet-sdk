using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed record SpectrAcqDevice(
    int DeviceId,
    string DeviceType,
    string SerialNumber,
    WebSocketCommunicator Communicator) :
    Device(DeviceId, DeviceType, SerialNumber, Communicator)
{
    /// <summary>
    /// Actively checks if the connection to the ICL is opened by sending a ccd_isOpen command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsOpenCommand(DeviceId), cancellationToken);

        if (result.Results.TryGetValue("open", out var bR))
        {
            return bool.Parse(bR.ToString());
        }

        return false;
    }

    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqOpenCommand(DeviceId), cancellationToken);
    }

    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqCloseCommand(DeviceId), cancellationToken);
    }

    public async Task<bool> GetAcquisitionBusyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsBusyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isBusy"];
    }

    public override async Task WaitForDeviceNotBusy(int initialWaitInMs = 250, int waitIntervalInMs = 250,
        CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await GetAcquisitionBusyAsync(cancellationToken))
        {
            Log.Information("Saq: Waiting for device operation to complete");
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }

    public Task WaitForDeviceNotBusy(TimeSpan? initialWait = null, TimeSpan? waitInterval = null,
        CancellationToken cancellationToken = default)
    {
        var init = initialWait ?? TimeSpan.FromMilliseconds(250);
        var wait = initialWait ?? TimeSpan.FromMilliseconds(250);
        return WaitForDeviceNotBusy(init.Milliseconds, wait.Milliseconds, cancellationToken);
    }

    public async Task<string> GetFirmwareVersionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetFirmwareVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["firmwareVersion"];
    }

    public async Task<string> GetFpgaVersionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetFpgaVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["FpgaVersion"];
    }

    public async Task<string> GetBordRevisionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetBoardRevisionCommand(DeviceId), cancellationToken);
        return (string)result.Results["boardRevision"];
    }

    public async Task<string> GetSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetSerialNumberCommand(DeviceId), cancellationToken);
        return (string)result.Results["serialNumber"];
    }

    public Task SetIntegrationTimeAsync(int integtrationTimeInSec, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetIntegrationTimeCommand(DeviceId, integtrationTimeInSec),
            cancellationToken);
    }

    public async Task<int> GetIntegrationTimeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetIntegrationTimeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["integrationTime"].ToString());
    }

    public Task SetHvBiasVoltageAsync(int biasVoltage, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetHvBiasVoltageCommand(DeviceId, biasVoltage), cancellationToken);
    }

    public async Task<int> GetHvBiasVoltageAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetHvBiasVoltageCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["biasVoltage"].ToString());
    }

    public async Task<int> GetMaxHvVoltageAllowedAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetMaxHvVoltageAllowedCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["biasVoltage"].ToString()) ;
    }

    public Task DefineAcqSetAsync(int scanCount, int timeStep, int integrationTime, int externalParam,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(
            new SaqDefineAcqSetCommand(DeviceId, scanCount, timeStep, integrationTime, externalParam),
            cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetAcqSetAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetAcqSetCommand(DeviceId), cancellationToken);
        return result.Results;
    }

    public Task StartAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqStartCommand(DeviceId), cancellationToken);
    }

    public Task StopAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqStopCommand(DeviceId), cancellationToken);
    }

    public Task PauseAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqPauseCommand(DeviceId), cancellationToken);
    }

    public Task ContinueAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqContinueCommand(DeviceId), cancellationToken);
    }

    public async Task<bool> GetIsDataAvailableAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsDataAvailableCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isDataAvailable"];
    }

    public async Task<List<Dictionary<string, object>>> GetAvailableDataAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsDataAvailableCommand(DeviceId), cancellationToken);
        return (List<Dictionary<string, object>>)result.Results["data"];
    }

    public Task ForceTriggerAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqForceTriggerCommand(DeviceId), cancellationToken);
    }

    public Task SetInTriggerModeAsync(InTriggerMode inTriggerMode, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetInTriggerModeCommand(DeviceId, inTriggerMode), cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetTriggerModeAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetTriggerModeCommand(DeviceId), cancellationToken);
        return (Dictionary<string, object>)result.Results["results"];
    }

    public async Task<string> GetLastErrorAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetLastErrorCommand(DeviceId), cancellationToken);
        return (string)result.Results["error"];
    }

    public async Task<string> GetErrorLogAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetErrorLogCommand(DeviceId), cancellationToken);
        return (string)result.Results["errors"];
    }
    
    public Task ClearErrorLogAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqClearErrorLogCommand(DeviceId), cancellationToken);
    }
}