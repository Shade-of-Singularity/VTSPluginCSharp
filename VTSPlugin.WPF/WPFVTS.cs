using CommunityToolkit.Mvvm.ComponentModel;
using Cysharp.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using VTSPlugin;

namespace VTSPlugin.WPF;

/// <summary>
/// Provides WPF-specific default initialization pipelines.
/// Those can be used to quickly start-up new projects.
/// </summary>
/// <remarks>
/// You can use those as an example to create your own initialization pipelines.
/// </remarks>
public sealed partial class WPFVTS : ObservableObject
{
    [MemberNotNullWhen(true, nameof(Logger))]
    [MemberNotNullWhen(true, nameof(Discovery))]
    [MemberNotNullWhen(true, nameof(Service))]
    [ObservableProperty] public partial bool IsInitialized { get; private set; }
    [ObservableProperty] public partial VTSLogger? Logger { get; private set; }
    [ObservableProperty] public partial VTSDiscoveryService? Discovery { get; private set; }
    [ObservableProperty] public partial VTSService? Service { get; private set; }

    readonly SemaphoreSlim Semaphore = new(0, 1);

    /// <inheritdoc/>
    public async UniTask Initialize()
    {
        await Semaphore.WaitAsync();
        try
        {
            if (IsInitialized)
            {
                Logger.TryLog("Already initialized!", this, VTSLoggingLevel.Warn);
                return;
            }
            else IsInitialized = true;

            Logger = new VTSLogger()
            {

            };
        }
        catch (Exception ex)
        {
            if (Logger is not null)
            {
                Logger.TryLog(ex, this);
                try { await Logger.Terminate(); }
                catch (Exception ex2) { IVTSLogger.Default.TryLog(ex2, this); }
            }
            else IVTSLogger.Default.TryLog(ex, this);
            (Logger ?? IVTSLogger.Default).TryLog(ex, this);
        }
        finally { Semaphore.Release(); }
    }

    /// <inheritdoc/>
    public async UniTask Terminate()
    {
        await Semaphore.WaitAsync();
        try
        {
            if (!IsInitialized)
            {
                IVTSLogger.Default.TryLog("Already Terminated!", this, VTSLoggingLevel.Warn);
                return;
            }
        }
        finally { Semaphore.Release(); }
        IsInitialized = false;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(WPFVTS)}]";
}

public sealed partial class VTSDiscoveryService(ushort port, IVTSLogger? logger = null) : ObservableObject, IVTSDiscoveryService
{
    [ObservableProperty] public partial ushort Port { get; private set; } = port == 0 ? VTSAPI.DefaultDiscoveryPort : port;
    [ObservableProperty] public partial TimeSpan EndPointTimeout { get; private set; }
    public ObservableCollection<IVTSEndPoint> EndPoints { get; private set; } = [];
    public IVTSLogger Logger { get; private set; } = logger ?? IVTSLogger.Default;
    public VTSDiscoveryService(IVTSLogger? logger = null) : this(VTSAPI.DefaultDiscoveryPort, logger) { }
}
