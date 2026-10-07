using CommunityToolkit.Mvvm.ComponentModel;
using Cysharp.Threading.Tasks;

namespace VTSPlugin.WPF;

/// <inheritdoc cref="IVTSService"/>
public sealed partial class VTSService : ObservableObject, IVTSService
{
    public required IVTSPlugin Plugin { get; init; }
    public required VTSServiceSettings Settings { get; init; }
    public required IVTSDiscoveryService DiscoveryService { get; init; }
    [ObservableProperty] public partial IVTSConnection? Connection { get; private set; }
    public UniTask<RequestResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData
    {
        throw new NotImplementedException();
    }
}
