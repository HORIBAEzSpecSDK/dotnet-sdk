using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Commands;

internal record IclInfoCommand() : Command("icl_info");

internal record IclShutdownCommand() : Command("icl_shutdown");

internal record IclBinaryModeAllCommand()
    : Command("icl_binMode", new Dictionary<string, object> { { "mode", "all" } });

internal record IclDiscoverCcdCommand(): Command("ccd_list");

internal record IclDiscoverMonochromatorDevicesCommand(): Command("mono_list");