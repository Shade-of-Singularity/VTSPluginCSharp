using System.Collections.ObjectModel;

namespace VTSPlugin;

/// <summary>
/// Discovers new <see cref="IVTSEndPoint"/>s.
/// </summary>
public interface IVTSDiscoveryService
{
    /// <summary>
    /// Port on which this <see cref="IVTSDiscoveryService"/> listens on.
    /// </summary>
    ushort Port { get; }
    /// <summary>
    /// Time until a <see cref="IVTSEndPoint"/> should be marked as timed-out.
    /// </summary>
    TimeSpan EndPointTimeout { get; }
    /// <summary>
    /// All currently observed VTube Studio instances in the local network.
    /// </summary>
    ObservableCollection<IVTSEndPoint> EndPoints { get; }
}
