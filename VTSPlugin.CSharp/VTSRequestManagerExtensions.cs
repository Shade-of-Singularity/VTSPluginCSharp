using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using VTSPlugin.Pooling;
using VTSPlugin.Requests;

namespace VTSPlugin;

/// <summary>
/// Useful extensions for working with <see cref="IVTSConnection"/>.
/// </summary>
#pragma warning disable CA1822 // Extension method analyzers have a bug, where they ask to be static when they shouldn't. This disables it.
public static class VTSRequestManagerExtensions
{
    extension(IVTSRequestManager manager)
    {
        /// <inheritdoc cref="IVTSRequestManager.Request{T}(VTSRequestTemplate, TimeSpan)"/>
        /// <remarks>
        /// Request will have an infinite timeout.
        /// </remarks>
        /// <seealso cref="Timeout.InfiniteTimeSpan"/>
        public UniTask<RequestResult<T>> Request<T>(VTSRequestTemplate request)
            where T : VTSResponseData
        {
            return manager.Request<T>(request, Timeout.InfiniteTimeSpan);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Requests
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Requests API state from currently connected VTube Studio app.
        /// <para>
        /// Never times out.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAPIState(IVTSRequestManager, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAPIStateResponseData>> RequestAPIState()
        {
            return RequestAPIState(manager, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Requests API state from currently connected VTube Studio app.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#api-details"/>
        /// </remarks>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        public UniTask<RequestResult<VTSAPIStateResponseData>> RequestAPIState(TimeSpan timeout)
        {
            var request = VTSAPIStateRequest.Rent();
            var task = manager.Request<VTSAPIStateResponseData>(request, timeout);
            VTSPool.Shared.Return(request);
            return task;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                           Authentication Request
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Requests authentication of an old session using info from provided <see cref="IVTSPlugin"/>.
        /// <para>
        /// Never times out.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAuthentication(IVTSRequestManager, IVTSPlugin, ITokenStorage, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAuthenticationResponseData>> RequestAuthentication
            (IVTSPlugin plugin, ITokenStorage storage)
        {
            return RequestAuthentication(manager, plugin.Name, plugin.Developer, storage.GetToken(plugin));
        }

        /// <summary>
        /// Requests authentication of an old session using info from provided <see cref="IVTSPlugin"/>.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <param name="plugin">Plugin into to use in authentication.</param>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        /// <inheritdoc cref="RequestAuthentication(IVTSRequestManager, string?, string?, string?, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAuthenticationResponseData>> RequestAuthentication
            (IVTSPlugin plugin, ITokenStorage storage, TimeSpan timeout)
        {
            return RequestAuthentication(manager, plugin.Name, plugin.Developer, storage.GetToken(plugin), timeout);
        }

        /// <summary>
        /// Requests authentication of an old session using provided <paramref name="token"/> and plugin info.
        /// <para>
        /// Never times out.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAuthentication(IVTSRequestManager, string?, string?, string?, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAuthenticationResponseData>> RequestAuthentication
            (string? pluginName, string? pluginDeveloper, string? token)
        {
            return RequestAuthentication(manager, pluginName, pluginDeveloper, token, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Requests authentication of an old session using provided <paramref name="token"/> and plugin info.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
        /// </remarks>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <param name="pluginDeveloper">Developer(s), who created this plugin.</param>
        /// <param name="token">Authentication token for authenticating previous sessions.</param>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        public UniTask<RequestResult<VTSAuthenticationResponseData>> RequestAuthentication
            (string? pluginName, string? pluginDeveloper, string? token, TimeSpan timeout)
        {
            pluginName ??= IVTSPlugin.DefaultName;
            pluginDeveloper ??= IVTSPlugin.DefaultDeveloper;

            var request = VTSAuthenticationRequest.Rent();
            request.Data.PluginName = pluginName;
            request.Data.PluginDeveloper = pluginDeveloper;
            request.Data.AuthenticationToken = token;
            var task = manager.Request<VTSAuthenticationResponseData>(request, timeout);
            VTSPool.Shared.Return(request);
            return task;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Token Request
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Requests new authentication token for a given <paramref name="plugin"/>.
        /// Newly generated token will be automatically stored in a provided <paramref name="storage"/>.
        /// <para>
        /// Never times out.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAuthenticationToken(IVTSRequestManager, IVTSPlugin, ITokenStorage, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAuthenticationTokenResponseData>> RequestAuthenticationToken
            (IVTSPlugin plugin, ITokenStorage storage)
        {
            return RequestAuthenticationToken(manager, plugin, storage, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Requests new authentication token for a given <paramref name="plugin"/>.
        /// Newly generated token will be automatically stored in a provided <paramref name="storage"/>.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <param name="plugin">Plugin into to use in authentication.</param>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        /// <inheritdoc cref="RequestAuthenticationToken(IVTSRequestManager, string?, string?, string?, TimeSpan)"/>
        public async UniTask<RequestResult<VTSAuthenticationTokenResponseData>> RequestAuthenticationToken
            (IVTSPlugin plugin, ITokenStorage storage, TimeSpan timeout)
        {
            var result = await RequestAuthenticationToken(manager, plugin.Name, plugin.Developer, plugin.Icon, timeout);
            if (result.IsSuccessful(out var data))
            {
                storage.SetToken(plugin, data.AuthenticationToken);
            }
            return result;
        }

        /// <summary>
        /// Requests new authentication token using provided plugin info.
        /// <para>
        /// Never times out.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAuthenticationToken(IVTSRequestManager, string?, string?, string?, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAuthenticationTokenResponseData>> RequestAuthenticationToken
            (string? pluginName, string? pluginDeveloper, string? pluginBase64Icon)
        {
            return RequestAuthenticationToken(manager, pluginName, pluginDeveloper, pluginBase64Icon, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Requests new authentication token using provided plugin info.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
        /// </remarks>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <param name="pluginDeveloper">Developer(s), who created this plugin.</param>
        /// <param name="pluginBase64Icon">Icon, encoded as Base64.</param>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        public UniTask<RequestResult<VTSAuthenticationTokenResponseData>> RequestAuthenticationToken
            (string? pluginName, string? pluginDeveloper, string? pluginBase64Icon, TimeSpan timeout)
        {
            pluginName ??= IVTSPlugin.DefaultName;
            pluginDeveloper ??= IVTSPlugin.DefaultDeveloper;
            pluginBase64Icon ??= IVTSPlugin.DefaultBase64Icon;

            var request = VTSAuthenticationTokenRequest.Rent();
            request.Data.PluginName = pluginName;
            request.Data.PluginDeveloper = pluginDeveloper;
            request.Data.PluginIcon = pluginBase64Icon;
            var task = manager.Request<VTSAuthenticationTokenResponseData>(request, timeout);
            VTSPool.Shared.Return(request);
            return task;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                          Available Models Request
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Requests a list of all available VTube Studio models.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="RequestAvailableModels(IVTSRequestManager, TimeSpan)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<RequestResult<VTSAvailableModelsResponseData>> RequestAvailableModels()
        {
            return RequestAvailableModels(manager, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Requests a list of all available VTube Studio models.
        /// <para>
        /// Times out after a given <paramref name="timeout"/> duration, unless the request is satisfied.
        /// Returns <see cref="RequestResult.FromFailed{T}(ErrorID)"/> if VTube Studio disconnects.
        /// See also: <see cref="ErrorID.VTubeStudioDisconnected"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// API guide: https://github.com/DenchiSoft/VTubeStudio#getting-a-list-of-available-vts-models
        /// </remarks>
        /// <param name="timeout">Maximum waiting time until the request times out.</param>
        public UniTask<RequestResult<VTSAvailableModelsResponseData>> RequestAvailableModels(TimeSpan timeout)
        {
            var request = VTSAvailableModelsRequest.Rent();
            var task = manager.Request<VTSAvailableModelsResponseData>(request, timeout);
            VTSPool.Shared.Return(request);
            return task;
        }
    }
}
#pragma warning restore CA1822 // Members can be made static (they cannot - it's a bug)

/// <summary>
/// Some test methods, to test how API looks like so far.
/// </summary>
static class APITests
{
    static async UniTaskVoid Main()
    {
        IVTSPlugin plugin = null!;
        IVTSRequestManager vts = null!;
        ITokenStorage storage = new DirectoryTokenStorage();

        var api = await vts.RequestAPIState();
        if (api.IsSuccessful(out var apiData))
        {
            IVTSLogger.Default.LogInfo($"API state (Active: {apiData.Active}, authenticated: {apiData.CurrentSessionAuthenticated}, VTS version: {apiData.VTubeStudioVersion})");
        }
        else
        {
            IVTSLogger.Default.LogWarn($"Cannot retrieve VTube Studio API state! Error message: {api.ErrorMessage.AsUTF8String()}");
        }

        var token = await vts.RequestAuthenticationToken(plugin, storage);
        if (token.IsSuccessful(out var tokenData))
        {
            // Note: Don't do this. Do not log access tokens to console, or any interfaces, where it can be seen by other people.
            IVTSLogger.Default.LogInfo($"Authenticated! New authentication token: {tokenData.AuthenticationToken}");
        }
        else
        {
            IVTSLogger.Default.LogWarn($"Cannot request new token! Error message: {token.ErrorMessage.AsUTF8String()}");
        }

        var auth = await vts.RequestAuthentication(plugin, storage);
        if (auth.IsSuccessful(out var authData) && authData.Authenticated)
        {
            if (authData.Authenticated)
            {
                IVTSLogger.Default.LogInfo($"Authenticated successfully!");
            }
            else
            {
                IVTSLogger.Default.LogWarn($"Authentication failed or cancelled was cancelled. Reason: {authData.Reason}");
            }
        }
        else
        {
            IVTSLogger.Default.LogWarn($"Authentication request failed! Error message: {auth.ErrorMessage.AsUTF8String()}");
        }

        var models = await vts.RequestAvailableModels();
        if (models.IsSuccessful(out var modelsData))
        {
            IVTSLogger.Default.LogInfo($"Models info (Amount: {modelsData.NumberOfModels}). Models:\n{string.Join("\n", modelsData.AvailableModels?.Select(static m => $"ID: {m.ModelID}, Name: {m.ModelName}, Loaded: {m.ModelLoaded}") ?? [])}");
        }
        else
        {
            IVTSLogger.Default.LogWarn($"Cannot retrieve available models from VTube Studio!");
        }
    }
}