namespace Horiba.Sdk.Tests;

/// <summary>
/// Shares a common context between all tests in a test class
/// </summary>
public class SpectrAcqDeviceTestFixture : IDisposable
{
    /// <summary>
    /// Represents the device under test
    /// </summary>
    public readonly SpectrAcqDevice Saq;
    public readonly DeviceManager Dm;
    
    /// <summary>
    /// Invoked once when the execution of a test class starts
    /// </summary>
    public SpectrAcqDeviceTestFixture()
    {
        Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        Saq = Dm.SpectrAcqDevices.First();
        Saq.OpenConnectionAsync().Wait();
    }
    
    /// <summary>
    /// Invoked once when the execution of a test class ends
    /// </summary>
    public void Dispose()
    {
        Saq.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}