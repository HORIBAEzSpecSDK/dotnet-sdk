using Horiba.Sdk.Devices;
using Serilog.Core;
namespace Horiba.Sdk.Examples.Mono;



class MonoProgram

{

    public static async Task MainAsync()
    {
        DeviceManager Dm = new DeviceManager();
        await Dm.StartAsync();
        var Mono = Dm.Monochromators.First();
        await Mono.OpenConnectionAsync();
        await Mono.HomeAsync();
        await Mono.WaitForDeviceNotBusy();

        Console.WriteLine(await Mono.GetCurrentWavelengthAsync());
        await Mono.MoveToWavelengthAsync(200);
        await Mono.WaitForDeviceNotBusy();
        Console.WriteLine(await Mono.GetCurrentWavelengthAsync());

        await Mono.SetMirrorPositionAsync(Enums.Mirror.Entrance, Enums.MirrorPosition.Axial);
        await Mono.WaitForDeviceNotBusy();
            
        await Mono.CloseConnectionAsync();
        await Dm.StopAsync();
    }

}