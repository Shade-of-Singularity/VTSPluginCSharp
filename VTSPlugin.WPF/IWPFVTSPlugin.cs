using System.ComponentModel;

namespace VTSPlugin.WPF;

/// <summary>
/// WPF specific interface for <see cref="IVTSPlugin"/>
/// </summary>
public interface IWPFVTSPlugin : IVTSPlugin, INotifyPropertyChanged;