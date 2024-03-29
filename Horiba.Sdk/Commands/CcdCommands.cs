using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Commands;

internal record CcdOpenCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_open")
{
    public int DeviceId { get; } = DeviceId;
}

internal record CcdCloseCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_close")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdIsConnectionOpenedCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_isOpen")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdGetTemperatureCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_getChipTemperature")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdGetChipSizeCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_getChipSize")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdGetSpeedCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_getSpeed")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdGetExposureTimeCommand(int DeviceId) : ChargedCoupleDeviceCommand(DeviceId, "ccd_getExposureTime")
{
    public int DeviceId { get; } = DeviceId;
}
internal record CcdSetExposureTimeCommand(int DeviceId, int ExposureTimeInMs) : ChargedCoupleDeviceCommand(DeviceId, "ccd_setExposureTime")
{
    public int DeviceId { get; } = DeviceId;
    public int ExposureTimeInMs { get; } = ExposureTimeInMs;
}