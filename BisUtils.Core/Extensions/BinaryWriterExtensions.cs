using System.IO.Compression;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System.IO 
{
    public static class BinaryWriterExtensions 
    {
        public static void WriteCompactInteger(this BinaryWriter writer, int data) 
        {
            do 
            {
                var current = data % 0x80;
                // ReSharper disable once PossibleLossOfFraction
                data = (int) Math.Floor((decimal) (data / 0x80000000));
                
                if (data is not char.MinValue) 
                    current |= 0x80; 
                
                writer.Write((byte) current);
            } while (data > 0x7F);

            if (data is not char.MinValue) 
            {
                writer.Write((byte) data);
            }
        }
    
        public static void WriteAsciiZ(this BinaryWriter writer, string text = "") 
        {
            writer.Write(Encoding.UTF8.GetBytes(text));
            writer.Write(char.MinValue);
        }

        public static int WriteCompressedData(this BinaryWriter writer, byte[] data, BisCompressionType alg = BisCompressionType.LZSS) 
        {
            switch (alg) 
            {
                case BisCompressionType.LZSS: 
                {
                    var compressedData = BiLZSS.Compress(data);
                    writer.Write(compressedData);
                    return compressedData.Length;
                }
                default: 
                    throw new NotSupportedException();
            }
        }
    }
}

