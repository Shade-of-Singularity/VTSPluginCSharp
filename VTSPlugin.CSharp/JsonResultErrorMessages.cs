namespace VTSPlugin.WPF;

internal static class JsonResultErrorMessages
{
    public static class MissingDeserializer
    {
        public static readonly string Message = "Missing deserializer!";
        public static readonly ReadOnlyMemory<byte> Memory = VTSAPI.Encoding.GetBytes(Message);
    }
    public static class DeserializationFailed
    {
        public static readonly string Message = "Deserialization failed! Cause was likely logged.";
        public static readonly ReadOnlyMemory<byte> Memory = VTSAPI.Encoding.GetBytes(Message);
    }
    public static class DeserializationException
    {
        public static readonly string Message = "Deserialization exception! Exception was likely logged.";
        public static readonly ReadOnlyMemory<byte> Memory = VTSAPI.Encoding.GetBytes(Message);
    }
}
