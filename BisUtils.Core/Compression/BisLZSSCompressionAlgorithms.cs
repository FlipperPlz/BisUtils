using BisUtils.Core.Compression.Externals;
using BisUtils.Core.Compression.Options;

namespace BisUtils.Core.Compression;

public class BisLZSSCompressionAlgorithms : IBisCompressionAlgorithm<BisLZSSCompressionOptions>, 
    IBisDecompressionAlgorithm<BisLZSSDecompressionOptions> {

    public long Compress(byte[] input, BinaryWriter output, BisLZSSCompressionOptions options) {
        var startPos = output.BaseStream.Position;
        
        if (!options.AlwaysCompress && input.Length < 1024) {
            output.Write(input);
            return output.BaseStream.Position - startPos;
        }

        var lzssHelper = new BisCompatibleLZSS();
        lzssHelper.Encode(input, output);

        if(options.WriteSignedChecksum) WriteCRC(input, output);

        return output.BaseStream.Position - startPos;
    }

    public long Decompress(Stream input, BinaryWriter output, BisLZSSDecompressionOptions options) {
        if (!options.AlwaysDecompress && input.Length < 1024) {
            using var buffer = new MemoryStream();
            input.CopyTo(buffer);
            output.Write(buffer.GetBuffer(), 0, (int)input.Length);
            return options.ExpectedSize;
        }
        const int N = 4096;
        const int F = 18;
        const int THRESHOLD = 2;
        var text_buf = new byte[N + F - 1];
        var outputBytes = new byte[options.ExpectedSize];

        if (options.ExpectedSize <= 0) {
            output.Write(outputBytes);
            return options.ExpectedSize;
        }

        using var inputReader = new BinaryReader(input, System.Text.Encoding.UTF8, true);
        int i; 
        var flags = 0; 
        int cSum = 0, iDst = 0, bytesLeft = options.ExpectedSize;
        for (i = 0; i < N - F; i++) text_buf[i] = 0x20;
        var r = N - F;

        while (bytesLeft > 0) {
            int c;
            if (((flags >>= 1) & 256) == 0) {
                c = inputReader.ReadByte();
                flags = c | 0xff00;
            }

            if ((flags & 1) != 0) {
                c = inputReader.ReadByte();
                if (options.UseSignedChecksum)
                    cSum += (sbyte) c;
                else
                    cSum += (byte) c;

                // save byte
                outputBytes[iDst++] = (byte) c;
                bytesLeft--;
                // continue decompression
                text_buf[r] = (byte) c;
                r++;
                r &= N - 1;
            }
            else {
                i = inputReader.ReadByte();
                int j = inputReader.ReadByte();
                i |= (j & 0xf0) << 4;
                j &= 0x0f;
                j += THRESHOLD;

                int ii = r - i,
                    jj = j + ii;

                if (j + 1 > bytesLeft) {
                    throw new ArgumentException("LZSS overflow");
                }

                for (; ii <= jj; ii++) {
                    c = text_buf[ii & (N - 1)];
                    if (options.UseSignedChecksum)
                        cSum += (sbyte) c;
                    else
                        cSum += (byte) c;

                    // save byte
                    outputBytes[iDst++] = (byte) c;
                    bytesLeft--;
                    // continue decompression
                    text_buf[r] = (byte) c;
                    r++;
                    r &= N - 1;
                }
            }
        }

        var csData = new byte[4];
        inputReader.Read(csData, 0, 4);
        var csr = BitConverter.ToInt32(csData, 0);
        
        if (options.UseSignedChecksum && csr != cSum) throw new ArgumentException("Checksum mismatch");

        output.Write(outputBytes);
        return outputBytes.Length;
    }

    private static void WriteCRC(byte[] input, BinaryWriter output) => 
        output.Write(BitConverter.GetBytes(input.Aggregate<byte, uint>(0, (current, t) => current + t)));
}
