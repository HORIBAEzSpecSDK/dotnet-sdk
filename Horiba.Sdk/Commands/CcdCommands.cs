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
internal record CcdGetExposureTimeCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getExposureTime", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetExposureTimeCommand(int DeviceId, int ExposureTimeInMs) : ChargedCoupleDeviceCommand("ccd_setExposureTime", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "time", ExposureTimeInMs }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int ExposureTimeInMs { get; } = ExposureTimeInMs;
}
internal record CcdGetAcquisitionReadyCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getAcquisitionReady", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetAcquisitionStartCommand(int DeviceId, bool IsShutterOpened) : ChargedCoupleDeviceCommand("ccd_setAcquisitionStart", 
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
internal record CcdGetAcquisitionDataCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getAcquisitionData", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdGetAcquisitionBusyCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getAcquisitionBusy", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetAcquisitionAbortCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_setAcquisitionAbort", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetXAxisConversionTypeCommand(int DeviceId, ConversionType ConversionType) : ChargedCoupleDeviceCommand("ccd_setXAxisConversionType", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "type", ConversionType }
    })
{
    public int DeviceId { get; } = DeviceId;
    public ConversionType ConversionType { get; } = ConversionType;
}
internal record CcdGetXAxisConversionTypeCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getXAxisConversionType", DeviceId)
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
internal record CcdGetNumberOfAveragesCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getNumberOfAvgs", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetNumberOfAveragesCommand(int DeviceId, int NumberOfAverages) : ChargedCoupleDeviceCommand("ccd_setNumberOfAvgs", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "count", NumberOfAverages }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int NumberOfAverages { get; } = NumberOfAverages;
}
internal record CcdGetGainCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getGain", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetGainCommand(int DeviceId, Gain Gain) : ChargedCoupleDeviceCommand("ccd_setNumberOfAvgs", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        // TODO do we use the string representation or the number representation of the enum
        { "token", Gain }
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
internal record CcdGetTimerResolutionCommand(int DeviceId) : ChargedCoupleDeviceCommand("ccd_getTimerResolution", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetTimerResolutionCommand(int DeviceId, int Resolution) : ChargedCoupleDeviceCommand("ccd_setTimerResolution", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "resolution", Resolution }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int Resolution { get; } = Resolution;
}
internal record CcdSetAcquisitionFormatCommand(int DeviceId, AcquisitionFormat Format, int NumberOfRois) : ChargedCoupleDeviceCommand("ccd_setAcqFormat", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "format", Format },
        { "numberOfRois", Format }
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
internal record CcdSetCleanCountCommand(int DeviceId, int Count, CleanCountMode Mode) : ChargedCoupleDeviceCommand("ccd_setCleanCount", 
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "count", Count },
        { "mode", Mode }
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