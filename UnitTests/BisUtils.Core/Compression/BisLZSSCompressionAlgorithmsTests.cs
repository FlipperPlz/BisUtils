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
    public void CompressLessThan1024BytesIfAlwaysCompress()
    {
        var data = GetLowEntropyInput(512);
        var opts = new BisLZSSCompressionOptions()
        {
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
    public void DoNotCompressLessThan1024BytesIfAlwaysCompressFalse()
    {
        var data = GetLowEntropyInput(512);
        var opts = new BisLZSSCompressionOptions()
        {
            AlwaysCompress = false,
            WriteSignedChecksum = false
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);

        _compression.Compress(data, output, opts);
        var compressed = buffer.ToArray();

        Assert.Equal(data, compressed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CanDecompress(bool useSignedChecksum) {
        var data = GetLowEntropyInput(8192);
        using var compressionBuffer = new MemoryStream();
        using var compressionOutput = new BinaryWriter(compressionBuffer, Encoding.UTF8, true);
        var compressionOptions = new BisLZSSCompressionOptions() {
            AlwaysCompress = true,
            WriteSignedChecksum = useSignedChecksum
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);
        var decompressionOptions = new BisLZSSDecompressionOptions() {
            AlwaysDecompress = true,
            UseSignedChecksum = useSignedChecksum,
            ExpectedSize = data.Length
        };

        _compression.Compress(data, compressionOutput, compressionOptions);
        compressionBuffer.Position = 0;
        var readLength = _compression.Decompress(compressionBuffer, output, decompressionOptions);
        var result = buffer.ToArray();

        Assert.Equal(data.Length, readLength);
        Assert.Equal(data, result);
    }

    [Fact]
    public void CanDecodeUncompressedWhenAlwaysDecompressFalse()
    {
        var data = GetLowEntropyInput(512);
        var compressionBuffer = new MemoryStream(data);
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);
        var decompressionOptions = new BisLZSSDecompressionOptions()
        {
            AlwaysDecompress = false,
            UseSignedChecksum = false,
            ExpectedSize = data.Length
        };

        _compression.Decompress(compressionBuffer, output, decompressionOptions);
        var result = buffer.ToArray();

        Assert.Equal(data, result);
    }

    [Theory]
    [InlineData(2048, 1024)]
    [InlineData(1024, 512)]
    public void ThrowsIfLengthMismatch(int actualLength, int expectedLength)
    {
        var data = GetLowEntropyInput(actualLength);
        using var compressionBuffer = new MemoryStream();
        using var compressionOutput = new BinaryWriter(compressionBuffer, Encoding.UTF8, true);
        var compressionOptions = new BisLZSSCompressionOptions()
        {
            AlwaysCompress = true,
            WriteSignedChecksum = false
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);
        var decompressionOptions = new BisLZSSDecompressionOptions()
        {
            AlwaysDecompress = true,
            UseSignedChecksum = false,
            ExpectedSize = expectedLength
        };

        _compression.Compress(data, compressionOutput, compressionOptions);
        compressionBuffer.Position = 0;
        
        Assert.Throws<ArgumentException>(() => _compression.Decompress(compressionBuffer, output, decompressionOptions));
    }

    [Fact]
    public void ThrowsIfChecksumMismatch()
    {
        var data = GetLowEntropyInput(8192);
        using var compressionBuffer = new MemoryStream();
        using var compressionOutput = new BinaryWriter(compressionBuffer, Encoding.UTF8, true);
        var compressionOptions = new BisLZSSCompressionOptions()
        {
            AlwaysCompress = true,
            WriteSignedChecksum = false
        };
        using var buffer = new MemoryStream();
        using var output = new BinaryWriter(buffer, Encoding.UTF8, true);
        var decompressionOptions = new BisLZSSDecompressionOptions()
        {
            AlwaysDecompress = true,
            UseSignedChecksum = true,
            ExpectedSize = data.Length
        };

        _compression.Compress(data, compressionOutput, compressionOptions);
        compressionBuffer.Position = 0;

        Assert.Throws<ArgumentException>(() => _compression.Decompress(compressionBuffer, output, decompressionOptions));
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
