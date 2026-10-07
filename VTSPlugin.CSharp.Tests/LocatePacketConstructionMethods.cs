using System.Reflection;

namespace VTSPlugin.CSharp.Tests;

[TestClass]
public sealed class LocatePacketConstructionMethods
{
    [TestMethod]
    public void FindNonAnnotatedPackets()
    {
        List<Type> missing = [];

        foreach (var type in typeof(VTSPacketTemplate).Assembly.GetTypes()
            .Where(static t => t.IsSealed &&
            (t.IsAssignableFrom(typeof(VTSPacket)) || t.IsAssignableFrom(typeof(VTSPacketData)))))
        {
            if (!type.IsDefined(typeof(VTSJsonConstructibleWithAttribute)))
                missing.Add(type);
        }

        if (missing.Count > 0)
        {
            throw new Exception($"Following classes should have {nameof(VTSJsonConstructibleWithAttribute)}:\n{string.Join("\n", missing.Select(static t => t.Name))}");
        }
    }

    [TestMethod]
    public void LocateJsonConstructs()
    {
        List<Type> invalid = [];

        foreach (var type in typeof(VTSPacketTemplate).Assembly.GetTypes()
            .Where(static t => t.IsDefined(typeof(VTSJsonConstructibleWithAttribute), inherit: false)))
        {
            var attribute = type.GetCustomAttribute<VTSJsonConstructibleWithAttribute>(inherit: false)!;
            var method = type.GetMethod(attribute.MethodName, VTSJsonConstructibleWithAttribute.MethodBindingFlags);
            if (method is null) invalid.Add(type);
        }

        if (invalid.Count > 0)
        {
            throw new Exception($"Following classes are missing valid JsonConstruct method:\n{string.Join("\n", invalid.Select(static t => t.Name))}");
        }
    }

    [TestMethod]
    public void LocateRentMethods()
    {
        List<Type> invalid = [];
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

        foreach (var type in typeof(VTSPacketTemplate).Assembly.GetTypes()
            .Where(static t => t.IsSealed &&
            (t.IsAssignableFrom(typeof(VTSPacket)) || t.IsAssignableFrom(typeof(VTSPacketData)))))
        {
            var methods = type.GetMethods(flags).Where(static m => m.Name == "Rent").ToList();
            if (methods.Count != 2)
            {
                invalid.Add(type);
            }
        }

        if (invalid.Count > 0)
        {
            throw new Exception($"Following classes should have 2 Rent methods:\n{string.Join("\n", invalid.Select(static t => t.Name))}");
        }
    }
}