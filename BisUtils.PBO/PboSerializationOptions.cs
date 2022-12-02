using BisUtils.Core;

namespace BisUtils.PBO; 

public class PboBinarizationOptions : BisBinarizationOptions {
    public static readonly PboBinarizationOptions DefaultOptions = new PboBinarizationOptions();
    
    public static readonly PboBinarizationOptions DebugOptions = new PboBinarizationOptions() {
        UseCommonTimeStamp = 218762506
    };
    
    
    public bool RequireVersionEntry { get; set; } = true;
    public bool RequireDummyEntry { get; set; } = true;
    public bool StrictVersionEntry { get; set; } = true;

    public ulong? UseCommonTimeStamp { get; set; } = null;
    public bool WriteDataOffsets { get; set; } = false;
    
}

public class PboDebinarizationOptions : BisDebinarizationOptions {
    public static readonly PboDebinarizationOptions DefaultOptions = new PboDebinarizationOptions();

    public bool VerifyChecksum { get; set; } = true;
    public bool RequireVersionEntry { get; set; } = true;
    
    // Forces deserializer to look for a SINGLE version entry at (and only at) the beginning of the stream
    public bool StrictVersionEntry { get; set; } = true;
    
    //TODO: follow data offsets
    public bool UseEntryDataOffsets { get; set; } = true;


}