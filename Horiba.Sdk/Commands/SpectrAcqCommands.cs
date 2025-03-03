using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Commands;

internal record SaqOpenCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_open", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqCloseCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_close", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqIsOpenCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_isOpen", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqIsBusyCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_isBusy", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetFirmwareVersionCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getFirmwareVersion", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetFpgaVersionCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getFPGAVersion", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetBoardRevisionCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getBoardRevision", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetSerialNumberCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getSerialNumber", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqSetIntegrationTimeCommand(int DeviceId, int IntegrationTimeInSec) : SpectrAcqDeviceCommand(
    "saq3_setIntegrationTime", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "integrationTime", IntegrationTimeInSec }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int IntegrationTimeInSec { get; } = IntegrationTimeInSec;
}

internal record SaqGetIntegrationTimeCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getIntegrationTime", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqSetHvBiasVoltageCommand(int DeviceId, int BiasVoltage) : SpectrAcqDeviceCommand(
    "saq3_setHVBiasVoltage", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "biasVoltage", BiasVoltage }
    })
{
    public int DeviceId { get; } = DeviceId;
    public int BiasVoltage { get; } = BiasVoltage;
}

internal record SaqGetHvBiasVoltageCommand(int DeviceId) : SpectrAcqDeviceCommand("saq3_getHVBiasVoltage", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetMaxHVVoltageAllowedCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getMaxHVVoltageAllowed", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqDefineAcqSetCommand(
    int DeviceId,
    int ScanCount,
    int TimeStep,
    int IntegrationTime,
    int ExternalParam) : SpectrAcqDeviceCommand("saq3_defineAcqSet", new Dictionary<string, object>
{
    { "index", DeviceId },
    { "scanCount", ScanCount },
    { "timeStep", TimeStep },
    { "integrationTime", IntegrationTime },
    { "externalParam", ExternalParam }
})
{
    public int DeviceId { get; } = DeviceId;
    public int ScanCount { get; } = ScanCount;
    public int TimeStep { get; } = TimeStep;
    public int IntegrationTime { get; } = IntegrationTime;
    public int ExternalParam { get; } = ExternalParam;
}

internal record SaqGetAcqSetCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getAcqSet", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqAcqStartCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_acqStart", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqAcqStopCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_acqStop", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqAcqPauseCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_acqPause", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqAcqContinueCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_acqContinue", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqIsDataAvailableCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_isDataAvailable", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetAvailableDataCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getAvailableData", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqForceTriggerCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_forceTrigger", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqSetInTriggerModeCommand(int DeviceId, InTriggerMode Mode) : SpectrAcqDeviceCommand(
    "saq3_setInTriggerMode", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "mode", Mode }
    })
{
    public int DeviceId { get; } = DeviceId;
    public InTriggerMode Mode { get; } = Mode;
}

internal record SaqGetTriggerModeCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getTriggerMode", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetLastErrorModeCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getLastError", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqGetErrorLogCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_getErrorLog", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record SaqClearErrorLogCommand(int DeviceId)
    : SpectrAcqDeviceCommand("saq3_clearErrorLog", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}



