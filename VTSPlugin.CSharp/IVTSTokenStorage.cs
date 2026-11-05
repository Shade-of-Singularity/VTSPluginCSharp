using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Interface for a service, managing tokens for known plugins.
/// </summary>
public interface IVTSTokenStorage : IVTSSubSystem, IHasVTSLogger
{
	/// <summary>
	/// Default thread-safe implementation for <see cref="IVTSTokenStorage"/>.
	/// </summary>
	public static IVTSTokenStorage Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConcurrentFileTokenStorage.Instance;
	}
	/// <summary>
	/// Retrieves token, associated with provided <paramref name="plugin"/>
	/// </summary>
	/// <returns>
	/// <see langword="string"/> - when an associated token exists.
	/// <see langword="null"/> - when there is no token, associated with provided <paramref name="plugin"/>.
	/// </returns>
	public string? GetToken(IVTSPlugin plugin);
	/// <summary>
	/// Associates provided <paramref name="token"/> with a provided <paramref name="plugin"/>.
	/// </summary>
	/// <remarks>
	/// <see langword="null"/> is treated as <see cref="DeleteToken(IVTSPlugin)"/>.
	/// </remarks>
	/// <param name="plugin"></param>
	/// <param name="token"></param>
	public void SetToken(IVTSPlugin plugin, string? token);
	/// <summary>
	/// Deletes token from the cache and underlying file in a system.
	/// </summary>
	public void DeleteToken(IVTSPlugin plugin);
}
