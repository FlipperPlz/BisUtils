
// ReSharper disable once CheckNamespace

using System.Text;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;

// ReSharper disable once CheckNamespace
namespace System.IO 
{
    public static class BinaryReaderExtensions 
    {
        public static uint ReadUInt24(this BinaryReader reader) => (uint)(reader.ReadByte() + (reader.ReadByte() << 8) + (reader.ReadByte() << 16));
        
        public static int ReadCompactInteger(this BinaryReader reader) 
        {
            var value = 0;
            for (var i = 0;; ++i) 
            {
                var v = reader.ReadByte();
                value |= v & 0x7F << (7 * i);
                if((v & 0x80) == 0) 
                    break;
            }

            return value;
        }
        
        public static string ReadAsciiZ(this BinaryReader reader) 
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != '\0') bytes.Add(b);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static MemoryStream ReadCompressedData<T>(this BinaryReader reader, BisDecompressionOptions options) {
            var decompressedDataStream = new MemoryStream();
            var decompressedDataWriter = new BinaryWriter(decompressedDataStream, Encoding.UTF8);

            typeof(T).GetMethod("Decompress")!.Invoke(null, new object[] {
                new MemoryStream(reader.ReadBytes(options.ExpectedSize)),
                decompressedDataWriter,
                options
            });

            return decompressedDataStream;
        }

    }
}

