namespace Horiba.Sdk.Devices;

internal abstract class DeviceDiscovery<T> where T : Device
{
    public abstract Task<List<T>> DiscoverDevicesAsync(CancellationToken cancellationToken);
}