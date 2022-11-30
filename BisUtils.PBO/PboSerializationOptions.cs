using BisUtils.Core;

namespace BisUtils.PBO; 

public class PboSerializationOptions : BisSerializationOptions {
    public static readonly PboSerializationOptions DefaultOptions = new PboSerializationOptions();
    
    public bool RequireVersionEntry { get; set; } = true;
    public bool RequireDummyEntry { get; set; } = true;
}

public class PboDeserializationOptions : BisDeserializationOptions {
    public static readonly PboDeserializationOptions DefaultOptions = new PboDeserializationOptions();

    public bool VerifyChecksum { get; set; } = true;
    public bool RequireVersionEntry { get; set; } = true;
    
    // Forces deserializer to look for a SINGLE version entry at (and only at) the beginning of the stream
    public bool StrictVersionEntry { get; set; } = true;

}