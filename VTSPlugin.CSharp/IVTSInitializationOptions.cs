using System.ComponentModel;
using VTSPlugin.Pooling;

namespace VTSPlugin;

/// <summary>
/// Base interface for <see cref="IVTSSubSystem{T}"/> initialization options.
/// Those options can be rented/returned using <see cref="VTSPool"/>.
/// </summary>
/// <remarks>
/// Implements <see cref="INotifyPropertyChanged"/>, despite usually being immutable, to avoid memory leaks in WPF.
/// </remarks>
public interface IVTSInitializationOptions
#if WPF
	: IVTSPoolable, IVTSFreezable, INotifyPropertyChanged
#else
	: IVTSPoolable, IVTSFreezable
#endif
;