namespace VTSPlugin;

/// <summary>
/// Settings for <see cref="IVTSService"/>.
/// </summary>
public readonly struct VTSServiceSettings()
{
    /// <summary>
    /// Preferences for which connections to expose and actively maintain.
    /// </summary>
    public enum ConnectionPreferences : byte
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

    /// <summary>
    /// Preference for handing new <see cref="IVTSConnection"/>s.
    /// </summary>
    public ConnectionPreferences ConnectionPreference { get; init; } = ConnectionPreferences.Active;
    /// <summary>
    /// Whether <see cref="IVTSService"/> should automatically connect to all new <see cref="IVTSEndPoint"/>s and authenticate a plugin there.
    /// </summary>
    public bool MaintainNewConnections { get; init; } = true;
}
