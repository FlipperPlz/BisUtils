using BisUtils.Core.Compression.Externals;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public class BisMiniLZOCompressionAlgorithms : IBisCompressionAlgorithm<BisCompressionOptions>, IBisDecompressionAlgorithm<BisDecompressionOptions> {
    public long Compress(MemoryStream input, BinaryWriter output, BisCompressionOptions options) {
        var startPos = output.BaseStream.Position;
        output.Write(MiniLZO.Compress(input.ToArray()));
        return output.BaseStream.Position - startPos;
    }

    public long Decompress(MemoryStream input, BinaryWriter output, BisDecompressionOptions options) {
        var decompressed = new byte[options.ExpectedSize];
        MiniLZO.Decompress(input.ToArray(), decompressed);
        output.Write(decompressed);
        return decompressed.Length;
    }
}