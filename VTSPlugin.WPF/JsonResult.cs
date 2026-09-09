using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin.WPF;

/// <summary>
/// Struct for holding results of a request in a lazily initialized form.
/// </summary>
/// TODO: Move to VTSPlugin namespace, to use in all other libraries.
/// Note: For standardization, we can instead create VTSPlugin.CSharp (root namespace: VTSPlugin), and for Godot: VTSPlugin.GDScript.
/// TODO: Add disposal methods for disposing arrays used for <see cref="ReadOnlyMemory{T}"/> properties.
public readonly struct JsonResult<T> : IDisposable
{
    /// <summary>
    /// Deserializer for deserializing <paramref name="data"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <param name="data">Data to deserialize.</param>
    /// <param name="instance">Deserialized instance of <typeparamref name="T"/>.</param>
    /// <returns>
    /// <see langword="true"/> if deserialized successfully, and <paramref name="data"/> was provided.
    /// <see langword="false"/> if deserialization failed without exception. Useful when deserializing structs.
    /// </returns>
    public delegate bool DeserializationHandler(ReadOnlyMemory<byte> data, [NotNullWhen(true)] out T instance);
    /// <summary>
    /// Handler controlling how any particular instance of <typeparamref name="T"/> should be disposed when not used anymore.
    /// </summary>
    /// <param name="instance">Instance of deserialized data, previously returned by <see cref="DeserializationHandler"/>.</param>
    public delegate void DisposeHandler(T instance);
    /// <summary>
    /// Memory segment on which all <see cref="ReadOnlyMemory{T}"/> properties are referencing.
    /// </summary>
    /// <remarks>
    /// Might be null if a dummy <see cref="FromFailed(ErrorID, ReadOnlyMemory{byte}, IVTSLogger?)"/> is returned.
    /// </remarks>
    public readonly required IMemoryOwner<byte>? Memory { get; init; }
    /// <summary>
    /// Whether result of the request was successful.
    /// </summary>
    /// <remarks>
    /// Set to <see langword="false"/> when client-side error occurred (like request timeout), or if received "APIError" message type.
    /// <para>
    /// <see cref="ErrorID"/> and <see cref="ErrorMessage"/> has anything important only if this value is set to <see langword="false"/>.
    /// </para>
    /// </remarks>
    public readonly required bool Successful { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.ErrorID"/> value parsed from the message, if <see cref="Successful"/> is <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="ErrorID.None"/> if <see cref="Successful"/> is <see langword="true"/>.
    /// </remarks>
    public readonly required ErrorID ErrorID { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.Message"/> shipped together with <see cref="VTSAPI.JsonPropertyNames.ErrorID"/> as UTF8 byte segment.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="ReadOnlyMemory{T}.Empty"/> when there is no message, or if <see cref="Successful"/> is <see langword="true"/>.
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> ErrorMessage { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.APIName"/> raw value as UTF8 byte segment.
    /// </summary>
    /// <remarks>
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> APIName { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.APIVersion"/> raw value as UTF8 byte segment.
    /// </summary>
    /// <remarks>
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> APIVersion { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.RequestID"/> raw value as UTF8 byte segment.
    /// </summary>
    /// <remarks>
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> RequestID { get; init; }
    /// <summary>
    /// <see cref="VTSAPI.JsonPropertyNames.MessageType"/> raw value as UTF8 byte segment.
    /// </summary>
    /// <remarks>
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> MessageType { get; init; }
    /// <summary>
    /// Data entry of the request response.
    /// Set to <see cref="ReadOnlyMemory{T}.Empty"/> if there is no data.
    /// </summary>
    /// <remarks>
    /// Can be parsed using methods from <see cref="VTSAPI.Encoding"/>.
    /// And can be read using any Json parser your heart desire.
    /// </remarks>
    public readonly required ReadOnlyMemory<byte> Data { get; init; }
    /// <summary>
    /// Logger to use on deserialization exception.
    /// </summary>
    /// <remarks>
    /// Same as the one used in 
    /// </remarks>
    public readonly required IVTSLogger Logger { get; init; }
    /// <summary>
    /// Deserializer method for deserializing <see cref="Data"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// In most cases, deserializers are cached per-type, so no allocations happen when assigning those.
    /// (This might not be the case with custom functions that return <see cref="JsonResult{T}"/>, i.e. community extensions, etc.)
    /// </remarks>
    public readonly required DeserializationHandler? Deserializer { get; init; }
    /// <summary>
    /// Dispose method for releasing data, held by the instance of <typeparamref name="T"/>, created by <see cref="Deserializer"/>.
    /// </summary>
    /// <remarks>
    /// Only used if you utilize <see cref="UnpackScoped"/> or <see cref="TryUnpackScoped"/> methods.
    /// </remarks>
    public readonly required DisposeHandler? Disposer { get; init; }
    /// <summary>
    /// If <see cref="Successful"/> is <see langword="true"/> - tries to parse <see cref="Data"/> to type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Unpack(out T?)"/>, any exceptions during deserialization are simply logged. No need to catch them on your own.
    /// </remarks>
    /// <param name="data">Instance of <typeparamref name="T"/>, deserialized from <see cref="Data"/>, or <see langword="default"/></param>
    /// <returns>
    /// <see langword="true"/> - if data was deserialized successfully!
    /// <see langword="false"/> - if <see cref="Successful"/> is <see langword="false"/>, or deserialization failed (see logger output).
    /// </returns>
    public bool TryUnpack([NotNullWhen(true)] out T? data)
    {
        if (!Successful || Deserializer is null)
        {
            data = default;
            return false;
        }

        try
        {
            if (Deserializer(Data, out T instance))
            {
                data = instance;
                return true;
            }
        }
        catch (Exception ex) { Logger.Log(ex); }

        data = default;
        return false;
    }
    /// <summary>
    /// If <see cref="Successful"/> is <see langword="true"/> - tries to parse <see cref="Data"/> to type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="TryUnpack(out T)"/>, deserialization exceptions will not be catched. You will need to catch them yourself.
    /// </remarks>
    /// <param name="data">Instance of <typeparamref name="T"/>, deserialized from <see cref="Data"/>, or <see langword="default"/></param>
    /// <returns>
    /// <see langword="true"/> - if data was deserialized successfully!
    /// <see langword="false"/> - if <see cref="Successful"/> is <see langword="false"/>.
    /// </returns>
    public bool Unpack([NotNullWhen(true)] out T? data)
    {
        if (!Successful || Deserializer is null)
        {
            data = default;
            return false;
        }

        if (Deserializer(Data, out T instance))
        {
            data = instance;
            return true;
        }

        data = default;
        return false;
    }
    /// <summary>
    /// Scope struct with deserialized data to use inside of <see langword="using"/> block.
    /// Any allocated resources will be released when <see langword="using"/> block quits.
    /// </summary>
    /// <remarks>
    /// Disposes provided <typeparamref name="T"/> instance using provided <see cref="DisposeHandler"/>
    /// when code quits <see langword="using"/> block.
    /// </remarks>
    public struct Scope : IDisposable
    {
        [Flags]
        enum ScopeStateFlags : byte
        {
            None,
            Successful = 0b001,
            Consumed = 0b010,
            HasDisposer = 0b100,
        }

        public readonly T? Data { get; }
        public readonly ErrorID ErrorID { get; }
        public readonly ReadOnlyMemory<byte> ErrorMessage { get; }
        public readonly DisposeHandler? Disposer { get; }
        [MemberNotNullWhen(true, nameof(Data))]
        public readonly bool Successful => (flags & ScopeStateFlags.Successful) != default;
        [MemberNotNullWhen(true, nameof(Data))]
        [MemberNotNullWhen(true, nameof(Disposer))]
        readonly bool ShouldDispose
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => flags == (ScopeStateFlags.Successful | ScopeStateFlags.HasDisposer);
        }

        ScopeStateFlags flags;
        private Scope(bool successful, T? data, DisposeHandler? disposer, ErrorID errorID, ReadOnlyMemory<byte> errorMessage)
        {
            Data = data;
            Disposer = disposer;
            ErrorID = errorID;
            ErrorMessage = errorMessage;
            ScopeStateFlags a = disposer is not null ? ScopeStateFlags.HasDisposer : default;
            ScopeStateFlags b = successful ? ScopeStateFlags.Successful : default;
            flags = a | b;
        }

        /// <summary>
        /// Consumes instance, preventing it from being disposed.
        /// </summary>
        public void Consume() => flags |= ScopeStateFlags.Consumed;
        /// <summary>
        /// Resets action, previously done by <see cref="Consume"/>, allowing <see cref="Dispose"/> to dispose the instance again.
        /// </summary>
        /// <remarks>
        /// <b>Using <see cref="Consume"/> and <see cref="Refresh"/> requires advanced understanding of <see cref="VTSPlugin"/> library.</b>
        /// Please just use <see langword="using"/> block with no extra steps if possible, as it is easier to reason about and less verbose.
        /// </remarks>
        public void Refresh() => flags &= ~ScopeStateFlags.Consumed;
        /// <summary>
        /// Disposes instance of <typeparamref name="T"/>, generated by a <see cref="Deserializer"/>.
        /// </summary>
        public readonly void Dispose()
        {
            if (ShouldDispose)
                Disposer(Data);
        }

        /// <summary>
        /// Creates a successfully deserialized scope.
        /// </summary>
        /// <param name="data">Instance to dispose when code quits <see langword="using"/> block.</param>
        /// <param name="disposer">Disposal method to use for disposing <paramref name="data"/> when code quits <see langword="using"/> block.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Scope FromSuccessful(T data, DisposeHandler? disposer) => new(true, data, disposer, ErrorID.None, ReadOnlyMemory<byte>.Empty);
        /// <summary>
        /// Creates a failed <see cref="Scope"/> containing no <see cref="Data"/>.
        /// </summary>
        /// <remarks>
        /// Calling <see cref="Refresh"/> on this scope will lead to undefined behaviour! (likely <see cref="NullReferenceException"/>s)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Scope FromFailed(ErrorID errorID, ReadOnlyMemory<byte> errorMessage) => new(false, default, default, errorID, errorMessage);
    }
    /// <summary>
    /// If returns <see cref="Scope.Successful"/> with <see langword="true"/> - tries to parse <see cref="Data"/> to type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// Unlike <see cref="UnpackScoped"/>, any exceptions during deserialization are simply logged. No need to catch them on your own.
    /// </returns>
    /// <remarks>
    /// <see langword="true"/> - if data was deserialized successfully!
    /// <see langword="false"/> - if <see cref="Successful"/> is <see langword="false"/>, or deserialization failed (see logger output).
    /// </remarks>
    public Scope TryUnpackScoped()
    {
        if (!Successful)
        {
            return Scope.FromFailed(ErrorID, ErrorMessage);
        }

        if (Deserializer is null)
        {
            return Scope.FromFailed(ErrorID.DeserializationFailed, JsonResultErrorMessages.MissingDeserializer.Memory);
        }

        try
        {
            if (Deserializer(Data, out var data))
            {
                return Scope.FromSuccessful(data, Disposer);
            }
            else
            {
                return Scope.FromFailed(ErrorID.DeserializationFailed, JsonResultErrorMessages.DeserializationFailed.Memory);
            }
        }
        catch (Exception ex) { Logger.Log(ex); }

        return Scope.FromFailed(ErrorID.DeserializationException, JsonResultErrorMessages.DeserializationException.Memory);
    }
    /// <summary>
    /// If returns <see cref="Scope.Successful"/> with <see langword="true"/> - tries to parse <see cref="Data"/> to type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// Unlike <see cref="TryUnpackScoped"/>, deserialization exceptions will not be catched. You will need to catch them yourself.
    /// </returns>
    public Scope UnpackScoped()
    {
        if (Successful)
        {
            if (Deserializer is null)
            {
                return Scope.FromFailed(ErrorID.DeserializationFailed, JsonResultErrorMessages.MissingDeserializer.Memory);
            }

            if (Deserializer(Data, out T data))
            {
                return Scope.FromSuccessful(data, Disposer);
            }
            else
            {
                return Scope.FromFailed(ErrorID.DeserializationFailed, JsonResultErrorMessages.DeserializationFailed.Memory);
            }
        }

        return Scope.FromFailed(ErrorID, ErrorMessage);
    }
    /// <summary>
    /// Disposes <see cref="Memory"/>, and so returns it to the pool.
    /// </summary>
    public void Dispose() => Memory?.Dispose();
    /// <inheritdoc cref="FromFailed(ErrorID, ReadOnlyMemory{byte}, IVTSLogger?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonResult<T> FromFailed(ErrorID errorId, IVTSLogger? logger = null) => FromFailed(errorId, ReadOnlyMemory<byte>.Empty, logger);
    /// <summary>
    /// Creates a failed <see cref="JsonResult{T}"/> with a specified <paramref name="errorId"/>.
    /// </summary>
    /// <param name="errorId"><see cref="ErrorID"/> to include.</param>
    /// <param name="errorMessage">Optional <see cref="ErrorMessage"/> to include as well.</param>
    /// <param name="logger">Logger to use in cases when user uses this failed result in a wrong way.</param>
    public static JsonResult<T> FromFailed(ErrorID errorId, ReadOnlyMemory<byte> errorMessage, IVTSLogger? logger = null) => new()
    { 
        Memory = null,
        Successful = false,
        ErrorID = errorId,
        ErrorMessage = errorMessage,
        APIName = ReadOnlyMemory<byte>.Empty,
        APIVersion = ReadOnlyMemory<byte>.Empty,
        MessageType = ReadOnlyMemory<byte>.Empty,
        RequestID = ReadOnlyMemory<byte>.Empty,
        Data = ReadOnlyMemory<byte>.Empty,
        Deserializer = null,
        Disposer = null,
        Logger = logger ?? IVTSLogger.Default,
    };
}
