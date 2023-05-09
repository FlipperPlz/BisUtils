using BisUtils.Core.Compression.Externals;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public class BisMiniLZOCompressionAlgorithms : IBisCompressionAlgorithm<BisCompressionOptions>, IBisDecompressionAlgorithm<BisDecompressionOptions> {
    public long Compress(byte[] input, BinaryWriter output, BisCompressionOptions options) {
        var startPos = output.BaseStream.Position;
        output.Write(MiniLZO.Compress(input));
        return output.BaseStream.Position - startPos;
    }

    public long Decompress(Stream input, BinaryWriter output, BisDecompressionOptions options) {
        var decompressed = new byte[options.ExpectedSize];
        using var buffer = new MemoryStream();
        input.CopyTo(buffer);
        MiniLZO.Decompress(buffer.ToArray(), decompressed);
        output.Write(decompressed);
        return decompressed.Length;
    }
}
