using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;
using System.Text;

namespace UnitTests.BisUtils.Core.Compression;

public class BisLZSSCompressionAlgorithmsTests
{
    private readonly BisLZSSCompressionAlgorithms _compression = new();

    [Fact]
    public void CanCompress() {
        var data = GetLowEntropyInput(8192);
        var opts = new BisLZSSCompressionOptions() {
            AlwaysCompress = true,
            WriteSignedChecksum = false
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);

        _compression.Compress(data, output, opts);
        var compressed = buffer.ToArray();

        Assert.True(compressed.Length < data.Length);
    }

    [Fact]
    public void CanDecompress() {
        var data = GetLowEntropyInput(8192);
        using var compressionBuffer = new MemoryStream();
        using var compressionOutput = new BinaryWriter(compressionBuffer, Encoding.UTF8, true);
        var compressionOptions = new BisLZSSCompressionOptions() {
            AlwaysCompress = true,
            WriteSignedChecksum = false
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);
        var decompressionOptions = new BisLZSSDecompressionOptions() {
            AlwaysDecompress = true,
            UseSignedChecksum = false,
            ExpectedSize = data.Length
        };

        _compression.Compress(data, compressionOutput, compressionOptions);
        compressionBuffer.Position = 0;
        var readLength = _compression.Decompress(compressionBuffer, output, decompressionOptions);
        var result = buffer.ToArray();

        Assert.Equal(data.Length, readLength);
        Assert.Equal(data, result);
    }

    private static byte[] GetLowEntropyInput(int length) {
        var result = new byte[length];
        var rng = new Random();
        for (var i = 0; i < result.Length; i++) {
            result[i] = (byte)rng.Next(0, 10);
        }
        return result;
    }
}
