namespace Horiba.Sdk.Devices;

public abstract class DeviceDiscoveryStrategy<T> where T : Device
{
    public abstract Task<List<T>> DiscoverDevicesAsync();
}