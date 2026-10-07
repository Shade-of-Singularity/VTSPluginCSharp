using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Interface for a service, managing tokens for known plugins.
/// </summary>
public interface ITokenStorage : IService, IHasVTSLogger
{
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

/// <summary>
/// <see cref="ITokenStorage"/> implementation, using directories as its main storage.
/// </summary>
public sealed class DirectoryTokenStorage : ITokenStorage
{
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