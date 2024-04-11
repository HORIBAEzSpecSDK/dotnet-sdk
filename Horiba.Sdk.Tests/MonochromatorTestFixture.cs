namespace Horiba.Sdk.Tests;

public class MonochromatorTestFixture : IDisposable
{
    public readonly DeviceManager Dm;
    public readonly MonochromatorDevice Mono;
    
    public MonochromatorTestFixture()
    {
        Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        Mono = Dm.Monochromators.First();
        Mono.OpenConnectionAsync().Wait();
        Mono.HomeAsync().Wait();
        Mono.WaitForDeviceNotBusy().Wait();
        Task.Delay(TimeSpan.FromSeconds(10)).Wait();
    }

    public void Dispose()
    {
        Mono.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}