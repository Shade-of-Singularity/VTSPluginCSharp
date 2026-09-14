using Cysharp.Threading.Tasks;
using System.Buffers;
using System.Collections.Concurrent;
using VTSPlugin;
using VTSPlugin.Legacy;
using VTSPlugin.Threading;

namespace VTSPlugin;

/// <summary>
/// Experimental implementation testing RequestID processing with zero allocation.
/// </summary>
/// <remarks>
/// Marked as abstract to allow overrides. A default final implementation is provided as <see cref="VTSConnection"/>.
/// </remarks>
public abstract class AbstractVTSConnection : IVTSRequestManager
{
    public IVTSLogger Logger { get; private set; } = IVTSLogger.Default;

    // TODO: Fail all requests on disconnection/end-point kill.
    readonly ConcurrentDictionary<ReadOnlyMemory<byte>, RequestEntry> Requests = new();
    readonly Random Shared = new();
    readonly struct RequestEntry(IMemoryOwner<byte> allocation, ResettableCompletionSource<IMemoryOwner<byte>> completion)
    {
        public readonly IMemoryOwner<byte> allocation = allocation;
        public readonly ResettableCompletionSource<IMemoryOwner<byte>> completion = completion;
    }

    public async UniTask<RequestResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData
    {
        // Generates RequestID using 16 readable characters from A to P (4 bits).
        const int RequestIDLength = 32;
        Span<byte> values = stackalloc byte[RequestIDLength];
        Shared.NextBytes(values);
        for (int i = 0; i < RequestIDLength; i += 4)
        {
            const int Shift = 'A';
            const int RangeMask = 0b1111; // Range: from A to P.
            values[i] = (byte)((Shift + values[i]) & RangeMask);
            values[i + 1] = (byte)((Shift + values[i + 1]) & RangeMask);
            values[i + 2] = (byte)((Shift + values[i + 2]) & RangeMask);
            values[i + 3] = (byte)((Shift + values[i + 3]) & RangeMask);
        }
        var owner = MemoryPool<byte>.Shared.Rent(RequestIDLength);
        var slice = owner.Memory[..RequestIDLength];
        values.CopyTo(slice.Span);

        // Tries to list the request entry.
        // If failed - returns failed result.
        var source = ResettableCompletionSourcePool.Shared.Rent<IMemoryOwner<byte>>();
        RequestEntry entry = new(owner, source);
        try
        {
            if (Requests.TryAdd(slice, entry))
            {
                // TODO: Send request using IVTSSocket here.

                // TODO: Add pool-able timeout handler here, if timeout is not infinite.

                // Awaits for request to fail or be satisfied.
                // TODO: Add better handling for: cancellation, deserialization failure, etc.
                await source.Task;

                return new JsonResult<T>()
                {
                    Successful = true,
                    ErrorID = ErrorID.None,
                    ErrorMessage = ReadOnlyMemory<byte>.Empty,
                    // TODO: Slice those from a received packet.
                    Timestamp = default,
                    APIName = ReadOnlyMemory<byte>.Empty,
                    RequestID = ReadOnlyMemory<byte>.Empty,
                    APIVersion = ReadOnlyMemory<byte>.Empty,
                    Data = ReadOnlyMemory<byte>.Empty,
                    // Temporary here, until API rework.
                    Logger = Logger,
                    Deserializer = (i) =>
                    {
                        throw new NotImplementedException();
                    }
                };
            }
            else
            {
                // TODO: Reattempt RequestID generation 1024 times instead, and release resources if failed.
                Logger.TryLog("Request with a same ID already registered!", this);
            }
        }
        catch (OperationCanceledException) // Cancelled or timed out.
        {
            return JsonResult<T>.FromFailed(ErrorID.None);
        }
        catch (Exception ex)
        {
            Logger.TryLog(ex, this);
            return JsonResult<T>.FromFailed(ErrorID.ClientSideException);
        }
        finally
        {
            ResettableCompletionSourcePool.Shared.Return(source);
            owner.Dispose();
        }
    }
}
