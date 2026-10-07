using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// Default inheritable implementation for <see cref="IVTSPacketSerializer"/>.
/// </summary>
/// <remarks>
/// Intended to be instantiated only once.
/// </remarks>
public abstract class AbstractVTSPacketSerializer : IVTSPacketSerializer, INotifyPropertyChanged
{
    /// <inheritdoc/>
    public bool EnablePrecompute { get; set; }

    /// <inheritdoc/>
    public VTSPrecomputeConfiguration PrecomputeConfiguration
    {
        get
        {
            lock (Lock) return field;
        }
        set
        {
            lock (Lock)
            {
                if (!value.Equals(field))
                {
                    field = value;
                    PrecomputeVersion++;
                }
            }
        }
    }

    /// <inheritdoc/>
    public IVTSLogger Logger
    {
        get => field;
        set => field = value ?? IVTSLogger.Default;
    } = IVTSLogger.Default;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Lock, used for accessing shared resources, such as <see cref="PrecomputeConfiguration"/>.
    /// </summary>
    protected readonly object Lock = new();
    /// <summary>
    /// Version at which all current precomputed values are registered.
    /// Miss-match on versions indicate that precomputed value is out-of-date, and it should be recomputed.
    /// </summary>
    private ulong PrecomputeVersion = 0;

    /// <summary>
    /// .ctor, constructing new <see cref="AbstractVTSPacketSerializer"/> with provided <paramref name="configuration"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="AbstractVTSPacketSerializer"/> should only be instantiated once, as it is uses generic cache,
    /// which is shared between child classes.
    /// </remarks>
    /// <param name="configuration"></param>
    protected AbstractVTSPacketSerializer(VTSPrecomputeConfiguration configuration)
    {
        PrecomputeConfiguration = configuration;
    }

    /// <summary>
    /// .ctor providing <see cref="VTSPrecomputeConfiguration.Default"/> as initial precompute settings.
    /// </summary>
    protected AbstractVTSPacketSerializer() : this(VTSPrecomputeConfiguration.Default) { }

    /// <inheritdoc/>
    public virtual string Serialize<T>(T instance, bool allowPrecompute = true) where T : VTSPacketTemplate
    {
        if (allowPrecompute && EnablePrecompute)
        {
            var descriptor = GetPrecomputeDescriptor<T>();
            if (descriptor.IsPrecomputable && instance is VTSPacket packet && MatchPrecompute(packet))
            {
                ulong version;
                lock (Lock) version = PrecomputeVersion;

                // Forces recompute if precompute settings versions have changed.
                string? prefix = null;
                string? suffix = null;
                if (descriptor.Version == version)
                {
                    prefix = GetPrecomputeRequestIDPrefix<T>();
                    suffix = GetPrecomputeRequestIDSuffix<T>();
                }
                else
                {
                    // Updates descriptor, so there will be no more updates unless the version changes again.
                    descriptor = descriptor.With(version);
                    SetDescriptor<T>(descriptor);
                }

                // Recomputes packet info if there is not prefix/postfix.
                string? id = packet.RequestID;
                if (prefix is null || suffix is null)
                {
                    try
                    {
                        packet.RequestID = string.Empty;
                        string raw = JsonSerializer.Serialize(instance, VTSAPI.JsonOptions);

                        // Tries to locate ("requestId":) JSON property.
                        // It will cache everything to the left and right of a target field.
                        const string FullJsonPropertyName = $"\"{VTSAPI.JsonPropertyNames.RequestID}\":";
                        int index = raw.IndexOf(FullJsonPropertyName);
                        if (index == -1)
                        {
                            if (Logger.ShouldLogWarn())
                                Logger.LogWarn($"Bug! Cannot precompute packet of a type {typeof(T)}! Falling back to regular serializer for this session.", ToString());
                            goto PreventPrecompute;
                        }

                        // Value of the RequestID, when set to an empty string, will serialize as ("").
                        // This function tries to locate a first part of it.
                        // However, pretty print can add a space between property name and its value, so we check two symbols instead of one.
                        int bracket = raw.IndexOf('"', index + FullJsonPropertyName.Length, 2);
                        if (bracket == -1)
                        {
                            if (Logger.ShouldLogWarn())
                                Logger.LogWarn($"Bug! Cannot locate (\"\") part in a precomputed JSON packet of type {typeof(T)}! Falling back to regular serializer for this session.", ToString());
                            goto PreventPrecompute;
                        }

                        prefix = raw[..(index + 1)];
                        suffix = raw[(index + 1)..];
                        SetPrecomputeRequestIDPrefix<T>(prefix);
                        SetPrecomputeRequestIDSuffix<T>(suffix);

                        // Small optimization - JSON packet used for precomputing has RequestID set to ("")
                        // If the original RequestID is also an empty string - we can reuse the packet.
                        // However, it does mean that we provide VTube Studio with ("") as RequestID - not null.
                        // It might lead to issues.
                        if (string.IsNullOrEmpty(id))
                            return raw;
                    }
                    catch (Exception ex)
                    {
                        if (Logger.ShouldLogException())
                        {
                            Logger.Log(ex, ToString());
                            if (Logger.ShouldLogWarn())
                                Logger.LogWarn($"Precompute for a type {typeof(T)} failed. Falling back to regular serializer for this session.");
                        }
                        goto PreventPrecompute;
                    }
                }

                // Uses prefixes to construct new value.
                return string.Concat(prefix, id, suffix);

                // Explicitly prevents all future attempts at precomputing.
                PreventPrecompute:
                packet.RequestID = id; // Restores proper ID, before normal serialization happens.
                SetDescriptor<T>(descriptor.With(isPrecomputable: false));
            }
        }

        return JsonSerializer.Serialize(instance, VTSAPI.JsonOptions);
    }

