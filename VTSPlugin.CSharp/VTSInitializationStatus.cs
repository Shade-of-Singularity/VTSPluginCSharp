namespace VTSPlugin;

/// <summary>
/// Status of an <see cref="IVTSSubSystem"/>.
/// </summary>
/// <remarks>
/// Optimized for both jump-table and binary operations with no compromises.
/// Outside of jump-tables, you can use <see cref="VTSInitializationStatusExtensions"/> to simplify checks.
/// </remarks>
public enum VTSInitializationStatus : byte
{
	/// <summary>
	/// (<see langword="default"/>) System is terminated, or was never initialized.
	/// </summary>
	/// <remarks>
	/// This is a Stable state (see also: <see cref="VTSInitializationStatusExtensions.IsStable"/>)
	/// </remarks>
	Terminated = 0b00,
	/// <summary>
	/// System is initializing.
	/// </summary>
	/// <remarks>
	/// This is a Transient state (see also: <see cref="VTSInitializationStatusExtensions.IsTransient"/>)
	/// </remarks>
	Initializing = 0b01,
	/// <summary>
	/// System is fully initialized.
	/// </summary>
	/// <remarks>
	/// This is a Stable state (see also: <see cref="VTSInitializationStatusExtensions.IsStable"/>)
	/// </remarks>
	Initialized = 0b10,
	/// <summary>
	/// System is terminating.
	/// </summary>
	/// <remarks>
	/// This is a Transient state (see also: <see cref="VTSInitializationStatusExtensions.IsTransient"/>)
	/// </remarks>
	Terminating = 0b11,
}
