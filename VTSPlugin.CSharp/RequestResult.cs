using System.Buffers;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

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
	/// <inheritdoc cref="FromFailed{T}(ErrorID, ReadOnlyMemory{byte})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RequestResult<T> FromFailed<T>(ErrorID errorId) where T : VTSPacketData
	{
		return FromFailed<T>(errorId, ReadOnlyMemory<byte>.Empty);
	}

	/// <summary>
	/// Creates a failed <see cref="RequestResult{T}"/> with a specified <paramref name="errorId"/>.
	/// </summary>
	/// <param name="errorId"><see cref="ErrorID"/> to include.</param>
	/// <param name="errorMessage">Optional <see cref="RequestResult{T}.ErrorMessage"/> to include as well.</param>
	public static RequestResult<T> FromFailed<T>(ErrorID errorId, ReadOnlyMemory<byte> errorMessage) where T : VTSPacketData => new()
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
	/// <inheritdoc cref="FromSuccessful{T}(IMemoryOwner{byte}?, T, long, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RequestResult<T> FromSuccessful<T>(IMemoryOwner<byte>? memory, T data,
		ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
		ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default) where T : VTSPacketData
	{
		return FromSuccessful(memory, data, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), apiName, apiVersion, requestID, messageType);
	}
	/// <summary>
	/// Creates a successful <see cref="RequestResult{T}"/>.
	/// </summary>
	public static RequestResult<T> FromSuccessful<T>(IMemoryOwner<byte>? memory, T data, long timestamp,
		ReadOnlyMemory<byte> apiName = default, ReadOnlyMemory<byte> apiVersion = default,
		ReadOnlyMemory<byte> requestID = default, ReadOnlyMemory<byte> messageType = default) where T : VTSPacketData
	{
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
