﻿using System.Diagnostics;
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
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
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

    public async Task<Dictionary<string,object>> GetIclInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Communicator.SendWithResponseAsync(new IclInfoCommand(), cancellationToken);
        return result.Results;
    }

    private void IclProcessOnExited(object sender, EventArgs e)
    {
        Log.Debug("ICL process terminated");
        _isIclRunning = false;
    }
}