using System.Reflection;
using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// Annotates that a specific <see cref="VTSRequest"/> class has method, used for constructing the class,
/// when deserializing using <see cref="JsonSerializer"/> with our <see cref="VTSAPI.JsonOptions"/>.
/// </summary>
/// <remarks>
/// Method, described with <paramref name="methodName"/> should be argument-less,
/// <see langword="static"/>, and return <see cref="object"/>.
/// </remarks>
/// <param name="methodName"> Method that construct the instance.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class VTSJsonConstructibleWithAttribute(string methodName) : Attribute
{
    /// <summary>
    /// Binding flags used when locating the <see cref="MethodName"/>.
    /// </summary>
    public const BindingFlags MethodBindingFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
    /// <summary>
    /// Name of the method, that constructs the instance.
    /// </summary>
    /// <remarks>
    /// Target method should be argument-less, <see langword="static"/>, and return <see cref="object"/>.
    /// Can be <see langword="private"/> to not 
    /// </remarks>
    public string MethodName { get; } = methodName;
}