using System.Diagnostics;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed class DeviceManager : IDeviceManager, IDisposable
{
    internal readonly Process IclProcess = new();
    private bool _isIclRunning;

    public DeviceManager(string iclExePath = @"C:\Program Files\HORIBA Scientific\SDK\icl.exe")
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        Communicator = new WebSocketCommunicator();
        IclProcess.StartInfo.FileName = iclExePath;
        IclProcess.Exited += IclProcessOnExited;
    }

    public WebSocketCommunicator Communicator { get; }
    public List<MonochromatorDevice> Monochromators { get; private set; } = [];
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; } = [];

    public async Task StartAsync(bool startIcl = true, bool enableBinaryMessages = false)
    {
        if (startIcl)
        {
            Log.Debug("Starting ICL...");
            IclProcess.Start();
            _isIclRunning = true;
        }

        await Communicator.OpenConnectionAsync();

        if (enableBinaryMessages) await Communicator.SendWithResponseAsync(new IclBinaryModeAllCommand());

        await DiscoverDevicesAsync();
    }

    public async Task StopAsync()
    {
        // TODO should we log these responses? Why do we need them?
        var info = await Communicator.SendWithResponseAsync(new IclInfoCommand());

        await Communicator.SendAsync(new IclShutdownCommand());

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
            Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
            ChargedCoupledDevices = await new ChargedCoupleDeviceDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        }
        catch (Exception e)
        {
        }
    }

    public void Dispose()
    {
        if (_isIclRunning)
        {
            Log.Debug("Killing ICL process...");
            IclProcess.Kill();
        }

        IclProcess.Exited -= IclProcessOnExited;
        IclProcess.Dispose();
        if (Communicator.IsConnectionOpened) Communicator.CloseConnectionAsync();
    }

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        Log.Debug("ICL process terminated");
        _isIclRunning = false;
    }
}