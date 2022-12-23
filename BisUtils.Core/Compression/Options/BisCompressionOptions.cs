using BisUtils.Core.Options;

namespace BisUtils.Core.Compression.Options; 

public class BisDecompressionOptions : BisOptions{
    public int ExpectedSize { get; set; } 
}

public abstract class BisCompressionOptions : BisOptions{
}