using System.Diagnostics;
using System.Net;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Serilog;

namespace Horiba.Sdk.Devices;

/// <summary>
/// This class is the entry point for controlling Horiba devices. It is responsible for establishing the connection to a device, starting up the ICL process, setting up the logging and triggering the discovery process of devices.
/// </summary>
public sealed class DeviceManager : IDisposable
{
    internal readonly Process IclProcess = new();
    private bool _isIclRunning;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iclExePath">The full path to the icl.exe. Defaults to 'C:/ProgramFiles/HORIBA Scientific/SDK/icl.exe'"</param>
    /// <param name="ipAddress">The address of the PC running the ICL process. Defaults to '127.0.0.1'</param>
    /// <param name="port">The port on which the ICL process is running. Defaults to 25010</param>
    public DeviceManager(string? iclExePath = null, IPAddress? ipAddress = null, int? port = null)
    {
        Communicator = new WebSocketCommunicator(ipAddress ?? IPAddress.Loopback, port ?? 25010);
        
        IclProcess.StartInfo.FileName = iclExePath ??
                                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\HORIBA Scientific\SDK\icl.exe";
        
        // NOTE first unsubscribe then subscribe so that we lower the chance to
        // have a memory leak in the case of unexpected crash
        IclProcess.Exited -= IclProcessOnExited;
        IclProcess.Exited += IclProcessOnExited;
    }

    public WebSocketCommunicator Communicator { get; }
    public List<MonochromatorDevice> Monochromators { get; private set; } = [];
    public List<ChargedCoupledDevice> ChargedCoupledDevices { get; private set; } = [];
    public List<SpectrAcqDevice> SpectrAcqDevices { get; private set; } = [];

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

    /// <summary>
    /// Initializes the connection to the ICL and starts the discovery process of devices.
    /// </summary>
    /// <param name="startIcl"></param>
    /// <param name="enableBinaryMessages"></param>
    public async Task StartAsync(bool startIcl = true, bool enableBinaryMessages = false)
    {
        if (startIcl)
        {
            Log.Debug("Starting ICL...");
            IclProcess.Start();
            _isIclRunning = true;
        }

        await Task.Delay(TimeSpan.FromSeconds(5));
        await Communicator.OpenConnectionAsync();

        if (enableBinaryMessages)
        {
            await Communicator.SendAsync(new IclBinaryModeAllCommand());
        }

        await DiscoverDevicesAsync();
    }

    /// <summary>
    /// Terminates the connection to the ICL by sending icl_shutdown and kills the related (if any) ICL process.
    /// </summary>
    public async Task StopAsync()
    {
        await Communicator.SendAsync(new IclShutdownCommand());

        if (_isIclRunning)
        {
            IclProcess.Kill();
            _isIclRunning = false;
        }
    }

    /// <summary>
    /// Initiates the discovery process of devices.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task DiscoverDevicesAsync(CancellationToken cancellationToken = default)
    {
        Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        ChargedCoupledDevices = await new ChargedCoupleDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        SpectrAcqDevices = await new SpectrAcqDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the ICL information by sending the icl_info command.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string,object>> GetIclInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclInfoCommand(), cancellationToken);
        return result.Results;
    }
    
    /// <summary>
    /// Retrieves the number of monochromator devices by sending the mono_listCount command.
    /// This command will return result only after completing discovery process.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> GetMonochromatorCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclMonochromatorListCountCommand(), cancellationToken);
        return (long)result.Results["count"];
    }
    
    /// <summary>
    /// Retrieves the number of Charged Couple Devices by sending the ccd_listCount command.
    /// This command will return result only after completing discovery process.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> GetCcdCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclCcdListCountCommand(), cancellationToken);
        return (long)result.Results["count"];
    }
    
        /// <summary>
    /// Retrieves the number of SpectrAcq3 Devices by sending the saq3_listCount command.
    /// This command will return result only after completing discovery process.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> GetSaqCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclSpectrAcqListCountCommand(), cancellationToken);
        return (long)result.Results["count"];
    }
    
    

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        Log.Debug("ICL process terminated");
        _isIclRunning = false;
    }
}