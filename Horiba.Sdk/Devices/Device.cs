using System.Diagnostics.CodeAnalysis;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Devices;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract record Device(int DeviceId, string DeviceType, string SerialNumber, WebSocketCommunicator Communicator)
{
    // TODO: make sure the exact strings are returned by the respective hardware devices
    private const string SyncerityOE_DeviceType = "HORIBA Scientific Syncerity";
    private const string SyncerityNIR_DeviceType = "SyncerityNIR";
    private const string SyncerityUVVis_DeviceType = "SyncerityUVVis";
    private const string SynapseCCD_DeviceType = "SynapseCCD";
    private const string Symphony2CCD_DeviceType = "Symphony2CCD";
    private const string SynapseIGA_DeviceType = "SunapseIGA";
    private const string Symphony2IGA_DeviceType = "Symphony2IGA";
    private const string SynapsePlus_DeviceType = "SynapsePlus";
    private const string SynapseEM_DeviceType = "SynapseEM";
    
    public string DeviceType { get; } = DeviceType;
    public string SerialNumber { get; } = SerialNumber;
    public int DeviceId { get; } = DeviceId;
    public WebSocketCommunicator Communicator { get; } = Communicator;
    public bool IsConnectionOpened { get; protected set; }
    public abstract Task<bool> IsConnectionOpenedAsync(CancellationToken cancellationToken = default);
    public abstract Task OpenConnectionAsync(CancellationToken cancellationToken = default);
    public abstract Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    public abstract Task WaitForDeviceNotBusy(int initialWaitInMs, int waitIntervalInMs, 
        CancellationToken cancellationToken = default);

    protected Gain GetDeviceSpecificGain(int gainToken)
    {
        return DeviceType switch 
        {
            // Syncerity devices
            SyncerityOE_DeviceType => (SyncerityOEGain)gainToken,
            SyncerityNIR_DeviceType => (SyncerityNIRGain)gainToken,
            SyncerityUVVis_DeviceType => (SyncerityUVVisGain)gainToken,
            
            // Synapse devices
            SynapseCCD_DeviceType => (SynapseCCDGain)gainToken,
            SynapseIGA_DeviceType => (SynapseIGAGain)gainToken,
            SynapsePlus_DeviceType => (SynapsePlusGain)gainToken,
            SynapseEM_DeviceType => (SynapseEMGain)gainToken,
            
            // Symphony2 devices
            Symphony2IGA_DeviceType => (Symphony2IGAGain)gainToken,
            Symphony2CCD_DeviceType => (Symphony2CCDGain)gainToken,
            _ => throw new ArgumentOutOfRangeException(nameof(DeviceType), DeviceType, null)
        };
    }
}