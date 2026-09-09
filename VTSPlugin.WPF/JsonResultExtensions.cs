using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace VTSPlugin.WPF;

/// <summary>
/// Some useful extensions for working with <see cref="JsonResult{T}"/>s.
/// </summary>
//public static class JsonResultExtensions
//{
//    extension<T>(Task<JsonResult<T>> task) where T : VTSResponseData
//    {
//        /// <inheritdoc cref="JsonResult{T}.UnpackScoped"/>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public async Task<JsonResult<T>.Scope> WithUnpackScoped() => (await task).UnpackScoped();

//        /// <inheritdoc cref="JsonResult{T}.TryUnpackScoped"/>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public async Task<JsonResult<T>.Scope> WithTryUnpackScoped() => (await task).TryUnpackScoped();
//    }

//    extension<T>(UniTask<JsonResult<T>> task) where T : VTSResponseData
//    {
//        /// <inheritdoc cref="JsonResult{T}.UnpackScoped"/>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public async UniTask<JsonResult<T>.Scope> WithUnpackScoped() => (await task).UnpackScoped();

//        /// <inheritdoc cref="JsonResult{T}.TryUnpackScoped"/>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public async UniTask<JsonResult<T>.Scope> WithTryUnpackScoped() => (await task).TryUnpackScoped();
//    }
//}
