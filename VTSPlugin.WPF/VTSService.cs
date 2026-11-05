using CommunityToolkit.Mvvm.ComponentModel;
using Cysharp.Threading.Tasks;
using System.ComponentModel;

namespace VTSPlugin.WPF;

/// <inheritdoc cref="IVTSService"/>
public sealed partial class VTSService : ObservableObject, IVTSService
{
    /// <inheritdoc/>
    [ObservableProperty] public partial IVTSPlugin? Plugin { get; private set; }

    /// <inheritdoc/>
    [ObservableProperty] public partial IVTSTokenStorage? TokenStorage { get; private set; }

    /// <inheritdoc/>
    [ObservableProperty] public partial IVTSDiscoveryService? DiscoveryService { get; private set; }

    /// <inheritdoc/>
    [ObservableProperty] public partial IVTSServiceInitializationOptions? Options { get; private set; }

    /// <inheritdoc/>
    [ObservableProperty] public partial IVTSLogger Logger { get; set; } = IVTSLogger.Default;

    /// <summary>
    /// Connection this <see cref="VTSService"/> manages.
    /// </summary>
    [ObservableProperty] public partial IVTSConnection? Connection { get; private set; }

    /// <inheritdoc/>
    public bool IsInitialized { get; private set; }

    readonly SemaphoreSlim Semaphore = new(0, 1);

    /// <inheritdoc/>
    public UniTask<bool> Initialize() => UniTask.FromResult(false);

    /// <inheritdoc/>
    public async UniTask<bool> Initialize(IVTSServiceInitializationOptions options)
    {
        await Semaphore.WaitAsync();
        try
        {
            if (IsInitialized) return true;
            else
            {
                await TerminateCore();
                await InitializeCore(options);
                Options = options;
            }
        }
        finally { Semaphore.Release(); }
    }

    async UniTask<bool> InitializeCore(IVTSServiceInitializationOptions options)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Logger.Log(ex, ToString());
        }
    }

    /// <inheritdoc/>
    public UniTask<RequestResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData
    {
        var connection = Connection;
        if (connection is null)
            return UniTask.FromResult(RequestResult.FromFailed<T>(ErrorID.VTubeStudioOffline));
        return connection.Request<T>(request, timeout);
    }

    /// <inheritdoc/>
    public async UniTask<bool> Terminate()
    {
        await Semaphore.WaitAsync();
        try
        {
            if (IsInitialized)
                return await TerminateCore();
            else
                return true;
        }
        finally { Semaphore.Release(); }
    }

    UniTask<bool> TerminateCore()
    {
        try
        {

        }
        catch (Exception ex)
        {
            Logger.Log(ex, );
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(VTSService)}]";

    internal static class KnownPropertyChangedArgs
    {
        public static readonly PropertyChangedEventArgs IsInitialized = new(nameof(VTSService.IsInitialized));
    }
}
