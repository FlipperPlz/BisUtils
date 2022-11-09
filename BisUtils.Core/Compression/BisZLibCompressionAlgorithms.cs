using System.IO.Compression;

namespace BisUtils.Core.Compression; 

public class BisZLibCompressionAlgorithms : IBisDecompressionAlgorithm, IBisCompressionAlgorithm {
    public long Decompress(MemoryStream input, BinaryWriter output, int expectedSize) {
        var startPos = output.BaseStream.Position;
        using var inputStream = new MemoryStream(input.ToArray());
        var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress);
        
        for ( var bytesRead = 0; bytesRead < expectedSize; ) {
            var toRead = 1000; // 1000 byte chunks
            if ( bytesRead + toRead > expectedSize ) toRead = expectedSize - bytesRead;
            var buffer = new byte[toRead];
            deflateStream.Read(buffer, 0, toRead);
            output.Write(buffer, 0, toRead);
            bytesRead += toRead;
        }

        return output.BaseStream.Position - startPos;
    }

    public long Compress(MemoryStream input, BinaryWriter output) {
        throw new NotSupportedException();
    }
}