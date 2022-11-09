namespace BisUtils.Core.Compression; 

public interface IBisDecompressionAlgorithm {
    /// <summary>
    /// Decompress a chunk of data using one of Bis' many compression algorithms.
    /// </summary>
    /// <param name="input">Compressed data - input</param>
    /// <param name="output">The output writer</param>
    /// <param name="expectedSize">The expected size of the decompressed data.</param>
    /// <returns>The amount of bytes written to the output stream.</returns>
    long Decompress(MemoryStream input, BinaryWriter output, int expectedSize);
}

public interface IBisCompressionAlgorithm {
    /// <summary>
    /// Compresses a chunk of data using one of Bis' many compression algorithms.
    /// </summary>
    /// <param name="input">Plain Data - input</param>
    /// <param name="output"></param>
    /// <returns>The amount of bytes written to the output stream.</returns>
    long Compress(MemoryStream input, BinaryWriter output);
}