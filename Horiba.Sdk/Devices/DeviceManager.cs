using System.Diagnostics;
using System.Net;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Serilog;

namespace Horiba.Sdk.Devices;

/// <summary>
/// Provides centralized management and discovery of HORIBA spectroscopic instruments.
/// This class serves as the main entry point for controlling monochromators, CCDs, and SpectrAcq devices.
/// It handles ICL process management, device discovery, and maintains WebSocket communication with instruments.
/// </summary>
/// <remarks>
/// The DeviceManager automatically starts the ICL (Instrument Control Layer) process if not already running,
/// establishes WebSocket communication, and provides thread-safe access to all connected devices.
/// </remarks>
/// <example>
/// <code>
/// // Initialize device manager with default settings
/// var deviceManager = new DeviceManager();
/// await deviceManager.StartAsync();
/// 
/// // Discover all connected devices
/// var devices = await deviceManager.DiscoverDevicesAsync();
/// 
/// // Access specific device types
/// var monochromators = deviceManager.Monochromators;
/// var ccds = deviceManager.ChargedCoupledDevices;
/// 
/// // Clean up when done
/// deviceManager.Dispose();
/// </code>
/// </example>
public sealed class DeviceManager : IDisposable
{
    
    
    internal readonly Process IclProcess = new();
    private bool _isIclRunning = Process.GetProcessesByName("icl").Any();

    /// <summary>
    /// Initializes a new instance of the DeviceManager class with the specified configuration.
    /// </summary>
    /// <param name="iclExePath">The full path to the icl.exe executable. If null, defaults to the standard installation path: 'C:/Program Files/HORIBA Scientific/SDK/icl.exe'</param>
    /// <param name="ipAddress">The IP address of the machine running the ICL process. If null, defaults to '127.0.0.1' (localhost)</param>
    /// <param name="port">The TCP port on which the ICL process is listening for WebSocket connections. If null, defaults to 25010</param>
    /// <param name="showIclConsoleOutput">If true, shows the console output of the ICL process for debugging purposes. If false, output is redirected and hidden. Defaults to true</param>
    /// <remarks>
    /// The constructor sets up the WebSocket communicator and configures the ICL process but does not start it.
    /// Call <see cref="StartAsync"/> to begin communication with instruments.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Default configuration
    /// var manager = new DeviceManager();
    /// 
    /// // Custom configuration
    /// var customManager = new DeviceManager(
    ///     iclExePath: @"C:\Custom\Path\icl.exe",
    ///     ipAddress: IPAddress.Parse("192.168.1.100"),
    ///     port: 25011,
    ///     showIclConsoleOutput: false
    /// );
    /// </code>
    /// </example>
    public DeviceManager(string? iclExePath = null, IPAddress? ipAddress = null, int? port = null,
        bool showIclConsoleOutput = true)
    {
        Communicator = new WebSocketCommunicator(ipAddress ?? IPAddress.Loopback, port ?? 25010);
        IclProcess.StartInfo.FileName = iclExePath ??
                                            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\HORIBA Scientific\SDK\icl.exe";
        if (showIclConsoleOutput == false)
        {
            IclProcess.StartInfo.UseShellExecute = false;
            IclProcess.StartInfo.RedirectStandardOutput = true;
            IclProcess.StartInfo.RedirectStandardError = true;

        }

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
        if (_isIclRunning && IclProcess != null)
        {
            try
            {
                // Only kill if the process is still running and hasn't exited
                if (!IclProcess.HasExited)
                {
                    Log.Debug("Killing ICL process...");
                    IclProcess.Kill();
                }
            }
            catch (InvalidOperationException)
            {
                // Process has already exited or is not associated with a running process
                Log.Debug("ICL process was already terminated or not associated");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // Process could not be terminated (access denied, etc.)
                Log.Warning("Could not terminate ICL process: {Message}", ex.Message);
            }
        }

        IclProcess.Exited -= IclProcessOnExited;
        IclProcess.Dispose();
        if (Communicator.IsConnectionOpened)
        {
            Communicator.CloseConnectionAsync();
        }
    }

