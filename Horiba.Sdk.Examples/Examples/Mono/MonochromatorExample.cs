using Horiba.Sdk.Devices;
using Serilog.Core;
namespace Horiba.Sdk.Examples.Mono;



class MonoProgram : IExample

{

    public async Task MainAsync(bool showIclConsoleOutput= false)
    {
        DeviceManager deviceManager = new DeviceManager(showIclConsoleOutput:showIclConsoleOutput);
        await deviceManager.StartAsync();
        var mono = deviceManager.Monochromators.First();
        await mono.OpenConnectionAsync();
        await mono.HomeAsync();
        await mono.WaitForDeviceNotBusy();

        Console.WriteLine(await mono.GetCurrentWavelengthAsync());
        await mono.MoveToWavelengthAsync(200);
        await mono.WaitForDeviceNotBusy();
        Console.WriteLine(await mono.GetCurrentWavelengthAsync());

        await mono.SetMirrorPositionAsync(Enums.Mirror.Entrance, Enums.MirrorPosition.Axial);
        await mono.WaitForDeviceNotBusy();
            
        await mono.CloseConnectionAsync();
        await deviceManager.StopAsync();
    }

}