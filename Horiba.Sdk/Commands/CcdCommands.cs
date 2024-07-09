using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Commands;

internal record CcdOpenCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_open", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdCloseCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_close", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdIsConnectionOpenedCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_isOpen", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetTemperatureCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getChipTemperature", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetChipSizeCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getChipSize", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetSpeedCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getSpeed", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetSpeedCommand(int DeviceId, Speed Speed) : ChargedCoupleDeviceCommand("ccd_setSpeed",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "token", (int)Speed }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Speed Speed { get; } = Speed;
}

internal record CcdGetExposureTimeCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getExposureTime", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetExposureTimeCommand(int DeviceId, int ExposureTimeInMs) : ChargedCoupleDeviceCommand(
    "ccd_setExposureTime",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "time", ExposureTimeInMs }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int ExposureTimeInMs { get; } = ExposureTimeInMs;
}

internal record CcdGetAcquisitionReadyCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_getAcquisitionReady", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetAcquisitionStartCommand(int DeviceId, bool IsShutterOpened) : ChargedCoupleDeviceCommand(
    "ccd_setAcquisitionStart",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "openShutter", IsShutterOpened }
    })
{
    public int DeviceId { get; } = DeviceId;
    public bool IsShutterOpened { get; } = IsShutterOpened;
}

internal record CcdSetRegionOfInterestCommand(int DeviceId, RegionOfInterest Region)
    : ChargedCoupleDeviceCommand("ccd_setRoi", Region.ToDeviceParameters(DeviceId))
{
    public int DeviceId { get; } = DeviceId;
    public RegionOfInterest Region { get; } = Region;
}

internal record CcdGetAcquisitionDataCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_getAcquisitionData", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetAcquisitionBusyCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_getAcquisitionBusy", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetAcquisitionAbortCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_setAcquisitionAbort", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetXAxisConversionTypeCommand(int DeviceId, ConversionType ConversionType)
    : ChargedCoupleDeviceCommand("ccd_setXAxisConversionType",
        new Dictionary<string, object>
        {
            { "index", DeviceId },
            { "type", (int)ConversionType }
        })
{
    public int DeviceId { get; } = DeviceId;
    public ConversionType ConversionType { get; } = ConversionType;
}

internal record CcdGetXAxisConversionTypeCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_getXAxisConversionType", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdRestartCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_restart", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetConfigCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getConfig", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetGainCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getGain", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetGainCommand(int DeviceId, Gain Gain) : ChargedCoupleDeviceCommand("ccd_setGain",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "token", (int)Gain }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Gain Gain { get; } = Gain;
}

internal record CcdGetFitParametersCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getFitParams", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetFitParametersCommand(int DeviceId, string Params) : ChargedCoupleDeviceCommand("ccd_setFitParams",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "params", Params }
    })
{
    public int DeviceId { get; } = DeviceId;
    public string Params { get; } = Params;
}

internal record CcdGetTimerResolutionCommand(int DeviceId)
    : ChargedCoupleDeviceCommand("ccd_getTimerResolution", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetTimerResolutionCommand(int DeviceId, TimerResolution Resolution) : ChargedCoupleDeviceCommand(
    "ccd_setTimerResolution",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "resolutionToken", (int)Resolution }
    })
{
    public int DeviceId { get; } = DeviceId;
    public TimerResolution Resolution { get; } = Resolution;
}

internal record CcdSetAcquisitionFormatCommand(int DeviceId, AcquisitionFormat Format, int NumberOfRois)
    : ChargedCoupleDeviceCommand("ccd_setAcqFormat",
        new Dictionary<string, object>
        {
            { "index", DeviceId },
            { "format", (int)Format },
            { "numberOfRois", NumberOfRois }
        })
{
    public int DeviceId { get; } = DeviceId;
    public AcquisitionFormat Format { get; } = Format;
    public int NumberOfRois { get; } = NumberOfRois;
}

internal record CcdSetAcquisitionCountCommand(int DeviceId, int Count) : ChargedCoupleDeviceCommand("ccd_setAcqCount",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "count", Count }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int Count { get; } = Count;
}

internal record CcdGetAcquisitionCountCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getAcqCount", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetCleanCountCommand(int DeviceId, int Count, CleanCountMode Mode) : ChargedCoupleDeviceCommand(
    "ccd_setCleanCount",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "count", Count },
        { "mode", (int)Mode }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int Count { get; } = Count;
    public CleanCountMode Mode { get; } = Mode;
}

internal record CcdGetCleanCountCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getCleanCount", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetDataSizeCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getDataSize", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdGetTriggerInCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getTriggerIn", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetTriggerInCommand(int DeviceId, Trigger Trigger, bool IsEnabled) : ChargedCoupleDeviceCommand(
    "ccd_setTriggerIn",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "enable", IsEnabled },
        { "address", (int)Trigger.TriggerAddress },
        { "event", (int)Trigger.TriggerEvent },
        { "signalType", (int)Trigger.TriggerSignalType }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Trigger Trigger { get; } = Trigger;
    public bool IsEnabled { get; } = IsEnabled;
}

internal record CcdGetSignalOutCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getSignalOut", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdSetSignalOutCommand(int DeviceId, Signal Signal, bool IsEnabled) : ChargedCoupleDeviceCommand(
    "ccd_setSignalOut",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "enable", IsEnabled },
        { "address", (int)Signal.SignalAddress },
        { "event", (int)Signal.SignalEvent},
        { "signalType", (int)Signal.SignalType }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Signal Signal { get; } = Signal;
    public bool IsEnabled { get; } = IsEnabled;
}