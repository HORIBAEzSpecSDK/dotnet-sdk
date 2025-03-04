using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
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
    
    public override async Task WaitForDeviceNotBusy(int initialWaitInMs, int waitIntervalInMs,
        CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await GetAcquisitionBusyAsync(cancellationToken))
        {
            Log.Information("CCD: Waiting for device operation to complete");
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
        var result = await Communicator.SendWithResponseAsync(new SaqGetFirmwareVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["firmwareVersion"];
    }
    
    public async Task<string> GetFpgaVersionAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetFpgaVersionCommand(DeviceId), cancellationToken);
        return (string)result.Results["FpgaVersion"];
    }
    
    public async Task<char> GetBordRevisionAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetBoardRevisionCommand(DeviceId), cancellationToken);
        return (char)result.Results["boardRevision"];
    }
        
    public async Task<string> GetSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetSerialNumberCommand(DeviceId), cancellationToken);
        return (string)result.Results["serialNumber"];
    }
    
    public Task SetIntegrationTimeAsync(int integtrationTimeInSec, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetIntegrationTimeCommand(DeviceId, integtrationTimeInSec), cancellationToken);
    }
    
    public async Task<int> GetIntegrationTimeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetIntegrationTimeCommand(DeviceId), cancellationToken);
        return (int)result.Results["integrationTime"];
    }
    public Task SetHvBiasVoltageAsync(int biasVoltage, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqSetHvBiasVoltageCommand(DeviceId, biasVoltage), cancellationToken);
    }
    public async Task<int> GetHvBiasVoltageAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetHvBiasVoltageCommand(DeviceId), cancellationToken);
        return (int)result.Results["biasVoltage"];
    }
    
    public async Task<int> GetMaxHvVoltageAllowedAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new SaqGetMaxHvVoltageAllowedCommand(DeviceId), cancellationToken);
        return (int)result.Results["biasVoltage"];
    }
    
    public Task DefineAcqSetAsync(int scanCount, int timeStep, int integrationTime, int externalParam , CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new SaqDefineAcqSetCommand(DeviceId, scanCount, timeStep, integrationTime, externalParam), cancellationToken);
    }
}