using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Thread-safe <see cref="IVTSTokenStorage"/> implementation, using files as its main storage.
/// </summary>
public sealed class ConcurrentFileTokenStorage : IVTSTokenStorage
{
	/// <summary>
	/// Default instance to
	/// </summary>
	public static readonly ConcurrentFileTokenStorage Instance = new();

	/// <inheritdoc/>
	public bool IsInitialized { get; set; }

	/// <inheritdoc/>
	public IVTSLogger Logger { get; set; } = IVTSLogger.Default;

	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	// Note: we can probably cache path to the token as well, to not recreate it...
	// As long as it doesn't consume too much memory with 100 tokens.
	readonly ConcurrentDictionary<string, string> Tokens = new();
	readonly Channel<TokenAction> ActionQueue = Channel.CreateSingleConsumerUnbounded<TokenAction>();
	readonly struct TokenAction(bool isRemoval, string id, string token)
	{
		public readonly bool IsRemoval = isRemoval;
		public readonly string ID = id;
		public readonly string Token = token;
	}

	/// <inheritdoc/>
	public void DeleteToken(IVTSPlugin plugin) => Tokens.TryRemove(RetrieveID(plugin), out _);

	/// <inheritdoc/>
	public string? GetToken(IVTSPlugin plugin) => Tokens.TryGetValue(RetrieveID(plugin), out string? token) ? token : default;

	/// <inheritdoc/>
	public void SetToken(IVTSPlugin plugin, string? token)
	{
		if (token is null)
			DeleteToken(plugin);
		else
			Tokens[RetrieveID(plugin)] = token;
	}

	/// <inheritdoc/>
	public UniTask<bool> Initialize()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public UniTask<bool> Terminate()
	{
		throw new NotImplementedException();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static string RetrieveID(IVTSPlugin plugin)
	{
		return plugin.ID ?? throw new ArgumentException($"ID from ({nameof(IVTSPlugin)}) cannot be null!");
	}
}