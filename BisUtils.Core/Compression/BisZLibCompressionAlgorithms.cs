using System.IO.Compression;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public class BisZLibCompressionAlgorithms : IBisDecompressionAlgorithm<BisDecompressionOptions>, IBisCompressionAlgorithm<BisCompressionOptions> {
    public long Decompress(Stream input, BinaryWriter output, BisDecompressionOptions options) {
        var startPos = output.BaseStream.Position;
        using var deflateStream = new DeflateStream(input, CompressionMode.Decompress, true);
        
        for ( var bytesRead = 0; bytesRead < options.ExpectedSize; ) {
            var toRead = 1000; // 1000 byte chunks
            if ( bytesRead + toRead > options.ExpectedSize ) toRead = options.ExpectedSize - bytesRead;
            var buffer = new byte[toRead];
            deflateStream.Read(buffer, 0, toRead);
            output.Write(buffer, 0, toRead);
            bytesRead += toRead;
        }

        return output.BaseStream.Position - startPos;
    }

    public long Compress(byte[] input, BinaryWriter output, BisCompressionOptions options) => throw new NotImplementedException();
}
