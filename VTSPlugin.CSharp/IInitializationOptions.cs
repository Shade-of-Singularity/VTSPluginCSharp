using System.ComponentModel;
using VTSPlugin.Pooling;

namespace VTSPlugin;

/// <summary>
/// Base interface for <see cref="IService{T}"/> initialization options.
/// Those options can be rented/returned using <see cref="VTSPool"/>.
/// </summary>
/// <remarks>
/// Implements <see cref="INotifyPropertyChanged"/>, despite usually being immutable, to avoid memory leaks in WPF.
/// </remarks>
public interface IInitializationOptions : INotifyPropertyChanged, IVTSPoolable;