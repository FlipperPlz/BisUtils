namespace BisUtils.EnfPack.Extensions;

using Core.Binarize.Options;
using Core.IO;

public static class EsPackReaderExtensions
{

    public static uint ReadUInt32BE(this BisBinaryReader reader)
    {
        var bigEndian = reader.ReadUInt32();
        if (BitConverter.IsLittleEndian)
        {
            bigEndian = ((bigEndian & 0x000000FFU) << 24) |
                        ((bigEndian & 0x0000FF00U) << 8) |
                        ((bigEndian & 0x00FF0000U) >> 8) |
                        ((bigEndian & 0xFF000000U) >> 24);
        }
        return bigEndian;
    }

    public static uint ScanUntil(this BisBinaryReader reader, IBinarizationOptions options, string magic)
    {
        var magicBytes = options.Charset.GetBytes(magic);
        var buffer = new Queue<byte>(magicBytes.Length);

        try
        {
            while(reader.BaseStream.Position < reader.BaseStream.Length)
            {
                buffer.Enqueue(reader.ReadByte());
                while (buffer.Count > magicBytes.Length)
                {
                    buffer.Dequeue();
                }

                if (buffer.SequenceEqual(magicBytes))
                {
                    return (uint)(reader.BaseStream.Position - magicBytes.Length);
                }
            }
        }
        catch (EndOfStreamException)
        {
            throw new ArgumentException($"Magic string '{magic}' not found in stream.");
        }
        throw new ArgumentException($"Magic string '{magic}' not found in stream.");
    }

}
