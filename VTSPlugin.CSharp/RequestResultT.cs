using Cysharp.Threading.Tasks;
using StackOnlyJsonParser;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks.Sources;
using VTSPlugin.Pooling;
using VTSPlugin.Requests;

namespace VTSPlugin;

public sealed class TestRequest : VTSRequest<TestRequestData>;
public sealed class TestRequestData : VTSRequestData;

[StackOnlyJsonType]
public readonly ref partial struct TestStackData : IEquatable<TestStackData>
{
	readonly byte raw;
	public override string ToString() => nameof(TestStackData);
	public override int GetHashCode() => 0;
	public override bool Equals([NotNullWhen(true)] object? obj) => false;
	public bool Equals(TestStackData other) => raw == other.raw;
	public static bool operator ==(TestStackData a, TestStackData b) => a.raw == b.raw;
	public static bool operator !=(TestStackData a, TestStackData b) => a.raw != b.raw;
}

// P.S.: we will definitely need some tests to ensure that library is reliable.
public static class HandlerUsage
{
	/// TODO: Missing a way to identify delegate-only events.
	/// Note: This might be a fundamental limitation of delegates. TState version was provided to resolve this.
	/// Remark: However, at least a timecode of the request would be nice to have on the caller...? If it worth it.
	/// P.S.: Everything here needs additional benchmarking.
	public static async UniTaskVoid CollectiveRequests()
	{
		// Normal delegate-based request.
		APIRequest(static (data) =>
		{
			IVTSLogger.Default.LogInfo($"{data}");
			return true;
		}, static (id, message) => IVTSLogger.Default.LogError($"{id}: {message}"));

		// Stateful delegate-based request.
		APIRequest(state: 42, static (state, data) =>
		{
			IVTSLogger.Default.LogInfo($"{data} ({state})");
			return true;
		}, static (state, id, message) => IVTSLogger.Default.LogError($"{id}: {message}"));

		// Async-based request.
		// Note: We can add a way to turn regular data holder in a local struct.
		// This will allow us to unpack and immediately release the data holder.
		// However, this is a very unreliable optimization. Best case scenario will
		
		// Note: if you know what you are doing, you can omit the `using` block.
		// If it fails - it will only let GC to eventually collect non-disposed data instance.
		// Program itself will run safely, just sub-optimally for a while.
		using var result = await APIRequestAsync();
		if (result.IsSuccessful(out var data))
		{
			IVTSLogger.Default.LogInfo($"{data}");
		}
		else
		{
			IVTSLogger.Default.LogError($"{result.ErrorID}: {result.ErrorMessage}");
		}

		// Stack-only delegate-based request.
		// Note: we might introduce API to turn use stack-only to fill-in class-based data holders.
		// This will allow storing request results long-term, for better or for worse, or move results around in async context.
		StackAPIRequest(static (ref reader) =>
		{
			TestStackData data = new(ref reader);
			IVTSLogger.Default.LogInfo($"{data.ToString()}");
		}, static (id, ref message) => IVTSLogger.Default.LogError($"{id}: {message.GetString()}"));

		// Stateful stack-only delegate-based request.
		StackAPIRequest(state: 24, static (state, ref reader) =>
		{
			TestStackData data = new(ref reader);
			IVTSLogger.Default.LogInfo($"{data.ToString()} ({state})");
		}, static (state, id, ref message) => IVTSLogger.Default.LogError($"{id}: {message.GetString()}"));
	}

	// 5 basically overloads per request...
	// This is insane.
	// I really hope I can find a way to automate this with Source Generators.
	public static void APIRequest(VTSSuccessResultEventHandler<VTSAPIStateResponseData>? onSuccess = null, VTSErrorResultEventHandler? onError = null)
	{
		var handler = VTSPool.Shared.Rent<VTSResultHandler<VTSAPIStateResponseData>>();
		handler.OnSuccess = onSuccess;
		handler.OnError = onError;
		// Send request somewhere here.
	}

