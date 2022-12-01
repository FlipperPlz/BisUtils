namespace BisUtils.Core.Compression.Options; 

public abstract class BisDecompressionOptions : BisOptions{
    public int ExpectedSize { get; set; } 
}

public abstract class BisCompressionOptions : BisOptions{
}