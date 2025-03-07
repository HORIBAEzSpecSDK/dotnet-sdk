using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Data;
using Horiba.Sdk.Enums;
using Newtonsoft.Json;
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
    /// Actively checks if the connection to the ICL is opened by sending a saq3_isOpen command
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

    /// <summary>
    /// Establishes a connection to the ICL by sending the saq3_open command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqOpenCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Terminates the connection to the ICL by sending the saq3_close command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqCloseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Retrieves the acquisition busy status of the SAQ by sending the saq3_isBusy command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> GetIsBusyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsBusyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isBusy"];
    }

    /// <summary>
    /// Starts a polling cycle to check if the device is busy. The cycle will last until the device reports
    /// it is not busy anymore. The polling cycle will start after the initial wait time. Every polling cycle
    /// actively sends the saq3_isBusy command
    /// </summary>
    /// <param name="initialWaitInMs">Defines the time before the polling cycle begins</param>
    /// <param name="waitIntervalInMs">Defines how long will a polling cycle is</param>
    /// <param name="cancellationToken"></param>
    public override async Task WaitForDeviceNotBusy(int initialWaitInMs = 250, int waitIntervalInMs = 250,
        CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await GetIsBusyAsync(cancellationToken))
        {
            Log.Information("Saq: Waiting for device operation to complete");
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }

    /// <summary>
    /// Starts a polling cycle to check if the device is busy. The cycle will last until the device reports
    /// it is not busy anymore. The polling cycle will start after the initial wait time. Every polling cycle
    /// actively sends the saq3_isBusy command
    /// </summary>
    /// <param name="initialWait"></param>
    /// <param name="waitInterval"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task WaitForDeviceNotBusy(TimeSpan? initialWait = null, TimeSpan? waitInterval = null,
        CancellationToken cancellationToken = default)
    {
        var init = initialWait ?? TimeSpan.FromMilliseconds(250);
        var wait = initialWait ?? TimeSpan.FromMilliseconds(250);
        return WaitForDeviceNotBusy(init.Milliseconds, wait.Milliseconds, cancellationToken);
    }

    /// <summary>
    /// Retrieves the firmware version of the SAQ by sending the saq3_getFirmwareVersion command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetFirmwareVersionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetFirmwareVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["firmwareVersion"];
    }

    /// <summary>
    /// Retrieves the FPGA version of the SAQ by sending the saq3_getFPGAVersion command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetFpgaVersionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetFpgaVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["FpgaVersion"];
    }

    /// <summary>
    /// Retrieves the board revision of the SAQ by sending the saq3_getBoardRevision command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetBordRevisionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetBoardRevisionCommand(DeviceId), cancellationToken);
        return (string)result.Results["boardRevision"];
    }

    /// <summary>
    /// Retrieves the serial number of the SAQ by sending the saq3_getSerialNumber command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetSerialNumberCommand(DeviceId), cancellationToken);
        return (string)result.Results["serialNumber"];
    }

    /// <summary>
    /// Sets the integration time of the SAQ by sending the saq3_setIntegrationTime command 
    /// </summary>
    /// <param name="integtrationTimeInSec"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetIntegrationTimeAsync(int integtrationTimeInSec, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetIntegrationTimeCommand(DeviceId, integtrationTimeInSec),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the integration time of the SAQ by sending the saq3_getIntegrationTime command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> GetIntegrationTimeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetIntegrationTimeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["integrationTime"].ToString());
    }

    /// <summary>
    /// Sets the bias voltage of the SAQ by sending the saq3_setHVBiasVoltage command 
    /// </summary>
    /// <param name="biasVoltage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetHvBiasVoltageAsync(int biasVoltage, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetHvBiasVoltageCommand(DeviceId, biasVoltage), cancellationToken);
    }

    /// <summary>
    /// Retrieves the bias voltage of the SAQ by sending the saq3_getHVBiasVoltage command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> GetHvBiasVoltageAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetHvBiasVoltageCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["biasVoltage"].ToString());
    }

    /// <summary>
    /// Retrieves the maximum allowed HV voltage of the SAQ by sending the saq3_getMaxHVVoltageAllowed command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> GetMaxHvVoltageAllowedAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetMaxHvVoltageAllowedCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["biasVoltage"].ToString());
    }

    /// <summary>
    /// Sets the acquisition set of the SAQ by sending the saq3_setAcqSet command 
    /// </summary>
    /// <param name="scanCount"></param>
    /// <param name="timeStep"></param>
    /// <param name="integrationTime"></param>
    /// <param name="externalParam"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetAcqSetAsync(int scanCount, int timeStep, int integrationTime, int externalParam,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(
            new SaqSetAcqSetCommand(DeviceId, scanCount, timeStep, integrationTime, externalParam),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the defined acquisition set of the SAQ by sending the saq3_getAcqSet command 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetAcqSetAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetAcqSetCommand(DeviceId), cancellationToken);
        return result.Results;
    }

    /// <summary>
    /// Starts the data acquisition of the SAQ by sending the saq3_acqStart command
    /// </summary>
    /// <param name="scanStartMode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAcquisitionAsync(ScanStartMode scanStartMode, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqStartCommand(DeviceId, scanStartMode), cancellationToken);
    }

    /// <summary>
    /// Stops the data acquisition of the SAQ by sending the saq3_acqStop command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqStopCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Pauses the data acquisition of the SAQ by sending the saq3_acqPause command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task PauseAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqPauseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Continues the paused data acquisition of the SAQ by sending the saq3_acqContinue command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ContinueAcquisitionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqAcqContinueCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Checks if there is any acquisition data of the SAQ available to retrieve by sending the saq3_isDataAvailable command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> GetIsDataAvailableAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqIsDataAvailableCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isDataAvailable"];
    }

    /// <summary>
    /// Retrieves available acquisition data of the SAQ by sending the saq3_getAvailableData command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SaqData> GetAvailableDataAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetAvailableDataCommand(DeviceId), cancellationToken);
        string jsonData = JsonConvert.SerializeObject(result.Results, Formatting.None);
        SaqData data = JsonConvert.DeserializeObject<SaqData>(jsonData);
        return data;
    }

    /// <summary>
    /// Forces the trigger of the SAQ by sending the saq3_forceTrigger command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ForceTriggerAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqForceTriggerCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Sets the input trigger mode of the SAQ by sending the saq3_setInTriggerMode command
    /// </summary>
    /// <param name="inTriggerMode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetInTriggerModeAsync(InTriggerMode inTriggerMode, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetInTriggerModeCommand(DeviceId, inTriggerMode), cancellationToken);
    }

    /// <summary>
    /// Retrieves the different trigger modes that are set of the SAQ by sending the saq3_getTriggerMode command.
    /// The following modes are returned: "scanStartMode", "inputTriggerMode", "outputTriggerMode", "ccdMode"
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetInTriggerModeAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetInTriggerModeCommand(DeviceId), cancellationToken);
        return (Dictionary<string, object>)result.Results["results"];
    }

    /// <summary>
    /// Retrieves the last error of the SAQ by sending the saq3_getLastError command.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetLastErrorAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetLastErrorCommand(DeviceId), cancellationToken);
        return (string)result.Results["error"];
    }

    /// <summary>
    /// Retrieves the error log of the SAQ by sending the saq3_getErrorLog command.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<ErrorEntry>> GetErrorLogAsync(
        CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new SaqGetErrorLogCommand(DeviceId), cancellationToken);
        return (List<ErrorEntry>)result.Results["errors"];
    }

    /// <summary>
    /// Cleans the error log of the SAQ by sending the saq3_clearErrorLog command.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ClearErrorLogAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqClearErrorLogCommand(DeviceId), cancellationToken);
    }
}