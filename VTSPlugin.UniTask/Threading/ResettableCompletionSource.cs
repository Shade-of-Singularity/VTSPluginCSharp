using Cysharp.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace VTSPlugin.Threading;

public sealed class ResettableCompletionSource<T> : IUniTaskSource<T>
{
    UniTaskCompletionSourceCore<T> Source;

    /// <summary>
    /// Completion source around a <see cref="ManualResetValueTaskSourceCore{TResult}"/>.
    /// </summary>
    public UniTask<T> Task => new(this, Source.Version);

    // Our API.
    public void Reset() => Source.Reset();

    // IUniTaskSource implementation.
    public T GetResult(short token) => Source.GetResult(token);
    public UniTaskStatus GetStatus(short token) => Source.GetStatus(token);
    public void OnCompleted(Action<object> continuation, object state, short token) => Source.OnCompleted(continuation, state, token);
    public UniTaskStatus UnsafeGetStatus() => Source.UnsafeGetStatus();
    void IUniTaskSource.GetResult(short token) => Source.GetResult(token);
}
