using System.Diagnostics;
using System.Net;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Serilog;

namespace Horiba.Sdk.Devices;

public sealed class DeviceManager : IDisposable
{
    internal readonly Process IclProcess = new();
    private bool _isIclRunning;

    public DeviceManager(string? iclExePath = null, IPAddress? ipAddress = null, int? port = null)
    {
        // TODO document the option to use Sentry sink
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        Communicator = new WebSocketCommunicator(ipAddress ?? IPAddress.Loopback, port ?? 25010);
        
        IclProcess.StartInfo.FileName = iclExePath ??
                                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\HORIBA Scientific\SDK\icl.exe";
        IclProcess.Exited += IclProcessOnExited;
    }

    public WebSocketCommunicator Communicator { get; }
    public List<MonochromatorDevice> Monochromators { get; private set; } = [];
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; } = [];

    public void Dispose()
    {
        if (_isIclRunning)
        {
            Log.Debug("Killing ICL process...");
            IclProcess.Kill();
        }

        IclProcess.Exited -= IclProcessOnExited;
        IclProcess.Dispose();
        if (Communicator.IsConnectionOpened)
        {
            Communicator.CloseConnectionAsync();
        }
    }

    public async Task StartAsync(bool startIcl = true, bool enableBinaryMessages = false)
    {
        if (startIcl)
        {
            Log.Debug("Starting ICL...");
            IclProcess.Start();
            _isIclRunning = true;
        }

        await Communicator.OpenConnectionAsync();

        if (enableBinaryMessages)
        {
            // TODO TEST if enabled, can I see binary message?
            await Communicator.SendAsync(new IclBinaryModeAllCommand());
        }

        await DiscoverDevicesAsync();
    }

    public async Task StopAsync()
    {
        await Communicator.SendAsync(new IclShutdownCommand());

        if (_isIclRunning)
        {
            IclProcess.Kill();
            _isIclRunning = false;
        }
    }

    public async Task DiscoverDevicesAsync(CancellationToken cancellationToken = default)
    {
        Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        ChargedCoupledDevices = await new ChargedCoupleDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
    }

    public async Task<string> GetIclInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclInfoCommand(), cancellationToken);
        return result.Results["info"].ToString();
    }

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        Log.Debug("ICL process terminated");
        _isIclRunning = false;
    }
}