	public static void APIRequest<TState>(TState? state,
		VTSStatefulSuccessResultEventHandler<TState, VTSAPIStateResponseData>? onSuccess = null,
		VTSStatefulErrorResultEventHandler<TState>? onError = null)
	{
		var handler = VTSPool.Shared.Rent<VTSStatefulResultHandler<TState, VTSAPIStateResponseData>>();
		handler.State = state;
		handler.OnSuccess = onSuccess;
		handler.OnError = onError;
		// Send request somewhere here.
	}

	public static UniTask<NewResult<VTSAPIStateResponseData>> APIRequestAsync()
	{
		var hander = VTSUniTaskAutoResetResultHandler<VTSAPIStateResponseData>.Create();
		// Send request somewhere here.
		return hander.Task;
	}

	public static void StackAPIRequest(VTSStackReaderSuccessResultEventHandler? onSuccess = null, VTSStackErrorResultEventHandler? onError = null)
	{
		var handler = VTSPool.Shared.Rent<VTSStackReaderResultHandler>();
		handler.OnSuccess = onSuccess;
		handler.OnError = onError;
		// Send request somewhere here.
	}

	public static void StackAPIRequest<TState>(TState? state,
		VTSStatefulStackReaderSuccessResultEventHandler<TState>? onSuccess = null,
		VTSStatefulStackErrorResultEventHandler<TState>? onError = null)
	{
		var handler = VTSPool.Shared.Rent<VTSStatefulStackReaderResultHandler<TState>>();
		handler.State = state;
		handler.OnSuccess = onSuccess;
		handler.OnError = onError;
		// Send request somewhere here.
	}