    /// <summary>
    /// Checks each instance individually, in an attempt to see if precompute should happen at all.
    /// Condition should describe instances with the same values as in <see cref="PrecomputeConfiguration"/>,
    /// because system will reuse cached values for instances, matching the specified settings.
    /// </summary>
    protected virtual bool MatchPrecompute(VTSPacket packet)
    {
        var c = PrecomputeConfiguration;
        return c.APIName == packet.APIName
            && c.APIVersion == packet.APIVersion;
    }

    /// <inheritdoc/>
    public virtual T Deserialize<T>(ReadOnlySpan<byte> utf8json) where T : VTSPacketTemplate
    {
        return JsonSerializer.Deserialize<T>(utf8json, VTSAPI.JsonOptions)!;
    }

    /// <summary>
    /// Retrieves descriptor for a provided <typeparamref name="T"/>.
    /// </summary>
    protected virtual PrecomputableDescriptor GetPrecomputeDescriptor<T>() where T : VTSPacketTemplate
    {
        return PrecomputeInfo<T>.Descriptor;
    }
    /// <summary>
    /// Sets descriptor for a provided <typeparamref name="T"/>.
    /// </summary>
    protected virtual void SetDescriptor<T>(PrecomputableDescriptor descriptor) where T : VTSPacketTemplate
    {
        PrecomputeInfo<T>.Descriptor = descriptor;
    }

    /// <summary>
    /// Retrieves RequestID prefix for a provided <typeparamref name="T"/>.
    /// Prefix is used in <see cref="string.Concat(string, string, string)"/> to construct precomputed <see cref="VTSRequest"/>s.
    /// </summary>
    protected virtual string? GetPrecomputeRequestIDPrefix<T>() where T : VTSPacketTemplate
    {
        return PrecomputeInfo<T>.RequestIDPrefix;
    }
    /// <summary>
    /// Sets RequestID prefix for a provided <typeparamref name="T"/>.
    /// Prefix is used in <see cref="string.Concat(string, string, string)"/> to construct precomputed <see cref="VTSRequest"/>s.
    /// </summary>
    protected virtual void SetPrecomputeRequestIDPrefix<T>(string? prefix) where T : VTSPacketTemplate
    {
        PrecomputeInfo<T>.RequestIDPrefix = prefix;
    }

