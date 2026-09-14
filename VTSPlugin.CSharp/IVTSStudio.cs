using System.Collections.ObjectModel;

namespace VTSPlugin;

/// <summary>
/// An abstract maintaining connections with all newly found <see cref="IVTSEndPoint"/>s.
/// </summary>
public interface IVTSStudio
{
    /// <summary>
    /// Discovery service this <see cref="IVTSStudio"/> tracks.
    /// </summary>
    IVTSDiscoveryService DiscoveryService { get; }
    /// <summary>
    /// All active VTube Studio connections.
    /// </summary>
    ObservableCollection<IVTSConnection> Connections { get; }
}
