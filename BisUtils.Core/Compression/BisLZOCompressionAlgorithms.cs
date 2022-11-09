using BisUtils.Core.Compression.Externals;

namespace BisUtils.Core.Compression; 

public class BisLZOCompressionAlgorithms : IBisCompressionAlgorithm, IBisDecompressionAlgorithm{
    public long Compress(MemoryStream input, BinaryWriter output) => throw new NotSupportedException();

    public long Decompress(MemoryStream input, BinaryWriter output, int expectedSize) {
        var startPos = output.BaseStream.Position;
        output.Write(BFF_LZO.ReadLZO(input, (uint) expectedSize));
        return output.BaseStream.Position - startPos;
    }
}