    /// <summary>
    /// Retrieves descriptor for a provided <typeparamref name="T"/>.
    /// </summary>
    protected virtual string? GetPrecomputeRequestIDSuffix<T>() where T : VTSPacketTemplate
    {
        return PrecomputeInfo<T>.RequestIDSuffix;
    }
    /// <summary>
    /// Sets RequestID suffix for a provided <typeparamref name="T"/>.
    /// Prefix is used in <see cref="string.Concat(string, string, string)"/> to construct precomputed <see cref="VTSRequest"/>s.
    /// </summary>
    protected virtual void SetPrecomputeRequestIDSuffix<T>(string? suffix) where T : VTSPacketTemplate
    {
        PrecomputeInfo<T>.RequestIDSuffix = suffix;
    }

    /// <summary>
    /// Invokes <see cref="PropertyChanged"/>.
    /// </summary>
    protected void InvokePropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);

    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(AbstractVTSPacketSerializer)}]";

    /// <summary>
    /// Generic holder for precompute cache.
    /// </summary>
    /// <remarks>
    /// This class is very low-level!
    /// Do not access without full understanding how it is used.
    /// </remarks>
    /// TODO: Revisit concurrency. I think it doesn't support it.
    private static class PrecomputeInfo<T>
    {
        /// <summary>
        /// Descriptor, outlining whether <typeparamref name="T"/> can be precomputed.
        /// Additionally, specifies which version of <see cref="PrecomputeConfiguration"/> was used
        /// when creating current precompute cache.
        /// </summary>
        /// <remarks>
        /// This parameter is very low-level!
        /// Do not access without full understanding how it is used.
        /// </remarks>
        public static PrecomputableDescriptor Descriptor { get; set; }
        /// <summary>
        /// Prefix to use before <see cref="VTSPacket.RequestID"/> in <see cref="string.Concat(string, string, string)"/>.
        /// </summary>
        /// <remarks>
        /// This parameter is very low-level!
        /// Do not access without full understanding how it is used.
        /// </remarks>
        public static string? RequestIDPrefix { get; set; }
        /// <summary>
        /// Suffix to use after <see cref="VTSPacket.RequestID"/> in <see cref="string.Concat(string, string, string)"/>.
        /// </summary>
        /// <remarks>
        /// This parameter is very low-level!
        /// Do not access without full understanding how it is used.
        /// </remarks>
        public static string? RequestIDSuffix { get; set; }

        static PrecomputeInfo()
        {
            if (!typeof(T).IsDefined(typeof(VTSPrecomputableAttribute)))
            {
                Descriptor = default;
            }
            // Only VTSRequests are supported.
            else if (typeof(T).IsAssignableFrom(typeof(VTSRequest)))
            {
                Descriptor = new(0uL, isPrecomputable: true);
            }
            else
            {
                Descriptor = default;
            }
        }
    }

    /// <summary>
    /// Descriptor about classes, annotated with <see cref="VTSPrecomputableAttribute"/>s.
    /// Only <see cref="VTSRequest"/>s are supported.
    /// </summary>
    /// <remarks>
    /// Packed into one struct, so CPU can fetch the info in a single load operation.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    protected readonly struct PrecomputableDescriptor
    {
        /// <summary>
        /// Mask, covering all the bits, corresponding to the <see cref="Version"/>.
        /// </summary>
        public const ulong VersionMask = ~(0xFFuL << 56);
        /// <summary>
        /// Max number a <see cref="Version"/> can take.
        /// </summary>
        public const ulong MaxVersion = VersionMask;

        /// <summary>
        /// Actual version of the <see cref="PrecomputableDescriptor"/>.
        /// </summary>
        public ulong Version
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RawView & VersionMask;
        }

        /// <summary>
        /// Raw representation of this struct.
        /// </summary>
        [FieldOffset(0)]
        public readonly ulong RawView;
        /// <summary>
        /// Indicates whether the type can be precomputed.
        /// </summary>
        [FieldOffset(7)]
        public readonly bool IsPrecomputable;

        /// <summary>
        /// Constructs <see cref="PrecomputableDescriptor"/> from its raw <see langword="ulong"/> representation.
        /// </summary>
        /// <param name="raw">Raw representation of a struct.</param>
        public PrecomputableDescriptor(ulong raw) => RawView = raw;

        /// <summary>
        /// Constructs <see cref="PrecomputableDescriptor"/> from only a <paramref name="isPrecomputable"/> flag.
        /// <see cref="Version"/> will be set to <see langword="ulong"/>'s default value - <c>0uL</c>.
        /// </summary>
        /// <param name="isPrecomputable">Whether the type allows precomputing.</param>
        public PrecomputableDescriptor(bool isPrecomputable) => IsPrecomputable = isPrecomputable;

        /// <summary>
        /// Constructs <see cref="PrecomputableDescriptor"/> from a target <paramref name="version"/>
        /// and a <paramref name="isPrecomputable"/> flag.
        /// </summary>
        /// <param name="version">Version to use. If exceeds the max size (<see cref="MaxVersion"/>) - will overflow.</param>
        /// <param name="isPrecomputable">Whether the type allows precomputing.</param>
        public PrecomputableDescriptor(ulong version, bool isPrecomputable)
        {
            if (version > MaxVersion)
                version -= MaxVersion;

            // Note: Do NOT reorder! IsPrecomputable flag should be written last!
            RawView = version;
            IsPrecomputable = isPrecomputable;
        }

        /// <summary>
        /// Constructs new <see cref="PrecomputableDescriptor"/> with the same <see cref="IsPrecomputable"/>,
        /// but a provided <paramref name="version"/> instead.
        /// </summary>
        /// <param name="version">New <see cref="Version"/> to construct <see cref="PrecomputableDescriptor"/> with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PrecomputableDescriptor With(ulong version) => new(version, IsPrecomputable);

        /// <summary>
        /// Constructs new <see cref="PrecomputableDescriptor"/> with the same <see cref="Version"/>,
        /// but a provided <paramref name="isPrecomputable"/> instead.
        /// </summary>
        /// <param name="isPrecomputable">New <see cref="IsPrecomputable"/> to construct <see cref="PrecomputableDescriptor"/> with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PrecomputableDescriptor With(bool isPrecomputable) => new(Version, isPrecomputable);

        /// <summary>
        /// Adds <paramref name="version"/> to the <see cref="Version"/> from provided <paramref name="info"/>.
        /// </summary>
        /// <param name="info"><see cref="PrecomputableDescriptor"/> to modify.</param>
        /// <param name="version">Version to add.</param>
        /// <returns>Newly constructed <see cref="PrecomputableDescriptor"/> struct.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrecomputableDescriptor operator +(PrecomputableDescriptor info, ulong version)
        {
            return new(info.Version + version, info.IsPrecomputable);
        }

        /// <summary>
        /// Adds <paramref name="version"/> to the <see cref="Version"/> from provided <paramref name="info"/>.
        /// </summary>
        /// <param name="info"><see cref="PrecomputableDescriptor"/> to modify.</param>
        /// <param name="version">Version delta to add.</param>
        /// <returns>Newly constructed <see cref="PrecomputableDescriptor"/> struct.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrecomputableDescriptor operator +(PrecomputableDescriptor info, long version)
        {
            return new((ulong)((long)info.Version + version), info.IsPrecomputable);
        }

        /// <summary>
        /// Subtracts <paramref name="version"/> from the <see cref="Version"/> from provided <paramref name="info"/>.
        /// </summary>
        /// <param name="info"><see cref="PrecomputableDescriptor"/> to modify.</param>
        /// <param name="version">Version to subtract.</param>
        /// <returns>Newly constructed <see cref="PrecomputableDescriptor"/> struct.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrecomputableDescriptor operator -(PrecomputableDescriptor info, ulong version)
        {
            return new(info.Version - version, info.IsPrecomputable);
        }

        /// <summary>
        /// Subtracts <paramref name="version"/> from the <see cref="Version"/> from provided <paramref name="info"/>.
        /// </summary>
        /// <param name="info"><see cref="PrecomputableDescriptor"/> to modify.</param>
        /// <param name="version">Version delta to subtract.</param>
        /// <returns>Newly constructed <see cref="PrecomputableDescriptor"/> struct.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrecomputableDescriptor operator -(PrecomputableDescriptor info, long version)
        {
            return new((ulong)((long)info.Version - version), info.IsPrecomputable);
        }
    }
}
