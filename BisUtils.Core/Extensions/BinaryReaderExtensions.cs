
// ReSharper disable once CheckNamespace

using System.Text;
using BisUtils.Core.Compression;

// ReSharper disable once CheckNamespace
namespace System.IO 
{
    public static class BinaryReaderExtensions 
    {
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

        public static MemoryStream ReadCompressedData<T>(this BinaryReader reader, int expectedSize)
            where T : IBisDecompressionAlgorithm {
            var decompressedDataStream = new MemoryStream();
            var decompressedDataWriter = new BinaryWriter(decompressedDataStream, Encoding.UTF8);

            typeof(T).GetMethod("Decompress")!.Invoke(null, new object[] {
                new MemoryStream(reader.ReadBytes(expectedSize)),
                decompressedDataWriter,
                expectedSize
            });

            return decompressedDataStream;
        }

    }
}

