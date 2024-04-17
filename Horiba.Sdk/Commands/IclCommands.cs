using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Commands;

internal record IclInfoCommand() : Command("icl_info");
internal record IclShutdownCommand() : Command("icl_shutdown");
internal record IclBinaryModeAllCommand()
    : Command("icl_binMode", new Dictionary<string, object> { { "mode", "all" } });
internal record IclCcdListCommand() : Command("ccd_list");
internal record IclCcdListCountCommand() : Command("ccd_listCount");
internal record IclDiscoverCcdCommand() : Command("ccd_discover");
internal record IclDiscoverMonochromatorDevicesCommand() : Command("mono_discover");
internal record IclMonochromatorListCommand() : Command("mono_list");
internal record IclMonochromatorListCountCommand() : Command("mono_listCount");