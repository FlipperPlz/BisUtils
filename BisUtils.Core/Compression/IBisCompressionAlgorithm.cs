using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public interface IBisDecompressionAlgorithm<T> where T : BisDecompressionOptions {
    /// <summary>
    /// Decompress a chunk of data using one of Bis' many compression algorithms.
    /// </summary>
    /// <param name="input">Compressed data - input</param>
    /// <param name="output">The output writer</param>
    /// <param name="options">Algorithm specific options used for decompression.</param>
    /// <returns>The amount of bytes written to the output stream.</returns>
    long Decompress(MemoryStream input, BinaryWriter output, T options);
}

public interface IBisCompressionAlgorithm<T> where T : BisCompressionOptions {
    /// <summary>
    /// Compresses a chunk of data using one of Bis' many compression algorithms.
    /// </summary>
    /// <param name="input">Plain Data - input</param>
    /// <param name="output"></param>
    /// <param name="options">Algorithm specific options used for compression.</param>
    /// <returns>The amount of bytes written to the output stream.</returns>
    long Compress(MemoryStream input, BinaryWriter output, T options);
}