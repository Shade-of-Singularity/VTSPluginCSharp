using System.Runtime.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base exception class for all <see cref="VTSPlugin"/> exceptions.
/// </summary>
/// <remarks>
/// As the library is WIP and has gone over a large rework - some exceptions might still inherit <see cref="Exception"/>.
/// If you will notice any of such exceptions - open a GitHub issue! Thank you for your help!
/// https://github.com/Shade-of-Singularity/VTSPluginCSharp/issues
/// </remarks>
public class VTSException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VTSException"/> class.
    /// </summary>
    /// <inheritdoc/>
    public VTSException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VTSException"/> class with a specified error <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <inheritdoc/>
    public VTSException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VTSException"/> class with a specified error
    /// </summary>
    /// <paramref name="message"/> and a reference to the <paramref name="innerException"/> that is the cause of this exception.
    /// <inheritdoc/>
    public VTSException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VTSException"/> class with serialized data.
    /// </summary>
    /// <inheritdoc/>
    protected VTSException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
