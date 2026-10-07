using System.ComponentModel;

namespace VTSPlugin;

/// <summary>
/// Information about the plugin.
/// </summary>
public interface IVTSPlugin : INotifyPropertyChanged
{
    public const string DefaultName = "Unknown";
    public const string DefaultDeveloper = "Unknown";
    public const string DefaultBase64Icon = null;

    /// <summary>
    /// Internal identifier for a plugin.
    /// Never leaves local machine. Never used in networking.
    /// Instead, allows differentiating any one <see cref="IVTSPlugin"/> from any other <see cref="IVTSPlugin"/>.
    /// </summary>
    string ID { get; }
    /// <summary>
    /// Plugin name.
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Name { get; }
    /// <summary>
    /// Plugin author.
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Developer { get; }
    /// <summary>
    /// Plugin icon, encoded as Base64 (See also: <seealso cref="Convert.ToBase64String(byte[])"/>)
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Icon { get; }

    /// <summary>
    /// Sets <see cref="Name"/>, <see cref="Developer"/> and <see cref="Icon"/> for a plugin.
    /// </summary>
    /// <remarks>
    /// Make sure to use a <see langword="lock"/> when modifying <see cref="Name"/>, <see cref="Developer"/> and <see cref="Icon"/> parameters!
    /// </remarks>
    /// <param name="id">Internal ID of the plugin.</param>
    /// <param name="name">Name of the plugin. <see langword="null"/> can be replaced with <see cref="DefaultName"/>.</param>
    /// <param name="author">Plugin author. <see langword="null"/> can be replaced with <see cref="DefaultDeveloper"/>.</param>
    /// <param name="icon">
    /// Plugin icon, converted to base 64 (See also: <seealso cref="Convert.ToBase64String(byte[])"/>).
    /// <see langword="null"/> can be replaced with <see cref="DefaultBase64Icon"/>.
    /// </param>
    void Set(string? id = null, string? name = null, string? author = null, string? icon = null);
}
