using System.Diagnostics;
using System.Xml;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public sealed class DeviceManager : IDeviceManager, IDisposable
{
    private bool _isIclRunning;
    internal readonly Process IclProcess = new();
    public WebSocketCommunicator Communicator { get; }
    public List<MonochromatorDevice> Monochromators { get; private set; }
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; }

    public DeviceManager(string iclExePath = @"C:\Program Files\HORIBA Scientific\SDK\icl.exe")
    {
        Communicator = new WebSocketCommunicator();
        IclProcess.StartInfo.FileName = iclExePath;
        IclProcess.Exited += IclProcessOnExited;
    }

    public async Task StartAsync(bool startIcl = true)
    {
        if (startIcl)
        {
            IclProcess.Start();
            _isIclRunning = true;
        }
        
        // TODO get ICL info

        //await DiscoverDevicesAsync();
    }

    public async Task StopAsync()
    {
        // TODO stop procedure: open communicator, send icl_info, send icl_shutdown, close communicator, stop external process,
        if (_isIclRunning)
        {
            IclProcess.Kill();
            _isIclRunning = false;
        }
    }

    public async Task DiscoverDevicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Communicator.OpenConnectionAsync(cancellationToken);
            Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync();
            ChargedCoupledDevices = await new ChargedCoupleDeviceDeviceDiscovery(Communicator).DiscoverDevicesAsync();

        }
        catch (Exception e)
        {
        }
        finally
        {
            await Communicator.CloseConnectionAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        if (_isIclRunning)
        {
            IclProcess.Kill();
        }
        IclProcess.Dispose();
    }

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        _isIclRunning = false;
    }
}