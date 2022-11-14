using BisUtils.Core.Compression.Externals;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public class BisLZOCompressionAlgorithms : IBisCompressionAlgorithm<BisCompressionOptions>, IBisDecompressionAlgorithm<BisDecompressionOptions> {
    public long Compress(MemoryStream input, BinaryWriter output, BisCompressionOptions options) 
        => throw new NotSupportedException();

    public long Decompress(MemoryStream input, BinaryWriter output, BisDecompressionOptions options) {
        var startPos = output.BaseStream.Position;
        output.Write(BFF_LZO.ReadLZO(input, (uint) options.ExpectedSize));
        return output.BaseStream.Position - startPos;
    }
}