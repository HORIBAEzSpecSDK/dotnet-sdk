namespace Horiba.Sdk.Tests;

public class ChargedCoupleDeviceTestFixture : IDisposable
{
    public readonly ChargedCoupledDevice Ccd;
    public readonly DeviceManager Dm;
    
    public ChargedCoupleDeviceTestFixture()
    {
        Dm = new DeviceManager();
        Dm.StartAsync().Wait();
        Ccd = Dm.ChargedCoupledDevices.First();
        Ccd.OpenConnectionAsync().Wait();
    }
    
    public void Dispose()
    {
        Ccd.CloseConnectionAsync().Wait();
        Dm.StopAsync().Wait();
    }
}