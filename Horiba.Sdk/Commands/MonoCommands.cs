using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Commands;

internal record MonoOpenCommand(int DeviceId) : MonochromatorDeviceCommand("mono_open", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoCloseCommand(int DeviceId) : MonochromatorDeviceCommand("mono_close", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoIsOpenCommand(int DeviceId) : MonochromatorDeviceCommand("mono_isOpen", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoIsBusyCommand(int DeviceId) : MonochromatorDeviceCommand("mono_isBusy", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoInitCommand(int DeviceId) : MonochromatorDeviceCommand("mono_init", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoGetConfigurationCommand(int DeviceId) : MonochromatorDeviceCommand("mono_getConfig", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoGetPositionCommand(int DeviceId) : MonochromatorDeviceCommand("mono_getPosition", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoSetPositionCommand(int DeviceId, float Wavelength) : MonochromatorDeviceCommand("mono_setPosition",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "wavelength", Wavelength }
    })
{
    public int DeviceId { get; } = DeviceId;
    public float Wavelength { get; } = Wavelength;
}

internal record MonoMoveToPositionCommand(int DeviceId, float Wavelength) : MonochromatorDeviceCommand(
    "mono_moveToPosition", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "wavelength", Wavelength }
    })
{
    public int DeviceId { get; } = DeviceId;
    public float Wavelength { get; } = Wavelength;
}

internal record MonoGetGratingPositionCommand(int DeviceId)
    : MonochromatorDeviceCommand("mono_getGratingPosition", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoMoveGratingCommand(int DeviceId, Grating Grating) : MonochromatorDeviceCommand("mono_moveGrating",
    new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "position", (int)Grating }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Grating Grating { get; } = Grating;
}

internal record MonoGetFilterWheelPositionCommand(int DeviceId, FilterWheel FilterWheel) : MonochromatorDeviceCommand(
    "mono_getFilterWheelPosition", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int)FilterWheel}
    })
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoMoveFilterWheelCommand(int DeviceId, FilterWheel FilterWheel, FilterWheelPosition FilterWheelPosition)
    : MonochromatorDeviceCommand("mono_moveFilterWheel", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int) FilterWheel},
        { "position", (int)FilterWheelPosition }
    })
{
    public int DeviceId { get; } = DeviceId;
    public FilterWheelPosition FilterWheelPosition { get; } = FilterWheelPosition;
}

internal record MonoGetMirrorPositionCommand(int DeviceId, Mirror Mirror) : MonochromatorDeviceCommand(
    "mono_getMirrorPosition", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int)Mirror }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Mirror Mirror { get; } = Mirror;
}

internal record MonoMoveMirrorCommand(int DeviceId, Mirror Mirror, MirrorPosition MirrorPosition)
    : MonochromatorDeviceCommand("mono_moveMirror", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int)Mirror },
        { "position", (int)MirrorPosition }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Mirror Mirror { get; } = Mirror;
    public MirrorPosition MirrorPosition { get; } = MirrorPosition;
}

internal record MonoGetSlitPositionInMMCommand(int DeviceId, Slit Slit) : MonochromatorDeviceCommand(
    "mono_getSlitPositionInMM", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int)Slit }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Slit Slit { get; } = Slit;
}

internal record MonoMoveSlitMMCommand(int DeviceId, Slit Slit, float PositionInMM) : MonochromatorDeviceCommand(
    "mono_moveSlitMM", new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "locationId", (int)Slit },
        { "position", PositionInMM }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Slit Slit { get; } = Slit;
    public float PositionInMM { get; } = PositionInMM;
}

internal record MonoShutterOpenCommand(int DeviceId) : MonochromatorDeviceCommand("mono_shutterOpen", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoShutterCloseCommand(int DeviceId) : MonochromatorDeviceCommand("mono_shutterClose", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoGetShutterStatusCommand(int DeviceId)
    : MonochromatorDeviceCommand("mono_getShutterStatus", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}

internal record MonoShutterSelectCommand(int DeviceId, Shutter Shutter)
    : MonochromatorDeviceCommand("mono_shutterSelect",
        new Dictionary<string, object>
    {
        { "index", DeviceId },
        { "shutter", (int)Shutter }
    })
{
    public int DeviceId { get; } = DeviceId;
    public Shutter Shutter { get; } = Shutter;
}

internal record MonoGetIsInitializedCommand(int DeviceId) : MonochromatorDeviceCommand("mono_isInitialized", DeviceId)
{
    public int DeviceId { get; } = DeviceId;
}