using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public sealed class DeviceManager : IDeviceManager
{
    public WebSocketCommunicator Communicator { get; private set; }
    public List<MonochromatorDevice> Monochromators { get; private set; }
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; }

    public DeviceManager()
    {
        Communicator = new WebSocketCommunicator();
    }

    public async Task StartAsync(bool startIcl = true)
    {
        if (startIcl)
        {
            // TODO start the external process
        }
        
        // TODO get ICL info

        await DiscoverDevicesAsync();
    }

    public async Task StopAsync()
    {
        // TODO stop procedure: open communicator, send icl_info, send icl_shutdown, close communicator, stop external process, 
    }

    public async Task DiscoverDevicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Communicator.OpenConnectionAsync(cancellationToken);

            await Communicator.SendAsync(new IclInfoCommand());
            Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync();
            ChargedCoupledDevices = await new ChargedCoupleDeviceDeviceDiscovery(Communicator).DiscoverDevicesAsync();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}