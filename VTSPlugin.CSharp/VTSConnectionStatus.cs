namespace VTSPlugin;

/// <summary>
/// Connection status of an <see cref="IVTSConnection"/>.
/// </summary>
/// Note: inherits <see langword="int"/> for usage in CLS-compliant <see cref="Interlocked"/> methods.
public enum VTSConnectionStatus : int // Must inherit int
{
    Idle,
    Connecting,
    // Authenticating?
    // Authenticated?
    Connected,
}
