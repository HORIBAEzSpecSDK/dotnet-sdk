using Horiba.Sdk.Communication;

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
    : ChargedCoupleDeviceCommand("ccd_serRoi", Region.ToDeviceParameters(DeviceId))
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