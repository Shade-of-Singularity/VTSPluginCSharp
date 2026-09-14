using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Struct for holding request response.
/// Rarely used fields are left as raw list of bytes - <see cref="ReadOnlyMemory{T}"/> to improve performance.
/// </summary>
/// <remarks>
/// Important!
/// For max performance, use <see cref="RequestResult{T}"/> within a <see langword="using"/> block,
/// or call <see cref="Dispose"/> when <see cref="RequestResult{T}"/> is not used anymore.
/// </remarks>
public readonly struct RequestResult<T> : IDisposable
{
    /// <summary>
    /// Memory segment on which all <see cref="ReadOnlyMemory{T}"/> properties are referencing.
    /// </summary>
    /// <remarks>
    /// Might be null if a dummy <see cref="FromFailed(ErrorID, ReadOnlyMemory{byte})"/> is returned.
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
    [MemberNotNullWhen(true, nameof(Data))]
    public readonly required bool Successful { get; init; }
    /// <summary>
    /// When the result arrived.
    /// </summary>
    /// <remarks>
    /// If failed client-side - returns <see cref="DateTimeOffset.UtcNow"/> via <see cref="DateTimeOffset.ToUnixTimeMilliseconds"/>.
    /// </remarks>
    public readonly required long Timestamp { get; init; }
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
    public readonly required T? Data { get; init; }
    /// <summary>
    /// Shorthand for checking for <see cref="Successful"/>.
    /// </summary>
    /// <remarks>
    /// Provides <paramref name="data"/> if <see cref="Successful"/> is true.
    /// Otherwise - <see langword="default"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSuccessful([NotNullWhen(true)] out T? data)
    {
        if (!Successful)
        {
            data = default;
            return false;
        }

        data = Data;
        return true;
    }
    /// <summary>
    /// Disposes <see cref="Memory"/>, and so returns it to the pool.
    /// </summary>
    public void Dispose() => Memory?.Dispose();
    /// <inheritdoc cref="FromFailed(ErrorID, ReadOnlyMemory{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromFailed(ErrorID errorId) => FromFailed(errorId, ReadOnlyMemory<byte>.Empty);
    /// <summary>
    /// Creates a failed <see cref="RequestResult{T}"/> with a specified <paramref name="errorId"/>.
    /// </summary>
    /// <param name="errorId"><see cref="ErrorID"/> to include.</param>
    /// <param name="errorMessage">Optional <see cref="ErrorMessage"/> to include as well.</param>
    public static RequestResult<T> FromFailed(ErrorID errorId, ReadOnlyMemory<byte> errorMessage) => new()
    {
        Memory = null,
        Successful = false,
        ErrorID = errorId,
        ErrorMessage = errorMessage,
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        APIName = ReadOnlyMemory<byte>.Empty,
        APIVersion = ReadOnlyMemory<byte>.Empty,
        MessageType = ReadOnlyMemory<byte>.Empty,
        RequestID = ReadOnlyMemory<byte>.Empty,
        Data = default,
    };
    /// <inheritdoc cref="FromSuccessful(IMemoryOwner{byte}?, T, long, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromSuccessful(IMemoryOwner<byte>? memory, T data,
        ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
        ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default)
    {
        return FromSuccessful(memory, data, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), apiName, apiVersion, requestID, messageType);
    }
    /// <summary>
    /// Creates a successful <see cref="RequestResult{T}"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static RequestResult<T> FromSuccessful(IMemoryOwner<byte>? memory, T data, long timestamp,
        ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
        ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default)
    {
        // Should only trigger with structs.
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        return new()
        {
            Memory = memory,
            Successful = true,
            ErrorID = ErrorID.None,
            ErrorMessage = ReadOnlyMemory<byte>.Empty,
            Timestamp = timestamp,
            APIName = apiName,
            APIVersion = apiVersion,
            MessageType = messageType,
            RequestID = requestID,
            Data = data,
        };
    }
}

/// <summary>
/// Helper struct to reference actual methods from <see cref="RequestResult{T}"/>.
/// </summary>
/// <remarks>
/// Doesn't not have any functionality outside of that! Use <see cref="RequestResult{T}"/> instead!
/// </remarks>
/// Note: readonly ref struct with no fields, to make it clear that struct should not be used anywhere.
/// I.E.: we make it break in as many ways as possible when attempted to be actually used.
[method: Obsolete($"Type-less struct should not be used! Use {nameof(RequestResult)}<T> instead!", error: true)]
public readonly ref struct RequestResult()
{
    /// <inheritdoc cref="RequestResult{T}.FromFailed(ErrorID)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromFailed<T>(ErrorID errorId)
    {
        return RequestResult<T>.FromFailed(errorId);
    }

    /// <inheritdoc cref="RequestResult{T}.FromFailed(ErrorID, ReadOnlyMemory{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromFailed<T>(ErrorID errorId, ReadOnlyMemory<byte> errorMessage)
    {
        return RequestResult<T>.FromFailed(errorId, errorMessage);
    }

    /// <inheritdoc cref="RequestResult{T}.FromSuccessful(IMemoryOwner{byte}?, T, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromSuccessful<T>(IMemoryOwner<byte>? memory, T data,
        ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
        ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default)
    {
        return FromSuccessful(memory, data, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), apiName, apiVersion, requestID, messageType);
    }

    /// <inheritdoc cref="RequestResult{T}.FromSuccessful(IMemoryOwner{byte}?, T, long, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RequestResult<T> FromSuccessful<T>(IMemoryOwner<byte>? memory, T data, long timestamp,
        ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
        ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default)
    {
        // Should only trigger with structs.
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        return new()
        {
            Memory = memory,
            Successful = true,
            ErrorID = ErrorID.None,
            ErrorMessage = ReadOnlyMemory<byte>.Empty,
            Timestamp = timestamp,
            APIName = apiName,
            APIVersion = apiVersion,
            MessageType = messageType,
            RequestID = requestID,
            Data = data,
        };
    }
}
