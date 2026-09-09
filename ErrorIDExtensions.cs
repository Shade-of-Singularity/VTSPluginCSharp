using System.Runtime.CompilerServices;

namespace VTSPlugin;

public static class ErrorIDExtensions
{
    extension(ErrorID id)
    {
        /// <summary>
        /// Checks if error appeared due to API error, after sending the request to VTube Studio.
        /// Alternative is <see cref="IsClientSide(ErrorID)"/>.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> - if <paramref name="id"/> represents one of the errors, VTube Studio may return on an API error.
        /// <see langword="false"/> - if <paramref name="id"/> represents an error, that occurred before anything was received from VTube Studio (timeout, etc)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsServerSide() => id >= ErrorID.None;

        /// <summary>
        /// Checks if error appeared due to to client-side error, such as: Request timeout, parsing error, etc.
        /// Alternative is <see cref="IsServerSide(ErrorID)"/>.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> - if <paramref name="id"/> represents an error, that occurred before anything was received from VTube Studio (timeout, etc)
        /// <see langword="false"/> - if <paramref name="id"/> represents one of the errors, VTube Studio may return on an API error.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsClientSide() => id < ErrorID.None;
    }
}