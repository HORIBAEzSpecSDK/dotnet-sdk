using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Devices;

public sealed record MonochromatorDevice(
    int DeviceId,
    string DeviceType,
    string SerialNumber,
    WebSocketCommunicator Communicator) :
    Device(DeviceId, DeviceType, SerialNumber, Communicator)
{
    /// <summary>
    ///     Checks if the connection to the monochromator is open.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns></returns>
    public override async Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default)
    {
        var response = await Communicator.SendWithResponseAsync(new MonoIsOpenCommand(DeviceId), cancellationToken);
        return bool.Parse(response.Results["open"].ToString());
    }

    /// <summary>
    ///     Opens the connection to the Monochromator
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoOpenCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    ///     Closes the connection to the Monochromator
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoCloseCommand(DeviceId), cancellationToken);
    }

    public override async Task WaitForDeviceBusy(int waitIntervalInMs = 1000, CancellationToken cancellationToken = default)
    {
        while (await IsDeviceBusyAsync(cancellationToken))
        {
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }

    /// <summary>
    ///     Checks if the monochromator is busy.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public async Task<bool> IsDeviceBusyAsync(CancellationToken cancellationToken = default)
    {
        var response = await Communicator.SendWithResponseAsync(new MonoIsBusyCommand(DeviceId), cancellationToken);
        return bool.Parse(response.Results["busy"].ToString());
    }

    /// <summary>
    ///     Starts the monochromator initialization process called "homing".
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task HomeAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoHomeCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    ///     Returns the configuration of the monochromator.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetDeviceConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO consider creating dedicated type for the configuration and return it instead
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetConfigurationCommand(DeviceId), cancellationToken);
        return response.Results;
    }

    /// <summary>
    ///     Current wavelength of the monochromator's position in nm.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="float" />
    /// </returns>
    public async Task<float> GetCurrentWavelengthAsync(
        CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetPositionCommand(DeviceId), cancellationToken);
        return float.Parse(response.Results["wavelength"].ToString());
    }

    /// <summary>
    ///     This command sets the wavelength value of the current grating position of the monochromator.
    ///     WARNING : This could potentially un-calibrate the monochromator and report an incorrect wavelength
    ///     compared to the actual output wavelength.
    /// </summary>
    /// <param name="wavelength">Wavelength in nm</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    public async Task<MirrorPosition> GetMirrorPosition(Mirror mirror, CancellationToken cancellationToken = default)
    {
        // TODO clarify how the Mirror enum relates to the device and how is it intended to be used
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetMirrorPositionCommand(DeviceId, mirror),
                cancellationToken);
        return (MirrorPosition)int.Parse(response.Results["position"].ToString());
    }

    public async Task<float> GetSlitPositionInMMAsync(Slit slit, CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetSlitPositionInMMCommand(DeviceId, slit),
                cancellationToken);
        return float.Parse(response.Results["position"].ToString());
    }

    public Task SetSlitPositionAsync(Slit slit, float position, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveSlitMMCommand(DeviceId, slit, position), cancellationToken);
    }

    public async Task<SlitStepPosition> GetSlitStepPositionAsync(Slit slit,
        CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetSlitStepPositionCommand(DeviceId, slit),
                cancellationToken);
        return (SlitStepPosition)int.Parse(response.Results["position"].ToString());
    }

    public Task SetSlitStepPositionAsync(Slit slit, SlitStepPosition stepPosition,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoMoveSlitCommand(DeviceId, slit, stepPosition), cancellationToken);
    }

    public Task OpenShutterAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoShutterOpenCommand(DeviceId), cancellationToken);
    }

    public Task CloseShutterAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new MonoShutterCloseCommand(DeviceId), cancellationToken);
    }

    public async Task<ShutterPosition> GetShutterPositionAsync(CancellationToken cancellationToken = default)
    {
        var response =
            await Communicator.SendWithResponseAsync(new MonoGetShutterStatusCommand(DeviceId), cancellationToken);
        return (ShutterPosition)int.Parse(response.Results["position"].ToString());
    }
}