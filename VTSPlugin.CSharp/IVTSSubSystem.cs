using Cysharp.Threading.Tasks;
using System.ComponentModel;

namespace VTSPlugin;

/// <summary>
/// Interface for all subsystems that can be initialized/terminated.
/// </summary>
/// <remarks>
/// Implements <see cref="INotifyPropertyChanged"/>, despite being immutable sometimes, to avoid memory leaks in WPF.
/// </remarks>
/// TODO: In non-WPF environments, we might introduce event for Status change.
///  It's needed for handling if some system suddenly quits.
public interface IVTSSubSystem : IVTSFreezable
#if NOTIFY_CHANGES
	, INotifyPropertyChanged
#endif
{
	/// <summary>
	/// Current status of the subsystem.
	/// </summary>
	VTSInitializationStatus Status { get; }
	/// <summary>
	/// Initializes <see cref="IVTSSubSystem"/>.
	/// </summary>
	/// <remarks>
	/// Concurrent subsystems allow only one <see cref="Initialize"/> or <see cref="Terminate"/> operation at once.
	/// Behaviour might change in the future, but right now it puts callers in a queue via <see cref="SemaphoreSlim"/>.
	/// </remarks>
	UniTask Initialize();
	/// <summary>
	/// Terminates the logger, flushes and closes underlying file streams, etc.
	/// </summary>
	/// <remarks>
	/// Concurrent subsystems allow only one <see cref="Initialize"/> or <see cref="Terminate"/> operation at once.
	/// Behaviour might change in the future, but right now it puts callers in a queue via <see cref="SemaphoreSlim"/>.
	/// </remarks>
	UniTask Terminate();
}