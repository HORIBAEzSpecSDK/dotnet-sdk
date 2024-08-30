using System.Diagnostics.CodeAnalysis;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed record ChargedCoupledDevice(
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
            await Communicator.SendWithResponseAsync(new CcdIsConnectionOpenedCommand(DeviceId), cancellationToken);

        if (result.Results.TryGetValue("open", out var bR))
        {
            return bool.Parse(bR.ToString());
        }

        return false;
    }

    /// <summary>
    /// Establishes a connection to the ICL by sending the ccd_open command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdOpenCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Terminates the connection to the ICL by sending the ccd_close command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task CloseConnectionAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdCloseCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Starts a polling cycle to check if the device is busy. The cycle will last until the device reports
    /// it is not busy anymore. The polling cycle will start after the initial wait time. Every polling cycle
    /// activly sends the ccd_getAcquisitionBusy command
    /// </summary>
    /// <param name="initialWaitInMs">Defines the time before the polling cycle begins</param>
    /// <param name="waitIntervalInMs">Defines how long will a polling cycle is</param>
    /// <param name="cancellationToken"></param>
    [SuppressMessage("ReSharper", "OptionalParameterHierarchyMismatch")]
    public override async Task WaitForDeviceNotBusy(int initialWaitInMs = 250, int waitIntervalInMs = 250,
        CancellationToken cancellationToken = default)
    {
        Task.Delay(initialWaitInMs, cancellationToken).Wait(cancellationToken);
        while (await GetAcquisitionBusyAsync(cancellationToken))
        {
            Log.Information("CCD: Waiting for device operation to complete");
            Task.Delay(waitIntervalInMs, cancellationToken).Wait(cancellationToken);
        }
    }

    /// <summary>
    /// Starts a polling cycle to check if the device is busy. The cycle will last until the device reports
    /// it is not busy anymore. The polling cycle will start after the initial wait time. Every polling cycle
    /// activly sends the ccd_getAcquisitionBusy command
    /// </summary>
    /// <param name="initialWaitInMs">Defines the time before the polling cycle begins</param>
    /// <param name="waitIntervalInMs">Defines how long will a polling cycle is</param>
    /// <param name="cancellationToken"></param>
    [SuppressMessage("ReSharper", "OptionalParameterHierarchyMismatch")]
    public Task WaitForDeviceNotBusy(TimeSpan? initialWait = null, TimeSpan? waitInterval = null,
        CancellationToken cancellationToken = default)
    {
        var init = initialWait ?? TimeSpan.FromMilliseconds(250);
        var wait = initialWait ?? TimeSpan.FromMilliseconds(250);
        return WaitForDeviceNotBusy(init.Milliseconds, wait.Milliseconds, cancellationToken);
    }

    /// <summary>
    /// Retrieves the temperature of the CCD chip by sending the ccd_getChipTemperature command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Temperature of the chip in degreese Celsius</returns>
    public async Task<double> GetChipTemperatureAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTemperatureCommand(DeviceId), cancellationToken);
        return double.Parse(result.Results["temperature"].ToString());
    }

    /// <summary>
    /// Retrieves the size of the CCD chip by sending the ccd_getChipSize command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A tuple containing two items representing the size of the chip in number of pixels</returns>
    public async Task<(int Width, int Height)> GetChipSizeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetChipSizeCommand(DeviceId), cancellationToken);
        return (int.Parse(result.Results["x"].ToString()), int.Parse(result.Results["y"].ToString()));
    }

    /// <summary>
    /// Retrieves the current speed of the CCD by sending the ccd_getSpeed command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="Speed"/></returns>
    public async Task<Speed> GetSpeedAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetSpeedCommand(DeviceId), cancellationToken);
        return GetDeviceSpecificSpeed(int.Parse(result.Results["token"].ToString()));
    }

    /// <summary>
    /// Sets the speed of the CCD by sending the ccd_setSpeed command
    /// </summary>
    /// <param name="speed">The <see cref="Speed"/> to be set</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetSpeedAsync(Speed speed, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetSpeedCommand(DeviceId, speed), cancellationToken);
    }

    /// <summary>
    /// Retrieves the exposure time of the CCD by sending the ccd_getExposureTime command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The exposure time im milliseconds</returns>
    public async Task<int> GetExposureTimeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetExposureTimeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["time"].ToString());
    }

    /// <summary>
    /// Sets the exposure time of the CCD (expressed in Timer Resolution units) by sending the ccd_setExposureTime command.
    /// NOTE:
    /// To check the current TimerResolution value see <see cref="GetTimerResolutionAsync"/>. Alternatively the Timer Resolution value can be set using <see cref="SetTimerResolutionAsync"/>
    /// 
    /// Examples:
    /// If Exposure Time is set to 50, and the Timer Resolution value is 1000, the CCD exposure time (integration time) = 50 milliseconds.
    /// If Exposure Time is set to 50, and the Timer Resolution value is 1, the CCD exposure time (integration time) = 50 microseconds.
    /// </summary>
    /// <param name="exposureTimeInMs">Target time in milliseconds</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetExposureTimeAsync(int exposureTimeInMs, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetExposureTimeCommand(DeviceId, exposureTimeInMs),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the acquisition ready status of the CCD by sending the ccd_getAcquisitionReady command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> GetAcquisitionReadyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionReadyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["ready"];
    }

    /// <summary>
    /// Sets the acquisition start of the CCD by sending the ccd_setAcquisitionStart command
    /// </summary>
    /// <param name="isShutterOpened">Controls the state of the shutter of the device</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetAcquisitionStartAsync(bool isShutterOpened, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetAcquisitionStartCommand(DeviceId, isShutterOpened),
            cancellationToken);
    }

    /// <summary>
    /// Sets the region of interest of the CCD by sending the ccd_setRegionOfInterest command
    /// </summary>
    /// <param name="regionOfInterest">The target region</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetRegionOfInterestAsync(RegionOfInterest regionOfInterest, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetRegionOfInterestCommand(DeviceId, regionOfInterest),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the acquisition data of the CCD by sending the ccd_getAcquisitionData command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dictionary containing the raw data of the device</returns>
    public async Task<Dictionary<string, object>> GetAcquisitionDataAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionDataCommand(DeviceId), cancellationToken);
        return result.Results;
    }

    /// <summary>
    /// Retrieves the acquisition busy status of the CCD by sending the ccd_getAcquisitionBusy command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> GetAcquisitionBusyAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionBusyCommand(DeviceId), cancellationToken);
        return (bool)result.Results["isBusy"];
    }

    /// <summary>
    /// Sets the X axis conversion type of the CCD by sending the ccd_setXAxisConversionType command
    /// </summary>
    /// <param name="conversionType">The target conversion</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetXAxisConversionTypeAsync(ConversionType conversionType, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetXAxisConversionTypeCommand(DeviceId, conversionType),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the X axis conversion type of the CCD by sending the ccd_getXAxisConversionType command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ConversionType> GetXAxisConversionTypeAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetXAxisConversionTypeCommand(DeviceId), cancellationToken);
        return (ConversionType)int.Parse(result.Results["type"].ToString());
    }

    /// <summary>
    /// Restarts the device by sending the ccd_restart command
    /// </summary>
    /// <param name="cancellationToken"></param>
    public Task RestartDeviceAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdRestartCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Retrieves the device configuration by sending the ccd_getConfig command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Dictionary of string-object pairs representing the configuration of the device</returns>
    public async Task<Dictionary<string, object>> GetDeviceConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetConfigCommand(DeviceId), cancellationToken);
        // TODO this is really breakable, find a better way to implement it
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(result.Results.First().Value.ToString());
    }

    /// <summary>
    /// Retrieves the gain of the CCD by sending the ccd_getGain command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Gain> GetGainAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetGainCommand(DeviceId), cancellationToken);
        return GetDeviceSpecificGain(int.Parse(result.Results["token"].ToString()));
    }

    /// <summary>
    /// Sets the gain of the CCD by sending the ccd_setGain command
    /// </summary>
    /// <param name="gain"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetGainAsync(Gain gain, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetGainCommand(DeviceId, gain), cancellationToken);
    }

    /// <summary>
    /// Retrieves the fit parameters of the CCD by sending the ccd_getFitParameters command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetFitParametersAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetFitParametersCommand(DeviceId), cancellationToken);
        return result.Results["params"].ToString();
    }

    /// <summary>
    /// Sets the fit parameters of the CCD by sending the ccd_setFitParameters command
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetFitParametersAsync(string parameters, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetFitParametersCommand(DeviceId, parameters), cancellationToken);
    }

    /// <summary>
    /// Retrieves the timer resolution of the CCD by sending the ccd_getTimerResolution command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The Timer Resolution in microseconds [uS]</returns>
    public async Task<int> GetTimerResolutionAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetTimerResolutionCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["resolutionToken"].ToString());
    }

    /// <summary>
    /// Sets the timer resolution of the CCD by sending the ccd_setTimerResolution command
    /// 
    /// Note: The timer resolution value of 1 microsecond is not supported by every CCD.
    /// </summary>
    /// <param name="resolution"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetTimerResolutionAsync(TimerResolution resolution, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetTimerResolutionCommand(DeviceId, resolution), cancellationToken);
    }

    /// <summary>
    /// Sets the acquisition format of the CCD by sending the ccd_setAcquisitionFormat command
    /// </summary>
    /// <param name="format"></param>
    /// <param name="numberOfRois"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetAcquisitionFormatAsync(AcquisitionFormat format, int numberOfRois,
        CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetAcquisitionFormatCommand(DeviceId, format, numberOfRois),
            cancellationToken);
    }

    /// <summary>
    /// Sets the acquisition count of the CCD by sending the ccd_setAcquisitionCount command
    /// </summary>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetAcquisitionCountAsync(int count, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetAcquisitionCountCommand(DeviceId, count), cancellationToken);
    }

    /// <summary>
    /// Retrieves the acquisition count of the CCD by sending the ccd_getAcquisitionCount command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> GetAcquisitionCountAsync(CancellationToken cancellationToken = default)
    {
        var result =
            await Communicator.SendWithResponseAsync(new CcdGetAcquisitionCountCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["count"].ToString());
    }

    /// <summary>
    /// Sets the clean count of the CCD by sending the ccd_setCleanCount command
    /// </summary>
    /// <param name="count"></param>
    /// <param name="mode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetCleanCountAsync(int count, CleanCountMode mode, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetCleanCountCommand(DeviceId, count, mode), cancellationToken);
    }
    
    /// <summary>
    /// Sets the acquisition abort of the CCD by sending the ccd_setAcquisitionAbort command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Task representing the communication between SDK and ICL</returns>
    public Task SetAcquisitionAbortAsync(CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetAcquisitionAbortCommand(DeviceId), cancellationToken);
    }

    /// <summary>
    /// Retrieves the clean count of the CCD by sending the ccd_getCleanCount command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> GetCleanCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetCleanCountCommand(DeviceId), cancellationToken);
        return $"count: {result.Results["count"]} mode: {result.Results["mode"]}";
    }

    /// <summary>
    /// Retrieves the data size of the CCD by sending the ccd_getDataSize command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> GetDataSizeAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetDataSizeCommand(DeviceId), cancellationToken);
        return int.Parse(result.Results["size"].ToString());
    }
    
    /// <summary>
    /// Retrieves the trigger in of the CCD by sending the ccd_getTriggerIn command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Trigger> GetTriggerInAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetTriggerInCommand(DeviceId), cancellationToken);
        return result.Results.ToTrigger();
    }
    
    /// <summary>
    /// Sets the trigger in of the CCD by sending the ccd_setTriggerIn command
    /// </summary>
    /// <param name="trigger">Description of the trigger to set</param>
    /// <param name="isEnabled">State of the trigger</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetTriggerInAsync(Trigger trigger, bool isEnabled, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetTriggerInCommand(DeviceId, trigger, isEnabled), cancellationToken);
    }
    
    /// <summary>
    /// Retrieves the signal in of the CCD by sending the ccd_getSignalIn command
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Signal> GetSignalOutAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new CcdGetSignalOutCommand(DeviceId), cancellationToken);
        return result.Results.ToSignal();
    }
    
    /// <summary>
    /// Sets the signal in of the CCD by sending the ccd_setSignalIn command
    /// </summary>
    /// <param name="signal">Description of the signal to set</param>
    /// <param name="isEnabled">State of the signal</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetSignalOutAsync(Signal signal, bool isEnabled, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetSignalOutCommand(DeviceId, signal, isEnabled), cancellationToken);
    }

    /// <summary>
    /// Sets the center wavelength value to be used in the grating equation.
    /// </summary>
    /// <param name="wavelength">Center wavelength</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SetCenterWavelengthAsync(float wavelength, CancellationToken cancellationToken = default)
    {
        return Communicator.SendAsync(new CcdSetCenterWavelengthCommand(DeviceId, wavelength), cancellationToken);
    }

/// <summary>
/// Finds the center wavelength positions based on the input range and pixel overlap.
/// The following commands are prerequisites and should be called prior to using this command:
/// <see cref="SetXAxisConversionTypeAsync"/>, <see cref="SetAcquisitionFormatAsync"/>, and <see cref="SetRegionOfInterestAsync"/>
/// </summary>
/// <param name="monoIndex">Used to identify which mono to target for the current grating density</param>
/// <param name="startWavelength">Start wavelength</param>
/// <param name="endWavelength">End wavelength</param>
/// <param name="overlap">Number of overlapping pixels</param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
    public Task<Response> CalculateRangePositionsAsync(int monoIndex, float startWavelength, float endWavelength, float overlap, CancellationToken cancellationToken = default)
    {
        return Communicator.SendWithResponseAsync(
            new CcdCalculateRangeModePositionsCommand(DeviceId, monoIndex, startWavelength, endWavelength, overlap),
            cancellationToken);
    }
}