    /// <summary>
    /// Asynchronously initializes the ICL process, establishes WebSocket connection, and discovers all connected devices.
    /// </summary>
    /// <param name="startIcl">If true, starts the ICL process. If false, assumes ICL is already running. Defaults to true</param>
    /// <param name="enableBinaryMessages">If true, enables binary message mode for improved performance with large data transfers. Defaults to false</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="CommunicationException">Thrown when unable to establish connection to ICL process</exception>
    /// <remarks>
    /// This method performs the following operations in sequence:
    /// 1. Starts the ICL process (if startIcl is true)
    /// 2. Waits 5 seconds for ICL to initialize
    /// 3. Opens WebSocket connection
    /// 4. Optionally enables binary message mode
    /// 5. Discovers all connected devices
    /// </remarks>
    /// <example>
    /// <code>
    /// var deviceManager = new DeviceManager();
    /// 
    /// // Standard startup
    /// await deviceManager.StartAsync();
    /// 
    /// // Connect to existing ICL process with binary mode
    /// await deviceManager.StartAsync(startIcl: false, enableBinaryMessages: true);
    /// </code>
    /// </example>
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
        await Task.Delay(TimeSpan.FromSeconds(5));
        await DiscoverDevicesAsync();
    }

    /// <summary>
    /// Asynchronously terminates the connection to the ICL by sending shutdown command and stops the ICL process.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method gracefully shuts down the ICL connection and terminates the process if it was started by this instance.
    /// Always call this method or dispose the DeviceManager when finished to ensure proper cleanup.
    /// </remarks>
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
    /// Asynchronously discovers and initializes all connected HORIBA devices.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the discovery operation</param>
    /// <returns>A task representing the asynchronous discovery operation</returns>
    /// <remarks>
    /// This method discovers devices in parallel for optimal performance and populates the 
    /// Monochromators, ChargedCoupledDevices, and SpectrAcqDevices collections.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Discover devices with timeout
    /// using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    /// await deviceManager.DiscoverDevicesAsync(cts.Token);
    /// 
    /// Console.WriteLine($"Found {deviceManager.Monochromators.Count} monochromators");
    /// Console.WriteLine($"Found {deviceManager.ChargedCoupledDevices.Count} CCDs");
    /// </code>
    /// </example>
    public async Task DiscoverDevicesAsync(CancellationToken cancellationToken = default)
    {
        Monochromators = await new MonochromatorDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        ChargedCoupledDevices = await new ChargedCoupleDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
        SpectrAcqDevices = await new SpectrAcqDeviceDiscovery(Communicator).DiscoverDevicesAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves comprehensive ICL system information.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
    /// <returns>A dictionary containing ICL version, build information, and system status</returns>
    /// <exception cref="CommunicationException">Thrown when communication with ICL fails</exception>
    /// <remarks>
    /// The returned dictionary typically contains keys such as 'version', 'build', 'timestamp', and other system information.
    /// </remarks>
    /// <example>
    /// <code>
    /// var iclInfo = await deviceManager.GetIclInfoAsync();
    /// Console.WriteLine($"ICL Version: {iclInfo["version"]}");
    /// Console.WriteLine($"Build: {iclInfo["build"]}");
    /// </code>
    /// </example>
    public async Task<Dictionary<string,object>> GetIclInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclInfoCommand(), cancellationToken);
        return result.Results;
    }
    
    /// <summary>
    /// Asynchronously retrieves the total number of discovered monochromator devices.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
    /// <returns>The number of monochromator devices available</returns>
    /// <exception cref="CommunicationException">Thrown when communication with ICL fails</exception>
    /// <remarks>
    /// This method will only return accurate results after the discovery process has completed.
    /// Call <see cref="DiscoverDevicesAsync"/> first to ensure devices are discovered.
    /// </remarks>
    public async Task<long> GetMonochromatorCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclMonochromatorListCountCommand(), cancellationToken);
        return (long)result.Results["count"];
    }
    
    /// <summary>
    /// Asynchronously retrieves the total number of discovered CCD (Charged Coupled Device) instruments.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
    /// <returns>The number of CCD devices available</returns>
    /// <exception cref="CommunicationException">Thrown when communication with ICL fails</exception>
    /// <remarks>
    /// This method will only return accurate results after the discovery process has completed.
    /// Call <see cref="DiscoverDevicesAsync"/> first to ensure devices are discovered.
    /// </remarks>
    public async Task<long> GetCcdCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclCcdListCountCommand(), cancellationToken);
        return (long)result.Results["count"];
    }
    
    /// <summary>
    /// Asynchronously retrieves the total number of discovered SpectrAcq3 devices.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
    /// <returns>The number of SpectrAcq3 devices available</returns>
    /// <exception cref="CommunicationException">Thrown when communication with ICL fails</exception>
    /// <remarks>
    /// This method will only return accurate results after the discovery process has completed.
    /// Call <see cref="DiscoverDevicesAsync"/> first to ensure devices are discovered.
    /// </remarks>
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
