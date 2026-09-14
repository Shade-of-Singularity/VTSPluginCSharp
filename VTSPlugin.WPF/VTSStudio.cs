using System.Collections.ObjectModel;

namespace VTSPlugin;

/// <inheritdoc cref="IVTSStudio"/>
public sealed class VTSStudio : IVTSStudio
{
    public ObservableCollection<IVTSConnection> Connections => throw new NotImplementedException();
}