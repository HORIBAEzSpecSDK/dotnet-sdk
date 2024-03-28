namespace Horiba.Sdk.Devices;

public abstract class DeviceDiscovery<T> where T : Device
{
    public abstract Task<List<T>> DiscoverDevicesAsync();
}