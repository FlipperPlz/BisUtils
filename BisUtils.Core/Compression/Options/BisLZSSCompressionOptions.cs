namespace BisUtils.Core.Compression.Options; 

public class BisLZSSDecompressionOptions : BisDecompressionOptions {
    public bool UseSignedChecksum { get; set; } = true;
    public bool AlwaysDecompress { get; set; } = false;
}
public class BisLZSSCompressionOptions : BisCompressionOptions {
    public bool WriteSignedChecksum { get; set; } = true;
    public bool AlwaysCompress { get; set; } = false;
}