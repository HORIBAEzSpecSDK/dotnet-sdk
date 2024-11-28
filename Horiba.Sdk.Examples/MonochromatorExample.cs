using System.Threading.Tasks;
using Horiba.Sdk.Devices;
namespace Horiba.Sdk.Examples;



class Program
{
    static async void Main()
    {
        var Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        var Mono = Dm.Monochromators.First();
        Mono.OpenConnectionAsync().Wait();
        Mono.HomeAsync().Wait();
        Mono.WaitForDeviceNotBusy().Wait();
        Task.Delay(TimeSpan.FromSeconds(10)).Wait();
        Console.WriteLine(Mono.GetCurrentWavelengthAsync());

        bool monoIsBusy = true;
        while (monoIsBusy)
        {
            await Task.Delay(100);
            monoIsBusy = await Mono.IsDeviceBusyAsync();
        }
        Console.WriteLine(Mono.GetCurrentWavelengthAsync());

        Mono.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}