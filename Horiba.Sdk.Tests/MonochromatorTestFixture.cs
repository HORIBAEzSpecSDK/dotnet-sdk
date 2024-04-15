namespace Horiba.Sdk.Tests;

/// <summary>
/// Shares a common context between all tests in a test class
/// </summary>
public class MonochromatorTestFixture : IDisposable
{
    /// <summary>
    /// Represents the device under test
    /// </summary>
    public readonly MonochromatorDevice Mono;
    public readonly DeviceManager Dm;

    
    /// <summary>
    /// Invoked once when the execution of a test class starts
    /// </summary>
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
    
    /// <summary>
    /// Invoked once when the execution of a test class ends
    /// </summary>
    public void Dispose()
    {
        Mono.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}