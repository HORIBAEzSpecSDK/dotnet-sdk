using Horiba.Sdk.Devices;
namespace Horiba.Sdk.Examples;



class Program

{

    static async Task MonoExample()
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

    static async Task Main()
    {
        await MonoExample();
    }
}