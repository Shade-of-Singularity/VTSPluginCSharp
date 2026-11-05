using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "extension keyword needs instance methods")]
public static class ErrorIDExtensions
{
	extension(ErrorID id)
	{
		/// <summary>
		/// Checks if error appeared due to API error, after sending the request to VTube Studio.
		/// Alternative is <see cref="IsClientSide(ErrorID)"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ErrorID.None"/> is not Client-side nor Server-side error!
		/// </remarks>
		/// <returns>
		/// <see langword="true"/> - if <paramref name="id"/> represents one of the errors, VTube Studio may return on an API error.
		/// <see langword="false"/> - if <paramref name="id"/> represents an error, that occurred before anything was received from VTube Studio (timeout, etc)
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsServerSide() => id > ErrorID.None;

		/// <summary>
		/// Checks if error appeared due to to client-side error, such as: Request timeout, parsing error, etc.
		/// Alternative is <see cref="IsServerSide(ErrorID)"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ErrorID.None"/> is not Client-side nor Server-side error!
		/// </remarks>
		/// <returns>
		/// <see langword="true"/> - if <paramref name="id"/> represents an error, that occurred before anything was received from VTube Studio (timeout, etc)
		/// <see langword="false"/> - if <paramref name="id"/> represents one of the errors, VTube Studio may return on an API error.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsClientSide() => id < ErrorID.None;
	}
}