using Cysharp.Threading.Tasks;
using System.ComponentModel;

namespace VTSPlugin;

/// <summary>
/// Service interface allowing to provide addition initialization options.
/// </summary>
/// <typeparam name="T">Initialization options to provide.</typeparam>
public interface IService<T> : IService where T : IInitializationOptions
{
    /// <summary>
    /// Initializes the service with provided <paramref name="options"/>.
    /// <para>
    /// In concurrent environments, will wait until <see cref="IService.Terminate"/> quits, and then will attempt initialization.
    /// </para>
    /// </summary>
    /// <remarks>
    /// You should not assume
    /// </remarks>
    UniTask<bool> Initialize(T options);
}

/// <summary>
/// Interface for all services that can be initialized/terminated.
/// </summary>
/// <remarks>
/// Implements <see cref="INotifyPropertyChanged"/>, despite being immutable sometimes, to avoid memory leaks in WPF.
/// </remarks>
public interface IService : INotifyPropertyChanged
{
    /// <summary>
    /// Whether service is initialized or not.
    /// </summary>
    /// <remarks>
    /// Should be covered by <see cref="INotifyPropertyChanged"/> callback.
    /// </remarks>
    bool IsInitialized { get; }
    /// <summary>
    /// Initializes <see cref="IService"/> with default initialization options.
    /// </summary>
    UniTask<bool> Initialize();
    /// <summary>
    /// Terminates the logger, flushes and closes underlying file stream.
    /// <para>
    /// In concurrent environments, will wait until <see cref="Terminate"/> quits, and then will attempt initialization.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Depending on the implementation, logger might still be usable even when not initialized.
    /// However, this is not guaranteed, and logging to files might probably be broken.
    /// </remarks>
    UniTask<bool> Terminate();
}