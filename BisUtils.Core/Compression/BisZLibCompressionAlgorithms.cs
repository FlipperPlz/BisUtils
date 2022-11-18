using System.IO.Compression;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression; 

public class BisZLibCompressionAlgorithms : IBisDecompressionAlgorithm<BisDecompressionOptions>, IBisCompressionAlgorithm<BisCompressionOptions> {
    public static long Decompress(MemoryStream input, BinaryWriter output, BisDecompressionOptions options) {
        var startPos = output.BaseStream.Position;
        using var inputStream = new MemoryStream(input.ToArray());
        var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress);
        
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

    public static long Compress(MemoryStream input, BinaryWriter output, BisCompressionOptions options) {
        throw new NotSupportedException();
    }
}