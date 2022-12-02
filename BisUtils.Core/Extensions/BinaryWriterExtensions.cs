using System.IO.Compression;
using System.Text;
using BisUtils.Core;
using BisUtils.Core.Compression;
using BisUtils.Core.Compression.Options;

// ReSharper disable once CheckNamespace
namespace System.IO 
{
    public static class BinaryWriterExtensions 
    {
        
        public static void WriteUInt24(this BinaryWriter writer, uint length)
        {
            writer.Write((byte)(length & 0xFF));
            writer.Write((byte)((length >> 8) & 0xFF));
            writer.Write((byte)((length >> 16) & 0xFF));
        }
        
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
        
        public static void WriteBinarized<T>(this BinaryWriter writer, T binarizable) where T : IBisBinarizable, new() => binarizable.WriteBinary(writer);

        
        
        // I wish I didn't have to use reflection for this but alas,
        //    .NET doesn't allow you to access static methods on generic types.
        public static long WriteCompressedData<T>(this BinaryWriter writer, byte[] data,
            BisCompressionOptions options) {
            return (long) typeof(T).GetMethod("Compress")!.Invoke(null, new object[] {
                new MemoryStream(data),
                writer,
                options
            })!;
        }
            
    }
}

