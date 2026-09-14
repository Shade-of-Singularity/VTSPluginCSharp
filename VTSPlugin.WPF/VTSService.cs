using CommunityToolkit.Mvvm.ComponentModel;

namespace VTSPlugin.WPF;

/// <inheritdoc cref="IVTSService"/>
public sealed partial class VTSService : ObservableObject, IVTSService
{
    public required IVTSPlugin Plugin { get; init; }
    public required VTSServiceSettings Settings { get; init; }
    [ObservableProperty] public partial IVTSConnection? Connection { get; private set; }
}
