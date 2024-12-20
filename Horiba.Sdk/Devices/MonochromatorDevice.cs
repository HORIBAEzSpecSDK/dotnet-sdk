using System.Diagnostics.CodeAnalysis;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed record MonochromatorDevice(
    int DeviceId,
    string DeviceType,
    string SerialNumber,
    WebSocketCommunicator Communicator) :
    Device(DeviceId, DeviceType, SerialNumber, Communicator)
{
    /// <summary>
    /// Checks if the connection to the monochromator is open.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns></returns>
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var response = await Communicator.SendWithResponseAsync(new MonoIsOpenCommand(DeviceId), cancellationToken);
        return bool.Parse(response.Results["open"].ToString());
    }

    /// <summary>
    /// Opens the connection to the Monochromator by sending mono_open command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoOpenCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Closes the connection to the Monochromator by sending mono_close command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoCloseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Waits for the device to report not busy
    /// </summary>
    /// <param name="initialWaitInMs">Defines the time before the waiting cycle begins</param>
    /// <param name="waitIntervalInMs">Defines how long will a waiting cycle lasts</param>
    /// <param name="cancellationToken"></param>
    [SuppressMessage("ReSharper", "OptionalParameterHierarchyMismatch")]
    public override async Task WaitForDeviceNotBusy(int initialWaitInMs = 500, int waitIntervalInMs = 500,
        CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await IsDeviceBusyAsync(cancellationToken))
        {
            Log.Information("Mono: Waiting for device operation to complete");
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }
    
    /// <summary>
    /// Actively checks if the device is busy by sending mono_isBusy command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsDeviceBusyAsync(CancellationToken cancellationToken = default)
    {
        var response = await Communicator.SendWithResponseAsync(new MonoIsBusyCommand(DeviceId), cancellationToken);
        return bool.Parse(response.Results["busy"].ToString());
    }

    /// <summary>
    /// Starts the monochromator initialization process called "homing" by sending mono_init command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task HomeAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoInitCommand(DeviceId), cancellationToken);
    }
    
    /// <summary>
    /// Retrieves the configuration of the monochromator by sending the mono_getConfig command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetDeviceConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetConfigurationCommand(DeviceId), cancellationToken);
        return response.Results;
    }
    
    /// <summary>
    /// Retrieves the current wavelength of the monochromator by sending the mono_getPosition command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<float> GetCurrentWavelengthAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetPositionCommand(DeviceId), cancellationToken);
        return float.Parse(response.Results["wavelength"].ToString());
    }

    /// <summary>
    ///     This command sets the wavelength value of the current grating position of the monochromator
    ///     by sending the mono_setPosition command.
    /// 
    ///     WARNING : This could potentially un-calibrate the monochromator and report an incorrect wavelength
    ///     compared to the actual output wavelength.
    /// </summary>
    /// <param name="wavelength">Wavelength in nm</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task CalibrateWavelengthAsync(float wavelength, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoSetPositionCommand(DeviceId, wavelength), cancellationToken);
    }

    /// <summary>
    ///     Orders the monochromator to move to the requested wavelength.
    /// </summary>
    /// <param name="wavelength">wavelength in nm</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task MoveToWavelengthAsync(float wavelength, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveToPositionCommand(DeviceId, wavelength), cancellationToken);
    }

    /// <summary>
    ///     Current grating of the turret
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="Grating" />
    /// </returns>
    public async Task<Grating> GetTurretGratingAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetGratingPositionCommand(DeviceId), cancellationToken);
        return (Grating)int.Parse(response.Results["position"].ToString());
    }

    /// <summary>
    ///     Select current turret grating
    /// </summary>
    /// <param name="grating">The <see cref="Grating" /> to be set.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetTurretGratingAsync(Grating grating, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveGratingCommand(DeviceId, grating), cancellationToken);
    }

    /// <summary>
    ///     Current position of the filter wheel.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="FilterWheelPosition" />
    /// </returns>
    public async Task<FilterWheelPosition> GetFilterWheelPositionAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetFilterWheelPositionCommand(DeviceId),
                cancellationToken);
        return (FilterWheelPosition)int.Parse(response.Results["position"].ToString());
    }

    /// <summary>
    ///     Sets the current position of the filter wheel.
    /// </summary>
    /// <param name="filterWheelPosition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetFilterWheelPositionAsync(FilterWheelPosition filterWheelPosition,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveFilterWheelCommand(DeviceId, filterWheelPosition), cancellationToken);
    }

    /// <summary>
    /// Retrieves the current mirror position by sending the mono_getMirrorPosition command
    /// </summary>
    /// <param name="mirror"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MirrorPosition> GetMirrorPosition(Mirror mirror, CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetMirrorPositionCommand(DeviceId, mirror),
                cancellationToken);
        return (MirrorPosition)int.Parse(response.Results["position"].ToString());
    }

    /// <summary>
    /// Sets the mirror position by sending the mono_moveMirror command
    /// </summary>
    /// <param name="mirror"></param>
    /// <param name="position"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetMirrorPositionAsync(Mirror mirror, MirrorPosition position,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveMirrorCommand(DeviceId, mirror, position), cancellationToken);
    }

    /// <summary>
    /// Retrieves the slit position in mm by sending the mono_getSlitPositionInMM command
    /// </summary>
    /// <param name="slit"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public async Task<float> GetSlitPositionInMMAsync(Slit slit, CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetSlitPositionInMMCommand(DeviceId, slit),
                cancellationToken);
        return float.Parse(response.Results["position"].ToString());
    }

    /// <summary>
    /// Sets the slit position in mm by sending the mono_moveSlitMM command
    /// </summary>
    /// <param name="slit"></param>
    /// <param name="position"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetSlitPositionAsync(Slit slit, float position, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveSlitMMCommand(DeviceId, slit, position), cancellationToken);
    }

   
    /// <summary>
    /// Opens the shutter by sending the mono_shutterOpen command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task OpenShutterAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoShutterOpenCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Closes the shutter by sending the mono_shutterClose command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task CloseShutterAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoShutterCloseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Retrieves the shutter position by sending the mono_getShutterStatus command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public async Task<ShutterPosition> GetShutterPositionAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetShutterStatusCommand(DeviceId), cancellationToken);
        return (ShutterPosition)int.Parse(response.Results["shutter 1"].ToString());
    }

    /// <summary>
    /// Sets the active shutter by sending the mono_shutterSelect command
    /// </summary>
    /// <param name="shutter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SelectShutterAsync(Shutter shutter, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoShutterSelectCommand(DeviceId, shutter), cancellationToken);
    }

    public async Task<bool> GetIsInitializedAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetIsInitializedCommand(DeviceId), cancellationToken);
        return bool.Parse(response.Results["initialized"].ToString());
    }
}