	public static void DelegateRequests()
	{
		{
			var handler = VTSPool.Shared.Rent<VTSResultHandler>();
			handler.OnSuccess = static () => IVTSLogger.Default.LogInfo($"Success!");
			handler.OnError = static (id, message) => IVTSLogger.Default.LogError($"{id}: {message}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSResultHandler<TestRequestData>>();
			handler.OnSuccess = static (data) =>
			{
				IVTSLogger.Default.LogInfo($"Success! {data}");
				return true;
			};
			handler.OnError = static (id, message) => IVTSLogger.Default.LogError($"{id}: {message}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSStatefulResultHandler<int>>();
			handler.State = 42;
			handler.OnSuccess = static (state) => IVTSLogger.Default.LogInfo($"Success ({state})!");
			handler.OnError = static (state, id, message) => IVTSLogger.Default.LogInfo($"({state}) {id}: {message}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSStatefulResultHandler<int, TestRequestData>>();
			handler.State = 42;
			handler.OnSuccess = static (state, data) =>
			{
				IVTSLogger.Default.LogInfo($"Success ({state})! {data}");
				return true;
			};
			handler.OnError = static (state, id, message) => IVTSLogger.Default.LogInfo($"({state}) {id}: {message}");
			VTSPool.Shared.Return(handler);
		}
	}

	public static async UniTaskVoid UniTaskRequests()
	{
		{
			var handler = VTSUniTaskAutoResetResultHandler.Create();
			var result = await handler.Task;
			if (result.IsSuccessful())
			{
				IVTSLogger.Default.LogInfo($"Success!");
			}
			else
			{
				IVTSLogger.Default.LogError($"{result.ErrorID}: {result.ErrorMessage}");
			}
		}
		{
			var handler = VTSUniTaskAutoResetResultHandler<TestRequestData>.Create();
			var result = await handler.Task;
			if (result.IsSuccessful(out var data))
			{
				IVTSLogger.Default.LogInfo($"Success! {data}");
			}
			else
			{
				IVTSLogger.Default.LogError($"{result.ErrorID}: {result.ErrorMessage}");
			}
			result.Dispose();
		}
	}

	public static void StackOnlyRequests()
	{
		{
			var handler = VTSPool.Shared.Rent<VTSStackResultHandler>();
			handler.OnSuccess = static () => IVTSLogger.Default.LogInfo($"Success!");
			handler.OnError = static (id, ref message) => IVTSLogger.Default.LogError($"{id}: {message.GetString()}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSStackReaderResultHandler>();
			handler.OnSuccess = static (ref reader) =>
			{
				TestStackData data = new(ref reader);
				IVTSLogger.Default.LogInfo($"Success! {data.ToString()}");
			};
			handler.OnError = static (id, ref message) => IVTSLogger.Default.LogError($"{id}: {message.GetString()}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSStatefulStackResultHandler<int>>();
			handler.State = 42;
			handler.OnSuccess = static (state) => IVTSLogger.Default.LogInfo($"Success ({state})!");
			handler.OnError = static (state, id, ref message) => IVTSLogger.Default.LogError($"({state}) {id}: {message.GetString()}");
			VTSPool.Shared.Return(handler);
		}
		{
			var handler = VTSPool.Shared.Rent<VTSStatefulStackReaderResultHandler<int>>();
			handler.State = 42;
			handler.OnSuccess = static (state, ref reader) =>
			{
				TestStackData data = new(ref reader);
				IVTSLogger.Default.LogInfo($"Success ({state})! {data.ToString()}");
			};
			handler.OnError = static (state, id, ref message) => IVTSLogger.Default.LogError($"({state}) {id}: {message.GetString()}");
			VTSPool.Shared.Return(handler);
		}
	}
}



/// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
/// .
/// .                                                Stack handlers
/// .
/// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
public delegate void VTSStackSuccessResultEventHandler();
public delegate void VTSStackReaderSuccessResultEventHandler(ref Utf8JsonReader reader);
public delegate void VTSStackErrorResultEventHandler(ErrorID errorID, ref Utf8JsonReader errorMessageReader);

public delegate void VTSStatefulStackSuccessResultEventHandler<TState>(TState? state);
public delegate void VTSStatefulStackReaderSuccessResultEventHandler<TState>(TState? state, ref Utf8JsonReader reader);
public delegate void VTSStatefulStackErrorResultEventHandler<TState>(TState? state, ErrorID errorID, ref Utf8JsonReader errorMessageReader);

public sealed class VTSStatefulStackReaderResultHandler<TState> : IVTSStackResultHandler
{
	public VTSStatefulStackReaderSuccessResultEventHandler<TState>? OnSuccess { get; set; }
	public VTSStatefulStackErrorResultEventHandler<TState>? OnError { get; set; }
	public TState? State { get; set; }

	public void SetError(ErrorID errorID, ref Utf8JsonReader errorMessageReader) => OnError?.Invoke(State, errorID, ref errorMessageReader);
	public void SetSuccess(ref Utf8JsonReader reader) => OnSuccess?.Invoke(State, ref reader);
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public sealed class VTSStatefulStackResultHandler<TState> : IVTSStackResultHandler
{
	public VTSStatefulStackSuccessResultEventHandler<TState>? OnSuccess { get; set; }
	public VTSStatefulStackErrorResultEventHandler<TState>? OnError { get; set; }
	public TState? State { get; set; }

	public void SetError(ErrorID errorID, ref Utf8JsonReader errorMessageReader) => OnError?.Invoke(State, errorID, ref errorMessageReader);
	public void SetSuccess(ref Utf8JsonReader reader) => OnSuccess?.Invoke(State);
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
		State = default;
	}
}

public sealed class VTSStackReaderResultHandler : IVTSStackResultHandler
{
	public VTSStackReaderSuccessResultEventHandler? OnSuccess { get; set; }
	public VTSStackErrorResultEventHandler? OnError { get; set; }

	public void SetError(ErrorID errorID, ref Utf8JsonReader errorMessageReader) => OnError?.Invoke(errorID, ref errorMessageReader);
	public void SetSuccess(ref Utf8JsonReader reader) => OnSuccess?.Invoke(ref reader);
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public sealed class VTSStackResultHandler : IVTSStackResultHandler
{
	public VTSStackSuccessResultEventHandler? OnSuccess { get; set; }
	public VTSStackErrorResultEventHandler? OnError { get; set; }

	public void SetError(ErrorID errorID, ref Utf8JsonReader errorMessageReader) => OnError?.Invoke(errorID, ref errorMessageReader);
	public void SetSuccess(ref Utf8JsonReader reader) => OnSuccess?.Invoke();
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public interface IVTSStackResultHandler : IVTSPoolable
{
	public void SetSuccess(ref Utf8JsonReader reader);
	public void SetError(ErrorID errorID, ref Utf8JsonReader errorMessageReader);
}



/// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
/// .
/// .                                              Poolable handlers
/// .
/// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
public delegate void VTSSuccessResultEventHandler();
/// <returns>
/// <see langword="true"/> - if <typeparamref name="TData"/> should be returned to <see cref="VTSPool"/> from which it came.
/// <see langword="false"/> - if you want to store <typeparamref name="TData"/> somewhere else, and don't want it to be returned.
/// </returns>
public delegate bool VTSSuccessResultEventHandler<TData>(TData data);
public delegate void VTSErrorResultEventHandler(ErrorID errorID, string errorMessage);

public delegate void VTSStatefulSuccessResultEventHandler<TState>(TState? state);
/// <returns>
/// <see langword="true"/> - if <typeparamref name="TData"/> should be returned to <see cref="VTSPool"/> from which it came.
/// <see langword="false"/> - if you want to store <typeparamref name="TData"/> somewhere else, and don't want it to be returned.
/// </returns>
public delegate bool VTSStatefulSuccessResultEventHandler<TState, TData>(TState? state, TData data);
public delegate void VTSStatefulErrorResultEventHandler<TState>(TState? state, ErrorID errorID, string errorMessage);


[Serializable]
public class VTSResultTargetTypeMismatchException : Exception
{
	public VTSResultTargetTypeMismatchException() { }
	public VTSResultTargetTypeMismatchException(string message) : base(message) { }
	public VTSResultTargetTypeMismatchException(string message, Exception inner) : base(message, inner) { }

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void Throw(Type expected, Type provided)
	{
		throw new VTSResultTargetTypeMismatchException($"Expected type ({expected.Name}), but got ({provided.Name})");
	}
}

public sealed class VTSUniTaskAutoResetResultHandler<TData> : IVTSResultHandler, IUniTaskSource<NewResult<TData>> where TData : VTSPacketData
{
	Type? IVTSResultHandler.TargetType => typeof(TData);
	public UniTask<NewResult<TData>> Task => new(this, Source.Version);

	private UniTaskCompletionSourceCore<NewResult<TData>> Source;
	private short Version;
	private VTSUniTaskAutoResetResultHandler() { }

	public static VTSUniTaskAutoResetResultHandler<TData> Create()
	{
		var handler = VTSPool.Shared.Rent<VTSUniTaskAutoResetResultHandler<TData>>(static () => new());
		handler.Version = handler.Source.Version;
		return handler;
	}

	public void SetError(ErrorID errorID, string errorMessage)
	{
		if (Version == Source.Version)
			Source.TrySetResult(new(errorID, errorMessage));
	}

	public void SetSuccess(VTSPacketData? data)
	{
		if (data is null)
			throw new ArgumentNullException(nameof(data));

		if (data is not TData d)
		{
			VTSResultTargetTypeMismatchException.Throw(typeof(TData), data.GetType());
			return;
		}

		if (Version == Source.Version)
		{
			Source.TrySetResult(new(d));
		}
	}

	// TODO: Check if it returns to the pool. Because I don't think it does.
	NewResult<TData> IUniTaskSource<NewResult<TData>>.GetResult(short token) => Source.GetResult(token);
	NewResult<TData> IValueTaskSource<NewResult<TData>>.GetResult(short token)
	{
		try
		{
			return Source.GetResult(token);
		}
		finally
		{
			VTSPool.Shared.Return(this);
		}
	}

	void IUniTaskSource.GetResult(short token) => ((IValueTaskSource)this).GetResult(token);
	UniTaskStatus IUniTaskSource<NewResult<TData>>.GetStatus(short token) => Source.GetStatus(token);
	UniTaskStatus IUniTaskSource.GetStatus(short token) => Source.GetStatus(token);
	UniTaskStatus IUniTaskSource.UnsafeGetStatus() => Source.UnsafeGetStatus();
	void IUniTaskSource<NewResult<TData>>.OnCompleted(Action<object> continuation, object state, short token) => Source.OnCompleted(continuation, state, token);
	void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token) => Source.OnCompleted(continuation, state, token);
	void IVTSPoolable.Reset() => Source.Reset();
}

public sealed class VTSUniTaskAutoResetResultHandler : IVTSResultHandler, IUniTaskSource<NewResult>
{
	Type? IVTSResultHandler.TargetType => null;
	public UniTask<NewResult> Task => new(this, Source.Version);

	private UniTaskCompletionSourceCore<NewResult> Source;
	private short Version;
	private VTSUniTaskAutoResetResultHandler() { }

	public static VTSUniTaskAutoResetResultHandler Create()
	{
		var handler = VTSPool.Shared.Rent<VTSUniTaskAutoResetResultHandler>(static () => new());
		handler.Version = handler.Source.Version;
		return handler;
	}

	public void SetError(ErrorID errorID, string errorMessage)
	{
		if (Version == Source.Version)
			Source.TrySetResult(new(errorID, errorMessage));
	}

	public void SetSuccess(VTSPacketData? data)
	{
		if (Version == Source.Version)
			Source.TrySetResult(NewResult.Successful);
	}

	// TODO: Check if it returns to the pool. Because I don't think it does.
	NewResult IUniTaskSource<NewResult>.GetResult(short token) => Source.GetResult(token);
	NewResult IValueTaskSource<NewResult>.GetResult(short token)
	{
		try
		{
			return Source.GetResult(token);
		}
		finally
		{
			VTSPool.Shared.Return(this);
		}
	}

	void IUniTaskSource.GetResult(short token) => ((IValueTaskSource)this).GetResult(token);
	UniTaskStatus IUniTaskSource<NewResult>.GetStatus(short token) => Source.GetStatus(token);
	UniTaskStatus IUniTaskSource.GetStatus(short token) => Source.GetStatus(token);
	UniTaskStatus IUniTaskSource.UnsafeGetStatus() => Source.UnsafeGetStatus();
	void IUniTaskSource<NewResult>.OnCompleted(Action<object> continuation, object state, short token) => Source.OnCompleted(continuation, state, token);
	void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token) => Source.OnCompleted(continuation, state, token);
	void IVTSPoolable.Reset() => Source.Reset();
}

public sealed class VTSStatefulResultHandler<TState, TData> : IVTSResultHandler where TData : VTSPacketData
{
	Type? IVTSResultHandler.TargetType => typeof(TData);
	public VTSStatefulSuccessResultEventHandler<TState, TData>? OnSuccess { get; set; }
	public VTSStatefulErrorResultEventHandler<TState>? OnError { get; set; }
	public TState? State { get; set; }

	public void SetError(ErrorID errorID, string errorMessage) => OnError?.Invoke(State, errorID, errorMessage);
	public void SetSuccess(VTSPacketData? data)
	{
		if (data is null)
			throw new ArgumentNullException(nameof(data));

		if (OnSuccess is null) return;
		if (data is not TData d)
		{
			VTSResultTargetTypeMismatchException.Throw(typeof(TData), data.GetType());
			return;
		}

		if (OnSuccess(State, d) == false)
			return;

		VTSPool.Shared.Return(d);
	}
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public sealed class VTSStatefulResultHandler<TState> : IVTSResultHandler
{
	Type? IVTSResultHandler.TargetType => null;
	public VTSStatefulSuccessResultEventHandler<TState>? OnSuccess { get; set; }
	public VTSStatefulErrorResultEventHandler<TState>? OnError { get; set; }
	public TState? State { get; set; }

	public void SetError(ErrorID errorID, string errorMessage) => OnError?.Invoke(State, errorID, errorMessage);
	public void SetSuccess(VTSPacketData? data) => OnSuccess?.Invoke(State);
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public sealed class VTSResultHandler<TData> : IVTSResultHandler where TData : VTSPacketData
{
	Type? IVTSResultHandler.TargetType => typeof(TData);
	public VTSSuccessResultEventHandler<TData>? OnSuccess { get; set; }
	public VTSErrorResultEventHandler? OnError { get; set; }

	public void SetError(ErrorID errorID, string errorMessage) => OnError?.Invoke(errorID, errorMessage);
	public void SetSuccess(VTSPacketData? data)
	{
		if (data is null)
			throw new ArgumentNullException(nameof(data));

		if (OnSuccess is null) return;
		if (data is not TData d)
		{
			VTSResultTargetTypeMismatchException.Throw(typeof(TData), data.GetType());
			return;
		}

		if (OnSuccess(d) == false)
			return;

		VTSPool.Shared.Return(d);
	}
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public sealed class VTSResultHandler : IVTSResultHandler
{
	Type? IVTSResultHandler.TargetType => null;
	public VTSSuccessResultEventHandler? OnSuccess { get; set; }
	public VTSErrorResultEventHandler? OnError { get; set; }

	public void SetError(ErrorID errorID, string errorMessage) => OnError?.Invoke(errorID, errorMessage);
	public void SetSuccess(VTSPacketData? data) => OnSuccess?.Invoke();
	public void Reset()
	{
		OnSuccess = null;
		OnError = null;
	}
}

public interface IVTSResultHandler : IVTSPoolable
{
	/// <summary>
	/// <see cref="Type"/> to provide in <see cref="SetSuccess(VTSPacketData?)"/>.
	/// <see langword="null"/> if type doesn't matter/ignored.
	/// </summary>
	Type? TargetType { get; }
	void SetError(ErrorID errorID, string errorMessage);
	void SetSuccess(VTSPacketData? data);
}

public readonly struct NewResult(ErrorID errorID, string errorMessage)
{
	public static readonly NewResult Successful = default;
	/// <summary>
	/// ErrorID from the error message.
	/// </summary>
	public readonly ErrorID ErrorID { get; } = errorID;
	/// <remarks>
	/// Error message is allocated as a <see langword="string"/>.
	/// Error messages do not appear unless something goes bad, and if it does - you'll usually log or display it anyway.
	/// So we allocate it. If we won't do it - most likely consumer will.
	/// </remarks>
	public readonly string ErrorMessage { get; } = errorMessage;
	public bool IsSuccessful() => ErrorID == ErrorID.None;
}

public readonly struct NewResult<T> : IDisposable where T : VTSPacketData
{
	/// <summary>
	/// Data rented from a <see cref="VTSPool"/>.
	/// </summary>
	/// <remarks>
	/// Result is successful if <see cref="Data"/> is not <see langword="null"/>.
	/// </remarks>
	public readonly T? Data { get; }
	/// <summary>
	/// ErrorID from the error message.
	/// </summary>
	public readonly ErrorID ErrorID { get; }
	/// <remarks>
	/// Error message is allocated as a <see langword="string"/>.
	/// Error messages do not appear unless something goes bad, and if it does - you'll usually log or display it anyway.
	/// So we allocate it. If we won't do it - most likely consumer will.
	/// </remarks>
	public readonly string ErrorMessage { get; }

	public NewResult(ErrorID errorId, string errorMessage)
	{
		Data = null;
		ErrorID = errorId;
		ErrorMessage = errorMessage;
	}

	public NewResult(T data) : this(data, string.Empty) { }
	public NewResult(T data, string errorMessage)
	{
		Data = data;
		ErrorID = ErrorID.None;
		ErrorMessage = errorMessage;
	}

	[MemberNotNullWhen(true, nameof(Data))]
	public bool IsSuccessful() => ErrorID == ErrorID.None;

	[MemberNotNullWhen(true, nameof(Data))]
	public bool IsSuccessful([NotNullWhen(true)] out T? data)
	{
		data = Data;
		return ErrorID == ErrorID.None;
	}

	public void Dispose()
	{
		if (Data is not null)
			VTSPool.Shared.Return(Data);
	}
}

/// <summary>
/// Struct for holding request response.
/// Rarely used fields are left as raw collection of bytes, <see cref="ReadOnlyMemory{T}"/>, to improve performance.
/// </summary>
/// <remarks>
/// Don't forger to <see cref="Dispose"/> after use!
/// (or use within <see langword="using"/> block)
/// </remarks>
public readonly struct RequestResult<T> : IDisposable where T : VTSPacketData
{
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
	/// Data deserialized from <see cref="Memory"/> using <see cref="VTSAPI.Encoding"/>.
	/// </summary>
	public readonly required T? Data { get; init; }
	/// <summary>
	/// Memory segment with raw remote <see cref="VTSPacket"/>.
	/// </summary>
	/// <remarks>
	/// <see langword="null"/> for requests which failed client-side.
	/// </remarks>
	/// Note: probably better to replace with already re-encoded to UTF8 char memory segment.
	public readonly required IMemoryOwner<char>? Memory { get; init; }
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
	/// Can be parsed using <see cref="VTSAPI.Encoding"/>.
	/// </remarks>
	/// Note: can be optimized by only registering bounds of the message.
	/// <see cref="ReadOnlyMemory{T}"/> stores reference to a location in memory, increasing its size by 8 bytes.
	public readonly required ReadOnlyMemory<byte> ErrorMessage { get; init; }
	/// <summary>
	/// <see cref="VTSAPI.JsonPropertyNames.APIName"/> raw value as UTF8 byte segment.
	/// </summary>
	/// <remarks>
	/// Can be parsed using <see cref="VTSAPI.Encoding"/>.
	/// </remarks>
	public readonly required ReadOnlyMemory<byte> APIName { get; init; }
	/// <summary>
	/// <see cref="VTSAPI.JsonPropertyNames.APIVersion"/> raw value as UTF8 byte segment.
	/// </summary>
	/// <remarks>
	/// Can be parsed using <see cref="VTSAPI.Encoding"/>.
	/// </remarks>
	public readonly required ReadOnlyMemory<byte> APIVersion { get; init; }
	/// <summary>
	/// <see cref="VTSAPI.JsonPropertyNames.RequestID"/> raw value as UTF8 byte segment.
	/// </summary>
	/// <remarks>
	/// Can be parsed using <see cref="VTSAPI.Encoding"/>.
	/// </remarks>
	public readonly required ReadOnlyMemory<byte> RequestID { get; init; }
	/// <summary>
	/// <see cref="VTSAPI.JsonPropertyNames.MessageType"/> raw value as UTF8 byte segment.
	/// </summary>
	/// <remarks>
	/// Can be parsed using <see cref="VTSAPI.Encoding"/>.
	/// </remarks>
	public readonly required ReadOnlyMemory<byte> MessageType { get; init; }
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
		data = Data;
		return Successful;
	}
	/// <summary>
	/// Disposes <see cref="Memory"/>, and so returns it to the pool.
	/// </summary>
	public void Dispose()
	{
		Memory?.Dispose();
		if (Data is not null)
			VTSPool.Shared.Return(Data);
	}
}
