namespace VTSPlugin;

/// <summary>
/// Preference for which connection to maintain if a new one is found.
/// </summary>
public enum VTSConnectionPreference
{
    /// <summary>
    /// Recommended and default mode.
    /// Prefers to maintain connection with currently connected instance until it quits.
    /// </summary>
    /// <remarks>
    /// Normally, connection remains active until it times out, or if VTube Studio explicitly closes the connection.
    /// Changing to <see cref="Recent"/> can be beneficial, if your plugin will never (!) work on a device with multiple VTube Studios.
    /// </remarks>
    Active,
    /// <summary>
    /// Prefers to switch to the most recent VTube Studio instance, even if another active connection already exist.
    /// </summary>
    /// <remarks>
    /// Normally, connection remains active until it times out, or if VTube Studio explicitly closes the connection.
    /// Changing to <see cref="Recent"/> can be beneficial, if your plugin will never (!) work on a device with multiple VTube Studios.
    /// </remarks>
    Recent,
}
