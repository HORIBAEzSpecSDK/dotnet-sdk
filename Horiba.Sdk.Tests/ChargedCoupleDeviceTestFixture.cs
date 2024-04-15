namespace Horiba.Sdk.Tests;

/// <summary>
/// Shares a common context between all tests in a test class
/// </summary>
public class ChargedCoupleDeviceTestFixture : IDisposable
{
    /// <summary>
    /// Represents the device under test
    /// </summary>
    public readonly ChargedCoupledDevice Ccd;
    public readonly DeviceManager Dm;
    
    /// <summary>
    /// Invoked once when the execution of a test class starts
    /// </summary>
    public ChargedCoupleDeviceTestFixture()
    {
        Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        Ccd = Dm.ChargedCoupledDevices.First();
        Ccd.OpenConnectionAsync().Wait();
    }
    
    /// <summary>
    /// Invoked once when the execution of a test class ends
    /// </summary>
    public void Dispose()
    {
        Ccd.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}