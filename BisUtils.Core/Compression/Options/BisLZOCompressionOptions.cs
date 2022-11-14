namespace BisUtils.Core.Compression.Options; 

public class BisLZOCompressionOptions : BisCompressionOptions {
    public bool WriteCompressionFlag { get; set; } = false;
    public bool ForceCompression { get; set; } = false;
}

public class BisLZODecompressionOptions : BisDecompressionOptions {
    public bool UseCompressionFlag { get; set; } = false;
    public bool ForceDecompression { get; set; } = false;
}