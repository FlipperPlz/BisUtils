using BisUtils.Core;

namespace BisUtils.PBO; 

public class PboSerializationOptions : BisSerializationOptions {
    public static readonly PboSerializationOptions DefaultOptions = new PboSerializationOptions();
    
    public static readonly PboSerializationOptions DebugOptions = new PboSerializationOptions() {
        UseCommonTimeStamp = 218762506
    };
    
    
    public bool RequireVersionEntry { get; set; } = true;
    public bool RequireDummyEntry { get; set; } = true;
    public bool StrictVersionEntry { get; set; } = true;

    public ulong? UseCommonTimeStamp { get; set; } = null;
    public bool WriteDataOffsets { get; set; } = false;
    
}

public class PboDeserializationOptions : BisDeserializationOptions {
    public static readonly PboDeserializationOptions DefaultOptions = new PboDeserializationOptions();

    public bool VerifyChecksum { get; set; } = true;
    public bool RequireVersionEntry { get; set; } = true;
    
    // Forces deserializer to look for a SINGLE version entry at (and only at) the beginning of the stream
    public bool StrictVersionEntry { get; set; } = true;
    
    //TODO: follow data offsets
    public bool UseEntryDataOffsets { get; set; } = true;


}