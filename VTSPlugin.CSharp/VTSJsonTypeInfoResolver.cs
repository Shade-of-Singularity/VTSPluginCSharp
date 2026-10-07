using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VTSPlugin;

namespace VTSPlugin;

/// <summary>
/// Implementation extending <see cref="DefaultJsonTypeInfoResolver"/>,
/// to skip any fields marked with <see langword="required"/> keyword.
/// </summary>
/// <remarks>
/// Provided, as for manual packet construction, the <see langword="required"/> keyword is quite useful.
/// But it should be skipped during deserialization, because it throws <see cref="JsonException"/> if VTS returns an API error message.
/// </remarks>
public sealed class VTSJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    /// <summary>
    /// Static instance to use in all serializers.
    /// </summary>
    public static readonly VTSJsonTypeInfoResolver Instance = new();

    /// <inheritdoc/>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo info = base.GetTypeInfo(type, options);
        foreach (JsonPropertyInfo property in info.Properties)
        {
            // Skip only if there is no JsonRequiredAttribute defined.
            if (!property.PropertyType.IsDefined(typeof(JsonRequiredAttribute), false))
                property.IsRequired = false;
        }
        if (type.IsDefined(typeof(VTSJsonConstructibleWithAttribute), false))
        {
            try
            {
                var attribute = type.GetCustomAttribute<VTSJsonConstructibleWithAttribute>(inherit: false);
                var method = type.GetMethod(attribute.MethodName, VTSJsonConstructibleWithAttribute.MethodBindingFlags);
                if (method.ReturnType != typeof(object))
                {
                    if (IVTSLogger.Default.ShouldLogError())
                        IVTSLogger.Default.LogError($"JSON constructor method '{method.Name}' should have 'object' as its return value!");
                }
                else if (method.ContainsGenericParameters)
                {
                    if (IVTSLogger.Default.ShouldLogError())
                        IVTSLogger.Default.LogError($"JSON constructor method '{method.Name}' cannot have generic parameters!");
                }
                else if (method.GetParameters().Length != 0)
                {
                    if (IVTSLogger.Default.ShouldLogError())
                        IVTSLogger.Default.LogError($"JSON constructor method '{method.Name}' should not have any parameters!");
                }
                else
                {
                    info.CreateObject = (Func<object>)method.CreateDelegate(typeof(Func<object>));
                }
            }
            catch (Exception ex)
            {
                if (IVTSLogger.Default.ShouldLogException())
                    IVTSLogger.Default.Log(ex, $"[{nameof(VTSJsonTypeInfoResolver)}]");
            }
        }
        return info;
    }
}
