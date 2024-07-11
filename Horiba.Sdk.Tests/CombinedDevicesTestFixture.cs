namespace Horiba.Sdk.Tests;

public class CombinedDevicesTestFixture : IDisposable
{
    /// <summary>
    /// Represents the device under test
    /// </summary>
    public readonly ChargedCoupledDevice Ccd;
    public readonly MonochromatorDevice Mono;
    public readonly DeviceManager Dm;
    
    /// <summary>
    /// Invoked once when the execution of a test class starts
    /// </summary>
    public CombinedDevicesTestFixture()
    {
        Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        Ccd = Dm.ChargedCoupledDevices.First();
        Mono = Dm.Monochromators.First();
        Ccd.OpenConnectionAsync().Wait();
        Mono.OpenConnectionAsync().Wait();
    }
    
    /// <summary>
    /// Invoked once when the execution of a test class ends
    /// </summary>
    public void Dispose()
    {
        Ccd.CloseConnectionAsync().Wait();
        Mono.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}