using BisUtils.Core.Compression.Externals;

namespace BisUtils.Core.Compression; 

public class BisMiniLZOCompressionAlgorithms : IBisCompressionAlgorithm, IBisDecompressionAlgorithm {
    public long Compress(MemoryStream input, BinaryWriter output) {
        var startPos = output.BaseStream.Position;
        output.Write(MiniLZO.Compress(input.ToArray()));
        return output.BaseStream.Position - startPos;
    }

    public long Decompress(MemoryStream input, BinaryWriter output, int expectedSize) {
        var decompressed = new byte[expectedSize];
        MiniLZO.Decompress(input.ToArray(), decompressed);
        output.Write(decompressed);
        return decompressed.Length;
    }
}