using System.Diagnostics;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;

namespace Horiba.Sdk.Devices;

public sealed class DeviceManager : IDeviceManager, IDisposable
{
    private bool _isIclRunning;
    internal readonly Process IclProcess = new();
    public WebSocketCommunicator Communicator { get; }
    public List<MonochromatorDevice> Monochromators { get; private set; } = [];
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; } = [];

    public DeviceManager(string iclExePath = @"C:\Program Files\HORIBA Scientific\SDK\icl.exe")
    {
        Communicator = new WebSocketCommunicator();
        IclProcess.StartInfo.FileName = iclExePath;
        IclProcess.Exited += IclProcessOnExited;
    }

    public async Task StartAsync(bool startIcl = true, bool enableBinaryMessages = true)
    {
        if (startIcl)
        {
            IclProcess.Start();
            _isIclRunning = true;
        }

        await Communicator.OpenConnectionAsync();
        
        // TODO should we log these responses? Why do we need them?
        var info = await Communicator.SendWithResponseAsync(new IclInfoCommand());

        if (enableBinaryMessages)
        {
            await Communicator.SendWithResponseAsync(new IclBinaryModeAllCommand());
        }

        //await DiscoverDevicesAsync();
    }

    public async Task StopAsync()
    {
        if (!Communicator.IsConnectionOpened)
        {
            await Communicator.OpenConnectionAsync();
        }

        // TODO should we log these responses? Why do we need them?
        var info = await Communicator.SendWithResponseAsync(new IclInfoCommand());

        await Communicator.SendAsync(new IclShutdownCommand());
        await Communicator.CloseConnectionAsync();
        
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
            Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync();
            ChargedCoupledDevices = await new ChargedCoupleDeviceDeviceDiscovery(Communicator).DiscoverDevicesAsync();
        }
        catch (Exception e)
        {
        }
    }

    public void Dispose()
    {
        if (_isIclRunning)
        {
            IclProcess.Kill();
        }
        IclProcess.Exited -= IclProcessOnExited;
        IclProcess.Dispose();
        Communicator.CloseConnectionAsync();
    }

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        _isIclRunning = false;
    }
}