using System.Runtime.CompilerServices;
using static VTSPlugin.VTSInitializationStatus;

namespace VTSPlugin;

/// <summary>
/// Useful extensions for working with <see cref="VTSInitializationStatus"/>.
/// </summary>
public static class VTSInitializationStatusExtensions
{
	/// <summary>
	/// Mask covering bits, which are common for transitional states.
	/// </summary>
	const VTSInitializationStatus TransientMask = (VTSInitializationStatus)0b01;
	/// <summary>
	/// Mask, covering all the valid bits.
	/// </summary>
	const VTSInitializationStatus FullMask = (VTSInitializationStatus)0b11;

	/// <summary>
	/// Maps to any state which is a transition between two stable ones.
	/// </summary>
	/// <remarks>
	/// <paramref name="status"/> <see langword="is"/> <see cref="Initializing"/> <see langword="or"/> <see cref="Terminating"/>
	/// </remarks>
	/// IsTransient: 01, 11
	/// - (00 AND 01) != 00 >>> 00 != 00
	/// - (01 AND 01) != 00 >>> 01 == 00
	/// - (10 AND 01) != 00 >>> 00 != 00
	/// - (11 AND 01) != 00 >>> 01 == 00
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsTransient(this VTSInitializationStatus status) => (status & TransientMask) == default;

	/// <summary>
	/// Maps to any state which are stable,
	/// and require additional input to change (i.e. <see cref="IVTSSubSystem.Initialize"/>, etc.).
	/// </summary>
	/// <remarks>
	/// <paramref name="status"/> <see langword="is"/> <see cref="Initialized"/> <see langword="or"/> <see cref="Terminated"/>
	/// </remarks>
	/// IsStable: 00, 10
	/// - (00 AND 01) == 00 >>> 00 == 00
	/// - (01 AND 01) == 00 >>> 01 != 00
	/// - (10 AND 01) == 00 >>> 00 == 00
	/// - (11 AND 01) == 00 >>> 01 != 00
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsStable(this VTSInitializationStatus status) => (status & TransientMask) != default;

	/// <summary>
	/// Moves status forward.
	/// </summary>
	/// <remarks>
	/// Loops back from last to default state:
	/// from <see cref="Terminating"/> to <see cref="Terminated"/>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static VTSInitializationStatus Next(this VTSInitializationStatus status) => unchecked(status + 1) & FullMask;

	/// <summary>
	/// Moves status backwards.
	/// </summary>
	/// <remarks>
	/// Loops to last state from default one:
	/// from <see cref="Terminated"/> to <see cref="Terminating"/>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static VTSInitializationStatus Previous(this VTSInitializationStatus status) => unchecked(status - 1) & FullMask;
}
